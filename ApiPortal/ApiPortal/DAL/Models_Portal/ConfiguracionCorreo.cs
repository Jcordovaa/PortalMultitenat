using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionCorreo
    {
        public int IdConfiguracionCorreo { get; set; }
        public string? SmtpServer { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public int? Puerto { get; set; }
        public int? Ssl { get; set; }
        public string? CorreoAvisoPago { get; set; }
        public string? AsuntoPagoCliente { get; set; }
        public string? AsuntoAccesoCliente { get; set; }
        public string? NombreCorreos { get; set; }
        public string? AsuntoEnvioDocumentos { get; set; }
        public string? TextoEnvioDocumentos { get; set; }
        public int? CantidadCorreosAcceso { get; set; }
        public int? CantidadCorreosNotificacion { get; set; }
        public string? LogoCorreo { get; set; }
        public string? TextoMensajeActivacion { get; set; }
        public string? TituloPagoCliente { get; set; }
        public string? TituloAccesoCliente { get; set; }
        public string? TituloEnvioDocumentos { get; set; }
        public string? TituloCambioDatos { get; set; }
        public string? TituloCambioClave { get; set; }
        public string? TituloRecuperarClave { get; set; }
        public string? AsuntoCambioDatos { get; set; }
        public string? AsuntoCambioClave { get; set; }
        public string? AsuntoRecuperarClave { get; set; }
        public string? TextoPagoCliente { get; set; }
        public string? TextoAccesoCliente { get; set; }
        public string? TextoCambioDatos { get; set; }
        public string? TextoCambioClave { get; set; }
        public string? TextoRecuperarClave { get; set; }
        public string? TituloAvisoPagoCliente { get; set; }
        public string? AsuntoAvisoPagoCliente { get; set; }
        public string? TextoAvisoPagoCliente { get; set; }
        public string? ColorBoton { get; set; }
        public string? TituloPagoSinComprobante { get; set; }
        public string? AsuntoPagoSinComprobante { get; set; }
        public string? TextoPagoSinComprobante { get; set; }
        public string? AsuntoCambioCorreo { get; set; }
        public string? TituloCambioCorreo { get; set; }
        public string? TextoCambioCorreo { get; set; }
        public string? TituloCobranza { get; set; }
        public string? TextoCobranza { get; set; }
        public string? AsuntoCobranza { get; set; }
        public string? TituloPreCobranza { get; set; }
        public string? AsuntoPreCobranza { get; set; }
        public string? TextoPreCobranza { get; set; }
        public string? TextoEstadoCuenta { get; set; }
        public string? TituloEstadoCuenta { get; set; }
        public string? AsuntoEstadoCuenta { get; set; }
        public string? CorreoOrigen { get; set; }
    }
}
