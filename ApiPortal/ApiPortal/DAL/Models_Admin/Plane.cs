using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class Plane
    {
        public Plane()
        {
            EmpresasPortals = new HashSet<EmpresasPortal>();
        }

        public int IdPlan { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<EmpresasPortal> EmpresasPortals { get; set; }
    }
}
