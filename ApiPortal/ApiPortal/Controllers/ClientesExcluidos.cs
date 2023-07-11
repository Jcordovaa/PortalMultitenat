using ApiPortal.Dal.Models_Admin;
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
    public class ClientesExcluidosController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientesExcluidosController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> GetClientesExcluidos()
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                LogApi logApi = new LogApi();
                logApi.Api = "api/ClientesExcluidos/GetClientesExcluidos";
                logApi.Inicio = DateTime.Now;
                logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
                var clientesExcluidos = _context.ClientesExcluidos.ToList();
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(clientesExcluidos);
            
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesExcluidos/GetClientesExcluidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
           
        }

        [HttpDelete("DeleteClienteExcluido/{id}"), Authorize]
        public async Task<ActionResult> DeleteClientesExcluidosAsync(int id)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                LogApi logApi = new LogApi();
                logApi.Api = "api/ClientesExcluidos/DeleteClienteExcluido";
                logApi.Inicio = DateTime.Now;
                logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

                ClientesExcluido clientesExcluidos = await _context.ClientesExcluidos.FindAsync(id);
                if (clientesExcluidos == null)
                {
                    return NotFound();
                }

                _context.ClientesExcluidos.Remove(clientesExcluidos);
                await _context.SaveChangesAsync();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(clientesExcluidos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesExcluidos/DeleteClienteExcluido";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
          
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> PostClientesExcluidos(ClientesExcluido clientesExcluidos)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                LogApi logApi = new LogApi();
                logApi.Api = "api/ClientesExcluidos/PostClientesExcluidos";
                logApi.Inicio = DateTime.Now;
                logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.ClientesExcluidos.Add(clientesExcluidos);
                await _context.SaveChangesAsync();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(clientesExcluidos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesExcluidos/PostClientesExcluidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
           
        }
    }
}
