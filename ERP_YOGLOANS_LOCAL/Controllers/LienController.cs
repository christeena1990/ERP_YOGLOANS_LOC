using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class LienController : Controller
    {
        // GET: Lien
        DB dbconnect = new DB();
        public ActionResult Lien_mark_view()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCertificateNumbers(string issue_no)
        {
            try
            {
                if (string.IsNullOrEmpty(issue_no))
                {
                    return Json(new { success = false, message = "Issue number is required." });
                }

                var certList = new List<SelectListItem>();

                // Fetch certificates from the database

                SqlParameter[] parameters = new SqlParameter[3];

                parameters[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 1 };
                parameters[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = Convert.ToInt32(issue_no) };
                parameters[2] = new SqlParameter("@module_id", SqlDbType.Int) { Value = Convert.ToInt32(Session["module_id"]?.ToString()) };
                
                dbconnect.Open();
                DataSet ds_Certi = new DataSet();
                ds_Certi = dbconnect.ExecuteDataset("[dbo].[DEB_lien_queries]", parameters);
                dbconnect.Close();

                if (ds_Certi != null && ds_Certi.Tables.Count > 0 && ds_Certi.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds_Certi.Tables[0].Rows)
                    {
                        certList.Add(new SelectListItem
                        {
                            Value = row["val"].ToString(),
                            Text = row["certificate_no"].ToString()
                        });
                    }
                    return Json(new { success = true, data = certList });
                }
                else
                {
                    return Json(new { success = false, message = "No certificates found for the provided issue number." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult GetLienData(string issue_no, string cert)
        {
            try
            {
                if (string.IsNullOrEmpty(issue_no) || string.IsNullOrEmpty(cert))
                {
                    return Json(new { success = false, message = "Issue number and certificate number are required." });
                }

                SqlParameter[] pr = new SqlParameter[4];

                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 2 };
                pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt) { Value = Convert.ToInt64(issue_no) };
                pr[2] = new SqlParameter("@cert", SqlDbType.VarChar, 15) { Value = cert };
                pr[3] = new SqlParameter("@module_id", SqlDbType.Int) { Value = Convert.ToInt32(Session["module_id"].ToString()) };

                dbconnect.Open();
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_lien_queries]", pr);
                dbconnect.Close();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var table1 = ds.Tables[0].AsEnumerable().Select(row => new
                    {
                        IssueNo = row["Issue_No"],
                        CertificateNo = row["Certificate"],
                        Customer = row["Name"],
                        CustomerID = row["customer_id"],
                        PANCard = row["pancard_no"]

                       
                    }).ToList();

                    var table2 = ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0
                        ? new
                        {
                            id = ds.Tables[1].Rows[0]["id"],
                            lien = ds.Tables[1].Rows[0]["lien"],
                            lien_markedto = ds.Tables[1].Rows[0]["lien_markedto"],
                            status = ds.Tables[1].Rows[0]["status"],
                            lbl = ds.Tables[1].Rows[0]["lbl"]
                        }
                        : null;

                    return Json(new { success = true, table1 = table1, table2 = table2 });
                }
                else
                {
                    return Json(new { success = false, message = "No data found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateLien(UpdateLienRequest request)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[7];

                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                pr[1] = new SqlParameter("@type_id", SqlDbType.Char) { Value = request.type_id };
                pr[2] = new SqlParameter("@id", SqlDbType.BigInt) { Value = request.id };
                pr[3] = new SqlParameter("@module_id", SqlDbType.Int) { Value = Session["module_id"].ToString() };
                pr[4] = new SqlParameter("@marked_against", SqlDbType.VarChar, 200) { Value = request.marked_against };
                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                pr[6] = new SqlParameter("@entry_by", SqlDbType.BigInt) { Value = Session["login_user"] };


                dbconnect.Open();
                string outputMessage = string.Empty;
                dbconnect.ExecuteStoredProcedure("[dbo].[DEB_lien_queries]", pr); 
                outputMessage = pr[5].Value.ToString();
                dbconnect.Close();

                string message = pr[5].Value.ToString();
                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public class UpdateLienRequest
        {
            public string type_id { get; set; }
            public long id { get; set; }
            public string marked_against { get; set; }
            
        }
    }
}