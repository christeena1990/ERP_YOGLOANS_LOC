
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
using System.ComponentModel;
using iTextSharp.text.pdf.parser.clipper;



namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
    {
        public class NCD_MonthlyInterest_EntryController : BaseController
    {
            // GET: NCD_Report_ApplicationStatus

            DB dbconnect = new DB();
            // GET: PL_Report_SanctionTracker
            public ActionResult NCD_MonthlyInterest_Entry()
            {
                //Function call for fill  loan category
                DataTable issue_types = fillissuedropdown();
                ViewData["IssueList"] = issue_types;


            DataTable bank_type = fillbankdropdown();
            ViewData["BankList"] = bank_type;



            // Set the current date
            string datetoday = DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                // Set the date 3 months ago from today
                string fromDate = DateTime.Now.AddMonths(-3).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

                // Store the dates in session
                Session["DateToday"] = datetoday;
                Session["FromDate"] = fromDate;



                return View();
            }





            public DataTable fillissuedropdown()
            {

                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 2;
                dt = dbconnect.ExecuteDataset("[dbo].[DEB_int_payment]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }


        public DataTable fillbankdropdown()
        {

            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;
            pr[1] = new SqlParameter("@branchid", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_int_payment]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }




        [HttpPost]
        public JsonResult GetCertificatesByIssue(int issue_no)
        {
            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;
            pr[1] = new SqlParameter("@issue_no", SqlDbType.Int);
            pr[1].Value = issue_no;

            dt = dbconnect.ExecuteDataset("[dbo].[DEB_int_payment]", pr).Tables[0];
            dbconnect.Close();

            var certificates = dt.AsEnumerable()
                .Select(row => new
                {
                    Value = row["certificate_no"],
                    Text = row["ctxt"] // Assuming these columns exist
                })
                .ToList();

            return Json(certificates);
        }



        [HttpGet]
        public ActionResult btn_report_Click(string month, int issueNo, int cert)

        {
            try
            {
               

                SqlParameter[] pr = new SqlParameter[4];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1;
                pr[1] = new SqlParameter("@month", SqlDbType.VarChar) ;
                pr[1].Value = month;
                pr[2] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                pr[2].Value = issueNo;
                pr[3] = new SqlParameter("@cert", SqlDbType.BigInt);
                pr[3].Value = cert;

                DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_int_payment]", pr);
                DataTable dt = ds != null && ds.Tables.Count > 0 ? ds.Tables[1] : null;
                DataTable dt1 = ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;

                // Check if the DataTable is not null and contains rows
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    // Assuming the value you need is in the first row and first column of dt1
                    var debentureCount = dt1.Rows[0][0];

                    // Store this single value in the session
                    Session["countofdebentures"] = debentureCount;
                }
                else
                {
                    // If dt1 is null or has no rows, handle accordingly
                    Session["countofdebentures"] = null; // Or any default value you prefer
                }

                // Check if the DataTable is null or empty
                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
                }


                var result = dt.AsEnumerable().Select(row => new
                {
                    Issue_No = Convert.ToString(row["Issue No"]),
                    Interest = row["Interest"] != DBNull.Value ? Convert.ToString(row["Interest"]) : null,
                    TDS = Convert.ToString(row["TDS"]),
                    Short_Recovery = row["Short Recovery"] != DBNull.Value ? Convert.ToString(row["Short Recovery"]) : null,
                    TotalTDS = row["Total TDS"] != DBNull.Value ? Convert.ToString(row["Total TDS"]) : null,
                    PayAmount = row["Pay Amount"] != DBNull.Value ? Convert.ToString(row["Pay Amount"]) : null,
                    DebentureAmount = row["Debenture Amount"] != DBNull.Value ? Convert.ToString(row["Debenture Amount"]) : null,
                    NoofDebentures = row["No.of Debentures"] != DBNull.Value ? Convert.ToString(row["No.of Debentures"]) : null,
                }).ToList();


                //return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);

                // Send back JSON with session value and result data
                return Json(new
                {
                    success = true,
                    data = result,
                    countofdebentures = Session["countofdebentures"] // Include session value here
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }







        //public ActionResult NCD_MonthlyInterestEntry_pdf()
        //    {
        //        // Retrieve the HTML string from session
        //        string htmlString = Session["HtmlString"] as string;
        //        string Month = Session["Month"] as string;
        //        string bankType = Session["bankType"] as string;
        //        string issueType = Session["issueType"] as string;
        //        string certificateType = Session["certificateType"] as string;


        //        ViewBag.HtmlString = htmlString;
        //        ViewBag.Month = Month;
        //        ViewBag.bankType = bankType;
        //        ViewBag.issueType = issueType;
        //        ViewBag.certificateType = certificateType;


        //        return new ViewAsPdf("NCD_MonthlyInterestEntry_pdf")
        //        {
        //            FileName = "MonthlyIntersest.pdf"
        //        };
        //    }




        //    [HttpPost]
        //    public ActionResult GeneratePdfClick(string Month, string issueType, string bankType, string certificateType, string htmlString)
        //    {


        //        string decodedHtmlString = HttpUtility.UrlDecode(htmlString);

        //        Session["HtmlString"] = decodedHtmlString;
        //        Session["currentDate"] = DateTime.Now.ToString("dd-MMM-yyyy");
        //        //Session["currentTime"] = DateTime.Now.ToString("HH:mm:ss");
        //        Session["currentTime"] = DateTime.Now.ToString("hh:mm:ss tt");


        //        Session["Month"] = Month;
        //        Session["bankType"] = bankType;
        //        Session["issueType"] = issueType;
        //        Session["certificateType"] = certificateType;

        //        return Json(new { success = true });
        //    }



        public ActionResult NCD_MonthlyInterestEntry_pdf()
        {
            // Retrieve data from session
            string htmlString = Session["HtmlString"] as string;
            string month = Session["Month"] as string;
            string bankType = Session["bankType"] as string;
            string issueType = Session["issueType"] as string;
            string certificateType = Session["certificateType"] as string;

            // Check if essential data exists
            if (string.IsNullOrEmpty(htmlString))
            {
                // Handle case when data is missing (redirect or show an error)
                return RedirectToAction("ErrorPage");
            }

            ViewBag.HtmlString = htmlString;
            ViewBag.Month = month;
            ViewBag.bankType = bankType;
            ViewBag.issueType = issueType;
            ViewBag.certificateType = certificateType;

            //return new ViewAsPdf("NCD_MonthlyInterestEntry_pdf")
            //{
            //    FileName = "MonthlyInterest.pdf",
            //    //CustomSwitches = "--load-error-handling ignore"
            //};

            return new ViewAsPdf("NCD_MonthlyInterestEntry_pdf")
            {
                FileName = "MonthlyInterest.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10),
                CustomSwitches = "--load-error-handling ignore"
            };


        }

        [HttpPost]
        public ActionResult GeneratePdfClick(string month, string issueType, string bankType, string certificateType, string htmlString)
        {
            // Decode HTML string if URL-encoded
            string decodedHtmlString = HttpUtility.UrlDecode(htmlString);

            // Store values in session for PDF generation
            Session["HtmlString"] = decodedHtmlString;
            Session["currentDate"] = DateTime.Now.ToString("dd-MMM-yyyy");
            Session["currentTime"] = DateTime.Now.ToString("hh:mm:ss tt");
            Session["Month"] = month;
            Session["bankType"] = bankType;
            Session["issueType"] = issueType;
            Session["certificateType"] = certificateType;

            return Json(new { success = true });
        }










        [HttpPost]
        public ActionResult btn_save_Click(string month, long issueNo, long cert, long bank_branch)
        {
            SqlParameter[] pr = new SqlParameter[8];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) ;
            pr[0].Value = 5;
            pr[1] = new SqlParameter("@month", SqlDbType.VarChar) ;
            pr[1].Value = month;
            pr[2] = new SqlParameter("@issue_no", SqlDbType.BigInt) ;
            pr[2].Value = issueNo;
            pr[3] = new SqlParameter("@cert", SqlDbType.BigInt) ;
            pr[3].Value = cert;
            pr[4] = new SqlParameter("@bank_branch", SqlDbType.BigInt) ;
            pr[4].Value = bank_branch;
            pr[5] = new SqlParameter("@enter_by", SqlDbType.BigInt) ;
            pr[5].Value = Session["login_user"] ;
            pr[6] = new SqlParameter("@login_branch_id", SqlDbType.BigInt);
            pr[6].Value = Session["branch_id"] ;
            pr[7] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[7].Direction = ParameterDirection.Output; // Set the output me

            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_int_payment]", pr);
            string outputMessage = pr[7].Value.ToString();
            // Return a JSON response to the AJAX success function
            return Json(new { success = true, message = "Data saved successfully" });
        }







    }
}