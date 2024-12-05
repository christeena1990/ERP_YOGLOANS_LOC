using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Customer_Approval_Model
    {
        public int h_type { get; set; }


        public int rekyc_status { get; set; }

        public string rekyc_reason { get; set; }

        public List<string> Columns { get; set; }
        public List<string> Column { get; set; }

        public List<Dictionary<string, object>> Data { get; set; }

        public List<Dictionary<string, object>> Data1 { get; set; }

        public int? customer_id { get; set; }

        public int sl_no { get; set; }
        
        public string modi_status { get; set; }
        public int? appr_type { get; set; }
        public int customer_temp_id { get; set; }

        public string customer_name { get; set; }

        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }
        public string mob_no { get; set; }

        public string category { get; set; }
    

        public string dob { get; set; }


        public string email { get; set; }
        public string spouse { get; set; }


        public string father_name { get; set; }


        public string mother_name { get; set; }

        public string cust_create_date { get; set; }
        public string cust_status { get; set; }
        public string cust_branch { get; set; }
        public string employee_name { get; set; }





        public int yearofstay { get; set; }
        public string guardiantype { get; set; }


        public string guardian_name { get; set; }
        public string residence_type { get; set; }
        public string marital_status { get; set; }
        public string dist_from_branch { get; set; }
        public string gender { get; set; }

        public string residence_no { get; set; }
        public string office_no { get; set; }
        public string annual_income { get; set; }
        public string education { get; set; }
        public string occupation { get; set; }



        public string returnReason { get; set; }

        public string rejectReason { get; set; }
        public string sno_m { get; set; }
        public string outmsg { get; set; }



        //---------------------------------------
        public string commu_address { get; set; }
        public string land_mark { get; set; }
        public string c_city { get; set; }
        public string c_pin { get; set; }
        public string c_post { get; set; }
        public string c_district { get; set; }
        public string c_state { get; set; }



        //---------------------------------------
        public string p_address { get; set; }
        public string p_mark { get; set; }
        public string p_city { get; set; }
        public string p_pin { get; set; }
        public string p_post { get; set; }
        public string p_district { get; set; }
        public string p_state { get; set; }


        public bool IsPermanentSameAsCommunication { get; set; }







        public string mobile_no { get; set; }

        public string branch_id { get; set; }

        public string branch_name { get; set; }

        public string enter_by { get; set; }

        public string select { get; set; }

        public string employ_name { get; set; }

        public string Text { get; set; }
        public string ImageUrl1 { get; internal set; }


        //------------KYC Table--------//
        public string CustomerPhotos { get; set; }
        public string proof_type { get; set; }
        public string kyc { get; set; }
        public string id_no { get; set; }
        public string valid_from { get; set; }
        public string valid_to { get; set; }
        public int kyc_id { get; set; }
        public string data { get; set; }
        public int QueryId { get; set; }
        
        public string kycItem { get; set; }



        public List<string> KYCDocuments { get; set; }

        public List<AttachmentModel> Attachments { get; set; }



    }

    public class Customer_Approval_Result
    {
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public string OutMessage { get; set; }
    }

    public class Customer_Approval_ViewModel
    {
        public List<string> Columns { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        //public List<CustomerModificationModel> ModificationData { get; set; }
        public string modi_status { get; set; }
        public int customer_id { get; set; }
        public int cus_id { get; set; }
        public int? appr_type { get; set; }
        public string customer_name { get; set; }
        public string mobile_no { get; set; }
        public int sl_no { get; set; }
        public string branch_id { get; set; }
        
       public string OutMessage { get; set; }

        public string branch_name { get; set; }
        public string employ_name { get; set; }
        public string city { get; set; }

        public string address { get; set; }
        public string modify_by { get; set; }
       
        public string modify_ststus { get; set; }

        public bool ShowApprovalList { get; set; }
        public bool ShowModifiedList { get; set; }

    }
    public class CustomerPhotoModel
    {
        public byte[] Photo { get; set; }
    }
    public class AttachmentModel
    {
        public string AttachmentType { get; set; }
        public string Base64String { get; set; }
    }
    public class CustomerApprovalViewModel
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string MobileNo { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string ModifyBy { get; set; }
 
        public string ModifyStatus { get; set; }

        public string OutMessage { get; set; }
    }


}


