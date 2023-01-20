using ApiPortal.Dal.Models_Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    //[EnableCors]
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

        [HttpPost("ActualizaConfiguracion"), Authorize]
        public async Task<ActionResult> ActualizaConfiguracion(ConfiguracionPortal model)
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
                log.Ruta = "api/ConfiguracionPorta/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
