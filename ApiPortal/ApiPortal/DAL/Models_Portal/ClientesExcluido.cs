using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ClientesExcluido
    {
        public int IdExcluido { get; set; }
        public string? RutCliente { get; set; }
        public string? CodAuxCliente { get; set; }
        public string? NombreCliente { get; set; }
    }
}
