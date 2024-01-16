using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionEmpresa
    {
        public int IdConfiguracionEmpresa { get; set; }
        public string? RutEmpresa { get; set; }
        public string? NombreEmpresa { get; set; }
        public string? Direccion { get; set; }
        public string? RutaGoogleMaps { get; set; }
        public string? Telefono { get; set; }
        public string? CorreoContacto { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? Twitter { get; set; }
        public string? Youtube { get; set; }
        public string? Linkedin { get; set; }
        public string? UrlPortal { get; set; }
        public string? Logo { get; set; }
        public string? Web { get; set; }
    }
}
