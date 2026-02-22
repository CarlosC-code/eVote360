
using AutoMapper;
using eVote360.Core.Application.Dtos.Dirigente.Candidatura;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Dirigente
{
    public sealed class AsignacionCandidatoPuestoService : IAsignacionCandidatoPuestoService
    {
        private readonly ICandidaturaRepository _candidaturaRepo;
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly IPartidoRepository _partidoRepo;
        private readonly IAlianzaRepository _alianzaRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public AsignacionCandidatoPuestoService(
            ICandidaturaRepository candidaturaRepo,
            ICandidatoRepository candidatoRepo,
            IPartidoRepository partidoRepo,
            IAlianzaRepository alianzaRepo,
            IEleccionRepository eleccionRepo,
            IMapper mapper)
        {
            _candidaturaRepo = candidaturaRepo;
            _candidatoRepo = candidatoRepo;
            _partidoRepo = partidoRepo;
            _alianzaRepo = alianzaRepo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<List<CandidaturaDto>> ListarAsync(int partidoActualId, CancellationToken ct)
        {
            var list = await _candidaturaRepo.FindAsync(ca => ca.PartidoId == partidoActualId, ct);
            return list.Select(_mapper.Map<CandidaturaDto>).ToList();
        }

        public async Task<int> AsignarAsync(int partidoActualId, CandidaturaCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede asignar candidatos con una elección activa."); 

            
            var candidato = await _candidatoRepo.GetByIdAsync(dto.CandidatoId, ct) ?? throw new InvalidOperationException("Candidato no existe.");
            var partidoDestino = await _partidoRepo.GetByIdAsync(partidoActualId, ct) ?? throw new InvalidOperationException("Partido destino no existe.");
            if (!partidoDestino.IsActive) throw new InvalidOperationException("Partido destino inactivo.");

            // 1) No duplicar puesto en el mismo partido
            if (await _candidaturaRepo.AnyAsync(ca => ca.PartidoId == partidoActualId && ca.PuestoElectivoId == dto.PuestoElectivoId, ct))
                throw new InvalidOperationException("Ya existe un candidato asignado a ese puesto en este partido."); 

            // 2) En el mismo partido, un candidato no puede aspirar a mss de un puesto
            if (candidato.PartidoId == partidoActualId)
            {
                if (await _candidaturaRepo.AnyAsync(ca => ca.PartidoId == partidoActualId && ca.CandidatoId == candidato.Id, ct))
                    throw new InvalidOperationException("Este candidato ya está asignado a un puesto dentro del partido."); 
            }
            else
            {
                
                var hayAlianzaActiva = await _alianzaRepo.AnyAsync(a =>
                    a.IsActive && ((a.PartidoAId == partidoActualId && a.PartidoBId == candidato.PartidoId)
                                || (a.PartidoBId == partidoActualId && a.PartidoAId == candidato.PartidoId)), ct);
                if (!hayAlianzaActiva)
                    throw new InvalidOperationException("No existe una alianza activa con el partido del candidato."); 

                if (candidato.PuestoElectivoOrigenId != dto.PuestoElectivoId)
                    throw new InvalidOperationException("Este candidato en su partido de origen aspira a un puesto diferente al seleccionado."); 
            }

            var entity = new Candidatura
            {
                Id = 0,
                CandidatoId = dto.CandidatoId,
                PartidoId = partidoActualId,
                PuestoElectivoId = dto.PuestoElectivoId,
                IsActive = true
            };

            var saved = await _candidaturaRepo.AddAsync(entity, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear la asignación.");
            return saved.Id;
        }

        public async Task<bool> EliminarAsync(int asignacionId, int partidoActualId, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede eliminar una asignación con elección activa."); 

            var current = await _candidaturaRepo.GetByIdAsync(asignacionId, ct);
            if (current is null || current.PartidoId != partidoActualId) return false;

            
            current.IsActive = false;
            return await _candidaturaRepo.UpdateAsync(current.Id, current, ct) is not null;
        }
    }
}
