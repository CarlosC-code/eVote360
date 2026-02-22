
using eVote360.Core.Application.Dtos.Admin.Partido;

namespace eVote360.Core.Application.Interface.Admin
{
    public interface IPartidoService
    {
        Task<PartidoDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<PartidoDto>> GetAllAsync(CancellationToken ct);
        Task<int> CreateAsync(PartidoCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(PartidoUpdateDto dto, CancellationToken ct);
        Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct);
    }
}

