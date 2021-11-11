using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
using InvoiceFormatSAT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace InvoiceFormatSAT.Controllers
{
    class InvoicePDFController
    {
        public InvoicePDFController(Invoice invoice, ILogger logs)
        {
            this.invoice = invoice;
            this.logs = logs;
        }
        private Color primaryColor = new Color(255, 87, 34);
        private Color secondaryColor = new Color(242, 242, 242);
        private Invoice invoice;
        private ILogger logs;
        private MemoryStream stream = new MemoryStream();

        private string formatAmount(double amount)
        {
            return amount.ToString("#,##0.00");
        }

        private TableBuilder getItems(TableBuilder tableBuilder, FontBuilder font)
        {

            tableBuilder.AddColumnPercentToTable("Línea", 5)
                        .AddColumnPercentToTable("Descripción", 35)
                        .AddColumnPercentToTable("Cantidad", 8)
                        .AddColumnPercentToTable("Precio Unitario", 13)
                        .AddColumnPercentToTable("Precio", 13)
                        .AddColumnPercentToTable("Descuento", 13)
                        .AddColumnPercentToTable("Total", 13)
                        .SetHeaderRowStyleBackColor(primaryColor)
                        .SetHeaderRowStyleFont(font)
                        .SetHeaderRowStyleMinHeight(new XUnit(15f))
                        .SetHeaderRowBorderColor(primaryColor);

            foreach (Item item in invoice.DTE.Items)
            {
                tableBuilder.AddRow(rowBuilder => {
                    rowBuilder.AddCell(item.Line.ToString())
                      .SetHorizontalAlignment(HorizontalAlignment.Center)
                      .SetPadding(2f);
                    rowBuilder.AddCell(item.Description)
                      .SetPadding(2f);
                    rowBuilder.AddCell(item.Amount.ToString())
                      .SetHorizontalAlignment(HorizontalAlignment.Center)
                      .SetPadding(2f);
                    rowBuilder.AddCell(formatAmount(item.UnitPrice))
                      .SetHorizontalAlignment(HorizontalAlignment.Right)
                      .SetPadding(2f);
                    rowBuilder.AddCell(formatAmount(item.Price))
                      .SetHorizontalAlignment(HorizontalAlignment.Right)
                      .SetPadding(2f);
                    rowBuilder.AddCell(formatAmount(item.Discount))
                      .SetHorizontalAlignment(HorizontalAlignment.Right)
                      .SetPadding(2f);
                    rowBuilder.AddCell(formatAmount(item.Total))
                      .SetHorizontalAlignment(HorizontalAlignment.Right)
                      .SetPadding(2f);

                    rowBuilder.SetBorderColor(Color.Gray, Color.White, Color.Gray, Color.White);

                    if (item.Line % 2 == 0)
                    {
                        rowBuilder.SetBackColor(secondaryColor);
                    }

                });
            }
            tableBuilder.AddFooterRowToTable(footerBuilder =>
            {
                footerBuilder.AddCellToRow("").SetBorderColor(Color.White, Color.Gray, Color.White, Color.White);
                footerBuilder.AddCellToRow("");
                footerBuilder.AddCellToRow("");
                footerBuilder.AddCellToRow("");
                footerBuilder.AddCellToRow("");
                footerBuilder.AddCellToRow("");
                footerBuilder.AddCellToRow("");
            });

            return tableBuilder;
        }

        public FileContentResult getPDF()
        {
            
            var box = new Box(50f, 40f);
            var font = FontBuilder.New();
            var headerFont = FontBuilder.New();

            font.SetColor(Color.Black);
            font.SetSize(6f);

            headerFont.SetColor(Color.White);
            headerFont.SetSize(6f);

            var myStrem = new MemoryStream();

            var documentBuilder = DocumentBuilder.New();
            var sectionBuilder = documentBuilder.AddSection();
            sectionBuilder.SetOrientation(PageOrientation.Portrait);
            sectionBuilder.SetMargins(box);
            sectionBuilder.SetStyleFont(font);


            sectionBuilder.AddTable(tableBuilder => { tableBuilder = getItems(tableBuilder, headerFont); }).ToSection();


            sectionBuilder.ToDocument();

            documentBuilder.Build(myStrem);
            return new FileContentResult(myStrem.ToArray(), "application/pdf") { FileDownloadName = "Hola.pdf" };
        }

    }


}
