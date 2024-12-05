using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Disbursement_model
    {
        public string appId { get; set; }
        public string loanAmt { get; set; }
        public string disAmt { get; set; }
        public string bankId { get; set; }
        public string paymode { get; set; }
        public string paymenttype { get; set; }        
        public string custAccntNumb { get; set; }
        public string ifsc { get; set; }
        public string custName { get; set; }

        public string bankName { get; set; }
    }
}