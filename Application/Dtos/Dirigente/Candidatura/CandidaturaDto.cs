

namespace eVote360.Core.Application.Dtos.Dirigente.Candidatura
{
    public class CandidaturaDto
    {
        public int Id { get; set; }
        public int CandidatoId { get; set; }
        public int PartidoId { get; set; }
        public int PuestoElectivoId { get; set; }
        public bool IsActive { get; set; }

    }
}
