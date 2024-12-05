using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Login
{
    public class Forgot_passwordController : Controller
    {
        DB dbconnect = new DB();

        //public ActionResult forgot_password()
        //{
        //    return View("forgot_password");
        //}

        // GET: Forgot_password
        public ActionResult forgot_password()
        {
            try
            {
                // Assuming dbconnect is your database connection class
                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 1;
                dt = dbconnect.ExecuteDataset("[dbo].[HR_forget_password]", pr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Session["message"] = dt.Rows[0][0].ToString();
                    Session["h_type"] = dt.Rows[0][1].ToString();


                }

                return View("forgot_password");
            }

            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                // Handle the exception as needed, for example:
                return View("Error");
            }



        }




        [HttpPost]
        public ActionResult forgot_password(string empcode, string hint)
        {
            try
            {

                string newPass = Encrypt("yoga123");
                SqlParameter[] pr = new SqlParameter[6];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 2;
                pr[1] = new SqlParameter("@emp_id", SqlDbType.BigInt);
                pr[1].Value = empcode;
                pr[2] = new SqlParameter("@hint", SqlDbType.VarChar, 100);
                pr[2].Value = hint;
                pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                pr[3].Direction = ParameterDirection.Output;
                pr[4] = new SqlParameter("@type", SqlDbType.BigInt);
                pr[4].Value = Session["h_type"]; // Assuming h_type is accessible in this context
                pr[5] = new SqlParameter("@pass", SqlDbType.NChar, 200);
                pr[5].Value = newPass;

                dbconnect.ExecuteStoredProcedure("[dbo].[HR_forget_password]", pr);
                string msg = pr[3].Value.ToString();



                if (msg == "1")
                {
                    return Json(new { success = true, Message = "second_div" });
                }
                else
                {
                    return Json(new { success = false, Message = msg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });
            }
        }




        [HttpPost]
        public ActionResult Reset_Employ_Password(forgot_Password_Model model)
        {
            //if (ModelState.IsValid)
            if (model != null)
            {
                // Valid model, proceed with password reset logic
                try
                {


                    string newPass = Encrypt(model.newpswd);
                    SqlParameter[] pr1 = new SqlParameter[6];
                    pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr1[0].Value = 3;
                    pr1[1] = new SqlParameter("@emp_id", SqlDbType.BigInt);
                    pr1[1].Value = model.empcode;
                    pr1[2] = new SqlParameter("@hint", SqlDbType.VarChar, 100);
                    pr1[2].Value = model.hint;
                    pr1[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                    pr1[3].Direction = ParameterDirection.Output;
                    pr1[4] = new SqlParameter("@type", SqlDbType.BigInt);
                    pr1[4].Value = Session["h_type"];// Assuming h_type is accessible in this context
                    pr1[5] = new SqlParameter("@pass", SqlDbType.NChar, 200);
                    pr1[5].Value = newPass;
                    dbconnect.ExecuteStoredProcedure("[dbo].[HR_forget_password]", pr1);
                    string msg = pr1[3].Value.ToString();

                    return Json(new { success = true, message = msg });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                // Invalid model, return a JSON response with an error message
                return Json(new { success = false, message = "Invalid model" });
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




        private string Encrypt(string clearText)
        {
            try
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return clearText;
        }

    }
}