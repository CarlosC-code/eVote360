

namespace eVote360.Core.Application.Dtos.Dirigente.Alianza
{
    public class AlianzaDto
    {
        public int Id { get; set; }
        public int PartidoAId { get; set; }
        public int PartidoBId { get; set; }
        public DateTime FechaAceptacionUtc { get; set; }
        public bool IsActive { get; set; }

    }
}
