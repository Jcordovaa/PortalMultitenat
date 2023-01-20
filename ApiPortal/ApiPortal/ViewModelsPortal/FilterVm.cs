namespace ApiPortal.ViewModelsPortal
{
    public class FilterVm
    {
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Nombre { get; set; }
        public int? Folio { get; set; }
        public string? TipoDoc { get; set; }
        public string? correos { get; set; }
        public int? TipoEnvioDoc { get; set; }
        //Para Envio de accesos
        //1 Todos los clientes
        //2 solo los que no tienen acceso creado
        public int? TipoBusqueda { get; set; }

        public DateTime? fechaDesde { get; set; }
        public DateTime? fechaHasta { get; set; }
        public string? ListaPrecio { get; set; }
        public string? CategoriaCliente { get; set; }
        public string? Vendedor { get; set; }
        public string? CondicionVenta { get; set; }
        public string? TipoAutomatización { get; set; }
        public int? IdCobranza { get; set; }
    }
}
