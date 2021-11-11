
using InvoiceFormatSAT.AddIns;

namespace InvoiceFormatSAT.Models
{
    public class Issuer : Person
    {
        public Issuer( string Code, 
                       string NIT, 
                       string TradeName,
                       string Name,
                       string Email,
                       string VATAfiliation,
                       string Address,
                       string PostalCode,
                       string Municipality,
                       string Department,
                       string Country) {

            this.Name           = Function.convertToUTF8(Name);
            this.Email          = Email;
            this.Address        = Function.convertToUTF8(Address);
            this.PostalCode     = PostalCode;
            this.Municipality   = Function.convertToUTF8(Municipality);
            this.Department     = Function.convertToUTF8(Department);
            this.Country        = Country;
            this.VATAfiliation  = VATAfiliation;
            this.Code           = Code;
            this.NIT            = NIT;
            this.TradeName      = Function.convertToUTF8(TradeName); 
        }

        public string VATAfiliation { get; set; }
        public string Code { get; set; }
        public string NIT { get; set; }
        public string TradeName { get; set; }
    }
}
