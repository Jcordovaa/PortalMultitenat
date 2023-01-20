using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class UsuariosPortal
    {
        public UsuariosPortal()
        {
            UsuariosPortalTokens = new HashSet<UsuariosPortalToken>();
        }

        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Avatar { get; set; }
        public int? IdRol { get; set; }
        public int IdImplementador { get; set; }
        public byte[]? ClaveHash { get; set; }
        public byte[]? ClaveSalt { get; set; }
        public int? Estado { get; set; }

        public virtual Implementador IdImplementadorNavigation { get; set; } = null!;
        public virtual RolesPortal? IdRolNavigation { get; set; }
        public virtual ICollection<UsuariosPortalToken> UsuariosPortalTokens { get; set; }
    }
}
