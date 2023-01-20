using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class UsuariosPortalToken
    {
        public int IdToken { get; set; }
        public int? IdUsuario { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public string? TokenRefresh { get; set; }
        public DateTime? TokenRefreshCreated { get; set; }
        public DateTime? TokenRefreshExpires { get; set; }

        public virtual UsuariosPortal? IdUsuarioNavigation { get; set; }
    }
}
