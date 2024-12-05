using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class canvasser_model
    {
        public string SearchInput { get; set; }
        public string SearchType { get; set; }


        public int CanvasserCode { get; set; }
        public string  can_list { get; set; }
        public int can_code { get; set; }

        public string  c_code { get; set; }
        public string can_name { get; set; }
        public string can_mob { get; set; }
        public string can_pan { get; set; }
        public string can_add { get; set; }
    }
}