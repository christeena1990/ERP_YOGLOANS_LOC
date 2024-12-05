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
    public class HomeController : BaseController
    {
        
        DB dbconnect = new DB();
        //private readonly int _roleId=1;
        //private readonly db _dbconnect;

        //public HomeController(int roleId, db dbconnect)
        //{
        //    _roleId = roleId;
        //    _dbconnect = dbconnect;
        //}



        public ActionResult Index()

        {
            if (Session["roll_id"] == null)
            {
                // Redirect to login or an error page if the session is not available
                return RedirectToAction("Error_page");
            }

            int roleId = Convert.ToInt32(Session["roll_id"]);

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1; // Setting query_id to 1 to retrieve module list
            pr[1] = new SqlParameter("@role_id", SqlDbType.Int);
            pr[1].Value = roleId;


            DataTable dt = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];


            List<int> moduleIds = new List<int>();
            foreach (DataRow row in dt.Rows)
            {
                int moduleId = Convert.ToInt32(row["module_id"]);
                string moduleName = row["module_name"].ToString();
                Session["ModuleName"] = moduleName;

                moduleIds.Add(moduleId);
            }

            if (moduleIds == null || !moduleIds.Any())
            {
                // If moduleIds is null or empty, set ViewBag.ModuleIds to an empty list
                ViewBag.ModuleIds = new List<int>();
            }
            else
            {
                // Store the moduleIds list in the ViewBag
                ViewBag.ModuleIds = moduleIds;

            }

            return View();
        }


        [HttpPost]
        public JsonResult GetSubMenu(int moduleId)
        {
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2; // Setting query_id to 2 to retrieve submenu list
            pr[1] = new SqlParameter("@module_id", SqlDbType.BigInt);
            pr[1].Value = moduleId;
            pr[2] = new SqlParameter("@role_id", SqlDbType.Int);
            pr[2].Value = Session["roll_id"];



            DataSet ds = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr);

            if (ds.Tables.Count < 3)
            {
                return Json(new { error = "Expected three tables in the DataSet." }, JsonRequestBehavior.AllowGet);
            }

            // Extract module name from the first result set
            string moduleName = ds.Tables[0].Rows[0]["module_name"].ToString();
            Session["ModuleName"] = moduleName.ToUpper();  // Store module name in session
            Session["module_id"] = moduleId;

            // Extract menus from the second result set
            DataTable menuTable = ds.Tables[1]; // Assuming this is the menu table
            List<SubMenu> menus = new List<SubMenu>();
            foreach (DataRow row in menuTable.Rows)
            {
                SubMenu menu = new SubMenu
                {
                    SubMenuId = Convert.ToInt32(row["id"]), // Ensure 'id' matches your database column name
                    SubMenuName = row["menu_name"].ToString(), // Ensure 'menu_name' matches your database column name
                    SubMenuUrl = row["url"].ToString() // Ensure 'url' matches your database column name
                };
                // Clean up the URL if necessary
                menu.SubMenuUrl = new Uri(new Uri(Request.Url.GetLeftPart(UriPartial.Authority)), menu.SubMenuUrl).ToString();
                menus.Add(menu);
            }

            // Store menus in session
            Session["Menus"] = menus;

            // Extract submenus from the third result set
            DataTable submenuTable = ds.Tables[2]; // Assuming this is the submenu table
            List<SubMenu> submenus = new List<SubMenu>();
            foreach (DataRow row in submenuTable.Rows)
            {
                SubMenu submenu = new SubMenu
                {
                    SubMenuId = Convert.ToInt32(row["sub_id"]), // Ensure 'sub_id' matches your database column name
                    SubMenuName = row["submenu_name"].ToString(), // Ensure 'submenu_name' matches your database column name
                    SubMenuUrl = row["url"].ToString(), // Ensure 'url' matches your database column name
                    ParentMenuId = Convert.ToInt32(row["menu_id"]) // Ensure 'parent_id' matches your database column name
                };
                // Clean up the URL if necessary
                submenu.SubMenuUrl = new Uri(new Uri(Request.Url.GetLeftPart(UriPartial.Authority)), submenu.SubMenuUrl).ToString();
                submenus.Add(submenu);
            }

            // Store submenus in session
            Session["SubMenus"] = submenus;

            // Return menus and submenus as JSON response
            return Json(new { menus, submenus }, JsonRequestBehavior.AllowGet);
        }














        //public ActionResult Index()
        //{

        //    SqlParameter[] pr = new SqlParameter[2];
        //    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
        //    pr[0].Value = 1;
        //    pr[1] = new SqlParameter("@role_id", SqlDbType.Int);
        //    pr[1].Value = 1;

        //    DataTable dt = new DataTable();
        //    dt = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];

        //    List<int> moduleIds = new List<int>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        int moduleId = Convert.ToInt32(row["module_id"]);
        //        moduleIds.Add(moduleId);
        //    }

        //    // Store the moduleIds list in the ViewBag
        //    ViewBag.ModuleIds = moduleIds;

        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Error_page()
        {
          
            return View();
        }


        [HttpPost]
        public JsonResult sessionLogout()
        {
            // Clear the session
            Session.Clear();
            Session.Abandon();

            // Optionally, add any other cleanup code here

            return Json(new { success = true });
        }


    }
}