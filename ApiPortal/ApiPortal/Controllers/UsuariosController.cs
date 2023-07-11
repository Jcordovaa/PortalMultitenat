using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using MercadoPago.Resource.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsuariosController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("GetUsuarioByMail"), Authorize]
        public async Task<ActionResult> GetUsuarioByMail(ClientesPortalVm cliente)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/GetUsuarioByMail";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                UsuariosVm retorno = new UsuariosVm();
                var usuariosPortal = _context.Usuarios.Where(x => x.Email == cliente.Correo).FirstOrDefault();

                if (usuariosPortal != null)
                {
                    retorno.Apellidos = usuariosPortal.Apellidos;
                    retorno.Nombres = usuariosPortal.Nombres;
                    retorno.Email = usuariosPortal.Email;
                    retorno.IdUsuario = usuariosPortal.IdUsuario;
                    retorno.IdPerfil = (int)usuariosPortal.IdPerfil;
                    retorno.NombrePerfil = _context.Perfils.Where(x => x.IdPerfil == retorno.IdPerfil).FirstOrDefault().Nombre;
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/GetUsuarioByMail";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getEmpresa"), Authorize]
        public async Task<ActionResult> getEmpresa()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/getEmpresa";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
          

            try
            {
                string retorno = string.Empty;
                retorno = _context.ConfiguracionEmpresas.FirstOrDefault().NombreEmpresa;

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/getEmpresa";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangePassword"), Authorize]
        public async Task<ActionResult> ChangePassword(UsuariosVm c)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/ChangePassword";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var usuario = await _context.Usuarios.Where(x => x.IdUsuario == c.IdUsuario).FirstOrDefaultAsync();
                if (usuario != null)
                {
                    Boolean errorEnvio = false;
                    string claveEnvio = string.Empty;
                    string body = string.Empty;
                    var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                    HashPassword aux = new HashPassword();
                    string hashPass = aux.HashCode(c.Password);

                    if (usuario.Password == hashPass)
                    {
                        return BadRequest("Clave no debe ser igual a la actual.");
                    }

                    claveEnvio = c.Password;
                    usuario.Password = hashPass;
                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    MailService emailService = new MailService(_context, _webHostEnvironment);
                    emailService.EnviaCorreoCambioClaveUsuario(usuario, claveEnvio);

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);

                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/ChangePassword";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangeCorreo"), Authorize]
        public async Task<ActionResult> ChangeCorreo(UsuariosVm c)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/ChangeCorreo";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var usuario = _context.Usuarios.Where(x => x.IdUsuario == c.IdUsuario).FirstOrDefault();
                if (usuario != null)
                {
                    string correoAntiguo = usuario.Email;
                    string body = string.Empty;
                    var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();

                    usuario.Email = c.Email;
                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    MailService emailService = new MailService(_context, _webHostEnvironment);
                    emailService.EnviaCambioCorreoUsuario(usuario, c, correoAntiguo);

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);

                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/ChangeCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("ActualizaUsuario"), Authorize]
        public async Task<ActionResult> ActualizaUsuario(UsuariosVm usuarios)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/ActualizaUsuario";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
          

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _context.Usuarios.Where(x => x.Email == usuarios.Email).FirstOrDefault();

                if (user != null)
                {
                    user.IdPerfil = usuarios.IdPerfil;
                    user.Nombres = usuarios.Nombres;
                    user.Apellidos = usuarios.Apellidos;
                    user.Activo = usuarios.Activo;
                    user.Email = usuarios.Email;

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    

                    if (user.Password != usuarios.Password)
                    {
                        HashPassword aux = new HashPassword();
                        string pass = aux.HashCode(usuarios.Password);
                        user.Password = pass;
                        user.CuentaActivada = 1;
                        _context.Entry(user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        bool errorEnvio = false;

                        MailService emailService = new MailService(_context, _webHostEnvironment);

                        errorEnvio = emailService.EnviaCorreoDatosUsuario(user, usuarios.Password);

                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);

                        if (errorEnvio)
                        {

                            return Ok(0);
                        }
                        else
                        {
                            return Ok(usuarios);
                        }

                    }

                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/ActualizaUsuario";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GuardarUsuario"), Authorize]
        public async Task<ActionResult> GuardarUsuario(UsuariosVm usuarios)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/GuardarUsuario";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var existUser = _context.Usuarios.Where(x => x.Email == usuarios.Email).FirstOrDefault();
                if (existUser != null)
                {
                    return Ok(-1);
                }

                Usuario user = new Usuario();
                user.Activo = usuarios.Activo;
                user.Apellidos = usuarios.Apellidos;
                user.CuentaActivada = usuarios.CuentaActivada;
                user.Email = usuarios.Email.ToLower();
                user.FechaCreacion = usuarios.FechaCreacion;
                user.FechaEnvioValidacion = usuarios.FechaEnvioValidacion;
                user.IdPerfil = usuarios.IdPerfil;
                user.IdUsuario = (int)usuarios.IdUsuario;
                user.Nombres = usuarios.Nombres;
                user.Password = usuarios.Password;
                string pass = RandomPassword.GenerateRandomPassword();
                HashPassword aux = new HashPassword(); 
                user.Password = aux.HashCode(pass);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Usuarios.Add(user);

                await _context.SaveChangesAsync();
                bool errorEnvio = false;
                MailService emailService = new MailService(_context, _webHostEnvironment);
                errorEnvio = emailService.EnviaAccesoUsuario(user, pass);


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                if (errorEnvio)
                {
                    return Ok(0);
                }
                else
                {
                    return Ok(user);
                }

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/GuardarUsuario";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("EliminaUsuarioId/{idUsuario}"), Authorize]
        public async Task<ActionResult> EliminaUsuarioId(int idUsuario)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/EliminaUsuarioId";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            
            try
            {
                Usuario usuario = _context.Usuarios.Find(idUsuario);
                if (usuario == null)
                {
                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);

                    return NotFound();
                }

                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);


                return Ok(usuario);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/EliminaUsuarioId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetUsuariosByPage"), Authorize]
        public async Task<ActionResult> GetUsuariosByPage(PaginadorVm pVm)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/GetUsuariosByPage";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var cantidad = pVm.EndRow - pVm.StartRow;
                var pagina = pVm.StartRow / cantidad + 1;

                var usuarios = _context.Usuarios.AsQueryable();

                if (!String.IsNullOrEmpty(pVm.Search))
                {
                    usuarios = usuarios.Where(x => x.Email.ToUpper().Contains(pVm.Search.ToUpper()) ||
                                                   x.Nombres.ToUpper().Contains(pVm.Search.ToUpper()) ||
                                                   x.Apellidos.ToUpper().Contains(pVm.Search.ToUpper()) ||
                                                   x.Nombres.ToUpper().Contains(pVm.Search.ToUpper())).AsQueryable();
                }

               var outusuarios = await usuarios.OrderBy(x => x.IdUsuario).Skip((Convert.ToInt32(pagina) - 1) * Convert.ToInt32(cantidad))
                            .Take(Convert.ToInt32(cantidad))
                            .AsNoTracking()
                            .ToListAsync();

                List<UsuariosVm> mappedusuarios = new List<UsuariosVm>();
                foreach (var item in outusuarios)
                {
                    UsuariosVm usuario = new UsuariosVm();
                    usuario.Activo = (int)item.Activo;
                    usuario.Apellidos = item.Apellidos;
                    usuario.CuentaActivada = (int)item.CuentaActivada;
                    usuario.Email = item.Email;
                    if (item.FechaCreacion != null) { usuario.FechaCreacion = (DateTime)item.FechaCreacion; }
                    if (item.FechaEnvioValidacion != null) { usuario.FechaEnvioValidacion = (DateTime)item.FechaEnvioValidacion; }
                    usuario.IdPerfil = (int)item.IdPerfil;
                    usuario.IdUsuario = item.IdUsuario;
                    usuario.Nombres = item.Nombres;
                    usuario.Password = item.Password;
                    var perfil = _context.Perfils.Where(x => x.IdPerfil == item.IdPerfil).FirstOrDefault();
                    if (perfil != null)
                    {
                        PerfilVm p = new PerfilVm();
                        p.IdPerfil = perfil.IdPerfil;
                        p.Descripcion = perfil.Descripcion;
                        p.Nombre = perfil.Nombre;
                        usuario.Perfil = p;
                    }
                    mappedusuarios.Add(usuario);
                }

                if (usuarios.Count() > 0)
                {
                    mappedusuarios[0].TotalFilas = usuarios.Count();
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(mappedusuarios);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/GetUsuariosByPage";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUsuarioId/{idUsuario}"), Authorize]
        public async Task<ActionResult> GetUsuarioId(int idUsuario)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/GetUsuarioId";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                Usuario usuario = _context.Usuarios.Find(idUsuario);


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                if (usuario == null)
                {
                    return NotFound();
                }


                return Ok(usuario);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/GetUsuarioId";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RestablecerContraseña"), Authorize]
        public async Task<ActionResult> RestablecerContraseña(UsuariosVm usuarios)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/Usuarios/RestablecerContraseña";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           
            try
            {
                var existUser = _context.Usuarios.Where(x => x.Email == usuarios.Email).FirstOrDefault();
                if (existUser != null)
                {
                    HashPassword aux = new HashPassword();
                    string pass = RandomPassword.GenerateRandomPassword();
                    existUser.CuentaActivada = 0;
                    existUser.Password = aux.HashCode(pass);
                    _context.Entry(existUser).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    bool errorEnvio = false;
                    MailService emailService = new MailService(_context, _webHostEnvironment);
                    errorEnvio = emailService.EnviaAccesoUsuario(existUser, pass);


                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);


                    if (errorEnvio)
                    {
                        return Ok(0);
                    }
                    else
                    {
                        return Ok(existUser);
                    }

                }
                else
                {
                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);


                    return Ok(-1);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Usuarios/ChangeCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
