using System;
using System.Collections.Generic;

namespace InvoiceFormatSAT.Models
{
    class TaxDocument
    {
        public string Currency { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Type { get; set; }
        public string Summary { get; set; }
        public double Cantidad { get; set; }
        public Issuer Issuer { get; set; }
        public Receiver Receiver { get; set; }
        public List<Phrases> Phrases { get; set; }
        public List<Item> Items { get; set; }
        public Total Total { get; set; }
        public Certification Certification { get; set; }
    }
}
