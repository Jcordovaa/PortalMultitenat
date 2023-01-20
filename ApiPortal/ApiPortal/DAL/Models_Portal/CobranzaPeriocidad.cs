using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class CobranzaPeriocidad
    {
        public CobranzaPeriocidad()
        {
            Automatizacions = new HashSet<Automatizacion>();
        }

        public int IdPeriocidad { get; set; }
        public string? Nombre { get; set; }
        public int? DiaMes { get; set; }
        public int? Estado { get; set; }

        public virtual ICollection<Automatizacion> Automatizacions { get; set; }
    }
}
