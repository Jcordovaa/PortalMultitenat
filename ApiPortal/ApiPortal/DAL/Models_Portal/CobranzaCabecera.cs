using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class CobranzaCabecera
    {
        public CobranzaCabecera()
        {
            CobranzaDetalles = new HashSet<CobranzaDetalle>();
        }

        public int IdCobranza { get; set; }
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
        public int? IdPeriodicidad { get; set; }
        public int? ExcluyeFeriado { get; set; }
        public int? ExcluyeFinDeSemana { get; set; }
        public int? DiaEnvio { get; set; }
        public int? DesdeMontoDeuda { get; set; }
        public int? HastaMontoDeuda { get; set; }
        public int? CantidadDocumentos { get; set; }
        public int? EjecutaSiguienteHabil { get; set; }
        public string? ListaPrecio { get; set; }
        public string? CategoriaCliente { get; set; }
        public string? CondicionVenta { get; set; }
        public string? Vendedor { get; set; }
        public string? CargosContactos { get; set; }
        public int? EnviaTodosContactos { get; set; }
        public int? EnviaCorreoFicha { get; set; }
        public int? EnviaTodosCargos { get; set; }
        public string? CanalesVenta { get; set; }
        public string? Cobradores { get; set; }

        public virtual EstadoCobranza? IdEstadoNavigation { get; set; }
        public virtual TipoCobranza? IdTipoCobranzaNavigation { get; set; }
        public virtual ICollection<CobranzaDetalle> CobranzaDetalles { get; set; }
    }
}
