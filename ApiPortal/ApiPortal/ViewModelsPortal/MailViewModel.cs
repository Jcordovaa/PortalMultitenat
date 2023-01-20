using System.Net.Mail;

namespace ApiPortal.ViewModelsPortal
{
    public class MailViewModel
    {
        public int tipo { get; set; }
        public string nombre { get; set; }
        public string asunto { get; set; }
        public string mensaje { get; set; }
        public string email_destinatario { get; set; }
        public string rutaArchivo { get; set; }
        public int tipoEnvioDoc { get; set; }
        public int folio { get; set; }
        public string codAux { get; set; }
        public string tipoDoc { get; set; }
        public List<Attachment> adjuntos { get; set; }
    }
}
