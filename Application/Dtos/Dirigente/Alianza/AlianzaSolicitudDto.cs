using eVote360.Core.Domain.Common.Enums;


namespace eVote360.Core.Application.Dtos.Dirigente.Alianza
{
    public class AlianzaSolicitudDto
    {
        public int Id { get; set; }
        public int PartidoSolicitanteId { get; set; }
        public int PartidoDestinoId { get; set; }
        public DateTime FechaSolicitudUtc { get; set; }
        public AllianceRequestStatus Estado { get; set; }

    }
}
