using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class PasarelaPagoLog
    {
        public int Id { get; set; }
        public int? IdPago { get; set; }
        public int? IdPasarela { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? Monto { get; set; }
        public string? Token { get; set; }
        public string? Codigo { get; set; }
        public string? Estado { get; set; }
        public string? OrdenCompra { get; set; }
        public string? MedioPago { get; set; }
        public int? Cuotas { get; set; }
        public string? Tarjeta { get; set; }
        public string? Url { get; set; }

        public virtual PagosCabecera? IdPagoNavigation { get; set; }
        public virtual PasarelaPago? IdPasarelaNavigation { get; set; }
    }
}
