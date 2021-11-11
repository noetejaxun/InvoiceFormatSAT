using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate
{
    public interface IExchangeRate
    {
        public int Currency { get; set; }
        public DateTime Date { get; set; }
        public double SaleValue { get; set; }
        public double PurchaseValue { get; set; }
    }
}
