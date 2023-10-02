using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class AccesoImplementacion
    {
        public AccesoImplementacion()
        {
            PermisosImplementacions = new HashSet<PermisosImplementacion>();
        }

        public int IdAcceso { get; set; }
        public string? Nombre { get; set; }
        public int? MenuPadre { get; set; }
        public int? Activo { get; set; }

        public virtual ICollection<PermisosImplementacion> PermisosImplementacions { get; set; }
    }
}
