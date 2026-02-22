using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Votacion
{
    public class ElectorIngresoViewModel
    {
        [Required(ErrorMessage = "Debe ingresar su documento de identidad.")]
        [RegularExpression(@"^\d{3,20}$", ErrorMessage = "Documento inválido.")]
        public string DocumentoIdentidad { get; set; } = default!;
    }
}
