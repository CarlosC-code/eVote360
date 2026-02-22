

namespace eVote360.Core.Application.Dtos.Elecciones
{
    public class ResultadoPuestoDto
    {
        public int PuestoElectivoId { get; set; }
        public string PuestoNombre { get; set; } = default!;
        public List<ItemResultadoCandidatoDto> Resultados { get; set; } = new();

    }
}
