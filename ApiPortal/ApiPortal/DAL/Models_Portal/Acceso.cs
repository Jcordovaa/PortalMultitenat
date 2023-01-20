using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Acceso
    {
        public Acceso()
        {
            Permisos = new HashSet<Permiso>();
        }

        public int IdAcceso { get; set; }
        public string? Nombre { get; set; }
        public int? MenuPadre { get; set; }
        public int? Activo { get; set; }

        public virtual ICollection<Permiso> Permisos { get; set; }
    }
}
