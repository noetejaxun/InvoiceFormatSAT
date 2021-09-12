
namespace InvoiceFormatSAT.Models
{
    class Issuer : Person
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

            this.Name = Name;
            this.Email = Email;
            this.Address = Address;
            this.PostalCode = PostalCode;
            this.Municipality = Municipality;
            this.Department = Department;
            this.Country = Country;
            this.VATAfiliation = VATAfiliation;
            this.Code = Code;
            this.NIT = NIT;
            this.TradeName = TradeName; 
        }

        public string VATAfiliation { get; set; }
        public string Code { get; set; }
        public string NIT { get; set; }
        public string TradeName { get; set; }
    }
}
