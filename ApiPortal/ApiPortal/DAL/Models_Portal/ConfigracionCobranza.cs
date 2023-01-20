using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfigracionCobranza
    {
        public int IdConfigCobranza { get; set; }
        public string? TipoDocumentoCobranza { get; set; }
        public string? CondicionesCredito { get; set; }
        public int? EnviaCobranza { get; set; }
        public int? CantidadDiasVencimiento { get; set; }
        public int? IdFrecuenciaEnvioCob { get; set; }
        public int? EnviaPreCobranza { get; set; }
        public int? CantidadDiasPrevios { get; set; }
        public int? IdFrecuenciaEnvioPre { get; set; }
    }
}
