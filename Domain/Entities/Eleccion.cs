
using eVote360.Core.Domain.Common;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Domain.Entities
{
    public class Eleccion : BasicEntity<int>
    {

        public string Nombre { get; set; } = default!;
        public DateTime FechaRealizacion { get; set; }
        public ElectionStatus Estado { get; set; } = ElectionStatus.EnProceso;

        // Puestos disputados en esta eleccion
        public ICollection<EleccionPuesto>? EleccionPuestos { get; set; } 

        // Candidaturas participantes 
        public ICollection<EleccionCandidatura>? EleccionCandidaturas { get; set; }

        // Votos emitidos en esta eleccion
        public ICollection<Voto>? Votos { get; set; } 

    }
}
