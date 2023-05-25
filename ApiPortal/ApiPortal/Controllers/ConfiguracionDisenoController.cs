using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Collections.Specialized.BitVector32;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionDisenoController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConfiguracionDisenoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetConfiguracion")]
        public async Task<ActionResult> GetConfiguracion()
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionDiseno/GetConfiguracion";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var configuracion = _context.ConfiguracionDisenos.FirstOrDefault();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(configuracion);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionDiseno/GetConfiguracion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("SaveConfiguracion/{seccion}"), Authorize]
        public async Task<ActionResult> SaveConfiguracion(int seccion, [FromBody] ConfiguracionDiseno config)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionDiseno/SaveConfiguracion";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                var configuracionActual = _context.ConfiguracionDisenos.FirstOrDefault();

                switch (seccion)
                {
                    case 1:
                        configuracionActual.ColorFondoPortada = config.ColorFondoPortada;
                        configuracionActual.ColorBotonInicioSesion = config.ColorBotonInicioSesion;
                        configuracionActual.ColorBotonPagoRapido = config.ColorBotonPagoRapido;
                        configuracionActual.ColorBotonPagar = config.ColorBotonPagar;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoPortada).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonInicioSesion).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonPagoRapido).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonPagar).IsModified = true;
                        break;

                    case 4:
                        configuracionActual.TituloPendientesDashboard = config.TituloPendientesDashboard;
                        configuracionActual.TituloVencidosDashboard = config.TituloVencidosDashboard;
                        configuracionActual.TituloPorVencerDashboard = config.TituloPorVencerDashboard;
                        configuracionActual.TituloUltimasCompras = config.TituloUltimasCompras;
                        configuracionActual.TituloMonedaPeso = config.TituloMonedaPeso;
                        configuracionActual.TituloOtraMoneda = config.TituloOtraMoneda;
                        configuracionActual.ColorFondoDocumentos = config.ColorFondoDocumentos;
                        configuracionActual.ColorTextoPendientes = config.ColorTextoPendientes;
                        configuracionActual.ColorFondoVencidos = config.ColorFondoVencidos;
                        configuracionActual.ColorTextoVencidos = config.ColorTextoVencidos;
                        configuracionActual.ColorTextoPorVencer = config.ColorTextoPorVencer;
                        configuracionActual.ColorFondoPorVencer = config.ColorFondoPorVencer;
                        configuracionActual.ColorSeleccionDocumentos = config.ColorSeleccionDocumentos;
                        configuracionActual.ColorFondoUltimasCompras = config.ColorFondoUltimasCompras;
                        configuracionActual.ColorBotonUltimasCompras = config.ColorBotonUltimasCompras;
                        configuracionActual.ColorHoverBotonUltimasCompras = config.ColorHoverBotonUltimasCompras;
                        configuracionActual.ColorTextoBotonUltimasCompras = config.ColorTextoBotonUltimasCompras;
                        _context.Entry(configuracionActual).Property(x => x.TituloPendientesDashboard).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloVencidosDashboard).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloPorVencerDashboard).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloUltimasCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloMonedaPeso).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloOtraMoneda).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoDocumentos).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorTextoPendientes).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoVencidos).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorTextoVencidos).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorTextoPorVencer).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoPorVencer).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorSeleccionDocumentos).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoUltimasCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonUltimasCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorHoverBotonUltimasCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorTextoBotonUltimasCompras).IsModified = true;
                        break;

                    case 5:
                        configuracionActual.TituloMisCompras = config.TituloMisCompras;
                        configuracionActual.TituloComprasFacturadas = config.TituloComprasFacturadas;
                        configuracionActual.TituloPendientesFacturar = config.TituloPendientesFacturar;
                        configuracionActual.TituloProductos = config.TituloProductos;
                        configuracionActual.TituloGuiasPendientes = config.TituloGuiasPendientes;
                        configuracionActual.ColorFondoMisCompras = config.ColorFondoMisCompras;
                        configuracionActual.ColorFondoPendientesMisCompras = config.ColorFondoPendientesMisCompras;
                        configuracionActual.ColorFondoProductosMisCompras = config.ColorFondoProductosMisCompras;
                        configuracionActual.ColorFondoGuiasMisCompras = config.ColorFondoGuiasMisCompras;
                        configuracionActual.ColorIconosMisCompras = config.ColorIconosMisCompras;
                        configuracionActual.ColorBotonBuscar = config.ColorBotonBuscar;
                        _context.Entry(configuracionActual).Property(x => x.TituloMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloComprasFacturadas).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloPendientesFacturar).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloProductos).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.TituloGuiasPendientes).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoPendientesMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoProductosMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoGuiasMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorIconosMisCompras).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonBuscar).IsModified = true;
                        break;

                    case 6:
                        configuracionActual.ColorBotonModificarPerfil = config.ColorBotonModificarPerfil;
                        configuracionActual.ColorBotonClavePerfil = config.ColorBotonClavePerfil;
                        configuracionActual.ColorBotonEstadoPerfil = config.ColorBotonEstadoPerfil;
                        configuracionActual.ColorHoverBotonesPerfil = config.ColorHoverBotonesPerfil;
                        configuracionActual.ColorBotonCancelarModalPerfil = config.ColorBotonCancelarModalPerfil;
                        configuracionActual.ColorBotonGuardarModalPerfil = config.ColorBotonGuardarModalPerfil;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonModificarPerfil).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonClavePerfil).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonEstadoPerfil).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorHoverBotonesPerfil).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonCancelarModalPerfil).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonGuardarModalPerfil).IsModified = true;
                        break;
                }

                _context.SaveChanges();

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(config);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionDiseno/SaveConfiguracion";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("UploadImages/{numeroImagen}"), Authorize]
        public async Task<ActionResult> UploadImages(int numeroImagen)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionDiseno/UploadImages";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
           

            try
            {
                var empresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                var configuracionDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                string rutSinFormato = empresa.RutEmpresa.Replace(".", "");
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        if (numeroImagen != 15)
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

                            switch (numeroImagen)
                            {
                                case 1:
                                    configuracionDiseno.LogoPortada = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.LogoPortada).IsModified = true;
                                    break;
                                case 2:
                                    configuracionDiseno.ImagenPortada = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.ImagenPortada).IsModified = true;
                                    break;
                                case 3:
                                    configuracionDiseno.LogoSidebar = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.LogoSidebar).IsModified = true;
                                    break;
                                case 4:
                                    configuracionDiseno.LogoMinimalistaSidebar = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.LogoMinimalistaSidebar).IsModified = true;
                                    break;
                                case 5:
                                    configuracionDiseno.BannerPagoRapido = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.BannerPagoRapido).IsModified = true;
                                    break;
                                case 6:
                                    configuracionDiseno.ImagenUltimasCompras = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.ImagenUltimasCompras).IsModified = true;
                                    break;
                                case 7:
                                    configuracionDiseno.IconoMisCompras = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.IconoMisCompras).IsModified = true;
                                    break;
                                case 8:
                                    configuracionDiseno.BannerMisCompras = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.BannerMisCompras).IsModified = true;
                                    break;
                                case 9:
                                    configuracionDiseno.ImagenUsuario = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.ImagenUsuario).IsModified = true;
                                    break;
                                case 10:
                                    configuracionDiseno.BannerPortal = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.BannerPortal).IsModified = true;
                                    break;
                                case 11:
                                    configuracionDiseno.IconoContactos = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.IconoContactos).IsModified = true;
                                    break;
                                case 12:
                                    configuracionDiseno.IconoClavePerfil = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.IconoClavePerfil).IsModified = true;
                                    break;
                                case 13:
                                    configuracionDiseno.IconoEditarPerfil = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.IconoEditarPerfil).IsModified = true;
                                    break;
                                case 14:
                                    configuracionDiseno.IconoEstadoPerfil = finalUrl;
                                    _context.Entry(configuracionDiseno).Property(x => x.IconoEstadoPerfil).IsModified = true;
                                    break;
                            }
                        }
                        else
                        {
                            //CODIGO PARA SUBIR UN FAVICON, SE NECESITA VER LOGICA PARA REALIZAR ESTO CON EL MULTITENANT Y EL FTP PARA ACCEDER A LAS CARPETAS DEL PROYECTOa
                            //var fileAux = httpRequest.Form.Files[0];
                            //string ftpServer = "ftps://waws-prod-blu-333.ftp.azurewebsites.windows.net";
                            //string ftpUsername = @"SOFCLUES-PD-APS-PORTALCLIENTE-FRONTEND\$SOFCLUES-PD-APS-PORTALCLIENTE-FRONTEND";
                            //string ftpPassword = "CtkjXJQHqusmCMCTfJ9kD6tyC8btZn8u6SAgJW8AllprKsiqoG1Wwhyx2ima";
                            //string filePath = @"C:\example.txt";

                            //FtpClient client = new FtpClient();
                            //client.Host = ftpServer;
                            //client.Credentials = new NetworkCredential(ftpUsername, "/site/wwwroot/" + ftpPassword);
                            //client.Connect();

                            //Stream requestStream = fileAux.InputStream;

                            //client.UploadStream(fileAux.InputStream, "favicon.ico");
                            //client.Disconnect();
                            //if (client.GetReply().Code == "226")
                            //{
                            //    configuracionDiseno.IconoEstadoPerfil = empresa.UrlPortal + "/" + fileAux.FileName;
                            //    db.Entry(configuracionDiseno).Property(x => x.Favicon).IsModified = true;
                            //}
                            //else
                            //{
                            //    return BadRequest();
                            //}

                        }




                    }
                    await _context.SaveChangesAsync();
                }

                logApi.Termino = DateTime.Now;
                logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                sf.guardarLogApi(logApi);

                return Ok(configuracionDiseno);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionDiseno/UploadImages";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

    }
}
