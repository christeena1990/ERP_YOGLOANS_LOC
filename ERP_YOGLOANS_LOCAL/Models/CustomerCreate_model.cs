using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class CustomerCreate_model
    {
        public string mobile_no { get; set; }
        public string fname { get; set; }
        public string ddl_address { get; set; }
        public string ddl_idproof { get; set; }
        public string customer_id { get; set; }
        public string lname { get; set; }
        public string txt_add_proof_no { get; set; }
        public string txt_id_proof_no { get; set; }
        public List<modify_details> kycgrid { get; set; }

    }

    public class ddl_fields
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class cust_details
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }

    }
    public class modify_details
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
        public string ModifyDate { get; set; }

        public string SlNo { get; set; }
    }

    public class searchPartial_model
    {
        public List<string> Col_Head { get; set; }
        public List<DataItems> Col_Data { get; set; }
    }
    public class searchPartial_model1
    {
        public List<string> Col_Head1 { get; set; }
        public List<DataItems1> Col_Data1 { get; set; }
    }

    public class DataItems
    {
        public string CustomerID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string PostOffice { get; set; }
        public string PinCode { get; set; }
        public string MobileNo { get; set; }
        public string Branch { get; set; }
        public string Dob { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public string Update_dt { get; set; }



    }

    public class DataItems1
    {
        //public string CustomerID { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        // public string FatherName { get; set; }
        public string Address { get; set; }
        // public string LandMark { get; set; }
        //  public string PostOffice { get; set; }
        //  public string PinCode { get; set; }
        public string MobileNo { get; set; }
        // public string Branch { get; set; }
        // public string Dob { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }



    }
    public class searchRequest_model
    {
        public string MobileNo { get; set; }
        public string Eid { get; set; }
        public string Id { get; set; }
        public string name { get; set; }
        public string f_name { get; set; }
        public string s_name { get; set; }
        public string CustId { get; set; }
        public string ddl_id { get; set; }
        public string ddl_add { get; set; }
        public int branchid { get; set; }

    }
    public class otp_model
    {
        public string MobNo { get; set; }
        public string UserId { get; set; }
        public string mobile_no { get; set; }

    }
    public class re_otp_model
    {
        public string MobNo { get; set; }
        public string UserId { get; set; }
        public string mobile_no { get; set; }

    }
    public class CreateTempCustomer_model
    {
        public int custid_temp { get; set; }
        public string mobile_no { get; set; }
        public string f_name { get; set; }
        public string s_name { get; set; }
        public string l_name { get; set; }
        public string name { get; set; }
        public string branch { get; set; }

        public string enter_by { get; set; }
    }

}