using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class menu_submenuController : Controller
    {
        // GET: menu_submenu
        DB dbconnect = new DB();
        public ActionResult add_menu_submenu()
        {

            DataTable role_types = role_id_type();
           // DataTable dt_role = new DataTable();
            ViewData["role_typeList"] = role_types;

            DataTable module_types = module_id_type();
          //  DataTable dt_module = new DataTable();
            ViewData["module_typeList"] = module_types;

            //DataTable menu_types = menu_id_type(25);
            //DataTable dt_menu = new DataTable();
            //ViewData["menu_typeList"] = menu_types;



            return View();
        }
        public DataTable role_id_type()
        {

            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            DataTable dt_role = new DataTable();
            dt_role = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];

            return dt_role;

        }
        public DataTable module_id_type()
        {

            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            DataTable dt_module = new DataTable();
            dt_module = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];

            return dt_module;

        }
      
        [HttpPost]
        public ActionResult menu_id_type(int module_id)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;

            pr[1] = new SqlParameter("@module_id", SqlDbType.Int);
            pr[1].Value = module_id;

            DataTable menu_types = new DataTable();
            menu_types = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];

            // Serialize DataTable to JSON
            string jsonMenuTypes = JsonConvert.SerializeObject(menu_types);

            // Return JSON response
            return Json(jsonMenuTypes);
        }
        [HttpPost]
        public ActionResult submenu_id_type(int menu_id)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@menu_id", SqlDbType.Int);
            pr[1].Value = menu_id;

            DataTable submenu_types = new DataTable();
            submenu_types = dbconnect.ExecuteDataset("[dbo].[SP_Menus]", pr).Tables[0];
          
            string jsonsubMenuTypes = JsonConvert.SerializeObject(submenu_types);
            
            return Json(jsonsubMenuTypes);
        }
       

        [HttpPost]
        public JsonResult add_menu_submenu_submit(menu_submenu_model model)
        {
            try
            {
                if (model.menu_type == 'E' && model.submenu_yesno == 'Y' && model.submenu_new_exists == 'E')
                {
                    try
                    {
                        SqlParameter[] pr1 = new SqlParameter[4];

                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 11 };                                              
                        pr1[1] = new SqlParameter("@menu_id", SqlDbType.Int) { Value = model.menu_id_drop };
                        pr1[2] = new SqlParameter("@role_id", SqlDbType.Int) { Value = model.role_id_drop };
                        pr1[3] = new SqlParameter("@submenu_id", SqlDbType.Int) { Value = model.submenu_id_drop.ToString()=="-1"?"0": model.submenu_id_drop.ToString() };
                        
                        dbconnect.ExecuteStoredProcedure("[SP_Menus]", pr1);

                        return Json(new { success = true, message = "Menu and submenu added successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Menu and submenu added fail!" });
                    }
                }
                if (model.menu_type == 'E' && model.submenu_yesno == 'Y' && model.submenu_new_exists == 'N')
                {
                    try
                    {
                        SqlParameter[] pr1 = new SqlParameter[5];

                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 10 };   
                        pr1[1] = new SqlParameter("@method_controller", SqlDbType.NVarChar, 200) { Value = model.url };
                        pr1[2] = new SqlParameter("@submenu_name", SqlDbType.NVarChar, 200) { Value = model.submenu_name };
                        pr1[3] = new SqlParameter("@menu_id", SqlDbType.Int) { Value = model.menu_id_drop };
                        pr1[4] = new SqlParameter("@role_id", SqlDbType.Int) { Value = model.role_id_drop };

                        dbconnect.ExecuteStoredProcedure("[SP_Menus]", pr1);

                        return Json(new { success = true, message = "Menu and submenu added successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Menu and submenu added fail!" });
                    }
                }
                if (model.menu_type == 'N' && model.submenu_yesno =='Y' && model.submenu_new_exists=='N' )
                {
                    try
                    {
                        SqlParameter[] pr1 = new SqlParameter[7];

                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 9 };
                        pr1[1] = new SqlParameter("@module_id", SqlDbType.Int) { Value = model.module_id_drop };
                        pr1[2] = new SqlParameter("@menu_name", SqlDbType.VarChar, 50) { Value = model.menu_name };
                        pr1[3] = new SqlParameter("@subMenu_Status", SqlDbType.Char, 1) { Value = model.submenu_yesno };
                        pr1[4] = new SqlParameter("@method_controller", SqlDbType.NVarChar, 200) { Value = model.url };
                        pr1[5] = new SqlParameter("@submenu_name", SqlDbType.VarChar, 50) { Value = model.submenu_name };
                        pr1[6] = new SqlParameter("@role_id", SqlDbType.Int) { Value = model.role_id_drop };

                        dbconnect.ExecuteStoredProcedure("[SP_Menus]", pr1);

                        return Json(new { success = true, message = "Menu and submenu added successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Menu and submenu added fail!" });
                    }
                }
                if (model.menu_type == 'N' && model.submenu_yesno == 'N')
                {
                    try 
                    { 
                        SqlParameter[] pr1 = new SqlParameter[6];

                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 8 };
                        pr1[1] = new SqlParameter("@module_id", SqlDbType.Int) { Value = model.module_id_drop };
                        pr1[2] = new SqlParameter("@menu_name", SqlDbType.VarChar, 50) { Value = model.menu_name };
                        pr1[3] = new SqlParameter("@subMenu_Status", SqlDbType.Char, 1) { Value = model.submenu_yesno };
                        pr1[4] = new SqlParameter("@method_controller", SqlDbType.NVarChar, 200) { Value = model.url };
                        pr1[5] = new SqlParameter("@role_id", SqlDbType.Int) { Value = model.role_id_drop };

                        dbconnect.ExecuteStoredProcedure("[SP_Menus]", pr1);

                        return Json(new { success = true, message = "Menu and submenu added successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Menu and submenu added fail!" });
                    }
                }
                if (model.menu_type == 'E' && model.submenu_yesno == 'N')
                {
                    try
                    {
                        SqlParameter[] pr1 = new SqlParameter[4];

                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 7 };
                        pr1[1] = new SqlParameter("@menu_id", SqlDbType.Int) { Value = model.menu_id_drop };
                        pr1[2] = new SqlParameter("@role_id", SqlDbType.Int) { Value = model.role_id_drop };
                        pr1[3] = new SqlParameter("@module_id", SqlDbType.Int) { Value = model.module_id_drop };
                        // pr1[3] = new SqlParameter("@submenu_id", SqlDbType.Int) { Value = model.submenu_id_drop.ToString() == "-1" ? "0" : model.submenu_id_drop.ToString() };

                        dbconnect.ExecuteStoredProcedure("[SP_Menus]", pr1);

                        return Json(new { success = true, message = "Menu and submenu added successfully." });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Menu and submenu added fail!" });
                    }
                }
                return Json(new { success = true, message = "Menu and submenu added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Menu and submenu added fail!" });
            }
        }

    }
}