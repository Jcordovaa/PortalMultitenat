using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class ConfiguracionCorreoImplementacion
    {
        public int IdConfiguracionCorreo { get; set; }
        public string? SmtpServer { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public int? Puerto { get; set; }
        public int? Ssl { get; set; }
        public string? TituloAccesoUsuario { get; set; }
        public string? AsuntoAccesoUsuario { get; set; }
        public string? TextoAccesoUsuario { get; set; }
        public string? TituloEnvioDocumentos { get; set; }
        public string? AsuntoEnvioDocumentos { get; set; }
        public string? TextoEnvioDocumentos { get; set; }
        public string? LogoCorreo { get; set; }
        public string? TextoMensajeActivacion { get; set; }
        public string? TituloCambioClave { get; set; }
        public string? TituloRecuperarClave { get; set; }
        public string? AsuntoCambioClave { get; set; }
        public string? AsuntoRecuperarClave { get; set; }
        public string? TextoCambioDatos { get; set; }
        public string? TextoCambioClave { get; set; }
        public string? TextoRecuperarClave { get; set; }
        public string? ColorBoton { get; set; }
        public string? AsuntoCambioCorreo { get; set; }
        public string? TituloCambioCorreo { get; set; }
        public string? TextoCambioCorreo { get; set; }
        public string? CorreoOrigen { get; set; }
    }
}
