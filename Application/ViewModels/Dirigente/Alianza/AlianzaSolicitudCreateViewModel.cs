using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Dirigente.Alianza
{

    public class AlianzaSolicitudCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar el partido destino.")]
        public int PartidoDestinoId { get; set; }
    }
}

