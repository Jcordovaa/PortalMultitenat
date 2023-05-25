using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class LogApiDetalle
    {
        public int Id { get; set; }
        public string? IdLogApi { get; set; }
        public string? Metodo { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public int? Segundos { get; set; }
    }
}
