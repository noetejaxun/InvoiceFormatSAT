using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate
{
    internal interface IExchangeRateController
    {
        public XDocument getXmlBody(DateTime fromDate, DateTime toDate);

        public Task<string> getExchangeRate(XDocument body);
        public Task<IExchangeRate> getExchangeRate(DateTime fromDate, DateTime toDate);
        public string getEmptyResult();
    }
}
