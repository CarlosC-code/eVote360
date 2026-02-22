using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Elecciones
{
    public class ItemResultadoViewModel
    {
        public int CandidaturaId { get; set; }
        public string CandidatoNombreCompleto { get; set; } = default!;
        public string PartidoSiglas { get; set; } = default!;
        public int Votos { get; set; }
        [Display(Name = "%")]
        public decimal Porcentaje { get; set; }
    }
}

