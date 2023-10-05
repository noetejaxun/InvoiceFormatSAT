using InvoiceFormatSAT.AddIns;
using InvoiceFormatSAT.Models.ExchangeRate;
using InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace InvoiceFormatSAT.Controllers.API.BanGuat
{
    public class ExchangeRateController : ConsumeAPI, IExchangeRateController
    {
        public async Task<string> getExchangeRate(XDocument body)
        {
            
            string exchangeContent = await consumeApi(
                "http://www.banguat.gob.gt/variables/ws/TipoCambio.asmx",
                body.ToString(),
                "text/xml");

            return exchangeContent == "" ? getEmptyResult()  : exchangeContent;
        }

        public string getEmptyResult()
        {
            try
            {
                return XDocument.Parse(@"<?xml version='1.0' encoding='utf-8'?>
                                        <soap:Envelope
                                            xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'
                                            xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                                            xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                                            <soap:Body>
                                                <TipoCambioRangoResponse xmlns='http://www.banguat.gob.gt/variables/ws/'>
                                                    <TipoCambioRangoResult>
                                                        <Vars>
                                                            <Var>
                                                                <moneda>0</moneda>
                                                                <fecha>00/00/0000</fecha>
                                                                <venta>0</venta>
                                                                <compra>0</compra>
                                                            </Var>
                                                        </Vars>
                                                        <TotalItems>1</TotalItems>
                                                    </TipoCambioRangoResult>
                                                </TipoCambioRangoResponse>
                                            </soap:Body>
                                        </soap:Envelope>").ToString();
            }
            catch (Exception)
            {
                return new XDocument().ToString();
            }
        }

        

        public XDocument getXmlBody(DateTime fromDate, DateTime toDate)
        {
            string fromDateString = fromDate.ToString("dd-MM-yyyy");
            string toDateString = toDate.ToString("dd-MM-yyyy");

            try
            {
                return XDocument.Parse(@"<?xml version='1.0' encoding='utf-8' ?>
                                        <soap:Envelope
                                            xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                                            xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                                            xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                            <soap:Body>
                                                <TipoCambioRango
                                                    xmlns='http://www.banguat.gob.gt/variables/ws/'>" +
                                                    $"<fechainit>{fromDateString}</fechainit>" +
                                                    $"<fechafin>{toDateString}</fechafin>" +
                                                @"</TipoCambioRango>
                                            </soap:Body>
                                        </soap:Envelope>");
            }
            catch (Exception)
            {
                return new XDocument();
            }
        }

        public async Task<IExchangeRate> getExchangeRate(DateTime fromDate, DateTime toDate)
        {
            XDocument exchangeRateXML = XDocument.Parse( await getExchangeRate( getXmlBody( fromDate, toDate ) ) );

            XmlDocument document = new XmlDocument();
            document.LoadXml(exchangeRateXML.ToString());

            var xml = JsonConvert.SerializeXmlNode(document);

            dynamic json = JsonConvert.DeserializeObject(xml);

            dynamic Var = Function.getDynamicObjectByXMLPath(json, new string[] {
                "soap:Envelope",
                "soap:Body",
                "TipoCambioRangoResponse",
                "TipoCambioRangoResult",
                "Vars",
                "Var"
            }, 0);

            int moneda      = Function.getIntValue(Var, "moneda");
            string fecha    = Function.getStringValue(Var, "fecha");
            double venta   = Function.getDoubleValue(Var, "venta");
            double compra  = Function.getDoubleValue(Var, "compra");
            string[] splitedDate = fecha.Split('/');

            if (compra == 0 || venta == 0)
            {
                moneda = 1;
                compra = 1;
                venta = 1;
                return new ExchangeRate(moneda, DateTime.Now, venta, compra);
            } else
            {
                int day     = int.Parse(splitedDate[0]);
                int month   = int.Parse(splitedDate[1]);
                int year    = int.Parse(splitedDate[2]);

                return new ExchangeRate(moneda, new DateTime(year, month, day), venta, compra);
            }
        }
    }
}
