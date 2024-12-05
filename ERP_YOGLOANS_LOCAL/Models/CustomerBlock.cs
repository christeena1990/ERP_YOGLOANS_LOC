using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class CustomerBlock
    {
        public string SelectedCustomerId { get; set; }
        public List<LoanDetails> LoanDetailsList { get; set; }
    }

    public class LoanDetails
    {
        public int ModuleId { get; set; }
        public string Module { get; set; }
        public string AccountNo { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime MaturityDt { get; set; }
        public decimal Amount { get; set; }
    }

    public class CustomerDetails
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string Branch { get; set; }
    }
    public class RequestDetails
    {
        public string CustomerId { get; set; }
        public string NewCustomerId { get; set; }
        public string Type { get; set; }
        public string Comments { get; set; }        
        public HttpPostedFileBase Document { get; set; }
    }

}