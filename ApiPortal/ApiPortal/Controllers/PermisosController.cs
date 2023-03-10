using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsPortal;
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
    public class PermisosController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PermisosController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetPermisos"), Authorize]
        public async Task<ActionResult> GetPermisos()
        {

            try
            {
                var permisos = _context.Permisos.ToList();
                List<PermisosVm> listaPermisos = new List<PermisosVm>();
                foreach (var item in permisos)
                {
                    PermisosVm p = new PermisosVm();
                    p.IdPermiso = item.IdPermiso;
                    p.IdPerfil = (int)item.IdPerfil;
                    p.IdAcceso = (int)item.IdAcceso;
                    p.Checked = false;
                    p.Perfil = new PerfilVm { IdPerfil = item.IdPerfilNavigation.IdPerfil, Nombre = item.IdPerfilNavigation.Nombre, Descripcion = item.IdPerfilNavigation.Descripcion };
                    p.Acceso = new AccesosVm { IdAcceso = item.IdAccesoNavigation.IdAcceso, Nombre = item.IdAccesoNavigation.Nombre, MenuPadre = item.IdAccesoNavigation.MenuPadre, Activo = item.IdAccesoNavigation.Activo };
                }

                return Ok(permisos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GetPermisos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPermisoId/{idPermiso}"), Authorize]
        public async Task<ActionResult> GetPermisoId(int idPermiso)
        {

            try
            {
                var permiso = _context.Permisos.Find(idPermiso);
                if (permiso == null)
                {
                    return NotFound();
                }

                return Ok(permiso);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GetPermisoId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ActualizaPermisos"), Authorize]
        public async Task<ActionResult> ActualizaPermisos(Permiso permisos)
        {
            try
            {
                _context.Entry(permisos).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(permisos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/ActualizaPermisos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GuardarPermiso"), Authorize]
        public async Task<ActionResult> GuardarPermiso(Permiso permisos)
        {
            try
            {
                _context.Permisos.Add(permisos);
                await _context.SaveChangesAsync();

                return Ok(permisos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GuardarPermiso";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetPermisosByPage"), Authorize]
        public async Task<ActionResult> GetPermisosByPage(PaginadorVm pVm)
        {
            try
            {
                var cantidad = pVm.EndRow - pVm.StartRow;
                var pagina = pVm.StartRow / cantidad + 1;

                var permisos = await _context.Permisos
                                  .Skip((Convert.ToInt32(pagina) - 1) * Convert.ToInt32(cantidad))
                                  .Take(Convert.ToInt32(cantidad))
                                  .AsNoTracking()
                                  .ToListAsync();

                if (!String.IsNullOrEmpty(pVm.Search))
                {
                    permisos = permisos.Where(x => x.IdPerfilNavigation.Nombre.ToUpper().Contains(pVm.Search.ToUpper()) ||
                                                   x.IdAccesoNavigation.Nombre.ToUpper().Contains(pVm.Search.ToUpper())).ToList();
                }

                //FCA 16-12-2021 Se reemplaza mapper
                List<PermisosVm> perm = new List<PermisosVm>();
                foreach (var item in permisos)
                {
                    PermisosVm permiso = new PermisosVm();
                    permiso.Acceso = new AccesosVm { IdAcceso = item.IdAccesoNavigation.IdAcceso, Nombre = item.IdAccesoNavigation.Nombre, MenuPadre = item.IdAccesoNavigation.MenuPadre, Activo = item.IdAccesoNavigation.Activo };
                    permiso.Checked = false;
                    permiso.IdAcceso = (int)item.IdAcceso;
                    permiso.IdPerfil = (int)item.IdPerfil;
                    permiso.IdPermiso = item.IdPermiso;
                    permiso.Perfil = new PerfilVm { IdPerfil = item.IdPerfilNavigation.IdPerfil, Nombre = item.IdPerfilNavigation.Nombre, Descripcion = item.IdPerfilNavigation.Descripcion };
                    perm.Add(permiso);
                }


                if (perm.Count() > 0)
                {
                    perm[0].TotalFilas = perm.Count();
                }

                return Ok(perm);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GetPermisosByPage";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("EliminaPermisosId/{idPermiso}"), Authorize]
        public async Task<ActionResult> EliminaPermisosId(int idPermiso)
        {
            try
            {
                var permiso = _context.Permisos.Find(idPermiso);
                if (permiso == null)
                {
                    return NotFound();
                }

                _context.Permisos.Remove(permiso);
                _context.SaveChanges();

                return Ok(permiso);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/EliminaPermisosId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPermisosByPerfil/{idPerfil}"), Authorize]
        public async Task<ActionResult> GetPermisosByPerfil(int idPerfil)
        {
            try
            {
                var acc = await _context.Accesos.Where(x => x.Activo == 1).ToListAsync();
                List<AccesosVm> accesos = new List<AccesosVm>();
                foreach (var item in acc)
                {
                    AccesosVm ac = new AccesosVm();
                    ac.Activo = (int)item.Activo;
                    ac.IdAcceso = item.IdAcceso;
                    ac.MenuPadre = item.MenuPadre;
                    ac.Nombre = item.Nombre;
                    accesos.Add(ac);
                }

                var permisosPerfil = await _context.Permisos.Where(x => x.IdPerfil == idPerfil).ToListAsync();

                foreach (var p in accesos)
                {
                    var added = permisosPerfil.Where(x => x.IdAcceso == p.IdAcceso).FirstOrDefault();
                    if (added != null)
                    {
                        p.Checked = true;
                    }
                    else
                    {
                        p.Checked = false;
                    }
                }

                return Ok(accesos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GetPermisosByPerfil";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("PostPermisosMasivo/{idPerfil}"), Authorize]
        public async Task<ActionResult> PostPermisosMasivo(int idPerfil, List<PermisosVm> permisos)
        {
            try
            {
                var permisosActuales = await _context.Permisos.Where(x => x.IdPerfil == idPerfil).ToListAsync();

                _context.Permisos.RemoveRange(permisosActuales);
                await _context.SaveChangesAsync();

                foreach (var permiso in permisos)
                {
                    if (Convert.ToBoolean(permiso.Checked))
                    {
                        _context.Permisos.Add(new Permiso
                        {
                            IdPermiso = 0,
                            IdPerfil = idPerfil,
                            IdAcceso = permiso.IdAcceso
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(permisos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/PostPermisosMasivo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetPermisosByEmail"), Authorize]
        public async Task<ActionResult> GetPermisosByEmail(AuthenticateVm model)
        {
            try
            {
                var permisos = new List<Permiso>();
                if (model.EsUsuario == true)
                {
                    var usuario = await _context.Usuarios.Where(x => x.Email == model.Email).FirstOrDefaultAsync();

                    if (usuario == null)
                    {
                        return NotFound();
                    }

                    permisos = await _context.Permisos.Where(x => x.IdPerfil == usuario.IdPerfil).ToListAsync();
                }
                else
                {
                    //Perfil 2: Usuario cliente
                    permisos = await _context.Permisos.Where(x => x.IdPerfil == 2).ToListAsync();
                }

                return Ok(permisos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Permisos/GetPermisosByEmail";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
