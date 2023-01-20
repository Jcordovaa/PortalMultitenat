using ApiPortal.Dal.Models_Portal;
using ApiPortal.Dal.Models_Admin;
using ApiPortal.Enums;
using ApiPortal.ModelSoftland;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace ApiPortal.Controllers
{
    //[EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesPortalController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClientesPortalController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
        }

        [HttpPost("SendAccesosCliente"), Authorize]
        public async Task<ActionResult<int>> SendAccesosClienteAsync(List<ClienteDTO> value)
        {
            try
            {
                var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                Boolean errorEnvio = false;
                string[] cargos = value[0].CodCargo.Split(';');
                string body = string.Empty;

                if (value.Count > 0)
                {
                    foreach (var item in value)
                    {

                        var contactos = await softlandService.GetAllContactosAsync(item.CodAux);
                        List<ContactoDTO> contactosFiltrados = new List<ContactoDTO>();
                        foreach (var c in contactos)
                        {
                            var exist = cargos.Where(x => x == c.CodCargo).FirstOrDefault();

                            if (exist != null)
                            {
                                contactosFiltrados.Add(c);
                            }
                        }

                        if (contactosFiltrados.Count() == 0 && Convert.ToBoolean(value[0].EnviarTodosContactos))
                        {
                            contactosFiltrados = contactos;
                        }

                        if (contactosFiltrados.Count > 0)
                        {
                            foreach (var c in contactosFiltrados)
                            {


                                var clave = RandomPassword.GenerateRandomPassword();

                                var cliente = _context.ClientesPortals.Where(x => x.Rut == item.Rut && x.CodAux == item.CodAux && x.Correo == c.Correo).FirstOrDefault();
                                if (cliente == null) //NUEVO ACCESO
                                {
                                    var nuevoCliente = new ClientesPortal();
                                    nuevoCliente.Rut = item.Rut;
                                    nuevoCliente.CodAux = item.CodAux;
                                    nuevoCliente.Nombre = item.Nombre;
                                    nuevoCliente.Correo = c.Correo.ToLower();
                                    nuevoCliente.Clave = clave;
                                    nuevoCliente.ActivaCuenta = 0;

                                    _context.ClientesPortals.Add(nuevoCliente);
                                    _context.SaveChanges();

                                }
                                else //ACTUALIZA ACCESO
                                {
                                    cliente.Correo = c.Correo.ToLower();
                                    cliente.ActivaCuenta = 0;
                                    cliente.Clave = clave;
                                    _context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                    _context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                    _context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                    _context.SaveChanges();
                                }


                                var logCorreo = new LogCorreo();
                                logCorreo.Fecha = DateTime.Now;
                                logCorreo.Rut = item.Rut;
                                logCorreo.CodAux = item.CodAux;
                                logCorreo.Tipo = "Acceso";
                                logCorreo.Estado = "PENDIENTE";

                                _context.LogCorreos.Add(logCorreo);
                                _context.SaveChanges();

                                using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
                                {
                                    body = reader.ReadToEnd();
                                }

                                body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                                body = body.Replace("{TEXTO}", configCorreo.TextoMensajeActivacion);
                                body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
                                body = body.Replace("{NOMBRE}", item.Nombre);
                                body = body.Replace("{RUT}", item.Rut);
                                body = body.Replace("{CORREO}", c.Correo.ToLower());
                                body = body.Replace("{CLAVE}", clave);
                                body = body.Replace("{Titulo}", configCorreo.TituloAccesoCliente);
                                body = body.Replace("{ColorBoton}", configCorreo.ColorBoton);
                                string datosCliente = Encrypt.Base64Encode(item.CodAux + ";" + c.Correo.ToLower());
                                body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

                                try
                                {
                                    using (MailMessage mailMessage = new MailMessage())
                                    {
                                        mailMessage.To.Add(c.Correo.ToLower());

                                        mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                                        mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
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

                                    var user = _context.Usuarios.Where(x => x.Email == item.CorreoUsuario).FirstOrDefault();
                                    if (user != null)
                                    {
                                        var registro = new RegistroEnvioCorreo();

                                        registro.FechaEnvio = DateTime.Today;
                                        registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                                        registro.IdTipoEnvio = 1;
                                        registro.IdUsuario = user.IdUsuario;

                                        _context.RegistroEnvioCorreos.Add(registro);
                                        _context.SaveChanges();
                                    }

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
                            }
                        }
                        else if (contactosFiltrados.Count() == 0 && Convert.ToBoolean(value[0].EnviarFicha))
                        {
                            var clave = RandomPassword.GenerateRandomPassword();

                            var cliente = _context.ClientesPortals.Where(x => x.Rut == item.Rut && x.CodAux == item.CodAux && x.Correo == item.Correo).FirstOrDefault();
                            if (cliente == null) //NUEVO ACCESO
                            {
                                var nuevoCliente = new ClientesPortal();
                                nuevoCliente.Rut = item.Rut;
                                nuevoCliente.CodAux = item.CodAux;
                                nuevoCliente.Nombre = item.Nombre;
                                nuevoCliente.Correo = item.Correo.ToLower();
                                nuevoCliente.Clave = clave;
                                nuevoCliente.ActivaCuenta = 0;
                                _context.ClientesPortals.Add(nuevoCliente);
                                _context.SaveChanges();

                            }
                            else //ACTUALIZA ACCESO
                            {
                                cliente.Correo = item.Correo.ToLower();
                                cliente.ActivaCuenta = 0;
                                cliente.Clave = clave;
                                _context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                _context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                _context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                _context.SaveChanges();
                            }


                            var logCorreo = new LogCorreo();
                            logCorreo.Fecha = DateTime.Now;
                            logCorreo.Rut = item.Rut;
                            logCorreo.CodAux = item.CodAux;
                            logCorreo.Tipo = "Acceso";
                            logCorreo.Estado = "PENDIENTE";

                            _context.LogCorreos.Add(logCorreo);
                            _context.SaveChanges();

                            using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/activacionCuenta.component.html")))
                            {
                                body = reader.ReadToEnd();
                            }

                            body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                            body = body.Replace("{TEXTO}", configCorreo.TextoMensajeActivacion);
                            body = body.Replace("{LOGO}", configCorreo.LogoCorreo);
                            body = body.Replace("{NOMBRE}", item.Nombre);
                            body = body.Replace("{RUT}", item.Rut);
                            body = body.Replace("{CORREO}", item.Correo.ToLower());
                            body = body.Replace("{CLAVE}", clave);
                            string datosCliente = Encrypt.Base64Encode(item.CodAux + ";" + item.Correo.ToLower());
                            body = body.Replace("{ENLACE}", configEmpresa.UrlPortal + "/#/sessions/activate-account/" + datosCliente);

                            try
                            {
                                using (MailMessage mailMessage = new MailMessage())
                                {
                                    mailMessage.To.Add(item.Correo.ToLower());

                                    mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                                    mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
                                    mailMessage.Body = body;
                                    mailMessage.IsBodyHtml = true;
                                    SmtpClient smtp = new SmtpClient();
                                    smtp.Host = configCorreo.SmtpServer;
                                    smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                    NetworkCred.UserName = configCorreo.Usuario;
                                    NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave);//Encrypt.DesEncriptar(configCorreo.Clave);
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

                                var user = _context.Usuarios.Where(x => x.Email == item.CorreoUsuario.ToLower()).FirstOrDefault();
                                if (user != null)
                                {
                                    var registro = new RegistroEnvioCorreo();

                                    registro.FechaEnvio = DateTime.Today;
                                    registro.HoraEnvio = DateTime.Now.ToString("HH:mm");
                                    registro.IdTipoEnvio = 1;
                                    registro.IdUsuario = user.IdUsuario;

                                    _context.RegistroEnvioCorreos.Add(registro);
                                    _context.SaveChanges();
                                }

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
                        }
                    }
                }

                if (errorEnvio)
                {
                    return Ok(-1);
                }
                else
                {
                    return Ok(1);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/SendAccesosCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CanActivateAccount")]
        public async Task<ActionResult> CanActivateAccount(ClientesPortalVm user)
        {
            try
            {
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                HashPassword aux = new HashPassword();

                if (user.CodAux == configEmpresa.RutEmpresa)
                {
                    var usuario = _context.Usuarios.Where(x => x.Email == user.Correo).FirstOrDefault();

                    if (usuario == null)
                    {
                        return BadRequest("Cuenta de usuario no es válida, compruebe la información ingresada.");
                    }

                    if (usuario.CuentaActivada == 1)
                    {
                        return BadRequest("Cuenta de usuario ya se encuentra activa.");
                    }
                    string pass = aux.HashCode(user.Clave);

                    if (pass != usuario.Password)
                    {
                        return BadRequest("Contraseña ingresada incorrecta.");
                    }
                }
                else
                {
                    var cliente = await _context.ClientesPortals.Where(x => x.CodAux == user.CodAux && x.Correo.Trim().ToLower() == user.Correo.Trim().ToLower()).FirstOrDefaultAsync();
                    if (cliente == null)
                    {
                        return BadRequest("Cuenta de usuario no es válida, compruebe la información ingresada.");
                    }

                    if (cliente.ActivaCuenta == 1)
                    {
                        return BadRequest("Cuenta de usuario ya se encuentra activa.");
                    }



                    if (user.Clave != cliente.Clave)
                    {
                        return BadRequest("Contraseña ingresada incorrecta.");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/CanActivateAccount";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActivateAccount")]
        public async Task<ActionResult> ActivateAccount(ClientesPortalVm user)
        {
            try
            {
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                HashPassword aux = new HashPassword();

                if (user.CodAux == configEmpresa.RutEmpresa)
                {
                    var usuario = _context.Usuarios.Where(x => x.Email == user.Correo).FirstOrDefault();
                    if (usuario == null)
                    {
                        return BadRequest("Cuenta de usuario no es válida, compruebe la información ingresada.");
                    }

                    usuario.Password = aux.HashCode(user.Clave);
                    usuario.CuentaActivada = 1;

                    _context.Entry(usuario).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cliente = _context.ClientesPortals.Where(x => x.CodAux == user.CodAux && x.Correo.Trim().ToLower() == user.Correo.Trim().ToLower()).FirstOrDefault();
                    if (cliente == null)
                    {
                        return BadRequest("Cuenta de usuario no es válida, compruebe la información ingresada.");
                    }

                    cliente.Clave = aux.HashCode(user.Clave);
                    cliente.ActivaCuenta = 1;

                    _context.Entry(cliente).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/ActivateAccount";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteByMailAndRut"), Authorize]
        public async Task<ActionResult> GetClienteByMailAndRut(ClientesPortalVm cliente)
        {
            ClienteDTO retorno = new ClienteDTO();

            try
            {
                var clientePortal = _context.ClientesPortals.Where(x => x.CodAux == cliente.CodAux && x.Correo == cliente.Correo).FirstOrDefault();
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                retorno = await sf.GetClienteSoftlandAsync(cliente.CodAux);
                if (clientePortal != null)
                {
                    retorno.IdCliente = clientePortal.IdCliente;
                }

                if (!string.IsNullOrEmpty(retorno.Rut))
                {
                    List<ContactoDTO> contactos = await sf.GetContactosClienteAsync(cliente.CodAux);
                    retorno.Contactos = contactos;
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
                log.Ruta = "api/ClientesPortal/GetClienteByMailAndRut";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaClienteSoftland"), Authorize]
        public async Task<ActionResult> ActualizaClienteSoftland(ClienteDTO cliente)
        {
            
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                await sf.UpdateAuxiliarPortalPagoAsync(cliente);

                MailViewModel mailVm = new MailViewModel();
                mailVm.email_destinatario = cliente.CorreoUsuario;
                mailVm.tipo = 7;
                MailService mail = new MailService(_context,_webHostEnvironment);
                await mail.EnviarCorreosAsync(mailVm);

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/ActualizaClienteSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangePassword"), Authorize]
        public async Task<ActionResult> ChangePassword(ClientesPortalVm c)
        {
            
            try
            {
                Boolean errorEnvio = false;
                string claveEnvio = string.Empty;
                string body = string.Empty;
                var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                var cliente = await _context.ClientesPortals.Where(x => x.IdCliente == c.IdCliente).FirstOrDefaultAsync();
                if (cliente != null)
                {
                    HashPassword aux = new HashPassword();
                    string hashPass = aux.HashCode(c.Clave);

                    if (cliente.Clave == hashPass)
                    {
                        return BadRequest("Clave no debe ser igual a la actual.");
                    }

                    claveEnvio = c.Clave;
                    cliente.Clave = hashPass;
                    _context.Entry(cliente).State = EntityState.Modified;
                    await _context.SaveChangesAsync();


                    var logCorreo = new LogCorreo();
                    logCorreo.Fecha = DateTime.Now;
                    logCorreo.Rut = cliente.Rut;
                    logCorreo.CodAux = cliente.CodAux;
                    logCorreo.Tipo = "Acceso";
                    logCorreo.Estado = "PENDIENTE";

                    _context.LogCorreos.Add(logCorreo);
                    _context.SaveChanges();

                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{NOMBRE}", cliente.Nombre);
                    body = body.Replace("{CLAVE}", claveEnvio);
                    body = body.Replace("{logo}", configEmpresa.UrlPortal + "/" + configEmpresa.Logo);
                    body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                    body = body.Replace("{Titulo}", configCorreo.TituloCambioClave);
                    body = body.Replace("{Texto}", configCorreo.TextoCambioClave);
                    try
                    {
                        using (MailMessage mailMessage = new MailMessage())
                        {
                            mailMessage.To.Add(cliente.Correo);

                            mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                            mailMessage.Subject = configCorreo.AsuntoAccesoCliente;
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
                        registro.IdTipoEnvio = 4;
                        registro.IdUsuario = cliente.IdCliente;

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
                }
                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/ChangePassword";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUltimasCompras/{codAux}"), Authorize]
        public async Task<ActionResult> GetUltimasCompras(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                var conf = _context.ConfiguracionPortals.FirstOrDefault();
                int top = (conf != null) ? (int)conf.CantidadUltimasCompras : 0;
                List<ComprasSoftlandDTO> compras = await sf.GetTopComprasAsync(codAux, top);
                compras = compras.OrderByDescending(x => x.FechaEmision).ToList();
                return Ok(compras);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetUltimasCompras";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDashboardDocumentosVencidos/{codAux}"), Authorize]
        public async Task<ActionResult> GetDashboardDocumentosVencidos(string codAux)
        {

            try
            {
                string utilizaApiSoftland = "1";
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DashboardDocumentosVm> retorno = new List<DashboardDocumentosVm>();

                List<ClienteSaldosDTO> documentosVencidos = await sf.GetDocumentosVencidosAsync(codAux);
                List<ClienteSaldosDTO> documentosPorVencer = await sf.GetDocumentosPorVencerAsync(codAux);
                List<ClienteSaldosDTO> documentosPendientes = await sf.GetDocumentosPendientesAsync(codAux);

                if (documentosVencidos.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosVencidos)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.SaldoBase);
                    }

                    d.Estado = "VENCIDO";
                    retorno.Add(d);
                }

                if (documentosPorVencer.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosPorVencer)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.SaldoBase);
                    }

                    d.Estado = "PORVENCER";
                    retorno.Add(d);
                }

                if (documentosPendientes.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosPendientes)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.SaldoBase);
                    }

                    d.Estado = "PENDIENTES";
                    retorno.Add(d);
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
                log.Ruta = "api/ClientesPortal/GetDashboardDocumentosVencidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosVencidos/{codAux}"), Authorize]
        public async Task<ActionResult> GetDocumentosVencidos(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ClienteSaldosDTO> documentosVencidos = await sf.GetDocumentosVencidosAsync(codAux);

                return Ok(documentosVencidos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosVencidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosPorVencer/{codAux}"), Authorize]
        public async Task<ActionResult> GetDocumentosPorVencer(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ClienteSaldosDTO> documentosPorVencer = await sf.GetDocumentosPorVencerAsync(codAux);

                return Ok(documentosPorVencer);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosPorVencer";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosPendientes/{codAux}"), Authorize]
        public async Task<ActionResult> GetDocumentosPendientes(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                List<ClienteSaldosDTO> documentosPendientes = await sf.GetDocumentosPendientesAsync(codAux);

                return Ok(documentosPendientes);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosPendientes";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllDocumentosContabilizados/{codAux}"), Authorize]
        public async Task<ActionResult> GetAllDocumentosContabilizados(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                List<ClienteSaldosDTO> documentosContabilizados = await sf.GetAllDocumentosContabilizadosAsync(codAux);

                return Ok(documentosContabilizados);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetAllDocumentosContabilizados";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDashboardCompras/{codAux}"), Authorize]
        public async Task<ActionResult> GetDashboardCompras(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DashboardComprasVm> retorno = new List<DashboardComprasVm>();
                List<DocumentosFacturadosDTO> compras = await sf.GetClientesComprasSoftlandAsync(codAux);
                List<NotaVentaClienteDTO> nv = await sf.GetNotasVentasPendientesAsync(codAux);
                List<ProductoDTO> productos = await sf.GetProductosCompradosAsync(codAux); 
                List<GuiaDespachoDTO> guias = await sf.GetGuiasPendientes(codAux);

                if (compras.Count > 0)
                {
                    DashboardComprasVm d = new DashboardComprasVm();
                    foreach (var item in compras)
                    {
                        d.CantidadDocumentos += 1;
                        d.MontoTotal = d.MontoTotal + Convert.ToInt32(item.Monto);
                    }

                    d.Tipo = "COMPRAS";
                    retorno.Add(d);
                }

                if (nv.Count > 0)
                {
                    DashboardComprasVm d = new DashboardComprasVm();
                    foreach (var item in nv)
                    {
                        d.CantidadDocumentos += 1;
                        d.MontoTotal = d.MontoTotal + Convert.ToInt32(item.MontoPendiente);
                    }

                    d.Tipo = "NV";
                    retorno.Add(d);
                }

                if (productos.Count > 0)
                {
                    DashboardComprasVm d = new DashboardComprasVm();
                    foreach (var item in productos)
                    {
                        d.CantidadDocumentos += 1;
                        d.MontoTotal = d.MontoTotal + Convert.ToInt32(item.TotalLinea);
                    }

                    d.Tipo = "PRODUCTOS";
                    retorno.Add(d);
                }

                if (guias.Count > 0)
                {
                    DashboardComprasVm d = new DashboardComprasVm();
                    foreach (var item in guias)
                    {
                        d.CantidadDocumentos += 1;
                        d.MontoTotal = d.MontoTotal + Convert.ToInt32(item.Total);
                    }

                    d.Tipo = "GUIAS";
                    retorno.Add(d);
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
                log.Ruta = "api/ClientesPortal/GetDashboardCompras";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteComprasFromSoftland"), Authorize]
        public async Task<ActionResult> GetClienteComprasFromSoftland(ClientesPortalVm model)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DocumentosFacturadosDTO> clients = await sf.GetClientesComprasSoftlandAsync(model.CodAux);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteComprasFromSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEstadoBloqueoCliente/{codAux}"), Authorize]
        public async Task<ActionResult> GetEstadoBloqueoCliente(string codAux)
        {

            try 
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                Boolean estado = await sf.GetEstadoBloqueoClienteAsync(codAux); 
                if (estado)
                {
                    return Ok(1);
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetEstadoBloqueoCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteDocumento"), Authorize]
        public async Task<ActionResult> GetClienteDocumento(FilterVm model)
        {
            DocumentosVm documento = new DocumentosVm();
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                string utilizaApiSoftland = "true";
                if (utilizaApiSoftland == "false")
                {
                    Generador reporte = new Generador(_context,_webHostEnvironment);
                    string archivo64 = string.Empty;
                    string tipoDoc = string.Empty;

                    Stream memoryStream = reporte.GenerarDocumentoElectronico((int)model.Folio, model.CodAux, out tipoDoc);
                    byte[] bytes;
                    using (var arc = new MemoryStream())
                    {
                        memoryStream.CopyTo(arc);
                        bytes = arc.ToArray();
                    }


                    archivo64 = Convert.ToBase64String(bytes);

                    documento.NombreArchivo = (tipoDoc == "B") ? "Boleta " + model.Folio + ".pdf" : (tipoDoc == "F") ? "Factura " + model.Folio + ".pdf" : "";
                    documento.Tipo = "PDF";
                    documento.Base64 = archivo64;
                    return Ok(documento);
                }
                else
                {
                    DocumentoDTO doc = await sf.obtenerDocumentoAPI(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux);
                    string base64 = await sf.obtenerPDFDocumento(Convert.ToInt32(model.Folio), model.TipoDoc, doc.cabecera.SubTipo);
                    documento.NombreArchivo = (model.TipoDoc == "B") ? "Boleta " + model.Folio + ".pdf" : (model.TipoDoc == "F") ? "Factura " + model.Folio + ".pdf" : "";
                    documento.Tipo = "PDF";
                    documento.Base64 = base64;
                    return Ok(documento);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteDocumento";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentoXML"), Authorize]
        public async Task<ActionResult> GetDocumentoXML(FilterVm model)
        {
            DocumentosVm documento = new DocumentosVm();
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                DocumentosVm dtResultado = await sf.obtenerXMLDTEAsync(Convert.ToInt32(model.Folio), model.CodAux, model.TipoDoc);

                if (!string.IsNullOrEmpty(dtResultado.Base64))
                {
                    XmlDocument xm = new XmlDocument();
                    xm.LoadXml(dtResultado.Base64);

                    var xmlDocString = xm.InnerXml;
                    var xmlDocByteArray = Encoding.ASCII.GetBytes(xmlDocString);
                    var xmlDocBase64 = Convert.ToBase64String(xmlDocByteArray); ;
                    documento.Tipo = "XML";
                    documento.Base64 = xmlDocBase64;
                    documento.NombreArchivo = dtResultado.NombreArchivo;
                }
                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentoXML";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDetalleCompra"), Authorize]
        public async Task<ActionResult> GetDetalleCompra(FilterVm model)
        {
           
            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                DocumentoDTO documento = new DocumentoDTO();

                string utilizaApiSoftland = "true";

                if (utilizaApiSoftland == "false")
                {
                    CabeceraDocumentoDTO dtResultado = sf.obtenerCabecera(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux);
                    List<DetalleDocumentoDTO> dtDetalle = sf.obtenerDetalle(Convert.ToInt32(model.Folio), dtResultado.Tipo, model.CodAux);


                    documento.cabecera = dtResultado;
                    documento.detalle = dtDetalle;
                }
                else
                {
                    documento = await sf.obtenerDocumentoAPI(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux);
                }


                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDetalleCompra";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDetalleDespacho"), Authorize]
        public async Task<ActionResult> GetDetalleDespacho(FilterVm model)
        {

            try
            {
                string utilizaApiSoftland = "true";
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DespachosDTO> listaRetorno = new List<DespachosDTO>();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    listaRetorno = sf.GetDespachosDocumento(Convert.ToInt32(model.Folio));
                    if (listaRetorno.Count > 0)
                    {
                        foreach (var item in listaRetorno)
                        {
                            if (item.Documento == "Guia Despacho")
                            {
                                item.Detalle = sf.GetDetalleDespacho(item.NroInt);
                            }

                        }
                    }
                }
                else
                {
                    listaRetorno = await sf.GetDepachoDocumentoAPIAsync(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux);
                }



                return Ok(listaRetorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDetalleDespacho";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EnvioDocumentos"), Authorize]
        public async Task<ActionResult> EnvioDocumentos(FilterVm model)
        {

            try
            {
                //envio de correo al cliente
                MailViewModel vm = new MailViewModel();
                vm.tipo = (int)MailTipo.EnvioDocumentosCliente;
                vm.folio = Convert.ToInt32(model.Folio);
                vm.codAux = model.CodAux;
                vm.tipoDoc = model.TipoDoc;
                vm.email_destinatario = model.correos;
                vm.tipoEnvioDoc = Convert.ToInt32(model.TipoEnvioDoc);
                MailService mail = new MailService(_context,_webHostEnvironment);
                await mail.EnviarCorreosAsync(vm);


                var user = _context.ClientesPortals.Where(x => x.Correo == model.correos).FirstOrDefault();
                if (user != null)
                {
                    RegistroEnvioCorreo registro = new RegistroEnvioCorreo
                    {
                        FechaEnvio = DateTime.Today,
                        HoraEnvio = DateTime.Now.ToString(),
                        IdTipoEnvio = 5,
                        IdCliente = user.IdCliente
                    };
                    _context.RegistroEnvioCorreos.Add(registro);
                    _context.SaveChanges();
                }


                return Ok(model);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/EnvioDocumentos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetContactosClienteSoftland/{codAux}"), Authorize]
        public async Task<ActionResult> GetContactosClienteSoftland(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ContactoDTO> contactos = await sf.GetContactosClienteAsync(codAux);
                return Ok(contactos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetContactosClienteSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetNotaVentaCliente/{codAux}"), Authorize]
        public async Task<ActionResult> GetNotaVentaCliente(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                List<NotaVentaClienteDTO> nv = await sf.GetNotasVentasPendientesAsync(codAux);
                return Ok(nv);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetNotaVentaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDetalleCompraNv"), Authorize]
        public async Task<ActionResult> GetDetalleCompraNv(FilterVm model)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                NotaVentaClienteDTO detalle = new NotaVentaClienteDTO();
                detalle = await sf.GetNotaVentaAsync(Convert.ToInt32(model.Folio), model.CodAux);
                detalle.detalle = await sf.GetNotaVentaDetalleAsync(Convert.ToInt32(model.Folio));

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDetalleCompraNv";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetProductosComprados"), Authorize]
        public async Task<ActionResult> GetProductosComprados(FilterVm model)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ProductoDTO> productos = await sf.GetProductosCompradosAsync(model.CodAux);

                return Ok(productos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetProductosComprados";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteEstadoComprasFromSoftland"), Authorize]
        public async Task<ActionResult> GetClienteEstadoComprasFromSoftland(FilterVm model)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                List<ClienteSaldosDTO> clients = await sf.GetClienteEstadoCuentaAsync(model.CodAux);

                if (model.Folio != 0)
                {
                    clients = clients.Where(x => x.Nro == model.Folio).ToList();
                }


                foreach (var item in clients)
                {
                    var pagosDetalle = _context.PagosDetalles.Where(x => x.Folio == item.Nro && x.TipoDocumento == item.TipoDoc).FirstOrDefault();

                    if (pagosDetalle != null)
                    {
                        var pagoCabecera = _context.PagosCabeceras.Where(x => x.IdPago == pagosDetalle.IdPago).FirstOrDefault();

                        if (pagoCabecera != null)
                        {

                            item.BloqueadoPago = pagoCabecera.IdPagoEstado == 4 ? true : false;
                        }
                    }
                }

                if (model.IdCobranza != 0 && model.IdCobranza != null)
                {
                    var documentosCobranzas = _context.CobranzaDetalles.Where(x => x.IdCobranza == model.IdCobranza && model.CodAux == x.CodAuxCliente).ToList();
                    string foliosDocumentos = string.Empty;
                    foreach (var item in documentosCobranzas)
                    {
                        if (string.IsNullOrEmpty(foliosDocumentos))
                        {
                            foliosDocumentos = item.Folio.ToString();
                        }
                        else
                        {
                            foliosDocumentos = foliosDocumentos + ";" + item.Folio.ToString();
                        }
                    }

                    clients = clients.Where(x => foliosDocumentos.Contains(x.Nro.ToString())).ToList();
                }

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteEstadoComprasFromSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEstadoConexionSoftland"), Authorize]
        public async Task<ActionResult> GetEstadoConexionSoftland()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                Boolean estado = await sf.verificaEstadoConexionSoftlandAsync();
                return Ok(estado);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetEstadoConexionSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SavePago"), Authorize]
        public async Task<ActionResult> SavePago(PagoCabeceraVm value)
        {

            try
            {
                string hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());

                var pagoEstado = new PagosCabecera();
                pagoEstado.IdPago = 0;
                if (value.EsPagoRapido == 1)
                {
                    pagoEstado.IdCliente = null;
                    pagoEstado.Nombre = value.Nombre;
                }
                else
                {
                    var cliente = _context.ClientesPortals.Where(x => x.Correo == value.Correo && x.CodAux == value.CodAux && x.Rut == value.Rut).FirstOrDefault();
                    pagoEstado.IdCliente = cliente.IdCliente;
                    pagoEstado.Nombre = cliente.Nombre;
                }

                pagoEstado.FechaPago = DateTime.Now;
                pagoEstado.HoraPago = hora;
                pagoEstado.MontoPago = value.MontoPago;
                pagoEstado.ComprobanteContable = string.Empty;
                pagoEstado.IdPagoEstado = 1; //ESTADO PENDIENTE
                pagoEstado.Rut = value.Rut;
                pagoEstado.CodAux = value.CodAux;

                pagoEstado.Correo = value.Correo;
                pagoEstado.IdPasarela = value.IdPasarela;
                pagoEstado.EsPagoRapido = value.EsPagoRapido;
                pagoEstado.IdClienteNavigation = null;
                pagoEstado.PasarelaPagoLogs = null;
                pagoEstado.IdPagoEstadoNavigation = null;
                pagoEstado.PagosDetalles = null;


                _context.PagosCabeceras.Add(pagoEstado);
                _context.SaveChanges();

                foreach (PagoDetalleVm row in value.PagosDetalle)
                {
                    var pagoEstadoDet = new PagosDetalle
                    {
                        IdPago = pagoEstado.IdPago,
                        Folio = row.Folio,
                        TipoDocumento = row.TipoDocumento,
                        CuentaContableDocumento = row.CuentaContableDocumento,
                        FechaEmision = row.FechaEmision,
                        FechaVencimiento = row.FechaVencimiento,
                        Total = row.Total,
                        Saldo = row.Saldo,
                        Apagar = row.APagar
                    };

                    _context.PagosDetalles.Add(pagoEstadoDet);
                }

                _context.SaveChanges();


                return Ok(pagoEstado.IdPago);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/SavePago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDashboardAdministrador"), Authorize]
        public async Task<ActionResult> GetDashboardAdministrador(FilterVm model)
        {

            try
            {
                List<DashboardAdministradorVm> dashboard = new List<DashboardAdministradorVm>();
                dashboard.Add(new DashboardAdministradorVm());
                var configPortal = _context.ConfiguracionPortals.ToList();
                int cantidadAnulados = (int)configPortal[0].CantUltPagosAnul;
                int cantidadPagos = (int)configPortal[0].CantUltPagosRec;
                List<PagosCabecera> pagos = new List<PagosCabecera>();
                List<PagosCabecera> anulados = new List<PagosCabecera>();

                if (model.TipoBusqueda == 1)
                {
                    DateTime hoy = DateTime.Today;
                    pagos = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago == hoy).Take(cantidadPagos).ToList();
                    anulados = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => x.IdPagoEstado != 2 && x.FechaPago == hoy).Take(cantidadAnulados).ToList();
                }

                if (model.TipoBusqueda == 2)
                {
                    var lunes = DateUtils.StartOfWeek(DateTime.Now);
                    var domingo = DateUtils.LastDayOfWeek(DateTime.Now);

                    pagos = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago >= lunes && x.FechaPago <= domingo).Take(cantidadPagos).ToList();
                    anulados = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => x.IdPagoEstado != 2 && x.FechaPago >= lunes && x.FechaPago <= domingo).Take(cantidadAnulados).ToList();
                }


                if (model.TipoBusqueda == 3)
                {
                    int mes = DateTime.Today.Month;
                    int anio = DateTime.Today.Year;

                    var primerDiaMes = new DateTime(anio, mes, 1);
                    var ultimoDiaMes = primerDiaMes.AddMonths(1).AddMilliseconds(-1);



                    pagos = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago >= primerDiaMes && x.FechaPago <= ultimoDiaMes).ToList();

                    anulados = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => x.IdPagoEstado != 2 && x.FechaPago >= primerDiaMes && x.FechaPago <= ultimoDiaMes).ToList();
                }

                if (model.TipoBusqueda == 4)
                {
                    int anio = DateTime.Today.Year;
                    var primerDiaAnio = new DateTime(anio, 1, 1);
                    var ultimoDiaAnio = new DateTime(anio, 12, 31);

                    pagos = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago >= primerDiaAnio && x.FechaPago <= ultimoDiaAnio).Take(cantidadPagos).ToList();
                    anulados = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => x.IdPagoEstado != 2 && x.FechaPago >= primerDiaAnio && x.FechaPago <= ultimoDiaAnio).Take(cantidadAnulados).ToList();
                }

                if (model.TipoBusqueda == 5 && model.fechaDesde != null && model.fechaHasta != null)
                {
                    pagos = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago >= model.fechaDesde && x.FechaPago <= model.fechaHasta).Take(cantidadPagos).ToList();
                    anulados = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => x.IdPagoEstado != 2 && x.FechaPago >= model.fechaDesde && x.FechaPago <= model.fechaHasta).Take(cantidadAnulados).ToList();
                }

                if (pagos != null)
                {
                    dashboard[0].MontoTotal = (int)pagos.Sum(x => x.MontoPago);
                    dashboard[0].CantidadDocumentos = pagos.Count();
                }




                foreach (var item in pagos)
                {
                    DashboardAdministradorVm da = new DashboardAdministradorVm();
                    da.ComprobanteContable = item.ComprobanteContable;
                    da.FechaPago = (DateTime)item.FechaPago;
                    da.IdPago = item.IdPago;
                    da.IdPagoEstado = (int)item.IdPagoEstado;
                    da.MontoPago = (int)item.MontoPago;
                    var folio = _context.PagosDetalles.Where(x => x.IdPago == item.IdPago).FirstOrDefault().Folio;
                    if (folio != null)
                    {
                        da.Folio = (int)folio;
                    }
                    dashboard.Add(da);
                }

                foreach (var item in anulados)
                {
                    DashboardAdministradorVm da = new DashboardAdministradorVm();
                    da.ComprobanteContable = item.ComprobanteContable;
                    da.FechaPago = (DateTime)item.FechaPago;
                    da.IdPago = item.IdPago;
                    da.IdPagoEstado = (int)item.IdPagoEstado;
                    da.MontoPago = (int)item.MontoPago;
                    var folio = _context.PagosDetalles.Where(x => x.IdPago == item.IdPago).FirstOrDefault().Folio;
                    if (folio != null)
                    {
                        da.Folio = (int)folio;
                    }
                    dashboard.Add(da);
                }

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDashboardAdministrador";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetExistCompras"), Authorize]
        public async Task<ActionResult> GetExistCompras()
        {

            try
            {
                DashboardAdministradorVm dashboard = new DashboardAdministradorVm();
                var configPortal = _context.ConfiguracionPortals.ToList();
                List<PagosCabecera> pagos = new List<PagosCabecera>();
                List<PagosCabecera> anulados = new List<PagosCabecera>();


                DateTime hoy = DateTime.Today;
                pagos = _context.PagosCabeceras.Where(x => x.FechaPago == hoy).ToList();

                if (pagos.Count() > 0)
                {
                    dashboard.Hoy = true;
                }



                var lunes = DateUtils.StartOfWeek(DateTime.Now);
                var domingo = DateUtils.LastDayOfWeek(DateTime.Now);

                pagos = _context.PagosCabeceras.Where(x => x.FechaPago >= lunes && x.FechaPago <= domingo).ToList();

                if (pagos.Count() > 0)
                {
                    dashboard.Semana = true;
                }

                int mes = DateTime.Today.Month;
                int anio = DateTime.Today.Year;

                var primerDiaMes = new DateTime(anio, mes, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

                pagos = _context.PagosCabeceras.Where(x => x.FechaPago >= primerDiaMes && x.FechaPago <= ultimoDiaMes).ToList();

                if (pagos.Count() > 0)
                {
                    dashboard.Mes = true;
                }

                var primerDiaAnio = new DateTime(anio, 1, 1);
                var ultimoDiaAnio = new DateTime(anio, 12, 31);

                pagos = _context.PagosCabeceras.Where(x => x.FechaPago >= primerDiaAnio && x.FechaPago <= ultimoDiaAnio).ToList();

                if (pagos.Count() > 0)
                {
                    dashboard.Anio = true;
                }

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetExistCompras";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDetallePago/{idPago}"), Authorize]
        public async Task<ActionResult> GetDetallePago(int idPago)
        {

            try
            {
                string tipoDocumento = string.Empty;
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                List<CondicionVentaDTO> condVenta = await sf.GetCondVentaAsync();

                PagoCabeceraVm cabecera = new PagoCabeceraVm();
                List<PagoDetalleVm> detalleCabecera = new List<PagoDetalleVm>();
                ClientesPortalVm cliente = new ClientesPortalVm();
                PagosEstadoVm estado = new PagosEstadoVm();
                List<PasarelaPagoLogVm> pasarela = new List<PasarelaPagoLogVm>();


                var cabeceraPago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (cabeceraPago != null)
                {

                    foreach (var item in cabeceraPago.PagosDetalles)
                    {

                        item.TipoDocumento = sf.GetDescDocumento(item.TipoDocumento);
                        PagoDetalleVm detalles = new PagoDetalleVm
                        {
                            APagar = (float)item.Apagar,
                            FechaEmision = (DateTime)item.FechaEmision,
                            FechaVencimiento = (DateTime)item.FechaVencimiento,
                            Folio = (int)item.Folio,
                            IdPago = item.IdPago,
                            IdPagoDetalle = item.IdPagoDetalle,
                            Saldo = (float)item.Saldo,
                            TipoDocumento = item.TipoDocumento,
                            Total = (float)item.Total
                        };
                        detalleCabecera.Add(detalles);
                    }
                    foreach (var item in cabeceraPago.PasarelaPagoLogs)
                    {
                        PasarelaPagoLogVm log = new PasarelaPagoLogVm
                        {
                            Codigo = item.Codigo,
                            Cuotas = (int)item.Cuotas,
                            Estado = item.Estado,
                            Fecha = (DateTime)item.Fecha,
                            Id = item.Id,
                            IdPago = (int)item.IdPago,
                            IdPasarela = (int)item.IdPasarela,
                            MedioPago = item.MedioPago,
                            Monto = (decimal)item.Monto,
                            OrdenCompra = item.OrdenCompra,
                            Tarjeta = item.Tarjeta,
                            Token = item.Token,
                            Url = item.Url
                        };
                        pasarela.Add(log);
                    }
                    if (cabeceraPago.IdClienteNavigation != null)
                    {
                        cliente.ActivaCuenta = (int)cabeceraPago.IdClienteNavigation.ActivaCuenta;
                        cliente.CodAux = cabeceraPago.IdClienteNavigation.CodAux;
                        cliente.Correo = cabeceraPago.IdClienteNavigation.Correo;
                        cliente.IdCliente = cabeceraPago.IdClienteNavigation.IdCliente;
                        cliente.Nombre = cabeceraPago.IdClienteNavigation.Nombre;
                        cliente.Rut = cabeceraPago.IdClienteNavigation.Rut;

                        List<ClienteDTO> listaClientes = await sf.BuscarClienteSoftland2Async(cabeceraPago.IdClienteNavigation.CodAux, "", "");
                        var aux = listaClientes.FirstOrDefault();
                        if (aux != null)
                        {
                            cliente.Direccion = aux.DirAux + " " + aux.DirNum;

                            if (condVenta.Where(x => x.CveCod == aux.CodCondVenta).FirstOrDefault() != null)
                            {
                                cliente.CondVenta = condVenta.Where(x => x.CveCod == aux.CodCondVenta).FirstOrDefault().CveDes;
                            }

                        }

                        cabecera.IdCliente = (int)cabeceraPago.IdCliente;
                    }
                    else
                    {
                        cliente.CodAux = cabeceraPago.CodAux;
                        cliente.Correo = cabeceraPago.Correo;
                        cliente.Rut = cabeceraPago.Rut;
                        cliente.Nombre = cabeceraPago.Nombre;

                    }




                    estado.IdPagosEstado = cabeceraPago.IdPagoEstadoNavigation.IdPagosEstado;
                    estado.Nombre = cabeceraPago.IdPagoEstadoNavigation.Nombre;

                    cabecera.ClientesPortal = cliente;
                    cabecera.ComprobanteContable = cabeceraPago.ComprobanteContable;
                    cabecera.FechaPago = (DateTime)cabeceraPago.FechaPago;
                    cabecera.HoraPago = cabeceraPago.HoraPago;

                    cabecera.IdPago = cabeceraPago.IdPago;
                    cabecera.IdPagoEstado = (int)cabeceraPago.IdPagoEstado;
                    cabecera.MontoPago = (float)cabeceraPago.MontoPago;
                    cabecera.PagosDetalle = detalleCabecera;
                    cabecera.PagosEstado = estado;
                    cabecera.PasarelaPagoLog = pasarela;
                }

                return Ok(cabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDetallePago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDatosPagoRapido/{idPago}"), Authorize]
        public async Task<ActionResult> GetDatosPagoRapido(int idPago)
        {

            try
            {
                string tipoDocumento = string.Empty;
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                List<CondicionVentaDTO> condVenta = await sf.GetCondVentaAsync();

                PagoCabeceraVm cabecera = new PagoCabeceraVm();

                var cabeceraPago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (cabeceraPago != null)
                {
                    cabecera.CodAux = cabeceraPago.CodAux;
                    cabecera.Rut = cabeceraPago.Rut;
                    cabecera.Correo = cabeceraPago.Correo;
                    cabecera.Nombre = cabecera.Nombre;

                }

                return Ok(cabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDatosPagoRapido";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentoPDF"), Authorize]
        public async Task<ActionResult> GetDocumentoPDF(CabeceraDocumentoDTO cabecera)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                string pdfBase64 = await sf.obtenerPDFDocumento(cabecera.Folio, cabecera.Tipo, cabecera.SubTipo);
                return Ok(pdfBase64);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentoPDF";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EnviaDocumentoPDF"), Authorize]
        public async Task<ActionResult> EnviaDocumentoPDF(EnvioDocumento envio)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);

                MailService mc = new MailService(_context, _webHostEnvironment);
                MailViewModel mv = new MailViewModel();
                mv.asunto = "Documento " + envio.folio.ToString();
                List<Attachment> adjuntos = new List<Attachment>();
                if (envio.TipoDocAEnviar == 1)
                {

                    if (envio.docsAEnviar == 1 || envio.docsAEnviar == 3)
                    {
                        string pdfBase64 = await sf.obtenerPDFDocumento(envio.folio, envio.tipoDoc, envio.subTipoDoc);
                        if (!string.IsNullOrEmpty(pdfBase64))
                        {
                            byte[] stemp = Convert.FromBase64String(pdfBase64);
                            Stream stream = new MemoryStream(stemp);
                            adjuntos.Add(new Attachment(stream, Path.GetFileName(mv.asunto + ".pdf"), "application/pdf"));
                        }
                    }

                    if (envio.docsAEnviar == 2 || envio.docsAEnviar == 3)
                    {
                        DocumentosVm xml = await sf.obtenerXMLDTEAsync(envio.folio, envio.codAux, envio.tipoDoc);
                        if (!string.IsNullOrEmpty(xml.Base64))
                        {
                            byte[] stemp = Convert.FromBase64String(xml.Base64);
                            Stream stream = new MemoryStream(stemp);
                            adjuntos.Add(new Attachment(stream, Path.GetFileName(xml.NombreArchivo + ".xml"), "text/xml"));
                        }

                    }
                }
                if (envio.TipoDocAEnviar == 2)
                {
                    string pdfBase64 = await sf.obtenerPDFDocumentoNv(envio.folio.ToString(), envio.codAux);
                    if (!string.IsNullOrEmpty(pdfBase64))
                    {
                        byte[] stemp = Convert.FromBase64String(pdfBase64);
                        Stream stream = new MemoryStream(stemp);
                        adjuntos.Add(new Attachment(stream, Path.GetFileName(mv.asunto + ".pdf"), "application/pdf"));
                    }

                }

                if (adjuntos.Count > 0)
                {
                    mv.email_destinatario = envio.destinatarios;
                    mv.adjuntos = adjuntos;
                    mv.tipo = 4;
                    await mc.EnviarCorreosAsync(mv);
                    return Ok(1);
                }
                else
                {
                    return Ok(0);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/EnviaDocumentoPDF";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetResumenContable/{codAux}"), Authorize]
        public async Task<ActionResult> GetResumenContable(string codAux)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                ResumenContableDTO resumen = await sf.obtenerResumenContable(codAux);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetResumenContable";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentoPDFNv"), Authorize]
        public async Task<ActionResult> GetDocumentoPDFNv(NotaVentaClienteDTO notaVenta)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);

                string pdfBase64 = await sf.obtenerPDFDocumentoNv(notaVenta.NVNumero, notaVenta.CodAux);
                return Ok(pdfBase64);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentoPDFNv";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesFiltro"), Authorize]
        public async Task<ActionResult> GetClientesFiltro(FilterVm model)
        {

            try
            {

                SoftlandService softlandService = new SoftlandService(_context,_webHostEnvironment);
                List<ClientesPortal> clients = softlandService.GetClientesSoftlandCobranza(model);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentoPDFNv";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosDashboarddAdmin"), Authorize]
        public async Task<ActionResult> GetDocumentosDashboarddAdmin()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<DashboardDocumentosVm> retorno = new List<DashboardDocumentosVm>();
                List<ClienteSaldosDTO> documentos = await sf.GetDocumentosDashboardAdminAsync("", 0);

                var documentosVencidos = documentos.Where(x => x.Estado == "Vencido" || x.Estado == "V").ToList();
                var documentosPendientes = documentos;
                var configPago = _context.ConfiguracionPagoClientes.FirstOrDefault();
                configPago.DiasPorVencer = configPago.DiasPorVencer == null ? 0 : configPago.DiasPorVencer;
                var documentosPorVencer = documentos.Where(x => (int)(Convert.ToDateTime(x.FechaVcto).Date - DateTime.Now.Date).TotalDays <= configPago.DiasPorVencer && x.Estado != "V").ToList();
                if (documentosVencidos.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosVencidos)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.Saldo);
                    }

                    d.Estado = "VENCIDO"; 
                    retorno.Add(d);
                }

                if (documentosPorVencer.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosPorVencer)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.Saldo);
                    }

                    d.Estado = "POR VENCER";
                    retorno.Add(d);
                }

                if (documentosPendientes.Count > 0)
                {
                    DashboardDocumentosVm d = new DashboardDocumentosVm();
                    foreach (var item in documentosPendientes)
                    {
                        d.CantidadDocumentos += 1;
                        d.TotalDocumentos = d.TotalDocumentos + Convert.ToDecimal(item.Saldo);
                    }

                    d.Estado = "PENDIENTES";
                    retorno.Add(d);
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
                log.Ruta = "api/ClientesPortal/GetDocumentosDashboarddAdmin";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosAdmin/{codAux}/{estado}"), Authorize]
        public async Task<ActionResult> GetDocumentosAdmin(string codAux, int estado)
        {

            try
            {
                if (codAux == "0")
                {
                    codAux = "";
                }

                List<ClienteDTO> clientes = new List<ClienteDTO>();
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ClienteSaldosDTO> documentos = await sf.GetDocumentosDashboardAdminAsync(codAux, estado);
                var documentosAgrupados = documentos.GroupBy(x => x.CodAux).ToList();

                foreach (var item in documentosAgrupados)
                {
                    var cliente = await sf.GetClienteSoftlandAsync(item.Key);
                    cliente.Documentos = documentos.Where(x => x.CodAux == item.Key).OrderByDescending(x => x.FechaEmision).ToList();
                    cliente.TotalDocumentos = cliente.Documentos.Sum(x => x.MontoBase);
                    cliente.TotalSaldo = cliente.Documentos.Sum(x => x.SaldoBase);
                    cliente.CantidadDocumentos = cliente.Documentos.Count();
                    clientes.Add(cliente);
                }


                return Ok(clientes);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosAdmin";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCorreosDTE/{rut}"), Authorize]
        public async Task<ActionResult> GetCorreosDTE(string rut)
        {

            try
            {
                string rutFormateado = rut.Replace(".", "");
                var correos = _admin.CsvEmpresasSiis.Where(x => x.Rut == rutFormateado).ToList();
                return Ok(correos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetCorreosDTE";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteGuiasDespacho"), Authorize]
        public async Task<ActionResult> GetClienteGuiasDespacho(ClientesPortalVm model)
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<GuiaDespachoDTO> guias = await sf.GetGuiasPendientes(model.CodAux);
                return Ok(guias);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteGuiasDespacho";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPDFPago/{numComprobante}")]
        public async Task<ActionResult> GetPDFPago(string numComprobante)
        {

            try
            {
                numComprobante = Encrypt.Base64Decode(numComprobante);
                string base64 = string.Empty;
                var pagoCabecera = _context.PagosCabeceras.Where(x => x.ComprobanteContable == numComprobante).FirstOrDefault();
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;


                if (pagoCabecera != null)
                {
                    string fecha = pagoCabecera.FechaPago.Value.Day.ToString() + "/" + pagoCabecera.FechaPago.Value.Month.ToString() + "/" + pagoCabecera.FechaPago.Value.Year.ToString();
                    string hora = pagoCabecera.HoraPago;
                    string comprobanteHtml = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/invoice.html")))
                    {
                        comprobanteHtml = reader.ReadToEnd();
                    }
                   
                    comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                        .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", numComprobante).Replace("{MONTOTOTAL}", pagoCabecera.MontoPago.Value.ToString("N0"));

                    string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    SoftlandService softlandService = new SoftlandService(_context,_webHostEnvironment);
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync();

                    //Obtenemos detalle
                    var pagosDetalle = _context.PagosDetalles.Where(x => x.IdPago == pagoCabecera.IdPago).ToList();
                    foreach (var det in pagosDetalle)
                    {
                        var tipoDoc = tiposDocumentos.Where(x => x.CodDoc == det.TipoDocumento).FirstOrDefault();

                        reemplazoDetalle = reemplazoDetalle + partes[1].Replace("{NUMDOC}", det.Folio.ToString()).Replace("{TIPODOC}", tipoDoc.DesDoc).Replace("{MONTODOC}", det.Total.Value.ToString("N0")).Replace("{SALDODOC}", det.Saldo.Value.ToString("N0"))
                            .Replace("{PAGADODOC}", det.Apagar.Value.ToString("N0"));

                    }

                    partes[1] = reemplazoDetalle;

                    string htmlFinal = string.Empty;

                    foreach (var p in partes)
                    {
                        htmlFinal = htmlFinal + p;
                    }


                    SelectPdf.HtmlToPdf converter2 = new SelectPdf.HtmlToPdf();
                    SelectPdf.PdfDocument doc2 = converter2.ConvertHtmlString(htmlFinal);

                    using (MemoryStream memoryStream2 = new MemoryStream())
                    {
                        doc2.Save(memoryStream2);

                        byte[] bytes = memoryStream2.ToArray();

                        base64 = System.Convert.ToBase64String(bytes);
                        memoryStream2.Close();
                    }
                    doc2.Close();
                }
                return Ok(base64);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetPDFPago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RecuperarContrasena"), Authorize]
        public async Task<ActionResult> RecuperarContrasena(ClienteDTO clienteVm)
        {

            try
            {
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                if (clienteVm.Rut == configEmpresa.RutEmpresa)
                {
                    var usuario = _context.Usuarios.Where(x => x.Email == clienteVm.Correo).FirstOrDefault();

                    if (usuario != null)
                    {
                        HashPassword aux = new HashPassword();
                        var clave = RandomPassword.GenerateRandomPassword();
                        usuario.Password = aux.HashCode(clave);
                        _context.Entry(usuario).Property(x => x.Password).IsModified = true;
                        _context.SaveChanges();


                        var logCorreo = new LogCorreo();
                        logCorreo.Fecha = DateTime.Now;
                        logCorreo.Rut = configEmpresa.RutEmpresa;
                        logCorreo.Tipo = "Acceso";
                        logCorreo.Estado = "PENDIENTE";

                        _context.LogCorreos.Add(logCorreo);
                        _context.SaveChanges();

                        var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{Texto}", "Se solicito una recuperacion de clave para su cuenta, debera ingresar con la nueva clave que se indica a continuación");
                        body = body.Replace("{logo}", configCorreo.LogoCorreo);
                        body = body.Replace("{NOMBRE}", usuario.Email);
                        body = body.Replace("{CLAVE}", clave);
                        body = body.Replace("{Titulo}", "Recuperar Clave");
                        try
                        {
                            using (MailMessage mailMessage = new MailMessage())
                            {
                                mailMessage.To.Add(usuario.Email);

                                mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                                mailMessage.Subject = "Recuperar Clave";
                                mailMessage.Body = body;
                                mailMessage.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = configCorreo.SmtpServer;
                                smtp.EnableSsl = (configCorreo.Ssl == 1) ? true : false;
                                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                                NetworkCred.UserName = configCorreo.Usuario;
                                NetworkCred.Password = Encrypt.Base64Decode(configCorreo.Clave); //Encrypt.DesEncriptar(configCorreo.Clave);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = NetworkCred;
                                smtp.Port = Convert.ToInt32(configCorreo.Puerto);
                                smtp.Send(mailMessage);

                            }
                        }
                        catch (Exception ex)
                        {
                            _context.LogCorreos.Attach(logCorreo);
                            logCorreo.Estado = "Error al enviar correo";
                            logCorreo.Error = ex.Message;
                            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                            _context.SaveChanges();
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

                        return Ok(1);
                    }
                    else
                    {
                        return Ok(-1);
                    }
                }
                else
                {
                    var cliente = _context.ClientesPortals.Where(x => x.Rut == clienteVm.Rut && x.CodAux == clienteVm.CodAux && x.Correo == clienteVm.Correo).FirstOrDefault();
                    if (cliente != null)
                    {
                        HashPassword aux = new HashPassword();
                        var clave = RandomPassword.GenerateRandomPassword();
                        cliente.Clave = aux.HashCode(clave);
                        _context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                        _context.SaveChanges();


                        var logCorreo = new LogCorreo();
                        logCorreo.Fecha = DateTime.Now;
                        logCorreo.Rut = cliente.Rut;
                        logCorreo.CodAux = cliente.CodAux;
                        logCorreo.Tipo = "Acceso";
                        logCorreo.Estado = "PENDIENTE";

                        _context.LogCorreos.Add(logCorreo);
                        _context.SaveChanges();

                        var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{Texto}", configCorreo.TextoRecuperarClave);
                        body = body.Replace("{logo}", configCorreo.LogoCorreo);
                        body = body.Replace("{NOMBRE}", cliente.Correo);
                        body = body.Replace("{CLAVE}", clave);
                        body = body.Replace("{Titulo}", configCorreo.TituloRecuperarClave);
                        try
                        {
                            using (MailMessage mailMessage = new MailMessage())
                            {
                                mailMessage.To.Add(cliente.Correo);

                                mailMessage.From = new MailAddress(configCorreo.CorreoOrigen, configCorreo.NombreCorreos);
                                mailMessage.Subject = configCorreo.AsuntoRecuperarClave;
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
                        }
                        catch (Exception ex)
                        {
                            _context.LogCorreos.Attach(logCorreo);
                            logCorreo.Estado = "Error al enviar correo";
                            logCorreo.Error = ex.Message;
                            _context.Entry(logCorreo).Property(x => x.Estado).IsModified = true;
                            _context.Entry(logCorreo).Property(x => x.Error).IsModified = true;
                            _context.SaveChanges();
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
                        registro.IdUsuario = cliente.IdCliente;

                        _context.RegistroEnvioCorreos.Add(registro);
                        _context.SaveChanges();

                        return Ok(1);
                    }
                    else
                    {
                        return Ok(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/RecuperarContrasena";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDocumentosPagados"), Authorize]
        public async Task<ActionResult> GetDocumentosPagados()
        {

            try
            {
                List<PagoCabeceraVm> retorno = new List<PagoCabeceraVm>();

                var documentos = _context.PagosCabeceras.Where(x => x.IdPagoEstado == 2 || x.IdPagoEstado == 4).OrderByDescending(x => x.IdPago).ToList();

                foreach (var item in documentos)
                {
                    PagoCabeceraVm pago = new PagoCabeceraVm();
                    pago.CodAux = item.CodAux;
                    pago.ComprobanteContable = item.ComprobanteContable;
                    pago.Correo = item.Correo;
                    pago.EsPagoRapido = (int)item.EsPagoRapido;
                    pago.FechaPago = (DateTime)item.FechaPago;
                    pago.HoraPago = item.HoraPago;
                    pago.IdCliente = (int)item.IdCliente;
                    pago.IdPago = item.IdPago;
                    pago.IdPagoEstado = (int)item.IdPagoEstado;
                    pago.IdPasarela = (int)item.IdPasarela;
                    pago.MontoPago = (Single)item.MontoPago;
                    pago.Nombre = item.Nombre;
                    pago.Rut = item.Rut;

                    List<PagoDetalleVm> detalles = new List<PagoDetalleVm>();
                    foreach (var det in item.PagosDetalles)
                    {
                        PagoDetalleVm detalle = new PagoDetalleVm();
                        detalle.APagar = (Single)det.Apagar;
                        detalle.CuentaContableDocumento = det.CuentaContableDocumento;
                        detalle.FechaEmision = (DateTime)det.FechaEmision;
                        detalle.FechaVencimiento = (DateTime)det.FechaVencimiento;
                        detalle.Folio = (int)det.Folio;
                        detalle.IdPago = det.IdPago;
                        detalle.IdPagoDetalle = det.IdPagoDetalle;
                        detalle.Saldo = (Single)det.Saldo;
                        detalle.TipoDocumento = det.TipoDocumento;
                        detalle.Total = (Single)det.Total;
                        detalles.Add(detalle);
                    }

                    List<PasarelaPagoLogVm> logs = new List<PasarelaPagoLogVm>();
                    foreach (var log in item.PasarelaPagoLogs)
                    {
                        PasarelaPagoLogVm detalle = new PasarelaPagoLogVm();
                        detalle.Codigo = log.Codigo;
                        detalle.Cuotas = (int)log.Cuotas;
                        detalle.Estado = log.Estado;
                        detalle.Fecha = (DateTime)log.Fecha;
                        detalle.Id = log.Id;
                        detalle.IdPago = (int)log.IdPago;
                        detalle.IdPasarela = (int)log.IdPasarela;
                        detalle.MedioPago = log.MedioPago;
                        detalle.Monto = (decimal)log.Monto;
                        detalle.OrdenCompra = log.OrdenCompra;
                        detalle.Tarjeta = log.Tarjeta;
                        detalle.Token = log.Token;
                        detalle.Url = log.Url;

                        logs.Add(detalle);
                    }



                    pago.PasarelaPagoLog = logs;
                    pago.PagosDetalle = detalles;

                    ClientesPortalVm cliente = new ClientesPortalVm();

                    if (item.IdClienteNavigation != null)
                    {
                        cliente.CodAux = item.IdClienteNavigation.CodAux;
                        cliente.Correo = item.IdClienteNavigation.Correo;
                        cliente.IdCliente = item.IdClienteNavigation.IdCliente;
                        cliente.Nombre = item.IdClienteNavigation.Nombre;
                        cliente.Rut = item.IdClienteNavigation.Rut;

                    }
                    else
                    {
                        var c = _context.ClientesPortals.Where(x => x.CodAux == item.CodAux).FirstOrDefault();
                        if (c != null)
                        {
                            cliente.CodAux = c.CodAux;
                            cliente.Correo = c.Correo;
                            cliente.IdCliente = c.IdCliente;
                            cliente.Nombre = c.Nombre;
                            cliente.Rut = c.Rut;
                        }
                    }


                    pago.ClientesPortal = cliente;

                    retorno.Add(pago);
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
                log.Ruta = "api/ClientesPortal/GetDocumentosPagados";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTopDeudores"), Authorize]
        public async Task<ActionResult> GetTopDeudores()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ClienteSaldosDTO> documentos = await sf.GetDocumentosDeudores("", 0);
                documentos = documentos.OrderByDescending(x => x.FechaEmision).ToList();

                List<DeudorVm> list = documentos.GroupBy(x => new { x.CodAux }).Select(g => new DeudorVm
                {
                    CodAux = g.Key.CodAux,
                    TotalDeuda = g.Where(j => Convert.ToDouble(j.SaldoBase) > 0).Sum(j => Convert.ToDouble(j.SaldoBase)),
                    CantidadDocumentos = g.Count(x => x.CodAux == x.CodAux)
                }).ToList().OrderByDescending(z => z.TotalDeuda).Take(10).ToList();

                SoftlandService softlandService = new SoftlandService(_context,_webHostEnvironment);
                foreach (var item in list)
                {
                    var cliente = await softlandService.BuscarClienteSoftlandAsync(item.CodAux, "", "");
                    if (cliente.Count > 0)
                    {
                        item.Rut = cliente[0].Rut;
                        item.RazonSocial = cliente[0].Nombre;
                    }
                }

                return Ok(list);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetTopDeudores";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetDeudaVsPagos"), Authorize]
        public async Task<ActionResult> GetDeudaVsPagos()
        {

            try
            {
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                List<ClienteSaldosDTO> documentos = await sf.GetDocumentosDashboardAdminAsync("", 0);
                var mesAñoAnterior = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, 1);
                DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime ultimoDiaMesActual = primerDiaMesActual.AddMonths(1).AddMilliseconds(-1);
                documentos = documentos.OrderBy(x => x.FechaVcto).Where(x => x.FechaVcto.Value.Date >= mesAñoAnterior.Date && x.FechaVcto.Value.Date <= ultimoDiaMesActual.Date).ToList();


                List<DeudaPagosVm> listaDeuda = documentos.GroupBy(x => new { x.FechaVcto.Value.Month, x.FechaVcto.Value.Year }).Select(g => new DeudaPagosVm
                {
                    Mes = g.Key.Month,
                    Anio = g.Key.Year,
                    FechaTexto = g.Key.Month.ToString("D2") + "/" + g.Key.Year.ToString().Substring(2),
                    TotalDeuda = g.Sum(j => Convert.ToDouble(j.MontoBase)),
                    Fecha = new DateTime(g.Key.Year, g.Key.Month, 1)
                }).ToList();

                var pagos = _context.PagosCabeceras.OrderBy(x => x.FechaPago).Where(x => x.IdPagoEstado == 2 || x.IdPagoEstado == 4 && x.FechaPago >= mesAñoAnterior && x.FechaPago <= ultimoDiaMesActual).ToList();


                List<DeudaPagosVm> listaPagos = pagos.GroupBy(x => new { x.FechaPago.Value.Month, x.FechaPago.Value.Year }).Select(g => new DeudaPagosVm
                {
                    Mes = g.Key.Month,
                    Anio = g.Key.Year,
                    FechaTexto = g.Key.Month.ToString("D2") + "/" + g.Key.Year.ToString().Substring(2),
                    TotalPagos = (double)g.Sum(j => j.MontoPago),
                    Fecha = new DateTime(g.Key.Year, g.Key.Month, 1)
                }).ToList();

                foreach (var item in listaDeuda)
                {
                    var p = listaPagos.Where(x => x.Mes == item.Mes && x.Anio == item.Anio).FirstOrDefault();
                    if (p != null)
                    {
                        item.TotalPagos = p.TotalPagos;
                    }
                    else
                    {
                        item.TotalPagos = 0;
                    }
                }



                for (int i = 0; i < 13; i++)
                {
                    var exist = listaDeuda.Where(x => x.Anio == ultimoDiaMesActual.Year && x.Mes == ultimoDiaMesActual.Month).FirstOrDefault();
                    if (exist == null)
                    {
                        listaDeuda.Add(new DeudaPagosVm
                        {
                            Anio = ultimoDiaMesActual.Year,
                            Mes = ultimoDiaMesActual.Month,
                            FechaTexto = ultimoDiaMesActual.Month.ToString("D2") + "/" + ultimoDiaMesActual.Year.ToString(),
                            TotalDeuda = 0,
                            TotalPagos = 0,
                            Fecha = new DateTime(ultimoDiaMesActual.Year, ultimoDiaMesActual.Month, 1)
                        });
                    }

                    ultimoDiaMesActual = ultimoDiaMesActual.AddMonths(-1);
                }

                listaDeuda = listaDeuda.OrderBy(x => x.Fecha).ToList();

                return Ok(listaDeuda);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDeudaVsPagos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesByCodAux"), Authorize]
        public async Task<ActionResult> GetClientesByCodAux(List<ClientesPortalVm> cliente)
        {

            try
            {
                List<ClienteDTO> retorno = new List<ClienteDTO>();

                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                foreach (var item in cliente)
                {
                    var c = await sf.GetClienteSoftlandAsync(item.CodAux);
                    retorno.Add(c);
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
                log.Ruta = "api/ClientesPortal/GetClientesByCodAux";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnviaCorreoComprobante/{idPago}"), Authorize]
        public async Task<ActionResult> EnviaCorreoComprobante(int idPago)
        {

            try
            {
                List<ClienteDTO> retorno = new List<ClienteDTO>();

                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (pago != null)
                {
                    if (!string.IsNullOrEmpty(pago.ComprobanteContable))
                    {
                        var config = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                        var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                        string fecha = pago.FechaPago.Value.Day.ToString() + "/" + pago.FechaPago.Value.Month.ToString() + "/" + pago.FechaPago.Value.Year.ToString();
                        string hora = pago.HoraPago;
                        string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                        string comprobanteHtml = string.Empty;
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "~/Uploads/MailTemplates/invoice.html")))
                        {
                            comprobanteHtml = reader.ReadToEnd();
                        }
                        
                        comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                            .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", pago.ComprobanteContable).Replace("{MONTOTOTAL}", pago.MontoPago.Value.ToString("N0").Replace(",", "."));

                        string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                        string reemplazoDetalle = string.Empty;

                        SoftlandService softlandService = new SoftlandService(_context,_webHostEnvironment);
                        var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync();
                        foreach (var det in pago.PagosDetalles)
                        {
                            var tipoDoc = tiposDocumentos.Where(x => x.CodDoc == det.TipoDocumento).FirstOrDefault();

                            reemplazoDetalle = reemplazoDetalle + partes[1].Replace("{NUMDOC}", det.Folio.ToString()).Replace("{TIPODOC}", tipoDoc.DesDoc).Replace("{MONTODOC}", det.Total.Value.ToString("N0").Replace(",", ".")).Replace("{SALDODOC}", det.Saldo.Value.ToString("N0").Replace(",", "."))
                                .Replace("{PAGADODOC}", det.Apagar.Value.ToString("N0").Replace(",", "."));

                        }

                        partes[1] = reemplazoDetalle;

                        string htmlFinal = string.Empty;

                        foreach (var p in partes)
                        {
                            htmlFinal = htmlFinal + p;
                        }

                        SelectPdf.HtmlToPdf converter2 = new SelectPdf.HtmlToPdf();
                        SelectPdf.PdfDocument doc2 = converter2.ConvertHtmlString(htmlFinal);


                        MailViewModel vm = new MailViewModel();
                        List<Attachment> listaAdjuntos = new List<Attachment>();

                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            doc2.Save(memoryStream2);

                            byte[] bytes = memoryStream2.ToArray();

                            memoryStream2.Close();
                            Attachment comprobanteTBK = new Attachment(new MemoryStream(bytes), "Comprobante Pago.pdf");
                            listaAdjuntos.Add(comprobanteTBK);
                        }
                        doc2.Close();


                        vm.adjuntos = listaAdjuntos;
                        vm.tipo = 5;
                        vm.nombre = pago.Nombre;
                        vm.email_destinatario = pago.Correo;
                        MailService mail = new MailService(_context,_webHostEnvironment);
                        await mail.EnviarCorreosAsync(vm);



                        SelectPdf.HtmlToPdf converter3 = new SelectPdf.HtmlToPdf();
                        SelectPdf.PdfDocument doc3 = converter3.ConvertHtmlString(htmlFinal);
                        List<Attachment> listaAdjuntos2 = new List<Attachment>();
                        using (MemoryStream memoryStream3 = new MemoryStream())
                        {

                            doc3.Save(memoryStream3);

                            byte[] bytes = memoryStream3.ToArray();

                            memoryStream3.Close();
                            Attachment comprobanteTBK2 = new Attachment(new MemoryStream(bytes), "Comprobante Pago.pdf");
                            listaAdjuntos2.Add(comprobanteTBK2);
                        }
                        doc3.Close();

                        var cliente = _context.ClientesPortals.Where(x => x.CodAux == pago.CodAux).FirstOrDefault();
                        var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                        MailViewModel vm2 = new MailViewModel();
                        vm2.tipo = 6;
                        vm2.nombre = "";
                        vm2.asunto = "";
                        vm2.mensaje = pago.ComprobanteContable + "|" + pago.CodAux + "|" + pago.Correo + "|" + pago.MontoPago.Value.ToString("N0").Replace(",", ".") + "|" + cliente.Nombre + "|" + cliente.Rut;
                        vm2.adjuntos = listaAdjuntos2;
                        vm2.email_destinatario = configCorreo.CorreoAvisoPago;
                        await mail.EnviarCorreosAsync(vm2);
                    }

                }


                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/enviaCorreoComprobante";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
