using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionPortal
    {
        public int IdConfiguracionPortal { get; set; }
        public int? CrmSoftland { get; set; }
        public int? InventarioSoftland { get; set; }
        public int? ContabilidadSoftland { get; set; }
        public int? NotaVentaSoftland { get; set; }
        public int? CantidadUltimasCompras { get; set; }
        public int? MuestraEstadoBloqueo { get; set; }
        public int? MuestraEstadoSobregiro { get; set; }
        public int? MuestraContactoComercial { get; set; }
        public int? MuestraContactosPerfil { get; set; }
        public int? MuestraContactosEnvio { get; set; }
        public int? HabilitaPagoRapido { get; set; }
        public string? ImagenCaberaPerfil { get; set; }
        public string? ImagenCabeceraCompras { get; set; }
        public int? PermiteExportarExcel { get; set; }
        public int? PermiteAbonoParcial { get; set; }
        public int? UtilizaNumeroDireccion { get; set; }
        public int? CantUltPagosRec { get; set; }
        public int? CantUltPagosAnul { get; set; }
        public int? MuestraUltimasCompras { get; set; }
        public int? MuestraBotonImprimir { get; set; }
        public int? MuestraBotonEnviarCorreo { get; set; }
        public int? MuestraResumen { get; set; }
        public int? ResumenContabilizado { get; set; }
        public int? MuestraComprasFacturadas { get; set; }
        public int? MuestraPendientesFacturar { get; set; }
        public int? MuestraProductos { get; set; }
        public int? MuestraGuiasPendientes { get; set; }
        public int? UtilizaDocumentoPagoRapido { get; set; }
        public int? EstadoImplementacion { get; set; }
    }
}
