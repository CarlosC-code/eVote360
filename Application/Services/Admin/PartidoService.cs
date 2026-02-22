using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.Partido;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class PartidoService : IPartidoService
    {
        private readonly IPartidoRepository _repo;
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly ICandidaturaRepository _candidaturaRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public PartidoService(
            IPartidoRepository repo,
            ICandidatoRepository candidatoRepo,
            ICandidaturaRepository candidaturaRepo,
            IEleccionRepository eleccionRepo,
            IMapper mapper)
        {
            _repo = repo;
            _candidatoRepo = candidatoRepo;
            _candidaturaRepo = candidaturaRepo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<PartidoDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var ent = await _repo.GetByIdAsync(id, ct);
            return ent is null ? null : _mapper.Map<PartidoDto>(ent);
        }

        public async Task<List<PartidoDto>> GetAllAsync(CancellationToken ct)
        {
            var all = await _repo.GetAllListAsync(ct);
            return all.Select(_mapper.Map<PartidoDto>).ToList();
        }

        public async Task<int> CreateAsync(PartidoCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear partido con elección activa.");

            if (await _repo.AnyAsync(p => p.Siglas == dto.Siglas, ct))
                throw new InvalidOperationException("Las siglas ya existen.");

            var ent = _mapper.Map<Partido>(dto);
            ent.IsActive = true;

            var saved = await _repo.AddAsync(ent, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear el partido.");
            return saved.Id;
        }

        public async Task<bool> UpdateAsync(PartidoUpdateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede editar partido con elección activa.");

            var current = await _repo.GetByIdAsync(dto.Id, ct);
            if (current is null) return false;

            if (await _repo.AnyAsync(p => p.Id != dto.Id && p.Siglas == dto.Siglas, ct))
                throw new InvalidOperationException("Ya existe otro partido con esas siglas.");

            var newValues = _mapper.Map<Partido>(dto);
            newValues.IsActive = current.IsActive;

            return await _repo.UpdateAsync(dto.Id, newValues, ct) is not null;
        }

        public async Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede cambiar estado del partido con elección activa.");

            var current = await _repo.GetByIdAsync(id, ct);
            if (current is null) return false;

            current.IsActive = active;
            var updated = await _repo.UpdateAsync(id, current, ct);
            if (updated is null) return false;

            if (!active)
            {
                var candidatos = await _candidatoRepo.FindAsync(c => c.PartidoId == id && c.IsActive, ct);
                foreach (var c in candidatos)
                {
                    c.IsActive = false;
                    await _candidatoRepo.UpdateAsync(c.Id, c, ct);
                }

                var candidaturas = await _candidaturaRepo.FindAsync(ca => ca.PartidoId == id && ca.IsActive, ct);
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

