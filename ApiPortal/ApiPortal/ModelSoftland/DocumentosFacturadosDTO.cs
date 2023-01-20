namespace ApiPortal.ModelSoftland
{
    public class DocumentosFacturadosDTO
    {
        public string Movtipdocref { get; set; }
        public string Documento { get; set; }
        public decimal Nro { get; set; }
        public DateTime Femision { get; set; }
        public DateTime Fvencimiento { get; set; }
        public decimal Monto { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; }
        public string NotaVenta { get; set; }
        public decimal APagar { get; set; }
        public string CodMon { get; set; }
        public string DesMon { get; set; }
        public double MontoMonedaOriginal { get; set; }
    }


    public class DocumentosFacturadosAPIDTO
    {
        public double Folio { get; set; }
        public string Fecha { get; set; }
        public string Vncimiento { get; set; }
        public double Monto { get; set; }
        public string Estado { get; set; }
        public int Nvnumero { get; set; }
        public string Tipo_Documento { get; set; }
        public string CodMon { get; set; }
        public string DesMon { get; set; }
        public double monto_moneda_original { get; set; }
        public string DocDes { get; set; }
    }
}
