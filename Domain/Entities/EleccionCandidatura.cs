using eVote360.Core.Domain.Common;


namespace eVote360.Core.Domain.Entities
{
    public class EleccionCandidatura : BasicEntity<int>
    {
        public int EleccionId { get; set; }
        public Eleccion Eleccion { get; set; } = default!;

        public int CandidaturaId { get; set; }
        public Candidatura Candidatura { get; set; } = default!;

    }
}
