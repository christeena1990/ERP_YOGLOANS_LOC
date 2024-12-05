using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP_YOGLOANS_LOCAL.Models.personal_loan_models.PLForms_Model;
using System.IO;
using System.Drawing.Imaging; // For ImageFormat

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_Disbursement_MemoController : Controller
    {
        DB dbconnect = new DB();

        // GET: PL_Disbursement_Memo
        public ActionResult PL_Disbursement_Memo_View()
        {
           
            LoanApplicationViewModel Model = new LoanApplicationViewModel();

            try
            {
                // Define SQL parameters
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 14 };
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14) { Value = Session["PL_application_id"] };
                pr[2] = new SqlParameter("@enter_by", SqlDbType.VarChar, 14) { Value = Session["login_user"] };

                // Execute the stored procedure and get the DataSet
                DataSet ds = new DataSet();
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                if (ds.Tables.Count > 0)
                {
                    // Check and process the first table
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt_ApplctnForm = ds.Tables[0];

                        Model.BranchName = dt_ApplctnForm.Rows[0]["branch_name"].ToString();
                        Model.ApplctnId = dt_ApplctnForm.Rows[0]["application_id"].ToString();
                        Model.CustomerName = dt_ApplctnForm.Rows[0]["customer_name"].ToString();

                        // Build address
                        var addressParts = new List<string>
                {
                    dt_ApplctnForm.Rows[0]["address"].ToString(),
                    dt_ApplctnForm.Rows[0]["land_mark"].ToString(),
                    dt_ApplctnForm.Rows[0]["post_name"].ToString(),
                    dt_ApplctnForm.Rows[0]["city"].ToString(),
                    dt_ApplctnForm.Rows[0]["district_name"].ToString(),
                    dt_ApplctnForm.Rows[0]["pin_code"].ToString()
                };
                        Model.Address = string.Join(", ", addressParts.Where(part => !string.IsNullOrWhiteSpace(part)));

                        // Map other properties
                        Model.PLinterestRate = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["int_rate"]);
                        Model.LoanAmountProposed = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["loan_amount"]);
                        Model.receivables = dt_ApplctnForm.Rows[0]["receivables"].ToString();
                        Model.TransferAmount = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["tfr_amt"]);
                        Model.DisbursementAmount = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["disb_amt"]);
                        Model.PenalCharges = dt_ApplctnForm.Rows[0]["penal_charge"].ToString();
                        Model.Date = dt_ApplctnForm.Rows[0]["dt"].ToString();
                        Model.Payeename = dt_ApplctnForm.Rows[0]["payee_name"].ToString();
                        Model.BankName = dt_ApplctnForm.Rows[0]["bank_name"].ToString();
                        Model.BankBranch = dt_ApplctnForm.Rows[0]["branch_name"].ToString();
                        Model.AcctNo = dt_ApplctnForm.Rows[0]["acc_no"].ToString();
                        Model.IFSC = dt_ApplctnForm.Rows[0]["ifsc_code"].ToString();
                    }

                    // Check and process the second table
                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        DataTable dt_user = ds.Tables[1];

                        Model.Username = dt_user.Rows[0]["name"].ToString();
                        Model.usercode = dt_user.Rows[0]["code"].ToString();
                        Model.userDesignation = dt_user.Rows[0]["designation"].ToString();
                        Model.UserBranchName = dt_user.Rows[0]["branch"].ToString();
                        Model.UserDate = dt_user.Rows[0]["dt"].ToString(); // Adjusted to a more appropriate field
                    }

                    // Check and process the third table
                    if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    {
                        DataTable dt_HOuser = ds.Tables[2];

                        Model.HOUsername = dt_HOuser.Rows[0]["name"].ToString();
                        Model.HOusercode = dt_HOuser.Rows[0]["code"].ToString();
                        Model.HOuserDesignation = dt_HOuser.Rows[0]["designation"].ToString();
                        Model.HOUserBranchName = dt_HOuser.Rows[0]["branch"].ToString();
                        Model.HOUserDate = dt_HOuser.Rows[0]["dt"].ToString(); // Adjusted to a more appropriate field
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return View(Model);
        }
        [HttpPost]
        public JsonResult SaveMemoAsImage(string appId)
        {
            try
            {
                // Generate your memo content as an image
                byte[] imageBytes = CreateImage(appId); // Ensure this method creates and returns the image bytes

                // Define your SQL parameters for saving to the database
                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 16 }; // Your query ID for upload Disbursement Memo
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar) { Value = appId };
                pr[2] = new SqlParameter("@data1", SqlDbType.VarBinary, imageBytes.Length) { Value = imageBytes }; // Store image as varbinary
                pr[3] = new SqlParameter("@enter_by", SqlDbType.VarChar) { Value = Session["login_user"].ToString() };

                // Output parameter for the message
                SqlParameter outMsgParam = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) // Adjust size as necessary
                {
                    Direction = ParameterDirection.Output
                };

                // Add the output parameter to the array
                pr[4] = outMsgParam;

                // Open DB connection and execute the stored procedure
                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                // Clear application_id from session
                //Session["PL_application_id"] = null;

                // Retrieve the message from the output parameter
                string outputMessage = outMsgParam.Value.ToString();

                return Json(new { success = true, message = outputMessage });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private byte[] CreateImage(string appId)
        {
            // Create a bitmap to draw on
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(800, 600))
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    // Fill the background
                    g.Clear(System.Drawing.Color.White);

                    // Draw the content
                    g.DrawString("DISBURSEMENT MEMO", new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.Black, new System.Drawing.PointF(10, 10));
                    g.DrawString($"Application ID: {appId}", new System.Drawing.Font("Arial", 16), System.Drawing.Brushes.Black, new System.Drawing.PointF(10, 50));
                    g.DrawString("Your other details go here...", new System.Drawing.Font("Arial", 16), System.Drawing.Brushes.Black, new System.Drawing.PointF(10, 80));

                    // Add more drawing as needed
                }

                // Save to a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png); // Save as PNG
                    return ms.ToArray(); // Return the byte array
                }
            }
        }

    }
}