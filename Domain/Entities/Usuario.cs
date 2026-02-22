using eVote360.Core.Domain.Common;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Domain.Entities
{
    public class Usuario : BasicEntity<int>
    {
        public required string Nombre { get; set; } = default!;
        public required string Apellido { get; set; } = default!;
        public required string Email { get; set; } = default!;

        public required string UserName { get; set; } = default!;
        public required string PasswordHash { get; set; } = default!;

        public required UserRole Rol { get; set; }

        // Relacion con asignación de partido
        public ICollection<DirigentePartido>? DirigentePartidos { get; set; }

    }
}
