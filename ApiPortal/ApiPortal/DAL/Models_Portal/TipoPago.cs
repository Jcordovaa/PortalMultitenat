using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class TipoPago
    {
        public int IdTipoPago { get; set; }
        public string? Nombre { get; set; }
        public int? Estado { get; set; }
        public string? TipoDocumento { get; set; }
        public string? CuentaContable { get; set; }
        public int? MuestraMonto { get; set; }
        public int? MuestraBanco { get; set; }
        public int? MuestraSerie { get; set; }
        public int? MuestraFecha { get; set; }
        public int? MuestraComprobante { get; set; }
        public int? MuestraCantidad { get; set; }
        public int? GeneraDte { get; set; }
        public DateTime? FechaMod { get; set; }
        public int? IdUsuarioMod { get; set; }
    }
}
