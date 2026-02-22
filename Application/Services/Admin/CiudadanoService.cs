using AutoMapper;
using eVote360.Core.Application.Dtos.Admin.Ciudadano;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Admin
{
    public sealed class CiudadanoService : ICiudadanoService
    {
        private readonly ICiudadanoRepository _repo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public CiudadanoService(ICiudadanoRepository repo, IEleccionRepository eleccionRepo, IMapper mapper)
        {
            _repo = repo; _eleccionRepo = eleccionRepo; _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct);

        public async Task<CiudadanoDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var ent = await _repo.GetByIdAsync(id, ct);
            return ent is null ? null : _mapper.Map<CiudadanoDto>(ent);
        }

        public async Task<CiudadanoDto?> GetByDocumentoAsync(string documento, CancellationToken ct)
        {
            var found = (await _repo.FindAsync(c => c.DocumentoIdentidad == documento, ct)).FirstOrDefault();
            return found is null ? null : _mapper.Map<CiudadanoDto>(found);
        }

        public async Task<List<CiudadanoDto>> GetAllAsync(CancellationToken ct)
        {
            var all = await _repo.GetAllListAsync(ct);
            return all.Select(_mapper.Map<CiudadanoDto>).ToList();
        }

        public async Task<int> CreateAsync(CiudadanoCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear ciudadano durante elección activa.");

            if (await _repo.AnyAsync(c => c.DocumentoIdentidad == dto.DocumentoIdentidad, ct))
                throw new InvalidOperationException("El número de documento ya existe.");

            var ent = _mapper.Map<Ciudadano>(dto);
            ent.IsActive = true;

            var saved = await _repo.AddAsync(ent, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear el ciudadano.");
            return saved.Id;
        }

        public async Task<bool> UpdateAsync(CiudadanoUpdateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede editar ciudadano durante elección activa.");

            var current = await _repo.GetByIdAsync(dto.Id, ct);
            if (current is null) return false;

            if (await _repo.AnyAsync(c => c.Id != dto.Id && c.DocumentoIdentidad == dto.DocumentoIdentidad, ct))
                throw new InvalidOperationException("Otro ciudadano ya tiene ese documento.");

            var newValues = _mapper.Map<Ciudadano>(dto);
            newValues.IsActive = current.IsActive;

            var updated = await _repo.UpdateAsync(dto.Id, newValues, ct);
            return updated is not null;
        }

        public async Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede cambiar estado del ciudadano durante elección activa.");

            var current = await _repo.GetByIdAsync(id, ct);
            if (current is null) return false;

            current.IsActive = active;
            var updated = await _repo.UpdateAsync(id, current, ct);
            return updated is not null;
        }
    }
}
