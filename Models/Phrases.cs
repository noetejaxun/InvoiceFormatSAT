namespace InvoiceFormatSAT.Models
{
    class Phrases
    {
        public Phrases (string Code, string Type)
        {
            this.Code = Code;
            this.Type = Type;
        }
        public string Code { get; set; }
        public string Type { get; set; }
    }
}
