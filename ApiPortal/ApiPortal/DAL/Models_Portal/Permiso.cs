using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Permiso
    {
        public int IdPermiso { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdAcceso { get; set; }
        public int? Modificar { get; set; }
        public int? Consultar { get; set; }
        public int? Actualizar { get; set; }
        public int? Insertar { get; set; }

        public virtual Acceso? IdAccesoNavigation { get; set; }
        public virtual Perfil? IdPerfilNavigation { get; set; }
    }
}
