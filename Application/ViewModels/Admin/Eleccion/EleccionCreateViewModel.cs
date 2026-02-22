using System.ComponentModel.DataAnnotations;

namespace eVote360.Core.Application.ViewModels.Admin.Eleccion
{
    public class EleccionCreateViewModel
    {
        [Required, StringLength(150)]
        public string Nombre { get; set; } = default!;

        [Required, DataType(DataType.Date)]
        public DateTime FechaRealizacion { get; set; }
    }
}
