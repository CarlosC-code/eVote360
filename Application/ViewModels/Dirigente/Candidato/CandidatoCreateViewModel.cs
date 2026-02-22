using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360.Core.Application.ViewModels.Dirigente.Candidato
{

    public class CandidatoCreateViewModel
    {
        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        [Required(ErrorMessage = "La foto es obligatoria.")]
        public IFormFile FotoFile { get; set; } = default!;


        public string? FotoPath { get; set; }

    }
}

