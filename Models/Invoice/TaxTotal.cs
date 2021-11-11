namespace InvoiceFormatSAT.Models
{
    public class TaxTotal
    {
        public TaxTotal(string Name, double Amount)
        {
            this.Name = Name;
            this.Amount = Amount;
        }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
