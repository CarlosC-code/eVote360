
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Candidatura : BasicEntity<int>
    {
        public int CandidatoId { get; set; }
        public Candidato Candidato { get; set; } = default!;

        // Partido por el que se postula (puede ser su partido de origen o un aliado)
        public int PartidoId { get; set; }
        public Partido Partido { get; set; } = default!;

        // Puesto al que aspira en ese partido
        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = default!;

        // Relacion con elecciones
        public ICollection<EleccionCandidatura>? EleccionCandidaturas { get; set; } 

        // Votos emitidos en elecciones para esta combinacion 
        public ICollection<Voto>? Votos { get; set; }

    }
}
