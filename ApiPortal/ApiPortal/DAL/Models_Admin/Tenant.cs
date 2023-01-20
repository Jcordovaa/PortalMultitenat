using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class Tenant
    {
        public int IdTenant { get; set; }
        public int? IdEmpresa { get; set; }
        public string? Identifier { get; set; }
        public string? Dominio { get; set; }
        public string? ConnectionString { get; set; }
        public int? Estado { get; set; }

        public virtual EmpresasPortal? IdEmpresaNavigation { get; set; }
    }
}
