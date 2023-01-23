using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MailController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetCantidadPorDia/{casilla}"), Authorize]
        public async Task<ActionResult> GetCantidadPorDia(string casilla)
        {

            try
            {
                casilla = (casilla == "-")? string.Empty: casilla;
                int casillas = 0;
                MailService ms = new MailService(_context,_webHostEnvironment);
                casillas = ms.calculaDisponiblesPorDia(casilla);

                return Ok(casillas);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Mail/GetCantidadPorDia";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EnviarCorreo"), Authorize]
        public async Task<ActionResult> EnviarCorreo(MailViewModel model)
        {
            try
            {
                MailService ms = new MailService(_context,_webHostEnvironment);
                ms.EnviarCorreosAsync(model);
                return Ok(model);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Mail/EnviarCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCorreosDisponiblesCobranza"), Authorize]
        public async Task<ActionResult> GetCorreosDisponiblesCobranza()
        {

            try
            {
                MailService mailService = new MailService(_context,_webHostEnvironment);
                var disponibles = mailService.calculaDisponiblesCobranza();

                return Ok(disponibles);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Mail/GetCorreosDisponiblesCobranza";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCantdidEnviada"), Authorize]
        public async Task<ActionResult> GetCantdidEnviada()
        {

            try
            {
                var fecha = DateTime.Today;
                int retorno = 0;
                var cantidad = _context.RegistroEnvioCorreos.Where(x => x.FechaEnvio == fecha && x.IdTipoEnvio == 1);

                if (cantidad != null)
                {
                    retorno = cantidad.Count();
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
                log.Ruta = "api/Mail/GetCantdidEnviada";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
