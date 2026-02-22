
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Application.Dtos.Admin.Usuario
{
    public class UsuarioUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? Password { get; set; } 
        public UserRole Rol { get; set; }

    }
}
