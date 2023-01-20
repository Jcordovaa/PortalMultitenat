namespace ApiPortal.ViewModelsPortal
{
    public class FiltroCobranzaVm
    {
        public string CodAuxCliente { get; set; }

        public string NombreCliente { get; set; }

        public string Estado { get; set; }
        public int Anio { get; set; }
        public Nullable<DateTime> Fecha { get; set; }
        public string TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public int IdUsuario { get; set; }
        public string NombreArchivo { get; set; }
        public string RutCliente { get; set; }
        public int TipoBusqueda { get; set; }
        public Nullable<DateTime> FechaHasta { get; set; }
        public string ComprobanteContable { get; set; }
        public string TipoDocumento { get; set; }
        public string CodigoTBK { get; set; }
        public int CantidadDias { get; set; }
        public int IdCobranza { get; set; }
        public string EmailCliente { get; set; }
        public int IdTipoCobranza { get; set; }
        public int TipoProgramacion { get; set; }
        public string NombreCobranza { get; set; }
        public int IdEstadoCobranza { get; set; }
        public int ExcluyeClientes { get; set; }
        public string ListasPrecio { get; set; }
        public string Vendedores { get; set; }
        public string CategoriasClientes { get; set; }
        public string CondicionesVenta { get; set; }
        public string CanalesVenta { get; set; }
        public string Cobradores { get; set; }
    }
}
