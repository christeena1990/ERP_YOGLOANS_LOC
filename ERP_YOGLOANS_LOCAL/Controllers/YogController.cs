using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class YogController : BaseController
    {
        DB dbconnect = new DB();
        // GET: Yog

        
        public ActionResult Yog_customer_Index()
        {
            yog_customer_index_model viewModel = new yog_customer_index_model();
            Fill_counts(viewModel);


            return View(viewModel);
        }

        private void Fill_counts(yog_customer_index_model viewModel)
        {
            //Session["branch_id"] = "1";
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"].ToString();

            DataSet result = dbconnect.ExecuteDataset("[dbo].[cust_queries]", pr);

            if (result != null && result.Tables.Count >= 4)
            {
                viewModel.FirstTable = result.Tables[0];
                viewModel.SecondTable = result.Tables[1];
                viewModel.ThirdTable = result.Tables[2];
                viewModel.forthTable = result.Tables[3];
            }
            else
            {
               
            }
        }


        public ActionResult Yog_ncd_Index()
        {
         return View();  
        }

        public ActionResult Yog_equifax_Index()
        {
            return View();
        }

        public ActionResult Yog_vehicle_Index()
        {
            return View();
        }

        public ActionResult Yog_bond_Index()
        {
            return View();
        }

        public ActionResult Yog_business_Index()
        {
            return View();
        }

        public ActionResult Yog_miscellaneous_Index()
        {
            return View();
        }
    }
}