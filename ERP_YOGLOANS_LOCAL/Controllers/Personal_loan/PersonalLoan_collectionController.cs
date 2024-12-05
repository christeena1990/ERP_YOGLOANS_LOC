using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PersonalLoan_collectionController : Controller
    {
        // GET: PersonalLoan_collection
        DB dbconnect = new DB();
        public ActionResult PersonalLoan_collection_view()
        {
            DataTable dt = PaymentMode_drop();
            ViewBag.PaymentModes = dt;
            return View();
        }
        public ActionResult GetSearchSuggestions(string search_txt)
        {
     
            SqlParameter[] pr = new SqlParameter[3];
        
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);

            pr[0].Value = 1;
            pr[1] = new SqlParameter("@search_txt", SqlDbType.VarChar, 50) { Value = search_txt };
       
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr).Tables[0];
            dbconnect.Close();
        
            var result = dt.AsEnumerable().Select(row => row["name"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SetCustomerSession(string LoanrId)
        {
            // Store the input value in the session
            Session["pl_loan_id"] = LoanrId;

            // Return a simple success response (optional)
            return Json(new { success = true });
        }
    
        public ActionResult grid_pl_loan_fill()
        {
            try
            {
                // Prepare the SQL parameter for the stored procedure
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@loan_no", SqlDbType.VarChar, 20);
                pr[0].Value = Session["pl_loan_id"].ToString(); // Ensure this session value is not null

                DataTable dt = new DataTable();

                // Open the database connection and execute the query
                dbconnect.Open();
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[pl_collection_display_data]", pr);
                dbconnect.Close();

                // Ensure that the DataSet contains at least one table
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dt = ds.Tables[0];

                    // Convert DataTable to a list of dictionaries
                    var list = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }
                        list.Add(dict);
                    }

                    return Json(new { success = true, data = list });
                }
                else
                {
                    // Handle case when no data is returned
                    return Json(new { success = false, message = "No data found for the given loan number." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error message
                return Json(new { success = false, message = ex.Message });
            }
        }

        public DataTable PaymentMode_drop()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }
        [HttpPost]
        public ActionResult Confirm_click(string concatenatedResult,string selectedValue,string totalCollectionAmount)
        {

            if(concatenatedResult !="" )
            {

                SqlParameter[] pr = new SqlParameter[6];

                pr[0] = new SqlParameter("@branch", SqlDbType.BigInt);
                pr[0].Value = Session["branch_id"];

                pr[1] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[1].Value = Session["login_user"];

                pr[2] = new SqlParameter("@data", SqlDbType.VarChar, 8000);
                pr[2].Value = concatenatedResult;

                pr[3] = new SqlParameter("@pay_mode", SqlDbType.BigInt);
                pr[3].Value = selectedValue;

                pr[4] = new SqlParameter("@totalamt", SqlDbType.BigInt);
                pr[4].Value = totalCollectionAmount;

                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[5].Direction = ParameterDirection.Output;

                dbconnect.Open();
                dbconnect.ExecuteDataset("[dbo].[pl_loan_collection_tmp_save]", pr);
                string msg = pr[5].Value.ToString();
                dbconnect.Close();

                return Json(new { success = true, message = msg });
            }
            else
            {
                return Json(new { success = false });
            }

        }

    }
}