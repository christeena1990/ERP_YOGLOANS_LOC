using ERP_YOGLOANS_LOCAL.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class UPI_Link_ReportController : Controller
    {//yfhgfhj
        // GET: UPI_Link_Report
        DB dbconnect = new DB();
        public ActionResult UPI_Link_ReportView()
        {
            int bfhjgh = 0;
            //Function call for fill  loan category
            DataTable stage_types = fillStagedropdown();
            ViewData["StageList"] = stage_types;

            // Set the current date
            string datetoday = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

            // Set the date 3 months ago from today
            string fromDate = DateTime.Now.AddMonths(-3).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

            // Store the dates in session
            Session["DateToday"] = datetoday;
            Session["FromDate"] = fromDate;



            return View();
        }
        public DataTable fillStagedropdown()
        {

            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[UPI_link_Transactions]", pr).Tables[0];
            dbconnect.Close();

            return dt;

        }
        public ActionResult btn_report_Click(string fromDate, string toDate, int StageType)
        {
            try
            {
                var branch = Session["CheckedFilterTreeValues"];

                SqlParameter[] pr = new SqlParameter[6];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };
                pr[1] = new SqlParameter("@From_dt", SqlDbType.DateTime) { Value = DateTime.Parse(fromDate) };
                pr[2] = new SqlParameter("@To_dt", SqlDbType.DateTime) { Value = DateTime.Parse(toDate) };
                pr[3] = new SqlParameter("@Stage", SqlDbType.Int) { Value = StageType };
                pr[4] = new SqlParameter("@branch", SqlDbType.VarChar, 5000) { Value = branch };
                pr[5] = new SqlParameter("@Module_id", SqlDbType.Int) { Value = Session["module_id"]?.ToString() };

               // int a = Convert.ToInt32(Session["module_id"]?.ToString());
                dbconnect.Open();
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[UPI_link_Transactions]", pr);
                dbconnect.Close();

                DataTable dt = ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;

               
                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
                }


                var result = dt.AsEnumerable().Select(row => new
                {
                    No = Convert.ToString(row["No"]),
                    LoanNo = row["LoanNo"] != DBNull.Value ? Convert.ToString(row["LoanNo"]) : null,
                    Customer = Convert.ToString(row["Customer"]),
                    Branch = row["Branch"] != DBNull.Value ? Convert.ToString(row["Branch"]) : null,
                    Mobile = row["Mobile"] != DBNull.Value ? Convert.ToString(row["Mobile"]) : null,
                    EMIAmount = row["EMIAmount"] != DBNull.Value ? Convert.ToString(row["EMIAmount"]) : null,
                    MessageAmt = row["MessageAmt"] != DBNull.Value ? Convert.ToString(row["MessageAmt"]) : null,
                    MessageDate = row["MessageDate"] != DBNull.Value ? Convert.ToString(row["MessageDate"]) : null,
                    LinkValidity = row["LinkValidity"] != DBNull.Value ? Convert.ToString(row["LinkValidity"]) : null,
                    PaidAmount= row["PaidAmount"] != DBNull.Value ? Convert.ToString(row["PaidAmount"]) : null,
                    ReceivedDate= row["ReceivedDate"] != DBNull.Value ? Convert.ToString(row["ReceivedDate"]) : null


                }).ToList();


                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        
    
    
    }
}