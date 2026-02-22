using eVote360.Core.Application.Dtos.Elecciones;
using eVote360.Core.Application.Interface.Votacion;
using eVote360.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eVote360.Core.Application.Services.Elecciones
{
    public sealed class ResultadosService : IResultadosService
    {
        private readonly IEleccionRepository _eleccionRepo;
        private readonly IEleccionPuestoRepository _eleccionPuestoRepo;
        private readonly IEleccionCandidaturaRepository _eleccionCandidaturaRepo;
        private readonly IVotoRepository _votoRepo;

        public ResultadosService(
            IEleccionRepository eleccionRepo,
            IEleccionPuestoRepository eleccionPuestoRepo,
            IEleccionCandidaturaRepository eleccionCandidaturaRepo,
            IVotoRepository votoRepo)
        {
            _eleccionRepo = eleccionRepo;
            _eleccionPuestoRepo = eleccionPuestoRepo;
            _eleccionCandidaturaRepo = eleccionCandidaturaRepo;
            _votoRepo = votoRepo;
        }

        public async Task<List<ResultadoPuestoDto>> ObtenerResultadosPorEleccionAsync(int eleccionId, CancellationToken ct)
        {
            var eleccion = await _eleccionRepo.GetByIdAsync(eleccionId, ct) ?? throw new InvalidOperationException("Elección no encontrada.");
            if (eleccion.Estado != eVote360.Core.Domain.Common.Enums.ElectionStatus.Finalizada)
                throw new InvalidOperationException("Solo se pueden consultar resultados de elecciones finalizadas."); 

            var puestos = await _eleccionPuestoRepo.FindAsync(ep => ep.EleccionId == eleccionId, ct);
            var resultados = new List<ResultadoPuestoDto>();

            foreach (var ep in puestos)
            {
                // Votos del puesto 
                var votosPuesto = await _votoRepo.FindAsync(v => v.EleccionId == eleccionId && v.PuestoElectivoId == ep.PuestoElectivoId, ct);
                var total = votosPuesto.Count;

                // Agrupar por candidatura (excluye "Ninguno")
                var grupo = votosPuesto
                    .Where(v => v.CandidaturaId.HasValue)
                    .GroupBy(v => v.CandidaturaId!.Value)
                    .Select(g => new { CandidaturaId = g.Key, Votos = g.Count() })
                    .ToList();

                // Necesitamos nombre completo del candidato y siglas del partido desde snapshot/candidaturas
               
                var candIds = grupo.Select(g => g.CandidaturaId).ToList();
                var snaps = await _eleccionCandidaturaRepo.GetAllQueryWithInclude(ec => ec.Candidatura, ec => ec.Candidatura.Candidato, ec => ec.Candidatura.Partido)
                    .Where(ec => ec.EleccionId == eleccionId && candIds.Contains(ec.CandidaturaId))
                    .ToListAsync(ct);

                var items = new List<ItemResultadoCandidatoDto>();
                foreach (var x in grupo)
                {
                    var snap = snaps.First(ec => ec.CandidaturaId == x.CandidaturaId);
                    var nombre = $"{snap.Candidatura.Candidato.Nombre} {snap.Candidatura.Candidato.Apellido}";
                    var siglas = snap.Candidatura.Partido.Siglas;
                    var porcentaje = total == 0 ? 0m : Math.Round((decimal)x.Votos * 100m / total, 2);
                    items.Add(new ItemResultadoCandidatoDto
                    {
                        CandidaturaId = x.CandidaturaId,
                        CandidatoNombreCompleto = nombre,
                        PartidoSiglas = siglas,
                        Votos = x.Votos,
                        Porcentaje = porcentaje
                    });
                }

                resultados.Add(new ResultadoPuestoDto
                {
                    PuestoElectivoId = ep.PuestoElectivoId,
                    PuestoNombre = "", 
                    Resultados = items.OrderByDescending(i => i.Votos).ToList()
                });
            }

            return resultados;
        }
    }
}

