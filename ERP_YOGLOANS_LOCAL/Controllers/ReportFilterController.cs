using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class ReportFilterController : Controller
    {
        DB dbconnect = new DB();
        // GET: ReportFilter
        public ActionResult ReportFilter_treeview()
        {
            if (Session["modulename"] == null)
            {
                return RedirectToAction("Login", "Smart_Login");
            }
            // This will replace the Page_Load event
            if (Session["modulename"] != null)
            {
                ViewBag.ModuleName = Session["modulename"].ToString();
                ViewBag.Branch = Session["branch"].ToString();
                ViewBag.User = Session["name"].ToString();
            }
            PopulateMenuItems();
            return View();
        }
        private void PopulateMenuItems()
        {
            SqlParameter[] pr1 = new SqlParameter[4];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr1[0].Value = 1;
            pr1[1] = new SqlParameter("@userid", SqlDbType.Int);
            pr1[1].Value = Session["login_user"];
            pr1[2] = new SqlParameter("@module_id", SqlDbType.Int);
            pr1[2].Value = Session["module_id"];
            pr1[3] = new SqlParameter("@branchid", SqlDbType.Int);
            pr1[3].Value = Session["branch_id"];

            DataTable TreemenuData = dbconnect.ExecuteDataset("[report_menu]", pr1).Tables[0];
            ViewBag.TreeReportItems = TreemenuData;
        }


        [HttpPost]
        public JsonResult GetCheckedValues(string checkedValues)
        {
            var checkedValuesArray = checkedValues?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Join the array into a comma-separated string
            //string commaSeparatedValues = string.Join(",", checkedValuesArray);
            string commaSeparatedValues = string.Join(",", checkedValuesArray) + ",";


            // Store the comma-separated string in the session
            Session["CheckedFilterTreeValues"] = commaSeparatedValues;

            //Session["CheckedFilterTreeValues"] = checkedValuesArray;
            return Json(new { success = true, checkedValues = checkedValuesArray });
        }







    }
}
