using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.personal_loan_models; 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Drawing.Imaging;
using System.IO;


namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class Personal_loanController : Controller
    {
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();
        doc_compression ImageCompress = new doc_compression();


        // GET: Personal_loan
        public ActionResult personal_loan_create()
        {           
            return View();
        }

        public JsonResult GetCustomerDetailsJson()
        {
            var pl_table_values = personal_loan_create_grid();

            var customerList = pl_table_values.AsEnumerable().Select(row => new
            {
                CustomerId = row["customer_id"],
                CustomerName = row["customer_name"],
                LoanNo = row["loan_no"],
                PlStatus = row["pl_status"]
            }).ToList();

            return Json(customerList, JsonRequestBehavior.AllowGet);
        }


        private DataTable personal_loan_create_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            DataTable dt = new DataTable();
           
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        [HttpPost]
        public JsonResult personal_loan_details(long loan_no)
        {
            Session["loan_no"] = loan_no;
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 2;
            pr[1] = new SqlParameter("@loan_no", SqlDbType.BigInt);
            pr[1].Value = Session["loan_no"];
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];
            dbconnect.Open();   
            DataTable dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close ();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                SqlParameter[] pr1 = new SqlParameter[3];
                pr1[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr1[0].Value = 33;
                pr1[1] = new SqlParameter("@loan_no", SqlDbType.VarChar,20);
                pr1[1].Value = Session["loan_no"];
                pr1[2] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                pr1[2].Value = row["CustomerId"].ToString();
                dbconnect.Open();
                DataTable dt1 = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr1).Tables[0];
                dbconnect.Close();
                decimal loan_amt = 0;

                if (dt1.Rows.Count > 0 && dt1.Rows[0]["loan_amount"] != DBNull.Value)
                {
                    decimal.TryParse(dt1.Rows[0]["loan_amount"].ToString(), out loan_amt);
                }

                var model = new
                {
                    CustomerId = row["CustomerId"] != DBNull.Value ? row["CustomerId"].ToString() : string.Empty,
                    CustomerName = row["CustomerName"] != DBNull.Value ? row["CustomerName"].ToString() : string.Empty,
                    MinLoan = row["MinLoan"] != DBNull.Value ? Convert.ToDecimal(row["MinLoan"]) : 0.0m,
                    MaxLoan = row["MaxLoan"] != DBNull.Value ? Convert.ToDecimal(row["MaxLoan"]) : 0.0m,
                    InterestRt = row["InterestRt"] != DBNull.Value ? row["InterestRt"].ToString() : string.Empty,
                    MaturityDt = row["MaturityDt"] != DBNull.Value ? row["MaturityDt"].ToString() : string.Empty,
                    LoanDt = row["LoanDt"] != DBNull.Value ? row["LoanDt"].ToString() : string.Empty ,
                    SchemeName = row["SchemeName"] != DBNull.Value ? row["SchemeName"].ToString() : string.Empty,
                    loan_amount = loan_amt

                };
                //Session["mobileNo"] = row["mobile"];

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            // Return null if no data found
            return Json(null, JsonRequestBehavior.AllowGet);


        }



        [HttpPost]
        public ActionResult SubmitPersonalLoanApplication(Pl_application_data pl_Application_Data)
        {

            try
            {
                // Initialize output parameters
                string applicationId = null;
                string outMsg = null;

                // Set up the parameters for the stored procedure
                SqlParameter[] pr = new SqlParameter[10];
                pr[0] = new SqlParameter("@branchid", SqlDbType.Int) { Value = Session["branch_id"] };
                pr[1] = new SqlParameter("@user", SqlDbType.BigInt) { Value = Session["login_user"] };
                pr[2] = new SqlParameter("@custid", SqlDbType.BigInt) { Value = pl_Application_Data.CustomerId };
                pr[3] = new SqlParameter("@max_loan_amt", SqlDbType.Float) { Value = pl_Application_Data.MaxLoanAmt };
                pr[4] = new SqlParameter("@req_amt", SqlDbType.Float) { Value = pl_Application_Data.RequestAmt };
                pr[5] = new SqlParameter("@int_rate", SqlDbType.Float) { Value = pl_Application_Data.IntRate };
                pr[6] = new SqlParameter("@maturity_dt", SqlDbType.Date) { Value = pl_Application_Data.MaturityDate };
                pr[7] = new SqlParameter("@gl_loan", SqlDbType.VarChar, 20) { Value = pl_Application_Data.LoanNo };

                SqlParameter outputApplicationId = new SqlParameter("@applicationid", SqlDbType.VarChar, 20)
                {
                    Direction = ParameterDirection.Output
                };
                pr[8] = outputApplicationId;

                SqlParameter outputMsg = new SqlParameter("@outmsg", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                pr[9] = outputMsg;

                dbconnect.Open();
                dbconnect.ExecuteDataset("PL_proc_loanApplication", pr);
                dbconnect.Close();
                // Get the output values
                applicationId = outputApplicationId.Value != DBNull.Value ? outputApplicationId.Value.ToString() : "No ID returned";
                outMsg = outputMsg.Value != DBNull.Value ? outputMsg.Value.ToString() : "No message returned";
                Session["application_id"] = applicationId;
                // Return the JSON result
                return Json(new { applicationId = applicationId, message = outMsg });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging library like log4net or NLog)
                // Log.Error(ex); // Example of logging, depends on your logging setup
                return Json(new { error = ex.Message });
            }
        }



        [HttpPost]
        public JsonResult Fill_downloadAgreement(long loan_no, long customerID)
        {
            Session["OTP_status"] = null;

            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 3;
            pr[1] = new SqlParameter("@loan_no", SqlDbType.BigInt);
            pr[1].Value = loan_no;
            pr[2] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[2].Value = customerID;
            // Assuming customer_id is not needed for fetching details with loan_no
            dbconnect.Open();
            DataSet ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr);
            dbconnect.Close();

            DataTable dt = ds.Tables[0];
            DataTable dt1 = ds.Tables[1];
            var downloadAgreementData = new DownloadAgreementData();
            if (dt.Rows.Count > 0)
            {
                downloadAgreementData.application_id = dt.Rows[0]["application_id"].ToString();
                downloadAgreementData.Name = dt.Rows[0]["customer_name"].ToString();
                downloadAgreementData.GlLoanNo = dt.Rows[0]["gl_loan_no"].ToString();
                downloadAgreementData.MaxLoanAmt = float.Parse(dt.Rows[0]["max_loan_amt"].ToString());
                downloadAgreementData.RequestAmt = float.Parse(dt.Rows[0]["loan_amount"].ToString());
                downloadAgreementData.otp = dt.Rows[0]["otp"].ToString();
                downloadAgreementData.otp_status = dt.Rows[0]["otp_status"].ToString();
                downloadAgreementData.MobNo = dt1.Rows[0]["mobile_no"].ToString();

                Session["OTP"] = dt.Rows[0]["otp"].ToString();
                Session["OTP_status"] = dt.Rows[0]["otp_status"].ToString();
                Session["application_id"] = dt.Rows[0]["application_id"].ToString();

            }

            return Json(downloadAgreementData, JsonRequestBehavior.AllowGet);



        }

        [HttpPost]
        public JsonResult Upload_attachments(SubmitToCPC submitToCPC)
        {
            if(submitToCPC.AppId == null)
            {
                SqlParameter[] pr1 = new SqlParameter[2];
                pr1[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr1[0].Value = 34;
                pr1[1] = new SqlParameter("@loan_no", SqlDbType.VarChar, 14);
                pr1[1].Value = submitToCPC.gl_loanNumber;

                dbconnect.Open();
                DataTable dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr1).Tables[0];
                dbconnect.Close();
                if (dt.Rows.Count > 0)
                {
                    submitToCPC.AppId = dt.Rows[0]["application_id"].ToString();
                }
            }

            byte[] dpnBytes = null;
            byte[] agreementBytes = null;
            byte[] applicationBytes = null;

            if (submitToCPC.Doc_agreement != null && submitToCPC.Doc_agreement.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    submitToCPC.Doc_agreement.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (submitToCPC.Doc_agreement.ContentType == "application/pdf")
                    {
                        agreementBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (submitToCPC.Doc_agreement.ContentType == "image/png" ||
                          submitToCPC.Doc_agreement.ContentType == "image/jpg" ||
                          submitToCPC.Doc_agreement.ContentType == "image/jpeg" ||
                          submitToCPC.Doc_agreement.ContentType == "image/bmp" ||
                          submitToCPC.Doc_agreement.ContentType == "image/gif")
                    {
                        agreementBytes = memoryStream.Length > 256 * 1024
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

            if (submitToCPC.Doc_application != null && submitToCPC.Doc_application.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    submitToCPC.Doc_application.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (submitToCPC.Doc_application.ContentType == "application/pdf")
                    {
                        applicationBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (submitToCPC.Doc_application.ContentType == "image/png" ||
                          submitToCPC.Doc_application.ContentType == "image/jpg" ||
                          submitToCPC.Doc_application.ContentType == "image/jpeg" ||
                          submitToCPC.Doc_application.ContentType == "image/bmp" ||
                          submitToCPC.Doc_application.ContentType == "image/gif")
                    {
                        applicationBytes = memoryStream.Length > 256 * 1024
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

            // Check and process DPN document
            if (submitToCPC.Doc_DPN != null && submitToCPC.Doc_DPN.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    submitToCPC.Doc_DPN.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (submitToCPC.Doc_DPN.ContentType == "application/pdf")
                    {
                        dpnBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (submitToCPC.Doc_DPN.ContentType == "image/png" ||
                          submitToCPC.Doc_DPN.ContentType == "image/jpg" ||
                          submitToCPC.Doc_DPN.ContentType == "image/jpeg" ||
                          submitToCPC.Doc_DPN.ContentType == "image/bmp" ||
                          submitToCPC.Doc_DPN.ContentType == "image/gif")
                    {
                        dpnBytes = memoryStream.Length > 256 * 1024
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

            SqlParameter[] pr = new SqlParameter[8];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 8;
            pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar,14);
            pr[1].Value = submitToCPC.AppId;
            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];
            pr[3] = new SqlParameter("@data1", SqlDbType.Binary);
            pr[3].Value = dpnBytes;
            pr[4] = new SqlParameter("@data2", SqlDbType.Binary);
            pr[4].Value = agreementBytes;
            pr[5] = new SqlParameter("@data3", SqlDbType.Binary);
            pr[5].Value = applicationBytes;
            
            SqlParameter outputMsg = new SqlParameter("@outmsg", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };
            pr[6] = outputMsg;

            pr[7] = new SqlParameter("@pay_mode", SqlDbType.Int);
            pr[7].Value = submitToCPC.paymode; 

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
            dbconnect.Close();



            string msg = pr[6].Value.ToString();
            return Json(new { success = true, message = msg });



        }

        [HttpPost]
        public JsonResult Download_Complete(long appli_id)
        {
           

            SqlParameter[] pr = new SqlParameter[4];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 7;
            pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14);
            pr[1].Value = appli_id;
            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];
           
            SqlParameter outputMsg = new SqlParameter("@outmsg", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };
            pr[3] = outputMsg;

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
            dbconnect.Close();



            string msg = pr[3].Value.ToString();
            return Json(new { success = true, message = msg });



        }

        [HttpPost]
        public JsonResult FillDropdown()
        {


            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 28;
            //pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14);
            //pr[1].Value = appli_id;
            //pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            //pr[2].Value = Session["login_user"];

            DataTable dt = new DataTable();  

            dbconnect.Open();
            dt=dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();


            // Convert DataTable to a list of objects
            var dropdownData = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                dropdownData.Add(new
                {
                    Value = row["id"], // Replace with actual column name for the dropdown value
                    Text = row["mode"]    // Replace with actual column name for the dropdown text
                });
            }

            // Return the data as JSON
            return Json(new { success = true, data = dropdownData });



        }

        [HttpPost]
        public JsonResult GetBankDetails(long cust_id)
        {


            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 27;
            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = cust_id;
            //pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            //pr[2].Value = Session["login_user"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();


            if (dt.Rows.Count > 0)
            {
                // Convert the single DataRow into a dictionary
                var rowDict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    rowDict[col.ColumnName] = dt.Rows[0][col]; // Access the first (and only) row
                }

                // Return the data as JSON
                return Json(new { success = true, data = rowDict });
            }
            else
            {
                // Return an error if no data is found
                return Json(new { success = false, message = "No data found." });
            }



        }

        [HttpPost]
        public JsonResult GetControlBalance(long cust_id)
        {


            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 7;
            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = cust_id;
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[gl_gl2]", pr).Tables[0];
            dbconnect.Close();

            var controlBalance = "";
            if (dt.Rows.Count > 0)
            {

                controlBalance = dt.Rows[0]["ctrl"].ToString();

                // Return the data as JSON
                return Json(new { success = true, data = controlBalance });
            }
            else
            {
                // Return an error if no data is found
                return Json(new { success = false, message = "No data found." });
            }



        }
    }
}