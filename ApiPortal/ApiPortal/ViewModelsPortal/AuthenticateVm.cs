using System.ComponentModel.DataAnnotations;

namespace ApiPortal.ViewModelsPortal
{
    public class AuthenticateVm
    {
        public string? Rut { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Token { get; set; }
        public string? Nombre { get; set; }
        public string? CodAux { get; set; }
        public bool? EsUsuario { get; set; }
    }
}
