
using eVote360.Core.Application.Dtos.Dirigente.Candidato;

namespace eVote360.Core.Application.Interface.Dirigente
{
    public interface ICandidatoService
    {
        Task<List<CandidatoDto>> ListarPorPartidoAsync(int partidoActualId, CancellationToken ct);
        Task<CandidatoDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<int> CreateAsync(int partidoActualId, CandidatoCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(CandidatoUpdateDto dto, CancellationToken ct);
        Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct);
    }
}
