using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Smart_Login_model
    {

        //public int UserId { get; set; }

       
        public string EmployeeCode { get; set; }

       
        public string EmployName { get; set; }

      
        public string EmployPassword { get; set; }

        public int RollId { get; set; }

        
        public string BranchName { get; set; }
        

        //public string branch_id { get; set; }

        //public string login_user { get; set; }

        // Add other properties as needed

        //public static Smart_Login_model GetByEmployeeCode(string employeeCode)
        //{
        //    return null;
        //}
    }

    public class user
    {

        //public int UserId { get; set; }


        public string EmployeeCode { get; set; }


        public string EmployName { get; set; }


        public string EmployPassword { get; set; }

        public int RollId { get; set; }


        public string BranchName { get; set; }

        // Add other properties as needed

        //public static Smart_Login_model GetByEmployeeCode(string employeeCode)
        //{
        //    return null;
        //}
    }


    public class Reset_pswd_model
    {


        public string currentpswd { get; set; }
        public string newpswd { get; set; }
        public string confirmpswd { get; set; }
    }

    public class forgot_Password_Model
    {

        public string empcode { get; set; }

        public string hint { get; set; }


        public string newpswd { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("newpswd", ErrorMessage = "Password and Confirm Password do not match")]
        [Display(Name = "Confirm Password")]
        public string cnfmpswd { get; set; }
    }

  


}
