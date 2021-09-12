namespace InvoiceFormatSAT.Models
{
    class Receiver : Person
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
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
            this.Address = Address;
            this.PostalCode = PostalCode;
            this.Municipality = Municipality;
            this.Department = Department;
            this.Country = Country;
        }
        public string Id { get; set; }

    }
}
