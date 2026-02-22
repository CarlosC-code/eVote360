

namespace eVote360.Core.Application.Dtos.Elecciones
{
    public class ItemResultadoCandidatoDto
    {
        public int CandidaturaId { get; set; }
        public string CandidatoNombreCompleto { get; set; } = default!;
        public string PartidoSiglas { get; set; } = default!;
        public int Votos { get; set; }
        public decimal Porcentaje { get; set; }

    }
}
