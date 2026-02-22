
using eVote360.Core.Application.Dtos.Dirigente.Candidatura;
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Application.ViewModels.Dirigente.AsignacionCandidatoPuesto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 

namespace eVote360.Controllers
{
    public class AsignacionCandidatoPuestoController : Controller
    {
        private readonly IAsignacionCandidatoPuestoService _service;
        private readonly ICandidatoService _candidatoService;
        private readonly IPuestoElectivoService _puestoService;

        public AsignacionCandidatoPuestoController(
            IAsignacionCandidatoPuestoService service,
            ICandidatoService candidatoService,
            IPuestoElectivoService puestoService)
        {
            _service = service;
            _candidatoService = candidatoService;
            _puestoService = puestoService;
        }

       
        private int ResolvePartidoIdOrThrow(int partidoId)
        {
            if (partidoId > 0) return partidoId;
            var fromSession = HttpContext.Session.GetInt32("PartidoId");
            if (fromSession is int pid && pid > 0) return pid;
            throw new InvalidOperationException("No se pudo resolver el Partido del usuario actual.");
        }

        public async Task<IActionResult> Index(int partidoId, CancellationToken ct)
        {
            partidoId = ResolvePartidoIdOrThrow(partidoId);   //  asegura > 0
            ViewBag.PartidoId = partidoId;

            var list = await _service.ListarAsync(partidoId, ct);
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int partidoId, CancellationToken ct)
        {
            partidoId = ResolvePartidoIdOrThrow(partidoId);   // asegura > 0
            ViewBag.PartidoId = partidoId;

            await CargarCombos(partidoId, ct);
            return View(new AsignacionCandidatoPuestoCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(int partidoId, AsignacionCandidatoPuestoCreateViewModel vm, CancellationToken ct)
        {
            partidoId = ResolvePartidoIdOrThrow(partidoId);   //  asegura > 0
            ViewBag.PartidoId = partidoId;

            if (!ModelState.IsValid)
            {
                await CargarCombos(partidoId, ct);
                return View(vm);
            }

            try
            {
                var id = await _service.AsignarAsync(partidoId, new CandidaturaCreateDto
                {
                    CandidatoId = vm.CandidatoId,
                    PuestoElectivoId = vm.PuestoElectivoId,
                    PartidoId = partidoId // redundante, el service lo fuerza
                }, ct);

                TempData["Success"] = "Asignación creada.";
                return RedirectToAction(nameof(Index), new { partidoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await CargarCombos(partidoId, ct);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int partidoId, CancellationToken ct)
        {
            try
            {
                partidoId = ResolvePartidoIdOrThrow(partidoId);   //  asegura > 0

                var ok = await _service.EliminarAsync(id, partidoId, ct);
                TempData[ok ? "Success" : "Error"] = ok ? "Asignación eliminada." : "No fue posible eliminar la asignación.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { partidoId });
        }

        private async Task CargarCombos(int partidoId, CancellationToken ct)
        {
            // Si partidoId era 0, esto garantizo >0 antes de entrar aquí
            var candidatos = await _candidatoService.ListarPorPartidoAsync(partidoId, ct);
            ViewBag.Candidatos = candidatos
                .Where(c => c.IsActive)
                .Select(c => new { c.Id, Nombre = $"{c.Nombre} {c.Apellido}" })
                .ToList();

            var puestos = await _puestoService.GetAllAsync(ct);
            ViewBag.Puestos = puestos
                .Where(p => p.IsActive)
                .Select(p => new { p.Id, p.Nombre })
                .ToList();
        }
    }
}
