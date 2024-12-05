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
    public class NCD_Tranfer_ApprovalController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_Tranfer_Approval_View()
        {
            var customerBankDetails = NCD_transfer_Approval_Grid();

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
        private DataTable NCD_transfer_Approval_Grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 9;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        [HttpGet]
        public JsonResult SetCustId_to_Session()
        {

            Session["Apr_custID"] = Session["customer_id_br_approval"];

            // Return the data as JSON
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SetCustId_to_Session1(string TRANSFERROR_customer_id)
        {

            Session["Apr_custID"] = TRANSFERROR_customer_id;

            // Return the data as JSON
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //return Json(customerList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Approvan_branch_textBox_fill(string Transfer_id, string customer_id)
        {
            Session["@trans_id_tran_appro"] = Transfer_id;
            Session["customer_id_br_approval"] = customer_id;
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 10;

            pr[1] = new SqlParameter("@trans_id", SqlDbType.BigInt);
            pr[1].Value = Transfer_id;

            DataSet ds = new DataSet();

            try
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr);
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
            if (ds.Tables[0].Rows.Count > 0)
            {

                resultData.Add(new
                {

                
                    trans_id = Transfer_id,
                    transtype = ds.Tables[0].Rows[0]["transtype"].ToString(),
                    TRANSFERROR_customer_id = ds.Tables[0].Rows[0]["TRANSFERROR_customer_id"].ToString(),
                    TRANSFERROR_name = ds.Tables[0].Rows[0]["TRANSFERROR_name"].ToString(),
                    Transferror_pan = ds.Tables[0].Rows[0]["Transferror_pan"].ToString(),
                    transferee_customer_id = ds.Tables[0].Rows[0]["transferee_customer_id"].ToString(),
                    transferee_name = ds.Tables[0].Rows[0]["transferee_name"].ToString(),
                    transferee_pan = ds.Tables[0].Rows[0]["transferee_pan"].ToString(),
                    transferee_dob_age = ds.Tables[0].Rows[0]["transferee_dob_age"].ToString(),
                    as15status = ds.Tables[0].Rows[0]["as15status"].ToString(),
                    taxpayee = ds.Tables[0].Rows[0]["taxpayee"].ToString(),
                    noof_applicant = ds.Tables[0].Rows[0]["noof_applicant"].ToString(),
                    repayment = ds.Tables[0].Rows[0]["repayment"].ToString(),
                    lien = ds.Tables[0].Rows[0]["lien"].ToString(),
                    nominee_Name = ds.Tables[0].Rows[0]["nominee_Name"].ToString(),
                    nominee_add = ds.Tables[0].Rows[0]["nominee_add"].ToString(),
                    relation = ds.Tables[0].Rows[0]["relation"].ToString(),
                    dp_id = ds.Tables[0].Rows[0]["dp_id"].ToString(),
                    dp_name = ds.Tables[0].Rows[0]["dp_name"].ToString(),


                }); ; ;
            }

            var table2Data = new
            {
                headings = ds.Tables[1].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                rows = ds.Tables[1].Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(item => item.ToString()).ToList()).ToList()
            };

            var table4Data = new
            {
                headings = ds.Tables[2].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                rows = ds.Tables[2].Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(item => item.ToString()).ToList()).ToList()
            };

            //var table4Data = new
            //{
            //    headings = ds.Tables[3].Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
            //    rows = ds.Tables[3].Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(item => item.ToString()).ToList()).ToList()
            //};

            if (ds.Tables[3].Rows.Count > 0 && ds.Tables[3].Rows[0][0] != DBNull.Value)
            {
                byte[] imageData = ds.Tables[3].Rows[0][0] as byte[];
                model.ImageUrl1 = imageData != null ? GetImageUrl(imageData) : null;
            }
            else
            {
                model.ImageUrl1 = null;
            }


            return Json(new { success = true, data = resultData, table2Data = table2Data,/* table3Data = table3Data,*/ table4Data = table4Data, authr_letter_imgurl = model.ImageUrl1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult auther_letter_select_popup()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 12;

            pr[1] = new SqlParameter("@trans_id", SqlDbType.BigInt);
            pr[1].Value = Session["@trans_id_tran_appro"];

            DataTable dt = new DataTable();
            ncd_branch_approval_model model = new ncd_branch_approval_model();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No documnet available!" });

            }
            else
            {
                model.ImageUrl2 = GetImageUrl(dt.Rows[0]["attachement"] as byte[]);



            }

            return Json(new { data = model.ImageUrl2 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult bank_attach_select_popup(string hiddenValue)
        {
            // long app_id = Convert.ToInt64(Session["application_id_br_appro"]);

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 11;

            pr[1] = new SqlParameter("@trans_id", SqlDbType.BigInt);
            pr[1].Value = Session["@trans_id_tran_appro"].ToString();

            pr[2] = new SqlParameter("@type", SqlDbType.VarChar);
            pr[2].Value = hiddenValue;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr).Tables[0];
            dbconnect.Close();

            ncd_branch_approval_model model = new ncd_branch_approval_model();
            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No documnet available!" });

            }
            else
            {
                model.ImageUrl3 = GetImageUrl(dt.Rows[0]["attachement_data"] as byte[]);

                //byte[] imageData = dt.Rows[0]["image"] as byte[];



                //Session["saved_img"] = imageData;

            }

            return Json(new { data = model.ImageUrl3 }, JsonRequestBehavior.AllowGet);
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
        public JsonResult Reject_BtnClick()
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@transfer_id", SqlDbType.BigInt);
            pr[1].Value = Session["@trans_id_tran_appro"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            DataTable dt = new DataTable();

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_transfer_save]", pr);
            dbconnect.Close();

            string msg = pr[3].Value.ToString();
            if (msg == "Rejected  Successfully")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Approve_BtnClick()
        {
            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7;

            pr[1] = new SqlParameter("@transfer_id", SqlDbType.BigInt);
            pr[1].Value = Session["@trans_id_tran_appro"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };

            pr[3] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[3].Value = Session["branch_id"];

            pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            DataTable dt = new DataTable();

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_transfer_save]", pr);
            dbconnect.Close();

            string msg = pr[4].Value.ToString();
            if (msg == "Approved Successfully")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult Return_BtnClick()
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[1].Value = Session["application_id_br_appro"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            DataTable dt = new DataTable();

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_branch_approval]", pr);
            dbconnect.Close();

            string msg = pr[3].Value.ToString();
            if (msg == "Form Return / Rejected Sucessfully...!")
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