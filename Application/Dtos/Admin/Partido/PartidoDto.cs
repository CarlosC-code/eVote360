

namespace eVote360.Core.Application.Dtos.Admin.Partido
{
    public class PartidoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public string Siglas { get; set; } = default!;
        public string? LogoPath { get; set; }
        public bool IsActive { get; set; }

    }
}
