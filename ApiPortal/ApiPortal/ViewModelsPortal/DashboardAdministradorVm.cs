namespace ApiPortal.ViewModelsPortal
{
    public class DashboardAdministradorVm
    {
        public int CantidadDocumentos { get; set; }
        public int MontoTotal { get; set; }
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public int MontoPago { get; set; }
        public string ComprobanteContable { get; set; }
        public int IdPagoEstado { get; set; }
        public int IdCliente { get; set; }
        public int Folio { get; set; }
        public bool Hoy { get; set; }
        public bool Semana { get; set; }
        public bool Mes { get; set; }
        public bool Anio { get; set; }
    }
}
