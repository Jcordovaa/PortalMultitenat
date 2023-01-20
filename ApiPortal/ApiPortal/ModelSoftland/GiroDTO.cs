namespace ApiPortal.ModelSoftland
{
    public class GiroDTO
    {
        public string IdGiro { get; set; }
        public string Nombre { get; set; }
        public int Estado { get; set; }
    }

    public class GiroAPIDTO
    {
        public string GirCod { get; set; }
        public string GirDes { get; set; }
    }
}
