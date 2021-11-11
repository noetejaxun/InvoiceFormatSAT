using InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate;
using System;

namespace InvoiceFormatSAT.Models.ExchangeRate
{
    public class ExchangeRate : IExchangeRate
    {
        public ExchangeRate(int currency,
                            DateTime date,
                            double saleValue,
                            double purchaseValue)
        {
            this.Currency = currency;
            this.Date = date;
            this.SaleValue = saleValue;
            this.PurchaseValue = purchaseValue;

        }
        public int Currency { get; set; }
        public DateTime Date { get; set; }
        public double SaleValue { get; set; }
        public double PurchaseValue { get; set; }
    }
}
