using System;

namespace InvoiceFormatSAT.Models
{
    public class Certification
    {
        public string CertifierNIT { get; set; }
        public string CertifierName { get; set; }
        public string Number { get; set; }
        public string Serie { get; set; }
        public string AuthorizationID { get; set; }
        public DateTime CertificationDate { get; set; }
    }
}
