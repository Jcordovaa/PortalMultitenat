using ApiPortal.ModelSoftland;

namespace ApiPortal.ViewModelsPortal
{
    public class CobranzaDetalleVm
    {
        public int? IdCobranzaDetalle { get; set; }
        public int? IdCobranza { get; set; }
        public int? Folio { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public float? Monto { get; set; }
        public string? RutCliente { get; set; }
        public string? CodAuxCliente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? CodTipoDocumento { get; set; }
        public int? IdEstado { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? HoraEnvio { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public string? ComprobanteContable { get; set; }
        public string? FolioDTE { get; set; }
        public int? IdPago { get; set; }
        public string? CuentaContable { get; set; }
        public string? EmailCliente { get; set; }
        public Boolean? selected { get; set; }
        public string? NombreCliente { get; set; }
        public string? FechaPagoTexto { get; set; }
        public string? NombreEstado { get; set; }
        public List<DocumentoContabilizadoAPIDTO>? Abonos { get; set; }
    }
}
