
namespace eVote360.Core.Application.Interface.Admin
{
    public interface IAsignacionDirigenteService
    {
        Task<int> AsignarAsync(int usuarioId, int partidoId, CancellationToken ct);
        Task<bool> EliminarAsync(int asignacionId, CancellationToken ct);
        Task<List<(int AsignacionId, int UsuarioId, string UsuarioNombre, int PartidoId, string PartidoSiglas)>> ListarAsync(CancellationToken ct);
    }
}
