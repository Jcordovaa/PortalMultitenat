namespace ApiPortal.ModelSoftland
{
    public class RegionDTO
    {
        public int IdRegion { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
    }

    public class RegionAPIDTO
    {
        public int id_Region { get; set; }
        public string Descripcion { get; set; }
    }
}
