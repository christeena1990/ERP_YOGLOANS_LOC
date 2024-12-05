using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Net;
using iTextSharp.text;
using System.Security.Cryptography.X509Certificates;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_ApproveBankController : BaseController
    {       
        DB dbconnect = new DB();
        public ActionResult Cus_bankdetailsapproval()
        {
            //var customerBankDetails = GetCustomerBankDetails_grid();
            //return View(customerBankDetails);

            var customerBankDetails = GetCustomerBankDetails_grid();
            bool isEmpty = customerBankDetails.Rows.Count == 0;

            ViewBag.IsEmpty = isEmpty;
            return View(customerBankDetails);
        }
        private DataTable GetCustomerBankDetails_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];
           
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;
            pr[1] = new SqlParameter("@branchid", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dt = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr).Tables[0];

            return dt;
        }

      
        //public JsonResult view_document_click(int customerId, string requestType)
        //{
        //    string fileUrl = Url.Action("ServeDocument", "Customer_ApproveBank", new { customerId = customerId, requestType = requestType });
        //    return Json(new { success = true, url = fileUrl });
        //}

        private bool IsPdf(byte[] byteData)
        {
            return byteData.Length > 4 &&
                   byteData[0] == 0x25 &&
                   byteData[1] == 0x50 &&
                   byteData[2] == 0x44 &&
                   byteData[3] == 0x46 &&
                   byteData[4] == 0x2D;
        }

        public ActionResult ServeDocument(int customerId, string requestType)
        {

            Session["new_modify"] = requestType.ToString();
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7;
            pr[1] = new SqlParameter("@modify_status", SqlDbType.Int);
            // pr[1].Value = requestType == "New" ? 0 : 1;
            pr[1].Value = Session["new_modify"].ToString()== "New" ? 0 : 1;
            pr[2] = new SqlParameter("@cust_id", SqlDbType.Int);
            pr[2].Value = customerId;

            DataTable ds = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr).Tables[0];

            if (ds.Rows.Count > 0)
            {
                          
                  
                Session["get_image"] = GetImageUrl(ds.Rows[0]["attachment_data"] as byte[]);
                return Json(new { success = true, imageUrl = Session["get_image"] });
            }

            return Json(new { success = false, message = "No document found." });
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
        public ActionResult recommend_btn_click(string requestType,int custid)
        {
            try
            {
               // string new_modi = Session["new_modify"].ToString();
                SqlParameter[] pr = new SqlParameter[6];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 9;
                pr[1] = new SqlParameter("@branchid", SqlDbType.Int);
                pr[1].Value = Session["branch_id"]; 
                pr[2] = new SqlParameter("@enterby", SqlDbType.BigInt);
                pr[2].Value = Session["login_user"];
                pr[3] = new SqlParameter("@modify_status", SqlDbType.Int);         
                pr[3].Value = requestType == "New" ? 0 : 1;
                pr[4] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[4].Value = custid;
                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[5].Direction = ParameterDirection.Output;

                dbconnect.ExecuteStoredProcedure("[SP_cust_bankAdd]", pr);

                string msg = pr[5].Value.ToString();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }
        public ActionResult approve_btn_click(string requestType, int custid)
        {
            try
            {
                // string new_modi = Session["new_modify"].ToString();
                SqlParameter[] pr = new SqlParameter[6];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 9;
                pr[1] = new SqlParameter("@branchid", SqlDbType.Int);
                pr[1].Value = Session["branch_id"];
                pr[2] = new SqlParameter("@enterby", SqlDbType.BigInt);
                pr[2].Value = Session["login_user"];
                pr[3] = new SqlParameter("@modify_status", SqlDbType.Int);
                pr[3].Value = requestType == "New" ? 0 : 1;
                pr[4] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[4].Value = custid;
                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[5].Direction = ParameterDirection.Output;

                dbconnect.ExecuteStoredProcedure("[SP_cust_bankAdd]", pr);

                string msg = pr[5].Value.ToString();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }

        public ActionResult reject_btn_click(string requestType, int custid,string rejectReason)
        {
            try
            {
                // string new_modi = Session["new_modify"].ToString();
                SqlParameter[] pr = new SqlParameter[7];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 8;
                pr[1] = new SqlParameter("@branchid", SqlDbType.Int);
                pr[1].Value = Session["branch_id"];
                pr[2] = new SqlParameter("@enterby", SqlDbType.BigInt);
                pr[2].Value = Session["login_user"];
                pr[3] = new SqlParameter("@modify_status", SqlDbType.Int);
                pr[3].Value = requestType == "New" ? 0 : 1;
                pr[4] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[4].Value = custid;
                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[5].Direction = ParameterDirection.Output;
                pr[6] = new SqlParameter("@comments", SqlDbType.VarChar,500);
                pr[6].Value = rejectReason;

                dbconnect.ExecuteStoredProcedure("[SP_cust_bankAdd]", pr);

                string msg = pr[5].Value.ToString();
                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }

    }
}