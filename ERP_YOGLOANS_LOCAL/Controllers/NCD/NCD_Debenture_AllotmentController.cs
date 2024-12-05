using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Xml.Linq;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Debenture_AllotmentController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_Debenture_Allotment_View()
        {
            return View();
        }
        public DataTable customerList_drop()
        {

            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_application_HO]", pr).Tables[0];
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
                    Text = row["value"].ToString(), // Replace 'CustomerName' with your actual column name
                    Value = row["val"].ToString()  // Replace 'CustomerID' with your actual column name
                });
            }

            return Json(customerList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCustomerDetails_grid_fill(string issue_no)
        {
            Session["issue_no_allpt_ncd"] = issue_no;

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7;

            pr[1] = new SqlParameter("@issue_no", SqlDbType.Int);
            pr[1].Value = issue_no.ToString();


            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_application_HO]", pr).Tables[0];
            dbconnect.Close();
            if (dt == null || dt.Rows.Count == 0)
            {
                return Json(new { success = false, Message = "No Data Available!" });
            }
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
        public JsonResult Approve_BtnClick(string concate_app_id)
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@app_data", SqlDbType.VarChar, 4000);
            pr[0].Value = concate_app_id;

            pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
            pr[1].Value = Session["issue_no_allpt_ncd"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];


            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[3].Direction = ParameterDirection.Output;

          

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[ncd_save_allotments]", pr);
            dbconnect.Close();

            string msg = pr[3].Value.ToString();
            if (msg == "Confirmed Sucessfully...!")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}