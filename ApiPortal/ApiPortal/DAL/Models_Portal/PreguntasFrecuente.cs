using System;
using System.Collections.Generic;

namespace ApiPortal.Dal.Models_Portal
{
    public partial class PreguntasFrecuente
    {
        public int IdPregunta { get; set; }
        public string? Pregunta { get; set; }
        public string? Respuesta { get; set; }
        public int? Estado { get; set; }
    }
}
