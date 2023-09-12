using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.Enums;
using ApiPortal.ModelSoftland;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using MercadoPago.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class SoftlandController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;

        public SoftlandController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("GetAllTipoDocSoftland"), Authorize]
        public async Task<ActionResult> GetAllTipoDocSoftland()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetAllTipoDocSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                List<TipoDocSoftlandDTO> aux = await sf.GetAllTipoDocSoftlandAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(aux);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetAllCuentasContablesSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                List<CuentasContablesSoftlandDTO> aux = await sf.GetAllCuentasContablesSoftlandAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(aux);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetAllCuentasContablesSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCuentasContablePago"), Authorize]
        public async Task<ActionResult> GetCuentasContablePago()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCuentasContablePago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<CuentasContablesSoftlandDTO> aux = await sf.getCuentasContablePagoAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(aux);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetCuentasContablePago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesAcceso"), Authorize]
        public async Task<ActionResult> GetClientesAcceso(FilterVm value)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetClientesAcceso";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var listaClientes = _context.ClientesPortals.Where(x => x.Clave == "" && x.ActivaCuenta == 0).ToList();
                var listaClientesSoftland = await sf.BuscarClienteSoftlandAsync(value.CodAux, value.Rut, value.Nombre, logApi.Id);

                //Agregar contactos a clientes
                foreach (var item in listaClientesSoftland)
                {
                    item.Contactos = await sf.GetAllContactosAsync(item.CodAux, logApi.Id); 
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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listaClientesSoftland);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetUbicaciones";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var regiones = await sf.GetRegionesSoftland(logApi.Id);
                var comunas = await sf.GetComunasSoftlandAsync(logApi.Id);

                var ubicaciones = new UbicacionVm
                {
                    Regiones = regiones,
                    Comunas = comunas
                };

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(ubicaciones);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetGiros";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var giros = await sf.GetGirosSoftlandAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(giros);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetVendedores";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var vendedores = await sf.GetVenedoresSoftlandAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(vendedores);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCategoriasCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var categoriasCliente = await sf.GetCategoriasClienteAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(categoriasCliente);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCondVentas";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var condicionesVenta = await sf.GetCondVentaAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(condicionesVenta);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetListasPrecio";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
          

            try
            {
                var listas = await sf.GetListPrecioAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetMonedas";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var monedas = await sf.GetMonedasAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(monedas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetClientesAcces";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var listaClientes = _context.ClientesPortals.ToList();
                var listaClientesSoftland = await sf.BuscarClienteSoftlandAccesosAsync(value.CodAux, value.Rut, value.Nombre, value.Vendedor, value.CondicionVenta, value.CategoriaCliente, value.ListaPrecio, 10, value.Pagina, logApi.Id);

                foreach (var item in listaClientesSoftland)
                {
                    var existe = listaClientes.Where(x => x.CodAux == item.CodAux && x.Rut == item.RutAux).FirstOrDefault();
                    if (existe != null)
                    {
                        item.AccesoEnviado = 1;
                    }
                    else
                    {
                        item.AccesoEnviado = 0;
                    }
                }

                if (value.TipoBusqueda == 2)
                {
                    //Remueve todos los que tengan accesos
                    List<ClienteAPIDTO> listaClientesSoftlandEliminar = new List<ClienteAPIDTO>();
                    foreach (var item in listaClientesSoftland)
                    {
                        var existe = listaClientes.Where(x => x.CodAux == item.CodAux && x.Rut == item.RutAux).ToList();

                        if (existe.Count > 0)
                        {
                            listaClientesSoftlandEliminar.Add(item);
                        }
                    }

                    foreach (var item in listaClientesSoftlandEliminar)
                    {
                        listaClientesSoftland.Remove(item);
                    }
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listaClientesSoftland);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCargos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           
            try
            {
                var cargos = await sf.GetCargosAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(cargos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCentrosCostosActivos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           
            try
            {

                var centros = sf.GetCentrosCostoActivos(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(centros);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetAreasNegocio";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var areas = sf.GetAreaNegocio(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(areas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCobradores";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var cobradores = sf.GetCobradores(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(cobradores);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetCanalesVenta";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var canalesVenta = sf.GetCanalesVenta(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(canalesVenta);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetExistModuloInventario";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                bool existModulo = false;
                var modulos = await sf.GetModulosSoftlandAsync(logApi.Id);

                var m = modulos.Where(x => x.Codi == "IW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetExistModuloNotaVenta";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                bool existModulo = false;
                var modulos = await sf.GetModulosSoftlandAsync(logApi.Id);

                var m = modulos.Where(x => x.Codi == "NW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/GetExistModuloContabilidad";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                bool existModulo = false;
                var modulos = await sf.GetModulosSoftlandAsync(logApi.Id);

                var m = modulos.Where(x => x.Codi == "CW").FirstOrDefault();
                if (m != null)
                {
                    existModulo = true;
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(existModulo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/GetExistModuloContabilidad";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ReprocesarPago/{idPago}"), Authorize]
        public async Task<ActionResult> ReprocesarPago(int idPago)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/ReprocesarPago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
          

            try
            {
                ComprobanteResponse comprobante = new ComprobanteResponse();
                comprobante.numero = await sf.ReprocesaPago(idPago, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
               
                return Ok(comprobante);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/ActualizaComprobante";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == p.IdPago).FirstOrDefault();
                if (pago != null)
                {
                    pago.ComprobanteContable = p.ComprobanteContable;
                    pago.IdPagoEstado = 2;
                    _context.PagosCabeceras.Attach(pago);

                  




                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                    string fecha = pago.FechaPago.Value.Day.ToString() + "/" + pago.FechaPago.Value.Month.ToString() + "/" + pago.FechaPago.Value.Year.ToString();
                    string hora = pago.HoraPago;
                    string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                    string comprobanteHtml = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/invoice.html")))
                    {
                        comprobanteHtml = reader.ReadToEnd();
                    }
                    //string comprobanteHtml = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/MailTemplates/invoice.html"));
                    comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                        .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", pago.ComprobanteContable).Replace("{MONTOTOTAL}", pago.MontoPago.Value.ToString("N0"));

                    string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApi.Id);
                    var pagosDetalles = _context.PagosDetalles.Where(x => x.IdPago == pago.IdPago).ToList();
                    foreach (var det in pagosDetalles)
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

                    _context.Entry(pago).Property(x => x.ComprobanteContable).IsModified = true;
                    _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                    _context.SaveChanges();

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Softland/ActualizaComprobante";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("CallbackPago")]
        public async Task<ActionResult> CallbackPago([FromQuery] int idPago, [FromQuery] int idPasarela, [FromQuery] string rutCliente, [FromQuery] int idCobranza, [FromQuery] string idAutomatizacion, [FromQuery] string datosPago, [FromQuery] string tenant, [FromQuery] TbkRedirect redirectTo = TbkRedirect.Front) //FCA 13-02-2023
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Softland/CallbackPago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            try
            {        
                string mensaje = "Pago procesado";

                if (string.IsNullOrEmpty(datosPago) && idPago != 0) //1. Validamos que los datos retornen con información
                {
                    mensaje = "Llamado no contiene la información del pago";
                }
                else  //2. Actualizamos estado de pago para retornar al cliente y el log de softlandpay
                {
                    using (var client = new HttpClient())
                    {
                        var pago = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(x => x.IdPago == idPago).FirstOrDefault();
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var pasarela = _context.PasarelaPagos.Where(x => x.IdPasarela == idPasarela).FirstOrDefault();
                        var logValida = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                        string accesToken = api.Token;


                        string url = pasarela.AmbienteConsultarPago.Replace("{ID}", logValida.Token)
                                                      .Replace("{ESPRODUCTIVO}", (pasarela.EsProduccion == 0 || pasarela.EsProduccion == null) ? "N" : "S");



                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            ResultadoEstadoPagoVPOS result = JsonConvert.DeserializeObject<ResultadoEstadoPagoVPOS>(content);

                            if (result.Estado == "PAGADO" || result.Estado == "AUTHORIZED") //Pago exitoso generar comprobante
                            {
                                logValida.Estado = result.Estado;
                                logValida.Fecha = DateTime.Now;
                                logValida.MedioPago = result.Medio_pago;
                                logValida.Cuotas = Convert.ToInt32(result.Cuotas);
                                logValida.Tarjeta = result.Forma_pago;

                                _context.Entry(logValida).Property(x => x.Fecha).IsModified = true;
                                _context.Entry(logValida).Property(x => x.Estado).IsModified = true;
                                _context.Entry(logValida).Property(x => x.MedioPago).IsModified = true;
                                _context.Entry(logValida).Property(x => x.Cuotas).IsModified = true;
                                _context.Entry(logValida).Property(x => x.Tarjeta).IsModified = true;
                                await _context.SaveChangesAsync();

                                //Genera comprobante contable por el pago realizado
                                PagoComprobanteVm comprobante = await sf.GeneraComprobantesContablesAsync(idPago, logValida.Token, logApi.Id);

                                if (comprobante.PagoId == 0)
                                {
                                    logApi.Termino = DateTime.Now;
                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                    sf.guardarLogApi(logApi);

                                    return Ok();
                                }
                                else
                                {
                                    if (idCobranza != 0)
                                    {

                                        var detallesPago = _context.PagosDetalles.Where(x => x.IdPago == idPago).ToList();
                                        string rutDesencriptado = Encrypt.Base64Decode(rutCliente);
                                        var detallesCobranza = _context.CobranzaDetalles.Where(x => x.IdCobranza == idCobranza && x.RutCliente == rutDesencriptado).ToList();
                                        var docsCliente = await sf.GetAllDocumentosContabilizadosCliente(pago.CodAux, logApi.Id);
                                        foreach (var detalleCobranza in detallesCobranza)
                                        {
                                            var detPago = detallesPago.OrderByDescending(x => x.IdPagoDetalle).Where(x => x.Folio == detalleCobranza.Folio && detalleCobranza.TipoDocumento == x.TipoDocumento).FirstOrDefault();
                                            if (detPago != null)
                                            {
                                                detalleCobranza.FechaPago = pago.FechaPago;
                                                detalleCobranza.HoraPago = pago.HoraPago;
                                                detalleCobranza.ComprobanteContable = comprobante.NumComprobante;
                                                detalleCobranza.IdPago = idPago;
                                                detalleCobranza.Pagado += detPago.Apagar;

                                                var existDoc = docsCliente.Where(x => x.Numdoc == detalleCobranza.Folio && x.Ttdcod == detalleCobranza.TipoDocumento).FirstOrDefault();
                                                if (existDoc != null)
                                                {
                                                    detalleCobranza.IdEstado = 4;
                                                }
                                                else
                                                {
                                                    detalleCobranza.IdEstado = 5;
                                                }

                                                _context.CobranzaDetalles.Attach(detalleCobranza);
                                                _context.Entry(detalleCobranza).Property(x => x.FechaPago).IsModified = true;
                                                _context.Entry(detalleCobranza).Property(x => x.HoraPago).IsModified = true;
                                                _context.Entry(detalleCobranza).Property(x => x.ComprobanteContable).IsModified = true;
                                                _context.Entry(detalleCobranza).Property(x => x.IdPago).IsModified = true;
                                                _context.Entry(detalleCobranza).Property(x => x.IdEstado).IsModified = true;
                                                _context.Entry(detalleCobranza).Property(x => x.Pagado).IsModified = true;

                                            }

                                        }
                                        _context.SaveChanges();
                                    }
                                    ClientesPortalController clientesController = new ClientesPortalController(_context, _webHostEnvironment, _admin, _contextAccessor);
                                    await clientesController.EnviaCorreoComprobante(idPago).ConfigureAwait(false);

                                    logApi.Termino = DateTime.Now;
                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                    sf.guardarLogApi(logApi);

                                    return Ok();
                                }

                            }
                            else
                            {
                                procesos = procesos + 1;
                                Thread.Sleep(milliseconds);

                                goto gotoCallback;
                            }

                        }
                        else
                        {
                            procesos = procesos + 1;
                            Thread.Sleep(milliseconds);

                            goto gotoCallback;
                        }
                    }
                }

                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                //FCA 14-01-2022 SE AGREGA LOG
                LogicalThreadContext.Properties["requestUri"] = "Api/venta/callbackSoftlandpay";
                log.Error(ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
