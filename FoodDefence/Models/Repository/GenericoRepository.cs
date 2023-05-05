using FoodDefence.Models.objectModel;
using FoodDefence.Models.Utils;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace FoodDefence.Models.Repository
{
    public class GenericoRepository
    {
        public List<ListadoTrampaClienteLocacionSector_Result> getListadoTrampaClienteLocacionSector(string cliente, string locacion, string sector, string numero, int? idTrampaTipo)
        {
            if (idTrampaTipo == 0)
                idTrampaTipo = null;
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.st_getListadoTrampaClienteLocacionSector(cliente, locacion, sector, numero, idTrampaTipo).ToList();
            }
        }

        #region QR
        public byte[] imprimirQRtrampas(List<TrampaQR> trampas)
        {
            PdfDocument document = new PdfDocument();

            // Sample uses DIN A4, page height is 29.7 cm. We use margins of 2.5 cm.
            LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(29.7 - 2.5));
            XUnit leftC1 = XUnit.FromCentimeter(2.5);
            XUnit leftC2 = XUnit.FromCentimeter(6.5);
            XUnit leftC3 = XUnit.FromCentimeter(10.5);
            XUnit leftC4 = XUnit.FromCentimeter(14.5);

            XUnit textLeftC1 = XUnit.FromCentimeter(2.5);
            XUnit textLeftC2 = XUnit.FromCentimeter(6.5);
            XUnit textLeftC3 = XUnit.FromCentimeter(10.5);
            XUnit textLeftC4 = XUnit.FromCentimeter(14.5);

            const int normalFontSize = 7;

            XFont fontNormal = new XFont("Verdana", normalFontSize, XFontStyle.Regular);


            int items = trampas.Count();

            XUnit top = XUnit.FromCentimeter(2);
            XUnit topText = XUnit.FromCentimeter(5.8);
            helper.CreatePage();
            double top2 = 2f;
            double topText2 = 5.8f;
            int columna = 1;
            int fila = 1;

            for (int item = 0; item < items; ++item)
            {
                string trampa = generarTextQR(trampas[item].idTrampaClienteLocacionSector.ToString());
                string numero = trampas[item].numero.ToString();
                XImage xImage = XImage.FromStream(generarQR(trampa.Trim()));

                if (fila > 6)
                {
                    //hoja nueva             
                    helper.CreatePage();
                    top2 = 2f;
                    topText2 = 5.8f;
                    columna = 1;
                    fila = 1;
                    top = XUnit.FromCentimeter(top2);
                    topText = XUnit.FromCentimeter(topText2);
                }

                XRect rect = new XRect();
                switch (columna)
                {
                    case 1:
                        helper.Gfx.DrawImage(xImage, leftC1, top, 122, 122);
                        rect = new XRect(leftC1, topText, 123, 30);
                        break;
                    case 2:
                        helper.Gfx.DrawImage(xImage, leftC2, top, 122, 122);
                        rect = new XRect(leftC2, topText, 123, 30);
                        break;
                    case 3:
                        helper.Gfx.DrawImage(xImage, leftC3, top, 122, 122);
                        rect = new XRect(leftC3, topText, 123, 30);
                        break;
                    case 4:
                        helper.Gfx.DrawImage(xImage, leftC4, top, 122, 122);
                        rect = new XRect(leftC4, topText, 123, 30);
                        break;
                }

                helper.Gfx.DrawString(numero.Trim() + "  " + trampa.Trim(), fontNormal, XBrushes.Black, rect, XStringFormats.TopCenter);
                columna = columna + 1;

                if (columna > 4)
                {
                    columna = 1;
                    top2 = top2 + 4.1f;
                    topText2 = topText2 + 4.1f;
                    top = XUnit.FromCentimeter(top2);
                    topText = XUnit.FromCentimeter(topText2);
                    fila = fila + 1;
                }


                //helper.Gfx.DrawString(line.ToString(), fontNormal, XBrushes.Black, leftLeft, topText, XStringFormats.TopLeft);                
                //XPen lineRed = new XPen(XColors.Red, 1);
                //helper.Gfx.DrawLine(lineRed, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(15), XUnit.FromCentimeter(2.5));



            }

            // Save the document... 
            //string filename = "C:\MultiplePages.pdf";
            //document.Save(filename);
            //// ...and start a viewer.
            //Process.Start(filename);

            byte[] fileContents = null;
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, true);
                fileContents = stream.ToArray();
            }
            return fileContents;
        }

        private Stream generarQR(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();


            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(7);
            //2.60 o //3.25
            MemoryStream memoryStream = new MemoryStream();

            qrCodeImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            return memoryStream;
        }

        private string generarTextQR(string text)
        {
            text = "0000000000" + text.Trim();
            return text.Substring(text.Length - 10, 10);
        }
        #endregion

        #region Graficos
        public byte[] generarChart(string pGraficos, string pPeriodo, string pPeriodoText, int pClienteLocacion, int pCliente)
        {
            // Create a MigraDoc document
            Document document = CreateDocument(pGraficos, pPeriodo, pPeriodoText, pClienteLocacion, pCliente);


            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;
            
            renderer.RenderDocument();
            
            byte[] fileContents = null;
            using (MemoryStream stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, true);
                fileContents = stream.ToArray();
            }
            return fileContents;
        }

        public List<DatosGraficos_Result> getDatosGraficos(string pGraficos, string pPeriodo, int pClienteLocacion)
        {
            string[] p = pPeriodo.Split(';');
            int m = int.Parse(p[0]);
            int y = int.Parse(p[1]);
            DateTime date = new DateTime(y, m, 1);
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    return db.st_getDatosGraficos(pGraficos, date, pClienteLocacion).ToList();
                }
                catch (Exception)
                {

                    List<DatosGraficos_Result> datosGraficos_Results = new List<DatosGraficos_Result>();
                    return datosGraficos_Results;
                }
            }
        }

        public List<GRAFICOS> getGraficosList(List<DatosGraficos_Result> lst) {


            var result = (from p in lst
                          group p by new { p.nro, p.titulo, p.otroFormatoEjeX } into gp
                          select new GRAFICOS
                          {
                              id = (int)gp.Key.nro,
                              descripcion = gp.Key.titulo,
                              otroFormatoEjeX = (bool)gp.Key.otroFormatoEjeX
                          }).ToList();

            return result;

        }

        public List<GRAFICOS> getGraficos()
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                return db.GRAFICOS.ToList();
            }
        }

        public Document CreateDocument(string pGraficos, string pPeriodo, string pPeriodoText, int pClienteLocacion, int pCliente)
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = "Grafico";

            List<DatosGraficos_Result> lstDatosGraficos =  getDatosGraficos(pGraficos, pPeriodo, pClienteLocacion);
            List<GRAFICOS> gs = getGraficosList(lstDatosGraficos);

            DefineStyles(document);
            DefineCover(document, gs);
            //TableOfContents.DefineTableOfContents(document);

            DefineContentSection(document);

            //Paragraphs.DefineParagraphs(document);

            
            foreach (var item in gs)
            {
                if (!item.otroFormatoEjeX)
                    DefineChartsOP1(document, item.descripcion, pPeriodoText, lstDatosGraficos, item.id);
                else
                    DefineChartsOP2(document, item.descripcion, pPeriodoText, lstDatosGraficos, item.id);
            }
                        
            return document;
        }

        public void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 15;
            style.Font.Bold = true;
            style.Font.Color = Colors.Black;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = false;
            style.Font.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(13, 110, 253);
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("12cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(13,110,253);
        }

        static void DefineContentSection(Document document)
        {
            Section section = document.AddSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;

            Paragraph paragraphHeader = new Paragraph();
            paragraphHeader.Format.Alignment = ParagraphAlignment.Right;
            MigraDoc.DocumentObjectModel.Shapes.Image image = paragraphHeader.AddImage(HostingEnvironment.MapPath("~/Content/Img/logo.png"));
            image.Width = "4cm";
            //paragraphHeader.AddImage(HostingEnvironment.MapPath("~/Content/Img/logo.png"));
            section.Headers.Primary.Add(paragraphHeader);
            section.Headers.EvenPage.Add(paragraphHeader.Clone());

            //HeaderFooter header = section.Headers.Primary;
            //header.Format.Alignment = ParagraphAlignment.Right;
            //MigraDoc.DocumentObjectModel.Shapes.Image image = header.AddImage(HostingEnvironment.MapPath("~/Content/Img/logo.png"));
            //image.Width = "4cm";
            //header.AddParagraph("");
            //header.Format.SpaceAfter = "1cm";

            

            // Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();

            // Add paragraph to footer for odd pages.
            section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());
        }

        public void DefineCover(Document document, List<GRAFICOS> gs)
        {
            Section section = document.AddSection();
            section.PageSetup.Orientation = Orientation.Landscape;
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.SpaceAfter = "0cm";
            MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage(HostingEnvironment.MapPath("~/Content/Img/logo.png"));            
            image.Width = "11cm";
            paragraph = section.AddParagraph("");            
            paragraph.Format.SpaceAfter = "1cm";

            foreach (var item in gs)
            {
                paragraph = section.AddParagraph();
                paragraph.Style = "TOC";
                Hyperlink hyperlink = paragraph.AddHyperlink(item.descripcion);
                hyperlink.AddText(item.descripcion + "\t");
                hyperlink.AddPageRefField(item.descripcion);
            }


            //paragraph = section.AddParagraph("A sample document that demonstrates the\ncapabilities of MigraDoc");
            //paragraph.Format.Font.Size = 16;
            //paragraph.Format.Font.Color = Colors.DarkRed;
            //paragraph.Format.SpaceBefore = "8cm";
            //paragraph.Format.SpaceAfter = "3cm";

            //paragraph = section.AddParagraph("Rendering date: ");
            //paragraph.AddDateField();
        }

                
        public void DefineChartsOP1(Document document, string Titulo, string Titulo2, List<DatosGraficos_Result> lstDatosGraficos, int nro)
        {
            Paragraph paragraph = document.LastSection.AddParagraph(Titulo, "Heading1");
            paragraph.AddBookmark(Titulo);
            //document.LastSection.AddParagraph( Titulo2, "Heading2");
            document.LastSection.LastParagraph.Format.SpaceAfter = "0.5cm";

            Chart chart = new Chart();
            chart.Left = 0;
            chart.Width = Unit.FromCentimeter(24);
            chart.Height = Unit.FromCentimeter(12);
            
            Series series = chart.SeriesCollection.AddSeries();
            series.ChartType = ChartType.Column2D;            
            series.HasDataLabel = true;
            series.FillFormat.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(13, 110, 253);
            
            XSeries xseries = chart.XValues.AddXSeries();

            var lstDatos = lstDatosGraficos.Where(w => w.nro == nro).OrderBy(o => o.dia).ToList();
            foreach (var item in lstDatos)
            {
                series.Add((double)item.cantidad);
               xseries.Add(item.leyendaX);
            }
            //series.Add(new double[] { 1, 17, 45});
            //series.HasDataLabel = true;

            //
            //xseries.Add("A");
            //xseries.Add("B");
            //xseries.Add("C");

            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "Periodos";
            chart.DataLabel.Position = DataLabelPosition.OutsideEnd;      
            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.HasMajorGridlines = true;
            chart.PlotArea.LineFormat.Color = Colors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;

            document.LastSection.Add(chart);
        }

        public void DefineChartsOP2(Document document, string Titulo, string Titulo2, List<DatosGraficos_Result> lstDatosGraficos, int nro)
        {
            Paragraph paragraph = document.LastSection.AddParagraph(Titulo, "Heading1");
            paragraph.AddBookmark(Titulo);

            //document.LastSection.AddParagraph( Titulo2, "Heading2");

            document.LastSection.LastParagraph.Format.SpaceAfter = "0.5cm";


            var lstDatos = lstDatosGraficos.Where(w => w.nro == nro).OrderBy(o => o.dia).ToList();

            Chart chart = new Chart();
            chart.Left = 0;
            chart.Width = Unit.FromCentimeter(24);
            chart.Height = Unit.FromCentimeter(12);
            chart.Format.Alignment = ParagraphAlignment.Center;
            chart.RightArea.AddLegend();


            XSeries xseries = chart.XValues.AddXSeries();

            int cant = 0;
            foreach (var item in lstDatos)
            {
                Series series = chart.SeriesCollection.AddSeries();
                series.ChartType = ChartType.Column2D;
                series.HasDataLabel = true;                
                // series.FillFormat.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(13, 110, 253);
                //series.FillFormat.Color = Colors.Red;


                cant++;
                series.Add((double)item.cantidad );

                series.Name = item.leyendaX;

            }

            xseries.Add(new string[] { "" });

            
            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "";
            chart.DataLabel.Position = DataLabelPosition.OutsideEnd;

            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.HasMajorGridlines = true;

            chart.PlotArea.LineFormat.Color = Colors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;

            document.LastSection.Add(chart);
        }


        #endregion

        public List<Periodo> getPeriodosOrdenTrabajo()
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                var result = (from p in db.ORDEN_TRABAJO
                              group p by new { p.fechaDeTrabajo.Year, p.fechaDeTrabajo.Month } into gp
                              select new
                              {
                                  gp.Key.Month,
                                  gp.Key.Year
                              }).ToList();

                Periodo oP = new Periodo();
                List<Periodo> listP = new List<Periodo>();
                foreach (var item in result)
                {
                    oP = new Periodo();
                    string fullMonthName = new DateTime(item.Year, item.Month, 1).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
                    oP.nombre = fullMonthName.ToUpper() + "  " + item.Year;
                    oP.key = item.Month + ";" + item.Year;
                    listP.Add(oP);
                }
                return listP;

            }
        }

        #region CSV
        public string exportarCSV(int pCliente, int pLocacion, DateTime pPeriodo)
        {
            using (FoodDefense_DevEntities db = new FoodDefense_DevEntities())
            {
                try
                {
                    var result = db.st_getDatosExportacion(pCliente, pLocacion, pPeriodo).ToList();

                    List<OrdenTrabajoCsv> fileCsv = (from otd in result
                                                     select new OrdenTrabajoCsv
                                                     {
                                                         accion = otd.accion,
                                                         mariposas = otd.mariposas == null ? 0 : (int)otd.mariposas,
                                                         minusculos = otd.minusculos == null ? 0 : (int)otd.minusculos,
                                                         moscas = otd.moscas == null ? 0 : (int)otd.moscas,
                                                         mosquitas = otd.mosquitas == null ? 0 : (int)otd.mosquitas,
                                                         polillas = otd.polillas == null ? 0 : (int)otd.polillas,
                                                         roedor = otd.roedor == null ? 0 : (int)otd.roedor,
                                                         insecto = otd.insecto == null ? 0 : (int)otd.insecto,
                                                         cucaAmericana = otd.cucaAmericana == null ? 0 : (int)otd.cucaAmericana,
                                                         cucaGermanica = otd.cucaGermanica == null ? 0 : (int)otd.cucaGermanica,
                                                         cantidad = otd.cantidad == null ? 0 : (int)otd.cantidad,
                                                         descripcion = otd.descripcion,
                                                         estado = otd.estado,
                                                         fechadetrabajo = otd.fechadetrabajo,
                                                         numero = otd.numero,
                                                         observaciones = otd.observaciones,
                                                         accionesPreventivas = otd.accionesPreventivas 
                                                     }).ToList();

                    string var = generar(fileCsv);

                    return var;
                }
                catch (Exception)
                {

                    return null;
                }

            }


        }
        public string generar(DataTable dt)
        {
            string csv = string.Empty;

            foreach (DataColumn column in dt.Columns)
            {
                //Cabecera CSV.
                csv += column.ColumnName + ';';
            }
            //nueva linea.
            csv += "\r\n";

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //Datos
                    csv += row[column.ColumnName].ToString().Replace(";", ",") + ';';
                }
                csv += "\r\n";
            }

            return csv;

        }
        public string generar<T>(List<T> items)
        {
            DataTable dt = ToDataTable(items);
            return generar(dt);
        }
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion



    }
}