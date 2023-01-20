using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class RegistroEnvioCorreo
    {
        public int IdRegistro { get; set; }
        public int? IdTipoEnvio { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public string? HoraEnvio { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdCliente { get; set; }
    }
}
