using ApiPortal.Dal.Models_Portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionDisenoController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConfiguracionDisenoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetConfiguracion")]
        public async Task<ActionResult> GetConfiguracion()
        {

            try
            {
                var configuracion = _context.ConfiguracionDisenos.FirstOrDefault();
                return Ok(configuracion);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionDiseno/GetConfiguracion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

    }
}
