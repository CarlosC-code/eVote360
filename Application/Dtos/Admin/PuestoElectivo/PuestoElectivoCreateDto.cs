using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360.Core.Application.Dtos.Admin.PuestoElectivo
{
    public class PuestoElectivoCreateDto
    {
        public string Nombre { get; set; } = default!;
        public string Descripcion { get; set; } = default!;

    }
}
