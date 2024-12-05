using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using static System.Net.Mime.MediaTypeNames;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Application_MoneyReceievedController : BaseController
    {
        DB dbconnect = new DB();
        // GET: NCD_Application_MoneyReceieved
        public ActionResult NCD_Application_MoneyReceieved()
        {
            //bankdropdown();
            var AccountMoneyreceivedDetails = ApplicantsGrid();
            ViewBag.AccountMoneyreceivedDetails = AccountMoneyreceivedDetails;

            return View();
        }


        public DataTable ApplicantsGrid()
        {

            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;


            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_application_HO]", pr).Tables[0];
            dbconnect.Close();
            return dt;
        }

       

        [HttpPost]
        public ActionResult ApplicantsGridwithtxtanddrpdwn(string app_id)
        {

            // Store app_id in the session
    Session["moneyapplicationid"] = app_id;
            // Define SQL parameters for the stored procedure
            SqlParameter[] pr = new SqlParameter[2];

            // First parameter: query_id
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2; // Assuming 2 is for fetching applicants

            // Second parameter: app_id
            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[1].Value = Convert.ToInt64(app_id); // Convert the app_id to long

            DataTable dt = new DataTable();

            // Open database connection
            dbconnect.Open();

            // Execute the stored procedure and fill the DataTable
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_application_HO]", pr).Tables[0];

            // Close the database connection
            dbconnect.Close();

            // Convert the DataTable to a list of dictionaries for JSON serialization
            var data = dt.AsEnumerable()
                         .Select(row => dt.Columns.Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]));

            // Return JSON with data and column names
            return Json(new
            {
                data = data,
                columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList()
            }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult bankdropdown()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;
            pr[1] = new SqlParameter("@branchid ", SqlDbType.VarChar);
            pr[1].Value = "0"; // Or pass the required branch ID if applicable

            DataTable dt = dbconnect.ExecuteDataset("[dbo].[service_charge_collection]", pr).Tables[0];

            // Convert DataTable to a list of key-value pairs for the dropdown
            var dropdownData = dt.AsEnumerable()
                                 .Select(row => new
                                 {
                                     Value = row["sub_accno"].ToString(),  // Assuming BankID is the value
                                     Text = row["sub_name"].ToString()  // Assuming BankName is the display text
                                 }).ToList();

            return Json(dropdownData, JsonRequestBehavior.AllowGet);
        }

      


        [HttpPost]
        public ActionResult SubmitAppMoneyReceived(string concatenatedData)
            
        {
            
            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[0].Value = Session["moneyapplicationid"];

            pr[1] = new SqlParameter("@type", SqlDbType.Int);
            pr[1].Value = 1;

            pr[2] = new SqlParameter("@data", SqlDbType.VarChar, 1000);
            pr[2].Value = concatenatedData; // Assign concatenatedData to the @data parameter

            pr[3] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[3].Value = Session["login_user"];

            pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[4].Direction = ParameterDirection.Output;

            // Execute the stored procedure
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_application_HO_save]", pr);
            
            
            // Check output message from stored procedure
            //string outputMessage = pr[4].Value.ToString();

            //if (outputMessage == "Submit successfully")
            //{
            //    return Json(new { success = true, message = outputMessage });
            //}
            //else
            //{

            //    return Json(new { success = false, message = outputMessage });
            //}



            string msg = pr[4].Value.ToString();


            if (msg == "Submit successfully")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }





        }





    }
}