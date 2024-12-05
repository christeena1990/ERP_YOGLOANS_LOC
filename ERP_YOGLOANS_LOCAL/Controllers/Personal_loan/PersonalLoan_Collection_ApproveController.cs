using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using iTextSharp.text;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PersonalLoan_Collection_ApproveController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult PersonalLoan_Collection_Approve_view()
        {
            return View();
        }
        public DataTable customerList_drop()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }
        public JsonResult GetCustomerList()
        {
            DataTable dt = customerList_drop();
            var customerList = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                customerList.Add(new SelectListItem
                {
                    Text = row["customer_name"].ToString(), // Replace 'CustomerName' with your actual column name
                    Value = row["customer_id"].ToString()  // Replace 'CustomerID' with your actual column name
                });
            }

            return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        public DataTable customer_branch_drop()
        {

            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        public JsonResult GetBranchList()
        {
            DataTable dt = customer_branch_drop();
            var customerList = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                customerList.Add(new SelectListItem
                {
                    Value = row["branch_id"].ToString(), // Replace 'CustomerName' with your actual column name
                    Text = row["branch_name"].ToString()  // Replace 'CustomerID' with your actual column name
                });
            }

            return Json(customerList, JsonRequestBehavior.AllowGet);
        }
        public DataTable bank_drop()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@branchid", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[service_charge_collection]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        public JsonResult GetBankList()
        {
            DataTable dt = bank_drop();
            var customerList = new List<SelectListItem>();

            foreach (DataRow row in dt.Rows)
            {
                customerList.Add(new SelectListItem
                {
                    Value = row["sub_accno"].ToString(), // Replace 'CustomerName' with your actual column name
                    Text = row["sub_name"].ToString()  // Replace 'CustomerID' with your actual column name
                });
            }

            return Json(customerList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCustomerDetails_grid_fill(string customerId,string branch_id)
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;
            if(customerId==null || customerId =="")
            {
                pr[1] = new SqlParameter("@customer_id", SqlDbType.Int);
                pr[1].Value = -1;
            }
            else
            {
                pr[1] = new SqlParameter("@customer_id", SqlDbType.Int);
                pr[1].Value = customerId.ToString();
            }
          

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];
            if(branch_id == null || branch_id == "")
            {
                  pr[3] = new SqlParameter("@sel_branch", SqlDbType.BigInt);
                  pr[3].Value =-1;

            }
            else
            {
                pr[3] = new SqlParameter("@sel_branch", SqlDbType.BigInt);
                pr[3].Value = branch_id;
            }
         

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr).Tables[0];
            dbconnect.Close();

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

        public ActionResult approve_click(string slnos)
        {
            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[0].Value = Session["login_user"];

            pr[1] = new SqlParameter("@branch", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];

            pr[2] = new SqlParameter("@data", SqlDbType.VarChar, 8000);
            pr[2].Value = slnos;

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 8000);
            pr[3].Direction = ParameterDirection.Output;

            pr[4] = new SqlParameter("@pay_mode", SqlDbType.BigInt);
            pr[4].Value = 1;

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[pl_collection_entry]", pr);
            string msg = pr[3].Value.ToString();
            dbconnect.Close();


            string[] parts = msg.Split(new[] { '@' }, 2);
            Session["voucher_part0"] = parts[0];
            string part2 = parts[1];

            // Return part1 and part2 in the JSON response
            return Json(new { success = true, part2 = part2 });



          
        }

        public ActionResult Reject_click(string slnos)
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 8;

            pr[1] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[1].Value = Session["login_user"];

            pr[2] = new SqlParameter("@data", SqlDbType.VarChar, 8000);
            pr[2].Value = slnos;

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 8000);
            pr[3].Direction = ParameterDirection.Output;

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[pl_collection_queries]", pr);
            string msg = pr[3].Value.ToString();
            dbconnect.Close();

            return Json(new { success = true });
        }
    }
}