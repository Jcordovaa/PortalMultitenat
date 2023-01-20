using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class EmpresaEstado
    {
        public EmpresaEstado()
        {
            EmpresasPortals = new HashSet<EmpresasPortal>();
        }

        public int IdEstado { get; set; }
        public string? Nombre { get; set; }

        public virtual ICollection<EmpresasPortal> EmpresasPortals { get; set; }
    }
}
