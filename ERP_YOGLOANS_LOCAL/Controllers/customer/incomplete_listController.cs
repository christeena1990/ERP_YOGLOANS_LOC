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
    public class incomplete_listController : BaseController
    {
       
       
        public async Task<ActionResult> incomplete_list_view()
        {
            var incompleteCustomerList = await GetIncompleteCustomer();
            ViewBag.IncompleteCustomerList = incompleteCustomerList;

            return View();
        }
        [HttpGet]

        //Method for Gridfill Incomplete Customer
        private async Task<List<cust_details>> GetIncompleteCustomer()
        {
            var Id = Session["branch_id"];

            // string apiUrl = "http://10.3.0.223:8081/api/GetCustDtl?Id=" + Id;

            string apiUrl = "http://10.10.0.47:83/api/GetCustDtl?Id=" + Id;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(data);

                ViewBag.Columns1 = ((JArray)result.Columns).ToObject<List<string>>();
                var dataList = ((JArray)result.Data).ToObject<List<Dictionary<string, object>>>();

                // Map the dictionary data to your model
                var incompleteCustomerList = new List<cust_details>();
                foreach (var item in dataList)
                {
                    var customer = new cust_details();
                    foreach (var keyValuePair in item)
                    {
                        var property = typeof(cust_details).GetProperty(keyValuePair.Key);
                        if (property != null)
                        {
                            // Convert the property value to the correct type
                            var convertedValue = Convert.ChangeType(keyValuePair.Value, property.PropertyType);
                            property.SetValue(customer, convertedValue);

                        }

                    }

                    incompleteCustomerList.Add(customer);
                }


                return incompleteCustomerList;
            }
        }
    }
}