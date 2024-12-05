using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class UPILinkRecord
    {
        public int SLNo { get; set; }
        public string LoanNo { get; set; }
        public int Installment { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public decimal EMIAmount { get; set; }
        public DateTime DueDate { get; set; }
    }
}