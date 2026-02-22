
using eVote360.Core.Application.Dtos.Admin.Usuario;
using eVote360.Core.Application.Helpers;
using eVote360.Core.Application.Interface;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.Usuario;
using eVote360.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 

namespace eVote360.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _userService;
        private readonly IUserSession _userSession;

        public LoginController(IUsuarioService userService, IUserSession userSession)
        {
            _userService = userService;
            _userSession = userSession;
        }

        public IActionResult Index()
        {
            if (_userSession.HasUser())
            {
                UsuarioViewModel? userSession = _userSession.GetUserSession();
                if (userSession != null)
                {
                    return userSession.Rol switch
                    {
                        UserRole.Administrador => RedirectToRoute(new { controller = "AdminHome", action = "Index" }),
                        UserRole.Dirigente => RedirectToRoute(new { controller = "DirigenteHome", action = "Index" }),
                        _ => RedirectToRoute(new { controller = "Login", action = "Index" }),
                    };
                }
            }

            return View(new LoginViewModel() { PasswordHash = "", UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.PasswordHash = "";
                return View(vm);
            }

            UsuarioDto? userDto = await _userService.LoginAsync(new LoginDto()
            {
                PasswordHash = vm.PasswordHash,
                UserName = vm.UserName
            });

            if (userDto == null)
            {
                ModelState.AddModelError("userValidation", "Data access is incorrect");
                vm.PasswordHash = "";
                return View(vm);
            }

            // VALIDAR PARTIDO ANTES DE GUARDAR SESION
            var partidoId = await _userService.GetPartidoAsignadoAsync(userDto.Id);

            if (userDto.Rol == UserRole.Dirigente && partidoId <= 0)
            {
                ModelState.AddModelError("", "El usuario no tiene partido asignado.");
                vm.PasswordHash = "";
                return View(vm);
            }

            
            UsuarioViewModel userVm = new()
            {
                Email = userDto.Email,
                Id = userDto.Id,
                Apellido = userDto.Apellido,
                Nombre = userDto.Nombre,
                Rol = userDto.Rol,
                UserName = userDto.UserName,
                IsActive = userDto.IsActive,
            };

            HttpContext.Session.Set<UsuarioViewModel>("User", userVm);

            if (partidoId > 0)
            {
                HttpContext.Session.SetInt32("PartidoId", (int)partidoId);
            }

            // REDIRECCION SEGUN ROL
            if (userVm.Rol == UserRole.Administrador)
                return RedirectToRoute(new { controller = "AdminHome", action = "Index" });

            if (userVm.Rol == UserRole.Dirigente)
                return RedirectToRoute(new { controller = "DirigenteHome", action = "Index" });

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public IActionResult AccessDenied()
        {
            if (_userSession.HasUser())
            {
                return View();
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
    }
}
