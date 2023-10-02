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
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;


namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesPortalController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClientesPortalController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
            _contextAccessor = contextAccessor;
        }

        [HttpPost("SendAccesosCliente"), Authorize]
        public async Task<ActionResult<int>> SendAccesosClienteAsync(EnvioAccesoClienteVm envio)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/SendAccesosCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

            try
            {
                if (envio == null)
                {
                    return BadRequest();
                }
                PortalClientesSoftlandContext aux = new PortalClientesSoftlandContext(_contextAccessor);
                this.enviaAccesoAsync(envio, aux, _webHostEnvironment);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "api/ClientesPortal/SendAccesosCliente/"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest();
            }
        }

        private async Task enviaAccesoAsync(EnvioAccesoClienteVm envio, PortalClientesSoftlandContext _context, IWebHostEnvironment _webHostEnvironment)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/enviaAccesoAsync";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

            try
            {
                using (var context = _context)
                {

                    if (envio.EnviaTodos == 0)
                    {
                        var value = envio.value;

                        var configCorreo = context.ConfiguracionCorreos.FirstOrDefault();
                        var configEmpresa = context.ConfiguracionEmpresas.FirstOrDefault();
                        MailService emailService = new MailService(context, _webHostEnvironment);
                        SoftlandService softlandService = new SoftlandService(context, _webHostEnvironment);
                        List<ClienteAPIDTO> clientesSinDatos = new List<ClienteAPIDTO>();
                        Boolean errorEnvio = false;
                        string[] cargos = value[0].CodCargo.Split(';');
                        string body = string.Empty;

                        if (value.Count > 0)
                        {
                            foreach (var item in value)
                            {

                                var contactos = await softlandService.GetAllContactosAsync(item.CodAux, logApi.Id);

                                List<ContactoDTO> contactosFiltrados = new List<ContactoDTO>();
                                foreach (var c in contactos)
                                {
                                    var exist = cargos.Where(x => x == c.CodCargo).FirstOrDefault();

                                    if (exist != null)
                                    {
                                        contactosFiltrados.Add(c);
                                    }
                                }

                                if (contactosFiltrados.Count() == 0 && value[0].EnviarTodosContactos == true)
                                {
                                    contactosFiltrados = contactos;
                                    contactosFiltrados = contactosFiltrados.DistinctBy(x => x.Correo).ToList();
                                }

                                if (contactosFiltrados.Count > 0)
                                {
                                    contactosFiltrados = contactosFiltrados.DistinctBy(x => x.Correo).ToList();
                                    contactosFiltrados = contactosFiltrados.Where(x => !string.IsNullOrWhiteSpace(x.Correo)).ToList();
                                    if (contactosFiltrados.Count == 0)
                                    {
                                        clientesSinDatos.Add(item);
                                    }
                                    foreach (var c in contactosFiltrados)
                                    {


                                        var clave = RandomPassword.GenerateRandomPassword();

                                        var cliente = context.ClientesPortals.Where(x => x.Rut == item.RutAux && x.CodAux == item.CodAux && x.Correo == c.Correo).FirstOrDefault();
                                        if (cliente == null) //NUEVO ACCESO
                                        {
                                            var nuevoCliente = new ClientesPortal();
                                            nuevoCliente.Rut = item.RutAux;
                                            nuevoCliente.CodAux = item.CodAux;
                                            nuevoCliente.Nombre = item.NomAux;
                                            nuevoCliente.Correo = c.Correo.ToLower();
                                            nuevoCliente.Clave = clave;
                                            nuevoCliente.ActivaCuenta = 0;

                                            context.ClientesPortals.Add(nuevoCliente);
                                            context.SaveChanges();

                                        }
                                        else //ACTUALIZA ACCESO
                                        {
                                            cliente.Correo = c.Correo.ToLower();
                                            cliente.ActivaCuenta = 0;
                                            cliente.Clave = clave;
                                            context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                            context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                            context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                            context.SaveChanges();
                                        }

                                        var envioConError = emailService.EnviaAcceso(item, c.Correo, clave);
                                        if (envioConError)
                                        {
                                            errorEnvio = true;
                                        }
                                    }
                                }
                                else if (contactosFiltrados.Count() == 0 && value[0].EnviarFicha == true)
                                {
                                    if (String.IsNullOrWhiteSpace(item.EMail))
                                    {
                                        clientesSinDatos.Add(item);
                                    }
                                    else
                                    {
                                        var clave = RandomPassword.GenerateRandomPassword();

                                        var cliente = context.ClientesPortals.Where(x => x.Rut == item.RutAux && x.CodAux == item.CodAux && x.Correo == item.EMail).FirstOrDefault();
                                        if (cliente == null) //NUEVO ACCESO
                                        {
                                            var nuevoCliente = new ClientesPortal();
                                            nuevoCliente.Rut = item.RutAux;
                                            nuevoCliente.CodAux = item.CodAux;
                                            nuevoCliente.Nombre = item.NomAux;
                                            nuevoCliente.Correo = item.EMail.ToLower();
                                            nuevoCliente.Clave = clave;
                                            nuevoCliente.ActivaCuenta = 0;
                                            context.ClientesPortals.Add(nuevoCliente);
                                            context.SaveChanges();

                                        }
                                        else //ACTUALIZA ACCESO
                                        {
                                            cliente.Correo = item.EMail.ToLower();
                                            cliente.ActivaCuenta = 0;
                                            cliente.Clave = clave;
                                            context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                            context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                            context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                            context.SaveChanges();
                                        }

                                        var envioConError = emailService.EnviaAcceso(item, item.EMail, clave);
                                        if (envioConError)
                                        {
                                            errorEnvio = true;
                                            clientesSinDatos.Add(item);
                                        }
                                    }

                                }
                                else
                                {
                                    clientesSinDatos.Add(item);
                                }
                            }

                            emailService.EnviaNotificacionEjecucionAccesos(clientesSinDatos);
                        }

                    }
                    else
                    {
                        var value = envio.value;
                        var configCorreo = context.ConfiguracionCorreos.FirstOrDefault();
                        var configEmpresa = context.ConfiguracionEmpresas.FirstOrDefault();
                        MailService emailService = new MailService(context, _webHostEnvironment);
                        SoftlandService softlandService = new SoftlandService(context, _webHostEnvironment);
                        List<ClienteAPIDTO> clientesSinDatos = new List<ClienteAPIDTO>();
                        Boolean errorEnvio = false;
                        string[] cargos = value[0].CodCargo.Split(';');
                        string body = string.Empty;

                        var clientes = await softlandService.BuscarClienteSoftlandAccesosAsync(envio.CodAux, envio.Rut, envio.Nombre, envio.Vendedor, envio.CondicionVenta, envio.CategoriaCliente, envio.ListaPrecio, 100, null, logApi.Id).ConfigureAwait(false);

                        foreach (var item in envio.Eliminados)
                        {
                            clientes.RemoveAll(x => x.CodAux == item.CodAux && x.RutAux == item.RutAux);
                        }

                        if (clientes.Count > 0)
                        {
                            foreach (var item in clientes)
                            {

                                var contactos = await softlandService.GetAllContactosAsync(item.CodAux, logApi.Id).ConfigureAwait(false);
                                List<ContactoDTO> contactosFiltrados = new List<ContactoDTO>();
                                foreach (var c in contactos)
                                {
                                    var exist = cargos.Where(x => x == c.CodCargo).FirstOrDefault();

                                    if (exist != null)
                                    {
                                        contactosFiltrados.Add(c);
                                    }
                                }

                                if (contactosFiltrados.Count() == 0 && value[0].EnviarTodosContactos == true)
                                {
                                    contactosFiltrados = contactos;
                                    contactosFiltrados = contactosFiltrados.DistinctBy(x => x.Correo).ToList();
                                }

                                if (contactosFiltrados.Count > 0)
                                {
                                    contactosFiltrados = contactosFiltrados.DistinctBy(x => x.Correo).ToList();
                                    contactosFiltrados = contactosFiltrados.Where(x => !string.IsNullOrWhiteSpace(x.Correo)).ToList();

                                    if (contactosFiltrados.Count == 0)
                                    {
                                        clientesSinDatos.Add(item);
                                    }
                                    else
                                    {
                                        foreach (var c in contactosFiltrados)
                                        {


                                            var clave = RandomPassword.GenerateRandomPassword();

                                            var cliente = context.ClientesPortals.Where(x => x.Rut == item.RutAux && x.CodAux == item.CodAux && x.Correo == c.Correo).FirstOrDefault();
                                            if (cliente == null) //NUEVO ACCESO
                                            {
                                                var nuevoCliente = new ClientesPortal();
                                                nuevoCliente.Rut = item.RutAux;
                                                nuevoCliente.CodAux = item.CodAux;
                                                nuevoCliente.Nombre = item.NomAux;
                                                nuevoCliente.Correo = c.Correo.ToLower();
                                                nuevoCliente.Clave = clave;
                                                nuevoCliente.ActivaCuenta = 0;

                                                context.ClientesPortals.Add(nuevoCliente);
                                                context.SaveChanges();

                                            }
                                            else //ACTUALIZA ACCESO
                                            {
                                                cliente.Correo = c.Correo.ToLower();
                                                cliente.ActivaCuenta = 0;
                                                cliente.Clave = clave;
                                                context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                                context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                                context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                                context.SaveChanges();
                                            }

                                            var envioConError = emailService.EnviaAcceso(item, c.Correo, clave);
                                            if (envioConError)
                                            {
                                                errorEnvio = true;
                                            }
                                        }
                                    }

                                }
                                else if (contactosFiltrados.Count() == 0 && value[0].EnviarFicha == true)
                                {
                                    if (String.IsNullOrWhiteSpace(item.EMail))
                                    {
                                        clientesSinDatos.Add(item);
                                    }
                                    else
                                    {
                                        var clave = RandomPassword.GenerateRandomPassword();

                                        var cliente = context.ClientesPortals.Where(x => x.Rut == item.RutAux && x.CodAux == item.CodAux && x.Correo == item.EMail).FirstOrDefault();
                                        if (cliente == null) //NUEVO ACCESO
                                        {
                                            var nuevoCliente = new ClientesPortal();
                                            nuevoCliente.Rut = item.RutAux;
                                            nuevoCliente.CodAux = item.CodAux;
                                            nuevoCliente.Nombre = item.NomAux;
                                            nuevoCliente.Correo = item.EMail.ToLower();
                                            nuevoCliente.Clave = clave;
                                            nuevoCliente.ActivaCuenta = 0;
                                            context.ClientesPortals.Add(nuevoCliente);
                                            context.SaveChanges();

                                        }
                                        else //ACTUALIZA ACCESO
                                        {
                                            cliente.Correo = item.EMail.ToLower();
                                            cliente.ActivaCuenta = 0;
                                            cliente.Clave = clave;
                                            context.Entry(cliente).Property(x => x.Clave).IsModified = true;
                                            context.Entry(cliente).Property(x => x.ActivaCuenta).IsModified = true;
                                            context.Entry(cliente).Property(x => x.Correo).IsModified = true;
                                            context.SaveChanges();
                                        }

                                        var envioConError = emailService.EnviaAcceso(item, item.EMail, clave);
                                        if (envioConError)
                                        {
                                            errorEnvio = true;
                                            clientesSinDatos.Add(item);
                                        }
                                    }

                                }
                                else
                                {
                                    clientesSinDatos.Add(item);
                                }
                            }


                            emailService.EnviaNotificacionEjecucionAccesos(clientesSinDatos);
                        }

                    }
                }
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
            }
            catch (Exception e)
            {

                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "enviaAccesoAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
        }

        [HttpPost("CanActivateAccount")]
        public async Task<ActionResult> CanActivateAccount(ClientesPortalVm user)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/CanActivateAccount";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/ActivateAccount";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/ActivateAccount";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteByMailAndRut")]
        public async Task<ActionResult> GetClienteByMailAndRut(ClientesPortalVm cliente)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteByMailAndRut";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            ClienteDTO retorno = new ClienteDTO();

            try
            {
                retorno = await sf.GetClienteSoftlandAsync(string.Empty, cliente.Rut, logApi.Id);


                if (!string.IsNullOrEmpty(retorno.Rut))
                {
                    List<ContactoDTO> contactos = await sf.GetContactosClienteAsync(retorno.CodAux, logApi.Id);
                    retorno.Contactos = contactos;
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
                log.Ruta = "api/ClientesPortal/GetClienteByMailAndRut";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaClienteSoftland"), Authorize]
        public async Task<ActionResult> ActualizaClienteSoftland(ClienteDTO cliente)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/ActualizaClienteSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

            try
            {
                await sf.UpdateAuxiliarPortalPagoAsync(cliente, logApi.Id);

                MailViewModel mailVm = new MailViewModel();
                mailVm.email_destinatario = cliente.CorreoUsuario;
                mailVm.tipo = 7;
                MailService mail = new MailService(_context, _webHostEnvironment);
                await mail.EnviarCorreosAsync(mailVm);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(cliente);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/ChangePassword";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


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

                    MailService emailService = new MailService(_context, _webHostEnvironment);
                    bool error = emailService.EnviaCambioContrasena(cliente, claveEnvio);
                    if (error)
                    {
                        errorEnvio = true;
                    }
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetUltimasCompras";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

            try
            {
                var conf = _context.ConfiguracionPortals.FirstOrDefault();
                int top = (conf != null) ? (int)conf.CantidadUltimasCompras : 0;
                List<ComprasSoftlandDTO> compras = await sf.GetTopComprasAsync(codAux, top, logApi.Id);
                compras = compras.OrderByDescending(x => x.FechaEmision).ToList();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(compras);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDashboardDocumentosVencidos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string utilizaApiSoftland = "1";
                DashboardDocumentosVm retorno = new DashboardDocumentosVm();

                retorno = await sf.GetMontosDashboardAdmin(codAux, logApi.Id);

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
                log.Ruta = "api/ClientesPortal/GetDashboardDocumentosVencidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosVencidos"), Authorize]
        public async Task<ActionResult> GetDocumentosVencidos(FilterVm filter)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosVencidos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                var documentos = await sf.GetDocumentosVencidosAsync(filter, logApi.Id);

                var monedas = await sf.GetMonedasAsync(logApi.Id);
                List<ClienteSaldosDTO> documentosVencidos = documentos.ConvertAll(d =>
                             new ClienteSaldosDTO
                             {
                                 //item.comprobanteContable = reader["Comprobante"].ToString();
                                 Documento = d.DesDoc,
                                 Nro = (double)d.Numdoc,
                                 FechaEmision = Convert.ToDateTime(d.Movfe),
                                 FechaVcto = Convert.ToDateTime(d.Movfv),
                                 Debe = (double)d.MovMontoMa,
                                 Haber = d.Saldoadic,
                                 Saldo = (double)d.Saldoadic,
                                 Detalle = "", // reader["Detalle"].ToString();
                                 Estado = d.Estado,
                                 Pago = "", // reader["Pago"].ToString();
                                 TipoDoc = d.Ttdcod,
                                 RazonSocial = "",
                                 CodigoMoneda = d.MonCod,
                                 MontoOriginalBase = d.MontoOriginalBase,
                                 DesMon = monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault() != null ? monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault().DesMon : "",
                                 CodAux = d.CodAux,
                                 MontoBase = d.MovMonto,
                                 SaldoBase = d.Saldobase,
                                 EquivalenciaMoneda = d.Equivalencia,
                                 MovEqui = d.MovEquiv,
                             }
                         );

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(documentosVencidos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosVencidos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosPorVencer"), Authorize]
        public async Task<ActionResult> GetDocumentosPorVencer(FilterVm filter)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosPorVencer";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                var documentos = await sf.GetDocumentosPorVencerAsync(filter, logApi.Id);

                var monedas = await sf.GetMonedasAsync(logApi.Id);
                List<ClienteSaldosDTO> documentosPorVencer = documentos.ConvertAll(d =>
                             new ClienteSaldosDTO
                             {
                                 //item.comprobanteContable = reader["Comprobante"].ToString();
                                 Documento = d.DesDoc,
                                 Nro = (double)d.Numdoc,
                                 FechaEmision = Convert.ToDateTime(d.Movfe),
                                 FechaVcto = Convert.ToDateTime(d.Movfv),
                                 Debe = (double)d.MovMontoMa,
                                 Haber = d.Saldoadic,
                                 Saldo = (double)d.Saldoadic,
                                 Detalle = "", // reader["Detalle"].ToString();
                                 Estado = d.Estado,
                                 Pago = "", // reader["Pago"].ToString();
                                 TipoDoc = d.Ttdcod,
                                 RazonSocial = "",
                                 CodigoMoneda = d.MonCod,
                                 MontoOriginalBase = d.MontoOriginalBase,
                                 DesMon = monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault() != null ? monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault().DesMon : "",
                                 CodAux = d.CodAux,
                                 MontoBase = d.MovMonto,
                                 SaldoBase = d.Saldobase,
                                 EquivalenciaMoneda = d.Equivalencia,
                                 MovEqui = d.MovEquiv,
                             }
                         );

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(documentosPorVencer);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosPorVencer";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosPendientes"), Authorize]
        public async Task<ActionResult> GetDocumentosPendientes(FilterVm filter)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosPendientes";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                var documentos = await sf.GetDocumentosPendientesAsync(filter, logApi.Id);

                var monedas = await sf.GetMonedasAsync(logApi.Id);
                List<ClienteSaldosDTO> documentosPendientes = documentos.ConvertAll(d =>
                             new ClienteSaldosDTO
                             {
                                 //item.comprobanteContable = reader["Comprobante"].ToString();
                                 Documento = d.DesDoc,
                                 Nro = (double)d.Numdoc,
                                 FechaEmision = Convert.ToDateTime(d.Movfe),
                                 FechaVcto = Convert.ToDateTime(d.Movfv),
                                 Debe = (double)d.MovMontoMa,
                                 Haber = d.Saldoadic,
                                 Saldo = (double)d.Saldoadic,
                                 Detalle = "", // reader["Detalle"].ToString();
                                 Estado = d.Estado,
                                 Pago = "", // reader["Pago"].ToString();
                                 TipoDoc = d.Ttdcod,
                                 RazonSocial = "",
                                 CodigoMoneda = d.MonCod,
                                 MontoOriginalBase = d.MontoOriginalBase,
                                 DesMon = monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault() != null ? monedas.Where(x => x.CodMon == d.MonCod).FirstOrDefault().DesMon : "",
                                 CodAux = d.CodAux,
                                 MontoBase = d.MovMonto,
                                 SaldoBase = d.Saldobase,
                                 EquivalenciaMoneda = d.Equivalencia,
                                 MovEqui = d.MovEquiv,
                             }
                         );

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(documentosPendientes);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetAllDocumentosContabilizados";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

            try
            {
                List<ClienteSaldosDTO> documentosContabilizados = await sf.GetAllDocumentosContabilizadosAsync(codAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documentosContabilizados);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDashboardCompras";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<DashboardComprasVm> retorno = new List<DashboardComprasVm>();
                List<DocumentosFacturadosDTO> compras = await sf.GetClientesComprasSoftlandAsync(codAux, logApi.Id);
                List<NotaVentaClienteDTO> nv = await sf.GetNotasVentasPendientesAsync(codAux, logApi.Id);
                List<ProductoDTO> productos = await sf.GetProductosCompradosAsync(codAux, logApi.Id);
                List<GuiaDespachoDTO> guias = await sf.GetGuiasPendientes(codAux, logApi.Id);

                if (compras.Count > 0)
                {
                    DashboardComprasVm d = new DashboardComprasVm();
                    foreach (var item in compras)
                    {
                        d.CantidadDocumentos += 1;
                        d.MontoTotal = d.MontoTotal + Convert.ToDecimal(item.Monto);
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
                        d.MontoTotal = d.MontoTotal + Convert.ToDecimal(item.MontoPendiente);
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
                        d.MontoTotal = d.MontoTotal + Convert.ToDecimal(item.TotalLinea);
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
                        d.MontoTotal = d.MontoTotal + Convert.ToDecimal(item.Total);
                    }

                    d.Tipo = "GUIAS";
                    retorno.Add(d);
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
                log.Ruta = "api/ClientesPortal/GetDashboardCompras";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteComprasFromSoftland"), Authorize]
        public async Task<ActionResult> GetClienteComprasFromSoftland(ClientesPortalVm model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteComprasFromSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<DocumentosFacturadosDTO> clients = await sf.GetClientesComprasSoftlandAsync(model.CodAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetEstadoBloqueoCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                Boolean estado = await sf.GetEstadoBloqueoClienteAsync(codAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
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
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteDocumento";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            DocumentosVm documento = new DocumentosVm();
            try
            {
                string utilizaApiSoftland = "true";
                if (utilizaApiSoftland == "false")
                {
                    Generador reporte = new Generador(_context, _webHostEnvironment);
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

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);
                    return Ok(documento);
                }
                else
                {
                    string base64 = await sf.obtenerPDFDocumento((int)model.Folio, model.TipoDoc, logApi.Id);
                    documento.NombreArchivo = (model.TipoDoc.Substring(0, 1) == "B") ? "Boleta " + model.Folio + ".pdf" : (model.TipoDoc.Substring(0, 1) == "F") ? "Factura " + model.Folio + ".pdf" : "";
                    documento.Tipo = "PDF";
                    documento.Base64 = base64;

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);
                    return Ok(documento);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetAutomatizaciones";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            DocumentosVm documento = new DocumentosVm();
            try
            {
                DocumentosVm dtResultado = await sf.obtenerXMLDTEAsync(Convert.ToInt32(model.Folio), model.CodAux, model.TipoDoc, logApi.Id);

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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDetalleCompra";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                DocumentoDTO documento = new DocumentoDTO();

                string utilizaApiSoftland = "true";

                if (utilizaApiSoftland == "false")
                {
                    CabeceraDocumentoDTO dtResultado = sf.obtenerCabecera(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux, logApi.Id);
                    List<DetalleDocumentoDTO> dtDetalle = sf.obtenerDetalle(Convert.ToInt32(model.Folio), dtResultado.Tipo, model.CodAux, logApi.Id);


                    documento.cabecera = dtResultado;
                    documento.detalle = dtDetalle;
                }
                else
                {
                    documento = await sf.obtenerDocumentoAPI(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux, logApi.Id);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDetalleDespacho";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string utilizaApiSoftland = "true";
                List<DespachosDTO> listaRetorno = new List<DespachosDTO>();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    listaRetorno = sf.GetDespachosDocumento(Convert.ToInt32(model.Folio), logApi.Id);
                    if (listaRetorno.Count > 0)
                    {
                        foreach (var item in listaRetorno)
                        {
                            if (item.Documento == "Guia Despacho")
                            {
                                item.Detalle = sf.GetDetalleDespacho(item.NroInt, logApi.Id);
                            }

                        }
                    }
                }
                else
                {
                    listaRetorno = await sf.GetDepachoDocumentoAPIAsync(Convert.ToInt32(model.Folio), model.TipoDoc, model.CodAux, logApi.Id);
                }


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listaRetorno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/EnvioDocumentos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


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
                MailService mail = new MailService(_context, _webHostEnvironment);
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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(model);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetContactosClienteSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<ContactoDTO> contactos = await sf.GetContactosClienteAsync(codAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(contactos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetNotaVentaCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<NotaVentaClienteDTO> nv = await sf.GetNotasVentasPendientesAsync(codAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(nv);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDetalleCompraNv";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                NotaVentaClienteDTO detalle = new NotaVentaClienteDTO();
                detalle = await sf.GetNotaVentaAsync(Convert.ToInt32(model.Folio), model.CodAux, logApi.Id);
                detalle.detalle = await sf.GetNotaVentaDetalleAsync(Convert.ToInt32(model.Folio), logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetProductosComprados";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<ProductoDTO> productos = await sf.GetProductosCompradosAsync(model.CodAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetProductosComprados";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClienteEstadoComprasFromSoftland")]
        public async Task<ActionResult> GetClienteEstadoComprasFromSoftland(FilterVm model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteEstadoComprasFromSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                if (string.IsNullOrEmpty(model.CodAux) && !string.IsNullOrEmpty(model.Rut))
                {
                    var datosCliente = await sf.GetClienteSoftlandAsync(string.Empty, model.Rut, logApi.Id);
                    model.CodAux = datosCliente.CodAux;
                }

                List<ClienteSaldosDTO> clients = await sf.GetClienteEstadoCuentaAsync(model.CodAux, logApi.Id);

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
                    var cobranza = _context.CobranzaCabeceras.Where(x => x.IdCobranza == model.IdCobranza).FirstOrDefault();
                    if (cobranza.FechaFin != null)
                    {
                        if (cobranza.FechaFin.Value.Date >= DateTime.Now.Date)
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

                            List<ClienteSaldosDTO> clientsAux = new List<ClienteSaldosDTO>();
                            foreach (var folio in foliosDocumentos.Split(';'))
                            {
                                var exist = clients.Where(x => x.Nro.ToString() == folio).ToList();
                                if (exist.Count > 0)
                                {
                                    foreach (var doc in exist)
                                    {
                                        clientsAux.Add(doc);
                                    }
                                }
                            }
                            clients = clientsAux;

                            foreach (var item in foliosDocumentos.Split(';'))
                            {
                                var detalle = documentosCobranzas.Where(x => x.Folio.ToString() == item).FirstOrDefault();
                                if (detalle != null)
                                {
                                    var detalleSoftland = clients.Where(x => x.Nro == detalle.Folio).FirstOrDefault();
                                    if (detalleSoftland != null)
                                    {
                                        detalle.Monto = (float?)detalleSoftland.MontoBase;
                                        detalle.Pagado = (float?)(detalleSoftland.MontoBase - detalleSoftland.APagar);
                                        _context.Entry(detalle).State = EntityState.Modified;
                                        _context.SaveChanges();
                                    }
                                }

                            }
                        }
                        else
                        {
                            clients = new List<ClienteSaldosDTO>();
                            clients.Add(new ClienteSaldosDTO { Documento = "Cobranza Vencida" });
                            return Ok(clients);
                        }
                    }
                    else
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

                        List<ClienteSaldosDTO> clientsAux = new List<ClienteSaldosDTO>();
                        foreach (var folio in foliosDocumentos.Split(';'))
                        {
                            var exist = clients.Where(x => x.Nro.ToString() == folio).ToList();
                            if (exist.Count > 0)
                            {
                                foreach (var doc in exist)
                                {
                                    clientsAux.Add(doc);
                                }
                            }
                        }
                        clients = clientsAux;

                        foreach (var item in foliosDocumentos.Split(';'))
                        {
                            var detalle = documentosCobranzas.Where(x => x.Folio.ToString() == item).FirstOrDefault();
                            if (detalle != null)
                            {
                                var detalleSoftland = clients.Where(x => x.Nro == detalle.Folio).FirstOrDefault();
                                if (detalleSoftland != null)
                                {
                                    detalle.Monto = (float?)detalleSoftland.MontoBase;
                                    detalle.Pagado = (float?)(detalleSoftland.MontoBase - detalleSoftland.APagar);
                                    _context.Entry(detalle).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                            }

                        }
                    }

                }


                if (!string.IsNullOrEmpty(model.AutomatizacionJson))
                {

                    string validaBase64 = Encrypt.EnsureBase64Length(model.AutomatizacionJson);
                    string jsonDecodificado = Encrypt.Base64Decode(validaBase64);


                    AutomatizacionVm automatizacion = JsonConvert.DeserializeObject<AutomatizacionVm>(jsonDecodificado);

                    Nullable<DateTime> fechaHasta = null;
                    Nullable<DateTime> fechaDesde = null;
                    if (automatizacion.Anio == null || automatizacion.Anio == 0)
                    {
                        automatizacion.Anio = 0;
                    }
                    else
                    {
                        fechaHasta = new DateTime((int)automatizacion.Anio, 12, 31, 0, 0, 0);
                        fechaDesde = new DateTime((int)automatizacion.Anio, 01, 01, 0, 0, 0);
                    }

                    if (!string.IsNullOrEmpty(automatizacion.TipoDocumentos))
                    {
                        clients = clients.Where(x => automatizacion.TipoDocumentos.Contains(x.TipoDoc)).ToList();
                    }


                    if (automatizacion.Anio != 0)
                    {
                        clients = clients.Where(x => x.FechaEmision.Value.Year == automatizacion.Anio).ToList();
                    }

                    if (fechaDesde != null)
                    {
                        clients = clients.Where(x => x.FechaEmision.Value.Date >= fechaDesde).ToList();
                    }

                    if (fechaHasta != null)
                    {
                        clients = clients.Where(x => x.FechaEmision.Value.Date <= fechaHasta).ToList();
                    }




                    switch (automatizacion.IdAutomatizacion)
                    {
                        case 1:
                            if (automatizacion.NumDoc.Count > 0)
                            {
                                List<ClienteSaldosDTO> clientsAux = new List<ClienteSaldosDTO>();

                                foreach (var folio in automatizacion.NumDoc)
                                {
                                    var exist = clients.Where(x => x.Nro == folio).ToList();
                                    if (exist.Count > 0)
                                    {
                                        foreach (var doc in exist)
                                        {
                                            clientsAux.Add(doc);
                                        }
                                    }
                                }
                                clients = clientsAux;
                            }
                            break;
                        case 3:
                            clients = clients.Where(x => (DateTime.Now.Date - x.FechaVcto.Value.Date).TotalDays >= automatizacion.DiasVencimiento).ToList();
                            break;
                    }
                }


                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteEstadoComprasFromSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEstadoConexionSoftland")]
        public async Task<ActionResult> GetEstadoConexionSoftland()
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetEstadoConexionSoftland";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                Boolean estado = await sf.verificaEstadoConexionSoftlandAsync(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(estado);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetEstadoConexionSoftland";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SavePago")]
        public async Task<ActionResult> SavePago(PagoCabeceraVm value)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/SavePago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(pagoEstado.IdPago);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDashboardAdministrador";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetExistCompras";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDetallePago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string tipoDocumento = string.Empty;

                List<CondicionVentaDTO> condVenta = await sf.GetCondVentaAsync(logApi.Id);

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

                        item.TipoDocumento = sf.GetDescDocumento(item.TipoDocumento, logApi.Id);
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

                        List<ClienteDTO> listaClientes = await sf.BuscarClienteSoftland2Async(cabeceraPago.IdClienteNavigation.CodAux, "", "", logApi.Id);
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
                    cabecera.PasarelaPagoLog = pasarela;
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(cabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDatosPagoRapido";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string tipoDocumento = string.Empty;

                List<CondicionVentaDTO> condVenta = await sf.GetCondVentaAsync(logApi.Id);

                PagoCabeceraVm cabecera = new PagoCabeceraVm();

                var cabeceraPago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (cabeceraPago != null)
                {
                    cabecera.CodAux = cabeceraPago.CodAux;
                    cabecera.Rut = cabeceraPago.Rut;
                    cabecera.Correo = cabeceraPago.Correo;
                    cabecera.Nombre = cabecera.Nombre;

                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(cabecera);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentoPDF";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string tipoDoc = cabecera.Tipo + cabecera.SubTipo;
                string pdfBase64 = await sf.obtenerPDFDocumento(cabecera.Folio, tipoDoc, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(pdfBase64);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/EnviaDocumentoPDF";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                MailService mc = new MailService(_context, _webHostEnvironment);
                MailViewModel mv = new MailViewModel();
                mv.asunto = "Documento " + envio.folio.ToString();
                List<Attachment> adjuntos = new List<Attachment>();
                if (envio.TipoDocAEnviar == 1)
                {

                    if (envio.docsAEnviar == 1 || envio.docsAEnviar == 3)
                    {
                        string tipoDoc = envio.tipoDoc + envio.subTipoDoc;
                        string pdfBase64 = await sf.obtenerPDFDocumento(envio.folio, tipoDoc, logApi.Id);
                        if (!string.IsNullOrEmpty(pdfBase64))
                        {
                            byte[] stemp = Convert.FromBase64String(pdfBase64);
                            Stream stream = new MemoryStream(stemp);
                            adjuntos.Add(new Attachment(stream, Path.GetFileName(mv.asunto + ".pdf"), "application/pdf"));
                        }
                    }

                    if (envio.docsAEnviar == 2 || envio.docsAEnviar == 3)
                    {
                        DocumentosVm xml = await sf.obtenerXMLDTEAsync(envio.folio, envio.codAux, envio.tipoDoc, logApi.Id);
                        byte[] myByte = System.Text.Encoding.UTF8.GetBytes(xml.Base64);
                        xml.Base64 = Convert.ToBase64String(myByte);
                        if (!string.IsNullOrEmpty(xml.Base64))
                        {
                            byte[] stemp = Convert.FromBase64String(xml.Base64);
                            Stream stream = new MemoryStream(stemp);
                            adjuntos.Add(new Attachment(stream, Path.GetFileName(xml.NombreArchivo), "text/xml"));
                        }

                    }
                }
                if (envio.TipoDocAEnviar == 2)
                {
                    string pdfBase64 = await sf.obtenerPDFDocumentoNv(envio.folio.ToString(), envio.codAux, logApi.Id);
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

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);
                    return Ok(1);
                }
                else
                {
                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);
                    return Ok(0);
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetResumenContable";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                ResumenContableDTO resumen = await sf.obtenerResumenContable(codAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentoPDFNv";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                string pdfBase64 = await sf.obtenerPDFDocumentoNv(notaVenta.NVNumero, notaVenta.CodAux, logApi.Id);
                var documento = new
                {
                    PdfBase64 = pdfBase64
                };

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClientesFiltro";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                List<ClientesPortal> clients = await softlandService.GetClientesSoftlandFiltrosAsync(model, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosDashboarddAdmin";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                DashboardDocumentosVm documentos = await sf.GetMontosDashboardAdmin(string.Empty, logApi.Id);

                //ULTIMOS PAGOS

                int mes = DateTime.Today.Month;
                int anio = DateTime.Today.Year;

                var primerDiaMes = new DateTime(anio, mes, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddMilliseconds(-1);

                var pagosTotales = _context.PagosCabeceras.OrderByDescending(x => x.IdPago).Where(x => (x.IdPagoEstado == 2 || x.IdPagoEstado == 4) && x.FechaPago >= primerDiaMes && x.FechaPago <= ultimoDiaMes).ToList();

                if (pagosTotales.Count > 0)
                {
                    documentos.CantidadDocumentosPagados = pagosTotales.Count();
                    documentos.MontoPagado = (decimal)pagosTotales.Sum(x => x.MontoPago);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documentos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosAdmin";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                if (codAux == "0")
                {
                    codAux = "";
                }

                List<ClienteDTO> clientes = new List<ClienteDTO>();
                List<ClienteSaldosDTO> documentos = await sf.GetDocumentosDashboardAdminAsync(codAux, estado, logApi.Id);
                var documentosAgrupados = documentos.GroupBy(x => x.CodAux).ToList();

                foreach (var item in documentosAgrupados)
                {
                    var cliente = await sf.GetClienteSoftlandAsync(item.Key, string.Empty, logApi.Id);
                    cliente.Documentos = documentos.Where(x => x.CodAux == item.Key).OrderByDescending(x => x.FechaEmision).ToList();
                    cliente.TotalDocumentos = cliente.Documentos.Sum(x => x.MontoBase);
                    cliente.TotalSaldo = cliente.Documentos.Sum(x => x.SaldoBase);
                    cliente.CantidadDocumentos = cliente.Documentos.Count();
                    clientes.Add(cliente);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetCorreosDTE";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string rutFormateado = rut.Replace(".", "");
                var correos = _admin.CsvEmpresasSiis.Where(x => x.Rut == rutFormateado).ToList();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(correos);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteGuiasDespacho";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<GuiaDespachoDTO> guias = await sf.GetGuiasPendientes(model.CodAux, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(guias);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetClienteGuiasDespacho";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetPDFPago/{idPago}")]
        public async Task<ActionResult> GetPDFPago(int idPago)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetPDFPago";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                string base64 = string.Empty;
                var pagoCabecera = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                string logoSoftlandFooter = configEmpresa.UrlPortal + "/assets/images/Softlandpiemail.png";


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
                        .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", idPago.ToString()).Replace("{MONTOTOTAL}", pagoCabecera.MontoPago.Value.ToString("N0")).Replace("{IMAGENFOOTER}", logoSoftlandFooter);

                    string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApi.Id);

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
                DocumentosVm documento = new DocumentosVm();
                documento.NombreArchivo = idPago.ToString() + ".pdf";
                documento.Base64 = base64;
                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documento);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetPDFPago";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RecuperarContrasena")]
        public async Task<ActionResult> RecuperarContrasena(ClienteDTO clienteVm)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/RecuperarContrasena";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                MailService emailService = new MailService(_context, _webHostEnvironment);
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
                        emailService.EnviaRecuperacionContrasenaUsuario(usuario, clave);

                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok(1);
                    }
                    else
                    {
                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
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


                        emailService.EnviaRecuperacionContrasenaCliente(cliente, clave);

                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok(1);
                    }
                    else
                    {
                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/RecuperarContrasena";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosPagados"), Authorize]
        public async Task<ActionResult> GetDocumentosPagados(FilterVm filtro)
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosPagados";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<PagoCabeceraVm> retorno = new List<PagoCabeceraVm>();

                var documentos = _context.PagosCabeceras.Where(x => x.IdPagoEstado == 2 || x.IdPagoEstado == 4).OrderByDescending(x => x.IdPago).AsQueryable();

                if (!string.IsNullOrEmpty(filtro.CodAux))
                {
                    documentos = documentos.Where(x => x.CodAux == filtro.CodAux).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filtro.Rut))
                {
                    documentos = documentos.Where(x => x.Rut == filtro.Rut).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filtro.NumComprobante))
                {
                    documentos = documentos.Where(x => x.ComprobanteContable == filtro.NumComprobante).AsQueryable();
                }


                if (filtro.TipoBusqueda != null && filtro.TipoBusqueda != 0)
                {
                    documentos = documentos.Where(x => x.IdPagoEstado == filtro.TipoBusqueda).AsQueryable();
                }

                if (filtro.fechaDesde != null)
                {
                    documentos = documentos.Where(x => x.FechaPago.Value.Date >= filtro.fechaDesde.Value.Date).AsQueryable();
                }

                if (filtro.fechaHasta != null)
                {
                    documentos = documentos.Where(x => x.FechaPago.Value.Date <= filtro.fechaHasta.Value.Date).AsQueryable();
                }

                var pagos = documentos.Skip((filtro.Pagina - 1) * 10).Take(10).ToList();

                foreach (var item in pagos)
                {
                    PagoCabeceraVm pago = new PagoCabeceraVm();
                    pago.CodAux = item.CodAux;
                    pago.ComprobanteContable = item.ComprobanteContable;
                    pago.Correo = item.Correo;
                    pago.EsPagoRapido = (int)item.EsPagoRapido;
                    pago.FechaPago = (DateTime)item.FechaPago;
                    pago.HoraPago = item.HoraPago;
                    pago.IdCliente = item.IdCliente;
                    pago.IdPago = item.IdPago;
                    pago.IdPagoEstado = (int)item.IdPagoEstado;
                    pago.IdPasarela = (int)item.IdPasarela;
                    pago.MontoPago = (Single)item.MontoPago;
                    pago.Nombre = item.Nombre;
                    pago.Rut = item.Rut;

                    List<PagoDetalleVm> detalles = new List<PagoDetalleVm>();
                    item.PagosDetalles = _context.PagosDetalles.Where(x => x.IdPago == item.IdPago).ToList();
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
                    item.PasarelaPagoLogs = _context.PasarelaPagoLogs.Where(x => x.IdPago == item.IdPago).ToList();
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

                if (retorno.Count > 0)
                {
                    retorno[0].TotalFilas = documentos.Count();
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
                log.Ruta = "api/ClientesPortal/GetDocumentosPagados";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetDocumentosPagadosExport"), Authorize]
        public async Task<ActionResult> GetDocumentosPagadosExport(FilterVm filtro)
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosPagadosExport";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<PagoCabeceraVm> retorno = new List<PagoCabeceraVm>();

                var documentos = _context.PagosCabeceras.Where(x => x.IdPagoEstado == 2 || x.IdPagoEstado == 4).OrderByDescending(x => x.IdPago).AsQueryable();

                if (!string.IsNullOrEmpty(filtro.CodAux))
                {
                    documentos = documentos.Where(x => x.CodAux == filtro.CodAux).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filtro.Rut))
                {
                    documentos = documentos.Where(x => x.Rut == filtro.Rut).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filtro.NumComprobante))
                {
                    documentos = documentos.Where(x => x.ComprobanteContable == filtro.NumComprobante).AsQueryable();
                }


                if (filtro.TipoBusqueda != null && filtro.TipoBusqueda != 0)
                {
                    documentos = documentos.Where(x => x.IdPagoEstado == filtro.TipoBusqueda).AsQueryable();
                }

                if (filtro.fechaDesde != null)
                {
                    documentos = documentos.Where(x => x.FechaPago.Value.Date >= filtro.fechaDesde.Value.Date).AsQueryable();
                }

                if (filtro.fechaHasta != null)
                {
                    documentos = documentos.Where(x => x.FechaPago.Value.Date <= filtro.fechaHasta.Value.Date).AsQueryable();
                }

                var pagos = documentos.ToList();

                foreach (var item in pagos)
                {
                    PagoCabeceraVm pago = new PagoCabeceraVm();
                    pago.CodAux = item.CodAux;
                    pago.ComprobanteContable = item.ComprobanteContable;
                    pago.Correo = item.Correo;
                    pago.EsPagoRapido = (int)item.EsPagoRapido;
                    pago.FechaPago = (DateTime)item.FechaPago;
                    pago.HoraPago = item.HoraPago;
                    pago.IdCliente = item.IdCliente;
                    pago.IdPago = item.IdPago;
                    pago.IdPagoEstado = (int)item.IdPagoEstado;
                    pago.IdPasarela = (int)item.IdPasarela;
                    pago.MontoPago = (Single)item.MontoPago;
                    pago.Nombre = item.Nombre;
                    pago.Rut = item.Rut;

                    List<PagoDetalleVm> detalles = new List<PagoDetalleVm>();
                    item.PagosDetalles = _context.PagosDetalles.Where(x => x.IdPago == item.IdPago).ToList();
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
                    item.PasarelaPagoLogs = _context.PasarelaPagoLogs.Where(x => x.IdPago == item.IdPago).ToList();
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

                if (retorno.Count > 0)
                {
                    retorno[0].TotalFilas = documentos.Count();
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
                log.Ruta = "api/ClientesPortal/GetDocumentosPagadosExport";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetResumenDocumentosCliente"), Authorize]
        public async Task<ActionResult> GetResumenDocumentosXCliente(FilterVm filter)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetResumenDocumentosCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                var documentos = await sf.GetResumenDocumentosXClienteAsync(filter, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documentos);


            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetResumenDocumentosXCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetDocumentosContabilizadosXCliente"), Authorize]
        public async Task<ActionResult> GetDocumentosContabilizadosXCliente(FilterVm filter)
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDocumentosContabilizadosXCliente";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {

                var documentos = await sf.GetDocumentosClienteAdministrador(filter, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(documentos);


            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDocumentosContabilizadosXCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTopDeudores"), Authorize]
        public async Task<ActionResult> GetTopDeudores()
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetTopDeudores";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<DeudorApiDTO> retornoDeudores = await sf.GetTopDeudores(logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(retornoDeudores);

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetDeudaVsPagos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<DocumentoContabilizadoAPIDTO> documentos = await sf.GetDocumentosDeudaVsPago(logApi.Id);
                var mesAñoAnterior = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, 1);
                DateTime primerDiaMesActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime ultimoDiaMesActual = primerDiaMesActual.AddMonths(1).AddMilliseconds(-1);
                var documentosDeudasPago = documentos.OrderBy(x => Convert.ToDateTime(x.Movfv)).Where(x => Convert.ToDateTime(x.Movfv).Date >= mesAñoAnterior.Date && Convert.ToDateTime(x.Movfv).Date <= ultimoDiaMesActual.Date).ToList();


                List<DeudaPagosVm> listaDeuda = documentosDeudasPago.GroupBy(x => new { Convert.ToDateTime(x.Movfv).Month, Convert.ToDateTime(x.Movfv).Year }).Select(g => new DeudaPagosVm
                {
                    Mes = g.Key.Month,
                    Anio = g.Key.Year,
                    FechaTexto = g.Key.Month.ToString("D2") + "/" + g.Key.Year.ToString().Substring(2),
                    TotalDeuda = (double)g.Sum(j => j.Saldobase),
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

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(listaDeuda);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/GetDeudaVsPagos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetClientesByCodAux")]
        public async Task<ActionResult> GetClientesByCodAux(List<ClientesPortalVm> cliente)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClientesByCodAux";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<ClienteDTO> retorno = new List<ClienteDTO>();
                foreach (var item in cliente)
                {
                    var c = await sf.GetClienteSoftlandAsync(item.CodAux, string.Empty, logApi.Id);
                    retorno.Add(c);
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
                log.Ruta = "api/ClientesPortal/GetClientesByCodAux";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("EnviaCorreoComprobante/{idPago}"), Authorize]
        public async Task<ActionResult> EnviaCorreoComprobante(int idPago)
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/EnviaCorreoComprobante";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                List<ClienteDTO> retorno = new List<ClienteDTO>();

                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (pago != null)
                {
                    var config = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                    var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                    string fecha = pago.FechaPago.Value.ToString("dd/MM/yyyy");
                    string hora = pago.HoraPago;
                    string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                    string comprobanteHtml = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/invoice.html")))
                    {
                        comprobanteHtml = reader.ReadToEnd();
                    }

                    comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                        .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", pago.IdPago.ToString()).Replace("{MONTOTOTAL}", pago.MontoPago.Value.ToString("N0").Replace(",", "."));

                    string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                    string reemplazoDetalle = string.Empty;

                    SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApi.Id);
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
                    MailService mail = new MailService(_context, _webHostEnvironment);
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
                    vm2.mensaje = pago.ComprobanteContable + "|" + pago.CodAux + "|" + pago.Correo + "|" + pago.MontoPago.Value.ToString("N0").Replace(",", ".") + "|" + cliente.Nombre + "|" + cliente.Rut + "|" + pago.IdPago.ToString();
                    vm2.adjuntos = listaAdjuntos2;
                    vm2.email_destinatario = configCorreo.CorreoAvisoPago;
                    if (!string.IsNullOrEmpty(configCorreo.CorreoAvisoPago))
                    {
                        await mail.EnviarCorreosAsync(vm2);
                    }


                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);
                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/enviaCorreoComprobante";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("GetClienteByCodAux")]
        public async Task<ActionResult> GetClienteByCodAux(ClientesPortalVm cliente)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/GetClienteByCodAux";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            ClienteDTO retorno = new ClienteDTO();

            try
            {
                var clientePortal = _context.ClientesPortals.Where(x => x.CodAux == cliente.CodAux && x.Correo == cliente.Correo).FirstOrDefault();
                retorno = await sf.GetClienteSoftlandAsync(cliente.CodAux, "", logApi.Id); //FCA 26-06-2022
                if (clientePortal != null)
                {
                    retorno.IdCliente = clientePortal.IdCliente;
                }


                List<ContactoDTO> contactos = await sf.GetContactosClienteAsync(cliente.CodAux, logApi.Id); //FCA 26-06-2022
                retorno.Contactos = contactos;

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
                log.Ruta = "api/ClientesPortal/GetClienteByCodAux";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("getPDFEstadoCuenta")]
        public async Task<ActionResult> getPDFEstadoCuenta(EstadoCuentaVm model)
        {

            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/getPDFEstadoCuenta";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                Generador genera = new Generador(_context, _webHostEnvironment);
                SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);

                PdfEstadoCuentaVm pdf = new PdfEstadoCuentaVm();
                if (model.IdCobranza != 0)
                {
                    var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApi.Id);
                    var listaDocPendientes = _context.CobranzaDetalles.Where(x => (x.IdEstado == 3 || x.IdEstado == 1 || x.IdEstado == 4) && x.IdCobranza == model.IdCobranza && x.CodAuxCliente == model.CodAux).ToList();
                    var documentos = await softlandService.GetAllDocumentosContabilizadosCliente(model.CodAux, logApi.Id);

                    foreach (var item in listaDocPendientes)
                    {
                        var doc = documentos.Where(x => x.Numdoc == item.Folio).FirstOrDefault();

                        if (doc != null)
                        {
                            item.Monto = (float?)doc.MovMonto;
                            if (doc.Saldobase < doc.MovMonto)
                            {
                                item.IdEstado = 4;
                                if (doc.Saldobase <= 0)
                                {
                                    item.Pagado = (float?)doc.MovMonto;
                                    item.IdEstado = 5;
                                }
                                else
                                {
                                    item.Pagado = (float?)(doc.MovMonto - doc.Saldobase);
                                }

                            }
                            _context.Entry(item).State = EntityState.Modified;
                        }
                    }

                    _context.SaveChanges();

                    listaDocPendientes = listaDocPendientes.Where(x => x.IdEstado != 5).ToList();
                    DetalleEnvioCobranzaVm listaEnvio = new DetalleEnvioCobranzaVm();

                    if (listaDocPendientes.Count > 0)
                    {
                        DetalleEnvioCobranzaVm DetalleCobranza = new DetalleEnvioCobranzaVm();
                        DetalleCobranza.RutCliente = listaDocPendientes[0].RutCliente;
                        DetalleCobranza.NombreCliente = (string.IsNullOrEmpty(listaDocPendientes[0].NombreCliente)) ? string.Empty : listaDocPendientes[0].NombreCliente;
                        DetalleCobranza.EmailCliente = listaDocPendientes[0].EmailCliente;
                        DetalleCobranza.CantidadDocumentosPendientes = listaDocPendientes.Count;
                        DetalleCobranza.MontoDeuda = Convert.ToInt32(listaDocPendientes.Sum(x => x.Monto));
                        DetalleCobranza.ListaDocumentos = new List<DocumentosCobranzaVM>();

                        //Agregamos documentos
                        foreach (var d in listaDocPendientes)
                        {
                            DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                            aux.Folio = (int)d.Folio;
                            aux.FechaEmision = (DateTime)d.FechaEmision;
                            aux.FechaVencimiento = (DateTime)d.FechaVencimiento;
                            aux.Monto = (int)d.Monto;
                            aux.TipoDocumento = tiposDocumentos.Where(x => x.CodDoc == d.TipoDocumento).FirstOrDefault().DesDoc;
                            DetalleCobranza.ListaDocumentos.Add(aux);
                        }

                        //Agrega detalle para envió
                        string nombreDocumento = string.Empty;
                        var cobranza = _context.CobranzaCabeceras.FirstOrDefault();
                        if (cobranza != null)
                        {
                            nombreDocumento = cobranza.IdTipoCobranza == 1 ? "Estado de Cuenta" : "Documentos Próximos a Vencer";
                        }
                        var docStream = genera.generaDetalleCobranza(DetalleCobranza, nombreDocumento);
                        string base64 = Convert.ToBase64String(docStream);

                        pdf.Nombre = nombreDocumento + ".pdf";
                        pdf.Base64 = base64;

                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok(pdf);
                    }
                    else
                    {
                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok("0");
                    }

                }
                else
                {
                    string validaBase64 = Encrypt.EnsureBase64Length(model.Automatizacion);
                    string jsonDecodificado = Encrypt.Base64Decode(validaBase64);


                    AutomatizacionVm automatizacion = JsonConvert.DeserializeObject<AutomatizacionVm>(jsonDecodificado);

                    Nullable<DateTime> fechaHasta = null;
                    Nullable<DateTime> fechaDesde = null;
                    if (automatizacion.Anio == null || automatizacion.Anio == 0)
                    {
                        automatizacion.Anio = 0;
                    }
                    else
                    {
                        fechaHasta = new DateTime((int)automatizacion.Anio, 12, 31, 0, 0, 0);
                        fechaDesde = new DateTime((int)automatizacion.Anio, 01, 01, 0, 0, 0);
                    }

                    var documentos = await softlandService.ObtenerDocumentosAutomaizacion(automatizacion.TipoDocumentos, model.CodAux, 0, logApi.Id);

                    switch (automatizacion.IdTipoAutomatizacion)
                    {
                        case 1:
                            if (automatizacion.NumDoc.Count > 0)
                            {
                                List<DocumentosCobranzaVm> clientsAux = new List<DocumentosCobranzaVm>();

                                foreach (var folio in automatizacion.NumDoc)
                                {
                                    var exist = documentos.Where(x => x.FolioDocumento == folio).ToList();
                                    if (exist.Count() > 0)
                                    {
                                        foreach (var doc in exist)
                                        {
                                            clientsAux.Add(doc);
                                        }
                                    }
                                }
                                documentos = clientsAux;
                            }
                            break;
                        case 3:
                            documentos = documentos.Where(x => (DateTime.Now.Date - x.FechaVencimiento.Date).TotalDays >= automatizacion.DiasVencimiento).ToList();
                            break;
                    }

                    if (documentos.Count() > 0)
                    {
                        DetalleEnvioCobranzaVm doc = new DetalleEnvioCobranzaVm();
                        doc.RutCliente = documentos[0].RutCliente;
                        doc.NombreCliente = (string.IsNullOrEmpty(documentos[0].NombreCliente)) ? string.Empty : documentos[0].NombreCliente;
                        doc.EmailCliente = documentos[0].EmailCliente;
                        doc.CantidadDocumentosPendientes = documentos.Count();
                        doc.MontoDeuda = Convert.ToDecimal(documentos[0].MontoDocumento);
                        doc.ListaDocumentos = new List<DocumentosCobranzaVM>();

                        foreach (var item in documentos)
                        {
                            DocumentosCobranzaVM aux = new DocumentosCobranzaVM();
                            aux.Folio = (int)item.FolioDocumento;
                            aux.FechaEmision = (DateTime)item.FechaEmision;
                            aux.FechaVencimiento = (DateTime)item.FechaVencimiento;
                            aux.Monto = (decimal)item.MontoDocumento;
                            aux.TipoDocumento = item.TipoDocumento;
                            doc.ListaDocumentos.Add(aux);
                        }

                        string nombreDocumento = string.Empty;


                        nombreDocumento = automatizacion.IdTipoAutomatizacion == 1 || automatizacion.IdTipoAutomatizacion == 2 ? "Documentos Próximos a Vencer" : "Estado de Cuenta";

                        var docStream = genera.generaDetalleCobranza(doc, nombreDocumento);
                        string base64 = Convert.ToBase64String(docStream);

                        pdf.Nombre = nombreDocumento + ".pdf";
                        pdf.Base64 = base64;

                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok(pdf);
                    }
                    else
                    {
                        logApi.Termino = DateTime.Now;
                        logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                        sf.guardarLogApi(logApi);
                        return Ok("0");
                    }



                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/getPDFEstadoCuenta";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("getPagosDocumento")]
        public async Task<ActionResult> getPagosDocumento(FilterVm model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ClientesPortal/getPagosDocumento";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();


            try
            {
                SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);

                List<ClienteSaldosDTO> clients = await softlandService.GetPagosDocumento(model, logApi.Id);

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ClientesPortal/getPagosDocumento";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
