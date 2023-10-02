using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class PermisosImplementacion
    {
        public int IdPermiso { get; set; }
        public int? IdRol { get; set; }
        public int? IdAcceso { get; set; }
        public int? Modificar { get; set; }
        public int? Consultar { get; set; }
        public int? Actualizar { get; set; }
        public int? Insertar { get; set; }

        public virtual AccesoImplementacion? IdAccesoNavigation { get; set; }
        public virtual RolesPortal? IdRolNavigation { get; set; }
    }
}
