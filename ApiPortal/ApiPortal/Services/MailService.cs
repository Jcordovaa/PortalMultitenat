using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsPortal;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace ApiPortal.Services
{
    public class MailService
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public MailService(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task EnviarCorreosAsync(MailViewModel model)
        {
            try
            {
                var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string logo = auxEmpresa.UrlPortal + "/" + auxEmpresa.Logo;
                string cc = string.Empty;
                List<DocumentosVm> adjuntos = new List<DocumentosVm>();

                string body = string.Empty;
                switch (model.tipo)
                {
                    case 1:

                        break;
                    case 2:
                        model.asunto = auxCorreo.AsuntoPagoCliente;
                        body = "Notificacion pago Cliente";
                        break;
                    case 3:
                        model.asunto = auxCorreo.AsuntoPagoCliente;
                        body = "Notificacion pago Empresa";
                        break;
                    case 4:

                        model.asunto = auxCorreo.AsuntoEnvioDocumentos;
                        body = this.PopulateBodyEnvioDocumentos(auxCorreo.TextoEnvioDocumentos, auxEmpresa.NombreEmpresa, auxCorreo.TituloEnvioDocumentos, logo);
                        break;

                    case 5:
                        model.asunto = auxCorreo.AsuntoPagoCliente;
                        body = await this.PopulateBodyEnvioComprobantePago(auxCorreo.TituloPagoCliente, auxCorreo.TextoPagoCliente, auxEmpresa.NombreEmpresa, logo);
                        break;

                    case 6:
                        model.asunto = auxCorreo.AsuntoAvisoPagoCliente;
                        body = await this.PopulateBodyAvisoPago(model.mensaje, auxCorreo.TituloAvisoPagoCliente, auxCorreo.TextoAvisoPagoCliente, auxEmpresa.NombreEmpresa, logo);
                        break;
                    case 7:
                        model.asunto = auxCorreo.AsuntoCambioDatos;
                        body = this.PopulateBodyEnvioDocumentos(auxCorreo.TextoCambioDatos, auxEmpresa.NombreEmpresa, auxCorreo.TituloCambioDatos, logo);
                        break;
                    case 8:
                        model.asunto = auxCorreo.AsuntoPagoSinComprobante;
                        body = await this.PopulateBodyPagoSinComprobante(auxCorreo.TituloPagoSinComprobante, auxCorreo.TextoPagoSinComprobante, auxEmpresa.NombreEmpresa, logo, model.mensaje);
                        break;
                }

                await this.SendHtmlFormattedEmail(model.email_destinatario, model.asunto, body, cc, adjuntos, auxCorreo, model.adjuntos);

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "MailController"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                throw;                
            }
        }
        
        private async Task SendHtmlFormattedEmail(string destinatario, string asunto, string body, string copia, List<DocumentosVm> adjuntos, ConfiguracionCorreo auxCorreo, List<Attachment> attachments)
        {

            HashPassword aux = new HashPassword();

            string server = auxCorreo.SmtpServer;
            string user = auxCorreo.Usuario;
            string pass = auxCorreo.Clave;
            string port = auxCorreo.Puerto.ToString();
            Boolean ssl = auxCorreo.Ssl == 1 ? true : false;

            using (MailMessage mailMessage = new MailMessage())
            {
                if (!String.IsNullOrEmpty(copia))
                {
                    string[] copias = copia.Split(';');

                    foreach (var item in copias)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            mailMessage.CC.Add(item);
                        }
                    }
                }

                foreach (var item in adjuntos)
                {
                    Attachment documento = new Attachment(item.documento, item.NombreArchivo);
                    mailMessage.Attachments.Add(documento);
                }

                if (destinatario.Contains(";"))
                {
                    string[] destinatarios = destinatario.Split(';');
                    foreach (var item in destinatarios)
                    {
                        if (!string.IsNullOrEmpty(item) && item.Trim() != ";")
                        {
                            mailMessage.To.Add(new MailAddress(item));
                        }
                    }
                }
                else
                {
                    mailMessage.To.Add(new MailAddress(destinatario));
                }

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        mailMessage.Attachments.Add(item);
                    }
                }


                mailMessage.From = new MailAddress(auxCorreo.CorreoOrigen, auxCorreo.NombreCorreos);
                mailMessage.Subject = asunto;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = server;
                smtp.EnableSsl = Convert.ToBoolean(ssl);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = user;
                NetworkCred.Password = Encrypt.Base64Decode(pass);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(port);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                await smtp.SendMailAsync(mailMessage);
            }
        }

        private string PopulateBodyEnvioDocumentos(string mensaje, string empresa, string titulo, string logo)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioDocumentos.component.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{TextoCorreo}", mensaje);
            body = body.Replace("{NombreEmpresa}", empresa);
            body = body.Replace("{TituloCorreo}", titulo);
            body = body.Replace("{LOGO}", logo);
            return body;
        }

        private async Task<string> PopulateBodyEnvioComprobantePago(string titulo, string mensaje, string empresa, string logo)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/envioDocumentos.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{TextoCorreo}", mensaje);
            body = body.Replace("{TituloCorreo}", titulo);
            body = body.Replace("{NombreEmpresa}", empresa);
            body = body.Replace("{LOGO}", logo);
            return body;
        }

        private async Task<string> PopulateBodyAvisoPago(string informacion, string titulo, string mensaje, string empresa, string logo)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/avisoVenta.component.html")))
            {
                body = reader.ReadToEnd();
            }
            string[] datos = informacion.Split('|');
            body = body.Replace("{NombreEmpresa}", empresa);
            body = body.Replace("{Titulo}", titulo);
            body = body.Replace("{Texto}", mensaje);
            body = body.Replace("{NOMBRECLIENTE}", datos[4]);
            body = body.Replace("{RUTCLIENTE}", datos[5]);
            body = body.Replace("{CORREOCLIENTE}", datos[2]);
            body = body.Replace("{NUMEROCOMPROBANTE}", datos[0]);
            body = body.Replace("{TOTAL}", datos[3].Replace(",", "."));
            body = body.Replace("{anio}", DateTime.Now.Year.ToString());
            body = body.Replace("{logo}", logo);
            return body;
        }

        private async Task<string> PopulateBodyPagoSinComprobante(string titulo, string mensaje, string empresa, string logo, string datos)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/avisoPagoSinComprobante.component.html")))
            {
                body = reader.ReadToEnd();
            }
            string[] informacion = datos.Split('|');
            body = body.Replace("{NombreEmpresa}", empresa);
            body = body.Replace("{Titulo}", titulo);
            body = body.Replace("{Texto}", mensaje);
            body = body.Replace("{NOMBRECLIENTE}", informacion[3]);
            body = body.Replace("{RUTCLIENTE}", informacion[4]);
            body = body.Replace("{CORREOCLIENTE}", informacion[1]);
            body = body.Replace("{MONTOPAGO}", informacion[2].Replace(",", "."));
            body = body.Replace("{anio}", DateTime.Now.Year.ToString());
            body = body.Replace("{logo}", logo);
            return body;
        }

        public int calculaDisponiblesCobranza()
        {
            try
            {
                int enviados = 0;
                int enviosDiarios = 0;
                int porEnviar = 0;
                int disponibles = 0;

                //Obtiene cantidad de correos enviados tipo cobranza
                enviados = _context.RegistroEnvioCorreos.Where(x => x.IdTipoEnvio == 6 && x.FechaEnvio == DateTime.Now).Count();

                //Obtiene cantidad de correos configurados para cobranza
                enviosDiarios = (int)_context.ConfiguracionCorreoCasillas.FirstOrDefault().CantidadDia; //FCA 18-07-2022 REVISAR ESTO
                //Calculo disponibles entre envios diarios y enviados
                disponibles = enviosDiarios - enviados;

                //Obtiene cantidad de correos por enviar para fecha de cobranza creada
                var cobranzas = _context.CobranzaCabeceras.Where(x => x.IdEstado == 1 && x.FechaInicio == DateTime.Now).ToList();
                int correosDelDia = 0;
                if (cobranzas.Count > 0)
                {
                    //Recorremos las cobranzas y obtenemos la cantidad de correos que se enviaran al contar los alumnos
                    foreach (var item in cobranzas)
                    {
                        if (item.CobranzaDetalles.Count > 0)
                        {
                            correosDelDia = correosDelDia + item.CobranzaDetalles.Select(x => x.RutCliente).Distinct().Count();
                        }
                    }
                }

                //Obtenemos disponibles
                disponibles = disponibles - correosDelDia;
                return disponibles;
            }
            catch
            {
                throw;
            }

            return 0;
        }

        public int calculaDisponiblesPorDia(string casilla)
        {
            try
            {

                int retorno = 0;
                if (string.IsNullOrEmpty(casilla))
                {
                    retorno = 1000;
                }
                else
                {
                    var casillas = _context.ConfiguracionCorreoCasillas.Where(x => x.Casilla.ToUpper() == casilla.ToUpper()).FirstOrDefault();

                    if (casillas != null)
                    {
                        retorno = (int)casillas.CantidadDia;
                    }
                }
                return retorno;
            }
            catch
            {
                throw;
            }
        }

        public void registraLogCorreo(string codAux, string rut, string correos)
        {
            try
            {
                var hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                foreach (var item in correos.Split(';'))
                {
                    var logCorreo = new LogCorreo();

                    logCorreo.Fecha = DateTime.Now;
                    logCorreo.CodAux = codAux;
                    logCorreo.Error = null;
                    logCorreo.Estado = "Enviado";
                    logCorreo.Rut = rut;
                    logCorreo.TipoCorreo = 2;
                    logCorreo.Tipo = "Cobranza";

                    _context.LogCorreos.Add(logCorreo);
                }
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
