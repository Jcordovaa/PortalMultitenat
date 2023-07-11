using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
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
    public class ConfiguracionCorreoController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfiguracionCorreoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("actualizaConfiguracionCorreo"), Authorize]
        public async Task<ActionResult> actualizaConfiguracionCorreo(ConfiguracionCorreo model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionCorreo/actualizaConfiguracionCorreo";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                string encrypt = Encrypt.Base64Encode(model.Clave);

                model.Clave = encrypt;

                var config = _context.ConfiguracionCorreos.FirstOrDefault();
                config.SmtpServer = model.SmtpServer;
                config.Usuario = model.Usuario;
                config.Clave = encrypt;
                config.CorreoOrigen = model.CorreoOrigen;
                config.Puerto = model.Puerto;
                config.Ssl = model.Ssl;
                config.CorreoAvisoPago = model.CorreoAvisoPago;
                config.CantidadCorreosAcceso = model.CantidadCorreosAcceso;
                config.CantidadCorreosNotificacion = model.CantidadCorreosNotificacion;
                config.NombreCorreos = model.NombreCorreos;

                _context.Entry(config).Property(x => x.SmtpServer).IsModified = true;
                _context.Entry(config).Property(x => x.Usuario).IsModified = true;
                _context.Entry(config).Property(x => x.Clave).IsModified = true;
                _context.Entry(config).Property(x => x.CorreoOrigen).IsModified = true;
                _context.Entry(config).Property(x => x.Puerto).IsModified = true;
                _context.Entry(config).Property(x => x.Ssl).IsModified = true;
                _context.Entry(config).Property(x => x.CorreoAvisoPago).IsModified = true;
                _context.Entry(config).Property(x => x.CantidadCorreosAcceso).IsModified = true;
                _context.Entry(config).Property(x => x.CantidadCorreosNotificacion).IsModified = true;
                _context.Entry(config).Property(x => x.NombreCorreos).IsModified = true;

                //db.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();

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
                log.Ruta = "api/ConfiguracionCorreo/actualizaConfiguracionCorreo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("actualizaTextos/{tipo}"), Authorize]
        public async Task<ActionResult> actualizaTextos(int tipo, [FromBody]ConfiguracionCorreo model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionCorreo/actualizaTextos";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var config = _context.ConfiguracionCorreos.FirstOrDefault();

                switch (tipo)
                {
                    case 1:
                        config.ColorBoton = model.ColorBoton;
                        _context.Entry(config).Property(x => x.ColorBoton).IsModified = true;
                        break;

                    case 2:
                        config.AsuntoPagoCliente = model.AsuntoPagoCliente;
                        config.TituloPagoCliente = model.TituloPagoCliente;
                        config.TextoPagoCliente = model.TextoPagoCliente;
                        _context.Entry(config).Property(x => x.AsuntoPagoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloPagoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoPagoCliente).IsModified = true;
                        break;

                    case 3:
                        config.AsuntoAccesoCliente = model.AsuntoAccesoCliente;
                        config.TituloAccesoCliente = model.TituloAccesoCliente;
                        config.TextoMensajeActivacion = model.TextoMensajeActivacion;
                        _context.Entry(config).Property(x => x.AsuntoAccesoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloAccesoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoMensajeActivacion).IsModified = true;
                        break;

                    case 4:
                        config.AsuntoCambioDatos = model.AsuntoCambioDatos;
                        config.TituloCambioDatos = model.TituloCambioDatos;
                        config.TextoCambioDatos = model.TextoCambioDatos;
                        _context.Entry(config).Property(x => x.AsuntoCambioDatos).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloCambioDatos).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoCambioDatos).IsModified = true;
                        break;

                    case 5:
                        config.AsuntoCambioClave = model.AsuntoCambioClave;
                        config.TituloCambioClave = model.TituloCambioClave;
                        config.TextoCambioClave = model.TextoCambioClave;
                        _context.Entry(config).Property(x => x.AsuntoCambioClave).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloCambioClave).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoCambioClave).IsModified = true;
                        break;

                    case 6:
                        config.AsuntoCambioCorreo = model.AsuntoCambioCorreo;
                        config.TituloCambioCorreo = model.TituloCambioCorreo;
                        config.TextoCambioCorreo = model.TextoCambioCorreo;
                        _context.Entry(config).Property(x => x.AsuntoCambioCorreo).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloCambioCorreo).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoCambioCorreo).IsModified = true;
                        break;

                    case 7:
                        config.TituloRecuperarClave = model.TituloRecuperarClave;
                        config.AsuntoRecuperarClave = model.AsuntoRecuperarClave;
                        config.TextoRecuperarClave = model.TextoRecuperarClave;
                        _context.Entry(config).Property(x => x.TituloRecuperarClave).IsModified = true;
                        _context.Entry(config).Property(x => x.AsuntoRecuperarClave).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoRecuperarClave).IsModified = true;
                        break;

                    case 8:
                        config.AsuntoEnvioDocumentos = model.AsuntoEnvioDocumentos;
                        config.TituloEnvioDocumentos = model.TituloEnvioDocumentos;
                        config.TextoEnvioDocumentos = model.TextoEnvioDocumentos;
                        _context.Entry(config).Property(x => x.AsuntoEnvioDocumentos).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloEnvioDocumentos).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoEnvioDocumentos).IsModified = true;
                        break;

                    case 9:
                        config.AsuntoAvisoPagoCliente = model.AsuntoAvisoPagoCliente;
                        config.TituloAvisoPagoCliente = model.TituloAvisoPagoCliente;
                        config.TextoAvisoPagoCliente = model.TextoAvisoPagoCliente;
                        _context.Entry(config).Property(x => x.AsuntoAvisoPagoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloAvisoPagoCliente).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoAvisoPagoCliente).IsModified = true;
                        break;

                    case 10:
                        config.AsuntoPagoSinComprobante = model.AsuntoPagoSinComprobante;
                        config.TituloPagoSinComprobante = model.TituloPagoSinComprobante;
                        config.TextoPagoSinComprobante = model.TextoPagoSinComprobante;
                        _context.Entry(config).Property(x => x.AsuntoPagoSinComprobante).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloPagoSinComprobante).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoPagoSinComprobante).IsModified = true;
                        break;

                    case 11:
                        config.AsuntoCobranza = model.AsuntoCobranza;
                        config.TituloCobranza = model.TituloCobranza;
                        config.TextoCobranza = model.TextoCobranza;
                        _context.Entry(config).Property(x => x.AsuntoCobranza).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloCobranza).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoCobranza).IsModified = true;
                        break;

                    case 12:
                        config.AsuntoPreCobranza = model.AsuntoPreCobranza;
                        config.TituloPreCobranza = model.TituloPreCobranza;
                        config.TextoPreCobranza = model.TextoPreCobranza;
                        _context.Entry(config).Property(x => x.AsuntoPreCobranza).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloPreCobranza).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoPreCobranza).IsModified = true;
                        break;

                    case 13:
                        config.AsuntoEstadoCuenta = model.AsuntoEstadoCuenta;
                        config.TituloEstadoCuenta = model.TituloEstadoCuenta;
                        config.TextoEstadoCuenta = model.TextoEstadoCuenta;
                        _context.Entry(config).Property(x => x.AsuntoEstadoCuenta).IsModified = true;
                        _context.Entry(config).Property(x => x.TituloEstadoCuenta).IsModified = true;
                        _context.Entry(config).Property(x => x.TextoEstadoCuenta).IsModified = true;
                        break;
                }

                await _context.SaveChangesAsync();


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
                log.Ruta = "api/ConfiguracionCorreo/actualizaTextos";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("uploadLogo"), Authorize]
        public async Task<ActionResult> uploadLogo()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionCorreo/uploadLogo";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var empresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                var configuracionCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                string rutSinFormato = empresa.RutEmpresa.Replace(".", "");
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {

                        var postedFile = file;
                        var folderPath = AppDomain.CurrentDomain.BaseDirectory + @"Uploads\" + rutSinFormato;


                        if (!Directory.Exists(folderPath))
                        {
                            //Si no existe lo creamos
                            Directory.CreateDirectory(folderPath);
                        }


                        var filePath = folderPath + "/" + postedFile.FileName;


                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await postedFile.CopyToAsync(stream);
                        }




                        var finalUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}" + "/Uploads/" + rutSinFormato + "/" + postedFile.FileName;


                        configuracionCorreo.LogoCorreo = finalUrl;
                        _context.Entry(configuracionCorreo).Property(x => x.LogoCorreo).IsModified = true;
                        await _context.SaveChangesAsync();
                    }

                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(configuracionCorreo);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionCorreo/uploadLogo";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("getTemplate/{tipo}"), Authorize]
        public async Task<ActionResult> getTemplate(int tipo, [FromBody] ConfiguracionCorreo model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionCorreo/getTemplate";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                string body = string.Empty;
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();

                switch (tipo)
                {
                    case 1:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TEXTO}", model.TextoMensajeActivacion);
                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{RUT}", "11.111.111-1");
                        body = body.Replace("{CORREO}", "prueba@prueba.cl");
                        body = body.Replace("{CLAVE}", "123456");
                        body = body.Replace("{Titulo}", model.TituloAccesoCliente);
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        string datosCliente = Encrypt.Base64Encode("123456" + ";" + "prueba@prueba.cl");
                        body = body.Replace("{ENLACE}", "");
                        break;

                    case 2:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioDocumentos.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{TextoCorreo}", model.TextoPagoCliente);
                        body = body.Replace("{TituloCorreo}", model.TituloPagoCliente);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        break;

                    case 3:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{EMPRESA}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TEXTO}", model.TextoMensajeActivacion);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{RUT}", "11.111.111-1");
                        body = body.Replace("{CORREO}", "prueba@prueba.cl");
                        body = body.Replace("{CLAVE}", "123456");
                        body = body.Replace("{Titulo}", model.TituloAccesoCliente);
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        body = body.Replace("{ENLACE}", "");
                        break;

                    case 4:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioDocumentos.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{TextoCorreo}", model.AsuntoCambioDatos);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloCambioDatos);
                        body = body.Replace("{LOGO}", configEmpresa.Logo);
                        break;

                    case 5:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{CLAVE}", "123456");
                        body = body.Replace("{logo}", model.LogoCorreo);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{Titulo}", model.TituloCambioClave);
                        body = body.Replace("{Texto}", model.TextoCambioClave);
                        break;

                    case 6:

                        break;

                    case 7:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{Texto}", model.TextoRecuperarClave);
                        body = body.Replace("{logo}", model.LogoCorreo);
                        body = body.Replace("{NOMBRE}", "prueba@prueba.cl");
                        body = body.Replace("{CLAVE}", "123456");
                        body = body.Replace("{Titulo}", model.TituloRecuperarClave);
                        break;

                    case 8:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioDocumentos.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{TextoCorreo}", model.TextoEnvioDocumentos);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloEnvioDocumentos);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        break;

                    case 9:

                        break;

                    case 10:

                        break;

                    case 11:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/cobranza.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{ENLACE}", $"{configEmpresa.UrlPortal}/#/sessions/pay/0/0/0/0");
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloCobranza);
                        body = body.Replace("{TextoCorreo}", model.TextoCobranza);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        body = body.Replace("{ENLACEDOCUMENTO}", "");

                        break;

                    case 12:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/cobranza.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{ENLACE}", "");
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloPreCobranza);
                        body = body.Replace("{TextoCorreo}", model.TextoPreCobranza);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        body = body.Replace("{ENLACEDOCUMENTO}", "");
                        break;

                    case 13:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/cobranza.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        body = body.Replace("{ENLACE}", "");
                        body = body.Replace("{NombreEmpresa}", configEmpresa.NombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloEstadoCuenta);
                        body = body.Replace("{TextoCorreo}", model.TextoEstadoCuenta);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        body = body.Replace("{ENLACEDOCUMENTO}", "");
                        break;
                }

                var html = new
                {
                    body = body
                };

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(html);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionCorreo/getTemplate";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetConfiguracionCorreo"), Authorize]
        public async Task<ActionResult> GetConfiguracionCorreo()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionCorreo/GetConfiguracionCorreo";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var conf = await _context.ConfiguracionCorreos.AsNoTracking().ToListAsync();
                foreach (var item in conf)
                {
                    item.Clave = Encrypt.Base64Decode(item.Clave);
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(conf);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
