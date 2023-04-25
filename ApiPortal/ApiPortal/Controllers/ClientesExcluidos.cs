using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
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
        public IQueryable<ClientesExcluido> GetClientesExcluidos()
        {
            return _context.ClientesExcluidos;
        }

        [HttpDelete("DeleteClienteExcluido/{id}"), Authorize]
        public async Task<ActionResult> DeleteClientesExcluidosAsync(int id)
        {
            ClientesExcluido clientesExcluidos = await _context.ClientesExcluidos.FindAsync(id);
            if (clientesExcluidos == null)
            {
                return NotFound();
            }

            _context.ClientesExcluidos.Remove(clientesExcluidos);
            await _context.SaveChangesAsync();

            return Ok(clientesExcluidos);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> PostClientesExcluidos(ClientesExcluido clientesExcluidos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ClientesExcluidos.Add(clientesExcluidos);
            await _context.SaveChangesAsync();

            return Ok(clientesExcluidos);
        }
    }
}
