namespace ApiPortal.ViewModelsPortal
{
    public class PagoDetalleVm
    {
        public int? IdPagoDetalle { get; set; }
        public int? IdPago { get; set; }
        public int? Folio { get; set; }
        public string? TipoDocumento { get; set; }
        public string? CuentaContableDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public float? Total { get; set; }
        public float? Saldo { get; set; }
        public float? APagar { get; set; }
    }
}
