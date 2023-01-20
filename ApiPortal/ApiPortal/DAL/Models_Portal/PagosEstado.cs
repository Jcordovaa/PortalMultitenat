using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class PagosEstado
    {
        public PagosEstado()
        {
            PagosCabeceras = new HashSet<PagosCabecera>();
        }

        public int IdPagosEstado { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<PagosCabecera> PagosCabeceras { get; set; }
    }
}
