using eVote360.Core.Application.Dtos.Admin.Partido;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.Partido;
using eVote360.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class PartidoController : Controller
    {
        private readonly IPartidoService _service;
        private readonly IWebHostEnvironment _env;

        public PartidoController(IPartidoService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new PartidoCreateViewModel());


        [HttpPost]
        public async Task<IActionResult> Create(PartidoCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            if (vm.Logo is null || vm.Logo.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.Logo), "El logo es obligatorio.");
                return View(vm);
            }

            try
            {
                // 1) Crear primero SIN logo para obtener el Id real
                var provisional = new PartidoCreateDto
                {
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Siglas = vm.Siglas,
                    LogoPath = string.Empty // temporal
                };

                var partidoId = await _service.CreateAsync(provisional, ct);

                // 2) Subir el logo ahora con el Id real 
                var logoPath = await FileManager.UploadAsync(
                    file: vm.Logo,
                    id: partidoId,
                    folderName: "partidos",
                    isEditMode: false,
                    imagePath: null,
                    ct: ct
                );

                // 3) Actualizar el partido con el LogoPath correcto
                var update = new PartidoUpdateDto
                {
                    Id = partidoId,
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Siglas = vm.Siglas,
                    LogoPath = logoPath
                };

                await _service.UpdateAsync(update, ct);

                TempData["Success"] = "Partido creado.";
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

            var vm = new PartidoEditViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Siglas = dto.Siglas,
                ExistingLogoPath = dto.LogoPath
            };
            return View(vm);
        }

        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> Edit(PartidoEditViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                // Si sube un nuevo logo, subelo y reemplaza; si no, conserva el existente
                if (vm.Logo != null && vm.Logo.Length > 0)
                {
                    var newLogoPath = await FileManager.UploadAsync(
                        file: vm.Logo,
                        id: vm.Id,                 
                        folderName: "partidos",
                        isEditMode: true,
                        imagePath: vm.ExistingLogoPath,   
                        ct: ct
                    );

                    vm.ExistingLogoPath = newLogoPath;
                }

                var dto = new PartidoUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Descripcion = vm.Descripcion,
                    Siglas = vm.Siglas,
                    LogoPath = vm.ExistingLogoPath 
                };

                var ok = await _service.UpdateAsync(dto, ct);
                if (!ok) return NotFound();

                TempData["Success"] = "Partido actualizado.";
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

                TempData["Success"] = active ? "Partido activado." : "Partido desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
