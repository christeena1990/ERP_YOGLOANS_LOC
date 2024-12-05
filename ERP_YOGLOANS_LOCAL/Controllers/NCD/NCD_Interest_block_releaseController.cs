using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.Drawing.Imaging;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Interest_block_releaseController : Controller
    {
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();
        doc_compression ImageCompress = new doc_compression();
        // GET: NCD_Interest_block_release
        public ActionResult NCD_Interest_block_release()
        {
            return View();
        }

        public ActionResult GetSearchSuggestions(string search_txt)
        {
            // Define the SQL parameters as per your existing method
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 14 };
            pr[1] = new SqlParameter("@searchdata", SqlDbType.VarChar, 50) { Value = search_txt };
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = Session["branch_id"] };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr).Tables[0];
            dbconnect.Close();
            // Create a list to hold the results
            var result = dt.AsEnumerable().Select(row => new
            {
                Name = row["name"].ToString(),
                DebId = row["deb_id"].ToString() // Ensure you get the DebID
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetIssueDetails(string issue_no, string certifi_no)
        {

            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
            pr1[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = issue_no };
            pr1[2] = new SqlParameter("@cert_no", SqlDbType.VarChar, 15) { Value = certifi_no };
            pr1[3] = new SqlParameter("@module_id", SqlDbType.VarChar, 15) { Value = Session["module_id"] };

            // Execute the stored procedure and get the results in a DataTable
            DataSet dt_dtls = new DataSet();
            dbconnect.Open();
            dt_dtls = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1);
            dbconnect.Close();

            DataTable dt1 = dt_dtls.Tables[0];
            DataTable dt2 = dt_dtls.Tables[1];
            DataTable dt3 = dt_dtls.Tables[2];

            List<Dictionary<string, object>> issuelist = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> banklist = new List<Dictionary<string, object>>();
            string comment = "";
            if (dt1 != null && dt1.Rows.Count > 0)
            {

                foreach (DataRow row in dt1.Rows)
                {
                    Dictionary<string, object> issue = new Dictionary<string, object>();
                    foreach (DataColumn col in dt1.Columns)
                    {
                        issue[col.ColumnName] = row[col];
                    }
                    issuelist.Add(issue);
                }

            }
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                foreach (DataRow row in dt2.Rows)
                {
                    Dictionary<string, object> bank = new Dictionary<string, object>();
                    foreach (DataColumn col in dt2.Columns)
                    {
                        bank[col.ColumnName] = row[col];
                    }
                    banklist.Add(bank);
                }
            }
            if (dt3 != null && dt3.Rows.Count > 0)
            {
                comment = dt3.Rows[0]["cmnt"].ToString();


            }
            var result = new
            {
                Issue = issuelist,
                Bank = banklist,
                Comment = comment
            };
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetBlockedIssueDetails(string issue_no, string certifi_no)
        {

            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 6 };
            pr1[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = issue_no };
            pr1[2] = new SqlParameter("@cert_no", SqlDbType.VarChar, 15) { Value = certifi_no };
            pr1[3] = new SqlParameter("@module_id", SqlDbType.BigInt) { Value = Session["module_id"] };

            // Execute the stored procedure and get the results in a DataTable
            DataTable dt_dtls = new DataTable();
            dbconnect.Open();
            dt_dtls = dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1).Tables[0];
            dbconnect.Close();            

            List<Dictionary<string, object>> issueDetailList = new List<Dictionary<string, object>>();
            
            if (dt_dtls != null && dt_dtls.Rows.Count > 0)
            {

                foreach (DataRow row in dt_dtls.Rows)
                {
                    Dictionary<string, object> issue = new Dictionary<string, object>();
                    foreach (DataColumn col in dt_dtls.Columns)
                    {
                        issue[col.ColumnName] = row[col];
                    }
                    issueDetailList.Add(issue);
                }

            }
           
            return Json(issueDetailList, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult SaveBlockRequest(NCD_Interest_Block request)
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

            SqlParameter[] pr1 = new SqlParameter[7];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 2 };
            pr1[1] = new SqlParameter("@id", SqlDbType.BigInt) { Value = request.deb_id };
            pr1[2] = new SqlParameter("@module_id", SqlDbType.Int) { Value = Session["module_id"] };
            pr1[3] = new SqlParameter("@blk_req_by", SqlDbType.BigInt) { Value = Session["login_user"] };
            pr1[4] = new SqlParameter("@blk_req_reason", SqlDbType.NVarChar, 200) { Value = request.Comments };
            pr1[5] = new SqlParameter("@data", SqlDbType.Binary) { Value = docBytes };
            pr1[6] = new SqlParameter("@out_msg", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
            

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1);
            dbconnect.Close();


            string msg = pr1[6].Value.ToString();
            return Json(new { success = true, message = msg }); 
        }

        [HttpPost]
        public ActionResult SaveBlockReleaseRequest(NCD_Interest_release request)
        {            

            SqlParameter[] pr1 = new SqlParameter[5];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 7 };
            pr1[1] = new SqlParameter("@rel_req_by", SqlDbType.BigInt) { Value = Session["login_user"] };
            pr1[2] = new SqlParameter("@blk_req_reason", SqlDbType.NVarChar, 200) { Value = request.Comments };
            pr1[3] = new SqlParameter("@sl_no", SqlDbType.BigInt) { Value = request.deb_id };
            pr1[4] = new SqlParameter("@out_msg", SqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };         
            
            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_BOND_interest_blk_proc]", pr1);
            dbconnect.Close();


            string msg = pr1[4].Value.ToString();
            return Json(new { success = true, message = msg });
        }
    }
}