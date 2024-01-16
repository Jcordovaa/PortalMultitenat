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
        public string? OtImplementacion { get; set; }
        public string? NombreImplementador { get; set; }
        public string? TelefonoImplementador { get; set; }
        public string? CorreoImplementador { get; set; }
        public DateTime? FechaInicioImplementacion { get; set; }
        public DateTime? FechaTerminoImplementacion { get; set; }
        public DateTime? FechaInicioContrato { get; set; }
        public DateTime? FechaTerminoContrato { get; set; }
        public int? IdPlan { get; set; }
        public int? IdImplementador { get; set; }
        public int? IdLineaProducto { get; set; }
        public int? IdAreaComercial { get; set; }
        public int? EnroladoVpos { get; set; }
        public int? EnroladoTbk { get; set; }
        public DateTime? FechaPasoProduccion { get; set; }

        public virtual EmpresaEstado? EstadoNavigation { get; set; }
        public virtual AreaComercial? IdAreaComercialNavigation { get; set; }
        public virtual EmpresasPortal? IdEmpresaNavigation { get; set; }
        public virtual Implementador? IdImplementadorNavigation { get; set; }
        public virtual LineaProducto? IdLineaProductoNavigation { get; set; }
        public virtual Plane? IdPlanNavigation { get; set; }
    }
}
