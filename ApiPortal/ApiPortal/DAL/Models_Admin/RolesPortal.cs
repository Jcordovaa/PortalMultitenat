using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class RolesPortal
    {
        public RolesPortal()
        {
            PermisosImplementacions = new HashSet<PermisosImplementacion>();
            UsuariosPortals = new HashSet<UsuariosPortal>();
        }

        public int IdRol { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<PermisosImplementacion> PermisosImplementacions { get; set; }
        public virtual ICollection<UsuariosPortal> UsuariosPortals { get; set; }
    }
}
