using eVote360.Core.Application.Dtos.Admin.Usuario;

namespace eVote360.Core.Application.Interface.Admin
{
    public interface IUsuarioService
    {
        Task<UsuarioDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<UsuarioDto>> GetAllAsync(CancellationToken ct);
        Task<int> CreateAsync(UsuarioCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(UsuarioUpdateDto dto, CancellationToken ct);
        Task<bool> SetActiveAsync(int id, bool active, CancellationToken ct);
        Task<UsuarioDto?> LoginAsync(LoginDto dto);

        Task<int?> GetPartidoAsignadoAsync(int usuarioId, CancellationToken ct = default);

    }
}
