namespace ApiPortal.ModelSoftland
{
    public class DocumentoPendienteFacturar
    {
        public int NVNumero { get; set; }
        public DateTime nvFem { get; set; }
        public string nvEstado { get; set; }
        public decimal nvSubTotal { get; set; }
        public List<ImpuestosDocumentoPendiente> Impuestos { get; set; }
        public decimal nvMonto { get; set; }
        public DateTime nvFeEnt { get; set; }
        public decimal Facturado { get; set; }
        public decimal Pendiente_Facturar { get; set; }
        public string NomAux { get; set; }
        public string DirLugarDespNw { get; set; }
        public string DirAux { get; set; }
        public string CveDes { get; set; }
        public string VenDes { get; set; }
    }

    public class ImpuestosDocumentoPendiente
    {
        public string Codigo { get; set; }
        public double Monto { get; set; }
    }
}
