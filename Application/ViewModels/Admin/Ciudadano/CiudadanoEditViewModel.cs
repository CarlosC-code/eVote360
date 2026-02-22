using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Admin.Ciudadano
{
    public class CiudadanoEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;

        [Required]
        [RegularExpression(@"^\d{3,20}$", ErrorMessage = "Documento inválido.")]
        public string DocumentoIdentidad { get; set; } = default!;
    }
}
