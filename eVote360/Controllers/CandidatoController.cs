
using System.Linq;
using System.Security.Claims;
using eVote360.Core.Application.Dtos.Dirigente.Candidato;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Application.ViewModels.Dirigente.Candidato;
using eVote360.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class CandidatoController : Controller
    {
        private readonly ICandidatoService _service;
        private readonly IWebHostEnvironment _env;

        public CandidatoController(ICandidatoService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

     
        private int? TryGetPartidoIdFromSession()
            => HttpContext?.Session?.GetInt32("PartidoId");

        private int? TryGetPartidoIdFromClaims()
        {
            // Busca claims comunes: "PartidoId", "partidoId", "partido"
            var claim = User?.Claims?.FirstOrDefault(c =>
                   string.Equals(c.Type, "PartidoId", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.Type, "partidoId", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.Type, "partido", StringComparison.OrdinalIgnoreCase));

            if (claim != null && int.TryParse(claim.Value, out var id) && id > 0)
                return id;

            return null;
        }

        private int? TryResolvePartidoIdFromContext()
            => TryGetPartidoIdFromSession() ?? TryGetPartidoIdFromClaims();

       
        private int ResolvePartidoIdOrThrow(int partidoId)
        {
            if (partidoId > 0) return partidoId;

            var resolved = TryResolvePartidoIdFromContext();
            if (resolved.HasValue && resolved.Value > 0) return resolved.Value;

            throw new InvalidOperationException("No se pudo resolver el Partido del usuario actual.");
        }

      
        public async Task<IActionResult> Index(int partidoId, CancellationToken ct)
        {
            // Si no viene, resuélvelo y redirige con el valor correcto
            if (partidoId <= 0)
            {
                var resolved = ResolvePartidoIdOrThrow(partidoId);
                return RedirectToAction(nameof(Index), new { partidoId = resolved });
            }

            ViewBag.PartidoId = partidoId;
            var list = await _service.ListarPorPartidoAsync(partidoId, ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create(int partidoId)
        {
            // Si no viene, resuélvelo y redirige con el valor correcto
            if (partidoId <= 0)
            {
                var resolved = ResolvePartidoIdOrThrow(partidoId);
                return RedirectToAction(nameof(Create), new { partidoId = resolved });
            }

            ViewBag.PartidoId = partidoId;
            return View(new CandidatoCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(int partidoId, CandidatoCreateViewModel vm, CancellationToken ct)
        {
            // Si vino 0 o no vino, resuélvelo aqui mismo
            if (partidoId <= 0)
            {
                try
                {
                    partidoId = ResolvePartidoIdOrThrow(partidoId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ViewBag.PartidoId = 0;
                    return View(vm);
                }
            }

            ViewBag.PartidoId = partidoId;
            if (!ModelState.IsValid) return View(vm);

            if (vm.FotoFile is null || vm.FotoFile.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.FotoFile), "La foto es obligatoria.");
                return View(vm);
            }

            try
            {
                var fotoPath = await FileManager.UploadAsync(
                    vm.FotoFile,
                    id: partidoId,
                    folderName: "candidatos",
                    isEditMode: false,
                    imagePath: null,
                    ct
                );

                var dto = new CandidatoCreateDto
                {
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    FotoPath = fotoPath
                };

                await _service.CreateAsync(partidoId, dto, ct);
                TempData["Success"] = "Candidato creado.";
                return RedirectToAction(nameof(Index), new { partidoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int partidoId, CancellationToken ct)
        {
            // Si no viene, resuelvelo y redirige con el valor correcto
            if (partidoId <= 0)
            {
                var resolved = ResolvePartidoIdOrThrow(partidoId);
                return RedirectToAction(nameof(Edit), new { id, partidoId = resolved });
            }

            ViewBag.PartidoId = partidoId;

            var dto = await _service.GetByIdAsync(id, ct);
            if (dto is null) return NotFound();

            return View(new CandidatoEditViewModel
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                ExistingFotoPath = dto.FotoPath
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int partidoId, CandidatoEditViewModel vm, CancellationToken ct)
        {
            if (partidoId <= 0)
            {
                try
                {
                    partidoId = ResolvePartidoIdOrThrow(partidoId);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    ViewBag.PartidoId = 0;
                    return View(vm);
                }
            }

            ViewBag.PartidoId = partidoId;
            if (!ModelState.IsValid) return View(vm);

            try
            {
                if (vm.Foto != null && vm.Foto.Length > 0)
                {
                    var fotoPath = await FileManager.UploadAsync(
                        file: vm.Foto,
                        id: partidoId,
                        folderName: "candidatos",
                        isEditMode: true,
                        imagePath: vm.ExistingFotoPath,
                        ct: ct
                    );

                    vm.ExistingFotoPath = fotoPath;
                }

                var dto = new CandidatoUpdateDto
                {
                    Id = vm.Id,
                    Nombre = vm.Nombre,
                    Apellido = vm.Apellido,
                    FotoPath = vm.ExistingFotoPath
                };

                var ok = await _service.UpdateAsync(dto, ct);
                if (!ok) return NotFound();

                TempData["Success"] = "Candidato actualizado.";
                return RedirectToAction(nameof(Index), new { partidoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int id, int partidoId, bool active, CancellationToken ct)
        {
            try
            {
                if (partidoId <= 0)
                {
                    partidoId = ResolvePartidoIdOrThrow(partidoId);
                }

                var ok = await _service.SetActiveAsync(id, active, ct);
                if (!ok) return NotFound();

                TempData["Success"] = active ? "Candidato activado." : "Candidato desactivado.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(Index), new { partidoId });
        }
    }
}
