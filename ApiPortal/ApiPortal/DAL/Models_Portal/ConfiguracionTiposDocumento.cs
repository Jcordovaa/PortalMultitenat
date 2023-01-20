using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ConfiguracionTiposDocumento
    {
        public int IdTipoDocConfig { get; set; }
        public int? IdConfiguracion { get; set; }
        public string? Nombre { get; set; }
        public string? CodErp { get; set; }
    }
}
