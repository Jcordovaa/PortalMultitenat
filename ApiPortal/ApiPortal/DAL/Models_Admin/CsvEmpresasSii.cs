using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Admin
{
    public partial class CsvEmpresasSii
    {
        public string Rut { get; set; } = null!;
        public string? RazonSocial { get; set; }
        public int? NroResolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? Mail { get; set; }
        public string? Url { get; set; }
        public DateTime? FechaCarga { get; set; }
    }
}
