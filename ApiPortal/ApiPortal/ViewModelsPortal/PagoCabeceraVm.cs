namespace ApiPortal.ViewModelsPortal
{
    public class PagoCabeceraVm
    {
        public int IdPago { get; set; }
        public Nullable<int> IdCliente { get; set; }
        public Nullable<System.DateTime> FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public Nullable<float> MontoPago { get; set; }
        public string? ComprobanteContable { get; set; }
        public Nullable<int> IdPagoEstado { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public Nullable<int> IdPasarela { get; set; }
        public Nullable<int> EsPagoRapido { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public ClientesPortalVm? ClientesPortal { get; set; }

        public List<PagoDetalleVm>? PagosDetalle { get; set; }
        public List<PasarelaPagoLogVm>? PasarelaPagoLog { get; set; }
        public int TotalFilas { get; set; }
    }
}
