namespace ApiPortal.ViewModelsPortal
{
    public class PasarelaPagoLogVm
    {
        public int? Id { get; set; }
        public int? IdPago { get; set; }
        public int? IdPasarela { get; set; }
        public System.DateTime? Fecha { get; set; }
        public decimal? Monto { get; set; }
        public string? Token { get; set; }
        public string? Codigo { get; set; }
        public string? Estado { get; set; }
        public string? OrdenCompra { get; set; }
        public string? MedioPago { get; set; }
        public int? Cuotas { get; set; }
        public string? Tarjeta { get; set; }
        public string? Url { get; set; }
        public string? ArchivoComprobante64 { get; set; }
        public string? TipoArchivo { get; set; }
        public string? ComprobanteContable { get; set; }
        public string? PasarelaPago { get; set; }
    }
}
