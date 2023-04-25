namespace ApiPortal.ModelSoftland
{
    public class ProductoDTO
    {
        public string CodProd { get; set; }
        public string DesProd { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal TotalLinea { get; set; }
        public string Documento { get; set; }
        public double Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoDoc { get; set; }
    }

    public class ProductoAPIDTO
    {
        public string DesDoc { get; set; }
        public double Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string CodProd { get; set; }
        public string DetProd { get; set; }
        public double CantFacturada { get; set; }
        public double PreUniMB { get; set; }
        public double TotalLinea { get; set; }
        public string Tipo_Documento { get; set; }
    }
}
