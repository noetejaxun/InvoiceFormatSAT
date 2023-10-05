using System;
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
using InvoiceFormatSAT.Controllers.API.BanGuat;
using InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate;
using Newtonsoft.Json;

namespace InvoiceFormatSAT.Functions
{
    public static class ExchangeRate
    {
        [FunctionName("ExchangeRate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "exchangeRate")] HttpRequest req,
            ILogger log)
        {
            ExchangeRateInput query = JsonConvert.DeserializeObject<ExchangeRateInput>( await new StreamReader(req.Body).ReadToEndAsync() );

            DateTime fromDate;
            DateTime toDate;

            fromDate = query.date;
            toDate   = query.date;

            IExchangeRateController exchange = new ExchangeRateController();
            try
            {
                IExchangeRate exchangeRate = await exchange.getExchangeRate(fromDate, toDate);
                return new OkObjectResult(exchangeRate);
            }
            catch (System.Exception)
            {
                return new ConflictObjectResult(new Error(true, 1, "Ha ingresado una fecha incorrecta.", (dynamic)query));
            }
            

            // InvoiceController invoiceController = new InvoiceController();

            // Invoice invoice = await invoiceController.getDTE(req);

            // if (invoice.DTE.HasError)
            // {
            //     DataValidation query = new DataValidation();
            //     query.autorizacion = "XXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
            //     query.emisor = "NIT emisor";
            //     query.receptor = "NIT receptor";

            //     return new ConflictObjectResult(new Error(invoice.DTEExists, 1, "Verifique los datos ingresados en los par�metros de selecci�n.", (dynamic)query));
            // }

        }
    }
}
