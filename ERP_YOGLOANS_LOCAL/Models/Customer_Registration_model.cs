using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Customer_Registration_model

    {
        public string customer_id { get; set; }

       
        public string Customer_name { get; set; }
        

       
        public string mob_no { get; set; }
       
        public int category_id { get; set; }

        public DateTime? dob { get; set; }


        public string email { get; set; }

       
        public string father_name { get; set; }

        
        public string mother_name { get; set; }

        public string spouse_name { get; set; }
        public int? yearofstay { get; set; }
        public string guardian { get; set; }
        public string guardian_name { get; set; }
        public string residence_type { get; set; }
        public string marital_status { get; set; }
        public int? dist_from_branch { get; set; }
        public string gender { get; set; }

        public string residence_no { get; set; }
      
        public string office_no { get; set; }

       
        public float? annualincome { get; set; }

        public int? education { get; set; }

        public int? occupation { get; set; }
        public string house_name { get; set; }

        public string landmark { get; set; }
        public string City { get; set; }

       
        public long? Pincode { get; set; }
       
        public string PostOffice { get; set; }
        public string Post_name { get; set; }
        public string District { get; set; }
        public string State { get; set; }

      
        public string p_house_name { get; set; }
        public string p_landmark { get; set; }
        public string p_City { get; set; }

        
        public long? p_Pincode { get; set; }
       
        public string p_PostOffice { get; set; }
        public string p_Post_name { get; set; }
        public string p_District { get; set; }
        public string p_State { get; set; }


       // public string image { get; set; }
        public HttpPostedFileBase image { get; set; }
        public string image_view { get; set; }

        public string ImageUrl1 { get; internal set; }
    }

    //public class CustomerModel
    //{
    //    public string GuardianValue { get; set; }
    //    public string GuardianText { get; set; }
    //}
}
