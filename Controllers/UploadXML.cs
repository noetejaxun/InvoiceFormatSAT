using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace InvoiceFormatSAT.Controllers
{
    class UploadXML
    {
        private ILogger log;

        public UploadXML(ILogger log)
        {
            this.log = log;
        }

        public dynamic getDynamicObject(dynamic obj, string property)
        {
            dynamic blank = JsonConvert.DeserializeObject("{}");
            try
            {
                return obj[property];
            }
            catch (Exception)
            {
                return blank;
            }
            
        }

        public string getDynamicValue(dynamic obj, string property)
        {
            string blank = "";
            try
            {
                return (string)obj[property];
            }
            catch (Exception )
            {
                return blank;
            }
            
        }

        public object getConvertedXML(dynamic xml) {

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(document);

            log.LogTrace("Json deserialized.");
            return JsonConvert.DeserializeObject(json);

        }
    }
}
