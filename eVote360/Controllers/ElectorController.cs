using eVote360.Core.Application.Dtos.Votacion;
using eVote360.Core.Application.Interface.Votacion;
using eVote360.Core.Application.ViewModels.Votacion;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class ElectorController : Controller
    {
        private readonly IElectorService _service;

        public ElectorController(IElectorService service) => _service = service;

        [HttpGet]
        public IActionResult Index() => View(new ElectorIngresoViewModel());

        [HttpPost]
        public async Task<IActionResult> Index(ElectorIngresoViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var (puedeVotar, mensaje, ciudadanoId) = await _service.PreCheckAsync(vm.DocumentoIdentidad, ct);
            if (!puedeVotar)
            {
                TempData["Error"] = mensaje;
                return View(vm);
            }

            TempData["DocumentoDigitado"] = vm.DocumentoIdentidad;
            TempData["CiudadanoId"] = ciudadanoId;
            return RedirectToAction(nameof(ValidacionIdentidad));
        }

        [HttpGet]
        public IActionResult ValidacionIdentidad()
        {
            var doc = TempData.Peek("DocumentoDigitado")?.ToString();
            if (string.IsNullOrWhiteSpace(doc)) return RedirectToAction(nameof(Index));
            return View(new ValidacionOcrViewModel { DocumentoDigitado = doc });
        }

        [HttpPost]
        public async Task<IActionResult> ValidacionIdentidad(ValidacionOcrViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);
            if (vm.FotoCedulaFrontal is null || vm.FotoCedulaFrontal.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.FotoCedulaFrontal), "Debe subir la foto frontal de su cédula.");
                return View(vm);
            }

            using var stream = vm.FotoCedulaFrontal.OpenReadStream();
            var ok = await _service.ValidarIdentidadConOcrAsync(vm.DocumentoDigitado, stream, ct);

            if (!ok)
            {
                TempData["Error"] = "Los datos extraídos de la foto no coinciden con el documento ingresado.";
                TempData["DocumentoDigitado"] = vm.DocumentoDigitado;
                return RedirectToAction(nameof(ValidacionIdentidad));
            }

            return RedirectToAction(nameof(Puestos));
        }

        [HttpGet]
        public async Task<IActionResult> Puestos(CancellationToken ct)
        {
            var puestos = await _service.ListarPuestosDeEleccionActivaAsync(ct);
            return View(puestos);
        }

        [HttpGet]
        public async Task<IActionResult> Votar(int puestoId, CancellationToken ct)
        {
            var ciudadanoId = TempData.Peek("CiudadanoId") as int?;
            if (!ciudadanoId.HasValue) return RedirectToAction(nameof(Index));

            var candidaturas = await _service.ListarCandidaturasPorPuestoEnActivaAsync(puestoId, ct);

            var vm = new VotoPorPuestoViewModel
            {
                CiudadanoId = ciudadanoId.Value,
                PuestoElectivoId = puestoId,
                Opciones = candidaturas.Select(c => new CandidaturaOptionViewModel
                {
                    CandidaturaId = c.Id,
                    Texto = $"Candidatura #{c.Id}"
                }).ToList()
            };
            vm.Opciones.Insert(0, new CandidaturaOptionViewModel { CandidaturaId = null, Texto = "Ninguno" });

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Votar(VotoPorPuestoViewModel vm, CancellationToken ct)
        {
            var ciudadanoId = TempData.Peek("CiudadanoId") as int?;
            if (!ciudadanoId.HasValue)
                return RedirectToAction(nameof(Index));

            // 🔹 Si el modelo no es válido, recargamos las opciones
            if (!ModelState.IsValid)
            {
                var candidaturas = await _service
                    .ListarCandidaturasPorPuestoEnActivaAsync(vm.PuestoElectivoId, ct);

                vm.CiudadanoId = ciudadanoId.Value;
                vm.Opciones = candidaturas.Select(c => new CandidaturaOptionViewModel
                {
                    CandidaturaId = c.Id,
                    Texto = $"Candidatura #{c.Id}"
                }).ToList();

                vm.Opciones.Insert(0, new CandidaturaOptionViewModel
                {
                    CandidaturaId = null,
                    Texto = "Ninguno"
                });

                return View(vm);
            }

            var dto = new VotoCreateDto
            {
                EleccionId = 0,
                CiudadanoId = ciudadanoId.Value,
                PuestoElectivoId = vm.PuestoElectivoId,
                CandidaturaId = vm.CandidaturaId
            };

            try
            {
                var ok = await _service.EmitirVotoAsync(dto, ct);
                if (!ok)
                    throw new InvalidOperationException("No se pudo registrar el voto.");

                TempData["Success"] = "Voto registrado.";
                return RedirectToAction(nameof(Puestos));
            }
            catch (Exception ex)
            {
                // 🔹 Si ocurre error también recargamos opciones
                var candidaturas = await _service
                    .ListarCandidaturasPorPuestoEnActivaAsync(vm.PuestoElectivoId, ct);

                vm.CiudadanoId = ciudadanoId.Value;
                vm.Opciones = candidaturas.Select(c => new CandidaturaOptionViewModel
                {
                    CandidaturaId = c.Id,
                    Texto = $"Candidatura #{c.Id}"
                }).ToList();

                vm.Opciones.Insert(0, new CandidaturaOptionViewModel
                {
                    CandidaturaId = null,
                    Texto = "Ninguno"
                });

                TempData["Error"] = ex.Message;
                return View(vm);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            var ciudadanoId = TempData.Peek("CiudadanoId") as int?;
            if (!ciudadanoId.HasValue) return RedirectToAction(nameof(Index));

            try
            {
                var ok = await _service.FinalizarVotacionAsync(ciudadanoId.Value, ct);
                TempData[ok ? "Success" : "Error"] = ok
                    ? "Votación finalizada. Se ha enviado un resumen a su correo."
                    : "No se pudo finalizar la votación.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
