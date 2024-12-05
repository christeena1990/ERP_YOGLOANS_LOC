using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;
using ERP_YOGLOANS_LOCAL.Models.Bond_models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_TransferRequestController : Controller
    {

        DB dbconnect = new DB();
        // GET: NCD_TransferRequest
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();
        public ActionResult Bond_TransferRequest()
        {
            return View();
        }
        public ActionResult GetSearchSuggestions(string search_txt)
        {
            string branchId = Session["branch_id"] as string;


            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
            pr[1] = new SqlParameter("@searchdata", SqlDbType.VarChar, 50) { Value = search_txt };
            pr[2] = new SqlParameter("@branch_id", SqlDbType.VarChar, 50) { Value = Session["branch_id"] };


            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr).Tables[0];
            dbconnect.Close();

            // Create a list to hold the results
            var result = dt.AsEnumerable().Select(row => new
            {
                Name = row["name"].ToString(),
                bond_id = row["bond_id"].ToString() // Ensure you get the DebID
            }).ToList();


            //var result = dt.AsEnumerable().Select(row => row["name"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDetails(string issue_no, long bond_id, string cerno)
        {
            SqlParameter[] pr = new SqlParameter[4];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 2 };
            pr[1] = new SqlParameter("@bond_id", SqlDbType.BigInt) { Value = bond_id };
            pr[2] = new SqlParameter("@issue_no", SqlDbType.VarChar) { Value = issue_no };
            pr[3] = new SqlParameter("@cerno", SqlDbType.VarChar, 50) { Value = cerno };

            DataSet ds = new DataSet();
            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr);
            dbconnect.Close();

            var bondTransferRequestDetails = new List<Bond_Transfer_Model>();

            // Check if the dataset has any tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    bondTransferRequestDetails.Add(new Bond_Transfer_Model()
                    {
                        Amount = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0m,
                        CustomerName = row["customer_name"].ToString(),
                        IssueNo = row["issue_no"] != DBNull.Value ? Convert.ToInt64(row["issue_no"]) : 0, // Use long here
                        CertificateNo = row["certificate_no"].ToString(),
                        InterestType = row["interest_type"].ToString(),
                        InterestRate = row["interest_rate"] != DBNull.Value ? Convert.ToDecimal(row["interest_rate"]) : 0m,
                        Period = row["period"] != DBNull.Value ? Convert.ToInt32(row["period"]) : 0,
                        bond_id = bond_id,  // Assuming you want to keep track of the deb_id
                        Certificate_No = cerno // Assuming this is also relevant to store
                    });
                }
            }

            return Json(bondTransferRequestDetails, JsonRequestBehavior.AllowGet);
        }

        //private bool IsPdf(byte[] byteData)
        //{
        //    // Check if the byte array represents a PDF file
        //    return byteData.Length > 4 &&
        //           byteData[0] == 0x25 &&
        //           byteData[1] == 0x50 &&
        //           byteData[2] == 0x44 &&
        //           byteData[3] == 0x46 &&
        //           byteData[4] == 0x2D;
        //}


        //private string GetImageUrl(byte[] imageBytes)
        //{


        //    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

        //    if (IsPdf(imageBytes))
        //    {
        //        // It's a PDF
        //        string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
        //        //kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
        //        return $"data:application/pdf;base64,{base64String}";

        //    }
        //    else
        //    {
        //        // It's an image
        //        string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
        //        // kycItem.Add("image", "data:image/png;base64," + base64Image);
        //        return $"data:image;base64,{base64String}";
        //    }


        //}


        [HttpPost]
        public ActionResult SubmitTransferRequest(Bond_Transfer_Model model)
        {
            try
            {
                // Ensure the session values are not null
                int BranchId = Convert.ToInt32(Session["branch_id"]);
                int Enter_by = Convert.ToInt32(Session["login_user"]);

                // Initialize the output message
                string outMsg = "Success"; // Default message
               // byte[] imageBytes = null;


                // Ensure these properties are correctly populated
                long issueNo = model.IssueNo;
                string certif = model.CertificateNo;
                int transtype = model.Transtype; // This should be set by your JavaScript


                // Check if the image is uploaded
                //if (model.image != null && model.image.ContentLength > 0)
                //{
                //    using (var memoryStream = new MemoryStream())
                //    {
                //        // Copy the uploaded file to memory stream
                //        model.image.InputStream.CopyTo(memoryStream);
                //        memoryStream.Position = 0;

                //        // Handle file compression based on content type
                //        switch (model.image.ContentType)
                //        {
                //            case "application/pdf":
                //                imageBytes = (memoryStream.Length > 256 * 1024)
                //                    ? doc.CompressFile(memoryStream.ToArray(), ".pdf")
                //                    : memoryStream.ToArray();
                //                break;

                //            case "image/png":
                //            case "image/jpg":
                //            case "image/jpeg":
                //            case "image/bmp":
                //            case "image/gif":
                //                imageBytes = (memoryStream.Length > 256 * 1024)
                //                    ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                //                    : memoryStream.ToArray();
                //                break;

                //            default:
                //                throw new NotSupportedException("Unsupported file type.");
                //        }
                //    }
                //}

                // Prepare SQL parameters for stored procedure
                SqlParameter[] parameters = new SqlParameter[8];

                parameters[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 }; // Assuming a default value
                parameters[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = model.IssueNo };
                parameters[2] = new SqlParameter("@certif", SqlDbType.VarChar, 15) { Value = model.CertificateNo };
                parameters[3] = new SqlParameter("@branch_id", SqlDbType.Int) { Value = BranchId };
                parameters[4] = new SqlParameter("@bond_id", SqlDbType.BigInt) { Value = model.bond_id }; // Now populated
                parameters[5] = new SqlParameter("@transtype", SqlDbType.Int) { Value = model.Transtype };
               // parameters[6] = new SqlParameter("@data", SqlDbType.VarBinary) { Value = imageBytes ?? (object)DBNull.Value };
                parameters[6] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };

                parameters[7] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Enter_by };


                // Execute the stored procedure
                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_transfer_save]", parameters);
                dbconnect.Close();

                // Retrieve the output message
                outMsg = parameters[6].Value != DBNull.Value ? parameters[6].Value.ToString() : "";

                // Return the result as JSON
                return Json(new { result = outMsg });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return Json(new { result = "An error occurred while submitting the transfer request." });
            }
        }

    }
}