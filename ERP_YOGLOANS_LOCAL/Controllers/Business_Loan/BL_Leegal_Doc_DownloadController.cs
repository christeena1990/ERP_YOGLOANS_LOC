using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.Legality_Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using System.Net;

namespace ERP_YOGLOANS_LOCAL.Controllers.Business_Loan
{
    public class BL_Leegal_Doc_DownloadController : Controller
    {
        DB dbconnect = new DB();
        // GET: BL_Leegal_Doc_Download
        public ActionResult BL_Leegal_Doc_Download()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> fillGrid()
        {
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 6;
            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Leegality_queries]", pr).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> customerList = new List<Dictionary<string, object>>();
            try
            {              

                if (dt.Rows.Count > 0)
                {
                    customerList = await GetCombinedCustomerListAsync(dt);
                }

            }
            catch (Exception ex)
            {
                string message= ex.Message;
            }
           

            // Return the data as JSON
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<List<Dictionary<string, object>>> GetCombinedCustomerListAsync(DataTable dt)
        {
            var fetcher = new Leegality_DocumentDetailsFetcher();
            List<Dictionary<string, object>> BL_customerList = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                // Initialize a dictionary for each row
                Dictionary<string, object> customers = new Dictionary<string, object>();

                // Populate the dictionary with DataTable row data
                foreach (DataColumn col in dt.Columns)
                {
                    customers[col.ColumnName] = row[col];
                }

                // Fetch the document status for each document_id
                string documentId = row["document_id"].ToString();
                var documentStatus_Jsonresponse = await fetcher.GetDocumentDetailsAsync(documentId);

                if (documentStatus_Jsonresponse != null)
                {
                    Leegality_docStatusClass documentStatus_apiresponse = JsonConvert.DeserializeObject<Leegality_docStatusClass>(documentStatus_Jsonresponse);

                    if (documentStatus_apiresponse.Status == 1)
                    {
                        /// Add API response data to the dictionary
                        customers["DocumentStatus"] = documentStatus_apiresponse.Data.Status;

                        // Concatenate signed status for each invite
                        string inviteSignedStatus = string.Join(", ", documentStatus_apiresponse.Data.Invitations.Select(i => $"{i.Name}: {(i.Signed ? "Signed" : "Not Signed")}"));
                        customers["InviteSignedStatus"] = inviteSignedStatus;
                    }
                    else
                    {
                        customers["DocumentStatus"] = "Error";
                        customers["InviteSignedStatus"] = "Error fetching data";
                    }

                }
               

                // Add the populated dictionary to the customer list
                BL_customerList.Add(customers);
            }

            return BL_customerList;
        }

        [HttpGet]
        public async Task<ActionResult> DownloadAndSaveDocument(string documentId,string appli_id)
        {
            Leegality_docDownloadApi _docApi = new Leegality_docDownloadApi();
            // Fetch document bytes
            byte[] documentBytes = await _docApi.FetchDocumentAsync(documentId);

            if (documentBytes == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Failed to fetch document.");
            }

            // Save document bytes to the database
            bool isSaved = SaveDocumentToDatabase(appli_id, documentBytes, documentId);

            if (!isSaved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Failed to save document to database.");
            }

            // Return file for download in the browser
            return File(documentBytes, "application/pdf", $"{documentId}.pdf");
        }

        private bool SaveDocumentToDatabase(string appli_id, byte[] documentBytes,string documentId)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 7;
                pr[1] = new SqlParameter("@application_id", SqlDbType.VarChar,14);
                pr[1].Value = appli_id;
                pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[2].Value = Session["login_user"];
                pr[3] = new SqlParameter("@data", SqlDbType.Binary);
                pr[3].Value = documentBytes;
                pr[4] = new SqlParameter("@document_id", SqlDbType.VarChar,20);
                pr[4].Value = documentId;

                
                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[Leegality_queries]", pr);
                dbconnect.Close();

                return true;
            }
            catch (Exception ex)
            {
                // Log error or handle it accordingly
                Console.WriteLine("Database error: " + ex.Message);
                return false;
            }
        }

    }
}