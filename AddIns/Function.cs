
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Web;
using System.Xml;

namespace InvoiceFormatSAT.AddIns
{
    public class Function
    {
        // Function to convert HTML caracter to UTF8
        public static string convertToUTF8(string value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        public static dynamic getDynamicObject(dynamic obj, string property)
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

        public static dynamic getDynamicObjectByXMLPath(dynamic obj, string[] properties, int index)
        {
            int nextIndex = index + 1;
            if (index == ( properties.Length -1 ))
            {
                return getDynamicObject(obj, properties[index]);
            }
            
            return getDynamicObjectByXMLPath( getDynamicObject( obj,  properties[index]),
                                              properties, nextIndex);
        }

        public static string getStringValue(dynamic obj, string property)
        {
            try {
                return (string)obj[property];
            } catch (Exception) {
                return "";
            }

        }

        public static int getIntValue(dynamic obj, string property)
        {
            try {
                return int.Parse(getStringValue(obj, property));
            } catch (Exception) {
                return 0;
            }
        }

        public static double getDoubleValue(dynamic obj, string property)
        {
            try {
                string value = getStringValue(obj, property);
                return double.Parse(string.IsNullOrEmpty(value) ? "0" : value, CultureInfo.InvariantCulture);
            } catch (Exception) {
                return 0;
            }
        }

        public static DateTime getDateTimeValue(dynamic obj, string property)
        {
            try {
                string value = getStringValue(obj, property);
                return DateTime.Parse(string.IsNullOrEmpty(value) ? DateTime.Now.ToString() : value);
            } catch (Exception) {
                return DateTime.Now;
            }
        }

        public static object getConvertedXML(dynamic xml)
        {

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(document);

            return JsonConvert.DeserializeObject(json);

        }
    }
}
