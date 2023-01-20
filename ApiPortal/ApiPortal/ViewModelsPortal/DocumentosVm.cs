namespace ApiPortal.ViewModelsPortal
{
    public class DocumentosVm
    {
        public string NombreArchivo { get; set; }
        public string Base64 { get; set; }
        public string Tipo { get; set; }
        public Stream documento { get; set; }
        public string RutaDocumento { get; set; }
    }
}
