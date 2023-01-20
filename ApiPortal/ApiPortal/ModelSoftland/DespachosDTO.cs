namespace ApiPortal.ModelSoftland
{
    public class DespachosDTO
    {
        public string Documento { get; set; }
        public int Folio { get; set; }
        public DateTime Fecha { get; set; }
        public int NroInt { get; set; }
        public List<DespachoDetalleDTO> Detalle { get; set; }
        public string Tipo { get; set; }
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public int CantidadFacturada { get; set; }

    }

    public class DespachoDetalleDTO
    {
        public string CodProducto { get; set; }
        public string DesProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public int CantidadTotal { get; set; }
    }

    public class DespachoAPIDTO
    {
        public string Tipo { get; set; }
        public string Subtipo { get; set; }
        public double Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string CodProd { get; set; }
        public int CantDespachada { get; set; }
        public string DetProd { get; set; }
        public int TotLinea { get; set; }
        public string TipoDocto { get; set; }

    }
}
