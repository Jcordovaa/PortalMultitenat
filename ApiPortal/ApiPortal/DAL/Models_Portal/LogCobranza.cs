using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class LogCobranza
    {
        public int IdLogCobranza { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public string? Estado { get; set; }
        public string? CobranzasConsideradas { get; set; }
        public int? AnioProceso { get; set; }
    }
}
