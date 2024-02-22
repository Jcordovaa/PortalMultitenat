using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsAdmin;

namespace ApiPortal.DAL.Models_Admin
{
    public class DatosImplementacionVm
    {
        public ApiSoftland? ApiSoftland { get; set; }
        public ConfiguracionCorreo? ConfiguracionCorreo { get; set; }
        public ConfiguracionDiseno? ConfiguracionDiseno { get; set; }
        public ConfiguracionEmpresa? ConfiguracionEmpresa { get; set; }
        public ConfiguracionPagoCliente? ConfiguracionPagoCliente { get; set; }
        public ConfiguracionPortal? ConfiguracionPortal { get; set; }
        public string? ServidorPortal { get; set; }
        public string? BaseDatosPortal { get; set; }
        public string? ClaveBaseDatosPortal { get; set; }
        public string? UsuarioBaseDatosPortal { get; set; }
        public int? UtilizaTransbank { get; set; }
        public int? UtilizaVirtualPos { get; set; }
        public string? CuentaContableTransbank { get; set; }
        public string? CuentaContableVirtualPos { get; set; }
        public string? DocumentoContableTransbank { get; set; }
        public string? DocumentoContableVirtualPos { get; set; }
        public string? CodigoComercioTransbank { get; set; }
        public int? AmbienteTransbank { get; set; }
        public string? ApiKeyTransbank { get; set; }
        public int IdServidorImplementacion { get; set; }
      

    }
}
