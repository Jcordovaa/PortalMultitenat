using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.ModelSoftland;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CobranzaController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CobranzaController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
        }

        private async Task<List<DocumentoClienteCobranzaVm>> GetDocumentosClientes2Async(FiltroCobranzaVm model)
        {
            List<DocumentoClienteCobranzaVm> retorno = new List<DocumentoClienteCobranzaVm>();

            try
            {
                //Obtiene cantidad correos disponibles 
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var documentos = await sf.GetDocumentosPendientesCobranzaAsync(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento, model.CantidadDias, model.ExcluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, model.Estado);

                //Retorna


                //Obtenemos los rut de clientes
                var clientes = documentos.Select(x => x.RutCliente).Distinct().ToList();

                foreach (var item in clientes)
                {
                    var docCliente = documentos.Where(x => x.RutCliente == item).ToList();

                    DocumentoClienteCobranzaVm doc = new DocumentoClienteCobranzaVm();
                    doc.RutCliente = item;
                    doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? "Sin Información" : docCliente[0].NombreCliente;
                    doc.CantidadDocumentos = docCliente.Count;
                    doc.MontoDeuda = Convert.ToInt32(docCliente.Sum(x => x.SaldoDocumento));
                    doc.ListaDocumentos = docCliente;

                    //var cliente = db.ClientesPortal.Where(x => x.Rut == item).FirstOrDefault();

                    var cliente = await sf.GetClienteSoftlandAsync(doc.RutCliente.Replace(".", "").Split('-')[0]);
                    if (cliente == null)
                    {
                        doc.Selected = false;
                    }
                    else
                    {
                        //Valida que tenga correos disponibles para envio
                        if (correosDisponibles > 0)
                        {
                            doc.Selected = false; //Solo para pruebas
                            correosDisponibles = correosDisponibles - 1;
                        }
                        else
                        {
                            doc.Selected = false;
                        }

                        doc.EmailCliente = cliente.Correo;
                        doc.RutCliente = cliente.Rut;
                        doc.NombreCliente = cliente.Nombre;
                    }

                    retorno.Add(doc);
                }

                retorno = retorno.OrderByDescending(x => x.RutCliente).ToList();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"cobranza\GetDocumentosCobranzaFiltro2";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

            }
            return retorno;
        }
      

        [HttpPost("SaveCobranza"), Authorize]
        public async Task<ActionResult> SaveCobranza(CobranzaCabeceraVM model)
        {
            try
            {
                //INSERTA CABECERA
                CobranzaCabecera cabecera = new CobranzaCabecera();
                cabecera.IdCobranza = 0;
                cabecera.Nombre = model.Nombre;
                cabecera.FechaCreacion = DateTime.Now;
                cabecera.HoraCreacion = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                cabecera.IdTipoCobranza = model.IdTipoCobranza;
                cabecera.Estado = model.Estado;
                cabecera.IdUsuario = model.IdUsuario;
                cabecera.TipoProgramacion = model.TipoProgramacion;
                cabecera.FechaInicio = model.FechaInicio;
                cabecera.FechaFin = model.FechaFin;
                cabecera.HoraDeEnvio = model.HoraDeEnvio;
                cabecera.DiaSemanaEnvio = model.DiaSemanaEnvio;
                cabecera.DiasToleranciaVencimiento = model.DiasToleranciaVencimiento;
                cabecera.IdEstado = model.IdEstado;
                cabecera.Anio = model.Anio;
                cabecera.TipoDocumento = model.TipoDocumento;
                cabecera.FechaDesde = model.FechaDesde;
                cabecera.FechaHasta = model.FechaHasta;
                cabecera.AplicaClientesExcluidos = model.AplicaClientesExcluidos;
                cabecera.EsCabeceraInteligente = model.EsCabeceraInteligente;
                cabecera.IdCabecera = 0;
                cabecera.EnviaEnlacePago = model.EnviaEnlacePago;
                cabecera.CobranzaDetalles = null;
                cabecera.IdPeriodicidad = model.IdPeriodicidad;
                cabecera.ExcluyeFeriado = model.ExcluyeFeriados;
                cabecera.ExcluyeFinDeSemana = model.ExcluyeFinDeSemana;
                cabecera.DiaEnvio = model.DiaEnvio;
                cabecera.ListaPrecio = model.ListaPrecio;
                cabecera.Vendedor = model.Vendedor;
                cabecera.CategoriaCliente = model.CategoriaCliente;
                cabecera.CondicionVenta = model.CondicionVenta;
                cabecera.CargosContactos = model.CargosContactos;
                cabecera.EnviaCorreoFicha = model.EnviaCorreoFicha;
                cabecera.EnviaTodosContactos = model.EnviaTodosContactos;
                cabecera.EnviaTodosCargos = model.EnviaTodosCargos;


                _context.CobranzaCabeceras.Add(cabecera);
                _context.SaveChanges();


                //INSERTA DETALLE
                var idCabecera = cabecera.IdCobranza; //Obtengo idCabecera

                foreach (CobranzaDetalleVm row in model.CobranzaDetalle)
                {
                    CobranzaDetalle det = new CobranzaDetalle();
                    det.IdCobranzaDetalle = 0;
                    det.IdCobranza = idCabecera;
                    det.Folio = row.Folio;
                    det.FechaEmision = row.FechaEmision;
                    det.FechaVencimiento = row.FechaVencimiento;
                    det.Monto = row.Monto;
                    det.RutCliente = row.RutCliente;
                    det.CodAuxCliente = row.CodAuxCliente;
                    det.TipoDocumento = row.TipoDocumento;
                    det.IdEstado = row.IdEstado;
                    det.FechaEnvio = null;
                    det.HoraEnvio = null;
                    det.FechaPago = null;
                    det.HoraPago = null;
                    det.ComprobanteContable = null;
                    det.FolioDte = null;
                    det.IdPago = null;
                    det.IdCobranzaNavigation = null;
                    det.IdEstadoNavigation = null;
                    det.CuentaContable = row.CuentaContable;
                    det.NombreCliente = row.NombreCliente;
                    det.Pagado = 0;
                    _context.CobranzaDetalles.Add(det);
                }

                _context.SaveChanges();

                return Ok(idCabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/SaveCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnviaCobranza"), Authorize]
        public async Task<ActionResult> EnviaCobranza()
        {
            string estadoLogC = string.Empty;
            LogCobranza lc = new LogCobranza();
            lc.FechaInicio = DateTime.Now;
            lc.Estado = "PROCESANDO";
            _context.LogCobranzas.Add(lc);
            _context.SaveChanges();
            SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

            try
            {
                //FCA 13-12-2021 Se crea flujo para cobranza inteligente
                #region Cobranza inteligente 2
                //Obtenemos hora actual
                int horaActual = DateTime.Now.Hour;
                int DiaActual = DateTime.Now.Day;
                DateTime fecha = DateTime.Now;
                DateTime fechaActual = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
                var listaInteligente = _context.CobranzaCabeceras.Where(x => x.Estado == 1 && x.TipoProgramacion == 2 && ((x.EsCabeceraInteligente == 1 && fechaActual >= x.FechaInicio && fechaActual <= x.FechaFin && x.HoraDeEnvio == horaActual) || x.EjecutaSiguienteHabil == 1)).AsNoTracking().ToList();
                var diasFeriados = _context.Feriados.ToList();

                //Recorremos una a una las cobranzas
                foreach (var item in listaInteligente)
                {

                    if (diasFeriados.Count > 0 && item.ExcluyeFeriado == 1)
                    {
                        var esFeriado = diasFeriados.Where(x => x.Fecha == fechaActual.Date).FirstOrDefault();
                        if (esFeriado != null)
                        {
                            item.EjecutaSiguienteHabil = 1;
                            _context.CobranzaCabeceras.Attach(item);
                            _context.Entry(item).Property(x => x.EjecutaSiguienteHabil).IsModified = true;
                            _context.SaveChanges();
                            continue;

                        }
                    }

                    if (item.ExcluyeFinDeSemana == 1 && (fechaActual.DayOfWeek == (DayOfWeek)6 || fechaActual.DayOfWeek == (DayOfWeek)0))
                    {
                        item.EjecutaSiguienteHabil = 1;
                        item.EjecutaSiguienteHabil = 1;
                        _context.CobranzaCabeceras.Attach(item);
                        _context.Entry(item).Property(x => x.EjecutaSiguienteHabil).IsModified = true;
                        _context.SaveChanges();
                        continue;
                    }

                    if (item.IdPeriodicidad == 1)
                    {
                        bool ejecutar = false;
                        string[] diasSemanas = item.DiaSemanaEnvio.Split(';');

                        var diaActual = fechaActual.DayOfWeek;

                        foreach (var dias in diasSemanas)
                        {
                            int numDia = int.Parse(dias);
                            if (numDia == 7)
                            {
                                numDia = 0;
                            }
                            DayOfWeek day = (DayOfWeek)numDia;


                            if (day == diaActual)
                            {
                                ejecutar = true;
                                break;
                            }

                        }
                        if (ejecutar == false)
                        {

                            continue;
                        }
                    }
                    else if (item.IdPeriodicidad == 2 && DiaActual != 1)
                    {
                        continue;
                    }
                    else if (item.IdPeriodicidad == 3 && DiaActual != 15)
                    {
                        continue;
                    }
                    else if (item.IdPeriodicidad == 4 && DiaActual != new DateTime(fechaActual.Year, fechaActual.Month, 1).AddMonths(1).AddDays(-1).Day)
                    {
                        continue;
                    }
                    else if (item.IdPeriodicidad == 5 && item.DiaEnvio != DiaActual) //FCA Revisar esto
                    {
                        continue;
                    }


                    FiltroCobranzaVm filtro = new FiltroCobranzaVm();
                    filtro.Anio = (int)item.Anio;
                    filtro.Fecha = (DateTime)item.FechaDesde;
                    filtro.FechaHasta = (DateTime)item.FechaHasta;
                    filtro.TipoDocumento = item.TipoDocumento;
                    filtro.CantidadDias = (int)item.DiasToleranciaVencimiento;
                    filtro.ExcluyeClientes = (int)item.AplicaClientesExcluidos;
                    if (item.IdTipoCobranza == 1)
                    {
                        filtro.Estado = "V";
                    }
                    else
                    {
                        filtro.Estado = "P";
                    }

                    List<DocumentoClienteCobranzaVm> documentos = await GetDocumentosClientes2Async(filtro);

                    documentos = documentos.Where(x => x.Selected == true).ToList();

                    if (item.HastaMontoDeuda != 0 && item.HastaMontoDeuda != null)
                    {
                        documentos = documentos.Where(x => x.MontoDeuda >= item.DesdeMontoDeuda && x.MontoDeuda <= item.HastaMontoDeuda).ToList();
                    }
                    if (item.CantidadDocumentos != 0 && item.CantidadDocumentos != null)
                    {
                        documentos = documentos.Where(x => x.CantidadDocumentos >= item.CantidadDocumentos).ToList();
                    }


                    CobranzaCabecera cobranzaHija = new CobranzaCabecera();
                    cobranzaHija.Anio = item.Anio;
                    cobranzaHija.AplicaClientesExcluidos = item.AplicaClientesExcluidos;
                    cobranzaHija.DiaSemanaEnvio = item.DiaSemanaEnvio;
                    cobranzaHija.DiasToleranciaVencimiento = item.DiasToleranciaVencimiento;
                    cobranzaHija.EnviaEnlacePago = item.EnviaEnlacePago;
                    cobranzaHija.EsCabeceraInteligente = 0;
                    cobranzaHija.FechaCreacion = DateTime.Now;
                    cobranzaHija.FechaDesde = DateTime.Now.Date;
                    cobranzaHija.FechaFin = DateTime.Now.Date;
                    cobranzaHija.FechaHasta = item.FechaHasta;
                    cobranzaHija.FechaInicio = item.FechaInicio;
                    cobranzaHija.HoraCreacion = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute;
                    cobranzaHija.HoraDeEnvio = item.HoraDeEnvio;
                    cobranzaHija.IdCabecera = item.IdCobranza;
                    cobranzaHija.IdEstado = 1;
                    cobranzaHija.IdPeriodicidad = item.IdPeriodicidad;
                    cobranzaHija.IdTipoCobranza = item.IdTipoCobranza;
                    cobranzaHija.TipoProgramacion = 1;
                    cobranzaHija.Estado = 1;
                    cobranzaHija.IdUsuario = item.IdUsuario;
                    var cobranzas = _context.CobranzaCabeceras.Where(x => x.IdCabecera == item.IdCobranza).Count() + 1;
                    cobranzaHija.Nombre = "Cobranza hija #" + cobranzas + "de :" + item.Nombre;
                    cobranzaHija.TipoDocumento = item.TipoDocumento;
                    item.EjecutaSiguienteHabil = 0;

                    _context.CobranzaCabeceras.Attach(item);
                    _context.Entry(item).Property(x => x.EjecutaSiguienteHabil).IsModified = true;
                    _context.CobranzaCabeceras.Add(cobranzaHija);
                    _context.SaveChanges();

                    List<CobranzaDetalle> listaDetalle = new List<CobranzaDetalle>();
                    foreach (var cliente in documentos)
                    {
                        foreach (var doc in cliente.ListaDocumentos)
                        {
                            CobranzaDetalle detalle = new CobranzaDetalle();
                            detalle.IdCobranza = cobranzaHija.IdCobranza;
                            detalle.Folio = doc.FolioDocumento;
                            detalle.FechaEmision = doc.FechaEmision;
                            detalle.FechaVencimiento = doc.FechaVencimiento;
                            detalle.Monto = doc.MontoDocumento;
                            detalle.RutCliente = doc.RutCliente;
                            detalle.TipoDocumento = doc.CodTipoDocumento;
                            detalle.IdEstado = 1;
                            detalle.CuentaContable = doc.CuentaContable;
                            detalle.EmailCliente = cliente.EmailCliente;
                            detalle.NombreCliente = doc.NombreCliente;
                            listaDetalle.Add(detalle);
                            _context.CobranzaDetalles.Add(detalle);
                            _context.SaveChanges();
                        }
                    }
                }
                //FCA 13-12-2021 Termina flujo cobranza inteligente
                #endregion Cobranza inteligente 2 


                Generador genera = new Generador(_context,_webHostEnvironment);
                var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string urlFrot = System.Configuration.ConfigurationManager.AppSettings["URL_FRONT"];

                //Obtiene correos disponibles
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();



                //Obtenemos tipos de documentos
                var tiposDocumentos = await sf.GetAllTipoDocSoftlandAsync();

                #region COBRANZA CLASICA
                //Genera envio de cobranzas, la ejecución la realiza algun procedimiento externo
                var listaManual = _context.CobranzaCabeceras.Where(x => x.Estado == 1 && x.TipoProgramacion == 1 && x.EsCabeceraInteligente == 0).AsNoTracking().ToList();

                int correosEnviados = 0;
                //Recorremos una a una las cobranzas
                foreach (var item in listaManual)
                {
                    //Validamos que cobranza no este vencida
                    if (item.FechaFin < DateTime.Now.Date) { continue; }
                    //Validamos hora de ejecución
                    if (horaActual != item.HoraDeEnvio) { continue; }
                    //Validamos fecha de ejecución para cobranzas pendientes
                    if (item.IdEstado == 1 && item.FechaInicio != DateTime.Now.Date.AddDays(1)) { continue; }
                    //Validamos fecha de ejecución para cobranzas parcialmente enviadas
                    if (item.IdEstado == 2 && item.FechaInicio > DateTime.Now.Date) { continue; }

                    //obtenemos documentos pendientes de pago para la cobranza
                    var listaDocPendientes = item.CobranzaDetalles.Where(x => x.IdEstado == 1).ToList();

                    //Obtenemos los clientes a los que se le enviara la cobranza
                    var clientes = listaDocPendientes.Select(x => x.RutCliente).Distinct().ToList();
                    List<DetalleEnvioCobranzaVm> listaEnvio = new List<DetalleEnvioCobranzaVm>();

                    //Recorremes y seleccionamos los documentos por clientes
                    foreach (var al in clientes)
                    {
                        var docCliente = listaDocPendientes.Where(x => x.RutCliente == al).ToList();

                        var contactos = await sf.GetContactosClienteAsync(docCliente[0].RutCliente.Replace(".", "").Split('-')[0]);
                        if (item.EnviaTodosCargos == 1)
                        {

                            if (contactos.Count == 0 && item.EnviaCorreoFicha == 1)
                            {
                                var cliente = await sf.GetClienteSoftlandAsync(docCliente[0].RutCliente.Replace(".", "").Split('-')[0]);
                                if (!string.IsNullOrEmpty(cliente.Correo))
                                {
                                    docCliente[0].EmailCliente = cliente.Correo;
                                }
                            }
                            else if (contactos.Count > 0)
                            {
                                string correos = string.Empty;
                                foreach (var c in contactos)
                                {
                                    if (string.IsNullOrEmpty(correos))
                                    {
                                        correos = c.Correo;
                                    }
                                    else
                                    {
                                        correos = correos + ";" + c.Correo;
                                    }
                                }
                                docCliente[0].EmailCliente = correos;
                            }
                        }
                        else
                        {
                            if (contactos.Count == 0)
                            {
                                string correos = string.Empty;
                                var cliente = await sf.GetClienteSoftlandAsync(docCliente[0].CodAuxCliente);
                                if (!string.IsNullOrEmpty(cliente.Correo))
                                {
                                    docCliente[0].EmailCliente = cliente.Correo;
                                }
                            }
                            else
                            {
                                var cargos = item.CargosContactos.Split(';');
                                string correos = string.Empty;
                                foreach (var c in contactos)
                                {
                                    var cargo = cargos.Where(x => x == c.CodCargo).FirstOrDefault();
                                    if (cargo != null)
                                    {
                                        if (string.IsNullOrEmpty(correos))
                                        {
                                            correos = c.Correo;
                                        }
                                        else
                                        {
                                            correos = correos + ";" + c.Correo;
                                        }
                                    }
                                }
                                docCliente[0].EmailCliente = correos;
                            }
                        }


                        DetalleEnvioCobranzaVm doc = new DetalleEnvioCobranzaVm();
                        doc.RutCliente = al;
                        doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? string.Empty : docCliente[0].NombreCliente;
                        doc.EmailCliente = docCliente[0].EmailCliente;
                        doc.CantidadDocumentosPendientes = docCliente.Count;
                        doc.MontoDeuda = Convert.ToInt32(docCliente.Sum(x => x.Monto));
                        doc.ListaDocumentos = new List<DocumentosCobranzaVM>();

                        //Agregamos documentos
                        foreach (var d in docCliente)
                        {
                            DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                            aux.Folio = (int)d.Folio;
                            aux.FechaEmision = (DateTime)d.FechaEmision;
                            aux.FechaVencimiento = (DateTime)d.FechaVencimiento;
                            aux.Monto = (int)d.Monto;
                            aux.TipoDocumento = tiposDocumentos.Where(x => x.CodDoc == d.TipoDocumento).FirstOrDefault().DesDoc;
                            doc.ListaDocumentos.Add(aux);
                        }

                        //Agrega detalle para envió
                        listaEnvio.Add(doc);
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
                            var docStream = genera.generaDetalleCobranza(cobranza);

                            string body = string.Empty;
                            string rutaCorreo = (item.IdTipoCobranza == 1) ? (item.EnviaEnlacePago == 0) ? "~/Uploads/MailTemplates/cobranzaSinEnlace.component.html" : "~/Uploads/MailTemplates/cobranza.component.html" : "~/Uploads/MailTemplates/precobranza.component.html";
                            string asunto = string.Empty;
                            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, rutaCorreo)))
                            {
                                body = reader.ReadToEnd();
                            }

                            string rutCliente = Convert.ToBase64String(Encoding.UTF8.GetBytes(cobranza.RutCliente));
                            body = body.Replace("{NOMBRE}", cobranza.NombreCliente);
                            if (item.EnviaEnlacePago == 1)
                            {
                                body = body.Replace("{ENLACE}", $"{urlFrot}/#/sessions/pay/{rutCliente}/0/{item.IdCobranza}");
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

                                    mail.registraLogCorreo(cobranza.RutCliente, cobranza.RutCliente.Replace(".", "").Split('-')[0], cobranza.EmailCliente);
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
                                    _context.Database.ExecuteSqlRaw(commandText, new[] { valEstado, valFecha, valHora, valRut, valId });
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

                                estadoLogC = estadoLogC + ";Error al enviar correo";
                                IdEstadoFinal = 2;
                                if (ex.Message == "Error al enviar correo.")
                                {
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                    }

                    //Actualiza estado cobranza cabecera

                    item.IdEstado = IdEstadoFinal;
                    _context.CobranzaCabeceras.Attach(item);
                    _context.Entry(item).Property(x => x.IdEstado).IsModified = true;
                    _context.SaveChanges();

                    //Agregar a logCobranza
                    lc.CobranzasConsideradas = lc.CobranzasConsideradas + item.IdCobranza + ";";
                }

                lc.FechaTermino = DateTime.Now;
                lc.Estado = "FINALIZADA" + estadoLogC;
                _context.Entry(lc).State = EntityState.Modified;
                _context.SaveChanges();
                #endregion

                #region COBRANZA INTELIGENTE



                #endregion



                return Ok(1);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/EnviaCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

                lc.FechaTermino = DateTime.Now;
                lc.Estado = "ERROR";
                _context.Entry(lc).State = EntityState.Modified;
                _context.SaveChanges();

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetCobranzaCliente"), Authorize]
        public async Task<ActionResult> GetCobranzaCliente(FiltroCobranzaVm model)
        {
            try
            {
                var tiposDocumentos = _context.TipoPagos.ToList();
                var cobranza = _context.CobranzaCabeceras.Where(x => x.IdCobranza == model.IdCobranza).FirstOrDefault();

                CobranzaCabeceraVM retorno = new CobranzaCabeceraVM();
                retorno.IdCobranza = cobranza.IdCobranza;
                retorno.EstaVencida = (cobranza.FechaFin >= DateTime.Now.Date) ? 0 : 1;
                retorno.CobranzaDetalle = new List<CobranzaDetalleVm>();

                foreach (var item in cobranza.CobranzaDetalles.Where(x => x.RutCliente == model.RutCliente && x.IdEstado != 5))
                {
                    var codTipo = tiposDocumentos.Where(x => x.TipoDocumento == item.TipoDocumento).First();
                    CobranzaDetalleVm det = new CobranzaDetalleVm();
                    det.selected = false;
                    det.IdCobranza = cobranza.IdCobranza;
                    det.IdCobranzaDetalle = item.IdCobranzaDetalle;
                    det.Folio = (int)item.Folio;
                    det.FechaEmision = (DateTime)item.FechaEmision;
                    det.FechaVencimiento = (DateTime)item.FechaVencimiento;
                    det.Monto = (int)item.Monto;
                    det.RutCliente = item.RutCliente;
                    det.CodTipoDocumento = item.TipoDocumento;
                    det.TipoDocumento = (codTipo != null) ? codTipo.Nombre : string.Empty;
                    det.CuentaContable = item.CuentaContable;
                    det.EmailCliente = item.EmailCliente;

                    retorno.CobranzaDetalle.Add(det);
                }

                return Ok(retorno);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosPendientes"), Authorize]
        public async Task<ActionResult> GetDocumentosPendientes(FiltroCobranzaVm model)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DocumentosCobranzaVm> listaDocumentos = new List<DocumentosCobranzaVm>();
                listaDocumentos = await sf.GetDocumentosPendientesCobranzaSinFiltroAsync(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento);

                if (!string.IsNullOrEmpty(model.Estado))
                {
                    if (model.Estado == "1")//PENDIENTE
                    {
                        return Ok(listaDocumentos.Where(x => x.Estado == "PENDIENTE").ToList());
                    }
                    else if (model.Estado == "2")//VENCIDO
                    {
                        return Ok(listaDocumentos.Where(x => x.Estado == "VENCIDO").ToList());
                    }
                }

                return Ok(listaDocumentos);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetDocumentosPendientes";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetExcelDocumentosPendientes"), Authorize]
        public async Task<ActionResult> GetExcelDocumentosPendientes(FiltroCobranzaVm model)
        {
            try
            {
                DocumentosVm documento = new DocumentosVm();
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DocumentosCobranzaVm> listaDocumentos = new List<DocumentosCobranzaVm>();
                List<DocumentosCobranzaVm> listaDocumentosExcel = new List<DocumentosCobranzaVm>();
                listaDocumentos = await sf.GetDocumentosPendientesCobranzaSinFiltroAsync(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento);

                if (!string.IsNullOrEmpty(model.Estado))
                {
                    if (model.Estado == "1")//PENDIENTE
                    {
                        listaDocumentosExcel = listaDocumentos.Where(x => x.Estado == "PENDIENTE").ToList();
                    }
                    else if (model.Estado == "2")//VENCIDO
                    {
                        listaDocumentosExcel = listaDocumentos.Where(x => x.Estado == "VENCIDO").ToList();
                    }
                }
                else
                {
                    listaDocumentosExcel = listaDocumentos;
                }


                Excel excel = new Excel();
                string archivo64 = string.Empty;

                string nombreCaja = string.Empty;
                Stream memoryStream = excel.ExcelDocumentosPendientes(listaDocumentosExcel);

                byte[] bytes;
                using (var arc = new MemoryStream())
                {
                    memoryStream.CopyTo(arc);
                    bytes = arc.ToArray();
                }

                archivo64 = Convert.ToBase64String(bytes);
                documento.NombreArchivo = "Reporte documentos pendientes.xlsx";
                documento.Tipo = "EXCEL";
                documento.Base64 = archivo64;
                return Ok(documento);


            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetExcelDocumentosPendientes";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAnioPagos"), Authorize]
        public async Task<ActionResult> GetAnioPagos()
        {

            try
            {
                List<int> años = new List<int>();

                var config = _context.ConfiguracionPagoClientes.FirstOrDefault();

                for (int i = Convert.ToInt32(config.AnioTributario); i <= DateTime.Now.Year; i++)
                {
                    años.Add(i);
                }

                return Ok(años);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetAnioPagos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetHorariosEnvio"), Authorize]
        public async Task<ActionResult> GetHorariosEnvio()
        {

            try
            {
                var horarios = _context.CobranzaHorarios.ToList();

                var retorno = new { horarios = horarios };
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetHorariosEnvio";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetCantidadDocumentosCobranza"), Authorize]
        public async Task<ActionResult> GetCantidadDocumentosCobranza(FiltroCobranzaVm model)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                int cantidad = sf.GetCantidadDocumentosFiltro(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento, model.CantidadDias, model.Estado, model.ExcluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes);
                return Ok(cantidad);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCantidadDocumentosCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosCobranzaFiltro"), Authorize]
        public async Task<ActionResult> GetDocumentosCobranzaFiltro(FiltroCobranzaVm model)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var documentos = sf.GetDocumentosPendientesCobranzaAsync(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento, model.CantidadDias, model.ExcluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, model.Estado);
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetDocumentosCobranzaFiltro";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosClientes"), Authorize]
        public async Task<ActionResult> GetDocumentosClientes(FiltroCobranzaVm model)
        {
            try
            {
                //Obtiene cantidad correos disponibles 
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var documentos = await sf.GetDocumentosPendientesCobranzaAsync(model.Anio, model.Fecha, model.FechaHasta, model.TipoDocumento, model.CantidadDias, model.ExcluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, model.Estado);

                //Retorna
                List<DocumentoClienteCobranzaVm> retorno = new List<DocumentoClienteCobranzaVm>();

                //Obtenemos los rut de clientes
                var clientes = documentos.Select(x => x.RutCliente).Distinct().ToList();

                foreach (var item in clientes)
                {
                    var docCliente = documentos.Where(x => x.RutCliente == item).ToList();

                    DocumentoClienteCobranzaVm doc = new DocumentoClienteCobranzaVm();
                    doc.RutCliente = item;
                    doc.Bloqueado = docCliente[0].Bloqueado;
                    doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? "Sin Información" : docCliente[0].NombreCliente;
                    doc.CantidadDocumentos = docCliente.Count;
                    doc.MontoDeuda = Convert.ToInt32(docCliente.Sum(x => x.SaldoDocumento));
                    doc.ListaDocumentos = docCliente;

                    var cliente = _context.ClientesPortals.Where(x => x.Rut == item).FirstOrDefault();


                    if (cliente == null)
                    {

                        doc.Selected = false;
                    }
                    else
                    {
                        doc.EmailCliente = cliente.Correo;
                        doc.RutCliente = cliente.Rut;

                    }

                    retorno.Add(doc);
                }

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetDocumentosClientes";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTipoCobranza"), Authorize]
        public async Task<ActionResult> GetTipoCobranza()
        {

            try
            {
                var tipos = _context.TipoCobranzas;
                List<TipoCobranzaVm> retorno = new List<TipoCobranzaVm>();
                foreach (var item in tipos)
                {
                    TipoCobranzaVm tipo = new TipoCobranzaVm();
                    tipo.IdTipoCobranza = item.IdTipoCobranza;
                    tipo.Nombre = item.Nombre;
                    tipo.Descripcion = item.Descripcion;

                    retorno.Add(tipo);
                }

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetTipoCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEstadosCobranza"), Authorize]
        public async Task<ActionResult> GetEstadosCobranza()
        {

            try
            {
                List<EstadoCobranzaVm> retorno = new List<EstadoCobranzaVm>();
                var estado = _context.EstadoCobranzas;

                foreach (var item in estado)
                {
                    EstadoCobranzaVm c = new EstadoCobranzaVm();
                    c.IdEstadoCobranza = item.IdEstadoCobranza;
                    c.Nombre = item.Nombre;

                    retorno.Add(c);
                }


                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetEstadosCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetCobranzasTipo"), Authorize]
        public async Task<ActionResult> GetCobranzasTipo(FiltroCobranzaVm model)
        {
            try
            {
                List<CobranzaCabeceraVM> retorno = new List<CobranzaCabeceraVM>();


                var cobranzas = _context.CobranzaCabeceras.ToList();


                if (model.IdTipoCobranza != 0)
                {
                    cobranzas = cobranzas.Where(x => x.IdTipoCobranza == model.IdTipoCobranza).ToList();
                }

                if (model.TipoProgramacion != 0)
                {
                    cobranzas = cobranzas.Where(x => x.TipoProgramacion == model.TipoProgramacion).ToList();
                }

                if (!string.IsNullOrEmpty(model.NombreCobranza))
                {
                    cobranzas = cobranzas.Where(x => x.Nombre.Contains(model.NombreCobranza)).ToList();
                }

                if (model.IdEstadoCobranza != 0)
                {
                    cobranzas = cobranzas.Where(x => x.IdEstado == model.IdEstadoCobranza).ToList();
                }

                if (model.Fecha != null && model.FechaHasta != null)
                {
                    //cobranzas = cobranzas.Where(x => x.FechaInicio >= model.fecha && x.FechaFin = model.fechaHasta).ToList();
                }

                foreach (var item in cobranzas)
                {
                    CobranzaCabeceraVM cob = new CobranzaCabeceraVM();
                    cob.IdCobranza = item.IdCobranza;
                    cob.Nombre = item.Nombre;
                    cob.FechaCreacion = (DateTime)item.FechaCreacion;
                    cob.HoraCreacion = item.HoraCreacion;
                    cob.IdTipoCobranza = (int)item.IdTipoCobranza;
                    cob.NombreTipoCobranza = item.IdTipoCobranzaNavigation.Nombre;
                    cob.Estado = (int)item.Estado;
                    cob.IdUsuario = (int)item.IdUsuario;
                    var usuario = _context.Usuarios.Find(cob.IdUsuario);
                    if (usuario != null)
                    {
                        cob.NombreUsuario = usuario.Nombres + " " + usuario.Apellidos;
                    }
                    cob.TipoProgramacion = (int)item.TipoProgramacion;
                    cob.FechaInicio = (DateTime)item.FechaInicio;
                    cob.FechaFin = (DateTime)item.FechaFin;
                    cob.HoraDeEnvio = (int)item.HoraDeEnvio;
                    cob.HoraEnvioTexto = ((cob.HoraDeEnvio < 10) ? "0" + cob.HoraDeEnvio.ToString() + ":00" : cob.HoraDeEnvio.ToString() + ":00") + (((cob.HoraDeEnvio < 12) ? " AM" : " PM"));
                    cob.DiaSemanaEnvio = item.DiaSemanaEnvio;
                    cob.DiasToleranciaVencimiento = (int)item.DiasToleranciaVencimiento;
                    cob.IdEstado = (int)item.IdEstado;
                    cob.NombreEstado = item.IdEstadoNavigation.Nombre;
                    cob.Anio = (int)item.Anio;
                    cob.TipoDocumento = item.TipoDocumento;
                    cob.FechaDesde = (DateTime)item.FechaDesde;
                    cob.FechaHasta = (DateTime)item.FechaHasta;
                    cob.AplicaClientesExcluidos = (int)item.AplicaClientesExcluidos;
                    cob.EsCabeceraInteligente = (int)item.EsCabeceraInteligente;
                    cob.IdCabecera = (int)item.IdCabecera;
                    cob.EnviaEnlacePago = (int)item.EnviaEnlacePago;

                    cob.TotalRecaudar = (float)item.CobranzaDetalles.Sum(x => x.Monto);
                    cob.TotalRecaudado = (float)item.CobranzaDetalles.Where(x => x.IdEstado == 5 || x.IdEstado == 4).Sum(x => x.Pagado);
                    cob.CantidadDocumentosEnviadosCobrar = item.CobranzaDetalles.Count;
                    cob.CantidadDocumentosPagados = item.CobranzaDetalles.Where(x => x.IdEstado == 5).ToList().Count;
                    if (cob.TotalRecaudar > 0)
                    {
                        cob.PorcentajeRecaudacion = (cob.TotalRecaudado * 100) / cob.TotalRecaudar;
                    }

                    cob.PorcentajeRecaudacion = Convert.ToSingle(decimal.Round(Convert.ToDecimal(cob.PorcentajeRecaudacion), 2));
                    cob.ColorPorcentajeRecaudacion = (cob.PorcentajeRecaudacion > 70) ? "success" : (cob.PorcentajeRecaudacion > 35) ? "warning" : "danger";
                    if (cob.CantidadDocumentosEnviadosCobrar > 0)
                    {
                        cob.PorcentajePagoDocumentos = (cob.CantidadDocumentosPagados * 100) / cob.CantidadDocumentosEnviadosCobrar;
                    }

                    retorno.Add(cob);
                }

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzasTipo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetCobranzasDetalle"), Authorize]
        public async Task<ActionResult> GetCobranzasDetalle(FiltroCobranzaVm model)
        {
            try
            {
                SoftlandService softlandService = new SoftlandService(_context,_webHostEnvironment);
                var tiposDocs = await softlandService.GetAllTipoDocSoftlandAsync();
                List<CobranzaDetalleVm> retorno = new List<CobranzaDetalleVm>();
                var detalles = _context.CobranzaDetalles.Where(x => x.IdCobranza == model.IdCobranza).ToList();

                foreach (var item in detalles)
                {


                    CobranzaDetalleVm cd = new CobranzaDetalleVm();
                    cd.IdCobranzaDetalle = item.IdCobranzaDetalle;
                    cd.IdCobranza = (int)item.IdCobranza;
                    cd.Folio = (int)item.Folio;
                    cd.FechaEmision = (DateTime)item.FechaEmision;
                    cd.FechaVencimiento = (DateTime)item.FechaVencimiento;
                    cd.Monto = (float)item.Monto;
                    cd.RutCliente = item.RutCliente;

                    var tipoDocumento = tiposDocs.Where(x => x.CodDoc == item.TipoDocumento).FirstOrDefault();
                    if (tipoDocumento != null)
                    {
                        cd.TipoDocumento = tipoDocumento.DesDoc;
                    }

                    cd.IdEstado = (int)item.IdEstado;
                    cd.NombreEstado = item.IdEstadoNavigation.Nombre;
                    //cd.FechaEnvio = (DateTime)item.FechaEnvio;
                    //cd.HoraEnvio = item.HoraEnvio;
                    if (item.FechaPago != null) { cd.FechaPago = (DateTime)item.FechaPago; }
                    cd.HoraPago = item.HoraPago;
                    cd.ComprobanteContable = item.ComprobanteContable;
                    cd.FolioDTE = item.FolioDte;
                    cd.EmailCliente = item.EmailCliente;
                    cd.NombreCliente = item.NombreCliente;
                    cd.FechaPagoTexto = (item.FechaPago != null) ? item.FechaPago.Value.ToShortDateString() : "";
                    retorno.Add(cd);
                }




                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzasDetalle";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCobranzaPeriocidad"), Authorize]
        public async Task<ActionResult> GetCobranzaPeriocidad()
        {

            try
            {
                var periocidad = _context.CobranzaPeriocidads;
                List<CobranzaPeriodicidadVm> retorno = new List<CobranzaPeriodicidadVm>();

                foreach (var item in periocidad)
                {
                    CobranzaPeriodicidadVm pe = new CobranzaPeriodicidadVm();
                    pe.IdPeriocidad = item.IdPeriocidad;
                    pe.Nombre = item.Nombre;
                    pe.DiaMes = (int)item.DiaMes;
                    pe.Estado = (int)item.Estado;
                    retorno.Add(pe);
                }

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzaPeriocidad";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ModificarEstadoCobranzaInteligente/{idCobranza:int}/{estado:int}"), Authorize]
        public async Task<ActionResult> ModificarEstadoCobranzaInteligente(int idCobranza, int estado)
        {

            try
            {
                var cobranza = _context.CobranzaCabeceras.Where(x => x.IdCobranza == idCobranza).FirstOrDefault();

                if (cobranza != null)
                {
                    cobranza.Estado = estado;
                    _context.Entry(cobranza).State = EntityState.Modified;
                    _context.SaveChanges();
                }

                return Ok(idCobranza);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/ModificarEstadoCobranzaInteligente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTiposDocumentosPago"), Authorize]
        public async Task<ActionResult> GetTiposDocumentosPago()
        {

            try
            {
                var configuracion = _context.ConfiguracionPagoClientes.FirstOrDefault();
                var tipoDocumento = new List<TipoDocSoftlandDTO>();
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<TipoDocSoftlandDTO> docsSoftland = await sf.GetAllTipoDocSoftlandAsync();
                foreach (var item in configuracion.TiposDocumentosDeuda.Split(';'))
                {
                    var doc = docsSoftland.Where(x => x.CodDoc == item).FirstOrDefault();

                    if (doc != null)
                    {
                        tipoDocumento.Add(doc);
                    }
                }


                return Ok(tipoDocumento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetTiposDocumentosPago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTiposDocumentosPagoCliente"), Authorize]
        public async Task<ActionResult> GetTiposDocumentosPagoCliente()
        {

            try
            {
                var configuracion = _context.ConfiguracionPagoClientes.FirstOrDefault();
                var tipoDocumento = new List<TipoPago>();
                foreach (var item in configuracion.TiposDocumentosDeuda.Split(';'))
                {
                    var tipo = _context.TipoPagos.Where(x => x.TipoDocumento == item).FirstOrDefault();

                    if (tipo != null)
                    {
                        tipoDocumento.Add(tipo);
                    }
                }


                return Ok(tipoDocumento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetTiposDocumentosPagoCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
