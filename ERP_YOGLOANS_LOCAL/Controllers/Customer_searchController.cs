using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Security.Policy;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class Customer_searchController : Controller
    {
        DB dbconnect = new DB();
        // GET: Customer_search
        public ActionResult CustomerSearch()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCustomers(Customer_Search customer_Search)
        {

            SqlParameter[] pr = new SqlParameter[5];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@mbno", SqlDbType.VarChar, 15);
            pr[1].Value = customer_Search.mobNum;
            pr[2] = new SqlParameter("@name", SqlDbType.VarChar, 50);
            pr[2].Value = customer_Search.name;
            pr[3] = new SqlParameter("@cus_id", SqlDbType.BigInt);
            pr[3].Value = customer_Search.custId;
            pr[4] = new SqlParameter("@branchid", SqlDbType.Int);
            pr[4].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_search_application]", pr).Tables[0];
            dbconnect.Close();

            // Convert DataTable to a list of dictionaries
            List<Dictionary<string, object>> customerList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> customer = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    customer[col.ColumnName] = row[col];
                }
                customerList.Add(customer);
            }

            // Return the data as JSON
            return Json(customerList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult SetCustId_to_Session(long custId)
        {

            Session["Apr_custID"] = custId;

            // Return the data as JSON
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            //return Json(customerList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetApplicationStartPage(long custId)
        {
            if (Session["application_subid"] == null)
            {
                Session["application_subid"] = "0";
            }


            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 3;
            pr1[1] = new SqlParameter("@module", SqlDbType.BigInt);
            pr1[1].Value = Session["module_id"];
            pr1[2] = new SqlParameter("@cus_id", SqlDbType.BigInt);
            pr1[2].Value = custId;
            pr1[3] = new SqlParameter("@cus_sl_no", SqlDbType.Int);
            pr1[3].Value = Session["application_subid"];
            pr1[4] = new SqlParameter("@user", SqlDbType.Int);
            pr1[4].Value = Session["login_user"];            
            pr1[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr1[5].Direction = ParameterDirection.Output;
            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[cust_search_application]", pr1).Tables[0];
            dbconnect.Close();
            string message= pr1[5].Value.ToString();

            if (message == "1")
            {
                if (Session["application_subid"].ToString() == "150")
                {
                    Session["application_custId_150"] = custId;
                }
                else if (Session["application_subid"].ToString() == "151") 
                {
                    Session["application_custId_151"] = custId;
                }
                else if (Session["application_subid"].ToString() == "152")
                {
                    Session["application_custId_152"] = custId;
                }
                else if (Session["application_subid"].ToString() == "160")
                {
                    Session["application_custId_160"] = custId;
                }
                else if (Session["application_subid"].ToString() == "161")
                {
                    Session["application_custId_161"] = custId;
                }
                else if (Session["application_subid"].ToString() == "162")
                {
                    Session["application_custId_162"] = custId;
                }
                else {
                    Session["application_custId"] = custId;
                }
        


                SqlParameter[] pr = new SqlParameter[4];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 2;
                pr[1] = new SqlParameter("@module", SqlDbType.BigInt);
                pr[1].Value = Session["module_id"];
                pr[2] = new SqlParameter("@branchid", SqlDbType.Int);
                pr[2].Value = Session["branch_id"];
                pr[3] = new SqlParameter("@cus_sl_no", SqlDbType.Int);
                pr[3].Value = Session["application_subid"];
                DataTable dt = new DataTable();

                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[cust_search_application]", pr).Tables[0];
                dbconnect.Close();
                string url = dt.Rows[0]["application_start_url"].ToString();

                Session["application_subid"] = "0";
                // Return the data as JSON
                return Json(new { success = true, message = url } , JsonRequestBehavior.AllowGet);

            }
            else
            {
                // return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                // Check if dt1 contains data
                List<object> dt1Data = new List<object>();
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow row in dt1.Rows)
                    {
                        dt1Data.Add(new
                        {
                            Column1 = row["Module"].ToString(), 
                            Column2 = row["Branch"].ToString(),
                            Column3 = row["Loan"].ToString(),
                            Column4 = row["Reason"].ToString(),
                            // Add other columns as needed
                        });
                    }
                }

                // Return the message and dt1 data in the response
                return Json(new { success = false, message = message, dt1Data = dt1Data }, JsonRequestBehavior.AllowGet);

            }

        }

    }
}