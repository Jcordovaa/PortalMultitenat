using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class Implementador
    {
        public Implementador()
        {
            EmpresasPortals = new HashSet<EmpresasPortal>();
            Tenants = new HashSet<Tenant>();
            UsuariosPortals = new HashSet<UsuariosPortal>();
        }

        public int IdImplementador { get; set; }
        public string? Rut { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<EmpresasPortal> EmpresasPortals { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<UsuariosPortal> UsuariosPortals { get; set; }
    }
}
