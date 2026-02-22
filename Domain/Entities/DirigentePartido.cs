
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class DirigentePartido : BasicEntity<int>
    {

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = default!;

        public int PartidoId { get; set; }
        public Partido Partido { get; set; } = default!;

    }
}
