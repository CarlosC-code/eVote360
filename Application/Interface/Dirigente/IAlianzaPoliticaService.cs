
using eVote360.Core.Application.Dtos.Dirigente.Alianza;

namespace eVote360.Core.Application.Interface.Dirigente
{
    public interface IAlianzaPoliticaService
    {
        
        Task<List<AlianzaSolicitudDto>> ListarPendientesRecibidasAsync(int partidoActualId, CancellationToken ct);
        Task<List<AlianzaSolicitudDto>> ListarSolicitudesEnviadasAsync(int partidoActualId, CancellationToken ct);
        Task<List<AlianzaDto>> ListarAlianzasActivasAsync(int partidoActualId, CancellationToken ct);

        
        Task<int> CrearSolicitudAsync(int partidoSolicitanteId, AlianzaSolicitudCreateDto dto, CancellationToken ct);
        Task<bool> AceptarSolicitudAsync(int solicitudId, int partidoDestinoId, CancellationToken ct);
        Task<bool> RechazarSolicitudAsync(int solicitudId, int partidoDestinoId, CancellationToken ct);
        Task<bool> EliminarSolicitudAsync(int solicitudId, int partidoSolicitanteId, CancellationToken ct);
    }
}
