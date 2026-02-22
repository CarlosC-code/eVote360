using System.ComponentModel.DataAnnotations;
using eVote360.Core.Domain.Common.Enums;

namespace eVote360.Core.Application.ViewModels.Admin.Usuario
{
    public class UsuarioCreateViewModel
    {
        [Required, StringLength(100)]
        public string Nombre { get; set; } = default!;

        [Required, StringLength(100)]
        public string Apellido { get; set; } = default!;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = default!;

        [Required, StringLength(100)]
        public string UserName { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        public UserRole Rol { get; set; }
    }
}