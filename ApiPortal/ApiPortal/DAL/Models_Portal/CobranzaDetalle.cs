using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class CobranzaDetalle
    {
        public int IdCobranzaDetalle { get; set; }
        public int? IdCobranza { get; set; }
        public int? Folio { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public float? Monto { get; set; }
        public string? RutCliente { get; set; }
        public string? CodAuxCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public int? IdEstado { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? HoraEnvio { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public string? ComprobanteContable { get; set; }
        public string? FolioDte { get; set; }
        public int? IdPago { get; set; }
        public string? CuentaContable { get; set; }
        public string? EmailCliente { get; set; }
        public string? NombreCliente { get; set; }
        public float? Pagado { get; set; }

        public virtual CobranzaCabecera? IdCobranzaNavigation { get; set; }
        public virtual EstadoCobranza? IdEstadoNavigation { get; set; }
    }
}
