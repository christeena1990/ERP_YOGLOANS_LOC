using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Data.SqlTypes;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Services.Description;
using Microsoft.Ajax.Utilities;
using Org.BouncyCastle.Asn1.Crmf;
using System.Net;


namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{

    public class AttendanceController : Controller
    {


        DB dbconnect = new DB();


        public ActionResult Mark_Attendance()
        {
            attendance_model model = new attendance_model();
            DataTable dt_time = new DataTable();
            //var time = "";
            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    //connection.Open();
            //    SqlCommand command = new SqlCommand("SELECT SYSDATETIME() AS time1", connection);
            //    SqlDataReader reader = command.ExecuteReader();
            //    {
            //        while (reader.Read())
            //        {
            //            // Retrieve the datetime value directly as a DateTime object
            //            time = reader.GetDateTime(reader.GetOrdinal("time1")).ToString("yyyy-MM-dd       HH:mm:ss");
            //        }
            //    }
            //    //connection.Close();
            //}
            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 3;

            dt_time = dbconnect.ExecuteDataset("[dbo].[cust_search_create]", pr).Tables[0];

            // dt_time = dbconnect.ExecuteQuery("select sysdatetime()").Tables[0];

            DateTime time1 = Convert.ToDateTime(dt_time.Rows[0][0]);
            string formattedTime = time1.ToString("yyyy-MM-dd HH:mm:ss");

            model.server_time = formattedTime;

            string clientIpAddress = GetClientIpAddress(Request);
            Session["h_IPaddress"] = clientIpAddress;
            string partial = clientIpAddress.Substring(0, clientIpAddress.LastIndexOf("."));

            return View("Mark_Attendance", model);
        }

        private string GetClientIpAddress(HttpRequestBase request)
        {
            string ipAddress = string.Empty;

            // First, try to get the IP address from the X-Forwarded-For header
            string xForwardedFor = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                string[] addresses = xForwardedFor.Split(',');
                if (addresses.Length != 0)
                {
                    ipAddress = addresses[0].Trim();
                }
            }

            // If X-Forwarded-For is empty, use REMOTE_ADDR
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = request.UserHostAddress;
            }

            // Exclude loopback addresses
            if (ipAddress == "::1" || ipAddress == "127.0.0.1")
            {
                ipAddress = GetLocalIPAddress();
            }

            return ipAddress;
        }

        private string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }




        [HttpPost]
        public JsonResult employeecode_checking(string employeCode, String time)

        {
            try
            {
                if (employeCode != "")
                {
                    Session["employee_code"] = employeCode;
                    //var server_time = time;
                    Session["server_time"] = time;
                    DateTime dateTimeValue = DateTime.Parse(time);

                    // Extracting Date and Time components
                    DateTime date = dateTimeValue.Date; // Date component
                    TimeSpan time1 = dateTimeValue.TimeOfDay; // Time component

                    SqlParameter[] pr2 = new SqlParameter[2];

                    pr2[0] = new SqlParameter("@UserID", SqlDbType.Int);
                    pr2[0].Value = Session["employee_code"];

                    pr2[1] = new SqlParameter("@time1", SqlDbType.NVarChar, 20);
                    //pr2[1].Value = Session["server_time"];
                    pr2[1].Value = time1.ToString();
                    //pr2[1].Value = DateTime.Today.Add(time1);

                    DataTable dt = new DataTable();
                    dt = dbconnect.ExecuteDataset("[dbo].[HRCheckUserStatus]", pr2).Tables[0];
                    //string decryptedValue = Decrypt(dt.Rows[0][2].ToString());

                    if (dt.Rows.Count > 0)
                    {
                        Session["password"] = Decrypt(dt.Rows[0][2].ToString());
                        Session["punch_branch"] = dt.Rows[0][0].ToString();

                        if (dt.Rows[0][3].ToString() == "2")
                        {
                            return Json(new { success = true, Message = "Attendance Already Marked, Go Home...!!" });
                        }
                        if (dt.Rows[0][3].ToString() == "1")
                        {
                            return Json(new { success = true, Message = "Attendance Already Marked for the current Session...Please Try Later...!!" });
                        }
                        if (dt.Rows[0][6].ToString() != "3")
                        {
                            return Json(new { success = true, Message = "Please update Goldloan LTV!"});
                        }
                        if (Session["password"].ToString() == "yoga123")
                        {
                            return Json(new { success = true, Message = "Please Change your Default Password...!" });
                        }
                        else
                        {
                            return Json(new { success = false, Message = "" });
                        }
                    }
                    else
                    {
                        return Json(new { success = true, Message = "Employee Does Not Exist" });
                    }

                }
                else
                {
                    return Json(new { success = false, Message = "" });
                }


            }


            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Employee Does Not Exist" });
            }
        }


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



        [HttpPost]
        public ActionResult password_checking(string empcode, string password, attendance_model model)
        {
            if (password == Session["password"].ToString())
            {



                try
                {
                    SqlParameter[] pr2 = new SqlParameter[1];

                    pr2[0] = new SqlParameter("@UserID", SqlDbType.Int);
                    pr2[0].Value = empcode;

                    DataTable dt = new DataTable();
                    dt = dbconnect.ExecuteDataset("[dbo].[HRRetrievePassword]", pr2).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        model.Employee_name = dt.Rows[0][0].ToString();
                        model.Employee_branch = dt.Rows[0][1].ToString();
                        model.Employee_shift_time = dt.Rows[0][2].ToString();

                    }
                    else
                    {

                    }



                    return Json(new { success = true, model.Employee_name, model.Employee_branch, model.Employee_shift_time });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Message = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, Message = "Incorrect Password!!" });
                //return RedirectToAction("Mark_Attendance");
            }

        }

        [HttpPost]
        public JsonResult markattandance(string hidden_time)


        {
            try
            {
                SqlParameter[] pr4 = new SqlParameter[2];
                pr4[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr4[0].Value = 59;
                pr4[1] = new SqlParameter("@user_id", SqlDbType.BigInt);
                pr4[1].Value = Session["employee_code"];

                DataTable dt = new DataTable();

                dt = dbconnect.ExecuteDataset("[dbo].[HR_apply_leave_new]", pr4).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        return Json(new { success = true, Message = "Are you sure want to Punch Out?   Punch Out Time : "+ hidden_time });
                    }
                    else
                    {

                        return Json(new { success = true, Message = "Are you sure want to Punch In?   Punch In Time  : " + hidden_time });
                        // return RedirectToAction("markattandance2");
                    }
                }
                else
                {
                    return Json(new { success = true, Message = "table is null" });
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }


            // return RedirectToAction("markattandance2");
            return Json(new { success = false, Massage = true });
            // return View();
        }


        [HttpPost]
        public ActionResult markattandance2(string hidden_time)

        {
            try
            {
                DataSet ds = new DataSet();
                SqlParameter[] pr10 = new SqlParameter[3];
                pr10[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr10[0].Value = 2;
                pr10[1] = new SqlParameter("@user_id", SqlDbType.BigInt);
                pr10[1].Value = Session["employee_code"];
                pr10[2] = new SqlParameter("@Message", SqlDbType.VarChar, 100);
                pr10[2].Direction = ParameterDirection.Output;
                dbconnect.ExecuteDataset("[dbo].[gl_control_balance_login_check]", pr10);
                string msg_1 = pr10[2].Value.ToString();

                DataTable dt_0 = new DataTable();

                SqlParameter[] pr_r = new SqlParameter[3];

                pr_r[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr_r[0].Value = 1;

                pr_r[1] = new SqlParameter("@user_id", SqlDbType.BigInt);
                pr_r[1].Value = Session["employee_code"];
                pr_r[2] = new SqlParameter("@Message", SqlDbType.VarChar, 100);
                pr_r[2].Direction = ParameterDirection.Output;

                dbconnect.ExecuteDataset("[dbo].[gl_control_balance_login_check]", pr_r);

                string msg = pr_r[2].Value.ToString();
                string[] parts = msg.Split('&');
                string part1 = parts[0].ToString();
                string part2 = parts[1].ToString();
                /////////////////////////////////////////////////////////////////////////////////////////////
                SqlParameter[] pr1 = new SqlParameter[1];

                pr1[0] = new SqlParameter("@userid", SqlDbType.BigInt);
                pr1[0].Value = Session["employee_code"];

                DataTable dt = new DataTable();

                dt = dbconnect.ExecuteDataset("[dbo].[HRRetrieveDetails]", pr1).Tables[0];
                ///////////////////////////////////////////////////////////////////////////////////////////////

                //string messageresult = string.Empty;

                SqlParameter[] pr11 = new SqlParameter[9];

                pr11[0] = new SqlParameter("@EmployeeCode", SqlDbType.Int);
                pr11[0].Value = Session["employee_code"];

                pr11[1] = new SqlParameter("@EmployeeBranch", SqlDbType.Int);
                pr11[1].Value = Convert.ToInt64(dt.Rows[0][6]);

                pr11[2] = new SqlParameter("@EmployeeShiftID", SqlDbType.Int);
                pr11[2].Value = Convert.ToInt64(dt.Rows[0][11]);

                pr11[3] = new SqlParameter("@EmployeeTourBranch", SqlDbType.Int);
                pr11[3].Value = Convert.ToInt64(dt.Rows[0][10]);

                pr11[4] = new SqlParameter("@PunchBranch", SqlDbType.Int);
                pr11[4].Value = Session["punch_branch"];

                string punch_datetime = DateTime.Now.Date.ToString();
                string[] dateTimeParts = punch_datetime.Split(' ');

                // Access the individual parts of the date and time
                string PunchDate = dateTimeParts[0];


                pr11[5] = new SqlParameter("@PunchDate", SqlDbType.Date);
                pr11[5].Value = PunchDate;

                pr11[6] = new SqlParameter("@PunchTime", SqlDbType.NVarChar, 30);
                pr11[6].Value = hidden_time;

                pr11[7] = new SqlParameter("@message", SqlDbType.NVarChar, 500);
                pr11[7].Direction = ParameterDirection.Output;

                pr11[8] = new SqlParameter("@IP_add", SqlDbType.NVarChar, 50);
                pr11[8].Value = Session["h_IPaddress"];

                dbconnect.ExecuteStoredProcedure("[dbo].[MarkAttendance]", pr11);

                string msg1 = pr11[7].Value.ToString();

                return Json(new { success = true, Message1 = msg1 });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message1 = ex.Message });
            }

        }



    }
}