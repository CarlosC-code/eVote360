
using eVote360.Core.Application.Dtos.Elecciones;

namespace eVote360.Core.Application.Interface.Admin
{
    public interface IEleccionService
    {
        Task<bool> ExisteEleccionActivaAsync(CancellationToken ct);
        Task<int> CrearNuevaAsync(EleccionCreateDto dto, CancellationToken ct);
        Task<bool> FinalizarActivaAsync(CancellationToken ct);
        Task<List<EleccionDto>> ListarAsync(CancellationToken ct);
        Task<List<EleccionResumenDto>> ResumenPorAnioAsync(int year, CancellationToken ct);
    }
}
