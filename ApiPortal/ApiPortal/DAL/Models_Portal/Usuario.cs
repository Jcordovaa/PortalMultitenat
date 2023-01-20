using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? Activo { get; set; }
        public int? IdPerfil { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaEnvioValidacion { get; set; }
        public int? CuentaActivada { get; set; }

        public virtual Perfil? IdPerfilNavigation { get; set; }
    }
}
