using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilesController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PerfilesController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetPerfiles"), Authorize]
        public async Task<ActionResult> GetPerfiles()
        {

            try
            {
                var perfiles = _context.Perfils.ToList();
                return Ok(perfiles);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfiles/GetPerfiles";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPerfilId/{idPerfil}"), Authorize]
        public async Task<ActionResult> GetPerfilId(int idPerfil)
        {

            try
            {
                Perfil perfil = _context.Perfils.Find(idPerfil);
                if (perfil == null)
                {
                    return NotFound();
                }

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfiles/GetPerfilId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ActualizaPerfil"), Authorize]
        public async Task<ActionResult> ActualizaPerfil(Perfil perfil)
        {
            try
            {
                _context.Entry(perfil).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfil/ActualizaPerfil";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GuardarPerfil"), Authorize]
        public async Task<ActionResult> GuardarPerfil(Perfil perfil)
        {
            try
            {
                _context.Perfils.Add(perfil);
                await _context.SaveChangesAsync();

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfil/GuardarPerfil";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetPerfilesByPage"), Authorize]
        public async Task<ActionResult> GetPerfilesByPage(PaginadorVm pVm)
        {
            try
            {
                var cantidad = pVm.EndRow - pVm.StartRow;
                var pagina = pVm.StartRow / cantidad + 1;

                var perfiles = _context.Perfils.AsQueryable();

                if (!String.IsNullOrEmpty(pVm.Search))
                {
                    perfiles = perfiles.Where(x => x.Nombre.ToUpper().Contains(pVm.Search.ToUpper())).AsQueryable();
                }

                var outPerfiles = await perfiles.OrderBy(x => x.IdPerfil).Skip((Convert.ToInt32(pagina) - 1) * Convert.ToInt32(cantidad))
                            .Take(Convert.ToInt32(cantidad))
                            .AsNoTracking()
                            .ToListAsync();

                List<PerfilVm> mappedPerfiles = new List<PerfilVm>();
                foreach (var item in outPerfiles)
                {
                    PerfilVm perfil = new PerfilVm();
                    perfil.Descripcion = item.Descripcion;
                    perfil.IdPerfil = item.IdPerfil;
                    perfil.Nombre = item.Nombre;
                    mappedPerfiles.Add(perfil);
                }

                if (perfiles.Count() > 0)
                {
                    mappedPerfiles[0].TotalFilas = perfiles.Count();
                }

                return Ok(mappedPerfiles);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfil/GetPerfilesByPage";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("EliminaPerfilId/{idPerfil}"), Authorize]
        public async Task<ActionResult> EliminaPerfilId(int idPerfil)
        {
            try
            {
                Perfil perfil = _context.Perfils.Find(idPerfil);
                if (perfil == null)
                {
                    return NotFound();
                }

                _context.Perfils.Remove(perfil);
                _context.SaveChanges();

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Perfil/EliminaPerfilId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
