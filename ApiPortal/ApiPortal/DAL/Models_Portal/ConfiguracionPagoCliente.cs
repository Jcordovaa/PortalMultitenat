using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionPagoCliente
    {
        public int? IdConfiguracionPago { get; set; }
        public string? CuentasContablesDeuda { get; set; }
        public string? TiposDocumentosDeuda { get; set; }
        public int? AnioTributario { get; set; }
        public string? MonedaUtilizada { get; set; }
        public string? GlosaComprobante { get; set; }
        public string? CentroCosto { get; set; }
        public string? AreaNegocio { get; set; }
        public int? DiasPorVencer { get; set; }
        public string? DocumentosCobranza { get; set; }
        public string? GlosaDetalle { get; set; }
        public string? GlosaPago { get; set; }
        public string? SegundaMonedaUtilizada { get; set; }
    }
}
