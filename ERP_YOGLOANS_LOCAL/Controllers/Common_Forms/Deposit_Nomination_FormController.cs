using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.Bond_models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Common_Forms
{
    public class Deposit_Nomination_FormController : Controller
    {
        DB dbconnect = new DB();
        // GET: Deposit_Nomination_Form
        public ActionResult Deposit_Nomination_Form_View()
        {
            Deposit_Nomination_Form_fill();
            return View();
        }


     

        [HttpPost]
        public ActionResult Deposit_Nomination_Form_fill()
        {
            var model = new nominee_details();
            var applicationId = Session["appid"]?.ToString();
            var moduleId = Session["module_id"]?.ToString(); // Get module_id from session

            if (string.IsNullOrEmpty(moduleId))
            {
                // Handle case when module_id is not set
                ViewBag.ErrorMessage = "Module ID is missing.";
                return View(model);
            }

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 6;
            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[1].Value = applicationId;

            dbconnect.Open();
            DataSet ds = new DataSet();

            if (moduleId == "15")
            {
                // Execute DEB_application_form if module_id is 15
                ds = dbconnect.ExecuteDataset("[dbo].[DEB_application_form]", pr);
            }
            else if (moduleId == "16")
            {
                // Execute BOND_application_form if module_id is 16
                ds = dbconnect.ExecuteDataset("[dbo].[BOND_application_form]", pr);
            }
            else
            {
                // Handle case when module_id does not match any known value
                ViewBag.ErrorMessage = "Invalid Module ID.";
                dbconnect.Close();
                return View(model);
            }

            dbconnect.Close();

            // Process the dataset if it's not null and has tables
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable nominee_dt = ds.Tables[0];
                if (nominee_dt != null && nominee_dt.Rows.Count > 0)
                {
                    var row = nominee_dt.Rows[0];
                    model.customer_name = row[0].ToString();
                    model.nominee_Name = row[1].ToString();
                    model.relation = row[2].ToString();
                    model.nominee_add = row[3].ToString();
                }
            }

            return View(model);
        }



        //[HttpPost]
        //public ActionResult Deposit_Nomination_Form_fill()
        //{
        //    var model = new nominee_details();
        //    var applicationId = Session["appid"]?.ToString();
        //    SqlParameter[] pr = new SqlParameter[2];
        //    pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
        //    pr[0].Value = 6;
        //    pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
        //    pr[1].Value = applicationId;
        //    dbconnect.Open();
        //    DataSet ds = new DataSet();
        //    ds = dbconnect.ExecuteDataset("[dbo].[BOND_application_form]", pr);
        //    dbconnect.Close();
        //    if (ds != null && ds.Tables.Count >= 0)
        //    {
        //        DataTable nominee_dt = ds.Tables[0];
        //        if (nominee_dt != null && nominee_dt.Rows.Count > 0)
        //        {
        //            var row = nominee_dt.Rows[0];
        //            model.customer_name= row[0].ToString();
        //            model.nominee_Name = row[1].ToString();
        //            model.relation = row[2].ToString();
        //            model.nominee_add = row[3].ToString();

        //        }
        //    }

        //    return View(model);
        //}




    }

}