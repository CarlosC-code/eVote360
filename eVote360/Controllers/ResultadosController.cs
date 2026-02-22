using eVote360.Core.Application.Interface.Votacion;
using Microsoft.AspNetCore.Mvc;

namespace eVote360.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly IResultadosService _service;

        public ResultadosController(IResultadosService service) => _service = service;

        [HttpGet]
        public IActionResult Index() => View(); 

        [HttpPost]
        public async Task<IActionResult> PorEleccion(int eleccionId, CancellationToken ct)
        {
            try
            {
                var data = await _service.ObtenerResultadosPorEleccionAsync(eleccionId, ct);
                return View("ResultadosPorEleccion", data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
