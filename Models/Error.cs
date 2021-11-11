using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceFormatSAT.Models
{
    public class Error
    {
        public Error(bool DTEExists, int Id, string Message, dynamic body)
        {
            this.DTEExists = DTEExists;
            this.Id = Id;
            this.Message = Message;
            this.body = body;
        }
        public bool DTEExists { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public dynamic body { get; set; }
    }
}
