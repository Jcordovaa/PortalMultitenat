namespace ApiPortal.ModelSoftland
{
    public class NotaVentaClienteDTO
    {
        public string? NroDocumento { get; set; }
        public string? NVNumero { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? Estado { get; set; }
        public int? Facturada { get; set; }
        public int? Despachada { get; set; }
        public string? NumOC { get; set; }
        public string? CodAux { get; set; }
        public string? Cliente { get; set; }
        public string? VenCod { get; set; }
        public string? Vendedor { get; set; }
        public string? CodLista { get; set; }
        public string? Lista { get; set; }
        public decimal? Monto { get; set; }
        public string? Direccion { get; set; }
        public string? CondicionVenta { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Impuestos { get; set; }
        public decimal? MontoFacturado { get; set; }
        public decimal? MontoPendiente { get; set; }
        public List<NotaVentaDetalleDTO>? detalle { get; set; }
    }

    public class NotaVentaDetalleDTO
    {
        public string? CodProducto { get; set; }
        public string? DesProducto { get; set; }
        public int? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Total { get; set; }
    }

    public class NotaVentaDetalleAPIDTO
    {
        public int NVNumero { get; set; }
        public double nvLinea { get; set; }
        public string CodProd { get; set; }
        public double nvCant { get; set; }
        public double nvPrecio { get; set; }
        public double TotalLinea { get; set; }
        public double nvTotDesc { get; set; }
        public string DetProd { get; set; }
        public string CodUMed { get; set; }
        public string DesUMed { get; set; }
    }
}
