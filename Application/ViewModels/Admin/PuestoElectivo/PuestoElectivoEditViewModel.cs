using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Admin.PuestoElectivo
{
    public class PuestoElectivoEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500)]
        public string Descripcion { get; set; } = default!;
    }
}
