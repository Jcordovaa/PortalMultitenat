using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class AutomatizacionController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AutomatizacionController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetAutomatizaciones"), Authorize]
        public ActionResult<object> GetAutomatizaciones()
        {
            try
            {
                var automatizaciones = _context.Automatizacions.ToList();
                return Ok(automatizaciones);
            }
            catch (Exception ex)
            {
                //FCA 17-12-2021  Se guarda log
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Automatizacion/GetAutomatizaciones";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnviaAutomatizaciones"), Authorize]
        public async Task<ActionResult<object>> EnviaAutomatizacionesAsync()
        {
            int horaActual = DateTime.Now.Hour;
            string estadoLogC = string.Empty;
            LogCobranza lc = new LogCobranza();
            lc.FechaInicio = DateTime.Now;
            lc.Estado = "PROCESANDO";
            _context.LogCobranzas.Add(lc);
            _context.SaveChanges();
            SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

            try
            {
                Generador genera = new Generador(_context,_webHostEnvironment);
                var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string urlFrot = System.Configuration.ConfigurationManager.AppSettings["URL_FRONT"];

                //Obtiene correos disponibles
                MailService mail = new MailService(_context, _webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                //Obtenemos tipos de documentos
                var tiposDocumentos = await sf.GetAllTipoDocSoftlandAsync();

                #region COBRANZA CLASICA
                //Genera envio de cobranzas, la ejecución la realiza algun procedimiento externo
                var listaAutomatizaciones = _context.Automatizacions.Where(x => x.Estado == 1).ToList();
                var feriados = _context.Feriados.ToList();
                var clientesExcluidos = _context.ClientesExcluidos.ToList();
                var configPago = _context.ConfiguracionPagoClientes.FirstOrDefault();
                int correosEnviados = 0;
                //Recorremos una a una las cobranzas
                foreach (var automatizacion in listaAutomatizaciones)
                {
                    if (automatizacion.Estado == 0)
                    {
                        continue;
                    }

                    var horario = _context.CobranzaHorarios.Where(x => x.IdHorario == automatizacion.IdHorario).FirstOrDefault();
                    if (horario.Hora != horaActual)
                    {
                        continue;
                    }

                    if (automatizacion.ExcluyeFestivos == 1)
                    {
                        var esFeriado = feriados.Where(x => x.Fecha.Value.Date == DateTime.Now.Date && x.Anio == DateTime.Now.Year).FirstOrDefault();
                        if (esFeriado != null)
                        {
                            continue;
                        }
                    }

                    switch (automatizacion.IdPerioricidad)
                    {
                        case 1:
                            if (DateTime.Now.Day != 1)
                            {
                                continue;
                            }
                            break;
                        case 2:
                            if (DateTime.Now.Day != 15)
                            {
                                continue;
                            }
                            break;
                        case 3:
                            if (DateTime.Now.Day != 30)
                            {
                                continue;
                            }
                            break;
                        case 4:
                            if (DateTime.Now.Day != automatizacion.DiaEnvio)
                            {
                                continue;
                            }
                            break;
                        case 5:
                            if ((int)DateTime.Now.DayOfWeek != automatizacion.DiaEnvio)
                            {
                                continue;
                            }
                            break;
                    }

                    Nullable<DateTime> fechaHasta = null;
                    Nullable<DateTime> fechaDesde = null;
                    if (automatizacion.Anio == null || automatizacion.Anio == 0)
                    {
                        automatizacion.Anio = 0;
                    }
                    else
                    {
                        fechaHasta = new DateTime((int)automatizacion.Anio, 12, 31, 0, 0, 0);
                        fechaDesde = new DateTime((int)automatizacion.Anio, 01, 01, 0, 0, 0);
                    }





                    var documentos = await sf.GetDocumentosPendientesCobranzaSinFiltroAsync((int)automatizacion.Anio, fechaDesde, fechaHasta, configPago.TiposDocumentosDeuda);

                    if (automatizacion.ExcluyeClientes == 1)
                    {
                        foreach (var item in clientesExcluidos)
                        {
                            documentos.RemoveAll(x => x.RutCliente == item.RutCliente);
                        }
                    }

                    switch (automatizacion.IdAutomatizacion)
                    {
                        case 1:
                            documentos = documentos.Where(x => (x.FechaVencimiento.Date - DateTime.Now.Date).TotalDays <= automatizacion.DiasVencimiento && (x.FechaVencimiento.Date - DateTime.Now.Date).TotalDays > 0).ToList();
                            break;
                        case 3:
                            documentos = documentos.Where(x => (DateTime.Now.Date - x.FechaVencimiento.Date).TotalDays >= automatizacion.DiasVencimiento).ToList();
                            break;
                    }

                    //Obtenemos los clientes a los que se le enviara la cobranza
                    var clientes = documentos.Select(x => x.RutCliente).Distinct().ToList();

                    List<DetalleEnvioCobranzaVm> listaEnvio = new List<DetalleEnvioCobranzaVm>();

                    //Recorremes y seleccionamos los documentos por clientes
                    foreach (var al in clientes)
                    {
                        var clienteApi = await sf.BuscarClienteSoftland2Async(al.Replace(".", "").Split('-')[0], string.Empty, string.Empty);

                        if (clienteApi.Count == 0)
                        {
                            continue;
                        }

                        //if (!automatizacion.CodCanalVenta.Contains(clienteApi[0].v))
                        //{

                        //}

                        if (!string.IsNullOrEmpty(clienteApi[0].CodCatCliente))
                        {
                            if (!automatizacion.CodCategoriaCliente.Contains(clienteApi[0].CodCatCliente))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }


                        if (!string.IsNullOrEmpty(clienteApi[0].CodCobrador))
                        {
                            if (!automatizacion.CodCobrador.Contains(clienteApi[0].CodCobrador))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(clienteApi[0].CodCondVenta))
                        {
                            if (!automatizacion.CodCondicionVenta.Contains(clienteApi[0].CodCondVenta))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(clienteApi[0].CodLista))
                        {

                            if (!automatizacion.CodListaPrecios.Contains(clienteApi[0].CodLista))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(clienteApi[0].CodVendedor))
                        {

                            if (!automatizacion.CodVendedor.Contains(clienteApi[0].CodVendedor))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }




                        var docCliente = documentos.Where(x => x.RutCliente == al).ToList();

                        var contactos = await sf.GetContactosClienteAsync(docCliente[0].RutCliente.Replace(".", "").Split('-')[0]);
                        string correos = string.Empty;
                        if (automatizacion.EnviaTodosContactos == 1)
                        {
                            foreach (var item in contactos)
                            {
                                if (!string.IsNullOrEmpty(item.Correo))
                                {
                                    if (string.IsNullOrEmpty(correos))
                                    {
                                        correos = item.Correo;
                                    }
                                    else
                                    {
                                        correos = correos + ";" + item.Correo;
                                    }
                                }

                            }
                        }
                        else
                        {
                            foreach (var item in contactos)
                            {
                                var exist = automatizacion.CodCargo.Split(';').Where(x => x == item.CodCargo).FirstOrDefault();
                                if (exist != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Correo))
                                    {
                                        if (string.IsNullOrEmpty(correos))
                                        {
                                            correos = item.Correo;
                                        }
                                        else
                                        {
                                            correos = correos + ";" + item.Correo;
                                        }
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(correos))
                        {
                            correos = clienteApi[0].Correo;
                        }

                        docCliente[0].EmailCliente = correos;


                        if (automatizacion.IdAutomatizacion == 3 && automatizacion.AgrupaCobranza != 1)
                        {
                            foreach (var item in docCliente)
                            {
                                DetalleEnvioCobranzaVm doc = new DetalleEnvioCobranzaVm();
                                doc.RutCliente = al;
                                doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? string.Empty : docCliente[0].NombreCliente;
                                doc.EmailCliente = docCliente[0].EmailCliente;
                                doc.CantidadDocumentosPendientes = docCliente.Count;
                                doc.MontoDeuda = Convert.ToInt32(item.MontoDocumento);
                                doc.ListaDocumentos = new List<DocumentosCobranzaVM>();

                                DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                                aux.Folio = (int)item.FolioDocumento;
                                aux.FechaEmision = (DateTime)item.FechaEmision;
                                aux.FechaVencimiento = (DateTime)item.FechaVencimiento;
                                aux.Monto = (int)item.MontoDocumento;
                                aux.TipoDocumento = item.TipoDocumento;
                                doc.ListaDocumentos.Add(aux);

                                //Agrega detalle para envió
                                listaEnvio.Add(doc);
                            }
                        }
                        else
                        {
                            DetalleEnvioCobranzaVm doc = new DetalleEnvioCobranzaVm();
                            doc.RutCliente = al;
                            doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? string.Empty : docCliente[0].NombreCliente;
                            doc.EmailCliente = docCliente[0].EmailCliente;
                            doc.CantidadDocumentosPendientes = docCliente.Count;
                            doc.MontoDeuda = Convert.ToInt32(docCliente.Sum(x => x.MontoDocumento));
                            doc.ListaDocumentos = new List<DocumentosCobranzaVM>();

                            //Agregamos documentos
                            foreach (var d in docCliente)
                            {
                                DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                                aux.Folio = (int)d.FolioDocumento;
                                aux.FechaEmision = (DateTime)d.FechaEmision;
                                aux.FechaVencimiento = (DateTime)d.FechaVencimiento;
                                aux.Monto = (int)d.MontoDocumento;
                                aux.TipoDocumento = d.TipoDocumento;
                                doc.ListaDocumentos.Add(aux);
                            }

                            //Agrega detalle para envió
                            listaEnvio.Add(doc);
                        }

                    }

                    int IdEstadoFinal = 3; //ESTADO ENVIADA

                    //Recorremos resultado de cobranza, generamos documento y enviamos correo
                    foreach (var cobranza in listaEnvio)
                    {
                        //Valida si existen correos disponbles para envio
                        if (correosEnviados >= correosDisponibles)
                        {
                            //No existen correos por lo que se cambia el estado a 2: ENVIADO PARCIALMENTE
                            IdEstadoFinal = 2;
                            break;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(cobranza.EmailCliente))
                            {
                                continue;
                            }
                            var docStream = genera.generaDetalleCobranza(cobranza);

                            string body = string.Empty;
                            string rutaCorreo = "~/Uploads/MailTemplates/cobranza.component.html";


                            string asunto = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, rutaCorreo)))
                            {
                                body = reader.ReadToEnd();
                            }

                            string rutCliente = Convert.ToBase64String(Encoding.UTF8.GetBytes(cobranza.RutCliente));
                            body = body.Replace("{NOMBRE}", cobranza.NombreCliente);
                            body = body.Replace("{ColorBoton}", auxCorreo.ColorBoton);

                            if (automatizacion.IdAutomatizacion == 1)
                            {
                                asunto = auxCorreo.AsuntoPreCobranza;
                                body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0");
                                body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                                body = body.Replace("{TituloCorreo}", auxCorreo.TituloPreCobranza);
                                body = body.Replace("{TextoCorreo}", auxCorreo.TextoPreCobranza);
                                body = body.Replace("{LOGO}", auxEmpresa.UrlPortal + "/" + auxEmpresa.Logo);
                            }
                            else if (automatizacion.IdAutomatizacion == 2)
                            {
                                asunto = auxCorreo.AsuntoEstadoCuenta;
                                body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0");
                                body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                                body = body.Replace("{TituloCorreo}", auxCorreo.TituloEstadoCuenta);
                                body = body.Replace("{TextoCorreo}", auxCorreo.TextoEstadoCuenta);
                                body = body.Replace("{LOGO}", auxEmpresa.UrlPortal + "/" + auxEmpresa.Logo);
                            }
                            else if (automatizacion.IdAutomatizacion == 3)
                            {
                                asunto = auxCorreo.AsuntoCobranza;
                                if (automatizacion.AgrupaCobranza == 1)
                                {
                                    body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/0");
                                }
                                else
                                {
                                    body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/0/{cobranza.ListaDocumentos[0].Folio}/0");
                                }
                                body = body.Replace("{NombreEmpresa}", auxEmpresa.NombreEmpresa);
                                body = body.Replace("{TituloCorreo}", auxCorreo.TituloCobranza);
                                body = body.Replace("{TextoCorreo}", auxCorreo.TextoCobranza);
                                body = body.Replace("{LOGO}", auxEmpresa.UrlPortal + "/" + auxEmpresa.Logo);
                            }



                            try
                            {
                                using (MailMessage mailMessage = new MailMessage())
                                {
                                    Attachment documento = new Attachment(new MemoryStream(docStream), "Estado Cuenta.pdf", "application/pdf");
                                    mailMessage.Attachments.Add(documento);

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

                                    MailService ms = new MailService(_context, _webHostEnvironment);
                                    ms.registraLogCorreo(cobranza.RutCliente, cobranza.RutCliente.Replace(".", "").Split('-')[0], cobranza.EmailCliente);
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

                                estadoLogC = estadoLogC + ";Error al enviar correo";
                                IdEstadoFinal = 2;
                                if (ex.Message == "Error al enviar correo.")
                                {
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                    }
                    //Agregar a logCobranza
                    string nombreAutomatizacion = string.Empty;
                    if (automatizacion.IdAutomatizacion == 1)
                    {
                        nombreAutomatizacion = "PRECOBRANZA";
                    }
                    else if (automatizacion.IdAutomatizacion == 2)
                    {
                        nombreAutomatizacion = "ESTADOCUENTA";
                    }
                    else if (automatizacion.IdAutomatizacion == 3)
                    {
                        nombreAutomatizacion = "COBRANZA";
                    }
                    lc.CobranzasConsideradas = lc.CobranzasConsideradas + nombreAutomatizacion + ";";
                }

                lc.FechaTermino = DateTime.Now;
                lc.Estado = "FINALIZADA" + estadoLogC;
                _context.Entry(lc).State = EntityState.Modified;
                _context.SaveChanges();
                #endregion

                return Ok(1);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"Automatizacion/EnviaAutomatizaciones";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

                lc.FechaTermino = DateTime.Now;
                lc.Estado = "ERROR";
                _context.Entry(lc).State = EntityState.Modified;
                _context.SaveChanges();

                return BadRequest();
            }
        }

        [HttpPost("GuardaAutomatizacion"), Authorize]
        public async Task<ActionResult<AuthenticateVm>> GuardaAutomatizacionAsync(AutomatizacionVm automatizacion)
        {
            try
            {
                var aut = _context.Automatizacions.Where(x => x.IdAutomatizacion == automatizacion.IdAutomatizacion).FirstOrDefault();

                if (aut != null)
                {
                    aut.AgrupaCobranza = automatizacion.AgrupaCobranza;
                    aut.Anio = automatizacion.Anio;
                    aut.CodCategoriaCliente = automatizacion.CodCategoriaCliente;
                    aut.CodCondicionVenta = automatizacion.CodCondicionVenta;
                    aut.CodListaPrecios = automatizacion.CodListaPrecios;
                    aut.CodVendedor = automatizacion.CodVendedor;
                    aut.DiasVencimiento = automatizacion.DiasVencimiento;
                    aut.Estado = automatizacion.Estado;
                    aut.ExcluyeClientes = automatizacion.ExcluyeClientes;
                    aut.ExcluyeFestivos = automatizacion.ExcluyeFestivos;
                    aut.IdHorario = automatizacion.IdHorario;
                    aut.IdPerioricidad = automatizacion.IdPerioricidad;
                    aut.IdTipoAutomatizacion = automatizacion.IdTipoAutomatizacion;
                    aut.MuestraSoloVencidos = automatizacion.MuestraSoloVencidos;
                    aut.TipoDocumentos = automatizacion.TipoDocumentos;
                    aut.DiaEnvio = automatizacion.DiaEnvio;
                    aut.DiasRecordatorio = automatizacion.DiasRecordatorio;
                    aut.AgrupaCobranza = automatizacion.AgrupaCobranza;
                    aut.CodCobrador = automatizacion.CodCobrador;
                    aut.CodCanalVenta = automatizacion.CodCanalVenta;
                    aut.CodCargo = automatizacion.CodCargo;
                    aut.EnviaCorreoFicha = automatizacion.EnviaCorreoFicha;
                    aut.EnviaTodosContactos = automatizacion.EnviaTodosContactos;

                }

                _context.Entry(aut);
                await _context.SaveChangesAsync();
                automatizacion.IdAutomatizacion = aut.IdAutomatizacion;

                return Ok(automatizacion);
            }
            catch (Exception ex)
            {
                //FCA 17-12-2021  Se guarda log
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Automatizacion/GuardaAutomaticacion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ActualizaAutomatizacion"), Authorize]
        public async Task<ActionResult<AuthenticateVm>> ActualizaAutomatizacionAsync(AutomatizacionVm automatizacion)
        {
            try
            {
                var aut = _context.Automatizacions.Where(x => x.IdAutomatizacion == automatizacion.IdAutomatizacion).FirstOrDefault();
                if (aut != null)
                {
                    aut.AgrupaCobranza = automatizacion.AgrupaCobranza;
                    aut.Anio = automatizacion.Anio;
                    aut.CodCategoriaCliente = automatizacion.CodCategoriaCliente;
                    aut.CodCondicionVenta = automatizacion.CodCondicionVenta;
                    aut.CodListaPrecios = automatizacion.CodListaPrecios;
                    aut.CodVendedor = automatizacion.CodVendedor;
                    aut.DiasVencimiento = automatizacion.DiasVencimiento;
                    aut.Estado = automatizacion.Estado;
                    aut.ExcluyeClientes = automatizacion.ExcluyeClientes;
                    aut.ExcluyeFestivos = automatizacion.ExcluyeFestivos;
                    aut.IdHorario = automatizacion.IdHorario;
                    aut.IdPerioricidad = automatizacion.IdPerioricidad;
                    aut.IdTipoAutomatizacion = automatizacion.IdTipoAutomatizacion;
                    aut.MuestraSoloVencidos = automatizacion.MuestraSoloVencidos;
                    aut.TipoDocumentos = automatizacion.TipoDocumentos;
                    aut.DiasRecordatorio = automatizacion.DiasRecordatorio;
                    aut.CodCobrador = automatizacion.CodCobrador;
                    aut.CodCanalVenta = automatizacion.CodCanalVenta;
                    aut.EnviaCorreoFicha = automatizacion.EnviaCorreoFicha;
                    aut.EnviaTodosContactos = automatizacion.EnviaTodosContactos;
                    _context.Entry(aut);
                    _context.SaveChanges();

                }

                return Ok();
            }
            catch (Exception ex)
            {
                //FCA 17-12-2021  Se guarda log
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Automatizacion/ActualizaAutomaticacion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTipos"), Authorize]
        public ActionResult<object> GetTipos()
        {
            try
            {
                var tipos = _context.TipoAutomatizacions.ToList();
                return Ok(tipos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Automatizacion/GetTipos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
