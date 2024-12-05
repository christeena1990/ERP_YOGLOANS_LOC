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

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_Report_SanctionTrackerController : Controller
    {
        DB dbconnect = new DB();
        // GET: PL_Report_SanctionTracker
        public ActionResult PL_SanctionTracker()
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
            pr[0].Value = 1;
            dt = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }

     


        public ActionResult btn_report_Click(string fromDate, string toDate, int StageType)
        {
            try
            {
                //string branch = "1"; // Hard-coded branch list

                var branch = Session["CheckedFilterTreeValues"];

                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 2;
                pr[1] = new SqlParameter("@fromdt", SqlDbType.DateTime);
                pr[1].Value = DateTime.Parse(fromDate) ;
                pr[2] = new SqlParameter("@todt", SqlDbType.DateTime);
                pr[2].Value = DateTime.Parse(toDate) ;
                pr[3] = new SqlParameter("@filter_type", SqlDbType.Int);
                pr[3].Value = StageType ;
                pr[4] = new SqlParameter("@branch", SqlDbType.VarChar);
                pr[4].Value = branch ;

                // Execute the stored procedure and get the DataTable
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr);
                DataTable dt = ds?.Tables[0];

                // Convert DataTable to a list of dynamic objects for JSON serialization
                var result = dt.AsEnumerable().Select(row => new
                {
                    Region = row.Field<string>("Region"),
                    Branch = row.Field<string>("Branch"),
                    AppID = row.Field<string>("AppID"),
                    CustID = row.IsNull("CustID") ? 0 : Convert.ToInt64(row["CustID"]), // Using Convert and null check
                    Name = row.Field<string>("Name"),
                    GoldNo = row.Field<string>("GoldNo"),
                    MaxLoan = row.IsNull("MaxLoan") ? 0 : Convert.ToDouble(row["MaxLoan"]), // Handling float with Convert
                    Amount = row.IsNull("Amount") ? 0 : Convert.ToDouble(row["Amount"]),
                    Doc_Chrg = row.IsNull("Doc_Chrg") ? 0 : Convert.ToDouble(row["Doc_Chrg"]),
                    MaturityDt = row.IsNull("MaturityDt") ? string.Empty : Convert.ToDateTime(row["MaturityDt"]).ToString("dd-MMM-yyyy"),
                    InterestRt = row.IsNull("InterestRt") ? 0 : Convert.ToDouble(row["InterestRt"]),
                    AppDt = row.IsNull("AppDt") ? string.Empty : Convert.ToDateTime(row["AppDt"]).ToString("dd-MMM-yyyy"),
                    Stage = row.Field<string>("Stage")
                }).ToList();

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        
    

public ActionResult PL_SanctionTracker_pdf()
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

        return new ViewAsPdf("PL_SanctionTracker_pdf")
        {
            FileName = "SanctionTrackerReport.pdf"
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