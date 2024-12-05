using ERP_YOGLOANS_LOCAL.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;



namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_AddBankController : BaseController
    {
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();

        // GET: Customer_Addbank
        public ActionResult Cus_addbank()
        {
            return View();
        }

        public ActionResult Cus_bankdetailsapproval()
        {
            return View();
        }


        // GET: Customer names autocomplete suggestions
        [HttpGet]
        public ActionResult AutocompleteSuggestions(string term)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 16;
                pr[1] = new SqlParameter("@cust_name", SqlDbType.VarChar, 500);
                pr[1].Value = term;

                //DataTable dt = dbconnect.ExecuteDataset("[dbo].[cust_bankadd_new]", pr).Tables[0];
                DataTable dt = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr).Tables[0];
                dbconnect.Close();


                List<Customer_AddBank_Model> suggestions = new List<Customer_AddBank_Model>();
                foreach (DataRow row in dt.Rows)
                {
                    suggestions.Add(new Customer_AddBank_Model
                    {
                        customerId = Convert.ToInt32(row["customer_id"]),
                        customer_name = row["name"].ToString()
                    });
                }

                // Optionally, you may filter or limit the number of suggestions here
                // For example, return only the first 5 suggestions
                suggestions = suggestions.Take(15).ToList();

                // Convert suggestions to a format suitable for autocomplete
                var formattedSuggestions = suggestions.Select(s => new { label = s.customer_name, value = s.customerId });

                return Json(formattedSuggestions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }



        [HttpPost]
        public ActionResult FetchBankDetails(string ifsc)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 4;
                pr[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 35);
                pr[1].Value = ifsc.Trim();

                DataTable dt = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr).Tables[0];
                dbconnect.Close();

                if (dt.Rows.Count > 0)
                {

                    int bankId = Convert.ToInt32(dt.Rows[0]["bank_id"]);

                    // Store bankId in TempData
                    TempData["BankId"] = bankId;

                    var bankDetails = new
                    {
                        bank_name = dt.Rows[0]["bank_name"].ToString(),
                        branch_name = dt.Rows[0]["branch_name"].ToString(),
                        bank_id = Convert.ToInt32(dt.Rows[0]["bank_id"])
                    };

                    return Json(bankDetails);

                }
                else
                {
                    return Json(new { error = "Bank details not found for the provided IFSC code" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult add_details(Customer_AddBank_Model model)
        {
            try
            {


                byte[] imageBytes = null;

                //if (model.image != null && model.image.ContentLength > 0)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        model.image.InputStream.CopyTo(memoryStream);
                //        memoryStream.Position = 0;

                //        if (model.image.ContentType == "application/pdf")
                //        {
                //            if (memoryStream.Length > 256 * 1024) // If file size > 250 KB
                //            {
                //                imageBytes = doc.CompressPdfToSize(memoryStream);
                //            }
                //            else
                //            {
                //                imageBytes = memoryStream.ToArray();
                //            }
                //        }
                //        else
                //        {
                //            if (memoryStream.Length > 256 * 1024) // If file size > 250 KB
                //            {
                //                imageBytes = doc.CompressImageToSize(memoryStream, ImageFormat.Jpeg);
                //            }
                //            else
                //            {
                //                imageBytes = memoryStream.ToArray();
                //            }
                //        }
                //    }
                //}

                if (model.image != null && model.image.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image.ContentType == "application/pdf")
                        {
                            imageBytes = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (model.image.ContentType == "image/png" ||
                              model.image.ContentType == "image/jpg" ||
                              model.image.ContentType == "image/jpeg" ||
                              model.image.ContentType == "image/bmp" ||
                              model.image.ContentType == "image/gif")
                        {
                            imageBytes = memoryStream.Length > 256 * 1024
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

                //int customerid = Convert.ToInt32(TempData["customerId"]);

                //int bankId = Convert.ToInt32(TempData["BankId"]);


                // Prepare parameters for the stored procedure
                SqlParameter[] pr = new SqlParameter[11];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 5;

                pr[1] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[1].Value = TempData["customerId"];

                pr[2] = new SqlParameter("@branchid", SqlDbType.Int);
                pr[2].Value = Session["branch_id"];

                pr[3] = new SqlParameter("@bank_id", SqlDbType.Int);
                pr[3].Value = TempData["BankId"];

                pr[4] = new SqlParameter("@payeName", SqlDbType.VarChar, 100);
                pr[4].Value = model.PayeeName;

                pr[5] = new SqlParameter("@accno", SqlDbType.VarChar, 100); // Adjust SqlDbType as per your database schema
                pr[5].Value = model.AccountNumber;

                pr[6] = new SqlParameter("@enterby", SqlDbType.Int);
                pr[6].Value = Session["login_user"];

                pr[7] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                pr[7].Direction = ParameterDirection.Output;

                pr[8] = new SqlParameter("@ifsc", SqlDbType.VarChar, 25); // Adjust SqlDbType as per your database schema
                pr[8].Value = model.ifscInput; // Assuming ifscInput is a string

                pr[9] = new SqlParameter("@data", SqlDbType.Binary);
                pr[9].Value = imageBytes;

                pr[10] = new SqlParameter("@modify_status", SqlDbType.BigInt);
                pr[10].Value = Session["modify_status"];



                // Call stored procedure to insert bank details
                dbconnect.ExecuteStoredProcedure("[dbo].[SP_cust_bankAdd]", pr);
                dbconnect.Close();


                // Read output message from stored procedure
                string outputMessage = pr[7].Value != DBNull.Value ? pr[7].Value.ToString() : "";

                return Json(new { success = true, message = outputMessage });

                // Determine action based on output message
                //if (outputMessage.StartsWith("Requested Successfully"))
                //{
                //    return Json(new { success = true, message = outputMessage });
                //}
                //else
                //{
                //    return Json(new { success = false, message = outputMessage });
                //}
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
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

        [HttpGet]
        public ActionResult View_Image(int modify_status)
        {
            var docDetails = new DocDetails
            {
                ImageUrl = "",
                ImageType = ""
            };


            byte[] attachmentData = null;
            try
            {
                
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 7;
                pr[1] = new SqlParameter("@modify_status", SqlDbType.Int);
                pr[1].Value = modify_status; // Ensure TempData["modify_status"] has the correct value
                pr[2] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[2].Value = Session["bank_cust_id"];

                DataTable ds = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr).Tables[0];
                dbconnect.Close();

                if (ds.Rows.Count > 0)
                {
                    attachmentData = ds.Rows[0]["attachment_data"] as byte[];

                    if (attachmentData != null && attachmentData.Length > 4)
                    {
                        string base64Data = Convert.ToBase64String(attachmentData, 0, attachmentData.Length);

                        // Check if the data is a PDF
                        if (attachmentData.Length > 4 &&
                            attachmentData[0] == 0x25 && // '%'
                            attachmentData[1] == 0x50 && // 'P'
                            attachmentData[2] == 0x44 && // 'D'
                            attachmentData[3] == 0x46 && // 'F'
                            attachmentData[4] == 0x2D)   // '-'
                        {
                            docDetails.ImageUrl = $"data:application/pdf;base64,{base64Data}";
                            docDetails.ImageType = "pdf";



                        }
                        else
                        {
                            // string mimeType = GetImageMimeType(attachmentData);
                            docDetails.ImageUrl = $"data:image;base64,{base64Data}";
                            docDetails.ImageType = "image";
                            // You might need to specify a more specific MIME type for images if possible.

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // Log exception or handle it as needed
                // Optionally set an error response or return a default value
                return Json(new { error = "An error occurred while processing the request." }, JsonRequestBehavior.AllowGet);
            }
            return Json(docDetails, JsonRequestBehavior.AllowGet);
        }






        [HttpPost]
        public ActionResult SearchCustomerDetails(int customerId)
        {
            try
            {
                Session["modify_status"] = 0;
                Session["bank_cust_id"] = customerId;
                // Set up SQL parameters for the initial search
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 2; // query_id 2 for searching customer details by customer ID
                pr[1] = new SqlParameter("@cust_id", SqlDbType.Int);
                pr[1].Value = customerId;
                pr[2] = new SqlParameter("@outmsg", SqlDbType.Int);
                pr[2].Direction = ParameterDirection.Output;

                // Execute stored procedure to get dataset
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr);
                dbconnect.Close();

                string outputMessage = pr[2].Value != DBNull.Value ? pr[2].Value.ToString() : "";

                TempData["customerId"] = customerId;

                // Initialize customer details object
                var customerDetails = new
                {
                    customerName = "",
                    address = "",
                    mobileNo = "",
                    branch = "",
                    remarks = ""
                };

                // Initialize customer bank details object
                var customerBankDetails = new
                {
                    CustomerID = "",
                    Bank = "",
                    IFSC = "",
                    Payee = "",
                    Account = "",
                    Status = "",
                    Branch = "",
                    //ImageUrl = "",
                    //ImageType = ""

                };

                // Initialize customer bank details object
                var customerBankDetailsModify = new
                {
                    CustomerID = "",
                    Bank = "",
                    IFSC = "",
                    Payee = "",
                    Account = "",
                    Status = "",
                    Branch = "",
                    //ImageUrl = "",
                    //ImageType = ""


                };

                // Define additional fields
                long liveStatus = 0;
                long modifyStatus = 0;
                long outmessage = 0;

                // Initialize combined details object
                var combinedDetails = new
                {
                    customerDetails = customerDetails,
                    customerBankDetails = customerBankDetails,
                    customerBankDetailsModify = customerBankDetailsModify,
                    liveStatus = liveStatus,
                    modifyStatus = modifyStatus,
                    outmessage = outmessage
                };

                // Check if dataset contains any tables and rows for customer details
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_list = ds.Tables[0];
                    long live = Convert.ToInt64(ds.Tables[1].Rows[0]["livecnt"]);
                    long modi = Convert.ToInt64(ds.Tables[1].Rows[0]["modifycnt"]);

                    // Update customer details object
                    customerDetails = new
                    {
                        customerName = dt_list.Rows[0]["Name"].ToString(),
                        address = dt_list.Rows[0]["Address"].ToString(),
                        mobileNo = dt_list.Rows[0]["Mobile"].ToString(),
                        branch = dt_list.Rows[0]["Branch"].ToString(),
                        remarks = dt_list.Rows[0]["Remarks"].ToString()
                    };

                    if (outputMessage == "1")
                    {
                        long out_msg = 1;
                        if (live == 1 && modi == 1) //The customer is modified but not approved
                        {
                            // Set up SQL parameters for the second query_id
                            SqlParameter[] pr1 = new SqlParameter[3];
                            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                            pr1[0].Value = 3; // query_id 3 for bank grid
                            pr1[1] = new SqlParameter("@cust_id", SqlDbType.Int);
                            pr1[1].Value = customerId;
                            pr1[2] = new SqlParameter("@outmsg", SqlDbType.Int);
                            pr1[2].Direction = ParameterDirection.Output;

                            // Execute stored procedure to get another dataset
                            DataSet ds2 = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr1);

                            if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                            {
                                DataTable dt = ds2.Tables[0];                              


                                // Update customer bank details object
                                customerBankDetails = new
                                {
                                    CustomerID = dt.Rows[0]["customer_id"].ToString(),
                                    Bank = dt.Rows[0]["Bank"].ToString().Trim(),
                                    IFSC = dt.Rows[0]["ifsc"].ToString(),
                                    Payee = dt.Rows[0]["Payee"].ToString(),
                                    Account = dt.Rows[0]["Account Number"].ToString(),
                                    Status = dt.Rows[0]["Status"].ToString(),
                                    Branch = dt.Rows[0]["Branch"].ToString(),                                    
                                };



                            }

                            if (ds2.Tables.Count > 0 && ds2.Tables[1].Rows.Count > 0)
                            {
                                DataTable dt = ds2.Tables[1];

                                // Update customer bank details object
                                customerBankDetailsModify = new
                                {
                                    CustomerID = dt.Rows[0]["customer_id"].ToString(),
                                    Bank = dt.Rows[0]["Bank"].ToString().Trim(),
                                    IFSC = dt.Rows[0]["ifsc"].ToString(),
                                    Payee = dt.Rows[0]["Payee"].ToString(),
                                    Account = dt.Rows[0]["Account Number"].ToString(),
                                    Status = dt.Rows[0]["Status"].ToString(),
                                    Branch = dt.Rows[0]["Branch"].ToString(),
                                    
                                };



                            }
                        }
                        else if (live == 1 && modi == 0) //The customer is live not modified
                        {
                            Session["modify_status"] = 1;
                            // Set up SQL parameters for the second query_id
                            SqlParameter[] pr1 = new SqlParameter[3];
                            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                            pr1[0].Value = 3; // query_id 3 for bank grid
                            pr1[1] = new SqlParameter("@cust_id", SqlDbType.Int);
                            pr1[1].Value = customerId;
                            pr1[2] = new SqlParameter("@outmsg", SqlDbType.Int);
                            pr1[2].Direction = ParameterDirection.Output;

                            // Execute stored procedure to get another dataset
                            DataSet ds2 = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr1);

                            if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                            {
                                DataTable dt = ds2.Tables[0];                                

                                // Update customer bank details object
                                customerBankDetails = new
                                {
                                    CustomerID = dt.Rows[0]["customer_id"].ToString(),
                                    Bank = dt.Rows[0]["Bank"].ToString().Trim(),
                                    IFSC = dt.Rows[0]["ifsc"].ToString(),
                                    Payee = dt.Rows[0]["Payee"].ToString(),
                                    Account = dt.Rows[0]["Account Number"].ToString(),
                                    Status = dt.Rows[0]["Status"].ToString(),
                                    Branch = dt.Rows[0]["Branch"].ToString()
                                };



                            }
                        }

                        else if (live == 0 && modi == 0) //Customer is not live but Approval is pending
                        {
                            //Session["modify_status"] = 0;
                            // Set up SQL parameters for the second query_id
                            SqlParameter[] pr1 = new SqlParameter[3];
                            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                            pr1[0].Value = 3; // query_id 3 for bank grid
                            pr1[1] = new SqlParameter("@cust_id", SqlDbType.Int);
                            pr1[1].Value = customerId;
                            pr1[2] = new SqlParameter("@outmsg", SqlDbType.Int);
                            pr1[2].Direction = ParameterDirection.Output;

                            // Execute stored procedure to get another dataset
                            DataSet ds2 = dbconnect.ExecuteDataset("[dbo].[SP_cust_bankAdd]", pr1);

                            if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                            {
                                DataTable dt = ds2.Tables[0];                                

                                // Update customer bank details object
                                customerBankDetails = new
                                {
                                    CustomerID = dt.Rows[0]["customer_id"].ToString(),
                                    Bank = dt.Rows[0]["Bank"].ToString().Trim(),
                                    IFSC = dt.Rows[0]["ifsc"].ToString(),
                                    Payee = dt.Rows[0]["Payee"].ToString(),
                                    Account = dt.Rows[0]["Account Number"].ToString(),
                                    Status = dt.Rows[0]["Status"].ToString(),
                                    Branch = dt.Rows[0]["Branch"].ToString()
                                };




                            }
                        }
                        else
                        {

                        }

                        combinedDetails = new
                        {
                            customerDetails = customerDetails,
                            customerBankDetails = customerBankDetails,
                            customerBankDetailsModify = customerBankDetailsModify,
                            liveStatus = live,
                            modifyStatus = modi,
                            outmessage = out_msg
                        };
                    }
                    else
                    {
                        long out_msg = 0;
                        combinedDetails = new
                        {
                            customerDetails = customerDetails,
                            customerBankDetails = customerBankDetails,
                            customerBankDetailsModify = customerBankDetailsModify,
                            liveStatus = live,
                            modifyStatus = modi,
                            outmessage = out_msg
                        };

                    }



                    // Update combined details object with updated customer details and bank details


                    // Return combined customer details as JSON
                    return Json(combinedDetails, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle case where no data is found for query_id 2
                    return Json(new { error = "No data found!" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.ToString()); // Or use your preferred logging mechanism

                // Return an appropriate error response
                return Json(new { error = "An error occurred while processing your request." });
            }
        }

    }
}

