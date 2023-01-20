namespace ApiPortal.ViewModelsPortal
{
    public class AutomatizacionVm
    {
        public int IdAutomatizacion { get; set; }
        public int IdTipoAutomatizacion { get; set; }
        public Nullable<int> Anio { get; set; }
        public string TipoDocumentos { get; set; }
        public Nullable<int> DiasVencimiento { get; set; }
        public Nullable<int> IdHorario { get; set; }
        public Nullable<int> IdPerioricidad { get; set; }
        public Nullable<int> ExcluyeFestivos { get; set; }
        public Nullable<int> ExcluyeClientes { get; set; }
        public string CodCategoriaCliente { get; set; }
        public string CodListaPrecios { get; set; }
        public string CodCondicionVenta { get; set; }
        public string CodVendedor { get; set; }
        public Nullable<int> MuestraSoloVencidos { get; set; }
        public Nullable<int> AgrupaCobranza { get; set; }
        public Nullable<int> Estado { get; set; }
        public string Nombre { get; set; }
        public Nullable<int> DiasRecordatorio { get; set; }
        public Nullable<int> DiaEnvio { get; set; }
        public string CodCanalVenta { get; set; }
        public string CodCobrador { get; set; }
        public string CodCargo { get; set; }
        public int EnviaTodosContactos { get; set; }
        public int EnviaCorreoFicha { get; set; }
    }
}
