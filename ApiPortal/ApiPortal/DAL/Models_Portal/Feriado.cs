using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Feriado
    {
        public int IdFeriado { get; set; }
        public string? Nombre { get; set; }
        public DateTime? Fecha { get; set; }
        public int? Anio { get; set; }
    }
}
