using eVote360.Core.Application.Dtos.Elecciones;

namespace eVote360.Core.Application.Interface.Votacion
{
    public interface IResultadosService
    {
        Task<List<ResultadoPuestoDto>> ObtenerResultadosPorEleccionAsync(int eleccionId, CancellationToken ct);


    }
}