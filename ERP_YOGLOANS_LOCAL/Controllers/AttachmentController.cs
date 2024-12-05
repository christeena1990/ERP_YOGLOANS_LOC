using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class AttachmentController : Controller
    {
        DB dbconnect = new DB();
        private object img_customer_m;
        private object ImageUrl1;
        private string base64String;

        private bool IsPdf(byte[] byteData)
        {
            // Check if the byte array represents a PDF file
            return byteData.Length > 4 &&
                   byteData[0] == 0x25 &&
                   byteData[1] == 0x50 &&
                   byteData[2] == 0x44 &&
                   byteData[3] == 0x46 &&
                   byteData[4] == 0x2D;
        }


        


        public ActionResult LoanAttachments_View()
        {
            LoanDocuments_Model model = new LoanDocuments_Model();
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
            pr[0].Value = 12;

            pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 15);
            pr[1].Value = Session["PL_application_id"];

            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[pl_queries_demo]", pr);

            if (ds.Tables.Count > 0)
            {
                DataTable dtDoc = ds.Tables[0];
                if (dtDoc.Rows.Count > 0)
                {
                    model.Data = new List<Dictionary<string, object>>();
                    model.HasData = true;  // Documents exist

                    foreach (DataRow data in ds.Tables[0].Rows)
                    {
                        var PL_DocItem = new Dictionary<string, object>
                {
                    { "Doc_name", data["Doc_name"].ToString() }
                };

                        if (data["Doc"] != DBNull.Value)
                        {
                            byte[] byteDoc = (byte[])data["Doc"];
                            string base64String = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);

                            if (IsPdf(byteDoc))
                            {
                                // It's a PDF
                                string base64Pdf = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                PL_DocItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                            }
                            else
                            {
                                // It's an image
                                string base64Image = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                PL_DocItem.Add("image", "data:image/png;base64," + base64Image);
                            }
                        }

                        model.Data.Add(PL_DocItem);
                    }
                }
                else
                {
                    model.HasData = false;  // No documents found
                }
            }

            return View("LoanAttachments_View", model);
        }

        // GET: Attachment
        public ActionResult BankAttachments_View()
        {
            LoanDocuments_Model model = new LoanDocuments_Model();
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
            pr[0].Value = 21;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = Session["customer_id"];

            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr);

            if (ds.Tables.Count > 0)
            {
                DataTable dtDoc = ds.Tables[0];
                if (dtDoc.Rows.Count > 0)
                {
                    model.Data = new List<Dictionary<string, object>>();
                    model.HasData = true;  // Documents exist

                    foreach (DataRow data in ds.Tables[0].Rows)
                    {
                        var PL_DocItem = new Dictionary<string, object>
                {
                    { "Doc_name", data["Doc_name"].ToString() }
                };

                        if (data["Doc"] != DBNull.Value)
                        {
                            byte[] byteDoc = (byte[])data["Doc"];
                            string base64String = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);

                            if (IsPdf(byteDoc))
                            {
                                // It's a PDF
                                string base64Pdf = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                PL_DocItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                            }
                            else
                            {
                                // It's an image
                                string base64Image = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                PL_DocItem.Add("image", "data:image/png;base64," + base64Image);
                            }
                        }

                        model.Data.Add(PL_DocItem);
                    }
                }
                else
                {
                    model.HasData = false;  // No documents found
                }
            }
            return View("BankAttachments_View", model);
        }

    }
}