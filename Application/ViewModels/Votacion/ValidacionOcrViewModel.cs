using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360.Core.Application.ViewModels.Votacion
{
    public class ValidacionOcrViewModel
    {
        [Required]
        public string DocumentoDigitado { get; set; } = default!;  

        [Required(ErrorMessage = "Debe subir la foto frontal de su cédula.")]
        public IFormFile FotoCedulaFrontal { get; set; } = default!;
    }
}
