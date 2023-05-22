using ApiPortal.Dal.Models_Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionPagoClienteController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConfiguracionPagoClienteController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetConfiguracion")]
        public async Task<ActionResult> GetConfiguracion()
        {

            try
            {
                LogApi logApi = new LogApi();
                logApi.Api = "api/ConfiguracionPago/GetConfiguracion";
                logApi.Inicio = DateTime.Now;
                var configuracionPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Inicio - logApi.Termino).Value.TotalSeconds);
                _context.LogApis.Add(logApi);
                _context.SaveChanges();
                return Ok(configuracionPortal);
               
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPago/GetConfiguracion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("actualizaConfiguracionPago"), Authorize]
        public async Task<ActionResult> actualizaConfiguracionPago(ConfiguracionPagoCliente model)
        {
            try
            {
                _context.Entry(model).State = EntityState.Modified;
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
                log.Ruta = "api/ConfiguracionPago/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
