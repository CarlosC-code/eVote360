using eVote360.Core.Application.Helpers;
using eVote360.Core.Application.Interface;
using eVote360.Core.Application.ViewModels.Admin.Usuario;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Middlewares
{
    public class UserSession : IUserSession
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSession(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasUser()
        {
            UsuarioViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UsuarioViewModel>("User");

            if (userViewModel == null)
            {
                return false;
            }

            return true;
        }

        public UsuarioViewModel? GetUserSession()
        {
            UsuarioViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UsuarioViewModel>("User");

            if (userViewModel == null)
            {
                return null;
            }

            return userViewModel;
        }

        public bool IsAdmin()
        {
            UsuarioViewModel? userViewModel = _httpContextAccessor.HttpContext?
                .Session.Get<UsuarioViewModel>("User");

            if (userViewModel == null)
            {
                return false;
            }

            return userViewModel.Rol == UserRole.Administrador;
        }
    }
}
