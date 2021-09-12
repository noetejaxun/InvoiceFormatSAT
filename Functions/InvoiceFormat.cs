using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InvoiceFormatSAT.Controllers;
using InvoiceFormatSAT.Models;

namespace InvoiceFormatSAT.Functions
{
    public static class InvoiceFormat
    {
        [FunctionName("InvoiceFormat")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoiceFormat")] HttpRequest req,
            ILogger log)
        {
            UploadXML upload = new UploadXML(log);
            TaxFile taxFile = new TaxFile(upload);

            dynamic json = upload.getConvertedXML(await new StreamReader(req.Body).ReadToEndAsync());

            Invoice invoice = new Invoice();

            invoice.TaxDocument = taxFile.getTaxDocument(json);

            return new OkObjectResult(invoice);
        }
    }
}
