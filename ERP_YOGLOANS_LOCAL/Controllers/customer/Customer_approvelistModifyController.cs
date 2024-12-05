using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_approvelistModifyController : Controller
    {

        DB dbconnect = new DB();


        [HttpGet]
        public async Task<ActionResult> Cus_modiapprovelist()
        {
            try
            {
                int branch_id = Convert.ToInt16(Session["branch_id"]); // Ensure this correctly retrieves branch_id

                // Call method to get data and OutMessage
                var resultModel = await GetModiSendforapprovalCustDtl(branch_id);

                // Create view model to pass to the view
                var viewModel = new Customer_Approval_ViewModel
                {
                    Columns = resultModel.Columns,
                    Data = resultModel.Data,
                    OutMessage = resultModel.OutMessage // Assuming OutMessage is retrieved and set correctly
                };

                // Set ViewBag for displaying OutMessage in view
                ViewBag.Message = resultModel.OutMessage;

                return View("Cus_modiapprovelist", viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while retrieving modified approval list.";
                return View(new Customer_Approval_ViewModel());
            }
        }



        private async Task<Customer_Approval_Result> GetModiSendforapprovalCustDtl(int branch_id)
        {
            Customer_Approval_Result viewModel = new Customer_Approval_Result();

            try
            {
                SqlParameter[] pr = new SqlParameter[4]; // Adjust the number of parameters as per your stored procedure
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1; // Assuming query_id = 1 for modified approval list
                pr[1] = new SqlParameter("@region_id", SqlDbType.Int);
                pr[1].Value = -1; // Default region_id as per your logic
                pr[2] = new SqlParameter("@branch_id", SqlDbType.Int);
                pr[2].Value = branch_id; // Pass branch_id obtained from session
                pr[3] = new SqlParameter("@outmsg", SqlDbType.NVarChar, -1);
                pr[3].Direction = ParameterDirection.Output; // Output parameter for @outmsg

                DataTable dt = new DataTable();

                // Execute stored procedure and fill DataSet
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr);

                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0]; // Assign DataTable from the first table in DataSet

                    var modisendapprovalCustDetailsList = new List<Dictionary<string, object>>();
                    var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new Dictionary<string, object>();

                        foreach (var columnName in columnNames)
                        {
                            item[columnName] = row[columnName];
                        }

                        modisendapprovalCustDetailsList.Add(item);
                    }

                    viewModel.Columns = columnNames;
                    viewModel.Data = modisendapprovalCustDetailsList;
                }
                else
                {
                    // Handle scenario where DataSet is null or does not contain any tables
                    viewModel.OutMessage = "Only HO employees can see the list...";
                    ViewBag.Message = viewModel.OutMessage;
                }

                // Get the value of @outmsg from the output parameter
                viewModel.OutMessage = pr[3].Value.ToString();

                return viewModel;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                ViewBag.Message = "An error occurred while retrieving modified approval list.";
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Select(Customer_Approval_ViewModel approval_ViewModel)
        {
            var customerID = approval_ViewModel.customer_id;
            var approve_type = approval_ViewModel.appr_type;
            var Slno = approval_ViewModel.sl_no;
            //var modi_status = approval_ViewModel.modi_status;

            Session["Apr_custID"] = customerID;
            Session["Approve_type"] = approve_type;/* approve_type=1 --> Fresh Approval 2-->Modify Approval*/
            Session["Slno"] = Slno;
            //Session["modification"] = modi_status;
            // Redirect to the next page
            return RedirectToAction("Cus_approve", "Customer_approve");

        }

        [HttpGet]
        public JsonResult GetRegions()
        {
            try
            {
                // Replace with your actual code to call stored procedure and fetch regions
                List<SelectListItem> regions = GetRegionOptionsFromStoredProcedure();

                return Json(regions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return Json(new { error = ex.Message });
            }
        }

        // Method to fetch regions from stored procedure
        private List<SelectListItem> GetRegionOptionsFromStoredProcedure()
        {
            int branch_id = Convert.ToInt32(Session["branch_id"]); // Adjust as needed

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7; // Query ID for fetching regions
            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = branch_id;

            DataTable dt = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr).Tables[0];

            // Populate dropdown list from DataTable
            List<SelectListItem> regionOptions = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                string region_id = row["region_id"].ToString();
                string region_name = row["region_name"].ToString();

                // Create SelectListItem and add to list
                regionOptions.Add(new SelectListItem { Value = region_id, Text = region_name });
            }

            return regionOptions;
        }



        [HttpPost]
        public async Task<ActionResult> SearchByRegion(int regionId)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1; // Assuming query_id 8 is for region search
                pr[1] = new SqlParameter("@region_id", SqlDbType.Int);
                pr[1].Value = regionId;

                DataTable dt = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr).Tables[0];

                var searchData = new List<Dictionary<string, object>>();
                var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

                foreach (DataRow row in dt.Rows)
                {
                    var item = new Dictionary<string, object>();
                    foreach (var columnName in columnNames)
                    {
                        item[columnName] = row[columnName];
                    }
                    searchData.Add(item);
                }

                return Json(new Customer_Approval_Result
                {
                    Columns = columnNames,
                    Data = searchData
                });




            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return Json(new { error = "An error occurred while searching by region." });
            }
        }


    }
}