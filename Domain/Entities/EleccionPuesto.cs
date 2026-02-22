
using eVote360.Core.Domain.Common;

namespace eVote360.Core.Domain.Entities
{
    public class EleccionPuesto : BasicEntity<int>
    {
        public int EleccionId { get; set; }
        public Eleccion Eleccion { get; set; } = default!;

        public int PuestoElectivoId { get; set; }
        public PuestoElectivo PuestoElectivo { get; set; } = default!;

    }
}
