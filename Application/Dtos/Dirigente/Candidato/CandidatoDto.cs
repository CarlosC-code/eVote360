

namespace eVote360.Core.Application.Dtos.Dirigente.Candidato
{
    public class CandidatoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string? FotoPath { get; set; }
        public int PartidoId { get; set; }
        public int? PuestoElectivoOrigenId { get; set; }
        public bool IsActive { get; set; }

    }
}
