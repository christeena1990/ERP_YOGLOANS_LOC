using ERP_YOGLOANS_LOCAL.Models;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class OtpController : BaseController
    {
        DB dbconnect = new DB();
        // GET: Otp
        [HttpPost]
        public JsonResult sendOTP(string mobNum)
        {


            SqlParameter[] pr1 = new SqlParameter[3];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
            //pr1[0].Value = 12;
            pr1[0].Value = 12;
            pr1[1] = new SqlParameter("@mbno", SqlDbType.VarChar, 50);
            pr1[1].Value = mobNum;
            pr1[2] = new SqlParameter("@out_msg", SqlDbType.VarChar, 200);
            pr1[2].Direction = ParameterDirection.Output;

            dbconnect.ExecuteStoredProcedure("[dbo].[cust_search_create]", pr1);
            var msg1 = pr1[2].Value.ToString();
            //if (msg1 != "1")
            if (msg1 == "0")
            {

                SqlParameter[] pr = new SqlParameter[4];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 7;
                pr[1] = new SqlParameter("@mbno", SqlDbType.VarChar, 50);
                pr[1].Value = mobNum;
                pr[2] = new SqlParameter("@user_id", SqlDbType.VarChar, 50);
                pr[2].Value = Session["login_user"].ToString();
                pr[3] = new SqlParameter("@out_msg", SqlDbType.VarChar, 200);
                pr[3].Direction = ParameterDirection.Output;

                //dbconnect.ExecuteStoredProcedure("[dbo].[sp_customer_search_create]", pr);
                dbconnect.ExecuteStoredProcedure("[dbo].[cust_search_create]", pr);
                string msg = pr[3].Value.ToString();

                string[] StoredOtp = msg.Split('&');
                Session["OTP"] = StoredOtp[2];

                //Otpresponse otpresponse = new Otpresponse();
                //otpresponse.Message = msg;
                return Json(new { success = true, message = "otp" });
            }

            else
                //return Json(new { success = false, message = "exists" });

            return Json(new { success = false, message = msg1 });
            //@out_msg
        }



        [HttpPost]
        public JsonResult VerifyOtp(string enteredOtp)
        {           

            if (Session["module_id"].ToString() == "28")
            {
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
                pr[0].Value = 5;
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 20);
                pr[1].Value = Session["application_id"];
                pr[2] = new SqlParameter("@otp", SqlDbType.BigInt);
                pr[2].Value = enteredOtp;

                dbconnect.Open(); 
                DataTable dt= dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
                dbconnect.Close();

                if (dt.Rows.Count > 0)
                {
                    var message = "";
                    if (dt.Rows[0]["otpstatus"].ToString() == "0")
                    {
                        message = dt.Rows[0]["otpmsg"].ToString();
                        Session["otpVerified"] = "0";
                        // Invalid OTP
                        return Json(new { success = false, message = message });
                    }
                    else
                    {
                        message = dt.Rows[0]["otpmsg"].ToString();
                        Session["otpVerified"] = "1";
                        // OTP is valid
                        return Json(new { success = true, message = message });
                    }
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            else
            {
                string storedOtp = Session["OTP"].ToString();
                if (enteredOtp == storedOtp)
                {
                    Session["otpVerified"] = "1";
                    // OTP is valid
                    return Json(new { success = true, message = "OTP verification successful." });
                    //return Json(new { success = true });
                }
                else
                {
                    Session["otpVerified"] = "0";
                    // Invalid OTP
                    return Json(new { success = false, message = "Invalid OTP. Please try again." });
                    //return Json(new { success = false });
                }

            }
            
            
                
        }



        [HttpPost]
        public JsonResult resendOTP(string mobNum, string appli_id = "")
        {
            if (Session["module_id"].ToString() == "25")
            {
                SqlParameter[] pr = new SqlParameter[4];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 14;
                pr[1] = new SqlParameter("@mbno", SqlDbType.VarChar, 50);
                pr[1].Value = mobNum;
                pr[2] = new SqlParameter("@user_id", SqlDbType.VarChar, 50);
                pr[2].Value = Session["login_user"].ToString();
                pr[3] = new SqlParameter("@out_msg", SqlDbType.VarChar, 200);
                pr[3].Direction = ParameterDirection.Output;

                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[cust_search_create]", pr);
                dbconnect.Close();
                string msg = pr[3].Value.ToString();

                string[] StoredOtp = msg.Split('&');
                Session["OTP"] = StoredOtp[2];

            }
            if (Session["module_id"].ToString() == "28")
            {
                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
                pr[0].Value = 4;
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 50);
                pr[1].Value = appli_id;
                pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200);
                pr[2].Direction = ParameterDirection.Output;

                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
                dbconnect.Close();
                string msg = pr[2].Value.ToString();

            }

            return Json(new { success = true, message = "otp" });

        }






    }
}