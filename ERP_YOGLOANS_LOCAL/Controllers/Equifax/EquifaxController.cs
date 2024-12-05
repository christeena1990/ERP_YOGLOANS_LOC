using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Web.Routing;
using ERP_YOGLOANS_LOCAL.Models.Equifax_Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.UI;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

using static ERP_YOGLOANS_LOCAL.Models.Equifax_Model.Equifax_Enquiry_Model;
using System.Web.UI.WebControls;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Ocsp;
using Microsoft.Ajax.Utilities;
using Rotativa;







namespace ERP_YOGLOANS_LOCAL.Controllers.Equifax
{
    public class EquifaxController : Controller
    {

        String[] data;
        Byte[] bytes;
        Byte[] bytes1;
        Byte[] bytes2;
        string jsonstring;

        DB dbconnect = new DB();

      


        // GET: Equifax
        public ActionResult Equifax_Enquiry()
        {

            //Function call for fill  loan category
            DataTable loan_types = loandropdown();
            ViewData["LoanList"] = loan_types;

            //Function call for fill vehicle category
            DataTable vehicle_types = vehicledropdown();
            ViewData["VehicleList"] = vehicle_types;


            return View();
        }

        public DataTable loandropdown()
        {
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 30;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[equifax_json_report]", pr).Tables[0];
            dbconnect.Close();
            return dt;
        }


        public DataTable vehicledropdown()
        {

            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 11;
            dt = dbconnect.ExecuteDataset("loan_enquiry", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }






        [HttpPost]
        public ActionResult PincodeChanged(string pincode)
        {

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;
            pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[1].Value = pincode.Trim();

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[loan_enquiry]", pr);
            dbconnect.Close();
            // Assuming you have two DataTables in your DataSet


            if (ds != null && ds.Tables.Count > 1)
            {
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];

                if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
                {
                    Session["stateName_eqfx_id"] = dt2.Rows[0][1].ToString();
                    List<SelectListItem> postOfficeList = dt1.AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Value = row["post_id"].ToString(),
                        Text = row["post_name"].ToString()
                    })
                    .ToList();


                    List<SelectListItem> stateList = dt2.AsEnumerable()
                        .Select(row => new SelectListItem
                        {
                            /*Value = "1"*/
                            Value = row["equifax_stateid"].ToString(),

                            Text = row["state_name"].ToString()
                            //,hn_state= dt1.Rows[0][1].ToString()

                        })
                        .ToList();

                    // Create an anonymous object to hold both lists
                    var result = new
                    {
                        PostOffices = postOfficeList,

                        States = stateList
                    };

                    return Json(result);
                }
                else
                {
                    var result = new { };
                    return Json(result);
                }




            }


            else
            {
                var result = new { };
                return Json(result);
            }



        }

        //-----------------------------------------------------------------------------------//


        [HttpPost]
        public ActionResult btn_submit_Click(Equifax_Enquiry_Model model)
        {
            SqlParameter[] pr = new SqlParameter[22];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;
            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branchid"];
            pr[2] = new SqlParameter("@en_name", SqlDbType.VarChar, 50);
            pr[2].Value = model.CustomerName;
            pr[3] = new SqlParameter("@en_addr", SqlDbType.VarChar, 500);
            pr[3].Value = model.CustomerAddress;
            pr[4] = new SqlParameter("@contact_no", SqlDbType.VarChar, 12);
            pr[4].Value = model.ContactNo1;
            pr[5] = new SqlParameter("@enquiry_amt", SqlDbType.BigInt);
            pr[5].Value = model.LoanAmount;

            pr[6] = new SqlParameter("@enter_by", SqlDbType.Int);
            pr[6].Value = Session["login_user"];
            pr[7] = new SqlParameter("@remarks", SqlDbType.VarChar, 100);
            pr[7].Value = model.Remarks;
            pr[8] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr[8].Direction = ParameterDirection.Output;
            if (model.LoanType == "10")
            {
                pr[9] = new SqlParameter("@vtype", SqlDbType.VarChar, 50);
                pr[9].Value = model.VehicleType;
            }
            else
            {
                pr[9] = new SqlParameter("@vtype", SqlDbType.VarChar, 50);
                pr[9].Value = "";
            }
            pr[10] = new SqlParameter("@contact_no2", SqlDbType.VarChar, 12);
            pr[10].Value = model.ContactNo2;
            pr[11] = new SqlParameter("@pan_no", SqlDbType.VarChar, 50);
            pr[11].Value = model.PANNumber;
            pr[12] = new SqlParameter("@aadhaar_no", SqlDbType.VarChar, 50);
            pr[12].Value = model.AadharNo;
            pr[13] = new SqlParameter("@voters_id", SqlDbType.VarChar, 50);
            pr[13].Value = model.VotersID;
            pr[14] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[14].Value = model.txt_cust_pincode.ToString();
            pr[15] = new SqlParameter("@post_id", SqlDbType.BigInt);
            pr[15].Value = model.PostOffice;
            pr[16] = new SqlParameter("@state_id", SqlDbType.BigInt);
            pr[16].Value = Session["stateName_eqfx_id"];
            pr[17] = new SqlParameter("@dob", SqlDbType.Date);
            pr[17].Value = model.DateOfBirth;
            pr[18] = new SqlParameter("@product", SqlDbType.Int);
            pr[18].Value = model.LoanType;
            pr[19] = new SqlParameter("@passport", SqlDbType.VarChar, 50);
            pr[19].Value = model.Passport;
            pr[20] = new SqlParameter("@drivinglic", SqlDbType.VarChar, 50);
            pr[20].Value = model.DrivingLicence;
            pr[21] = new SqlParameter("@rationCard", SqlDbType.VarChar, 50);
            pr[21].Value = model.RationCard;




            dbconnect.ExecuteStoredProcedure("[dbo].[loan_enquiry]", pr);
            dbconnect.Close();
            string h_msg = (string)pr[8].Value;

            //Split and process the output message if needed
            var data = h_msg.Split('@');



            if (data[1] != "1")
            {


                string state = "";
                string cust_name = model.CustomerName;
                string dob = model.DateOfBirth.ToString("yyyy-MM-dd");
                string address_line = model.CustomerAddress.Trim();
                string pin = model.txt_cust_pincode.ToString().Trim();

                // Retrieve data from the second stored procedure
                SqlParameter[] pr8 = new SqlParameter[2];
                pr8[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr8[0].Value = 12;
                pr8[1] = new SqlParameter("@phone_no", SqlDbType.BigInt);
                pr8[1].Value = pin;



                //DataTable dt1 = dbconnect.ExecuteDataset("[dbo].[equifax_json_report]", pr8).Tables[0];
                DataTable dt1 = dbconnect.ExecuteDataset("[equifax_json_report]", pr8).Tables[0];
                dbconnect.Close();

                if (dt1.Rows.Count > 0)
                {
                    state = dt1.Rows[0][0].ToString();
                }

                //// Collect additional data
                //string phone_num = model.ContactNo1;
                //string pan_num = model.PANNumber;
                //string aadhar = model.AadharNo;
                //string voter_id = model.VotersID;
                //string passport = model.Passport;
                //string driving = model.DrivingLicence;
                //string ration = model.RationCard;



                // Ensure null values are replaced with empty strings for all IDDetails fields

                string phone_num = model.ContactNo1 ?? "";
                string pan_num = model.PANNumber ?? "";
                string aadhar = model.AadharNo ?? "";
                string voter_id = model.VotersID ?? "";
                string passport = model.Passport ?? "";
                string driving = model.DrivingLicence ?? "";
                string ration = model.RationCard ?? "";



                /////////////////////////////////////////////////// ///////////////////////////////////////////////////
                /////////////////////////////////////////////////// ///////////////////////////////////////////////////

                Rootrequestobject root_requestobject = new Rootrequestobject();
                Requestheader requestheader = new Requestheader();
                Requestbody obj_requestbody = new Requestbody();
                Inquiryaddress obj_address = new Inquiryaddress();
                Inquiryphone obj_phone = new Inquiryphone();
                Score obj_score = new Score();
                Mfidetails obj_mfidetails = new Mfidetails();
                Familydetail obj_familydetail = new Familydetail();

                Iddetail obj_iddetail1 = new Iddetail();
                Iddetail obj_iddetail2 = new Iddetail();
                Iddetail obj_iddetail3 = new Iddetail();
                Iddetail obj_iddetail4 = new Iddetail();
                Iddetail obj_iddetail5 = new Iddetail();
                Iddetail obj_iddetail6 = new Iddetail();
                Iddetail obj_iddetail7 = new Iddetail();

                //Setting Request Header
                string[] product_Code = { "CCR" };
                requestheader.CustomerId = "4455";
                requestheader.UserId = "STS_YOGCCR";
                requestheader.Password = "W3#QeicsB";
                requestheader.MemberNumber = "032FZ01330";
                requestheader.SecurityCode = "8RI";
                requestheader.CustRefField = "1212";
                requestheader.ProductCode = product_Code;
                root_requestobject.RequestHeader = requestheader; //Requestheader


                //Name and DOB details
                obj_requestbody.InquiryPurpose = "00";
                obj_requestbody.FirstName = cust_name;
                obj_requestbody.MiddleName = "";
                obj_requestbody.LastName = "";
                obj_requestbody.DOB = dob;

                //Address details 
                string[] addresstype = { "H" };
                obj_address.seq = "1";
                obj_address.AddressType = addresstype;
                obj_address.AddressLine1 = address_line;
                obj_address.State = state;
                obj_address.Postal = pin;
                obj_requestbody.InquiryAddresses = new[] { obj_address };

                //Contact details  
                string[] phonetype = { "M" };
                obj_phone.seq = "1";
                obj_phone.Number = phone_num;
                obj_phone.PhoneType = phonetype;
                obj_requestbody.InquiryPhones = new[] { obj_phone };

                //ID details            
                obj_iddetail1.seq = "1";
                obj_iddetail1.IDType = "T";
                obj_iddetail1.IDValue = pan_num;
                obj_iddetail1.Source = "Inquiry";

                obj_iddetail2.seq = "2";
                obj_iddetail2.IDType = "P";
                obj_iddetail2.IDValue = passport;
                obj_iddetail2.Source = "Inquiry";

                obj_iddetail3.seq = "3";
                obj_iddetail3.IDType = "V";
                obj_iddetail3.IDValue = voter_id;
                obj_iddetail3.Source = "Inquiry";

                obj_iddetail4.seq = "4";
                obj_iddetail4.IDType = "D";
                obj_iddetail4.IDValue = driving;
                obj_iddetail4.Source = "Inquiry";

                obj_iddetail5.seq = "5";
                obj_iddetail5.IDType = "M";
                obj_iddetail5.IDValue = aadhar;
                obj_iddetail5.Source = "Inquiry";

                obj_iddetail6.seq = "6";
                obj_iddetail6.IDType = "R";
                obj_iddetail6.IDValue = ration;
                obj_iddetail6.Source = "Inquiry";

                obj_iddetail7.seq = "7";
                obj_iddetail7.IDType = "O";
                obj_iddetail7.IDValue = "";
                obj_iddetail7.Source = "Inquiry";

                obj_requestbody.IDDetails = new[] { obj_iddetail1 , obj_iddetail2 , obj_iddetail3 , obj_iddetail4 , obj_iddetail5 , obj_iddetail6
            , obj_iddetail7};

                obj_familydetail.seq = "1";
                obj_familydetail.AdditionalNameType = "K02";
                obj_familydetail.AdditionalName = "";

                Familydetail obj_familydetail1 = new Familydetail();
                obj_familydetail1.seq = "2";
                obj_familydetail1.AdditionalNameType = "K01";
                obj_familydetail1.AdditionalName = "";

                obj_mfidetails.FamilyDetails = new[] { obj_familydetail, obj_familydetail1 };
                obj_requestbody.MFIDetails = obj_mfidetails; //MFI details

                root_requestobject.RequestBody = obj_requestbody; //Requestbody

                obj_score.Type = "ERS";
                obj_score.Version = "4.0";

                root_requestobject.Score = new[] { obj_score }; //Score

                string input = JsonConvert.SerializeObject(root_requestobject);

                //jsonstring = StartAsyn(root_requestobject).GetAwaiter().GetResult();

              

                //abhila
                //jsonstring = "{\"InquiryResponseHeader\":{\"ClientID\":\"032FZ01330\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1512319082\",\"ProductCode\":[\"CCR\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-17\",\"Time\":\"11:53:48\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"00\",\"FirstName\":\"Abhila K S\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"Koottalakkal House\",\"State\":\"KL        \",\"Postal\":\"680551\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9567427023\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"DVAPA7275R\",\"Source\":\"Inquiry\"},{\"seq\":\"2\",\"IDType\":\"P\",\"Source\":\"Inquiry\"},{\"seq\":\"3\",\"IDType\":\"V\",\"Source\":\"Inquiry\"},{\"seq\":\"4\",\"IDType\":\"D\",\"Source\":\"Inquiry\"},{\"seq\":\"5\",\"IDType\":\"M\",\"IDValue\":\"XXXXXXXXXXXX\",\"Source\":\"Inquiry\"},{\"seq\":\"6\",\"IDType\":\"R\",\"Source\":\"Inquiry\"},{\"seq\":\"7\",\"IDType\":\"O\",\"Source\":\"Inquiry\"}],\"DOB\":\"1989-02-15\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}},\"Score\":[{\"Type\":\"ERS\",\"Version\":\"4.0\"}],\"CCRResponse\":{\"Status\":\"1\",\"CIRReportDataLst\":[{\"InquiryResponseHeader\":{\"CustomerCode\":\"YGKM\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1512319082\",\"ProductCode\":[\"PCS\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-17\",\"Time\":\"11:53:48\",\"HitCode\":\"10\",\"CustomerName\":\"YGKM\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"Other\",\"FirstName\":\"Abhila K S\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"Koottalakkal House\",\"State\":\"KL        \",\"Postal\":\"680551\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9567427023\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"DVAPA7275R\",\"Source\":\"Inquiry\"},{\"seq\":\"5\",\"IDType\":\"M\",\"IDValue\":\"XXXXXXXXXXXX\",\"Source\":\"Inquiry\"}],\"DOB\":\"1989-02-15\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}},\"Score\":[{\"Type\":\"ERS\",\"Version\":\"4.0\"}],\"CIRReportData\":{\"IDAndContactInfo\":{\"PersonalInfo\":{\"Name\":{\"FullName\":\"ABHILA K \",\"FirstName\":\"ABHILA \",\"LastName\":\"K \"},\" AliasName\":{},\"DateOfBirth\":\"1989-02-15\",\"Gender\":\"Female\",\"Age\":{\"Age\":\"35\"},\"PlaceOfBirthInfo\":{}},\"IdentityInfo\":{\"PANId\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"IdNumber\":\"DVAPA7275R\"}],\"OtherId\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"IdNumber\":\"40027005831045\"}]},\"AddressInfo\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"Address\":\"KOOTTALAKKAL HOUSE CHITTILAPPILLY CHITTILAPPILLY THRISSUR 844 KERALA\",\"State\":\"KL\",\"Postal\":\"680551\"},{\"Seq\":\"2\",\"ReportedDate\":\"2024-06-30\",\"Address\":\"YOGAKSHEMA SABHA VADAKKE STAND ROAD  THIRUVAMBADY 844 KERALA\",\"State\":\"KL\",\"Postal\":\"680022\",\"Type\":\"Office\"},{\"Seq\":\"3\",\"ReportedDate\":\"2024-04-30\",\"Address\":\"KOOTTALAKKAL HOUSE CHITTILAPPILLY P O CHITTILAPPILLY  NR IES COLLEGE KERALA 680551\",\"State\":\"KL\",\"Postal\":\"680551\",\"Type\":\"Primary\"},{\"Seq\":\"4\",\"ReportedDate\":\"2023-09-01\",\"Address\":\"KOOTTALAKKAL HOUSE CHITTILAPPILLY P O CHITTILAPPILLY KERALA 680551\",\"State\":\"KL\",\"Postal\":\"680551\",\"Type\":\"Primary\"},{\"Seq\":\"5\",\"ReportedDate\":\"2022-11-30\",\"Address\":\"KOOTTALAKKAL HOUSE CHITTILAPPILLY P O CHITTILAPPILLY THRISSUR KERALA 680551\",\"State\":\"KL\",\"Postal\":\"680551\",\"Type\":\"Primary\"}],\"PhoneInfo\":[{\"seq\":\"1\",\"typeCode\":\"H\",\"ReportedDate\":\"2024-06-30\",\"Number\":\"9567427023\"},{\"seq\":\"2\",\"typeCode\":\"M\",\"ReportedDate\":\"2024-06-30\",\"Number\":\"9567427023\"}],\"EmailAddressInfo\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-06-18\",\"EmailAddress\":\"ABHILA.CHITTILAPPILLY@GMAIL.COM\"}]},\"RetailAccountDetails\":[{\"seq\":\"1\",\"AccountNumber\":\"**********\",\"Institution\":\"BANK\",\"AccountType\":\"Two-wheeler Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"83818\",\"PastDueAmount\":\"0\",\"Open\":\"Yes\",\"SanctionAmount\":\"83818\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2024-06-17\",\"RepaymentTenure\":\"24\",\"InstallmentAmount\":\"4418\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"New Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"2\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"35990\",\"LastPaymentDate\":\"2024-05-02\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2023-08-27\",\"DateClosed\":\"2024-05-04\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"12\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"3\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"1036\",\"Open\":\"No\",\"SanctionAmount\":\"11847\",\"LastPaymentDate\":\"2023-04-02\",\"DateReported\":\"2023-04-12\",\"DateOpened\":\"2022-08-12\",\"DateClosed\":\"2023-04-02\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"12\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"4\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"10600\",\"LastPaymentDate\":\"2023-02-02\",\"DateReported\":\"2023-04-01\",\"DateOpened\":\"2022-06-18\",\"DateClosed\":\"2023-02-04\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"12\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"5\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"10600\",\"LastPaymentDate\":\"2023-02-02\",\"DateReported\":\"2023-04-01\",\"DateOpened\":\"2022-06-18\",\"DateClosed\":\"2023-02-04\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"12\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]}],\"RetailAccountsSummary\":{\"NoOfAccounts\":\"5\",\"NoOfActiveAccounts\":\"1\",\"NoOfWriteOffs\":\"0\",\"TotalPastDue\":\"0.00\",\"MostSevereStatusWithIn24Months\":\"Non-Delnqt\",\"SingleHighestCredit\":\"0.00\",\"SingleHighestSanctionAmount\":\"83818.00\",\"TotalHighCredit\":\"0.00\",\"AverageOpenBalance\":\"83818.00\",\"SingleHighestBalance\":\"83818.00\",\"NoOfPastDueAccounts\":\"0\",\"NoOfZeroBalanceAccounts\":\"0\",\"RecentAccount\":\"Two-wheeler Loan on 17-06-2024\",\"OldestAccount\":\"Consumer Loan on 18-06-2022\",\"TotalBalanceAmount\":\"83818.00\",\"TotalSanctionAmount\":\"83818.00\",\"TotalCreditLimit\":\"0.0\",\"TotalMonthlyPaymentAmount\":\"4418.00\"},\"ScoreDetails\":[{\"Type\":\"ERS\",\"Version\":\"4.0\",\"Name\":\"ERS4.0\",\"Value\":\"738\",\"ScoringElements\":[{\"type\":\"RES\",\"seq\":\"1\",\"code\":\"200\",\"Description\":\"No Live Accounts\"},{\"type\":\"RES\",\"seq\":\"2\",\"code\":\"204\",\"Description\":\"Number of Home Loans\"},{\"type\":\"RES\",\"seq\":\"3\",\"code\":\"202\",\"Description\":\"Balance On Unsecured Trade\"}]}],\"Enquiries\":[{\"seq\":\"1\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"Date\":\"2022-11-04\",\"Time\":\"09:49\",\"RequestPurpose\":\"13\",\"Amount\":\"10000\"},{\"seq\":\"2\",\"Institution\":\"FINANCE\",\"Date\":\"2022-11-02\",\"Time\":\"13:09\",\"RequestPurpose\":\"00\"},{\"seq\":\"3\",\"Institution\":\"FINANCE\",\"Date\":\"2022-11-02\",\"Time\":\"12:51\",\"RequestPurpose\":\"00\"},{\"seq\":\"4\",\"Institution\":\"FINANCE\",\"Date\":\"2022-10-25\",\"Time\":\"13:22\",\"RequestPurpose\":\"00\"},{\"seq\":\"5\",\"Institution\":\"FINANCE\",\"Date\":\"2022-10-22\",\"Time\":\"11:13\",\"RequestPurpose\":\"00\"},{\"seq\":\"6\",\"Institution\":\"FINANCE\",\"Date\":\"2022-10-01\",\"Time\":\"16:20\",\"RequestPurpose\":\"00\"},{\"seq\":\"7\",\"Institution\":\"FINANCE\",\"Date\":\"2022-09-27\",\"Time\":\"10:28\",\"RequestPurpose\":\"00\"},{\"seq\":\"8\",\"Institution\":\"FINANCE\",\"Date\":\"2022-09-27\",\"Time\":\"10:09\",\"RequestPurpose\":\"00\"},{\"seq\":\"9\",\"Institution\":\"FINANCE\",\"Date\":\"2022-09-27\",\"Time\":\"09:58\",\"RequestPurpose\":\"00\"},{\"seq\":\"10\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"Date\":\"2022-09-16\",\"Time\":\"13:07\",\"RequestPurpose\":\"13\",\"Amount\":\"100000\"}],\"EnquirySummary\":{\"Purpose\":\"ALL\",\"Total\":\"10\",\"Past30Days\":\"0\",\"Past12Months\":\"0\",\"Past24Months\":\"10\",\"Recent\":\"2022-11-04\"},\"OtherKeyInd\":{\"AgeOfOldestTrade\":\"26\",\"NumberOfOpenTrades\":\"1\",\"AllLinesEVERWritten\":\"0.00\",\"AllLinesEVERWrittenIn9Months\":\"0\",\"AllLinesEVERWrittenIn6Months\":\"0\"},\"RecentActivities\":{\"AccountsDeliquent\":\"0\",\"AccountsOpened\":\"1\",\"TotalInquiries\":\"0\",\"AccountsUpdated\":\"2\"}}},{\"InquiryResponseHeader\":{\"ClientID\":\"032FZ01330\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1512319082\",\"ProductCode\":[\"MCS\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-17\",\"Time\":\"11:53:48\",\"HitCode\":\"00\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"00\",\"FirstName\":\"Abhila K S\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"Koottalakkal House\",\"State\":\"KL        \",\"Postal\":\"680551\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9567427023\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"DVAPA7275R\",\"Source\":\"Inquiry\"},{\"seq\":\"5\",\"IDType\":\"M\",\"IDValue\":\"XXXXXXXXXXXX\",\"Source\":\"Inquiry\"}],\"DOB\":\"1989-02-15\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}}}]}}";
                //savithri
                jsonstring = "{\"InquiryResponseHeader\":{\"ClientID\":\"032FZ01330\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1533455618\",\"ProductCode\":[\"CCR\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-29\",\"Time\":\"12:09:10\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"00\",\"FirstName\":\"SAVITHIRI\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"51 SAKTHI NAGAR COIMBATORE\",\"State\":\"TN        \",\"Postal\":\"641015\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9500770546\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"GSNPS5215J\",\"Source\":\"Inquiry\"},{\"seq\":\"2\",\"IDType\":\"P\",\"Source\":\"Inquiry\"},{\"seq\":\"3\",\"IDType\":\"V\",\"IDValue\":\"RTG1027267\",\"Source\":\"Inquiry\"},{\"seq\":\"4\",\"IDType\":\"D\",\"Source\":\"Inquiry\"},{\"seq\":\"5\",\"IDType\":\"M\",\"Source\":\"Inquiry\"},{\"seq\":\"6\",\"IDType\":\"R\",\"Source\":\"Inquiry\"},{\"seq\":\"7\",\"IDType\":\"O\",\"Source\":\"Inquiry\"}],\"DOB\":\"1979-10-11\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}},\"Score\":[{\"Type\":\"ERS\",\"Version\":\"4.0\"}],\"CCRResponse\":{\"Status\":\"1\",\"CIRReportDataLst\":[{\"InquiryResponseHeader\":{\"CustomerCode\":\"YGKM\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1533455618\",\"ProductCode\":[\"PCS\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-29\",\"Time\":\"12:09:10\",\"HitCode\":\"10\",\"CustomerName\":\"YGKM\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"Other\",\"FirstName\":\"SAVITHIRI\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"51 SAKTHI NAGAR COIMBATORE\",\"State\":\"TN        \",\"Postal\":\"641015\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9500770546\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"GSNPS5215J\",\"Source\":\"Inquiry\"},{\"seq\":\"3\",\"IDType\":\"V\",\"IDValue\":\"RTG1027267\",\"Source\":\"Inquiry\"}],\"DOB\":\"1979-10-11\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}},\"Score\":[{\"Type\":\"ERS\",\"Version\":\"4.0\"}],\"CIRReportData\":{\"IDAndContactInfo\":{\"PersonalInfo\":{\"Name\":{\"FullName\":\"SAVITHRI MANAVALAN \",\"FirstName\":\"SAVITHRI \",\"LastName\":\"MANAVALAN \"},\" AliasName\":{},\"DateOfBirth\":\"1979-10-11\",\"Gender\":\"Female\",\"Age\":{\"Age\":\"44\"},\"PlaceOfBirthInfo\":{},\"TotalIncome\":\"9000\"},\"IdentityInfo\":{\"PANId\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-07-31\",\"IdNumber\":\"GSNPS5215J\"}],\"VoterID\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-07-31\",\"IdNumber\":\"RTG1027267\"},{\"seq\":\"2\",\"ReportedDate\":\"2024-04-30\",\"IdNumber\":\"FLH3089117\"}],\"NationalIDCard\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-07-31\",\"IdNumber\":\"XXXXXXXXXXXX\"}],\"RationCard\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-07-31\",\"IdNumber\":\"333757864839\"}],\"OtherId\":[{\"seq\":\"1\",\"ReportedDate\":\"2024-04-01\",\"IdNumber\":\"20020314962639\"}]},\"AddressInfo\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-07-31\",\"Address\":\"SUNDARAM 51 SAKTHI NAGAR  VINAYAGAR KOVI ALAMELU OUT COIMBATORE UPPILIPALAYAM S O\",\"State\":\"TN\",\"Postal\":\"641015\",\"Type\":\"Owns,Permanent\"},{\"Seq\":\"2\",\"ReportedDate\":\"2024-07-31\",\"Address\":\"NO4 MUTHU NAGAR  UPPILIPALAYAM POST MASAKALIPALAYAM VR PURAM UPPILIPALAYAM COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\",\"Type\":\"Owns,Primary\"},{\"Seq\":\"3\",\"ReportedDate\":\"2024-01-31\",\"Address\":\"25 BHARATHI NAGAR  NEAR SENTHIL RICE MANDI UPPLIPALAYAMPOST COIMBATORE COIMBATORE V R PURAM 1084 TAMIL NADU\",\"State\":\"TN\",\"Postal\":\"641015\",\"Type\":\"Primary\"},{\"Seq\":\"4\",\"ReportedDate\":\"2023-11-30\",\"Address\":\"SAKTHI NAGAR VINAYAGAR KOVIL\",\"State\":\"TN\",\"Postal\":\"641015\",\"Type\":\"Rents,Primary\"},{\"Seq\":\"5\",\"ReportedDate\":\"2023-01-31\",\"Address\":\"51 SAKTHINAGER VINAYAKAR KOVIL STREET  COIMBATORE TAMIL NADU 641015\",\"State\":\"TN\",\"Postal\":\"641015\",\"Type\":\"Owns,Permanent\"}],\"PhoneInfo\":[{\"seq\":\"1\",\"typeCode\":\"H\",\"ReportedDate\":\"2024-02-29\",\"Number\":\"9500770546\"},{\"seq\":\"2\",\"typeCode\":\"H\",\"ReportedDate\":\"2024-05-31\",\"Number\":\"9500470810\"},{\"seq\":\"3\",\"typeCode\":\"M\",\"ReportedDate\":\"2024-07-31\",\"Number\":\"9500770546\"},{\"seq\":\"4\",\"typeCode\":\"M\",\"ReportedDate\":\"2024-07-31\",\"Number\":\"9488830411\"}],\"EmailAddressInfo\":[{\"seq\":\"1\",\"ReportedDate\":\"2023-01-31\",\"EmailAddress\":\"RICHMERCY2001@GMAIL.COM\"}]},\"RetailAccountDetails\":[{\"seq\":\"1\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"38300\",\"PastDueAmount\":\"0\",\"Open\":\"Yes\",\"SanctionAmount\":\"38300\",\"LastPaymentDate\":\"2023-04-12\",\"DateReported\":\"2023-04-30\",\"DateOpened\":\"2023-04-12\",\"InterestRate\":\"20\",\"RepaymentTenure\":\"9\",\"CollateralType\":\"Gold\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"2\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"2071\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-11-07\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"3\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"2071\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-11-07\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"4\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"2071\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-11-09\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"5\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"2071\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-11-08\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"6\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"2071\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-11-08\",\"AccountStatus\":\"New Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"7\",\"AccountNumber\":\"**********\",\"Institution\":\"BANK\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"51457\",\"PastDueAmount\":\"0\",\"LastPayment\":\"155696\",\"Open\":\"Yes\",\"SanctionAmount\":\"135176\",\"LastPaymentDate\":\"2024-06-29\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2022-03-19\",\"RepaymentTenure\":\"36\",\"InstallmentAmount\":\"5952\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Current Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"8\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"8000\",\"PastDueAmount\":\"0\",\"Open\":\"Yes\",\"SanctionAmount\":\"8000\",\"LastPaymentDate\":\"2023-11-20\",\"DateReported\":\"2024-05-31\",\"DateOpened\":\"2023-11-20\",\"InterestRate\":\"28\",\"RepaymentTenure\":\"12\",\"AccountStatus\":\"Current Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"05-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"9\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Two-wheeler Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"42109\",\"PastDueAmount\":\"8769\",\"Open\":\"Yes\",\"SanctionAmount\":\"69750\",\"LastPaymentDate\":\"2023-11-25\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2021-12-13\",\"RepaymentTenure\":\"36\",\"InstallmentAmount\":\"2923\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"60-89 days past due\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-21\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"10\",\"AccountNumber\":\"110107100049\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"AccountType\":\"Business Loan\",\"OwnershipType\":\"Joint Account\",\"Balance\":\"34227\",\"PastDueAmount\":\"34227\",\"Open\":\"Yes\",\"SanctionAmount\":\"60000\",\"LastPaymentDate\":\"2024-07-30\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-06-27\",\"InterestRate\":\"26\",\"RepaymentTenure\":\"12\",\"InstallmentAmount\":\"5732\",\"AccountStatus\":\"180-359 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"11\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"24502\",\"PastDueAmount\":\"24985\",\"Open\":\"Yes\",\"SanctionAmount\":\"24502\",\"LastPaymentDate\":\"2023-09-16\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2022-12-22\",\"InstallmentAmount\":\"24502\",\"AccountStatus\":\"180-359 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"12\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Commercial Vehicle Loan\",\"OwnershipType\":\"Guarantor\",\"Balance\":\"31287\",\"PastDueAmount\":\"27575\",\"Open\":\"Yes\",\"SanctionAmount\":\"40000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-08-12\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"180-359 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"13\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Business Loan - Unsecured\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"30000\",\"Open\":\"No\",\"SanctionAmount\":\"30000\",\"LastPaymentDate\":\"2024-02-22\",\"DateReported\":\"2024-08-03\",\"DateOpened\":\"2023-10-03\",\"DateClosed\":\"2024-02-22\",\"Reason\":\"Closed Account\",\"InterestRate\":\"28.00\",\"RepaymentTenure\":\"5\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SPM\"},{\"key\":\"12-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SPM\"},{\"key\":\"11-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SPM\"},{\"key\":\"10-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SPM\"}]},{\"seq\":\"14\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"38300\",\"LastPaymentDate\":\"2023-06-01\",\"DateReported\":\"2024-01-20\",\"DateOpened\":\"2023-04-12\",\"DateClosed\":\"2024-01-10\",\"Reason\":\"Closed Account\",\"InterestRate\":\"31\",\"RepaymentTenure\":\"9\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"15\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"57400\",\"LastPaymentDate\":\"2023-06-01\",\"DateReported\":\"2024-01-20\",\"DateOpened\":\"2023-05-13\",\"DateClosed\":\"2024-01-02\",\"Reason\":\"Closed Account\",\"InterestRate\":\"31\",\"RepaymentTenure\":\"9\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-24\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"16\",\"AccountNumber\":\"210107106847\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"50400\",\"LastPaymentDate\":\"2023-11-21\",\"DateReported\":\"2023-12-31\",\"DateOpened\":\"2023-06-23\",\"DateClosed\":\"2023-11-21\",\"Reason\":\"Closed Account\",\"InterestRate\":\"14\",\"RepaymentTenure\":\"12\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"12-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"17\",\"AccountNumber\":\"210107106848\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"50400\",\"LastPaymentDate\":\"2023-11-21\",\"DateReported\":\"2023-12-31\",\"DateOpened\":\"2023-06-23\",\"DateClosed\":\"2023-11-21\",\"Reason\":\"Closed Account\",\"InterestRate\":\"14\",\"RepaymentTenure\":\"12\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"12-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"18\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"55800\",\"LastPaymentDate\":\"2023-11-20\",\"DateReported\":\"2023-11-30\",\"DateOpened\":\"2023-09-30\",\"DateClosed\":\"2023-11-20\",\"Reason\":\"Closed Account\",\"InterestRate\":\"20\",\"RepaymentTenure\":\"12\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"19\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"57200\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"57200\",\"LastPaymentDate\":\"2023-09-30\",\"DateReported\":\"2023-09-30\",\"DateOpened\":\"2023-06-01\",\"DateClosed\":\"2023-09-30\",\"Reason\":\"Closed Account\",\"InterestRate\":\"24\",\"RepaymentTenure\":\"9\",\"CollateralType\":\"Gold\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"09-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"20\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"35000\",\"LastPaymentDate\":\"2020-09-29\",\"DateReported\":\"2023-07-31\",\"DateOpened\":\"2019-07-01\",\"DateClosed\":\"2020-09-29\",\"Reason\":\"Closed or Paid Account/Zero Balance\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-20\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-20\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"21\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"32190\",\"Open\":\"No\",\"SanctionAmount\":\"36200\",\"LastPaymentDate\":\"2023-06-12\",\"DateReported\":\"2023-06-30\",\"DateOpened\":\"2022-12-05\",\"DateClosed\":\"2023-06-14\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"36\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"22\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"47000\",\"LastPaymentDate\":\"2017-09-02\",\"DateReported\":\"2023-04-01\",\"DateOpened\":\"2016-10-31\",\"DateClosed\":\"2021-06-05\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"10\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-21\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-21\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-21\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-21\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-21\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-21\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-21\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"23\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-02-12\",\"DateReported\":\"2023-02-28\",\"DateOpened\":\"2023-01-06\",\"DateClosed\":\"2023-02-12\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"02-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"24\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-02-12\",\"DateReported\":\"2023-02-28\",\"DateOpened\":\"2023-01-03\",\"DateClosed\":\"2023-02-12\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"02-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"25\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-03\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-07\",\"DateClosed\":\"2023-01-03\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"26\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-08\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-10\",\"DateClosed\":\"2023-01-08\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"27\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-06\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-06\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"28\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-08\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-09\",\"DateClosed\":\"2023-01-08\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"29\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-06\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-06\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"30\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-06\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-06\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"31\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-07\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"32\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-07\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"33\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-06\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-06\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"34\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2023-01-06\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-12-08\",\"DateClosed\":\"2023-01-06\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"35\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-11-07\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-10-08\",\"DateClosed\":\"2022-11-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"36\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-11-07\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-10-09\",\"DateClosed\":\"2022-11-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"37\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-11-07\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-10-07\",\"DateClosed\":\"2022-11-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"38\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-11-07\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-10-07\",\"DateClosed\":\"2022-11-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"39\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-11-07\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2022-10-08\",\"DateClosed\":\"2022-11-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"40\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-08-09\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-07-11\",\"DateClosed\":\"2022-08-09\",\"Reason\":\"Closed Account\",\"InterestRate\":\"36\",\"RepaymentTenure\":\"2\",\"InstallmentAmount\":\"0\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"41\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-10-07\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-09-05\",\"DateClosed\":\"2022-10-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"42\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-10-07\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-09-05\",\"DateClosed\":\"2022-10-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"43\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-10-07\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-09-06\",\"DateClosed\":\"2022-10-07\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"44\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"3000\",\"LastPaymentDate\":\"2022-10-27\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-09-20\",\"DateClosed\":\"2022-10-27\",\"Reason\":\"Closed Account\",\"InterestRate\":\"36\",\"RepaymentTenure\":\"2\",\"InstallmentAmount\":\"0\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"45\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"4000\",\"LastPaymentDate\":\"2022-10-13\",\"DateReported\":\"2022-10-31\",\"DateOpened\":\"2022-09-26\",\"DateClosed\":\"2022-10-13\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"46\",\"AccountNumber\":\"**********\",\"Institution\":\"BANK\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"19693\",\"Open\":\"No\",\"SanctionAmount\":\"18749\",\"LastPaymentDate\":\"2022-09-02\",\"DateReported\":\"2022-09-30\",\"DateOpened\":\"2022-03-01\",\"DateClosed\":\"2022-09-09\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"6\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"09-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"47\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-09-05\",\"DateReported\":\"2022-09-30\",\"DateOpened\":\"2022-08-09\",\"DateClosed\":\"2022-09-05\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"09-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"48\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"6500\",\"LastPaymentDate\":\"2022-09-26\",\"DateReported\":\"2022-09-30\",\"DateOpened\":\"2022-08-20\",\"DateClosed\":\"2022-09-26\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"09-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"49\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-08-09\",\"DateReported\":\"2022-08-31\",\"DateOpened\":\"2022-07-09\",\"DateClosed\":\"2022-08-09\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"50\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"3500\",\"LastPaymentDate\":\"2022-08-20\",\"DateReported\":\"2022-08-31\",\"DateOpened\":\"2022-07-30\",\"DateClosed\":\"2022-08-20\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"51\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-07-09\",\"DateReported\":\"2022-07-31\",\"DateOpened\":\"2022-06-14\",\"DateClosed\":\"2022-07-09\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"52\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"1500\",\"LastPaymentDate\":\"2022-07-29\",\"DateReported\":\"2022-07-31\",\"DateOpened\":\"2022-07-01\",\"DateClosed\":\"2022-07-29\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"53\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-06-12\",\"DateReported\":\"2022-06-30\",\"DateOpened\":\"2022-05-14\",\"DateClosed\":\"2022-06-12\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"54\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2000\",\"LastPaymentDate\":\"2022-05-14\",\"DateReported\":\"2022-05-31\",\"DateOpened\":\"2022-04-30\",\"DateClosed\":\"2022-05-14\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"05-22\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"55\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"8279\",\"DateReported\":\"2021-03-31\",\"DateOpened\":\"2019-12-13\",\"DateClosed\":\"2021-03-10\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"03-21\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-21\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-21\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-20\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-20\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-20\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-20\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-20\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"56\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Commercial Vehicle Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"212634\",\"Open\":\"No\",\"SanctionAmount\":\"299630\",\"LastPaymentDate\":\"2020-12-14\",\"DateReported\":\"2020-12-31\",\"DateOpened\":\"2019-03-19\",\"DateClosed\":\"2020-12-15\",\"Reason\":\"Closed Account\",\"InterestRate\":\"24.000\",\"RepaymentTenure\":\"32\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"12-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-19\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-19\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"57\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Commercial Vehicle Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"40805\",\"Open\":\"No\",\"SanctionAmount\":\"40000\",\"LastPaymentDate\":\"2020-12-14\",\"DateReported\":\"2020-12-31\",\"DateOpened\":\"2020-08-24\",\"DateClosed\":\"2020-12-14\",\"Reason\":\"Closed Account\",\"InterestRate\":\"14.000\",\"RepaymentTenure\":\"48\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"12-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"58\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Business Loan - Secured\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2020-08-24\",\"DateReported\":\"2020-09-30\",\"DateOpened\":\"2019-07-30\",\"DateClosed\":\"2020-09-15\",\"Reason\":\"Closed Account\",\"InterestRate\":\"24.000\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"09-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"59\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"4000\",\"LastPaymentDate\":\"2020-05-15\",\"DateReported\":\"2020-08-31\",\"DateOpened\":\"2019-10-14\",\"DateClosed\":\"2020-06-19\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"6\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"60\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"55000\",\"LastPaymentDate\":\"2020-03-09\",\"DateReported\":\"2020-03-31\",\"DateOpened\":\"2019-03-08\",\"DateClosed\":\"2020-03-31\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"03-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"61\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"45000\",\"LastPaymentDate\":\"2020-03-25\",\"DateReported\":\"2020-03-31\",\"DateOpened\":\"2019-05-04\",\"DateClosed\":\"2020-03-31\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"03-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"62\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"3500\",\"LastPaymentDate\":\"2020-02-02\",\"DateReported\":\"2020-02-29\",\"DateOpened\":\"2020-01-02\",\"DateClosed\":\"2020-02-02\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"02-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-20\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"63\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"5000\",\"LastPaymentDate\":\"2020-01-01\",\"DateReported\":\"2020-01-31\",\"DateOpened\":\"2019-12-01\",\"DateClosed\":\"2020-01-01\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"01-20\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"64\",\"AccountNumber\":\"**********\",\"Institution\":\"BANK\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"LastPayment\":\"10093\",\"Open\":\"No\",\"SanctionAmount\":\"9521\",\"LastPaymentDate\":\"2019-11-02\",\"DateReported\":\"2019-11-30\",\"DateOpened\":\"2019-05-04\",\"DateClosed\":\"2019-11-12\",\"Reason\":\"Closed Account\",\"RepaymentTenure\":\"6\",\"InstallmentAmount\":\"0\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"65\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"5000\",\"LastPaymentDate\":\"2019-11-29\",\"DateReported\":\"2019-11-30\",\"DateOpened\":\"2019-11-01\",\"DateClosed\":\"2019-11-29\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"66\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"5000\",\"LastPaymentDate\":\"2019-10-31\",\"DateReported\":\"2019-10-31\",\"DateOpened\":\"2019-09-29\",\"DateClosed\":\"2019-10-31\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"10-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"67\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"4000\",\"LastPaymentDate\":\"2019-08-30\",\"DateReported\":\"2019-08-31\",\"DateOpened\":\"2019-08-12\",\"DateClosed\":\"2019-08-30\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"68\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"8000\",\"LastPaymentDate\":\"2019-07-04\",\"DateReported\":\"2019-07-31\",\"DateOpened\":\"2019-06-07\",\"DateClosed\":\"2019-07-04\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"69\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"24500\",\"LastPaymentDate\":\"2017-03-05\",\"DateReported\":\"2019-07-31\",\"DateOpened\":\"2016-04-30\",\"DateClosed\":\"2017-04-14\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"70\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"22000\",\"LastPaymentDate\":\"2017-09-29\",\"DateReported\":\"2019-07-31\",\"DateOpened\":\"2016-09-17\",\"DateClosed\":\"2017-10-19\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-16\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"71\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"15990\",\"LastPaymentDate\":\"2019-05-17\",\"DateReported\":\"2019-06-30\",\"DateOpened\":\"2018-11-19\",\"DateClosed\":\"2019-05-17\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-18\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-18\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"72\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"1000\",\"LastPaymentDate\":\"2019-06-14\",\"DateReported\":\"2019-06-30\",\"DateOpened\":\"2018-11-19\",\"DateClosed\":\"2019-06-26\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-18\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-18\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"73\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"5000\",\"LastPaymentDate\":\"2019-06-07\",\"DateReported\":\"2019-06-30\",\"DateOpened\":\"2019-05-17\",\"DateClosed\":\"2019-06-07\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"AssetClassification\":\"Standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"74\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"6299\",\"LastPaymentDate\":\"2017-03-04\",\"DateReported\":\"2019-05-22\",\"DateOpened\":\"2016-03-04\",\"DateClosed\":\"2017-03-05\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"75\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"16409\",\"LastPaymentDate\":\"2017-06-29\",\"DateReported\":\"2019-05-22\",\"DateOpened\":\"2016-11-13\",\"DateClosed\":\"2017-07-01\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"76\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"7000\",\"LastPaymentDate\":\"2018-06-30\",\"DateReported\":\"2019-05-22\",\"DateOpened\":\"2017-03-25\",\"DateClosed\":\"2018-07-01\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"05-19\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-19\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"77\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Commercial Vehicle Loan\",\"OwnershipType\":\"Guarantor\",\"Balance\":\"65994\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"260000\",\"LastPaymentDate\":\"2014-11-17\",\"DateReported\":\"2018-11-30\",\"DateOpened\":\"2012-06-29\",\"DateClosed\":\"2014-11-25\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-18\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-16\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-15\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-14\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"78\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Gold Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"2500\",\"DateReported\":\"2017-08-31\",\"DateOpened\":\"2017-07-25\",\"DateClosed\":\"2017-07-25\",\"Reason\":\"Closed Account\",\"AccountStatus\":\"Closed Account\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"08-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"79\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4199\",\"PastDueAmount\":\"4199\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-08\",\"RepaymentTenure\":\"2\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"360-539 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"80\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4342\",\"PastDueAmount\":\"4342\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-07\",\"RepaymentTenure\":\"3\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"360-539 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"81\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4199\",\"PastDueAmount\":\"4199\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-08\",\"RepaymentTenure\":\"2\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"360-539 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"82\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4202\",\"PastDueAmount\":\"4202\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-07\",\"RepaymentTenure\":\"2\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"360-539 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"83\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4202\",\"PastDueAmount\":\"4202\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-07\",\"RepaymentTenure\":\"2\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"360-539 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"84\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4062\",\"PastDueAmount\":\"4062\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-07\",\"RepaymentTenure\":\"1\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"540-719 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"540+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"85\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4062\",\"PastDueAmount\":\"4062\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-07\",\"RepaymentTenure\":\"1\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"540-719 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"540+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"86\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Short Term Personal Loan [Unsecured]\",\"OwnershipType\":\"Individual\",\"Balance\":\"4063\",\"PastDueAmount\":\"4063\",\"Open\":\"Yes\",\"SanctionAmount\":\"2000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-01-06\",\"RepaymentTenure\":\"1\",\"TermFrequency\":\"Bullet Payment\",\"AccountStatus\":\"540-719 days past due\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"540+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"360+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"180+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"120+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"90+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"87\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"14299\",\"PastDueAmount\":\"4816\",\"WriteOffAmount\":\"19308\",\"Open\":\"Yes\",\"SanctionAmount\":\"14300\",\"LastPaymentDate\":\"2024-06-12\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2022-05-03\",\"AccountStatus\":\"Charge Off/Written Off\",\"AssetClassification\":\"Doubtful\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"DBT\"},{\"key\":\"06-24\",\"PaymentStatus\":\"SUB\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SUB\"},{\"key\":\"05-24\",\"PaymentStatus\":\"SUB\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SUB\"},{\"key\":\"04-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-24\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-24\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"08-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"07-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"06-22\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"05-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]},{\"seq\":\"88\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Personal Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"5417\",\"PastDueAmount\":\"5600\",\"LastPayment\":\"766\",\"WriteOffAmount\":\"5600\",\"Open\":\"Yes\",\"SanctionAmount\":\"6000\",\"LastPaymentDate\":\"2024-06-10\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2022-10-13\",\"RepaymentTenure\":\"2\",\"InstallmentAmount\":\"3183\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Charge Off/Written Off\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-24\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-24\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"WOF\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"89\",\"AccountNumber\":\"**********\",\"Institution\":\"FINANCE\",\"AccountType\":\"Consumer Loan\",\"OwnershipType\":\"Individual\",\"Balance\":\"0\",\"PastDueAmount\":\"0\",\"Open\":\"No\",\"SanctionAmount\":\"64000\",\"DateReported\":\"2022-11-30\",\"DateOpened\":\"2019-09-18\",\"DateClosed\":\"2021-03-24\",\"Reason\":\"Closed Account\",\"TermFrequency\":\"Monthly\",\"AccountStatus\":\"Settled\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"11-22\",\"PaymentStatus\":\"SET\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-22\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-21\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-21\",\"PaymentStatus\":\"CLSD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-21\",\"PaymentStatus\":\"*\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-21\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-20\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-20\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-20\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"07-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"06-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"11-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"10-19\",\"PaymentStatus\":\"000\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"},{\"key\":\"09-19\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"*\"}]},{\"seq\":\"90\",\"AccountNumber\":\"**********\",\"Institution\":\"BANK\",\"AccountType\":\"Other\",\"OwnershipType\":\"Individual\",\"Balance\":\"5855\",\"PastDueAmount\":\"4032\",\"Open\":\"Yes\",\"SanctionAmount\":\"10000\",\"LastPaymentDate\":\"2024-07-01\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-09-27\",\"InterestRate\":\"10.65\",\"TermFrequency\":\"Monthly\",\"CollateralValue\":\"10000\",\"AccountStatus\":\"Sub-standard\",\"AssetClassification\":\"Sub-standard\",\"source\":\"INDIVIDUAL\",\"History48Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"SUB\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SUB\"},{\"key\":\"06-24\",\"PaymentStatus\":\"SUB\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"SUB\"},{\"key\":\"05-24\",\"PaymentStatus\":\"60+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"30+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"03-24\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"02-24\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"01-24\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"12-23\",\"PaymentStatus\":\"01+\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"11-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"10-23\",\"PaymentStatus\":\"STD\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"},{\"key\":\"09-23\",\"PaymentStatus\":\"NEW\",\"SuitFiledStatus\":\"*\",\"AssetClassificationStatus\":\"STD\"}]}],\"RetailAccountsSummary\":{\"NoOfAccounts\":\"90\",\"NoOfActiveAccounts\":\"23\",\"NoOfWriteOffs\":\"2\",\"TotalPastDue\":\"143335.00\",\"MostSevereStatusWithIn24Months\":\"WOF\",\"SingleHighestCredit\":\"0.00\",\"SingleHighestSanctionAmount\":\"135176.00\",\"TotalHighCredit\":\"0.00\",\"AverageOpenBalance\":\"13006.04\",\"SingleHighestBalance\":\"65994.00\",\"NoOfPastDueAccounts\":\"16\",\"NoOfZeroBalanceAccounts\":\"0\",\"RecentAccount\":\"Gold Loan on 20-11-2023\",\"OldestAccount\":\"Commercial Vehicle Loan on 29-06-2012\",\"TotalBalanceAmount\":\"299139.00\",\"TotalSanctionAmount\":\"432028.00\",\"TotalCreditLimit\":\"0.0\",\"TotalMonthlyPaymentAmount\":\"42292.00\"},\"ScoreDetails\":[{\"Type\":\"ERS\",\"Version\":\"4.0\",\"Name\":\"ERS4.0\",\"Value\":\"409\",\"ScoringElements\":[{\"type\":\"RES\",\"seq\":\"1\",\"code\":\"400\",\"Description\":\"Occurances of default account\"},{\"type\":\"RES\",\"seq\":\"2\",\"code\":\"402\",\"Description\":\"Overdue Amount\"},{\"type\":\"RES\",\"seq\":\"3\",\"code\":\"403\",\"Description\":\"Delinquency presence\"}]}],\"Enquiries\":[{\"seq\":\"1\",\"Institution\":\"FINANCE\",\"Date\":\"2024-07-04\",\"Time\":\"11:19\",\"RequestPurpose\":\"05\",\"Amount\":\"13149\"},{\"seq\":\"2\",\"Institution\":\"FINANCE\",\"Date\":\"2023-10-30\",\"Time\":\"11:44\",\"RequestPurpose\":\"05\",\"Amount\":\"5000\"},{\"seq\":\"3\",\"Institution\":\"FINANCE\",\"Date\":\"2023-07-26\",\"Time\":\"13:28\",\"RequestPurpose\":\"0E\"},{\"seq\":\"4\",\"Institution\":\"FINANCE\",\"Date\":\"2023-07-26\",\"Time\":\"13:27\",\"RequestPurpose\":\"0E\"},{\"seq\":\"5\",\"Institution\":\"FINANCE\",\"Date\":\"2023-06-02\",\"Time\":\"14:35\",\"RequestPurpose\":\"00\"},{\"seq\":\"6\",\"Institution\":\"FINANCE\",\"Date\":\"2023-05-22\",\"Time\":\"12:52\",\"RequestPurpose\":\"00\"},{\"seq\":\"7\",\"Institution\":\"FINANCE\",\"Date\":\"2023-05-22\",\"Time\":\"12:51\",\"RequestPurpose\":\"00\"},{\"seq\":\"8\",\"Institution\":\"FINANCE\",\"Date\":\"2023-03-01\",\"Time\":\"16:03\",\"RequestPurpose\":\"0E\"},{\"seq\":\"9\",\"Institution\":\"FINANCE\",\"Date\":\"2022-12-31\",\"Time\":\"11:31\",\"RequestPurpose\":\"05\",\"Amount\":\"30000\"},{\"seq\":\"10\",\"Institution\":\"FINANCE\",\"Date\":\"2022-10-19\",\"Time\":\"14:01\",\"RequestPurpose\":\"0E\"},{\"seq\":\"11\",\"Institution\":\"FINANCE\",\"Date\":\"2022-09-17\",\"Time\":\"10:48\",\"RequestPurpose\":\"0E\"},{\"seq\":\"12\",\"Institution\":\"FINANCE\",\"Date\":\"2022-09-17\",\"Time\":\"22:10\",\"RequestPurpose\":\"05\",\"Amount\":\"40000\"},{\"seq\":\"13\",\"Institution\":\"FINANCE\",\"Date\":\"2022-08-09\",\"Time\":\"23:46\",\"RequestPurpose\":\"00\",\"Amount\":\"0\"},{\"seq\":\"14\",\"Institution\":\"FINANCE\",\"Date\":\"2022-08-09\",\"Time\":\"15:17\",\"RequestPurpose\":\"05\",\"Amount\":\"15000\"},{\"seq\":\"15\",\"Institution\":\"FINANCE\",\"Date\":\"2022-08-02\",\"Time\":\"16:04\",\"RequestPurpose\":\"00\",\"Amount\":\"0\"},{\"seq\":\"16\",\"Institution\":\"FINANCE\",\"Date\":\"2022-06-30\",\"Time\":\"12:32\",\"RequestPurpose\":\"05\",\"Amount\":\"10000\"},{\"seq\":\"17\",\"Institution\":\"FINANCE\",\"Date\":\"2022-06-23\",\"Time\":\"14:00\",\"RequestPurpose\":\"00\",\"Amount\":\"0\"},{\"seq\":\"18\",\"Institution\":\"FINANCE\",\"Date\":\"2022-04-23\",\"Time\":\"08:18\",\"RequestPurpose\":\"05\",\"Amount\":\"300000\"},{\"seq\":\"19\",\"Institution\":\"FINANCE\",\"Date\":\"2017-03-27\",\"Time\":\"12:20\",\"RequestPurpose\":\"06\"}],\"EnquirySummary\":{\"Purpose\":\"ALL\",\"Total\":\"21\",\"Past30Days\":\"0\",\"Past12Months\":\"2\",\"Past24Months\":\"12\",\"Recent\":\"2024-07-04\"},\"OtherKeyInd\":{\"AgeOfOldestTrade\":\"146\",\"NumberOfOpenTrades\":\"17\",\"AllLinesEVERWritten\":\"2.22\",\"AllLinesEVERWrittenIn9Months\":\"1\",\"AllLinesEVERWrittenIn6Months\":\"2\"},\"RecentActivities\":{\"AccountsDeliquent\":\"15\",\"AccountsOpened\":\"0\",\"TotalInquiries\":\"1\",\"AccountsUpdated\":\"18\"}}},{\"InquiryResponseHeader\":{\"CustomerCode\":\"ZFXO\",\"CustRefField\":\"1212\",\"ReportOrderNO\":\"1533455618\",\"ProductCode\":[\"MCS\"],\"SuccessCode\":\"1\",\"Date\":\"2024-08-29\",\"Time\":\"12:09:10\",\"HitCode\":\"10\",\"CustomerName\":\"ZFXO\"},\"InquiryRequestInfo\":{\"InquiryPurpose\":\"Other\",\"FirstName\":\"SAVITHIRI\",\"InquiryAddresses\":[{\"seq\":\"1\",\"AddressType\":[\"H\"],\"AddressLine1\":\"51 SAKTHI NAGAR COIMBATORE\",\"State\":\"TN        \",\"Postal\":\"641015\"}],\"InquiryPhones\":[{\"seq\":\"1\",\"PhoneType\":[\"M\"],\"Number\":\"9500770546\"}],\"IDDetails\":[{\"seq\":\"1\",\"IDType\":\"T\",\"IDValue\":\"GSNPS5215J\",\"Source\":\"Inquiry\"},{\"seq\":\"3\",\"IDType\":\"V\",\"IDValue\":\"RTG1027267\",\"Source\":\"Inquiry\"}],\"DOB\":\"1979-10-11\",\"MFIDetails\":{\"FamilyDetails\":[{\"seq\":\"1\",\"AdditionalNameType\":\"K02\"},{\"seq\":\"2\",\"AdditionalNameType\":\"K01\"}]}},\"Score\":[{\"Type\":\"ERS\",\"Version\":\"4.0\"}],\"CIRReportData\":{\"IDAndContactInfo\":{\"PersonalInfo\":{\"Name\":{\"FullName\":\"SAVITHIRI\"},\" AliasName\":{\"AliasName\":\"BAKKIYALAKSHMI S\",\"ReportedDate\":\"31-12-2017\"},\"DateOfBirth\":\"1979-10-11\",\"Gender\":\"Female\",\"Age\":{\"Age\":\"44\"},\"PlaceOfBirthInfo\":{},\"Occupation\":\"COMPANY\",\"MaritalStatus\":\"Married\"},\"IdentityInfo\":{\"VoterID\":[{\"seq\":\"1\",\"ReportedDate\":\"2023-08-31\",\"IdNumber\":\"RTG1027267\"},{\"seq\":\"1\",\"ReportedDate\":\"2022-05-31\",\"IdNumber\":\"WJB1153782\"}],\"NationalIDCard\":[{\"seq\":\"1\",\"ReportedDate\":\"2023-07-31\",\"IdNumber\":\"XXXXXXXXXXXX\"},{\"seq\":\"2\",\"ReportedDate\":\"2021-11-30\",\"IdNumber\":\"XXXXXXXXXXXX\"}],\"RationCard\":[{\"seq\":\"2\",\"ReportedDate\":\"2022-10-31\",\"IdNumber\":\"333758E11\"},{\"seq\":\"1\",\"ReportedDate\":\"2023-04-30\",\"IdNumber\":\"333757864839\"}],\"OtherId\":[{\"seq\":\"1\",\"ReportedDate\":\"2016-10-05\",\"IdNumber\":\"RTG1027267\"},{\"seq\":\"2\",\"ReportedDate\":\"2022-02-28\",\"IdNumber\":\"244339E11\"}]},\"AddressInfo\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"},{\"Seq\":\"2\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SAKTHI NAGAR  ONDIPUDUR VINAYAGAR KOVIL STREET  COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"},{\"Seq\":\"3\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SAKTHI NAGAR  ONDIPUDUR VINAYAGAR KOVIL STREET  COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"},{\"Seq\":\"4\",\"ReportedDate\":\"2024-03-31\",\"Address\":\"51 SAKTHI NAGAR  COIMBATORE TAMIL NADU 641015\",\"State\":\"TN\",\"Postal\":\"641015\"},{\"Seq\":\"5\",\"ReportedDate\":\"2023-10-31\",\"Address\":\"51 SAKTHI NAGAR  COIMBATORE TAMIL NADU 641015\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"PhoneInfo\":[{\"seq\":\"1\",\"typeCode\":\"H\",\"ReportedDate\":\"2021-10-31\",\"Number\":\"919487216546\"},{\"seq\":\"2\",\"typeCode\":\"H\",\"ReportedDate\":\"2016-11-21\",\"Number\":\"9488830411\"},{\"seq\":\"3\",\"typeCode\":\"M\",\"ReportedDate\":\"2016-01-06\",\"Number\":\"9488830411\"},{\"seq\":\"4\",\"typeCode\":\"M\",\"ReportedDate\":\"2021-10-31\",\"Number\":\"919487216546\"}]},\"MicrofinanceAccountDetails\":[{\"seq\":\"1\",\"branchIDMFI\":\"BSS BC 081\",\"kendraIDMFI\":\"549717\",\"id\":\"014413920\",\"AccountNumber\":\"014413920\",\"CurrentBalance\":\"1807\",\"Institution\":\"Kotak Mahindra Bank\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"ANIMAL HUSBANDRY\",\"Open\":\"Yes\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2024-06-27\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2022-09-08\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2022-09-07\",\"DateApplied\":\"2022-09-07\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Bi-weekly\",\"InstallmentAmount\":\"500\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Current Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2889815025\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SAKTHI NAGAR  ONDIPUDUR VINAYAGAR KOVIL STREET  COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-05-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"BSS BC 081\",\"KendraIDMFI\":\"549717\",\"History24Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"000\"},{\"key\":\"05-24\",\"PaymentStatus\":\"000\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"2\",\"branchIDMFI\":\"BSS BC 081\",\"kendraIDMFI\":\"549717\",\"id\":\"017822291\",\"AccountNumber\":\"017822291\",\"CurrentBalance\":\"12281\",\"Institution\":\"Kotak Mahindra Bank\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"36000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"ANIMAL HUSBANDRY\",\"Open\":\"Yes\",\"SanctionAmount\":\"36000\",\"LastPaymentDate\":\"2024-06-27\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2023-02-09\",\"LoanCycleID\":\"2\",\"DateSanctioned\":\"2022-12-30\",\"DateApplied\":\"2022-12-30\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Bi-weekly\",\"InstallmentAmount\":\"900\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Current Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2890320904\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SAKTHI NAGAR  ONDIPUDUR VINAYAGAR KOVIL STREET  COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-05-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"BSS BC 081\",\"KendraIDMFI\":\"549717\",\"History24Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"000\"},{\"key\":\"05-24\",\"PaymentStatus\":\"000\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\"},{\"key\":\"02-23\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"3\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020277\",\"id\":\"0002IGN13436\",\"AccountNumber\":\"0002IGN13436\",\"CurrentBalance\":\"7992\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"50000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"VEGITABLE\",\"Open\":\"Yes\",\"SanctionAmount\":\"50000\",\"LastPaymentDate\":\"2024-06-28\",\"DateReported\":\"2024-06-30\",\"DateOpened\":\"2023-03-20\",\"LoanCycleID\":\"6\",\"DateSanctioned\":\"2023-03-17\",\"DateApplied\":\"2023-03-15\",\"AppliedAmount\":\"50000\",\"NoOfInstallments\":\"77\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"950\",\"KeyPerson\":{\"Name\":\"RICHEARD IMPRANCH MANAVALAN\",\"RelationType\":\"Son\",\"associationType\":\"K\"},\"AccountStatus\":\"Current Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"50000\",\"NumberOfMeetingsHeld\":\"299\",\"NumberOfMeetingsMissed\":\"271\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2666940496\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-06-30\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020277\",\"History24Months\":[{\"key\":\"06-24\",\"PaymentStatus\":\"000\"},{\"key\":\"05-24\",\"PaymentStatus\":\"000\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"4\",\"branchIDMFI\":\"81\",\"kendraIDMFI\":\"549717\",\"id\":\"14413920\",\"AccountNumber\":\"14413920\",\"CurrentBalance\":\"12111\",\"Institution\":\"BSS Microfinance Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"ANIMAL HUSBANDRY\",\"Open\":\"Yes\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2023-01-31\",\"DateReported\":\"2023-01-31\",\"DateOpened\":\"2022-09-08\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2022-09-07\",\"DateApplied\":\"2022-09-07\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Bi-weekly\",\"InstallmentAmount\":\"500\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Current Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2451311858\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2022-12-31\",\"Address\":\"51 SAKTHI NAGAR  ONDIPUDUR VINAYAGAR KOVIL STREET  COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2022-12-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"81\",\"KendraIDMFI\":\"549717\",\"History24Months\":[{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\"},{\"key\":\"04-22\",\"PaymentStatus\":\"*\"},{\"key\":\"03-22\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"5\",\"branchIDMFI\":\"ANRMAIN\",\"id\":\"103013A12302\",\"AccountNumber\":\"103013A12302\",\"CurrentBalance\":\"5630\",\"Institution\":\"SUBIKSHAM WOMENS WELFARE FOUNDATION\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"AGRICULTURE\",\"Open\":\"Yes\",\"SanctionAmount\":\"20000\",\"DateReported\":\"2022-05-31\",\"DateOpened\":\"2021-10-11\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2021-10-11\",\"DateApplied\":\"2021-10-11\",\"AppliedAmount\":\"20000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"570\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Current Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"O\",\"InsurancePolicyAmount\":\"20000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI\",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2112308881\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"UYR0009233\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"RationCard\":[{\"IdNumber\":\"13G0300403\"}],\"OtherId\":[{\"IdNumber\":\"13G0300403\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2022-05-31\",\"Address\":\"51 SARAVANAMPATTI COIMBATORE 641015\",\"State\":\"TN\",\"Postal\":\"641653\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2022-05-31\",\"Number\":\"9486035920\"},{\"seq\":\"1\",\"ReportedDate\":\"2022-03-31\",\"Number\":\"8300018977\"}]},\"BranchIDMFI\":\"ANRMAIN\",\"History24Months\":[{\"key\":\"05-22\",\"PaymentStatus\":\"000\"},{\"key\":\"04-22\",\"PaymentStatus\":\"01+\"},{\"key\":\"03-22\",\"PaymentStatus\":\"01+\"},{\"key\":\"02-22\",\"PaymentStatus\":\"000\"},{\"key\":\"01-22\",\"PaymentStatus\":\"000\"},{\"key\":\"12-21\",\"PaymentStatus\":\"000\"},{\"key\":\"11-21\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"6\",\"branchIDMFI\":\"80107\",\"kendraIDMFI\":\"4745285\",\"id\":\"200062210239388\",\"AccountNumber\":\"200062210239388\",\"CurrentBalance\":\"29818\",\"Institution\":\"IDFC Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"6020\",\"DisbursedAmount\":\"70000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"AGRI-MARGINAL FARMER\",\"Open\":\"Yes\",\"SanctionAmount\":\"70000\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2023-03-07\",\"LoanCycleID\":\"1\",\"AppliedAmount\":\"70000\",\"NoOfInstallments\":\"104\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"860\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"30-59 days past due\",\"DaysPastDue\":\"45\",\"MaxDaysPastDue\":\"45\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2646107478\",\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-07-15\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  A LAMELU OUT UPPILIPALAYAM\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-07-15\",\"Number\":\"919487216546\"},{\"seq\":\"1\",\"ReportedDate\":\"2024-07-15\",\"Number\":\"919487216546\"}]},\"BranchIDMFI\":\"80107\",\"KendraIDMFI\":\"4745285\",\"History24Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"30+\"},{\"key\":\"06-24\",\"PaymentStatus\":\"01+\"},{\"key\":\"05-24\",\"PaymentStatus\":\"000\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\"},{\"key\":\"03-23\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"7\",\"branchIDMFI\":\"KARUMATHAMPATTY MML\",\"kendraIDMFI\":\"ML0180S02C0302\",\"id\":\"1101800101155995\",\"AccountNumber\":\"1101800101155995\",\"CurrentBalance\":\"5087\",\"Institution\":\"Muthoot Microfin Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"6240\",\"DisbursedAmount\":\"40000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"SERVICES\",\"Open\":\"Yes\",\"SanctionAmount\":\"40000\",\"LastPaymentDate\":\"2023-11-25\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2022-02-05\",\"LoanCycleID\":\"1\",\"DateApplied\":\"2022-02-05\",\"AppliedAmount\":\"40000\",\"NoOfInstallments\":\"24\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"2080\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"180-359 days past due\",\"DaysPastDue\":\"231\",\"MaxDaysPastDue\":\"231\",\"TypeOfInsurance\":\"C\",\"InsurancePolicyAmount\":\"40000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2187858769\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SAKTHI NAGAR  VINAYAKAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH 641015\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-05-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"KARUMATHAMPATTY MML\",\"KendraIDMFI\":\"ML0180S02C0302\",\"History24Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"06-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"05-24\",\"PaymentStatus\":\"120+\"},{\"key\":\"04-24\",\"PaymentStatus\":\"120+\"},{\"key\":\"03-24\",\"PaymentStatus\":\"90+\"},{\"key\":\"02-24\",\"PaymentStatus\":\"60+\"},{\"key\":\"01-24\",\"PaymentStatus\":\"30+\"},{\"key\":\"12-23\",\"PaymentStatus\":\"01+\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"8\",\"branchIDMFI\":\"1169\",\"kendraIDMFI\":\"512112\",\"id\":\"5206752\",\"AccountNumber\":\"5206752\",\"CurrentBalance\":\"0\",\"Institution\":\"Belstar Investment Fin\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"40000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"SERVICES\",\"Open\":\"No\",\"SanctionAmount\":\"40000\",\"LastPaymentDate\":\"2024-05-23\",\"DateReported\":\"2024-05-31\",\"DateOpened\":\"2022-06-17\",\"DateClosed\":\"2024-05-23\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2022-06-17\",\"AppliedAmount\":\"40000\",\"NoOfInstallments\":\"23\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"40000\",\"NumberOfMeetingsHeld\":\"12\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI\",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2544883260\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-03-31\",\"Address\":\"51 SAKTHI NAGAR  COIMBATORE TAMIL NADU 641015\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-03-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"1169\",\"KendraIDMFI\":\"512112\",\"History24Months\":[{\"key\":\"05-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"000\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"000\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"000\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"9\",\"branchIDMFI\":\"13\",\"kendraIDMFI\":\"1948\",\"id\":\"12344156680\",\"AccountNumber\":\"12344156680\",\"CurrentBalance\":\"0\",\"Institution\":\"SUBIKSHAM WOMENS WELFARE FOUNDATION\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"SHG LOAN-AGRI\",\"Open\":\"No\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2022-08-12\",\"DateReported\":\"2024-05-31\",\"DateOpened\":\"2021-10-11\",\"DateClosed\":\"2022-08-12\",\"Reason\":\"Closed Account\",\"DateSanctioned\":\"2021-10-11\",\"DateApplied\":\"2021-10-11\",\"AppliedAmount\":\"20000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Wife\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"NumberOfMeetingsHeld\":\"52\",\"NumberOfMeetingsMissed\":\"10\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI\",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2531777704\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"RationCard\":[{\"IdNumber\":\"333758E11\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-05-31\",\"Address\":\"51 SARAVANAMPATTI COIMBATORE 641015 TAMIL NADU INDIA\",\"State\":\"TN\",\"Postal\":\"641031\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-05-31\",\"Number\":\"9486035920\"}]},\"BranchIDMFI\":\"13\",\"KendraIDMFI\":\"1948\",\"History24Months\":[{\"key\":\"05-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"03-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"01-24\",\"PaymentStatus\":\"*\"},{\"key\":\"12-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-23\",\"PaymentStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"*\"},{\"key\":\"09-23\",\"PaymentStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"*\"},{\"key\":\"07-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"05-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"03-23\",\"PaymentStatus\":\"*\"},{\"key\":\"02-23\",\"PaymentStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"CLSD\"}]},{\"seq\":\"10\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020277\",\"id\":\"0002MPN01567\",\"AccountNumber\":\"0002MPN01567\",\"CurrentBalance\":\"0\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"10000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"THATCHES\",\"Open\":\"No\",\"SanctionAmount\":\"10000\",\"LastPaymentDate\":\"2024-05-31\",\"DateReported\":\"2024-05-31\",\"DateOpened\":\"2023-07-31\",\"DateClosed\":\"2024-05-31\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"7\",\"DateSanctioned\":\"2023-07-28\",\"DateApplied\":\"2023-07-28\",\"AppliedAmount\":\"10000\",\"NoOfInstallments\":\"40\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"RICHEARD IMPRANCH MANAVALAN\",\"RelationType\":\"Son\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"10000\",\"NumberOfMeetingsHeld\":\"295\",\"NumberOfMeetingsMissed\":\"526\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2860590575\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-04-30\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020277\",\"History24Months\":[{\"key\":\"05-24\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"04-24\",\"PaymentStatus\":\"000\"},{\"key\":\"03-24\",\"PaymentStatus\":\"000\"},{\"key\":\"02-24\",\"PaymentStatus\":\"000\"},{\"key\":\"01-24\",\"PaymentStatus\":\"000\"},{\"key\":\"12-23\",\"PaymentStatus\":\"000\"},{\"key\":\"11-23\",\"PaymentStatus\":\"000\"},{\"key\":\"10-23\",\"PaymentStatus\":\"000\"},{\"key\":\"09-23\",\"PaymentStatus\":\"*\"},{\"key\":\"08-23\",\"PaymentStatus\":\"000\"},{\"key\":\"07-23\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"11\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020277\",\"id\":\"0002MPL010625\",\"AccountNumber\":\"0002MPL010625\",\"CurrentBalance\":\"0\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"10000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"FISHERIES\",\"Open\":\"No\",\"SanctionAmount\":\"10000\",\"LastPaymentDate\":\"2023-07-31\",\"DateReported\":\"2023-07-31\",\"DateOpened\":\"2022-10-31\",\"DateClosed\":\"2023-07-31\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"5\",\"DateSanctioned\":\"2022-10-29\",\"DateApplied\":\"2022-10-29\",\"AppliedAmount\":\"10000\",\"NoOfInstallments\":\"40\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"RICHEARD IMPRANCH MANAVALAN\",\"RelationType\":\"Son\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"10000\",\"NumberOfMeetingsHeld\":\"254\",\"NumberOfMeetingsMissed\":\"543\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2531927218\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2023-01-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020277\",\"History24Months\":[{\"key\":\"07-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"06-23\",\"PaymentStatus\":\"000\"},{\"key\":\"05-23\",\"PaymentStatus\":\"000\"},{\"key\":\"04-23\",\"PaymentStatus\":\"*\"},{\"key\":\"03-23\",\"PaymentStatus\":\"000\"},{\"key\":\"02-23\",\"PaymentStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"12\",\"branchIDMFI\":\"1897\",\"kendraIDMFI\":\"1258100075\",\"id\":\"52210080156734\",\"AccountNumber\":\"52210080156734\",\"CurrentBalance\":\"0\",\"Institution\":\"Bandhan Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"30000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"BUFFALO BUSINESS\",\"Open\":\"No\",\"SanctionAmount\":\"30000\",\"LastPaymentDate\":\"2022-11-01\",\"DateReported\":\"2023-04-30\",\"DateOpened\":\"2021-12-24\",\"DateClosed\":\"2022-11-01\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"AppliedAmount\":\"30000\",\"NoOfInstallments\":\"48\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"SUNDARAM\",\"RelationType\":\"Other\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2151770788\",\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2022-12-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2022-12-31\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"1897\",\"KendraIDMFI\":\"1258100075\",\"History24Months\":[{\"key\":\"04-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"03-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"01-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-22\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-22\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"000\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\"},{\"key\":\"03-22\",\"PaymentStatus\":\"000\"},{\"key\":\"02-22\",\"PaymentStatus\":\"000\"},{\"key\":\"01-22\",\"PaymentStatus\":\"000\"},{\"key\":\"12-21\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"13\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020277\",\"id\":\"0002IGL050644\",\"AccountNumber\":\"0002IGL050644\",\"CurrentBalance\":\"0\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"40000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"AGRIOTH\",\"Open\":\"No\",\"SanctionAmount\":\"40000\",\"LastPaymentDate\":\"2023-03-20\",\"DateReported\":\"2023-03-31\",\"DateOpened\":\"2022-06-08\",\"DateClosed\":\"2023-03-20\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"4\",\"DateSanctioned\":\"2022-06-07\",\"DateApplied\":\"2022-06-03\",\"AppliedAmount\":\"40000\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"RICHEARD IMPRANCH MANAVALAN\",\"RelationType\":\"Son\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"40000\",\"NumberOfMeetingsHeld\":\"237\",\"NumberOfMeetingsMissed\":\"441\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2356291886\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2023-01-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020277\",\"History24Months\":[{\"key\":\"03-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"14\",\"branchIDMFI\":\"13\",\"kendraIDMFI\":\"1948\",\"id\":\"1-23441-56680\",\"AccountNumber\":\"1-23441-56680\",\"CurrentBalance\":\"0\",\"Institution\":\"SUBIKSHAM WOMENS WELFARE FOUNDATION\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"SHG LOAN-PURPOSEOFLO\",\"Open\":\"No\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2022-08-12\",\"DateReported\":\"2023-03-31\",\"DateOpened\":\"2021-10-11\",\"DateClosed\":\"2022-08-12\",\"Reason\":\"Closed Account\",\"DateSanctioned\":\"2021-10-11\",\"DateApplied\":\"2021-10-11\",\"AppliedAmount\":\"20000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI\",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2567719002\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"RationCard\":[{\"IdNumber\":\"333758E11\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2022-09-30\",\"Address\":\"51 SARAVANAMPATTI COIMBATORE 641015 TAMIL NADU INDIA\",\"State\":\"TN\",\"Postal\":\"641670\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2022-09-30\",\"Number\":\"9486035920\"}]},\"BranchIDMFI\":\"13\",\"KendraIDMFI\":\"1948\",\"History24Months\":[{\"key\":\"03-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"*\"},{\"key\":\"01-23\",\"PaymentStatus\":\"*\"},{\"key\":\"12-22\",\"PaymentStatus\":\"*\"},{\"key\":\"11-22\",\"PaymentStatus\":\"*\"},{\"key\":\"10-22\",\"PaymentStatus\":\"*\"},{\"key\":\"09-22\",\"PaymentStatus\":\"CLSD\"}]},{\"seq\":\"15\",\"branchIDMFI\":\"80107\",\"kendraIDMFI\":\"4745285\",\"id\":\"GV8010718914131\",\"AccountNumber\":\"GV8010718914131\",\"CurrentBalance\":\"0\",\"Institution\":\"IDFC Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"49314\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"ALLIED AGRI-BEE-KEEP\",\"Open\":\"No\",\"SanctionAmount\":\"49314\",\"DateReported\":\"2023-03-31\",\"DateOpened\":\"2021-12-27\",\"DateClosed\":\"2023-03-07\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"AppliedAmount\":\"49314\",\"NoOfInstallments\":\"104\",\"RepaymentTenure\":\"Others\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2151010634\",\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2023-01-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH UPPILIPALAYAM COIMBATORE UPPILIPALAYAM\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2023-03-15\",\"Number\":\"919487216546\"},{\"seq\":\"1\",\"ReportedDate\":\"2023-03-15\",\"Number\":\"919487216546\"}]},\"BranchIDMFI\":\"80107\",\"KendraIDMFI\":\"4745285\",\"History24Months\":[{\"key\":\"03-23\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-23\",\"PaymentStatus\":\"000\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"000\"},{\"key\":\"10-22\",\"PaymentStatus\":\"000\"},{\"key\":\"09-22\",\"PaymentStatus\":\"000\"},{\"key\":\"08-22\",\"PaymentStatus\":\"*\"},{\"key\":\"07-22\",\"PaymentStatus\":\"000\"},{\"key\":\"06-22\",\"PaymentStatus\":\"000\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\"},{\"key\":\"03-22\",\"PaymentStatus\":\"000\"},{\"key\":\"02-22\",\"PaymentStatus\":\"000\"},{\"key\":\"01-22\",\"PaymentStatus\":\"000\"},{\"key\":\"12-21\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"16\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020277\",\"id\":\"0002IGL050643\",\"AccountNumber\":\"0002IGL050643\",\"CurrentBalance\":\"0\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"35000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"TOILARING\",\"Open\":\"No\",\"SanctionAmount\":\"35000\",\"LastPaymentDate\":\"2022-06-08\",\"DateReported\":\"2022-07-30\",\"DateOpened\":\"2021-09-30\",\"DateClosed\":\"2022-06-08\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"3\",\"DateSanctioned\":\"2021-09-29\",\"DateApplied\":\"2021-09-29\",\"AppliedAmount\":\"35000\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"RICHEARD IMPRANCH MANAVALAN\",\"RelationType\":\"Son\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"35000\",\"NumberOfMeetingsHeld\":\"202\",\"NumberOfMeetingsMissed\":\"77\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2063544418\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2022-07-30\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  SULUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020277\",\"History24Months\":[{\"key\":\"07-22\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"06-22\",\"PaymentStatus\":\"*\"},{\"key\":\"05-22\",\"PaymentStatus\":\"000\"},{\"key\":\"04-22\",\"PaymentStatus\":\"000\"},{\"key\":\"03-22\",\"PaymentStatus\":\"*\"},{\"key\":\"02-22\",\"PaymentStatus\":\"*\"},{\"key\":\"01-22\",\"PaymentStatus\":\"000\"},{\"key\":\"12-21\",\"PaymentStatus\":\"*\"},{\"key\":\"11-21\",\"PaymentStatus\":\"*\"},{\"key\":\"10-21\",\"PaymentStatus\":\"*\"},{\"key\":\"09-21\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"17\",\"branchIDMFI\":\"80107\",\"kendraIDMFI\":\"2598087\",\"id\":\"GV801077172725\",\"AccountNumber\":\"GV801077172725\",\"CurrentBalance\":\"0\",\"Institution\":\"IDFC Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"39201\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"BUSINESS-OTHERS\",\"Open\":\"No\",\"SanctionAmount\":\"39201\",\"DateReported\":\"2021-11-30\",\"DateOpened\":\"2019-06-04\",\"DateClosed\":\"2021-11-12\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"AppliedAmount\":\"39201\",\"NoOfInstallments\":\"106\",\"RepaymentTenure\":\"Others\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"1435564081\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2021-10-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH COIMBATORE UPPILIPALAYAM\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2021-10-31\",\"Number\":\"919487216546\"},{\"seq\":\"1\",\"ReportedDate\":\"2021-10-31\",\"Number\":\"919487216546\"}]},\"BranchIDMFI\":\"80107\",\"KendraIDMFI\":\"2598087\",\"History24Months\":[{\"key\":\"11-21\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-21\",\"PaymentStatus\":\"30+\"},{\"key\":\"09-21\",\"PaymentStatus\":\"60+\"},{\"key\":\"08-21\",\"PaymentStatus\":\"30+\"},{\"key\":\"07-21\",\"PaymentStatus\":\"30+\"},{\"key\":\"06-21\",\"PaymentStatus\":\"30+\"},{\"key\":\"05-21\",\"PaymentStatus\":\"01+\"},{\"key\":\"04-21\",\"PaymentStatus\":\"000\"},{\"key\":\"03-21\",\"PaymentStatus\":\"01+\"},{\"key\":\"02-21\",\"PaymentStatus\":\"01+\"},{\"key\":\"01-21\",\"PaymentStatus\":\"01+\"},{\"key\":\"12-20\",\"PaymentStatus\":\"01+\"},{\"key\":\"11-20\",\"PaymentStatus\":\"01+\"},{\"key\":\"10-20\",\"PaymentStatus\":\"01+\"},{\"key\":\"09-20\",\"PaymentStatus\":\"000\"},{\"key\":\"08-20\",\"PaymentStatus\":\"000\"},{\"key\":\"07-20\",\"PaymentStatus\":\"000\"},{\"key\":\"06-20\",\"PaymentStatus\":\"000\"},{\"key\":\"05-20\",\"PaymentStatus\":\"000\"},{\"key\":\"04-20\",\"PaymentStatus\":\"000\"},{\"key\":\"03-20\",\"PaymentStatus\":\"*\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\"},{\"key\":\"01-20\",\"PaymentStatus\":\"000\"},{\"key\":\"12-19\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"18\",\"branchIDMFI\":\"2\",\"kendraIDMFI\":\"00020250\",\"id\":\"0002IGL050642\",\"AccountNumber\":\"0002IGL050642\",\"CurrentBalance\":\"0\",\"Institution\":\"Arise Investments and Capital Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"30000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"FURNITURE\",\"Open\":\"No\",\"SanctionAmount\":\"30000\",\"LastPaymentDate\":\"2020-03-04\",\"DateReported\":\"2020-05-11\",\"DateOpened\":\"2019-03-08\",\"DateClosed\":\"2020-03-04\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"2\",\"DateSanctioned\":\"2019-03-07\",\"DateApplied\":\"2019-03-06\",\"AppliedAmount\":\"30000\",\"NoOfInstallments\":\"51\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"30000\",\"NumberOfMeetingsHeld\":\"102\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"1597160929\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2020-03-02\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"2\",\"KendraIDMFI\":\"00020250\",\"History24Months\":[{\"key\":\"05-20\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"04-20\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"03-20\",\"PaymentStatus\":\"000\"},{\"key\":\"02-20\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"19\",\"branchIDMFI\":\"62\",\"kendraIDMFI\":\"2598087\",\"id\":\"GV804513283012\",\"AccountNumber\":\"GV804513283012\",\"CurrentBalance\":\"0\",\"Institution\":\"IDFC Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"30000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"BUSINESS-PURCHASEOFS\",\"Open\":\"No\",\"SanctionAmount\":\"30000\",\"DateReported\":\"2019-06-30\",\"DateOpened\":\"2018-08-04\",\"DateClosed\":\"2019-06-04\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"AppliedAmount\":\"30000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Others\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"1423464691\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2019-05-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH COIMBATORE UPPILIPALAYAM\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2019-05-31\",\"Number\":\"919487216546\"},{\"seq\":\"1\",\"ReportedDate\":\"2019-05-31\",\"Number\":\"919487216546\"}]},\"BranchIDMFI\":\"62\",\"KendraIDMFI\":\"2598087\",\"History24Months\":[{\"key\":\"06-19\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"05-19\",\"PaymentStatus\":\"000\"},{\"key\":\"04-19\",\"PaymentStatus\":\"000\"},{\"key\":\"03-19\",\"PaymentStatus\":\"000\"},{\"key\":\"02-19\",\"PaymentStatus\":\"000\"},{\"key\":\"01-19\",\"PaymentStatus\":\"000\"},{\"key\":\"12-18\",\"PaymentStatus\":\"000\"},{\"key\":\"11-18\",\"PaymentStatus\":\"000\"},{\"key\":\"10-18\",\"PaymentStatus\":\"000\"},{\"key\":\"09-18\",\"PaymentStatus\":\"000\"},{\"key\":\"08-18\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"20\",\"branchIDMFI\":\"62\",\"kendraIDMFI\":\"2598087\",\"id\":\"GV804511787917-\",\"AccountNumber\":\"GV804511787917-\",\"CurrentBalance\":\"0\",\"Institution\":\"IDFC Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"BUSINESS-PURCHASEOFS\",\"Open\":\"No\",\"SanctionAmount\":\"20000\",\"LastPaymentDate\":\"2018-11-30\",\"DateReported\":\"2018-11-30\",\"DateOpened\":\"2017-12-05\",\"DateClosed\":\"2018-08-04\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2017-11-16\",\"DateApplied\":\"2017-11-15\",\"AppliedAmount\":\"20000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"SUNDARAM S\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN DHANDAPANI\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"19866\",\"NumberOfMeetingsHeld\":\"98\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"1077061979\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2017-12-31\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH UPPILIPALAYAM COIMBATORE COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2017-12-31\",\"Number\":\"9487216546\"}]},\"BranchIDMFI\":\"62\",\"KendraIDMFI\":\"2598087\",\"History24Months\":[{\"key\":\"11-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"08-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"07-18\",\"PaymentStatus\":\"000\"},{\"key\":\"06-18\",\"PaymentStatus\":\"000\"},{\"key\":\"05-18\",\"PaymentStatus\":\"000\"},{\"key\":\"04-18\",\"PaymentStatus\":\"000\"},{\"key\":\"03-18\",\"PaymentStatus\":\"000\"},{\"key\":\"02-18\",\"PaymentStatus\":\"000\"},{\"key\":\"01-18\",\"PaymentStatus\":\"000\"},{\"key\":\"12-17\",\"PaymentStatus\":\"01+\"}]},{\"seq\":\"21\",\"branchIDMFI\":\"1186\",\"kendraIDMFI\":\"11860038\",\"id\":\"118671000005651\",\"AccountNumber\":\"118671000005651\",\"CurrentBalance\":\"0\",\"Institution\":\"Ujjivan Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"8000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"02\",\"Open\":\"No\",\"SanctionAmount\":\"8000\",\"LastPaymentDate\":\"2011-12-22\",\"DateReported\":\"2018-08-02\",\"DateOpened\":\"2010-12-22\",\"DateClosed\":\"2011-12-22\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2010-12-20\",\"AppliedAmount\":\"8000\",\"NoOfInstallments\":\"12\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALLAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALLAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHRI  MANAVALLAN      \",\"MFIDOB\":\"1982-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"103351418\",\"MFIIdentification\":{\"RationCard\":[{\"IdNumber\":\"12G0023941\"}],\"OtherId\":[{\"IdNumber\":\"12 G 0023941\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2018-08-02\",\"Address\":\"NO 25 A BHARATHI NAGAR  UPPILIPALAYAM SINGANALLUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"1186\",\"KendraIDMFI\":\"11860038\",\"History24Months\":[{\"key\":\"08-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"07-18\",\"PaymentStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-16\",\"PaymentStatus\":\"CLSD\"}]},{\"seq\":\"22\",\"branchIDMFI\":\"1186\",\"kendraIDMFI\":\"11860038\",\"id\":\"1186712000001791\",\"AccountNumber\":\"1186712000001791\",\"CurrentBalance\":\"0\",\"Institution\":\"Ujjivan Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"5000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"02\",\"Open\":\"No\",\"SanctionAmount\":\"5000\",\"LastPaymentDate\":\"2012-08-14\",\"DateReported\":\"2018-08-02\",\"DateOpened\":\"2011-08-17\",\"DateClosed\":\"2012-08-14\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2011-08-03\",\"DateApplied\":\"2011-08-03\",\"AppliedAmount\":\"5000\",\"NoOfInstallments\":\"12\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALLAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALLAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHRI  MANAVALLAN      \",\"MFIDOB\":\"1982-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"165119367\",\"MFIIdentification\":{\"RationCard\":[{\"IdNumber\":\"12G0023941\"}],\"OtherId\":[{\"IdNumber\":\"12G0023941\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2018-08-02\",\"Address\":\"NO 25 A BHARATHI NAGAR  UPPILIPALAYAM SINGANALLUR\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"1186\",\"KendraIDMFI\":\"11860038\",\"History24Months\":[{\"key\":\"08-18\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"07-18\",\"PaymentStatus\":\"*\"},{\"key\":\"06-18\",\"PaymentStatus\":\"*\"},{\"key\":\"05-18\",\"PaymentStatus\":\"*\"},{\"key\":\"04-18\",\"PaymentStatus\":\"*\"},{\"key\":\"03-18\",\"PaymentStatus\":\"*\"},{\"key\":\"02-18\",\"PaymentStatus\":\"*\"},{\"key\":\"01-18\",\"PaymentStatus\":\"*\"},{\"key\":\"12-17\",\"PaymentStatus\":\"*\"},{\"key\":\"11-17\",\"PaymentStatus\":\"*\"},{\"key\":\"10-17\",\"PaymentStatus\":\"*\"},{\"key\":\"09-17\",\"PaymentStatus\":\"*\"},{\"key\":\"08-17\",\"PaymentStatus\":\"*\"},{\"key\":\"07-17\",\"PaymentStatus\":\"*\"},{\"key\":\"06-17\",\"PaymentStatus\":\"*\"},{\"key\":\"05-17\",\"PaymentStatus\":\"*\"},{\"key\":\"04-17\",\"PaymentStatus\":\"*\"},{\"key\":\"03-17\",\"PaymentStatus\":\"*\"},{\"key\":\"02-17\",\"PaymentStatus\":\"*\"},{\"key\":\"01-17\",\"PaymentStatus\":\"*\"},{\"key\":\"12-16\",\"PaymentStatus\":\"*\"},{\"key\":\"11-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-16\",\"PaymentStatus\":\"CLSD\"}]},{\"seq\":\"23\",\"branchIDMFI\":\"3018\",\"kendraIDMFI\":\"SELVI HOUSE30180002\",\"id\":\"30188150154730\",\"AccountNumber\":\"30188150154730\",\"CurrentBalance\":\"0\",\"Institution\":\"Jana Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"35000\",\"LoanPurpose\":\"A - BUYING EBUSINESS\",\"Open\":\"No\",\"SanctionAmount\":\"35000\",\"DateReported\":\"2017-03-05\",\"DateOpened\":\"2014-11-24\",\"DateClosed\":\"2016-12-31\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"4\",\"DateSanctioned\":\"2014-11-23\",\"AppliedAmount\":\"35000\",\"NoOfInstallments\":\"24\",\"RepaymentTenure\":\"Monthly\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"35000\",\"NumberOfMeetingsHeld\":\"1\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"541526098\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}],\"OtherId\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2016-11-21\",\"Address\":\"25-A BHARATHI NAGAR  1ST ST  UPPILIPALAYAM  NEAR TEMPLE COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2016-11-21\",\"Number\":\"9488830411\"},{\"seq\":\"1\",\"ReportedDate\":\"2016-01-06\",\"Number\":\"9488830411\"}]},\"BranchIDMFI\":\"3018\",\"KendraIDMFI\":\"SELVI HOUSE30180002\",\"History24Months\":[{\"key\":\"03-17\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"02-17\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"01-17\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-16\",\"PaymentStatus\":\"000\"},{\"key\":\"11-16\",\"PaymentStatus\":\"000\"},{\"key\":\"10-16\",\"PaymentStatus\":\"000\"},{\"key\":\"09-16\",\"PaymentStatus\":\"000\"},{\"key\":\"08-16\",\"PaymentStatus\":\"000\"},{\"key\":\"07-16\",\"PaymentStatus\":\"000\"},{\"key\":\"06-16\",\"PaymentStatus\":\"000\"},{\"key\":\"05-16\",\"PaymentStatus\":\"000\"},{\"key\":\"04-16\",\"PaymentStatus\":\"000\"},{\"key\":\"03-16\",\"PaymentStatus\":\"000\"},{\"key\":\"02-16\",\"PaymentStatus\":\"000\"},{\"key\":\"01-16\",\"PaymentStatus\":\"000\"},{\"key\":\"12-15\",\"PaymentStatus\":\"000\"},{\"key\":\"11-15\",\"PaymentStatus\":\"000\"},{\"key\":\"10-15\",\"PaymentStatus\":\"000\"},{\"key\":\"09-15\",\"PaymentStatus\":\"000\"},{\"key\":\"08-15\",\"PaymentStatus\":\"000\"},{\"key\":\"07-15\",\"PaymentStatus\":\"000\"},{\"key\":\"06-15\",\"PaymentStatus\":\"000\"},{\"key\":\"05-15\",\"PaymentStatus\":\"000\"},{\"key\":\"04-15\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"24\",\"branchIDMFI\":\"1204\",\"kendraIDMFI\":\"30947\",\"id\":\"CBECBSCBS30947040311\",\"AccountNumber\":\"CBECBSCBS30947040311\",\"CurrentBalance\":\"0\",\"Institution\":\"Asirvad Microfinance Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"15000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"0006\",\"Open\":\"No\",\"SanctionAmount\":\"15000\",\"LastPaymentDate\":\"2016-07-13\",\"DateReported\":\"2016-10-06\",\"DateOpened\":\"2015-01-09\",\"DateClosed\":\"2016-07-13\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2015-01-09\",\"AppliedAmount\":\"15000\",\"NoOfInstallments\":\"18\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Wife\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"15000\",\"NumberOfMeetingsHeld\":\"12\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"557583861\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"OtherId\":[{\"IdNumber\":\"244339E11\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2016-10-06\",\"Address\":\"25 BHARTHI NAGAR  VARADHARAJAPURAM COIMBATORE -641015 COIMBATORE SOUTH\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"1204\",\"KendraIDMFI\":\"30947\",\"History24Months\":[{\"key\":\"10-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"08-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"07-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"06-16\",\"PaymentStatus\":\"000\"},{\"key\":\"05-16\",\"PaymentStatus\":\"000\"},{\"key\":\"04-16\",\"PaymentStatus\":\"000\"},{\"key\":\"03-16\",\"PaymentStatus\":\"000\"},{\"key\":\"02-16\",\"PaymentStatus\":\"000\"},{\"key\":\"01-16\",\"PaymentStatus\":\"000\"},{\"key\":\"12-15\",\"PaymentStatus\":\"000\"},{\"key\":\"11-15\",\"PaymentStatus\":\"000\"},{\"key\":\"10-15\",\"PaymentStatus\":\"*\"},{\"key\":\"09-15\",\"PaymentStatus\":\"*\"},{\"key\":\"08-15\",\"PaymentStatus\":\"000\"},{\"key\":\"07-15\",\"PaymentStatus\":\"*\"},{\"key\":\"06-15\",\"PaymentStatus\":\"000\"},{\"key\":\"05-15\",\"PaymentStatus\":\"000\"},{\"key\":\"04-15\",\"PaymentStatus\":\"000\"},{\"key\":\"03-15\",\"PaymentStatus\":\"000\"},{\"key\":\"02-15\",\"PaymentStatus\":\"NEW\"},{\"key\":\"01-15\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"25\",\"branchIDMFI\":\"1204\",\"kendraIDMFI\":\"30947\",\"id\":\"CBECBSCBS30947040322\",\"AccountNumber\":\"CBECBSCBS30947040322\",\"CurrentBalance\":\"0\",\"Institution\":\"Asirvad Microfinance Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"1500\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"0006\",\"Open\":\"No\",\"SanctionAmount\":\"1500\",\"LastPaymentDate\":\"2016-07-13\",\"DateReported\":\"2016-10-06\",\"DateOpened\":\"2015-01-09\",\"DateClosed\":\"2016-07-13\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"2\",\"DateSanctioned\":\"2015-01-09\",\"AppliedAmount\":\"1500\",\"NoOfInstallments\":\"18\",\"RepaymentTenure\":\"Monthly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Wife\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"1500\",\"NumberOfMeetingsHeld\":\"12\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"557583866\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"OtherId\":[{\"IdNumber\":\"244339E11\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2016-10-06\",\"Address\":\"25 BHARTHI NAGAR  VARADHARAJAPURAM COIMBATORE -641015 COIMBATORE SOUTH\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"1204\",\"KendraIDMFI\":\"30947\",\"History24Months\":[{\"key\":\"10-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"08-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"07-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"06-16\",\"PaymentStatus\":\"000\"},{\"key\":\"05-16\",\"PaymentStatus\":\"000\"},{\"key\":\"04-16\",\"PaymentStatus\":\"000\"},{\"key\":\"03-16\",\"PaymentStatus\":\"000\"},{\"key\":\"02-16\",\"PaymentStatus\":\"000\"},{\"key\":\"01-16\",\"PaymentStatus\":\"000\"},{\"key\":\"12-15\",\"PaymentStatus\":\"000\"},{\"key\":\"11-15\",\"PaymentStatus\":\"000\"},{\"key\":\"10-15\",\"PaymentStatus\":\"*\"},{\"key\":\"09-15\",\"PaymentStatus\":\"*\"},{\"key\":\"08-15\",\"PaymentStatus\":\"000\"},{\"key\":\"07-15\",\"PaymentStatus\":\"*\"},{\"key\":\"06-15\",\"PaymentStatus\":\"000\"},{\"key\":\"05-15\",\"PaymentStatus\":\"000\"},{\"key\":\"04-15\",\"PaymentStatus\":\"000\"},{\"key\":\"03-15\",\"PaymentStatus\":\"000\"},{\"key\":\"02-15\",\"PaymentStatus\":\"NEW\"},{\"key\":\"01-15\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"26\",\"branchIDMFI\":\"PEELAMEDU\",\"kendraIDMFI\":\"PLED/CENTRE-201\",\"id\":\"PLED2717\",\"AccountNumber\":\"PLED2717\",\"CurrentBalance\":\"0\",\"Institution\":\"Muthoot Fincorp Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"12000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"A35\",\"Open\":\"No\",\"SanctionAmount\":\"12000\",\"LastPaymentDate\":\"2015-09-23\",\"DateReported\":\"2016-01-06\",\"DateOpened\":\"2014-09-26\",\"DateClosed\":\"2015-11-04\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateApplied\":\"2014-09-26\",\"AppliedAmount\":\"12000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"C\",\"InsurancePolicyAmount\":\"12000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI\",\"MFIDOB\":\"1980-04-12\",\"MFIGender\":\"Female\",\"MemberId\":\"522430096\",\"MFIIdentification\":{\"NationalIDCard\":[{\"IdNumber\":\"XXXXXXXXXXXX\"}],\"OtherId\":[{\"IdNumber\":\"244339036384\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2016-01-06\",\"Address\":\"24 SAKTHI VINAYAGAR KOVIL 2 STREET  UPPLIPALAYAM\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2016-01-06\",\"Number\":\"9487216546\"}]},\"BranchIDMFI\":\"PEELAMEDU\",\"KendraIDMFI\":\"PLED/CENTRE-201\",\"History24Months\":[{\"key\":\"01-16\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-15\",\"PaymentStatus\":\"000\"},{\"key\":\"09-15\",\"PaymentStatus\":\"000\"},{\"key\":\"08-15\",\"PaymentStatus\":\"000\"},{\"key\":\"07-15\",\"PaymentStatus\":\"000\"},{\"key\":\"06-15\",\"PaymentStatus\":\"000\"},{\"key\":\"05-15\",\"PaymentStatus\":\"000\"},{\"key\":\"04-15\",\"PaymentStatus\":\"000\"},{\"key\":\"03-15\",\"PaymentStatus\":\"000\"},{\"key\":\"02-15\",\"PaymentStatus\":\"000\"},{\"key\":\"01-15\",\"PaymentStatus\":\"000\"},{\"key\":\"12-14\",\"PaymentStatus\":\"000\"},{\"key\":\"11-14\",\"PaymentStatus\":\"000\"},{\"key\":\"10-14\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"27\",\"branchIDMFI\":\"SINGANALLUR-RAJALAKSHMI MILLS\",\"kendraIDMFI\":\"SGNL/C39\",\"id\":\"SGNL484SC\",\"AccountNumber\":\"SGNL484SC\",\"CurrentBalance\":\"0\",\"Institution\":\"Muthoot Fincorp Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"15000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"A35\",\"Open\":\"No\",\"SanctionAmount\":\"15000\",\"LastPaymentDate\":\"2014-10-15\",\"DateReported\":\"2015-10-08\",\"DateOpened\":\"2013-10-18\",\"DateClosed\":\"2015-07-31\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"2\",\"DateApplied\":\"2013-10-18\",\"AppliedAmount\":\"15000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"C\",\"InsurancePolicyAmount\":\"15000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"M  SAVITHRI      \",\"MFIDOB\":\"1980-08-08\",\"MFIGender\":\"Female\",\"MemberId\":\"654992609\",\"MFIIdentification\":{\"OtherId\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2015-08-05\",\"Address\":\"25 A BHARATHI NAGAR  1ST STREET  UPPILIPALAYAM PO  COIMBATORE 641015\",\"State\":\"TN\",\"Postal\":\"641005\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2015-08-05\",\"Number\":\"9787196649\"}]},\"BranchIDMFI\":\"SINGANALLUR-RAJALAKSHMI MILLS\",\"KendraIDMFI\":\"SGNL/C39\",\"History24Months\":[{\"key\":\"10-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"08-15\",\"PaymentStatus\":\"CLSD\"}]},{\"seq\":\"28\",\"branchIDMFI\":\"3018\",\"kendraIDMFI\":\"SHYMALA HOUSE30180002\",\"id\":\"30188150016471\",\"AccountNumber\":\"30188150016471\",\"CurrentBalance\":\"0\",\"Institution\":\"Jana Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"30000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"INCOME GENERATING\",\"Open\":\"No\",\"SanctionAmount\":\"30000\",\"DateReported\":\"2015-02-10\",\"DateOpened\":\"2012-11-07\",\"DateClosed\":\"2014-11-30\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"3\",\"DateSanctioned\":\"2012-11-05\",\"AppliedAmount\":\"30000\",\"NoOfInstallments\":\"24\",\"RepaymentTenure\":\"Monthly\",\"KeyPerson\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"K\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"30000\",\"NumberOfMeetingsHeld\":\"1\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  M      \",\"MFIDOB\":\"1979-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"304836933\",\"MFIIdentification\":{\"VoterID\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2014-12-07\",\"Address\":\"25-A BHARATHI NAGAR  1ST ST  UPPILIPALAYAM  NEAR TEMPLE COIMBATORE\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2014-12-07\",\"Number\":\"9488830411\"}]},\"BranchIDMFI\":\"3018\",\"KendraIDMFI\":\"SHYMALA HOUSE30180002\",\"History24Months\":[{\"key\":\"02-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"01-15\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-14\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-14\",\"PaymentStatus\":\"000\"},{\"key\":\"10-14\",\"PaymentStatus\":\"000\"},{\"key\":\"09-14\",\"PaymentStatus\":\"000\"},{\"key\":\"08-14\",\"PaymentStatus\":\"000\"},{\"key\":\"07-14\",\"PaymentStatus\":\"000\"},{\"key\":\"06-14\",\"PaymentStatus\":\"000\"},{\"key\":\"05-14\",\"PaymentStatus\":\"000\"},{\"key\":\"04-14\",\"PaymentStatus\":\"000\"},{\"key\":\"03-14\",\"PaymentStatus\":\"000\"},{\"key\":\"02-14\",\"PaymentStatus\":\"000\"},{\"key\":\"01-14\",\"PaymentStatus\":\"000\"},{\"key\":\"12-13\",\"PaymentStatus\":\"000\"},{\"key\":\"11-13\",\"PaymentStatus\":\"000\"},{\"key\":\"10-13\",\"PaymentStatus\":\"000\"},{\"key\":\"09-13\",\"PaymentStatus\":\"000\"},{\"key\":\"08-13\",\"PaymentStatus\":\"000\"},{\"key\":\"07-13\",\"PaymentStatus\":\"000\"},{\"key\":\"06-13\",\"PaymentStatus\":\"000\"},{\"key\":\"05-13\",\"PaymentStatus\":\"000\"},{\"key\":\"04-13\",\"PaymentStatus\":\"000\"},{\"key\":\"03-13\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"29\",\"branchIDMFI\":\"SINGANALLOOR\",\"kendraIDMFI\":\"SGNL/CENTRE-82\",\"id\":\"SGNL1120\",\"AccountNumber\":\"SGNL1120\",\"CurrentBalance\":\"0\",\"Institution\":\"Muthoot Fincorp Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"12000\",\"LoanCategory\":\"JLG Group\",\"LoanPurpose\":\"A35\",\"Open\":\"No\",\"SanctionAmount\":\"12000\",\"LastPaymentDate\":\"2013-10-15\",\"DateReported\":\"2014-01-01\",\"DateOpened\":\"2012-11-24\",\"DateClosed\":\"2013-10-18\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateApplied\":\"2012-11-24\",\"AppliedAmount\":\"12000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"C\",\"InsurancePolicyAmount\":\"12000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"M  SAVITHRI      \",\"MFIDOB\":\"1980-08-08\",\"MFIGender\":\"Female\",\"MemberId\":\"298370087\",\"MFIIdentification\":{\"RationCard\":[{\"IdNumber\":\"12/G/0023941\"}],\"OtherId\":[{\"IdNumber\":\"12/G/0023941\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2014-01-01\",\"Address\":\"25/A BHARATHI NAGAR  1ST STREET  UPPILIPALAYAM PO  COIMBATORE-641015\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2014-01-01\",\"Number\":\"9787196649\"}]},\"BranchIDMFI\":\"SINGANALLOOR\",\"KendraIDMFI\":\"SGNL/CENTRE-82\",\"History24Months\":[{\"key\":\"01-14\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-13\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-13\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-13\",\"PaymentStatus\":\"000\"},{\"key\":\"09-13\",\"PaymentStatus\":\"000\"},{\"key\":\"08-13\",\"PaymentStatus\":\"000\"},{\"key\":\"07-13\",\"PaymentStatus\":\"000\"},{\"key\":\"06-13\",\"PaymentStatus\":\"000\"},{\"key\":\"05-13\",\"PaymentStatus\":\"000\"},{\"key\":\"04-13\",\"PaymentStatus\":\"000\"},{\"key\":\"03-13\",\"PaymentStatus\":\"000\"},{\"key\":\"02-13\",\"PaymentStatus\":\"000\"},{\"key\":\"01-13\",\"PaymentStatus\":\"000\"},{\"key\":\"12-12\",\"PaymentStatus\":\"NEW\"}]},{\"seq\":\"30\",\"branchIDMFI\":\"SINGANALLOOR\",\"kendraIDMFI\":\"SGNL/CENTRE-65\",\"id\":\"SGNL856\",\"AccountNumber\":\"SGNL856\",\"CurrentBalance\":\"0\",\"Institution\":\"Muthoot Fincorp Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"10000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"A35\",\"Open\":\"No\",\"SanctionAmount\":\"10000\",\"LastPaymentDate\":\"2012-11-07\",\"DateReported\":\"2013-02-08\",\"DateOpened\":\"2011-11-19\",\"DateClosed\":\"2012-11-24\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateApplied\":\"2011-11-19\",\"AppliedAmount\":\"10000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"0\",\"KeyPerson\":{},\"Nominee\":{\"Name\":\"MANAVALAN\",\"RelationType\":\"Husband\",\"associationType\":\"N\"},\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"C\",\"InsurancePolicyAmount\":\"10000\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"M  SAVITHRI      \",\"MFIDOB\":\"1980-08-08\",\"MFIGender\":\"Female\",\"MemberId\":\"263340687\",\"MFIIdentification\":{\"RationCard\":[{\"IdNumber\":\"12/G/0023941\"}],\"OtherId\":[{\"IdNumber\":\"12/G/0023941\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2013-01-04\",\"Address\":\"25/A BHARATHI NAGAR  1ST STREET  UPPILIPALAYAM PO  COIMBATORE-641015\",\"State\":\"TN\",\"Postal\":\"641015\"}]},\"BranchIDMFI\":\"SINGANALLOOR\",\"KendraIDMFI\":\"SGNL/CENTRE-65\",\"History24Months\":[{\"key\":\"02-13\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"01-13\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-12\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-12\",\"PaymentStatus\":\"000\"},{\"key\":\"10-12\",\"PaymentStatus\":\"*\"},{\"key\":\"09-12\",\"PaymentStatus\":\"*\"},{\"key\":\"08-12\",\"PaymentStatus\":\"*\"},{\"key\":\"07-12\",\"PaymentStatus\":\"*\"},{\"key\":\"06-12\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"31\",\"branchIDMFI\":\"3018\",\"kendraIDMFI\":\"ARUKANI HOUSE30180002\",\"id\":\"30188020023200\",\"AccountNumber\":\"30188020023200\",\"CurrentBalance\":\"0\",\"Institution\":\"Jana Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"20000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"INCOME GENERATING\",\"Open\":\"No\",\"SanctionAmount\":\"20000\",\"DateReported\":\"2013-01-08\",\"DateOpened\":\"2011-04-18\",\"DateClosed\":\"2012-10-31\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"2\",\"DateSanctioned\":\"2011-04-11\",\"AppliedAmount\":\"20000\",\"NoOfInstallments\":\"18\",\"RepaymentTenure\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"20000\",\"NumberOfMeetingsHeld\":\"1\",\"NumberOfMeetingsMissed\":\"0\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN  MANAVALAN    \",\"MFIDOB\":\"1979-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"135600007\",\"MFIIdentification\":{\"OtherId\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2012-11-30\",\"Address\":\"25-ABHARATHI NAGAR 1ST ST  UPPILIPALAYAMNEAR CHIPS STALLCOIMBATORE\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2012-11-30\",\"Number\":\"9488830411\"},{\"seq\":\"1\",\"ReportedDate\":\"2012-10-22\",\"Number\":\"9677984543\"}]},\"BranchIDMFI\":\"3018\",\"KendraIDMFI\":\"ARUKANI HOUSE30180002\",\"History24Months\":[{\"key\":\"01-13\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"12-12\",\"PaymentStatus\":\"*\"},{\"key\":\"11-12\",\"PaymentStatus\":\"000\"},{\"key\":\"10-12\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-12\",\"PaymentStatus\":\"000\"},{\"key\":\"08-12\",\"PaymentStatus\":\"000\"},{\"key\":\"07-12\",\"PaymentStatus\":\"000\"},{\"key\":\"06-12\",\"PaymentStatus\":\"000\"},{\"key\":\"05-12\",\"PaymentStatus\":\"000\"},{\"key\":\"04-12\",\"PaymentStatus\":\"000\"},{\"key\":\"03-12\",\"PaymentStatus\":\"000\"},{\"key\":\"02-12\",\"PaymentStatus\":\"000\"},{\"key\":\"01-12\",\"PaymentStatus\":\"000\"},{\"key\":\"12-11\",\"PaymentStatus\":\"*\"},{\"key\":\"11-11\",\"PaymentStatus\":\"000\"},{\"key\":\"10-11\",\"PaymentStatus\":\"000\"},{\"key\":\"09-11\",\"PaymentStatus\":\"*\"},{\"key\":\"08-11\",\"PaymentStatus\":\"000\"},{\"key\":\"07-11\",\"PaymentStatus\":\"000\"},{\"key\":\"06-11\",\"PaymentStatus\":\"*\"},{\"key\":\"05-11\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"32\",\"branchIDMFI\":\"3018\",\"kendraIDMFI\":\"KALIKUTTY HOUSE30180003\",\"id\":\"30188140007430\",\"AccountNumber\":\"30188140007430\",\"CurrentBalance\":\"0\",\"Institution\":\"Jana Small Finance Bank Ltd\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"0\",\"DisbursedAmount\":\"10000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"OTHERS\",\"Open\":\"No\",\"SanctionAmount\":\"10000\",\"DateReported\":\"2011-12-20\",\"DateOpened\":\"2010-02-27\",\"DateClosed\":\"2011-02-17\",\"Reason\":\"Closed Account\",\"LoanCycleID\":\"1\",\"DateSanctioned\":\"2010-02-26\",\"AppliedAmount\":\"10000\",\"NoOfInstallments\":\"12\",\"RepaymentTenure\":\"Monthly\",\"AccountStatus\":\"Closed Account\",\"DaysPastDue\":\"0\",\"MaxDaysPastDue\":\"0\",\"TypeOfInsurance\":\"L\",\"InsurancePolicyAmount\":\"10000\",\"NumberOfMeetingsHeld\":\"1\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1981-01-01\",\"MFIGender\":\"Female\",\"MemberId\":\"135600509\",\"MFIIdentification\":{\"OtherId\":[{\"IdNumber\":\"RTG1027267\"}]},\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2011-12-20\",\"Address\":\"BHARATHI NAGAR 1ST STREET  UPPILIPALAYAM\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2011-12-20\",\"Number\":\"9677984543\"}]},\"BranchIDMFI\":\"3018\",\"KendraIDMFI\":\"KALIKUTTY HOUSE30180003\",\"History24Months\":[{\"key\":\"12-11\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"11-11\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"10-11\",\"PaymentStatus\":\"CLSD\"},{\"key\":\"09-11\",\"PaymentStatus\":\"*\"},{\"key\":\"08-11\",\"PaymentStatus\":\"000\"},{\"key\":\"07-11\",\"PaymentStatus\":\"000\"},{\"key\":\"06-11\",\"PaymentStatus\":\"*\"},{\"key\":\"05-11\",\"PaymentStatus\":\"000\"}]},{\"seq\":\"33\",\"branchIDMFI\":\"1897\",\"kendraIDMFI\":\"1258100075\",\"id\":\"52220063504002\",\"AccountNumber\":\"52220063504002\",\"CurrentBalance\":\"25771\",\"Institution\":\"Bandhan Bank Limited\",\"InstitutionType\":\"MFI\",\"PastDueAmount\":\"25771\",\"DisbursedAmount\":\"45000\",\"LoanCategory\":\"JLG Individual\",\"LoanPurpose\":\"TAILORING AND WEAVIN\",\"Open\":\"Yes\",\"SanctionAmount\":\"45000\",\"LastPaymentDate\":\"2023-11-04\",\"DateReported\":\"2024-07-31\",\"DateOpened\":\"2022-11-14\",\"LoanCycleID\":\"10\",\"AppliedAmount\":\"45000\",\"NoOfInstallments\":\"52\",\"RepaymentTenure\":\"Weekly\",\"InstallmentAmount\":\"1040\",\"KeyPerson\":{\"Name\":\"SUNDARAM\",\"RelationType\":\"Father\",\"associationType\":\"K\"},\"AccountStatus\":\"360-539 days past due\",\"DaysPastDue\":\"426\",\"MaxDaysPastDue\":\"426\",\"source\":\"INDIVIDUAL\",\"AdditionalMFIDetails\":{\"MFIClientFullname\":\"SAVITHIRI  MANAVALAN      \",\"MFIDOB\":\"1979-10-11\",\"MFIGender\":\"Female\",\"MemberId\":\"2551747883\",\"MFIAddress\":[{\"Seq\":\"1\",\"ReportedDate\":\"2024-06-30\",\"Address\":\"51 SAKTHI NAGER VINAYAGAR KOVIL STREET  ALAMELU OUT COIMBATORE SOUTH COIMBATORE\",\"State\":\"TN\",\"Postal\":\"641015\"}],\"MFIPhones\":[{\"seq\":\"0\",\"ReportedDate\":\"2024-06-30\",\"Number\":\"9500770546\"}]},\"BranchIDMFI\":\"1897\",\"KendraIDMFI\":\"1258100075\",\"History24Months\":[{\"key\":\"07-24\",\"PaymentStatus\":\"360+\"},{\"key\":\"06-24\",\"PaymentStatus\":\"360+\"},{\"key\":\"05-24\",\"PaymentStatus\":\"360+\"},{\"key\":\"04-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"03-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"02-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"01-24\",\"PaymentStatus\":\"180+\"},{\"key\":\"12-23\",\"PaymentStatus\":\"180+\"},{\"key\":\"11-23\",\"PaymentStatus\":\"*\"},{\"key\":\"10-23\",\"PaymentStatus\":\"120+\"},{\"key\":\"09-23\",\"PaymentStatus\":\"120+\"},{\"key\":\"08-23\",\"PaymentStatus\":\"90+\"},{\"key\":\"07-23\",\"PaymentStatus\":\"60+\"},{\"key\":\"06-23\",\"PaymentStatus\":\"30+\"},{\"key\":\"05-23\",\"PaymentStatus\":\"60+\"},{\"key\":\"04-23\",\"PaymentStatus\":\"30+\"},{\"key\":\"03-23\",\"PaymentStatus\":\"01+\"},{\"key\":\"02-23\",\"PaymentStatus\":\"01+\"},{\"key\":\"01-23\",\"PaymentStatus\":\"000\"},{\"key\":\"12-22\",\"PaymentStatus\":\"000\"},{\"key\":\"11-22\",\"PaymentStatus\":\"NEW\"}]}],\"MicrofinanceAccountsSummary\":{\"id\":\"INDIVIDUAL\",\"NoOfActiveAccounts\":\"8\",\"TotalPastDue\":\"38031.00\",\"NoOfPastDueAccounts\":\"3\",\"RecentAccount\":\"MicroFinance Personal Loan on 31-07-2023\",\"TotalBalanceAmount\":\"100497.00\",\"TotalMonthlyPaymentAmount\":\"19560.00\",\"TotalWrittenOffAmount\":\"0.00\",\"Id\":\"INDIVIDUAL\"},\"IncomeDetails\":[{\"occupation\":\"AGRI-MARGINAL FARMER (UP TO 2.5 ACRE)\",\"monthlyIncome\":\"15000\",\"monthlyExpense\":\"0\",\"seq\":3,\"reportedDate\":\"2024-07-15\"},{\"occupation\":\"BUSINESS SELF EMPLOYED\",\"monthlyIncome\":\"10000\",\"seq\":2,\"reportedDate\":\"2024-06-30\"},{\"monthlyIncome\":\"12000\",\"seq\":6,\"reportedDate\":\"2024-06-30\"},{\"monthlyIncome\":\"15000\",\"monthlyExpense\":\"1500\",\"povertyIndex\":\"0\",\"seq\":1,\"reportedDate\":\"2024-05-31\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"40000\",\"povertyIndex\":\"1\",\"seq\":4,\"reportedDate\":\"2024-05-31\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"40000\",\"povertyIndex\":\"1\",\"seq\":5,\"reportedDate\":\"2024-05-31\"},{\"occupation\":\"COMPANY\",\"monthlyIncome\":\"10000\",\"monthlyExpense\":\"5000\",\"seq\":8,\"reportedDate\":\"2024-05-31\"},{\"monthlyIncome\":\"12000\",\"seq\":9,\"reportedDate\":\"2024-04-30\"},{\"occupation\":\"16\",\"monthlyIncome\":\"25000\",\"monthlyExpense\":\"16300\",\"seq\":7,\"reportedDate\":\"2024-03-31\"},{\"occupation\":\"AGRI-MARGINAL FARMER (UP TO 2.5 ACRE)\",\"monthlyIncome\":\"15000\",\"monthlyExpense\":\"0\",\"seq\":14,\"reportedDate\":\"2023-03-15\"},{\"seq\":10,\"reportedDate\":\"2023-01-31\"},{\"seq\":12,\"reportedDate\":\"2023-01-31\"},{\"occupation\":\"BUSINESS SELF EMPLOYED\",\"monthlyIncome\":\"375000\",\"seq\":11,\"reportedDate\":\"2022-12-31\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"8000\",\"povertyIndex\":\"1\",\"seq\":15,\"reportedDate\":\"2022-12-31\"},{\"occupation\":\"COMPANY\",\"monthlyIncome\":\"20000\",\"monthlyExpense\":\"10000\",\"seq\":13,\"reportedDate\":\"2022-09-30\"},{\"seq\":16,\"reportedDate\":\"2022-07-30\"},{\"occupation\":\"COMPANY\",\"monthlyIncome\":\"12500\",\"seq\":17,\"reportedDate\":\"2022-05-31\"},{\"occupation\":\"SERVICE-TRADER\",\"monthlyIncome\":\"9500\",\"monthlyExpense\":\"0\",\"seq\":18,\"reportedDate\":\"2021-10-31\"},{\"seq\":19,\"reportedDate\":\"2020-03-02\"},{\"occupation\":\"SERVICE-TRADER\",\"monthlyIncome\":\"9500\",\"monthlyExpense\":\"0\",\"seq\":20,\"reportedDate\":\"2019-05-31\"},{\"occupation\":\"CV\",\"monthlyIncome\":\"10000\",\"monthlyExpense\":\"5305\",\"seq\":22,\"reportedDate\":\"2018-08-02\"},{\"occupation\":\"CV\",\"monthlyIncome\":\"10000\",\"monthlyExpense\":\"5305\",\"seq\":23,\"reportedDate\":\"2018-08-02\"},{\"occupation\":\"SERVICE-TRADER\",\"monthlyIncome\":\"9500\",\"assetOwnership\":\"Y\",\"seq\":21,\"reportedDate\":\"2017-12-31\"},{\"occupation\":\"SALARIED\",\"monthlyIncome\":\"10000\",\"monthlyExpense\":\"5000\",\"assetOwnership\":\"Y\",\"seq\":24,\"reportedDate\":\"2016-11-21\"},{\"seq\":25,\"reportedDate\":\"2016-10-06\"},{\"seq\":26,\"reportedDate\":\"2016-10-06\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"7000\",\"monthlyExpense\":\"4700\",\"povertyIndex\":\"0\",\"seq\":27,\"reportedDate\":\"2016-01-06\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"8000\",\"monthlyExpense\":\"5500\",\"povertyIndex\":\"1\",\"seq\":28,\"reportedDate\":\"2015-08-05\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"10000\",\"assetOwnership\":\"Y\",\"seq\":29,\"reportedDate\":\"2014-12-07\"},{\"occupation\":\"Z03\",\"monthlyIncome\":\"8000\",\"monthlyExpense\":\"5500\",\"povertyIndex\":\"1\",\"seq\":30,\"reportedDate\":\"2014-01-01\"},{\"monthlyIncome\":\"9500\",\"monthlyExpense\":\"4900\",\"povertyIndex\":\"0\",\"seq\":31,\"reportedDate\":\"2013-01-04\"},{\"occupation\":\"Z02\",\"monthlyIncome\":\"10000\",\"monthlyExpense\":\"5000\",\"assetOwnership\":\"Y\",\"seq\":32,\"reportedDate\":\"2012-11-30\"},{\"occupation\":\"Z02\",\"monthlyIncome\":\"13000\",\"monthlyExpense\":\"9440\",\"assetOwnership\":\"Y\",\"seq\":33,\"reportedDate\":\"2011-12-20\"}],\"familyDetailsInfo\":{\"relatives\":[{\"AdditionalNameType\":\"Wife\",\"AdditionalName\":\"MANAVALAN\"},{\"AdditionalNameType\":\"Father\",\"AdditionalName\":\"MANAVALAN\"}]},\"ScoreDetails\":[{\"Type\":\"MRS\",\"Name\":\"Microfinance Risk Score\",\"Value\":\"300\",\"ScoringElements\":[{\"seq\":\"1\",\"code\":\"104\",\"Description\":\"Delinquency\"},{\"seq\":\"2\",\"code\":\"101\",\"Description\":\"Delinquency\"}]}],\"Enquiries\":[{\"seq\":\"1\",\"Institution\":\"Suryoday Small Finance Bank Limited\",\"Date\":\"2023-07-26\",\"Time\":\"13:28\",\"RequestPurpose\":\"0E\"},{\"seq\":\"2\",\"Institution\":\"Suryoday Small Finance Bank Limited\",\"Date\":\"2023-07-26\",\"Time\":\"13:27\",\"RequestPurpose\":\"0E\"},{\"seq\":\"3\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"Date\":\"2023-06-02\",\"Time\":\"14:35\",\"RequestPurpose\":\"00\"},{\"seq\":\"4\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"Date\":\"2023-05-22\",\"Time\":\"12:52\",\"RequestPurpose\":\"00\"},{\"seq\":\"5\",\"Institution\":\"Yogakshemam Loans Ltd.\",\"Date\":\"2023-05-22\",\"Time\":\"12:51\",\"RequestPurpose\":\"00\"},{\"seq\":\"6\",\"Institution\":\"IIFL SAMASTA FINANCE LIMITED\",\"Date\":\"2023-03-01\",\"Time\":\"16:03\",\"RequestPurpose\":\"0E\"},{\"seq\":\"7\",\"Institution\":\"Suryoday Small Finance Bank Limited\",\"Date\":\"2022-09-17\",\"Time\":\"10:48\",\"RequestPurpose\":\"0E\"},{\"seq\":\"8\",\"Institution\":\"SUBIKSHAM WOMENS WELFARE FOUNDATION\",\"Date\":\"2021-10-05\",\"Time\":\"18:53\",\"RequestPurpose\":\"3E\",\"Amount\":\"20000\"},{\"seq\":\"9\",\"Institution\":\"ESAF Small Finance Bank LTD\",\"Date\":\"2015-12-10\",\"Time\":\"10:20\",\"RequestPurpose\":\"0E\",\"Amount\":\"15000\"}],\"EnquirySummary\":{},\"OtherKeyInd\":{\"NumberOfOpenTrades\":\"8\"}}}]}}";

                var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

                // Deserialize JSON data to model
                //var model = JsonConvert.DeserializeObject<Equifax_Enquiry_Model>(jsonString);


                Error error = new Error();
                if (rootObject.Error != null)
                {
                    error = rootObject.Error;
                    string error_msg = error.ErrorDesc.ToString() + ": Message from Equifax Server.";

                    // Store the error message in TempData or ViewBag to display in the view
                    TempData["ErrorMessage"] = error_msg;

                    // Redirect to a view that can show the error message
                    return RedirectToAction("Equifax_Enquiry", "Equifax");
                }

                else
                {
                    /* Byte[] bytes2 = createEquifaxPDF(jsonstring);*/ // Function for creating Equifax PDF                   
                    Equifax_Enquiry_Model model1 = new Equifax_Enquiry_Model();

                    getjsondata(jsonstring, model1);


                    if (rootObject.CCRResponse.CIRReportDataLst[0].InquiryResponseHeader.HitCode == "00")
                    {
                        string mfi_score = "";
                        if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData != null)
                        {
                            if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails != null)
                            {
                                mfi_score = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails[0].Value;
                            }
                        }

                        try
                        {
                            // Setup SqlParameters
                            SqlParameter[] pr11 = new SqlParameter[4];
                            pr11[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                            pr11[0].Value = 10;
                            pr11[1] = new SqlParameter("@enquiry_id", SqlDbType.BigInt);
                            pr11[1].Value = data[0];
                            pr11[2] = new SqlParameter("@equifax", SqlDbType.VarChar, 50);
                            pr11[2].Value = "-2";
                            pr11[3] = new SqlParameter("@mfi_score", SqlDbType.VarChar, 50);
                            pr11[3].Value = mfi_score;

                            // Execute the stored procedure
                            dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr11);
                            dbconnect.Close();

                            // Set a success message in TempData
                            TempData["SuccessMessage"] = "Confirmed!! Score -1";

                            // Redirect to a view or return a view
                            return RedirectToAction("Equifax_Enquiry", "Equifax");
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions and set error message in TempData if needed
                            TempData["ErrorMessage"] = ex.Message;
                            return RedirectToAction("Equifax_Enquiry", "Equifax");
                        }


                    }
                    else
                    {

                        string equi_score = "", mfi_score = "";
                        if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData != null)
                        {
                            if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.ScoreDetails != null)
                            {
                                equi_score = rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.ScoreDetails[0].Value;
                            }
                        }

                        if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData != null)
                        {
                            if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails != null)
                            {
                                mfi_score = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails[0].Value;
                            }
                        }


                        SqlParameter[] pr12 = new SqlParameter[4];
                        pr12[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                        pr12[0].Value = 10;
                        pr12[1] = new SqlParameter("@enquiry_id", SqlDbType.BigInt);
                        pr12[1].Value = data[0];
                        pr12[2] = new SqlParameter("@equifax", SqlDbType.VarChar, 50);
                        pr12[2].Value = equi_score;
                        pr12[3] = new SqlParameter("@mfi_score", SqlDbType.VarChar, 50);
                        pr12[3].Value = mfi_score;
                        dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr12);
                        dbconnect.Close();


                    }
                    SqlParameter[] pr9 = new SqlParameter[4];
                    pr9[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr9[0].Value = 11;
                    pr9[1] = new SqlParameter("@enquiry_id", SqlDbType.BigInt);
                    pr9[1].Value = data[0];
                    pr9[2] = new SqlParameter("@data", SqlDbType.Binary);
                    pr9[2].Value = bytes2;
                    pr9[3] = new SqlParameter("@phone_no", SqlDbType.BigInt);
                    pr9[3].Value = Session["login_user"];
                    dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr9);
                    dbconnect.Close();


                    //TempData["SuccessMessage"] = "Operation successful!";
                    //return GeneratePdf(jsonstring);



                    // Return the PDF from the partial view without rendering the partial view
                    return new Rotativa.PartialViewAsPdf("EquifaxEnquiryPdfPartialView", model1)
                    {
                        FileName = "EquifaxEnquiry.pdf", // Optional: set the filename of the PDF
                    };




                    //return new Rotativa.PartialViewAsPdf("EquifaxEnquryPdfPartialView", model1)
                    //{
                    //    FileName = "EquifaxEnquiry.pdf" // Optional: set the filename of the PDF

                    //};



                    //TempData["SuccessMessage"] = "Operation successful!";
                    //TempData["ShowDownloadButton"] = true; // Set this flag to show the download button
                    //return PartialView("EquifaxEnquryPdfPartialView", model1);




                }


                //// ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Confirmed!!')", true);

                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "sucess", "sucess();", true);



            }
            else
            {
                //h_msg.Value = data[0];
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "error1", "alert('Pending Enquiry!!');", true);
                TempData["ErrorMessage"] = "Pending Enquiry!";
                return RedirectToAction("Equifax_Enquiry", "Equifax");
            }
        }


        private ActionResult GeneratePdf(string jsonString)
        {
            var pdfResult = new Rotativa.ViewAsPdf("EquifaxEnquiryPdfPartialView", jsonString)
            {
                FileName = "EquifaxEnquiry.pdf"
            };
            return pdfResult;
        }


        public async Task<string> StartAsyn(Rootrequestobject root_requestobject)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.Timeout = TimeSpan.FromMinutes(2);

                    var json = JsonConvert.SerializeObject(root_requestobject);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Log request for debugging
                    Console.WriteLine("Sending API Request: " + json);

                    HttpResponseMessage response = await client.PostAsync("https://ists.equifax.co.in/cir360service/cir360report", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorDetails = await response.Content.ReadAsStringAsync();
                        throw new Exception($"API call failed with status {response.StatusCode}: {errorDetails}");
                    }

                    // Read the response
                    string responseJson = await response.Content.ReadAsStringAsync();

                    // Log response for debugging
                    Console.WriteLine("API Response: " + responseJson);

                    return responseJson;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: " + ex.Message);
                    throw;
                }
            }
        }






        //public async Task<string> StartAsyn(Rootrequestobject root_requestobject)
        //{
        //    // Initialize HttpClient
        //    using (var client = new HttpClient())
        //    {
        //        // Set security protocol (if needed)
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        client.Timeout = TimeSpan.FromMinutes(1);

        //        // Serialize the request object to JSON
        //        var json = JsonConvert.SerializeObject(root_requestobject);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        // Send POST request
        //        HttpResponseMessage response = await client.PostAsync("https://ists.equifax.co.in/cir360service/cir360report", content);

        //        // Ensure successful response
        //        response.EnsureSuccessStatusCode();

        //        // Read response content as string
        //        string responseJson = await response.Content.ReadAsStringAsync();
        //        return responseJson;
        //    }
        //}

        //[HttpPost]
        //public ActionResult GenerateReport(string jsonString)
        //{
        //    // Deserialize the JSON string into your ReportModel class
        //    var reportModel = JsonConvert.DeserializeObject<Equifax_Enquiry_Model.ReportModel>(jsonString);

        //    // Return the partial view with the deserialized ReportModel
        //    return PartialView("EquifaxEnquiryPdfPartialView", reportModel);
        //}







        public void getjsondata(string jsonstring, Equifax_Enquiry_Model model)
        {
            try
            {
                var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

                if (rootObject.InquiryResponseHeader != null)
                {
                    ProcessInquiryResponseHeader(rootObject.InquiryResponseHeader, model);
                }

                if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData != null ||
                    rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData != null)
                {
                    Cirreportdata cirreportdata = rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData;

                    if (cirreportdata.IDAndContactInfo != null)
                    {
                        ProcessIdAndContactInfo(cirreportdata.IDAndContactInfo, model);

                        // Create a model instance
                        var modeladrs = new Equifax_Enquiry_Model();

                        // Process address information
                        ProcessAddressInfo(cirreportdata.IDAndContactInfo, modeladrs, model.ClientId);

                        // Store the address list in ViewData
                        ViewData["AddressList"] = modeladrs.AddressList;
                    }


                    /// Call the ProcessRetailAccountSummary method, passing the original jsonstring and model
                    ProcessRetailAccountSummary(jsonstring, model);
                    if (ViewBag.ReAccSumm != null)
                    {
                        // Do something with ViewBag.ReAccSumm if needed
                    }
                    // Assuming Re_accnt_summ is populated correctly in your controller
                    //DataTable Re_accnt_summ = new DataTable();
                    //// Populate DataTable (your existing logic)

                    //ViewBag.ReAccSumm = Re_accnt_summ;



                    // Assuming values are set correctly
                    string clientid = model.ClientId;
                    string reportOrderno = model.OrderNo;
                    string date = model.Date;
                    string time = model.Time;

                    InsertDataIntoDatabase(clientid, reportOrderno, date, time, model);

                    ProcessEquifaxRiskScoreData(jsonstring);

                    // Assuming you have a way to retrieve ViewData in the current context
                    if (ViewData["ScoreList"] != null)
                    {
                        model.ScoreList = (List<Scores>)ViewData["ScoreList"];
                    }


                    //ProcessMicrofinanceRiskScoreData(jsonstring);

                    //if (ViewData["ScoreList"] != null)
                    //{
                    //    ViewData["mfScoreList"] = (List<Scores>)ViewData["ScoreList"];
                    //    ViewData["lbl_mfi_score"] = ViewData["lbl_mfi_score"];
                    //}


                    ProcessMicrofinanceRiskScoreData(jsonstring);

                    if (ViewData["mfi_ScoreList"] != null)
                    {
                        ViewData["mfScoreList"] = ViewData["mfi_ScoreList"];
                        ViewData["lbl_mfi_score"] = ViewData["lbl_mfi_score"];
                    }






                    // Get the Microfinance Account Summary DataTable
                    DataTable microfinanceAccountSummary = GetMicrofinanceAccountSummary(jsonstring, model);

                    // Store the DataTable in  ViewData

                    ViewData["MicrofinanceAccountSummary"] = microfinanceAccountSummary;


                    // Call GenerateRetailAccountDetailsHtml method and store result
                    string retailAccountDetailsHtml = GenerateRetailAccountDetailsHtml(jsonstring, model);
                    ViewData["RetailAccountDetailsHtml"] = retailAccountDetailsHtml;


                    // Call GenerateMicrofinanceAccountDetails method and store result
                    string microfinanceAccountDetailsHtml = GenerateMicrofinanceAccountDetails(jsonstring, model);

                    ViewData["MicrofinanceAccountDetailsHtml"] = microfinanceAccountDetailsHtml;


                    // Process enquiries
                    var enquiries = ProcessEnquiries(rootObject);
                    ViewBag.Enquiries = enquiries;

                    // Call InquiryDetails to populate ViewBag/ViewData
                    InquiryDetails(jsonstring, model);

                    var addressData = ViewBag.AddressData;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }







        private void ProcessInquiryResponseHeader(Inquiryresponseheader header, Equifax_Enquiry_Model model)
        {
            if (header.ClientID != null)
            {
                model.ClientId = header.ClientID.ToString();
            }
            if (header.ReportOrderNO != null)
            {
                model.OrderNo = header.ReportOrderNO.ToString();
            }
            if (header.Date != null)
            {
                model.Date = header.Date.ToString();
            }
            if (header.Time != null)
            {
                model.Time = header.Time.ToString();
            }
        }



        private void InsertDataIntoDatabase(string clientid, string reportOrderno, string date, string time, Equifax_Enquiry_Model model)
        {
            SqlParameter[] pr = new SqlParameter[16];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
            pr[1] = new SqlParameter("@equi_custid", SqlDbType.NVarChar, 50) { Value = clientid };
            pr[2] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = reportOrderno };
            pr[3] = new SqlParameter("@date", SqlDbType.Date) { Value = string.IsNullOrEmpty(date) ? (object)DBNull.Value : DateTime.Parse(date) };
            pr[4] = new SqlParameter("@time", SqlDbType.NVarChar, 50) { Value = time };
            pr[5] = new SqlParameter("@name", SqlDbType.VarChar, 50) { Value = model.lbl_pname };
            pr[6] = new SqlParameter("@age", SqlDbType.Int) { Value = string.IsNullOrEmpty(model.lbl_age) ? (object)DBNull.Value : int.Parse(model.lbl_age) };
            pr[7] = new SqlParameter("@gender", SqlDbType.Char) { Value = model.lbl_gender };
            pr[8] = new SqlParameter("@dob", SqlDbType.Date) { Value = string.IsNullOrEmpty(model.lbl_dob) ? (object)DBNull.Value : DateTime.Parse(model.lbl_dob) };
            pr[9] = new SqlParameter("@pan_id", SqlDbType.NVarChar, 100) { Value = model.lbl_pan };
            pr[10] = new SqlParameter("@voter_id", SqlDbType.NVarChar, 100) { Value = model.lbl_voter };
            pr[11] = new SqlParameter("@other_id", SqlDbType.NVarChar, 100) { Value = model.lbl_otherid };
            pr[12] = new SqlParameter("@phone_no", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(model.lbl_home) ? (object)DBNull.Value : long.Parse(model.lbl_home) };
            pr[13] = new SqlParameter("@mobile_no", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(model.lbl_mob) ? (object)DBNull.Value : long.Parse(model.lbl_mob) };
            pr[14] = new SqlParameter("@outmsg", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pr[15] = new SqlParameter("@application_id", SqlDbType.BigInt) { Value = 0 };

            dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr);
            string ID = pr[14].Value.ToString();
            dbconnect.Close();
        }



        private void ProcessIdAndContactInfo(Idandcontactinfo idandcontactinfo, Equifax_Enquiry_Model model)
        {
            if (idandcontactinfo.PersonalInfo != null)
            {
                Personalinfo personal = idandcontactinfo.PersonalInfo;

                if (personal.Name != null)
                {
                    if (personal.Name.FullName != null)
                    {
                        model.lbl_name = string.IsNullOrEmpty(personal.Name.FullName) ? personal.Name.FirstName : personal.Name.FullName;
                        model.lbl_pname = personal.Name.FirstName;
                    }
                    else
                    {
                        model.lbl_pname = personal.Name.FirstName;
                    }
                }
                if (personal.Age != null)
                {
                    model.lbl_age = personal.Age.age?.ToString() ?? "";
                }
                if (personal.DateOfBirth != null)
                {
                    model.lbl_dob = personal.DateOfBirth.ToString();
                }
                if (personal.Gender != null)
                {
                    model.lbl_gender = personal.Gender.ToString();
                }
                if (personal.Occupation != null)
                {
                    model.lbl_occupation = personal.Occupation.ToString();
                }
                if (personal.MaritalStatus != null)
                {
                    model.lbl_marital = personal.MaritalStatus.ToString();
                }
            }
            if (idandcontactinfo.IdentityInfo != null)
            {
                Identityinfo identityinfo = idandcontactinfo.IdentityInfo;

                if (identityinfo.PANId != null)
                {
                    model.lbl_pan = identityinfo.PANId[0].IdNumber.ToString();
                }
                if (identityinfo.VoterID != null)
                {
                    model.lbl_voter = identityinfo.VoterID[0].IdNumber.ToString();
                }
                if (identityinfo.NationalIDCard != null)
                {
                    model.lbl_uid = identityinfo.NationalIDCard[0].IdNumber.ToString();
                }
                if (identityinfo.OtherId != null)
                {
                    model.lbl_otherid = identityinfo.OtherId[0].IdNumber.ToString();
                }
                if (identityinfo.RationCard != null)
                {
                    model.lbl_ration = identityinfo.RationCard[0].IdNumber.ToString();
                }
            }
            if (idandcontactinfo.PhoneInfo != null)
            {
                foreach (var phone in idandcontactinfo.PhoneInfo)
                {
                    if (phone.typeCode == "H")
                    {
                        model.lbl_home = phone.Number;
                    }
                    if (phone.typeCode == "M")
                    {
                        model.lbl_mob = phone.Number;
                    }
                }
            }
        }






        private void ProcessAddressInfo(Idandcontactinfo idandcontactinfo, Equifax_Enquiry_Model model, string ID)
        {
            if (idandcontactinfo.AddressInfo != null)
            {
                model.AddressList = idandcontactinfo.AddressInfo.Select(e => new Address
                {
                    Seq = string.IsNullOrEmpty(e.Seq) ? 0 : ParseInt(e.Seq),
                    Address1 = e.Address ?? string.Empty,
                    ReportedDate = e.ReportedDate ?? string.Empty,
                    Postal = e.Postal ?? string.Empty,
                    State = e.State ?? string.Empty
                }).ToList();

                foreach (var e in idandcontactinfo.AddressInfo)
                {
                    string addr = e.Address ?? string.Empty;
                    string Rdate = e.ReportedDate ?? string.Empty;
                    string postal = e.Postal ?? "0";
                    string state = e.State ?? string.Empty;

                    SqlParameter[] pr1 = new SqlParameter[6];
                    pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 2 };
                    pr1[1] = new SqlParameter("@address", SqlDbType.NVarChar, 100) { Value = addr };
                    pr1[2] = new SqlParameter("@date", SqlDbType.Date) { Value = string.IsNullOrEmpty(Rdate) ? (object)DBNull.Value : ParseDate(Rdate) };
                    pr1[3] = new SqlParameter("@post", SqlDbType.BigInt) { Value = ParseLong(postal) };
                    pr1[4] = new SqlParameter("@gender", SqlDbType.VarChar, 50) { Value = state };
                    pr1[5] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? 0 : ParseLong(ID) };

                    // Execute the stored procedure
                    dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr1);
                }
            }
        }

        private int ParseInt(string value)
        {
            int result;
            return int.TryParse(value, out result) ? result : 0;
        }

        private long ParseLong(string value)
        {
            long result;
            return long.TryParse(value, out result) ? result : 0;
        }

        private DateTime ParseDate(string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result) ? result : default(DateTime);
        }


        public ActionResult ProcessEquifaxRiskScoreData(string jsonstring)
        {
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);
            var modelscore = new Equifax_Enquiry_Model();

            if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData != null) // Equifax Risk Score
            {
                if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.ScoreDetails != null)
                {
                    var score_details1 = rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.ScoreDetails;

                    // Populate the model with score details
                    modelscore.ScoreList = score_details1.Select(a => new Scores
                    {
                        Name = a.Name ?? string.Empty,
                        Value = a.Value ?? string.Empty,
                        Type = a.Type ?? string.Empty,
                        Elements = a.ScoringElements.Select(se => new ScoreElement
                        {
                            Seq = int.Parse(se.seq),
                            Description = se.Description ?? string.Empty,
                        }).ToList()
                    }).ToList();

                    // Example of additional processing
                    //foreach (var a in score_details1)
                    //{
                    //    string seq = a.Name ?? string.Empty;
                    //    string description = a.Value ?? "0";
                    //    //string scrtype = a.Type ?? string.Empty;

                    //    //SqlParameter[] pr2 = new SqlParameter[5];
                    //    //pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                    //    //pr2[1] = new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = screname }; // score_name
                    //    //pr2[2] = new SqlParameter("@post", SqlDbType.BigInt) { Value = Convert.ToInt64(scrvalue) }; // score_value
                    //    //pr2[3] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = scrtype }; // score_type
                    //    //pr2[4] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? 0 : Convert.ToInt64(ID) }; // slno_master

                    //    //// Execute the stored procedure
                    //    //dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr2);
                    //    //dbconnect.Close();
                    //}

                    //modelscore.lbl_equi_score = "Equifax Risk Score:";
                    ViewData["lbl_equi_score"] = "Equifax Risk Score:";

                    // Pass the ScoreList to the view
                    ViewData["ScoreList"] = modelscore.ScoreList;
                }
            }

            // Return a view (update as necessary)
            return View(modelscore);
        }



        public ActionResult ProcessMicrofinanceRiskScoreData(string jsonstring)
        {
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);
            var modelscore1 = new Equifax_Enquiry_Model();

            if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData != null)
            {
                if (rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails != null)
                {
                    var score_details1 = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails;

                    // Populate the model with score details
                    modelscore1.ScoreList = score_details1.Select(a => new Scores
                    {
                        Name = a.Name ?? string.Empty,
                        Value = a.Value ?? string.Empty,
                        Type = a.Type ?? string.Empty,
                        Elements = a.ScoringElements.Select(se => new ScoreElement
                        {
                            Seq = int.Parse(se.seq),
                            Description = se.Description ?? string.Empty,
                        }).ToList()
                    }).ToList();

                    // Create SqlParameter array for stored procedure execution
                    //                //SqlParameter[] pr2 = new SqlParameter[5];
                    //                //pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                    //                //pr2[1] = new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = screname }; // score_name
                    //                //pr2[2] = new SqlParameter("@post", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(scrvalue) ? 0 : Convert.ToInt64(scrvalue) }; // score_value
                    //                //pr2[3] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = scrtype }; // score_type
                    //                //pr2[4] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? 0 : Convert.ToInt64(ID) }; // slno_master

                    //                //// Execute the stored procedure

                    //                //    dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr2);


                    ViewData["lbl_mfi_score"] = "Microfinance Risk Score:";
                    ViewData["mfi_ScoreList"] = modelscore1.ScoreList;
                    //ViewData["lbl_mfi_score"] = "Microfinance Risk Score:";

                    //// Pass the ScoreList to the view
                    //ViewData["ScoreList"] = modelscore1.ScoreList;
                }
            }

            // Return a view (update as necessary)
            return View(modelscore1);
        }




        //public ActionResult ProcessMicrofinanceRiskScoreData(string jsonstring)
        //{
        //    var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);
        //    var modelscore1 = new Equifax_Enquiry_Model();

        //    // Initialize the Microfinance Score List
        //    modelscore1.ScoreList = new List<Scores>();

        //    // Check if Microfinance Risk Score data is present
        //    if (rootObject?.CCRResponse?.CIRReportDataLst?[1]?.CIRReportData != null)
        //    {
        //        var scoreDetails = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails;

        //        if (scoreDetails != null)
        //        {
        //            foreach (var a in scoreDetails)
        //            {
        //                var screname = a.Name ?? string.Empty;
        //                var scrvalue = a.Value ?? string.Empty;
        //                var scrtype = a.Type ?? string.Empty;

        //                // Create SqlParameter array for stored procedure execution
        //                //SqlParameter[] pr2 = new SqlParameter[5];
        //                //pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
        //                //pr2[1] = new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = screname }; // score_name
        //                //pr2[2] = new SqlParameter("@post", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(scrvalue) ? 0 : Convert.ToInt64(scrvalue) }; // score_value
        //                //pr2[3] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = scrtype }; // score_type
        //                //pr2[4] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? 0 : Convert.ToInt64(ID) }; // slno_master

        //                //// Execute the stored procedure

        //                //    dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr2);

        //            }

        //            // Set ViewData values for the Microfinance Risk Score
        //            ViewData["lbl_mfi_score"] = "Microfinance Risk Score:";
        //            ViewData["mfScoreList"] = scoreDetails.Select(a => new Scores
        //            {
        //                Name = a.Name ?? string.Empty,
        //                Value = a.Value ?? string.Empty,
        //                Type = a.Type ?? string.Empty,
        //                Elements = a.ScoringElements?.Select(se => new ScoreElement
        //                {
        //                    Seq = int.Parse(se.seq),
        //                    Description = se.Description ?? string.Empty,
        //                }).ToList() ?? new List<ScoreElement>()
        //            }).ToList();


        //        }
        //    }

        //    // Return the view with the model containing Microfinance Risk Score data
        //    return View(modelscore1);
        //}






        //public ActionResult ProcessMicrofinanceRiskScoreData(string jsonstring)
        //{
        //    // Deserialize JSON string into the root object
        //    var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

        //    // Check if Microfinance Risk Score data is present
        //    if (rootObject?.CCRResponse?.CIRReportDataLst?[1]?.CIRReportData != null)
        //    {
        //        var scoreDetails2 = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails;

        //        if (scoreDetails2 != null)
        //        {



        //                foreach (var a in scoreDetails2)
        //                {
        //                    var screname = a.Name ?? string.Empty;
        //                    var scrvalue = a.Value ?? string.Empty;
        //                    var scrtype = a.Type ?? string.Empty;

        //                    //// Create SqlParameter array for stored procedure execution
        //                    //SqlParameter[] pr2 = new SqlParameter[5];
        //                    //pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
        //                    //pr2[1] = new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = screname }; // score_name
        //                    //pr2[2] = new SqlParameter("@post", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(scrvalue) ? 0 : Convert.ToInt64(scrvalue) }; // score_value
        //                    //pr2[3] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = scrtype }; // score_type
        //                    //pr2[4] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? 0 : Convert.ToInt64(ID) }; // slno_master

        //                    //// Execute the stored procedure
        //                    //dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr2);
        //                }


        //            // Set ViewData for Microfinance Risk Score
        //            ViewData["lbl_mfi_score"] = "Microfinance Risk Score:";
        //            ViewData["mfScoreList"] = scoreDetails2;
        //        }
        //    }

        //    return View();
        //}






        //public ActionResult ProcessMicrofinanceRiskScoreData(string jsonstring)
        //{
        //    var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);
        //    var modelscore = new Equifax_Enquiry_Model();

        //    // Check if Microfinance Risk Score data is present
        //    if (rootObject?.CCRResponse?.CIRReportDataLst?[1]?.CIRReportData != null)
        //    {
        //        var scoreDetails = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.ScoreDetails;

        //        if (scoreDetails != null)
        //        {
        //            // Populate the model with Microfinance Risk Score details
        //            modelscore.ScoreList = scoreDetails.Select(a => new Scores
        //            {
        //                Name = a.Name ?? string.Empty,
        //                Value = a.Value ?? string.Empty,
        //                Type = a.Type ?? string.Empty,
        //                Elements = a.ScoringElements?.Select(se => new ScoreElement
        //                {
        //                    Seq = int.Parse(se.seq),
        //                    Description = se.Description ?? string.Empty,
        //                }).ToList() ?? new List<ScoreElement>()
        //            }).ToList();

        //            // Set ViewData values for the Microfinance Risk Score
        //            ViewData["lbl_mfi_score"] = "Microfinance Risk Score:";
        //            ViewData["mfScoreList"] = modelscore.ScoreList;
        //        }
        //    }

        //    // Return the view with the model containing Microfinance Risk Score data
        //    return View(modelscore);
        //}




        public void ProcessRetailAccountSummary(string jsonstring, Equifax_Enquiry_Model model)
        {
            var rootObject = JsonConvert.DeserializeObject<dynamic>(jsonstring);

            if (rootObject != null && rootObject.CCRResponse != null)
            {
                if (rootObject.CCRResponse.CIRReportDataLst.Count > 0 &&
                    rootObject.CCRResponse.CIRReportDataLst[0] != null &&
                    rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData != null)
                {
                    var retailAccountSummary = rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.RetailAccountsSummary;

                    if (retailAccountSummary != null)
                    {
                        var ReAcc_Summ = retailAccountSummary;

                        // Populate DataTable with RetailAccountSummary data
                        DataTable Re_accnt_summ = new DataTable();
                        Re_accnt_summ.Columns.Add("NoOfAccounts");
                        Re_accnt_summ.Columns.Add("NoOfActiveAccounts");
                        Re_accnt_summ.Columns.Add("NoOfWriteOffs");
                        Re_accnt_summ.Columns.Add("TotalPastDue");
                        Re_accnt_summ.Columns.Add("MostSevereStatusWithIn24Months");
                        Re_accnt_summ.Columns.Add("SingleHighestCredit");
                        Re_accnt_summ.Columns.Add("SingleHighestSanctionAmount");
                        Re_accnt_summ.Columns.Add("TotalHighCredit");
                        Re_accnt_summ.Columns.Add("AverageOpenBalance");
                        Re_accnt_summ.Columns.Add("SingleHighestBalance");
                        Re_accnt_summ.Columns.Add("NoOfPastDueAccounts");
                        Re_accnt_summ.Columns.Add("NoOfZeroBalanceAccounts");
                        Re_accnt_summ.Columns.Add("RecentAccount");
                        Re_accnt_summ.Columns.Add("OldestAccount");
                        Re_accnt_summ.Columns.Add("TotalBalanceAmount");
                        Re_accnt_summ.Columns.Add("TotalSanctionAmount");
                        Re_accnt_summ.Columns.Add("TotalCreditLimit");
                        Re_accnt_summ.Columns.Add("TotalMonthlyPaymentAmount");

                        Re_accnt_summ.Rows.Add(
                            ReAcc_Summ.NoOfAccounts ?? "",
                            ReAcc_Summ.NoOfActiveAccounts ?? "",
                            ReAcc_Summ.NoOfWriteOffs ?? "",
                            ReAcc_Summ.TotalPastDue ?? "",
                            ReAcc_Summ.MostSevereStatusWithIn24Months ?? "",
                            ReAcc_Summ.SingleHighestCredit ?? "",
                            ReAcc_Summ.SingleHighestSanctionAmount ?? "",
                            ReAcc_Summ.TotalHighCredit ?? "",
                            ReAcc_Summ.AverageOpenBalance ?? "",
                            ReAcc_Summ.SingleHighestBalance ?? "",
                            ReAcc_Summ.NoOfPastDueAccounts ?? "",
                            ReAcc_Summ.NoOfZeroBalanceAccounts ?? "",
                            ReAcc_Summ.RecentAccount ?? "",
                            ReAcc_Summ.OldestAccount ?? "",
                            ReAcc_Summ.TotalBalanceAmount ?? "",
                            ReAcc_Summ.TotalSanctionAmount ?? "",
                            ReAcc_Summ.TotalCreditLimit ?? "",
                            ReAcc_Summ.TotalMonthlyPaymentAmount ?? ""
                        );
                        // Assuming lbl_retail and DataList2 are controls in your view
                        model.lbl_retail = "Retail Account Summary :";
                        // Set ViewBag.ReAccSumm
                        ViewBag.ReAccSumm = Re_accnt_summ;
                    }
                }
            }
        }

        private DataTable GetMicrofinanceAccountSummary(string jsonString, Equifax_Enquiry_Model model)
        {
            // Deserialize the JSON string into the RootObject
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

            DataTable accnt_summ = new DataTable();

            // Check if CIRReportData and MicrofinanceAccountsSummary exist
            if (rootObject?.CCRResponse?.CIRReportDataLst?[1]?.CIRReportData?.MicrofinanceAccountsSummary != null)
            {
                var Acc_Summ = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.MicrofinanceAccountsSummary;

                // Initialize columns for DataTable
                accnt_summ.Columns.Add("id");
                accnt_summ.Columns.Add("NoOfActiveAccounts");
                accnt_summ.Columns.Add("TotalPastDue");
                accnt_summ.Columns.Add("NoOfPastDueAccounts");
                accnt_summ.Columns.Add("RecentAccount");
                accnt_summ.Columns.Add("TotalBalanceAmount");
                accnt_summ.Columns.Add("TotalMonthlyPaymentAmount");
                accnt_summ.Columns.Add("TotalWrittenOffAmount");

                // Extract and assign values (use `?.` to avoid null reference exceptions)
                var id = Acc_Summ.Id ?? string.Empty;
                var activAcc = Acc_Summ.NoOfActiveAccounts ?? string.Empty;
                var totPastdue = Acc_Summ.TotalPastDue ?? string.Empty;
                var pastdueAcc = Acc_Summ.NoOfPastDueAccounts ?? string.Empty;
                var receAcc = Acc_Summ.RecentAccount ?? string.Empty;
                var balanceAmt = Acc_Summ.TotalBalanceAmount ?? string.Empty;
                var monthAmt = Acc_Summ.TotalMonthlyPaymentAmount ?? string.Empty;
                var writeoffAmt = Acc_Summ.TotalWrittenOffAmount ?? string.Empty;

                // Add row to DataTable
                accnt_summ.Rows.Add(id, activAcc, totPastdue, pastdueAcc, receAcc, balanceAmt, monthAmt, writeoffAmt);
                model.lbl_micro = "Micro Finance Account Summary :";


            }

            return accnt_summ;
        }





        public string GenerateRetailAccountDetailsHtml(string jsonstring, Equifax_Enquiry_Model model)
        {
            StringBuilder sb_main = new StringBuilder();

            // Deserialize JSON string into RootObject
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

            // Retail Account Details
            if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData != null)
            {
                if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.RetailAccountDetails != null)
                {
                    var Ret_acc_detail = rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.RetailAccountDetails;

                    foreach (var re_de in Ret_acc_detail)
                    {
                        // Extracting values
                        var AccountNumber = re_de.AccountNumber ?? "";
                        var Balance = re_de.Balance ?? "";
                        var Open = re_de.Open ?? "";
                        var DateReported = re_de.DateReported ?? "";
                        var Institution = re_de.Institution ?? "";
                        var PastDueAmount = re_de.PastDueAmount ?? "";
                        var DateOpened = re_de.DateOpened ?? "";
                        var AccountType = re_de.AccountType ?? "";
                        var LastPayment = re_de.LastPayment ?? "";
                        var LastPaymentDate = re_de.LastPaymentDate ?? "";
                        var DateClosed = re_de.DateClosed ?? "";
                        var OwnershipType = re_de.OwnershipType ?? "";
                        var SanctionAmount = re_de.SanctionAmount ?? "";
                        var Reason = re_de.Reason ?? "";
                        var RepaymentTenure = re_de.RepaymentTenure ?? "";
                        var InstallmentAmount = re_de.InstallmentAmount ?? "";
                        var AccountStatus = re_de.AccountStatus ?? "";

                        // Generating HTML
                        //sb_main.Append("<h3 style='width:100%;background-color:maroon;color:white;'>Retail Account Details</h3>");
                        sb_main.Append("<h3 style='width:100%; font-size:16px; background-color: maroon; color: white; -webkit-print-color-adjust: exact; color-adjust: exact;'>Retail Account Details</h3>");

                        sb_main.Append("<table style='font-size:15px;font-style:italic;color:gray;width:100%'>");
                        sb_main.Append("<tr><td style='font-size:15px;'> Acc#");
                        sb_main.Append(AccountNumber);
                        sb_main.Append("</td><td style='font-size:15px;' >Balance:");
                        sb_main.Append(Balance);
                        sb_main.Append("</td><td style='font-size:15px;'>Open:");
                        sb_main.Append(Open);
                        sb_main.Append("</td><td style='font-size:15px;'>Date Reported:");
                        sb_main.Append(DateReported);
                        sb_main.Append("</td></tr><tr><td style='font-size:15px;'>Institution:");
                        sb_main.Append(Institution);
                        sb_main.Append("</td><td style='font-size:15px;'>Past Due Amount:");
                        sb_main.Append(PastDueAmount);
                        sb_main.Append("</td><td style='font-size:15px;'>Date Opened:");
                        sb_main.Append(DateOpened);
                        sb_main.Append("</td></tr><tr><td style='font-size:15px;'>Type:");
                        sb_main.Append(AccountType);
                        sb_main.Append("</td><td style='font-size:15px;'>Last Payment:");
                        sb_main.Append(LastPayment);
                        sb_main.Append("</td><td style='font-size:15px;'>Last Payment Date:");
                        sb_main.Append(LastPaymentDate);
                        sb_main.Append("</td><td style='font-size:15px;'>Date Closed:");
                        sb_main.Append(DateClosed);
                        sb_main.Append("</td></tr><tr><td style='font-size:15px;'>Ownership Type:");
                        sb_main.Append(OwnershipType);
                        sb_main.Append("</td><td style='font-size:15px;'>Sanction Amount:");
                        sb_main.Append(SanctionAmount);
                        sb_main.Append("</td><td style='font-size:15px;'>Reason:");
                        sb_main.Append(Reason);
                        sb_main.Append("</td></tr><tr><td colspan='4' style='font-size:15px;'><hr /></td></tr><tr><td>Repayment Tenure:");
                        sb_main.Append(RepaymentTenure);
                        sb_main.Append("</td><td style='font-size:15px;'>Monthly Payment Amount:");
                        sb_main.Append(InstallmentAmount);
                        sb_main.Append("</td></tr><tr><td style='font-size:15px;' colspan='4'>Account Status: ");
                        sb_main.Append(AccountStatus);
                        sb_main.Append("</td></tr><tr><td style='font-size:15px;' colspan='4'>Asset Classification:</td></tr><tr><td colspan='4'>Suit Filed Status:</td></tr>" +
                            "<tr><td colspan='4'><u>History</u></td></tr></table>");
                        // Handling History48Months
                        if (re_de.History48Months != null)
                        {
                            var his_length = re_de.History48Months.Length;
                            string sl_no1 = "";

                            foreach (var history in re_de.History48Months)
                            {
                                var paymentStatus = history.PaymentStatus ?? "";
                                var AssetClassificationStatus = history.AssetClassificationStatus ?? "";
                                var SuitFiledStatus = history.SuitFiledStatus ?? "";
                                var key = history.key ?? "";

                                // Update stored procedures
                                SqlParameter[] pr6 = new SqlParameter[21];
                                pr6[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 5 };
                                pr6[1] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = model.ID == "" ? "0" : model.ID };
                                pr6[2] = new SqlParameter("@acc_number", SqlDbType.NVarChar, 50) { Value = AccountNumber };
                                pr6[3] = new SqlParameter("@Institution", SqlDbType.NVarChar, 100) { Value = Institution };
                                pr6[4] = new SqlParameter("@balance", SqlDbType.Float) { Value = Balance == "" ? "0" : Balance };
                                pr6[5] = new SqlParameter("@date_opened", SqlDbType.NVarChar, 50) { Value = DateOpened };
                                pr6[6] = new SqlParameter("@past_due_amt", SqlDbType.Float) { Value = PastDueAmount == "" ? "0" : PastDueAmount };
                                pr6[7] = new SqlParameter("@int_rate", SqlDbType.Float) { Value = 0 };
                                pr6[8] = new SqlParameter("@type", SqlDbType.VarChar, 50) { Value = AccountType };
                                pr6[9] = new SqlParameter("@last_payment", SqlDbType.NVarChar, 50) { Value = LastPayment };
                                pr6[10] = new SqlParameter("@last_pay_dt", SqlDbType.NVarChar, 50) { Value = LastPaymentDate };
                                pr6[11] = new SqlParameter("@date_closed", SqlDbType.NVarChar, 50) { Value = DateClosed };
                                pr6[12] = new SqlParameter("@ownership_type", SqlDbType.NVarChar, 50) { Value = OwnershipType };
                                pr6[13] = new SqlParameter("@write_off_amt", SqlDbType.Float) { Value = 0 };
                                pr6[14] = new SqlParameter("@sanction_amt", SqlDbType.Float) { Value = SanctionAmount == "" ? "0" : SanctionAmount };
                                pr6[15] = new SqlParameter("@reason", SqlDbType.NVarChar, 100) { Value = Reason };
                                pr6[16] = new SqlParameter("@repayment_tenure", SqlDbType.Int) { Value = RepaymentTenure == "" ? "0" : RepaymentTenure };
                                pr6[17] = new SqlParameter("@monthly_pay_amt", SqlDbType.Float) { Value = InstallmentAmount == "" ? "0" : InstallmentAmount };
                                pr6[18] = new SqlParameter("@credit_limit", SqlDbType.Float) { Value = 0 };
                                pr6[19] = new SqlParameter("@acc_status", SqlDbType.VarChar, 50) { Value = AccountStatus };
                                pr6[20] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
                                // Assuming oh is your database helper
                                dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr6);
                                dbconnect.Close();

                                sl_no1 = pr6[20].Value.ToString();

                                SqlParameter[] pr7 = new SqlParameter[6];
                                pr7[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 6 };
                                pr7[1] = new SqlParameter("@sl_detail4", SqlDbType.BigInt) { Value = sl_no1 == "" ? "0" : sl_no1 };
                                pr7[2] = new SqlParameter("@equi_custid", SqlDbType.NVarChar, 50) { Value = paymentStatus };
                                pr7[3] = new SqlParameter("@pan_id", SqlDbType.NVarChar, 100) { Value = AssetClassificationStatus };
                                pr7[4] = new SqlParameter("@voter_id", SqlDbType.NVarChar, 50) { Value = SuitFiledStatus };
                                pr7[5] = new SqlParameter("@other_id", SqlDbType.NVarChar, 50) { Value = key };
                                dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr7);
                                dbconnect.Close();
                            }

                            // Generate History Table
                            StringBuilder sb = new StringBuilder();
                            var length3 = 25;
                            if (his_length < length3) { length3 = his_length; }
                            sb.Append("<table style='font-size:4px;'>");
                            sb.Append("<tr><td>Account Status :</td>");
                            //for (int i = 0; i < length3; i++)
                            //{
                            //    var AccountStatus1 = re_de.History48Months[i].AccountStatus ?? "";
                            //    sb.Append("<td>");
                            //    sb.Append(AccountStatus1);
                            //    sb.Append("</td>");
                            //}
                            sb.Append("</tr><tr><td>Payment Status:</td>");
                            for (int i = 0; i < length3; i++)
                            {
                                var PaymentStatus = re_de.History48Months[i].PaymentStatus ?? "";
                                sb.Append("<td>");
                                sb.Append(PaymentStatus);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr><tr><td>Asset Classification Status:</td>");
                            for (int i = 0; i < length3; i++)
                            {
                                var AssetClassificationStatus = re_de.History48Months[i].AssetClassificationStatus ?? "";
                                sb.Append("<td>");
                                sb.Append(AssetClassificationStatus);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr><tr><td>Suit Filed Status:</td>");
                            for (int i = 0; i < length3; i++)
                            {
                                var SuitFiledStatus = re_de.History48Months[i].SuitFiledStatus ?? "";
                                sb.Append("<td>");
                                sb.Append(SuitFiledStatus);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr><tr><td>Key:</td>");
                            for (int i = 0; i < length3; i++)
                            {
                                var key = re_de.History48Months[i].key ?? "";
                                sb.Append("<td>");
                                sb.Append(key);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr></table>");

                            sb_main.Append(sb.ToString());
                        }
                    }
                }
            }

            return sb_main.ToString();
        }







        public string GenerateMicrofinanceAccountDetails(string jsonstring, Equifax_Enquiry_Model model)
        {
            // Parse the JSON string into a rootObject
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

            if (rootObject?.CCRResponse?.CIRReportDataLst?[1]?.CIRReportData?.MicrofinanceAccountDetails == null)
            {
                // Handle the case where MicrofinanceAccountDetails is null
                return "No microfinance account details available.";
            }

            var Micr_acc_detail = rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.MicrofinanceAccountDetails;
            StringBuilder sb_main1 = new StringBuilder();

            foreach (var mi_de in Micr_acc_detail)
            {
                // Initialize variables
                string nominee_name = mi_de.Nominee?.Name ?? "";
                string nominee_type = mi_de.Nominee?.RelationType ?? "";
                string key_per_name = mi_de.KeyPerson?.Name ?? "";
                string key_relation = mi_de.KeyPerson?.RelationType ?? "";

                // Initialize other variables
                var AccountNumber = mi_de.AccountNumber ?? "";
                var CurrentBalance = mi_de.CurrentBalance ?? "";
                var AccountStatus = mi_de.AccountStatus ?? "";
                var DateReported = mi_de.DateReported ?? "";
                var Institution = mi_de.Institution ?? "";
                var PastDueAmount = mi_de.PastDueAmount ?? "";
                var DisbursedAmount = mi_de.DisbursedAmount ?? "";
                var DateOpened = mi_de.DateOpened ?? "";
                var LastPayment = mi_de.LastPayment ?? "";
                var LastPaymentDate = mi_de.LastPaymentDate ?? "";
                var DateClosed = mi_de.DateClosed ?? "";
                var LoanCategory = mi_de.LoanCategory ?? "";
                var WriteOffAmount = mi_de.WriteOffAmount ?? "";
                var Reason = mi_de.Reason ?? "";
                var LoanPurpose = mi_de.LoanPurpose ?? "";
                var InstitutionType = mi_de.InstitutionType ?? "";
                var LoanCycleID = mi_de.LoanCycleID ?? "";
                var RepaymentTenure = mi_de.RepaymentTenure ?? "";
                var SanctionAmount = mi_de.SanctionAmount ?? "";
                var DateSanctioned = mi_de.DateSanctioned ?? "";
                var NoOfInstallments = mi_de.NoOfInstallments ?? "";
                var InstallmentAmount = mi_de.InstallmentAmount ?? "";
                var AppliedAmount = mi_de.AppliedAmount ?? "";
                var DateApplied = mi_de.DateApplied ?? "";

                // Append HTML content
                sb_main1.Append("<h3 style='width: 100%; background-color: #31423e; color: white;'>Microfinance Account Details</h3>");
                sb_main1.Append("<table style='font-size:x-small;font-style:italic;color:gray;width:100%;'>");
                sb_main1.Append("<tr><td>Acc#").Append(AccountNumber).Append("</td><td>Current Balance:").Append(CurrentBalance)
                    .Append("</td><td>Status:").Append(AccountStatus).Append("</td><td>Date Reported:")
                    .Append(DateReported).Append("</td></tr><tr><td>Institution:").Append(Institution)
                    .Append("</td><td>Past Due Amount:").Append(PastDueAmount).Append("</td><td>Disbursed Amount:")
                    .Append(DisbursedAmount).Append("</td><td>Date Opened:").Append(DateOpened).Append("</td></tr><tr><td>")
                    .Append("</td><td>Last Payment:").Append(LastPayment).Append("</td><td>Last Payment Date:")
                    .Append(LastPaymentDate).Append("</td><td>Date Closed:").Append(DateClosed).Append("</td></tr><tr><td>Loan Category:")
                    .Append(LoanCategory).Append("</td><td>Write-Off Amount:").Append(WriteOffAmount).Append("</td><td>Reason:")
                    .Append(Reason).Append("</td><td>Date Written Off:</td></tr>");
                sb_main1.Append("<tr><td>Loan Purpose :").Append(LoanPurpose).Append("</td><td colspan='3'>Institution Type :")
                    .Append(InstitutionType).Append("</td></tr>");
                sb_main1.Append("<tr><td colspan='4'><hr /></td></tr><tr><td colspan='4'><u>History 24 Months</u></td></tr></table>");

                // Execute stored procedure for each account detail
                //SqlParameter[] pr4 = new SqlParameter[30];
                //pr4[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 7 };
                //pr4[1] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = model.ID ?? "0" };
                //pr4[2] = new SqlParameter("@acc_number", SqlDbType.NVarChar, 50) { Value = AccountNumber };
                //pr4[3] = new SqlParameter("@Institution", SqlDbType.NVarChar, 100) { Value = Institution };
                //pr4[4] = new SqlParameter("@balance", SqlDbType.Float) { Value = string.IsNullOrEmpty(CurrentBalance) ? "0" : CurrentBalance };
                //pr4[5] = new SqlParameter("@type", SqlDbType.VarChar, 50) { Value = AccountStatus };
                //pr4[6] = new SqlParameter("@open_type", SqlDbType.NVarChar, 50) { Value = DateReported };
                //pr4[7] = new SqlParameter("@past_due_amt", SqlDbType.Float) { Value = string.IsNullOrEmpty(PastDueAmount) ? "0" : PastDueAmount };
                //pr4[8] = new SqlParameter("@credit_limit", SqlDbType.Float) { Value = string.IsNullOrEmpty(DisbursedAmount) ? "0" : DisbursedAmount };
                //pr4[9] = new SqlParameter("@date_opened", SqlDbType.NVarChar, 50) { Value = DateOpened };
                //pr4[10] = new SqlParameter("@last_payment", SqlDbType.NVarChar, 50) { Value = LastPayment };
                //pr4[11] = new SqlParameter("@last_pay_dt", SqlDbType.NVarChar, 50) { Value = LastPaymentDate };
                //pr4[12] = new SqlParameter("@date_closed", SqlDbType.NVarChar, 50) { Value = DateClosed };
                //pr4[13] = new SqlParameter("@ownership_type", SqlDbType.NVarChar, 50) { Value = LoanCategory };
                //pr4[14] = new SqlParameter("@write_off_amt", SqlDbType.Float) { Value = 0 };
                //pr4[15] = new SqlParameter("@reason", SqlDbType.NVarChar, 100) { Value = Reason };
                //pr4[16] = new SqlParameter("@pan_id", SqlDbType.NVarChar, 50) { Value = "" };
                //pr4[17] = new SqlParameter("@voter_id", SqlDbType.NVarChar, 50) { Value = LoanPurpose };
                //pr4[18] = new SqlParameter("@other_id", SqlDbType.NVarChar, 50) { Value = InstitutionType };
                //pr4[19] = new SqlParameter("@repayment_tenure", SqlDbType.Int) { Value = string.IsNullOrEmpty(LoanCycleID) ? "0" : LoanCycleID };
                //pr4[20] = new SqlParameter("@age", SqlDbType.Int) { Value = string.IsNullOrEmpty(NoOfInstallments) ? "0" : NoOfInstallments };
                //pr4[21] = new SqlParameter("@address", SqlDbType.NVarChar, 50) { Value = RepaymentTenure };
                //pr4[22] = new SqlParameter("@monthly_pay_amt", SqlDbType.Float) { Value = string.IsNullOrEmpty(InstallmentAmount) ? "0" : InstallmentAmount };
                //pr4[23] = new SqlParameter("@sanction_amt", SqlDbType.Float) { Value = string.IsNullOrEmpty(SanctionAmount) ? "0" : SanctionAmount };
                //pr4[24] = new SqlParameter("@int_rate", SqlDbType.Float) { Value = string.IsNullOrEmpty(AppliedAmount) ? "0" : AppliedAmount };
                //pr4[25] = new SqlParameter("@date_reported", SqlDbType.NVarChar, 50) { Value = DateSanctioned };
                //pr4[26] = new SqlParameter("@collateral_type", SqlDbType.NVarChar, 50) { Value = DateApplied };
                //pr4[27] = new SqlParameter("@suit_field_status", SqlDbType.NVarChar, 50) { Value = "" };
                //pr4[28] = new SqlParameter("@date_of_default", SqlDbType.NVarChar, 50) { Value = "" };
                //pr4[29] = new SqlParameter("@legal_case_status", SqlDbType.NVarChar, 50) { Value = "" };

                //dbconnect.ExecuteStoredProcedure("PL_PROC_MICROFINANCEACCOUNTS", pr4);

                // Append additional details to the StringBuilder
                sb_main1.Append("<table><tr><td>Nominee Name:</td><td>").Append(nominee_name).Append("</td></tr>")
                    .Append("<tr><td>Nominee Type:</td><td>").Append(nominee_type).Append("</td></tr>")
                    .Append("<tr><td>Key Person Name:</td><td>").Append(key_per_name).Append("</td></tr>")
                    .Append("<tr><td>Key Person Relation:</td><td>").Append(key_relation).Append("</td></tr>")
                    .Append("</table><br/>");
            }

            return sb_main1.ToString();
        }


        public List<Enquiry> ProcessEnquiries(Rootobject rootObject)
        {
            var enquiries = new List<Enquiry>();

            if (rootObject != null)
            {
                // First CIRReportDataLst
                if (rootObject.CCRResponse.CIRReportDataLst[0]?.CIRReportData?.Enquiries != null)
                {
                    enquiries.AddRange(rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData.Enquiries);
                }

                // Second CIRReportDataLst
                if (rootObject.CCRResponse.CIRReportDataLst[1]?.CIRReportData?.Enquiries != null)
                {
                    enquiries.AddRange(rootObject.CCRResponse.CIRReportDataLst[1].CIRReportData.Enquiries);
                }
            }

            return enquiries;
        }






        public void InquiryDetails(string jsonstring, Equifax_Enquiry_Model model)
        {
            // Initialize variables
            string fname = "";
            string otherid = "";
            string pan1 = "";
            string voterid = "";
            string nationalid = "";
            string rationCard = "";
            string mob_no = "";
            string home = "";
            string dob = "";
            string ID = "";

            // Deserialize JSON string to Rootobject
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonstring);

            // Extract client and report information from the model
            var clientid = model.ClientId;
            var reportOrderno = model.OrderNo;
            var date = model.Date;
            var time = model.Time;

            // Get InquiryRequestInfo from the deserialized object
            Inquiryrequestinfo1 inquiryrequestinfo = rootObject.CCRResponse.CIRReportDataLst
                .FirstOrDefault()?.InquiryRequestInfo;

            if (inquiryrequestinfo != null)
            {
                fname = inquiryrequestinfo.FirstName ?? "";

                // Set ViewBag properties
                ViewBag.Fname = fname;

                if (inquiryrequestinfo.IDDetails != null)
                {
                    foreach (var idde in inquiryrequestinfo.IDDetails)
                    {
                        string idtype = idde.IDType;
                        switch (idtype)
                        {
                            case "M":
                                otherid = idde.IDValue;
                                break;
                            case "T":
                                pan1 = idde.IDValue;
                                break;
                            case "V":
                                voterid = idde.IDValue;
                                break;
                            case "P":
                                nationalid = idde.IDValue;
                                break;
                            case "R":
                                rationCard = idde.IDValue;
                                break;
                        }
                    }
                    ViewBag.Voterid = voterid;
                    ViewBag.Nationalid = nationalid;
                    ViewBag.Otherid = otherid;
                    ViewBag.RationCard = rationCard;
                    ViewBag.Pan = pan1;
                }

                if (inquiryrequestinfo.InquiryPhones != null)
                {
                    foreach (var pho in inquiryrequestinfo.InquiryPhones)
                    {
                        if (pho.PhoneType[0].ToString() == "M")
                        {
                            mob_no = pho.Number;
                            ViewBag.MobileNo = mob_no;
                        }
                        if (pho.PhoneType[0].ToString() == "H")
                        {
                            home = pho.Number;
                            ViewBag.Home = home;
                        }
                    }
                }

                if (inquiryrequestinfo.DOB != null)
                {
                    dob = inquiryrequestinfo.DOB;
                    ViewBag.Dob = dob;
                }
            }

            // Execute stored procedure
            SqlParameter[] pr = new SqlParameter[16];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
            pr[1] = new SqlParameter("@equi_custid", SqlDbType.NVarChar, 50) { Value = clientid };
            pr[2] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = reportOrderno };
            pr[3] = new SqlParameter("@date", SqlDbType.Date) { Value = string.IsNullOrEmpty(date) ? (object)DBNull.Value : date };
            pr[4] = new SqlParameter("@time", SqlDbType.NVarChar, 50) { Value = time };
            pr[5] = new SqlParameter("@name", SqlDbType.VarChar, 50) { Value = fname };
            pr[6] = new SqlParameter("@age", SqlDbType.Int) { Value = 0 };
            pr[7] = new SqlParameter("@gender", SqlDbType.Char) { Value = "" };
            pr[8] = new SqlParameter("@dob", SqlDbType.Date) { Value = string.IsNullOrEmpty(dob) ? (object)DBNull.Value : dob };
            pr[9] = new SqlParameter("@pan_id", SqlDbType.NVarChar, 100) { Value = pan1 };
            pr[10] = new SqlParameter("@voter_id", SqlDbType.NVarChar, 100) { Value = voterid };
            pr[11] = new SqlParameter("@other_id", SqlDbType.NVarChar, 100) { Value = otherid };
            pr[12] = new SqlParameter("@phone_no", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(home) ? (object)0 : home };
            pr[13] = new SqlParameter("@mobile_no", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(mob_no) ? (object)0 : mob_no };
            pr[14] = new SqlParameter("@outmsg", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pr[15] = new SqlParameter("@application_id", SqlDbType.BigInt) { Value = 0 };
            dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr);
            ID = pr[14].Value.ToString();
            dbconnect.Close();

            // Check for null CIRReportData and handle accordingly
            if (rootObject.CCRResponse.CIRReportDataLst[0].CIRReportData == null)
            {
                SqlParameter[] pr2 = new SqlParameter[5];
                pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                pr2[1] = new SqlParameter("@name", SqlDbType.VarChar, 100) { Value = "" };
                pr2[2] = new SqlParameter("@post", SqlDbType.BigInt) { Value = -2 };
                pr2[3] = new SqlParameter("@report_orderno", SqlDbType.VarChar, 50) { Value = "ERS" };
                pr2[4] = new SqlParameter("@ID", SqlDbType.BigInt) { Value = string.IsNullOrEmpty(ID) ? (object)0 : ID };
                dbconnect.ExecuteStoredProcedure("[dbo].[equifax_json_report]", pr2);
                dbconnect.Close();

                // Populate ViewBag with default values if CIRReportData is null
                ViewBag.EquifaxScore = "Equifax Risk Score:";

                DataTable score = new DataTable();
                score.Columns.Add("Name", typeof(string));
                score.Columns.Add("Value", typeof(string));
                score.Columns.Add("Type", typeof(string));
                score.Columns.Add("ScoringElements", typeof(string));
                score.Rows.Add("Equifax Risk Score", "-2", "ERS", "");
                ViewBag.ScoreData = score;
            }

            // Process and populate address data
            DataTable addr = new DataTable();
            addr.Columns.Add("Seq", typeof(string));
            addr.Columns.Add("Address", typeof(string));
            addr.Columns.Add("ReportedDate", typeof(string));
            addr.Columns.Add("Postal", typeof(string));
            addr.Columns.Add("State", typeof(string));

            if (inquiryrequestinfo?.InquiryAddresses != null && inquiryrequestinfo.InquiryAddresses.Length > 0)
            {
                var address = inquiryrequestinfo.InquiryAddresses[0];
                addr.Rows.Add("1", address.AddressLine1 ?? "", "", address.Postal ?? "", address.State ?? "");
            }

            ViewBag.AddressData = addr;
        }













    }
}













