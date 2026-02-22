using eVote360.Core.Application.Dtos.Dirigente.Alianza;
using eVote360.Core.Application.Interface.Dirigente;
using eVote360.Core.Application.ViewModels.Dirigente.Alianza;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class AlianzasController : Controller
    {
        private readonly IAlianzaPoliticaService _service;

        public AlianzasController(IAlianzaPoliticaService service) => _service = service;

        // Vista principal con las 3 listas
        public async Task<IActionResult> Index(int partidoId, CancellationToken ct)
        {
            ViewBag.PartidoId = partidoId;

            var pendientes = await _service.ListarPendientesRecibidasAsync(partidoId, ct);
            var enviadas = await _service.ListarSolicitudesEnviadasAsync(partidoId, ct);
            var activas = await _service.ListarAlianzasActivasAsync(partidoId, ct);

            ViewBag.Pendientes = pendientes;
            ViewBag.Enviadas = enviadas;
            ViewBag.Activas = activas;

            return View();
        }

        [HttpGet]
        public IActionResult CrearSolicitud(int partidoId)
        {
            ViewBag.PartidoId = partidoId;
            return View(new AlianzaSolicitudCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(int partidoId, AlianzaSolicitudCreateViewModel vm, CancellationToken ct)
        {
            ViewBag.PartidoId = partidoId;
            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _service.CrearSolicitudAsync(partidoId, new AlianzaSolicitudCreateDto
                {
                    PartidoDestinoId = vm.PartidoDestinoId
                }, ct);

                TempData["Success"] = "Solicitud de alianza creada.";
                return RedirectToAction(nameof(Index), new { partidoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Aceptar(int solicitudId, int partidoId, CancellationToken ct)
        {
            try
            {
                var ok = await _service.AceptarSolicitudAsync(solicitudId, partidoId, ct);
                TempData[ok ? "Success" : "Error"] = ok ? "Alianza aceptada." : "No fue posible aceptar la alianza.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { partidoId });
        }

        [HttpPost]
        public async Task<IActionResult> Rechazar(int solicitudId, int partidoId, CancellationToken ct)
        {
            try
            {
                var ok = await _service.RechazarSolicitudAsync(solicitudId, partidoId, ct);
                TempData[ok ? "Success" : "Error"] = ok ? "Solicitud rechazada." : "No fue posible rechazar la solicitud.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { partidoId });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarSolicitud(int solicitudId, int partidoId, CancellationToken ct)
        {
            try
            {
                var ok = await _service.EliminarSolicitudAsync(solicitudId, partidoId, ct);
                TempData[ok ? "Success" : "Error"] = ok ? "Solicitud eliminada." : "No fue posible eliminar la solicitud.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { partidoId });
        }
    }
}

