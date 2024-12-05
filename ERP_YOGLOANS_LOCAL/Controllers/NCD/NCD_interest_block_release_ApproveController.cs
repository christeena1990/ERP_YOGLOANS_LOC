using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_interest_block_release_ApproveController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_interest_block_release_Approve()
        {
            return View();
        }

        [HttpGet]
        public JsonResult fill_blockGrid()
        {
            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 3;
            pr1[1] = new SqlParameter("@module_id", SqlDbType.BigInt);
            pr1[1].Value = Session["module_id"];

            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> issuelist = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt1.Rows)
            {
                Dictionary<string, object> issue = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    issue[col.ColumnName] = row[col];
                }
                issuelist.Add(issue);
            }

            // Return the data as JSON
            return Json(issuelist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult fill_releaseGrid()
        {
            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 8;
            pr1[1] = new SqlParameter("@module_id", SqlDbType.BigInt);
            pr1[1].Value = Session["module_id"];

            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> issuelist = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt1.Rows)
            {
                Dictionary<string, object> issue = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    issue[col.ColumnName] = row[col];
                }
                issuelist.Add(issue);
            }

            // Return the data as JSON
            return Json(issuelist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult viewCirtificate(string deb_id)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 11;

            pr[1] = new SqlParameter("@id", SqlDbType.BigInt);
            pr[1].Value = deb_id;

            DataTable dt = new DataTable();
            
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr).Tables[0];
            dbconnect.Close();
            string attachment = "";
            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No document available!" });

            }
            else
            {
                attachment = GetImageUrl(dt.Rows[0]["attachment"] as byte[]);

            }

            return Json(new { data = attachment }, JsonRequestBehavior.AllowGet);
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
        public JsonResult Block_ApproveClick(string deb_id)
        {
            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 4;
            pr1[1] = new SqlParameter("@blk_ap_rej_by", SqlDbType.BigInt);
            pr1[1].Value = Session["login_user"];
            pr1[2] = new SqlParameter("@out_msg", SqlDbType.VarChar, 50);
            pr1[2].Direction = ParameterDirection.Output;
            pr1[3] = new SqlParameter("@id", SqlDbType.BigInt);
            pr1[3].Value = deb_id;            

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_BOND_interest_blk_proc]", pr1);            
            dbconnect.Close();

            String msg = pr1[2].Value.ToString();

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult Block_RejectClick(string deb_id,string comments)
        {
            SqlParameter[] pr2 = new SqlParameter[5];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 5;
            pr2[1] = new SqlParameter("@blk_ap_rej_by", SqlDbType.BigInt);
            pr2[1].Value = Session["login_user"];
            pr2[2] = new SqlParameter("@out_msg", SqlDbType.VarChar, 500);
            pr2[2].Direction = ParameterDirection.Output;
            pr2[3] = new SqlParameter("@id", SqlDbType.BigInt);
            pr2[3].Value = deb_id;
            pr2[4] = new SqlParameter("@blk_req_reason", SqlDbType.NVarChar, 200);
            pr2[4].Value = comments;

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_BOND_interest_blk_proc]", pr2);
            dbconnect.Close();

            String msg = pr2[2].Value.ToString();

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public JsonResult Release_ApproveClick(string deb_id)
        {
            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 9;
            pr1[1] = new SqlParameter("@rel_ap_rej_by", SqlDbType.BigInt);
            pr1[1].Value = Session["login_user"];
            pr1[2] = new SqlParameter("@out_msg", SqlDbType.VarChar, 50);
            pr1[2].Direction = ParameterDirection.Output;
            pr1[3] = new SqlParameter("@id", SqlDbType.BigInt);
            pr1[3].Value = deb_id;

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_BOND_interest_blk_proc]", pr1);
            dbconnect.Close();

            String msg = pr1[2].Value.ToString();

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Release_RejectClick(string deb_id,string comments)
        {
            SqlParameter[] pr2 = new SqlParameter[5];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 10;
            pr2[1] = new SqlParameter("@rel_ap_rej_by", SqlDbType.BigInt);
            pr2[1].Value = Session["login_user"];
            pr2[2] = new SqlParameter("@out_msg", SqlDbType.VarChar, 500);
            pr2[2].Direction = ParameterDirection.Output;
            pr2[3] = new SqlParameter("@id", SqlDbType.BigInt);
            pr2[3].Value = deb_id;
            pr2[4] = new SqlParameter("@rel_rej_reason", SqlDbType.NVarChar, 200);
            pr2[4].Value = comments;

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[DEB_BOND_interest_blk_proc]", pr2);
            dbconnect.Close();

            String msg = pr2[2].Value.ToString();

            return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);

        }
    }
}