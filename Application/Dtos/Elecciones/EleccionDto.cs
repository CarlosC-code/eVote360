using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Application.Dtos.Elecciones
{
    public class EleccionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public DateTime FechaRealizacion { get; set; }
        public ElectionStatus Estado { get; set; }

    }
}
