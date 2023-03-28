using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDefence.Models.Utils
{
    public class archivoQR
    {
        //public  void DefineTables(Document document)
        //{
        //    Paragraph paragraph = document.LastSection.AddParagraph();
        //    DemonstrateSimpleTable(document);
        //    DemonstrateAlignment(document);
        //    DemonstrateCellMerge(document);
        //}

        //public static void DemonstrateSimpleTable(Document document)
        //{

        //    Table table = new Table();
        //    table.Borders.Width = 0;

        //    Column column = table.AddColumn(Unit.FromCentimeter(5));
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    table.AddColumn(Unit.FromCentimeter(5));

        //    Row row = table.AddRow();
        //    row.VerticalAlignment = VerticalAlignment.Center;
        //    Cell cell = row.Cells[0];
        //    cell.AddParagraph();
        //    cell = row.Cells[1];
        //    cell.AddParagraph();

        //    row = table.AddRow();
        //    cell = row.Cells[0];
        //    cell.AddParagraph("1");
        //    cell = row.Cells[1];
        //    cell.AddParagraph(FillerText.ShortText);

        //    row = table.AddRow();
        //    cell = row.Cells[0];
        //    cell.AddParagraph("2");
        //    cell = row.Cells[1];
        //    cell.AddParagraph(FillerText.Text);

        //    table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

        //    document.LastSection.Add(table);
        //}

        //public  void DemonstrateAlignment(Document document)
        //{
        //    document.LastSection.AddParagraph("Cell Alignment", "Heading2");

        //    Table table = document.LastSection.AddTable();
        //    table.Borders.Visible = true;
        //    table.Format.Shading.Color = Colors.LavenderBlush;
        //    table.Shading.Color = Colors.Salmon;
        //    table.TopPadding = 5;
        //    table.BottomPadding = 5;

        //    Column column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Left;

        //    column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    table.Rows.Height = 35;

        //    Row row = table.AddRow();
        //    row.VerticalAlignment = VerticalAlignment.Top;
        //    row.Cells[0].AddParagraph("Text");
        //    row.Cells[1].AddParagraph("Text");
        //    row.Cells[2].AddParagraph("Text");

        //    row = table.AddRow();
        //    row.VerticalAlignment = VerticalAlignment.Center;
        //    row.Cells[0].AddParagraph("Text");
        //    row.Cells[1].AddParagraph("Text");
        //    row.Cells[2].AddParagraph("Text");

        //    row = table.AddRow();
        //    row.VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[0].AddParagraph("Text");
        //    row.Cells[1].AddParagraph("Text");
        //    row.Cells[2].AddParagraph("Text");
        //}

        //public  void DemonstrateCellMerge(Document document)
        //{
        //    document.LastSection.AddParagraph("Cell Merge", "Heading2");

        //    Table table = document.LastSection.AddTable();
        //    table.Borders.Visible = true;
        //    table.TopPadding = 5;
        //    table.BottomPadding = 5;

        //    Column column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Left;

        //    column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    column = table.AddColumn();
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    table.Rows.Height = 35;

        //    Row row = table.AddRow();
        //    row.Cells[0].AddParagraph("Merge Right");
        //    row.Cells[0].MergeRight = 1;

        //    row = table.AddRow();
        //    row.VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[0].MergeDown = 1;
        //    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[0].AddParagraph("Merge Down");

        //    table.AddRow();
        //}
    }
}