
using eVote360.Core.Application.Interface;
using eVote360.Core.Application.Interface.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eVote360.Controllers
{
    public class DirigenteHomeController : Controller
    {
        private readonly IUserSession _userSession;
        private readonly IUsuarioService _usuarioService;

        public DirigenteHomeController(IUserSession userSession, IUsuarioService usuarioService)
        {
            _userSession = userSession;
            _usuarioService = usuarioService;
        }

        public async Task<IActionResult> Index()
        {
            // Validar sesion
            if (!_userSession.HasUser()) return RedirectToRoute(new { controller = "Login", action = "Index" });

            var user = _userSession.GetUserSession();
            if (user is null) return RedirectToRoute(new { controller = "Login", action = "Index" });

            // Validar que sea Dirigente y que tenga partido asignado
            if (user.Rol != eVote360.Core.Domain.Common.Enums.UserRole.Dirigente)
                return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });

            // Servicio que te diga el partido asignado al dirigente (implementacion abajo)
            var partido = await _usuarioService.GetPartidoAsignadoAsync(user.Id);
            if (partido == null)
            {
                TempData["LoginError"] = "No tiene un partido político asignado, contacte un administrador.";
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            
            return View(/* dirigenteHomeVm */);
        }
    }
}
