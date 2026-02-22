using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Admin.Ciudadano
{

    public class CiudadanoCreateViewModel
    {
        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "El documento es obligatorio.")]
        [RegularExpression(@"^\d{3,20}$", ErrorMessage = "Documento inválido.")]
        public string DocumentoIdentidad { get; set; } = default!;
    }
}
