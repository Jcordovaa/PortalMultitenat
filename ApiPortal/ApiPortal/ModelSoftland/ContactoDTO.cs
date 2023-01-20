namespace ApiPortal.ModelSoftland
{
    public class ContactoDTO
    {
        public string? NombreContacto { get; set; }
        public string? Correo { get; set; }
        public string? CargoContacto { get; set; }
        public string? CodCargo { get; set; }
        public string? Telefono { get; set; }
    }
    public class ContactoApiDTO
    {
        public string NomCon { get; set; }
        public string Email { get; set; }
        public string CarCon { get; set; }
        public string CarNom { get; set; }
    }
}
