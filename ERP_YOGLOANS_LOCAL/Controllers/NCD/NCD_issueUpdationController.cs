using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_issueUpdationController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_issueUpdation()
        {
            return View();
        }

        [HttpGet]
        public JsonResult getData(string inputMonth)
        {
            // Split the string by "-"
            string[] parts = inputMonth.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 8;
            pr1[1] = new SqlParameter("@period", SqlDbType.Int); // Month
            pr1[1].Value = month;
            pr1[2] = new SqlParameter("@year", SqlDbType.BigInt); // Year
            pr1[2].Value = year;
            pr1[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[3].Direction = ParameterDirection.Output;

            DataSet ds = new DataSet();
            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[DEB_issue_registration]", pr1);
            dbconnect.Close();

            string message= string.Empty;
            message = pr1[3].Value.ToString();
            if (string.IsNullOrEmpty(message))
            {
                var response = new Dictionary<string, List<Dictionary<string, object>>>();

                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        var tableData = new List<Dictionary<string, object>>();
                        DataTable table = ds.Tables[i];

                        foreach (DataRow row in table.Rows)
                        {
                            var rowData = new Dictionary<string, object>();
                            foreach (DataColumn col in table.Columns)
                            {
                                rowData[col.ColumnName] = row[col];
                            }
                            tableData.Add(rowData);
                        }

                        // Add table data to response
                        response[$"Table{i + 1}"] = tableData; // Key names: Table1, Table2, ...
                    }
                }

                // Return all tables in a single JSON object
                return Json(new { success = true, data = response }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { success = false, data = message }, JsonRequestBehavior.AllowGet);
            }
           
           
        }

        [HttpPost]
        public JsonResult updateISIN_number(string issue_no,string reg_startdate,string inputValue)
        {
            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 9;
            pr1[1] = new SqlParameter("@isin_no", SqlDbType.VarChar,20); 
            pr1[1].Value = inputValue;
            pr1[2] = new SqlParameter("@issue_no", SqlDbType.BigInt); 
            pr1[2].Value = issue_no;
            pr1[3] = new SqlParameter("@startdate", SqlDbType.Date); 
            pr1[3].Value = reg_startdate;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt); 
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            //DataSet ds = dbconnect.ExecuteDataset("DEB_Application", pr2);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
           
        }


        [HttpPost]
        public JsonResult updateTrustDate(string month_year, string trust_date)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 10;
            pr1[1] = new SqlParameter("@trust_Date", SqlDbType.Date);
            pr1[1].Value = trust_date;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            //DataSet ds = dbconnect.ExecuteDataset("DEB_Application", pr2);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult updateROC_Date(string month_year, string roc_date)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 11;
            pr1[1] = new SqlParameter("@trust_Date", SqlDbType.Date);
            pr1[1].Value = roc_date;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);           
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult updateROC_id(string month_year, string roc_id)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 12;
            pr1[1] = new SqlParameter("@isin_no", SqlDbType.VarChar,20);
            pr1[1].Value = roc_id;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult updateSRN_No(string month_year, string srnNo)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 13;
            pr1[1] = new SqlParameter("@srn_no", SqlDbType.NVarChar, 20);
            pr1[1].Value = srnNo;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult updateCharge_Id(string month_year, string chargeNo)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[6];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 14;
            pr1[1] = new SqlParameter("@isin_no", SqlDbType.VarChar, 20);
            pr1[1].Value = chargeNo;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult updateSatisfactionDt(string month_year, string satisfy_dt,string satisfy_no)
        {
            string[] parts = month_year.Split('-');

            // Extract year and month
            string year = parts[0];
            string month = parts[1];

            SqlParameter[] pr1 = new SqlParameter[7];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 15;
            pr1[1] = new SqlParameter("@trust_Date", SqlDbType.Date);
            pr1[1].Value = satisfy_dt;
            pr1[2] = new SqlParameter("@period", SqlDbType.Int);
            pr1[2].Value = month;
            pr1[3] = new SqlParameter("@year", SqlDbType.BigInt);
            pr1[3].Value = year;
            pr1[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr1[4].Direction = ParameterDirection.Output;
            pr1[5] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr1[5].Value = Session["login_user"];
            pr1[6] = new SqlParameter("@isin_no", SqlDbType.VarChar,20);
            pr1[6].Value = satisfy_no;

            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_issue_registration", pr1);
            dbconnect.Close();

            string message = pr1[4].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }

    }
}