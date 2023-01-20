using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace ApiPortal.Controllers
{
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

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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

            try
            {
                string retorno = string.Empty;
                retorno = _context.ConfiguracionEmpresas.FirstOrDefault().NombreEmpresa;
                return Ok(retorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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

                    var logCorreo = new LogCorreo();
                    logCorreo.Fecha = DateTime.Now;
                    logCorreo.Tipo = "Acceso";
                    logCorreo.Estado = "PENDIENTE";

                    _context.LogCorreos.Add(logCorreo);
                    _context.SaveChanges();

                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;

                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath,"Uploads/MailTemplates/envioClave.component.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{NOMBRE}", usuario.Nombres);
                    body = body.Replace("{CLAVE}", claveEnvio);
                    body = body.Replace("{logo}", logo);
                    body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                    body = body.Replace("{Titulo}", configCorreo.TituloCambioClave);
                    body = body.Replace("{Texto}", configCorreo.TextoCambioClave);
                    try
                    {
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            mailMessage.To.Add(usuario.Email);

                            mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                            mailMessage.Subject = configCorreo.AsuntoCambioClave;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = configCorreo.SmtpServer;
                            smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                            NetworkCred.UserName = configCorreo.Usuario;
                            NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            smtp.Send(mailMessage);
                        }

                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Acceso Enviado";
                        logCorreo.Error = "";
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();


                        var registro = new RegistroEnvioCorreo();

                        registro.FechaEnvio = DateTime.Today;
                        registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                        registro.IdTipoEnvio = 4;
                        registro.IdUsuario = usuario.IdUsuario;

                        _context.RegistroEnvioCorreos.Add(registro);
                        _context.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Error al enviar correo";
                        logCorreo.Error = ex.Message;
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();
                        errorEnvio = true;
                    }

                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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

                    var logCorreo = new LogCorreo();
                    logCorreo.Fecha = DateTime.Now;
                    logCorreo.Tipo = "Acceso";
                    logCorreo.Estado = "PENDIENTE";

                    _context.LogCorreos.Add(logCorreo);
                    _context.SaveChanges();

                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;

                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/cambioCorreo.component.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{NOMBRE}", usuario.Nombres);
                    body = body.Replace("{CORREO}", c.Email);
                    body = body.Replace("{logo}", logo);
                    body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                    body = body.Replace("{Titulo}", configCorreo.TextoCambioCorreo);
                    body = body.Replace("{Texto}", configCorreo.TextoCambioCorreo);
                    try
                    {
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            mailMessage.To.Add(usuario.Email);
                            mailMessage.To.Add(correoAntiguo);

                            mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                            mailMessage.Subject = configCorreo.AsuntoCambioCorreo;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = configCorreo.SmtpServer;
                            smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                            NetworkCred.UserName = configCorreo.Usuario;
                            NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            smtp.Send(mailMessage);
                        }

                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Acceso Enviado";
                        logCorreo.Error = "";
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();


                        var registro = new RegistroEnvioCorreo();

                        registro.FechaEnvio = DateTime.Today;
                        registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                        registro.IdTipoEnvio = 4;
                        registro.IdUsuario = usuario.IdUsuario;

                        _context.RegistroEnvioCorreos.Add(registro);
                        _context.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Error al enviar correo";
                        logCorreo.Error = ex.Message;
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();
                        return BadRequest();
                    }

                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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



                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath,"~/Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                        var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                        Boolean errorEnvio = false;



                        var logCorreo = new LogCorreo();
                        logCorreo.Fecha = DateTime.Now;
                        logCorreo.Rut = configEmpresa.RutEmpresa;
                        logCorreo.Tipo = "Acceso";
                        logCorreo.Estado = "PENDIENTE";

                        _context.LogCorreos.Add(logCorreo);
                        _context.SaveChanges();

                        body = body.Replace("{NOMBRE}", user.Nombres + " " + user.Apellidos);
                        body = body.Replace("{CLAVE}", usuarios.Password);
                        body = body.Replace("{logo}", configEmpresa.UrlPortal + "/" + configEmpresa.Logo);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{Titulo}", "Actualización Clave");
                        body = body.Replace("{Texto}", "Estimado usuario, se ha realizado un cambio de clave para su cuenta.");

                        try
                        {
                            using (MailMessage mailMessage = new MailMessage())
                            {
                                mailMessage.To.Add(usuarios.Email.ToLower());
                                mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                                mailMessage.Subject = "Actualización Clave de Acceso";
                                mailMessage.Body = body;
                                mailMessage.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = configCorreo.SmtpServer;
                                smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                NetworkCred.UserName = configCorreo.Usuario;
                                NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                                smtp.Send(mailMessage);
                            }

                            _context.LogCorreos.Attach(logCorreo);
                            logCorreo.Estado = "Acceso Enviado";
                            logCorreo.Error = "";
                            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                            _context.SaveChanges();


                            var registro = new RegistroEnvioCorreo();

                            registro.FechaEnvio = DateTime.Today;
                            registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                            registro.IdTipoEnvio = 1;

                            _context.RegistroEnvioCorreos.Add(registro);
                            _context.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            _context.LogCorreos.Attach(logCorreo);
                            logCorreo.Estado = "Error al enviar correo";
                            logCorreo.Error = ex.Message;
                            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                            _context.SaveChanges();
                            errorEnvio = true;
                        }


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
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath,"Uploads/MailTemplates/activacionCuenta.component.html")))
                {
                    body = reader.ReadToEnd();
                }

                var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                Boolean errorEnvio = false;



                var logCorreo = new LogCorreo();
                logCorreo.Fecha = DateTime.Now;
                logCorreo.Rut = configEmpresa.RutEmpresa;
                logCorreo.Tipo = "Acceso";
                logCorreo.Estado = "PENDIENTE";

                _context.LogCorreos.Add(logCorreo);
                _context.SaveChanges();

                body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                body = body.Replace("{TEXTO}", "Estimado usuario, a continuación, encontrará las credenciales para poder realizar la activación de su cuenta en nuestro portal, una vez activada, deberas ingresar con el Rut de la empresa, correo y clave registrados.");
                body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
                body = body.Replace("{NOMBRE}", user.Nombres + " " + user.Apellidos);
                body = body.Replace("{RUT}", configEmpresa.RutEmpresa);
                body = body.Replace("{CORREO}", user.Email.ToLower());
                body = body.Replace("{CLAVE}", pass);
                body = body.Replace("{Titulo}", "Activación de Cuenta");
                body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
                string datosCliente = Encrypt.Base64Encode(configEmpresa.RutEmpresa + ";" + user.Email.ToLower());
                body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

                try
                {
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.To.Add(usuarios.Email.ToLower());

                        mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                        mailMessage.Subject = "Activación de Cuenta";
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = configCorreo.SmtpServer;
                        smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                        NetworkCred.UserName = configCorreo.Usuario;
                        NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); 
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                        smtp.Send(mailMessage);
                    }

                    _context.LogCorreos.Attach(logCorreo);
                    logCorreo.Estado = "Acceso Enviado";
                    logCorreo.Error = "";
                    _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                    _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                    _context.SaveChanges();


                    var registro = new RegistroEnvioCorreo();

                    registro.FechaEnvio = DateTime.Today;
                    registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                    registro.IdTipoEnvio = 1;

                    _context.RegistroEnvioCorreos.Add(registro);
                    _context.SaveChanges();

                }
                catch (Exception ex)
                {
                    _context.LogCorreos.Attach(logCorreo);
                    logCorreo.Estado = "Error al enviar correo";
                    logCorreo.Error = ex.Message;
                    _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                    _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                    _context.SaveChanges();
                    errorEnvio = true;
                }


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
                log.IdTipoProceso = -1;
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
            try
            {
                Usuario usuario = _context.Usuarios.Find(idUsuario);
                if (usuario == null)
                {
                    return NotFound();
                }

                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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

                return Ok(mappedusuarios);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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
            try
            {
                Usuario usuario = _context.Usuarios.Find(idUsuario);
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
                log.IdTipoProceso = -1;
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


                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath,"~/Uploads/MailTemplates/activacionCuenta.component.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    Boolean errorEnvio = false;



                    var logCorreo = new LogCorreo();
                    logCorreo.Fecha = DateTime.Now;
                    logCorreo.Rut = configEmpresa.RutEmpresa;
                    logCorreo.Tipo = "Acceso";
                    logCorreo.Estado = "PENDIENTE";

                    _context.LogCorreos.Add(logCorreo);
                    _context.SaveChanges();

                    body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                    body = body.Replace("{TEXTO}", "Estimado usuario se realizo una solicitud para reestablecer su clave, debido a esto su cuenta ha sido deshabilitada, a continuación, encontrará las credenciales para poder realizar la activación de su cuenta en nuestro portal, una vez activada, deberas ingresar con el Rut de la empresa, correo y clave registrados.");
                    body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
                    body = body.Replace("{NOMBRE}", existUser.Nombres + " " + existUser.Apellidos);
                    body = body.Replace("{RUT}", configEmpresa.RutEmpresa);
                    body = body.Replace("{CORREO}", existUser.Email.ToLower());
                    body = body.Replace("{CLAVE}", pass);
                    body = body.Replace("{Titulo}", "Activación de Cuenta");
                    body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
                    string datosCliente = Encrypt.Base64Encode(configEmpresa.RutEmpresa + ";" + existUser.Email.ToLower());
                    body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

                    try
                    {
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            mailMessage.To.Add(usuarios.Email.ToLower());

                            mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                            mailMessage.Subject = "Activación de Cuenta";
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = configCorreo.SmtpServer;
                            smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                            NetworkCred.UserName = configCorreo.Usuario;
                            NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); 
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                            smtp.Send(mailMessage);
                        }

                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Acceso Enviado";
                        logCorreo.Error = "";
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();


                        var registro = new RegistroEnvioCorreo();

                        registro.FechaEnvio = DateTime.Today;
                        registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                        registro.IdTipoEnvio = 1;

                        _context.RegistroEnvioCorreos.Add(registro);
                        _context.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        _context.LogCorreos.Attach(logCorreo);
                        logCorreo.Estado = "Error al enviar correo";
                        logCorreo.Error = ex.Message;
                        _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                        _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                        _context.SaveChanges();
                        errorEnvio = true;
                    }


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
                    return Ok(-1);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
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
