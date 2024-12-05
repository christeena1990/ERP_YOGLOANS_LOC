using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class BOND_scheduleController : Controller
    {
        db dbconnect = new db();
        // GET: BOND_schedule
        public ActionResult BOND_schedule()
        {
            return View();
        }

        [HttpGet]
        public JsonResult fill_schemeType()
        {
            SqlParameter[] pr1 = new SqlParameter[1];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 5;

            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[BOND_report]", pr1).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> schemeTypelist = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt1.Rows)
            {
                Dictionary<string, object> scheme = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    scheme[col.ColumnName] = row[col];
                }
                schemeTypelist.Add(scheme);
            }

            // Return the data as JSON
            return Json(schemeTypelist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult fill_issueNumber()
        {
            SqlParameter[] pr1 = new SqlParameter[1];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 6;

            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[BOND_report]", pr1).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> issuelist = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt1.Rows)
            {
                Dictionary<string, object> issues = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    issues[col.ColumnName] = row[col];
                }
                issuelist.Add(issues);
            }

            // Return the data as JSON
            return Json(issuelist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult btn_report_Click(NCD_shedule nCD_Shedule)
        {
            try
            {
                var branch = Session["CheckedFilterTreeValues"];

                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 7 };
                pr[1] = new SqlParameter("@start", SqlDbType.DateTime) { Value = DateTime.Parse(nCD_Shedule.toDate) };
                pr[2] = new SqlParameter("@inttype", SqlDbType.Char, 1) { Value = nCD_Shedule.scheme_type };
                pr[3] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = nCD_Shedule.issue_no };
                pr[4] = new SqlParameter("@branch", SqlDbType.VarChar, 5000) { Value = branch };

                DataSet ds = dbconnect.ExecuteDataset("[dbo].[BOND_report]", pr);

                // Process first table
                var resultList = new List<object>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var rowData = new
                    {
                        Issue_NO = row["Issue_NO"],
                        Branch = row["Branch"],
                        Certificate = row["Certificate"],
                        Customer_Name = row["Customer_Name"],
                        Amount = row["Amount"],
                        Rate = row["Rate"],
                        Interest_type = row["Interest_type"],
                        Duration = row["Duration"],
                        Dist_From = row["Dist_From"],
                        Dist_To = row["Dist_To"],
                        Issue_Dt = row["Issue_Dt"],
                        Maturity_Dt = row["Maturity_Dt"],
                        Bond_ID = row["Bond_ID"],
                        IFSC_Set = row["IFSC_Set"],
                        Bank_Set = row["Bank_Set"],
                        A_C_No_Set = row["A/C No Set"],
                        IFSC_Int = row["IFSC_Int"],
                        Bank_Int = row["Bank_Int"],
                        A_C_No_Int = row["A/C No Int"],
                        Customer_ID = row["Customer ID"],
                        PAN = row["PAN"],
                        Payable = row["Payable"]
                    };
                    resultList.Add(rowData);
                }

                // Process second table and add to resultList
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var rowData = new
                    {
                        Issue_NO = row[0],
                        Branch = row[1],
                        Certificate = row[2],
                        Customer_Name = row[3],
                        Amount = row[4],
                        Rate = row[5],
                        Interest_type = row[6],
                        Duration = row[7],
                        Dist_From = row[8],
                        Dist_To = row[9],
                        Allotment_Dt = row[10],
                        Matirity_Dt = row[11],
                        Deb_ID = row[12],
                        IFSC_Set = row[13],
                        Bank_Set = row[14],
                        A_C_No_Set = row[15],
                        IFSC_Int = row[16],
                        Bank_Int = row[17],
                        A_C_No_Int = row[18],
                        Customer_ID = row[19],
                        PAN = row[20],
                        Payable = row[21]
                    };
                    resultList.Add(rowData);
                }

                return Json(new { success = true, data = resultList });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}