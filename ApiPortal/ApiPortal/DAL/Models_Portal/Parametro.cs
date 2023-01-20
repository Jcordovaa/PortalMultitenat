using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class Parametro
    {
        public int IdParametro { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Valor { get; set; }
        public int? Estado { get; set; }
    }
}
