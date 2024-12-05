using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Diagnostics;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.IO;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Settlment_RequestController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_Settlment_Request_View()
        {

            var customerBankDetails = BOND_transfer_Approval_Grid();

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
        private DataTable BOND_transfer_Approval_Grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        public ActionResult GetSearchSuggestions(string search_txt)
        {
            // Define the SQL parameters as per your existing method
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3};
            pr[1] = new SqlParameter("@searchdata", SqlDbType.VarChar, 50) { Value = search_txt };
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = Session["branch_id"] };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();
            // Create a list to hold the results
            var result = dt.AsEnumerable().Select(row => new
            {
                Name = row["name"].ToString(),
                DebId = row["deb_id"].ToString() // Ensure you get the DebID
            }).ToList();

         
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult locking_test(String issue_no, String certifi_no,String deb_id)
        {
            Session["deb_id"] = deb_id;
            Session["certifi_no_premeture"] = certifi_no;
            Session["issue_no_premeture"] = issue_no;
            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@issuno", SqlDbType.Int);
            pr[1].Value = issue_no;

            pr[2] = new SqlParameter("@cerno", SqlDbType.Int);
            pr[2].Value = certifi_no;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            string msg = dt.Rows[0]["msg"].ToString();
            if (msg == "0")
            {
                return Json(new { data = msg, success = true }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new {data= msg,  success = false }, JsonRequestBehavior.AllowGet);

            }


        }
        [HttpPost]
        public ActionResult text_box_fill_premeture()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["deb_id"];

            DataSet ds = new DataSet();

            try
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr);
            }
            finally
            {
                dbconnect.Close();
            }
            if (ds == null || ds.Tables.Count == 0)
            {
                return Json(new { success = false, message = "No data found" }, JsonRequestBehavior.AllowGet);
            }
            else if (ds.Tables[1].Rows[0]["lien"].ToString() == "Y")
            {
                return Json(new { success = false, message = "This Debenture Against Lien Marked" }, JsonRequestBehavior.AllowGet);
            }
            ncd_branch_approval_model model = new ncd_branch_approval_model();

            var resultData = new List<object>();


            resultData.Add(new
            {

                interstamt = ds.Tables[0].Rows[0]["interstamt"].ToString(),
                eranint = ds.Tables[0].Rows[0]["eranint"].ToString(),
                tot_tds = ds.Tables[0].Rows[0]["tot_tds"].ToString(),
                payamt = ds.Tables[0].Rows[0]["payamt"].ToString(),
                balpay = ds.Tables[0].Rows[0]["balpay"].ToString(),
                pan = ds.Tables[0].Rows[0]["pan"].ToString(),
                settpay = ds.Tables[0].Rows[0]["settpay"].ToString(),
                trid = ds.Tables[0].Rows[0]["trid"].ToString(),
                pantrack = ds.Tables[0].Rows[0]["pantrack"].ToString(),
                start = ds.Tables[0].Rows[0]["start"].ToString(),
                lastintamt = ds.Tables[0].Rows[0]["lastintamt"].ToString(),
                lasttds = ds.Tables[0].Rows[0]["lasttds"].ToString(),
                short_recovery = ds.Tables[0].Rows[0]["short_recovery"].ToString(),
                tds_accrued = ds.Tables[0].Rows[0]["tds_accrued"].ToString(),
                pre_intrate = ds.Tables[0].Rows[0]["intrate"].ToString(),
                preamount = ds.Tables[0].Rows[0]["preamount"].ToString(),


                maturity_amount = ds.Tables[1].Rows[0]["maturity_amount"].ToString(),
                lien = ds.Tables[1].Rows[0]["lien"].ToString(),
                customer_id = ds.Tables[1].Rows[0]["customer_id"].ToString(),
                customer_name = ds.Tables[1].Rows[0]["customer_name"].ToString(),
                interest_rate = ds.Tables[1].Rows[0]["interest_rate"].ToString(),
                period = ds.Tables[1].Rows[0]["period"].ToString(),
                amount = ds.Tables[1].Rows[0]["amount"].ToString(),
                interest_type = ds.Tables[1].Rows[0]["interest_type"].ToString(),
                allotment_date = ds.Tables[1].Rows[0]["allotment_date"].ToString(),
                maturity_date = ds.Tables[1].Rows[0]["maturity_date"].ToString(),
                cirtificate_no = Session["certifi_no_premeture"].ToString(),
                issue_no = Session["issue_no_premeture"].ToString(),
            }) ; ; ;

            return Json(new { success = true, data = resultData, }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult textBox_fill_meture(string bed_id)
        {
            Session["deb_id"] = bed_id;
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = bed_id;

            DataSet ds = new DataSet();

            try
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr);
            }
            finally
            {
                dbconnect.Close();
            }
            if (ds == null || ds.Tables.Count == 0)
            {
                return Json(new { success = false, message = "No data found" }, JsonRequestBehavior.AllowGet);
            }
            else if (ds.Tables[1].Rows[0]["lien"].ToString() == "Y")
            {
                return Json(new { success = false, message = "This Debenture Against Lien Marked" }, JsonRequestBehavior.AllowGet);
            }
            ncd_branch_approval_model model = new ncd_branch_approval_model();

            var resultData = new List<object>();


            resultData.Add(new
            {

                interstamt = ds.Tables[0].Rows[0]["interstamt"].ToString(),
                eranint = ds.Tables[0].Rows[0]["eranint"].ToString(),
                tot_tds = ds.Tables[0].Rows[0]["tot_tds"].ToString(),
                payamt = ds.Tables[0].Rows[0]["payamt"].ToString(),
                balpay = ds.Tables[0].Rows[0]["balpay"].ToString(),
                pan = ds.Tables[0].Rows[0]["pan"].ToString(),
                settpay = ds.Tables[0].Rows[0]["settpay"].ToString(),
                trid = ds.Tables[0].Rows[0]["trid"].ToString(),
                pantrack = ds.Tables[0].Rows[0]["pantrack"].ToString(),
                start = ds.Tables[0].Rows[0]["start"].ToString(),
                lastintamt = ds.Tables[0].Rows[0]["lastintamt"].ToString(),
                lasttds = ds.Tables[0].Rows[0]["lasttds"].ToString(),
                short_recovery = ds.Tables[0].Rows[0]["short_recovery"].ToString(),
                tds_accrued = ds.Tables[0].Rows[0]["tds_accrued"].ToString(),

                maturity_amount = ds.Tables[1].Rows[0]["maturity_amount"].ToString(),
                lien = ds.Tables[1].Rows[0]["lien"].ToString(),

               
            }); ; ;

            return Json(new { success = true, data = resultData, }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult requestBtn_click(string panData, string settl_Amt, string total_int,
          string tot_tds, string total_paidamt, string total_payable, string tot_eranint,
          string lasttds, string short_recovery, String trid,String close_type)
        {
           
            SqlParameter[] pr = new SqlParameter[18];

            pr[0] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[0].Value = Session["deb_id"];

            pr[1] = new SqlParameter("@total_int", SqlDbType.Float);
            pr[1].Value = total_int;

            pr[2] = new SqlParameter("@total_tds", SqlDbType.Float);
            pr[2].Value = lasttds;

            pr[3] = new SqlParameter("@total_paidamt", SqlDbType.Float);
            pr[3].Value = total_paidamt;

            pr[4] = new SqlParameter("@total_payable", SqlDbType.Float);
            pr[4].Value = total_payable;

            pr[5] = new SqlParameter("@settle_amt", SqlDbType.Float);
            pr[5].Value = settl_Amt;

            pr[6] = new SqlParameter("@close_type", SqlDbType.Char, 1);
            pr[6].Value = close_type;

            pr[7] = new SqlParameter("@pan", SqlDbType.VarChar, 10);
            pr[7].Value = panData;
            if (panData != "")
            {
                pr[8] = new SqlParameter("@trackid", SqlDbType.BigInt);
                pr[8].Value = 0;
            }
            else
            {
                pr[8] = new SqlParameter("@trackid", SqlDbType.BigInt);
                pr[8].Value = trid;
            }

            pr[9] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[9].Value = Session["login_user"];

            pr[10] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[10].Value = Session["branch_id"];
            if (panData != "")
            {
                pr[11] = new SqlParameter("@type", SqlDbType.Int);
                pr[11].Value = "1";
            }
            else
            {
                pr[11] = new SqlParameter("@type", SqlDbType.Int);
                pr[11].Value = "2";
            }

            pr[12] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[12].Direction = ParameterDirection.Output;

            pr[13] = new SqlParameter("@total_eraned", SqlDbType.Float);
            pr[13].Value = tot_eranint;

            pr[14] = new SqlParameter("@settat", SqlDbType.Char, 1);
            pr[14].Value = "H";

            pr[15] = new SqlParameter("@tdspay", SqlDbType.Float);
            pr[15].Value = tot_tds;

            pr[16] = new SqlParameter("@short", SqlDbType.Float);
            pr[16].Value = short_recovery;

            pr[17] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[17].Value = 1;

          //  pr[18] = new SqlParameter("@attachment", SqlDbType.VarBinary) { Value = fileBytes };

            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_settlement_save]", pr);
            String msg = pr[12].Value.ToString();
            if (msg == "Requested Successfully ")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}