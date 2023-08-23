namespace ApiPortal.ViewModelsPortal
{
    public class DetalleEnvioCobranzaVm
    {
        public string? RutCliente { get; set; }
        public string? NombreCliente { get; set; }
        public string? EmailCliente { get; set; }
        public int? CantidadDocumentosPendientes { get; set; }
        public decimal? MontoDeuda { get; set; }
        public List<DocumentosCobranzaVM>? ListaDocumentos { get; set; }
        public string? CodAux { get; set; }
        public int? IdCobranza { get; set; }
        public string? AutomatizacionJson { get; set; }
    }

    public class DocumentosCobranzaVM
    {
        public int Folio { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Monto { get; set; }
        public string TipoDocumento { get; set; }
    }
}
