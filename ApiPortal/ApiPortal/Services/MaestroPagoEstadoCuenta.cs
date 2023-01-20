using ApiPortal.Dal.Models_Portal;
using ApiPortal.ModelSoftland;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiPortal.Services
{
    public class MaestroPagoEstadoCuenta
    {
        private PortalClientesSoftlandContext _context = new PortalClientesSoftlandContext();
        SqlConnection conSoftland = new SqlConnection("");

        public string GetNumeroComprobanteContable(string año, string mes)
        {
            string comprobante = string.Empty;
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "SELECT  " +
                                  " CASE   " +
                                  " WHEN LEN(ISNULL((max(CpbNum) + 1), 1)) = 1  THEN CONCAT('0000000', cast(ISNULL((max(CpbNum) + 1), 1) as varchar)) " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 2  THEN CONCAT('000000', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))   " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 3  THEN CONCAT('00000', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))    " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 4  THEN CONCAT('0000', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))     " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 5  THEN CONCAT('000', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))      " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 6  THEN CONCAT('00', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))       " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 7  THEN CONCAT('0', cast(ISNULL((max(CpbNum) + 1), 1) as varchar))        " +
                                  " WHEN LEN(ISNULL((max(CpbNum) +1), 1)) = 8  THEN cast(ISNULL((max(CpbNum) +1), 1) as varchar) END as CpbNum        " +
                                  " FROM softland.cwcpbte WHERE CpbAno = '" + año + "'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();

                ClienteDTO item = new ClienteDTO();
                while (reader.Read())
                {
                    comprobante = reader["CpbNum"].ToString();

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
                    Ruta = "MaestroPagoEstadoCuenta/GetNumeroComprobanteContable"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }

            return comprobante;
        }

        public Boolean SaveComprobanteCabecera(string mes, string año, string numComprobante, string codAux)
        {
            try
            {
                var confPago = _context.ConfiguracionPagoClientes.FirstOrDefault();

                string glosa = confPago.GlosaComprobante + " " + codAux;
                if (glosa.Length > 60)
                {
                    glosa = glosa.Substring(0, 60);
                }

                conSoftland.Open();

                SqlCommand cmdC = new SqlCommand("INSERT INTO [softland].[cwcpbte]      " +
                                                "                        ([CpbAno]      " +
                                                "                        ,[CpbNum]      " +
                                                "                        ,[AreaCod]     " +
                                                "                        ,[CpbFec]      " +
                                                "                        ,[CpbMes]      " +
                                                "                        ,[CpbEst]      " +
                                                "                        ,[CpbTip]      " +
                                                "                        ,[CpbNui]      " +
                                                "                        ,[CpbGlo]      " +
                                                "                        ,[CpbImp]      " +
                                                "                        ,[CpbCon]      " +
                                                "                        ,[Sistema]     " +
                                                "                        ,[Proceso]     " +
                                                "                        ,[Usuario]     " +
                                                "                        ,[CpbNormaIFRS]" +
                                                "                        ,[CpbNormaTrib]" +
                                                "                        ,[CpbAnoRev]   " +
                                                "                        ,[CpbNumRev]   " +
                                                "                        ,[SistemaMod]  " +
                                                "                        ,[ProcesoMod])  " +
                                                "VALUES                             " +
                                                "(@Año " +
                                                ", @CpbNum " +
                                                ",'000' " +
                                                ",CAST(GETDATE() AS Date) " +
                                                ",@Mes " +
                                                ", 'V' " +
                                                ", 'T' " +
                                                ",(SELECT " +
                                                "CASE " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 1  THEN CONCAT('0000000', cast(ISNULL((max(CpbNui) + 1), 1) as varchar)) " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 2  THEN CONCAT('000000', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))  " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 3  THEN CONCAT('00000', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))   " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 4  THEN CONCAT('0000', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))    " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 5  THEN CONCAT('000', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))     " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 6  THEN CONCAT('00', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))      " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 7  THEN CONCAT('0', cast(ISNULL((max(CpbNui) + 1), 1) as varchar))       " +
                                                "WHEN LEN(ISNULL((max(CpbNui) + 1), 1)) = 8  THEN cast(ISNULL((max(CpbNui) + 1), 1) as varchar) END " +
                                                "FROM softland.cwcpbte WHERE CpbAno = @Año and CpbMes = @Mes and CpbTip = 'T') " +
                                                ",@Glosa " +
                                                ", 'N' " +
                                                ", 'S' " +
                                                ", 'CW' " +
                                                ", 'Comprobante' " +
                                                ", 'softland' " +
                                                ", 'S' " +
                                                ", 'S' " +
                                                ", '0000' " +
                                                ", '00000000' " +
                                                ", 'CW' " +
                                                ", 'Comprobante')");
                cmdC.CommandType = CommandType.Text;
                cmdC.Connection = conSoftland;
                cmdC.Parameters.AddWithValue("@Año", año);
                cmdC.Parameters.AddWithValue("@CpbNum", numComprobante);
                cmdC.Parameters.AddWithValue("@Mes", mes);
                cmdC.Parameters.AddWithValue("@Glosa", glosa);


                cmdC.ExecuteNonQuery();
                conSoftland.Close();

                return true;
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "MaestroPagoEstadoCuenta/SaveComprobanteCabecera"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
                return false;
            }
        }

        public Boolean SaveComprobanteContrapartida(string mes, string año, string numComprobante, PagosCabecera item, int correlativo, string codigoAutorizacionPasarela, string codAux)
        {
            try
            {
                //Obtenemos configuración de pagos
                var conf = _context.ConfiguracionPagoClientes.FirstOrDefault();

                //Obtenemos detalle de pasare
                var pasarela = _context.PasarelaPagos.Find(item.IdPasarela);

                //Obtenemos parametrización de cuenta contable
                var cuenta = this.GetConfiguracionCuentaContable(pasarela.CuentaContable);

                //Define glosa
                string glosa = conf.GlosaComprobante + " " + codAux;

                //Maximo 60 caracteres permitidos en Softland
                if (glosa.Length > 60)
                {
                    glosa = glosa.Substring(0, 60);
                }

                //Definimos y asignamos variables segun parametrización de cuenta contable

                string codAuxPago = "0000000000";
                if (cuenta.ManejaAuxiliar == 1)
                {
                    if (!string.IsNullOrEmpty(codAux)) { codAuxPago = codAux; }
                }

                string centroCosto = "00000000";
                if (cuenta.ManejaCentroCosto == 1)
                {
                    if (!string.IsNullOrEmpty(conf.CentroCosto)) { centroCosto = conf.CentroCosto; }
                }

                string tipoDocumento = "00";
                string nroDocumento = "0";
                string tipoDocumentoRef = "00";
                string nroDocumentoRef = "0";

                if (cuenta.ManejaDocumento == 1)
                {
                    if (!string.IsNullOrEmpty(pasarela.TipoDocumento)) { tipoDocumento = pasarela.TipoDocumento; }

                    if (!string.IsNullOrEmpty(codigoAutorizacionPasarela)) { nroDocumento = codigoAutorizacionPasarela; }

                    if (!string.IsNullOrEmpty(pasarela.TipoDocumento)) { tipoDocumentoRef = pasarela.TipoDocumento; }

                    if (!string.IsNullOrEmpty(codigoAutorizacionPasarela)) { nroDocumentoRef = codigoAutorizacionPasarela; }
                }

                string tipoDocConciliacion = "00";
                string numDocConciliacion = "0";
                if (cuenta.ManejaConciliacion == 1)
                {
                    if (!string.IsNullOrEmpty(pasarela.TipoDocumento)) { tipoDocConciliacion = pasarela.TipoDocumento; }

                    if (!string.IsNullOrEmpty(codigoAutorizacionPasarela)) { numDocConciliacion = codigoAutorizacionPasarela; }
                }

                string areaNegocio = "000";
                if (!string.IsNullOrEmpty(conf.AreaNegocio)) { areaNegocio = conf.AreaNegocio; }

                string fechaEmision = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                string fechaVencimiento = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());


                conSoftland.Open();

                SqlCommand cmdC = new SqlCommand("INSERT INTO [softland].[cwmovim]          " +
                                                 "                       ([CpbAno]          " +
                                                 "                       ,[CpbNum]          " +
                                                 "                       ,[MovNum]          " +
                                                 "                       ,[AreaCod]         " +
                                                 "                       ,[PctCod]          " +
                                                 "                       ,[CpbFec]          " +
                                                 "                       ,[CpbMes]          " +
                                                 "                       ,[VendCod]         " +
                                                 "                       ,[CodAux]          " +
                                                 "                       ,[TtdCod]          " +
                                                 "                       ,[NumDoc]          " +
                                                 "                       ,[MovFe]           " +
                                                 "                       ,[MovFv]           " +
                                                 "                       ,[MovTipDocRef]    " +
                                                 "                       ,[MovNumDocRef]    " +
                                                 "                       ,[MovDebe]         " +
                                                 "                       ,[MovHaber]        " +
                                                 "                       ,[MovGlosa]        " +
                                                 "                       ,[MonCod]          " +
                                                 "                       ,[MovEquiv]        " +
                                                 "                       ,[MovDebeMa]       " +
                                                 "                       ,[MovHaberMa]      " +
                                                 "                       ,[Cuota]           " +
                                                 "                       ,[CuotaRef]        " +
                                                 "                       ,[CvCod]           " +
                                                 "                       ,[UbicCod]         " +
                                                 "                       ,[CajCod]          " +
                                                 "                       ,[IfCod]           " +
                                                 "                       ,[MovIfCant]       " +
                                                 "                       ,[DgaCod]          " +
                                                 "                       ,[MovDgCant]       " +
                                                 "                       ,[CcCod]           " +
                                                 "                       ,[TipDocCb]        " +
                                                 "                       ,[NumDocCb]        " +
                                                 "                       ,[MovAEquiv]       " +
                                                 "                       ,[FecPag]          " +
                                                 "                       ,[CbaNumMov]       " +
                                                 "                       ,[CbaAnoC]         " +
                                                 "                       ,[GrabaDLib]       " +
                                                 "                       ,[MtoTotal]        " +
                                                 "                       ,[Marca]           " +
                                                 "                       ,[Impreso]         " +
                                                 "                       ,[CpbNormaIFRS]    " +
                                                 "                       ,[CpbNormaTrib])   " +
                                                 "VALUES                                    " +
                                                 "                       (@Año              " +
                                                 "                       , @CpbNum          " +
                                                 "                       , @Correlativo     " +
                                                 "                       , @AreaNegocio     " +
                                                 "                       , @CuentaContable  " +
                                                 "                       , CAST(GETDATE() AS Date)        " +
                                                 "                       , @Mes             " +
                                                 "                       , '000'            " +
                                                 "                       , '0000000000'     " +
                                                 "                       , @TipoDocPago             " +
                                                 "                       , @CodigoAutorizacionPasarela  " +
                                                 "                       , CONVERT(varchar,@FechaEmision,112)        " +
                                                 "                       , CONVERT(varchar,@FechaVencimiento,112)        " +
                                                 "                       , @TipoDocPago     " +
                                                 "                       , @CodigoAutorizacionPasarela  " +
                                                 "                       , @MontoDoc        " +
                                                 "                       , 0                " +
                                                 "                       , @Glosa           " +
                                                 "                       , @CodMoneda             " +
                                                 "                       , 1                " +
                                                 "                       , @MontoDoc        " +
                                                 "                       , 0                " +
                                                 "                       , 0                " +
                                                 "                       , 0                " +
                                                 "                       , '000'            " +
                                                 "                       , '000'            " +
                                                 "                       , '0000000000'     " +
                                                 "                       , '000'            " +
                                                 "                       , '0'              " +
                                                 "                       , '00000000'       " +
                                                 "                       , '0'              " +
                                                 "                       , @CentroCosto       " +
                                                 "                       , @TipoDocCB             " +
                                                 "                       , @NumDocCB              " +
                                                 "                       , 'S'              " +
                                                 "                       , CAST(GETDATE() AS Date)        " +
                                                 "                       , '0'              " +
                                                 "                       , '0'              " +
                                                 "                       , 'S'              " +
                                                 "                       , '0'              " +
                                                 "                       , 'N'              " +
                                                 "                       , 'N'              " +
                                                 "                       , 'S'              " +
                                                 "                       , 'S')");
                cmdC.CommandType = CommandType.Text;
                cmdC.Connection = conSoftland;
                cmdC.Parameters.AddWithValue("@Año", año);
                cmdC.Parameters.AddWithValue("@CpbNum", numComprobante);
                cmdC.Parameters.AddWithValue("@Correlativo", correlativo);
                cmdC.Parameters.AddWithValue("@AreaNegocio", areaNegocio);
                cmdC.Parameters.AddWithValue("@CuentaContable", pasarela.CuentaContable);
                cmdC.Parameters.AddWithValue("@Mes", mes);
                cmdC.Parameters.AddWithValue("@TipoDocPago", tipoDocumento);
                cmdC.Parameters.AddWithValue("@CodigoAutorizacionPasarela", nroDocumento);
                cmdC.Parameters.AddWithValue("@FechaEmision", fechaEmision);
                cmdC.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                cmdC.Parameters.AddWithValue("@MontoDoc", item.MontoPago);
                cmdC.Parameters.AddWithValue("@Glosa", glosa);
                cmdC.Parameters.AddWithValue("@CodMoneda", conf.MonedaUtilizada);
                cmdC.Parameters.AddWithValue("@CentroCosto", centroCosto);
                cmdC.Parameters.AddWithValue("@TipoDocCB", tipoDocConciliacion);
                cmdC.Parameters.AddWithValue("@NumDocCB", numDocConciliacion);




                cmdC.ExecuteNonQuery();
                conSoftland.Close();

                return true;
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "MaestroPagoEstadoCuenta/SaveComprobanteContrapartida"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
                return false;
            }
        }

        public Boolean SaveComprobanteDetalleDoc(string mes, string año, string numComprobante, PagosDetalle det, int correlativo, string codigoAutorizacionPasarela, string codAux, int idPasarela)
        {
            try
            {
                //Obtenemos configuración de pagos
                var conf = _context.ConfiguracionPagoClientes.FirstOrDefault();

                //Obtenemos detalle de pasare
                var pasarela = _context.PasarelaPagos.Find(idPasarela);

                //Obtenemos cuenta contable del documento
                string cuentaContable = det.CuentaContableDocumento;

                //Obtenemos parametrización de cuenta contable
                var cuenta = this.GetConfiguracionCuentaContable(cuentaContable);

                //Define glosa
                string glosa = conf.GlosaComprobante + " " + codAux + " " + det.TipoDocumento + " " + det.Folio;

                //Maximo 60 caracteres permitidos en Softland
                if (glosa.Length > 60)
                {
                    glosa = glosa.Substring(0, 60);
                }

                //Definimos y asignamos variables segun parametrización de cuenta contable

                string codAuxPago = "0000000000";
                if (cuenta.ManejaAuxiliar == 1)
                {
                    if (!string.IsNullOrEmpty(codAux)) { codAuxPago = codAux; }
                }

                string centroCosto = "00000000";
                if (cuenta.ManejaCentroCosto == 1)
                {
                    if (!string.IsNullOrEmpty(conf.CentroCosto)) { centroCosto = conf.CentroCosto; }
                }

                string tipoDocumento = "00";
                string nroDocumento = "0";
                string tipoDocumentoRef = "00";
                string nroDocumentoRef = "0";

                if (cuenta.ManejaDocumento == 1)
                {
                    if (!string.IsNullOrEmpty(pasarela.TipoDocumento)) { tipoDocumento = pasarela.TipoDocumento; }

                    if (!string.IsNullOrEmpty(codigoAutorizacionPasarela)) { nroDocumento = codigoAutorizacionPasarela; }

                    if (!string.IsNullOrEmpty(det.TipoDocumento)) { tipoDocumentoRef = det.TipoDocumento; }

                    if (!string.IsNullOrEmpty(det.Folio.ToString())) { nroDocumentoRef = det.Folio.ToString(); }
                }

                string tipoDocConciliacion = "00";
                string numDocConciliacion = "0";
                if (cuenta.ManejaConciliacion == 1)
                {
                    if (!string.IsNullOrEmpty(det.TipoDocumento)) { tipoDocConciliacion = det.TipoDocumento; }

                    if (!string.IsNullOrEmpty(det.Folio.ToString())) { numDocConciliacion = det.Folio.ToString(); }
                }

                string areaNegocio = "000";
                if (!string.IsNullOrEmpty(conf.AreaNegocio)) { areaNegocio = conf.AreaNegocio; }

                string fechaEmision = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                if (!string.IsNullOrEmpty(det.FechaEmision.ToString()))
                {
                    fechaEmision = Convert.ToDateTime(det.FechaEmision).Year.ToString() + ((Convert.ToDateTime(det.FechaEmision).Month < 10) ? "0" + Convert.ToDateTime(det.FechaEmision).Month.ToString() : Convert.ToDateTime(det.FechaEmision).Month.ToString()) + ((Convert.ToDateTime(det.FechaEmision).Day < 10) ? "0" + Convert.ToDateTime(det.FechaEmision).Day.ToString() : Convert.ToDateTime(det.FechaEmision).Day.ToString());
                }


                string fechaVencimiento = DateTime.Now.Year.ToString() + ((DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + ((DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                if (!string.IsNullOrEmpty(det.FechaVencimiento.ToString()))
                {
                    fechaVencimiento = Convert.ToDateTime(det.FechaVencimiento).Year.ToString() + ((Convert.ToDateTime(det.FechaVencimiento).Month < 10) ? "0" + Convert.ToDateTime(det.FechaVencimiento).Month.ToString() : Convert.ToDateTime(det.FechaVencimiento).Month.ToString()) + ((Convert.ToDateTime(det.FechaVencimiento).Day < 10) ? "0" + Convert.ToDateTime(det.FechaVencimiento).Day.ToString() : Convert.ToDateTime(det.FechaVencimiento).Day.ToString());
                }


                conSoftland.Open();

                SqlCommand cmdC = new SqlCommand("INSERT INTO [softland].[cwmovim]              " +
                                                 "                ([CpbAno]                     " + //1 año
                                                 "                ,[CpbNum]                     " + //2 NroComprobante
                                                 "                ,[MovNum]                     " + //3 Correlativo
                                                 "                ,[AreaCod]                    " + //4 Area de negocio (Solo si tuviera parametrizado)
                                                 "                ,[PctCod]                     " + //5 Cuenta contable
                                                 "                ,[CpbFec]                     " + //6 Fecha generacion comprobante
                                                 "                ,[CpbMes]                     " + //7 Mes comprobante
                                                 "                ,[VendCod]                    " + //8 No aplica (enviar 000)
                                                 "                ,[CodAux]                     " + //9 Codigo auxiliar, solo si cuenta contable lo solicita (Enviar por defecto 0000000000)
                                                 "                ,[TtdCod]                     " + //10 Tipo documento, solo si cuenta contable lo solicita
                                                 "                ,[NumDoc]                     " + //11 Nro documento
                                                 "                ,[MovFe]                      " + //12 Fecha emision
                                                 "                ,[MovFv]                      " + //13 Fecha vencimiento
                                                 "                ,[MovTipDocRef]               " + //14 Tipo documento pago, solo si cuenta contable lo solicita
                                                 "                ,[MovNumDocRef]               " + //15 Nro comprobante pago 
                                                 "                ,[MovDebe]                    " + //16 debe
                                                 "                ,[MovHaber]                   " + //17 haber
                                                 "                ,[MovGlosa]                   " + //18 Glosa movimiento
                                                 "                ,[MonCod]                     " + //19 Codigo moneda
                                                 "                ,[MovEquiv]                   " + //20 Equivalencia moneda
                                                 "                ,[MovDebeMa]                  " + //21 debe
                                                 "                ,[MovHaberMa]                 " + //22 haber
                                                 "                ,[Cuota]                      " + //23
                                                 "                ,[CuotaRef]                   " + //24
                                                 "                ,[CvCod]                      " + //25
                                                 "                ,[UbicCod]                    " + //26
                                                 "                ,[CajCod]                     " + //27
                                                 "                ,[IfCod]                      " + //28
                                                 "                ,[MovIfCant]                  " + //29
                                                 "                ,[DgaCod]                     " + //30
                                                 "                ,[MovDgCant]                  " + //31
                                                 "                ,[CcCod]                      " + //32 Centro de costo
                                                 "                ,[TipDocCb]                   " + //33 tipo documento conciliacion
                                                 "                ,[NumDocCb]                   " + //34 numero documento conciliacion
                                                 "                ,[MovAEquiv]                  " + //35
                                                 "                ,[FecPag]                     " + //36
                                                 "                ,[CbaNumMov]                  " + //37
                                                 "                ,[CbaAnoC]                    " + //38
                                                 "                ,[GrabaDLib]                  " + //39
                                                 "                ,[MtoTotal]                   " + //40
                                                 "                ,[Marca]                      " + //41
                                                 "                ,[Impreso]                    " + //42
                                                 "                ,[FormadePag]                 " + //43
                                                 "                ,[CpbNormaIFRS]               " + //44
                                                 "                ,[CpbNormaTrib])              " + //45
                                            "VALUES                                             " +
                                                 "               (@Año                          " +
                                                 "               ,@CpbNum                       " +
                                                 "               ,@Correlativo                  " +
                                                 "               ,@AreaNegocio                        " +
                                                 "               ,@CuentaContable " +
                                                 "               ,CAST(GETDATE() AS Date) " +
                                                 "               ,@Mes " +
                                                 "               ,'000' " +
                                                 "               ,@CodAuxiliar    " +
                                                 "               ,@TipoDocPasarela           " +
                                                 "               ,@CodigoAutorizacionPasarela                                                                                            " + //Numero transaccion transbank
                                                 "               , CONVERT(varchar,@FechaEmision,112)    " +
                                                 "               , CONVERT(varchar,@FechaVencimiento,112) " +
                                                 "               ,@TipoDoc    " +
                                                 "               ,@Folio     " +
                                                 "               ,0                                                                                                                      " +
                                                 "               ,@MontoDoc         " +
                                                 "               ,@Glosa            " +
                                                 "               ,@CodMoneda              " +
                                                 "               ,1                 " +
                                                 "               ,0                 " +
                                                 "               ,@MontoDoc         " +
                                                 "               ,0                 " +
                                                 "               ,0                 " +
                                                 "               ,'000'             " +
                                                 "               ,'000'             " +
                                                 "               ,'0000000000'      " +
                                                 "               ,'000'             " +
                                                 "               ,'0'               " +
                                                 "               ,'00000000'        " +
                                                 "               ,'0'               " +
                                                 "               ,@CentroCosto         " +
                                                 "               ,@TipoDocCB              " +
                                                 "               ,@NumDocCB               " +
                                                 "               ,'S'               " +
                                                 "               ,GETDATE()         " +
                                                 "               ,'0'               " +
                                                 "               ,'0'               " +
                                                 "               ,'S'               " +
                                                 "               ,'0'               " +
                                                 "               ,'N'               " +
                                                 "               ,'N'               " +
                                                 "               ,'5'               " +
                                                 "               ,'S'               " +
                                                 "               ,'S')");
                cmdC.CommandType = CommandType.Text;
                cmdC.Connection = conSoftland;
                cmdC.Parameters.AddWithValue("@Año", año);
                cmdC.Parameters.AddWithValue("@CpbNum", numComprobante);
                cmdC.Parameters.AddWithValue("@Correlativo", correlativo);
                cmdC.Parameters.AddWithValue("@AreaNegocio", areaNegocio);
                cmdC.Parameters.AddWithValue("@CuentaContable", cuentaContable);
                cmdC.Parameters.AddWithValue("@Mes", mes);
                cmdC.Parameters.AddWithValue("@CodAuxiliar", codAuxPago);
                cmdC.Parameters.AddWithValue("@TipoDocPasarela", tipoDocumento);
                cmdC.Parameters.AddWithValue("@CodigoAutorizacionPasarela", nroDocumento);
                cmdC.Parameters.AddWithValue("@FechaEmision", fechaEmision);
                cmdC.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                cmdC.Parameters.AddWithValue("@TipoDoc", tipoDocumentoRef);
                cmdC.Parameters.AddWithValue("@Folio", nroDocumentoRef);
                cmdC.Parameters.AddWithValue("@MontoDoc", det.Apagar);
                cmdC.Parameters.AddWithValue("@Glosa", glosa);
                cmdC.Parameters.AddWithValue("@CodMoneda", conf.MonedaUtilizada);
                cmdC.Parameters.AddWithValue("@CentroCosto", centroCosto);
                cmdC.Parameters.AddWithValue("@TipoDocCB", tipoDocConciliacion);
                cmdC.Parameters.AddWithValue("@NumDocCB", numDocConciliacion);


                cmdC.ExecuteNonQuery();
                conSoftland.Close();

                return true;
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "MaestroPagoEstadoCuenta/SaveComprobanteDetalleDoc"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
                return false;
            }
        }

        public string GetCuentaContableParametros(string tipoDoc)
        {
            string cuenta = string.Empty;
            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                cmd.CommandText = "SELECT CtaCliente AS CuentaContable FROM Softland.iwparam WHERE CodFaDTECW  = '" + tipoDoc + "' OR CodBoCaDTECW = '" + tipoDoc + "' " +
                                   " UNION " +
                                   " SELECT CtaCliMonExt AS CuentaContable FROM Softland.iwparam WHERE CodFaDTEExenCW = '" + tipoDoc + "' OR CodBoCaDTEExenCW = '" + tipoDoc + "'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                reader = cmd.ExecuteReader();

                ClienteDTO item = new ClienteDTO();
                while (reader.Read())
                {
                    cuenta = reader["CuentaContable"].ToString();

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
                    Ruta = "MaestroPagoEstadoCuenta/GetCuentaContableParametros"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                conSoftland.Close();
            }

            return cuenta;
        }

        public CuentaContableDTO GetConfiguracionCuentaContable(string codCuentaContable)
        {
            CuentaContableDTO retorno = new CuentaContableDTO();

            try
            {
                conSoftland.Open();

                SqlCommand cmd = new SqlCommand();
                SqlDataReader result;
                cmd.CommandText = "select PCCODI, PCDESC,PCCCOS,PCAUXI,PCCDOC,PCCONB from softland.CWPCtas where PCNIVEL = (SELECT ParNni FROM softland.cwparam) and PCCODI = '" + codCuentaContable + "'";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conSoftland;
                result = cmd.ExecuteReader();

                while (result.Read())
                {
                    retorno.CodCuentaContable = result["PCCODI"].ToString();
                    retorno.Descripcion = result["PCDESC"].ToString();
                    retorno.ManejaCentroCosto = (result["PCCCOS"].ToString() == "S") ? 1 : 0;
                    retorno.ManejaAuxiliar = (result["PCAUXI"].ToString() == "S") ? 1 : 0;
                    retorno.ManejaDocumento = (result["PCCDOC"].ToString() == "S") ? 1 : 0;
                    retorno.ManejaConciliacion = (result["PCCONB"].ToString() == "S") ? 1 : 0;
                }
                result.Close();
            }
            catch (Exception e)
            {
                LogProceso log = new LogProceso
                {
                    Excepcion = e.ToString(),
                    Fecha = DateTime.Now.Date,
                    Hora = DateTime.Now.ToString("HH:mm:ss"),
                    Mensaje = e.Message,
                    Ruta = "MaestroPagoEstadoCuenta/GetConfiguracionCuentaContable"
                };
                _context.LogProcesos.Add(log);
                _context.SaveChanges();
                return retorno;
            }
            finally { conSoftland.Close(); }

            return retorno;
        }
    }
}
