using eVote360.Core.Application.ViewModels.Admin.Usuario;

namespace eVote360.Core.Application.Interface
{
    public interface IUserSession
    {
        UsuarioViewModel? GetUserSession();
        bool HasUser();

        bool IsAdmin();
    }
}