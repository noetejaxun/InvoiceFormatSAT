using System;
using System.Collections.Generic;

namespace InvoiceFormatSAT.Models
{
    public class InvoiceDocument
    {
        public string AuthorizationID { get; set; }
        public string Number { get; set; }
        public string Serie { get; set; }
        public string Type { get; set; }
        public string Currency { get; set; }
        public string OriginalCurrency { get; set; }
        public string State { get; set; }
        public bool Canceled { get; set; }
        public string CanceledDate { get; set; }
        public bool HasError { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Summary { get; set; }
        public double oilAmount { get; set; }
        public Issuer Issuer { get; set; }
        public Receiver Receiver { get; set; }
        public List<Phrases> Phrases { get; set; }
        public List<Item> Items { get; set; }
        public Total Total { get; set; }
        public Certification Certification { get; set; }
        public List<Complement> Complements { get; set; }
    }
}
