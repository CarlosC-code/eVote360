
namespace eVote360.Core.Application.Dtos.Elecciones
{
    public class EleccionResumenDto
    {
        public int EleccionId { get; set; }
        public string Nombre { get; set; } = default!;
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatos { get; set; }
        public int TotalVotosEmitidos { get; set; }

    }
}
