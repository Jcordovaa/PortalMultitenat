namespace ApiPortal.ViewModelsPortal
{
    public class DocumentosCobranzaVm
    {
        public int FolioDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public string CodTipoDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string RutCliente { get; set; }
        public string NombreCliente { get; set; }
        public int DiasAtraso { get; set; }
        public string Estado { get; set; }
        public string CuentaContable { get; set; }
        public string NombreCuenta { get; set; }
        public decimal MontoDocumento { get; set; }
        public decimal SaldoDocumento { get; set; }
        public string Bloqueado { get; set; }
        public string EmailCliente { get; set; }
    }
}
