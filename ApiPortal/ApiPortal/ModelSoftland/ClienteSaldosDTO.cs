namespace ApiPortal.ModelSoftland
{
    public class ClienteSaldosDTO
    {
        public string? Documento { get; set; }
        public string? CodAux { get; set; }
        public double? Nro { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVcto { get; set; }
        public double? Debe { get; set; }
        public double? Haber { get; set; }
        public double? Saldo { get; set; }
        public string? Detalle { get; set; }
        public string? Estado { get; set; }
        public string? Pago { get; set; }
        public string? TipoDoc { get; set; }
        public string? RazonSocial { get; set; }
        public string? ComprobanteContable { get; set; }
        public string? CuentaContable { get; set; }
        public double? APagar { get; set; }
        public double? MontoBase { get; set; }
        public double? SaldoBase { get; set; }
        public string? CodigoMoneda { get; set; }
        public double? EquivalenciaMoneda { get; set; }
        public string? DesMon { get; set; }
        public double? MontoOriginalBase { get; set; }
        public bool? BloqueadoPago { get; set; }
        public double? MovEqui { get; set; }
        public List<ClienteSaldosDTO>? Abonos { get; set; }
        public string? IdPaginador { get; set; }
        public int? TotalFilas { get; set; }
    }
}
