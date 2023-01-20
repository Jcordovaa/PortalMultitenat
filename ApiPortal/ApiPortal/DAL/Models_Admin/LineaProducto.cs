using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class LineaProducto
    {
        public LineaProducto()
        {
            EmpresasPortals = new HashSet<EmpresasPortal>();
        }

        public int IdLinea { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<EmpresasPortal> EmpresasPortals { get; set; }
    }
}
