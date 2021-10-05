using InvoiceFormatSAT.Models.InterfacesSatApi;
using System;

namespace InvoiceFormatSAT.Models.satApi
{
    internal class XmlQuery : IQuery
    {
        public string autorizacion { get; set; }
        public string emisor { get; set; }
        public string receptor { get; set; }
        public double monto { get; set; }
        public DateTime fechaHoraConsulta { get; set; }
        public string estado { get; set; }
    }
}
