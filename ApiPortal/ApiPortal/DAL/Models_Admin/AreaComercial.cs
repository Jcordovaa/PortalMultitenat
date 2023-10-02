using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class AreaComercial
    {
        public AreaComercial()
        {
            EmpresasPortals = new HashSet<EmpresasPortal>();
            Tenants = new HashSet<Tenant>();
        }

        public int IdArea { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<EmpresasPortal> EmpresasPortals { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
    }
}
