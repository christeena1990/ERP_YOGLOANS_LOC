using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Joint_ApplicantsController : BaseController
    {
        DB dbconnect = new DB();
        // GET: NCD_Joint_Applicants
      
        public ActionResult NCD_Joint_Applicants()
        {
            //Session["application_subid"] = 150;

            var JointaccountDetails= ApplicantsGrid();
            ViewBag.JointaccountDetails = JointaccountDetails;

            // Check if the session variable 'application_custId_150' is not null
            if (Session["application_custId_150"] != null)
            {
                var applicantDetails = Applicantdetails();
                if (applicantDetails.Rows.Count > 0)
                {
                    // Assuming the first row contains 'jointApplicantname' and 'jointApplicantpan' in the 0th and 1st columns respectively
                    ViewBag.jointApplicantname = applicantDetails.Rows[0][0].ToString(); // Replace with actual column index if different
                    ViewBag.jointApplicantpan = applicantDetails.Rows[0][1].ToString();  // Replace with actual column index if different
                }
            }

                return View();
           
        }



        [HttpPost]
        public ActionResult SetSessionValue()
        {
            Session["application_subid"] = 150;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }





        public DataTable ApplicantsGrid()
        {
            
                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1;

                pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
                pr[1].Value = Session["branch_id"]; // Ensure branch_id is converted to an integer

            DataTable dt = new DataTable();
            dbconnect.Open();
             dt = dbconnect.ExecuteDataset("[dbo].[DEB_jointapplication]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }





        //[HttpPost]
        //public ActionResult ExistingjointApplicantsGrid(string applicationId)
        //{
        //    SqlParameter[] pr = new SqlParameter[2];

        //    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
        //    pr[0].Value = 2;

        //    pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
        //    pr[1].Value = Convert.ToInt64(applicationId);

        //    DataTable dt = new DataTable();
        //    dbconnect.Open();
        //    dt = dbconnect.ExecuteDataset("[dbo].[DEB_jointapplication]", pr).Tables[0];
        //    dbconnect.Close();
        //    // Convert DataTable to a list of dictionaries to return as JSON
        //    var data = dt.AsEnumerable()
        //                 .Select(row => dt.Columns.Cast<DataColumn>()
        //                 .ToDictionary(col => col.ColumnName, col => row[col]));

        //    return Json(new { data = data, columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList() }, JsonRequestBehavior.AllowGet);
        //}



        public DataTable Applicantdetails()
        {
          
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.Int);
            pr[1].Value = Session["application_custId_150"]; // Ensure branch_id is converted to an integer

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_jointapplication]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }

        public ActionResult JointAplcnt_Btn_clk(string applicationIdTextbox, string jointApplicantname)
        {
            SqlParameter[] prr = new SqlParameter[6];

            prr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            prr[0].Value = 3;

            prr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            prr[1].Value = Convert.ToInt64(applicationIdTextbox); // Pass the application ID from the form input

            prr[2] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            prr[2].Value = Session["application_custId_150"]; // Assuming you have the customer ID stored in the session

            prr[3] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            prr[3].Value = Session["login_user"]; // Assuming you have the user ID stored in the session

            prr[4] = new SqlParameter("@applicantname", SqlDbType.VarChar, 4000);
            prr[4].Value = jointApplicantname; // Pass the applicant name from the form input

            prr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            prr[5].Direction = ParameterDirection.Output; // Set the output message

            // Execute the stored procedure
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_jointapplication]", prr);

            // Check output message from stored procedure
            string outputMessage = prr[5].Value.ToString();


            //////////////  alert when success or error    /////////

            if (outputMessage == "Submit successfully")
            {
                TempData["AlertType"] = "success"; // Success flag
            }
            else
            {
                TempData["AlertType"] = "error"; // Error flag
            }

            TempData["Message"] = outputMessage; // Store the output message




            ///////////////////////////////////


            //TempData["Message"] = outputMessage;


            // Clear specific session value
            Session.Remove("application_custId_150");


            return RedirectToAction("NCD_Joint_Applicants");
        }


        [HttpPost]
        public JsonResult ClearSession()
        {
            // Clear the session variable
            Session.Remove("application_custId_150");

            // Return a success response
            return Json(new { success = true });
        }


    }
}