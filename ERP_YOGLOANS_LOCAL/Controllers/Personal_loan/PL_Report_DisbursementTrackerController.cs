using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_Report_DisbursementTrackerController : Controller
    {
        DB dbconnect = new DB();
        // GET: PL_Report_DisbursementTracker
        public ActionResult PL_DisbursementTracker()
        {

                //Function call for fill  stage category
                DataTable stage_types = fillStagedropdown();
                ViewData["StageList"] = stage_types;

                // Set the current date
                string datetoday = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                // Set the date 3 months ago from today
                string fromDate = DateTime.Now.AddMonths(-3).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

                // Store the dates in session
                Session["DateToday"] = datetoday;
                Session["FromDate"] = fromDate;

                // Optionally, if you need these values in the view for display or form inputs
                //ViewBag.DateToday = datetoday;
                //ViewBag.FromDate = fromDate;

                return View();
            }

            public DataTable fillStagedropdown()
            {

                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 3;
                dt = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }


        public ActionResult btn_report_Click(string fromDate, string toDate, int StageType)
        {
            try
            {
               // string branch = "1"; // Hard-coded branch list
                var branch = Session["CheckedFilterTreeValues"];

                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 };
                pr[1] = new SqlParameter("@fromdt", SqlDbType.DateTime) { Value = DateTime.Parse(fromDate) };
                pr[2] = new SqlParameter("@todt", SqlDbType.DateTime) { Value = DateTime.Parse(toDate) };
                pr[3] = new SqlParameter("@filter_type", SqlDbType.Int) { Value = StageType };
                pr[4] = new SqlParameter("@branch", SqlDbType.VarChar) { Value = branch };

                // Execute the stored procedure and get the DataTable
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr);
                DataTable dt = ds?.Tables[0];

                

                // Convert DataTable to a list of objects
                var result = dt.AsEnumerable().Select(row => new
                {
                    Region = row.Field<string>("Region"),
                    Branch = row.Field<string>("Branch"),
                    Loan = row.Field<string>("Loan"),
                    GlNo = row.Field<string>("GlNo"),
                    CustomerID = row.Field<long?>("CustomerID"), // Use nullable types if necessary
                    Name = row.Field<string>("Name"),
                    IntRate = row.Field<double?>("IntRate"),
                    LoanAmt = row.Field<double?>("LoanAmt"),
                    Outstanding_Amt = row.Field<double?>("Outstanding_Amt"),
                    LoanDt = row.Field<string>("LoanDt"),
                    MaturityDt = row.Field<string>("MaturityDt"),
                    Status = row.Field<string>("Status"),
                    CloseDt = row.Field<string>("CloseDt") ?? "N/A"
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


