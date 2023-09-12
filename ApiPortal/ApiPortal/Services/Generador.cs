using ApiPortal.Dal.Models_Portal;
using ApiPortal.ViewModelsPortal;
using iText.Barcodes;
using System.Data;
using System.Drawing;
using System.Text;

namespace ApiPortal.Services
{
    public class Generador
    {
        private readonly PortalClientesSoftlandContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Generador(PortalClientesSoftlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public Stream GenerarDocumentoElectronico(int folio, string codAux, out string tipoDoc)
        {
            Stream buffer = new MemoryStream();

            try
            {
                tipoDoc = string.Empty;
                SoftlandService sf = new SoftlandService(_context,_webHostEnvironment);
                DataTable dtCabecera = sf.obtenerCabeceraDataTable(folio, tipoDoc, codAux); //FCA 05-07-2022
                tipoDoc = dtCabecera.Rows[0]["Tipo"].ToString();
                DataTable dtFirma = new DataTable();
                dtFirma = sf.obtenerFirmaDTE(folio, tipoDoc, codAux);

                DataTable dtDetalle = new DataTable();
                dtDetalle = sf.obtenerDetalleDataTable(folio, tipoDoc, codAux); //FCA 05-07-2022

                DataTable dtReferencia = new DataTable();
                dtReferencia = sf.obtenerReferencia(folio, tipoDoc, codAux);

                var rutaBoleta = _context.Parametros.Where(x => x.Nombre == "RutaRPTBoleta").FirstOrDefault();
                var rutaFactura = _context.Parametros.Where(x => x.Nombre == "RutaRPTFactura").FirstOrDefault();


                string nombreDocumento = string.Empty;
                DataSet dsResultado = new DataSet();

                dtCabecera.TableName = "tbCabecera";
                dtCabecera.Columns.Add("PDF417", typeof(byte[]));

                DataTable dtPdf = new DataTable();
                dtPdf = dtFirma;
                string firmaDte = string.Empty;

                if (dtPdf.Rows.Count > 0)
                {
                    firmaDte = dtPdf.Rows[0][0].ToString();
                    firmaDte = firmaDte.Replace(Environment.NewLine, "");
                }


                if (!String.IsNullOrEmpty(firmaDte))
                {
                    Bitmap pdf417 = this.Crea_PDF417(firmaDte.Trim());
                    ImageConverter converter = new ImageConverter();
                    byte[] codPdf417 = (byte[])converter.ConvertTo(pdf417, typeof(byte[]));
                    dtCabecera.Rows[0]["PDF417"] = codPdf417;
                }

                string rutaReporte = string.Empty;
                if (tipoDoc == "F")
                {
                    rutaReporte = rutaFactura.Valor;
                }
                else if (tipoDoc == "B")
                {
                    rutaReporte = rutaBoleta.Valor;
                }

                dsResultado.Tables.Add(dtCabecera);

                dtDetalle.TableName = "tbDetalle";
                dsResultado.Tables.Add(dtDetalle);

                dtReferencia.TableName = "tbReferencia";
                dsResultado.Tables.Add(dtReferencia);

                //ReportDocument cryRpt;
                //cryRpt = new ReportDocument();

                //string reporte = rutaReporte;
                //cryRpt.Load(reporte);
                //cryRpt.SetDataSource(dsResultado);

                //buffer = (cryRpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));

                return buffer;
            }
            catch (Exception ex)
            {
                tipoDoc = string.Empty;
                return buffer;
            }
        }

        public Bitmap Crea_PDF417(String cod)
        {
            Encoding iso_8859_1 = Encoding.GetEncoding("ISO-8859-1");
            byte[] isoBytes = iso_8859_1.GetBytes(cod);

            int escala = 1;
            BarcodePDF417 barcode = new BarcodePDF417();
            barcode.SetOptions(BarcodePDF417.PDF417_FORCE_BINARY);
            barcode.SetCodeColumns(18);
            barcode.SetErrorLevel(5);
            barcode.SetCode(iso_8859_1.GetBytes(cod)); //LA PASO COMO BYTE YA QUE COMO STRING TENGO PROBLEMAS CON ACENTOS Y LA Ñ          


            //Bitmap bm = new Bitmap(barcode.CreateDrawingImage(Color.Black, Color.White));
            Bitmap bm = new Bitmap("");

            //nunca pasara por aca ya que la escala siempre sera 1
            if (escala != 1)
            {
                Image original = bm;
                int finalW = Convert.ToInt32(bm.Width * escala);
                int finalH = Convert.ToInt32(bm.Height * escala);

                Bitmap retBitmap = new Bitmap(finalW, finalH);
                //Bitmap retBitmap = new Bitmap(340, 113); sii pide 9cms ancho y 3 de alto                 
                Graphics retgr = Graphics.FromImage(retBitmap);
                retgr.ScaleTransform(escala, escala);
                retgr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                retgr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                retgr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                retgr.DrawImage(bm, new Point(0, 0));
                return retBitmap;
            }
            else
            {
                return bm;
            }

        }

        #region COBRANZAS AUTOMATICAS

        public byte[] generaDetalleCobranza(DetalleEnvioCobranzaVm cobranza, string nombreCobranza)
        {
            //Stream buffer = new MemoryStream();
            byte[] buffer;
            try
            {
                //JCA 28-10-2021: Modifica generación de documento
                //Obtenemos reporte en html 
                var reporte = System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads/Rpt/CobranzaGeorgeChaytor.html"));
                var configEmpresa = _context.ConfiguracionEmpresas.FirstOrDefault();
                string logo = configEmpresa.UrlPortal + "/" + configEmpresa.Logo;
                string logoSoftlandFooter = configEmpresa.UrlPortal + "/assets/images/Softlandpiemail.png";

                //Remplaza valores cabecera
                reporte = reporte.Replace("{FECHA}", DateTime.Now.ToShortDateString());
                reporte = reporte.Replace("{NOMBREDOCUMENTO}", nombreCobranza);
                reporte = reporte.Replace("{RUTALUMNO}", cobranza.RutCliente);
                reporte = reporte.Replace("{NOMBREALUMNO}", cobranza.NombreCliente);
                reporte = reporte.Replace("{RUTAPODERADO}", "");
                reporte = reporte.Replace("{NOMBREAPODERADO}", "");
                reporte = reporte.Replace("{LOGO}", logo);
                reporte = reporte.Replace("{CORREOEMPRESA}", configEmpresa.CorreoContacto);
                reporte = reporte.Replace("{WEBEMPRESA}", configEmpresa.Web);
                reporte = reporte.Replace("{TELEFONOEMPRESA}", configEmpresa.Telefono);
                reporte = reporte.Replace("{NOMBREEMPRESA}", configEmpresa.NombreEmpresa);
                reporte = reporte.Replace("{RUTEMPRESA}", configEmpresa.RutEmpresa);
                reporte = reporte.Replace("{MONTODEUDA}", "$" + String.Format("{0:#,##0}", cobranza.MontoDeuda));
                reporte = reporte.Replace("{IMAGENFOOTER}", logoSoftlandFooter);

                //Separamo del html la opcion donde va el detalle
                string[] cadenas = reporte.Split(new string[] { "<!--detalle-->" }, StringSplitOptions.None);

                //Completamos el detalle en el html
                string detalleDocs = string.Empty;
                foreach (var item in cobranza.ListaDocumentos)
                {
                    string strNumber = String.Format("{0:#,##0}", item.Monto);
                    detalleDocs = detalleDocs + cadenas[1].Replace("{numeroDoc}", item.Folio.ToString())
                                                          .Replace("{TipoDoc}", item.TipoDocumento)
                                                          .Replace("{FechaDoc}", item.FechaVencimiento.ToShortDateString())
                                                          .Replace("{montoDoc}", strNumber);
                }

                //concatemos el html completo
                cadenas[1] = detalleDocs;
                string htmlCompleto = string.Empty;
                foreach (var item in cadenas)
                {
                    htmlCompleto = htmlCompleto + item;
                }

                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;
                converter.Options.AllowContentHeightResize = true;
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlCompleto);
            

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    doc.Save(memoryStream);

                    buffer = memoryStream.ToArray();

                    memoryStream.Close();

                }

                doc.Close();



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return buffer;
        }
        #endregion
    }
}
