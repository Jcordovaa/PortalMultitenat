using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.ModelSoftland;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoftlandController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SoftlandController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
        }

        [HttpGet("GetAllTipoDocSoftland"), Authorize]
        public async Task<ActionResult> GetAllTipoDocSoftland()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<TipoDocSoftlandDTO> aux = await sf.GetAllTipoDocSoftlandAsync();
                return Ok(aux);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetAllTipoDocSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllCuentasContablesSoftland"), Authorize]
        public async Task<ActionResult> GetAllCuentasContablesSoftland()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                List<CuentasContablesSoftlandDTO> aux = await sf.GetAllCuentasContablesSoftlandAsync();
                return Ok(aux);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetAllCuentasContablesSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesAcceso"), Authorize]
        public async Task<ActionResult> GetClientesAcceso(FilterVm value)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var listaClientes = _context.ClientesPortals.Where(x => x.Clave == "" && x.ActivaCuenta == 0).ToList();
                var listaClientesSoftland = await sf.BuscarClienteSoftlandAsync(value.CodAux, value.Rut, value.Nombre);

                //Agregar contactos a clientes
                foreach (var item in listaClientesSoftland)
                {
                    item.Contactos = await sf.GetAllContactosAsync(item.CodAux); 
                }

                if (value.TipoBusqueda == 2)
                {
                    //Remueve todos los que tengan accesos
                    foreach (var item in listaClientesSoftland)
                    {
                        var existe = listaClientes.Where(x => x.CodAux == item.CodAux && x.Rut == item.Rut).ToList();

                        if (existe.Count > 0)
                        {
                            listaClientesSoftland.Remove(item);
                        }
                    }
                }

                return Ok(listaClientesSoftland);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetClientesAcceso";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUbicaciones"), Authorize]
        public async Task<ActionResult> GetUbicaciones()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var regiones = await sf.GetRegionesSoftland();
                var comunas = await sf.GetComunasSoftlandAsync();

                var ubicaciones = new UbicacionVm
                {
                    Regiones = regiones,
                    Comunas = comunas
                };

                return Ok(ubicaciones);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetUbicaciones";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetGiros"), Authorize]
        public async Task<ActionResult> GetGiros()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var giros = await sf.GetGirosSoftlandAsync();

                return Ok(giros);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetGiros";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetVendedores"), Authorize]
        public async Task<ActionResult> GetVendedores()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var vendedores = await sf.GetVenedoresSoftlandAsync();

                return Ok(vendedores);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetVendedores";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCategoriasCliente"), Authorize]
        public async Task<ActionResult> GetCategoriasCliente()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var categoriasCliente = await sf.GetCategoriasClienteAsync();

                return Ok(categoriasCliente);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCategoriasCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCondVentas"), Authorize]
        public async Task<ActionResult> GetCondVentas()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var condicionesVenta = await sf.GetCondVentaAsync();

                return Ok(condicionesVenta);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCondVentas";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetListasPrecio"), Authorize]
        public async Task<ActionResult> GetListasPrecio()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var listas = await sf.GetListPrecioAsync();

                return Ok(listas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetListasPrecio";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMonedas"), Authorize]
        public async Task<ActionResult> GetMonedas()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var monedas = await sf.GetMonedasAsync();

                return Ok(monedas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetMonedas";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesAcces"), Authorize]
        public async Task<ActionResult> GetClientesAcces(FilterVm value)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var listaClientes = _context.ClientesPortals.ToList();
                var listaClientesSoftland = await sf.BuscarClienteSoftland2Async(value.CodAux, value.Rut, value.Nombre);

                //Agregar contactos a clientes
                foreach (var item in listaClientesSoftland)
                {


                    if (string.IsNullOrEmpty(item.Correo))
                    {
                        item.Contactos = await sf.GetAllContactosAsync(item.CodAux);
                        foreach (var c in item.Contactos)
                        {
                            if (!string.IsNullOrEmpty(c.Correo))
                            {
                                item.Correo = c.Correo;
                                break;
                            }
                        }
                    }
                   
                }

                if (value.TipoBusqueda == 2)
                {
                    //Remueve todos los que tengan accesos
                    foreach (var item in listaClientesSoftland)
                    {
                        var existe = listaClientes.Where(x => x.CodAux == item.CodAux && x.Rut == item.Rut).ToList();

                        if (existe.Count > 0)
                        {
                            listaClientesSoftland.Remove(item);
                        }
                    }
                }

                return Ok(listaClientesSoftland);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetClientesAcces";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCargos"), Authorize]
        public async Task<ActionResult> GetCargos()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var cargos = await sf.GetCargosAsync();

                return Ok(cargos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCargos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCentrosCostosActivos"), Authorize]
        public async Task<ActionResult> GetCentrosCostosActivos()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var centros = sf.GetCentrosCostoActivos();

                return Ok(centros);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCentrosCostosActivos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAreasNegocio"), Authorize]
        public async Task<ActionResult> GetAreasNegocio()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var areas = sf.GetAreaNegocio();

                return Ok(areas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetAreasNegocio";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCobradores"), Authorize]
        public async Task<ActionResult> GetCobradores()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var cobradores = sf.GetCobradores();

                return Ok(cobradores);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCobradores";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCanalesVenta"), Authorize]
        public async Task<ActionResult> GetCanalesVenta()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var canalesVenta = sf.GetCanalesVenta();

                return Ok(canalesVenta);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCanalesVenta";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetExistModuloInventario"), Authorize]
        public async Task<ActionResult> GetExistModuloInventario()
        {

            try
            {
                bool existModulo = false;
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var modulos = await sf.GetModulosSoftlandAsync();

                var m = modulos.Where(x => x.Codi == "IW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetExistModuloInventario";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetExistModuloNotaVenta"), Authorize]
        public async Task<ActionResult> GetExistModuloNotaVenta()
        {

            try
            {
                bool existModulo = false;
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var modulos = await sf.GetModulosSoftlandAsync();

                var m = modulos.Where(x => x.Codi == "NW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetExistModuloNotaVenta";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetExistModuloContabilidad"), Authorize]
        public async Task<ActionResult> GetExistModuloContabilidad()
        {

            try
            {
                bool existModulo = false;
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var modulos = await sf.GetModulosSoftlandAsync();

                var m = modulos.Where(x => x.Codi == "CW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetExistModuloContabilidad";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ReprocesarPago/{idPago}"), Authorize]
        public async Task<ActionResult> ReprocesarPago(int idPago)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var numComprobante = await sf.ReprocesaPago(idPago);
                return Ok(numComprobante);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/ReprocesarPago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaComprobante"), Authorize]
        public async Task<ActionResult> ActualizaComprobante(PagoCabeceraVm p)
        {
            try
            {
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == p.IdPago).FirstOrDefault();
                if (pago != null)
                {
                    pago.ComprobanteContable = p.ComprobanteContable;
                    pago.IdPagoEstado = 2;
                    _context.PagosCabeceras.Attach(pago);

                    _context.Entry(pago).Property(x => x.ComprobanteContable).IsModified = true;
                    _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                    _context.SaveChanges();




                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                    string fecha = pago.FechaPago.Value.Day.ToString() + "/" + pago.FechaPago.Value.Month.ToString() + "/" + pago.FechaPago.Value.Year.ToString();
                    string hora = pago.HoraPago;
                    string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                    string comprobanteHtml = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath,"~/Uploads/MailTemplates/invoice.html")))
                    {
                        comprobanteHtml = reader.ReadToEnd();
                    }
                    //string comprobanteHtml = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/MailTemplates/invoice.html"));
                    comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                        .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", pago.ComprobanteContable).Replace("{MONTOTOTAL}", pago.MontoPago.Value.ToString("N0"));

                    string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync();
                    foreach (var det in pago.PagosDetalles)
                    {
                        var tipoDoc = tiposDocumentos.Where(x => x.CodDoc == det.TipoDocumento).FirstOrDefault();

                        reemplazoDetalle = reemplazoDetalle + partes[1].Replace("{NUMDOC}", det.Folio.ToString()).Replace("{TIPODOC}", tipoDoc.DesDoc).Replace("{MONTODOC}", det.Total.Value.ToString("N0")).Replace("{SALDODOC}", det.Saldo.Value.ToString("N0"))
                            .Replace("{PAGADODOC}", det.Apagar.Value.ToString("N0"));

                    }

                    partes[1] = reemplazoDetalle;

                    string htmlFinal = string.Empty;

                    foreach (var pa in partes)
                    {
                        htmlFinal = htmlFinal + pa;
                    }

                    SelectPdf.HtmlToPdf converter2 = new SelectPdf.HtmlToPdf();
                    SelectPdf.PdfDocument doc2 = converter2.ConvertHtmlString(htmlFinal);


                    MailViewModel vm = new MailViewModel();
                    List<Attachment> listaAdjuntos = new List<Attachment>();

                    using (MemoryStream memoryStream2 = new MemoryStream())
                    {
                        doc2.Save(memoryStream2);

                        byte[] bytes = memoryStream2.ToArray();

                        memoryStream2.Close();
                        Attachment comprobanteTBK = new Attachment(new MemoryStream(bytes), "Comprobante Pago.pdf");
                        listaAdjuntos.Add(comprobanteTBK);
                    }
                    doc2.Close();


                    vm.adjuntos = listaAdjuntos;
                    vm.tipo = 5;
                    vm.nombre = pago.Nombre;
                    vm.email_destinatario = pago.Correo;
                    MailService mail = new MailService(_context,_webHostEnvironment);
                    await mail.EnviarCorreosAsync(vm);

                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/ActualizaComprobante";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
