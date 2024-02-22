using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class ServidoresImplementacion
    {
        public int IdServidorImplementacion { get; set; }
        public string? NombreServidor { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public int? Estado { get; set; }
    }
}
