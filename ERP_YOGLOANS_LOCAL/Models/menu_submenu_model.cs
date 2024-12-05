using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class menu_submenu_model
    {
       public int role_id_drop {  get; set; }
        public int module_id_drop { get; set; }        
        public int menu_id_drop { get; set; }
        public string menu_name {  get; set; }       
        public char submenu_yesno {  get; set; }
        public int submenu_id_drop {  get; set; }
        public string submenu_name { get;  set; }
        public string url {  get; set; }       
        public char menu_type { get; set; }
        
       public char submenu_new_exists { get; set; }

    }

    public class SubMenu
    {
        public int SubMenuId { get; set; }
        public string SubMenuName { get; set; }
        public string SubMenuUrl { get; set; }
        public int ParentMenuId { get; set; } // Add this property if it doesn't exist

    }



    public class ModuleMenuViewModel
    {
        public string ModuleName { get; set; }
        public List<SubMenu> Menus { get; set; }
        public List<SubMenu> SubMenus { get; set; }
    }



}