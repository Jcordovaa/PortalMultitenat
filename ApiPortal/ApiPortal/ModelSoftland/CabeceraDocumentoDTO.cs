namespace ApiPortal.ModelSoftland
{
    public class DocumentoDTO
    {
        public CabeceraDocumentoDTO cabecera { get; set; }
        public List<DetalleDocumentoDTO> detalle { get; set; }
    }

    public class CabeceraDocumentoDTO
    {
        public int Folio { get; set; }
        public string Rut { get; set; }
        public string RazonSocial { get; set; }
        public string Giro { get; set; }
        public string Ciudad { get; set; }
        public string Comuna { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Telefono { get; set; }
        public string CondVenta { get; set; }
        public string Vendedor { get; set; }
        public string Patente { get; set; }
        public float Neto { get; set; }
        public float Descuento { get; set; }
        public float Iva { get; set; }
        public float Total { get; set; }
        public string Tipo { get; set; }
        public string nvnumero { get; set; }
        public string SubTipo { get; set; }
        public string Estado { get; set; }
    }

    public class DetalleDocumentoDTO
    {
        public string Codigo { get; set; }
        public string DesProd { get; set; }
        public int Cantidad { get; set; }
        public string CodUmed { get; set; }
        public float PrecioUnitario { get; set; }
        public float Descuento { get; set; }
        public float Total { get; set; }
        public int CantidadDespachada { get; set; }
    }

    public class EnvioDocumento
    {
        public string destinatarios { get; set; }
        public string codAux { get; set; }
        public string tipoDoc { get; set; }
        public string subTipoDoc { get; set; }
        public int folio { get; set; }
        public int TipoDocAEnviar { get; set; }
        public int docsAEnviar { get; set; }
    }
}
