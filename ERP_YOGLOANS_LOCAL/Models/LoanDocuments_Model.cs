using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class LoanDocuments_Model
    {
        public List<Dictionary<string, object>> Data { get; set; }
        public bool HasData { get; set; }
    }
}