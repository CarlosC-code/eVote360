using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.ViewModels.Admin.Eleccion;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class EleccionController : Controller
    {
        private readonly IEleccionService _service;
        public EleccionController(IEleccionService service) => _service = service;

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var list = await _service.ListarAsync(ct);
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new EleccionCreateViewModel { FechaRealizacion = DateTime.Today });

        [HttpPost]
        public async Task<IActionResult> Create(EleccionCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            try
            {
                await _service.CrearNuevaAsync(new EleccionCreateDto
                {
                    Nombre = vm.Nombre,
                    FechaRealizacion = vm.FechaRealizacion
                }, ct);

                TempData["Success"] = "Elección creada y activada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            try
            {
                var ok = await _service.FinalizarActivaAsync(ct);
                TempData[ok ? "Success" : "Error"] = ok
                    ? "Elección finalizada."
                    : "No hay elección activa para finalizar.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Resumen() => View();

        [HttpPost]
        public async Task<IActionResult> Resumen(int year, CancellationToken ct)
        {
            try
            {
                var data = await _service.ResumenPorAnioAsync(year, ct);
                return View("ResumenResultados", data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


