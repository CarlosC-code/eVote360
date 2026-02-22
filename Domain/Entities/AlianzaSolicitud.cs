using eVote360.Core.Domain.Common;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Domain.Entities
{
    public class AlianzaSolicitud : BasicEntity<int>
    {
        public int PartidoSolicitanteId { get; set; }
        public Partido PartidoSolicitante { get; set; } = default!;

        public int PartidoDestinoId { get; set; }
        public Partido PartidoDestino { get; set; } = default!;

        public DateTime FechaSolicitudUtc { get; set; } = DateTime.UtcNow;
        public AllianceRequestStatus Estado { get; set; } = AllianceRequestStatus.EnEspera;

    }
}
