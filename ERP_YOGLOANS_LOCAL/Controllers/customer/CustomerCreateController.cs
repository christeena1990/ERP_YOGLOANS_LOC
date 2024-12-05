using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using iTextSharp.xmp.impl;



namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class CustomerCreateController : BaseController
    {
        // GET: CustomerCreate
        DB dbconnect = new DB();

        public async Task<ActionResult> CustomerCreate()
        {
            Session["cust_slno"] = null;
            //Session["cust_slno"] = null;
            //Function call for fill guardian type
            DataTable addressproof = Filladdressproof();
            ViewData["AddressproofList"] = addressproof;

            DataTable idproof = Fillidproof();
            ViewData["idproofList"] = idproof;




            //Fillgrid Incomplete Customer
            var incompleteCustomerList = await GetIncompleteCustomer();
            ViewBag.IncompleteCustomerList = incompleteCustomerList;


            var modifyCustomerList = await GetModifyReturnCustomer();
            ViewBag.ModifyCustomerList = modifyCustomerList;



            cust_details model = new cust_details();
            Session["TempCID"] = model.ID;
            Session["MobileNo"] = model.MobileNo;
            Session["CustomerName"] = model.Name;
            Session["cust_status"] = model.Status;
            //Session["branch_id"] = "1";




            return View();
        }


        [HttpPost]
        public ActionResult SetMobileNumber(string mobileNumber)
        {
            try
            {
                // Store the mobile number in Session
                Session["MobileNo"] = mobileNumber;

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return Json(new { success = false, error = ex.Message });
            }
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
        [HttpGet]

        //Method for Gridfill Incomplete Customer
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

        //[HttpPost]
        //public async Task<ActionResult> Search_customer(CustomerCreate_model modelObj)
        //{
        //    int hasvalue = 0;
        //    if (modelObj.mobile_no == null && modelObj.customer_id == null && modelObj.txt_add_proof_no == null && modelObj.txt_id_proof_no == null && modelObj.fname == null && modelObj.lname == null)
        //    {
        //        hasvalue = 1;

        //        alert("Please enter values for at least one field.");


        //    }
        //    else

        //    {

        //        //string apiUrl = "http://10.3.0.223:8081/api/GetSearchedCustDtl";

        //        string apiUrl = "http://10.10.0.47:83/api/GetSearchedCustDtl";

        //        searchRequest_model searchRequest = new searchRequest_model
        //        {

        //            MobileNo = modelObj.mobile_no,
        //            Eid = modelObj.txt_add_proof_no,
        //            Id = modelObj.txt_id_proof_no,
        //            CustId = modelObj.customer_id,
        //            f_name = modelObj.fname,
        //            s_name = modelObj.lname,
        //            ddl_id = modelObj.ddl_idproof,
        //            ddl_add = modelObj.ddl_address,
        //            name = modelObj.fname + " " + modelObj.lname

        //        };
        //        Session["ddl_add"] = searchRequest.ddl_add;
        //        Session["Eid"] = searchRequest.Eid;


        //        Session["ddl_id"] = searchRequest.ddl_id;
        //        Session["Id"] = searchRequest.Id;


        //        Session["f_name"] = searchRequest.f_name;
        //        Session["s_name"] = searchRequest.s_name;


        //        try
        //        {
        //            using (HttpClient httpClient = new HttpClient())
        //            {
        //                // Serialize the searchRequest to JSON
        //                var jsonRequestData = JsonConvert.SerializeObject(searchRequest);

        //                // Create the HttpContent for the POST request
        //                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

        //                // Send the POST request
        //                var response = await httpClient.PostAsync(apiUrl, content);
        //                response.EnsureSuccessStatusCode();

        //                // Read the response content
        //                var responseData = await response.Content.ReadAsStringAsync();

        //                // Deserialize the response content
        //                var searchResult = JsonConvert.DeserializeObject<searchPartial_model>(responseData);

        //                // Set ViewBag for the column headers
        //                ViewBag.Col_Head = searchResult.Col_Head;
        //                ViewBag.Col_Data = searchResult.Col_Data;

        //                if (ViewBag.Col_Data == null || ViewBag.Col_Data.Count == 0)
        //                {

        //                    //string apiUrl = "http://10.3.0.223:8081/api/GetSearchedCustDtl_incomplete";

        //                    string apiUrl1 = "http://10.10.0.47:83/api/GetSearchedCustDtl_incomplete";

        //                    searchRequest_model searchRequest1 = new searchRequest_model
        //                    {

        //                        MobileNo = modelObj.mobile_no,
        //                        Eid = modelObj.txt_add_proof_no,
        //                        Id = modelObj.txt_id_proof_no,


        //                    };


        //                    using (HttpClient httpClient1 = new HttpClient())
        //                    {
        //                        // Serialize the searchRequest to JSON
        //                        var jsonRequestData1 = JsonConvert.SerializeObject(searchRequest1);

        //                        // Create the HttpContent for the POST request
        //                        var content1 = new StringContent(jsonRequestData1, Encoding.UTF8, "application/json");

        //                        // Send the POST request
        //                        var response1 = await httpClient1.PostAsync(apiUrl1, content1);
        //                        response1.EnsureSuccessStatusCode();

        //                        // Read the response content
        //                        var responseData1 = await response1.Content.ReadAsStringAsync();

        //                        // Deserialize the response content
        //                        var searchResult1 = JsonConvert.DeserializeObject<searchPartial_model1>(responseData1);

        //                        // System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(searchResult1));

        //                        // Set ViewBag for the column headers
        //                        ViewBag.Col_Head1 = searchResult1.Col_Head1;
        //                        ViewBag.Col_Data1 = searchResult1.Col_Data1;

        //                        // System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(ViewBag.Col_Data1));


        //                        return PartialView("_search_incomplete_partial1");
        //                    }
        //                }

        //                DataTable addressproof = Filladdressproof();
        //                ViewData["AddressproofList"] = addressproof;

        //                DataTable idproof = Fillidproof();
        //                ViewData["idproofList"] = idproof;

        //                Session["CustomerID_M"] = searchResult.Col_Data.Select(item => item.CustomerID).FirstOrDefault();




        //                return PartialView("_search_table_partial");
        //                //return View("CustomerCreate");
        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // Handle HTTP request exceptions
        //            ViewBag.ErrorMessage = "Error in the HTTP request: " + ex.Message;
        //        }
        //        catch (JsonException ex)
        //        {
        //            // Handle JSON serialization/deserialization exceptions
        //            ViewBag.ErrorMessage = "Error in JSON serialization/deserialization: " + ex.Message;
        //        }
        //        catch (Exception ex)
        //        {
        //            // Handle other exceptions
        //            ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
        //        }

        //    }

        //    // If an error occurred, return an error partial view
        //    return View("_error_partial");
        //    // View("");
        //}

        [HttpPost]
        public async Task<ActionResult> Search_customer(CustomerCreate_model modelObj)
        {
            if (modelObj.mobile_no == null && modelObj.customer_id == null && modelObj.txt_add_proof_no == null && modelObj.txt_id_proof_no == null && modelObj.fname == null && modelObj.lname == null)
            {
                alert("Please enter values for at least one field.");
                return View("_error_partial");
            }

            //string apiUrl = "http://10.3.0.223:8081/api/GetSearchedCustDtl";

            string apiUrl1 = "http://10.10.0.47:83/api/GetSearchedCustDtl";
 


            //string apiUrl = "http://10.3.0.223:8081/api/GetSearchedCustDtl_incomplete";

            string apiUrl2 = "http://10.10.0.47:83/api/GetSearchedCustDtl_incomplete";

            searchRequest_model searchRequest = new searchRequest_model
            {
                MobileNo = modelObj.mobile_no,
                Eid = modelObj.txt_add_proof_no,
                Id = modelObj.txt_id_proof_no,
                CustId = modelObj.customer_id,
                f_name = modelObj.fname,
                s_name = modelObj.lname,
                ddl_id = modelObj.ddl_idproof,
                ddl_add = modelObj.ddl_address,
                name = modelObj.fname + " " + modelObj.lname,
               branchid =Convert.ToInt32 (Session["branch_id"])
            
            };

            Session["ddl_add"] = searchRequest.ddl_add;
            Session["Eid"] = searchRequest.Eid;
            Session["ddl_id"] = searchRequest.ddl_id;
            Session["Id"] = searchRequest.Id;
            Session["f_name"] = searchRequest.f_name;
            Session["s_name"] = searchRequest.s_name;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var jsonRequestData = JsonConvert.SerializeObject(searchRequest);
                    var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                    var response1 = await httpClient.PostAsync(apiUrl1, content);
                    response1.EnsureSuccessStatusCode();
                    var responseData1 = await response1.Content.ReadAsStringAsync();
                    var searchResult1 = JsonConvert.DeserializeObject<searchPartial_model>(responseData1);

                    var jsonRequestData2 = JsonConvert.SerializeObject(new { searchRequest.MobileNo, searchRequest.Eid, searchRequest.Id ,searchRequest.branchid});
                    var content2 = new StringContent(jsonRequestData2, Encoding.UTF8, "application/json");

                    var response2 = await httpClient.PostAsync(apiUrl2, content2);
                    response2.EnsureSuccessStatusCode();
                    var responseData2 = await response2.Content.ReadAsStringAsync();
                    var searchResult2 = JsonConvert.DeserializeObject<searchPartial_model1>(responseData2);

                    ViewBag.Col_Head = searchResult1.Col_Head;
                    ViewBag.Col_Data = searchResult1.Col_Data;
                    ViewBag.Col_Head1 = searchResult2.Col_Head1;
                    ViewBag.Col_Data1 = searchResult2.Col_Data1;

                    return PartialView("_combined_partial_view");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View("_error_partial");
            }
        }

        private void alert(string v)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<ActionResult> send_otp()
        {
            try
            {
                // Check if the mobile number is not null
                if (Session["MobileNo"] != null)
                {

                    //string apiUrl = "http://10.3.0.223:8081/api/getOtp";

                    string apiUrl = "http://10.10.0.47:83/api/getOtp";

                    otp_model otpRequest = new otp_model
                    {
                        MobNo = Session["MobileNo"].ToString(),
                        UserId = Session["login_user"].ToString()

                    };


                    using (HttpClient httpClient = new HttpClient())
                    {
                        // Serialize the otpRequest to JSON
                        var jsonRequestData = JsonConvert.SerializeObject(otpRequest);

                        // Create the HttpContent for the POST request
                        var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                        // Send the POST request
                        var response = await httpClient.PostAsync(apiUrl, content);

                        // Read the response content
                        var responseData = await response.Content.ReadAsStringAsync();

                        // Deserialize the response content to extract the "Message" value
                        var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseData);
                        var messageValue = jsonResponse?["Message"]?.ToString();

                        // Set TempData for later use in VerifyOtp
                        TempData["StoredOtp"] = messageValue;

                        return Json(new { otp_message = messageValue });
                    }
                }
                else
                {
                    // Mobile number is null, return an error message
                    return Json(new { error = "Mobile number is null. Please provide a valid mobile number." });
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                ViewBag.ErrorMessage = "Error in the HTTP request: " + ex.Message;
            }
            catch (JsonException ex)
            {
                // Handle JSON serialization/deserialization exceptions
                ViewBag.ErrorMessage = "Error in JSON serialization/deserialization: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            }

            // If an error occurred, return an error partial view
            return View();
        }


        [HttpPost]
        public ActionResult VerifyOtp(string enteredOtp)
        {
            // Assume you have a stored OTP value, for example, in TempData
            //var storedOtp = TempData["StoredOtp"]?.ToString();

            var storedOtp = TempData["StoredOtp"]?.ToString();
            string[] StoredOtp = storedOtp.Split('&');

            if (enteredOtp == StoredOtp[2])
            {
                // OTP is valid
                return Json(new { success = true });
            }
            else
            {
                // Invalid OTP
                return Json(new { success = false });
            }
        }


        public ActionResult CustomerCreate_form1()
        {

            string mobileNo = Session["MobileNo"] as string;
            string customerID = Session["CustomerID"] as string;
            string customerName = Session["CustomerName"] as string;

            DataTable addressproof = Filladdressproof();
            ViewData["AddressproofList"] = addressproof;

            DataTable idproof = Fillidproof();
            ViewData["idproofList"] = idproof;

            // Return the view
            return View();
        }


        public ActionResult resend_otp(searchRequest_model model)
        {


            DataTable addressproof = Filladdressproof();
            ViewData["AddressproofList"] = addressproof;

            DataTable idproof = Fillidproof();
            ViewData["idproofList"] = idproof;

            // Assuming searchRequest.ddl_add is a string containing both no and value separated by a hyphen

            // Return the view

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> ResendOtp(CustomerCreate_model re_otp_model)
        {
            try
            {
                // Check if TempData["MobileNumber"] is not null
                if (Session["MobileNo"] != null)
                {

                   // string apiUrl = "http://10.3.0.223:8081/api/re_getOtp";

                    string apiUrl = "http://10.10.0.47:83/api/re_getOtp"; 

                  
                    re_otp_model otpRequest = new re_otp_model
                    {
                        MobNo = Session["MobileNo"].ToString(),
                        UserId = Session["login_user"].ToString()
                    };

                    using (HttpClient httpClient = new HttpClient())
                    {
                        // Serialize the otpRequest to JSON
                        var jsonRequestData = JsonConvert.SerializeObject(otpRequest);

                        // Create the HttpContent for the POST request
                        var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                        // Send the POST request
                        var response = await httpClient.PostAsync(apiUrl, content);
                        //response.EnsureSuccessStatusCode();

                        // Read the response content
                        var responseData = await response.Content.ReadAsStringAsync();

                        // Deserialize the response content to extract the "Message" value
                        var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseData);
                        var messageValue = jsonResponse?["Message"]?.ToString();

                        // Set TempData for later use in VerifyOtp
                        TempData["StoredOtp"] = messageValue;



                        DataTable addressproof = Filladdressproof();
                        ViewData["AddressproofList"] = addressproof;

                        DataTable idproof = Fillidproof();
                        ViewData["idproofList"] = idproof;


                        // Return the partial view with the search result
                        //return Json(new { otp_message = messageValue });
                        return View("resend_otp");
                    }
                }
                else
                {
                    // Handle the case where TempData["MobileNumber"] is null
                    return Json(new { error = "MobileNumber not found in Session." });
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult> CreateTempCustomer(CreateTempCustomer_model modelObj)
        {
            Session["cust_status"] = null;
            //Session["branch_id"] = "1";

            //string apiUrl = "http://10.3.0.223:8081/api/CreateTempCustomer";

            string apiUrl = "http://10.10.0.47:83/api/CreateTempCustomer";

            CreateTempCustomer_model createRequest = new CreateTempCustomer_model
            {
                mobile_no = modelObj.mobile_no,
                f_name = modelObj.f_name,
                s_name = modelObj.s_name,
                l_name = modelObj.l_name?.Trim() ?? "",
                branch = Session["branch_id"].ToString(),
                enter_by = Session["login_user"].ToString()

            };
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Serialize the searchRequest to JSON
                    var jsonRequestData = JsonConvert.SerializeObject(createRequest);

                    // Create the HttpContent for the POST request
                    var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                    // Send the POST request
                    var response = await httpClient.PostAsync(apiUrl, content);
                    response.EnsureSuccessStatusCode();

                    // Read the response content
                    var responseData = await response.Content.ReadAsStringAsync();

                    // Deserialize the response content
                    var CreateResult = JsonConvert.DeserializeObject<searchPartial_model>(responseData);
                    // splitting the response

                    var startIndex = responseData.IndexOf('"') + 1; // Find the first double quote
                    var endIndex = responseData.LastIndexOf('"'); // Find the last double quote


                    var messagePart = responseData.Substring(startIndex, endIndex - startIndex);


                    string[] msg = responseData.Split('@');
                    string disp_msg = msg[0].ToString();
                    string temp_cid = msg[1].ToString();
                    string fullnameWithExtra = msg[2].Trim();


                    string[] fullnameParts = fullnameWithExtra.Split('"');
                    string fullname = fullnameParts[0].Trim();

                    Session["TempCID"] = temp_cid;

                    DataTable addressproof = Filladdressproof();
                    ViewData["AddressproofList"] = addressproof;

                    DataTable idproof = Fillidproof();
                    ViewData["idproofList"] = idproof;



                    Session["MobileNo"] = modelObj.mobile_no;
                    Session["CustomerName"] = fullname;
                    Session["cust_status"] = "0";


                    return RedirectToAction("Cust_Registration", "Customer_Registration");


                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                ViewBag.ErrorMessage = "Error in the HTTP request: " + ex.Message;
            }
            catch (JsonException ex)
            {
                // Handle JSON serialization/deserialization exceptions
                ViewBag.ErrorMessage = "Error in JSON serialization/deserialization: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
            }

            // If an error occurred, return an error partial view
            return View("_error_partial");
            // View("");



        }


        public ActionResult test_view()
        {
            return View();
        }


        public DataTable Filladdressproof()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_dropdown]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        public DataTable Fillidproof()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;
            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_dropdown]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }


        public ActionResult incomplete_continue(int id)
        {
            // CustomerCreate_model model = new CustomerCreate_model();

            Session["TempCID"] = id;


            DB dbconnect = new DB();


            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 11;

            pr[1] = new SqlParameter("@custid_temp", SqlDbType.BigInt);
            pr[1].Value = Session["TempCID"];

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_search_create]", pr).Tables[0];
            dbconnect.Close();



            if (dt.Rows.Count > 0)
            {

                Session["TempCID"] = dt.Rows[0][0].ToString();
                Session["MobileNo"] = dt.Rows[0][7].ToString();
                Session["CustomerName"] = dt.Rows[0][1].ToString();
                Session["cust_status"] = dt.Rows[0][11].ToString();

            }

            return RedirectToAction("Cust_Registration", "Customer_Registration");

        }

        public ActionResult modify_return_continue(int id, int sl_no)
        {

            Session["CustomerID_M"] = id;
            Session["cust_slno"] = sl_no;

            Session["Hidden_kyc_edit"] = null;
            Session["hid_kyc_edit_status"] = null;

            return Json(new { success = true });

        }

        public ActionResult search_view(int id)
        {

            Session["CustomerID_M"] = id;



            Session["cust_slno"] = "";

            return Json(new { success = true });

        }
    }
}