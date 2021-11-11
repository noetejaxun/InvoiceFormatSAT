using InvoiceFormatSAT.AddIns;
using System.Collections.Generic;

namespace InvoiceFormatSAT.Models
{
    public class Item
    {
        public Item(string Type,
                    int Line,
                    double Amount,
                    string UnitOfMeasure,
                    string Description,
                    double UnitPrice,
                    double Price,
                    double Discount,
                    double Total,
                    List<Tax> Taxes)
        {
            this.Taxes = Taxes;
            this.Type = Type;
            this.Line = Line;
            this.Amount = Amount;
            this.UnitOfMeasure = UnitOfMeasure;
            this.Description = Function.convertToUTF8(Description);
            this.UnitPrice = UnitPrice;
            this.Price = Price;
            this.Discount = Discount;
            this.Total = Total;

        }
        public string Type { get; set; }
        public int Line { get; set; }
        public double Amount { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        public List<Tax> Taxes { get; set; }

    }
}
