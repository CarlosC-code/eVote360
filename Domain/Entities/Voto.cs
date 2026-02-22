using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Voto : BasicEntity<int>
    {
        public int EleccionId { get; set; }
        public Eleccion Eleccion { get; set; } = default!;

        public int CiudadanoId { get; set; }
        public Ciudadano Ciudadano { get; set; } = default!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = default!;

        public int? CandidaturaId { get; set; }
        public Candidatura? Candidatura { get; set; }

        // Si es voto a candidato
        public int? CandidatoId { get; set; }
        public Candidato? Candidato { get; set; }

        // Partido por el que compite ese candidato 
        public int? PartidoId { get; set; }
        public Partido? Partido { get; set; }

        // Opción "Ninguno"
        public bool EsNinguno { get; set; } = false;

        public DateTime FechaEmisionUtc { get; set; } = DateTime.UtcNow;

    }
}
