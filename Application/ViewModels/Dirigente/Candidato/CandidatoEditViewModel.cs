using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360.Core.Application.ViewModels.Dirigente.Candidato
{

    public class CandidatoEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        public IFormFile? Foto { get; set; }            
        public string? ExistingFotoPath { get; set; }    
    }
}
