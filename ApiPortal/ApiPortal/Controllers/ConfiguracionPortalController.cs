using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]

    [ApiController]
    public class ConfiguracionPortalController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConfiguracionPortalController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetConfiguracionPortal")]
        public async Task<ActionResult> GetConfiguracionPortal()
        {

            try
            {
                var configuracionPortal = _context.ConfiguracionPortals.FirstOrDefault();
                return Ok(configuracionPortal);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPorta/GetConfiguracionPortal";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaConfiguracion/{dias}"), Authorize]
        public async Task<ActionResult> ActualizaConfiguracion(int dias, [FromBody]ConfiguracionPortal model)
        {
            try
            {
                _context.Entry(model).State = EntityState.Modified;
                var configPagos = _context.ConfiguracionPagoClientes.FirstOrDefault();
                configPagos.DiasPorVencer = dias;
                _context.Entry(configPagos).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPorta/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllConfiguracionPortal")]
        public async Task<ActionResult> GetAllConfiguracionPortal()
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var configuracionDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                var configuracionPortal = _context.ConfiguracionPortals.FirstOrDefault();
                var configuracionPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                var modulos = await sf.GetModulosSoftlandAsync();
                bool existModuloInventario = false;
                bool existModuloNotaVenta = false;
                bool existModuloContabilidad = false;

                existModuloInventario = modulos.Where(x => x.Codi == "IW").FirstOrDefault() == null ? false : true;
                existModuloNotaVenta = modulos.Where(x => x.Codi == "NW").FirstOrDefault() == null ? false : true;
                existModuloContabilidad = modulos.Where(x => x.Codi == "CW").FirstOrDefault() == null ? false : true;

                var configuracionCompleta = new
                {
                    ConfiguracionDiseno = configuracionDiseno,
                    ConfiguracionPagoCliente = configuracionPago,
                    ConfiguracionPortal = configuracionPortal,
                    ExistModuloContabilidad = existModuloContabilidad,
                    ExistModuloInventario = existModuloInventario,
                    ExistModuloNotaVenta = existModuloNotaVenta
                };
           


                return Ok(configuracionCompleta);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPorta/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
