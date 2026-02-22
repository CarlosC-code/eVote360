
using eVote360.Core.Application.Dtos.Dirigente.Candidatura;

namespace eVote360.Core.Application.Interface.Dirigente
{
    public interface IAsignacionCandidatoPuestoService
    {
        Task<List<CandidaturaDto>> ListarAsync(int partidoActualId, CancellationToken ct);
        Task<int> AsignarAsync(int partidoActualId, CandidaturaCreateDto dto, CancellationToken ct);
        Task<bool> EliminarAsync(int asignacionId, int partidoActualId, CancellationToken ct); 
    }
}
