using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.IO;
using System.Drawing.Imaging;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class BOND_Transfere_BankaddController : Controller
    {
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();
        // GET: NCD_bank_details
        DB dbconnect = new DB();
        public ActionResult BOND_Transfere_Bankadd_View()
        {
            var customerBankDetails = bank_add_Customer_Grid();

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
        private DataTable bank_add_Customer_Grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 8;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Bond_transfer_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        [HttpPost]
        public JsonResult BankAdd_textBox_fill(string customerid)
        {
            Session["ncd_bankadd_customerid"] = customerid.ToString();
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = Session["ncd_bankadd_customerid"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_CustomerBank]", pr).Tables[0];
            dbconnect.Close();

            // Convert DataTable to a list of dictionaries to return as JSON
            var data = dt.AsEnumerable()
                         .Select(row => dt.Columns.Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]));

            return Json(new { data = data, columns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult attachmet_select_popup(string dep_id, String module_id)
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@dep_id", SqlDbType.BigInt);
            pr[1].Value = dep_id;

            pr[2] = new SqlParameter("@module_id", SqlDbType.BigInt);
            pr[2].Value = module_id;

            pr[3] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[3].Value = Session["ncd_bankadd_customerid"];

            DataTable dt = new DataTable();
            ncd_bank_add_model model = new ncd_bank_add_model();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_CustomerBank]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No documnet available!" });

            }
            else
            {
                model.ImageUrl1 = GetImageUrl(dt.Rows[0]["attachment_data"] as byte[]);

                //byte[] imageData = dt.Rows[0]["image"] as byte[];



                //Session["saved_img"] = imageData;

            }

            return Json(new { data = model.ImageUrl1 }, JsonRequestBehavior.AllowGet);
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
        private DataTable Bank_select(string ifsc_code)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 15);
            pr[1].Value = ifsc_code;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_CustomerBank]", pr).Tables[0];
            dbconnect.Close();
            return dt;
        }

        [HttpPost]
        public JsonResult GetBankDetails(string ifsc_code)
        {
            DataTable dt = Bank_select(ifsc_code);

            // Convert DataTable to a list or JSON-friendly structure
            var bankDetails = dt.AsEnumerable().Select(row => new {
                BankName = row["bank_name"].ToString(),
                BranchName = row["branch_name"].ToString(),
                Bank_id = row["bank_id"].ToString()
                // Add other fields as required
            }).ToList();

            return Json(bankDetails, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult submit_form_1(string customerName, string ifsc_code, string bankName, string branchName, string accountNo, HttpPostedFileBase file, string bank_id, string firstCellValue)
        {
            byte[] fileBytes;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (file.ContentType == "application/pdf")
                    {
                        fileBytes = memoryStream.Length > 256 * 1024
                            ? doc.CompressFile(pdfData, ".pdf")
                            : memoryStream.ToArray();
                    }
                    else if (file.ContentType == "image/png" ||
                             file.ContentType == "image/jpg" ||
                             file.ContentType == "image/jpeg" ||
                             file.ContentType == "image/bmp" ||
                             file.ContentType == "image/gif")
                    {
                        fileBytes = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }

                SqlParameter[] pr = new SqlParameter[11]; // Adjusted size to 10

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 };
                pr[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 15) { Value = ifsc_code };
                pr[2] = new SqlParameter("@transfer_id", SqlDbType.BigInt) { Value = firstCellValue };
                pr[3] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = Session["ncd_bankadd_customerid"] };
                pr[4] = new SqlParameter("@branch_id", SqlDbType.Int) { Value = Session["branch_id"] };
                pr[5] = new SqlParameter("@bank_id", SqlDbType.BigInt) { Value = bank_id };
                pr[6] = new SqlParameter("@bankac", SqlDbType.VarChar) { Value = accountNo };
                pr[7] = new SqlParameter("@bank_attach", SqlDbType.VarBinary) { Value = fileBytes };
                pr[8] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };
                pr[9] = new SqlParameter("@payeename", SqlDbType.VarChar) { Value = customerName };
                //pr[9] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                SqlParameter outputMsg = new SqlParameter("@outmsg", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                pr[10] = outputMsg;


                dbconnect.Open();
                // dt = dbconnect.ExecuteDataset("[dbo].[DEB_CustomerBank]", pr).Tables[0];
                dbconnect.ExecuteDataset("[dbo].[BOND_transfer_save]", pr);

                dbconnect.Close();
                string msg = pr[10].Value.ToString();

                return Json(new { success = true, message = msg });
            }
            catch (NotSupportedException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (SqlException ex)
            {
                // Log SQL exception here
                return Json(new { success = false, message = "Database error: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log general exceptions here
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult submit_form_2(
                                  string customerName1, string customerName2,
                                  string ifscCode1, string ifscCode2,
                                  string bankName, string branchName,
                                  string accountNo1, string accountNo2,
                                  HttpPostedFileBase fileInput1, HttpPostedFileBase fileInput2,
                                  HttpPostedFileBase Authorization_txtattach,
                                  string bank_id1, string bank_id2, string applicationid)
        {
            byte[] fileBytes1;
            byte[] fileBytes2;
            byte[] fileBytes3;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileInput1.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (fileInput1.ContentType == "application/pdf")
                    {
                        fileBytes1 = memoryStream.Length > 256 * 1024
                            ? doc.CompressFile(pdfData, ".pdf")
                            : memoryStream.ToArray();
                    }
                    else if (fileInput1.ContentType == "image/png" ||
                             fileInput1.ContentType == "image/jpg" ||
                             fileInput1.ContentType == "image/jpeg" ||
                             fileInput1.ContentType == "image/bmp" ||
                             fileInput1.ContentType == "image/gif")
                    {
                        fileBytes1 = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    fileInput2.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (fileInput2.ContentType == "application/pdf")
                    {
                        fileBytes2 = memoryStream.Length > 256 * 1024
                            ? doc.CompressFile(pdfData, ".pdf")
                            : memoryStream.ToArray();
                    }
                    else if (fileInput2.ContentType == "image/png" ||
                             fileInput2.ContentType == "image/jpg" ||
                             fileInput2.ContentType == "image/jpeg" ||
                             fileInput2.ContentType == "image/bmp" ||
                             fileInput2.ContentType == "image/gif")
                    {
                        fileBytes2 = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    Authorization_txtattach.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (Authorization_txtattach.ContentType == "application/pdf")
                    {
                        fileBytes3 = memoryStream.Length > 256 * 1024
                            ? doc.CompressFile(pdfData, ".pdf")
                            : memoryStream.ToArray();
                    }
                    else if (Authorization_txtattach.ContentType == "image/png" ||
                             Authorization_txtattach.ContentType == "image/jpg" ||
                             Authorization_txtattach.ContentType == "image/jpeg" ||
                             Authorization_txtattach.ContentType == "image/bmp" ||
                             Authorization_txtattach.ContentType == "image/gif")
                    {
                        fileBytes3 = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }
                SqlParameter[] pr = new SqlParameter[17]; // Adjusted size to 10

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };
                pr[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 15) { Value = ifscCode1 };
                pr[2] = new SqlParameter("@ifsc_int", SqlDbType.VarChar, 15) { Value = ifscCode2 };
                pr[3] = new SqlParameter("@transfer_id", SqlDbType.BigInt) { Value = applicationid };
                pr[4] = new SqlParameter("@customer_id", SqlDbType.BigInt) { Value = Session["ncd_bankadd_customerid"] };
                pr[5] = new SqlParameter("@branch_id", SqlDbType.Int) { Value = Session["branch_id"] };
                pr[6] = new SqlParameter("@bank_id", SqlDbType.BigInt) { Value = bank_id1 };
                pr[7] = new SqlParameter("@bank_id_int", SqlDbType.BigInt) { Value = bank_id2 };
                pr[8] = new SqlParameter("@bankac", SqlDbType.VarChar, 50) { Value = accountNo1 };
                pr[9] = new SqlParameter("@bankac_int", SqlDbType.VarChar, 50) { Value = accountNo2 };
                pr[10] = new SqlParameter("@bank_attach", SqlDbType.VarBinary) { Value = fileBytes1 };
                pr[11] = new SqlParameter("@bank_attach_int", SqlDbType.VarBinary) { Value = fileBytes2 };
                pr[12] = new SqlParameter("@attachement", SqlDbType.VarBinary) { Value = fileBytes3 };
                pr[13] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };
                pr[14] = new SqlParameter("@payeename", SqlDbType.VarChar, 50) { Value = customerName1 };
                pr[15] = new SqlParameter("@payeename_int", SqlDbType.VarChar, 50) { Value = customerName2 };
                //pr[9] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                SqlParameter outputMsg = new SqlParameter("@outmsg", SqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };
                pr[16] = outputMsg;


                dbconnect.Open();
                // dt = dbconnect.ExecuteDataset("[dbo].[DEB_CustomerBank]", pr).Tables[0];
                dbconnect.ExecuteDataset("[dbo].[BOND_transfer_save]", pr);

                dbconnect.Close();
                string msg = pr[16].Value.ToString();

                return Json(new { success = true, message = msg });
            }
            catch (NotSupportedException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (SqlException ex)
            {
                // Log SQL exception here
                return Json(new { success = false, message = "Database error: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Log general exceptions here
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


    }
}