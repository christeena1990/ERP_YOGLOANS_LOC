using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using ERP_YOGLOANS_LOCAL.Models;


namespace ERP_YOGLOANS_LOCAL.Models
{
    public class tbl_customer_photo
    {
        public long cid { get; set; }
        public byte[] FileName { get; set; }
        public HttpPostedFileBase image { get; set; }
        public HttpPostedFileBase image1 { get; set; }
        public HttpPostedFileBase image3 { get; set; }
        public HttpPostedFileBase image4 { get; set; }
        public string CustomerName { get; set; }

        //public HttpPostedFileBase fileAttachment { get; set; }
        public int status_id { get; set; }
        public long enter_by { get; set; }
        public DateTime enter_dt { get; set; }

        public string ImageUrl { get; set; }
        public string Proof_Type { get; set; }
        public string KYC { get; set; }

        public string photo { get; set; }
        public int sno { get; set; }

        public int kyc_id { get; set; }
        public string id_no { get; set; }
        public string from_date { get; set; }

        public string to_date { get; set; }

        public string dob_validation { get; set; }
        public string row { get; set; }
        public List<kyc_upload> kycgrid { get; set; }
        public string pan_customer_name { get; set; }
        public string pan_dob { get; set; }
        public string pan_father_name { get; set; }


    }
 
    public class kyc_upload
    {
        public long cid { get; set; }
        public int kyc_id { get; set; }
        public byte[] Image { get; set; }
        public string Proof_Type { get; set; }
        public int sno { get; set; }
        public string KYC { get; set; }
        public string id_no { get; set; }
        public string FromDate { get; set; }

        public string ToDate { get; set; }
        public int Columns { get; set; }
        public int kycgrid { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPdf { get; set; }
        public string pan_customer_name { get; set; }
        public string pan_dob { get; set; }
        public string pan_father_name{ get; set; }
    }
    public class declaration_view
    {
        public int cid { get; set; }
        public int customer_temp_id { get; set; }

        public string customer_name { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }

        public string gender { get; set; }
        public string m_status { get; set; }

        public string branch_name { get; set; }


        public string mob_no { get; set; }

        public string dob { get; set; }

        public string email { get; set; }

        public string father_name { get; set; }

        public string mother_name { get; set; }

        public string guardian_name { get; set; }
        public string residence_type { get; set; }
        public string marital_status { get; set; }
        public string spouse_name { get; set; }



        public string occupation { get; set; }

        public string education { get; set; }
        public string nationality { get; set; }

        public float Annual_income { get; set; }
        //  public float annualincome { get; set; }


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

        public string mobile_no { get; set; }

        public string ImageUrl1 { get; set; }
        public string photo { get; set; }
        //------------------
        public string PANCard { get; set; }
        public string passport { get; set; }

        public string DrivingLicense { get; set; }

        public string VotersIdentityCard { get; set; }
        public string RationCard { get; set; }
        public string AadharCard { get; set; }
        public string AadharUnmasked { get; set; }

        public string todaydate { get; set; }


        public bool IsPermanentSameAsCommunication { get; set; }
    }

    public class form_60_model
    {
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }

       
        public string dob { get; set; }

        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Month1 { get; set; }
        public string Month2 { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string fathers_name { get; set; }
        public string address { get; set; }
        public string land_mark { get; set; }
        public string city { get; set; }
        public string district_name { get; set; }
        public string state_name { get; set; }
        public string pin_code { get; set; }
        public string mobile_no { get; set; }
        public string aadhar { get; set; }

        public string todaydate { get; set; }

    }
    public class KYCModel
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public string CustomerID { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please select a KYC option")]
        public int SelectedKYCOption { get; set; }

        [Required(ErrorMessage = "ID Number is required")]
        public string IDNumber { get; set; }



    }



    public class KYCType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class image_onload
    {
        public string ImageUrl { get; set; }
        // Add other properties if needed
    }



    public class KYCModel3
    {
        public string proof_type { get; set; }
        public string kyc_name { get; set; }

        public string sl_no { get; set; }

        public int kyc_id { get; set; }
    }
}







