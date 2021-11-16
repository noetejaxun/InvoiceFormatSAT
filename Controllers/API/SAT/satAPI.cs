using InvoiceFormatSAT.Models.satApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;

namespace InvoiceFormatSAT.Controllers.API.SAT
{
    public class satAPI : ConsumeAPI
    {
        public async Task<string> getXmlFEL(ResponseValidation responseValidation)
        {
            try
            {
                XmlQuery xmlQuery = new XmlQuery();
                xmlQuery.autorizacion       = responseValidation.detalle[0].numeroUUID;
                xmlQuery.emisor             = responseValidation.detalle[0].nitEmisor;
                xmlQuery.receptor           = responseValidation.detalle[0].idReceptor;
                xmlQuery.monto              = responseValidation.detalle[0].granTotal;
                xmlQuery.estado             = responseValidation.detalle[0].estado;
                xmlQuery.fechaHoraConsulta  = responseValidation.detalle[0].fechaHoraConsulta;

                return await consumeApi(
                    "https://felav02.c.sat.gob.gt/verificador-rest/rest/publico/consultaXML",
                    JsonConvert.SerializeObject(xmlQuery),
                    "application/json");
            }
            catch (Exception)
            {
                return "{}";
            }
        }

        public async Task<ResponseValidation> getValidationFEL(XmlQuery xmlQuery)
        {
            string response = await consumeApi(
                "https://felav02.c.sat.gob.gt/verificador-rest/rest/publico/consultaDatosValidacion",
                JsonConvert.SerializeObject(xmlQuery),
                "application/json");
            var format = "dd/MM/yyyy HH:mm:ss";
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            ResponseValidation validationData = JsonConvert.DeserializeObject<ResponseValidation>(response, dateTimeConverter);

            return validationData;
        }
    }
}
