using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class Form_MarkingController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult Form_Marking_View()
        {
            var customerBankDetails = Fill_Grid();

            // Check if DataTable has rows and columns
            if (customerBankDetails != null && customerBankDetails.Rows.Count > 0)
            {
                Console.WriteLine("DataTable Rows: " + customerBankDetails.Rows.Count);
                Console.WriteLine("DataTable Columns: " + customerBankDetails.Columns.Count);
            }
            else
            {
                Console.WriteLine("No data found in DataTable");
            }

            ViewBag.CustomerBankDetails = customerBankDetails;
            return View();
        }
        private DataTable Fill_Grid()
        {
            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        [HttpPost]
        public ActionResult StoreDataForBlankPage(string secondColumnData, string thirdColumnData)
        {
          
            Session["pan_no"] = secondColumnData;
            Session["Track_ID"] = thirdColumnData;

            return Json(new { success = true });
        }

    }
}