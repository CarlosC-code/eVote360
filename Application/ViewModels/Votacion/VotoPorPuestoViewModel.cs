using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Votacion
{
    public class VotoPorPuestoViewModel
    {
        [Required]
        public int EleccionId { get; set; }

        [Required]
        public int CiudadanoId { get; set; }

        [Required]
        public int PuestoElectivoId { get; set; }

        
        public int? CandidaturaId { get; set; }

       
        public string? PuestoNombre { get; set; }
        public List<CandidaturaOptionViewModel>? Opciones { get; set; } 
    }
}
