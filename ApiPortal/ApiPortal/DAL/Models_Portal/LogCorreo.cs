using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class LogCorreo
    {
        public int IdLogEmail { get; set; }
        public DateTime? Fecha { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public string? Error { get; set; }
        public int? TipoCorreo { get; set; }
    }
}
