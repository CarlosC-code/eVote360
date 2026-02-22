using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class PuestoElectivo : BasicEntity<int>
    {
        public required string Nombre { get; set; } = default!;
        public required string Descripcion { get; set; } = default!;

        public ICollection<Candidato>? CandidatosOrigen { get; set; } 
        public ICollection<Candidatura>? Candidaturas { get; set; } 
        public ICollection<EleccionPuesto>? EleccionPuestos { get; set; } 
        public ICollection<Voto>? Votos { get; set; }

    }
}
