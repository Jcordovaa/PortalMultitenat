namespace ApiPortal.ViewModelsPortal
{
    public class PaginadorVm
    {
        public int? StartRow { get; set; }
        public int? EndRow { get; set; }
        public string? SortBy { get; set; }
        public string? Search { get; set; }
        public string? CondicionVenta { get; set; }
        public string? CategoriaCliente { get; set; }
        public string? ListaPrecio { get; set; }
        public string? TiposDocumentos { get; set; }
        public string? Vendedor { get; set; }
        public int? Tipo { get; set; }
    }
}
