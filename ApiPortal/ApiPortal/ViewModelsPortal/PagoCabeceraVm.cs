namespace ApiPortal.ViewModelsPortal
{
    public class PagoCabeceraVm
    {
        public int? IdPago { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public float? MontoPago { get; set; }
        public string? ComprobanteContable { get; set; }
        public int? IdPagoEstado { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public int? IdPasarela { get; set; }
        public int? EsPagoRapido { get; set; }
        public virtual List<PagoDetalleVm>? PagosDetalle { get; set; }
        public virtual ClientesPortalVm? ClientesPortal { get; set; }
        public virtual PagosEstadoVm? PagosEstado { get; set; }
        public virtual ICollection<PasarelaPagoLogVm>? PasarelaPagoLog { get; set; }
    }
}
