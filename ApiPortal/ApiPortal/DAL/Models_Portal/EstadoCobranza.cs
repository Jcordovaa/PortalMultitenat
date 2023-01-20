using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class EstadoCobranza
    {
        public EstadoCobranza()
        {
            CobranzaCabeceras = new HashSet<CobranzaCabecera>();
            CobranzaDetalles = new HashSet<CobranzaDetalle>();
        }

        public int IdEstadoCobranza { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<CobranzaCabecera> CobranzaCabeceras { get; set; }
        public virtual ICollection<CobranzaDetalle> CobranzaDetalles { get; set; }
    }
}
