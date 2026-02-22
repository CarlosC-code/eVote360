using eVote360.Core.Application.Dtos.Admin.Ciudadano;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.Ciudadano;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class CiudadanoController : Controller
    {
        private readonly ICiudadanoService _service;

        public CiudadanoController(ICiudadanoService service) => _service = service;

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new CiudadanoCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CiudadanoCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            try
            {
                var dto = new CiudadanoCreateDto
                {
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Email = vm.Email,
                    DocumentoIdentidad = vm.DocumentoIdentidad
                };
                await _service.CreateAsync(dto, ct);
                TempData["Success"] = "Ciudadano creado.";
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

            return View(new CiudadanoEditViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                DocumentoIdentidad = dto.DocumentoIdentidad
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CiudadanoEditViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            try
            {
                var dto = new CiudadanoUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    Email = vm.Email,
                    DocumentoIdentidad = vm.DocumentoIdentidad
                };
                var ok = await _service.UpdateAsync(dto, ct);
                if (!ok) return NotFound();

                TempData["Success"] = "Ciudadano actualizado.";
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
                TempData["Success"] = active ? "Ciudadano activado." : "Ciudadano desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
