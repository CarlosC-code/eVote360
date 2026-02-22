

namespace eVote360.Core.Application.Dtos.Votacion
{
    public class VotoCreateDto
    {
        public int EleccionId { get; set; }
        public int CiudadanoId { get; set; }
        public int PuestoElectivoId { get; set; }
        public int? CandidaturaId { get; set; }

    }
}
