using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class TipoAutomatizacion
    {
        public TipoAutomatizacion()
        {
            Automatizacions = new HashSet<Automatizacion>();
        }

        public int IdTipo { get; set; }
        public string? Nombre { get; set; }
        public int? EsPosterior { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<Automatizacion> Automatizacions { get; set; }
    }
}
