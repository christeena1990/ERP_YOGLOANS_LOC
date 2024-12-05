using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;
using ERP_YOGLOANS_LOCAL.Models.Legality_Model;
using System.Threading.Tasks;
using System.IO;


namespace ERP_YOGLOANS_LOCAL.Controllers.Business_Loan
{
    public class ApplicationForm_BLKeralaController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult ApplicationForm_BLKerala()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Fillform(string applicationId)
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@module_id", SqlDbType.BigInt) { Value = 11 };
            pr[1] = new SqlParameter("@application_id", SqlDbType.BigInt) { Value = applicationId };

            dbconnect.Open();
            DataSet ds = dbconnect.ExecuteDataset("application_merge", pr);
            dbconnect.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
                var applicationData = new
                {
                    ApplicationForm = GetTableData(ds.Tables[0]),
                    SanctionLetter = GetTableData(ds.Tables[1]),
                    DPN = GetTableData(ds.Tables[2]),
                    Agreement = GetTableData(ds.Tables[3]),
                    RepaymentSchedule = GetTableData(ds.Tables[4]),
                    TermsConditions = GetTableData(ds.Tables[5]),
                    Annexure = GetTableData(ds.Tables[6]),
                    DisbursementMemo = GetTableData(ds.Tables[7]),
                };

                var jsonData = JsonConvert.SerializeObject(new { success = true, data = applicationData });
                return Content(jsonData, "application/json");
            }
            else
            {
                return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<Dictionary<string, object>> GetTableData(DataTable table)
        {
            return table.AsEnumerable().Select(row =>
            {
                var dictionary = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    dictionary[column.ColumnName] = row[column] ?? DBNull.Value; // Handle null values
                }
                return dictionary;
            }).ToList();
        }

        [HttpPost]
        public async Task<ActionResult> SendDocToLeegality(HttpPostedFileBase pdfFile, string applicationId)
        {
            if (pdfFile != null && !string.IsNullOrEmpty(applicationId))
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Read PDF file into a memory stream
                    await pdfFile.InputStream.CopyToAsync(memoryStream);
                    byte[] pdfBytes = memoryStream.ToArray();

                    // Convert to base64 if required
                    string base64String = Convert.ToBase64String(pdfBytes);

                    // Create the Leegality request
                    Leegality_Business_request leegality_Business_Request = new Leegality_Business_request
                    {
                        profileId = "KZI4WJD",
                        file = new DocumentFile
                        {
                            name = "GeneratedPDF",
                            file = base64String // PDF as base64 string
                        },
                        irn = applicationId // Use the application ID
                    };

                    // Process invitees if needed (assuming you're fetching them from a database)
                    //DataTable dtInvitees = await FetchInviteesAsync(applicationId);
                    //if (dtInvitees != null && dtInvitees.Rows.Count > 0)
                    //{
                    //    leegality_Business_Request.invitees = dtInvitees.AsEnumerable().Select((row, index) => new Invitee
                    //    {
                    //        name = row["coappli_name"].ToString(),
                    //        email = row["email_id"].ToString(),
                    //        phone = index == 0 ? "9446332086" : "9567427023" // Example phone numbers
                    //    }).ToArray();
                    //}

                    // Manually set two invitees for testing
                    leegality_Business_Request.invitees = new[]
                    {
                        new Invitee
                        {
                            name = "SIGI RAJU",
                            email = "sigi@yogloans.com",
                            phone = "9446332086"
                        },
                        new Invitee
                        {
                            name = "ABHILA K S",
                            email = "abhila.chittilappilly@gmail.com",
                            phone = "9567427023"
                        },
                         new Invitee
                        {
                            name = "Renya K",
                            email = "itdev@yogloans.com",
                            phone = "9061050308"
                        }
                    };


                    // Serialize the request for API use
                    string inputJson = JsonConvert.SerializeObject(leegality_Business_Request);
                    LeegalityApiClient leegalityApiClient = new LeegalityApiClient();

                    // Send request to Leegality
                    string response = await leegalityApiClient.SendLeegalityRequestAsync(leegality_Business_Request);

                    // Return response back to the frontend
                    return Json(new { success = true, data = response });
                }
            }

            return Json(new { success = false, message = "File or application ID is missing" });
        }

        // Example method to fetch invitees, replace with your actual data fetching logic
        private async Task<DataTable> FetchInviteesAsync(string applicationId)
        {
            DataTable dtInvitees = new DataTable();
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 2 };
            pr[1] = new SqlParameter("@application_id", SqlDbType.VarChar, 14) { Value = applicationId };

            dbconnect.Open();
            dtInvitees = dbconnect.ExecuteDataset("[dbo].[Leegality_queries]", pr).Tables[0];
            dbconnect.Close();

            return dtInvitees;
        }




    }


}