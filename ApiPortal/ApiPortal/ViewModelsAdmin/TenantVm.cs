using ApiPortal.Dal.Models_Admin;
using ApiPortal.DAL.Models_Admin;

namespace ApiPortal.ViewModelsAdmin
{
    public class TenantVm
    {
        public int? IdTenant { get; set; }
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
        public string? RutEmpresa { get; set; }
        public string? NombreEmpresa { get; set; }

        public EmpresaEstadoVm? EstadoTenant { get; set; }
        public  AreaComercialVm? AreaComercial { get; set; }
        public  ImplementadorVm? Implementador { get; set; }
        public  LineaProductoVm? LineaProducto { get; set; }
        public  PlanVm? Plan { get; set; }
        public DatosImplementacionVm? DatosImplementacion { get; set; }
        public IFormFile? LogoCorreo { get; set; }
        public IFormFile? ImagenUltimasCompras { get; set; }
        public IFormFile? BannerPortal { get; set; }
        public IFormFile? ImagenUsuario { get; set; }
        public IFormFile? IconoContactos { get; set; }
        public IFormFile? BannerMisCompras { get; set; }
        public IFormFile? IconoMisCompras { get; set; }
        public IFormFile? IconoClavePerfil { get; set; }
        public IFormFile? IconoEditarPerfil { get; set; }
        public IFormFile? IconoEstadoPerfil { get; set; }
        public IFormFile? ImagenPortada { get; set; }
        public IFormFile? LogoPortada { get; set; }
        public IFormFile? BannerPagoRapido { get; set; }
        public IFormFile? LogoMinimalistaSidebar { get; set; }
        public IFormFile? LogoSidebar { get; set; }

    }
}
