using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_ApplicationFormDownloadController : Controller
    {
        DB dbconnect = new DB();
        // GET: Bond_ApplicationFormDownload
        public ActionResult Bond_ApplicationFormDownload()
        {



                var JointaccountDetails = fillgrid();
                ViewBag.accountDetails = JointaccountDetails;

                return View();
            }


            public DataTable fillgrid()
            {

                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1;

                pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr[1].Value = Session["branch_id"]; // Ensure branch_id is converted to an integer

                DataTable dt = new DataTable();
                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_application_form]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }


            public ActionResult Bond_ApplicationFormstatusupdate(string appid, string custid, string issueno)
            {

                SqlParameter[] pr = new SqlParameter[2];

                Session["appid"] = appid;
                Session["custid"] = custid;
                Session["issueno"] = issueno;

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 2;

                pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
                pr[1].Value = appid; // Ensure branch_id is converted to an integer

                DataTable dt = new DataTable();
                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_application_form]", pr);

                dbconnect.Close();



                return Json(new { success = true });
            }

        }
    }
























    