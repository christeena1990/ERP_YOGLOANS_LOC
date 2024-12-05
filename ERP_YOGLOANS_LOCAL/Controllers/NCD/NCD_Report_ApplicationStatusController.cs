
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Globalization;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;
using System.Web.UI;
using Rotativa;



namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Report_ApplicationStatusController : Controller
    {
        // GET: NCD_Report_ApplicationStatus
   
        DB dbconnect = new DB();
        // GET: PL_Report_SanctionTracker
        public ActionResult NCD_Report_ApplicationStatus()
        {
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
            pr[0].Value = 1;
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }




        public ActionResult btn_report_Click(string fromDate, string toDate, int StageType)
        {
            try
            {
                var branch = Session["CheckedFilterTreeValues"];

                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 2 };
                pr[1] = new SqlParameter("@start", SqlDbType.DateTime) { Value = DateTime.Parse(fromDate) };
                pr[2] = new SqlParameter("@end", SqlDbType.DateTime) { Value = DateTime.Parse(toDate) };
                pr[3] = new SqlParameter("@type", SqlDbType.Int) { Value = StageType };
                pr[4] = new SqlParameter("@branch", SqlDbType.VarChar, 5000) { Value = branch };

                DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr);
                DataTable dt = ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;

                // Check if the DataTable is null or empty
                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
                }

              
                var result = dt.AsEnumerable().Select(row => new
                {
                    branch_name = Convert.ToString(row["branch_name"]),
                    application_id = row["application_id"] != DBNull.Value ? Convert.ToString(row["application_id"]) : null,
                    appmoney_no = Convert.ToString(row["appmoney_no"]),
                    customer_name = row["customer_name"] != DBNull.Value ? Convert.ToString(row["customer_name"]) : null,
                    customer_id = row["customer_id"] != DBNull.Value ? Convert.ToString(row["customer_id"]) : null,
                    deb_amount = row["deb_amount"] != DBNull.Value ? Convert.ToString(row["deb_amount"]) : null,
                    issue_no = row["issue_no"] != DBNull.Value ? Convert.ToString(row["issue_no"]) : null,
                    Interest_type = row["Interest_type"] != DBNull.Value ? Convert.ToString(row["Interest_type"]) : null,
                    STATUS = row["STATUS"] != DBNull.Value ? Convert.ToString(row["STATUS"]) : null
                }).ToList();


                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public ActionResult NCD_Applicationstatusreport_pdf()
        {
            // Retrieve the HTML string from session
            string htmlString = Session["HtmlString"] as string;
            string fromDate = Session["fromDate"] as string;
            string toDate = Session["toDate"] as string;
            string stageType = Session["StageType"] as string;
            string totalAmount = Session["totalAmount"] as string;
            string totalCount = Session["totalCount"] as string;
            string currentDate = Session["currentDate"] as string;
            string currentTime = Session["currentTime"] as string;

            ViewBag.HtmlString = htmlString;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.StageType = stageType;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentDate = currentDate;
            ViewBag.CurrentTime = currentTime;

            return new ViewAsPdf("NCD_Applicationstatusreport_pdf")
            {
                FileName = "ApplicationStatusReport.pdf"
            };
        }

        [HttpPost]
        public ActionResult GeneratePdfClick(string fromDate, string toDate, string StageType, string totalAmount, string totalCount, string htmlString)
        {
            DateTime fromDateParsed = DateTime.Parse(fromDate);
            DateTime toDateParsed = DateTime.Parse(toDate);

            string decodedHtmlString = HttpUtility.UrlDecode(htmlString);

            Session["HtmlString"] = decodedHtmlString;
            Session["currentDate"] = DateTime.Now.ToString("dd-MMM-yyyy");
            //Session["currentTime"] = DateTime.Now.ToString("HH:mm:ss");
            Session["currentTime"] = DateTime.Now.ToString("hh:mm:ss tt");

            Session["fromDate"] = fromDateParsed.ToString("dd-MMM-yyyy");
            Session["toDate"] = toDateParsed.ToString("dd-MMM-yyyy");
            Session["StageType"] = StageType;
            Session["totalAmount"] = totalAmount;
            Session["totalCount"] = totalCount;

            return Json(new { success = true });
        }





    }
}