
using eVote360.Core.Application.Interface.Admin;
using eVote360.Core.Domain.Common.Enums;
using eVote360.Core.Application.ViewModels.Admin.AsignacionDirigente;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class AsignacionDirigenteController : Controller
    {
        private readonly IAsignacionDirigenteService _service;
        private readonly IUsuarioService _usuarioService;
        private readonly IPartidoService _partidoService;

        public AsignacionDirigenteController(
            IAsignacionDirigenteService service,
            IUsuarioService usuarioService,
            IPartidoService partidoService)
        {
            _service = service;
            _usuarioService = usuarioService;
            _partidoService = partidoService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var rels = await _service.ListarAsync(ct);
            return View(rels);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await CargarCombos(ct);
            return View(new AsignacionDirigenteCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AsignacionDirigenteCreateViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombos(ct);
                return View(vm);
            }

            try
            {
                await _service.AsignarAsync(vm.UsuarioId, vm.PartidoId, ct);
                TempData["Success"] = "Asignación creada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                await CargarCombos(ct);
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                var ok = await _service.EliminarAsync(id, ct);
                if (!ok) return NotFound();
                TempData["Success"] = "Asignación eliminada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCombos(CancellationToken ct)
        {
            var usuarios = await _usuarioService.GetAllAsync(ct);
            var rels = await _service.ListarAsync(ct);
            var asignados = rels.Select(r => r.UsuarioId).ToHashSet();

            ViewBag.Dirigentes = usuarios
                .Where(u => u.IsActive && u.Rol == UserRole.Dirigente && !asignados.Contains(u.Id))
                .Select(u => new { u.Id, Nombre = $"{u.Nombre} {u.Apellido}" })
                .ToList();

            var partidos = await _partidoService.GetAllAsync(ct);
            ViewBag.Partidos = partidos
                .Where(p => p.IsActive)
                .Select(p => new { p.Id, Nombre = $"{p.Nombre} [{p.Siglas}]" })
                .ToList();
        }
    }
}
