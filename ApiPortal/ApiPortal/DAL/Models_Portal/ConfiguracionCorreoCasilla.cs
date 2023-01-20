using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionCorreoCasilla
    {
        public int IdCasilla { get; set; }
        public string? Casilla { get; set; }
        public int? CantidadDia { get; set; }
    }
}
