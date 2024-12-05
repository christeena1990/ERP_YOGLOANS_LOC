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

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_ChequePresentedController : Controller
    {

        DB dbconnect = new DB();

        [HttpGet]
        public async Task<ActionResult> NCD_ChequePresented()
        {
            try
            {
                // Call method to get data
                var chequepresentedData = await GetchequepresentedData(3); // Assuming query_id = 4 for scheme approval

                return View("NCD_ChequePresented", chequepresentedData);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                ViewBag.ErrorMessage = "An error occurred while retrieving the scheme approval list.";
                return View(new NCD_Debenture_Scheme_Model()); // Ensure this matches the expected model in your view
            }
        }

        private async Task<NCD_Debenture_Scheme_Model> GetchequepresentedData(int queryId)
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
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_application_HO]", pr);

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




    }
}