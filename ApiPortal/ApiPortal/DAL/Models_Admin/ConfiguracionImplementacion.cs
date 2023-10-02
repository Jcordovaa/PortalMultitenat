using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class ConfiguracionImplementacion
    {
        public string? DominioImplementacion { get; set; }
        public string? ApiSoftland { get; set; }
        public string? TokenApiSoftland { get; set; }
        public string? AppService { get; set; }
        public string? IpAppService { get; set; }
        public int IdConfiguracion { get; set; }
    }
}
