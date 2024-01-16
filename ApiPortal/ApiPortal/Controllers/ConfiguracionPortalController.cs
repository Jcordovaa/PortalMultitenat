using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPortal.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]

    [ApiController]
    public class ConfiguracionPortalController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IHttpContextAccessor _contextAccessor;

        public ConfiguracionPortalController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("GetConfiguracionPortal")]
        public async Task<ActionResult> GetConfiguracionPortal()
        {
           
         

            try
            {
                var dominioAdmin = _admin.ConfiguracionImplementacions.FirstOrDefault();
                if (dominioAdmin.DominioImplementacion == new Uri(_contextAccessor.HttpContext.Request.Headers["Origin"]).Host)
                {
                    var configuracionPortal = new ConfiguracionPortal
                    {
                        CantidadUltimasCompras = 10,
                        HabilitaPagoRapido = 0,
                        CantUltPagosAnul = 5,
                        CantUltPagosRec = 5,
                        ContabilidadSoftland = 1,
                        CrmSoftland = 1,
                        InventarioSoftland = 1,
                        MuestraBotonEnviarCorreo = 1,
                        MuestraBotonImprimir = 1,
                        MuestraComprasFacturadas = 1,
                        MuestraContactoComercial = 1,
                        MuestraContactosEnvio = 1,
                        MuestraEstadoBloqueo = 1,
                        MuestraContactosPerfil = 1,
                        MuestraEstadoSobregiro = 1,
                        MuestraGuiasPendientes = 1,
                        MuestraPendientesFacturar = 1,
                        MuestraProductos = 1,
                        MuestraResumen = 1,
                        MuestraUltimasCompras = 1,
                        NotaVentaSoftland = 1,
                        PermiteAbonoParcial = 1,
                        ResumenContabilizado = 1,
                        PermiteExportarExcel = 1,
                        UtilizaDocumentoPagoRapido = 1,
                        UtilizaNumeroDireccion = 1,
                        EstadoImplementacion = 3
                    };

                    return Ok(configuracionPortal);
                }
                else
                {
                    SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                    LogApi logApi = new LogApi();
                    logApi.Api = "api/ConfiguracionPorta/GetConfiguracionPortal";
                    logApi.Inicio = DateTime.Now;
                    logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
                    var configuracionPortal = _context.ConfiguracionPortals.FirstOrDefault();

                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);

                    return Ok(configuracionPortal);
                }
       
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPorta/GetConfiguracionPortal";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaConfiguracion/{dias}"), Authorize]
        public async Task<ActionResult> ActualizaConfiguracion(int dias, [FromBody]ConfiguracionPortal model)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ConfiguracionPorta/ActualizaConfiguracion";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            try
            {
                _context.Entry(model).State = EntityState.Modified;
                var configPagos = _context.ConfiguracionPagoClientes.FirstOrDefault();
                configPagos.DiasPorVencer = dias;
                _context.Entry(configPagos).State = EntityState.Modified;
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
                log.Ruta = "api/ConfiguracionPorta/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllConfiguracionPortal")]
        public async Task<ActionResult> GetAllConfiguracionPortal()
        {
           
         

            try
            {
                var dominioAdmin = _admin.ConfiguracionImplementacions.FirstOrDefault();
                if (dominioAdmin.DominioImplementacion == new Uri(_contextAccessor.HttpContext.Request.Headers["Origin"]).Host)
                {
                    var configuracionDiseno = new ConfiguracionDiseno
                    {
                        ColorBotonBuscar = "#263db5",
                        ColorBotonCancelarModalPerfil = "#263db5",
                        ColorBotonClavePerfil = "#263db5",
                        ColorBotonEstadoPerfil = "#263db5",
                        ColorBotonGuardarModalPerfil = "#263db5",
                        ColorBotonInicioSesion = "#263db5",
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

                    var configuracionPortal = new ConfiguracionPortal
                    {
                        CantidadUltimasCompras = 10,
                        HabilitaPagoRapido = 0,
                        CantUltPagosAnul = 5,
                        CantUltPagosRec = 5,
                        ContabilidadSoftland = 1,
                        CrmSoftland = 1,
                        InventarioSoftland = 1,
                        MuestraBotonEnviarCorreo = 1,
                        MuestraBotonImprimir = 1,
                        MuestraComprasFacturadas = 1,
                        MuestraContactoComercial = 1,
                        MuestraContactosEnvio = 1,
                        MuestraEstadoBloqueo = 1,
                        MuestraContactosPerfil = 1,
                        MuestraEstadoSobregiro = 1,
                        MuestraGuiasPendientes = 1,
                        MuestraPendientesFacturar = 1,
                        MuestraProductos = 1,
                        MuestraResumen = 1,
                        MuestraUltimasCompras = 1,
                        NotaVentaSoftland = 1,
                        PermiteAbonoParcial = 1,
                        ResumenContabilizado = 1,
                        PermiteExportarExcel = 1,
                        UtilizaDocumentoPagoRapido = 1,
                        UtilizaNumeroDireccion = 1,
                        EstadoImplementacion = 3
                    };

                    var configuracionPago = new ConfiguracionPagoCliente
                    {
                        AnioTributario = 2000,
                        DiasPorVencer = 10,
                        GlosaComprobante = "CONTABILIZACION ",
                        MonedaUtilizada = "01"
                    };

                    var configuracionCompleta = new
                    {
                        ConfiguracionDiseno = configuracionDiseno,
                        ConfiguracionPagoCliente = configuracionPago,
                        ConfiguracionPortal = configuracionPortal,
                        ExistModuloContabilidad = true,
                        ExistModuloInventario = true,
                        ExistModuloNotaVenta = true
                    };

                    return Ok(configuracionCompleta);
                }
                else
                {
                    SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                    LogApi logApi = new LogApi();
                    logApi.Api = "api/ConfiguracionPorta/GetAllConfiguracionPortal";
                    logApi.Inicio = DateTime.Now;
                    logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();

                    var configuracionDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                    var configuracionPortal = _context.ConfiguracionPortals.FirstOrDefault();
                    var configuracionPago = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    configuracionDiseno.TituloUltimasCompras = "Últimas {cantidad} Compras Facturadas";

                    var modulos = await sf.GetModulosSoftlandAsync(logApi.Id);
                    bool existModuloInventario = false;
                    bool existModuloNotaVenta = false;
                    bool existModuloContabilidad = false;

                    existModuloInventario = modulos.Where(x => x.Codi == "IW").FirstOrDefault() == null ? false : true;
                    existModuloNotaVenta = modulos.Where(x => x.Codi == "NW").FirstOrDefault() == null ? false : true;
                    existModuloContabilidad = modulos.Where(x => x.Codi == "CW").FirstOrDefault() == null ? false : true;

                    var configuracionCompleta = new
                    {
                        ConfiguracionDiseno = configuracionDiseno,
                        ConfiguracionPagoCliente = configuracionPago,
                        ConfiguracionPortal = configuracionPortal,
                        ExistModuloContabilidad = existModuloContabilidad,
                        ExistModuloInventario = existModuloInventario,
                        ExistModuloNotaVenta = existModuloNotaVenta
                    };


                    logApi.Termino = DateTime.Now;
                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                    sf.guardarLogApi(logApi);

                    return Ok(configuracionCompleta);
                }
               

            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/ConfiguracionPorta/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
