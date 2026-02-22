using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;
using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Application.Dtos.Votacion;

namespace eVote360.Core.Application.Interface.Votacion
{
    public interface IElectorService
    {
       
        Task<(bool PuedeVotar, string Mensaje, int? CiudadanoId)> PreCheckAsync(string documentoIdentidad, CancellationToken ct);

        
        Task<bool> ValidarIdentidadConOcrAsync(string documentoDigitado, Stream fotoCedulaFrontal, CancellationToken ct);

       
        Task<List<PuestoElectivoDto>> ListarPuestosDeEleccionActivaAsync(CancellationToken ct);
        Task<List<EleccionCandidaturaDto>> ListarCandidaturasPorPuestoEnActivaAsync(int puestoElectivoId, CancellationToken ct);

       
        Task<bool> EmitirVotoAsync(VotoCreateDto dto, CancellationToken ct); 
        Task<bool> FinalizarVotacionAsync(int ciudadanoId, CancellationToken ct); // envía correo resumen
    }
}
