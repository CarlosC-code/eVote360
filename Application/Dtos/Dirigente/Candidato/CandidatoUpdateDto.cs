
namespace eVote360.Core.Application.Dtos.Dirigente.Candidato
{
    public class CandidatoUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string? FotoPath { get; set; } 

    }
}
