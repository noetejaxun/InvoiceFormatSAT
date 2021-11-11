using InvoiceFormatSAT.Models.InterfacesSatApi;

namespace InvoiceFormatSAT.Models.satApi
{
    public class DataValidation : IQuery
    {
        public string autorizacion { get; set; }
        public string emisor { get; set; }
        public string receptor { get; set; }
        public double monto { get; set; }
    }
}
