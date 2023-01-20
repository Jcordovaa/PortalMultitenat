using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class ClientesPortal
    {
        public ClientesPortal()
        {
            PagosCabeceras = new HashSet<PagosCabecera>();
        }

        public int IdCliente { get; set; }
        public string? Rut { get; set; }
        public string? CodAux { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public int? ActivaCuenta { get; set; }

        public virtual ICollection<PagosCabecera> PagosCabeceras { get; set; }
    }
}
