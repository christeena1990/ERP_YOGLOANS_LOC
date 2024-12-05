using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{
    public class Smart_LoginController : Controller
    {
        DB dbconnect = new DB();


        //private db dbconnect = new db();



        // GET: Smart_Login
        public ActionResult Login()
        {
            return View();
        }




        [HttpPost]
        public ActionResult CheckCode(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode))
            {
                // Handle empty employee code (if needed)
                return Json(new { success = false, Message = "Employee code is empty." });
            }

            try
            {
                // Call your server-side logic to execute the stored procedure and process the result

                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1;
                pr[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                pr[1].Value = employeeCode; // Use the passed employeeCode parameter

                DataSet ds = dbconnect.ExecuteDataset("[dbo].[login_pcdr]", pr);

                if (ds != null && ds.Tables.Count >= 0)
                {
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];
                    DataTable dt3 = ds.Tables[2];

                    // Check if dt1 is not null and has rows
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {

                        string encryptedPassword = dt1.Rows[0]["c1"].ToString();
                        // You can use encryptedPassword as needed

                        //return Json(new { success = true, EncryptedPassword = encryptedPassword, Message = "Login successful." });

                        if (dt2 != null && dt2.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dt2.Rows[0]["c2"]) == 1)
                            {
                                if (dt3 != null && dt3.Rows.Count > 0)
                                {
                                    string dt3Message = dt3.Rows[0]["c3"].ToString();
                                    string[] splitMessage = dt3Message.Split('#');

                                    if (splitMessage.Length == 2 && int.TryParse(splitMessage[0], out int messageType))
                                    {
                                        if (messageType == 1)
                                            //return Json(new { success = false, Message = splitMessage[1] });
                                            return Json(new { success = true, Message = splitMessage[1] });

                                        if (messageType == 5)
                                            return Json(new { success = false, Message = splitMessage[1] });

                                    }
                                    else
                                    {
                                        return Json(new { success = true });
                                    }

                                }
                                // dt2 == 0, show alert  on nov 11 2024
                                else
                                {
                                    return Json(new { success = false, Message = "Not Punching Today...!" });

                                }
                            }




                            else
                            {
                                return Json(new { success = false, Message = "Not Punching Today...!" });

                            }
                        }
                        //on nov 11 2024
                        else
                        {
                            return Json(new { success = false, Message = "Not Punching Today...!" });

                        }


                    }

                    else
                    {
                        // Return a specific JSON response to trigger an alert
                        return Json(new { success = false, Message = "Employee does not exist." });
                    }

                    // Process dt2 and dt3 as needed



                    //// Return success message here if needed
                    return Json(new { success = true, Message = "Login successful." });
                }
                else
                {
                    return Json(new { success = false, Message = "Employee does not exist." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "An error occurred: " + ex.Message });
            }
        }



        [HttpPost]
        public ActionResult Login(Smart_Login_model model)

        {
            try
            {
                // Generate a unique session ID
                string sessionId = Guid.NewGuid().ToString();
                Session["SessionId"] = sessionId;


                if (ModelState.IsValid)
                {
                    // Check if the user is already logged in
                    if (Session["IsLoggedIn"] != null && (bool)Session["IsLoggedIn"] == true)
                    {
                        //return Json(new { success = false, message = "User already logged in", sessionId = Session["SessionId"] });
                        return Json(new { success = false, message = " \"Multiple tabs or windows are not supported. Please use the existing session or close this tab to continue.\"", sessionId = Session["SessionId"] });

                       


                    }

                    Session["roll_id"] = -1;
                    Session["module_id"] = 0;
                    Session["login_user"] = model.EmployeeCode;
                    Session["t"] = "N";



                    // Call LoginCheck and return its result directly
                    //return LoginCheck(model);
                    try
                    {
                        SqlParameter[] pr = new SqlParameter[2];
                        pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                        pr[0].Value = 21;
                        pr[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                        pr[1].Value = model.EmployeeCode;

                        DataTable dt = new DataTable();
                        dt = dbconnect.ExecuteDataset("[dbo].[Re_finance_pcdr]", pr).Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            string loginstatus = dt.Rows[0]["login_status"].ToString();

                            if (string.IsNullOrEmpty(model.EmployPassword))
                            {
                                return Json(new { success = true });
                            }

                            string password = dt.Rows[0]["employ_password"].ToString();
                            string Decript_pass = Decrypt(password);

                            if (Decript_pass.Trim() == model.EmployPassword)
                            {
                                Session["module_id"] = -1;
                                Session["login_user"] = model.EmployeeCode;
                                Session["roll_id"] = dt.Rows[0]["roll_id"].ToString();
                                Session["name"] = dt.Rows[0]["employ_name"].ToString();
                                Session["branch"] = dt.Rows[0]["branch_name"].ToString();
                                Session["branch_id"] = dt.Rows[0]["branch_id"].ToString();
                                // New session variable to track login status
                                Session["IsLoggedIn"] = true; // Set session variable indicating user is logged in
                                Session["SessionId"] = sessionId; // Store the unique session ID
                                Session.Timeout = 15;




                                if (Decript_pass == "yoga123")
                                {
                                    //ClientScript.RegisterStartupScript(GetType(), "password", "new_window();", true);
                                    //return RedirectToAction("Resetpassword", "Reset_Password");
                                    return Json(new { success = true, message = "Resetpassword" });
                                }
                                ////////////////////////////////////////////////from here  added march 28//////////////////////////////////////////////////////////////////////////////////////////
                                else
                                {
                                    SqlParameter[] pr2 = new SqlParameter[5];
                                    pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                                    pr2[0].Value = 5;
                                    pr2[1] = new SqlParameter("@branch", SqlDbType.BigInt);
                                    pr2[1].Value = Session["branch_id"];
                                    pr2[2] = new SqlParameter("@user", SqlDbType.BigInt);
                                    pr2[2].Value = Session["login_user"];
                                    pr2[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                                    pr2[3].Direction = ParameterDirection.Output;
                                    pr2[4] = new SqlParameter("@roll_id", SqlDbType.BigInt);
                                    pr2[4].Value = Session["roll_id"];

                                    dbconnect.ExecuteStoredProcedure("[dbo].[HR_branch_cashier_allowance]", pr2);

                                    string outputMessage = pr2[3].Value != DBNull.Value ? pr2[3].Value.ToString() : "";

                                    ////////////////////////////////////////////////////////////////////////////////////////////////////////

                                    if (outputMessage == "1")
                                    {
                                        return Json(new { success = true, message = "Cashier_assign" });
                                    }
                                    else if (outputMessage == "2")
                                    {
                                        return Json(new { success = true, message = "Cashier Not Assigned" });

                                    }
                                    else if (outputMessage == "3")
                                    {
                                        return Json(new { success = true, message = "Cashier_approve" });

                                    }
                                    else if (outputMessage == "4")
                                    {
                                        return Json(new { success = true, message = "Cashier Request Not Approved" });
                                    }
                                    else
                                    {

                                        ///////////////////////////////////////////////////////////////////////////////////////////////////////



                                        SqlParameter[] pr1 = new SqlParameter[2];
                                        pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                                        pr1[0].Value = 58;
                                        pr1[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                                        pr1[1].Value = Session["login_user"];

                                        // Execute another stored procedure if needed
                                        dbconnect.ExecuteStoredProcedure("[dbo].[Re_finance_pcdr]", pr1);

                                        // Redirect to home.aspx page
                                        return Json(new { success = true, message = "Home", sessionId = sessionId });
                                    }
                                }


                                ////////////////////////////////////till here  ... added march 28////////////////////////////////////////////////////////////////////////////////////////////////////////////

                                //return RedirectToAction("AnotherAction", "AnotherController");
                                //return Json(new { success = true, message = "Home" });



                            }


                            else
                            {
                                return Json(new { success = false, message = "Incorrect password." });
                            }

                        }
                        else
                        {
                            return Json(new { success = false, message = "An error occurred." });

                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            // If ModelState is not valid or an error occurred, return the view with the model
            return View(model);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


       


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






        private string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        byte[] decryptedBytes = ms.ToArray();
                        return Encoding.Unicode.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error decrypting password: " + ex.Message);
            }

        }


        public ActionResult Logout()
        {
            // Clear the session variables
            Session.Clear();
            Session.Abandon();

            // Redirect to the login page
            return RedirectToAction("Login", "Smart_Login");
        }
    }
}

