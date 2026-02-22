
namespace eVote360.Core.Application.Dtos.Admin.Ciudadano
{
    public class CiudadanoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DocumentoIdentidad { get; set; } = default!;

    }
}
