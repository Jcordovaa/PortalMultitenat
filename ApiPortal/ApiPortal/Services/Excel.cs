using ApiPortal.ViewModelsPortal;

namespace ApiPortal.Services
{
    public class Excel
    {
        public Stream ExcelDocumentosPendientes(List<DocumentosCobranzaVm> documentos)
        {
            //Obtiene todos los procesos excel ejecutandose antes de generar documento
            //List<int> procesosPrevios = obtieneProcesosEnEjecucion();

            Stream buffer = new MemoryStream();
            //string rutaTemporal = string.Empty;
            //try
            //{
            //    DataTable dtDetalle = new DataTable();
            //    dtDetalle.TableName = "tbDetalle";
            //    dtDetalle.Columns.Add("Rut");
            //    dtDetalle.Columns.Add("Nombre");
            //    dtDetalle.Columns.Add("Emision", typeof(DateTime));
            //    dtDetalle.Columns.Add("Vencimiento", typeof(DateTime));
            //    dtDetalle.Columns.Add("Tipo");
            //    dtDetalle.Columns.Add("Folio");
            //    dtDetalle.Columns.Add("Monto", typeof(int));
            //    dtDetalle.Columns.Add("Saldo", typeof(int));
            //    dtDetalle.Columns.Add("Estado");
            //    dtDetalle.Columns.Add("Dias_Vencimiento");


            //    foreach (var item in documentos)
            //    {
            //        dtDetalle.Rows.Add(item.RutCliente,
            //                            item.NombreCliente,
            //                            item.FechaEmision,
            //                            item.FechaVencimiento,
            //                            item.CodTipoDocumento,
            //                            item.FolioDocumento,
            //                            item.MontoDocumento,
            //                            item.SaldoDocumento,
            //                            item.Estado,
            //                            (item.DiasAtraso > 0) ? item.DiasAtraso : 0);
            //    }

            //    if (dtDetalle.Rows.Count > 0)
            //    {
            //        var excelApp = new ExcelApp.Application();
            //        excelApp.Workbooks.Add();

            //        ExcelApp._Worksheet workSheet = excelApp.ActiveSheet;
            //        workSheet.Name = "Documentos Pendientes";

            //        int celdaInicio = 1;
            //        // column headings
            //        for (var i = 0; i < dtDetalle.Columns.Count; i++)
            //        {
            //            workSheet.Cells[celdaInicio, i + 1] = dtDetalle.Columns[i].ColumnName;
            //        }

            //        for (var i = 0; i < dtDetalle.Rows.Count; i++)
            //        {
            //            // to do: format datetime values before printing
            //            for (var j = 0; j < dtDetalle.Columns.Count; j++)
            //            {
            //                workSheet.Cells[i + 2, j + 1] = dtDetalle.Rows[i][j];
            //            }
            //        }

            //        //Cabecera                    
            //        var excelCellrange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, dtDetalle.Columns.Count]];
            //        excelCellrange.EntireColumn.AutoFit();
            //        excelCellrange.Interior.Color = System.Drawing.ColorTranslator.FromHtml("#B4C6E7");
            //        Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
            //        border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            //        border.Weight = 2d;


            //        //var parametro = dbc.Parametros.Where(x => x.Nombre == "RutaArchivoTemporal").FirstOrDefault();
            //        string nombreDoc = this.GenerateRandomNombre() + ".xlsx";
            //        rutaTemporal = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Uploads/Temp/"), nombreDoc);
            //        workSheet.SaveAs(rutaTemporal);

            //        //Obtiene todos los procesos excel posterior a la generación del documento
            //        List<int> procesosActuales = obtieneProcesosEnEjecucion();
            //        this.killProcesses(procesosPrevios, procesosActuales);

            //        byte[] filesData = File.ReadAllBytes(rutaTemporal);

            //        buffer = new MemoryStream(filesData);

            //        //Here you delete the saved file
            //        if (File.Exists(rutaTemporal))
            //        {
            //            File.Delete(rutaTemporal);
            //        }


            //    }

            //}
            //catch
            //{
                ////Obtiene todos los procesos excel posterior a la generación del documento
                //List<int> procesosActuales = obtieneProcesosEnEjecucion();
                //this.killProcesses(procesosPrevios, procesosActuales);

                //if (!string.IsNullOrEmpty(rutaTemporal))
                //{
                //    if (File.Exists(rutaTemporal))
                //    {
                //        File.Delete(rutaTemporal);
                //    }

                //}

                //throw;
            //}

            return buffer;
        }
    }
}
