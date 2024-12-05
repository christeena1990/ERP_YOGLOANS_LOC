using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static ERP_YOGLOANS_LOCAL.Models.NCD_Models.NCD_Debenture_Scheme_Model;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_cheque_realisation_dishonourController : Controller
    {
        DB dbconnect = new DB();

        // Main action method
        public async Task<ActionResult> Bond_cheque_realisation_dishonour(int? selectedBankId = null)
        {
            var viewModel = new NCD_Debenture_Scheme_Model
            {
                Banks = GetBankList() // Fetch the list of banks
            };

            if (selectedBankId.HasValue)
            {
                var chequeRealizations = await GetChequeRealizations(selectedBankId.Value);
                ViewBag.ChequeRealizations = chequeRealizations.Data; // Store in ViewBag for view access
            }
            else
            {
                ViewBag.ChequeRealizations = new List<ChequeRealizationViewModel>(); // Empty list if no bank is selected
            }

            return View(viewModel);
        }



        // Method to get the list of banks
        private List<Bank> GetBankList()
        {
            var bankList = new List<Bank>();
            string branchId = Session["branch_id"] as string; // Get the branch ID as a string

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 8 };
            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int) { Value = branchId };

            // Execute stored procedure and fill DataSet
            DataSet ds = dbconnect.ExecuteDataset("[dbo].[BOND_application_HO]", pr);

            if (ds != null && ds.Tables.Count > 0)
            {
                // Loop through the data in the first table
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    bankList.Add(new Bank
                    {
                        SubAccNo = row["sub_accno"].ToString(),
                        SubName = row["sub_name"].ToString()
                    });
                }
            }

            return bankList.Distinct().ToList(); // Ensure distinct values
        }

        public async Task<NCD_Debenture_Scheme_Model> GetChequeRealizations(int selectedBankId)
        {
            var viewModel = new NCD_Debenture_Scheme_Model();


            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 }; // Adjust query_id as needed
                pr[1] = new SqlParameter("@br_bank", SqlDbType.Int) { Value = selectedBankId }; // Pass the selected bank ID

                // Execute stored procedure and fill DataSet
                DataSet ds = await Task.Run(() => dbconnect.ExecuteDataset("[dbo].[BOND_application_HO]", pr));

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0]; // Get the first DataTable

                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new Dictionary<string, object>
                            {
                                { "BranchName_Bond", row["branch_name"].ToString() },
                                { "ApplicationId_Bond", row["application_id"].ToString() },
                                { "AppMoneyNo_Bond", row["appmoney_no"].ToString() },
                                { "CustomerName_Bond", row["customer_name"].ToString() },
                                { "BankName_Bond", row["bank_name"].ToString() },
                                { "IFSCCode_Bond", row["ifsc_code"].ToString() },
                                { "BondAmount_Bond", Convert.ToDecimal(row["bond_amount"]) },
                                { "TrackId_Bond", Convert.ToInt32(row["track_id"]) },
                                { "ChequeNumber_Bond", row["cheque_number"].ToString() },
                                { "Amount_Bond", Convert.ToDecimal(row["amount"]) },
                                { "SubName_Bond", row["sub_name"].ToString() },
                                { "ChequeDate_Bond", Convert.ToDateTime(row["cheque_date"]) },
                                  //{ "ClearingDate", Convert.ToDateTime(row["clearing_date"]) }
                            };

                        viewModel.Data.Add(item);
                    }


                }
                else
                {
                    viewModel.OutMessage = "No data found.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                viewModel.OutMessage = "An error occurred while retrieving the cheque realization data.";
                throw; // Re-throw to preserve stack trace
            }

            return viewModel; // Return the populated viewModel
        }

        [HttpPost]
        public ActionResult RealizeCheque(List<ApprovalData> approvals)
        {
            try
            {
                int queryId = 6;

                // Concatenate approval data into a single string
                var dataBuilder = new StringBuilder();
                foreach (var approval in approvals)
                {
                    // Concatenate trackId, applicationId, and clearingDate
                    string entry = $"{approval.trackId}~{approval.clearingDate}~{approval.applicationId}";
                    dataBuilder.Append(entry + "$"); // Append each entry with a delimiter
                }

                // Remove the trailing '$' if present
                string concatenatedData = dataBuilder.ToString().TrimEnd('$');

                SqlParameter[] parameters = new SqlParameter[4];

                parameters[0] = new SqlParameter("@type", SqlDbType.Int);
                parameters[0].Value = queryId;

                parameters[1] = new SqlParameter("@data", SqlDbType.VarChar, 1000); // Adjust size as necessary
                parameters[1].Value = concatenatedData;

                parameters[2] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                parameters[2].Value = Session["login_user"].ToString();

                parameters[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                parameters[3].Direction = ParameterDirection.Output;

                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_application_HO_save]", parameters);

                string msg = parameters[3].Value != DBNull.Value ? parameters[3].Value.ToString() : "An error occurred.";
                return Json(new { result = msg });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return Json(new { result = "An error occurred while realizing the cheque." });
            }
        }


        [HttpPost]
        public ActionResult DishorCheque(List<ApprovalData> approvals)
        {
            try
            {
                int queryId = 7;

                // Concatenate approval data into a single string
                var dataBuilder = new StringBuilder();
                foreach (var approval in approvals)
                {
                    // Concatenate trackId, applicationId, and clearingDate
                    string entry = $"{approval.trackId}~{approval.clearingDate}~{approval.applicationId}";
                    dataBuilder.Append(entry + "$"); // Append each entry with a delimiter
                }

                // Remove the trailing '$' if present
                string concatenatedData = dataBuilder.ToString().TrimEnd('$');

                SqlParameter[] parameters = new SqlParameter[4];

                parameters[0] = new SqlParameter("@type", SqlDbType.Int);
                parameters[0].Value = queryId;

                parameters[1] = new SqlParameter("@data", SqlDbType.VarChar, 1000); // Adjust size as necessary
                parameters[1].Value = concatenatedData;

                parameters[2] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                parameters[2].Value = Session["login_user"].ToString();

                parameters[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                parameters[3].Direction = ParameterDirection.Output;

                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_application_HO_save]", parameters);

                string msg = parameters[3].Value != DBNull.Value ? parameters[3].Value.ToString() : "An error occurred.";
                return Json(new { result = msg });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return Json(new { result = "An error occurred while realizing the cheque." });
            }
        }
    }
}