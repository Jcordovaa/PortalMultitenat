namespace ApiPortal.ViewModelsPortal
{
    public class ResultadoVpos
    {
        public string medio_pago { get; set; }
        public string id_transaccion { get; set; }
        public string id_merchant { get; set; }
        public string url_pago { get; set; }
    }

    public class ResultadoEstadoPagoVPOS
    {
        public string Medio_pago { get; set; }
        public string Id_interno { get; set; }
        public string Id_transaccion { get; set; }
        public string Monto_total { get; set; }
        public string Monto_bruto { get; set; }
        public string Monto_impuestos { get; set; }
        public string Estado { get; set; }
        public string Fecha { get; set; }
        public string Metodo_Pago { get; set; }
        public string Forma_pago { get; set; }
        public string Cuotas { get; set; }
    }
}
