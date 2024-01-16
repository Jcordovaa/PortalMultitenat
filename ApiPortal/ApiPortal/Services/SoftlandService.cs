using Microsoft.Data.SqlClient;
using ApiPortal.ModelSoftland;
using System.Data;
using System.Net;
using Newtonsoft.Json;
using ApiPortal.ViewModelsPortal;
using System.Web;
using System.Net.Mail;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsAdmin;
using ApiPortal.DAL.Models_Admin;
using System.Runtime.Intrinsics.X86;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApiPortal.Dal.Models_Admin;

namespace ApiPortal.Services
{
    public class SoftlandService
    {
        private readonly SqlConnection conSoftland;
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        string utilizaApiSoftland = "true"; /*ConfigurationManager.AppSettings["UtilizaApiSoftland"];*/
        public SoftlandService(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            conSoftland = new SqlConnection("");
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<TipoDocSoftlandDTO>> GetAllTipoDocSoftlandAsync(string logApiId)
        {
            List<TipoDocSoftlandDTO> retorno = new List<TipoDocSoftlandDTO>();
            try
            {

                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT CodDoc, DesDoc FROM SOFTLAND.cwttdoc ORDER BY DesDoc";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        TipoDocSoftlandDTO aux = new TipoDocSoftlandDTO();
                        aux.CodDoc = reader["CodDoc"].ToString();
                        aux.DesDoc = reader["DesDoc"].ToString();

                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaTiposDeDocumentos.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaTiposDeDocumentos";



                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<TipoDocSoftlandAPIDTO>> listaTipoDocs = JsonConvert.DeserializeObject<List<List<TipoDocSoftlandAPIDTO>>>(content);

                            foreach (var item in listaTipoDocs)
                            {
                                var tiposDocs = item.OrderBy(x => x.DesDoc).ToList();
                                foreach (var tDoc in tiposDocs)
                                {
                                    TipoDocSoftlandDTO aux = new TipoDocSoftlandDTO();
                                    aux.CodDoc = tDoc.CodDoc;
                                    aux.DesDoc = tDoc.DesDoc;
                                    retorno.Add(aux);
                                }

                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetAllTipoDocSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllTipoDocSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();
                }
            }
            return retorno;
        }

        public async Task<List<CuentasContablesSoftlandDTO>> GetAllCuentasContablesSoftlandAsync(string logApiId)
        {
            List<CuentasContablesSoftlandDTO> retorno = new List<CuentasContablesSoftlandDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.cwpctas ORDER BY PCDESC";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        CuentasContablesSoftlandDTO aux = new CuentasContablesSoftlandDTO();
                        aux.Codigo = reader["PCCODI"].ToString();
                        aux.Nombre = reader["PCDESC"].ToString();
                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaPlanDeCuentas.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaPlanDeCuentas";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<CuentasContablesSoftlandAPIDTO>> listaCuentasContables = JsonConvert.DeserializeObject<List<List<CuentasContablesSoftlandAPIDTO>>>(content);
                            foreach (var cuentasContables in listaCuentasContables)
                            {
                                foreach (var item in cuentasContables)
                                {
                                    CuentasContablesSoftlandDTO aux = new CuentasContablesSoftlandDTO();
                                    aux.Codigo = item.PCCODI;
                                    aux.Nombre = item.PCDESC;
                                    retorno.Add(aux);
                                }
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetAllCuentasContablesSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllCuentasContablesSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();
                }

            }
            return retorno;

        }

        public async Task<List<CuentasContablesSoftlandDTO>> getCuentasContablePagoAsync(string logApiId)
        {
            List<CuentasContablesSoftlandDTO> retorno = new List<CuentasContablesSoftlandDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.cwpctas ORDER BY PCDESC";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        CuentasContablesSoftlandDTO aux = new CuentasContablesSoftlandDTO();
                        aux.Codigo = reader["PCCODI"].ToString();
                        aux.Nombre = reader["PCDESC"].ToString();
                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.CuentasPasarelaPagos.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "CuentasPasarelaPagos";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<CuentasContablesSoftlandAPIDTO>> listaCuentasContables = JsonConvert.DeserializeObject<List<List<CuentasContablesSoftlandAPIDTO>>>(content);
                            foreach (var cuentasContables in listaCuentasContables)
                            {
                                foreach (var item in cuentasContables)
                                {
                                    CuentasContablesSoftlandDTO aux = new CuentasContablesSoftlandDTO();
                                    aux.Codigo = item.PCCODI;
                                    aux.Nombre = item.PCDESC;
                                    retorno.Add(aux);
                                }
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/getCuentasContablePagoAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllCuentasContablesSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();
                }

            }
            return retorno;

        }

        public async Task<List<ContactoDTO>> GetAllContactosAsync(string codAux, string logApiId)
        {
            List<ContactoDTO> retorno = new List<ContactoDTO>();
            try
            {

                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select c.NomCon, c.Email, car.CarNom, car.CarCod from softland.cwtaxco c " +
                                        "INNER JOIN softland.cwtcarg car                            " +
                                        "on c.CarCon = car.CarCod                                   " +
                                        "where CodAuc = '" + codAux + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ContactoDTO aux = new ContactoDTO();
                        aux.NombreContacto = reader["NomCon"].ToString();
                        aux.Correo = reader["Email"].ToString();
                        aux.CargoContacto = reader["CarNom"].ToString();
                        aux.CodCargo = reader["CarCod"].ToString();
                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {

                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ContactosXauxiliar.Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ContactosXauxiliar";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ContactoApiDTO>> listaContactos = JsonConvert.DeserializeObject<List<List<ContactoApiDTO>>>(content);
                            listaContactos[0] = listaContactos[0].Where(x => x.Email != null && x.Email != "").ToList();
                            foreach (var item in listaContactos[0])
                            {
                                ContactoDTO aux = new ContactoDTO();
                                aux.NombreContacto = item.NomCon;
                                aux.Correo = item.Email;
                                aux.CargoContacto = item.CarNom;
                                aux.CodCargo = item.CarCon;
                                retorno.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetAllContactosAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllContactos"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }
            return retorno;
        }

        public async Task<List<ClienteDTO>> BuscarClienteSoftlandAsync(string codAux, string rut, string nombre, string logApiId)
        {
            List<ClienteDTO> retorno = new List<ClienteDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    string sqlWhere = string.Empty;
                    if (!string.IsNullOrEmpty(codAux))
                    {
                        sqlWhere = sqlWhere + " WHERE CodAux='" + codAux + "' ";
                    }

                    if (!string.IsNullOrEmpty(rut))
                    {
                        if (string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + " WHERE RutAux='" + rut + "' ";
                        }
                        else
                        {
                            sqlWhere = sqlWhere + " AND RutAux='" + rut + "' ";
                        }
                    }

                    if (!string.IsNullOrEmpty(nombre))
                    {
                        if (string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + " WHERE NomAux like '%" + nombre + "%' ";
                        }
                        else
                        {
                            sqlWhere = sqlWhere + " AND NomAux like '%" + nombre + "%' ";
                        }
                    }

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT RutAux, CodAux, NomAux, EMail  FROM softland.cwtauxi " + sqlWhere;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ClienteDTO aux = new ClienteDTO();
                        aux.Rut = reader["RutAux"].ToString();
                        aux.CodAux = reader["CodAux"].ToString();
                        aux.Correo = reader["EMail"].ToString();
                        aux.Nombre = reader["NomAux"].ToString();
                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaCliente.Replace("{AREADATOS}", api.AreaDatos).Replace("pagina={PAGINA}&", "").Replace("&cantidad={CANTIDAD}", "");

                        if (!string.IsNullOrEmpty(codAux))
                        {
                            url = url.Replace("{CODAUX}", codAux);
                        }
                        else
                        {
                            url = url.Replace("codaux={CODAUX}&", "");
                        }

                        if (!string.IsNullOrEmpty(rut))
                        {
                            url = url.Replace("{RUT}", rut);
                        }
                        else
                        {
                            url = url.Replace("rut={RUT}&", "");
                        }

                        if (!string.IsNullOrEmpty(nombre))
                        {
                            url = url.Replace("{NOMBRE}", nombre);
                        }
                        else
                        {
                            url = url.Replace("&nombre={NOMBRE}", "");
                        }

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaCliente";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ClienteAPIDTO>> clientes = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);
                            clientes[0] = clientes[0].Where(x => x.CodAux != null).ToList();
                            foreach (var c in clientes[0])
                            {
                                ClienteDTO aux = new ClienteDTO();
                                aux.Rut = c.RutAux;
                                aux.CodAux = c.CodAux;
                                aux.Correo = c.EMail;
                                aux.Nombre = c.NomAux;
                                retorno.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/BuscarClienteSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/BuscarClienteSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }
            return retorno;
        }

        public async Task<List<RegionDTO>> GetRegionesSoftland(string logApiId)
        {
            List<RegionDTO> retorno = new List<RegionDTO>();

            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader result;
                    cmd.CommandText = "select * from softland.cwtregion";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    result = cmd.ExecuteReader();

                    while (result.Read())
                    {
                        RegionDTO item = new RegionDTO();
                        item.IdRegion = (result["id_Region"] != DBNull.Value) ? Convert.ToInt32(result["id_Region"]) : 0;
                        item.Nombre = (result["Descripcion"] != DBNull.Value) ? result["Descripcion"].ToString() : "";
                        retorno.Add(item);
                    }
                    result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaRegiones.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaRegiones";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<RegionAPIDTO>> listaRegiones = JsonConvert.DeserializeObject<List<List<RegionAPIDTO>>>(content);
                            foreach (var regiones in listaRegiones)
                            {
                                foreach (var item in regiones)
                                {
                                    RegionDTO aux = new RegionDTO();
                                    aux.IdRegion = item.id_Region;
                                    aux.Nombre = item.Descripcion;
                                    retorno.Add(aux);
                                }

                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetRegionesSoftland"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetRegionesSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false")
                { conSoftland.Close(); }
            }

            return retorno;
        }

        public async Task<List<ComunaDTO>> GetComunasSoftlandAsync(string logApiId)
        {
            List<ComunaDTO> retorno = new List<ComunaDTO>();

            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader result;
                    cmd.CommandText = "select * from softland.cwtcomu";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    result = cmd.ExecuteReader();

                    while (result.Read())
                    {
                        ComunaDTO item = new ComunaDTO();
                        item.CodComunaSoftland = (result["ComCod"] != DBNull.Value) ? result["ComCod"].ToString() : "";
                        item.IdRegion = (result["id_Region"] != DBNull.Value) ? Convert.ToInt32(result["id_Region"]) : 0;
                        item.Nombre = (result["ComDes"] != DBNull.Value) ? result["ComDes"].ToString() : "";
                        retorno.Add(item);
                    }
                    result.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaComunas.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaComunas";

                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);


                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ComunaAPIDTO>> listaComunas = JsonConvert.DeserializeObject<List<List<ComunaAPIDTO>>>(content);
                            foreach (var comunas in listaComunas)
                            {
                                foreach (var item in comunas)
                                {
                                    ComunaDTO aux = new ComunaDTO();
                                    aux.CodComunaSoftland = item.ComCod;
                                    aux.IdRegion = item.id_Region;
                                    aux.Nombre = item.ComDes;
                                    retorno.Add(aux);
                                }
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetComunasSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetComunasSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false")
                { conSoftland.Close(); }
            }

            return retorno;
        }

        public async Task<List<GiroDTO>> GetGirosSoftlandAsync(string logApiId)
        {
            List<GiroDTO> item = new List<GiroDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.cwtgiro";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        GiroDTO aux = new GiroDTO();
                        aux.IdGiro = reader["GirCod"].ToString();
                        aux.Nombre = reader["GirDes"].ToString();
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaGiros.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaGiros";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<GiroAPIDTO>> listaGiros = JsonConvert.DeserializeObject<List<List<GiroAPIDTO>>>(content);
                            foreach (var giros in listaGiros)
                            {
                                foreach (var giro in giros)
                                {
                                    GiroDTO aux = new GiroDTO();
                                    aux.IdGiro = giro.GirCod;
                                    aux.Nombre = giro.GirDes;
                                    item.Add(aux);
                                }
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetGirosSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetGirosSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false")
                { conSoftland.Close(); }
            }

            return item;
        }

        public async Task<List<CargoDTO>> GetCargosAsync(string logApiId)
        {
            List<CargoDTO> retorno = new List<CargoDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.cwtcarg";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        CargoDTO item = new CargoDTO();

                        item.CarCod = reader["CarCod"].ToString();
                        item.CarNom = reader["CarNom"].ToString();
                        retorno.Add(item);
                    }

                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaCargos.Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaCargos";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<CargoApiDTO>> listaCargos = JsonConvert.DeserializeObject<List<List<CargoApiDTO>>>(content);
                            foreach (var cargo in listaCargos[0])
                            {
                                CargoDTO aux = new CargoDTO();
                                aux.CarCod = cargo.CarCod;
                                aux.CarNom = cargo.CarNom;
                                retorno.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetCargosAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCargos"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }

            return retorno;
        }

        public List<CentroCostoDTO> GetCentrosCostoActivos(string logApiId)
        {
            List<CentroCostoDTO> retorno = new List<CentroCostoDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "select * from softland.cwtccos where ACTIVO = 'S'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    CentroCostoDTO item = new CentroCostoDTO();

                    item.CodiCC = reader["CodiCC"].ToString();
                    item.DescCC = reader["DescCC"].ToString();
                    item.NivelCC = Convert.ToInt32(reader["NivelCC"]);
                    item.Activo = reader["ACTIVO"].ToString();
                    retorno.Add(item);
                }

                reader.Close();
                conSoftland.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCentrosCostoActivos"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }

            return retorno;
        }

        public List<AreaNegocioDTO> GetAreaNegocio(string logApiId)
        {
            List<AreaNegocioDTO> retorno = new List<AreaNegocioDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "select * from softland.cwtaren";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    AreaNegocioDTO item = new AreaNegocioDTO();

                    item.CodArn = reader["CodArn"].ToString();
                    item.DesArn = reader["DesArn"].ToString();
                    retorno.Add(item);
                }

                reader.Close();
                conSoftland.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAreaNegocio"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }

            return retorno;
        }

        public async Task<ClienteDTO> GetClienteSoftlandAsync(string codAux, string rut, string logApiId)
        {
            ClienteDTO retorno = new ClienteDTO();

            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader result;
                    cmd.CommandText = "SELECT  top 1 softland.cwtauxi.Bloqueado, softland.cwtauxi.CodAux, softland.cwtauxi.NomAux AS Cliente, softland.cwtauxi.RutAux AS Rut, softland.cwtgiro.GirDes AS Giro, softland.cwtcomu.ComDes AS Comuna, softland.cwtcomu.ComCod, softland.cwtauxi.ComAux," +
                                      "                       softland.cwtciud.CiuDes AS Ciudad, softland.cwtauxi.CiuAux, softland.cwtpais.PaiDes AS Pais, softland.cwtauxi.DirAux AS Direccion, softland.cwtauxi.DirNum AS DirNum, softland.cwtauxi.FonAux1 AS Fono,  " +
                                      "                      softland.cwtauxi.Bloqueado, softland.cwtregion.Descripcion AS RegionNombre, softland.cwtauxi.Region, softland.cwtauxi.EMail, softland.cwtauxi.esReceptorDTE, softland.cwtauxi.eMailDTE " +
                                      "                      ,isnull((select top 1 GirAux from softland.cwtauxi where CodAux = '" + codAux + "'),'') as GirAux " +
                                      "                      ,case when softland.cwtaxco.NomCon is null then softland.cwtauxi.NomAux else softland.cwtaxco.NomCon end as nombreContacto " +

                                      "FROM softland.cwtauxi LEFT OUTER JOIN " +
                                      "                      softland.cwtcomu ON softland.cwtauxi.ComAux = softland.cwtcomu.ComCod LEFT OUTER JOIN " +
                                      "                      softland.cwtgiro ON softland.cwtauxi.GirAux = softland.cwtgiro.GirCod LEFT OUTER JOIN " +
                                      "                      softland.cwtregion ON softland.cwtauxi.Region = softland.cwtregion.id_Region LEFT OUTER JOIN " +
                                      "                      softland.cwtciud ON softland.cwtauxi.CiuAux = softland.cwtciud.CiuCod LEFT OUTER JOIN " +
                                      "                      softland.cwtpais ON softland.cwtauxi.PaiAux = softland.cwtpais.PaiCod " +
                                      "                      LEFT OUTER JOIN softland.cwtaxco ON softland.cwtaxco.CodAuc = softland.cwtauxi.CodAux " +
                                      "WHERE (softland.cwtauxi.ActAux = 'S') AND (softland.cwtauxi.CodAux = '" + codAux + "')";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    result = cmd.ExecuteReader();


                    while (result.Read())
                    {
                        retorno.Rut = (result["Rut"] != DBNull.Value) ? result["Rut"].ToString() : string.Empty;
                        retorno.Nombre = (result["Cliente"] != DBNull.Value) ? result["Cliente"].ToString() : string.Empty;
                        retorno.Correo = (result["Email"] != DBNull.Value) ? result["Email"].ToString() : string.Empty;
                        retorno.ComCod = (result["ComCod"] != DBNull.Value) ? result["ComCod"].ToString() : string.Empty;
                        retorno.ComunaNombre = (result["Comuna"] != DBNull.Value) ? result["Comuna"].ToString() : string.Empty;
                        retorno.IdRegion = (result["Region"] != DBNull.Value) ? Convert.ToInt32(result["Region"]) : 0;
                        retorno.RegionNombre = (result["RegionNombre"] != DBNull.Value) ? result["RegionNombre"].ToString() : string.Empty;
                        retorno.DirAux = (result["Direccion"] != DBNull.Value) ? result["Direccion"].ToString() : string.Empty;
                        retorno.DirNum = (result["DirNum"] != DBNull.Value) ? result["DirNum"].ToString() : string.Empty;
                        retorno.Telefono = (result["Fono"] != DBNull.Value) ? result["Fono"].ToString() : string.Empty;
                        retorno.Estado = (result["Bloqueado"] != DBNull.Value) ? result["Bloqueado"].ToString() == "N" ? 1 : 0 : 1;
                        retorno.EsReceptorDTE = (result["esReceptorDTE"] != DBNull.Value) ? result["esReceptorDTE"].ToString() != "N" ? "1" : "0" : "1";
                        retorno.EmailDTE = (result["eMailDTE"] != DBNull.Value) ? result["eMailDTE"].ToString() : string.Empty;
                        retorno.CodGiro = (result["GirAux"] != DBNull.Value) ? result["GirAux"].ToString() : string.Empty;
                        retorno.NomGiro = (result["Giro"] != DBNull.Value) ? result["Giro"].ToString() : string.Empty;
                    }
                    result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", "").Replace("{CATCLI}", "").Replace("{CODAUX}", codAux).Replace("{CODLISTA}", "").Replace("{CODVEN}", "").Replace("{CONVTA}", "")
                           .Replace("{NOMBRE}", "").Replace("{RUT}", rut);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaCliente";

                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ClienteAPIDTO>> cliente = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);

                            var c = cliente[0].Where(x => x.CodAux != null).FirstOrDefault();
                            if (c != null)
                            {
                                retorno.Rut = c.RutAux;
                                retorno.Nombre = c.NomAux;
                                retorno.Correo = c.EMail;
                                retorno.ComCod = c.ComCod;
                                retorno.CodAux = c.CodAux;
                                retorno.ComunaNombre = c.ComDes;
                                retorno.IdRegion = (int)c.Id_Region;
                                retorno.RegionNombre = c.RegionDes;
                                retorno.DirAux = c.DirAux;
                                retorno.DirNum = c.DirNum;
                                retorno.Telefono = c.FonAux1;
                                retorno.Estado = c.Bloqueado == "N" ? 1 : 0;
                                retorno.EsReceptorDTE = c.EsReceptorDTE;
                                retorno.CodGiro = c.Giraux;
                                retorno.NomGiro = c.GirDes;
                                retorno.EmailDTE = c.EMailDTE;

                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetClienteSoftland"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetClienteSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false")
                { conSoftland.Close(); }
            }

            return retorno;
        }

        public async Task<bool> UpdateAuxiliarPortalPagoAsync(ClienteDTO cliente, string logApiId)
        {
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE softland.cwtauxi " +
                                               "SET Region = @Region,   " +
                                               "    ComAux = @ComAux,   " +
                                               "    FonAux1 = @FonAux1,   " +
                                               "    DirAux = @DirAux,   " +
                                               "    DirNum = @DirNum,   " +
                                               "    NomAux = @NomAux,   " +
                                               "    GirAux = @GirAux,   " +
                                               "    eMailDTE = @eMailDTE, " +
                                               "    esReceptorDTE = @esReceptorDTE " +
                                               "where RutAux = @RutAux");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    cmd.Parameters.AddWithValue("@Region", cliente.IdRegion);
                    cmd.Parameters.AddWithValue("@ComAux", cliente.ComCod);
                    cmd.Parameters.AddWithValue("@FonAux1", cliente.Telefono);
                    cmd.Parameters.AddWithValue("@DirAux", cliente.DirAux);
                    if (!string.IsNullOrEmpty(cliente.DirNum)) { cmd.Parameters.AddWithValue("@DirNum", cliente.DirNum); } else { cmd.Parameters.AddWithValue("@DirNum", DBNull.Value); }
                    if (!string.IsNullOrEmpty(cliente.EmailDTE)) { cmd.Parameters.AddWithValue("@eMailDTE", cliente.EmailDTE); cmd.Parameters.AddWithValue("@esReceptorDTE", "S"); } else { cmd.Parameters.AddWithValue("@eMailDTE", DBNull.Value); cmd.Parameters.AddWithValue("@esReceptorDTE", "N"); }
                    cmd.Parameters.AddWithValue("@NomAux", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@GirAux", cliente.CodGiro);
                    cmd.Parameters.AddWithValue("@RutAux", cliente.Rut);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ActualizaCliente.Replace("{AREADATOS}", api.AreaDatos).Replace("{CODAUX}", cliente.CodAux).Replace("{REGION}", cliente.IdRegion.ToString())
                            .Replace("{COMAUX}", cliente.ComCod).Replace("{FONAUX1}", cliente.Telefono).Replace("{DIRAUX}", cliente.DirAux).Replace("{NOMAUX}", cliente.Nombre)
                            .Replace("{GIRAUX}", cliente.CodGiro).Replace("{AREADATOS}", api.AreaDatos);

                        if (!string.IsNullOrEmpty(cliente.DirNum)) { url = url.Replace("{DIRNUM}", cliente.DirNum); } else { url = url.Replace("dirnum={DIRNUM}&", ""); }
                        if (!string.IsNullOrEmpty(cliente.EmailDTE)) { url = url.Replace("{EMAILDTE}", cliente.EmailDTE).Replace("{ESRECEPTORDTE}", "S"); } else { url = url.Replace("eMailDte={EMAILDTE}&", "").Replace("{ESRECEPTORDTE}", "N"); }

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ActualizaCliente";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/UpdateAuxiliarPortalPagoAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/UpdateAuxiliarPortalPago"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false")
                { conSoftland.Close(); }
            }

            return true;
        }

        public async Task<List<DocumentosFacturadosDTO>> GetClientesComprasSoftlandAsync(string rutCliente, string logApiId)
        {
            List<DocumentosFacturadosDTO> retorno = new List<DocumentosFacturadosDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    var confSoftl = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    string[] cuentasContables = confSoftl.CuentasContablesDeuda.Split(';');
                    string cuentaContable = string.Empty;

                    foreach (var cuenta in cuentasContables)
                    {
                        cuentaContable = cuentaContable + $"'{cuenta}',";
                    }

                    cuentaContable = cuentaContable.Substring(0, cuentaContable.Length - 1);
                    conSoftland.Open();

                    DateTime rangoConsulta = DateTime.Now.AddDays(-30);
                    string mes = (rangoConsulta.Month < 10) ? "0" + rangoConsulta.Month.ToString() : rangoConsulta.Month.ToString();
                    string dia = (rangoConsulta.Day < 10) ? "0" + rangoConsulta.Day.ToString() : rangoConsulta.Day.ToString();
                    string año = rangoConsulta.Year.ToString();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "SELECT                           " + "\n" +
                                        "ttdoc.CodDoc,                  " + "\n" +
                                        "ttdoc.DesDoc as Documento,     " + "\n" +
                                        "GSAEN.Folio,                   " + "\n" +
                                        "GSAEN.Fecha,                   " + "\n" +
                                        "GSAEN.FechaVenc,               " + "\n" +
                                        "GSAEN.Total,                   " + "\n" +
                                        "GSAEN.nvnumero                 " + "\n" +
                                        "FROM softland.iw_gsaen AS GSAEN INNER JOIN softland.cwttdoc as ttdoc on ttdoc.CodDoc = GSAEN.TtdCod " + "\n" +
                                        "WHERE GSAEN.Tipo IN('F','B')   " + "\n" +
                                        "and GSAEN.EnMantencion <> -1   " + "\n" +
                                        "and GSAEN.Estado = 'V'         " + "\n" +
                                        "and GSAEN.CodAux = '" + rutCliente + "'  " + "\n" +
                                        "ORDER BY GSAEN.Fecha DESC";

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        DocumentosFacturadosDTO item = new DocumentosFacturadosDTO();
                        item.Movtipdocref = (reader["CodDoc"] == DBNull.Value) ? "" : reader["CodDoc"].ToString();
                        item.Documento = (reader["Documento"] == DBNull.Value) ? "" : reader["Documento"].ToString();
                        item.Nro = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Folio"]);
                        item.Femision = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                        item.Fvencimiento = (reader["FechaVenc"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["FechaVenc"]);
                        item.Monto = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Total"]);
                        item.NotaVenta = (reader["nvnumero"] == DBNull.Value) ? "" : reader["nvnumero"].ToString();

                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    var config = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    int anioActual = DateTime.Now.Year + 1;
                    while (anioActual > config.AnioTributario)
                    {
                        using (var client = new HttpClient())
                        {

                            anioActual = anioActual - 1;
                            string url = api.Url + api.DocumentosFacturados.Replace("{CODAUX}", rutCliente).Replace("{ANIO}", anioActual.ToString()).Replace("tipo={TIPO}&", "").Replace("{TOP}", "9999999999").Replace("{AREADATOS}", api.AreaDatos);

                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "DocumentosFacturados";


                            HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                List<List<DocumentosFacturadosAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentosFacturadosAPIDTO>>>(content);
                                documentos[0] = documentos[0].Where(x => x.Folio != null && x.Folio != 0 && x.Tipo_Documento.Substring(0, 1) != "S").OrderByDescending(x => x.Fecha).ToList();
                                foreach (var doc in documentos[0])
                                {
                                    DocumentosFacturadosDTO item = new DocumentosFacturadosDTO();
                                    item.Movtipdocref = doc.Tipo_Documento;
                                    item.Documento = doc.DocDes;
                                    item.Nro = (decimal)doc.Folio;
                                    item.Femision = Convert.ToDateTime(doc.Fecha);
                                    item.Fvencimiento = Convert.ToDateTime(doc.Vncimiento);
                                    item.Monto = (decimal)doc.Monto;
                                    item.CodMon = doc.CodMon;
                                    item.DesMon = doc.DesMon;
                                    item.MontoMonedaOriginal = doc.monto_moneda_original;
                                    item.NotaVenta = doc.Nvnumero.ToString();

                                    retorno.Add(item);
                                }
                            }
                            else
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/GetClientesComprasSoftlandAsync"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetClientesComprasSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }

            return retorno;
        }

        public async Task<List<NotaVentaClienteDTO>> GetNotasVentasPendientesAsync(string codAux, string logApiId)
        {
            List<NotaVentaClienteDTO> retorno = new List<NotaVentaClienteDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "select * from (SELECT NV.nvnumero,                          " + "\n" +
                                       " NV.nvFem,                                         " + "\n" +
                                       " nv.nvFeEnt,                                                " + "\n" +
                                       " NV.nvMonto,                                       " + "\n" +
                                       " sum(GSAEN.Total) as Facturado,                             " + "\n" +
                                       " (nv.nvMonto - sum(GSAEN.Total)) as Pendiente,    " + "\n" +
                                       "nv.nvEstado                                       " + "\n" +
                                       " FROM SOFTLAND.nw_nventa AS NV INNER JOIN softland.iw_gsaen AS GSAEN ON GSAEN.nvnumero = NV.NVNumero " + "\n" +
                                       " WHERE GSAEN.Tipo in ('F', 'B') " + "\n" +
                                       " AND nv.CodAux = '" + codAux + "' " + "\n" +
                                       " group by NV.NVNumero,nv.nvFem,nv.nvFeEnt, nv.nvmonto, GSAEN.Total, nv.nvEstado) as NV where nv.Pendiente > 100 order by nv.nvFem desc ";

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        NotaVentaClienteDTO item = new NotaVentaClienteDTO();
                        item.NVNumero = (reader["nvnumero"] == DBNull.Value) ? "" : reader["nvnumero"].ToString();
                        item.Fecha = (reader["nvFem"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["nvFem"]);
                        item.FechaEntrega = (reader["nvFeEnt"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["nvFeEnt"]);
                        item.Estado = (reader["nvEstado"] == DBNull.Value) ? "" : (reader["nvEstado"].ToString() == "P") ? "PENDIENTE" : (reader["nvEstado"].ToString() == "A") ? "APROBADO" : (reader["nvEstado"].ToString() == "C") ? "CONCLUIDA" : "";
                        item.Monto = (reader["nvMonto"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["nvMonto"]);
                        item.MontoFacturado = (reader["Facturado"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Facturado"]);
                        item.MontoPendiente = (reader["Pendiente"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Pendiente"]);
                        retorno.Add(item);

                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.PendientesPorFacturar.Replace("{AREADATOS}", api.AreaDatos).Replace("{CODAUX}", codAux);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "PendientesPorFacturar";

                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);


                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<DocumentoPendienteFacturar> documentosPendientes = JsonConvert.DeserializeObject<List<DocumentoPendienteFacturar>>(content);
                            documentosPendientes = documentosPendientes.Where(x => x.NVNumero != null && x.NVNumero != 0).OrderByDescending(x => x.NVNumero).ToList();
                            foreach (var doc in documentosPendientes)
                            {
                                NotaVentaClienteDTO item = new NotaVentaClienteDTO();
                                item.NVNumero = doc.NVNumero.ToString();
                                item.Fecha = doc.nvFem;
                                item.FechaEntrega = doc.nvFeEnt;
                                item.Estado = doc.nvEstado;
                                item.Monto = doc.nvMonto;
                                item.MontoFacturado = doc.Facturado;
                                item.MontoPendiente = doc.Pendiente_Facturar;
                                retorno.Add(item);
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetNotasVentasPendientesAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetNotasVentasPendientes"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }


            return retorno;
        }

        public async Task<List<ProductoDTO>> GetProductosCompradosAsync(string codAux, string logApiId)
        {
            List<ProductoDTO> retorno = new List<ProductoDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT                                       " + "\n" +
                                        "cwttdoc.DesDoc as Documento,               " + "\n" +
                                        "GSAEN.Folio,                               " + "\n" +
                                        "GSAEN.Fecha,                               " + "\n" +
                                        "CodProd,                                   " + "\n" +
                                        "DetProd,                                   " + "\n" +
                                        "CantFacturada as Cantidad,                 " + "\n" +
                                        "PreUniMB as Precio,                        " + "\n" +
                                        "iw_gmovi.TotLinea                          " + "\n" +
                                        "FROM softland.iw_gsaen AS GSAEN            " + "\n" +
                                        "INNER JOIN softland.iw_gmovi               " + "\n" +
                                        "on GSAEN.Tipo = iw_gmovi.Tipo              " + "\n" +
                                        "AND GSAEN.NroInt = iw_gmovi.NroInt         " + "\n" +
                                        "inner join softland.cwttdoc                " + "\n" +
                                        "on GSAEN.TtdCod = softland.cwttdoc.CodDoc  " + "\n" +
                                        "WHERE GSAEN.Tipo IN('F','B','N','D')       " + "\n" +
                                        "and GSAEN.EnMantencion <> -1               " + "\n" +
                                        "and GSAEN.Estado = 'V'                     " + "\n" +
                                        "and GSAEN.CodAux = '" + codAux + "'        " + "\n" +
                                        "ORDER BY GSAEN.Fecha DESC";

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ProductoDTO item = new ProductoDTO();
                        item.CodProd = (reader["CodProd"] == DBNull.Value) ? "" : reader["CodProd"].ToString();
                        item.DesProd = (reader["DetProd"] == DBNull.Value) ? "" : reader["DetProd"].ToString();
                        item.Cantidad = (reader["Cantidad"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Cantidad"]);
                        item.Precio = (reader["Precio"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Precio"]);
                        item.TotalLinea = (reader["TotLinea"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["TotLinea"]);
                        item.Documento = (reader["Documento"] == DBNull.Value) ? "" : reader["Documento"].ToString();
                        item.Folio = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Folio"]);
                        item.Fecha = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ProductosComprados.Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ProductosComprados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ProductoAPIDTO>> listaProductos = JsonConvert.DeserializeObject<List<List<ProductoAPIDTO>>>(content);
                            listaProductos[0] = listaProductos[0].Where(x => x.Folio != null && x.Folio != 0).OrderByDescending(x => x.Fecha).ToList();
                            foreach (var producto in listaProductos[0])
                            {
                                ProductoDTO item = new ProductoDTO();
                                item.CodProd = producto.CodProd;
                                item.DesProd = producto.DetProd;
                                item.Cantidad = (int)producto.CantFacturada;
                                item.Precio = (decimal)producto.PreUniMB;
                                item.TotalLinea = (decimal)producto.TotalLinea;
                                item.Documento = producto.DesDoc;
                                item.Folio = producto.Folio;
                                item.Fecha = producto.Fecha;
                                item.TipoDoc = producto.Tipo + producto.SubTipo;
                                retorno.Add(item);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetProductosCompradosAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetProductosComprados"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }

            return retorno;
        }

        public DataTable obtenerCabeceraDataTable(int folio, string tipoDoc, string codAux)
        {
            conSoftland.Open();
            DataTable dtResultado = new DataTable();

            string sqlCabecera = "SELECT " +
                                    "     f.Folio                                                                                           " +
                                    "	,ISNULL(aux.RutAux, '') as Rut                                                                      " +
                                    "	,ISNULL(aux.NomAux, '') as RazonSocial                                                              " +
                                    "	,ISNULL((select gir.GirDes from softland.CWTGiro gir where gir.GirCod = aux.GirAux),'') as Giro     " +
                                    "	,ISNULL((select ciu.CiuDes from softland.cwtciud ciu where ciu.CiuCod = aux.CiuAux),'') as Ciudad   " +
                                    "	,ISNULL((select com.ComDes from softland.cwtcomu com where com.ComCod = aux.ComAux),'') as Comuna   " +
                                    "	,ISNULL(aux.DirAux, '') as Direccion                                                                " +
                                    "	,ISNULL(f.Glosa, '') as Descripcion                                                                 " +
                                    "	,f.Fecha as FechaEmision                                                                            " +
                                    "   ,f.FechaVenc as FechaVencimiento                                                                    " +
                                    "	,ISNULL(aux.FonAux1, '') as Telefono                                                                " +
                                    "	,ISNULL(c.CveDes, '') as CondVenta                                                                  " +
                                    "	,ISNULL(v.VenDes, '') as Vendedor                                                                   " +
                                    "	,ISNULL(f.Patente, '') as Patente                                                                   " +
                                    "	,ISNULL(f.NetoAfecto, 0) as Neto                                                                    " +
                                    "	,ISNULL(f.Descto01, 0) as Descuento                                                                 " +
                                    "	,CAST(ISNULL(f.IVA, 0) AS INT) as Iva                                                                            " +
                                    "	,ISNULL(f.Total, 0) as Total                                                                        " +
                                    "   , f.Tipo                                                                                            " +
                                    "   , f.nvnumero                                                                                         " +
                                    " FROM softland.iw_gsaen f                                                                               " +
                                    " INNER JOIN softland.cwtauxi aux                                                                        " +
                                    " on aux.CodAux = f.CodAux                                                                               " +
                                    " LEFT JOIN softland.cwtconv c                                                                           " +
                                    " on c.CveCod = f.CondPago                                                                               " +
                                    " LEFT JOIN softland.cwtvend v                                                                           " +
                                    " ON v.VenCod = f.CodVendedor                                                                            " +
                                    " WHERE f.Folio =" + folio +
                                    //" AND f.Tipo = '" + tipoDoc + "'                                                                          " +
                                    " AND f.CodAux = '" + codAux + "' AND Tipo in ('B','F'); ";
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = sqlCabecera;

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                dtResultado.Load(reader);


                reader.Close();
                conSoftland.Close();


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerCabecera"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        public DataTable obtenerFirmaDTE(int folio, string tipoDoc, string codAux)
        {
            conSoftland.Open();
            DataTable dtResultado = new DataTable();

            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "Select Doc.FirmaDTE                      " +
                                "   FROM softland.IW_GSaEn G                " +
                                "   LEFT JOIN softland.DTE_DocCab Doc       " +
                                "   ON G.Tipo = Doc.Tipo                    " +
                                "   AND G.NroInt = Doc.NroInt               " +
                                "   WHERE G.Tipo = '" + tipoDoc + "'                      " +
                                "   AND G.CodAux = '" + codAux + "'                " +
                                "   AND G.Folio =" + folio;

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                dtResultado.Load(reader);


                reader.Close();
                conSoftland.Close();

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerFirmaDTE"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        public DataTable obtenerDetalleDataTable(int folio, string tipoDoc, string codAux)
        {
            conSoftland.Open();
            DataTable dtResultado = new DataTable();

            string sqlDetalle = "SELECT                                     " +
                            "    p.CodProd as Codigo                        " +
                            "	,p.DesProd                                  " +
                            "	,d.CantFacturada as Cantidad                " +
                            "	,p.CodUMed as CodUMed                       " +
                            "	,u.DesUMed as UMed                          " +
                            "	,CASE WHEN f.tipo = 'F' then d.PreUniMB else CASE WHEN f.tipo = 'B' then  d.PreUniBoleta end end as PrecioUnitario               " +
                            "	,d.DescMov01 as Descuento                   " +
                            "	,(CASE WHEN f.tipo = 'F' then d.PreUniMB else CASE WHEN f.tipo = 'B' then  d.PreUniBoleta end end * d.CantFacturada) AS Total    " +
                            " FROM softland.iw_gsaen f                       " +
                            " INNER JOIN softland.IW_GMOVI d                 " +
                            " on d.NroInt = f.NroInt                         " +
                            " and d.Tipo = f.Tipo                            " +
                            " INNER JOIN softland.iw_tprod p                 " +
                            " LEFT JOIN softland.iw_tumed u                  " +
                            " on u.CodUMed = p.CodUMed                       " +
                            " ON p.CodProd = d.CodProd                       " +
                            " WHERE f.Folio = " + folio +
                            " AND f.Tipo = '" + tipoDoc + "'                               " +
                            " AND f.CodAux = '" + codAux + "'";
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = sqlDetalle;

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();

                dtResultado.Load(reader);

                reader.Close();
                conSoftland.Close();


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerDetalle"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        public DataTable obtenerReferencia(int folio, string tipoDoc, string codAux)
        {
            conSoftland.Open();
            DataTable dtResultado = new DataTable();

            string sqlReferencia = "SELECT r.CodRefSII, doc.DesRefSII, r.FolioRef, r.FechaRef " +
                                    " from softland.iw_gsaen_refdte r                          " +
                                    " INNER JOIN softland.iw_gsaen f                           " +
                                    " on r.NroInt = f.NroInt                                   " +
                                    " and r.Tipo = f.Tipo                                      " +
                                    " INNER JOIN softland.DTE_SiiTDocRef doc                   " +
                                    " ON doc.CodRefSII = r.CodRefSII                           " +
                                    " WHERE f.Folio = " + folio +
                                    " AND f.Tipo = '" + tipoDoc + "'                                         " +
                                    " AND f.CodAux = '" + codAux + "'; ";

            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = sqlReferencia;

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                dtResultado.Load(reader);


                reader.Close();
                conSoftland.Close();


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerReferencia"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        public async Task<DocumentosVm> obtenerXMLDTEAsync(int folio, string codAux, string tipoDoc, string logApiId)
        {

            DocumentosVm dtResultado = new DocumentosVm();

            try
            {

                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select arch.Archivo as Archivo, dot.Archivo as Nombre,f.Tipo      " +
                                        "from softland.iw_gsaen f                   " +
                                        "INNER JOIN softland.dte_doccab dot         " +
                                        "ON dot.Tipo = f.Tipo                       " +
                                        "AND dot.NroInt = f.NroInt                  " +
                                        "INNER JOIN softland.dte_archivos arch      " +
                                        "on arch.NroInt = f.NroInt                  " +
                                        "and arch.Folio = f.Folio                   " +
                                        "AND arch.ID_Archivo = dot.IDXMLDoc         " +
                                        "WHERE f.CodAux = '" + codAux.Split('-')[0].Replace(".", "") + "'                " +
                                        "AND f.Folio =" + folio;

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        dtResultado.Base64 = (reader["Archivo"] == DBNull.Value) ? "" : reader["Archivo"].ToString();
                        dtResultado.NombreArchivo = (reader["Nombre"] == DBNull.Value) ? "" : reader["Nombre"].ToString();
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.DetalleDocumento.Replace("{FOLIO}", folio.ToString()).Replace("{SUBTIPO}", tipoDoc.Substring(1, 1)).Replace("{TIPODOC}", tipoDoc.Substring(0, 1)).Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DetalleDocumento";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            DocumentoAPIDTO doc = JsonConvert.DeserializeObject<DocumentoAPIDTO>(content);
                            if (doc.dte != null)
                            {

                                dtResultado.Base64 = doc.dte.archivo;
                                dtResultado.NombreArchivo = doc.dte.nombre_archivo;
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/obtenerXMLDTE"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerXMLDTE"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }
            return dtResultado;
        }

        public List<DespachosDTO> GetDespachosDocumento(int folio, string logApiId)
        {
            List<DespachosDTO> retorno = new List<DespachosDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "select CASE when iw_gsaen.Tipo = 'F' then 'Factura' end  as Documento,  " +
                                  "  iw_gsaen.Folio,                                                       " +
                                  "  iw_gsaen.Fecha,                                                       " +
                                  "  iw_gsaen.NroInt,                                                      " +
                                  "  iw_gsaen.Tipo                                                         " +
                                  "  from softland.iw_gsaen                                                " +
                                  "  inner join softland.iw_gmovi as movi                                  " +
                                  "  on movi.Tipo = iw_gsaen.Tipo                                          " +
                                  "  and movi.NroInt = iw_gsaen.NroInt                                     " +
                                  "  where iw_gsaen.Tipo = 'F'                                             " +
                                  "  AND Folio = " + folio +
                                  "  and movi.CantFacturada <> 0                                           " +
                                  "  and SubTipoDocto = 'T'                                                " +
                                  "  union                                                                 " +
                                  "  select CASE when Tipo = 'S' then 'Guia Despacho' end as Documento,    " +
                                  "  Folio,                                                                " +
                                  "  Fecha,                                                                " +
                                  "  NroInt,                                                               " +
                                  "  Tipo                                                                  " +
                                  "  from softland.iw_gsaen                                                " +
                                  "  where Concepto = '02'                                                 " +
                                  "  and Tipo = 'S'                                                        " +
                                  "  AND Factura = " + folio +
                                  "  and SubTipoDocto = 'T'";

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    DespachosDTO item = new DespachosDTO();
                    item.Documento = (reader["Documento"] == DBNull.Value) ? "" : reader["Documento"].ToString();
                    item.Folio = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Folio"]);
                    item.Fecha = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                    item.NroInt = (reader["NroInt"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["NroInt"]);
                    item.Tipo = (reader["Tipo"] == DBNull.Value) ? "" : reader["Tipo"].ToString();
                    retorno.Add(item);
                }
                reader.Close();
                conSoftland.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDespachosDocumento"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

                conSoftland.Close();
            }


            return retorno;
        }

        public async Task<List<DespachosDTO>> GetDepachoDocumentoAPIAsync(int folio, string tipoDoc, string codaux, string logApiId)
        {
            List<DespachosDTO> retorno = new List<DespachosDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.DespachoDeDocumento.Replace("{TIPO}", tipoDoc.Substring(0, 1)).Replace("{FOLIO}", folio.ToString()).Replace("{CODAUX}", codaux).Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DespachoDeDocumento";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<DespachoAPIDTO>> listaDespachos = JsonConvert.DeserializeObject<List<List<DespachoAPIDTO>>>(content);
                        listaDespachos[1] = listaDespachos[1].Where(x => x.CantDespachada > 0).ToList();

                        var documentoOriginal = await this.obtenerDocumentoAPI(folio, tipoDoc, codaux, logApiId);
                        foreach (var detalleDespacho in listaDespachos[1])
                        {
                            DespachosDTO item = new DespachosDTO();
                            if (detalleDespacho.TipoDocto == "F")
                            {
                                item.Documento = "Factura";
                            }
                            else if (detalleDespacho.TipoDocto == "S")
                            {
                                item.Documento = "Guia Despacho";
                            }

                            DespachoDetalleDTO detalle = new DespachoDetalleDTO();
                            item.Cantidad = detalleDespacho.CantDespachada;
                            item.CodProducto = detalleDespacho.CodProd;
                            item.DesProducto = detalleDespacho.DetProd;
                            item.Total = detalleDespacho.TotLinea;
                            item.Folio = (int)detalleDespacho.Folio;
                            item.Fecha = detalleDespacho.Fecha;
                            item.Tipo = detalleDespacho.Tipo;

                            if (documentoOriginal.detalle.Count > 0)
                            {
                                var producto = documentoOriginal.detalle.Where(x => x.Codigo == item.CodProducto).FirstOrDefault();
                                if (producto != null)
                                {
                                    item.CantidadFacturada = producto.Cantidad;
                                }
                            }

                            retorno.Add(item);
                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetDepachoDocumentoAPIAsync"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDepachoDocumentoAPIAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            return retorno;

        }

        public List<DespachoDetalleDTO> GetDetalleDespacho(int nroInt, string logApiId)
        {
            List<DespachoDetalleDTO> retorno = new List<DespachoDetalleDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "select CodProd,CantDespachada,DetProd, TotLinea from [softland].[iw_gmovi] where NroInt =" + nroInt + " and Tipo = 'S'";

                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    DespachoDetalleDTO item = new DespachoDetalleDTO();
                    item.CodProducto = (reader["CodProd"] == DBNull.Value) ? "" : reader["CodProd"].ToString();
                    item.DesProducto = (reader["DetProd"] == DBNull.Value) ? "" : reader["DetProd"].ToString();
                    item.Cantidad = (reader["CantDespachada"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["CantDespachada"]);
                    item.Total = (reader["TotLinea"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["TotLinea"]);
                    retorno.Add(item);
                }
                reader.Close();
                conSoftland.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDetalleDespacho"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }


            return retorno;
        }

        public async Task<List<ContactoDTO>> GetContactosClienteAsync(string codaux, string logApiId)
        {
            List<ContactoDTO> retorno = new List<ContactoDTO>();

            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader result;
                    cmd.CommandText = "select NomCon, Email, FonCon, car.CarCod, car.CarNom " +
                                      "  from softland.cwtaxco c " +
                                      "  LEFT JOIN softland.cwtcarg car " +
                                      "  on car.CarCod = c.CarCon where CodAuc = '" + codaux + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    result = cmd.ExecuteReader();

                    while (result.Read())
                    {
                        ContactoDTO item = new ContactoDTO();
                        item.Correo = (result["Email"] != DBNull.Value) ? result["Email"].ToString() : "";
                        item.NombreContacto = (result["NomCon"] != DBNull.Value) ? result["NomCon"].ToString() : "";
                        item.Telefono = (result["FonCon"] != DBNull.Value) ? result["FonCon"].ToString() : "";
                        item.CodCargo = (result["CarCod"] != DBNull.Value) ? result["CarCod"].ToString() : "";
                        item.CargoContacto = (result["CarNom"] != DBNull.Value) ? result["CarNom"].ToString() : "";
                        retorno.Add(item);
                    }
                    result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ContactosXauxiliar.Replace("{CODAUX}", codaux).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ContactosXauxiliar";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ContactoApiDTO>> listaContactos = JsonConvert.DeserializeObject<List<List<ContactoApiDTO>>>(content);
                            listaContactos[0] = listaContactos[0].Where(x => x.Email != null && x.Email != "").ToList();
                            foreach (var item in listaContactos[0])
                            {
                                ContactoDTO aux = new ContactoDTO();
                                aux.NombreContacto = item.NomCon;
                                aux.Correo = item.Email;
                                aux.CargoContacto = item.CarNom;
                                aux.CodCargo = item.CarCon;
                                retorno.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetContactosCliente"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetContactosCliente"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();

                }
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        public async Task<NotaVentaClienteDTO> GetNotaVentaAsync(int nvNumero, string codAux, string logApiId)
        {
            NotaVentaClienteDTO item = new NotaVentaClienteDTO();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select NVNumero,  " +
                                     "       nvEstado,   " +
                                     "       nvFem,      " +
                                     "       a.NomAux,   " +
                                     "       a.DirAux,   " +
                                     "       c.CveDes,   " +
                                     "       v.VenDes,   " +
                                     "       nv.nvSubTotal, " +
                                     "       (select SUM(Impto) from softland.nw_impto where nvNumero = nv.NVNumero) as Impuestos, " +
                                     "       nv.nvMonto " +
                                     "   from softland.nw_nventa nv     " +
                                     "   INNER JOIN softland.cwtauxi a  " +
                                     "   on a.CodAux = nv.CodAux        " +
                                     "   LEFT JOIN softland.cwtconv c   " +
                                     "   on c.cveCod = nv.cveCod        " +
                                     "   LEFT JOIN softland.cwtvend v   " +
                                     "   on v.VenCod = nv.VenCod        " +
                                     "   where NVNumero =" + nvNumero;

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {

                        item.NVNumero = (reader["NVNumero"] == DBNull.Value) ? "" : reader["NVNumero"].ToString();
                        item.Fecha = (reader["nvFem"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["nvFem"]);
                        item.Estado = (reader["nvEstado"] == DBNull.Value) ? "" : (reader["nvEstado"].ToString() == "P") ? "PENDIENTE" : (reader["nvEstado"].ToString() == "A") ? "APROBADO" : (reader["nvEstado"].ToString() == "C") ? "CONCLUIDA" : "";
                        item.Cliente = (reader["NomAux"] == DBNull.Value) ? "" : reader["NomAux"].ToString();
                        item.Direccion = (reader["DirAux"] == DBNull.Value) ? "" : reader["DirAux"].ToString();
                        item.CondicionVenta = (reader["CveDes"] == DBNull.Value) ? "" : reader["CveDes"].ToString();
                        item.Vendedor = (reader["VenDes"] == DBNull.Value) ? "" : reader["VenDes"].ToString();
                        item.SubTotal = (reader["nvSubTotal"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["nvSubTotal"]);
                        item.Impuestos = (reader["Impuestos"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["Impuestos"]);
                        item.Monto = (reader["nvMonto"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["nvMonto"]);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.PendientesPorFacturar.Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "PendientesPorFacturar";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<DocumentoPendienteFacturar> documentosPendientes = JsonConvert.DeserializeObject<List<DocumentoPendienteFacturar>>(content);

                            var nv = documentosPendientes.Where(x => x.NVNumero == nvNumero).FirstOrDefault();
                            if (nv != null)
                            {
                                item.NVNumero = nv.NVNumero.ToString();
                                item.Fecha = nv.nvFem;
                                item.Estado = nv.nvEstado;
                                item.Cliente = nv.NomAux;
                                item.Direccion = nv.DirAux;
                                item.Vendedor = nv.VenDes;
                                item.SubTotal = nv.nvSubTotal;
                                decimal impuesto = 0;
                                foreach (var imp in nv.Impuestos)
                                {
                                    impuesto += (decimal)imp.Monto;
                                }
                                item.Impuestos = impuesto;
                                item.Monto = nv.nvMonto;
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetNotaVentaAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetNotaVenta"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }

            }


            return item;
        }

        public async Task<List<NotaVentaDetalleDTO>> GetNotaVentaDetalleAsync(int nvNumero, string logApiId)
        {
            List<NotaVentaDetalleDTO> retorno = new List<NotaVentaDetalleDTO>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select CodProd, DetProd, nvCant, nvPrecio, nvTotLinea  from softland.nw_detnv where NVNumero =" + nvNumero;

                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        NotaVentaDetalleDTO item = new NotaVentaDetalleDTO();
                        item.CodProducto = (reader["CodProd"] == DBNull.Value) ? "" : reader["CodProd"].ToString();
                        item.DesProducto = (reader["DetProd"] == DBNull.Value) ? "" : reader["DetProd"].ToString();
                        item.Cantidad = (reader["nvCant"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["nvCant"]);
                        item.Precio = (reader["nvPrecio"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["nvPrecio"]);
                        item.Total = (reader["nvTotLinea"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["nvTotLinea"]);
                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.DetalleNotaDeVenta.Replace("{NVNUMERO}", nvNumero.ToString()).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DetalleNotaDeVenta";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<NotaVentaDetalleAPIDTO>> detalle = JsonConvert.DeserializeObject<List<List<NotaVentaDetalleAPIDTO>>>(content);
                            detalle[0] = detalle[0].Where(x => x.NVNumero != null && x.NVNumero != 0).ToList();
                            foreach (var det in detalle[0])
                            {
                                NotaVentaDetalleDTO item = new NotaVentaDetalleDTO();

                                item.CodProducto = det.CodProd;
                                item.DesProducto = det.DetProd;
                                item.Cantidad = (int?)det.nvCant;
                                item.Precio = (decimal)det.nvPrecio;
                                item.Total = (decimal)det.TotalLinea;
                                retorno.Add(item);
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetNotaVentaDetalleAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetNotaVentaDetalleAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();
                }
            }


            return retorno;
        }

        public async Task<List<ClienteSaldosDTO>> GetClienteEstadoCuentaAsync(string codaux, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            string tablaTemporal = string.Empty;
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    var configuracionPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                    string docs = string.Empty;

                    foreach (var item in configuracionPago.TiposDocumentosDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(docs))
                        {
                            docs = "'" + item + "'";
                        }
                        else
                        {
                            docs = docs + ",'" + item + "'";
                        }
                    }

                    string cuentasContables = string.Empty;
                    foreach (var item in configuracionPago.CuentasContablesDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(cuentasContables))
                        {
                            cuentasContables = "'" + item + "'";
                        }
                        else
                        {
                            cuentasContables = cuentasContables + ",'" + item + "'";
                        }
                    }

                    string fechaTabla = ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()) +
                                   ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) +
                                    DateTime.Now.Year.ToString() +
                                   ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) +
                                   ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString()) +
                                   ((DateTime.Now.Second < 10) ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString());

                    tablaTemporal = "CW" + codaux + fechaTabla + "_";

                    string fecha = DateTime.Now.Year.ToString() + "/" + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "/" + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());


                    SqlCommand cmd = new SqlCommand("softland.cw_psnpConsultaDetalleCtaConDoc05", conSoftland);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("xblnSoloSaldo", 0);
                    cmd.Parameters.AddWithValue("xstrMoneda ", configuracionPago.MonedaUtilizada);
                    cmd.Parameters.AddWithValue("xstrFecha", fecha);
                    cmd.Parameters.AddWithValue("xvarAuxiliar", codaux);
                    cmd.Parameters.AddWithValue("xvarCuenta", "");
                    cmd.Parameters.AddWithValue("xvarTipoDocto", ""); //TIPO DOCUMENTO, DEJAR EN BLANCO PARA QUE OBTENGA TODOS Y LUEGO FILTRAMOS EN SIGUIETNE QUERY
                    cmd.Parameters.AddWithValue("xvarAreaNegocio", "");
                    cmd.Parameters.AddWithValue("xIncluyePagos", "");
                    cmd.Parameters.AddWithValue("opt_NroOper", 0);
                    cmd.Parameters.AddWithValue("ctlType_NroOper", "");
                    cmd.Parameters.AddWithValue("mstrAnoPrimerPeriodoFull", configuracionPago.AnioTributario);
                    cmd.Parameters.AddWithValue("AnoIni", configuracionPago.AnioTributario);
                    cmd.Parameters.AddWithValue("mfExistenMovAperturaPeriodo1", 0);
                    cmd.Parameters.AddWithValue("optAuxiliares", 0);
                    cmd.Parameters.AddWithValue("pstrCpbNumTemp", tablaTemporal + "1"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_1 (CW + usuario + fecha + minutos + segundos + _ + 1)
                    cmd.Parameters.AddWithValue("pstrQTemp ", tablaTemporal + "0"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_0 (CW + usuario + fecha + minutos + segundos + _ + 0) Esta tabla sera usada para la consulta final
                    cmd.Parameters.AddWithValue("pstrMontoTemp", tablaTemporal + "2"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_2 (CW + usuario + fecha + minutos + segundos + _ + 2)
                    cmd.Parameters.AddWithValue("pstrSaldoTemp", tablaTemporal + "3"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_3 (CW + usuario + fecha + minutos + segundos + _ + 3)
                    cmd.Parameters.AddWithValue("pstrMovFe", tablaTemporal + "4"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_4 (CW + usuario + fecha + minutos + segundos + _ + 4)
                    cmd.Parameters.AddWithValue("pstrMovFv", tablaTemporal + "5"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_5 (CW + usuario + fecha + minutos + segundos + _ + 5)
                    cmd.Parameters.AddWithValue("pstrCpbFec", tablaTemporal + "6"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_6 (CW + usuario + fecha + minutos + segundos + _ + 6)
                    cmd.Parameters.AddWithValue("mstrPais", "CL");
                    cmd.Parameters.AddWithValue("pstrManejaCC", "N");
                    cmd.Parameters.AddWithValue("pstrCCosto", "");
                    cmd.Parameters.AddWithValue("pintNivelCC", 1);
                    cmd.Parameters.AddWithValue("pstrPagoOtraArea", "N");

                    conSoftland.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conSoftland.Close();
                        da.Fill(dt);
                    }

                    //Validamos si ejecuciòn fue exitos
                    if (dt.Rows.Count > 0)
                    {
                        conSoftland.Open();
                        SqlDataReader reader;
                        cmd.CommandText = "select pctcod as CuentaContable," +
                                      "  CpbNum as Comprobante, " +
                                      "  movtipdocref as TipoDoc, " +
                                      "  (Select top 1 tdoc.DesDoc from softland.cwttdoc as tdoc where tdoc.CodDoc = portal.MovTipDocRef) as Documento,  " +
                                      "  portal.MovNumDocRef as Nro, " +
                                      "  (select top 1 cmovi.MovFe from softland.cwmovim as cmovi " +
                                      "  where CodAux = portal.CodAux  and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                      "  (cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                      "  (cmovi.CpbNum = portal.CpbNum)) as Femision, " +
                                      "  (select top 1 cmovi.MovFv from softland.cwmovim as cmovi " +
                                      "  where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                      "  (cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                      "  (cmovi.CpbNum = portal.CpbNum)) as Fvencimiento, " +
                                      "  (select top 1 cmovi.MovDebe from softland.cwmovim as cmovi " +
                                      "  where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                      "  (cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                      "  (cmovi.CpbNum = portal.CpbNum)) as Debe, " +
                                      "  (select cwtauxi.NomAux from softland.cwtauxi where codaux = portal.CodAux) as RazonSocial, " +
                                      "  portal.Saldo, " +
                                      "  CASE WHEN(select top 1 cmovi.MovFv from softland.cwmovim as cmovi " +
                                      "  where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                      "  (cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                      "  (cmovi.CpbNum = portal.CpbNum)) <= GETDATE() THEN 'Vencido' else 'Pendiente' end as estado " +
                                      "  from SOFTLAND." + tablaTemporal + "0" + " as portal where Saldo > 0 and movtipdocref in (" + docs + ") and pctcod in (" + cuentasContables + ");";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conSoftland;

                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            ClienteSaldosDTO item = new ClienteSaldosDTO();
                            item.CuentaContable = reader["CuentaContable"].ToString();
                            item.ComprobanteContable = reader["Comprobante"].ToString();
                            item.Documento = reader["Documento"].ToString();
                            item.Nro = (reader["Nro"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Nro"]);
                            item.FechaEmision = (reader["Femision"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Femision"]);
                            item.FechaVcto = (reader["Fvencimiento"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fvencimiento"]);
                            item.Debe = (reader["Debe"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Debe"]);
                            item.Saldo = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                            item.APagar = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                            item.Estado = reader["Estado"].ToString();
                            item.TipoDoc = reader["TipoDoc"].ToString();
                            item.RazonSocial = (reader["RazonSocial"] == DBNull.Value) ? "" : reader["RazonSocial"].ToString();
                            retorno.Add(item);
                        }
                        reader.Close();
                        conSoftland.Close();
                    }

                    conSoftland.Open();
                    SqlCommand cmdElimina = new SqlCommand("DROP TABLE IF EXISTS SOFTLAND." + tablaTemporal + "0;");
                    cmdElimina.CommandType = CommandType.Text;
                    cmdElimina.Connection = conSoftland;
                    cmdElimina.ExecuteNonQuery();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {

                        var monedas = await this.GetMonedasAsync(logApiId);
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        string accesToken = api.Token;
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                        string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        int cantidad = 100;
                        int pagina = 1;
                        if (string.IsNullOrEmpty(codaux))
                        {
                            codaux = string.Empty;
                        }
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codaux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {

                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);

                            if (documentos[0].Count > 0)
                            {
                                pagina = pagina + 1;

                                while (documentos[0].Count < documentos[0][0].total)
                                {
                                    using (var client2 = new HttpClient())
                                    {

                                        string url2 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codaux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");


                                        client2.BaseAddress = new Uri(url2);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "DocumentosContabilizados";


                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);

                                            documentos[0].AddRange(documentos2[0]);
                                            pagina = pagina + 1;
                                        }
                                        else
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response2.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content2,
                                                Ruta = "SoftlandService/GetClienteEstadoCuenta"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                        }

                                    }
                                }

                                //var AbonosDocumentos = documentos[0].Where(x => x.MovNumDocRef != x.Numdoc).ToList();
                                documentos[0] = documentos[0].Where(x => x.Saldobase >= 0 && x.Numdoc == x.MovNumDocRef).ToList();

                                retorno = documentos[0].ConvertAll(doc => new ClienteSaldosDTO
                                {

                                    //item.comprobanteContable = reader["Comprobante"].ToString();
                                    Documento = doc.DesDoc,
                                    Nro = (double)doc.Numdoc,
                                    FechaEmision = Convert.ToDateTime(doc.Movfe),
                                    FechaVcto = Convert.ToDateTime(doc.Movfv),
                                    Debe = (double)doc.MovMontoMa,
                                    Haber = doc.Saldoadic,
                                    Saldo = (double)doc.Saldoadic,
                                    Detalle = "", // reader["Detalle"].ToString();
                                    Estado = doc.Estado,
                                    Pago = "", // reader["Pago"].ToString();
                                    TipoDoc = doc.Ttdcod,
                                    RazonSocial = "",
                                    CodigoMoneda = doc.MonCod,
                                    CodAux = doc.CodAux,
                                    MontoBase = doc.MovMonto,
                                    SaldoBase = doc.Saldobase,
                                    EquivalenciaMoneda = doc.Equivalencia,
                                    APagar = doc.Saldobase,
                                    MontoOriginalBase = doc.MontoOriginalBase,
                                    MovEqui = doc.MovEquiv,
                                    DesMon = monedas.Where(x => x.CodMon == doc.MonCod).FirstOrDefault() != null ? monedas.Where(x => x.CodMon == doc.MonCod).FirstOrDefault().DesMon : ""
                                });

                                if (retorno.Count > 0)
                                {
                                    var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                                    foreach (var pago in pagosPendientes)
                                    {
                                        foreach (var item in retorno)
                                        {
                                            var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Nro && x.TipoDocumento == item.TipoDoc).FirstOrDefault();
                                            if (detalle != null)
                                            {
                                                item.SaldoBase = item.SaldoBase - detalle.Apagar;
                                                item.APagar = item.SaldoBase;
                                            }
                                        }
                                    }

                                }
                            }



                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetClienteEstadoCuenta"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Close();

                    //Elimina tabla temporal
                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand("DROP TABLE IF EXISTS SOFTLAND." + tablaTemporal + "0;");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    cmd.ExecuteNonQuery();
                    conSoftland.Close();
                }
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetClienteEstadoCuenta"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async Task<bool> verificaEstadoConexionSoftlandAsync(string logApiId)
        {
            bool conexionExitosa = false;
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    conSoftland.Open();
                    conSoftland.Close();

                    conexionExitosa = true;
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaMonedas.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaMonedas";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            conexionExitosa = true;
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/verificaEstadoConexionSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/verificaEstadoConexionSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();

            }
            return conexionExitosa;
        }

        public async Task<PagoComprobanteVm> GeneraComprobantesContablesAsync(int idPago, string codigoAutorizacionPasarela, string logApiId)
        {
            string numeroComprobante = string.Empty;
            string numPago = string.Empty;

            try
            {

                if (utilizaApiSoftland == "false")
                {
                    MaestroPagoEstadoCuenta ma = new MaestroPagoEstadoCuenta();

                    //Obtiene Pago y su detalle
                    var item = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                    //Validamos si obtenemos el codigo auxiliar del cliente logeado en el portal o del cliente que realizo un pago rapido
                    string codAux = string.Empty;
                    if (item.EsPagoRapido == 1) //Pago realizado desde boton pago rapido
                    {
                        codAux = item.CodAux;
                    }
                    else //Pago realizado desde el estado de cuenta del portal
                    {
                        codAux = item.IdClienteNavigation.CodAux;
                    }

                    //Declara variables mes y año en el cual se generara comprobante contable
                    string mes = item.FechaPago.Value.ToString("MM");
                    string año = item.FechaPago.Value.ToString("yyyy");

                    //Obtiene numero comprabante disponible por correlativo
                    numeroComprobante = ma.GetNumeroComprobanteContable(año, mes);

                    //Inserta cabecera comprobante contable
                    Boolean s1 = ma.SaveComprobanteCabecera(mes, año, numeroComprobante, codAux);
                    if (s1)
                    {
                        //Variable correlativo para los movimientos que se generaran, primer registros parte en 0
                        int correlativo = 0;

                        //Inserta como movimiento cada documento seleccionado para el pago
                        foreach (var det in item.PagosDetalles)
                        {
                            Boolean s2 = ma.SaveComprobanteDetalleDoc(mes, año, numeroComprobante, det, correlativo, codigoAutorizacionPasarela, codAux, Convert.ToInt32(item.IdPasarela));
                            correlativo = correlativo + 1;
                        }

                        //Inserta contrapartida
                        Boolean s3 = ma.SaveComprobanteContrapartida(mes, año, numeroComprobante, item, correlativo, codigoAutorizacionPasarela, codAux);

                        //Actualiza pago agregando comprobante contable generado
                        item.ComprobanteContable = numeroComprobante;
                        item.IdPagoEstado = 2; //ESTADO PAGADO
                        _context.PagosCabeceras.Attach(item);
                        _context.Entry(item).Property(x => x.ComprobanteContable).IsModified = true;
                        _context.Entry(item).Property(x => x.IdPagoEstado).IsModified = true;
                        _context.SaveChanges();

                        try
                        {


                            //AQUI DEBE ENVIAR CORREO A CLIENTE CON COMPROBANTE PDF DEL PAGO TRANSBANK TAL COMO LO HACE LA TIENDA Y OCUPAR STREAM 
                            //AQUI DEBE ENVIAR CORREO A EMPRESA A LOS DESTINATARIOS CONFIGURADOS EN LA TABLA CONFIGURACIONCORREO CON COMPROBANTE PDF DEL PAGO TRANSBANK TAL COMO LO HACE LA TIENDA Y OCUPAR STREAM  
                            var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                            var configDiseno = _context.ConfiguracionDisenos.FirstOrDefault();
                            string fecha = DateTime.Now.ToString("dd/MM/yyyy");
                            string hora = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString();
                            string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;

                            string comprobanteHtml = System.IO.File.ReadAllText(Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"), "~/Uploads/MailTemplates/compraMailToPDF.component.html"));
                            comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                                .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora);

                            string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                            string reemplazoDetalle = string.Empty;

                            SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                            var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApiId);
                            foreach (var det in item.PagosDetalles)
                            {
                                var tipoDoc = tiposDocumentos.Where(x => x.CodDoc == det.TipoDocumento).FirstOrDefault();

                                reemplazoDetalle = reemplazoDetalle + partes[1].Replace("{NUMDOC}", det.Folio.ToString()).Replace("{TIPODOC}", tipoDoc.DesDoc).Replace("{MONTODOC}", det.Total.ToString().Replace(",", ".")).Replace("{SALDODOC}", det.Saldo.ToString().Replace(",", "."))
                                    .Replace("{PAGADODOC}", det.Apagar.ToString().Replace(",", "."));

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
                            vm.nombre = item.Nombre;
                            vm.email_destinatario = item.Correo;
                            MailService mail = new MailService(_context, _webHostEnvironment);
                            await mail.EnviarCorreosAsync(vm);




                            ////Envia correo a empresa notificando pago
                            ///
                            var cliente = _context.ClientesPortals.Where(x => x.CodAux == item.CodAux).FirstOrDefault();
                            var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                            MailViewModel vm2 = new MailViewModel();
                            vm2.tipo = 6;
                            vm2.nombre = "";
                            vm2.asunto = "";
                            vm2.mensaje = numeroComprobante + "|" + item.CodAux + "|" + item.Correo + "|" + item.MontoPago + "|" + cliente.Nombre + "|" + cliente.Rut;
                            vm2.adjuntos = listaAdjuntos;
                            vm2.email_destinatario = configCorreo.CorreoAvisoPago;
                            await mail.EnviarCorreosAsync(vm2);
                        }
                        catch { }

                    }
                    else
                    {
                        return new PagoComprobanteVm();
                    }
                }
                else
                {

                    ContabilizaPagoVm contabiliza = new ContabilizaPagoVm();
                    //Obtenemos configuración
                    var config = _context.ConfiguracionPagoClientes.FirstOrDefault();

                    //Obtenemos datos de pago
                    var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                    if (pago != null)
                    {
                        numPago = pago.IdPago.ToString();
                        //Obtenemos detalle de paog
                        var pagosDetalle = _context.PagosDetalles.Where(x => x.IdPago == idPago).ToList();

                        //Obtenemos datos de pasarela pago
                        var pasarela = _context.PasarelaPagos.Find(pago.IdPasarela);

                        //Creamos clase de pago
                        contabiliza.cuentaContable = pasarela.CuentaContable;
                        contabiliza.tipoDocumento = pasarela.TipoDocumento;
                        contabiliza.fechaContabilizacion = DateTime.Now;
                        contabiliza.NumDoc = pago.IdPago.ToString();
                        contabiliza.areaNegocio = (string.IsNullOrEmpty(config.AreaNegocio)) ? string.Empty : config.AreaNegocio;
                        contabiliza.glosaEncabezado = (string.IsNullOrEmpty(config.GlosaComprobante)) ? "Comprobante Pago Portal" : config.GlosaComprobante;

                        //Agregamos documentos a pagar
                        contabiliza.DetalleDocumento = new List<DetalleDocumento>();
                        foreach (var item in pagosDetalle)
                        {
                            DetalleDocumento det = new DetalleDocumento
                            {
                                tipoDocumento = item.TipoDocumento,
                                folioDocumento = item.Folio.ToString(),
                                montoPagado = (double)item.Apagar,
                                glosaMovimiento = (string.IsNullOrEmpty(config.GlosaDetalle)) ? "Comprobante Pago Portal " + item.TipoDocumento + " " + item.Folio : config.GlosaComprobante + " " + item.TipoDocumento + " " + item.Folio
                            };

                            contabiliza.DetalleDocumento.Add(det);
                        }

                        string jsonString = JsonConvert.SerializeObject(contabiliza);

                        using (var client = new HttpClient())
                        {
                            var api = _context.ApiSoftlands.FirstOrDefault();
                            string accesToken = api.Token;

                            string url = api.Url + api.ContabilizaPagos;
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


                            var formData = new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("vJson", jsonString),
                                 };
                            HttpContent data = new FormUrlEncodedContent(formData);


                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "ContabilizaPagos";


                            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, data).ConfigureAwait(false);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);


                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                content = JsonConvert.DeserializeObject<string>(content);
                                CapturaComprobanteResponse comprobante = JsonConvert.DeserializeObject<CapturaComprobanteResponse>(content);

                                //JCA: Pendiente definir retorno del metodo


                                if (comprobante.comprobante != null && comprobante.respuesta == "OK")
                                {

                                    if (!string.IsNullOrEmpty(comprobante.comprobante[0].numero))
                                    {
                                        numeroComprobante = comprobante.comprobante[0].numero;
                                        pago.ComprobanteContable = comprobante.comprobante[0].numero;
                                        pago.IdPagoEstado = 2; //ESTADO PAGADO
                                        _context.PagosCabeceras.Attach(pago);
                                        _context.Entry(pago).Property(x => x.ComprobanteContable).IsModified = true;
                                        _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                        _context.SaveChanges();

                                    }
                                    else
                                    {
                                        pago.IdPagoEstado = 4; //ESTADO PAGADO
                                        _context.PagosCabeceras.Attach(pago);
                                        _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                        _context.SaveChanges();

                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = "Comprobante generado con error o no generado",
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = comprobante.respuesta,
                                            Ruta = "SoftlandService/GeneraComprobantesContables"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }

                                }
                                else
                                {
                                    pago.IdPagoEstado = 4; //ESTADO PAGADO
                                    _context.PagosCabeceras.Attach(pago);
                                    _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                    _context.SaveChanges();

                                    LogProceso log = new LogProceso
                                    {
                                        Excepcion = comprobante.error[0].Mensaje,
                                        Fecha = DateTime.Now.Date,
                                        Hora = DateTime.Now.ToString("HH:mm:ss"),
                                        Mensaje = comprobante.respuesta,
                                        Ruta = "SoftlandService/GeneraComprobantesContables"
                                    };
                                    _context.LogProcesos.Add(log);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                pago.IdPagoEstado = 4; //ESTADO PAGADO
                                _context.PagosCabeceras.Attach(pago);
                                _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                _context.SaveChanges();

                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/GeneraComprobantesContables"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        numeroComprobante = string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GeneraComprobantesContables"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            if (utilizaApiSoftland == "true" && string.IsNullOrEmpty(numeroComprobante))
            {
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();
                if (pago != null)
                {
                    MailService mail = new MailService(_context, _webHostEnvironment);
                    var cliente = _context.ClientesPortals.Where(x => x.CodAux == pago.CodAux).FirstOrDefault();
                    var configCorreo = _context.ConfiguracionCorreos.FirstOrDefault();
                    MailViewModel vm2 = new MailViewModel();
                    vm2.tipo = 8;
                    vm2.nombre = "";
                    vm2.asunto = "";
                    vm2.mensaje = pago.CodAux + "|" + pago.Correo + "|" + pago.MontoPago.Value.ToString("N0") + "|" + cliente.Nombre + "|" + cliente.Rut + "|" + pago.IdPago.ToString();
                    vm2.email_destinatario = configCorreo.CorreoAvisoPago;
                    if (!string.IsNullOrEmpty(configCorreo.CorreoAvisoPago))
                    {
                        mail.EnviarCorreosAsync(vm2);
                    }

                }

            }

            PagoComprobanteVm pagoComprobante = new PagoComprobanteVm
            {
                NumComprobante = numeroComprobante,
                PagoId = idPago
            };
            return pagoComprobante;
        }

        public async System.Threading.Tasks.Task<List<DocumentoContabilizadoAPIDTO>> GetDocumentosVencidosAsync(FilterVm filter, string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();
            string tablaTemporal = string.Empty;
            try
            {
                if (utilizaApiSoftland == "false")
                {

                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string diasPorVencer = "";
                        int cantidad = 100;
                        int pagina = filter.Pagina;
                        string folio = "";
                        string rutAux = "";
                        string estadoDocs = "V";
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                        string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", filter.CodAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasPorVencer).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estadoDocs).Replace("{FOLIO}", folio).Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", rutAux).Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            if (documentos[0].Count > 0)
                            {

                                retorno = documentos[0].ToList();

                                if (retorno.Count > 0)
                                {
                                    var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                                    foreach (var pago in pagosPendientes)
                                    {
                                        foreach (var item in retorno)
                                        {
                                            var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Numdoc && x.TipoDocumento == item.Ttdcod).FirstOrDefault();
                                            if (detalle != null)
                                            {
                                                item.Saldoadic = item.Saldoadic - detalle.Apagar;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosVencidos"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }

                    }
                }

            }
            catch (Exception e)
            {
                if (utilizaApiSoftland == "false")
                {

                }
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosVencidos"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async System.Threading.Tasks.Task<List<DocumentoContabilizadoAPIDTO>> GetDocumentosPorVencerAsync(FilterVm filter, string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();
            string tablaTemporal = string.Empty;
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {

                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string diasPorVencer = configPortal.DiasPorVencer != null ? configPortal.DiasPorVencer.ToString() : "";
                        int cantidad = 100;
                        int pagina = filter.Pagina;
                        string folio = "";
                        string rutAux = "";
                        string estadoDocs = "P";
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                        string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", filter.CodAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasPorVencer).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estadoDocs).Replace("{FOLIO}", folio).Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            if (documentos[0].Count > 0)
                            {

                                retorno = documentos[0].ToList();

                                if (retorno.Count > 0)
                                {
                                    var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                                    foreach (var pago in pagosPendientes)
                                    {
                                        foreach (var item in retorno)
                                        {
                                            var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Numdoc && x.TipoDocumento == item.Ttdcod).FirstOrDefault();
                                            if (detalle != null)
                                            {
                                                item.Saldoadic = item.Saldoadic - detalle.Apagar;
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosPorVencer"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {

                }
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosPorVencer"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async System.Threading.Tasks.Task<List<DocumentoContabilizadoAPIDTO>> GetDocumentosPendientesAsync(FilterVm filter, string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string diasPorVencer = "";
                    int cantidad = 100;
                    int pagina = filter.Pagina;
                    string estadoDocs = "";
                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", filter.CodAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasPorVencer).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estadoDocs).Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocumentosContabilizados";

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);


                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                        if (documentos[0].Count > 0)
                        {
                            retorno = documentos[0].ToList();

                            if (retorno.Count > 0)
                            {
                                var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                                foreach (var pago in pagosPendientes)
                                {
                                    foreach (var item in retorno)
                                    {
                                        var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Numdoc && x.TipoDocumento == item.Ttdcod).FirstOrDefault();
                                        if (detalle != null)
                                        {
                                            item.Saldoadic = item.Saldoadic - detalle.Apagar;
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetDocumentosPendientes"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosPendientes"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async System.Threading.Tasks.Task<List<ClienteSaldosDTO>> GetAllDocumentosContabilizadosAsync(string codAux, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            string tablaTemporal = string.Empty;
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    //OBTIENE INICIO AÑO CONTABLE SOFTLAND
                    //string añoContableEmpresa = this.GetAñoTributarioSoftland();
                    string añoContableEmpresa = "2017";


                    //string docs = string.Empty;

                    //foreach (var item in documentos.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(docs))
                    //    {
                    //        docs = "'" + item + "'";
                    //    }
                    //    else
                    //    {
                    //        docs = docs + ",'" + item + "'";
                    //    }
                    //}

                    //string cuentasContables = string.Empty;
                    //foreach (var item in cuentas.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(cuentasContables))
                    //    {
                    //        cuentasContables = "'" + item + "'";
                    //    }
                    //    else
                    //    {
                    //        cuentasContables = cuentasContables + ",'" + item + "'";
                    //    }
                    //}

                    string fechaTabla = ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()) +
                                   ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) +
                                    DateTime.Now.Year.ToString() +
                                   ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) +
                                   ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString()) +
                                   ((DateTime.Now.Second < 10) ? "0" + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString());

                    tablaTemporal = "CW" + codAux + fechaTabla + "_";

                    string fecha = DateTime.Now.Year.ToString() + "/" + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "/" + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());


                    SqlCommand cmd = new SqlCommand("softland.cw_psnpConsultaDetalleCtaConDoc05", conSoftland);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.AddWithValue("xblnSoloSaldo", 0);
                    cmd.Parameters.AddWithValue("xstrMoneda ", "01"); //Obtener desde configuración portal
                    cmd.Parameters.AddWithValue("xstrFecha", fecha);
                    cmd.Parameters.AddWithValue("xvarAuxiliar", codAux);
                    cmd.Parameters.AddWithValue("xvarCuenta", "1-01-12");  //SE DEBE CAMBIAR POSTERIORMENTE
                    cmd.Parameters.AddWithValue("xvarTipoDocto", ""); //TIPO DOCUMENTO, DEJAR EN BLANCO PARA QUE OBTENGA TODOS Y LUEGO FILTRAMOS EN SIGUIETNE QUERY
                    cmd.Parameters.AddWithValue("xvarAreaNegocio", "");
                    cmd.Parameters.AddWithValue("xIncluyePagos", "");
                    cmd.Parameters.AddWithValue("opt_NroOper", 0);
                    cmd.Parameters.AddWithValue("ctlType_NroOper", "");
                    cmd.Parameters.AddWithValue("mstrAnoPrimerPeriodoFull", añoContableEmpresa);
                    cmd.Parameters.AddWithValue("AnoIni", añoContableEmpresa);
                    cmd.Parameters.AddWithValue("mfExistenMovAperturaPeriodo1", 0);
                    cmd.Parameters.AddWithValue("optAuxiliares", 0);
                    cmd.Parameters.AddWithValue("pstrCpbNumTemp", tablaTemporal + "1"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_1 (CW + usuario + fecha + minutos + segundos + _ + 1)
                    cmd.Parameters.AddWithValue("pstrQTemp ", tablaTemporal + "0"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_0 (CW + usuario + fecha + minutos + segundos + _ + 0) Esta tabla sera usada para la consulta final
                    cmd.Parameters.AddWithValue("pstrMontoTemp", tablaTemporal + "2"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_2 (CW + usuario + fecha + minutos + segundos + _ + 2)
                    cmd.Parameters.AddWithValue("pstrSaldoTemp", tablaTemporal + "3"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_3 (CW + usuario + fecha + minutos + segundos + _ + 3)
                    cmd.Parameters.AddWithValue("pstrMovFe", tablaTemporal + "4"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_4 (CW + usuario + fecha + minutos + segundos + _ + 4)
                    cmd.Parameters.AddWithValue("pstrMovFv", tablaTemporal + "5"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_5 (CW + usuario + fecha + minutos + segundos + _ + 5)
                    cmd.Parameters.AddWithValue("pstrCpbFec", tablaTemporal + "6"); //Crear nombre tabla temporal CWJCORDOVA14032021195805_6 (CW + usuario + fecha + minutos + segundos + _ + 6)
                    cmd.Parameters.AddWithValue("mstrPais", "CL");
                    cmd.Parameters.AddWithValue("pstrManejaCC", "N");
                    cmd.Parameters.AddWithValue("pstrCCosto", "");
                    cmd.Parameters.AddWithValue("pintNivelCC", 1);
                    cmd.Parameters.AddWithValue("pstrPagoOtraArea", "N");

                    conSoftland.Open();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        conSoftland.Close();
                        da.Fill(dt);
                    }

                    //Validamos si ejecuciòn fue exitos
                    if (dt.Rows.Count > 0)
                    {
                        conSoftland.Open();
                        SqlDataReader reader;
                        cmd.CommandText = "select " +
                                            " CpbNum as Comprobante, cta.PCDESC as Cuenta, movtipdocref as TipoDoc, " +
                                            " (Select top 1 tdoc.DesDoc from softland.cwttdoc as tdoc where tdoc.CodDoc = portal.MovTipDocRef) as Documento,  " +
                                            " portal.MovNumDocRef as Nro, " +
                                            " (select top 1 cmovi.MovFe from softland.cwmovim as cmovi " +
                                            " where CodAux = portal.CodAux  and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                            "(cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                            "(cmovi.CpbNum = portal.CpbNum)) as Femision, " +
                                            " (select top 1 cmovi.MovFv from softland.cwmovim as cmovi " +
                                            " where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                            "(cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                            "(cmovi.CpbNum = portal.CpbNum)) as Fvencimiento, " +
                                            " (select top 1 cmovi.MovDebe from softland.cwmovim as cmovi " +
                                            " where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                            "(cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                            "(cmovi.CpbNum = portal.CpbNum)) as Debe, " +
                                            " (select top 1 cwtauxi.NomAux from softland.cwtauxi where codaux = portal.CodAux) as RazonSocial, " +
                                            " portal.Saldo, " +
                                            " CASE WHEN(select top 1 cmovi.MovFv from softland.cwmovim as cmovi " +
                                            " where CodAux = portal.CodAux and(cmovi.MovTipDocRef = portal.MovTipDocRef) and " +
                                            "(cmovi.MovNumDocRef = portal.MovNumDocRef) and(cmovi.CpbNum = portal.CpbNum) and " +
                                            "(cmovi.CpbNum = portal.CpbNum)) <= GETDATE() THEN 'Vencido' else 'Por Vencer' end as estado " +
                                            " from SOFTLAND." + tablaTemporal + "0" + " as portal inner join[softland].[cwpctas] as cta on portal.pctcod = cta.PCCODI " +
                                            " where Saldo > 0";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conSoftland;

                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            ClienteSaldosDTO item = new ClienteSaldosDTO();
                            //item.comprobanteContable = reader["Comprobante"].ToString();
                            item.Documento = reader["Documento"].ToString();
                            item.Nro = (reader["Nro"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Nro"]);
                            item.FechaEmision = (reader["Femision"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Femision"]);
                            item.FechaVcto = (reader["Fvencimiento"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fvencimiento"]);
                            item.Debe = (reader["Debe"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Debe"]);
                            //item.Haber = (reader["Haber"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Haber"]);
                            item.Saldo = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                            item.Detalle = ""; // reader["Detalle"].ToString();
                            item.Estado = reader["Estado"].ToString();
                            item.Pago = ""; // reader["Pago"].ToString();
                            item.TipoDoc = reader["TipoDoc"].ToString();
                            item.RazonSocial = (reader["RazonSocial"] == DBNull.Value) ? "" : reader["RazonSocial"].ToString();
                            retorno.Add(item);
                        }
                        reader.Close();
                        conSoftland.Close();
                    }

                    conSoftland.Open();
                    SqlCommand cmdElimina = new SqlCommand("DROP TABLE IF EXISTS SOFTLAND." + tablaTemporal + "0;");
                    cmdElimina.CommandType = CommandType.Text;
                    cmdElimina.Connection = conSoftland;
                    cmdElimina.ExecuteNonQuery();
                    conSoftland.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", DateTime.Now.Year.ToString()).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", "").Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos).Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var configPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                            var documentosContablesDeuda = configPago.TiposDocumentosDeuda;
                            var cuentasContablesDeuda = configPago.CuentasContablesDeuda;

                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            foreach (var doc in documentos[0])
                            {
                                bool esValidoDocumento = false;
                                bool esValidoCuenta = false;

                                var existDocumentoDeuda = documentosContablesDeuda.Split(';').Where(x => x == doc.Ttdcod).FirstOrDefault();
                                if (existDocumentoDeuda != null)
                                {
                                    esValidoDocumento = true;
                                }

                                var existCuentaDeuda = cuentasContablesDeuda.Split(';').Where(x => x == doc.Pctcod).FirstOrDefault();
                                if (existCuentaDeuda != null)
                                {
                                    esValidoCuenta = true;
                                }

                                if (!esValidoDocumento || !esValidoCuenta)
                                {
                                    continue;
                                }
                                ClienteSaldosDTO item = new ClienteSaldosDTO();
                                //item.comprobanteContable = reader["Comprobante"].ToString();
                                item.Documento = doc.DesDoc;
                                item.Nro = (double)doc.Numdoc;
                                item.FechaEmision = Convert.ToDateTime(doc.Movfe);
                                item.FechaVcto = Convert.ToDateTime(doc.Movfv);
                                item.Debe = (double)doc.MovMonto;
                                item.Haber = doc.Saldoadic;
                                item.Saldo = (double)doc.Saldoadic;
                                item.Detalle = ""; // reader["Detalle"].ToString();
                                item.Estado = doc.Estado;
                                item.Pago = ""; // reader["Pago"].ToString();
                                item.TipoDoc = doc.Ttdcod;
                                item.RazonSocial = "";
                                var mon = monedas.Where(x => x.CodMon == item.CodigoMoneda).FirstOrDefault();
                                if (mon != null) { item.DesMon = mon.DesMon; }
                                //item.CodigoMoneda = doc.MonCod;
                                item.CodigoMoneda = doc.MonCod;
                                item.MontoBase = doc.Saldobase;
                                item.SaldoBase = doc.Saldobase;
                                item.EquivalenciaMoneda = doc.Equivalencia;
                                item.MontoOriginalBase = doc.MontoOriginalBase;
                                retorno.Add(item);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetAllDocumentosContabilizadosAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();

                    //Elimina tabla temporal
                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand("DROP TABLE IF EXISTS SOFTLAND." + tablaTemporal + "0;");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    cmd.ExecuteNonQuery();
                    conSoftland.Close();
                }
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllDocumentosContabilizadosAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async System.Threading.Tasks.Task<List<ComprasSoftlandDTO>> GetTopComprasAsync(string codAux, int top, string logApiId)
        {
            List<ComprasSoftlandDTO> item = new List<ComprasSoftlandDTO>();
            try
            {

                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT TOP " + top +
                                      "  CASE WHEN(Tipo = 'F') THEN 'FACTURA' " +
                                      "  WHEN(Tipo = 'B') THEN 'BOLETA' " +
                                      "  WHEN(Tipo = 'N') THEN 'NOTA DE CREDITO' " +
                                      "  WHEN(Tipo = 'D') THEN 'NOTA DE DEBITO' " +
                                      "  END AS Documento, Folio, Fecha, FechaVenc as Vencimiento, Total, Tipo " +
                                      "  FROM softland.iw_gsaen WHERE CodAux = '" + codAux + "' AND Tipo IN('F', 'B', 'D', 'N') and EnMantencion <> -1 ORDER BY Fecha DESC";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ComprasSoftlandDTO aux = new ComprasSoftlandDTO();
                        aux.Folio = Convert.ToInt32(reader["Folio"]);
                        aux.FechaEmision = Convert.ToDateTime(reader["Fecha"]);
                        aux.Total = Convert.ToInt32(reader["Total"]);
                        aux.Documento = reader["Documento"].ToString();
                        aux.Tipo = reader["Tipo"].ToString(); //FCA 05-07-2022
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    var configPortal = _context.ConfiguracionPortals.FirstOrDefault();
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    var configPagos = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    int anioActual = DateTime.Now.Year + 1;
                    while (item.Count() < configPortal.CantidadUltimasCompras && anioActual >= configPagos.AnioTributario)
                    {
                        anioActual = anioActual - 1;
                        using (var client = new HttpClient())
                        {

                            string url = api.Url + api.DocumentosFacturados.Replace("{CODAUX}", codAux).Replace("{ANIO}", anioActual.ToString()).Replace("tipo={TIPO}&", "").Replace("{TOP}", configPortal.CantidadUltimasCompras.ToString()).Replace("{AREADATOS}", api.AreaDatos);

                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "DocumentosFacturados";


                            HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                List<List<DocumentosFacturadosAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentosFacturadosAPIDTO>>>(content);
                                documentos[0] = documentos[0].Where(x => x.Folio != null && x.Folio != 0 && x.Tipo_Documento != "S").OrderByDescending(x => x.Fecha).ToList();
                                foreach (var doc in documentos[0])
                                {
                                    ComprasSoftlandDTO aux = new ComprasSoftlandDTO();
                                    aux.Folio = (int)doc.Folio;
                                    aux.FechaEmision = Convert.ToDateTime(doc.Fecha);
                                    aux.Total = (decimal)doc.Monto;
                                    aux.Documento = doc.DocDes;
                                    aux.Tipo = doc.Tipo_Documento;
                                    item.Add(aux);
                                    if (item.Count() == configPortal.CantidadUltimasCompras)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/GetTopCompras"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetTopCompras"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return item;
        }

        public async System.Threading.Tasks.Task<bool> GetEstadoBloqueoClienteAsync(string codAux, string logApiId) //FCA 16-06-2022
        {
            Boolean estado = false;
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select Bloqueado from softland.cwtauxi where CodAux = '" + codAux + "'";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        if (reader["Bloqueado"].ToString() == "S")
                        {
                            estado = false;
                        }
                        else
                        {
                            estado = true;
                        }

                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ResumenContable.Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ResumenContable";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            ResumenContableAPIDTO resumenContable = JsonConvert.DeserializeObject<ResumenContableAPIDTO>(content);
                            if (resumenContable.EstadoBloqueo == "S")
                            {
                                estado = false;
                            }
                            else
                            {
                                estado = true;
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetEstadoBloqueoClienteAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetEstadoBloqueoCliente"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return estado;
        }

        //FCA 19-08-2021 Obtiene vendedores de softland
        public async Task<List<VendedorDTO>> GetVenedoresSoftlandAsync(string logApiId)
        {
            List<VendedorDTO> item = new List<VendedorDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.Cwtvend";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        VendedorDTO aux = new VendedorDTO();
                        aux.CodTipV = reader["CodTipV"].ToString();
                        aux.Email = reader["Email"].ToString();
                        aux.Usuario = reader["Usuario"].ToString();
                        aux.VenCod = reader["VenCod"].ToString();
                        aux.VenDes = reader["VenDes"].ToString();
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ObtieneVendedores.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ObtieneVendedores";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<VendedorAPIDTO>> vendedores = JsonConvert.DeserializeObject<List<List<VendedorAPIDTO>>>(content);
                            foreach (var vendedor in vendedores[0])
                            {
                                VendedorDTO aux = new VendedorDTO();
                                aux.Email = vendedor.email;
                                aux.VenCod = vendedor.codi;
                                aux.VenDes = vendedor.descripcion;
                                item.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetVenedoresSoftlandAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetVenedoresSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return item;
        }

        //FCA 19-08-2021 Obtiene categorias de clientes de softland
        public async Task<List<CategoriaClienteDTO>> GetCategoriasClienteAsync(string logApiId)
        {
            List<CategoriaClienteDTO> item = new List<CategoriaClienteDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.Cwtcgau";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        CategoriaClienteDTO aux = new CategoriaClienteDTO();
                        aux.CatCod = reader["CatCod"].ToString();
                        aux.CatDes = reader["CatDes"].ToString();
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ObtieneCategoriaClientes.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ObtieneCategoriaClientes";

                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            if (content.Contains("404"))
                            {
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/GetCategoriasClienteAsync"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                            else
                            {
                                List<List<CategoriaClienteAPIDTO>> categorias = JsonConvert.DeserializeObject<List<List<CategoriaClienteAPIDTO>>>(content);
                                foreach (var categoria in categorias[0])
                                {
                                    CategoriaClienteDTO aux = new CategoriaClienteDTO();
                                    aux.CatCod = categoria.codigo;
                                    aux.CatDes = categoria.descripcion;
                                    item.Add(aux);
                                }
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetCategoriasClienteAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCategoriasCliente"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return item;
        }

        //FCA 19-08-2021 Obtiene condiciones de venta de softland
        public async Task<List<CondicionVentaDTO>> GetCondVentaAsync(string logApiId)
        {
            List<CondicionVentaDTO> item = new List<CondicionVentaDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.Cwtconv";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        CondicionVentaDTO aux = new CondicionVentaDTO();
                        aux.CveCod = reader["CveCod"].ToString();
                        aux.CveDes = reader["CveDes"].ToString();
                        aux.CveDias = int.Parse(reader["CveDias"].ToString());
                        aux.cveNvCto = int.Parse(reader["cveNvCto"].ToString());
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ObtieneCondicionesVenta.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ObtieneCondicionesVenta";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<CondicionVentaAPIDTO>> condiciones = JsonConvert.DeserializeObject<List<List<CondicionVentaAPIDTO>>>(content);
                            foreach (var condicion in condiciones[0])
                            {
                                CondicionVentaDTO aux = new CondicionVentaDTO();
                                aux.CveCod = condicion.codi;
                                aux.CveDes = condicion.descripcion;
                                aux.CveDias = condicion.dias;
                                item.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetCondVentaAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCondVenta"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return item;
        }

        //FCA 19-08-2021 Obtiene  listas de precio de softland
        public async Task<List<ListaPrecioDTO>> GetListPrecioAsync(string logApiId)
        {
            List<ListaPrecioDTO> item = new List<ListaPrecioDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.Iw_tlispre";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ListaPrecioDTO aux = new ListaPrecioDTO();
                        aux.CodLista = reader["CodLista"].ToString();
                        aux.DesLista = reader["DesLista"].ToString();
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ObtieneListasPrecio.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ObtieneListasPrecio";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ListaPrecioAPIDTO>> listas = JsonConvert.DeserializeObject<List<List<ListaPrecioAPIDTO>>>(content);
                            foreach (var lista in listas[0])
                            {
                                if (!string.IsNullOrEmpty(lista.CodLista))
                                {
                                    ListaPrecioDTO aux = new ListaPrecioDTO();
                                    aux.CodLista = lista.CodLista;
                                    aux.DesLista = lista.DesLista;
                                    item.Add(aux);
                                }

                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetListPrecioAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetListPrecio"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                { conSoftland.Close(); }
            }

            return item;
        }

        //FCA 19-08-2021 Obtiene  codigo de moneda de softland
        public async Task<List<MonedaDTO>> GetMonedasAsync(string logApiId)
        {
            List<MonedaDTO> item = new List<MonedaDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select * from softland.cwtmone";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        MonedaDTO aux = new MonedaDTO();
                        aux.CodMon = reader["CodMon"].ToString();
                        aux.DesMon = reader["DesMon"].ToString();
                        item.Add(aux);
                    }
                    reader.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaMonedas.Replace("{AREADATOS}", api.AreaDatos);
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaMonedas";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<MonedaAPIDTO>> monedas = JsonConvert.DeserializeObject<List<List<MonedaAPIDTO>>>(content);
                            foreach (var moneda in monedas[0])
                            {
                                MonedaDTO aux = new MonedaDTO();
                                aux.CodMon = moneda.CodMon;
                                aux.DesMon = moneda.DesMon;
                                item.Add(aux);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetMonedasAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetMonedas"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return item;
        }

        public List<CanalVentaDTO> GetCanalesVenta(string logApiId)
        {
            List<CanalVentaDTO> item = new List<CanalVentaDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "select * from softland.cwtcana";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    CanalVentaDTO aux = new CanalVentaDTO();
                    aux.CanCod = reader["CanCod"].ToString();
                    aux.CanDes = reader["CanDes"].ToString();
                    item.Add(aux);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCanalesVenta"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return item;
        }

        public List<CobradorDTO> GetCobradores(string logApiId)
        {
            List<CobradorDTO> item = new List<CobradorDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "SELECT * FROM softland.cwtcobr";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    CobradorDTO aux = new CobradorDTO();
                    aux.CobCod = reader["CobCod"].ToString();
                    aux.CobDes = reader["CobDes"].ToString();
                    item.Add(aux);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetCobradores"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return item;
        }

        public async Task<List<ClienteAPIDTO>> BuscarClienteSoftlandAccesosAsync(string codAux, string rut, string nombre, string vendedor, string condicionVenta, string categoriaCliente, string listaPrecio, int cantidad, Nullable<int> pagina, string logApiId)
        {
            List<ClienteAPIDTO> retorno = new List<ClienteAPIDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    //conSoftland.Open();

                    //string sqlWhere = string.Empty;
                    //if (!string.IsNullOrEmpty(codAux))
                    //{
                    //    sqlWhere = sqlWhere + " WHERE c.CodAux='" + codAux + "' ";
                    //}

                    //if (!string.IsNullOrEmpty(rut))
                    //{
                    //    if (string.IsNullOrEmpty(sqlWhere))
                    //    {
                    //        sqlWhere = sqlWhere + " WHERE c.RutAux='" + rut + "' ";
                    //    }
                    //    else
                    //    {
                    //        sqlWhere = sqlWhere + " AND c.RutAux='" + rut + "' ";
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(nombre))
                    //{
                    //    if (string.IsNullOrEmpty(sqlWhere))
                    //    {
                    //        sqlWhere = sqlWhere + " WHERE c.NomAux like '%" + nombre + "%' ";
                    //    }
                    //    else
                    //    {
                    //        sqlWhere = sqlWhere + " AND c.NomAux like '%" + nombre + "%' ";
                    //    }
                    //}

                    //SqlCommand cmd = new SqlCommand();
                    //SqlDataReader reader;
                    //cmd.CommandText = "select c.RutAux, c.CodAux, c.NomAux, c.EMail, c.DirAux, c.DirNum, d.VenCod, b.convta, b.CatCli, b.CodLista from softland.Cwtauxi c left join softland.cwtcvcl b on c.CodAux = b.CodAux left join softland.cwtauxven d on d.CodAux = c.CodAux " + sqlWhere;
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Connection = conSoftland;
                    //reader = cmd.ExecuteReader();


                    //while (reader.Read())
                    //{
                    //    ClienteVm aux = new ClienteVm();
                    //    aux.Rut = reader["RutAux"].ToString();
                    //    aux.CodAux = reader["CodAux"].ToString();
                    //    aux.Correo = reader["EMail"].ToString();
                    //    aux.Nombre = reader["NomAux"].ToString();
                    //    aux.CodVendedor = reader["VenCod"].ToString();
                    //    aux.CodCondVenta = reader["convta"].ToString();
                    //    aux.CodCatCliente = reader["CatCli"].ToString();
                    //    aux.CodLista = reader["CodLista"].ToString();
                    //    aux.DirAux = reader["DirAux"].ToString();
                    //    aux.DirNum = reader["DirNum"].ToString();
                    //    retorno.Add(aux);
                    //}
                    //reader.Close();
                    //conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        if (pagina != null)
                        {
                            var api = _context.ApiSoftlands.FirstOrDefault();
                            string accesToken = api.Token;
                            string url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", categoriaCliente).Replace("{CODAUX}", codAux).Replace("{CODLISTA}", listaPrecio).Replace("{CODVEN}", vendedor).Replace("{CONVTA}", condicionVenta)
                                .Replace("{NOMBRE}", nombre).Replace("{RUT}", rut);
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "ConsultaCliente";


                            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                List<List<ClienteAPIDTO>> clientesApi = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);
                                clientesApi[0] = clientesApi[0].Where(x => x.CodAux != null).ToList();
                                if (clientesApi.Count > 0)
                                {
                                    retorno = clientesApi[0];
                                }

                            }
                            else
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/BuscarClienteSoftland2"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            pagina = 1;
                            var api = _context.ApiSoftlands.FirstOrDefault();
                            string accesToken = api.Token;
                            string url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", categoriaCliente).Replace("{CODAUX}", codAux).Replace("{CODLISTA}", listaPrecio).Replace("{CODVEN}", vendedor).Replace("{CONVTA}", condicionVenta)
                                .Replace("{NOMBRE}", nombre).Replace("{RUT}", rut);
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "ConsultaCliente";


                            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                List<List<ClienteAPIDTO>> clientesApi = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);
                                clientesApi[0] = clientesApi[0].Where(x => x.CodAux != null).ToList();
                                if (clientesApi.Count > 0)
                                {
                                    retorno = clientesApi[0];

                                    while (retorno.Count < int.Parse(clientesApi[0][0].Total))
                                    {
                                        using (var client2 = new HttpClient())
                                        {
                                            pagina = pagina + 1;
                                            string url2 = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", categoriaCliente).Replace("{CODAUX}", codAux).Replace("{CODLISTA}", listaPrecio).Replace("{CODVEN}", vendedor).Replace("{CONVTA}", condicionVenta)
                                  .Replace("{NOMBRE}", nombre).Replace("{RUT}", rut);
                                            client2.BaseAddress = new Uri(url2);
                                            client2.DefaultRequestHeaders.Accept.Clear();
                                            client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                            client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                            LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                            logApiDetalle2.IdLogApi = logApiId;
                                            logApiDetalle2.Inicio = DateTime.Now;
                                            logApiDetalle2.Metodo = "ConsultaCliente";


                                            HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress);

                                            logApiDetalle2.Termino = DateTime.Now;
                                            logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                            this.guardarDetalleLogApi(logApiDetalle2);

                                            if (response2.IsSuccessStatusCode)
                                            {
                                                var content2 = await response2.Content.ReadAsStringAsync();
                                                List<List<ClienteAPIDTO>> clientesApi2 = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content2);
                                                clientesApi2[0] = clientesApi2[0].Where(x => x.CodAux != null).ToList();
                                                if (clientesApi2.Count > 0)
                                                {
                                                    retorno.AddRange(clientesApi2[0]);
                                                }
                                            }
                                            else
                                            {
                                                var content2 = await response2.Content.ReadAsStringAsync();
                                                LogProceso log = new LogProceso
                                                {
                                                    Excepcion = response.StatusCode.ToString(),
                                                    Fecha = DateTime.Now.Date,
                                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                    Mensaje = content2,
                                                    Ruta = "SoftlandService/BuscarClienteSoftland2"
                                                };
                                                _context.LogProcesos.Add(log);
                                                _context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/BuscarClienteSoftland2"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/BuscarClienteSoftland2"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();
                }

            }
            return retorno;

        }

        //FCA 25-08-2021 Obtiene descripcion documento  de softland
        public string GetDescDocumento(string codigo, string logApiId)
        {
            string retorno = string.Empty;
            List<MonedaDTO> item = new List<MonedaDTO>();
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = " select DesDoc from softland.cwttdoc where CodDoc = '" + codigo + "'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    retorno = reader["DesDoc"].ToString();
                }
                reader.Close();
                conSoftland.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDescDocumento"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }

            return retorno;
        }

        //FCA 05-07-2022
        public CabeceraDocumentoDTO obtenerCabecera(int folio, string tipoDoc, string codAux, string logApiId)
        {
            CabeceraDocumentoDTO dtResultado = new CabeceraDocumentoDTO();
            try
            {
                conSoftland.Open();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "SELECT " +
                                        "     f.Folio                                                                                           " +
                                        "	,ISNULL(aux.RutAux, '') as Rut                                                                      " +
                                        "	,ISNULL(aux.NomAux, '') as RazonSocial                                                              " +
                                        "	,ISNULL((select gir.GirDes from softland.CWTGiro gir where gir.GirCod = aux.GirAux),'') as Giro     " +
                                        "	,ISNULL((select ciu.CiuDes from softland.cwtciud ciu where ciu.CiuCod = aux.CiuAux),'') as Ciudad   " +
                                        "	,ISNULL((select com.ComDes from softland.cwtcomu com where com.ComCod = aux.ComAux),'') as Comuna   " +
                                        "	,ISNULL(aux.DirAux, '') as Direccion                                                                " +
                                        "	,ISNULL(f.Glosa, '') as Descripcion                                                                 " +
                                        "	,f.Fecha as FechaEmision                                                                            " +
                                        "   ,f.FechaVenc as FechaVencimiento                                                                    " +
                                        "	,ISNULL(aux.FonAux1, '') as Telefono                                                                " +
                                        "	,ISNULL(c.CveDes, '') as CondVenta                                                                  " +
                                        "	,ISNULL(v.VenDes, '') as Vendedor                                                                   " +
                                        "	,ISNULL(f.Patente, '') as Patente                                                                   " +
                                        "	,ISNULL(f.NetoAfecto, 0) as Neto                                                                    " +
                                        "	,ISNULL(f.Descto01, 0) as Descuento                                                                 " +
                                        "	,CAST(ISNULL(f.IVA, 0) AS INT) as Iva                                                                            " +
                                        "	,ISNULL(f.Total, 0) as Total                                                                        " +
                                        "   , f.Tipo                                                                                            " +
                                        "   , f.nvnumero                                                                                         " +
                                        " FROM softland.iw_gsaen f                                                                               " +
                                        " INNER JOIN softland.cwtauxi aux                                                                        " +
                                        " on aux.CodAux = f.CodAux                                                                               " +
                                        " LEFT JOIN softland.cwtconv c                                                                           " +
                                        " on c.CveCod = f.CondPago                                                                               " +
                                        " LEFT JOIN softland.cwtvend v                                                                           " +
                                        " ON v.VenCod = f.CodVendedor                                                                            " +
                                        " WHERE f.Folio =" + folio +
                                        //" AND f.Tipo = '" + tipoDoc + "'                                                                          " +
                                        " AND f.CodAux = '" + codAux + "' AND Tipo in ('B','F'); ";




                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    dtResultado.Ciudad = (reader["Ciudad"] == DBNull.Value) ? "" : reader["Ciudad"].ToString();
                    dtResultado.Comuna = (reader["Comuna"] == DBNull.Value) ? "" : reader["Comuna"].ToString();
                    dtResultado.CondVenta = (reader["CondVenta"] == DBNull.Value) ? "" : reader["CondVenta"].ToString();
                    dtResultado.Descripcion = (reader["Descripcion"] == DBNull.Value) ? "" : reader["Descripcion"].ToString();
                    dtResultado.Descuento = (reader["Descuento"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Descuento"]);
                    dtResultado.Direccion = (reader["Direccion"] == DBNull.Value) ? "" : reader["Direccion"].ToString();
                    dtResultado.FechaEmision = (reader["FechaEmision"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["FechaEmision"]);
                    dtResultado.FechaVencimiento = (reader["FechaVencimiento"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["FechaVencimiento"]);
                    dtResultado.Folio = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Folio"]);
                    dtResultado.Giro = (reader["Giro"] == DBNull.Value) ? "" : reader["Giro"].ToString();
                    dtResultado.Iva = (reader["Iva"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Iva"]);
                    dtResultado.Neto = (reader["Neto"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Neto"]);
                    dtResultado.nvnumero = (reader["nvnumero"] == DBNull.Value) ? "" : reader["nvnumero"].ToString();
                    dtResultado.Patente = (reader["Patente"] == DBNull.Value) ? "" : reader["Patente"].ToString();
                    dtResultado.RazonSocial = (reader["RazonSocial"] == DBNull.Value) ? "" : reader["RazonSocial"].ToString();
                    dtResultado.Rut = (reader["Rut"] == DBNull.Value) ? "" : reader["Rut"].ToString();
                    dtResultado.Telefono = (reader["Telefono"] == DBNull.Value) ? "" : reader["Telefono"].ToString();
                    dtResultado.Tipo = (reader["Tipo"] == DBNull.Value) ? "" : reader["Tipo"].ToString();
                    dtResultado.Total = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);
                    dtResultado.Vendedor = (reader["Vendedor"] == DBNull.Value) ? "" : reader["Vendedor"].ToString();
                }
                reader.Close();
                conSoftland.Close();


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerCabecera"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        public List<DetalleDocumentoDTO> obtenerDetalle(int folio, string tipoDoc, string codAux, string logApiId) //FCA 05-07-2022
        {
            List<DetalleDocumentoDTO> dtResultado = new List<DetalleDocumentoDTO>(); //FCA 05-07-2022
            try
            {
                conSoftland.Open();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "SELECT                                     " +
                                "    p.CodProd as Codigo                        " +
                                "	,p.DesProd                                  " +
                                "	,d.CantFacturada as Cantidad                " +
                                "	,p.CodUMed as CodUMed                       " +
                                "	,u.DesUMed as UMed                          " +
                                "	,CASE WHEN f.tipo = 'F' then d.PreUniMB else CASE WHEN f.tipo = 'B' then  d.PreUniBoleta end end as PrecioUnitario               " +
                                "	,d.DescMov01 as Descuento                   " +
                                "	,(CASE WHEN f.tipo = 'F' then d.PreUniMB else CASE WHEN f.tipo = 'B' then  d.PreUniBoleta end end * d.CantFacturada) AS Total    " +
                                " FROM softland.iw_gsaen f                       " +
                                " INNER JOIN softland.IW_GMOVI d                 " +
                                " on d.NroInt = f.NroInt                         " +
                                " and d.Tipo = f.Tipo                            " +
                                " INNER JOIN softland.iw_tprod p                 " +
                                " LEFT JOIN softland.iw_tumed u                  " +
                                " on u.CodUMed = p.CodUMed                       " +
                                " ON p.CodProd = d.CodProd                       " +
                                " WHERE f.Folio = " + folio +
                                " AND f.Tipo = '" + tipoDoc + "'                               " +
                                " AND f.CodAux = '" + codAux + "'";




                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    DetalleDocumentoDTO item = new DetalleDocumentoDTO();
                    item.Codigo = (reader["Codigo"] == DBNull.Value) ? "" : reader["Codigo"].ToString();
                    item.Cantidad = (reader["Cantidad"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Cantidad"]);
                    item.CodUmed = (reader["CodUmed"] == DBNull.Value) ? "" : reader["CodUmed"].ToString();
                    item.Descuento = (reader["Descuento"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Descuento"]);
                    item.DesProd = (reader["DesProd"] == DBNull.Value) ? "" : reader["DesProd"].ToString();
                    item.PrecioUnitario = (reader["PrecioUnitario"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PrecioUnitario"]);
                    item.Total = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);

                    dtResultado.Add(item);
                }
                reader.Close();
                conSoftland.Close();


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerDetalle"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }
            return dtResultado;
        }

        //fca 05-07-2022
        public async System.Threading.Tasks.Task<DocumentoDTO> obtenerDocumentoAPI(int folio, string tipoDoc, string codAux, string logApiId) //FCA 16-06-2022
        {
            DocumentoDTO doc = new DocumentoDTO();
            CabeceraDocumentoDTO cab = new CabeceraDocumentoDTO();
            List<DetalleDocumentoDTO> det = new List<DetalleDocumentoDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.DetalleDocumento.Replace("{FOLIO}", folio.ToString()).Replace("{SUBTIPO}", tipoDoc.Substring(1, 1)).Replace("{TIPODOC}", tipoDoc.Substring(0, 1)).Replace("{CODAUX}", codAux).Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DetalleDocumento";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        DocumentoAPIDTO documento = JsonConvert.DeserializeObject<DocumentoAPIDTO>(content);

                        if (documento.cabecera != null)
                        {
                            cab.Ciudad = documento.cabecera.ciudes;
                            cab.Comuna = documento.cabecera.comdes;
                            cab.CondVenta = ""; //REVISAR NO ESTA
                            cab.Descripcion = documento.cabecera.glosa;
                            cab.Descuento = (float)documento.cabecera.descto01;
                            cab.Direccion = documento.cabecera.diraux;
                            cab.FechaEmision = documento.cabecera.fecha;
                            cab.FechaVencimiento = documento.cabecera.fechavenc;
                            cab.Folio = (int)documento.cabecera.folio;
                            cab.Giro = documento.cabecera.girdes;
                            cab.Iva = (float)documento.cabecera.iva;
                            cab.Neto = (float)documento.cabecera.netoafecto;
                            cab.nvnumero = documento.cabecera.nvnumero.ToString();
                            cab.Total = (float)documento.cabecera.total;
                            cab.Vendedor = documento.cabecera.vendes;
                            cab.SubTipo = documento.cabecera.subtipo;
                            cab.Tipo = documento.cabecera.tipo;
                            cab.RazonSocial = documento.cabecera.nomaux;
                            cab.Rut = documento.cabecera.rutaux;
                            if (cab.FechaVencimiento != null)
                            {
                                cab.Estado = cab.FechaVencimiento < DateTime.Now.Date ? "V" : "P";
                            }



                            foreach (var item in documento.detalle)
                            {
                                DetalleDocumentoDTO linea = new DetalleDocumentoDTO();

                                linea.Cantidad = (int)item.CantFacturada;
                                linea.CantidadDespachada = (int)item.CantDespachada;

                                linea.Codigo = item.CodProd;
                                linea.CodUmed = item.CodUMed;
                                linea.Descuento = (float)item.TotalDescMov;
                                linea.DesProd = item.DetProd;
                                linea.PrecioUnitario = (float)item.PreUniMB;
                                linea.Total = (float)item.TotLinea;

                                det.Add(linea);
                            }
                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/obtenerDocumentoAPI"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }
                doc.cabecera = cab;
                doc.detalle = det;

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerDocumentoAPI"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();

            }


            return doc;
        }

        public async System.Threading.Tasks.Task<string> obtenerPDFDocumento(int folio, string tipoDoc, string logApiId) //FCA 16-06-2022
        {
            string base64 = string.Empty;
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.ObtenerPdfDocumento.Replace("{FOLIO}", folio.ToString()).Replace("{TIPO}", tipoDoc.Substring(0, 1)).Replace("{SUBTIPO}", tipoDoc.Substring(1, 1)).Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "ObtenerPdfDocumento";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        base64 = JsonConvert.DeserializeObject<string>(content);
                    }
                    else
                    {

                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/obtenerPDFDocumento"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();

                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerPDFDocumento"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }


            return base64;
        }

        public async System.Threading.Tasks.Task<ResumenContableDTO> obtenerResumenContable(string codAux, string logApiId) //FCA 16-06-2022
        {
            ResumenContableDTO resumen = new ResumenContableDTO();
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    var config = _context.ConfiguracionPortals.FirstOrDefault();
                    string url = api.Url + api.ResumenContable.Replace("{CODAUX}", codAux).Replace("{CONTABILIZADO}", config.ResumenContabilizado.ToString()).Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "ResumenContable";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        resumen = JsonConvert.DeserializeObject<ResumenContableDTO>(content);
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/obtenerResumenContable"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerResumenContable"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }


            return resumen;
        }

        //FCA 06-07-2022 OBTIENE DOCUMENTOS FACTURADOS PARA RDASHBOARD
        public async System.Threading.Tasks.Task<List<ClienteSaldosDTO>> GetDocumentosDashboard(string codAux, string logApiId) //FCA 16-06-2022
        {
            List<ClienteSaldosDTO> documentos = new List<ClienteSaldosDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.DocumentosFacturados.Replace("{CODAUX}", codAux).Replace("{ANIO}", DateTime.Now.Year.ToString()).Replace("tipo={TIPO}&", "").Replace("top={TOP}&", "").Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocumentosFacturados";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        //SE DEBE CONTINUAR AQUI
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetDocumentosDashboard"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerResumenContable"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }


            return documentos;
        }

        public async System.Threading.Tasks.Task<string> obtenerPDFDocumentoNv(string nvNumero, string codaux, string logApiId) //FCA 16-06-2022
        {
            string base64 = string.Empty;
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.ObtenerPdf.Replace("{AREADATOS}", api.AreaDatos).Replace("{NVNUMERO}", nvNumero);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "ObtenerPdf";

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);


                    if (response.IsSuccessStatusCode)
                    {
                        base64 = await response.Content.ReadAsStringAsync();
                        base64 = base64.Replace("\"", "");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/obtenerPDFDocumentoNv"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/obtenerPDFDocumento"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }


            return base64;
        }

        public async System.Threading.Tasks.Task<List<GuiaDespachoDTO>> GetGuiasPendientes(string codaux, string logApiId)
        {
            List<GuiaDespachoDTO> retorno = new List<GuiaDespachoDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {

                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ObtieneGuiasPendientes.Replace("{CODAUX}", codaux).Replace("{AREADATOS}", api.AreaDatos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ObtieneGuiasPendientes";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<GuiaDespachoAPIDTO>> guias = JsonConvert.DeserializeObject<List<List<GuiaDespachoAPIDTO>>>(content);
                            guias[0] = guias[0].Where(x => x.folio != null && x.folio != 0).OrderByDescending(x => x.folio).ToList();
                            foreach (var doc in guias[0])
                            {
                                GuiaDespachoDTO item = new GuiaDespachoDTO();
                                item.Movtipdocref = doc.tipo + doc.subtipo;
                                item.Nro = (int)doc.folio;
                                item.Fecha = doc.Fecha;
                                item.Total = (int)doc.total;
                                item.Estado = doc.estado;
                                item.CodBode = doc.codbode;
                                retorno.Add(item);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetGuiasPendientes"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }


                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetGuiasPendientes"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }


            return retorno;
        }

        #region METODOS COBRANZA AUTOMATICA
        public async Task<List<ResumenDocumentosClienteApiDTO>> GetDocumentosPendientesCobranzaAsync(FiltroCobranzaVm filtro, string logApiId)
        {
            List<ResumenDocumentosClienteApiDTO> retorno = new List<ResumenDocumentosClienteApiDTO>();

            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    //var configuracionPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    //string documentos = string.Empty;
                    //if (string.IsNullOrEmpty(filtro.TipoDocumento))
                    //{
                    //    foreach (var doc in configuracionPortal.TiposDocumentosDeuda.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(documentos))
                    //        {
                    //            documentos = "'" + doc + "'";
                    //        }
                    //        else
                    //        {
                    //            documentos = documentos + ",'" + doc + "'";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    var tiposDocs = filtro.TipoDocumento.Split(';');
                    //    foreach (var doc in tiposDocs)
                    //    {
                    //        if (string.IsNullOrEmpty(documentos))
                    //        {
                    //            documentos = "'" + doc + "'";
                    //        }
                    //        else
                    //        {
                    //            documentos = documentos + ",'" + doc + "'";
                    //        }
                    //    }
                    //}



                    //string cuentas = string.Empty;
                    //foreach (var c in configuracionPortal.CuentasContablesDeuda.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(cuentas))
                    //    {
                    //        cuentas = "'" + c + "'";
                    //    }
                    //    else
                    //    {
                    //        cuentas = cuentas + ",'" + c + "'";
                    //    }
                    //}




                    //string sqlWhere = string.Empty;



                    ////if (fechaDesde == null && fechaHasta == null)
                    ////{
                    ////    string fecha = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                    ////    sqlWhere = sqlWhere + " AND CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + fecha + ",112)";
                    ////}

                    ////if (fechaDesde != null && fechaHasta != null)
                    ////{
                    ////    string feDesde = fechaDesde?.Year.ToString() + ((fechaDesde?.Month < 10) ? "0" + fechaDesde?.Month.ToString() : fechaDesde?.Month.ToString()) + ((fechaDesde?.Day < 10) ? "0" + fechaDesde?.Day.ToString() : fechaDesde?.Day.ToString());
                    ////    string feHasta = fechaHasta?.Year.ToString() + ((fechaHasta?.Month < 10) ? "0" + fechaHasta?.Month.ToString() : fechaHasta?.Month.ToString()) + ((fechaHasta?.Day < 10) ? "0" + fechaHasta?.Day.ToString() : fechaHasta?.Day.ToString());
                    ////    sqlWhere = sqlWhere + " AND (CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) >= CONVERT(VARCHAR(10)," + feDesde + ",112) AND  CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + feHasta + ",112))";
                    ////}

                    //bool esJoin = false;

                    //if (!string.IsNullOrEmpty(filtro.ListasPrecio))
                    //{
                    //    string lp = string.Empty;
                    //    foreach (var item in filtro.ListasPrecio.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(lp))
                    //        {
                    //            lp = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            lp += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(lp))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodLista in (" + lp + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CanalesVenta))
                    //{
                    //    string canales = string.Empty;
                    //    foreach (var item in filtro.CanalesVenta.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(canales))
                    //        {
                    //            canales = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            canales += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(canales))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodCan in (" + canales + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.Cobradores))
                    //{
                    //    string c = string.Empty;
                    //    foreach (var item in filtro.Cobradores.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(c))
                    //        {
                    //            c = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            c += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(c))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodCob  in (" + c + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CondicionesVenta))
                    //{
                    //    string condV = string.Empty;
                    //    foreach (var item in filtro.CondicionesVenta.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(condV))
                    //        {
                    //            condV = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            condV += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(condV))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.ConVta in (" + condV + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.Vendedores))
                    //{
                    //    string vend = string.Empty;
                    //    foreach (var item in filtro.Vendedores.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(vend))
                    //        {
                    //            vend = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            vend += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(vend))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodVen in (" + vend + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CategoriasClientes))
                    //{
                    //    string catcli = string.Empty;
                    //    foreach (var item in filtro.CategoriasClientes.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(catcli))
                    //        {
                    //            catcli = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            catcli += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(catcli))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CatCli in (" + catcli + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //string join = string.Empty;
                    //if (esJoin)
                    //{
                    //    join = " inner join softland.cwtcvcl on cwtauxi.CodAux = softland.cwtcvcl.CodAux ";
                    //}

                    ////Excluye alumnos del listado
                    //if (filtro.ExcluyeClientes == 1)
                    //{
                    //    var clientes = _context.ClientesExcluidos.ToList();
                    //    string rutAlumnos = string.Empty;
                    //    foreach (var item in clientes)
                    //    {
                    //        if (string.IsNullOrEmpty(rutAlumnos))
                    //        {
                    //            rutAlumnos = "'" + item.RutCliente + "'";
                    //        }
                    //        else
                    //        {
                    //            rutAlumnos += ",'" + item.RutCliente + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(rutAlumnos))
                    //    {
                    //        sqlWhere = sqlWhere + " AND cwtauxi.RutAux not in (" + rutAlumnos + ") ";
                    //    }
                    //}

                    //string sqlDias = string.Empty;

                    //if (filtro.CantidadDias > 0 && filtro.Estado == "VENCIDO")
                    //{
                    //    sqlDias = " WHERE Atrasado >=" + filtro.CantidadDias;
                    //}
                    //else if (filtro.CantidadDias > 0 && filtro.Estado == "PENDIENTE")
                    //{
                    //    sqlDias = " WHERE (Atrasado*-1) <=" + filtro.CantidadDias;
                    //}

                    //string sqlOrder = "ORDER BY Atrasado ";

                    //if (!string.IsNullOrEmpty(filtro.Estado))
                    //{
                    //    if (string.IsNullOrEmpty(sqlDias))
                    //    {
                    //        sqlDias = " WHERE Estado = '" + filtro.Estado + "'";
                    //    }
                    //    else
                    //    {
                    //        sqlDias = sqlDias + " AND Estado = '" + filtro.Estado + "'";
                    //    }

                    //    if (filtro.Estado == "PENDIENTE")
                    //    {
                    //        sqlOrder = sqlOrder + "asc";
                    //    }
                    //    else //VENCIDO
                    //    {
                    //        sqlOrder = sqlOrder + "desc";
                    //    }

                    //}

                    ////if (año != 0)
                    ////{
                    ////    sqlDias = sqlDias + " AND YEAR(Emision) = " + año;
                    ////}
                    ////else
                    ////{
                    ////    sqlDias = sqlDias + " AND YEAR(Emision) >= " + configuracionPortal.AnioTributario;
                    ////}




                    //conSoftland.Open();

                    //SqlCommand cmd = new SqlCommand();
                    //SqlDataReader result;
                    //cmd.CommandText = "Select * from (select " +
                    //                    "cwpctas.pccodi, " +
                    //                    "cwpctas.pcdesc, " +
                    //                    "cwtauxi.codaux, " +
                    //                    "cwtauxi.RutAux, " +
                    //                    "cwtauxi.nomaux, " +
                    //                    "cwtauxi.Bloqueado, " +
                    //                    "min(cwmovim.movfe) as Emision, " +
                    //                    "min(cwmovim.MovFv) as Vencimiento, " +
                    //                    "DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) AS Atrasado," +
                    //                    "CASE " +
                    //                    "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) > 0 THEN 'VENCIDO' " +
                    //                    "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) <= 0 THEN 'PENDIENTE' " +
                    //                    "END AS Estado, " +
                    //                    "cwttdoc.CodDoc as TipoDoc, " +
                    //                    "cwttdoc.desdoc as Documento, " +
                    //                    "cwmovim.movnumdocref as Nro, " +
                    //                    "sum(cwmovim.movdebe - cwmovim.movhaber) as Saldo, " +
                    //                    "sum(cwmovim.movdebe) as Debe " +
                    //                    "From softland.cwcpbte inner join softland.cwmovim on cwcpbte.cpbano = cwmovim.cpbano and cwcpbte.cpbnum = cwmovim.cpbnum inner join softland.cwtauxi on " +
                    //                    "cwtauxi.codaux = cwmovim.codaux inner join softland.cwpctas on cwmovim.pctcod = cwpctas.pccodi left join softland.cwttdoc on " +
                    //                    "cwmovim.movtipdocref = cwttdoc.coddoc left join softland.cwtaren on cwmovim.AreaCod = cwTAren.CodArn " + join +
                    //                    "Where(((cwmovim.cpbNum <> '00000000')  or(cwmovim.cpbano = '2017' AND cwmovim.cpbNum = '00000000'))) and " +
                    //                    "(cwcpbte.cpbest = 'V') " +
                    //                    "and cwmovim.PctCod in (" + cuentas + ") " +
                    //                    "and cwmovim.MovTipDocRef in (" + documentos + ") " + sqlWhere + " " +
                    //                    "Group By cwtauxi.Bloqueado, cwpctas.pccodi , cwpctas.pcdesc, cwtauxi.codaux, cwtauxi.RutAux, cwmovim.movnumdocref, cwtauxi.nomaux, cwttdoc.desdoc, cwmovim.AreaCod,  " +
                    //                    "cwTAren.DesArn, cwpctas.PCAUXI, cwpctas.PCCDOC,  cwttdoc.coddoc " +
                    //                    "Having(Sum((cwmovim.movdebe - cwmovim.movhaber)) > 0)) as tabla " + sqlDias + sqlOrder;
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Connection = conSoftland;
                    //result = cmd.ExecuteReader();

                    //while (result.Read())
                    //{
                    //    DocumentosCobranzaVm aux = new DocumentosCobranzaVm();
                    //    aux.FolioDocumento = Convert.ToInt32(result["Nro"]);
                    //    aux.TipoDocumento = result["Documento"].ToString();
                    //    aux.CodTipoDocumento = result["TipoDoc"].ToString();
                    //    aux.FechaEmision = Convert.ToDateTime(result["Emision"]);
                    //    aux.FechaVencimiento = Convert.ToDateTime(result["Vencimiento"]);
                    //    aux.RutCliente = result["RutAux"].ToString();
                    //    aux.Bloqueado = result["Bloqueado"].ToString();
                    //    aux.NombreCliente = result["Nomaux"].ToString();
                    //    aux.DiasAtraso = Convert.ToInt32(result["Atrasado"]);
                    //    aux.Estado = result["Estado"].ToString();
                    //    aux.CuentaContable = result["pccodi"].ToString();
                    //    aux.NombreCuenta = result["pcdesc"].ToString();
                    //    aux.MontoDocumento = Convert.ToDecimal(result["Debe"]);
                    //    aux.SaldoDocumento = Convert.ToDecimal(result["Saldo"]);

                    //    retorno.Add(aux);
                    //}
                    //result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var cuentasContables = await this.GetAllCuentasContablesSoftlandAsync(logApiId);
                        //var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var fechaActual = DateTime.Now;
                        string fecha = string.Empty;
                        //if (fechaDesde != null)
                        //{
                        //    fecha = fechaDesde?.ToString("yyyy'-'MM'-'dd");//fechaDesde.Year.ToString() + " - 0" + fechaDesde.Month.ToString() + "-0" + fechaDesde.Day.ToString();
                        //}
                        //else
                        //{
                        //    fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        //}

                        string fechaVencimientoDesde = filtro.Fecha != null ? filtro.Fecha.Value.ToString("yyyyMMdd") : string.Empty;
                        string fechaVencimientoHasta = filtro.FechaHasta != null ? filtro.FechaHasta.Value.ToString("yyyyMMdd") : string.Empty;


                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";

                        string listaDocumentos = string.Empty;
                        if (!string.IsNullOrEmpty(filtro.TipoDocumento))
                        {
                            listaDocumentos = filtro.TipoDocumento.Replace(";", ",");
                        }
                        else
                        {
                            listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        }


                        string estado = string.Empty;
                        if (filtro.Estado == "VENCIDO")
                        {
                            estado = "V";

                        }
                        else if (filtro.Estado == "PENDIENTE")
                        {
                            estado = "P";

                        }

                        string listaCategorias = !string.IsNullOrEmpty(filtro.CategoriasClientes) ? filtro.CategoriasClientes.Replace(";", ",") : string.Empty;
                        string listaVendedores = !string.IsNullOrEmpty(filtro.Vendedores) ? filtro.Vendedores.Replace(";", ",") : string.Empty;
                        string listaCondicionesVenta = !string.IsNullOrEmpty(filtro.CondicionesVenta) ? filtro.CondicionesVenta.Replace(";", ",") : string.Empty;
                        int diasVencimiento = filtro.CantidadDias != null ? filtro.CantidadDias : 0;

                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", fecha).Replace("{SOLOSALDO}", "0").Replace("{AREADATOS}", api.AreaDatos).Replace("codaux={CODAUX}&", "");
                        string url = api.Url + api.DocContabilizadosResumenxRut.Replace("{CANTIDAD}", filtro.Cantidad.ToString()).Replace("{CODAUX}", filtro.CodAuxCliente).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasVencimiento.ToString()).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estado).Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", filtro.Pagina.ToString()).Replace("{RUTAUX}", filtro.RutCliente).Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", fechaVencimientoDesde).Replace("{FECHAVENCIMIENTOHASTA}", fechaVencimientoHasta).Replace("{LISTACAGETORIAS}", listaCategorias)
                        .Replace("{LISTACONDICIONVENTA}", listaCondicionesVenta).Replace("{LISTAVENDEDORES}", listaVendedores);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizadosResumenXRut";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ResumenDocumentosClienteApiDTO>> clientes = JsonConvert.DeserializeObject<List<List<ResumenDocumentosClienteApiDTO>>>(content);
                            retorno = clientes[0].Where(x => !string.IsNullOrEmpty(x.codaux)).ToList();
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosPendientesCobranzaAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"Softland\GetDocumentosPendientesCobranzaAsync";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return retorno;
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        public async Task<List<DocumentoContabilizadoAPIDTO>> GetDocumentosXCliente(FiltroCobranzaVm filtro, string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();

            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    //var configuracionPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    //string documentos = string.Empty;
                    //if (string.IsNullOrEmpty(filtro.TipoDocumento))
                    //{
                    //    foreach (var doc in configuracionPortal.TiposDocumentosDeuda.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(documentos))
                    //        {
                    //            documentos = "'" + doc + "'";
                    //        }
                    //        else
                    //        {
                    //            documentos = documentos + ",'" + doc + "'";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    var tiposDocs = filtro.TipoDocumento.Split(';');
                    //    foreach (var doc in tiposDocs)
                    //    {
                    //        if (string.IsNullOrEmpty(documentos))
                    //        {
                    //            documentos = "'" + doc + "'";
                    //        }
                    //        else
                    //        {
                    //            documentos = documentos + ",'" + doc + "'";
                    //        }
                    //    }
                    //}



                    //string cuentas = string.Empty;
                    //foreach (var c in configuracionPortal.CuentasContablesDeuda.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(cuentas))
                    //    {
                    //        cuentas = "'" + c + "'";
                    //    }
                    //    else
                    //    {
                    //        cuentas = cuentas + ",'" + c + "'";
                    //    }
                    //}




                    //string sqlWhere = string.Empty;



                    ////if (fechaDesde == null && fechaHasta == null)
                    ////{
                    ////    string fecha = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                    ////    sqlWhere = sqlWhere + " AND CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + fecha + ",112)";
                    ////}

                    ////if (fechaDesde != null && fechaHasta != null)
                    ////{
                    ////    string feDesde = fechaDesde?.Year.ToString() + ((fechaDesde?.Month < 10) ? "0" + fechaDesde?.Month.ToString() : fechaDesde?.Month.ToString()) + ((fechaDesde?.Day < 10) ? "0" + fechaDesde?.Day.ToString() : fechaDesde?.Day.ToString());
                    ////    string feHasta = fechaHasta?.Year.ToString() + ((fechaHasta?.Month < 10) ? "0" + fechaHasta?.Month.ToString() : fechaHasta?.Month.ToString()) + ((fechaHasta?.Day < 10) ? "0" + fechaHasta?.Day.ToString() : fechaHasta?.Day.ToString());
                    ////    sqlWhere = sqlWhere + " AND (CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) >= CONVERT(VARCHAR(10)," + feDesde + ",112) AND  CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + feHasta + ",112))";
                    ////}

                    //bool esJoin = false;

                    //if (!string.IsNullOrEmpty(filtro.ListasPrecio))
                    //{
                    //    string lp = string.Empty;
                    //    foreach (var item in filtro.ListasPrecio.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(lp))
                    //        {
                    //            lp = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            lp += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(lp))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodLista in (" + lp + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CanalesVenta))
                    //{
                    //    string canales = string.Empty;
                    //    foreach (var item in filtro.CanalesVenta.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(canales))
                    //        {
                    //            canales = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            canales += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(canales))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodCan in (" + canales + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.Cobradores))
                    //{
                    //    string c = string.Empty;
                    //    foreach (var item in filtro.Cobradores.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(c))
                    //        {
                    //            c = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            c += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(c))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodCob  in (" + c + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CondicionesVenta))
                    //{
                    //    string condV = string.Empty;
                    //    foreach (var item in filtro.CondicionesVenta.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(condV))
                    //        {
                    //            condV = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            condV += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(condV))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.ConVta in (" + condV + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.Vendedores))
                    //{
                    //    string vend = string.Empty;
                    //    foreach (var item in filtro.Vendedores.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(vend))
                    //        {
                    //            vend = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            vend += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(vend))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodVen in (" + vend + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(filtro.CategoriasClientes))
                    //{
                    //    string catcli = string.Empty;
                    //    foreach (var item in filtro.CategoriasClientes.Split(';'))
                    //    {
                    //        if (string.IsNullOrEmpty(catcli))
                    //        {
                    //            catcli = "'" + item + "'";
                    //        }
                    //        else
                    //        {
                    //            catcli += ",'" + item + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(catcli))
                    //    {
                    //        sqlWhere = sqlWhere + "AND softland.cwtcvcl.CatCli in (" + catcli + ")";
                    //        esJoin = true;
                    //    }
                    //}

                    //string join = string.Empty;
                    //if (esJoin)
                    //{
                    //    join = " inner join softland.cwtcvcl on cwtauxi.CodAux = softland.cwtcvcl.CodAux ";
                    //}

                    ////Excluye alumnos del listado
                    //if (filtro.ExcluyeClientes == 1)
                    //{
                    //    var clientes = _context.ClientesExcluidos.ToList();
                    //    string rutAlumnos = string.Empty;
                    //    foreach (var item in clientes)
                    //    {
                    //        if (string.IsNullOrEmpty(rutAlumnos))
                    //        {
                    //            rutAlumnos = "'" + item.RutCliente + "'";
                    //        }
                    //        else
                    //        {
                    //            rutAlumnos += ",'" + item.RutCliente + "'";
                    //        }
                    //    }

                    //    if (!string.IsNullOrEmpty(rutAlumnos))
                    //    {
                    //        sqlWhere = sqlWhere + " AND cwtauxi.RutAux not in (" + rutAlumnos + ") ";
                    //    }
                    //}

                    //string sqlDias = string.Empty;

                    //if (filtro.CantidadDias > 0 && filtro.Estado == "VENCIDO")
                    //{
                    //    sqlDias = " WHERE Atrasado >=" + filtro.CantidadDias;
                    //}
                    //else if (filtro.CantidadDias > 0 && filtro.Estado == "PENDIENTE")
                    //{
                    //    sqlDias = " WHERE (Atrasado*-1) <=" + filtro.CantidadDias;
                    //}

                    //string sqlOrder = "ORDER BY Atrasado ";

                    //if (!string.IsNullOrEmpty(filtro.Estado))
                    //{
                    //    if (string.IsNullOrEmpty(sqlDias))
                    //    {
                    //        sqlDias = " WHERE Estado = '" + filtro.Estado + "'";
                    //    }
                    //    else
                    //    {
                    //        sqlDias = sqlDias + " AND Estado = '" + filtro.Estado + "'";
                    //    }

                    //    if (filtro.Estado == "PENDIENTE")
                    //    {
                    //        sqlOrder = sqlOrder + "asc";
                    //    }
                    //    else //VENCIDO
                    //    {
                    //        sqlOrder = sqlOrder + "desc";
                    //    }

                    //}

                    ////if (año != 0)
                    ////{
                    ////    sqlDias = sqlDias + " AND YEAR(Emision) = " + año;
                    ////}
                    ////else
                    ////{
                    ////    sqlDias = sqlDias + " AND YEAR(Emision) >= " + configuracionPortal.AnioTributario;
                    ////}




                    //conSoftland.Open();

                    //SqlCommand cmd = new SqlCommand();
                    //SqlDataReader result;
                    //cmd.CommandText = "Select * from (select " +
                    //                    "cwpctas.pccodi, " +
                    //                    "cwpctas.pcdesc, " +
                    //                    "cwtauxi.codaux, " +
                    //                    "cwtauxi.RutAux, " +
                    //                    "cwtauxi.nomaux, " +
                    //                    "cwtauxi.Bloqueado, " +
                    //                    "min(cwmovim.movfe) as Emision, " +
                    //                    "min(cwmovim.MovFv) as Vencimiento, " +
                    //                    "DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) AS Atrasado," +
                    //                    "CASE " +
                    //                    "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) > 0 THEN 'VENCIDO' " +
                    //                    "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) <= 0 THEN 'PENDIENTE' " +
                    //                    "END AS Estado, " +
                    //                    "cwttdoc.CodDoc as TipoDoc, " +
                    //                    "cwttdoc.desdoc as Documento, " +
                    //                    "cwmovim.movnumdocref as Nro, " +
                    //                    "sum(cwmovim.movdebe - cwmovim.movhaber) as Saldo, " +
                    //                    "sum(cwmovim.movdebe) as Debe " +
                    //                    "From softland.cwcpbte inner join softland.cwmovim on cwcpbte.cpbano = cwmovim.cpbano and cwcpbte.cpbnum = cwmovim.cpbnum inner join softland.cwtauxi on " +
                    //                    "cwtauxi.codaux = cwmovim.codaux inner join softland.cwpctas on cwmovim.pctcod = cwpctas.pccodi left join softland.cwttdoc on " +
                    //                    "cwmovim.movtipdocref = cwttdoc.coddoc left join softland.cwtaren on cwmovim.AreaCod = cwTAren.CodArn " + join +
                    //                    "Where(((cwmovim.cpbNum <> '00000000')  or(cwmovim.cpbano = '2017' AND cwmovim.cpbNum = '00000000'))) and " +
                    //                    "(cwcpbte.cpbest = 'V') " +
                    //                    "and cwmovim.PctCod in (" + cuentas + ") " +
                    //                    "and cwmovim.MovTipDocRef in (" + documentos + ") " + sqlWhere + " " +
                    //                    "Group By cwtauxi.Bloqueado, cwpctas.pccodi , cwpctas.pcdesc, cwtauxi.codaux, cwtauxi.RutAux, cwmovim.movnumdocref, cwtauxi.nomaux, cwttdoc.desdoc, cwmovim.AreaCod,  " +
                    //                    "cwTAren.DesArn, cwpctas.PCAUXI, cwpctas.PCCDOC,  cwttdoc.coddoc " +
                    //                    "Having(Sum((cwmovim.movdebe - cwmovim.movhaber)) > 0)) as tabla " + sqlDias + sqlOrder;
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Connection = conSoftland;
                    //result = cmd.ExecuteReader();

                    //while (result.Read())
                    //{
                    //    DocumentosCobranzaVm aux = new DocumentosCobranzaVm();
                    //    aux.FolioDocumento = Convert.ToInt32(result["Nro"]);
                    //    aux.TipoDocumento = result["Documento"].ToString();
                    //    aux.CodTipoDocumento = result["TipoDoc"].ToString();
                    //    aux.FechaEmision = Convert.ToDateTime(result["Emision"]);
                    //    aux.FechaVencimiento = Convert.ToDateTime(result["Vencimiento"]);
                    //    aux.RutCliente = result["RutAux"].ToString();
                    //    aux.Bloqueado = result["Bloqueado"].ToString();
                    //    aux.NombreCliente = result["Nomaux"].ToString();
                    //    aux.DiasAtraso = Convert.ToInt32(result["Atrasado"]);
                    //    aux.Estado = result["Estado"].ToString();
                    //    aux.CuentaContable = result["pccodi"].ToString();
                    //    aux.NombreCuenta = result["pcdesc"].ToString();
                    //    aux.MontoDocumento = Convert.ToDecimal(result["Debe"]);
                    //    aux.SaldoDocumento = Convert.ToDecimal(result["Saldo"]);

                    //    retorno.Add(aux);
                    //}
                    //result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var cuentasContables = await this.GetAllCuentasContablesSoftlandAsync(logApiId);
                        //var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var fechaActual = DateTime.Now;
                        string fecha = string.Empty;
                        //if (fechaDesde != null)
                        //{
                        //    fecha = fechaDesde?.ToString("yyyy'-'MM'-'dd");//fechaDesde.Year.ToString() + " - 0" + fechaDesde.Month.ToString() + "-0" + fechaDesde.Day.ToString();
                        //}
                        //else
                        //{
                        //    fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        //}

                        string fechaVencimientoDesde = filtro.Fecha != null ? filtro.Fecha.Value.ToString("yyyyMMdd") : string.Empty;
                        string fechaVencimientoHasta = filtro.FechaHasta != null ? filtro.FechaHasta.Value.ToString("yyyyMMdd") : string.Empty;


                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";

                        string listaDocumentos = string.Empty;
                        if (!string.IsNullOrEmpty(filtro.TipoDocumento))
                        {
                            listaDocumentos = filtro.TipoDocumento.Replace(";", ",");
                        }
                        else
                        {
                            listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        }


                        string estado = string.Empty;
                        if (filtro.Estado == "VENCIDO")
                        {
                            estado = "V";

                        }
                        else if (filtro.Estado == "PENDIENTE")
                        {
                            estado = "P";

                        }

                        string listaCategorias = !string.IsNullOrEmpty(filtro.CategoriasClientes) ? filtro.CategoriasClientes.Replace(";", ",") : string.Empty;
                        string listaVendedores = !string.IsNullOrEmpty(filtro.Vendedores) ? filtro.Vendedores.Replace(";", ",") : string.Empty;
                        string listaCondicionesVenta = !string.IsNullOrEmpty(filtro.CondicionesVenta) ? filtro.CondicionesVenta.Replace(";", ",") : string.Empty;
                        int diasVencimiento = filtro.CantidadDias != null ? filtro.CantidadDias : 0;

                        int pagina = 1;
                        int cantidad = 50;

                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", fecha).Replace("{SOLOSALDO}", "0").Replace("{AREADATOS}", api.AreaDatos).Replace("codaux={CODAUX}&", "");
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", filtro.CodAuxCliente).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasVencimiento.ToString()).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estado).Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", fechaVencimientoDesde).Replace("{FECHAVENCIMIENTOHASTA}", fechaVencimientoHasta).Replace("{LISTACAGETORIAS}", listaCategorias)
                        .Replace("{LISTACONDICIONVENTA}", listaCondicionesVenta).Replace("{LISTAVENDEDORES}", listaVendedores);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> clientes = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            retorno = clientes[0].Where(x => !string.IsNullOrEmpty(x.CodAux)).ToList();
                            if (retorno.Count > 0)
                            {

                                while (retorno.Count < retorno[0].total)
                                {
                                    pagina = pagina + 1;
                                    using (var client2 = new HttpClient())
                                    {
                                        string url2 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasVencimiento.ToString()).Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", estado).Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                                         .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", fechaVencimientoDesde).Replace("{FECHAVENCIMIENTOHASTA}", fechaVencimientoHasta).Replace("{LISTACAGETORIAS}", listaCategorias)
                                         .Replace("{LISTACONDICIONVENTA}", listaCondicionesVenta).Replace("{LISTAVENDEDORES}", listaVendedores);

                                        client2.BaseAddress = new Uri(url2);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "DocumentosContabilizados";

                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response2.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);
                                            var listDocs = documentos2[0].Where(x => !string.IsNullOrEmpty(x.CodAux)).ToList();
                                            if (listDocs.Count > 0)
                                            {
                                                retorno.AddRange(listDocs);
                                            }

                                        }
                                        else
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response2.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content2,
                                                Ruta = "SoftlandService/GetDocumentosXCliente"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosPendientesCobranzaAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"Softland\GetDocumentosPendientesCobranzaAsync";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return retorno;
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        public async Task<List<DocumentosCobranzaVm>> GetDocumentosPendientesCobranzaSinFiltroAsync(Nullable<DateTime> fechaDesde, Nullable<DateTime> fechaHasta, string tipoDocumento, string logApiId)
        {
            List<DocumentosCobranzaVm> retorno = new List<DocumentosCobranzaVm>();

            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    var configuracionPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    string documentos = string.Empty;
                    if (string.IsNullOrEmpty(tipoDocumento))
                    {
                        foreach (var doc in configuracionPortal.TiposDocumentosDeuda.Split(';'))
                        {
                            if (string.IsNullOrEmpty(documentos))
                            {
                                documentos = "'" + doc + "'";
                            }
                            else
                            {
                                documentos = documentos + ",'" + doc + "'";
                            }
                        }
                    }
                    else
                    {
                        documentos = "'" + tipoDocumento + "'";
                    }



                    string cuentas = string.Empty;
                    foreach (var c in configuracionPortal.CuentasContablesDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(cuentas))
                        {
                            cuentas = "'" + c + "'";
                        }
                        else
                        {
                            cuentas = cuentas + ",'" + c + "'";
                        }
                    }

                    string sqlWhere = string.Empty;

                    //if (año != 0)
                    //{
                    //    sqlWhere = sqlWhere + " AND CWCpbte.CpbAno = " + año;
                    //}

                    //if (fechaDesde == null && fechaHasta == null)
                    //{
                    //    string fecha = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                    //    sqlWhere = sqlWhere + " AND CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + fecha + ",112)";
                    //}

                    //if (fechaDesde != null && fechaHasta != null)
                    //{
                    //    string feDesde = fechaDesde?.Year.ToString() + ((fechaDesde?.Month < 10) ? "0" + fechaDesde?.Month.ToString() : fechaDesde?.Month.ToString()) + ((fechaDesde?.Day < 10) ? "0" + fechaDesde?.Day.ToString() : fechaDesde?.Day.ToString());
                    //    string feHasta = fechaHasta?.Year.ToString() + ((fechaHasta?.Month < 10) ? "0" + fechaHasta?.Month.ToString() : fechaHasta?.Month.ToString()) + ((fechaHasta?.Day < 10) ? "0" + fechaHasta?.Day.ToString() : fechaHasta?.Day.ToString());
                    //    sqlWhere = sqlWhere + " AND (CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) >= CONVERT(VARCHAR(10)," + feDesde + ",112) AND  CONVERT(VARCHAR(10),CWCpbte.CpbFec,112) <= CONVERT(VARCHAR(10)," + feHasta + ",112))";
                    //}

                    conSoftland.Open();

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader result;
                    cmd.CommandText = "select " +
                                        "cwpctas.pccodi, " +
                                        "cwpctas.pcdesc, " +
                                        "cwtauxi.codaux, " +
                                        "cwtauxi.RutAux, " +
                                        "cwtauxi.nomaux, " +
                                        "min(cwmovim.movfe) as Emision, " +
                                        "min(cwmovim.MovFv) as Vencimiento, " +
                                        "DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) AS Atrasado," +
                                        "CASE " +
                                        "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) > 0 THEN 'VENCIDO' " +
                                        "WHEN DATEDIFF(day, min(cwmovim.MovFv), GETDATE()) <= 0 THEN 'PENDIENTE' " +
                                        "END AS Estado, " +
                                        "cwttdoc.CodDoc as TipoDoc, " +
                                        "cwttdoc.desdoc as Documento, " +
                                        "cwmovim.movnumdocref as Nro, " +
                                        "sum(cwmovim.movdebe - cwmovim.movhaber) as Saldo, " +
                                        "sum(cwmovim.movdebe) as Debe " +
                                        "From softland.cwcpbte inner join softland.cwmovim on cwcpbte.cpbano = cwmovim.cpbano and cwcpbte.cpbnum = cwmovim.cpbnum inner join softland.cwtauxi on " +
                                        "cwtauxi.codaux = cwmovim.codaux inner join softland.cwpctas on cwmovim.pctcod = cwpctas.pccodi left join softland.cwttdoc on " +
                                        "cwmovim.movtipdocref = cwttdoc.coddoc left join softland.cwtaren on cwmovim.AreaCod = cwTAren.CodArn " +
                                        "Where(((cwmovim.cpbNum <> '00000000')  or(cwmovim.cpbano = '2017' AND cwmovim.cpbNum = '00000000'))) and " +
                                        "(cwcpbte.cpbest = 'V') " +
                                        "and cwmovim.PctCod in (" + cuentas + ") " +
                                        "and cwmovim.TtdCod in (" + documentos + ") " + sqlWhere + " " +
                                        "Group By cwpctas.pccodi , cwpctas.pcdesc, cwtauxi.codaux, cwtauxi.RutAux, cwmovim.movnumdocref, cwtauxi.nomaux, cwttdoc.desdoc, cwmovim.AreaCod,  " +
                                        "cwTAren.DesArn, cwpctas.PCAUXI, cwpctas.PCCDOC,  cwttdoc.coddoc " +
                                        "Having(Sum((cwmovim.movdebe - cwmovim.movhaber)) > 0) " +
                                        "order by Estado desc, cwpctas.pccodi, cwtauxi.codaux, cwmovim.movnumdocref, Vencimiento--opcional";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    result = cmd.ExecuteReader();

                    while (result.Read())
                    {

                        DocumentosCobranzaVm aux = new DocumentosCobranzaVm();
                        aux.FolioDocumento = Convert.ToInt32(result["Nro"]);
                        aux.TipoDocumento = result["Documento"].ToString();
                        aux.CodTipoDocumento = result["TipoDoc"].ToString();
                        aux.FechaEmision = Convert.ToDateTime(result["Emision"]);
                        aux.FechaVencimiento = Convert.ToDateTime(result["Vencimiento"]);
                        aux.RutCliente = result["RutAux"].ToString();
                        aux.Bloqueado = result["Bloqueado"].ToString();

                        if (!string.IsNullOrEmpty(aux.RutCliente))
                        {
                            var cliente = _context.ClientesPortals.Where(x => x.Rut == aux.RutCliente).FirstOrDefault();
                            aux.NombreCliente = (cliente != null) ? cliente.Nombre : "";
                        }
                        aux.DiasAtraso = Convert.ToInt32(result["Atrasado"]);
                        aux.Estado = result["Estado"].ToString();
                        aux.CuentaContable = result["pccodi"].ToString();
                        aux.NombreCuenta = result["pcdesc"].ToString();
                        aux.MontoDocumento = Convert.ToDecimal(result["Debe"]);
                        aux.SaldoDocumento = Convert.ToDecimal(result["Saldo"]);
                        retorno.Add(aux);
                    }
                    result.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var cuentasContables = await this.GetAllCuentasContablesSoftlandAsync(logApiId);
                        //var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var fechaActual = new DateTime();
                        string fecha = string.Empty;
                        //if (fechaDesde != null)
                        //{
                        //    fecha = fechaDesde?.Year.ToString() + "-0" + fechaDesde?.Month.ToString() + "-0" + fechaDesde?.Day.ToString();
                        //}
                        //else
                        //{
                        //    fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        //}
                        string fechaVencimientoDesde = string.Empty;
                        string fechaVencimientoHasta = string.Empty;
                        if (fechaDesde != null)
                        {
                            fechaVencimientoDesde = fechaDesde.Value.Year.ToString() + fechaDesde.Value.Month.ToString("00") + fechaDesde.Value.Day.ToString("00");
                        }
                        if (fechaHasta != null)
                        {
                            fechaVencimientoHasta = fechaHasta.Value.Year.ToString() + fechaHasta.Value.Month.ToString("00") + fechaHasta.Value.Day.ToString("00");
                        }

                        string accesToken = api.Token;
                        string listaDocumentos = string.Empty;
                        // string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", fecha).Replace("{SOLOSALDO}", "0").Replace("{AREADATOS}", api.AreaDatos).Replace("codaux={CODAUX}&", "");
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";

                        if (!string.IsNullOrEmpty(tipoDocumento))
                        {
                            listaDocumentos = tipoDocumento.Replace(";", ",");
                        }
                        else
                        {
                            listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        }

                        int cantidad = 100;
                        int pagina = 1;
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "0").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", fechaVencimientoDesde).Replace("{FECHAVENCIMIENTOHASTA}", fechaVencimientoHasta).Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);

                            if (documentos[0].Count > 0)
                            {
                                pagina = pagina + 1;

                                while (documentos[0].Count < documentos[0][0].total)
                                {
                                    using (var client2 = new HttpClient())
                                    {

                                        string url2 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "0").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", fechaVencimientoDesde).Replace("{FECHAVENCIMIENTOHASTA}", fechaVencimientoHasta).Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                                        client2.BaseAddress = new Uri(url2);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "DocumentosContabilizados";


                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response2.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);

                                            documentos[0].AddRange(documentos2[0]);
                                            pagina = pagina + 1;
                                        }
                                        else
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response2.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content2,
                                                Ruta = "SoftlandService/GetDocumentosPendientes"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                        }

                                    }
                                }


                                //if (año != null && año != 0)
                                //{
                                //    documentos[0] = documentos[0].Where(x => x.Movfe.Value.Year == año).ToList();
                                //}

                                documentos[0] = documentos[0].Where(x => x.Saldobase > 0).ToList();
                                //if (fechaHasta != null)
                                //{
                                //    documentos[0] = documentos[0].Where(x => x.Movfe <= fechaHasta).ToList();
                                //}

                                //if (!string.IsNullOrEmpty(tipoDocumento))
                                //{
                                //    documentos[0] = documentos[0].Where(x => tipoDocumento.Contains(x.Ttdcod)).ToList();
                                //}
                            }



                            foreach (var doc in documentos[0])
                            {
                                string url2 = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", "").Replace("{CATCLI}", "").Replace("{CODAUX}", doc.CodAux).Replace("{CODLISTA}", "").Replace("{CODVEN}", "").Replace("{CONVTA}", "")
                            .Replace("{NOMBRE}", "").Replace("{RUT}", "");

                                using (var client2 = new HttpClient())
                                {
                                    client2.BaseAddress = new Uri(url2);
                                    client2.DefaultRequestHeaders.Accept.Clear();
                                    client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                    //client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                    client2.DefaultRequestHeaders.Add("SApiKey", accesToken);
                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                    LogApiDetalle logApiDetalle3 = new LogApiDetalle();
                                    logApiDetalle3.IdLogApi = logApiId;
                                    logApiDetalle3.Inicio = DateTime.Now;
                                    logApiDetalle3.Metodo = "ConsultaTiposDeDocumentos";


                                    HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress);

                                    logApiDetalle3.Termino = DateTime.Now;
                                    logApiDetalle3.Segundos = (int?)Math.Round((logApiDetalle3.Termino - logApiDetalle3.Inicio).Value.TotalSeconds);
                                    this.guardarDetalleLogApi(logApiDetalle3);

                                    if (response2.IsSuccessStatusCode)
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        List<List<ClienteAPIDTO>> clientes = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content2);
                                        clientes[0] = clientes[0].Where(x => x.CodAux != null).ToList();


                                        var valido = clientes[0].Where(x => x.CodAux == doc.CodAux).FirstOrDefault();
                                        if (valido != null)
                                        {
                                            DocumentosCobranzaVm aux = new DocumentosCobranzaVm();
                                            aux.FolioDocumento = (int)doc.Numdoc;
                                            aux.TipoDocumento = doc.DesDoc;
                                            aux.CodTipoDocumento = doc.Ttdcod;
                                            aux.FechaEmision = Convert.ToDateTime(doc.Movfe);
                                            aux.FechaVencimiento = Convert.ToDateTime(doc.Movfv);
                                            aux.RutCliente = valido.RutAux;
                                            aux.Bloqueado = valido.Bloqueado;
                                            aux.NombreCliente = valido.NomAux;
                                            aux.DiasAtraso = (int)(fechaActual.Date - doc.Movfv.Value.Date).TotalDays;
                                            aux.Estado = doc.Estado == "V" ? "VENCIDO" : doc.Estado == "P" ? "PENDIENTE" : "";
                                            aux.CuentaContable = doc.Pctcod;
                                            var cuenta = cuentasContables.Where(x => x.Codigo == doc.Pctcod).FirstOrDefault();
                                            if (cuenta != null) { aux.NombreCuenta = cuenta.Nombre; }
                                            aux.MontoDocumento = (decimal)doc.MovMonto;
                                            aux.SaldoDocumento = (decimal)doc.Saldobase;
                                            retorno.Add(aux);
                                        }


                                    }
                                    else
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = response2.StatusCode.ToString(),
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = content2,
                                            Ruta = "SoftlandService/GetDocumentosPendientes"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }
                                }
                            }




                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosPendientes"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"Softland\GetDocumentosPendientes";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return retorno;
            }
            finally { conSoftland.Close(); }

            return retorno;
        }




        public async Task<List<ClientesPortal>> GetClientesSoftlandFiltrosAsync(FilterVm filtros, string logApiId)
        {
            List<ClientesPortal> retorno = new List<ClientesPortal>();
            try
            {
                if (utilizaApiSoftland == "false")
                {
                    string innerJoin = string.Empty;
                    string sqlWhere = string.Empty;

                    if (!string.IsNullOrEmpty(filtros.ListaPrecio) || !string.IsNullOrEmpty(filtros.CondicionVenta) || !string.IsNullOrEmpty(filtros.Vendedor) || !string.IsNullOrEmpty(filtros.CategoriaCliente))
                    {
                        innerJoin = "inner join softland.cwtcvcl on softland.cwtauxi.CodAux = softland.cwtcvcl.CodAux ";
                    }

                    if (!string.IsNullOrEmpty(filtros.ListaPrecio))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodLista = '" + filtros.ListaPrecio + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtcvcl.CodLista = '" + filtros.ListaPrecio + "' ";
                        }
                    }

                    if (!string.IsNullOrEmpty(filtros.CondicionVenta))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtcvcl.ConVta = '" + filtros.CondicionVenta + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtcvcl.ConVta = '" + filtros.CondicionVenta + "' ";
                        }
                    }


                    if (!string.IsNullOrEmpty(filtros.Vendedor))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtcvcl.CodVen = '" + filtros.Vendedor + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtcvcl.CodVen = '" + filtros.Vendedor + "' ";
                        }
                    }

                    if (!string.IsNullOrEmpty(filtros.CategoriaCliente))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtcvcl.CatCli = '" + filtros.CategoriaCliente + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtcvcl.CatCli = '" + filtros.CategoriaCliente + "' ";
                        }
                    }

                    if (!string.IsNullOrEmpty(filtros.Nombre))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtauxi.NomAux = '" + filtros.Nombre + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtauxi.NomAux = '" + filtros.Nombre + "' ";
                        }
                    }


                    if (!string.IsNullOrEmpty(filtros.CodAux))
                    {
                        if (!string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + "AND softland.cwtauxi.CodAux = '" + filtros.CodAux + "' ";
                        }
                        else
                        {
                            sqlWhere = "WHERE softland.cwtauxi.CodAux = '" + filtros.CodAux + "' ";
                        }
                    }


                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "select softland.cwtauxi.CodAux, softland.cwtauxi.NomAux, softland.cwtauxi.RutAux from softland.cwtauxi " + innerJoin + sqlWhere;




                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ClientesPortal item = new ClientesPortal();
                        item.CodAux = (reader["CodAux"] == DBNull.Value) ? "" : reader["CodAux"].ToString();
                        item.Rut = (reader["RutAux"] == DBNull.Value) ? "" : reader["RutAux"].ToString();
                        item.Nombre = (reader["NomAux"] == DBNull.Value) ? "" : reader["NomAux"].ToString().ToUpper();
                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        int pagina = 1;
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        string url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", "").Replace("{CATCLI}", filtros.CategoriaCliente).Replace("{CODAUX}", filtros.CodAux).Replace("{CODLISTA}", filtros.ListaPrecio).Replace("{CODVEN}", filtros.Vendedor).Replace("{CONVTA}", filtros.CondicionVenta)
                           .Replace("{NOMBRE}", filtros.Nombre).Replace("{RUT}", filtros.Rut);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaCliente";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ClienteAPIDTO>> clientes = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);
                            clientes[0] = clientes[0].Where(x => x.CodAux != null && x.CodAux != "").ToList();
                            if (clientes[0].Count() > 0)
                            {
                                while (int.Parse(clientes[0][0].Total) > clientes[0].Count())
                                {
                                    using (var client2 = new HttpClient())
                                    {
                                        pagina = pagina + 1;
                                        url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", filtros.CategoriaCliente).Replace("{CODAUX}", filtros.CodAux).Replace("{CODLISTA}", filtros.ListaPrecio).Replace("{CODVEN}", filtros.Vendedor).Replace("{CONVTA}", filtros.CondicionVenta)
                          .Replace("{NOMBRE}", filtros.Nombre).Replace("{RUT}", filtros.Rut);
                                        client2.BaseAddress = new Uri(url);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "ConsultaTiposDeDocumentos";


                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response2.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<ClienteAPIDTO>> clientes2 = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content2);
                                            if (clientes2[0].Count() > 0)
                                            {
                                                clientes[0].AddRange(clientes2[0]);
                                            }

                                        }
                                        else
                                        {
                                            var content2 = await response.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content,
                                                Ruta = "SoftlandService/GetClientesSoftlandFiltrosAsync"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }

                            retorno = clientes[0].ConvertAll(c => new ClientesPortal
                            {
                                Rut = c.RutAux,
                                Nombre = c.NomAux,
                                Correo = c.EMail,
                                CodAux = c.CodAux
                            });

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetClientesSoftlandFiltrosAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                if (utilizaApiSoftland == "fasle")
                {
                    conSoftland.Close();
                }

                LogProceso log = new LogProceso();
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"SoftlandService\GetClientesSoftlandCobranza";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                throw;
            }


            return retorno;
        }
        #endregion

        public async Task<List<ClienteSaldosDTO>> GetDocumentosDashboardAdminAsync(string codAux, int estado, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    var configuracionPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                    string docs = string.Empty;

                    foreach (var item in configuracionPago.TiposDocumentosDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(docs))
                        {
                            docs = "'" + item + "'";
                        }
                        else
                        {
                            docs = docs + ",'" + item + "'";
                        }
                    }

                    string cuentasContables = string.Empty;
                    foreach (var item in configuracionPago.CuentasContablesDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(cuentasContables))
                        {
                            cuentasContables = "'" + item + "'";
                        }
                        else
                        {
                            cuentasContables = cuentasContables + ",'" + item + "'";
                        }
                    }

                    string sqlWhere = string.Empty;
                    if (!string.IsNullOrEmpty(codAux) && codAux != "null")
                    {
                        sqlWhere = "CodAux = " + codAux + "AND";
                    }

                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT ISNULL((SELECT ctdoc.DesDoc FROM softland.cwttdoc as ctdoc WHERE ctdoc.CodDoc = CWDoctosPeriodo.Ttdcod),'Sin Documento') as Documento, TtdCod, " +
                                      " NumDOC as Folio,MovFe as Fecha, MovFv as [Fecha Vcto.], isnull(sum(MovDebe) + Sum(MovHaber), 0) as Monto, isnull(sum(Debe) - Sum(Haber), 0) as Saldo,  " +
                                      " CASE WHEN(MovFv <= GETDATE()) THEN 'Vencido' else 'Pendiente' end as Estado, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                                      " FROM softland.CWDoctosPeriodoFull AS CWDoctosPeriodo left join softland.cwtauxi on softland.cwtauxi.CodAux = CWDoctosPeriodo.CodAux " +
                                      " WHERE TtdCod in (" + docs + ") AND " + sqlWhere +
                                      " (PctCod IN(" + cuentasContables + ")) AND " +
                                      " (CpbAno >= " + configuracionPago.AnioTributario + ") " +
                                      " GROUP BY  TtdCod,NumDOC, MovFe, MovFv, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                                      " HAVING(sum(Debe) - Sum(Haber)) <> 0 ORDER BY  MovFe DESC ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ClienteSaldosDTO item = new ClienteSaldosDTO();
                        item.Documento = reader["Documento"].ToString();
                        item.RazonSocial = reader["NomAux"].ToString();
                        item.Nro = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Folio"]);
                        item.FechaEmision = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                        item.FechaVcto = (reader["Fecha Vcto."] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha Vcto."]);
                        item.Debe = (reader["Monto"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Monto"]);
                        item.Saldo = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                        item.Estado = reader["Estado"].ToString();
                        item.TipoDoc = reader["TtdCod"].ToString();
                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();

                    if (retorno.Count > 0 && estado != 0)
                    {
                        if (estado == 1)
                        {
                            retorno = retorno.Where(x => x.Estado == "Pendiente").ToList();
                        }
                        else if (estado == 2)
                        {
                            retorno = retorno.Where(x => x.Estado == "Vencido").ToList();
                        }

                    }
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;

                        string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", "").Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos).Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");
                        if (!string.IsNullOrEmpty(codAux))
                        {
                            url = url.Replace("{CODAUX}", codAux);
                        }
                        else
                        {
                            url = url.Replace("codaux={CODAUX}&", "");
                        }

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            documentos[0] = documentos[0].Where(x => x.Saldobase > 0).ToList();
                            if (estado == 2)
                            {
                                documentos[0] = documentos[0].Where(x => x.Estado == "V").ToList();
                            }
                            else if (estado == 3)
                            {
                                var configPago = _context.ConfiguracionPagoClientes.FirstOrDefault();
                                configPago.DiasPorVencer = configPago.DiasPorVencer == null ? 0 : configPago.DiasPorVencer;
                                documentos[0] = documentos[0].Where(x => (int)(Convert.ToDateTime(x.Movfv).Date - DateTime.Now.Date).TotalDays <= configPago.DiasPorVencer && x.Estado != "V").ToList();

                            }

                            var configPagos = _context.ConfiguracionPagoClientes.FirstOrDefault();

                            var documentosContablesDeuda = configPagos.TiposDocumentosDeuda;
                            var cuentasContablesDeuda = configPagos.CuentasContablesDeuda;
                            foreach (var doc in documentos[0])
                            {

                                bool esValidoDocumento = false;
                                bool esValidoCuenta = false;

                                var existDocumentoDeuda = documentosContablesDeuda.Split(';').Where(x => x == doc.Ttdcod).FirstOrDefault();
                                if (existDocumentoDeuda != null)
                                {
                                    esValidoDocumento = true;
                                }

                                var existCuentaDeuda = cuentasContablesDeuda.Split(';').Where(x => x == doc.Pctcod).FirstOrDefault();
                                if (existCuentaDeuda != null)
                                {
                                    esValidoCuenta = true;
                                }

                                if (!esValidoDocumento || !esValidoCuenta)
                                {
                                    continue;
                                }
                                ClienteSaldosDTO item = new ClienteSaldosDTO();
                                //item.comprobanteContable = reader["Comprobante"].ToString();
                                item.Documento = doc.DesDoc;
                                item.Nro = (double)doc.Numdoc;
                                item.FechaEmision = Convert.ToDateTime(doc.Movfe);
                                item.FechaVcto = Convert.ToDateTime(doc.Movfv);
                                item.Debe = (double)doc.MovMontoMa;
                                item.Haber = doc.Saldoadic;
                                item.Saldo = (double)doc.Saldoadic;
                                item.Detalle = ""; // reader["Detalle"].ToString();
                                item.Estado = doc.Estado;
                                item.Pago = ""; // reader["Pago"].ToString();
                                item.TipoDoc = doc.Ttdcod;
                                item.RazonSocial = "";
                                item.CodigoMoneda = doc.MonCod;
                                item.MontoOriginalBase = doc.MontoOriginalBase;
                                var mon = monedas.Where(x => x.CodMon == item.CodigoMoneda).FirstOrDefault();
                                if (mon != null) { item.DesMon = mon.DesMon; }
                                //item.CodigoMoneda = "03";
                                item.CodAux = doc.CodAux;
                                item.MontoBase = doc.MovMonto;
                                item.SaldoBase = doc.Saldobase;
                                item.EquivalenciaMoneda = doc.Equivalencia;
                                retorno.Add(item);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosDashboardAdminAsync"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosDashboardAdmin"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        public async Task<List<DocumentoContabilizadoAPIDTO>> GetAllDocumentosContabilizadosCliente(string codAux, string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    var monedas = await this.GetMonedasAsync(logApiId);
                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                    //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);

                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    int cantidad = 100;
                    int pagina = 1;
                    string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocumentosContabilizados";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);

                        if (documentos[0].Count > 0)
                        {

                            retorno = documentos[0];
                            pagina = pagina + 1;
                            while (retorno.Count < retorno[0].total)
                            {
                                using (var client2 = new HttpClient())
                                {

                                    string url2 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");


                                    client2.BaseAddress = new Uri(url2);
                                    client2.DefaultRequestHeaders.Accept.Clear();
                                    client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                    client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                    LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                    logApiDetalle2.IdLogApi = logApiId;
                                    logApiDetalle2.Inicio = DateTime.Now;
                                    logApiDetalle2.Metodo = "DocumentosContabilizados";


                                    HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                    logApiDetalle2.Termino = DateTime.Now;
                                    logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                    this.guardarDetalleLogApi(logApiDetalle2);

                                    if (response2.IsSuccessStatusCode)
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);

                                        retorno.AddRange(documentos2[0]);
                                        pagina = pagina + 1;
                                    }
                                    else
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = response2.StatusCode.ToString(),
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = content2,
                                            Ruta = "SoftlandService/GetAllDocumentosContabilizadosCliente"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }

                                }
                            }
                        }
                        retorno = documentos[0];

                        if (retorno.Count > 0)
                        {
                            var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                            foreach (var pago in pagosPendientes)
                            {
                                foreach (var item in retorno)
                                {
                                    var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Numdoc && x.TipoDocumento == item.Ttdcod).FirstOrDefault();
                                    if (detalle != null)
                                    {
                                        item.Saldobase = item.Saldobase - detalle.Apagar;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetAllDocumentosContabilizadosCliente"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {

                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetAllDocumentosContabilizadosCliente"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async Task<List<ClienteSaldosDTO>> GetDocumentosDeudores(string codAux, int estado, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    var configuracionPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                    string docs = string.Empty;

                    foreach (var item in configuracionPago.TiposDocumentosDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(docs))
                        {
                            docs = "'" + item + "'";
                        }
                        else
                        {
                            docs = docs + ",'" + item + "'";
                        }
                    }

                    string cuentasContables = string.Empty;
                    foreach (var item in configuracionPago.CuentasContablesDeuda.Split(';'))
                    {
                        if (string.IsNullOrEmpty(cuentasContables))
                        {
                            cuentasContables = "'" + item + "'";
                        }
                        else
                        {
                            cuentasContables = cuentasContables + ",'" + item + "'";
                        }
                    }

                    string sqlWhere = string.Empty;
                    if (!string.IsNullOrEmpty(codAux) && codAux != "null")
                    {
                        sqlWhere = "CodAux = " + codAux + "AND";
                    }

                    conSoftland.Open();
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "SELECT ISNULL((SELECT ctdoc.DesDoc FROM softland.cwttdoc as ctdoc WHERE ctdoc.CodDoc = CWDoctosPeriodo.Ttdcod),'Sin Documento') as Documento, TtdCod, " +
                                      " NumDOC as Folio,MovFe as Fecha, MovFv as [Fecha Vcto.], isnull(sum(MovDebe) + Sum(MovHaber), 0) as Monto, isnull(sum(Debe) - Sum(Haber), 0) as Saldo,  " +
                                      " CASE WHEN(MovFv <= GETDATE()) THEN 'Vencido' else 'Pendiente' end as Estado, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                                      " FROM softland.CWDoctosPeriodoFull AS CWDoctosPeriodo left join softland.cwtauxi on softland.cwtauxi.CodAux = CWDoctosPeriodo.CodAux " +
                                      " WHERE TtdCod in (" + docs + ") AND " + sqlWhere +
                                      " (PctCod IN(" + cuentasContables + ")) AND " +
                                      " (CpbAno >= " + configuracionPago.AnioTributario + ") " +
                                      " GROUP BY  TtdCod,NumDOC, MovFe, MovFv, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                                      " HAVING(sum(Debe) - Sum(Haber)) <> 0 ORDER BY  MovFe DESC ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ClienteSaldosDTO item = new ClienteSaldosDTO();
                        item.Documento = reader["Documento"].ToString();
                        item.RazonSocial = reader["NomAux"].ToString();
                        item.Nro = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Folio"]);
                        item.FechaEmision = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                        item.FechaVcto = (reader["Fecha Vcto."] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha Vcto."]);
                        item.Debe = (reader["Monto"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Monto"]);
                        item.Saldo = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                        item.Estado = reader["Estado"].ToString();
                        item.TipoDoc = reader["TtdCod"].ToString();
                        retorno.Add(item);
                    }
                    reader.Close();
                    conSoftland.Close();

                    if (retorno.Count > 0 && estado != 0)
                    {
                        if (estado == 1)
                        {
                            retorno = retorno.Where(x => x.Estado == "Pendiente").ToList();
                        }
                        else if (estado == 2)
                        {
                            retorno = retorno.Where(x => x.Estado == "Vencido").ToList();
                        }

                    }
                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var monedas = await this.GetMonedasAsync(logApiId);
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                        string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", "").Replace("{SOLOSALDO}", "1").Replace("{AREADATOS}", api.AreaDatos).Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");
                        if (!string.IsNullOrEmpty(codAux))
                        {
                            url = url.Replace("{CODAUX}", codAux);
                        }
                        else
                        {
                            url = url.Replace("codaux={CODAUX}&", "");
                        }

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            documentos[0] = documentos[0].Where(x => x.Saldobase > 0).ToList();
                            if (estado == 2)
                            {
                                documentos[0] = documentos[0].Where(x => x.Estado == "V").ToList();
                            }
                            else if (estado == 3)
                            {
                                var configPago = _context.ConfiguracionPagoClientes.FirstOrDefault();
                                configPago.DiasPorVencer = configPago.DiasPorVencer == null ? 0 : configPago.DiasPorVencer;
                                documentos[0] = documentos[0].Where(x => (int)(Convert.ToDateTime(x.Movfv).Date - DateTime.Now.Date).TotalDays <= configPago.DiasPorVencer && x.Estado != "V").ToList();

                            }

                            var configPagos = _context.ConfiguracionPagoClientes.FirstOrDefault();

                            var documentosContablesDeuda = configPagos.TiposDocumentosDeuda;
                            var cuentasContablesDeuda = configPagos.CuentasContablesDeuda;
                            foreach (var doc in documentos[0])
                            {

                                bool esValidoDocumento = false;
                                bool esValidoCuenta = false;

                                var existDocumentoDeuda = documentosContablesDeuda.Split(';').Where(x => x == doc.Ttdcod).FirstOrDefault();
                                if (existDocumentoDeuda != null)
                                {
                                    esValidoDocumento = true;
                                }

                                var existCuentaDeuda = cuentasContablesDeuda.Split(';').Where(x => x == doc.Pctcod).FirstOrDefault();
                                if (existCuentaDeuda != null)
                                {
                                    esValidoCuenta = true;
                                }

                                if (!esValidoDocumento || !esValidoCuenta)
                                {
                                    continue;
                                }
                                ClienteSaldosDTO item = new ClienteSaldosDTO();
                                //item.comprobanteContable = reader["Comprobante"].ToString();
                                item.Documento = doc.DesDoc;
                                item.Nro = (double)doc.Numdoc;
                                item.FechaEmision = Convert.ToDateTime(doc.Movfe);
                                item.FechaVcto = Convert.ToDateTime(doc.Movfv);
                                item.Debe = (double)doc.MovMontoMa;
                                item.Haber = doc.Saldoadic;
                                item.Saldo = (double)doc.Saldoadic;
                                item.Detalle = ""; // reader["Detalle"].ToString();
                                item.Estado = doc.Estado;
                                item.Pago = ""; // reader["Pago"].ToString();
                                item.TipoDoc = doc.Ttdcod;
                                item.RazonSocial = "";
                                item.CodigoMoneda = doc.MonCod;
                                item.MontoOriginalBase = doc.MontoOriginalBase;
                                var mon = monedas.Where(x => x.CodMon == item.CodigoMoneda).FirstOrDefault();
                                if (mon != null) { item.DesMon = mon.DesMon; }
                                //item.CodigoMoneda = "03";
                                item.CodAux = doc.CodAux;
                                item.MontoBase = doc.MovMonto;
                                item.SaldoBase = doc.Saldobase;
                                item.EquivalenciaMoneda = doc.Equivalencia;
                                retorno.Add(item);
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetDocumentosDeudores"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosDeudores"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        //FCA 19-08-2021 Obtiene vendedores de softland
        public async Task<List<ModulosAPIDTO>> GetModulosSoftlandAsync(string logApiId)
        {
            List<ModulosAPIDTO> item = new List<ModulosAPIDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string url = api.Url + api.ObtieneModulos.Replace("{AREADATOS}", api.AreaDatos);
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "ObtieneModulos";



                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<ModulosAPIDTO>> modulos = JsonConvert.DeserializeObject<List<List<ModulosAPIDTO>>>(content);
                        item = modulos[0];
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetModulosSoftlandAsync"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetVenedoresSoftland"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            return item;
        }

        public async Task<string> ReprocesaPago(int idPago, string logApiId)
        {

            string numComprobante = string.Empty;

            try
            {
                ContabilizaPagoVm contabiliza = new ContabilizaPagoVm();
                //Obtenemos configuración
                var config = _context.ConfiguracionPagoClientes.FirstOrDefault();

                //Obtenemos datos de pago
                var pago = _context.PagosCabeceras.Where(x => x.IdPago == idPago).FirstOrDefault();

                if (pago != null)
                {
                    if (string.IsNullOrEmpty(pago.ComprobanteContable))
                    {
                        //Obtenemos datos de pasarela pago
                        var pasarela = _context.PasarelaPagos.Find(pago.IdPasarela);

                        //Creamos clase de pago
                        contabiliza.cuentaContable = pasarela.CuentaContable;
                        contabiliza.tipoDocumento = pasarela.TipoDocumento;
                        contabiliza.fechaContabilizacion = DateTime.Now;
                        contabiliza.NumDoc = pago.IdPago.ToString();
                        contabiliza.areaNegocio = (string.IsNullOrEmpty(config.AreaNegocio)) ? string.Empty : config.AreaNegocio;
                        contabiliza.glosaEncabezado = (string.IsNullOrEmpty(config.GlosaComprobante)) ? "Comprobante Pago Portal" : config.GlosaComprobante;

                        //Agregamos documentos a pagar
                        contabiliza.DetalleDocumento = new List<DetalleDocumento>();
                        pago.PagosDetalles = _context.PagosDetalles.Where(x => x.IdPago == pago.IdPago).ToList();
                        foreach (var item in pago.PagosDetalles)
                        {
                            DetalleDocumento det = new DetalleDocumento
                            {
                                tipoDocumento = item.TipoDocumento,
                                folioDocumento = item.Folio.ToString(),
                                montoPagado = (double)item.Apagar,
                                glosaMovimiento = (string.IsNullOrEmpty(config.GlosaDetalle)) ? "Comprobante Pago Portal " + item.TipoDocumento + " " + item.Folio : config.GlosaComprobante + " " + item.TipoDocumento + " " + item.Folio
                            };

                            contabiliza.DetalleDocumento.Add(det);
                        }

                        string jsonString = JsonConvert.SerializeObject(contabiliza);

                        using (var client = new HttpClient())
                        {


                            var api = _context.ApiSoftlands.FirstOrDefault();
                            string accesToken = api.Token;

                            string url = api.Url + api.ContabilizaPagos;
                            client.BaseAddress = new Uri(url);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");
                            client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            var formData = new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("vJson", jsonString),
                                 };
                            HttpContent data = new FormUrlEncodedContent(formData);


                            LogApiDetalle logApiDetalle = new LogApiDetalle();
                            logApiDetalle.IdLogApi = logApiId;
                            logApiDetalle.Inicio = DateTime.Now;
                            logApiDetalle.Metodo = "ContabilizaPagos";


                            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, data).ConfigureAwait(false);

                            logApiDetalle.Termino = DateTime.Now;
                            logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                content = JsonConvert.DeserializeObject<string>(content);
                                CapturaComprobanteResponse comprobante = JsonConvert.DeserializeObject<CapturaComprobanteResponse>(content);

                                //JCA: Pendiente definir retorno del metodo


                                if (comprobante.comprobante != null)
                                {

                                    if (!string.IsNullOrEmpty(comprobante.comprobante[0].numero))
                                    {
                                        numComprobante = comprobante.comprobante[0].numero;
                                        pago.ComprobanteContable = comprobante.comprobante[0].numero;
                                        pago.IdPagoEstado = 2; //ESTADO PAGADO
                                        _context.PagosCabeceras.Attach(pago);
                                        _context.Entry(pago).Property(x => x.ComprobanteContable).IsModified = true;
                                        _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                        _context.SaveChanges();


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
                                        //string comprobanteHtml = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/MailTemplates/invoice.html"));
                                        comprobanteHtml = comprobanteHtml.Replace("{LOGO}", logo).Replace("{EMPRESA}", configEmpresa.NombreEmpresa).Replace("{RUT}", configEmpresa.RutEmpresa).Replace("{DIRECCION}", configEmpresa.Direccion)
                                            .Replace("{CORREO}", configEmpresa.CorreoContacto).Replace("{EMISION}", fecha).Replace("{HORA}", hora).Replace("{NUMCOMPROBANTE}", numComprobante).Replace("{MONTOTOTAL}", pago.MontoPago.Value.ToString("N0"));

                                        string[] partes = comprobanteHtml.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);
                                        string reemplazoDetalle = string.Empty;

                                        SoftlandService softlandService = new SoftlandService(_context, _webHostEnvironment);
                                        var tiposDocumentos = await softlandService.GetAllTipoDocSoftlandAsync(logApiId);
                                        foreach (var det in pago.PagosDetalles)
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
                                        MailService ms = new MailService(_context, _webHostEnvironment);
                                        await ms.EnviarCorreosAsync(vm);


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
                                        vm2.mensaje = numComprobante + "|" + pago.CodAux + "|" + pago.Correo + "|" + pago.MontoPago.Value.ToString("N0") + "|" + cliente.Nombre + "|" + cliente.Rut;
                                        vm2.adjuntos = listaAdjuntos2;
                                        vm2.email_destinatario = configCorreo.CorreoAvisoPago;
                                        await ms.EnviarCorreosAsync(vm2);


                                    }
                                    else
                                    {
                                        pago.IdPagoEstado = 4; //ESTADO PAGADO
                                        _context.PagosCabeceras.Attach(pago);
                                        _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                        _context.SaveChanges();

                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = "Comprobante generado con error o no generado",
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = comprobante.respuesta,
                                            Ruta = "SoftlandService/GeneraComprobantesContables"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }

                                }
                                else
                                {
                                    pago.IdPagoEstado = 4; //ESTADO PAGADO
                                    _context.PagosCabeceras.Attach(pago);
                                    _context.Entry(pago).Property(x => x.IdPagoEstado).IsModified = true;
                                    _context.SaveChanges();

                                    LogProceso log = new LogProceso
                                    {
                                        Excepcion = comprobante.error[0].Mensaje,
                                        Fecha = DateTime.Now.Date,
                                        Hora = DateTime.Now.ToString("HH:mm:ss"),
                                        Mensaje = comprobante.respuesta,
                                        Ruta = "SoftlandService/GeneraComprobantesContables"
                                    };
                                    _context.LogProcesos.Add(log);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content,
                                    Ruta = "SoftlandService/ReprocesaPago"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        numComprobante = pago.ComprobanteContable;
                    }

                }
                else
                {
                    numComprobante = string.Empty;
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/ReprocesaPago"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return numComprobante;
        }


        public async Task<DashboardDocumentosVm> GetMontosDashboardAdmin(string codAux, string logApiId)
        {
            DashboardDocumentosVm retorno = new DashboardDocumentosVm();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {

                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {

                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "0101";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                        string listadocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : string.Empty;
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : string.Empty;
                        string url = api.Url + api.DocumentosContabilizadosResumen.Replace("{DESDE}", "").Replace("{CODAUX}", codAux).Replace("{LISTADOCUMENTOS}", listadocumentos).Replace("{LISTACUENTAS}", listacuentas).Replace("{DIASXVENCER}", configPortal.DiasPorVencer.ToString());

                        //url = url.Replace("codaux={CODAUX}&", "");


                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizadosResumen";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DashboardDocumentosVm>> documentos = JsonConvert.DeserializeObject<List<List<DashboardDocumentosVm>>>(content);
                            retorno = documentos[0][0];
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetMontosDashboardAdmin"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetMontosDashboardAdmin"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return retorno;
        }


        public async Task<List<DeudorApiDTO>> GetTopDeudores(string logApiId)
        {
            List<DeudorApiDTO> retorno = new List<DeudorApiDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {


                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "0101";
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                        string listaCuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : string.Empty;
                        string listaTipoDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : string.Empty;
                        string url = api.Url + api.TopDeudores.Replace("{CANTIDADTOPE}", "10").Replace("{DESDE}", fecha).Replace("{LISTACUENTAS}", listaCuentas).Replace("{LISTATIPODOCUMENTOS}", listaTipoDocumentos);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "TopDeudores";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DeudorApiDTO>> deudores = JsonConvert.DeserializeObject<List<List<DeudorApiDTO>>>(content);
                            retorno = deudores[0];
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetTopDeudores"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetTopDeudores"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return retorno;
        }

        public async Task<List<DocumentoContabilizadoAPIDTO>> GetDocumentosDeudaVsPago(string logApiId)
        {
            List<DocumentoContabilizadoAPIDTO> retorno = new List<DocumentoContabilizadoAPIDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    //var configuracionPago = db.ConfiguracionPagoCliente.FirstOrDefault();

                    //string docs = string.Empty;

                    //foreach (var item in configuracionPago.TiposDocumentosDeuda.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(docs))
                    //    {
                    //        docs = "'" + item + "'";
                    //    }
                    //    else
                    //    {
                    //        docs = docs + ",'" + item + "'";
                    //    }
                    //}

                    //string cuentasContables = string.Empty;
                    //foreach (var item in configuracionPago.CuentasContablesDeuda.Split(';'))
                    //{
                    //    if (string.IsNullOrEmpty(cuentasContables))
                    //    {
                    //        cuentasContables = "'" + item + "'";
                    //    }
                    //    else
                    //    {
                    //        cuentasContables = cuentasContables + ",'" + item + "'";
                    //    }
                    //}

                    //string sqlWhere = string.Empty;

                    //conSoftland.Open();
                    //SqlCommand cmd = new SqlCommand();
                    //SqlDataReader reader;
                    //cmd.CommandText = "SELECT ISNULL((SELECT ctdoc.DesDoc FROM softland.cwttdoc as ctdoc WHERE ctdoc.CodDoc = CWDoctosPeriodo.Ttdcod),'Sin Documento') as Documento, TtdCod, " +
                    //                  " NumDOC as Folio,MovFe as Fecha, MovFv as [Fecha Vcto.], isnull(sum(MovDebe) + Sum(MovHaber), 0) as Monto, isnull(sum(Debe) - Sum(Haber), 0) as Saldo,  " +
                    //                  " CASE WHEN(MovFv <= GETDATE()) THEN 'Vencido' else 'Pendiente' end as Estado, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                    //                  " FROM softland.CWDoctosPeriodoFull AS CWDoctosPeriodo left join softland.cwtauxi on softland.cwtauxi.CodAux = CWDoctosPeriodo.CodAux " +
                    //                  " WHERE TtdCod in (" + docs + ") AND " + sqlWhere +
                    //                  " (PctCod IN(" + cuentasContables + ")) AND " +
                    //                  " (CpbAno >= " + configuracionPago.AnioTributario + ") " +
                    //                  " GROUP BY  TtdCod,NumDOC, MovFe, MovFv, softland.cwtauxi.RutAux, softland.cwtauxi.NomAux  " +
                    //                  " HAVING(sum(Debe) - Sum(Haber)) <> 0 ORDER BY  MovFe DESC ";
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Connection = conSoftland;

                    //reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //    ClienteSaldosDTO item = new ClienteSaldosDTO();
                    //    item.Documento = reader["Documento"].ToString();
                    //    item.RazonSocial = reader["NomAux"].ToString();
                    //    item.Nro = (reader["Folio"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Folio"]);
                    //    item.FechaEmision = (reader["Fecha"] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha"]);
                    //    item.FechaVcto = (reader["Fecha Vcto."] == DBNull.Value) ? DateTime.Now : Convert.ToDateTime(reader["Fecha Vcto."]);
                    //    item.Debe = (reader["Monto"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Monto"]);
                    //    item.Saldo = (reader["Saldo"] == DBNull.Value) ? 0 : Convert.ToDouble(reader["Saldo"]);
                    //    item.Estado = reader["Estado"].ToString();
                    //    item.TipoDoc = reader["TtdCod"].ToString();
                    //    retorno.Add(item);
                    //}
                    //reader.Close();
                    //conSoftland.Close();


                }
                else //FCA 16-06-2022
                {

                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    int anio = DateTime.Now.Date.Year - 2;

                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    //string url = api.Url + api.DocumentosContabilizados.Replace("{CODAUX}", codAux).Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);
                    // string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", fecha).Replace("solosaldo={SOLOSALDO}&", "").Replace("{AREADATOS}", api.AreaDatos);

                    //url = url.Replace("codaux={CODAUX}&", "");

                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    int cantidad = 100;
                    int pagina = 1;

                    using (var client = new HttpClient())
                    {
                        var fecha = configPortal.AnioTributario + "-01-01";
                        string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                       .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");


                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "DocumentosContabilizados";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);

                            if (documentos[0].Count > 0)
                            {
                                pagina = pagina + 1;

                                while (documentos[0].Count < documentos[0][0].total)
                                {
                                    using (var client2 = new HttpClient())
                                    {

                                        string url3 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                            .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                                        client2.BaseAddress = new Uri(url3);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "DocumentosContabilizados";


                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response2.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);

                                            documentos[0].AddRange(documentos2[0]);
                                            pagina = pagina + 1;
                                        }
                                        else
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content2,
                                                Ruta = "SoftlandService/GetDocumentosDeudaVsPago"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                            break;
                                        }

                                    }
                                }
                                documentos[0] = documentos[0].Where(x => x.Saldobase > 0).ToList();
                                retorno.AddRange(documentos[0]);
                            }
                            else
                            {
                                var content2 = await response.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content2,
                                    Ruta = "SoftlandService/GetDocumentosDeudaVsPago"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetDocumentosDeudaVsPago"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }
            finally { conSoftland.Close(); }

            return retorno;
        }


        public async System.Threading.Tasks.Task<List<ResumenDocumentosClienteApiDTO>> GetResumenDocumentosXClienteAsync(FilterVm filter, string logApiId)
        {

            List<ResumenDocumentosClienteApiDTO> retorno = new List<ResumenDocumentosClienteApiDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string diasPorVencer = "";
                    int cantidad = 10;
                    string estadoDocs = "";

                    if (filter.TipoBusqueda == 3)
                    {
                        diasPorVencer = configPortal.DiasPorVencer != null ? configPortal.DiasPorVencer.ToString() : "";
                        estadoDocs = "p";
                    }

                    if (filter.TipoBusqueda == 2)
                    {
                        estadoDocs = "V";
                    }

                    string emisionDesde = filter.fechaDesde != null ? filter.fechaDesde.Value.ToString("yyyyMMdd") : string.Empty;
                    string emisionHasta = filter.fechaHasta != null ? filter.fechaHasta.Value.ToString("yyyyMMdd") : string.Empty;
                    string codAux = filter.CodAux != null ? filter.CodAux : string.Empty;
                    string folio = filter.Folio != null && filter.Folio != 0 ? filter.Folio.ToString() : string.Empty;


                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    string url = api.Url + api.DocContabilizadosResumenxRut.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", fecha).Replace("{DIASXVENCER}", diasPorVencer).Replace("{EMISIONDESDE}", emisionDesde).Replace("{EMISIONHASTA}", emisionHasta).Replace("{ESTADO}", estadoDocs).Replace("{FOLIO}", folio).Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", filter.Pagina.ToString()).Replace("{RUTAUX}", codAux).Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocContabilizadosResumenxRut";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<ResumenDocumentosClienteApiDTO>> documentos = JsonConvert.DeserializeObject<List<List<ResumenDocumentosClienteApiDTO>>>(content);
                        if (documentos[0].Count > 0)
                        {

                            retorno = documentos[0].Where(x => x.codaux != null).ToList();
                            int cantidadDocumentos = retorno.Sum(x => x.CantidadDoctos);
                            decimal totalMonto = (decimal)retorno.Sum(x => x.saldototal);
                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetResumenDocumentosPendientesXClienteAsync"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }

                }
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetResumenDocumentosPendientesXClienteAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public async System.Threading.Tasks.Task<List<ClienteSaldosDTO>> GetDocumentosClienteAdministrador(FilterVm filter, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            try
            {

                using (var client = new HttpClient())
                {
                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    string accesToken = api.Token;
                    string diasPorVencer = "";
                    int cantidad = 10;
                    string estadoDocs = "";

                    if (filter.TipoBusqueda == 3)
                    {
                        diasPorVencer = configPortal.DiasPorVencer != null ? configPortal.DiasPorVencer.ToString() : "";
                        estadoDocs = "P";
                    }

                    if (filter.TipoBusqueda == 2)
                    {
                        estadoDocs = "V";
                    }

                    string emisionDesde = filter.fechaDesde != null ? filter.fechaDesde.Value.ToString("yyyyMMdd") : string.Empty;
                    string emisionHasta = filter.fechaHasta != null ? filter.fechaHasta.Value.ToString("yyyyMMdd") : string.Empty;
                    string codAux = filter.CodAux != null ? filter.CodAux : string.Empty;
                    string folio = filter.Folio != 0 ? filter.Folio.ToString() : string.Empty;


                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", diasPorVencer).Replace("{EMISIONDESDE}", emisionDesde).Replace("{EMISIONHASTA}", emisionHasta).Replace("{ESTADO}", estadoDocs).Replace("{FOLIO}", folio).Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", filter.Pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "1").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocumentosContabilizados";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                        if (documentos[0].Count > 0)
                        {
                            if (filter.TipoBusqueda == 2)
                            {
                                documentos[0] = documentos[0].Where(x => x.Movfv.Value.Date < DateTime.Now.Date).ToList();
                            }

                            retorno = documentos[0].ConvertAll(d =>
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
                                CodAux = d.CodAux,
                                MontoBase = d.MovMonto,
                                SaldoBase = d.Saldobase,
                                EquivalenciaMoneda = d.Equivalencia,
                                MovEqui = d.MovEquiv,
                                TotalFilas = d.total
                            }
                        );
                            if (retorno.Count > 0)
                            {
                                var pagosPendientes = _context.PagosCabeceras.Include(x => x.PagosDetalles).Where(j => j.IdPagoEstado == 4 && j.CodAux == retorno[0].CodAux).ToList();
                                foreach (var pago in pagosPendientes)
                                {
                                    foreach (var item in retorno)
                                    {
                                        var detalle = pago.PagosDetalles.Where(x => x.Folio == item.Nro && x.TipoDocumento == item.TipoDoc).FirstOrDefault();
                                        if (detalle != null)
                                        {
                                            item.SaldoBase = item.SaldoBase - detalle.Apagar;
                                        }
                                    }
                                }

                            }

                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/GetResumenDocumentosPendientesXClienteAsync"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }


            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetResumenDocumentosPendientesXClienteAsync"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }



        public async Task<List<ClienteDTO>> BuscarClienteSoftland2Async(string codAux, string rut, string nombre, string logApiId)
        {
            List<ClienteDTO> retorno = new List<ClienteDTO>();
            try
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Open();

                    string sqlWhere = string.Empty;
                    if (!string.IsNullOrEmpty(codAux))
                    {
                        sqlWhere = sqlWhere + " WHERE c.CodAux='" + codAux + "' ";
                    }

                    if (!string.IsNullOrEmpty(rut))
                    {
                        if (string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + " WHERE c.RutAux='" + rut + "' ";
                        }
                        else
                        {
                            sqlWhere = sqlWhere + " AND c.RutAux='" + rut + "' ";
                        }
                    }

                    if (!string.IsNullOrEmpty(nombre))
                    {
                        if (string.IsNullOrEmpty(sqlWhere))
                        {
                            sqlWhere = sqlWhere + " WHERE c.NomAux like '%" + nombre + "%' ";
                        }
                        else
                        {
                            sqlWhere = sqlWhere + " AND c.NomAux like '%" + nombre + "%' ";
                        }
                    }

                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;
                    cmd.CommandText = "select c.RutAux, c.CodAux, c.NomAux, c.EMail, c.DirAux, c.DirNum, d.VenCod, b.convta, b.CatCli, b.CodLista from softland.Cwtauxi c left join softland.cwtcvcl b on c.CodAux = b.CodAux left join softland.cwtauxven d on d.CodAux = c.CodAux " + sqlWhere;
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conSoftland;
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        ClienteDTO aux = new ClienteDTO();
                        aux.Rut = reader["RutAux"].ToString();
                        aux.CodAux = reader["CodAux"].ToString();
                        aux.Correo = reader["EMail"].ToString();
                        aux.Nombre = reader["NomAux"].ToString();
                        aux.CodVendedor = reader["VenCod"].ToString();
                        aux.CodCondVenta = reader["convta"].ToString();
                        aux.CodCatCliente = reader["CatCli"].ToString();
                        aux.CodLista = reader["CodLista"].ToString();
                        aux.DirAux = reader["DirAux"].ToString();
                        aux.DirNum = reader["DirNum"].ToString();
                        retorno.Add(aux);
                    }
                    reader.Close();
                    conSoftland.Close();
                }
                else
                {
                    int pagina = 1;
                    int cantClientes = 0;
                    using (var client = new HttpClient())
                    {
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        string accesToken = api.Token;
                        //string url = api.Url + api.ConsultaCliente.Replace("{AREADATOS}", api.AreaDatos).Replace("{PAGINA}", pagina.ToString()).Replace("{CANTIDAD}", "");

                        //if (!string.IsNullOrEmpty(codAux))
                        //{
                        //    url = url.Replace("{CODAUX}", codAux);
                        //}
                        //else
                        //{
                        //    url = url.Replace("codaux={CODAUX}&", "");
                        //}

                        //if (!string.IsNullOrEmpty(rut))
                        //{
                        //    url = url.Replace("{RUT}", rut);
                        //}
                        //else
                        //{
                        //    url = url.Replace("rut={RUT}&", "");
                        //}

                        //if (!string.IsNullOrEmpty(nombre))
                        //{
                        //    url = url.Replace("{NOMBRE}", nombre);
                        //}
                        //else
                        //{
                        //    url = url.Replace("nombre={NOMBRE}&", "");
                        //}

                        string url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", "").Replace("{CODAUX}", codAux).Replace("{CODLISTA}", "").Replace("{CODVEN}", "").Replace("{CONVTA}", "")
                            .Replace("{NOMBRE}", nombre).Replace("{RUT}", rut);

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "ConsultaCliente";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            List<List<ClienteAPIDTO>> clientesApi = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content);
                            cantClientes = clientesApi[0].Count();

                            clientesApi[0] = clientesApi[0].Where(x => x.CodAux != null).ToList();

                            var clientes = clientesApi[0].ConvertAll(d =>
                                new ClienteDTO
                                {
                                    Rut = d.RutAux,
                                    CodAux = d.CodAux,
                                    Correo = d.EMail,
                                    Nombre = d.NomAux,
                                    DirAux = d.DirAux,
                                    DirNum = d.DirNum,
                                    CodVendedor = d.CodVen,
                                    CodLista = d.CodLista,
                                    CodCondVenta = d.ConVta,
                                    CodCatCliente = d.catcli,
                                    CodCobrador = d.Codcob
                                }
                            );

                            retorno.AddRange(clientes);

                            while (cantClientes < int.Parse(clientesApi[0][0].Total))
                            {
                                using (var clientWhile = new HttpClient())
                                {
                                    pagina = pagina + 1;
                                    url = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", pagina.ToString()).Replace("{CATCLI}", "").Replace("{CODAUX}", codAux).Replace("{CODLISTA}", "").Replace("{CODVEN}", "").Replace("{CONVTA}", "")
                            .Replace("{NOMBRE}", nombre).Replace("{RUT}", rut);

                                    clientWhile.BaseAddress = new Uri(url);
                                    clientWhile.DefaultRequestHeaders.Accept.Clear();
                                    clientWhile.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                    clientWhile.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                    LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                    logApiDetalle2.IdLogApi = logApiId;
                                    logApiDetalle2.Inicio = DateTime.Now;
                                    logApiDetalle2.Metodo = "ConsultaCliente";


                                    HttpResponseMessage responseWhile = await clientWhile.GetAsync(clientWhile.BaseAddress);

                                    logApiDetalle2.Termino = DateTime.Now;
                                    logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                    this.guardarDetalleLogApi(logApiDetalle2);

                                    if (responseWhile.IsSuccessStatusCode)
                                    {
                                        var contentWhile = await responseWhile.Content.ReadAsStringAsync();
                                        List<List<ClienteAPIDTO>> clientesApiWhile = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(contentWhile);
                                        cantClientes = cantClientes + clientesApiWhile[0].Count();
                                        //var clientesWhile = clientesApiWhile[0];
                                        clientesApi[0] = clientesApi[0].Where(x => x.CodAux != null).ToList();
                                        var clientesWhile = clientesApiWhile[0].ConvertAll(d =>
                                                new ClienteDTO
                                                {
                                                    Rut = d.RutAux,
                                                    CodAux = d.CodAux,
                                                    Correo = d.EMail,
                                                    Nombre = d.NomAux,
                                                    DirAux = d.DirAux,
                                                    DirNum = d.DirNum,
                                                    CodVendedor = d.CodVen,
                                                    CodLista = d.CodLista,
                                                    CodCondVenta = d.ConVta,
                                                    CodCatCliente = d.catcli,
                                                    CodCobrador = d.Codcob
                                                });

                                        retorno.AddRange(clientesWhile);
                                    }
                                    else
                                    {
                                        var contentWhile = await responseWhile.Content.ReadAsStringAsync();
                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = responseWhile.StatusCode.ToString(),
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = contentWhile,
                                            Ruta = "SoftlandService/GetMonedas"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }
                                }
                            }

                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetMonedas"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/BuscarClienteSoftland2"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {
                    conSoftland.Close();
                }

            }
            return retorno;

        }


        public async Task<List<DocumentosCobranzaVm>> ObtenerDocumentosAutomaizacion(string tipoDocumento, string codAux, Nullable<int> numDoc, string logApiId)
        {
            List<DocumentosCobranzaVm> retorno = new List<DocumentosCobranzaVm>();
            try
            {

                using (var client = new HttpClient())
                {
                    var api = _context.ApiSoftlands.FirstOrDefault();
                    var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                    var monedas = await this.GetMonedasAsync(logApiId);
                    var cuentasContables = await this.GetAllCuentasContablesSoftlandAsync(logApiId);
                    //var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    var fechaActual = new DateTime();
                    string fecha = string.Empty;
                    //if (fechaDesde != null)
                    //{
                    //    fecha = fechaDesde?.Year.ToString() + "-0" + fechaDesde?.Month.ToString() + "-0" + fechaDesde?.Day.ToString();
                    //}
                    //else
                    //{
                    //    fecha = configPortal.AnioTributario.ToString() + "-01-01";
                    //}

                    string accesToken = api.Token;
                    // string url = api.Url + api.DocumentosContabilizados.Replace("{DESDE}", fecha).Replace("{SOLOSALDO}", "0").Replace("{AREADATOS}", api.AreaDatos).Replace("{CODAUX}", codAux);

                    string listaDocumentos = string.Empty;
                    string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                    if (!string.IsNullOrEmpty(tipoDocumento))
                    {
                        listaDocumentos = tipoDocumento.Replace(";", ",");
                    }
                    else
                    {
                        listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                    }

                    int cantidad = 100;
                    int pagina = 1;
                    string url = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codAux).Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "0").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    LogApiDetalle logApiDetalle = new LogApiDetalle();
                    logApiDetalle.IdLogApi = logApiId;
                    logApiDetalle.Inicio = DateTime.Now;
                    logApiDetalle.Metodo = "DocumentosContabilizados";


                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    logApiDetalle.Termino = DateTime.Now;
                    logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                    this.guardarDetalleLogApi(logApiDetalle);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);

                        if (documentos[0].Count > 0)
                        {
                            pagina = pagina + 1;

                            while (documentos[0].Count < documentos[0][0].total)
                            {
                                using (var client2 = new HttpClient())
                                {

                                    string url3 = api.Url + api.DocumentosContabilizados.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", "").Replace("{DESDE}", "").Replace("{DIASPORVENCER}", "").Replace("{EMISIONDESDE}", "").Replace("{EMISIONHASTA}", "").Replace("{ESTADO}", "").Replace("{FOLIO}", "").Replace("{LISTACUENTAS}", listacuentas)
                        .Replace("{LISTADOCUMENTOS}", listaDocumentos).Replace("{PAGINA}", pagina.ToString()).Replace("{RUTAUX}", "").Replace("{SOLOSALDO}", "0").Replace("{MONEDA}", "").Replace("{FECHAVENCIMIENTODESDE}", "").Replace("{FECHAVENCIMIENTOHASTA}", "").Replace("{LISTACAGETORIAS}", "")
                        .Replace("{LISTACONDICIONVENTA}", "").Replace("{LISTAVENDEDORES}", "");

                                    client2.BaseAddress = new Uri(url3);
                                    client2.DefaultRequestHeaders.Accept.Clear();
                                    client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                    client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                    LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                    logApiDetalle2.IdLogApi = logApiId;
                                    logApiDetalle2.Inicio = DateTime.Now;
                                    logApiDetalle2.Metodo = "DocumentosContabilizados";


                                    HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                    logApiDetalle2.Termino = DateTime.Now;
                                    logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                    this.guardarDetalleLogApi(logApiDetalle2);

                                    if (response2.IsSuccessStatusCode)
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);

                                        documentos[0].AddRange(documentos2[0]);
                                        pagina = pagina + 1;
                                    }
                                    else
                                    {
                                        var content2 = await response2.Content.ReadAsStringAsync();
                                        LogProceso log = new LogProceso
                                        {
                                            Excepcion = response2.StatusCode.ToString(),
                                            Fecha = DateTime.Now.Date,
                                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                                            Mensaje = content2,
                                            Ruta = "SoftlandService/ObtenerDocumentosAutomaizacion"
                                        };
                                        _context.LogProcesos.Add(log);
                                        _context.SaveChanges();
                                    }

                                }
                            }

                            //if (año != 0)
                            //{
                            //    documentos[0] = documentos[0].Where(x => x.Movfe.Value.Year == año).ToList();
                            //}

                            // documentos[0] = documentos[0].Where(x => x.Saldobase > 0).ToList();
                            //if (fechaHasta != null)
                            //{
                            //    documentos[0] = documentos[0].Where(x => x.Movfe <= fechaHasta).ToList();
                            //}

                            //if (!string.IsNullOrEmpty(tipoDocumento)){
                            //    documentos[0] = documentos[0].Where(x => tipoDocumento.Contains(x.Ttdcod)).ToList();
                            //}

                        }

                        string url2 = api.Url + api.ConsultaCliente.Replace("{CANTIDAD}", "").Replace("{PAGINA}", "").Replace("{CATCLI}", "").Replace("{CODAUX}", codAux).Replace("{CODLISTA}", "").Replace("{CODVEN}", "").Replace("{CONVTA}", "")
                            .Replace("{NOMBRE}", "").Replace("{RUT}", "");


                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri(url2);
                            client2.DefaultRequestHeaders.Accept.Clear();
                            client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                            //client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                            client2.DefaultRequestHeaders.Add("SApiKey", accesToken);
                            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                            LogApiDetalle logApiDetalle3 = new LogApiDetalle();
                            logApiDetalle3.IdLogApi = logApiId;
                            logApiDetalle3.Inicio = DateTime.Now;
                            logApiDetalle3.Metodo = "ConsultaTiposDeDocumentos";


                            HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress);

                            logApiDetalle3.Termino = DateTime.Now;
                            logApiDetalle3.Segundos = (int?)Math.Round((logApiDetalle3.Termino - logApiDetalle3.Inicio).Value.TotalSeconds);
                            this.guardarDetalleLogApi(logApiDetalle3);

                            if (response2.IsSuccessStatusCode)
                            {
                                var content2 = await response2.Content.ReadAsStringAsync();
                                List<List<ClienteAPIDTO>> clientes = JsonConvert.DeserializeObject<List<List<ClienteAPIDTO>>>(content2);
                                clientes[0] = clientes[0].Where(x => x.CodAux != null).ToList();

                                if (numDoc != 0)
                                {
                                    documentos[0] = documentos[0].Where(x => x.Numdoc == numDoc).ToList();
                                }
                                foreach (var item in documentos[0])
                                {
                                    DocumentosCobranzaVm aux = new DocumentosCobranzaVm();
                                    aux.FolioDocumento = (int)item.Numdoc;
                                    aux.TipoDocumento = item.DesDoc;
                                    aux.CodTipoDocumento = item.Ttdcod;
                                    aux.FechaEmision = Convert.ToDateTime(item.Movfe);
                                    aux.FechaVencimiento = Convert.ToDateTime(item.Movfv);
                                    aux.RutCliente = clientes[0][0].RutAux;
                                    aux.Bloqueado = clientes[0][0].Bloqueado;
                                    aux.NombreCliente = clientes[0][0].NomAux;
                                    aux.DiasAtraso = (int)(fechaActual.Date - item.Movfv.Value.Date).TotalDays;
                                    aux.Estado = item.Estado == "V" ? "VENCIDO" : item.Estado == "P" ? "PENDIENTE" : "";
                                    aux.CuentaContable = item.Pctcod;
                                    var cuenta = cuentasContables.Where(x => x.Codigo == item.Pctcod).FirstOrDefault();
                                    if (cuenta != null) { aux.NombreCuenta = cuenta.Nombre; }
                                    aux.MontoDocumento = (decimal)item.MovMonto;
                                    aux.SaldoDocumento = (decimal)item.Saldobase;
                                    retorno.Add(aux);
                                }
                            }
                            else
                            {
                                var content2 = await response2.Content.ReadAsStringAsync();
                                LogProceso log = new LogProceso
                                {
                                    Excepcion = response2.StatusCode.ToString(),
                                    Fecha = DateTime.Now.Date,
                                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                                    Mensaje = content2,
                                    Ruta = "SoftlandService/ObtenerDocumentosAutomaizacion"
                                };
                                _context.LogProcesos.Add(log);
                                _context.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        LogProceso log = new LogProceso
                        {
                            Excepcion = response.StatusCode.ToString(),
                            Fecha = DateTime.Now.Date,
                            Hora = DateTime.Now.ToString("HH:mm:ss"),
                            Mensaje = content,
                            Ruta = "SoftlandService/ObtenerDocumentosAutomaizacion"
                        };
                        _context.LogProcesos.Add(log);
                        _context.SaveChanges();
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                LogProceso log = new LogProceso();
                log.IdTipoProceso = -1;
                log.Fecha = DateTime.Now;
                log.Hora = ((DateTime.Now.Hour < 10) ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + ":" + ((DateTime.Now.Minute < 10) ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString());
                log.Ruta = @"Softland\ObtenerDocumentosAutomaizacion";
                log.Mensaje = ex.Message;
                log.Excepcion = ex.ToString();
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return retorno;
            }


        }


        public async Task<List<ClienteSaldosDTO>> GetPagosDocumento(FilterVm filter, string logApiId)
        {
            List<ClienteSaldosDTO> retorno = new List<ClienteSaldosDTO>();
            string tablaTemporal = string.Empty;
            try
            {
                if (utilizaApiSoftland == "false")
                {

                }
                else //FCA 16-06-2022
                {
                    using (var client = new HttpClient())
                    {

                        var monedas = await this.GetMonedasAsync(logApiId);
                        var api = _context.ApiSoftlands.FirstOrDefault();
                        var configPortal = _context.ConfiguracionPagoClientes.FirstOrDefault();
                        var fecha = configPortal.AnioTributario.ToString() + "-01-01";
                        string accesToken = api.Token;
                        //string url = api.Url + api.DocumentosContabilizados.Replace("{SOLOSALDO}", "1").Replace("{DESDE}", fecha).Replace("{AREADATOS}", api.AreaDatos);
                        //if (!string.IsNullOrEmpty(codaux))
                        //{
                        //    url = url.Replace("{CODAUX}", codaux);
                        //}
                        //else
                        //{
                        //    url = url.Replace("codaux={CODAUX}&", "");
                        //}
                        string listacuentas = !string.IsNullOrEmpty(configPortal.CuentasContablesDeuda) ? configPortal.CuentasContablesDeuda.Replace(";", ",") : "";
                        string listaDocumentos = !string.IsNullOrEmpty(configPortal.TiposDocumentosDeuda) ? configPortal.TiposDocumentosDeuda.Replace(";", ",") : "";
                        int cantidad = 100;
                        int pagina = 1;
                        string codaux = string.Empty;
                        if (!string.IsNullOrEmpty(filter.CodAux))
                        {
                            codaux = filter.CodAux;
                        }

                        string url = api.Url + api.PagosxDocumento.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codaux).Replace("{DOCUMENTO}", filter.TipoDoc).Replace("{FOLIO}", filter.Folio.ToString()).Replace("{PAGINA}", pagina.ToString());

                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                        client.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                        LogApiDetalle logApiDetalle = new LogApiDetalle();
                        logApiDetalle.IdLogApi = logApiId;
                        logApiDetalle.Inicio = DateTime.Now;
                        logApiDetalle.Metodo = "PagosxDocumento";


                        HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                        logApiDetalle.Termino = DateTime.Now;
                        logApiDetalle.Segundos = (int?)Math.Round((logApiDetalle.Termino - logApiDetalle.Inicio).Value.TotalSeconds);
                        this.guardarDetalleLogApi(logApiDetalle);

                        if (response.IsSuccessStatusCode)
                        {

                            var content = await response.Content.ReadAsStringAsync();
                            List<List<DocumentoContabilizadoAPIDTO>> documentos = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content);
                            documentos[0] = documentos[0].Where(x => x.Numdoc != 0).ToList();
                            if (documentos[0].Count > 0)
                            {
                                pagina = pagina + 1;

                                while (documentos[0].Count < documentos[0][0].total)
                                {
                                    using (var client2 = new HttpClient())
                                    {

                                        string url2 = api.Url + api.PagosxDocumento.Replace("{CANTIDAD}", cantidad.ToString()).Replace("{CODAUX}", codaux).Replace("{DOCUMENTO}", filter.TipoDoc).Replace("{FOLIO}", filter.Folio.ToString()).Replace("{PAGINA}", pagina.ToString());


                                        client2.BaseAddress = new Uri(url2);
                                        client2.DefaultRequestHeaders.Accept.Clear();
                                        client2.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                                        client2.DefaultRequestHeaders.Add("SApiKey", accesToken); //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                                        LogApiDetalle logApiDetalle2 = new LogApiDetalle();
                                        logApiDetalle2.IdLogApi = logApiId;
                                        logApiDetalle2.Inicio = DateTime.Now;
                                        logApiDetalle2.Metodo = "PagosxDocumento";


                                        HttpResponseMessage response2 = await client2.GetAsync(client2.BaseAddress).ConfigureAwait(false);

                                        logApiDetalle2.Termino = DateTime.Now;
                                        logApiDetalle2.Segundos = (int?)Math.Round((logApiDetalle2.Termino - logApiDetalle2.Inicio).Value.TotalSeconds);
                                        this.guardarDetalleLogApi(logApiDetalle2);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            List<List<DocumentoContabilizadoAPIDTO>> documentos2 = JsonConvert.DeserializeObject<List<List<DocumentoContabilizadoAPIDTO>>>(content2);
                                            documentos2[0] = documentos2[0].Where(x => x.Numdoc != 0).ToList();
                                            documentos[0].AddRange(documentos2[0]);
                                            pagina = pagina + 1;
                                        }
                                        else
                                        {
                                            var content2 = await response2.Content.ReadAsStringAsync();
                                            LogProceso log = new LogProceso
                                            {
                                                Excepcion = response2.StatusCode.ToString(),
                                                Fecha = DateTime.Now.Date,
                                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                                Mensaje = content2,
                                                Ruta = "SoftlandService/GetPagosDocumento"
                                            };
                                            _context.LogProcesos.Add(log);
                                            _context.SaveChanges();
                                        }

                                    }
                                }

                                retorno = documentos[0].ConvertAll(doc => new ClienteSaldosDTO
                                {

                                    //item.comprobanteContable = reader["Comprobante"].ToString();
                                    Documento = doc.DesDoc,
                                    Nro = (double)doc.Numdoc,
                                    FechaEmision = Convert.ToDateTime(doc.Movfe),
                                    FechaVcto = Convert.ToDateTime(doc.Movfv),
                                    Debe = (double)doc.MovMontoMa,
                                    Haber = doc.Saldoadic,
                                    Saldo = (double)doc.Saldoadic,
                                    Detalle = "", // reader["Detalle"].ToString();
                                    Estado = doc.Estado,
                                    Pago = "", // reader["Pago"].ToString();
                                    TipoDoc = doc.Ttdcod,
                                    RazonSocial = "",
                                    CodigoMoneda = doc.MonCod,
                                    CodAux = doc.CodAux,
                                    MontoBase = doc.MovMonto,
                                    SaldoBase = doc.Saldobase,
                                    EquivalenciaMoneda = doc.Equivalencia,
                                    APagar = doc.Saldobase,
                                    MontoOriginalBase = doc.MontoOriginalBase,
                                    MovEqui = doc.MovEquiv,
                                    DesMon = monedas.Where(x => x.CodMon == doc.MonCod).FirstOrDefault() != null ? monedas.Where(x => x.CodMon == doc.MonCod).FirstOrDefault().DesMon : ""
                                });
                            }
                        }
                        else
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            LogProceso log = new LogProceso
                            {
                                Excepcion = response.StatusCode.ToString(),
                                Fecha = DateTime.Now.Date,
                                Hora = DateTime.Now.ToString("HH:mm:ss"),
                                Mensaje = content,
                                Ruta = "SoftlandService/GetPagosDocumento"
                            };
                            _context.LogProcesos.Add(log);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (utilizaApiSoftland == "false") //FCA 16-06-2022
                {

                }
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "SoftlandService/GetPagosDocumento"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
            }

            return retorno;
        }

        public void guardarLogApi(LogApi logApi)
        {
            var configApi = _context.ApiSoftlands.FirstOrDefault();

            if (configApi.HabilitaLogApi == 1)
            {
                _context.Add(logApi);
                _context.SaveChanges();
            }

        }

        public void guardarDetalleLogApi(LogApiDetalle logApiDetalle)
        {
            var configApi = _context.ApiSoftlands.FirstOrDefault();

            if (configApi.HabilitaLogApi == 1)
            {
                _context.Add(logApiDetalle);
                _context.SaveChanges();
            }

        }


        //FCA IMPLEMENTACION
        public async Task<bool> validaConexionApiImplementacionAsync(ApiSoftlandVm api)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string accesToken = api.Token;
                    string url = api.Url + api.ConsultaTiposDeDocumentos;

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", api.Token);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;



                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> validaConexionBaseDatosAsync(DatosImplementacionVm datosImplementacion)
        {
            try
            {
                string connectionString = "Data Source=" + datosImplementacion.ServidorPortal + ";Initial Catalog=" + datosImplementacion.BaseDatosPortal + ";" +
                                      "user id=" + datosImplementacion.UsuarioBaseDatosPortal + ";password=" + datosImplementacion.ClaveBaseDatosPortal + ";Encrypt=False;";
                SqlConnection con = new SqlConnection(connectionString);

                con.Open();
                con.Close();

                return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<List<CuentasContablesSoftlandDTO>> GetAllCuentasContablesImplementacionAsync(ApiSoftlandVm api)
        {
            List<CuentasContablesSoftlandDTO> retorno = new List<CuentasContablesSoftlandDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    string accesToken = api.Token;
                    string url = api.Url + api.ConsultaPlanDeCuentas;

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", api.Token);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;



                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<CuentasContablesSoftlandAPIDTO>> listaCuentasContables = JsonConvert.DeserializeObject<List<List<CuentasContablesSoftlandAPIDTO>>>(content);
                        var cuentasContables = listaCuentasContables[0].Where(x => !string.IsNullOrEmpty(x.PCCODI)).ToList();

                        retorno = cuentasContables.ConvertAll(x => new CuentasContablesSoftlandDTO
                        {
                            Codigo = x.PCCODI,
                            Nombre = x.PCDESC
                        });
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new Exception("Error al obtener datos");

                    }
                }
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<List<TipoDocSoftlandDTO>> GetAllTiposDocumentosImplementacionAsync(ApiSoftlandVm api)
        {
            List<TipoDocSoftlandDTO> retorno = new List<TipoDocSoftlandDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    string accesToken = api.Token;
                    string url = api.Url + api.ConsultaTiposDeDocumentos;

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", api.Token);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;



                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<TipoDocSoftlandAPIDTO>> listaTipoDocs = JsonConvert.DeserializeObject<List<List<TipoDocSoftlandAPIDTO>>>(content);

                        var documentos = listaTipoDocs[0].Where(x => !string.IsNullOrEmpty(x.CodDoc)).ToList();

                        retorno = documentos.ConvertAll(x => new TipoDocSoftlandDTO
                        {
                            CodDoc = x.CodDoc,
                            DesDoc = x.DesDoc
                        });
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new Exception("Error al obtener datos");

                    }
                }
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<List<CuentasContablesSoftlandDTO>> GetAllCuentasContablesPasarela(ApiSoftlandVm api)
        {
            List<CuentasContablesSoftlandDTO> retorno = new List<CuentasContablesSoftlandDTO>();
            try
            {
                using (var client = new HttpClient())
                {
                    string accesToken = api.Token;
                    string url = api.Url + api.CuentasPasarelaPagos;

                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                    client.DefaultRequestHeaders.Add("SApiKey", api.Token);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;



                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        List<List<CuentasContablesSoftlandAPIDTO>> listaCuentasContables = JsonConvert.DeserializeObject<List<List<CuentasContablesSoftlandAPIDTO>>>(content);
                        var cuentasContables = listaCuentasContables[0].Where(x => !string.IsNullOrEmpty(x.PCCODI)).ToList();
                        retorno = cuentasContables.ConvertAll(x => new CuentasContablesSoftlandDTO
                        {
                            Codigo = x.PCCODI,
                            Nombre = x.PCDESC
                        });
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new Exception("Error al obtener datos");

                    }
                }
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }

        }
        public async Task<bool> validaConexionBaseDatosAsync(string conectionString)
        {
            try
            {
                SqlConnection con = new SqlConnection(conectionString + "Encrypt=False;");

                con.Open();
                con.Close();

                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public void CrearTablas(string conectionString)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(conectionString))
                {
                    connection.Open();

                    // Lee el script SQL desde el archivo
                    string script = string.Empty;
                    using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/Crea Tablas.sql")))
                    {
                        script = reader.ReadToEnd();
                    }


                    // Ejecuta el script utilizando SMO
                    Server server = new Server(new ServerConnection(connection));
                    server.ConnectionContext.ExecuteNonQuery(script);
                }
            }
            catch
            {
                throw;
            }

        }

        public bool TableExists(string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = @TableName";
                    cmd.Parameters.AddWithValue("@TableName", "Usuarios");

                    int tableCount = (int)cmd.ExecuteScalar();

                    return tableCount > 0;
                }
            }
        }


        public bool EnrolarVposVm(Tenant tenant)
        {
            try
            {

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
