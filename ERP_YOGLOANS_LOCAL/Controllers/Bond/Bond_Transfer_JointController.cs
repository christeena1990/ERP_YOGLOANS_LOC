using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_Transfer_JointController : BaseController
    {
        DB dbconnect = new DB();
        // GET: Bond_Trasfer_Joint
        public ActionResult Bond_Transfer_Joint()
        {

                //Session["application_subid"] = 162;

                var JointaccountDetails = ApplicantsGrid();
                ViewBag.JointaccountDetails = JointaccountDetails;

                // Check if the session variable 'application_custId_162' is not null
                if (Session["application_custId_162"] != null)
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
            Session["application_subid"] = 162;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }




        public DataTable ApplicantsGrid()
            {

                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 4;

                pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
                pr[1].Value = Session["branch_id"]; // Ensure branch_id is converted to an integer

                DataTable dt = new DataTable();
                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }

            //[HttpPost]
            //public ActionResult ExistingjointApplicantsGrid(string applicationId)
            //{
            //    SqlParameter[] pr = new SqlParameter[2];

            //    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            //    pr[0].Value = 5;

            //    pr[1] = new SqlParameter("@trans_id", SqlDbType.BigInt);
            //    pr[1].Value = Convert.ToInt64(applicationId);

            //    DataTable dt = new DataTable();
            //    dbconnect.Open();
            //    dt = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr).Tables[0];
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
                pr[0].Value = 7;

                pr[1] = new SqlParameter("@customer_id", SqlDbType.Int);
                pr[1].Value = Session["application_custId_162"]; // Ensure branch_id is converted to an integer

                DataTable dt = new DataTable();
                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }

            public ActionResult JointAplcnt_Btn_clk(string applicationIdTextbox, string jointApplicantname, string issue_no, string certif, string bond_id)
            {
                SqlParameter[] prr = new SqlParameter[8];

                prr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                prr[0].Value = 3;

                prr[1] = new SqlParameter("@transfer_id", SqlDbType.BigInt);
                prr[1].Value = Convert.ToInt64(applicationIdTextbox); // Pass the application ID from the form input

                prr[2] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                prr[2].Value = Session["application_custId_162"]; // Assuming you have the customer ID stored in the session

                prr[3] = new SqlParameter("@enter_by", SqlDbType.BigInt);
                prr[3].Value = Session["login_user"]; // Assuming you have the user ID stored in the session

                prr[4] = new SqlParameter("@issue_no", SqlDbType.VarChar, 4000);
                prr[4].Value = issue_no; // Pass the applicant name from the form input

                prr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                prr[5].Direction = ParameterDirection.Output; // Set the output message


                prr[6] = new SqlParameter("@certif", SqlDbType.VarChar, 4000);
                prr[6].Value = certif; // Pass the applicant name from the form input

                prr[7] = new SqlParameter("@bond_id", SqlDbType.VarChar, 4000);
                prr[7].Value = bond_id; // Pass the applicant name from the form input



                // Execute the stored procedure
                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_transfer_save]", prr);

                // Check output message from stored procedure
                string outputMessage = prr[5].Value.ToString();




            //////////////  alert when success or error    /////////

            if (outputMessage == "Saved Successfully")
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
            Session.Remove("application_custId_162");

                // Instead of redirecting, return JSON with the message
                //return Json(new { success = true, message = outputMessage });
                return RedirectToAction("Bond_Transfer_Joint");
            }

        [HttpPost]
        public JsonResult ClearSession()
        {
            // Clear the session variable
            Session.Remove("application_custId_162");

            // Return a success response
            return Json(new { success = true });
        }




    }
}






