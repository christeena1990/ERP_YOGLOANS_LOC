using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_Scheme_ApprovalController : Controller
    {
        DB dbconnect = new DB();

        [HttpGet]
        public async Task<ActionResult> Bond_Scheme_Approval()
        {
            try
            {
                // Call method to get data
                var schemeApprovalData = await GetSchemeApprovalData(4); // Assuming query_id = 4 for scheme approval

                return View("Bond_Scheme_Approval", schemeApprovalData);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                ViewBag.ErrorMessage = "An error occurred while retrieving the scheme approval list.";
                return View(new NCD_Debenture_Scheme_Model()); // Ensure this matches the expected model in your view
            }
        }


        private async Task<NCD_Debenture_Scheme_Model> GetSchemeApprovalData(int queryId)
        {
            var viewModel = new NCD_Debenture_Scheme_Model
            {
                Columns = new List<string>(),
                Data = new List<Dictionary<string, object>>()
            };

            try
            {
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = queryId };

                // Execute stored procedure and fill DataSet
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[BOND_issue_registration]", pr);

                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0]; // Get the first DataTable

                    // Populate column names
                    viewModel.Columns = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

                    // Populate data
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new Dictionary<string, object>();
                        foreach (var columnName in viewModel.Columns)
                        {
                            item[columnName] = row[columnName];
                        }
                        viewModel.Data.Add(item);
                    }
                }
                else
                {
                    viewModel.OutMessage = "No data found.";
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                // Log the exception
                ViewBag.Message = "An error occurred while retrieving the scheme approval data.";
                throw; // Re-throw to preserve stack trace
            }
        }

        [HttpGet]
        public ActionResult GetDetails(string issue_no)
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };
            pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = Convert.ToInt64(issue_no) };

            DataSet ds = new DataSet();
            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[BOND_issue_registration]", pr);
            dbconnect.Close();

            var ncdSchemeDetails = new List<NCD_Debenture_Scheme_Model.NCDDetails>(); // List to store details

            // Check if the dataset has any tables and rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ncdSchemeDetails.Add(new NCD_Debenture_Scheme_Model.NCDDetails()
                    {

                        TotalBondAmount = row["totbond_amt"].ToString(),
                        FaceValue_Bond = Convert.ToSingle(row["face_value"]),
                        No_of_bond = Convert.ToInt32(row["total_bond"]),
                        MinAmount_Bond = Convert.ToSingle(row["min_amount"]),
                        LockingPeriod_Bond = Convert.ToInt32(row["locking_period"]),
                        SeniorCitizenInterest_Bond = row["senior_citizen_interest"].ToString(),
                        MeetDate_Bond = row["meet_date"].ToString(),
                        RocDate_bond = row["roc_date"].ToString(),
                        TrustDate_Bond = row["trust_Date"].ToString(),
                        CanvasserCommission_Bond = row["canvasser_commission"].ToString(),

                    });
                }
            }


            return Json(ncdSchemeDetails, JsonRequestBehavior.AllowGet); // Return the list of details
        }

        [HttpGet]
        public ActionResult GetCanvasserIncentives(string issue_no)
        {
            // Similar logic to fetch canvasser incentives from the database
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 }; // Change this according to your stored procedure logic
            pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = Convert.ToInt64(issue_no) };

            DataSet ds = new DataSet();
            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[BOND_issue_registration]", pr);
            dbconnect.Close();

            var canvasserIncentives = new List<NCD_Debenture_Scheme_Model.CanvasserModel>();

            if (ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    canvasserIncentives.Add(new NCD_Debenture_Scheme_Model.CanvasserModel()
                    {
                        AmountRange_Bond = row["Amount_Range"].ToString(),
                        Commission_Bond = Convert.ToDecimal(row["Commission"])
                    });
                }
            }

            return Json(canvasserIncentives, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ApproveIssue(string issue_no)
        {
            try
            {


                int queryId = 6;

                SqlParameter[] parameters = new SqlParameter[4];

                parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                parameters[0].Value = queryId;

                parameters[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                parameters[1].Value = Convert.ToInt64(issue_no);

                parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                parameters[2].Direction = ParameterDirection.Output;

                parameters[3] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                parameters[3].Value = Session["login_user"].ToString();

                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_issue_registration]", parameters);

                string msg = parameters[2].Value != DBNull.Value ? parameters[2].Value.ToString() : string.Empty;
                return Json(new { result = msg });


            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");


                return RedirectToAction("NCD_Scheme_Approval");
            }
        }

        [HttpPost]
        public ActionResult RejectIssue(string issue_no)
        {
            try
            {


                int queryId = 7;

                SqlParameter[] parameters = new SqlParameter[4];

                parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                parameters[0].Value = queryId;

                parameters[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                parameters[1].Value = Convert.ToInt64(issue_no);

                parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                parameters[2].Direction = ParameterDirection.Output;

                parameters[3] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                parameters[3].Value = Session["login_user"].ToString();

                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_issue_registration]", parameters);

                string msg = parameters[2].Value != DBNull.Value ? parameters[2].Value.ToString() : string.Empty;
                return Json(new { result = msg });


            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");


                return RedirectToAction("NCD_Scheme_Approval");
            }
        }

    }
}