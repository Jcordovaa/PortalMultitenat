using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Collections.Specialized.BitVector32;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ApiPortal.Dal.Models_Admin;
using Microsoft.AspNetCore.Hosting;

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
        private readonly PortalAdministracionSoftlandContext _admin;

        public ConfiguracionDisenoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _admin = admin;
        }

        [HttpGet("GetConfiguracion")]
        public async Task<ActionResult> GetConfiguracion()
        {

            var dominioAdmin = _admin.ConfiguracionImplementacions.FirstOrDefault();
            if (dominioAdmin.DominioImplementacion == new Uri(_httpContextAccessor.HttpContext.Request.Headers["Origin"]).Host)
            {
                var configuracion = new ConfiguracionDiseno
                {
                    ColorBotonBuscar = "#263db5",
                    ColorBotonCancelarModalPerfil = "#263db5",
                    ColorBotonClavePerfil = "#263db5",
                    ColorBotonEstadoPerfil = "#263db5",
                    ColorBotonGuardarModalPerfil = "#263db5",
                    ColorBotonInicioSesion = "#fff",
                    ColorBotonModificarPerfil = "#263db5",
                    ColorBotonPagar = "#263db5",
                    ColorBotonPagoRapido = "#263db5",
                    ColorBotonUltimasCompras = "#263db5",
                    ColorFondoDocumentos = "#263db5",
                    ColorFondoGuiasMisCompras = "#263db5",
                    ColorFondoMisCompras = "#263db5",
                    ColorFondoPendientesMisCompras = "#263db5",
                    ColorFondoPortada = "#263db5",
                    ColorFondoPorVencer = "#263db5",
                    ColorFondoProductosMisCompras = "#263db5",
                    ColorFondoResumenContable = "#263db5",
                    ColorFondoUltimasCompras = "#263db5",
                    ColorFondoVencidos = "#263db5",
                    ColorHoverBotonesPerfil = "#677ce6",
                    ColorHoverBotonUltimasCompras = "#677ce6",
                    ColorIconoPendientes = "#fff",
                    ColorIconoPorVencer = "#fff",
                    ColorIconosMisCompras = "#fff",
                    ColorIconoVencidos = "#fff",
                    ColorPaginador = "#fff",
                    ColorSeleccionDocumentos = "#677ce6",
                    ColorTextoBotonUltimasCompras = "#fff",
                    ColorTextoFechaUltimasCompras = "#333",
                    ColorTextoMisCompras = "#fff",
                    ColorTextoMontoUltimasCompras = "#333",
                    ColorTextoPendientes = "#fff",
                    ColorTextoPorVencer = "#fff",
                    ColorTextoUltimasCompras = "#333",
                    TextoCobranzaExpirada = "El link para realizar este pago ha expirado, inicie sesión en el portal para realizar el pago de los documentos.",
                    TextoDescargaCobranza = "Si la descarga no ha iniciado presione el botón para descargar el documento.",
                    TextoNoConsideraTodaDeuda = "El portal podria no considerar todos los tipos de documentos, Por tanto los montos podrian no reflejarse en su totalidad.",
                    TituloResumenContable = "Estado Contable",
                    TituloUltimasCompras = "Últimas {cantidad} Compras Facturadas",
                    TituloMisCompras = "Mis Compras",
                    TituloComprasFacturadas = "Compras Facturadas",
                    TituloPendientesFacturar = "Compras pendientes de facturar",
                    TituloProductos = "Productos Comprados",
                    TituloGuiasPendientes = "Despachos pendientes de facturar",
                    ColorTextoVencidos = "#fff",
                    TituloMonedaPeso = "Moneda Nacional",
                    TituloPendientesDashboard = "Documentos Pendientes",
                    TituloVencidosDashboard = "Documentos Vencidos",
                    TituloPorVencerDashboard = "Documentos por vencer",
                    TituloOtraMoneda = "Otras Monedas",
                    BannerMisCompras = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/BannerMisCompras.png",
                    BannerPagoRapido = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/BannerPagoRapido.png",
                    BannerPortal = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/BannerPortal.png",
                    IconoClavePerfil = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/IconoClavePerfil.png",
                    IconoContactos = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/IconoContactos.png",
                    IconoEditarPerfil = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/IconoEditarPerfil.png",
                    IconoMisCompras = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/IconoMisCompras.png",
                    ImagenPortada = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/ImagenPortada.png",
                    ImagenUltimasCompras = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/ImagenUltimasCompras.png",
                    ImagenUsuario = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/ImagenUsuario.png",
                    LogoPortada = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/LogoPortada.png",
                    LogoMinimalistaSidebar = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/SoftlandLatera.png",
                    LogoSidebar = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/logov2.png"
                };

                return Ok(configuracion);
            }
            else
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
                    log.Hora = DateTime.Now.ToString("HH:mm:ss");
                    log.Excepcion = ex.StackTrace;
                    log.Mensaje = ex.Message;
                    log.Ruta = "api/ConfiguracionDiseno/GetConfiguracion";
                    _context.LogProcesos.Add(log);
                    _context.SaveChanges();
                    return BadRequest(ex.Message);
                }
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
                        configuracionActual.TextoDescargaCobranza = config.TextoDescargaCobranza;
                        _context.Entry(configuracionActual).Property(x => x.ColorFondoPortada).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonInicioSesion).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonPagoRapido).IsModified = true;
                        _context.Entry(configuracionActual).Property(x => x.ColorBotonPagar).IsModified = true;
                        break;

                    case 3:
                        configuracionActual.TextoCobranzaExpirada = config.TextoCobranzaExpirada;
                        _context.Entry(configuracionActual).Property(x => x.TextoCobranzaExpirada).IsModified = true;
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
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
                var apiSoftland = _context.ApiSoftlands.FirstOrDefault();

                string rutSinFormato = apiSoftland.UrlAlmacenamientoArchivos.Split('/')[apiSoftland.UrlAlmacenamientoArchivos.Split('/').Length - 2].ToString();
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = _httpContextAccessor.HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {

                        var postedFile = file;

                        //JCA 01-06-2023: Cambio por Blob service de azure
                        //Instanciamos BlobServiceClient con la cadena de conexion donde almacenaremos los archivos
                        string connectionString = apiSoftland.CadenaAlmacenamientoAzure;
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                        //Validamos si contenedor para empresa existe
                        string containerName = rutSinFormato;
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                        bool exists = await containerClient.ExistsAsync();
                        if (!exists)
                        {
                            //Contenedor no existe, debemos crearlo con acceso publico
                            containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                        }


                        //Cambiamos nombre archivo
                        Utils util = new Utils();
                        string nombreArchivo = util.nombreArchivo(postedFile.FileName, numeroImagen);

                        using (var stream = System.IO.File.Create(nombreArchivo))
                        {
                            postedFile.CopyTo(stream);

                            //Obtenemos archivo para sobrescribir en caso de que exista
                            BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

                            //var stream2 = postedFile.OpenReadStream();
                            //var memoryStream = new MemoryStream();
                            //await stream2.CopyToAsync(memoryStream);


                            //using FileStream nuevoFileStream = memoryStream;
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            //nuevoFileStream.Close();
                        }

                        //Obtenemos archivo para sobrescribir en caso de que exista
                        //BlobClient blobClient = containerClient.GetBlobClient(nombreAchivo);

                        //var stream2 = postedFile.OpenReadStream();
                        //var memoryStream = new MemoryStream();
                        //await stream2.CopyToAsync(memoryStream);


                        //using FileStream nuevoFileStream = memoryStream;
                        //await blobClient.UploadAsync(postedFile.OpenReadStream(), overwrite: true);
                        //nuevoFileStream.Close();



                        //var folderPath = AppDomain.CurrentDomain.BaseDirectory + @"Uploads\" + rutSinFormato;


                        //if (!Directory.Exists(folderPath))
                        //{
                        //    //Si no existe lo creamos
                        //    Directory.CreateDirectory(folderPath);
                        //}


                        //var filePath = folderPath + "/" + postedFile.FileName;


                        //if (System.IO.File.Exists(filePath))
                        //{
                        //    System.IO.File.Delete(filePath);
                        //}

                        //using (var stream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await postedFile.CopyToAsync(stream);
                        //}



                        var finalUrl = apiSoftland.UrlAlmacenamientoArchivos + nombreArchivo;

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
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
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
