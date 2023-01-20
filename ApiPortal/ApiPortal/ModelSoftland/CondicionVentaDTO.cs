namespace ApiPortal.ModelSoftland
{
    public class CondicionVentaDTO
    {
        public string CveCod { get; set; }
        public string CveDes { get; set; }
        public int CveDias { get; set; }
        public int cveNvCto { get; set; }
    }

    public class CondicionVentaAPIDTO
    {
        public string codi { get; set; }
        public string descripcion { get; set; }
        public int dias { get; set; }
    }
}
