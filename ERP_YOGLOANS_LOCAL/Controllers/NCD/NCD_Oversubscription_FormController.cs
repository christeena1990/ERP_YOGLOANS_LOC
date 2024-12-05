using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Web.Services.Description;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Oversubscription_FormController : Controller
    {
        DB dbconnect = new DB();
        // GET: NCD_Oversubscription_Form
        public ActionResult NCD_Oversubscription_Form()
        {
            NCD_Oversubscription_Formfill();
            return View();
        }

        public ActionResult NCD_Oversubscription_Formfill()
        {
            // Retrieve Application ID from Session
            string appid = Session["appid"] as string;
            var model = new NCD_Oversubscriptionmodel();

            try
            {
                // Validate if appid is null or empty
                if (string.IsNullOrEmpty(appid))
                {
                    return RedirectToAction("ErrorPage"); // Redirect to an error page if appid is missing
                }

                // Set up parameters for the stored procedure [dbo].[DEB_application_form]
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };
                pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = Convert.ToInt64(appid) };

                // Execute the stored procedure and retrieve the result as a DataSet
                dbconnect.Open();
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_application_form]", pr);
                dbconnect.Close();

                // Extract the first DataTable from the DataSet
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];

                    // Process the DataTable if it contains rows
                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];

                        // Populate the model with data
                       
                        model.CustomerName = row["customer_name"]?.ToString();
                        model.Address = row["address"]?.ToString();
                        model.LandMark = row["land_mark"]?.ToString();
                        model.Place = row["place"]?.ToString();
                        model.Post = row["post"]?.ToString();
                        model.AggrAmtFrom = row["aggr_amt_from"]?.ToString();
                        model.AggrAmtTo = row["aggr_amt_to"]?.ToString();
                        model.AggrAmtTo1000 = row["aggr_amt_to_1000"]?.ToString();
                        model.AggrAmtFrom1000 = row["aggr_amt_from_1000"]?.ToString();
                        model.IssueNo = row["issue_no"]?.ToString();
                        model.Date = row["date"]?.ToString();

                    }

                    else
                    {
                        ViewBag.Message = "No data found for the provided Application ID.";
                    }
                }
                else
                {
                    ViewBag.Message = "No tables returned from the database.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception and return an error view
                ViewBag.Error = "An error occurred while processing your request. Please try again later.";
                // Log `ex` for debugging
            }
          
            // Return the populated model to the view
            return View(model);
        }















    }
}