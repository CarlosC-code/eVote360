using eVote360.Core.Application.Dtos.Admin.PuestoElectivo;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.PuestoElectivo;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class PuestoElectivoController : Controller
    {
        private readonly IPuestoElectivoService _service;

        public PuestoElectivoController(IPuestoElectivoService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new PuestoElectivoCreateViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(PuestoElectivoCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new PuestoElectivoCreateDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion
                };

                await _service.CreateAsync(dto, ct);
                TempData["Success"] = "Puesto electivo creado.";
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

            var vm = new PuestoElectivoEditViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PuestoElectivoEditViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var dto = new PuestoElectivoUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion
                };

                var ok = await _service.UpdateAsync(dto, ct);
                if (!ok) return NotFound();

                TempData["Success"] = "Puesto electivo actualizado.";
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

                TempData["Success"] = active ? "Puesto activado." : "Puesto desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
