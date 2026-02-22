using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class Ciudadano : BasicEntity<int>
    {
        public required string Nombre { get; set; } = default!;
        public required string Apellido { get; set; } = default!;
        public required string Email { get; set; } = default!;

      
        public required string DocumentoIdentidad { get; set; } = default!;

        // Navegacion
        public ICollection<Voto>? Votos { get; set; } 

    }
}
