using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class modify_model
    {

        //Personal informations
        public int customer_id { get; set; }
        public string Customer_name { get; set; }
        public string category_id { get; set; }
        public string branch_name { get; set; }
        //public DateTime create_date { get; set; }

        public string create_date { get; set; }

        public string status_id { get; set; }
        public string mob_no { get; set; }
        public string land_no { get; set; }
        public string office_no { get; set; }
        public string email { get; set; }
        // public DateTime? dob { get; set; }

        public string dob { get; set; }

        public string gender { get; set; }
        public string marital_status { get; set; }
        public string spause_name { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
        public string guardian { get; set; }
        public string residence_type { get; set; }
        public int yearofstay { get; set; }
        public int dist_from_branch { get; set; }
        public int branch_id { get; set; }
        public float annual_income { get; set; }
        public string residence_no { get; set; }
        public int education { get; set; }

        public int occupation { get; set; }
        public string occupation_d { get; set; }

        public string return_reason { get; set; }

        //Photo
        public string ImageUrl1 { get; internal set; }


        //Communication Address
        public string house_name { get; set; }
        public string landmark { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string PostOffice { get; set; }
        public string Post_name { get; set; }
        public string District { get; set; }
        public string State { get; set; }


        //Permananent address
        public string p_house_name { get; set; }
        public string p_landmark { get; set; }
        public string p_City { get; set; }
        public string p_Pincode { get; set; }
        public string p_PostOffice { get; set; }
        public string p_Post_name { get; set; }
        public string p_District { get; set; }
        public string p_State { get; set; }
        public int hidden_mobilestatus { get; set; }
        public int rekyc_status { get; set; }




        // KYC details
        //public List<kyc_upload_M> kycgrids { get; set; }



        public string kyc { get; set; }
        public int kyc_id { get; set; }
        public string id_no { get; set; }
        public string proof_type { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }      
        public string todaydate { get; set; }
        

        public List<Dictionary<string, object>> Data_M { get; set; }

        public List<ERP_YOGLOANS_LOCAL.Models.kyc_upload_M> kycgrids { get; set; }



        public HttpPostedFileBase image { get; set; }
        public HttpPostedFileBase image1 { get; set; }
        public HttpPostedFileBase image3 { get; set; }
        public HttpPostedFileBase image4 { get; set; }

        public HttpPostedFileBase image5 { get; set; }

        public string ImageUrl_pattach { get; internal set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }

        public string photo { get; set; }



        public string PANCard { get; set; }

        public string passport { get; set; }
        public string DrivingLicense { get; set; }

        public string VotersIdentityCard { get; set; }
        public string RationCard { get; set; }
        public string AadharCard { get; set; }
        public string AadharUnmasked { get; set; }

        public int hidden_kyc_status { get; set; }

        public string btn_edit_status { get; set; }
        //pan details
        public string pan_father_name { get; set; }
        public string pan_dob { get; set; }

        public string pan_name { get; set; }

    }
    public class kyc_upload_M
    {
        public int sno { get; set; }
        public long cid { get; set; }
        public int kyc_id { get; set; }
        public string KYC { get; set; }
        public byte[] Image { get; set; }
        public string Proof_Type { get; set; }


        public string id_no { get; set; }
        public int Columns { get; set; }
        public int kycgrid { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPdf { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Father {get;set;}
        public string Name { get; set; }
        public string DOB { get; set; }



    }

    public class KYCType1
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}