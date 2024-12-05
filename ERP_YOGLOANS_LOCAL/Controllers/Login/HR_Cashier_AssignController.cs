using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models; // Assuming this is your EF model namespace

namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{
    public class HR_Cashier_AssignController : Controller
    {
        private DB dbconnect = new DB(); // Initialize your DbContext

        // GET: HR_Cashier_Assign
        public ActionResult Cashier_Assign()
        {
          
            FillGrid(); // Call the FillGrid method to populate the ViewBag data
            return View();
        }

        private void FillGrid()
        {
            var branch_id = Session["branch_id"];
            var login_user = Session["login_user"];


            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@branch", SqlDbType.BigInt);
            pr[1].Value = branch_id;
            pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[2].Value = login_user;

            // Example of using Entity Framework for executing stored procedure
            DataSet ds = dbconnect.ExecuteDataset("[dbo].[HR_branch_cashier_allowance]", pr);

            if (ds.Tables.Count >= 2) // Ensure there are at least 2 tables in the DataSet
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    ViewBag.GridData = dt;
                }
                else
                {
                    ViewBag.GridData = null;
                }

                var dt5 = ds.Tables[1]; //existing cashier dtls
                if (dt5.Rows.Count > 0)
                {
                    ViewBag.CashierLabel = " Cashier Approval waiting for " + dt5.Rows[0][0].ToString() + "... If you want to change cashier please assign from below list.";
                }
                else
                {
                    ViewBag.CashierLabel = null;
                }
            }
            else
            {
                // Handle the case where there are not enough tables in the DataSet
                ViewBag.GridData = null;
                ViewBag.CashierLabel = null;
            }
        }
        public ActionResult Logout()
        {
            // Perform any logout logic here (e.g., clearing session, cookies, etc.)

            // Redirect to the login page or any other desired page
            return RedirectToAction("Login", "Smart_Login"); // Assuming "Login" is the action and "Account" is the controller for your login page
        }
        [HttpPost]
        public ActionResult AssignCashiers(string selectedEmploye)
       {
            try
            {
               
                   // Get branch ID and user ID from session
                    long branch_id = Convert.ToInt64(Session["branch_id"]);
                    long login_user = Convert.ToInt64(Session["login_user"]);

                    long cashierIdLong = Convert.ToInt64(selectedEmploye);

                    // Execute stored procedure
                    SqlParameter[] pr = new SqlParameter[5];
                    pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr[0].Value = 2;
                    pr[1] = new SqlParameter("@branch", SqlDbType.BigInt);
                    pr[1].Value = branch_id;
                    pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
                    pr[2].Value = login_user;
                    pr[3] = new SqlParameter("@cashier", SqlDbType.VarChar, -1);
                    pr[3].Value = cashierIdLong;
                    pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    pr[4].Direction = ParameterDirection.Output;

                    // Execute stored procedure
                    dbconnect.ExecuteStoredProcedure("[dbo].[HR_branch_cashier_allowance]", pr);
                    string outmsg = pr[4].Value.ToString();
                    //if (!string.IsNullOrEmpty(outmsg))
                    //{
                        return Json(new { success = true, message = outmsg });
                    //}
                //}            

               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



    }
}
