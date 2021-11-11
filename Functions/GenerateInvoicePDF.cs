using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using InvoiceFormatSAT.Controllers;
using InvoiceFormatSAT.Models;

namespace InvoiceFormatSAT.Functions
{
    public static class GenerateInvoicePDF
    {
        [FunctionName("GenerateInvoicePDF")]
        public static async Task<FileContentResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "generatePDF")] HttpRequest req,
            ILogger log)
        {
            try
            {
                /*string body = await new StreamReader(req.Body).ReadToEndAsync();
                Invoice invoice =  JsonConvert.DeserializeObject<Invoice>(body);*/

                InvoiceController invoiceController = new InvoiceController();

                Invoice invoice = await invoiceController.getDTE(req);

                InvoicePDFController invoicePDF = new InvoicePDFController(invoice, log);

                return invoicePDF.getPDF();

            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
