
using AutoMapper;
using eVote360.Core.Application.Dtos.Dirigente.Alianza;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;


namespace eVote360.Core.Application.Services.Dirigente
{
    public sealed class AlianzaPoliticaService : IAlianzaPoliticaService
    {
        private readonly IAlianzaSolicitudRepository _solRepo;
        private readonly IAlianzaRepository _alianzaRepo;
        private readonly IPartidoRepository _partidoRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public AlianzaPoliticaService(
            IAlianzaSolicitudRepository solRepo,
            IAlianzaRepository alianzaRepo,
            IPartidoRepository partidoRepo,
            IEleccionRepository eleccionRepo,
            IMapper mapper)
        {
            _solRepo = solRepo;
            _alianzaRepo = alianzaRepo;
            _partidoRepo = partidoRepo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<List<AlianzaSolicitudDto>> ListarPendientesRecibidasAsync(int partidoActualId, CancellationToken ct)
        {
            var list = await _solRepo.FindAsync(s => s.PartidoDestinoId == partidoActualId && s.Estado == AllianceRequestStatus.EnEspera, ct);
            return list.Select(_mapper.Map<AlianzaSolicitudDto>).ToList();
        }

        public async Task<List<AlianzaSolicitudDto>> ListarSolicitudesEnviadasAsync(int partidoActualId, CancellationToken ct)
        {
            var list = await _solRepo.FindAsync(s => s.PartidoSolicitanteId == partidoActualId, ct);
            return list.Select(_mapper.Map<AlianzaSolicitudDto>).ToList();
        }

        public async Task<List<AlianzaDto>> ListarAlianzasActivasAsync(int partidoActualId, CancellationToken ct)
        {
            var list = await _alianzaRepo.FindAsync(a =>
                (a.PartidoAId == partidoActualId || a.PartidoBId == partidoActualId) && a.IsActive, ct);
            return list.Select(_mapper.Map<AlianzaDto>).ToList();
        }

        public async Task<int> CrearSolicitudAsync(int partidoSolicitanteId, AlianzaSolicitudCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se pueden crear solicitudes durante elección activa."); 

            if (partidoSolicitanteId == dto.PartidoDestinoId)
                throw new InvalidOperationException("No puede solicitar alianza con su mismo partido.");

            // Ambos partidos deben existir y estar activos
            var pA = await _partidoRepo.GetByIdAsync(partidoSolicitanteId, ct) ?? throw new InvalidOperationException("Partido solicitante no existe.");
            var pB = await _partidoRepo.GetByIdAsync(dto.PartidoDestinoId, ct) ?? throw new InvalidOperationException("Partido destino no existe.");
            if (!pA.IsActive || !pB.IsActive) throw new InvalidOperationException("Ambos partidos deben estar activos.");

            // No permitir duplicadas ni cruzadas pendientes
            var yaPendiente = await _solRepo.AnyAsync(s =>
                (s.PartidoSolicitanteId == partidoSolicitanteId && s.PartidoDestinoId == dto.PartidoDestinoId && s.Estado == AllianceRequestStatus.EnEspera)
                || (s.PartidoSolicitanteId == dto.PartidoDestinoId && s.PartidoDestinoId == partidoSolicitanteId && s.Estado == AllianceRequestStatus.EnEspera), ct);
            if (yaPendiente)
                throw new InvalidOperationException("Ya existe una solicitud de alianza pendiente entre estos partidos."); 

            // Si ya existe una alianza activa entre ambos, no solicitar de nuevo
            var alianzaActiva = await _alianzaRepo.AnyAsync(a =>
                (a.PartidoAId == partidoSolicitanteId && a.PartidoBId == dto.PartidoDestinoId
                 || a.PartidoAId == dto.PartidoDestinoId && a.PartidoBId == partidoSolicitanteId) && a.IsActive, ct);
            if (alianzaActiva)
                throw new InvalidOperationException("Ya existe una alianza activa entre estos partidos.");

            var entity = _mapper.Map<AlianzaSolicitud>(dto);
            entity.PartidoSolicitanteId = partidoSolicitanteId;
            entity.Estado = AllianceRequestStatus.EnEspera;
            entity.IsActive = true;

            var saved = await _solRepo.AddAsync(entity, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear la solicitud.");
            return saved.Id;
        }

        public async Task<bool> AceptarSolicitudAsync(int solicitudId, int partidoDestinoId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se pueden aceptar solicitudes durante elección activa."); 

            var sol = await _solRepo.GetByIdAsync(solicitudId, ct);
            if (sol is null || sol.PartidoDestinoId != partidoDestinoId || sol.Estado != AllianceRequestStatus.EnEspera)
                throw new InvalidOperationException("Solicitud inválida.");

            sol.Estado = AllianceRequestStatus.Aceptada;
            var updated = await _solRepo.UpdateAsync(sol.Id, sol, ct);
            if (updated is null) return false;

            // Crear la alianza formal
            var alianza = new Alianza
            {
                Id = solicitudId,
                PartidoAId = sol.PartidoSolicitanteId,
                PartidoBId = sol.PartidoDestinoId,
                FechaAceptacionUtc = DateTime.UtcNow,
                IsActive = true
            };
            return await _alianzaRepo.AddAsync(alianza, ct) is not null;
        }

        public async Task<bool> RechazarSolicitudAsync(int solicitudId, int partidoDestinoId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se pueden rechazar solicitudes durante elección activa."); 

            var sol = await _solRepo.GetByIdAsync(solicitudId, ct);
            if (sol is null || sol.PartidoDestinoId != partidoDestinoId || sol.Estado != AllianceRequestStatus.EnEspera)
                throw new InvalidOperationException("Solicitud inválida.");

            sol.Estado = AllianceRequestStatus.Rechazada;
            return await _solRepo.UpdateAsync(sol.Id, sol, ct) is not null;
        }

        public async Task<bool> EliminarSolicitudAsync(int solicitudId, int partidoSolicitanteId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se pueden eliminar solicitudes durante elección activa."); 

            var sol = await _solRepo.GetByIdAsync(solicitudId, ct);
            if (sol is null || sol.PartidoSolicitanteId != partidoSolicitanteId)
                return false;

            return await _solRepo.DeleteAsync(solicitudId, ct);
        }
    }
}
