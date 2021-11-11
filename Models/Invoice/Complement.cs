using System;

namespace InvoiceFormatSAT.Models
{
    public class Complement
    {
        public Complement(  string complementID,
                            string name,
                            string uri,
                            DateTime documentIssueDate,
                            string adjustmentReason,
                            string sourceDocumentAuthorizationID) {
            this.complementID = complementID;
            this.name = name;
            this.uri = uri;
            this.documentIssueDate = documentIssueDate;
            this.adjustmentReason = adjustmentReason;
            this.sourceDocumentAuthorizationID = sourceDocumentAuthorizationID;
        }
        public string complementID { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public DateTime documentIssueDate { get; set; }
        public string adjustmentReason { get; set; }
        public string sourceDocumentAuthorizationID { get; set; }

    }
}
