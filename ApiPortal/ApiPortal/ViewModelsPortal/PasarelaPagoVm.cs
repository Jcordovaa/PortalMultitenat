namespace ApiPortal.ViewModelsPortal
{
    public class PasarelaPagoVm
    {
        public int IdPasarela { get; set; }
        public string? Nombre { get; set; }
        public string? Protocolo { get; set; }
        public string? Ambiente { get; set; }
        public string? TipoDocumento { get; set; }
        public string? CuentaContable { get; set; }
        public string? Logo { get; set; }
        public Nullable<int> Estado { get; set; }
        public string? MonedaPasarela { get; set; }
        public string? UsuarioSoftlandPay { get; set; }
        public string? ClaveSoftlandPay { get; set; }
        public string? EmpresaSoftlandPay { get; set; }
        public string? CodigoMedioPagoSoftlandPay { get; set; }
        public Nullable<int> ManejaAtributos { get; set; }
        public Nullable<int> ManejaAuxiliar { get; set; }
        public Nullable<int> EsProduccion { get; set; }
        public string? AmbienteConsultarPago { get; set; }
        public string? CodigoComercio { get; set; }
        public string? SecretKey { get; set; }
    }
}
