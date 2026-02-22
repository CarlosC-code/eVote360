
using eVote360.Core.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly IUserSession _userSession;

        public AdminHomeController(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public IActionResult Index()
        {
            // Solo usuarios con sesion y rol Admin
            if (!_userSession.HasUser()) return RedirectToRoute(new { controller = "Login", action = "Index" });
            if (!_userSession.IsAdmin()) return RedirectToRoute(new { controller = "Login", action = "AccessDenied" });

            
            return View();
        }
    }
}
