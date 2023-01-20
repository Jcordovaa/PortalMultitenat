namespace ApiPortal.ViewModelsPortal
{
    public class PasarelaPagoVm
    {
        public int IdPasarela { get; set; }
        public string Nombre { get; set; }
        public string CodigoComercioTBK { get; set; }
        public string ApiKeyPasarela { get; set; }
        public string SecretKeyPasarela { get; set; }
        public string Protocolo { get; set; }
        public string Ambiente { get; set; }
        public string TipoDocumento { get; set; }
        public string CuentaContable { get; set; }
        public string Logo { get; set; }
        public int Estado { get; set; }
        public string MonedaPasarela { get; set; }
    }
}
