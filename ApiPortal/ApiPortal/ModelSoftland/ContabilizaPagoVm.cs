namespace ApiPortal.ModelSoftland
{
    public class ContabilizaPagoVm
    {
        public string cuentaContable { get; set; }
        public string tipoDocumento { get; set; }
        public DateTime fechaContabilizacion { get; set; }
        public string NumDoc { get; set; }
        public string areaNegocio { get; set; }
        public string tipoInterno { get; set; }
        public string numeroInterno { get; set; }
        public string glosaEncabezado { get; set; }
        public List<DetalleDocumento> DetalleDocumento { get; set; }

    }

    public class DetalleDocumento
    {
        public string tipoDocumento { get; set; }
        public string folioDocumento { get; set; }
        public double montoPagado { get; set; }
        public string glosaMovimiento { get; set; }
    }
}
