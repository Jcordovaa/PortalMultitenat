using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ApiSoftland
    {
        public int Id { get; set; }
        public string? Ambiente { get; set; }
        public string? Url { get; set; }
        public string? Token { get; set; }
        public string? AreaDatos { get; set; }
        public string? ConsultaTiposDeDocumentos { get; set; }
        public string? ConsultaPlanDeCuentas { get; set; }
        public string? ConsultaRegiones { get; set; }
        public string? ConsultaComunas { get; set; }
        public string? ConsultaGiros { get; set; }
        public string? ContactosXauxiliar { get; set; }
        public string? ConsultaCliente { get; set; }
        public string? ActualizaCliente { get; set; }
        public string? ResumenContable { get; set; }
        public string? CapturaComprobantes { get; set; }
        public string? DocumentosFacturados { get; set; }
        public string? DetalleDocumento { get; set; }
        public string? ObtenerPdfDocumento { get; set; }
        public string? DespachoDeDocumento { get; set; }
        public string? ProductosComprados { get; set; }
        public string? PendientesPorFacturar { get; set; }
        public string? DetalleNotaDeVenta { get; set; }
        public string? ObtenerPdf { get; set; }
        public string? ObtieneGuiasPendientes { get; set; }
        public string? Login { get; set; }
        public string? ObtieneVendedores { get; set; }
        public string? ObtieneCondicionesVenta { get; set; }
        public string? ObtieneListasPrecio { get; set; }
        public string? ObtieneCategoriaClientes { get; set; }
        public string? DocumentosContabilizados { get; set; }
        public string? ObtieneModulos { get; set; }
        public string? ConsultaMonedas { get; set; }
        public string? ContabilizaPagos { get; set; }
        public string? ConsultaCargos { get; set; }
        public string? DocumentosContabilizadosResumen { get; set; }
        public string? TopDeudores { get; set; }
        public string? TransbankRegistrarCliente { get; set; }
        public string? DocContabilizadosResumenxRut { get; set; }
        public string? PagosxDocumento { get; set; }
        public int? HabilitaLogApi { get; set; }
        public string? CadenaAlmacenamientoAzure { get; set; }
        public string? UrlAlmacenamientoArchivos { get; set; }
    }
}
