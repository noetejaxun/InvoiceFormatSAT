using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InvoiceFormatSAT.Models;
using InvoiceFormatSAT.Controllers;
using InvoiceFormatSAT.Models.satApi;

namespace InvoiceFormatSAT.Functions
{
    public static class InvoiceFormat
    {
        [FunctionName("InvoiceFormat")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "invoiceFormat")] HttpRequest req,
            ILogger log)
        {
            InvoiceController invoiceController = new InvoiceController();

            Invoice invoice = await invoiceController.getDTE(req);

            if (invoice.DTE.HasError)
            {
                DataValidation query = new DataValidation();
                query.autorizacion = "XXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
                query.emisor = "NIT emisor";
                query.receptor = "NIT receptor";

                return new ConflictObjectResult(new Error(invoice.DTEExists, 1, "Verifique los datos ingresados en los parámetros de selección.", (dynamic)query));
            }

            return new OkObjectResult(invoice);
        }
    }
}
