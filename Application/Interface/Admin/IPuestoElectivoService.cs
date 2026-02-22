
using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;

namespace eVote360.Core.Application.Interface.Admin
{
    public interface IPuestoElectivoService
    {
        Task<PuestoElectivoDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<PuestoElectivoDto>> GetAllAsync(CancellationToken ct);
        Task<int> CreateAsync(PuestoElectivoCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(PuestoElectivoUpdateDto dto, CancellationToken ct);
        Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct);
    }
}