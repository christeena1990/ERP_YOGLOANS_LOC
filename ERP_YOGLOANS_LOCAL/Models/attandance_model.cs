using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class attendance_model
    {

        public string EmployeeCode_att { get; set; }

        public string server_time { get; set; }

        public string Password_att { get; set; }
        public string Employee_name { get; set; }

        public string Employee_branch { get; set; }
        public string Employee_shift_time { get; set; }
        public string hidden_time { get; set; }
    }
}