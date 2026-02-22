
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Candidato : BasicEntity<int>
    {
        public required string Nombre { get; set; } = default!;
        public required string Apellido { get; set; } = default!;
        public string? FotoPath { get; set; }

        // FK al partido de origen (obligatorio)
        public int PartidoId { get; set; }
        public Partido Partido { get; set; } = default!;

        // Puesto de origen 
        public int? PuestoElectivoOrigenId { get; set; }
        public PuestoElectivo? PuestoElectivoOrigen { get; set; }

        // Candidaturas en partidos (propio o aliados)
        public ICollection<Candidatura>? Candidaturas { get; set; } 

        public ICollection<Voto>? Votos { get; set; } 
    }

}

