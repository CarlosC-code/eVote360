using System.ComponentModel.DataAnnotations;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Application.ViewModels.Admin.Usuario
{
    public class UsuarioEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;

        [Required, StringLength(100)]
        public string UserName { get; set; } = default!;

        [DataType(DataType.Password)]
        public string? Password { get; set; } 

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public UserRole Rol { get; set; }
    }
}
