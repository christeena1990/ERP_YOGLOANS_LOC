using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class SOSalertController : BaseController
    {
        private DB dbconnect = new DB(); // Assuming db is your database connection class

        [HttpPost]
        public JsonResult SendSOSAlert()
        {
            int branchId = Convert.ToInt32(Session["branch_id"]);
            int empId = Convert.ToInt32(Session["login_user"]);
            string empName = Session["name"].ToString();
            string branchName = Session["branch"].ToString(); // Fetch branch name from session

            try
            {
                DateTime currentDateTime = (DateTime)dbconnect.ExecuteQuery("SELECT GETDATE()").Tables[0].Rows[0][0];

                SqlParameter[] pr2 = new SqlParameter[5]; // Adjust the size according to your needs

                pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr2[0].Value = 1;

                pr2[1] = new SqlParameter("@branch_id", SqlDbType.Int);
                pr2[1].Value = branchId;

                pr2[2] = new SqlParameter("@emp_id", SqlDbType.Int);
                pr2[2].Value = empId;

                pr2[3] = new SqlParameter("@emp_name", SqlDbType.VarChar, 50);
                pr2[3].Value = empName;

                pr2[4] = new SqlParameter("@branch_name", SqlDbType.VarChar, 50);
                pr2[4].Value = branchName;

                //pr2[5] = new SqlParameter("@current_dt", SqlDbType.DateTime);
                //pr2[5].Value = currentDateTime;

                dbconnect.ExecuteStoredProcedure("sos_alert", pr2);

                return Json(new { success = true, message = "SOS Alert Sent Successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, message = "Error sending SOS Alert: " + ex.Message });
            }
        }
    }
}