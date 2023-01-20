using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class TipoEnvio
    {
        public int IdTipo { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }
    }
}
