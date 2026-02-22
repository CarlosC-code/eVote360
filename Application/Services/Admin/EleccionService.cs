
using AutoMapper;
using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class EleccionService : IEleccionService
    {
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IPuestoElectivoRepository _puestoRepo;
        private readonly IPartidoRepository _partidoRepo;
        private readonly ICandidaturaRepository _candidaturaRepo;
        private readonly IEleccionPuestoRepository _eleccionPuestoRepo;
        private readonly IEleccionCandidaturaRepository _eleccionCandidaturaRepo;
        private readonly IVotoRepository _votoRepo;
        private readonly IMapper _mapper;

        public EleccionService(
            IEleccionRepository eleccionRepo,
            IPuestoElectivoRepository puestoRepo,
            IPartidoRepository partidoRepo,
            ICandidaturaRepository candidaturaRepo,
            IEleccionPuestoRepository eleccionPuestoRepo,
            IEleccionCandidaturaRepository eleccionCandidaturaRepo,
            IVotoRepository votoRepo,
            IMapper mapper)
        {
            _eleccionRepo = eleccionRepo;
            _puestoRepo = puestoRepo;
            _partidoRepo = partidoRepo;
            _candidaturaRepo = candidaturaRepo;
            _eleccionPuestoRepo = eleccionPuestoRepo;
            _eleccionCandidaturaRepo = eleccionCandidaturaRepo;
            _votoRepo = votoRepo;
            _mapper = mapper;
        }

        public Task<bool> ExisteEleccionActivaAsync(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<int> CrearNuevaAsync(EleccionCreateDto dto, CancellationToken ct)
        {
            // 1) No debe existir eleccion activa
            if (await ExisteEleccionActivaAsync(ct))
                throw new InvalidOperationException("Ya existe una elección activa.");

            // 2) 1 puesto activo
            var puestosActivos = await _puestoRepo.FindAsync(p => p.IsActive, ct);
            if (puestosActivos.Count == 0)
                throw new InvalidOperationException("Debe existir al menos un puesto electivo activo.");

            // 3) 2 partidos activos
            var partidosActivos = await _partidoRepo.FindAsync(p => p.IsActive, ct);
            if (partidosActivos.Count < 2)
                throw new InvalidOperationException("No hay suficientes partidos políticos para realizar una elección.");

            // 4) Cada partido activo debe tener candidaturas activas para todos los puestos activos
            var mensajesFaltantes = new List<string>();
            foreach (var partido in partidosActivos)
            {
                var puestosFaltantes = new List<string>();
                foreach (var puesto in puestosActivos)
                {
                    var existe = await _candidaturaRepo.AnyAsync(ca =>
                        ca.PartidoId == partido.Id &&
                        ca.PuestoElectivoId == puesto.Id &&
                        ca.IsActive &&
                        ca.Candidato.IsActive, ct);

                    if (!existe)
                        puestosFaltantes.Add(puesto.Nombre);
                }

                if (puestosFaltantes.Count > 0)
                {
                    mensajesFaltantes.Add(
                        $"El partido político {partido.Nombre}[{partido.Siglas}] no tiene candidatos registrados para: {string.Join(", ", puestosFaltantes)}.");
                }
            }

            if (mensajesFaltantes.Count > 0)
                throw new InvalidOperationException(string.Join(" ", mensajesFaltantes));

            // Crear elección
            var eleccion = _mapper.Map<Eleccion>(dto);
            eleccion.Estado = ElectionStatus.EnProceso;
            eleccion.IsActive = true;

            var savedElection = await _eleccionRepo.AddAsync(eleccion, ct);
            if (savedElection is null) throw new InvalidOperationException("No se pudo crear la elección.");

            // Snapshot de puestos activos
            foreach (var p in puestosActivos)
            {
                var ep = new EleccionPuesto
                {
                    Id = 0,
                    EleccionId = savedElection.Id,
                    PuestoElectivoId = p.Id,
                    IsActive = true
                };
                if (await _eleccionPuestoRepo.AddAsync(ep, ct) is null)
                    throw new InvalidOperationException("Error al guardar puestos de la elección.");
            }

            // Snapshot de candidaturas activas
            var candidaturasActivas = await _candidaturaRepo.FindAsync(ca => ca.IsActive && ca.Candidato.IsActive, ct);
            foreach (var ca in candidaturasActivas)
            {
                var ec = new EleccionCandidatura
                {
                    Id = 0,
                    EleccionId = savedElection.Id,
                    CandidaturaId = ca.Id,
                    IsActive = true
                };
                if (await _eleccionCandidaturaRepo.AddAsync(ec, ct) is null)
                    throw new InvalidOperationException("Error al guardar candidaturas de la elección.");
            }

            return savedElection.Id;
        }

        public async Task<bool> FinalizarActivaAsync(CancellationToken ct)
        {
            var activa = (await _eleccionRepo.FindAsync(e => e.Estado == ElectionStatus.EnProceso, ct)).FirstOrDefault();
            if (activa is null) return false;

            activa.Estado = ElectionStatus.Finalizada;
            return await _eleccionRepo.UpdateAsync(activa.Id, activa, ct) is not null;
        }

        public async Task<List<EleccionDto>> ListarAsync(CancellationToken ct)
        {
            var all = await _eleccionRepo.GetAllListAsync(ct);
            return all.OrderByDescending(e => e.Id).Select(_mapper.Map<EleccionDto>).ToList();
        }

        public async Task<List<EleccionResumenDto>> ResumenPorAnioAsync(int year, CancellationToken ct)
        {
            var elecciones = await _eleccionRepo.FindAsync(e => e.FechaRealizacion.Year == year, ct);
            var result = new List<EleccionResumenDto>();

            foreach (var e in elecciones)
            {
                var eleccionId = e.Id;

               
                var candidaturasAll = await _candidaturaRepo.GetAllListWithIncludeAsync(ct, ca => ca.EleccionCandidaturas!, ca => ca.Partido, ca => ca.Candidato);
                var candidaturasDeEleccion = candidaturasAll
                    .Where(ca => ca.EleccionCandidaturas != null && ca.EleccionCandidaturas.Any(x => x.EleccionId == eleccionId))
                    .ToList();

                var partidosParticipantes = candidaturasDeEleccion.Select(ca => ca.PartidoId).Distinct().Count();
                var candidatosParticipantes = candidaturasDeEleccion.Select(ca => ca.CandidatoId).Distinct().Count();

                var totalVotos = (await _votoRepo.FindAsync(v => v.EleccionId == eleccionId, ct)).Count;

                result.Add(new EleccionResumenDto
                {
                    EleccionId = eleccionId,
                    Nombre = e.Nombre,
                    CantidadPartidos = partidosParticipantes,
                    CantidadCandidatos = candidatosParticipantes,
                    TotalVotosEmitidos = totalVotos
                });
            }

            return result;
        }
    }
}
