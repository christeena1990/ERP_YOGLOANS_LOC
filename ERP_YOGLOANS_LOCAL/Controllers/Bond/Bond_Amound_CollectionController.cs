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
using System.Drawing.Imaging;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_Amound_CollectionController : Controller
    {
        // GET: Bond_Amound_Collection
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();
        public ActionResult Bond_Amound_Collection_View()
        {
            var customerBankDetails = Collection_Customer_Grid();

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
        private DataTable Collection_Customer_Grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        private DataTable paymode_drop()
        {
            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr).Tables[0];
            dbconnect.Close();
            return dt;
        }
        public ActionResult GetPayModes()
        {
            // Call the method that gets the pay mode data
            DataTable payModeTable = paymode_drop();

            // Convert DataTable to a list of SelectListItems
            List<SelectListItem> payModes = new List<SelectListItem>();

            foreach (DataRow row in payModeTable.Rows)
            {
                payModes.Add(new SelectListItem
                {
                    Value = row["paymode_val"].ToString(), // Assuming the ID column is 'PayModeId'
                    Text = row["paymode_text"].ToString() // Assuming the name column is 'PayModeName'
                });
            }

            // Return the list as JSON
            return Json(payModes, JsonRequestBehavior.AllowGet);
        }
        private DataTable Bank_select(string ifsc_code)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 15);
            pr[1].Value = ifsc_code;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr).Tables[0];
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
        public ActionResult Add_Btn_click_grid_fill_ready(string applicationId)
        {
            try
            {


                SqlParameter[] pr11 = new SqlParameter[2];
                pr11[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 8 };
                pr11[1] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = applicationId };



                DataTable dt = new DataTable();

                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[bond_AmountCollection]", pr11).Tables[0];
                dbconnect.Close();
                if (dt.Rows.Count <= 0)
                {
                    return Json(new { sucess = false, message = "No documnet available!" });

                }


                return Json(new
                {
                    success = true,
                    data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])).ToList(),
                    headers = dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList()  // Return column headers
                });

            }
            catch (Exception ex)
            {
                // Log the error (ex) as needed
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public ActionResult Add_Btn_click(FormCollection form, HttpPostedFileBase bank_attachment)
        {
            try
            {
                byte[] fileBytes;

                using (var memoryStream = new MemoryStream())
                {
                    bank_attachment.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (bank_attachment.ContentType == "application/pdf")
                    {
                        fileBytes = memoryStream.Length > 256 * 1024
                            ? doc.CompressFile(pdfData, ".pdf")
                            : memoryStream.ToArray();
                    }
                    else if (bank_attachment.ContentType == "image/png" ||
                             bank_attachment.ContentType == "image/jpg" ||
                             bank_attachment.ContentType == "image/jpeg" ||
                             bank_attachment.ContentType == "image/bmp" ||
                             bank_attachment.ContentType == "image/gif")
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

                long applicationId = Convert.ToInt64(form["application_id"]);
                Session["application_id_coll_"] = applicationId.ToString();
                string paymode = form["paymode"];
                string payeeName = form["payee_name"];
                string bankName = form["bankName"];
                string branchName = form["branchName"];
                string accountNo = form["AccountNo"];
                string amount = form["amount"];
                string chequeTransDate = form["cheque_transDate"];
                string referenceTextbox = form["Referance_textbox"];
                int bankId = Convert.ToInt32(form["bank_hid_id"]);

                string concateData = $"{paymode}*{accountNo}*{payeeName}*{chequeTransDate}*{referenceTextbox}*{amount}*{bankId}";

                SqlParameter[] pr11 = new SqlParameter[5];
                pr11[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };
                pr11[1] = new SqlParameter("@enter_by", SqlDbType.Int) { Value = Convert.ToInt32(Session["login_user"]) };
                pr11[2] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = applicationId };
                pr11[3] = new SqlParameter("@bank_attach", SqlDbType.VarBinary) { Value = fileBytes };
                pr11[4] = new SqlParameter("@candata", SqlDbType.VarChar, 4000) { Value = concateData };

                DataTable dt = new DataTable();

                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr11).Tables[0];
                dbconnect.Close();



                return Json(new
                {
                    success = true,
                    data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])).ToList(),
                    headers = dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList()  // Return column headers
                });

            }
            catch (Exception ex)
            {
                // Log the error (ex) as needed
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        public ActionResult row_delete_click(string track_id)
        {
            SqlParameter[] pr11 = new SqlParameter[3];
            pr11[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 6 };
            pr11[1] = new SqlParameter("@track_id", SqlDbType.Int) { Value = track_id };
            pr11[2] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = Session["application_id_coll_"] };

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr11).Tables[0];
            dbconnect.Close();
            return Json(new
            {
                success = true,
                data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])).ToList(),
                headers = dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList()  // Return column headers
            });

        }
        public ActionResult popup_click(string track_id)
        {
            SqlParameter[] pr11 = new SqlParameter[3];
            pr11[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 7 };
            pr11[1] = new SqlParameter("@track_id", SqlDbType.Int) { Value = track_id };
            pr11[2] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = Session["application_id_coll_"] };

            DataTable dt = new DataTable();
            ncd_bank_add_model model = new ncd_bank_add_model();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr11).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No documnet available!" });

            }
            else
            {
                model.ImageUrl1 = GetImageUrl(dt.Rows[0]["attachment_data"] as byte[]);

            }

            return Json(new { data = model.ImageUrl1 }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult submit_Btn_click()
        {



            // Prepare SQL parameters
            SqlParameter[] pr11 = new SqlParameter[4];

            pr11[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 3 };
            pr11[1] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
            pr11[2] = new SqlParameter("@enter_by", SqlDbType.Int) { Value = Convert.ToInt32(Session["login_user"]) };
            pr11[3] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = Session["application_id_coll_"] };

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[BOND_AmountCollection]", pr11);
            dbconnect.Close();
            string msg = pr11[1].Value.ToString();


            return Json(new { success = true, data = msg });
        }
    }
}