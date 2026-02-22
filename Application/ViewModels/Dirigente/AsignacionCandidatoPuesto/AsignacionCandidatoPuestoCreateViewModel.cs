using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Dirigente.AsignacionCandidatoPuesto
{
    public class AsignacionCandidatoPuestoCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un candidato.")]
        public int CandidatoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un puesto electivo.")]
        public int PuestoElectivoId { get; set; }
    }
}
