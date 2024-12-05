using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class history_pageloadController : Controller
    {
        // GET: history_pageload

        DB dbconnect = new DB();
        public ActionResult history_pageload()
        {

            return View();
        }
        public ActionResult GetSearchSuggestions(string search_txt)
        {
            // Define the SQL parameters as per your existing method
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 27 };
            pr[1] = new SqlParameter("@search_txt", SqlDbType.VarChar, 50) { Value = search_txt };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[customer_history]", pr).Tables[0];
            dbconnect.Close();
            // Convert the DataTable results to a list of strings for JSON response
            var result = dt.AsEnumerable().Select(row => row["Customer"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        [HttpPost]
        public ActionResult SetCustomerSession(string customerId)
        {
            // Store the input value in the session
            Session["customer_id_his"] = customerId;

            // Return a simple success response (optional)
            return Json(new { success = true });
        }
    }
}