namespace ApiPortal.ModelSoftland
{
    public class ComunaDTO
    {
        public string Nombre { get; set; }
        public int? Estado { get; set; }
        public int? IdCiudad { get; set; }
        public int? IdRegion { get; set; }
        public string CodComunaSoftland { get; set; }
    }

    public class ComunaAPIDTO
    {
        public int? id_Region { get; set; }
        public string ComCod { get; set; }
        public string ComDes { get; set; }
    }
}
