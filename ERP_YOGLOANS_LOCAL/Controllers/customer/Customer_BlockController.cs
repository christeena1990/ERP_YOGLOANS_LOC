using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;
using System.IO;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_BlockController : BaseController
    {
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();
        doc_compression ImageCompress = new doc_compression();
        // GET: Customer_Block
        public ActionResult Customer_block()
        {
            return View();
        }

        public ActionResult GetSearchSuggestions(string search_txt)
        {
            // Define the SQL parameters as per your existing method
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 10 };
            pr[1] = new SqlParameter("@search_txt", SqlDbType.VarChar, 50) { Value = search_txt };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", pr).Tables[0];
            dbconnect.Close();
            // Convert the DataTable results to a list of strings for JSON response
            var result = dt.AsEnumerable().Select(row => row["Customer"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerDetails(string customer_id)
        {
            Session["customer_id_his"]= customer_id;
            Session["Apr_custID"] = customer_id;

            SqlParameter[] pr1 = new SqlParameter[3];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
            pr1[1] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = customer_id };
            pr1[2] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = Session["branch_id"] };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt_dtls = new DataTable();
            dbconnect.Open();
            dt_dtls = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", pr1).Tables[0];
            dbconnect.Close();

            var customerDetails = new CustomerDetails();
            if (dt_dtls.Rows.Count > 0)
            {
                DataRow row = dt_dtls.Rows[0];
                customerDetails.CustomerId = row["customer_id"].ToString();
                customerDetails.CustomerName = row["customer_name"].ToString();
                customerDetails.MobileNo = row["mobile_no"].ToString();
                customerDetails.Message = row["msg"].ToString();
                customerDetails.Branch= row["branch_name"].ToString();
            }


            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 2 };
            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = customer_id };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", pr).Tables[0];
            dbconnect.Close();

            var loanDetailsList = new List<LoanDetails>();
            foreach (DataRow row in dt.Rows)
            {
                loanDetailsList.Add(new LoanDetails()
                {
                    ModuleId = Convert.ToInt32(row["module_id"]),
                    Module = row["Module"].ToString(),
                    AccountNo = row["Account No"].ToString(),
                    StartDt = Convert.ToDateTime(row["Start Dt"]),
                    MaturityDt = Convert.ToDateTime(row["Maturity Dt"]),
                    Amount = Convert.ToDecimal(row["Amount"])
                });

            }

            var result = new
            {
                Customer = customerDetails,
                Loans = loanDetailsList
            };

            return Json(result, JsonRequestBehavior.AllowGet);


        }


        public ActionResult GetCustomerDetails2(string customer_id)
        {
            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 11 };
            pr1[1] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = customer_id };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt_dtls = new DataTable();
            dbconnect.Open();
            dt_dtls = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", pr1).Tables[0];
            dbconnect.Close();

            var customerDetails = new CustomerDetails();
            string message = "";

            if (dt_dtls.Rows.Count > 0)
            {
                DataRow row = dt_dtls.Rows[0];
                customerDetails.CustomerId = row["customer_id"].ToString();
                customerDetails.CustomerName = row["customer_name"].ToString();
                customerDetails.MobileNo = row["mobile_no"].ToString();
                message = row["msg"].ToString();
            }

            return Json(new { customerDetails, message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveRequest(RequestDetails request)
        {

            byte[] docBytes = null;

            // Check and process the first image (model.image)
            if (request.Document != null && request.Document.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    request.Document.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (request.Document.ContentType == "application/pdf")
                    {
                        docBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (request.Document.ContentType == "image/png" ||
                          request.Document.ContentType == "image/jpg" ||
                          request.Document.ContentType == "image/jpeg" ||
                          request.Document.ContentType == "image/bmp" ||
                          request.Document.ContentType == "image/gif")
                    {
                        docBytes = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        // Handle unsupported content types or throw an error
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }
            }

            SqlParameter[] pr1 = new SqlParameter[8];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 3 };
            pr1[1] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = request.CustomerId };
            pr1[2] = new SqlParameter("@new_custid", SqlDbType.BigInt) { Value = request.NewCustomerId };
            pr1[3] = new SqlParameter("@type", SqlDbType.Int) { Value = request.Type };
            pr1[4] = new SqlParameter("@comments", SqlDbType.NVarChar,500) { Value = request.Comments };
            pr1[5] = new SqlParameter("@user", SqlDbType.BigInt) { Value = Session["login_user"] };
            pr1[6] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
            pr1[7] = new SqlParameter("@image", SqlDbType.Binary) { Value = docBytes };

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", pr1);
            dbconnect.Close();


            string msg = pr1[6].Value.ToString();
            return Json(new { success = true, message = msg });
            //return Json(result, JsonRequestBehavior.AllowGet);


        }
    }
}