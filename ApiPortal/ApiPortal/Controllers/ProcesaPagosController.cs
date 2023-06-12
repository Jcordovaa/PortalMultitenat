using ApiPortal.Dal.Models_Admin;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.Enums;
using ApiPortal.Services;
using ApiPortal.ViewModelsPortal;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;
using Transbank.Common;
using Transbank.Webpay.Common;
using Transbank.Webpay.WebpayPlus;

namespace ApiPortal.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcesaPagosController : ControllerBase
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly PortalAdministracionSoftlandContext _admin;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string message;
        private string ruta64 = string.Empty;

        public ProcesaPagosController(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment, PortalAdministracionSoftlandContext admin)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _admin = admin;
        }


        [HttpPost("GeneraPagoElectronico")]
        public async Task<IActionResult> GeneraPagoElectronico([FromQuery] int idPago, [FromQuery] int idPasarela, [FromQuery] string rutCliente, [FromQuery] int idCobranza, [FromQuery] string datosPago, [FromQuery] string tenant, [FromQuery] TbkRedirect redirectTo = TbkRedirect.Front)
        {
            SoftlandService sf = new SoftlandService(_context, _webHostEnvironment);
            LogApi logApi = new LogApi();
            logApi.Api = "api/ProcesaPagos/GeneraPagoElectronico";
            logApi.Inicio = DateTime.Now;
            logApi.Id = RandomPassword.GenerateRandomText() + logApi.Inicio.ToString();
            

            string rutEncriptado = "";
            if (redirectTo == TbkRedirect.PagoRapido)
            {
                if (string.IsNullOrEmpty(rutCliente))
                {
                    rutEncriptado = "0";
                }
                else
                {
                    rutEncriptado = rutCliente;
                }
            }
            else
            {
                rutEncriptado = rutCliente;
            }

            //Declaramos el viewModel para navegar entre los resultados de las pasarelas de pago (Iniciar, terminar, cancelado, nulo, error)
            PasarelaVM vm = new PasarelaVM();

            //Declaramos varibles de montos
            int monto = 0;
            int iva = 0;
            int montoNeto = 0;

            //Variable url que retornara al portal
            String redirectUrl = string.Empty;


            var parametros = _context.Parametros.ToList();

            switch (redirectTo)
            {
                case TbkRedirect.Front:
                    string urlFront = parametros.Where(x => x.Nombre == "UrlPagoFront").FirstOrDefault().Valor;
                    redirectUrl = $"{urlFront}/#/payment/accounts-state";
                    break;
                case TbkRedirect.Portal:
                    string urlPortal = parametros.Where(x => x.Nombre == "UrlPagoPortal").FirstOrDefault().Valor;
                    redirectUrl = $"{urlPortal}/#/sessions/paymentportal";
                    break;
                case TbkRedirect.PagoRapido:
                    string urlPagoRapido = parametros.Where(x => x.Nombre == "UrlPagoRapido").FirstOrDefault().Valor; 
                    redirectUrl = $"{urlPagoRapido}/#/sessions/pay/" + rutEncriptado + "/0/" + idCobranza.ToString();

                    break;
            }

            //Variable log para registrar intención de pago y el resultado de esta
            PasarelaPagoLog log = new PasarelaPagoLog();

            try
            {
                //Iniciamos protocolo de seguridad
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //Obtenemos la configuración de la pasarela
                var pasarela = _context.PasarelaPagos.Find(idPasarela);

                //Obtenemos detalle del pago
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();
                monto = Convert.ToInt32(pago.MontoPago);

                //Obtenemos el host (dominio) desde donde se esta ejecutando el llamado a tbk (ejemplos: localhost o berrylion.cl) 
                String httpHost = HttpContext.Request.Host.ToString();

                //Obtenemos la url o ruta que se agregar al host 
                String selfURL = HttpContext.Request.Path.ToString();

                //Obtenemos la accion que se ejecutara:
                //init: Iniciamos la intgracion con tbk
                //result: Retornamos desde el pago en tbk
                //end: Cuando terminamos el flujo y volvemos a la tienda
                //nullify: Cuando la transaccion es anulada desde el portal tbk
                string action = !String.IsNullOrEmpty(HttpContext.Request.Query["action"]) ? HttpContext.Request.Query["action"] : "init";

                //Creamos las url completas con el protocolo configurado http o https
                string sample_baseurl = pasarela.Protocolo + httpHost + selfURL;
                string completeUrl = pasarela.Protocolo + httpHost;

                //Creamos url que se enviara a tbk y una vez finalizada la transacción retornara para poder identificar en que flujo se ejecuta
                string urlReturn = sample_baseurl + $"?action=result&idPago={idPago}&idPasarela={idPasarela}&rutCliente={rutEncriptado}&idCobranza={idCobranza}&datosPago={datosPago}&redirectTo={redirectTo}&tenant={tenant}"; ;

                //Creamos la url que se utilizara para finalizar el proceso
                string urlFinal = sample_baseurl + $"?action=end&idPago={idPago}&idPasarela={idPasarela}&rutCliente={rutEncriptado}&idCobranza={idCobranza}&datosPago={datosPago}&redirectTo={redirectTo}&tenant={tenant}";

                //Creamos la url que se utilizara para procesos fallidos
                string urlFallo = sample_baseurl + $"?action=failure&idPago={idPago}&idPasarela={idPasarela}&rutCliente={rutEncriptado}&idCobranza={idCobranza}&datosPago={datosPago}&redirectTo={redirectTo}&tenant={tenant}";

                //Definimos el flujo por pasarela
                #region WEBPAY
                if (pasarela.IdPasarela == -1) //WEBPAY
                {
                    //Asignamos valores de empresa          
                    string _commerceCode = "";
                    string _apiKey = "";

                    //Declaramos y asignamos los  headers solicitados por TBK
                    string _commerceCodeHeaderName = "Tbk-Api-Key-Id";
                    string _apiKeyHeaderName = "Tbk-Api-Key-Secret";
                    RequestServiceHeaders _headers = new RequestServiceHeaders(_apiKeyHeaderName, _commerceCodeHeaderName);

                    //Configuramos el ambiente en el que se ejecutara la integración
                    //INTEGRACION = WebpayIntegrationType.Test
                    //PRODUCCION = WebpayIntegrationType.Live
                    WebpayIntegrationType _integrationType = (pasarela.Ambiente == "PRODUCCION") ? WebpayIntegrationType.Live : WebpayIntegrationType.Test;

                    //Asignamos la configuración que se enviara en el Transaction.Create
                    //Options config = new Options(/*_commerceCode, _apiKey, */"", "", _integrationType, _headers);
                    Options config = new Options(_commerceCode, _apiKey,  _integrationType);


                    //Swich que se ejecutara cada vez que se ingrese a este contralodor y permite identificar la accion a realizar durante la integración con TBK
                    switch (action)
                    {
                        default: //Init crea la transaccion con tbk y levanta el portal

                            //Registramos intención de pago en estado pendiente
                            log.IdPago = idPago;
                            log.IdPasarela = idPasarela;
                            log.Fecha = DateTime.Now;
                            log.Monto = monto;
                            log.Token = string.Empty;
                            log.Codigo = string.Empty;
                            log.Estado = "Pendiente";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = string.Empty;
                            log.Cuotas = 0;
                            log.Tarjeta = string.Empty;
                            log.Url = completeUrl;

                            _context.PasarelaPagoLogs.Add(log);
                            await _context.SaveChangesAsync();


                            //Creamos la transacción en Tbk
                            //var result = Transaction.Create(idPago.ToString(), new Random().Next(0, 5000).ToString(), monto, urlReturn);
                            var tx = new Transaction(config);
                            var result = tx.Create(idPago.ToString(), new Random().Next(0, 5000).ToString(), monto, urlReturn);

                            //Si token es invalido, se envia a la pagina de error
                            if (String.IsNullOrEmpty(result.Token))
                            {
                                //Actualiza log intención de pago con error
                                log.Estado = "Error";
                                log.Fecha = DateTime.Now;
                                log.IdPagoNavigation = null;
                                log.IdPasarelaNavigation = null;
                                _context.Entry(log).State = EntityState.Modified;
                                await _context.SaveChangesAsync();

                                //Retorna al portal en base 64 el state de error 5 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("5;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }

                            //Si resultado es correcto poblamos viewModel para redireccionar a la pagina de resultado
                            vm.Token = result.Token;
                            vm.Url = result.Url;
                            vm.Step = "Init";
                            vm.Message = message;
                            vm.AuthorizationCode = "";
                            vm.Amount = 0;
                            vm.BuyOrder = "";

                            break;
                        case "result": //result se ejecuta siempre que retorne información desde Tbk


                            //Si TBK_TOKEN tiene valor significa que la transacción fue anulada por el usuario en el portal de tbk
                            if (Request.Form["TBK_TOKEN"].ToString() != null)
                            {
                                //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a anulada
                                log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                                log.Fecha = DateTime.Now;
                                log.Estado = "Anulado por cliente";
                                log.Token = Request.Form["TBK_TOKEN"].ToString();
                                log.IdPagoNavigation = null;
                                log.IdPasarelaNavigation = null;
                                _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                _context.Entry(log).Property(x => x.Token).IsModified = true;
                                await _context.SaveChangesAsync();

                                //Retorna al portal en base 64 el state de anulado 3 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("3;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }

                            //Obtenemos token de aprobación
                            string token = Request.Form["token_ws"];

                            //Confirmamos la transacción en tbk y obtenemos los valores de esta
                            //var result2 = Transaction.Commit(token, config);
                            var tx2 = new Transaction(config);
                            var result2 = tx2.Commit(token);


                            if (result2.ResponseCode == 0) //Si la transacción es exitosa en tbk retornara 0
                            {

                                //Llenamos viewModel para redirección a END por transacción exitosa
                                vm.Token = token;
                                vm.Url = urlFinal;
                                vm.Step = "Get Result";
                                vm.Message = message;
                                vm.AuthorizationCode = result2.AuthorizationCode;
                                vm.Amount = (double)result2.Amount;
                                vm.BuyOrder = result2.BuyOrder;

                                //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a Finalizada
                                log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                                log.Fecha = result2.TransactionDate;
                                log.Monto = (decimal?)vm.Amount;
                                log.Token = token;
                                log.Codigo = vm.AuthorizationCode;
                                log.Estado = "Finalizado";
                                log.OrdenCompra = idPago.ToString();
                                log.MedioPago = result2.PaymentTypeCode;
                                log.Cuotas = result2.InstallmentsNumber;
                                log.Tarjeta = result2.CardDetail.CardNumber;
                                _context.PasarelaPagoLogs.Attach(log);
                                _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                _context.Entry(log).Property(x => x.Monto).IsModified = true;
                                _context.Entry(log).Property(x => x.Token).IsModified = true;
                                _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                                _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                _context.Entry(log).Property(x => x.OrdenCompra).IsModified = true;
                                _context.Entry(log).Property(x => x.MedioPago).IsModified = true;
                                _context.Entry(log).Property(x => x.Cuotas).IsModified = true;
                                _context.Entry(log).Property(x => x.Tarjeta).IsModified = true;
                                _context.SaveChanges();


                                //Genera comprobante contable por el pago realizado
                                sf.GeneraComprobantesContablesAsync(idPago, log.Codigo, logApi.Id);
                            }
                            else //Si el pago es rechazado en cualquiera de sus codigos
                            {
                                //Codigos de rechazo
                                //-1 = Rechazo de transacción - Reintente (Posible error en el ingreso de datos de la transacción)
                                //-2 = Rechazo de transacción(Se produjo fallo al procesar la transacción.Este mensaje de rechazo está relacionado a parámetros de la tarjeta y/ o su cuenta asociada)
                                //-3 = Error en transacción(Interno Transbank)
                                //-4 = Rechazo emisor(Rechazada por parte del emisor)
                                //- 5 = Rechazo - Posible Fraude(Transacción con riesgo de posible fraude)

                                //Llenamos viewModel para redirección a END por transacción Rechazada
                                vm.Token = token;
                                vm.Url = urlFinal;
                                vm.Step = "Error";
                                vm.Message = message;
                                vm.AuthorizationCode = result2.AuthorizationCode;
                                vm.Amount = (double)result2.Amount;
                                vm.BuyOrder = result2.BuyOrder;

                                //Obtenemos intencion de pago registrada como pendiente y actualizamos estado de rechazo retornado por TBK
                                log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                                log.Fecha = result2.TransactionDate;
                                log.Monto = (decimal?)vm.Amount;
                                log.Token = token;
                                log.Codigo = vm.AuthorizationCode;
                                log.Estado = (result2.ResponseCode == -1) ? "-1 Rechazo de transacción - Reintente (Posible error en el ingreso de datos de la transacción)" :
                                                 (result2.ResponseCode == -2) ? "-2 Rechazo de transacción(Se produjo fallo al procesar la transacción.Este mensaje de rechazo está relacionado a parámetros de la tarjeta y/ o su cuenta asociada)" :
                                                 (result2.ResponseCode == -3) ? "-3 Error en transacción(Interno Transbank)" :
                                                 (result2.ResponseCode == -4) ? "-4 Rechazo emisor(Rechazada por parte del emisor)" :
                                                 (result2.ResponseCode == -5) ? "- 5 Rechazo - Posible Fraude(Transacción con riesgo de posible fraude)" : "Sin información desde TBK";
                                log.OrdenCompra = idPago.ToString();
                                log.MedioPago = result2.PaymentTypeCode;
                                log.Cuotas = result2.InstallmentsNumber;
                                log.Tarjeta = result2.CardDetail.CardNumber;
                                log.IdPagoNavigation = null;
                                log.IdPagoNavigation = null;
                                _context.PasarelaPagoLogs.Attach(log);
                                _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                _context.Entry(log).Property(x => x.Monto).IsModified = true;
                                _context.Entry(log).Property(x => x.Token).IsModified = true;
                                _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                                _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                _context.Entry(log).Property(x => x.OrdenCompra).IsModified = true;
                                _context.Entry(log).Property(x => x.MedioPago).IsModified = true;
                                _context.Entry(log).Property(x => x.Cuotas).IsModified = true;
                                _context.Entry(log).Property(x => x.Tarjeta).IsModified = true;
                                await _context.SaveChangesAsync();

                                //Retorna al portal en base 64 el state de rechazado 2 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("2;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }

                            break;
                        case "end": //end proceso final cuando la trassaccion es exitosa



                            vm.Token = "";
                            vm.Url = "";
                            vm.Step = "end";
                            vm.Message = message;
                            vm.AuthorizationCode = string.Empty; ;
                            vm.Amount = (double)monto;
                            vm.BuyOrder = "";

                            if (Request.Form["token_ws"].ToString() != null)
                            {
                                //Retorna al portal en base 64 el state de aprobado 1 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }
                            else if (Request.Form["TBK_TOKEN"].ToString() != null)
                            {
                                //Retorna al portal en base 64 el state de anulado 3 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("3;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }

                            break;
                        case "nullify":
                            //PROCESO NO SE UTILIZA MOMENTANEAMENTE
                            break;
                    }
                }
                #endregion

                #region MERCADO PAGO
                if (pasarela.IdPasarela == 2) //Mercado Pago
                {
                    switch (action)
                    {
                        default: //Init crea la transaccion con MELI y levanta el portal


                            //Registramos intención de pago en estado pendiente
                            log.IdPago = idPago;
                            log.IdPasarela = idPasarela;
                            log.Fecha = DateTime.Now;
                            log.Monto = monto;
                            log.Token = string.Empty;
                            log.Codigo = string.Empty;
                            log.Estado = "Pendiente";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = string.Empty;
                            log.Cuotas = 0;
                            log.Tarjeta = string.Empty;
                            log.Url = completeUrl;

                            _context.PasarelaPagoLogs.Add(log);
                            await _context.SaveChangesAsync();

                            //Asignamos el token entregado por mercado libre al cliente, el token nos indicara si es produccion o integracion
                            //MercadoPagoConfig.AccessToken = pasarela.ApiKeyPasarela;

                            ////Iniciamos preferencia y configuramos el pago en MELI
                            var request = new PreferenceRequest()
                            {
                                Items = new List<PreferenceItemRequest>
                            {
                                new PreferenceItemRequest
                                {
                                    Title = "Pago " + idPago.ToString(),
                                    Quantity = 1,
                                    CurrencyId = pasarela.MonedaPasarela,
                                    UnitPrice = monto,
                                },
                            },
                                BackUrls = new PreferenceBackUrlsRequest
                                {
                                    Success = urlReturn,
                                    Failure = urlFallo,
                                    Pending = "",
                                },
                                AutoReturn = "approved",
                                ExternalReference = idPago.ToString(),
                            };

                            var client = new PreferenceClient();
                            //Creamos la preferencia de pago en MELI
                            Preference preference = await client.CreateAsync(request);

                            //Poblamos viewModel para redireccionar a la pagina de resultado
                            vm.Token = preference.Id;
                            vm.Url = preference.InitPoint;
                            vm.Step = "Init";
                            vm.Message = string.Empty;
                            vm.AuthorizationCode = string.Empty;
                            vm.Amount = 0;
                            vm.BuyOrder = string.Empty;

                            break;

                        case "result": //result se ejecuta siempre que retorne información

                            //Obtenemos valores retornados por MELI en los parametros
                            string token = HttpContext.Request.Form["merchant_order_id"];
                            string id = HttpContext.Request.Form["payment_id"];
                            string estado = HttpContext.Request.Form["status"];
                            string ventaId = HttpContext.Request.Form["external_reference"];
                            string pagoTipo = HttpContext.Request.Form["payment_type"];

                            //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a Finalizada
                            log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                            log.Fecha = pago.FechaPago;
                            log.Monto = (decimal?)monto;
                            log.Token = token;
                            log.Codigo = id;
                            log.Estado = "Finalizado";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = (pagoTipo == "credit_card") ? "Credito" : "";
                            log.IdPagoNavigation = null;
                            log.IdPasarelaNavigation = null;
                            _context.PasarelaPagoLogs.Attach(log);
                            _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                            _context.Entry(log).Property(x => x.Monto).IsModified = true;
                            _context.Entry(log).Property(x => x.Token).IsModified = true;
                            _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                            _context.Entry(log).Property(x => x.Estado).IsModified = true;
                            _context.Entry(log).Property(x => x.OrdenCompra).IsModified = true;
                            _context.Entry(log).Property(x => x.MedioPago).IsModified = true;
                            await _context.SaveChangesAsync();


                            //Llenamos viewModel para redirección a END por transacción exitosa
                            vm.Token = token;
                            vm.Url = urlFinal;
                            vm.Step = "Get Result";
                            vm.Message = "";
                            vm.AuthorizationCode = id;
                            vm.Amount = (double)monto;
                            vm.BuyOrder = ventaId;

                            //Generamos comprobante contable
                            sf.GeneraComprobantesContablesAsync(idPago, log.Codigo, logApi.Id);

                            //Retorna al portal en base 64 el state de finalizado 1 y el id del pago
                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago));
                            return Redirect($"{redirectUrl}?state={ruta64}");

                        case "end":
                            break;
                        case "failure":

                            string tokenFail = HttpContext.Request.Form["merchant_order_id"];
                            string idFail = HttpContext.Request.Form["payment_id"];
                            string estadoFail = HttpContext.Request.Form["status"];
                            string ventaIdFail = HttpContext.Request.Form["external_reference"];
                            string pagoTipoFail = HttpContext.Request.Form["payment_type"];

                            //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a Finalizada
                            log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                            log.Fecha = pago.FechaPago;
                            log.Monto = (decimal?)monto;
                            log.Token = tokenFail;
                            log.Codigo = idFail;
                            log.Estado = (estadoFail == "rejected") ? "Rechazado" : "";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = (pagoTipoFail == "credit_card") ? "Credito" : "";
                            log.IdPagoNavigation = null;
                            log.IdPasarelaNavigation = null;
                            _context.PasarelaPagoLogs.Attach(log);
                            _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                            _context.Entry(log).Property(x => x.Monto).IsModified = true;
                            _context.Entry(log).Property(x => x.Token).IsModified = true;
                            _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                            _context.Entry(log).Property(x => x.Estado).IsModified = true;
                            _context.Entry(log).Property(x => x.OrdenCompra).IsModified = true;
                            _context.Entry(log).Property(x => x.MedioPago).IsModified = true;
                            await _context.SaveChangesAsync();


                            //Retorna al portal en base 64 el state de error 3 y el id del pago
                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("3;" + idPago));
                            return Redirect($"{redirectUrl}?state={ruta64}");

                    }
                }
                #endregion

                #region FLOW
                if (pasarela.IdPasarela == 3) //Flow
                {
                    switch (action)
                    {
                        default: //Init crea la transaccion con FLOW y levanta checkout

                            //Registramos intención de pago en estado pendiente
                            log.IdPago = idPago;
                            log.IdPasarela = idPasarela;
                            log.Fecha = DateTime.Now;
                            log.Monto = monto;
                            log.Token = string.Empty;
                            log.Codigo = string.Empty;
                            log.Estado = "Pendiente";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = string.Empty;
                            log.Cuotas = 0;
                            log.Tarjeta = string.Empty;
                            log.Url = completeUrl;

                            _context.PasarelaPagoLogs.Add(log);
                            await _context.SaveChangesAsync();

                            //Declaramos diccionario ordenado alfabeticamente con los parametros que se enviaran a la API
                            var values = new SortedDictionary<string, string>
                        {
                           //{ "apiKey", pasarela.ApiKeyPasarela },
                           { "subject", "Pago " + idPago.ToString() },
                           { "currency", pasarela.MonedaPasarela },
                           { "amount", monto.ToString() },
                           { "email", (pago.EsPagoRapido == 1)? pago.Correo : pago.IdClienteNavigation.Correo },
                           { "commerceOrder", idPago.ToString() },
                           { "urlConfirmation", urlReturn },
                           { "urlReturn", urlFinal },
                        };

                            //Formateamos los parametros para luego firmarlos
                            
                            string formateo = Encrypt.QueryString(values);

                            //Agregamos al diccionario del parametro "S" con los campos firmados
                            //values.Add("s", GetHash(formateo, pasarela.SecretKeyPasarela));

                            // Como ya esta incluido el parametro s en el diccionario se lo pasamos al objeto FormUrlEncodedContent.
                            var content = new FormUrlEncodedContent(values);
                            ResponseFlow response = new ResponseFlow();

                            //llamamos a la API que crea la orden de pago
                            using (var http = new HttpClient())
                            {
                                // El resultado no entregará la respuesta que viene desde el servidor de flow si no otros detalles, para obtenerlo se usa el metodo ReadAsAsync;
                                var result = await http.PostAsync(pasarela.Ambiente + "/payment/create", content);


                                if (result.IsSuccessStatusCode)
                                {
                                    //response = await result.Content.ReadAsAsync<ResponseFlow>();
                                    response = await result.Content.ReadFromJsonAsync<ResponseFlow>();
                                }
                                else
                                {
                                    //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a error
                                    log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                                    log.Estado = "Error";
                                    log.IdPagoNavigation = null;
                                    log.IdPasarelaNavigation = null;
                                    _context.PasarelaPagoLogs.Attach(log);
                                    _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                    await _context.SaveChangesAsync();


                                    //Retorna al portal en base 64 el state de error 4 y el id del pago
                                    ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago));
                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }
                            }

                            //Creamos URL para levantar checkout
                            vm.Token = response.token;
                            vm.Url = response.url + "?token=" + response.token;
                            vm.Step = "Init";
                            vm.Message = "";
                            vm.AuthorizationCode = "";
                            vm.Amount = 0;
                            vm.BuyOrder = "";

                            break;
                        case "result":


                            break;
                        case "end":
                            //Obtenemos intencion de pago registrada como pendiente y actualizamos estado a error
                            log = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();
                            log.IdPagoNavigation = null;
                            log.IdPasarelaNavigation = null;



                            //Valida si retorna token desde flow
                            string tokenResult = string.Empty;
                            if (!string.IsNullOrEmpty(Request.Form["token"]))
                            {
                                tokenResult = Request.Form["token"];
                            }
                            else
                            {
                                //Actualiza intención de pago en estado pendiente a error
                                log.Estado = "Error";
                                _context.PasarelaPagoLogs.Attach(log);
                                _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                await _context.SaveChangesAsync();

                                //Retorna al portal en base 64 el state de error 4 y el id del pago
                                ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago));
                                return Redirect($"{redirectUrl}?state={ruta64}");
                            }

                            //Consulta estado de pago en flow
                            var valueVal = new SortedDictionary<string, string>
                            {
                                //{ "apiKey", pasarela.ApiKeyPasarela },
                                { "token", tokenResult },
                            };

                            //Formateamos los parametros para luego firmarlos
                            string formateo2 = Encrypt.QueryString(valueVal);

                            //Agregamos al diccionario del parametro "S" con los campos firmados
                            //valueVal.Add("s", GetHash(formateo2, pasarela.SecretKeyPasarela));


                            var contentVal = new FormUrlEncodedContent(valueVal);
                            ResponseFlow responseVal = new ResponseFlow();

                            //llamamos a la API que crea la orden de pago
                            using (var http = new HttpClient())
                            {
                                // El resultado no entregará la respuesta que viene desde el servidor de flow si no otros detalles, para obtenerlo se usa el metodo ReadAsAsync;
                                var result = http.GetAsync(pasarela.Ambiente + "/payment/getStatus?" + "apiKey="/* + pasarela.ApiKeyPasarela + "&token=" + tokenResult + "&s=" + GetHash(formateo2, pasarela.SecretKeyPasarela)*/).Result;


                                if (result.IsSuccessStatusCode)
                                {
                                    //responseVal = await result.Content.ReadAsAsync<ResponseFlow>();
                                    responseVal = await result.Content.ReadFromJsonAsync<ResponseFlow>();
                                }
                                else
                                {
                                    //Actualiza intención de pago en estado pendiente a error
                                    log.Estado = "Error";
                                    _context.PasarelaPagoLogs.Attach(log);
                                    _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    //Retorna al portal en base 64 el state de error 4 y el id del pago
                                    ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago));
                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }
                            }

                            if (responseVal != null)
                            {
                                log.Fecha = Convert.ToDateTime(responseVal.paymentData.date);
                                log.Token = tokenResult;
                                log.Codigo = responseVal.flowOrder.ToString();
                                log.Estado = (responseVal.status == 1) ? "Pendiente de pago" : (responseVal.status == 2) ? "Finalizado" : (responseVal.status == 3) ? "Rechazada" : (responseVal.status == 4) ? "Anulada" : "";
                                log.OrdenCompra = responseVal.commerceOrder;
                                log.MedioPago = responseVal.paymentData.media;
                                _context.PasarelaPagoLogs.Attach(log);
                                _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                _context.Entry(log).Property(x => x.Token).IsModified = true;
                                _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                                _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                _context.Entry(log).Property(x => x.OrdenCompra).IsModified = true;
                                await _context.SaveChangesAsync();

                                if (responseVal.status == 2) //Pagada
                                {
                                    //Genera comprobante contable por el pago realizado
                                    sf.GeneraComprobantesContablesAsync(idPago, log.Codigo, logApi.Id);

                                    ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago));
                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }
                                else if (responseVal.status == 3)
                                {
                                    ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago));
                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }
                                else if (responseVal.status == 4)
                                {
                                    ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago));
                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }

                            }
                            break;

                    }
                }
                #endregion

                #region FPAY
                if (pasarela.IdPasarela == 4) //FPay
                {

                }
                #endregion

                #region SOFTLANDPAY VPOS
                if (pasarela.IdPasarela == 5 || pasarela.IdPasarela == 1)
                {
                    //Swich que se ejecutara cada vez que se ingrese a este contralodor y permite identificar la accion a realizar durante la integración
                    switch (action)
                    {
                        default: //Init crea la transaccion y levanta el checkout

                            //Registramos intención de pago en estado pendiente
                            log.IdPago = idPago;
                            log.IdPasarela = idPasarela;
                            log.Fecha = DateTime.Now;
                            log.Monto = Convert.ToDecimal(monto);
                            log.Token = string.Empty;
                            log.Codigo = string.Empty;
                            log.Estado = "Pendiente";
                            log.OrdenCompra = idPago.ToString();
                            log.MedioPago = string.Empty;
                            log.Cuotas = 0;
                            log.Tarjeta = string.Empty;
                            log.Url = completeUrl;

                            _context.PasarelaPagoLogs.Add(log);
                            await _context.SaveChangesAsync();

                            using (var client = new HttpClient())
                            {
                                var api = _context.ApiSoftlands.FirstOrDefault();
                                string accesToken = api.Token;
                                string datosPagoDescrinptados = Encrypt.Base64Decode(datosPago);


                                string url = pasarela.Ambiente.Replace("{REDIRECCION}", HttpUtility.UrlEncode(urlFinal))
                                                              .Replace("{CALLBACK}", HttpUtility.UrlEncode(urlReturn))
                                                              .Replace("{IDINTERNO}", idPago.ToString())
                                                              .Replace("{TOTAL}", monto.ToString())
                                                              .Replace("{BRUTO}", montoNeto.ToString())
                                                              .Replace("{RUT}", datosPagoDescrinptados.Split(';')[2])
                                                              .Replace("{TIPO}", "B")
                                                              .Replace("{IMPUESTO}", iva.ToString())
                                                              .Replace("{NOMBRE}", WebUtility.UrlDecode(datosPagoDescrinptados.Split(';')[0]))
                                                              .Replace("{APELLIDO}", WebUtility.UrlDecode(datosPagoDescrinptados.Split(';')[1]))
                                                              .Replace("{CORREO}", datosPagoDescrinptados.Split(';')[3])
                                                              .Replace("{ESPRODUCTIVO}", (pasarela.EsProduccion == 0 || pasarela.EsProduccion == null) ? "N" : "S")
                                                              .Replace("{AREADATOS}", pasarela.EmpresaSoftlandPay);

                                var multipart = new MultipartFormDataContent();
                                if (pasarela.IdPasarela == 1)
                                {
                                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");

                                    multipart.Add(new StringContent(urlFinal), "url_redireccion");
                                    multipart.Add(new StringContent(urlReturn), "url_callback");
                                    multipart.Add(new StringContent(idPago.ToString()), "id_interno");
                                    multipart.Add(new StringContent(monto.ToString()), "monto_total");
                                    multipart.Add(new StringContent(montoNeto.ToString()), "monto_bruto");
                                    multipart.Add(new StringContent(datosPagoDescrinptados.Split(';')[2]), "rutCliente");
                                    multipart.Add(new StringContent("B"), "tipo");
                                    multipart.Add(new StringContent(iva.ToString()), "monto_impuestos");
                                    multipart.Add(new StringContent(WebUtility.UrlDecode(datosPagoDescrinptados.Split(';')[0])), "nombre_cliente");
                                    multipart.Add(new StringContent(WebUtility.UrlDecode(datosPagoDescrinptados.Split(';')[1])), "apellido_cliente");
                                    multipart.Add(new StringContent(datosPagoDescrinptados.Split(';')[3]), "correo_cliente");
                                    multipart.Add(new StringContent(pasarela.EsProduccion == 0 || pasarela.EsProduccion == null ? "N" : "S"), "esProductivo");
                                }


                                client.BaseAddress = new Uri(url);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                HttpResponseMessage response = new HttpResponseMessage();

                                if (pasarela.IdPasarela == 1)
                                {
                                    response = await client.PostAsync(client.BaseAddress, multipart).ConfigureAwait(false);
                                }
                                else
                                {
                                    response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);
                                }

                                if (response.IsSuccessStatusCode)
                                {
                                    var content = await response.Content.ReadAsStringAsync();
                                    ResultadoVpos result = JsonConvert.DeserializeObject<ResultadoVpos>(content);

                                    vm.Token = result.id_merchant;
                                    vm.Url = result.url_pago;
                                    vm.Step = "Init";
                                    vm.Message = message;
                                    vm.AuthorizationCode = result.id_transaccion;
                                    vm.Amount = 0;
                                    vm.BuyOrder = "";

                                    log.Estado = "Procesando";
                                    log.Fecha = DateTime.Now;
                                    log.Token = vm.Token;
                                    log.Codigo = vm.AuthorizationCode;

                                    _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                    _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                    _context.Entry(log).Property(x => x.Token).IsModified = true;
                                    _context.Entry(log).Property(x => x.Codigo).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    var retorno = new { estado = 1, enlacePago = vm.Url };
                                    //return Redirect(vm.Url);
                                    logApi.Termino = DateTime.Now;
                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                    sf.guardarLogApi(logApi);

                                    return Ok(retorno);
                                }
                                else
                                {
                                    log.Estado = "Error";
                                    log.Fecha = DateTime.Now;

                                    _context.Entry(log).Property(x => x.Fecha).IsModified = true;
                                    _context.Entry(log).Property(x => x.Estado).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    //Retorna al portal en base 64 el state de error 5 y el id del pago

                                    if (redirectTo == TbkRedirect.PagoRapido)
                                    {
                                        if (!string.IsNullOrEmpty(rutEncriptado) && rutEncriptado != "0")
                                        {
                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("5;" + idPago + ";0"));

                                        }
                                        else
                                        {
                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("5;" + idPago + ";1"));
                                        }

                                    }
                                    else
                                    {
                                        ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("5;" + idPago));
                                    }

                                    logApi.Termino = DateTime.Now;
                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                    sf.guardarLogApi(logApi);

                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                }
                            }

                            break;
                        case "end": //end proceso final cuando la trassaccion es retornada desde vpos

                            int reprocesos = 5;
                            int procesos = 1;
                        gotoCallback:
                            int milliseconds = 1500;

                            vm.Token = "";
                            vm.Url = "";
                            vm.Step = "end";
                            vm.Message = message;
                            vm.AuthorizationCode = string.Empty; ;
                            vm.Amount = (double)monto;
                            vm.BuyOrder = "";

                            var logValida = _context.PasarelaPagoLogs.Where(x => x.IdPago == idPago).FirstOrDefault();

                            if (logValida != null)
                            {
                                if (procesos <= reprocesos)
                                {
                                    using (var client = new HttpClient())
                                    {
                                        var api = _context.ApiSoftlands.FirstOrDefault();
                                        string accesToken = api.Token;


                                        string url = pasarela.AmbienteConsultarPago.Replace("{ID}", logValida.Token)
                                                                      .Replace("{ESPRODUCTIVO}", (pasarela.EsProduccion == 0 || pasarela.EsProduccion == null) ? "N" : "S");



                                        client.BaseAddress = new Uri(url);
                                        client.DefaultRequestHeaders.Accept.Clear();
                                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);
                                        if (response.IsSuccessStatusCode)
                                        {
                                            var content = await response.Content.ReadAsStringAsync();
                                            ResultadoEstadoPagoVPOS result = JsonConvert.DeserializeObject<ResultadoEstadoPagoVPOS>(content);

                                            if (result.Estado == "PAGADO" || result.Estado == "AUTHORIZED") //Pago exitoso generar comprobante
                                            {
                                                logValida.Estado = result.Estado;
                                                logValida.Fecha = DateTime.Now;
                                                logValida.MedioPago = result.Medio_pago;
                                                logValida.Cuotas = Convert.ToInt32(result.Cuotas);
                                                logValida.Tarjeta = result.Forma_pago;

                                                _context.Entry(logValida).Property(x => x.Fecha).IsModified = true;
                                                _context.Entry(logValida).Property(x => x.Estado).IsModified = true;
                                                _context.Entry(logValida).Property(x => x.MedioPago).IsModified = true;
                                                _context.Entry(logValida).Property(x => x.Cuotas).IsModified = true;
                                                _context.Entry(logValida).Property(x => x.Tarjeta).IsModified = true;
                                                await _context.SaveChangesAsync();

                                                //Genera comprobante contable por el pago realizado
                                                string numComprobante = await sf.GeneraComprobantesContablesAsync(idPago, logValida.Token, logApi.Id);

                                                if (string.IsNullOrEmpty(numComprobante))
                                                {
                                                    string folios = string.Empty;
                                                    if (pago.PagosDetalles.Count > 0)
                                                    {
                                                        foreach (var item in pago.PagosDetalles)
                                                        {
                                                            if (string.IsNullOrEmpty(folios))
                                                            {
                                                                folios = ";" + item.Folio.ToString();
                                                            }
                                                            else
                                                            {
                                                                folios = folios + "-" + item.Folio.ToString();
                                                            }
                                                        }

                                                    }

                                                    if (redirectTo == TbkRedirect.PagoRapido)
                                                    {
                                                        if (!string.IsNullOrEmpty(rutEncriptado) && rutEncriptado != "0")
                                                        {

                                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago + ";0" + folios));

                                                        }
                                                        else
                                                        {
                                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago + ";1" + folios));
                                                        }

                                                    }
                                                    else
                                                    {
                                                        ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("4;" + idPago + folios));
                                                    }

                                                    logApi.Termino = DateTime.Now;
                                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                                    sf.guardarLogApi(logApi);

                                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                                }
                                                else
                                                {
                                                    if (idCobranza != 0)
                                                    {

                                                        var detallesPago = _context.PagosDetalles.Where(x => x.IdPago == idPago).ToList();
                                                        string rutDesencriptado = Encrypt.Base64Decode(rutCliente);
                                                        var detallesCobranza = _context.CobranzaDetalles.Where(x => x.IdCobranza == idCobranza && x.RutCliente == rutDesencriptado).ToList();
                                                        var docsCliente = await sf.GetAllDocumentosContabilizadosCliente(pago.CodAux, logApi.Id);
                                                        foreach (var detalleCobranza in detallesCobranza)
                                                        {
                                                            var detPago = detallesPago.OrderByDescending(x => x.IdPagoDetalle).Where(x => x.Folio == detalleCobranza.Folio && detalleCobranza.TipoDocumento == x.TipoDocumento).FirstOrDefault();
                                                            if (detPago != null)
                                                            {
                                                                detalleCobranza.FechaPago = pago.FechaPago;
                                                                detalleCobranza.HoraPago = pago.HoraPago;
                                                                detalleCobranza.ComprobanteContable = numComprobante;
                                                                detalleCobranza.IdPago = idPago;
                                                                detalleCobranza.Pagado += detPago.Apagar;

                                                                var existDoc = docsCliente.Where(x => x.Numdoc == detalleCobranza.Folio && x.Ttdcod == detalleCobranza.TipoDocumento).FirstOrDefault();
                                                                if (existDoc != null)
                                                                {
                                                                    detalleCobranza.IdEstado = 4;
                                                                }
                                                                else
                                                                {
                                                                    detalleCobranza.IdEstado = 5;
                                                                }

                                                                _context.CobranzaDetalles.Attach(detalleCobranza);
                                                                _context.Entry(detalleCobranza).Property(x => x.FechaPago).IsModified = true;
                                                                _context.Entry(detalleCobranza).Property(x => x.HoraPago).IsModified = true;
                                                                _context.Entry(detalleCobranza).Property(x => x.ComprobanteContable).IsModified = true;
                                                                _context.Entry(detalleCobranza).Property(x => x.IdPago).IsModified = true;
                                                                _context.Entry(detalleCobranza).Property(x => x.IdEstado).IsModified = true;
                                                                _context.Entry(detalleCobranza).Property(x => x.Pagado).IsModified = true;

                                                            }

                                                        }
                                                        _context.SaveChanges();
                                                    }
                                                    //ClientesPortalController clientesController = new ClientesPortalController(_context,_webHostEnvironment,_admin, IHttpContextAccessor contextAccessor);
                                                    //clientesController.EnviaCorreoComprobante(idPago).ConfigureAwait(false);
                                                    if (redirectTo == TbkRedirect.PagoRapido)
                                                    {
                                                        if (!string.IsNullOrEmpty(rutEncriptado) && rutEncriptado != "0")
                                                        {
                                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago + ";0"));
                                                        }
                                                        else
                                                        {
                                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago + ";1"));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("1;" + idPago));

                                                    }

                                                    logApi.Termino = DateTime.Now;
                                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                                    sf.guardarLogApi(logApi);

                                                    return Redirect($"{redirectUrl}?state={ruta64}");
                                                }

                                            }
                                            else
                                            {
                                                procesos = procesos + 1;
                                                Thread.Sleep(milliseconds);

                                                goto gotoCallback;
                                            }

                                        }
                                        else
                                        {
                                            procesos = procesos + 1;
                                            Thread.Sleep(milliseconds);

                                            goto gotoCallback;
                                        }
                                    }

                                    procesos = procesos + 1;
                                    Thread.Sleep(milliseconds);


                                    goto gotoCallback;
                                }
                                else
                                {
                                    //error al procesar
                                    if (redirectTo == TbkRedirect.PagoRapido)
                                    {
                                        if (!string.IsNullOrEmpty(rutEncriptado))
                                        {
                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("2;" + idPago + ";0"));
                                        }
                                        else
                                        {
                                            ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("2;" + idPago + ";1"));
                                        }
                                    }
                                    else
                                    {
                                        ruta64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("2;" + idPago));
                                       
                                    }

                                    logApi.Termino = DateTime.Now;
                                    logApi.Segundos = (int?)Math.Round((logApi.Termino - logApi.Inicio).Value.TotalSeconds);
                                    sf.guardarLogApi(logApi);

                                    return Redirect($"{redirectUrl}?state={ruta64}");

                                }


                            }

                            break;
                    }
                }
                #endregion



            }
            catch (Exception e)
            {
                LogProceso logs = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "ProcesaPagosController"
                };
                _context.LogProcesos.Add(logs);
                _context.SaveChanges();
            }

            return Ok(vm);
        }
    }
}
