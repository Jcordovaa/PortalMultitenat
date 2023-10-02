namespace ApiPortal.ViewModelsAdmin
{
    public class PaginadorImplementacionVm
    {
        public int? StartRow { get; set; }
        public int? EndRow { get; set; }
        public string? SortBy { get; set; }
        public int? Cantidad { get; set; }
        public string? Search { get; set; }
        public int? implementador { get; set; }
        public string? RutEmpresa { get; set; }
        public int? Pagina { get; set; }
        public int? Estado { get; set; }
    }
}
