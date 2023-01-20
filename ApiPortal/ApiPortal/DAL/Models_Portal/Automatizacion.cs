using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Automatizacion
    {
        public int IdAutomatizacion { get; set; }
        public int IdTipoAutomatizacion { get; set; }
        public int? Anio { get; set; }
        public string? TipoDocumentos { get; set; }
        public int? DiasVencimiento { get; set; }
        public int? IdHorario { get; set; }
        public int? IdPerioricidad { get; set; }
        public int? ExcluyeFestivos { get; set; }
        public int? ExcluyeClientes { get; set; }
        public string? CodCategoriaCliente { get; set; }
        public string? CodListaPrecios { get; set; }
        public string? CodCondicionVenta { get; set; }
        public string? CodVendedor { get; set; }
        public int? MuestraSoloVencidos { get; set; }
        public int? AgrupaCobranza { get; set; }
        public int? Estado { get; set; }
        public int? DiasRecordatorio { get; set; }
        public int? DiaEnvio { get; set; }
        public string? CodCanalVenta { get; set; }
        public string? CodCobrador { get; set; }
        public string? CodCargo { get; set; }
        public int? EnviaTodosContactos { get; set; }
        public int? EnviaCorreoFicha { get; set; }

        public virtual CobranzaHorario? IdHorarioNavigation { get; set; }
        public virtual CobranzaPeriocidad? IdPerioricidadNavigation { get; set; }
        public virtual TipoAutomatizacion IdTipoAutomatizacionNavigation { get; set; } = null!;
    }
}
