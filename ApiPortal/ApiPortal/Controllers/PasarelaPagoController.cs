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
    public class PasarelaPagoController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PasarelaPagoController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetAllPasarelas"), Authorize]
        public async Task<ActionResult> GetAllPasarelas()
        {

            try
            {
                var pasarela = _context.PasarelaPagos.Where(x => x.Estado == 1).AsQueryable();
                List<PasarelaPagoVm> pasarelas = new List<PasarelaPagoVm>();

                foreach (var item in pasarela)
                {
                    PasarelaPagoVm p = new PasarelaPagoVm
                    {
                        Ambiente = item.Ambiente,
                        CuentaContable = item.CuentaContable,
                        Estado = (int)item.Estado,
                        IdPasarela = item.IdPasarela,
                        Logo = item.Logo,
                        MonedaPasarela = item.MonedaPasarela,
                        Nombre = item.Nombre,
                        Protocolo = item.Protocolo,
                        TipoDocumento = item.TipoDocumento
                    };
                    pasarelas.Add(p);
                }

                return Ok(pasarelas);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/PasarelaPago/GetAllPasarelas";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ActualizaPasarelas"), Authorize]
        public async Task<ActionResult> ActualizaPasarelas(PasarelaPago model)
        {
            try
            {
                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                return Ok();
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/PasarelaPago/GetCobranzaCliente";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetLog/{idPago}")]
        public async Task<ActionResult> GetLog(int idPago)
        {

            try
            {
                var log = await _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefaultAsync();

                if (log == null)
                {
                    LogProceso logError = new LogProceso();
                    logError.Fecha = DateTime.Now;
                    logError.IdTipoProceso = -1;
                    logError.Excepcion = "Log pasarela NULL";
                    logError.Mensaje = "Log pasarela NULL";
                    logError.Ruta = "api/PasarelaPago/GetLog";
                    _context.LogProcesos.Add(logError);
                    _context.SaveChanges();
                    return BadRequest();
                }

                var pasarela = _context.PasarelaPagos.Where(x => x.IdPasarela == log.IdPasarela).FirstOrDefault();
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                var pasarelaLog = new PasarelaPagoLogVm();
                pasarelaLog.Id = log.Id;
                pasarelaLog.IdPago = (int)log.IdPago;
                pasarelaLog.IdPasarela = (int)log.IdPasarela;
                pasarelaLog.Fecha = (DateTime)log.Fecha;
                pasarelaLog.Monto = (decimal)log.Monto;
                pasarelaLog.Token = log.Token;
                pasarelaLog.Codigo = log.Codigo;
                pasarelaLog.Estado = log.Estado;
                pasarelaLog.OrdenCompra = log.OrdenCompra;
                pasarelaLog.MedioPago = log.MedioPago;
                pasarelaLog.Cuotas = (int)log.Cuotas;
                pasarelaLog.Tarjeta = log.Tarjeta;
                pasarelaLog.Url = log.Url;
                pasarelaLog.PasarelaPago = pasarela.Nombre;
                pasarelaLog.ComprobanteContable = pago.ComprobanteContable;
                pasarelaLog.ArchivoComprobante64 = "";
                pasarelaLog.TipoArchivo = "PDF";

                return Ok(pasarelaLog);
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.IdTipoProceso = -1;
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/PasarelaPago/GetLog";
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
