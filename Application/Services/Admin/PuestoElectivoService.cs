
using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class PuestoElectivoService : IPuestoElectivoService
    {
        private readonly IPuestoElectivoRepository _puestoRepo;
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly ICandidaturaRepository _candidaturaRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public PuestoElectivoService(
            IPuestoElectivoRepository puestoRepo,
            ICandidatoRepository candidatoRepo,
            ICandidaturaRepository candidaturaRepo,
            IEleccionRepository eleccionRepo,
            IMapper mapper)
        {
            _puestoRepo = puestoRepo;
            _candidatoRepo = candidatoRepo;
            _candidaturaRepo = candidaturaRepo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<PuestoElectivoDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var ent = await _puestoRepo.GetByIdAsync(id, ct);
            return ent is null ? null : _mapper.Map<PuestoElectivoDto>(ent);
        }

        public async Task<List<PuestoElectivoDto>> GetAllAsync(CancellationToken ct)
        {
            var list = await _puestoRepo.GetAllListAsync(ct);
            return list.Select(_mapper.Map<PuestoElectivoDto>).ToList();
        }

        public async Task<int> CreateAsync(PuestoElectivoCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear un puesto mientras exista una elección activa.");

            var ent = _mapper.Map<PuestoElectivo>(dto);
            ent.IsActive = true;

            var saved = await _puestoRepo.AddAsync(ent, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear el puesto.");
            return saved.Id;
        }

        public async Task<bool> UpdateAsync(PuestoElectivoUpdateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede editar un puesto mientras exista una elección activa.");

            var current = await _puestoRepo.GetByIdAsync(dto.Id, ct);
            if (current is null) return false;

            var newValues = _mapper.Map<PuestoElectivo>(dto);
            newValues.IsActive = current.IsActive;

            return await _puestoRepo.UpdateAsync(dto.Id, newValues, ct) is not null;
        }

        public async Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede cambiar el estado de un puesto mientras exista una elección activa.");

            var current = await _puestoRepo.GetByIdAsync(id, ct);
            if (current is null) return false;

            current.IsActive = active;
            var updated = await _puestoRepo.UpdateAsync(id, current, ct);
            if (updated is null) return false;

            if (!active)
            {
                // Desactivar candidatos de origen
                var candOrigen = await _candidatoRepo.FindAsync(c => c.PuestoElectivoOrigenId == id && c.IsActive, ct);
                foreach (var c in candOrigen)
                {
                    c.IsActive = false;
                    await _candidatoRepo.UpdateAsync(c.Id, c, ct);
                }

                // Desactivar candidaturas ligadas al puesto
                var candidaturas = await _candidaturaRepo.FindAsync(ca => ca.PuestoElectivoId == id && ca.IsActive, ct);
                foreach (var ca in candidaturas)
                {
                    ca.IsActive = false;
                    await _candidaturaRepo.UpdateAsync(ca.Id, ca, ct);
                }
            }

            return true;
        }
    }
}
