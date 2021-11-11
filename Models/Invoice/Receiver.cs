using InvoiceFormatSAT.AddIns;

namespace InvoiceFormatSAT.Models
{
    public class Receiver : Person
    {
        public Receiver ( string Id,
                          string Name,
                          string Email,
                          string Address,
                          string PostalCode,
                          string Municipality,
                          string Department,
                          string Country )
        {
            this.Id             = Id;
            this.Name           = Function.convertToUTF8(Name);
            this.Email          = Email;
            this.Address        = Function.convertToUTF8(Address);
            this.PostalCode     = PostalCode;
            this.Municipality   = Function.convertToUTF8(Municipality);
            this.Department     = Function.convertToUTF8(Department);
            this.Country        = Country;
        }
        public string Id { get; set; }

    }
}
