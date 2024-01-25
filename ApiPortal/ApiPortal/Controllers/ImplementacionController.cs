using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using iText.Commons.Actions.Contexts;
using ApiPortal.ViewModelsPortal;
using ApiPortal.ViewModelsAdmin;
using Microsoft.AspNetCore.Authorization;
using ApiPortal.Services;
using ApiPortal.DAL.Models_Admin;
using Microsoft.Data.SqlClient;
using System.Security.Policy;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using ApiPortal.Security;
using System.Net;
using ApiPortal.ModelSoftland;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace ApiPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImplementacionController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        public ImplementacionController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
            _contextAccessor = contextAccessor;
        }


        // GET: api/Implementacion
        [HttpPost("ObtenerEmpresasPorPagina"), Authorize]
        public async Task<ActionResult> GetEmpresasByPage(PaginadorImplementacionVm paginador)
        {
            try
            {
                List<EmpresaVm> result = new List<EmpresaVm>();
                var empresas = _admin.EmpresasPortals.Include(x => x.IdEstadoNavigation).Include(x => x.Tenants).ThenInclude(x => x.IdImplementadorNavigation)
                  .Include(x => x.Tenants).ThenInclude(x => x.EstadoNavigation)
                   .Include(x => x.Tenants).ThenInclude(x => x.IdPlanNavigation)
                    .Include(x => x.Tenants).ThenInclude(x => x.IdAreaComercialNavigation)
                     .Include(x => x.Tenants).ThenInclude(x => x.IdLineaProductoNavigation).AsQueryable();

                if (paginador.Estado != null && paginador.Estado != 0)
                {
                    empresas = empresas.Where(x => x.Tenants.Any(j => j.Estado == paginador.Estado)).AsQueryable();
                }

                if (!string.IsNullOrEmpty(paginador.Search))
                {
                    empresas = empresas.Where(x => x.RazonSocial.Contains(paginador.Search)).AsQueryable();
                }

                if (paginador.implementador != 0 && paginador.implementador != null)
                {
                    empresas = empresas.Where(x => x.IdImplementador == paginador.implementador).AsQueryable();
                }

                if (!string.IsNullOrEmpty(paginador.RutEmpresa))
                {
                    empresas = empresas.Where(x => x.Rut == paginador.RutEmpresa).AsQueryable();
                }

                var empresasPaginadas = empresas.Skip((int)((paginador.Pagina - 1) * paginador.EndRow)).Take((int)paginador.Cantidad).ToList();

                if (empresasPaginadas.Count > 0)
                {
                    result = empresasPaginadas.ConvertAll(x => new EmpresaVm
                    {
                        Estado = x.IdEstadoNavigation == null ? null : new EmpresaEstadoVm
                        {
                            IdEstado = x.IdEstadoNavigation.IdEstado,
                            Nombre = x.IdEstadoNavigation.Nombre
                        },
                        IdEmpresa = x.IdEmpresa,
                        IdEstado = x.IdEstado,
                        RazonSocial = x.RazonSocial,
                        Rut = x.Rut,
                        Tenants = x.Tenants.Count() == 0 ? new List<TenantVm>() : x.Tenants.ToList().ConvertAll(j => new TenantVm
                        {
                            ConnectionString = j.ConnectionString,
                            Dominio = j.Dominio,
                            Estado = j.Estado,
                            IdEmpresa = j.IdEmpresa,
                            Identifier = j.Identifier,
                            IdTenant = j.IdTenant,
                            Implementador = j.IdImplementadorNavigation == null ? null : new ImplementadorVm
                            {
                                Estado = j.IdImplementadorNavigation.Estado,
                                IdImplementador = j.IdImplementadorNavigation.IdImplementador,
                                Nombre = j.IdImplementadorNavigation.Nombre,
                                Rut = j.IdImplementadorNavigation.Rut
                            },
                            AreaComercial = j.IdAreaComercialNavigation == null ? null : new AreaComercialVm
                            {
                                Estado = j.IdAreaComercialNavigation.Estado,
                                IdArea = j.IdAreaComercialNavigation.IdArea,
                                Nombre = j.IdAreaComercialNavigation.Nombre,
                            },
                            CorreoImplementador = j.CorreoImplementador,
                            FechaInicioContrato = j.FechaInicioContrato,
                            FechaInicioImplementacion = j.FechaInicioImplementacion,
                            FechaTerminoContrato = j.FechaTerminoContrato,
                            FechaTerminoImplementacion = j.FechaTerminoImplementacion,
                            IdAreaComercial = j.IdAreaComercial,
                            IdImplementador = j.IdImplementador,
                            IdLineaProducto = j.IdLineaProducto,
                            IdPlan = j.IdPlan,
                            LineaProducto = j.IdLineaProductoNavigation == null ? null : new LineaProductoVm
                            {
                                Estado = j.IdLineaProductoNavigation.Estado,
                                IdLinea = j.IdLineaProductoNavigation.IdLinea,
                                Nombre = j.IdLineaProductoNavigation.Nombre
                            },
                            NombreImplementador = j.NombreImplementador,
                            OtImplementacion = j.OtImplementacion,
                            Plan = j.IdPlanNavigation == null ? null : new PlanVm
                            {
                                Descripcion = j.IdPlanNavigation.Descripcion,
                                Estado = j.IdPlanNavigation.Estado,
                                IdPlan = j.IdPlanNavigation.IdPlan,
                                Nombre = j.IdPlanNavigation.Nombre
                            },
                            TelefonoImplementador = j.TelefonoImplementador,
                            EstadoTenant = j.EstadoNavigation == null ? null : new EmpresaEstadoVm
                            {
                                IdEstado = j.EstadoNavigation.IdEstado,
                                Nombre = j.EstadoNavigation.Nombre
                            },
                            NombreEmpresa = x.RazonSocial,
                            RutEmpresa = x.Rut,
                            EnroladoVPos = j.EnroladoVpos == null ? 0 : 1,
                            EnroladoTbk = j.EnroladoTbk == null ? 0 : 1,
                            FechaPasoProduccion = j.FechaPasoProduccion
                        }),
                        TipoCliente = x.TipoCliente,
                        TotalFilas = empresas.Count()

                    });
                }

                return Ok(result);

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerEmpresasPorPagina";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerImplementadores"), Authorize]
        public async Task<ActionResult> GetImplementadores()
        {
            try
            {
                List<ImplementadorVm> result = new List<ImplementadorVm>();
                var implementadores = _admin.Implementadors.ToList();

                result = implementadores.ConvertAll(x => new ImplementadorVm
                {
                    Estado = x.Estado,
                    IdImplementador = x.IdImplementador,
                    Nombre = x.Nombre,
                    Rut = x.Rut
                });

                return Ok(result);

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerImplementadores";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerAreasComerciales"), Authorize]
        public async Task<ActionResult> GetAreasComerciales()
        {
            try
            {
                List<AreaComercialVm> result = new List<AreaComercialVm>();
                var areas = _admin.AreaComercials.ToList();

                result = areas.ConvertAll(x => new AreaComercialVm
                {
                    Estado = x.Estado,
                    Nombre = x.Nombre,
                    IdArea = x.IdArea
                });

                return Ok(result);

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerAreasComerciales";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerLineasProducto"), Authorize]
        public async Task<ActionResult> GetLineasProductos()
        {
            try
            {
                List<LineaProductoVm> result = new List<LineaProductoVm>();
                var areas = _admin.LineaProductos.ToList();

                result = areas.ConvertAll(x => new LineaProductoVm
                {
                    Estado = x.Estado,
                    Nombre = x.Nombre,
                    IdLinea = x.IdLinea
                });

                return Ok(result);

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerLineasProducto";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerPlanes"), Authorize]
        public async Task<ActionResult> GetPlanes()
        {
            try
            {
                List<PlanVm> result = new List<PlanVm>();
                var planes = _admin.Planes.ToList();

                result = planes.ConvertAll(x => new PlanVm
                {
                    Estado = x.Estado,
                    Nombre = x.Nombre,
                    Descripcion = x.Descripcion,
                    IdPlan = x.IdPlan
                });

                return Ok(result);

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerPlanes";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CrearEmpresa"), Authorize]
        public async Task<ActionResult> CrearEmpresa(EmpresaVm empresa)
        {
            try
            {
                EmpresasPortal nuevaEmpresa = new EmpresasPortal
                {
                    CorreoImplementador = empresa.CorreoImplementador,
                    FechaInicioContrato = empresa.FechaInicioContrato,
                    FechaTerminoContrato = empresa.FechaTerminoContrato,
                    IdAreaComercial = empresa.IdAreaComercial,
                    IdEstado = 1,
                    IdImplementador = empresa.IdImplementador,
                    IdLineaProducto = empresa.IdLineaProducto,
                    IdPlan = empresa.IdPlan,
                    OtImplementacion = empresa.OtImplementacion,
                    RazonSocial = empresa.RazonSocial,
                    Rut = empresa.Rut,
                    TipoCliente = empresa.TipoCliente
                };

                _admin.EmpresasPortals.Add(nuevaEmpresa);
                _admin.SaveChanges();
                return Ok();

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/CrearEmpresa";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EditarEmpresa"), Authorize]
        public async Task<ActionResult> EditarEmpresa(EmpresaVm empresa)
        {
            try
            {
                var empresaEditar = _admin.EmpresasPortals.Where(x => x.IdEmpresa == empresa.IdEmpresa).FirstOrDefault();
                if (empresaEditar != null)
                {

                    empresaEditar.CorreoImplementador = empresa.CorreoImplementador;
                    empresaEditar.FechaInicioContrato = empresa.FechaInicioContrato;
                    empresaEditar.FechaTerminoContrato = empresa.FechaTerminoContrato;
                    empresaEditar.IdAreaComercial = empresa.IdAreaComercial;
                    empresaEditar.IdEstado = 1;
                    empresaEditar.IdImplementador = empresa.IdImplementador;
                    empresaEditar.IdLineaProducto = empresa.IdLineaProducto;
                    empresaEditar.IdPlan = empresa.IdPlan;
                    empresaEditar.OtImplementacion = empresa.OtImplementacion;
                    empresaEditar.RazonSocial = empresa.RazonSocial;
                    empresaEditar.Rut = empresa.Rut;
                    empresaEditar.TipoCliente = empresa.TipoCliente;


                    _admin.Entry(empresaEditar).State = EntityState.Modified;
                    await _admin.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/EditarEmpresa";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("EliminarEmpresa/{idEmpresa}"), Authorize]
        public async Task<ActionResult> EliminarEmpresa(int idEmpresa)
        {
            try
            {
                var empresa = _admin.EmpresasPortals.Where(x => x.IdEmpresa == idEmpresa).FirstOrDefault();
                if (empresa != null)
                {
                    _admin.EmpresasPortals.Remove(empresa);
                    await _admin.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/EliminarEmpresa";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validaConexionApi"), Authorize]
        public async Task<ActionResult> ValidaConexionApi(ApiSoftlandVm apiModel)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var retorno = await sf.validaConexionApiImplementacionAsync(apiModel);
                return Ok(retorno);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/validaConexionApi";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("validaConexionBaseDatos"), Authorize]
        public async Task<ActionResult> ValidaConexionBaseDatos(DatosImplementacionVm datosImplementacion)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var retorno = await sf.validaConexionBaseDatosAsync(datosImplementacion);
                return Ok(retorno);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/validaConexionBaseDatos";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("ObtenerCuentasContables"), Authorize]
        public async Task<ActionResult> ObtenerCuentasContables(ApiSoftlandVm apiModel)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var retorno = await sf.GetAllCuentasContablesImplementacionAsync(apiModel);
                return Ok(retorno);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerCuentasContables";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ObtenerTiposDocumento"), Authorize]
        public async Task<ActionResult> ObtenerTiposDocumento(ApiSoftlandVm apiModel)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var retorno = await sf.GetAllTiposDocumentosImplementacionAsync(apiModel);
                return Ok(retorno);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerTiposDocumento";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ObtenerCuentasContablesPasarelas"), Authorize]
        public async Task<ActionResult> ObtenerCuentasContablesPasarelas(ApiSoftlandVm apiModel)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                var retorno = await sf.GetAllCuentasContablesPasarela(apiModel);
                return Ok(retorno);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtenerCuentasContablesPasarelas";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ObtieneDatosTenant"), Authorize]
        public async Task<ActionResult> ObtieneDatosTenant(TenantVm tenant)
        {
            try
            {

                if (tenant.IdTenant == 0 || tenant.IdTenant == null)
                {

                    tenant.DatosImplementacion = new DatosImplementacionVm
                    {
                        ApiSoftland = new ApiSoftland
                        {
                            Url = "",
                            Token = "PX_tcNEzdLWFAbcfmg5ZWtx2faHgLOBymv2KjtGmhzGeqAXH5Si4t4l5WuURRpb4ZvzVCnIHBKR7RxgrtxpWhzbZGIIZUldYatX8nhQ4gqk1iP8h0h3BG3Zjfrp9D4kErxE_",
                            AreaDatos = "",
                            ConsultaTiposDeDocumentos = "CW/ConsultaTiposDeDocumentos",
                            ConsultaPlanDeCuentas = "CW/ConsultaPlanDeCuentas",
                            ConsultaRegiones = "CW/ConsultaRegiones",
                            ConsultaComunas = "CW/ConsultaComunas",
                            ConsultaGiros = "CW/ConsultaGiros",
                            ContactosXauxiliar = "CW/ContactosXAuxiliar?codaux={CODAUX}",
                            ConsultaCliente = "CW/ConsultaCliente?cantidad={CANTIDAD}&catCli={CATCLI}&codaux={CODAUX}&codLista={CODLISTA}&codVen={CODVEN}&conVta={CONVTA}&nombre={NOMBRE}&pagina={PAGINA}&rut={RUT}",
                            ActualizaCliente = "CW/ActualizaCliente?codaux={CODAUX}&region={REGION}&comaux={COMAUX}&fonaux1={FONAUX1}&diraux={DIRAUX}&dirnum={DIRNUM}&nomaux={NOMAUX}&giraux={GIRAUX}&esReceptorDTE={ESRECEPTORDTE}&eMailDte={EMAILDTE}",
                            ResumenContable = "CW/ResumenContable?codaux={CODAUX}&NoContabilizado={CONTABILIZADO}",
                            DocumentosFacturados = "IW/DocumentosFacturados?codaux={CODAUX}&ano={ANIO}&tipo={TIPO}&top={TOP}",
                            DetalleDocumento = "IW/DetalleDocumento?codaux={CODAUX}&folio={FOLIO}&subtipo={SUBTIPO}&tipo={TIPODOC}",
                            ObtenerPdfDocumento = "IW/ObtenerPdfDocumento?tipo={TIPO}&subtipo={SUBTIPO}&folio={FOLIO}",
                            DespachoDeDocumento = "IW/DespachoDeDocumento?tipo={TIPO}&folio={FOLIO}&codaux={CODAUX}",
                            ProductosComprados = "IW/ProductosComprados?codaux={CODAUX}",
                            PendientesPorFacturar = "NW/ObtieneNotasVentaPendientesPorFacturar?codaux={CODAUX}",
                            DetalleNotaDeVenta = "NW/DetalleNotaDeVenta?nvnumero={NVNUMERO}",
                            ObtenerPdf = "NW/ObtenerPdf?nvnumero={NVNUMERO}",
                            ObtieneGuiasPendientes = "IW/ObtieneGuiasPendientes?codaux={CODAUX}",
                            ObtieneVendedores = "IW/ObtieneVendedores",
                            ObtieneCondicionesVenta = "IW/ObtieneCondicionesVenta",
                            ObtieneListasPrecio = "IW/ObtieneListasPrecio",
                            ObtieneCategoriaClientes = "IW/ObtieneCategoriaClientes",
                            DocumentosContabilizados = "CW/DocumentosContabilizados?cantidad={CANTIDAD}&codaux={CODAUX}&diasxvencer={DIASPORVENCER}&Emisiondesde={EMISIONDESDE}&emisionhasta={EMISIONHASTA}&estado={ESTADO}&folio={FOLIO}&listacuentas={LISTACUENTAS}&listadocumentos={LISTADOCUMENTOS}&pagina={PAGINA}&rutaux={RUTAUX}&solosaldo={SOLOSALDO}&Vctodesde={FECHAVENCIMIENTODESDE}&Vctohasta={FECHAVENCIMIENTOHASTA}&listacategorias={LISTACAGETORIAS}&listacondicionventa={LISTACONDICIONVENTA}&listavendedores={LISTAVENDEDORES}",
                            ObtieneModulos = "IW/ObtieneModulos",
                            ConsultaMonedas = "CW/ConsultaMonedas",
                            ContabilizaPagos = "CW/CWContabilizaPagos",
                            ConsultaCargos = "CW/ObtenerCargos",
                            DocumentosContabilizadosResumen = "CW/DocumentosContabilizadosResumen?codaux={CODAUX}&diasxvencer={DIASXVENCER}&listacuentas={LISTACUENTAS}&listadocumentos={LISTADOCUMENTOS}",
                            TopDeudores = "CW/TopDeudores?CantidadTope={CANTIDADTOPE}&desde={DESDE}&listaCuentas={LISTACUENTAS}&listaTipodocumentos={LISTATIPODOCUMENTOS}",
                            TransbankRegistrarCliente = "VW/VWTransbankRegistrarCliente",
                            DocContabilizadosResumenxRut = "CW/DocContabilizadosResumenxRut?cantidad={CANTIDAD}&codaux={CODAUX}&desde={DESDE}&diasxvencer={DIASXVENCER}&Emisiondesde={EMISIONDESDE}&emisionhasta={EMISIONHASTA}&estado={ESTADO}&folio={FOLIO}&listacuentas={LISTACUENTAS}&listadocumentos={LISTADOCUMENTOS}&moneda={MONEDA}&pagina={PAGINA}&rutaux={RUTAUX}&solosaldo={SOLOSALDO}&Vctodesde={FECHAVENCIMIENTODESDE}&Vctohasta={FECHAVENCIMIENTOHASTA}&listacategorias={LISTACAGETORIAS}&listacondicionventa={LISTACONDICIONVENTA}&listavendedores={LISTAVENDEDORES}",
                            PagosxDocumento = "CW/PagosxDocumento?cantidad={CANTIDAD}&codaux={CODAUX}&documento={DOCUMENTO}&folio={FOLIO}&pagina={PAGINA}",
                            HabilitaLogApi = 0,
                            CadenaAlmacenamientoAzure = "DefaultEndpointsProtocol=https;AccountName=sofcluesstaportalcliente;AccountKey=5nPtQyN7+Frvk5s6YKfZUlpcTLWKGsbDnD2/gMTOgkvKGGM89Lig0G9jysGjA0lHz3Vzn1KtRxQk+AStrdB0VA==;EndpointSuffix=core.windows.net",
                            UrlAlmacenamientoArchivos = "https://sofcluesstaportalcliente.blob.core.windows.net/" + tenant.RutEmpresa.Replace(".", "").Replace("-", "") + "/",
                            CuentasPasarelaPagos = "CW/CuentasPasarelaPagos",
                            ReintentosCallback = 5,
                            ReintentosRedirect = 7,
                            MiliSegundosReintentoRedirect = 1500,
                            MilisegundosReintoCalback = 1500
                        },
                        ConfiguracionCorreo = new ConfiguracionCorreo
                        {
                            SmtpServer = "smtp.sendgrid.net",
                            Usuario = "apikey",
                            Clave = Encrypt.Base64Decode("U0cuS2w2UDBRblpSLUdfWURlNGJVb3M1QS5lV21SZnlqNko3TDgwbjRHanpiSUI4SGZIeVp1b3pVdlNSSWRPd2VzUkRZ"),
                            Puerto = 587,
                            Ssl = 1,
                            AsuntoPagoCliente = "Comprobante de Pago",
                            AsuntoAccesoCliente = "Acceso",
                            AsuntoEnvioDocumentos = "Documentos de compra",
                            TextoEnvioDocumentos = "Estimado Cliente, adjunto encontrara los documentos solicitados.",
                            CantidadCorreosAcceso = 100,
                            CantidadCorreosNotificacion = 100,
                            TextoMensajeActivacion = "Estimado cliente, a continuación, encontrará las credenciales para poder realizar la activación de tu cuenta en nuestro portal, una vez activada, deberas ingresar con el Rut de la empresa, correo y clave registrados.",
                            TituloPagoCliente = "Pago de Documentos",
                            TituloAccesoCliente = "Activación de Cuenta",
                            TituloEnvioDocumentos = "Documentos de compra",
                            TituloCambioDatos = "Actualizacion de datos",
                            TituloCambioClave = "Actualizacion Clave",
                            TituloRecuperarClave = "Recuperar Contraseña",
                            AsuntoCambioDatos = "Actualización de datos",
                            AsuntoCambioClave = "Actualización clave",
                            AsuntoRecuperarClave = "Recuperar Contraseña",
                            TextoPagoCliente = "Adjunto encontrara el detalle del pago realizado.",
                            TextoAccesoCliente = "Se ha generado una cuenta con los siguientes datos:",
                            TextoCambioDatos = "Estimado usuario, los datos de empresa han sido actualizados.",
                            TextoCambioClave = "Estimado se ha realizado un cambio de clave la cual se entrega a cotinuación",
                            TextoRecuperarClave = "Se realizo una recuperacion de contraseña, debera ingresar con la nueva contraseña que se indica a continuación",
                            TituloAvisoPagoCliente = "Notificación Pago Cliente",
                            AsuntoAvisoPagoCliente = "Pago Cliente",
                            TextoAvisoPagoCliente = "Estimado, Se ha realizado el siguiente pago: ",
                            ColorBoton = "#263db5",
                            TituloPagoSinComprobante = "Problema Generación Comprobante",
                            AsuntoPagoSinComprobante = "Problema Generación Comprobante",
                            AsuntoCambioCorreo = "Cambio Correo",
                            TituloCambioCorreo = "Cambio Correo",
                            TextoCambioCorreo = "Se realizo un cambio de correo para su cuenta.",
                            TituloCobranza = "Cobranza",
                            TextoCobranza = "Estimado cliente le recordamos que existen documentos vencidos, favor regularizar.",
                            AsuntoCobranza = "Cobranza",
                            TituloPreCobranza = "Recordatorio",
                            AsuntoPreCobranza = "Recordatorio",
                            TextoPreCobranza = "Estimado Cliente, le informamos que tiene documentos próximos a vencer.",
                            TituloEstadoCuenta = "Estado de cuenta",
                            AsuntoEstadoCuenta = "Estado de cuenta",
                            TextoEstadoCuenta = "Estimado Cliente, en el presente correo podra encontrar información sobre su estado de cuenta.",
                            TextoPagoSinComprobante = "Se ha realizado un pago a través del portal pero no fue posible generar el comprobante contable, a continuación encontrará los datos del cliente para validar y reprocesar o generar el comprobante.",
                            LogoCorreo = "https://sofcluesstaportalcliente.blob.core.windows.net/imagendefecto/LogoCorreo.png",
                            CorreoOrigen = "no-responder@softlandcloud.cl"
                        },
                        ConfiguracionPortal = new ConfiguracionPortal
                        {
                            CantidadUltimasCompras = 10,
                            HabilitaPagoRapido = 1,
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
                            UtilizaNumeroDireccion = 1
                        },
                        ConfiguracionPagoCliente = new ConfiguracionPagoCliente
                        {
                            AnioTributario = 2000,
                            DiasPorVencer = 10,
                            GlosaComprobante = "CONTABILIZACION ",
                            MonedaUtilizada = "01"
                        },
                        ConfiguracionDiseno = new ConfiguracionDiseno
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
                        },
                        ConfiguracionEmpresa = new ConfiguracionEmpresa
                        {
                            NombreEmpresa = tenant.NombreEmpresa,
                            RutEmpresa = tenant.RutEmpresa
                        },
                        AmbienteTransbank = 0,
                        ApiKeyTransbank = "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C",
                        CodigoComercioTransbank = "597055555532",
                        UtilizaTransbank = 0,
                        UtilizaVirtualPos = 0,
                    };
                }
                else
                {
                    var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                    var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                    var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                    var configuracionEmpresa = newContextPortal.ConfiguracionEmpresas.FirstOrDefault();
                    var configuracionCorreo = newContextPortal.ConfiguracionCorreos.FirstOrDefault();
                    var configuracionPagoCliente = newContextPortal.ConfiguracionPagoClientes.FirstOrDefault();
                    var configuracionDiseno = newContextPortal.ConfiguracionDisenos.FirstOrDefault();
                    var apiSoftland = newContextPortal.ApiSoftlands.FirstOrDefault();
                    var configuracionPortal = newContextPortal.ConfiguracionPortals.FirstOrDefault();
                    var transbank = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 1).FirstOrDefault();
                    var virtualPos = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 5).FirstOrDefault();

                    if (!string.IsNullOrEmpty(configuracionCorreo.Clave))
                    {
                        configuracionCorreo.Clave = Encrypt.Base64Decode(configuracionCorreo.Clave);
                    }

                    tenant.DatosImplementacion = new DatosImplementacionVm
                    {
                        ApiSoftland = apiSoftland,
                        ConfiguracionCorreo = configuracionCorreo,
                        ConfiguracionDiseno = configuracionDiseno,
                        ConfiguracionEmpresa = configuracionEmpresa,
                        ConfiguracionPagoCliente = configuracionPagoCliente,
                        ConfiguracionPortal = configuracionPortal,
                        ServidorPortal = builder.DataSource,
                        BaseDatosPortal = builder.InitialCatalog,
                        UsuarioBaseDatosPortal = builder.UserID,
                        ClaveBaseDatosPortal = builder.Password,
                        AmbienteTransbank = (int)(transbank.EsProduccion == null ? 0 : transbank.EsProduccion),
                        CodigoComercioTransbank = transbank.CodigoComercio,
                        ApiKeyTransbank = transbank.SecretKey,
                        CuentaContableTransbank = transbank.CuentaContable,
                        CuentaContableVirtualPos = virtualPos.CuentaContable,
                        DocumentoContableTransbank = transbank.TipoDocumento,
                        DocumentoContableVirtualPos = virtualPos.TipoDocumento,
                        UtilizaTransbank = (int)transbank.Estado,
                        UtilizaVirtualPos = (int)virtualPos.Estado
                    };

                }


                return Ok(tenant);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/ObtieneDatosTenant";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("CreaActualizaTenant"), Authorize]
        public async Task<ActionResult> CreaActualizaTenant([FromForm] TenantVm tenant)
        {
            try
            {
                SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
                tenant.ConnectionString = "Data Source=" + tenant.DatosImplementacion.ServidorPortal + ";Initial Catalog=" + tenant.DatosImplementacion.BaseDatosPortal + ";" +
                                    "user id=" + tenant.DatosImplementacion.UsuarioBaseDatosPortal + ";password=" + tenant.DatosImplementacion.ClaveBaseDatosPortal + ";Encrypt=False;";
                var existenTablas = sf.TableExists(tenant.ConnectionString);
                if (tenant.IdTenant == 0 || tenant.IdTenant == null || !existenTablas)
                {
                    var empresa = _admin.EmpresasPortals.Where(x => x.Rut == tenant.RutEmpresa).FirstOrDefault();

                    if (tenant.IdTenant == null || tenant.IdTenant == 0)
                    {
                       

                        Dal.Models_Admin.Tenant newTenant = new Dal.Models_Admin.Tenant
                        {
                            ConnectionString = tenant.ConnectionString,
                            CorreoImplementador = tenant.CorreoImplementador,
                            Dominio = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host,
                            Estado = 2,
                            FechaInicioContrato = tenant.FechaInicioContrato,
                            FechaInicioImplementacion = tenant.FechaInicioImplementacion,
                            FechaTerminoContrato = tenant.FechaTerminoContrato,
                            FechaTerminoImplementacion = tenant.FechaTerminoImplementacion,
                            IdAreaComercial = tenant.IdAreaComercial,
                            IdEmpresa = empresa.IdEmpresa,
                            Identifier = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host,
                            IdImplementador = tenant.IdImplementador,
                            IdLineaProducto = tenant.IdLineaProducto,
                            IdPlan = tenant.IdPlan,
                            NombreImplementador = tenant.NombreImplementador,
                            OtImplementacion = tenant.OtImplementacion,
                            TelefonoImplementador = tenant.TelefonoImplementador,
                            EnroladoTbk = 0,
                            EnroladoVpos = 0,
                            FechaPasoProduccion = null
                        };
                        _admin.Tenants.Add(newTenant);
                        await _admin.SaveChangesAsync();
                    }
                    else
                    {
                        var newTenant = _admin.Tenants.Where(x => x.IdTenant == tenant.IdTenant).FirstOrDefault();


                        newTenant.ConnectionString = tenant.ConnectionString;
                        newTenant.CorreoImplementador = tenant.CorreoImplementador;
                        newTenant.Dominio = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host;
                        newTenant.Estado = tenant.Estado;
                        newTenant.FechaInicioContrato = tenant.FechaInicioContrato;
                        newTenant.FechaInicioImplementacion = tenant.FechaInicioImplementacion;
                        newTenant.FechaTerminoContrato = tenant.FechaTerminoContrato;
                        newTenant.FechaTerminoImplementacion = tenant.FechaTerminoImplementacion;
                        newTenant.IdAreaComercial = tenant.IdAreaComercial;
                        newTenant.IdEmpresa = tenant.IdEmpresa;
                        newTenant.Identifier = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host;
                        newTenant.IdImplementador = tenant.IdImplementador;
                        newTenant.IdLineaProducto = tenant.IdLineaProducto;
                        newTenant.IdPlan = tenant.IdPlan;
                        newTenant.NombreImplementador = tenant.NombreImplementador;
                        newTenant.OtImplementacion = tenant.OtImplementacion;
                        newTenant.TelefonoImplementador = tenant.TelefonoImplementador;
                        newTenant.IdTenant = (int)tenant.IdTenant;
                        newTenant.FechaPasoProduccion = tenant.FechaPasoProduccion;
                        newTenant.EnroladoTbk = tenant.EnroladoTbk;
                        newTenant.EnroladoVpos = tenant.EnroladoVPos;
                        _admin.Entry(newTenant);
                        await _admin.SaveChangesAsync();
                    }




                    string connectionString = tenant.DatosImplementacion.ApiSoftland.CadenaAlmacenamientoAzure;
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    //Validamos si contenedor para empresa existe
                    string containerName = tenant.RutEmpresa.Replace(".", "").Replace("-", "");
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    bool exists = await containerClient.ExistsAsync();
                    if (!exists)
                    {
                        //Contenedor no existe, debemos crearlo con acceso publico
                        containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                    }


                    //Cambiamos nombre archivo
                    Utils util = new Utils();

                    if (tenant.LogoPortada != null)
                    {
                        string LogoPortada = util.nombreArchivo(tenant.LogoPortada.FileName, 1);
                        using (var stream = System.IO.File.Create(LogoPortada))
                        {
                            tenant.LogoPortada.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoPortada);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoPortada = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoPortada;
                        }
                    }

                    if (tenant.ImagenPortada != null)
                    {
                        string ImagenPortada = util.nombreArchivo(tenant.ImagenPortada.FileName, 2);
                        using (var stream = System.IO.File.Create(ImagenPortada))
                        {
                            tenant.ImagenPortada.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenPortada);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenPortada = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenPortada;
                        }
                    }

                    if (tenant.LogoSidebar != null)
                    {
                        string LogoSidebar = util.nombreArchivo(tenant.LogoSidebar.FileName, 3);
                        using (var stream = System.IO.File.Create(LogoSidebar))
                        {
                            tenant.LogoSidebar.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoSidebar);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoSidebar = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoSidebar;
                        }
                    }

                    if (tenant.LogoMinimalistaSidebar != null)
                    {
                        string LogoMinimalistaSidebar = util.nombreArchivo(tenant.LogoMinimalistaSidebar.FileName, 4);
                        using (var stream = System.IO.File.Create(LogoMinimalistaSidebar))
                        {
                            tenant.LogoMinimalistaSidebar.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoMinimalistaSidebar);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoMinimalistaSidebar = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoMinimalistaSidebar;
                        }
                    }

                    if (tenant.BannerPagoRapido != null)
                    {
                        string BannerPagoRapido = util.nombreArchivo(tenant.BannerPagoRapido.FileName, 5);
                        using (var stream = System.IO.File.Create(BannerPagoRapido))
                        {
                            tenant.BannerPagoRapido.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerPagoRapido);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerPagoRapido = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerPagoRapido;
                        }
                    }

                    if (tenant.ImagenUltimasCompras != null)
                    {
                        string ImagenUltimasCompras = util.nombreArchivo(tenant.ImagenUltimasCompras.FileName, 6);
                        using (var stream = System.IO.File.Create(ImagenUltimasCompras))
                        {
                            tenant.ImagenUltimasCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenUltimasCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUltimasCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenUltimasCompras;
                        }
                    }

                    if (tenant.IconoMisCompras != null)
                    {
                        string IconoMisCompras = util.nombreArchivo(tenant.IconoMisCompras.FileName, 7);
                        using (var stream = System.IO.File.Create(IconoMisCompras))
                        {
                            tenant.IconoMisCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoMisCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoMisCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoMisCompras;
                        }
                    }

                    if (tenant.BannerMisCompras != null)
                    {
                        string BannerMisCompras = util.nombreArchivo(tenant.BannerMisCompras.FileName, 8);
                        using (var stream = System.IO.File.Create(BannerMisCompras))
                        {
                            tenant.BannerMisCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerMisCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerMisCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerMisCompras;
                        }
                    }

                    if (tenant.ImagenUsuario != null)
                    {
                        string ImagenUsuario = util.nombreArchivo(tenant.ImagenUsuario.FileName, 9);
                        using (var stream = System.IO.File.Create(ImagenUsuario))
                        {
                            tenant.ImagenUsuario.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenUsuario);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUsuario = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenUsuario;
                        }
                    }

                    if (tenant.BannerPortal != null)
                    {
                        string BannerPortal = util.nombreArchivo(tenant.BannerPortal.FileName, 10);
                        using (var stream = System.IO.File.Create(BannerPortal))
                        {
                            tenant.BannerPortal.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerPortal);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerPortal = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerPortal;
                        }
                    }

                    if (tenant.IconoContactos != null)
                    {
                        string IconoContactos = util.nombreArchivo(tenant.IconoContactos.FileName, 11);
                        using (var stream = System.IO.File.Create(IconoContactos))
                        {
                            tenant.IconoContactos.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoContactos);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoContactos = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoContactos;
                        }
                    }

                    if (tenant.IconoClavePerfil != null)
                    {
                        string IconoClavePerfil = util.nombreArchivo(tenant.IconoClavePerfil.FileName, 12);
                        using (var stream = System.IO.File.Create(IconoClavePerfil))
                        {
                            tenant.IconoClavePerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoClavePerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoClavePerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoClavePerfil;
                        }
                    }

                    if (tenant.IconoEditarPerfil != null)
                    {
                        string IconoEditarPerfil = util.nombreArchivo(tenant.IconoEditarPerfil.FileName, 13);
                        using (var stream = System.IO.File.Create(IconoEditarPerfil))
                        {
                            tenant.IconoEditarPerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoEditarPerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoEditarPerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoEditarPerfil;
                        }
                    }

                    if (tenant.IconoEstadoPerfil != null)
                    {
                        string IconoEstadoPerfil = util.nombreArchivo(tenant.IconoEstadoPerfil.FileName, 14);
                        using (var stream = System.IO.File.Create(IconoEstadoPerfil))
                        {
                            tenant.IconoEstadoPerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoEstadoPerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoEstadoPerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoEstadoPerfil;
                        }
                    }

                    if (tenant.LogoCorreo != null)
                    {
                        string LogoCorreo = "LogoCorreo." + tenant.LogoCorreo.FileName.Split('.')[1];
                        using (var stream = System.IO.File.Create(LogoCorreo))
                        {
                            tenant.LogoCorreo.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoCorreo);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionCorreo.LogoCorreo = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoCorreo;
                        }
                    }




                    sf.CrearTablas(tenant.ConnectionString);

                    var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                    var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                    var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                    var transbank = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 1).FirstOrDefault();
                    var virtualPos = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 5).FirstOrDefault();

                    if (!string.IsNullOrEmpty(tenant.DatosImplementacion.ConfiguracionCorreo.Clave))
                    {
                        tenant.DatosImplementacion.ConfiguracionCorreo.Clave = Encrypt.Base64Encode(tenant.DatosImplementacion.ConfiguracionCorreo.Clave);
                    }

                    newContextPortal.ConfiguracionEmpresas.Add(tenant.DatosImplementacion.ConfiguracionEmpresa);
                    newContextPortal.ConfiguracionCorreos.Add(tenant.DatosImplementacion.ConfiguracionCorreo);
                    newContextPortal.ConfiguracionPagoClientes.Add(tenant.DatosImplementacion.ConfiguracionPagoCliente);
                    newContextPortal.ConfiguracionDisenos.Add(tenant.DatosImplementacion.ConfiguracionDiseno);
                    newContextPortal.ApiSoftlands.Add(tenant.DatosImplementacion.ApiSoftland);
                    tenant.DatosImplementacion.ConfiguracionPortal.EstadoImplementacion = 2;
                    newContextPortal.ConfiguracionPortals.Add(tenant.DatosImplementacion.ConfiguracionPortal);

                    transbank.Ambiente = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWTransbankGenerarPago";
                    transbank.TipoDocumento = tenant.DatosImplementacion.DocumentoContableTransbank;
                    transbank.AmbienteConsultarPago = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWTransbankObtenerEstadoPago?id_transaccion={ID}&esProductivo={ESPRODUCTIVO}";
                    transbank.CodigoComercio = tenant.DatosImplementacion.CodigoComercioTransbank;
                    transbank.CuentaContable = tenant.DatosImplementacion.CuentaContableTransbank;
                    transbank.Estado = tenant.DatosImplementacion.UtilizaTransbank;
                    transbank.EsProduccion = tenant.DatosImplementacion.AmbienteTransbank;
                    transbank.SecretKey = tenant.DatosImplementacion.ApiKeyTransbank;


                    virtualPos.Ambiente = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VirtualPosGenerarPago?url_redireccion={REDIRECCION}&url_callback={CALLBACK}&id_interno={IDINTERNO}&monto_total={TOTAL}&monto_bruto={BRUTO}&rutCliente={RUT}&tipo={TIPO}&monto_impuestos={IMPUESTO}&nombre_cliente={NOMBRE}&apellido_cliente={APELLIDO}&correo_cliente={CORREO}&esProductivo={ESPRODUCTIVO}";
                    virtualPos.TipoDocumento = tenant.DatosImplementacion.DocumentoContableVirtualPos;
                    virtualPos.AmbienteConsultarPago = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWVirtualPosObtenerEstadoPago?id_transaccion={ID}&esProductivo={ESPRODUCTIVO}";
                    virtualPos.CuentaContable = tenant.DatosImplementacion.CuentaContableVirtualPos;
                    virtualPos.Estado = tenant.DatosImplementacion.UtilizaVirtualPos;

                    newContextPortal.Entry(transbank);
                    newContextPortal.Entry(virtualPos);

                    Parametro urlPagoFront = new Parametro
                    {
                        Nombre = "UrlPagoFront",
                        Estado = 1,
                        Descripcion = "Url para redirección de pago",
                        Valor = tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal
                    };

                    Parametro urlPagoPortal = new Parametro
                    {
                        Nombre = "UrlPagoPortal",
                        Estado = 1,
                        Descripcion = "Url para redirección de pago",
                        Valor = tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal
                    };

                    Parametro urlPagoRapido = new Parametro
                    {
                        Nombre = "UrlPagoRapido",
                        Estado = 1,
                        Descripcion = "Url para redirección de pago",
                        Valor = tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal
                    };

                    Parametro horasToken = new Parametro
                    {
                        Nombre = "HorasToken",
                        Estado = 1,
                        Descripcion = "Cantidad de horas validas para token",
                        Valor = "9"
                    };

                    newContextPortal.Parametros.Add(urlPagoFront);
                    newContextPortal.Parametros.Add(urlPagoPortal);
                    newContextPortal.Parametros.Add(urlPagoRapido);
                    newContextPortal.Parametros.Add(horasToken);

                    Usuario user = new Usuario
                    {
                        Activo = 1,
                        Apellidos = "Implementador",
                        CuentaActivada = 1,
                        Email = "jcordova@intgra.cl",
                        IdPerfil = 1,
                        Nombres = "Usuario",
                        Password = "fEqNCco3Yq9h5ZUglD3CZJT4lBs="
                    };
                    newContextPortal.Usuarios.Add(user);



                    await newContextPortal.SaveChangesAsync();


                }
                else
                {
                    var newTenant = _admin.Tenants.Where(x => x.IdTenant == tenant.IdTenant).FirstOrDefault();


                    newTenant.ConnectionString = tenant.ConnectionString;
                    newTenant.CorreoImplementador = tenant.CorreoImplementador;
                    newTenant.Dominio = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host;
                    newTenant.Estado = tenant.Estado;
                    newTenant.FechaInicioContrato = tenant.FechaInicioContrato;
                    newTenant.FechaInicioImplementacion = tenant.FechaInicioImplementacion;
                    newTenant.FechaTerminoContrato = tenant.FechaTerminoContrato;
                    newTenant.FechaTerminoImplementacion = tenant.FechaTerminoImplementacion;
                    newTenant.IdAreaComercial = tenant.IdAreaComercial;
                    newTenant.IdEmpresa = tenant.IdEmpresa;
                    newTenant.Identifier = new Uri(tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal).Host;
                    newTenant.IdImplementador = tenant.IdImplementador;
                    newTenant.IdLineaProducto = tenant.IdLineaProducto;
                    newTenant.IdPlan = tenant.IdPlan;
                    newTenant.NombreImplementador = tenant.NombreImplementador;
                    newTenant.OtImplementacion = tenant.OtImplementacion;
                    newTenant.TelefonoImplementador = tenant.TelefonoImplementador;
                    newTenant.IdTenant = (int)tenant.IdTenant;
                    newTenant.FechaPasoProduccion = tenant.FechaPasoProduccion;
                    newTenant.EnroladoTbk = tenant.EnroladoTbk;
                    newTenant.EnroladoVpos = tenant.EnroladoVPos;
                    _admin.Entry(newTenant);
                    await _admin.SaveChangesAsync();

                    string connectionString = tenant.DatosImplementacion.ApiSoftland.CadenaAlmacenamientoAzure;
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    //Validamos si contenedor para empresa existe
                    string containerName = tenant.RutEmpresa.Replace(".", "").Replace("-", "");
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    bool exists = await containerClient.ExistsAsync();
                    if (!exists)
                    {
                        //Contenedor no existe, debemos crearlo con acceso publico
                        containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                    }


                    //Cambiamos nombre archivo
                    Utils util = new Utils();

                    if (tenant.LogoPortada != null)
                    {
                        string LogoPortada = util.nombreArchivo(tenant.LogoPortada.FileName, 1);
                        using (var stream = System.IO.File.Create(LogoPortada))
                        {
                            tenant.LogoPortada.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoPortada);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoPortada = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoPortada;
                        }
                    }

                    if (tenant.ImagenPortada != null)
                    {
                        string ImagenPortada = util.nombreArchivo(tenant.ImagenPortada.FileName, 2);
                        using (var stream = System.IO.File.Create(ImagenPortada))
                        {
                            tenant.ImagenPortada.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenPortada);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenPortada = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenPortada;
                        }
                    }

                    if (tenant.LogoSidebar != null)
                    {
                        string LogoSidebar = util.nombreArchivo(tenant.LogoSidebar.FileName, 3);
                        using (var stream = System.IO.File.Create(LogoSidebar))
                        {
                            tenant.LogoSidebar.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoSidebar);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoSidebar = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoSidebar;
                        }
                    }

                    if (tenant.LogoMinimalistaSidebar != null)
                    {
                        string LogoMinimalistaSidebar = util.nombreArchivo(tenant.LogoMinimalistaSidebar.FileName, 4);
                        using (var stream = System.IO.File.Create(LogoMinimalistaSidebar))
                        {
                            tenant.LogoMinimalistaSidebar.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoMinimalistaSidebar);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.LogoMinimalistaSidebar = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoMinimalistaSidebar;
                        }
                    }

                    if (tenant.BannerPagoRapido != null)
                    {
                        string BannerPagoRapido = util.nombreArchivo(tenant.BannerPagoRapido.FileName, 5);
                        using (var stream = System.IO.File.Create(BannerPagoRapido))
                        {
                            tenant.BannerPagoRapido.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerPagoRapido);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerPagoRapido = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerPagoRapido;
                        }
                    }

                    if (tenant.ImagenUltimasCompras != null)
                    {
                        string ImagenUltimasCompras = util.nombreArchivo(tenant.ImagenUltimasCompras.FileName, 6);
                        using (var stream = System.IO.File.Create(ImagenUltimasCompras))
                        {
                            tenant.ImagenUltimasCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenUltimasCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUltimasCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenUltimasCompras;
                        }
                    }

                    if (tenant.IconoMisCompras != null)
                    {
                        string IconoMisCompras = util.nombreArchivo(tenant.IconoMisCompras.FileName, 7);
                        using (var stream = System.IO.File.Create(IconoMisCompras))
                        {
                            tenant.IconoMisCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoMisCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoMisCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoMisCompras;
                        }
                    }

                    if (tenant.BannerMisCompras != null)
                    {
                        string BannerMisCompras = util.nombreArchivo(tenant.BannerMisCompras.FileName, 8);
                        using (var stream = System.IO.File.Create(BannerMisCompras))
                        {
                            tenant.BannerMisCompras.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerMisCompras);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerMisCompras = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerMisCompras;
                        }
                    }

                    if (tenant.ImagenUsuario != null)
                    {
                        string ImagenUsuario = util.nombreArchivo(tenant.ImagenUsuario.FileName, 9);
                        using (var stream = System.IO.File.Create(ImagenUsuario))
                        {
                            tenant.ImagenUsuario.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(ImagenUsuario);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUsuario = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + ImagenUsuario;
                        }
                    }

                    if (tenant.BannerPortal != null)
                    {
                        string BannerPortal = util.nombreArchivo(tenant.BannerPortal.FileName, 10);
                        using (var stream = System.IO.File.Create(BannerPortal))
                        {
                            tenant.BannerPortal.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(BannerPortal);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.BannerPortal = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + BannerPortal;
                        }
                    }

                    if (tenant.IconoContactos != null)
                    {
                        string IconoContactos = util.nombreArchivo(tenant.IconoContactos.FileName, 11);
                        using (var stream = System.IO.File.Create(IconoContactos))
                        {
                            tenant.IconoContactos.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoContactos);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoContactos = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoContactos;
                        }
                    }

                    if (tenant.IconoClavePerfil != null)
                    {
                        string IconoClavePerfil = util.nombreArchivo(tenant.IconoClavePerfil.FileName, 12);
                        using (var stream = System.IO.File.Create(IconoClavePerfil))
                        {
                            tenant.IconoClavePerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoClavePerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoClavePerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoClavePerfil;
                        }
                    }

                    if (tenant.IconoEditarPerfil != null)
                    {
                        string IconoEditarPerfil = util.nombreArchivo(tenant.IconoEditarPerfil.FileName, 13);
                        using (var stream = System.IO.File.Create(IconoEditarPerfil))
                        {
                            tenant.IconoEditarPerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoEditarPerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoEditarPerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoEditarPerfil;
                        }
                    }

                    if (tenant.IconoEstadoPerfil != null)
                    {
                        string IconoEstadoPerfil = util.nombreArchivo(tenant.IconoEstadoPerfil.FileName, 14);
                        using (var stream = System.IO.File.Create(IconoEstadoPerfil))
                        {
                            tenant.IconoEstadoPerfil.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(IconoEstadoPerfil);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionDiseno.IconoEstadoPerfil = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + IconoEstadoPerfil;
                        }
                    }

                    if (tenant.LogoCorreo != null)
                    {
                        string LogoCorreo = "LogoCorreo." + tenant.LogoCorreo.FileName.Split('.')[1];
                        using (var stream = System.IO.File.Create(LogoCorreo))
                        {
                            tenant.LogoCorreo.CopyTo(stream);
                            BlobClient blobClient = containerClient.GetBlobClient(LogoCorreo);
                            stream.Position = 0;
                            await blobClient.UploadAsync(stream, overwrite: true);
                            tenant.DatosImplementacion.ConfiguracionCorreo.LogoCorreo = tenant.DatosImplementacion.ApiSoftland.UrlAlmacenamientoArchivos + LogoCorreo;
                        }
                    }

                    var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                    var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                    var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                    var configuracionEmpresa = newContextPortal.ConfiguracionEmpresas.FirstOrDefault();
                    var configuracionCorreo = newContextPortal.ConfiguracionCorreos.FirstOrDefault();
                    var configuracionPagoCliente = newContextPortal.ConfiguracionPagoClientes.FirstOrDefault();
                    var configuracionDiseno = newContextPortal.ConfiguracionDisenos.FirstOrDefault();
                    var apiSoftland = newContextPortal.ApiSoftlands.FirstOrDefault();
                    var configuracionPortal = newContextPortal.ConfiguracionPortals.FirstOrDefault();
                    var transbank = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 1).FirstOrDefault();
                    var virtualPos = newContextPortal.PasarelaPagos.Where(x => x.IdPasarela == 5).FirstOrDefault();




                    configuracionEmpresa.UrlPortal = tenant.DatosImplementacion.ConfiguracionEmpresa.UrlPortal;

                    configuracionCorreo.AsuntoAccesoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoAccesoCliente;
                    configuracionCorreo.AsuntoAvisoPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoAvisoPagoCliente;
                    configuracionCorreo.AsuntoCambioClave = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoCambioClave;
                    configuracionCorreo.AsuntoCambioCorreo = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoCambioCorreo;
                    configuracionCorreo.AsuntoCambioDatos = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoCambioDatos;
                    configuracionCorreo.AsuntoCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoCobranza;
                    configuracionCorreo.AsuntoEnvioDocumentos = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoEnvioDocumentos;
                    configuracionCorreo.AsuntoEstadoCuenta = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoEstadoCuenta;
                    configuracionCorreo.AsuntoPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoPagoCliente;
                    configuracionCorreo.AsuntoPagoSinComprobante = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoPagoSinComprobante;
                    configuracionCorreo.AsuntoPreCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoPreCobranza;
                    configuracionCorreo.AsuntoRecuperarClave = tenant.DatosImplementacion.ConfiguracionCorreo.AsuntoRecuperarClave;

                    if (!string.IsNullOrEmpty(tenant.DatosImplementacion.ConfiguracionCorreo.Clave))
                    {
                        tenant.DatosImplementacion.ConfiguracionCorreo.Clave = Encrypt.Base64Encode(tenant.DatosImplementacion.ConfiguracionCorreo.Clave);
                    }

                    configuracionCorreo.Clave = tenant.DatosImplementacion.ConfiguracionCorreo.Clave;
                    configuracionCorreo.ColorBoton = tenant.DatosImplementacion.ConfiguracionCorreo.ColorBoton;
                    configuracionCorreo.CorreoAvisoPago = tenant.DatosImplementacion.ConfiguracionCorreo.CorreoAvisoPago;
                    configuracionCorreo.CorreoOrigen = tenant.DatosImplementacion.ConfiguracionCorreo.CorreoOrigen;
                    configuracionCorreo.LogoCorreo = tenant.DatosImplementacion.ConfiguracionCorreo.LogoCorreo;
                    configuracionCorreo.NombreCorreos = tenant.DatosImplementacion.ConfiguracionCorreo.NombreCorreos;
                    configuracionCorreo.Puerto = tenant.DatosImplementacion.ConfiguracionCorreo.Puerto;
                    configuracionCorreo.SmtpServer = tenant.DatosImplementacion.ConfiguracionCorreo.SmtpServer;
                    configuracionCorreo.Ssl = tenant.DatosImplementacion.ConfiguracionCorreo.Ssl;
                    configuracionCorreo.TextoAccesoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TextoAccesoCliente;
                    configuracionCorreo.TextoAvisoPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TextoAvisoPagoCliente;
                    configuracionCorreo.TextoCambioClave = tenant.DatosImplementacion.ConfiguracionCorreo.TextoCambioClave;
                    configuracionCorreo.TextoCambioCorreo = tenant.DatosImplementacion.ConfiguracionCorreo.TextoCambioCorreo;
                    configuracionCorreo.TextoCambioDatos = tenant.DatosImplementacion.ConfiguracionCorreo.TextoCambioDatos;
                    configuracionCorreo.TextoCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.TextoCobranza;
                    configuracionCorreo.TextoEnvioDocumentos = tenant.DatosImplementacion.ConfiguracionCorreo.TextoEnvioDocumentos;
                    configuracionCorreo.TextoEstadoCuenta = tenant.DatosImplementacion.ConfiguracionCorreo.TextoEstadoCuenta;
                    configuracionCorreo.TextoMensajeActivacion = tenant.DatosImplementacion.ConfiguracionCorreo.TextoMensajeActivacion;
                    configuracionCorreo.TextoPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TextoPagoCliente;
                    configuracionCorreo.TextoPagoSinComprobante = tenant.DatosImplementacion.ConfiguracionCorreo.TextoPagoSinComprobante;
                    configuracionCorreo.TextoPreCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.TextoPreCobranza;
                    configuracionCorreo.TextoRecuperarClave = tenant.DatosImplementacion.ConfiguracionCorreo.TextoRecuperarClave;
                    configuracionCorreo.TituloAccesoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TituloAccesoCliente;
                    configuracionCorreo.TituloAvisoPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TituloAvisoPagoCliente;
                    configuracionCorreo.TituloCambioClave = tenant.DatosImplementacion.ConfiguracionCorreo.TituloCambioClave;
                    configuracionCorreo.TituloCambioCorreo = tenant.DatosImplementacion.ConfiguracionCorreo.TituloCambioCorreo;
                    configuracionCorreo.TituloCambioDatos = tenant.DatosImplementacion.ConfiguracionCorreo.TituloCambioDatos;
                    configuracionCorreo.TituloCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.TituloCobranza;
                    configuracionCorreo.TituloEnvioDocumentos = tenant.DatosImplementacion.ConfiguracionCorreo.TituloEnvioDocumentos;
                    configuracionCorreo.TituloEstadoCuenta = tenant.DatosImplementacion.ConfiguracionCorreo.TituloEstadoCuenta;
                    configuracionCorreo.TituloPagoCliente = tenant.DatosImplementacion.ConfiguracionCorreo.TituloPagoCliente;
                    configuracionCorreo.TituloPagoSinComprobante = tenant.DatosImplementacion.ConfiguracionCorreo.TituloPagoSinComprobante;
                    configuracionCorreo.TituloPreCobranza = tenant.DatosImplementacion.ConfiguracionCorreo.TituloPreCobranza;
                    configuracionCorreo.TituloRecuperarClave = tenant.DatosImplementacion.ConfiguracionCorreo.TituloRecuperarClave;
                    configuracionCorreo.Usuario = tenant.DatosImplementacion.ConfiguracionCorreo.Usuario;

                    configuracionPagoCliente.CuentasContablesDeuda = tenant.DatosImplementacion.ConfiguracionPagoCliente.CuentasContablesDeuda;
                    configuracionPagoCliente.TiposDocumentosDeuda = tenant.DatosImplementacion.ConfiguracionPagoCliente.TiposDocumentosDeuda;
                    configuracionPagoCliente.DiasPorVencer = tenant.DatosImplementacion.ConfiguracionPagoCliente.DiasPorVencer;
                    configuracionPagoCliente.GlosaComprobante = tenant.DatosImplementacion.ConfiguracionPagoCliente.GlosaComprobante;

                    configuracionDiseno.BannerMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.BannerMisCompras;
                    configuracionDiseno.BannerPagoRapido = tenant.DatosImplementacion.ConfiguracionDiseno.BannerPagoRapido;
                    configuracionDiseno.BannerPortal = tenant.DatosImplementacion.ConfiguracionDiseno.BannerPortal;
                    configuracionDiseno.ColorBotonBuscar = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonBuscar;
                    configuracionDiseno.ColorBotonCancelarModalPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonCancelarModalPerfil;
                    configuracionDiseno.ColorBotonClavePerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonClavePerfil;
                    configuracionDiseno.ColorBotonEstadoPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonEstadoPerfil;
                    configuracionDiseno.ColorBotonGuardarModalPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonGuardarModalPerfil;
                    configuracionDiseno.ColorBotonInicioSesion = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonInicioSesion;
                    configuracionDiseno.ColorBotonModificarPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonModificarPerfil;
                    configuracionDiseno.ColorBotonPagar = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonPagar;
                    configuracionDiseno.ColorBotonPagoRapido = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonPagoRapido;
                    configuracionDiseno.ColorBotonUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorBotonUltimasCompras;
                    configuracionDiseno.ColorFondoDocumentos = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoDocumentos;
                    configuracionDiseno.ColorFondoGuiasMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoGuiasMisCompras;
                    configuracionDiseno.ColorFondoMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoMisCompras;
                    configuracionDiseno.ColorFondoPendientesMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoPendientesMisCompras;
                    configuracionDiseno.ColorFondoPortada = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoPortada;
                    configuracionDiseno.ColorFondoPorVencer = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoPorVencer;
                    configuracionDiseno.ColorFondoProductosMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoProductosMisCompras;
                    configuracionDiseno.ColorFondoResumenContable = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoResumenContable;
                    configuracionDiseno.ColorFondoUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoUltimasCompras;
                    configuracionDiseno.ColorFondoVencidos = tenant.DatosImplementacion.ConfiguracionDiseno.ColorFondoVencidos;
                    configuracionDiseno.ColorHoverBotonesPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.ColorHoverBotonesPerfil;
                    configuracionDiseno.ColorHoverBotonUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorHoverBotonUltimasCompras;
                    configuracionDiseno.ColorIconoPendientes = tenant.DatosImplementacion.ConfiguracionDiseno.ColorIconoPendientes;
                    configuracionDiseno.ColorIconoPorVencer = tenant.DatosImplementacion.ConfiguracionDiseno.ColorIconoPorVencer;
                    configuracionDiseno.ColorIconosMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorIconosMisCompras;
                    configuracionDiseno.ColorIconoVencidos = tenant.DatosImplementacion.ConfiguracionDiseno.ColorIconoVencidos;
                    configuracionDiseno.ColorPaginador = tenant.DatosImplementacion.ConfiguracionDiseno.ColorPaginador;
                    configuracionDiseno.ColorSeleccionDocumentos = tenant.DatosImplementacion.ConfiguracionDiseno.ColorSeleccionDocumentos;
                    configuracionDiseno.ColorTextoBotonUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoBotonUltimasCompras;
                    configuracionDiseno.ColorTextoFechaUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoFechaUltimasCompras;
                    configuracionDiseno.ColorTextoMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoMisCompras;
                    configuracionDiseno.ColorTextoMontoUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoMontoUltimasCompras;
                    configuracionDiseno.ColorTextoPendientes = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoPendientes;
                    configuracionDiseno.ColorTextoPorVencer = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoPorVencer;
                    configuracionDiseno.ColorTextoUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoUltimasCompras;
                    configuracionDiseno.ColorTextoVencidos = tenant.DatosImplementacion.ConfiguracionDiseno.ColorTextoVencidos;
                    configuracionDiseno.IconoClavePerfil = tenant.DatosImplementacion.ConfiguracionDiseno.IconoClavePerfil;
                    configuracionDiseno.IconoContactos = tenant.DatosImplementacion.ConfiguracionDiseno.IconoContactos;
                    configuracionDiseno.IconoEditarPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.IconoEditarPerfil;
                    configuracionDiseno.IconoEstadoPerfil = tenant.DatosImplementacion.ConfiguracionDiseno.IconoEstadoPerfil;
                    configuracionDiseno.IconoMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.IconoMisCompras;
                    configuracionDiseno.ImagenPortada = tenant.DatosImplementacion.ConfiguracionDiseno.ImagenPortada;
                    configuracionDiseno.ImagenUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUltimasCompras;
                    configuracionDiseno.ImagenUsuario = tenant.DatosImplementacion.ConfiguracionDiseno.ImagenUsuario;
                    configuracionDiseno.LogoMinimalistaSidebar = tenant.DatosImplementacion.ConfiguracionDiseno.LogoMinimalistaSidebar;
                    configuracionDiseno.LogoPortada = tenant.DatosImplementacion.ConfiguracionDiseno.LogoPortada;
                    configuracionDiseno.LogoSidebar = tenant.DatosImplementacion.ConfiguracionDiseno.LogoSidebar;
                    configuracionDiseno.TextoCobranzaExpirada = tenant.DatosImplementacion.ConfiguracionDiseno.TextoCobranzaExpirada;
                    configuracionDiseno.TextoDescargaCobranza = tenant.DatosImplementacion.ConfiguracionDiseno.TextoDescargaCobranza;
                    configuracionDiseno.TextoNoConsideraTodaDeuda = tenant.DatosImplementacion.ConfiguracionDiseno.TextoNoConsideraTodaDeuda;
                    configuracionDiseno.TituloComprasFacturadas = tenant.DatosImplementacion.ConfiguracionDiseno.TituloComprasFacturadas;
                    configuracionDiseno.TituloGuiasPendientes = tenant.DatosImplementacion.ConfiguracionDiseno.TituloGuiasPendientes;
                    configuracionDiseno.TituloMisCompras = tenant.DatosImplementacion.ConfiguracionDiseno.TituloMisCompras;
                    configuracionDiseno.TituloMonedaPeso = tenant.DatosImplementacion.ConfiguracionDiseno.TituloMonedaPeso;
                    configuracionDiseno.TituloOtraMoneda = tenant.DatosImplementacion.ConfiguracionDiseno.TituloOtraMoneda;
                    configuracionDiseno.TituloPendientesDashboard = tenant.DatosImplementacion.ConfiguracionDiseno.TituloPendientesDashboard;
                    configuracionDiseno.TituloPendientesFacturar = tenant.DatosImplementacion.ConfiguracionDiseno.TituloPendientesFacturar;
                    configuracionDiseno.TituloPorVencerDashboard = tenant.DatosImplementacion.ConfiguracionDiseno.TituloPorVencerDashboard;
                    configuracionDiseno.TituloProductos = tenant.DatosImplementacion.ConfiguracionDiseno.TituloProductos;
                    configuracionDiseno.TituloResumenContable = tenant.DatosImplementacion.ConfiguracionDiseno.TituloResumenContable;
                    configuracionDiseno.TituloUltimasCompras = tenant.DatosImplementacion.ConfiguracionDiseno.TituloUltimasCompras;
                    configuracionDiseno.TituloVencidosDashboard = tenant.DatosImplementacion.ConfiguracionDiseno.TituloVencidosDashboard;

                    apiSoftland.AreaDatos = tenant.DatosImplementacion.ApiSoftland.AreaDatos;
                    apiSoftland.Url = tenant.DatosImplementacion.ApiSoftland.Url;
                    apiSoftland.Token = tenant.DatosImplementacion.ApiSoftland.Token;

                    configuracionPortal.CantidadUltimasCompras = tenant.DatosImplementacion.ConfiguracionPortal.CantidadUltimasCompras;
                    configuracionPortal.HabilitaPagoRapido = tenant.DatosImplementacion.ConfiguracionPortal.HabilitaPagoRapido;
                    configuracionPortal.MuestraBotonEnviarCorreo = tenant.DatosImplementacion.ConfiguracionPortal.MuestraBotonEnviarCorreo;
                    configuracionPortal.MuestraBotonImprimir = tenant.DatosImplementacion.ConfiguracionPortal.MuestraBotonImprimir;
                    configuracionPortal.MuestraComprasFacturadas = tenant.DatosImplementacion.ConfiguracionPortal.MuestraComprasFacturadas;
                    configuracionPortal.MuestraContactoComercial = tenant.DatosImplementacion.ConfiguracionPortal.MuestraContactoComercial;
                    configuracionPortal.MuestraContactosEnvio = tenant.DatosImplementacion.ConfiguracionPortal.MuestraContactosEnvio;
                    configuracionPortal.MuestraContactosPerfil = tenant.DatosImplementacion.ConfiguracionPortal.MuestraContactosPerfil;
                    configuracionPortal.MuestraEstadoBloqueo = tenant.DatosImplementacion.ConfiguracionPortal.MuestraEstadoBloqueo;
                    configuracionPortal.MuestraEstadoSobregiro = tenant.DatosImplementacion.ConfiguracionPortal.MuestraEstadoSobregiro;
                    configuracionPortal.MuestraGuiasPendientes = tenant.DatosImplementacion.ConfiguracionPortal.MuestraGuiasPendientes;
                    configuracionPortal.MuestraPendientesFacturar = tenant.DatosImplementacion.ConfiguracionPortal.MuestraPendientesFacturar;
                    configuracionPortal.MuestraProductos = tenant.DatosImplementacion.ConfiguracionPortal.MuestraProductos;
                    configuracionPortal.MuestraResumen = tenant.DatosImplementacion.ConfiguracionPortal.MuestraResumen;
                    configuracionPortal.MuestraUltimasCompras = tenant.DatosImplementacion.ConfiguracionPortal.MuestraUltimasCompras;
                    configuracionPortal.PermiteAbonoParcial = tenant.DatosImplementacion.ConfiguracionPortal.PermiteAbonoParcial;
                    configuracionPortal.PermiteExportarExcel = tenant.DatosImplementacion.ConfiguracionPortal.PermiteExportarExcel;
                    configuracionPortal.ResumenContabilizado = tenant.DatosImplementacion.ConfiguracionPortal.ResumenContabilizado;
                    configuracionPortal.UtilizaDocumentoPagoRapido = tenant.DatosImplementacion.ConfiguracionPortal.UtilizaDocumentoPagoRapido;
                    configuracionPortal.UtilizaNumeroDireccion = tenant.DatosImplementacion.ConfiguracionPortal.UtilizaNumeroDireccion;

                    transbank.Ambiente = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWTransbankGenerarPago";
                    transbank.TipoDocumento = tenant.DatosImplementacion.DocumentoContableTransbank;
                    transbank.AmbienteConsultarPago = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWTransbankObtenerEstadoPago?id_transaccion={ID}&esProductivo={ESPRODUCTIVO}";
                    transbank.CodigoComercio = tenant.DatosImplementacion.CodigoComercioTransbank;
                    transbank.CuentaContable = tenant.DatosImplementacion.CuentaContableTransbank;
                    transbank.Estado = tenant.DatosImplementacion.UtilizaTransbank;
                    transbank.EsProduccion = tenant.DatosImplementacion.AmbienteTransbank;
                    transbank.SecretKey = tenant.DatosImplementacion.ApiKeyTransbank;
                    newContextPortal.Entry(transbank);

                    virtualPos.Ambiente = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VirtualPosGenerarPago?url_redireccion={REDIRECCION}&url_callback={CALLBACK}&id_interno={IDINTERNO}&monto_total={TOTAL}&monto_bruto={BRUTO}&rutCliente={RUT}&tipo={TIPO}&monto_impuestos={IMPUESTO}&nombre_cliente={NOMBRE}&apellido_cliente={APELLIDO}&correo_cliente={CORREO}&esProductivo={ESPRODUCTIVO}";
                    virtualPos.TipoDocumento = tenant.DatosImplementacion.DocumentoContableVirtualPos;
                    virtualPos.AmbienteConsultarPago = tenant.DatosImplementacion.ApiSoftland.Url + "VW/VWVirtualPosObtenerEstadoPago?id_transaccion={ID}&esProductivo={ESPRODUCTIVO}";
                    virtualPos.CuentaContable = tenant.DatosImplementacion.CuentaContableVirtualPos;
                    virtualPos.Estado = tenant.DatosImplementacion.UtilizaVirtualPos;

                    newContextPortal.Entry(virtualPos);
                    newContextPortal.Entry(configuracionPortal);
                    newContextPortal.Entry(apiSoftland);
                    newContextPortal.Entry(configuracionDiseno);
                    newContextPortal.Entry(configuracionPagoCliente);
                    newContextPortal.Entry(configuracionCorreo);

                    await newContextPortal.SaveChangesAsync();

                }


                return Ok(tenant);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/CreaActualizaTenant";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("getTemplate/{tipo}/{nombreEmpresa}"), Authorize]
        public async Task<ActionResult> getTemplate(int tipo, string nombreEmpresa, [FromBody] ConfiguracionCorreo model)
        {
            try
            {
                string body = string.Empty;

                switch (tipo)
                {
                    case 1:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{EMPRESA}", nombreEmpresa);
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
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        break;

                    case 3:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/activacionCuenta.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{EMPRESA}", nombreEmpresa);
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
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
                        body = body.Replace("{TituloCorreo}", model.TituloCambioDatos);
                        body = body.Replace("{LOGO}", model.LogoCorreo);
                        break;

                    case 5:
                        using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/MailTemplates/envioClave.component.html")))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{NOMBRE}", "Cliente de Prueba");
                        body = body.Replace("{CLAVE}", "123456");
                        body = body.Replace("{logo}", model.LogoCorreo);
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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

                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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
                        body = body.Replace("{ENLACE}", $"portal/#/sessions/pay/0/0/0/0");
                        body = body.Replace("{ColorBoton}", model.ColorBoton);
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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
                        body = body.Replace("{NombreEmpresa}", nombreEmpresa);
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

                return Ok(html);
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/getTemplate";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("EnrolarTransbank"), Authorize]
        public async Task<ActionResult> EnrolarTransbank(EnrolarTransbankVm tbk)
        {
            try
            {
                var tenant = _admin.Tenants.Where(x => x.IdTenant == tbk.IdTenant).FirstOrDefault();
                if (tenant == null)
                {
                    return BadRequest();
                }

                var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                var api = newContextPortal.ApiSoftlands.FirstOrDefault();
                using (var client = new HttpClient())
                {

                    string accesToken = api.Token;
                    string url = api.Url + "VW/VWTransbankRegistrarCliente";


                    var data = new Dictionary<string, string>
                    {
                        { "apikey", tbk.ApiKey },
                        { "esProductivo", tbk.EsProductivo == 0 || tbk.EsProductivo == null ? "N" : "S" },
                        { "secret", tbk.SecretKey },
                    };

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");


                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    HttpResponseMessage response = new HttpResponseMessage();

                    var content = new FormUrlEncodedContent(data);
                    response = await client.PostAsync(client.BaseAddress, content).ConfigureAwait(false);


                    if (response.IsSuccessStatusCode)
                    {
                        tenant.EnroladoTbk = 1;
                        _admin.Entry(tenant).State = EntityState.Modified;
                        _admin.SaveChanges();
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/EnrolarTransbank";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("EnrolarVpos"), Authorize]
        public async Task<ActionResult> EnrolarVpos(EnrolarVposVm vpos)
        {
            try
            {
                var tenant = _admin.Tenants.Where(x => x.IdTenant == vpos.IdTenant).FirstOrDefault();
                if (tenant == null)
                {
                    return BadRequest();
                }

                var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                var api = newContextPortal.ApiSoftlands.FirstOrDefault();
                using (var client = new HttpClient())
                {

                    string accesToken = api.Token;
                    string url = api.Url + "VW/VWVirtualPosEnrolar";


                    var data = new Dictionary<string, string>
                    {
                        { "correo_contacto", vpos.CorreoContacto },
                        { "esProductivo", vpos.EsProductivo == 0 || vpos.EsProductivo == null ? "N" : "S" },
                        { "nombre_contacto", vpos.NombreContacto },
                        { "nombre_fantasia", vpos.NombreEmpresa },
                        { "rut_comercio", vpos.RutEmpresa },
                        { "rut_contacto", vpos.RutContacto },
                    };

                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");


                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    HttpResponseMessage response = new HttpResponseMessage();

                    var content = new FormUrlEncodedContent(data);
                    response = await client.PostAsync(client.BaseAddress, content).ConfigureAwait(false);


                    if (response.IsSuccessStatusCode)
                    {
                        var content2 = await response.Content.ReadAsStringAsync();
                       EnrolarVPosResponse result = JsonConvert.DeserializeObject<EnrolarVPosResponse>(content2);

                        if(result.status == "OK")
                        {
                            tenant.EnroladoVpos = 1;
                            _admin.Entry(tenant).State = EntityState.Modified;
                            _admin.SaveChanges();
                            return Ok(true);
                        }
                        else
                        {
                            return Ok(false);
                        }              
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/EnrolarVpos";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("PasoProduccion/{idtenant}"), Authorize]
        public async Task<ActionResult> PasoProduccion(int idtenant, [FromBody] UsuariosVm usuario)
        {
            try
            {
                var tenant = _admin.Tenants.Where(x => x.IdTenant == idtenant).FirstOrDefault();
                if(tenant == null)
                {
                    return BadRequest();
                }

                tenant.Estado = 3;
                _admin.Entry(tenant).State = EntityState.Modified;
               
                var options = new DbContextOptionsBuilder<PortalClientesSoftlandContext>().UseSqlServer(tenant.ConnectionString).Options;
                var builder = new SqlConnectionStringBuilder(tenant.ConnectionString);
                var newContextPortal = new PortalClientesSoftlandContext(options, _contextAccessor);

                var configPortal = newContextPortal.ConfiguracionPortals.FirstOrDefault();
                configPortal.EstadoImplementacion = 3;
                newContextPortal.Entry(configPortal).State = EntityState.Modified;
              

                var existUser = newContextPortal.Usuarios.Where(x => x.Email == usuario.Email).FirstOrDefault();
                if (existUser != null)
                {
                    return Ok(-1);
                }

                Usuario user = new Usuario();
                user.Activo = 1;
                user.Apellidos = "Administrador";
                user.CuentaActivada = 0;
                user.Email = usuario.Email.ToLower();
                user.FechaCreacion = DateTime.Now.Date;
                user.FechaEnvioValidacion = DateTime.Now.Date;
                user.IdPerfil = 1;
                user.Nombres = "Usuario";
                string pass = RandomPassword.GenerateRandomPassword();
                HashPassword aux = new HashPassword();
                user.Password = aux.HashCode(pass);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                newContextPortal.Usuarios.Add(user);

               
                bool errorEnvio = false;
                MailService emailService = new MailService(newContextPortal, _webHostEnvironment);
                errorEnvio = emailService.EnviaAccesoUsuario(user, pass);

                if (errorEnvio)
                {
                    return Ok(0);
                }
                else
                {
                    await newContextPortal.SaveChangesAsync();
                    _admin.SaveChanges();
                    return Ok(user);
                }

            }
            catch (Exception ex)
            {

                LogProcesosAdmin log = new LogProcesosAdmin();
                log.Fecha = DateTime.Now;
                log.Hora = DateTime.Now.ToString("HH:mm:ss");
                log.Excepcion = ex.StackTrace;
                log.Mensaje = ex.Message;
                log.Ruta = "api/Implementacion/PasoProduccion";
                _admin.LogProcesosAdmins.Add(log);
                _admin.SaveChanges();
                return BadRequest(ex.Message);
            }
        }
    }
}
