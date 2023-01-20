namespace ApiPortal.ModelSoftland
{
    public class ErrorCapturaComprobante
    {
        public string Mensaje { get; set; }
    }

    public class CapturaComprobanteResponse
    {
        public string respuesta { get; set; }
        public List<ComprobanteResponse> comprobante { get; set; }
        public List<ErrorCapturaComprobante> error { get; set; }
    }



    public class CapturaComprobanteResponseError
    {
        public string respuesta { get; set; }
        public string comprobante { get; set; }
        public List<ErrorCapturaComprobante> error { get; set; }
    }


    public class ComprobanteResponse
    {
        public string anno { get; set; }
        public string numero { get; set; }
    }
}
