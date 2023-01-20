using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Perfil
    {
        public Perfil()
        {
            Permisos = new HashSet<Permiso>();
            Usuarios = new HashSet<Usuario>();
        }

        public int IdPerfil { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
