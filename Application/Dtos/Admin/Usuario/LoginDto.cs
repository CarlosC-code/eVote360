namespace eVote360.Core.Application.Dtos.Admin.Usuario
{
    public class LoginDto
    {
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
    }
}
