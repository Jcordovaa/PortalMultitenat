using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class LogSoftlandpay
    {
        public int Id { get; set; }
        public string? Origen { get; set; }
        public string? IdTransaccion { get; set; }
        public string? MedioPago { get; set; }
        public string? Estado { get; set; }
        public string? MontoBruto { get; set; }
        public string? MontoTotal { get; set; }
        public string? MontoImpuestos { get; set; }
        public string? Fecha { get; set; }
        public string? IdInterno { get; set; }
        public string? Request { get; set; }
    }
}
