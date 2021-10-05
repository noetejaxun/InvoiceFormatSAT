namespace InvoiceFormatSAT.Models.InterfacesSatApi
{
    internal interface IQuery
    {
        public string autorizacion { get; set; }
        public string emisor { get; set; }
        public string receptor { get; set; }
        public double monto { get; set; }
    }
}
