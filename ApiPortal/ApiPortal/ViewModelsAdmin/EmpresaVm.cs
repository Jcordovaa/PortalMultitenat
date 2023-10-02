using ApiPortal.Dal.Models_Admin;

namespace ApiPortal.ViewModelsAdmin
{
    public class EmpresaVm
    {
        public int? IdEmpresa { get; set; }
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
        public string? NombreImplementador { get; set; }
        public string? TelefonoImplementador { get; set; }
        public string? CorreoImplementador { get; set; }
        public int? TipoCliente { get; set; }

        public  EmpresaEstadoVm? Estado { get; set; }
        public  ImplementadorVm? Implementador { get; set; }
        public  List<TenantVm>? Tenants { get; set; }
        public int? TotalFilas { get; set; }
    }
}
