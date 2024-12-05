using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{
    public class HR_Cashier_ApproveController : Controller
    {

        private DB dbconnect = new DB(); // Initialize your DbContext

        // GET: HR_Cashier_Assign
        public ActionResult Cashier_Approve()
        {
            var branch_id = Session["branch_id"];
            var login_user = Session["login_user"];

            FillGrid(); // Call the FillGrid method to populate the ViewBag data
            return View();
        }

        private void FillGrid()
        {
            

            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 3;
            pr[1] = new SqlParameter("@branch", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];

            DataTable dt = new DataTable();
            // Assuming ExecuteDataset is a method in your db class
            dt = dbconnect.ExecuteDataset("[dbo].[HR_branch_cashier_allowance]", pr).Tables[0];

            ViewBag.CashierData = dt; // Store the DataTable in ViewBag
        }

        public ActionResult Logout()
        {
            // Perform any logout logic here (e.g., clearing session, cookies, etc.)

            // Redirect to the login page or any other desired page
            return RedirectToAction("Login", "Smart_Login"); // Assuming "Login" is the action and "Account" is the controller for your login page
        }


        [HttpPost]
        public ActionResult Approve()
        {
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 4;
            pr[1] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[1].Value = Session["login_user"];
            pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr[2].Direction = ParameterDirection.Output;

            // Execute stored procedure
            dbconnect.ExecuteStoredProcedure("[dbo].[HR_branch_cashier_allowance]", pr);

            // Retrieve output parameter value
            string outMsg = pr[2].Value.ToString();

            return Json(new { success = true, message = outMsg }); // Assuming you want to return JSON
        }

        [HttpPost]
        public ActionResult Reject()
        {
            // Get the logged-in user's ID from the session
            int userId = Convert.ToInt32(Session["login_user"]);

            // Initialize the output message
            string outMsg = string.Empty;

            try
            {
                // Call the reject stored procedure
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 6;
                pr[1] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[1].Value = Session["login_user"];
                pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[2].Direction = ParameterDirection.Output;

                // Execute the stored procedure
                dbconnect.ExecuteStoredProcedure("[dbo].[HR_branch_cashier_allowance]", pr);

                // Get the output message from the stored procedure
                outMsg = pr[2].Value.ToString();

                if (outMsg.StartsWith("Success"))
                {
                    // Redirect to logout page
                    return RedirectToAction("Logout", "Smart_Login");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                outMsg = "Error: " + ex.Message;
            }

            // Return the output message as JSON
            return Json(new { message = outMsg });
        }
    }
    
}
