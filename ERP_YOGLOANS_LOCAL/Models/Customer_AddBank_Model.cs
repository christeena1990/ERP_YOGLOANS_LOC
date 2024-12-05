using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Customer_AddBank_Model
    {
        public string ImageUrl1 { get; internal set; }
        public int customerId { get; set; }
        public string customer_name { get; set; }
        public string address { get; set; }
        public string phone_no { get; set; }
        public int branch_id { get; set; }
        public string remarks { get; set; }



        public string bank { get; set; }
        public string branch { get; set; }
        public int bank_id { get; set; }
        public HttpPostedFileBase image { get; set; }

        public string SearchInput { get; set; }


        [Required(ErrorMessage = "Payee name is required.")]
        public string PayeeName { get; set; }

        [Required(ErrorMessage = "Bank account number is required.")]
        public string AccountNumber { get; set; }

        public long EnteredBy { get; set; }
        public int ModifyStatus { get; set; }

        public string ifscInput { get; set; }



    }

    public class DocDetails
    {
        public string ImageUrl { get; set; }
        public string ImageType { get; set; }
    }

}