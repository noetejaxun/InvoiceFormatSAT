
namespace InvoiceFormatSAT.Models
{
    class Tax
    {
        public Tax(string Name,
                   int Code,
                   double AmountWithoutVAT,
                   double VATAmount)
        {
            this.Name = Name;
            this.Code = Code;
            this.AmountWithoutVAT = AmountWithoutVAT;
            this.VATAmount = VATAmount;
        }
        public string Name { get; set; }
        public int Code { get; set; }
        public double AmountWithoutVAT { get; set; }
        public double VATAmount { get; set; }
    }
}
