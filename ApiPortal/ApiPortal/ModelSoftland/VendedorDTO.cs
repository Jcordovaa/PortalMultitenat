namespace ApiPortal.ModelSoftland
{
    public class VendedorDTO
    {
        public string VenCod { get; set; }
        public string VenDes { get; set; }
        public string CodTipV { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
    }

    public class VendedorAPIDTO
    {
        public string codi { get; set; }
        public string descripcion { get; set; }
        public string email { get; set; }
    }
}
