using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_CirtificateController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult Bond_Cirtificate_view()
        {

            DataTable issue_types = fillissuedropdown();
            ViewData["IssueList"] = issue_types;

            DataTable bank_type = fillbranchdropdown();
            ViewData["BankList"] = bank_type;

            return View();
        }
        public DataTable fillissuedropdown()
        {

            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_certificate]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        public DataTable fillbranchdropdown()
        {

            DataTable dt = new DataTable();
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 2;
            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[BOND_certificate]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }
        [HttpPost]
        public JsonResult GetgridData(int bankId, int? issueNo = null)
        {
            Session["bankId_interestaprvl"] = bankId;
            Session["issueNo_interestaprvl"] = issueNo;

            try
            {
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 3 };
                pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = bankId };
                pr[2] = new SqlParameter("@issue_no", SqlDbType.BigInt);

                if (issueNo != null)
                {
                    pr[2].Value = issueNo;
                }
                else
                {
                    pr[2].Value = null;
                }
                dbconnect.Open();
                DataTable dt = dbconnect.ExecuteDataset("[dbo].[BOND_certificate]", pr).Tables[0];
                dbconnect.Close();

                return Json(new { success = true, data = dt.AsEnumerable().Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col])) });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult StoreDataForBlankPage(string checkedData)
        {

            Session["Bond_cirtificate_BondId"] = checkedData;


            return Json(new { success = true });
        }
    }
}