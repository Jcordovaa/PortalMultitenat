using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class TipoCobranza
    {
        public TipoCobranza()
        {
            CobranzaCabeceras = new HashSet<CobranzaCabecera>();
        }

        public int IdTipoCobranza { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<CobranzaCabecera> CobranzaCabeceras { get; set; }
    }
}
