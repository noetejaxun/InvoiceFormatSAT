using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using InvoiceFormatSAT.Controllers;
using InvoiceFormatSAT.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using InvoiceFormatSAT.Models.satApi;
using System;

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

            string body = await new StreamReader(req.Body).ReadToEndAsync();
            //DataValidation dataValidation = JsonConvert.DeserializeObject<DataValidation>(body);
            XmlQuery xmlQuery = JsonConvert.DeserializeObject<XmlQuery>(body);

            string apiResponse = "";

            //using (var httpClient = new HttpClient())
            //{
            //    StringContent content = new StringContent(JsonConvert.SerializeObject(dataValidation), Encoding.UTF8, "application/json");

            //    using (var response = await httpClient.PostAsync("https://felav02.c.sat.gob.gt/verificador-rest/rest/publico/consultaDatosValidacion", content))
            //    {
            //        apiResponse = await response.Content.ReadAsStringAsync();
            //    }
            //}

            //JObject jsonConverted = JObject.Parse(apiResponse);
            //JArray details = (JArray)jsonConverted["detalle"];
            //apiResponse = "";

            //if ((bool)details[0]["existeDte"])
            //{
                
            //}
            using (var httpClient = new HttpClient())
            {
                xmlQuery.fechaHoraConsulta = DateTime.Now;
                StringContent content = new StringContent(JsonConvert.SerializeObject(xmlQuery), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://felav02.c.sat.gob.gt/verificador-rest/rest/publico/consultaXML", content))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            JObject jsonConvertedXML = JObject.Parse(apiResponse);
            JArray detailsXML = (JArray)jsonConvertedXML["detalle"];
            string xmlToConvert = (string)detailsXML[0];

            dynamic json = upload.getConvertedXML(xmlToConvert);

            Invoice invoice = new Invoice();

            invoice.TaxDocument = taxFile.getTaxDocument(json);

            return new OkObjectResult(invoice);
        }
    }
}
