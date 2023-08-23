using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.ModelSoftland;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ApiPortal.Controllers
{
    [EnableCors()]
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetDocumentosClientes2Async";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            List<DocumentoClienteCobranzaVm> retorno = new List<DocumentoClienteCobranzaVm>();

            try
            {
                //Obtiene cantidad correos disponibles 
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                int excluyeClientes = (int)(model.ExcluyeClientes == null ? 0 : model.ExcluyeClientes);
                var documentos = await sf.GetDocumentosPendientesCobranzaAsync( model.TipoDocumento, model.CantidadDias, excluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, logApi.Id, model.Estado);

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

                    var cliente = await sf.GetClienteSoftlandAsync(string.Empty, doc.RutCliente, logApi.Id);
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
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"cobranza\GetDocumentosCobranzaFiltro2";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

            }

            logApi.Termino = DateTime.Now;
            logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
            sf.guardarLogApi(logApi);

            return retorno;
        }


        [HttpPost("SaveCobranza"), Authorize]
        public async Task<ActionResult> SaveCobranza(CobranzaCabeceraVM model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/SaveCobranza";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(idCabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/EnviaCobranza";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            string estadoLogC = string.Empty;
            LogCobranza lc = new LogCobranza();
            lc.FechaInicio = DateTime.Now;
            lc.Estado = "PROCESANDO";
            _context.LogCobranzas.Add(lc);
            _context.SaveChanges();


            try
            {

                int horaActual = DateTime.Now.Hour;
                int DiaActual = DateTime.Now.Day;
                DateTime fecha = DateTime.Now;
                DateTime fechaActual = new DateTime(fecha.Year, fecha.Month, fecha.Day, 23, 59, 59);
                var diasFeriados = _context.Feriados.ToList();
                
                var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string urlFrot = System.Configuration.ConfigurationManager.AppSettings["URL_FRONT"];

                //Obtiene correos disponibles
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();



                //Obtenemos tipos de documentos
                var tiposDocumentos = await sf.GetAllTipoDocSoftlandAsync(logApi.Id);

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
                    var listaDocPendientes = _context.CobranzaDetalles.Where(x => x.IdEstado == 1 && x.IdCobranza == item.IdCobranza).ToList();

                    //Obtenemos los clientes a los que se le enviara la cobranza
                    var clientes = listaDocPendientes.Select(x => x.RutCliente).Distinct().ToList();
                    List<DetalleEnvioCobranzaVm> listaEnvio = new List<DetalleEnvioCobranzaVm>();

                    //Recorremes y seleccionamos los documentos por clientes
                    foreach (var al in clientes)
                    {
                        var docCliente = listaDocPendientes.Where(x => x.RutCliente == al).ToList();

                        var contactos = await sf.GetContactosClienteAsync(docCliente[0].CodAuxCliente, logApi.Id);
                        var cliente = await sf.GetClienteSoftlandAsync(docCliente[0].CodAuxCliente, string.Empty, logApi.Id);
                        if (item.EnviaTodosCargos == 1)
                        {

                            if (contactos.Count == 0 && item.EnviaCorreoFicha == 1)
                            {

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

                                if (string.IsNullOrEmpty(correos) && item.EnviaCorreoFicha == 1)
                                {
                                    correos = cliente.Correo;
                                }
                                docCliente[0].EmailCliente = correos;
                            }
                        }
                        else
                        {
                            if (contactos.Count == 0 && item.EnviaCorreoFicha == 1)
                            {
                                string correos = string.Empty;
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

                                if (string.IsNullOrEmpty(correos) && item.EnviaTodosContactos == 1)
                                {
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
                                }

                                if (string.IsNullOrEmpty(correos) && item.EnviaCorreoFicha == 1)
                                {
                                    correos = cliente.Correo;
                                }
                                docCliente[0].EmailCliente = correos;
                            }
                        }


                        DetalleEnvioCobranzaVm doc = new DetalleEnvioCobranzaVm();
                        doc.RutCliente = al;
                        doc.NombreCliente = (string.IsNullOrEmpty(docCliente[0].NombreCliente)) ? string.Empty : docCliente[0].NombreCliente;
                        doc.EmailCliente = docCliente[0].EmailCliente;
                        doc.CantidadDocumentosPendientes = docCliente.Count;
                        doc.MontoDeuda = Convert.ToDecimal(docCliente.Sum(x => x.Monto));
                        doc.ListaDocumentos = new List<DocumentosCobranzaVM>();
                        doc.CodAux = docCliente[0].CodAuxCliente;
                        doc.IdCobranza = item.IdCobranza;

                        //Agregamos documentos
                        foreach (var d in docCliente)
                        {
                            DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                            aux.Folio = (int)d.Folio;
                            aux.FechaEmision = (DateTime)d.FechaEmision;
                            aux.FechaVencimiento = (DateTime)d.FechaVencimiento;
                            aux.Monto = (decimal)d.Monto;
                            aux.TipoDocumento = tiposDocumentos.Where(x => x.CodDoc == d.TipoDocumento).FirstOrDefault().DesDoc;
                            doc.ListaDocumentos.Add(aux);
                        }

                        //Agrega detalle para envió
                        listaEnvio.Add(doc);
                    }

                    int IdEstadoFinal = 3; //ESTADO ENVIADA

                    //Recorremos resultado de cobranza, generamos documento y enviamos correo
                    MailService emailService = new MailService(_context, _webHostEnvironment);
                    string response = await emailService.EnviaCobranzaAsync(listaEnvio, item);
                    if (!string.IsNullOrEmpty(response))
                    {
                        if (response == "2")
                        {
                            IdEstadoFinal = 2;
                        }
                        else
                        {
                            string[] spliteResponse = response.Split('-');
                            estadoLogC = spliteResponse[0];
                            if (spliteResponse.Length > 1)
                            {
                                IdEstadoFinal = int.Parse(spliteResponse[1]);
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


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(1);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetCobranzaCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            
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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);


                return Ok(retorno);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetDocumentosPendientes";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
        

            try
            {
                List<DocumentosCobranzaVm> listaDocumentos = new List<DocumentosCobranzaVm>();
                listaDocumentos = await sf.GetDocumentosPendientesCobranzaSinFiltroAsync( model.TipoDocumento, logApi.Id);

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listaDocumentos);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetDocumentosPendientes";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

      

        [HttpGet("GetAnioPagos"), Authorize]
        public async Task<ActionResult> GetAnioPagos()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetAnioPagos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                List<int> años = new List<int>();

                var config = _context.ConfiguracionPagoClientes.FirstOrDefault();

                for (int i = Convert.ToInt32(config.AnioTributario); i <= DateTime.Now.Year; i++)
                {
                    años.Add(i);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(años);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetHorariosEnvio";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var horarios = _context.CobranzaHorarios.ToList();

                var retorno = new { horarios = horarios };

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetHorariosEnvio";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetDocumentosCobranzaFiltro"), Authorize]
        public async Task<ActionResult> GetDocumentosCobranzaFiltro(FiltroCobranzaVm model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetDocumentosCobranzaFiltro";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            if (model == null)
            {
                return BadRequest();
            }

            try
            {
                int excluyeClientes = (int)(model.ExcluyeClientes == null ? 0 : model.ExcluyeClientes);
                var documentos = sf.GetDocumentosPendientesCobranzaAsync(model.TipoDocumento, model.CantidadDias, excluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, logApi.Id, model.Estado);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documentos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetDocumentosClientes";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                //Obtiene cantidad correos disponibles 
                MailService mail = new MailService(_context,_webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                int excluyeClientes = (int)(model.ExcluyeClientes == null ? 0 : model.ExcluyeClientes);
                var documentos = await sf.GetDocumentosPendientesCobranzaAsync(model.TipoDocumento, model.CantidadDias, excluyeClientes, model.ListasPrecio, model.CondicionesVenta, model.Vendedores, model.CategoriasClientes, model.CanalesVenta, model.Cobradores, logApi.Id, model.Estado);

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetTipoCobranza";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetEstadosCobranza";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetCobranzasTipo";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
          

            try
            {
                List<CobranzaCabeceraVM> retorno = new List<CobranzaCabeceraVM>();


                var cobranzas = _context.CobranzaCabeceras.ToList();


                if (model.IdTipoCobranza != 0 && model.IdTipoCobranza != null)
                {
                    cobranzas = cobranzas.Where(x => x.IdTipoCobranza == model.IdTipoCobranza).ToList();
                }

                if (model.TipoProgramacion != 0 && model.IdTipoCobranza != null)
                {
                    cobranzas = cobranzas.Where(x => x.TipoProgramacion == model.TipoProgramacion).ToList();
                }

                if (!string.IsNullOrEmpty(model.NombreCobranza))
                {
                    cobranzas = cobranzas.Where(x => x.Nombre.Contains(model.NombreCobranza)).ToList();
                }

                if (model.IdEstadoCobranza != 0 && model.IdEstadoCobranza != null)
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
                    var tipoCobranza = _context.TipoCobranzas.Where(x => x.IdTipoCobranza == item.IdTipoCobranza).FirstOrDefault();
                    cob.NombreTipoCobranza = tipoCobranza.Nombre;
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
                    var estadoCobranza = _context.EstadoCobranzas.Where(x => x.IdEstadoCobranza == item.IdEstado).FirstOrDefault();
                    cob.NombreEstado = estadoCobranza.Nombre;
                    cob.Anio = (int)item.Anio;
                    cob.TipoDocumento = item.TipoDocumento;
                    cob.FechaDesde = item.FechaDesde == null ? null : (DateTime)item.FechaDesde;
                    cob.FechaHasta = item.FechaHasta == null ? null : (DateTime)item.FechaHasta;
                    cob.AplicaClientesExcluidos = (int)item.AplicaClientesExcluidos;
                    cob.EsCabeceraInteligente = (int)item.EsCabeceraInteligente;
                    cob.IdCabecera = (int)item.IdCabecera;
                    cob.EnviaEnlacePago = (int)item.EnviaEnlacePago;

                    retorno.Add(cob);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzasTipo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCobranzaGraficos/{id}"), Authorize]
        public async Task<ActionResult> GetCobranzaGraficos(int id)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetCobranzaGraficos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                CobranzaCabeceraVM retorno = new CobranzaCabeceraVM();


                var cobranza = _context.CobranzaCabeceras.Where(x => x.IdCobranza == id).FirstOrDefault();

                if (cobranza != null)
                {
                    retorno.IdCobranza = cobranza.IdCobranza;
                    retorno.Nombre = cobranza.Nombre;
                    retorno.FechaCreacion = (DateTime)cobranza.FechaCreacion;
                    retorno.HoraCreacion = cobranza.HoraCreacion;
                    retorno.IdTipoCobranza = (int)cobranza.IdTipoCobranza;

                    var tipoCobranza = _context.TipoCobranzas.Where(x => x.IdTipoCobranza == cobranza.IdTipoCobranza).FirstOrDefault();
                    retorno.NombreTipoCobranza = tipoCobranza.Nombre;
                    retorno.Estado = (int)cobranza.Estado;
                    retorno.IdUsuario = (int)cobranza.IdUsuario;
                    var usuario = _context.Usuarios.Find(retorno.IdUsuario);
                    if (usuario != null)
                    {
                        retorno.NombreUsuario = usuario.Nombres + " " + usuario.Apellidos;
                    }
                    retorno.TipoProgramacion = (int)cobranza.TipoProgramacion;
                    retorno.FechaInicio = (DateTime)cobranza.FechaInicio;
                    retorno.FechaFin = (DateTime)cobranza.FechaFin;
                    retorno.HoraDeEnvio = (int)cobranza.HoraDeEnvio;
                    retorno.HoraEnvioTexto = ((retorno.HoraDeEnvio < 10) ? "0" + retorno.HoraDeEnvio.ToString() + ":00" : retorno.HoraDeEnvio.ToString() + ":00") + (((retorno.HoraDeEnvio < 12) ? " AM" : " PM"));
                    retorno.DiaSemanaEnvio = cobranza.DiaSemanaEnvio;
                    retorno.DiasToleranciaVencimiento = (int)cobranza.DiasToleranciaVencimiento;
                    retorno.IdEstado = (int)cobranza.IdEstado;

                    var estadoCobranza = _context.EstadoCobranzas.Where(x => x.IdEstadoCobranza == cobranza.IdEstado).FirstOrDefault();
                    retorno.NombreEstado = estadoCobranza.Nombre;
                    retorno.Anio = (int)cobranza.Anio;
                    retorno.TipoDocumento = cobranza.TipoDocumento;
                    retorno.FechaDesde = cobranza.FechaDesde == null ? null : (DateTime)cobranza.FechaDesde;
                    retorno.FechaHasta = cobranza.FechaHasta == null ? null : (DateTime)cobranza.FechaHasta;
                    retorno.AplicaClientesExcluidos = (int)cobranza.AplicaClientesExcluidos;
                    retorno.EsCabeceraInteligente = (int)cobranza.EsCabeceraInteligente;
                    retorno.IdCabecera = (int)cobranza.IdCabecera;
                    retorno.EnviaEnlacePago = (int)cobranza.EnviaEnlacePago;

                    var detalleCobranza = _context.CobranzaDetalles.Where(x => x.IdCobranza == cobranza.IdCobranza).ToList();
                    retorno.TotalRecaudar = (float)detalleCobranza.Sum(x => x.Monto);
                    retorno.TotalRecaudado = (float)detalleCobranza.Where(x => x.IdEstado == 5 || x.IdEstado == 4).Sum(x => x.Pagado);
                    retorno.CantidadDocumentosEnviadosCobrar = detalleCobranza.Count;
                    retorno.CantidadDocumentosPagados = detalleCobranza.Where(x => x.IdEstado == 5).ToList().Count;
                    if (retorno.TotalRecaudar > 0)
                    {
                        retorno.PorcentajeRecaudacion = (retorno.TotalRecaudado * 100) / retorno.TotalRecaudar;
                    }

                    retorno.PorcentajeRecaudacion = Convert.ToSingle(decimal.Round(Convert.ToDecimal(retorno.PorcentajeRecaudacion), 2));
                    retorno.ColorPorcentajeRecaudacion = (retorno.PorcentajeRecaudacion > 70) ? "success" : (retorno.PorcentajeRecaudacion > 35) ? "warning" : "danger";
                    if (retorno.CantidadDocumentosEnviadosCobrar > 0)
                    {
                        retorno.PorcentajePagoDocumentos = (retorno.CantidadDocumentosPagados * 100) / retorno.CantidadDocumentosEnviadosCobrar;
                    }
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"cobranza\GetCobranzaGraficos";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

                return BadRequest();
            }
        }

        [HttpPost("GetCobranzasDetalle"), Authorize]
        public async Task<ActionResult> GetCobranzasDetalle(FiltroCobranzaVm model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetCobranzasDetalle";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                if(model == null)
                {
                    return BadRequest();
                }
                SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                var tiposDocs = await softlandService.GetAllTipoDocSoftlandAsync(logApi.Id);
                List<CobranzaDetalleVm> retorno = new List<CobranzaDetalleVm>();
                var detalles = _context.CobranzaDetalles.Where(x => x.IdCobranza == model.IdCobranza).ToList();
                List<DocumentoContabilizadoAPIDTO> documentosActualizados = new List<DocumentoContabilizadoAPIDTO>();
                var codigosAuxiliares = detalles.GroupBy(x => x.CodAuxCliente).Distinct().ToList();
                foreach (var item in codigosAuxiliares)
                {
                    var documentos = await softlandService.GetAllDocumentosContabilizadosCliente(item.Key, logApi.Id);
                    documentosActualizados.AddRange(documentos);
                }

                foreach (var item in detalles)
                {

                    var docActualizado = documentosActualizados.Where(x => x.CodAux == item.CodAuxCliente && item.Folio == x.Numdoc && item.TipoDocumento == x.Ttdcod).FirstOrDefault();
                    if (docActualizado != null)
                    {

                        item.Monto = (float?)docActualizado.MovMonto;
                        if (docActualizado.Saldobase < docActualizado.MovMonto)
                        {
                            item.IdEstado = 4;
                            if (docActualizado.Saldobase <= 0)
                            {
                                item.Pagado = (float?)docActualizado.MovMonto;
                                item.IdEstado = 5;
                            }
                            else
                            {
                                item.Pagado = (float?)(docActualizado.MovMonto - docActualizado.Saldobase);
                            }
                        }

                        _context.Entry(item).State = EntityState.Modified;
                    }

                    CobranzaDetalleVm cd = new CobranzaDetalleVm();

                    cd.Abonos = documentosActualizados.Where(x => x.MovNumDocRef == item.Folio && x.Numdoc != item.Folio).ToList();
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
                    cd.CodTipoDocumento = tipoDocumento.CodDoc;
                    var estadoCobranza = _context.EstadoCobranzas.Where(x => x.IdEstadoCobranza == item.IdEstado).FirstOrDefault();
                    cd.NombreEstado = estadoCobranza.Nombre;
                    //cd.FechaEnvio = (DateTime)item.FechaEnvio;
                    //cd.HoraEnvio = item.HoraEnvio;
                    if (item.FechaPago != null) { cd.FechaPago = (DateTime)item.FechaPago; }
                    cd.HoraPago = item.HoraPago;
                    cd.ComprobanteContable = item.ComprobanteContable;
                    cd.FolioDTE = item.FolioDte;
                    cd.EmailCliente = item.EmailCliente;
                    cd.NombreCliente = item.NombreCliente;
                    cd.FechaPagoTexto = (item.FechaPago != null) ? item.FechaPago.Value.ToShortDateString() : "";
                    cd.CodAuxCliente = item.CodAuxCliente;
                    retorno.Add(cd);
                }

                _context.SaveChanges();



                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetCobranzaPeriocidad";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetCobranzaPeriocidad";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }    

        [HttpGet("GetTiposDocumentosPago"), Authorize]
        public async Task<ActionResult> GetTiposDocumentosPago()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetTiposDocumentosPago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var configuracion = _context.ConfiguracionPagoClientes.FirstOrDefault();
                var tipoDocumento = new List<TipoDocSoftlandDTO>();
                List<TipoDocSoftlandDTO> docsSoftland = await sf.GetAllTipoDocSoftlandAsync(logApi.Id);
                if(!string.IsNullOrEmpty(configuracion.TiposDocumentosDeuda))
                {
                    foreach (var item in configuracion.TiposDocumentosDeuda.Split(';'))
                    {
                        var doc = docsSoftland.Where(x => x.CodDoc == item).FirstOrDefault();

                        if (doc != null)
                        {
                            tipoDocumento.Add(doc);
                        }
                    }
                }
                else
                {
                    tipoDocumento.AddRange(docsSoftland);
                }


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(tipoDocumento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/GetTiposDocumentosPagoCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(tipoDocumento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/GetTiposDocumentosPagoCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("DeleteCobranza/{idCobranza}"), Authorize]
        public async Task<ActionResult> DeleteCobranza(int idCobranza)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/cobranza/DeleteCobranza";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                var detalles = _context.CobranzaDetalles.Where(x => x.IdCobranza == idCobranza);

                if(detalles.Count() > 0)
                {
                    _context.CobranzaDetalles.RemoveRange(detalles);
                    _context.SaveChanges();
                }
             

                var cobranza = _context.CobranzaCabeceras.Where(x => x.IdCobranza == idCobranza).FirstOrDefault();
                if(cobranza != null)
                {
                    _context.CobranzaCabeceras.Remove(cobranza);
                    _context.SaveChanges();
                }
             
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Cobranza/DeleteCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
