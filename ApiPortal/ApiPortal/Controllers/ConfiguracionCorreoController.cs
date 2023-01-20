using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionCorreoController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConfiguracionCorreoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("actualizaConfiguracionCorreo"), Authorize]
        public async Task<ActionResult> actualizaConfiguracionCorreo(ConfiguracionCorreo model)
        {
            try
            {
                string encrypt = Encrypt.Base64Encode(model.Clave);

                model.Clave = encrypt;

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
                log.Ruta = "api/ConfiguracionCorreo/actualizaConfiguracionCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetConfiguracionCorreo"), Authorize]
        public async Task<ActionResult> GetConfiguracionCorreo()
        {

            try
            {
                var conf = await _context.ConfiguracionCorreos.ToListAsync();
                foreach (var item in conf)
                {
                    item.Clave = Encrypt.Base64Decode(item.Clave);
                }
                return Ok(conf);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionCorreo/GetConfiguracionCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
