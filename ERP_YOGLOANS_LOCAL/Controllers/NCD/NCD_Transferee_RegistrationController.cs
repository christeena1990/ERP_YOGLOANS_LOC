using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Transferee_RegistrationController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_Transferee_Registration_View()
        {
           // Session["application_subid"] = 151;

            var JointaccountDetails = ApplicantsGrid();
            ViewBag.JointaccountDetails = JointaccountDetails;

            // Check if the session variable 'application_custId' is not null
            if (Session["application_custId_151"] != null)
            {
                var applicantDetails = Applicantdetails();
                if (applicantDetails.Tables.Count > 0)
                {
                    


                    ViewBag.jointApplicantname = applicantDetails.Tables[0].Rows[0][0]?.ToString(); // Replace index if different
                    string dob = applicantDetails.Tables[0].Rows[0][1]?.ToString();
                    string age = applicantDetails.Tables[0].Rows[0][2]?.ToString();
                    Session["transtere_age"] = age;
                    string dobWithAge = dob + " / " + age + "Yrs";
                    ViewBag.dob_age = dobWithAge;
                    ViewBag.jointApplicantpan = applicantDetails.Tables[1].Rows[0][0]?.ToString();

                }
            }
            //Session["application_custId_151"] = 0;
            return View();
        }

        [HttpPost]
        public ActionResult SetSessionValue()
        {
            Session["application_subid"] = 151;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public DataTable ApplicantsGrid()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"]; // Ensure branch_id is converted to an integer

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }
        public DataSet Applicantdetails()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.Int);
            pr[1].Value = Session["application_custId_151"]; // Ensure branch_id is converted to an integer

            DataSet ds = new DataSet();
            dbconnect.Open();

            ds = dbconnect.ExecuteDataset("[dbo].[DEB_Application]", pr);
            dbconnect.Close();
            //Session["application_custId_151"] = 0;
            return ds;

        }
        [HttpPost]
        public ActionResult ExistingjointApplicantsGrid(string applicationId)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[1].Value = Convert.ToInt64(applicationId);

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_jointapplication]", pr).Tables[0];
            dbconnect.Close();
            // Convert DataTable to a list of dictionaries to return as JSON
            var data = dt.AsEnumerable()
                         .Select(row => dt.Columns.Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]));

            return Json(new { data = data, columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList() }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FillDropdowns()
        {
            SqlParameter[] pr2 = new SqlParameter[3];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 4;

            pr2[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr2[1].Value = Session["application_custId_151"].ToString();

            pr2[2] = new SqlParameter("@cust_age", SqlDbType.Int);
            pr2[2].Value = Session["transtere_age"];
            DataSet ds=new DataSet();
           
            dbconnect.Open();
             ds = dbconnect.ExecuteDataset("DEB_Application", pr2);
            dbconnect.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
              
                var dropdownData = new
                {
                    FifteenGOptions = ds.Tables[0].AsEnumerable().Select(row => new
                    {
                        Id = row["_15g_id"],
                        Name = row["_15g_name"]
                    }).ToList(),

                    RepaymentOptions = ds.Tables[1].AsEnumerable().Select(row => new
                    {
                        Id = row["id"],
                        Name = row["repayment"]
                    }).ToList(),

                    DpOptions = ds.Tables[2].AsEnumerable().Select(row => new
                    {
                        Id = row["dp_id"],
                        Name = row["dp_name"]
                    }).ToList()
                };

                return Json(new { success = true, data = dropdownData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult SubmitApplication(Submit_NCD_Application submit_NCD_Application)
        {
         
            SqlParameter[] pr2 = new SqlParameter[20];

            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 2;

            pr2[1] = new SqlParameter("@transfer_id", SqlDbType.BigInt);
            pr2[1].Value = submit_NCD_Application.transfer_ID;

            pr2[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr2[2].Value = Session["branch_id"].ToString();

            pr2[3] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr2[3].Value = Session["login_user"].ToString();
       
            pr2[4] = new SqlParameter("@nofapplicant", SqlDbType.Int);
            pr2[4].Value = submit_NCD_Application.nofapplicant;

            pr2[5] = new SqlParameter("@ac_type", SqlDbType.Char, 1);
            pr2[5].Value = submit_NCD_Application.ac_type;

            pr2[6] = new SqlParameter("@tax_payee", SqlDbType.Char, 1);
            pr2[6].Value = "N";

            pr2[7] = new SqlParameter("@lienholder", SqlDbType.VarChar, 50);
            pr2[7].Value = submit_NCD_Application.lienholder;

            pr2[8] = new SqlParameter("@lien", SqlDbType.Char, 1);
            pr2[8].Value = submit_NCD_Application.lien;

            pr2[9] = new SqlParameter("@rpay", SqlDbType.Char, 1);
            pr2[9].Value = submit_NCD_Application.rpay;

            pr2[10] = new SqlParameter("@15_status", SqlDbType.Char, 1);
            pr2[10].Value = submit_NCD_Application._15_status;

            pr2[11] = new SqlParameter("@15_type", SqlDbType.Char, 1);
            pr2[11].Value = submit_NCD_Application._15_type;

            pr2[12] = new SqlParameter("@dp_id", SqlDbType.VarChar, 100);
            pr2[12].Value = submit_NCD_Application.dp_id;

            pr2[13] = new SqlParameter("@dp_name", SqlDbType.VarChar, 200);
            pr2[13].Value = submit_NCD_Application.dp_name;

            pr2[14] = new SqlParameter("@pancard", SqlDbType.VarChar, 20);
            pr2[14].Value = submit_NCD_Application.pancard;

            pr2[15] = new SqlParameter("@nominee", SqlDbType.VarChar, 100);
            pr2[15].Value = submit_NCD_Application.nominee;

            pr2[16] = new SqlParameter("@nominee_add", SqlDbType.VarChar, 200);
            pr2[16].Value = submit_NCD_Application.nominee_add;

            pr2[17] = new SqlParameter("@relation", SqlDbType.VarChar, 50);
            pr2[17].Value = submit_NCD_Application.relation;

            pr2[18] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr2[18].Direction = ParameterDirection.Output;

            pr2[19] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr2[19].Value = Session["application_custId_151"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_transfer_save", pr2);            
            dbconnect.Close();

            string message = pr2[18].Value.ToString();
            Session["application_custId_151"] = null;
            Session["application_subid"] = "0";
            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }
    }
}