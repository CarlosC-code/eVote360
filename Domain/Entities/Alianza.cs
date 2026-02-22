
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Alianza : BasicEntity<int>
    {

       
        public int PartidoAId { get; set; }
        public Partido PartidoA { get; set; } = default!;

        public int PartidoBId { get; set; }
        public Partido PartidoB { get; set; } = default!;

        public DateTime FechaAceptacionUtc { get; set; } = DateTime.UtcNow;

    }
}
