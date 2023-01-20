using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class LogProceso
    {
        public int IdTipoProceso { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? Ruta { get; set; }
        public string? Mensaje { get; set; }
        public string? Excepcion { get; set; }
    }
}
