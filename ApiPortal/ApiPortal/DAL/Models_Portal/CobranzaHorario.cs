using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class CobranzaHorario
    {
        public CobranzaHorario()
        {
            Automatizacions = new HashSet<Automatizacion>();
        }

        public int IdHorario { get; set; }
        public string? Horario { get; set; }
        public int? Hora { get; set; }
        public int? Minuto { get; set; }

        public virtual ICollection<Automatizacion> Automatizacions { get; set; }
    }
}
