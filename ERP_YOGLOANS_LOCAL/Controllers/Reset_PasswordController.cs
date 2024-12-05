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

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class Reset_PasswordController : BaseController
    {
        DB dbconnect = new DB();
        //db_main dbconnect_main = new db_main();
        string pass1 = null;

        public ActionResult Resetpassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string currentPassword, string newPassword, string confirmPassword, Reset_pswd_model Model)
        {

            try
            {

                /////////////////////////////////////////   Fetch current password from database   //////////////////////////////////////////////////////

                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 3;

                pr[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                pr[1].Value = Session["login_user"].ToString();

                DataTable dt = dbconnect.ExecuteDataset("[dbo].[login_pcdr]", pr).Tables[0];
                string pass = dt.Rows[0][0].ToString();
                //pass0 stores the current password of the employee
                string pass0 = Decrypt(pass);

                /////////////////////////////////////////  Fetch last password from history table  //////////////////////////////////////

                SqlParameter[] pr1 = new SqlParameter[2];
                pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr1[0].Value = 6;

                pr1[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                pr1[1].Value = Session["login_user"].ToString();

                DataTable dt1 = dbconnect.ExecuteDataset("[dbo].[login_pcdr]", pr1).Tables[0];
                string h_value = dt1.Rows[0][0].ToString();
                //pass1 stores the previous password of the employee from the history table .
                string pass1 = Decrypt(h_value);

                if (pass0 != currentPassword)
                {
                    return Json(new { success = false, Message = "The entered Current Password is not correct..!" });
                }
                else if (newPassword == currentPassword)
                {
                    return Json(new { success = false, Message = "You cannot enter the last two passwords as your new one..!" });
                }
                else if (pass1 == newPassword)
                {
                    return Json(new { success = false, Message = "You cannot enter the last two passwords as your new one..!" });
                }

                else if (newPassword != confirmPassword)
                {
                    return Json(new { success = false, Message = "Please enter the new password and confirm password as same..!" });
                }
                
               
                else
                {

                    //////////////////////  when the pswd from the db and the current pswd  are same  /////////////////////////


                    if (pass0 == currentPassword)
                    {
                        String encripted_pass = Encrypt(confirmPassword);

                        SqlParameter[] pr2 = new SqlParameter[3];
                        pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
                        pr2[0].Value = 4;

                        pr2[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                        pr2[1].Value = Session["login_user"].ToString();

                        pr2[2] = new SqlParameter("@employ_password", SqlDbType.VarChar, 100);
                        pr2[2].Value = encripted_pass;

                        dbconnect.ExecuteStoredProcedure("[dbo].[login_pcdr]", pr2);


                        //DataTable dt2 = dbconnect.ExecuteDataset("[dbo].[login_pcdr]", pr2).Tables[0];

                        SqlParameter[] pr3 = new SqlParameter[3];
                        pr3[0] = new SqlParameter("@query_id", SqlDbType.Int);
                        pr3[0].Value = 5;

                        pr3[1] = new SqlParameter("@employ_code", SqlDbType.VarChar, 100);
                        pr3[1].Value = Session["login_user"].ToString();

                        pr3[2] = new SqlParameter("@employ_password", SqlDbType.VarChar, 100);
                        pr3[2].Value = encripted_pass;

                        dbconnect.ExecuteStoredProcedure("[dbo].[login_pcdr]", pr3);

                        Session.Abandon();
                        return Json(new { success = true, Message = "Password changed successfully." });

                    }
                    else
                    {
                        return Json(new { success = false, Message = "Error changing password. Please try again." });

                    }
                }
            }

            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return Json(new { success = false, Message = "An error occurred while processing the request." });
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

