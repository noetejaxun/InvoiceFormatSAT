using System.Collections.Generic;

namespace InvoiceFormatSAT.Models
{
    class Total
    {
        public Total(double GrandTotal,
                     List<TaxTotal> TaxTotals)
        {
            this.GrandTotal = GrandTotal;
            this.TaxTotals = TaxTotals;
        }
        public double GrandTotal { get; set; }
        public List<TaxTotal> TaxTotals { get; set; }
    }
}
