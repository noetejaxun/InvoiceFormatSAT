
namespace InvoiceFormatSAT.Models
{
    public class Tax
    {
        public Tax(string Name,
                   int Code,
                   double AmountWithoutVAT,
                   double Amount,
                   double GravableUnitAmount)
        {
            this.Name = Name;
            this.Code = Code;
            this.AmountWithoutVAT = AmountWithoutVAT;
            this.Amount = Amount;
            this.GravableUnitAmount = GravableUnitAmount;
        }
        public string Name { get; set; }
        public int Code { get; set; }
        public double AmountWithoutVAT { get; set; }
        public double GravableUnitAmount { get; set; }
        public double Amount { get; set; }
    }
}
