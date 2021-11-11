namespace InvoiceFormatSAT.Models
{
    public class Invoice
    {
        public Invoice(InvoiceDocument DTE)
        {
            this.DTE = DTE;
        }
        public bool DTEExists { get; set; }
        public InvoiceDocument DTE { get; set; }
    }
}
