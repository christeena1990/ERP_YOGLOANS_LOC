using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Data.Common;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_approvelistController : BaseController
    {

        DB dbconnect = new DB();


        [HttpGet]
        public async Task<ActionResult> Cus_approvelist()
        {
            // Console.WriteLine("Cus_approvelist action called");
            var approvalList = await GetSendforapprovalCustDtl();

            var viewModel = new Customer_Approval_ViewModel
            {
                Columns = null, // Set it to its default value or empty list/array
                Data = null      // Set it to its default value or empty list/array
                                 // Add other properties if needed
            };
            viewModel.Columns = approvalList.Columns;
            viewModel.Data = approvalList.Data;
            return View(viewModel);
            //return View("Cus_approvelist", viewModel);
        }

        private async Task<Customer_Approval_Result> GetSendforapprovalCustDtl()
        {
            //Session["branch_id"] = "1";
            var branch_id = Session["branch_id"];

           // string apiUrl = "http://10.3.0.223:8081/api/getSendforapprovalCustDtl?branch_id=" + branch_id;

            string apiUrl = "http://10.10.0.47:83/api/getSendforapprovalCustDtl?branch_id=" + branch_id;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Customer_Approval_Result>(data);

                return new Customer_Approval_Result
                {
                    Columns = result.Columns,
                    Data = result.Data
                };
            }
        }


        [HttpPost]
        public ActionResult Select(Customer_Approval_ViewModel approval_ViewModel)
        {
            var customerID = approval_ViewModel.customer_id;
            var approve_type = approval_ViewModel.appr_type;
            var Slno = approval_ViewModel.sl_no;
            //var modi_status = approval_ViewModel.modi_status;

            Session["Apr_custID"] = customerID;
            Session["Approve_type"] = approve_type;/* approve_type=1 --> Fresh Approval 2-->Modify Approval*/
            Session["Slno"] = Slno;
            //Session["modification"] = modi_status;
            // Redirect to the next page
            return RedirectToAction("Cus_approve", "Customer_approve");

        }


       


    }
}



