using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class modify_returnController : BaseController
    {

        public async Task<ActionResult> modify_return_view()
        {
            var modifyCustomerList = await GetModifyReturnCustomer();
            ViewBag.ModifyCustomerList = modifyCustomerList;


            return View();
        }

        [HttpGet]
        private async Task<List<modify_details>> GetModifyReturnCustomer()
        {
            var Id = Session["branch_id"];
            //  string apiUrl = "http://10.3.0.223:8081/api/getModified_Return?branch_id=" + Id;

            string apiUrl = "http://10.10.0.47:83/api/getModified_Return?branch_id=" + Id;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(data);

                ViewBag.Columns = ((JArray)result.Columns).ToObject<List<string>>();
                var dataList = ((JArray)result.Data).ToObject<List<Dictionary<string, object>>>();

                // Map the dictionary data to your model
                var modifyCustomerList = new List<modify_details>();
                foreach (var item in dataList)
                {
                    var customer = new modify_details();
                    foreach (var keyValuePair in item)
                    {
                        var property = typeof(modify_details).GetProperty(keyValuePair.Key);
                        if (property != null)
                        {
                            // Convert the property value to the correct type
                            var convertedValue = Convert.ChangeType(keyValuePair.Value, property.PropertyType);
                            property.SetValue(customer, convertedValue);

                        }

                    }

                    modifyCustomerList.Add(customer);
                }



                return modifyCustomerList;
            }
        }
        public ActionResult modify_return_continue(int id, int sl_no)
        {

            Session["CustomerID_M"] = id;
            Session["cust_slno"] = sl_no;

            Session["Hidden_kyc_edit"] = null;
            Session["hid_kyc_edit_status"] = null;

            return Json(new { success = true });

        }
    }
}