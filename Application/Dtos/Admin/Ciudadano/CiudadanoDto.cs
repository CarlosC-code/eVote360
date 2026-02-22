

namespace eVote360.Core.Application.Dtos.Admin.Ciudadano
{
    public class CiudadanoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DocumentoIdentidad { get; set; } = default!;
        public bool IsActive { get; set; }

    }
}
