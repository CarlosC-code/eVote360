namespace eVote360.Core.Application.ViewModels.Elecciones
{
    public class ResultadoPuestoViewModel
    {
        public int PuestoElectivoId { get; set; }
        public string PuestoNombre { get; set; } = default!;
        public List<ItemResultadoViewModel> Resultados { get; set; } = new();
    }
}
