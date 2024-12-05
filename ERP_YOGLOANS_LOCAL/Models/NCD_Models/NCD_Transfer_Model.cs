using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.NCD_Models
{
    public class NCD_Transfer_Model
    {

        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public long IssueNo { get; set; }  // Changed to long
        public string CertificateNo { get; set; }
        public string InterestType { get; set; }
        public decimal InterestRate { get; set; }
        public int Period { get; set; }
        public long Deb_id { get; set; }  // Changed to long
        public string Certificate_No { get; set; }
        public int BranchId { get; set; }
        public int Transtype { get; set; } 
        public byte[] Data { get; set; }

        public class DocDetails
        {
            public string ImageUrl { get; set; }
            public string ImageType { get; set; }
        }
        public HttpPostedFileBase image { get; set; }

       

    }
}