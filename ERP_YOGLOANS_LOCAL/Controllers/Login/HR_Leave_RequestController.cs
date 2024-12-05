
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.Threading.Tasks;

namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{
    public class HR_Leave_RequestController : Controller
    {
        public ActionResult HRLeaveRequest()
        {
            // Check if "login_user" is not null before proceeding
            if (Session["login_user"] != null)
            {
                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 21;
                pr[1] = new SqlParameter("@emp_id", SqlDbType.BigInt);
                pr[1].Value = Session["login_user"];

                DB dbconnect = new DB(); // Instantiate your db connection
                dt = dbconnect.ExecuteDataset("[dbo].[HRsalary_edit]", pr).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    return View(dt); 

                }
            }

            return Content("");
        }

        public JsonResult CheckHRLeaveRequestData()
        {
            if (Session["login_user"] != null)
            {
                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 21;
                pr[1] = new SqlParameter("@emp_id", SqlDbType.BigInt);
                pr[1].Value = Session["login_user"];

                DB dbconnect = new DB(); // Instantiate your db connection
                dt = dbconnect.ExecuteDataset("[dbo].[HRsalary_edit]", pr).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    return Json(new { hasData = true }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { hasData = false }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult demo_demo()
        {
            return View();
        }
    }
}
