using eVote360.Core.Application.Dtos.Admin.Usuario;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new UsuarioCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new UsuarioCreateDto
                {
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Email = vm.Email,
                    UserName = vm.UserName,
                    Password = vm.Password,
                    Rol = vm.Rol
                };

                await _service.CreateAsync(dto, ct);
                TempData["Success"] = "Usuario creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(id, ct);
            if (dto is null) return NotFound();

            var vm = new UsuarioEditViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                UserName = dto.UserName,
                Rol = dto.Rol
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsuarioEditViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new UsuarioUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Email = vm.Email,
                    UserName = vm.UserName,
                    Password = vm.Password, 
                    Rol = vm.Rol
                };

                var ok = await _service.UpdateAsync(dto, ct);
                if (!ok) return NotFound();

                TempData["Success"] = "Usuario actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id, bool active, CancellationToken ct)
        {
            try
            {
                var ok = await _service.SetActiveAsync(id, active, ct);
                if (!ok) return NotFound();

                TempData["Success"] = active ? "Usuario activado." : "Usuario desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
