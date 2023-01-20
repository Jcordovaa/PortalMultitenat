namespace ApiPortal.ViewModelsPortal
{
    public class DocumentoClienteCobranzaVm
    {
        public Boolean Selected { get; set; }
        public int CantidadDocumentos { get; set; }
        public int MontoDeuda { get; set; }
        public List<DocumentosCobranzaVm> ListaDocumentos { get; set; }
        public string EmailCliente { get; set; }
        public string RutCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Bloqueado { get; set; }
    }
}
