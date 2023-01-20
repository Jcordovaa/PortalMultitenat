using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class EmpresasPortal
    {
        public EmpresasPortal()
        {
            Tenants = new HashSet<Tenant>();
        }

        public int IdEmpresa { get; set; }
        public string? Rut { get; set; }
        public string? RazonSocial { get; set; }
        public int? IdPlan { get; set; }
        public DateTime? FechaInicioContrato { get; set; }
        public DateTime? FechaTerminoContrato { get; set; }
        public DateTime? FechaInicioImplementacion { get; set; }
        public DateTime? FechaTerminoImplementacion { get; set; }
        public int? IdImplementador { get; set; }
        public int? IdAreaComercial { get; set; }
        public int? IdLineaProducto { get; set; }
        public string? OtImplementacion { get; set; }
        public int? IdEstado { get; set; }

        public virtual AreaComercial? IdAreaComercialNavigation { get; set; }
        public virtual EmpresaEstado? IdEstadoNavigation { get; set; }
        public virtual Implementador? IdImplementadorNavigation { get; set; }
        public virtual LineaProducto? IdLineaProductoNavigation { get; set; }
        public virtual Plane? IdPlanNavigation { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
    }
}
