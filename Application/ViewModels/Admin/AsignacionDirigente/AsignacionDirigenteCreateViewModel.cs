using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Admin.AsignacionDirigente
{

    public class AsignacionDirigenteCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un dirigente.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un partido.")]
        public int PartidoId { get; set; }
    }
}
