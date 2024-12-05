using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.IO;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class BOND_Settlment_SanctionController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult BOND_Settlment_Sanction_View()
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
            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            pr[2] = new SqlParameter("@sett", SqlDbType.Char);
            pr[2].Value = 'H';

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        [HttpPost]
        public JsonResult textBox_fill(string bond_id)
        {
            Session["bond_id"] = bond_id;

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@bond_id", SqlDbType.BigInt);
            pr[1].Value = bond_id;

            DataSet ds = new DataSet();

            try
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[BOND_settlement_queries]", pr);
            }
            finally
            {
                dbconnect.Close();
            }
            if (ds == null || ds.Tables.Count == 0)
            {
                return Json(new { success = false, message = "No data found" }, JsonRequestBehavior.AllowGet);
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
            TempData["start"] = ds.Tables[0].Rows[0]["start"].ToString();
            TempData["lastintamt"] = ds.Tables[0].Rows[0]["lastintamt"].ToString();
            return Json(new { success = true, data = resultData, }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult panORform60_status()
        {
      

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@bond_id", SqlDbType.BigInt);
            pr[1].Value = Session["bond_id"];

            DataSet ds = new DataSet();

           
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[BOND_settlement_queries]", pr);
                dbconnect.Close();

            var resultData = new List<object>();


            resultData.Add(new
            {

                //panorform60_type = ds.Tables[0].Rows[0]["type"].ToString(),
                //type_form = ds.Tables[0].Rows[0]["type_form"].ToString(),
                //idno = ds.Tables[0].Rows[0]["idno"].ToString(),
                //return_reason = ds.Tables[0].Rows[0]["ReturnReason"].ToString(),

                panorform60_type = ds.Tables[0].Rows[0]["type"]?.ToString() ?? string.Empty,
                type_form = ds.Tables[0].Rows[0]["type_form"]?.ToString() ?? string.Empty,
                idno = ds.Tables[0].Rows[0]["idno"]?.ToString() ?? string.Empty,
                return_reason = ds.Tables[0].Rows[0]["ReturnReason"]?.ToString() ?? string.Empty,


            }); ; ;

            return Json(new { success = true,data= resultData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult viewCirtificate()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;

            pr[1] = new SqlParameter("@bond_id", SqlDbType.BigInt);
            pr[1].Value = Session["bond_id"];

            DataTable dt = new DataTable();
            ncd_branch_approval_model model = new ncd_branch_approval_model();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No documnet available!" });

            }
            else
            {
                model.ImageUrl2 = GetImageUrl(dt.Rows[0]["attachment"] as byte[]);

            }

            return Json(new { data = model.ImageUrl2 }, JsonRequestBehavior.AllowGet);
        }
        private bool IsPdf(byte[] byteData)
        {
            // Check if the byte array represents a PDF file
            return byteData.Length > 4 &&
                   byteData[0] == 0x25 &&
                   byteData[1] == 0x50 &&
                   byteData[2] == 0x44 &&
                   byteData[3] == 0x46 &&
                   byteData[4] == 0x2D;
        }
        private string GetImageUrl(byte[] imageBytes)
        {


            string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

            if (IsPdf(imageBytes))
            {
                // It's a PDF
                string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                //kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                return $"data:application/pdf;base64,{base64String}";

            }
            else
            {
                // It's an image
                string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                // kycItem.Add("image", "data:image/png;base64," + base64Image);
                return $"data:image;base64,{base64String}";
            }
        }
        [HttpPost]
        public JsonResult requestBtn_click( string settl_Amt, string total_int,
    string tot_tds, string total_paidamt, string total_payable, string tot_eranint,
    string lasttds, string short_recovery, String trid,String bal,string MaturityDt)
        {           

            SqlParameter[] pr = new SqlParameter[18];

            pr[0] = new SqlParameter("@bond_id", SqlDbType.BigInt);
            pr[0].Value = Session["bond_id"];

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
            pr[6].Value = "M";

            pr[7] = new SqlParameter("@balance", SqlDbType.Float);
            pr[7].Value = bal;

            pr[8] = new SqlParameter("@intamt", SqlDbType.Float);
            pr[8].Value = TempData["lastintamt"];

            //pr[7] = new SqlParameter("@pan", SqlDbType.VarChar, 10);
            //pr[7].Value = panData;
            //if (panData != "")
            //{
            //    pr[8] = new SqlParameter("@trackid", SqlDbType.BigInt);
            //    pr[8].Value = 0;
            //}
            //else
            //{
            //    pr[8] = new SqlParameter("@trackid", SqlDbType.BigInt);
            //    pr[8].Value = trid;
            //}

            pr[9] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[9].Value = Session["login_user"];

            pr[10] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[10].Value = Session["branch_id"];
            //if (panData != "")
            //{
            //    pr[11] = new SqlParameter("@type", SqlDbType.Int);
            //    pr[11].Value = "1";
            //}
            //else
            //{
            //    pr[11] = new SqlParameter("@type", SqlDbType.Int);
            //    pr[11].Value = "2";
            //}
            pr[11] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[11].Value = 2;

            pr[12] = new SqlParameter("@intfrom", SqlDbType.VarChar, 100);
            pr[12].Value = TempData["start"]; 

            pr[13] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[13].Direction = ParameterDirection.Output;

            pr[14] = new SqlParameter("@total_eraned", SqlDbType.Float);
            pr[14].Value = tot_eranint;

            pr[15] = new SqlParameter("@settat", SqlDbType.Char, 1);
            pr[15].Value = "H";

            pr[16] = new SqlParameter("@tdspay", SqlDbType.Float);
            pr[16].Value = tot_tds;

            pr[17] = new SqlParameter("@short", SqlDbType.Float);
            pr[17].Value = short_recovery;



            //pr[18] = new SqlParameter("@attachment", SqlDbType.VarBinary) { Value = fileBytes };
            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[BOND_settlement_save]", pr);
            dbconnect.Close();
            String msg = pr[13].Value.ToString();
            if (msg == "Submit Successfully ")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Reject_click(string rejectReason)
        {

            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@bond_id", SqlDbType.BigInt);
            pr[1].Value = Session["bond_id"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[3].Direction = ParameterDirection.Output;

            pr[4] = new SqlParameter("@reason", SqlDbType.NVarChar, 500);  // New parameter for rejection reason
            pr[4].Value = rejectReason;

            DataSet ds = new DataSet();

            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[BOND_settlement_save]", pr);
            dbconnect.Close();

            String msg = pr[3].Value.ToString();

            if (msg == "Rejected Successfully ")
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