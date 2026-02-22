using AutoMapper;
using eVote360.Core.Application.Dtos.Dirigente.Candidato;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Interfaces;

namespace eVote360.Core.Application.Services.Dirigente
{
    public sealed class CandidatoService : ICandidatoService
    {
        private readonly ICandidatoRepository _candidatoRepo;
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IMapper _mapper;

        public CandidatoService(ICandidatoRepository candidatoRepo, IEleccionRepository eleccionRepo, IMapper mapper)
        {
            _candidatoRepo = candidatoRepo;
            _eleccionRepo = eleccionRepo;
            _mapper = mapper;
        }

        private Task<bool> HayEleccionActiva(CancellationToken ct)
            => _eleccionRepo.AnyAsync(e => e.Estado == ElectionStatus.EnProceso, ct); 

        public async Task<List<CandidatoDto>> ListarPorPartidoAsync(int partidoActualId, CancellationToken ct)
        {
            var list = await _candidatoRepo.FindAsync(c => c.PartidoId == partidoActualId, ct);
            return list.Select(_mapper.Map<CandidatoDto>).ToList();
        }

        public async Task<CandidatoDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var ent = await _candidatoRepo.GetByIdAsync(id, ct);
            return ent is null ? null : _mapper.Map<CandidatoDto>(ent);
        }

        public async Task<int> CreateAsync(int partidoActualId, CandidatoCreateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede crear candidato durante elección activa."); 

            var ent = _mapper.Map<Candidato>(dto);
            ent.PartidoId = partidoActualId; 
            ent.IsActive = true;

            var saved = await _candidatoRepo.AddAsync(ent, ct);
            if (saved is null) throw new InvalidOperationException("No se pudo crear el candidato.");
            return saved.Id;
        }

        public async Task<bool> UpdateAsync(CandidatoUpdateDto dto, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede editar candidato durante elección activa."); 

            var current = await _candidatoRepo.GetByIdAsync(dto.Id, ct);
            if (current is null) return false;

          
            current.Nombre = dto.Nombre;
            current.Apellido = dto.Apellido;
            if (!string.IsNullOrWhiteSpace(dto.FotoPath)) current.FotoPath = dto.FotoPath;

            return await _candidatoRepo.UpdateAsync(current.Id, current, ct) is not null;
        }

        public async Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct)
        {
            if (await HayEleccionActiva(ct))
                throw new InvalidOperationException("No se puede cambiar estado del candidato durante elección activa."); 

            var current = await _candidatoRepo.GetByIdAsync(id, ct);
            if (current is null) return false;

            current.IsActive = active;
            return await _candidatoRepo.UpdateAsync(id, current, ct) is not null;
        }
    }
}
