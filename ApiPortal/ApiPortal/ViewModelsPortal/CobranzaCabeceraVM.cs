using ApiPortal.ModelSoftland;

namespace ApiPortal.ViewModelsPortal
{
    public class CobranzaCabeceraVM
    {
        public int? IdCobranza { get; set; }
        public string? Nombre { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string? HoraCreacion { get; set; }
        public int? IdTipoCobranza { get; set; }
        public int? Estado { get; set; }
        public int? IdUsuario { get; set; }
        public int? TipoProgramacion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? HoraDeEnvio { get; set; }
        public string? DiaSemanaEnvio { get; set; }
        public int? DiasToleranciaVencimiento { get; set; }
        public int? IdEstado { get; set; }
        public int? Anio { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? AplicaClientesExcluidos { get; set; }
        public int? EsCabeceraInteligente { get; set; }
        public int? IdCabecera { get; set; }
        public int? EnviaEnlacePago { get; set; }
        public int? EstaVencida { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreEstado { get; set; }
        public string? NombreTipoCobranza { get; set; }
        public float? TotalRecaudar { get; set; }
        public float? TotalRecaudado { get; set; }
        public float? PorcentajeRecaudacion { get; set; }
        public int? CantidadDocumentosEnviadosCobrar { get; set; }
        public int? CantidadDocumentosPagados { get; set; }
        public float? PorcentajePagoDocumentos { get; set; }
        public string? ColorPorcentajeRecaudacion { get; set; }
        public string? Imagen { get; set; }
        public string? HoraEnvioTexto { get; set; }
        public int? IdPeriodicidad { get; set; } //FCA 07-12-2021
        public int? ExcluyeFeriados { get; set; } //FCA 09-12-2021
        public int? ExcluyeFinDeSemana { get; set; } //FCA 09-12-2021
        public int? DiaEnvio { get; set; } //FCA 09-12-2021
        public string? ListaPrecio { get; set; }
        public string? CategoriaCliente { get; set; }
        public string? CondicionVenta { get; set; }
        public string? Vendedor { get; set; }
        public string? CargosContactos { get; set; }
        public Nullable<int> EnviaCorreoFicha { get; set; }
        public Nullable<int> EnviaTodosContactos { get; set; }
        public Nullable<int> EnviaTodosCargos { get; set; }
        public string? canalesVenta { get; set; }
        public string? cobradores { get; set; }

        public List<CobranzaDetalleVm>? CobranzaDetalle { get; set; }
    }
}
