using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eVote360.Core.Application.ViewModels.Admin.Partido
{
    public class PartidoCreateViewModel
    {
        [Required, StringLength(150)]
        public string Nombre { get; set; } = default!;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required, StringLength(20)]
        public string Siglas { get; set; } = default!;

        [Required(ErrorMessage = "El logo es obligatorio.")]
        public IFormFile Logo { get; set; } = default!; 
    }
}