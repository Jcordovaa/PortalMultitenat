using ApiPortal.ModelSoftland;

namespace ApiPortal.ViewModelsPortal
{
    public class EnvioAccesoClienteVm
    {
        public List<ClienteAPIDTO> value { get; set; }
        public int EnviaTodos { get; set; }
        public List<ClienteAPIDTO> Eliminados { get; set; }
        public string? ListaPrecio { get; set; }
        public string? CategoriaCliente { get; set; }
        public string? Vendedor { get; set; }
        public string? CondicionVenta { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Nombre { get; set; }
    }
}
