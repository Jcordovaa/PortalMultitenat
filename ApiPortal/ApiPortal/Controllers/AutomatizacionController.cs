using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                LogApi logApi = new LogApi();
                logApi.Api = "api/Automatizacion/GetAutomatizaciones";
                logApi.Inicio = DateTime.Now;
                logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

                var automatizaciones = _context.Automatizacions.ToList();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
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
            LogApi logApi = new LogApi();
            logApi.Api = "api/Automatizacion/EnviaAutomatizaciones";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            int horaActual = DateTime.Now.Hour;
            string estadoLogC = string.Empty;
            LogCobranza lc = new LogCobranza();
            lc.FechaInicio = DateTime.Now;
            lc.Estado = "PROCESANDO";
            _context.LogCobranzas.Add(lc);
            _context.SaveChanges();
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);

            try
            {
                Generador genera = new Generador(_context, _webHostEnvironment);
                var auxCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var auxEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string urlFrot = System.Configuration.ConfigurationManager.AppSettings["URL_FRONT"];

                //Obtiene correos disponibles
                MailService mail = new MailService(_context, _webHostEnvironment);
                int correosDisponibles = mail.calculaDisponiblesCobranza();

                //Obtenemos tipos de documentos
                var tiposDocumentos = await sf.GetAllTipoDocSoftlandAsync(logApi.Id);

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





                    var documentos = await sf.GetDocumentosPendientesCobranzaSinFiltroAsync((int)automatizacion.Anio, fechaDesde, fechaHasta, automatizacion.TipoDocumentos, logApi.Id);

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
                            documentos = documentos.Where(x => (x.FechaVencimiento.Date - DateTime.Now.Date).TotalDays == automatizacion.DiasVencimiento && (x.FechaVencimiento.Date - DateTime.Now.Date).TotalDays > 0).ToList();
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
                        var clienteApi = await sf.BuscarClienteSoftland2Async(string.Empty, al, string.Empty, logApi.Id);

                        if (clienteApi.Count == 0)
                        {
                            continue;
                        }

                        //if (!automatizacion.CodCanalVenta.Contains(clienteApi[0].v))
                        //{

                        //}

                        if (!string.IsNullOrEmpty(automatizacion.CodCategoriaCliente))
                        {
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

                        }


                        if (!string.IsNullOrEmpty(automatizacion.CodCobrador))
                        {
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
                        }


                        if (!string.IsNullOrEmpty(automatizacion.CodCondicionVenta))
                        {
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
                        }


                        if (!string.IsNullOrEmpty(automatizacion.CodListaPrecios))
                        {
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
                        }


                        if (!string.IsNullOrEmpty(automatizacion.CodVendedor))
                        {
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
                        }





                        var docCliente = documentos.Where(x => x.RutCliente == al).ToList();

                        var contactos = await sf.GetContactosClienteAsync(clienteApi[0].CodAux, logApi.Id);
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

                        if (string.IsNullOrEmpty(correos) && automatizacion.EnviaCorreoFicha == 1)
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
                                doc.CodAux = clienteApi[0].CodAux;

                                AutomatizacionVm aut = new AutomatizacionVm();

                                aut.AgrupaCobranza = automatizacion.AgrupaCobranza;
                                aut.Anio = automatizacion.Anio;
                                aut.CodCanalVenta = automatizacion.CodCanalVenta;
                                aut.CodCargo = automatizacion.CodCargo;
                                aut.CodCategoriaCliente = automatizacion.CodCategoriaCliente;
                                aut.CodCobrador = automatizacion.CodCobrador;
                                aut.CodCondicionVenta = automatizacion.CodCondicionVenta;
                                aut.CodListaPrecios = automatizacion.CodListaPrecios;
                                aut.CodVendedor = automatizacion.CodVendedor;
                                aut.DiaEnvio = automatizacion.DiaEnvio;
                                aut.DiasRecordatorio = automatizacion.DiasRecordatorio;
                                aut.DiasVencimiento = automatizacion.DiasVencimiento;
                                aut.EnviaCorreoFicha = (int)automatizacion.EnviaCorreoFicha;
                                aut.EnviaTodosContactos = (int)automatizacion.EnviaTodosContactos;
                                aut.Estado = automatizacion.Estado;
                                aut.ExcluyeClientes = automatizacion.ExcluyeClientes;
                                aut.ExcluyeFestivos = automatizacion.ExcluyeFestivos;
                                aut.IdAutomatizacion = automatizacion.IdAutomatizacion;
                                aut.IdHorario = automatizacion.IdHorario;
                                aut.IdPerioricidad = automatizacion.IdPerioricidad;
                                aut.IdTipoAutomatizacion = automatizacion.IdTipoAutomatizacion;
                                aut.MuestraSoloVencidos = automatizacion.MuestraSoloVencidos;

                                List<int> folios = new List<int>();
                                folios.Add(item.FolioDocumento);
                                aut.NumDoc = folios;
                                aut.TipoDocumentos = automatizacion.TipoDocumentos;

                                doc.AutomatizacionJson = JsonConvert.SerializeObject(aut);

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
                            doc.CodAux = clienteApi[0].CodAux;

                            AutomatizacionVm aut = new AutomatizacionVm();

                            aut.AgrupaCobranza = automatizacion.AgrupaCobranza;
                            aut.Anio = automatizacion.Anio;
                            aut.CodCanalVenta = automatizacion.CodCanalVenta;
                            aut.CodCargo = automatizacion.CodCargo;
                            aut.CodCategoriaCliente = automatizacion.CodCategoriaCliente;
                            aut.CodCobrador = automatizacion.CodCobrador;
                            aut.CodCondicionVenta = automatizacion.CodCondicionVenta;
                            aut.CodListaPrecios = automatizacion.CodListaPrecios;
                            aut.CodVendedor = automatizacion.CodVendedor;
                            aut.DiaEnvio = automatizacion.DiaEnvio;
                            aut.DiasRecordatorio = automatizacion.DiasRecordatorio;
                            aut.DiasVencimiento = automatizacion.DiasVencimiento;
                            aut.EnviaCorreoFicha = (int)automatizacion.EnviaCorreoFicha;
                            aut.EnviaTodosContactos = (int)automatizacion.EnviaTodosContactos;
                            aut.Estado = automatizacion.Estado;
                            aut.ExcluyeClientes = automatizacion.ExcluyeClientes;
                            aut.ExcluyeFestivos = automatizacion.ExcluyeFestivos;
                            aut.IdAutomatizacion = automatizacion.IdAutomatizacion;
                            aut.IdHorario = automatizacion.IdHorario;
                            aut.IdPerioricidad = automatizacion.IdPerioricidad;
                            aut.IdTipoAutomatizacion = automatizacion.IdTipoAutomatizacion;
                            aut.MuestraSoloVencidos = automatizacion.MuestraSoloVencidos;
                            List<int> folios = new List<int>();

                            foreach (var item in docCliente)
                            {
                                folios.Add(item.FolioDocumento);
                            }
                            aut.NumDoc = folios;
                            aut.TipoDocumentos = automatizacion.TipoDocumentos;

                            doc.AutomatizacionJson = JsonConvert.SerializeObject(aut);

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

                    MailService mailService = new MailService(_context, _webHostEnvironment);
                    int idEstadoEnvio = await mailService.EnviaAutomatizacionAsync(listaEnvio, automatizacion);
                    if (idEstadoEnvio != 0) { IdEstadoFinal = idEstadoEnvio; }

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Automatizacion/GuardaAutomatizacion";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           
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
                    aut.IdTipoAutomatizacion = (int)automatizacion.IdTipoAutomatizacion;
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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
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

    

        [HttpGet("GetTipos"), Authorize]
        public ActionResult<object> GetTipos()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Automatizacion/GetTipos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           
            try
            {
                var tipos = _context.TipoAutomatizacions.ToList();
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
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
