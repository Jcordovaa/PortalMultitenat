using ApiPortal.Controllers;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.ModelSoftland;
using ApiPortal.ViewModelsPortal;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

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
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioDocumentos.component.html")))
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
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/avisoVenta.component.html")))
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
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/avisoPagoSinComprobante.component.html")))
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


        public void EnviaCambioCorreoUsuario(Usuario usuario, UsuariosVm c, string correoAntiguo)
        {

            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            string body = string.Empty;
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;

            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/cambioCorreo.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NOMBRE}", usuario.Nombres);
            body = body.Replace("{CORREO}", c.Email);
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Titulo}", configCorreo.TituloCambioCorreo);
            body = body.Replace("{Texto}", configCorreo.TextoCambioCorreo);
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(usuario.Email);
                    mailMessage.To.Add(correoAntiguo);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoCambioCorreo;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 4;
                registro.IdUsuario = usuario.IdUsuario;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                throw;
            }
        }

        public void EnviaCorreoCambioClaveUsuario(Usuario usuario, string claveEnvio)
        {
            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            string body = string.Empty;
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;

            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NOMBRE}", usuario.Nombres);
            body = body.Replace("{CLAVE}", claveEnvio);
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Titulo}", configCorreo.TituloCambioClave);
            body = body.Replace("{Texto}", configCorreo.TextoCambioClave);
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(usuario.Email);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoCambioClave;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 4;
                registro.IdUsuario = usuario.IdUsuario;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                throw;
            }
        }

        public bool EnviaAcceso(ClienteAPIDTO item, string correo, string clave)
        {

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();

            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = item.RutAux;
            logCorreo.CodAux = item.CodAux;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
            body = body.Replace("{TEXTO}", configCorreo.TextoMensajeActivacion);
            body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
            body = body.Replace("{NOMBRE}", item.NomAux);
            body = body.Replace("{RUT}", item.RutAux);
            body = body.Replace("{CORREO}", correo.ToLower());
            body = body.Replace("{CLAVE}", clave);
            body = body.Replace("{Titulo}", configCorreo.TituloAccesoCliente);
            body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
            string datosCliente = Encrypt.Base64Encode(item.CodAux + ";" + correo.ToLower());
            body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(correo.ToLower());

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();

                var user = _context.Usuarios.Where(x => x.Email == item.correoUsuario).FirstOrDefault();
                if (user != null)
                {
                    var registro = new RegistroEnvioCorreo();

                    registro.FechaEnvio = DateTime.Today;
                    registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                    registro.IdTipoEnvio = 1;
                    registro.IdUsuario = user.IdUsuario;

                    _context.RegistroEnvioCorreos.Add(registro);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public async Task<bool> EnviaNotificacionEjecucionAccesos(List<ClienteAPIDTO> listaNoEnviados)
        {

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();

            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Tipo = "Notificacion";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            try
            {

                string body = string.Empty;
                string htmlFinal = string.Empty;
                string ruta = listaNoEnviados.Count > 0 ? "Uploads/MailTemplates/envioAccesoClientesErrores.html" : "Uploads/MailTemplates/envioAccesoClientesExitoso.html";
                using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, ruta)))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                body = body.Replace("{LOGO}", configCorreo.LogoCorreo);

                if (listaNoEnviados.Count > 0)
                {
                    string[] partes = body.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    foreach (var det in listaNoEnviados)
                    {

                        reemplazoDetalle = reemplazoDetalle + partes[1].Replace("{RUT}", det.RutAux).Replace("{NOMBRE}", det.NomAux);

                    }

                    partes[1] = reemplazoDetalle;



                    foreach (var p in partes)
                    {
                        htmlFinal = htmlFinal + p;
                    }
                }
                else
                {
                    htmlFinal = body;
                }

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(configCorreo.CorreoAvisoPago);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
                    mailMessage.Body = htmlFinal;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    smtp.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public bool EnviaAccesoUsuario(Usuario user, string pass)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
            {
                body = reader.ReadToEnd();
            }

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            Boolean errorEnvio = false;



            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = configEmpresa.RutEmpresa;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
            body = body.Replace("{TEXTO}", "Estimado usuario, a continuación, encontrará las credenciales para poder realizar la activación de su cuenta en nuestro portal, una vez activada, deberas ingresar con el Rut de la empresa, correo y clave registrados.");
            body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
            body = body.Replace("{NOMBRE}", user.Nombres + " " + user.Apellidos);
            body = body.Replace("{RUT}", configEmpresa.RutEmpresa);
            body = body.Replace("{CORREO}", user.Email.ToLower());
            body = body.Replace("{CLAVE}", pass);
            body = body.Replace("{Titulo}", "Activación de Cuenta");
            body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
            string datosCliente = Encrypt.Base64Encode(configEmpresa.RutEmpresa + ";" + user.Email.ToLower());
            body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(user.Email.ToLower());

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = "Activación de Cuenta";
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 1;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                errorEnvio = true;
            }

            return errorEnvio;
        }


        public bool EnviaReestablecerContraseñaUsuario(Usuario user, string pass)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
            {
                body = reader.ReadToEnd();
            }

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            Boolean errorEnvio = false;



            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = configEmpresa.RutEmpresa;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
            body = body.Replace("{TEXTO}", "Estimado usuario, a continuación, encontrará las credenciales para poder realizar la activación de su cuenta en nuestro portal, una vez activada, deberas ingresar con el Rut de la empresa, correo y clave registrados.");
            body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
            body = body.Replace("{NOMBRE}", user.Nombres + " " + user.Apellidos);
            body = body.Replace("{RUT}", configEmpresa.RutEmpresa);
            body = body.Replace("{CORREO}", user.Email.ToLower());
            body = body.Replace("{CLAVE}", pass);
            body = body.Replace("{Titulo}", "Activación de Cuenta");
            body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
            string datosCliente = Encrypt.Base64Encode(configEmpresa.RutEmpresa + ";" + user.Email.ToLower());
            body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(user.Email.ToLower());

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = "Activación de Cuenta";
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 1;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                errorEnvio = true;
            }

            return errorEnvio;
        }

        public bool EnviaCorreoDatosUsuario(Usuario user, string pass)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
            {
                body = reader.ReadToEnd();
            }

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            Boolean errorEnvio = false;



            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = configEmpresa.RutEmpresa;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            body = body.Replace("{NOMBRE}", user.Nombres + " " + user.Apellidos);
            body = body.Replace("{CLAVE}", pass);
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Titulo}", "Actualización Clave");
            body = body.Replace("{Texto}", "Estimado usuario, se ha realizado un cambio de clave para su cuenta.");

            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(user.Email.ToLower());
                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = "Actualización Clave de Acceso";
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 1;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                errorEnvio = true;
            }
            return errorEnvio;
        }


        public async Task<string> EnviaCobranzaAsync(List<DetalleEnvioCobranzaVm> listaEnvio, CobranzaCabecera item)
        {
            string estadoLogC = string.Empty;
            int correosEnviados = 0;
            int correosDisponibles = this.calculaDisponiblesCobranza();
            int horaActual = DateTime.Now.Hour;
            int DiaActual = DateTime.Now.Day;
            DateTime fecha = DateTime.Now;
            DateTime fechaActual = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
            Generador genera = new Generador(_context, _webHostEnvironment);
            var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            string urlFrot = auxEmpresa.UrlPortal;

            foreach (var cobranza in listaEnvio)
            {

                //Valida si existen correos disponbles para envio
                if (correosEnviados >= correosDisponibles)
                {
                    //No existen correos por lo que se cambia el estado a 2: ENVIADO PARCIALMENTE
                    return "2";
                }
                else
                {
                    //var docStream = genera.generaDetalleCobranza(cobranza);

                    string body = string.Empty;
                    string rutaCorreo = (item.IdTipoCobranza == 1) ? (item.EnviaEnlacePago == 0) ? "Uploads/MailTemplates/cobranzaSinEnlace.component.html" : "Uploads/MailTemplates/cobranza.component.html" : "~/Uploads/MailTemplates/precobranza.component.html";
                    string asunto = string.Empty;
                    using (StreamReader reader =  new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, rutaCorreo)))
                    {
                        body = reader.ReadToEnd();
                    }

                    string rutCliente = Convert.ToBase64String(Encoding.UTF8.GetBytes(cobranza.RutCliente));
                    body = body.Replace("{NOMBRE}", cobranza.NombreCliente);
                    if (item.EnviaEnlacePago == 1)
                    {
                        body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/{item.IdCobranza}/0");
                        body = body.Replace("{ColorBoton}", auxCorreo.ColorBoton);
                    }

                    if (item.IdTipoCobranza == 1)
                    {
                        asunto = auxCorreo.AsuntoCobranza;
                        body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", auxCorreo.TituloCobranza);
                        body = body.Replace("{TextoCorreo}", auxCorreo.TextoCobranza);
                        body = body.Replace("{LOGO}", auxEmpresa.UrlPortal + "/" + auxEmpresa.Logo);
                    }
                    else if (item.IdCobranza == 2)
                    {
                        asunto = auxCorreo.AsuntoPreCobranza;
                        body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", auxCorreo.TituloPreCobranza);
                        body = body.Replace("{TextoCorreo}", auxCorreo.TextoPreCobranza);
                        body = body.Replace("{LOGO}", auxCorreo.LogoCorreo);
                    }

                    string codAuxEncriptado = Encrypt.Base64Encode(cobranza.CodAux);
                    var elanceDocumento = urlFrot + "/#/sessions/account-state-view/" + codAuxEncriptado.Replace("=", "") + "/" + cobranza.IdCobranza + "/0";

                    body = body.Replace("{ENLACEDOCUMENTO}", elanceDocumento);


                    try
                    {
                        if (!string.IsNullOrEmpty(cobranza.EmailCliente))
                        {
                            using (MailMessage mailMessage = new MailMessage())
                            {
                                //Attachment documento = new Attachment(new MemoryStream(docStream), "Estado Cuenta.pdf", "application/pdf");
                                //mailMessage.Attachments.Add(documento);

                                if (cobranza.EmailCliente.Contains(";"))
                                {
                                    string[] destinatarios = cobranza.EmailCliente.Split(';');
                                    foreach (var d in destinatarios)
                                    {
                                        if (!string.IsNullOrEmpty(d) && d.Trim() != ";")
                                        {
                                            mailMessage.To.Add(new MailAddress(d));
                                        }
                                    }
                                }
                                else
                                {
                                    mailMessage.To.Add(new MailAddress(cobranza.EmailCliente));
                                }

                                mailMessage.From = new MailAddress(auxCorreo.CorreoOrigen, auxCorreo.NombreCorreos);
                                mailMessage.Subject = asunto;
                                mailMessage.Body = body;
                                mailMessage.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = auxCorreo.SmtpServer;
                                smtp.EnableSsl = (auxCorreo.Ssl == 1) ? true : false;
                                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                NetworkCred.UserName = auxCorreo.Usuario;
                                NetworkCred.Password = Encrypt.Base64Decode(auxCorreo.Clave);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = Convert.ToInt32(auxCorreo.Puerto);
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                await smtp.SendMailAsync(mailMessage);

                                this.registraLogCorreo(cobranza.RutCliente, cobranza.RutCliente.Replace(".", "").Split('-')[0], cobranza.EmailCliente);
                            }

                            //Actualiza Correos enviados
                            try
                            {
                                var commandText = "UPDATE CobranzaDetalle SET IdEstado = @IdEstado, FechaEnvio = @FechaEnvio, HoraEnvio = @HoraEnvio  WHERE IdCobranza = @IdCobranza AND RutCliente = @RutCliente";
                                var valEstado = new SqlParameter("@IdEstado", 3); //ESTADO ENVIADA
                                var valFecha = new SqlParameter("@FechaEnvio", DateTime.Now);
                                var valHora = new SqlParameter("@HoraEnvio", (horaActual < 10) ? $"0{horaActual}:00" : $"{horaActual}:00");
                                var valRut = new SqlParameter("@RutCliente", cobranza.RutCliente);
                                var valId = new SqlParameter("@IdCobranza", item.IdCobranza);
                                _context.Database.ExecuteSqlRaw(commandText,  valEstado, valFecha, valHora, valRut, valId );
                            }
                            catch (Exception ex)
                            {
                                //Registrar en tabla log
                                LogProceso log = new LogProceso();
                                log.IdTipoProceso = -1;
                                log.Fecha = DateTime.Now;
                                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                                log.Ruta = @"cobranza\Update\CobranzaDetalle";
                                log.Mensaje = ex.Message;
                                log.Excepcion = ex.ToString();
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();

                                estadoLogC = estadoLogC + ";Error al actualizar detalle";
                            }

                            correosEnviados += cobranza.EmailCliente.Split(';').Count();
                        }

                    }
                    catch (Exception ex)
                    {
                        //Registrar en tabla log
                        LogProceso log = new LogProceso();
                        log.IdTipoProceso = -1;
                        log.Fecha = DateTime.Now;
                        log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                        log.Ruta = @"cobranza\SendMail";
                        log.Mensaje = ex.Message;
                        log.Excepcion = ex.ToString();
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();

                        estadoLogC = estadoLogC + ";Error al enviar correo-2";
                        if (ex.Message == "Error al enviar correo.")
                        {
                            Thread.Sleep(5000);
                        }
                    }
                }
            }

            return estadoLogC;
        }


        public async Task<int> EnviaAutomatizacionAsync(List<DetalleEnvioCobranzaVm> listaEnvio, Automatizacion automatizacion)
        {
            int correosDisponibles = this.calculaDisponiblesCobranza();
            int correosEnviados = 0;
            int idEstado = 0;
            Generador genera = new Generador(_context, _webHostEnvironment);
            var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            string urlFrot = auxEmpresa.UrlPortal;
            //Recorremos resultado de cobranza, generamos documento y enviamos correo
            foreach (var cobranza in listaEnvio)
            {
                //Valida si existen correos disponbles para envio
                if (correosEnviados >= correosDisponibles)
                {
                    //No existen correos por lo que se cambia el estado a 2: ENVIADO PARCIALMENTE
                    return 1;
                }
                else
                {
                    if (string.IsNullOrEmpty(cobranza.EmailCliente))
                    {
                        continue;
                    }
                    //var docStream = genera.generaDetalleCobranza(cobranza);

                    string body = string.Empty;
                    string rutaCorreo = "Uploads/MailTemplates/cobranza.component.html";


                    string asunto = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, rutaCorreo)))
                    {
                        body = reader.ReadToEnd();
                    }

                    string rutCliente = Convert.ToBase64String(Encoding.UTF8.GetBytes(cobranza.RutCliente));
                    body = body.Replace("{NOMBRE}", cobranza.NombreCliente);
                    body = body.Replace("{ColorBoton}", auxCorreo.ColorBoton);

                    if (automatizacion.IdTipoAutomatizacion == 1)
                    {
                        asunto = auxCorreo.AsuntoPreCobranza;
                        body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0/" + Encrypt.Base64Encode(cobranza.AutomatizacionJson));
                        body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", auxCorreo.TituloPreCobranza);
                        body = body.Replace("{TextoCorreo}", auxCorreo.TextoPreCobranza);
                        body = body.Replace("{LOGO}", auxCorreo.LogoCorreo);
                    }
                    else if (automatizacion.IdTipoAutomatizacion == 2)
                    {
                        asunto = auxCorreo.AsuntoEstadoCuenta;
                        body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0/" + Encrypt.Base64Encode(cobranza.AutomatizacionJson));
                        body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", auxCorreo.TituloEstadoCuenta);
                        body = body.Replace("{TextoCorreo}", auxCorreo.TextoEstadoCuenta);
                        body = body.Replace("{LOGO}", auxCorreo.LogoCorreo);
                    }
                    else if (automatizacion.IdTipoAutomatizacion == 3)
                    {
                        asunto = auxCorreo.AsuntoCobranza;
                        if (automatizacion.AgrupaCobranza == 1)
                        {
                            body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0/" + Encrypt.Base64Encode(cobranza.AutomatizacionJson));
                        }
                        else
                        {
                            body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/0/{cobranza.ListaDocumentos[0].Folio}/0/" + Encrypt.Base64Encode(cobranza.AutomatizacionJson));
                        }
                        body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", auxCorreo.TituloCobranza);
                        body = body.Replace("{TextoCorreo}", auxCorreo.TextoCobranza);
                        body = body.Replace("{LOGO}", auxCorreo.LogoCorreo);
                    }


                    string codAuxEncriptado = Encrypt.Base64Encode(cobranza.CodAux);
                    string automatizacionEncriptada = Encrypt.Base64Encode(cobranza.AutomatizacionJson);
                    var elanceDocumento = urlFrot + "/#/sessions/account-state-view/" + codAuxEncriptado.Replace("=", "") + "/0/" + automatizacionEncriptada;

                    body = body.Replace("{ENLACEDOCUMENTO}", elanceDocumento);

                    try
                    {
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            //Attachment documento = new Attachment(new MemoryStream(docStream), "Estado Cuenta.pdf", "application/pdf");
                            //mailMessage.Attachments.Add(documento);

                            if (cobranza.EmailCliente.Contains(";"))
                            {
                                string[] destinatarios = cobranza.EmailCliente.Split(';');
                                foreach (var d in destinatarios)
                                {
                                    if (!string.IsNullOrEmpty(d) && d.Trim() != ";")
                                    {
                                        mailMessage.To.Add(new MailAddress(d));
                                    }
                                }
                            }
                            else
                            {
                                mailMessage.To.Add(new MailAddress(cobranza.EmailCliente));
                            }

                            mailMessage.From = new MailAddress(auxCorreo.CorreoOrigen, auxCorreo.NombreCorreos);
                            mailMessage.Subject = asunto;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = auxCorreo.SmtpServer;
                            smtp.EnableSsl = (auxCorreo.Ssl == 1) ? true : false;
                            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                            NetworkCred.UserName = auxCorreo.Usuario;
                            NetworkCred.Password = Encrypt.Base64Decode(auxCorreo.Clave);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Convert.ToInt32(auxCorreo.Puerto);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            await smtp.SendMailAsync(mailMessage);

                            this.registraLogCorreo(cobranza.RutCliente, cobranza.RutCliente.Replace(".", "").Split('-')[0], cobranza.EmailCliente);
                        }

                        correosEnviados += cobranza.EmailCliente.Split(';').Count();
                    }
                    catch (Exception ex)
                    {
                        //Registrar en tabla log
                        LogProceso log = new LogProceso();
                        log.IdTipoProceso = -1;
                        log.Fecha = DateTime.Now;
                        log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                        log.Ruta = @"Autometizacion\EnviaAutomatizaciones";
                        log.Mensaje = ex.Message;
                        log.Excepcion = ex.ToString();
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();

                        //estadoLogC = estadoLogC + ";Error al enviar correo";
                        //IdEstadoFinal = 2;
                        if (ex.Message == "Error al enviar correo.")
                        {
                            Thread.Sleep(5000);
                        }
                        idEstado = 2;
                    }
                }
            }
            return idEstado;
        }


        public void EnviaRecuperacionContrasenaUsuario(Usuario usuario, string clave)
        {

            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();

            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = configEmpresa.RutEmpresa;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();


            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Texto}", "Se solicito una recuperacion de clave para su cuenta, debera ingresar con la nueva clave que se indica a continuación");
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NOMBRE}", usuario.Email);
            body = body.Replace("{CLAVE}", clave);
            body = body.Replace("{Titulo}", "Recuperar Clave");
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(usuario.Email);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = "Recuperar Clave";
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);

                }
            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
            }


            _context.LogCorreos.Attach(logCorreo);
            logCorreo.Estado = "Acceso Enviado";
            logCorreo.Error = "";
            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
            _context.SaveChanges();


            var registro = new RegistroEnvioCorreo();

            registro.FechaEnvio = DateTime.Today;
            registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
            registro.IdTipoEnvio = 4;
            registro.IdUsuario = usuario.IdUsuario;

            _context.RegistroEnvioCorreos.Add(registro);
            _context.SaveChanges();
        }

        public bool EnviaRecuperacionContrasenaCliente(ClientesPortal cliente, string clave)
        {

            bool errorEnvio = false;

            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = cliente.Rut;
            logCorreo.CodAux = cliente.CodAux;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Texto}", configCorreo.TextoRecuperarClave);
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NOMBRE}", cliente.Correo);
            body = body.Replace("{CLAVE}", clave);
            body = body.Replace("{Titulo}", configCorreo.TituloRecuperarClave);
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(cliente.Correo);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoRecuperarClave;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Send(mailMessage);

                }
            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                errorEnvio = true;
            }


            _context.LogCorreos.Attach(logCorreo);
            logCorreo.Estado = "Acceso Enviado";
            logCorreo.Error = "";
            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
            _context.SaveChanges();


            var registro = new RegistroEnvioCorreo();

            registro.FechaEnvio = DateTime.Today;
            registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
            registro.IdTipoEnvio = 4;
            registro.IdUsuario = cliente.IdCliente;

            _context.RegistroEnvioCorreos.Add(registro);
            _context.SaveChanges();

            return errorEnvio;
        }

        public bool EnviaCambioContrasena(ClientesPortal cliente, string claveEnvio)
        {
            var logCorreo = new LogCorreo();
            logCorreo.Fecha = DateTime.Now;
            logCorreo.Rut = cliente.Rut;
            logCorreo.CodAux = cliente.CodAux;
            logCorreo.Tipo = "Acceso";
            logCorreo.Estado = "PENDIENTE";

            _context.LogCorreos.Add(logCorreo);
            _context.SaveChanges();

            string body = string.Empty;
            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NOMBRE}", cliente.Nombre);
            body = body.Replace("{CLAVE}", claveEnvio);
            body = body.Replace("{logo}", configCorreo.LogoCorreo);
            body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
            body = body.Replace("{Titulo}", configCorreo.TituloCambioClave);
            body = body.Replace("{Texto}", configCorreo.TextoCambioClave);
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(cliente.Correo);

                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                    mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = configCorreo.SmtpServer;
                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = configCorreo.Usuario;
                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                    smtp.UseDefaultCredentials = false;
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                    smtp.Send(mailMessage);
                }

                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Acceso Enviado";
                logCorreo.Error = "";
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();


                var registro = new RegistroEnvioCorreo();

                registro.FechaEnvio = DateTime.Today;
                registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                registro.IdTipoEnvio = 4;
                registro.IdUsuario = cliente.IdCliente;

                _context.RegistroEnvioCorreos.Add(registro);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _context.LogCorreos.Attach(logCorreo);
                logCorreo.Estado = "Error al enviar correo";
                logCorreo.Error = ex.Message;
                _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }




    }
}
