using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class PagosCabecera
    {
        public PagosCabecera()
        {
            PagosDetalles = new HashSet<PagosDetalle>();
            PasarelaPagoLogs = new HashSet<PasarelaPagoLog>();
        }

        public int IdPago { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public float? MontoPago { get; set; }
        public string? ComprobanteContable { get; set; }
        public int? IdPagoEstado { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public int? IdPasarela { get; set; }
        public int? EsPagoRapido { get; set; }

        public virtual ClientesPortal? IdClienteNavigation { get; set; }
        public virtual PagosEstado? IdPagoEstadoNavigation { get; set; }
        public virtual ICollection<PagosDetalle> PagosDetalles { get; set; }
        public virtual ICollection<PasarelaPagoLog> PasarelaPagoLogs { get; set; }
    }
}
