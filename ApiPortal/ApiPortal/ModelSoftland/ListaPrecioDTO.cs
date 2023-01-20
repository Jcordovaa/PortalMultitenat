namespace ApiPortal.ModelSoftland
{
    public class ListaPrecioDTO
    {
        public string CodLista { get; set; }
        public string DesLista { get; set; }
    }

    public class ListaPrecioAPIDTO
    {
        public string CodLista { get; set; }
        public string DesLista { get; set; }
        public string TipoLista { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
    }
}
