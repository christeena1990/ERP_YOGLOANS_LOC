using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Drawing.Imaging;
using System.Web.Services.Description;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.IO;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Application_creationController : Controller
    {
        DB dbconnect = new DB();

        FileCompressor doc = new FileCompressor();
        doc_compression ImageCompress = new doc_compression();

        // GET: NCD_Application_creation
        public ActionResult NCD_Application_create()
        {
            return View();
        }
        [HttpGet]
        public JsonResult NCD_fillGrid()
        {
            SqlParameter[] pr1 = new SqlParameter[1];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 1;

            DataTable dt1 = new DataTable();

            dbconnect.Open();
            dt1 = dbconnect.ExecuteDataset("[dbo].[DEB_Application]", pr1).Tables[0];
            dbconnect.Close();

            List<Dictionary<string, object>> schemelist = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt1.Rows)
            {
                Dictionary<string, object> scheme = new Dictionary<string, object>();
                foreach (DataColumn col in dt1.Columns)
                {
                    scheme[col.ColumnName] = row[col];
                }
                schemelist.Add(scheme);
            }

            // Return the data as JSON
            return Json(schemelist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Fill_customerDetails()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 3;
            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = Session["application_custId"];
            DataSet ds = new DataSet();

            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[DEB_Application]", pr);
            dbconnect.Close();

            // Check if the dataset contains data in both tables
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow rowTable1 = ds.Tables[0].Rows[0]; // First table row (for customer_name, dob, age)

                // Check if the second table has data for id_no
                string idNo = "";
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    DataRow rowTable2 = ds.Tables[1].Rows[0]; // Second table row (for id_no)
                    idNo = rowTable2["id_no"].ToString();
                }

                // Prepare customer details object
                var customerDetails = new
                {
                    customer_id = Session["application_custId"],
                    customer_name = rowTable1["customer_name"].ToString(),
                    dob = Convert.ToDateTime(rowTable1["dob"]).ToString("dd/MM/yyyy"), // Format date as needed
                    age = rowTable1["age"].ToString(),
                    id_no = idNo
                };

                // Return the customer details as JSON
                return Json(new { success = true, data = customerDetails }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Return an error message if no data is found
                return Json(new { success = false, message = "No customer details found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult InterestCalculation(NCD_Interest nCD_Interest)
        {
            SqlParameter[] pr = new SqlParameter[4];
            pr[0] = new SqlParameter("@type", SqlDbType.BigInt);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
            pr[1].Value = nCD_Interest.issue_no;

            pr[2] = new SqlParameter("@amount", SqlDbType.Float);
            pr[2].Value = nCD_Interest.amount;

            pr[3] = new SqlParameter("@customerid", SqlDbType.Float);
            pr[3].Value = Session["application_custId"].ToString();
            dbconnect.Open();
            DataTable dt = dbconnect.ExecuteDataset("DEB_interest_calculate", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {
                var InterestDetails = new
                {
                    total_int = dt.Rows[0]["total_int"].ToString(),
                    tot_amt = dt.Rows[0]["tot_amt"].ToString(),
                    nodb = dt.Rows[0]["nodb"].ToString(),
                    startdate = dt.Rows[0]["startdate"].ToString(),
                    senrate = dt.Rows[0]["senrate"].ToString()

                };
                return Json(new { success = true, data = InterestDetails }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No details found" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult GetCanvasserDtls(long canvasserCode,long canvasserType)
        {
            SqlParameter[] pr2 = new SqlParameter[3];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 2;

            pr2[1] = new SqlParameter("@can_code", SqlDbType.BigInt);
            pr2[1].Value = canvasserCode;

            pr2[2] = new SqlParameter("@can_type", SqlDbType.Int);
            pr2[2].Value = canvasserType;

            dbconnect.Open();
            DataTable dt = dbconnect.ExecuteDataset("DEB_Application", pr2).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {
                var CanvasserDetails = new
                {
                    canvasser = dt.Rows[0]["canvasser"].ToString(),
                    //category_name = dt.Rows[0]["category_name"].ToString()
                   // update_status = dt.Rows[0]["update_status"].ToString()
                };
                return Json(new { success = true, data = CanvasserDetails }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No details found" }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public JsonResult FillDropdowns(Int16 cust_age)
        {
            SqlParameter[] pr2 = new SqlParameter[3];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 4;

            pr2[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr2[1].Value = Session["application_custId"].ToString();

            pr2[2] = new SqlParameter("@cust_age", SqlDbType.Int);
            pr2[2].Value = cust_age;

            dbconnect.Open();
            DataSet ds = dbconnect.ExecuteDataset("DEB_Application", pr2);
            dbconnect.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
                // Assuming the dataset returns different tables for different dropdowns
                var dropdownData = new
                {
                    FifteenGOptions = ds.Tables[0].AsEnumerable().Select(row => new
                    {
                        Id = row["_15g_id"],
                        Name = row["_15g_name"]
                    }).ToList(),

                    RepaymentOptions = ds.Tables[1].AsEnumerable().Select(row => new
                    {
                        Id = row["id"],
                        Name = row["repayment"]
                    }).ToList(),

                    DpOptions = ds.Tables[2].AsEnumerable().Select(row => new
                    {
                        Id = row["dp_id"],
                        Name = row["dp_name"]
                    }).ToList()
                };

                return Json(new { success = true, data = dropdownData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SubmitApplication(Submit_NCD_Application submit_NCD_Application)
        {

            byte[] docBytes = null;

            // Check and process the first image (model.image)
            if (submit_NCD_Application.Document != null && submit_NCD_Application.Document.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    submit_NCD_Application.Document.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (submit_NCD_Application.Document.ContentType == "application/pdf")
                    {
                        docBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (submit_NCD_Application.Document.ContentType == "image/png" ||
                          submit_NCD_Application.Document.ContentType == "image/jpg" ||
                          submit_NCD_Application.Document.ContentType == "image/jpeg" ||
                          submit_NCD_Application.Document.ContentType == "image/bmp" ||
                          submit_NCD_Application.Document.ContentType == "image/gif")
                    {
                        docBytes = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        // Handle unsupported content types or throw an error
                        throw new NotSupportedException("Unsupported file type.");
                    }
                }
            }



            SqlParameter[] pr2 = new SqlParameter[28];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 5;

            pr2[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr2[1].Value = Session["application_custId"].ToString();

            pr2[2] = new SqlParameter("@issue_no", SqlDbType.BigInt);
            pr2[2].Value = submit_NCD_Application.issue_no;

            pr2[3] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr2[3].Value = Session["branch_id"].ToString();

            pr2[4] = new SqlParameter("@amount", SqlDbType.Float);
            pr2[4].Value = submit_NCD_Application.amount;

            pr2[5] = new SqlParameter("@matamt", SqlDbType.Float);
            pr2[5].Value = submit_NCD_Application.matamt;

            pr2[6] = new SqlParameter("@totint", SqlDbType.Float);
            pr2[6].Value = submit_NCD_Application.totint;

            pr2[7] = new SqlParameter("@matdate", SqlDbType.VarChar,10);
            pr2[7].Value = submit_NCD_Application.matdate;

            pr2[8] = new SqlParameter("@nod", SqlDbType.BigInt);
            pr2[8].Value = submit_NCD_Application.nod;

            pr2[9] = new SqlParameter("@canvas_code", SqlDbType.BigInt);
            pr2[9].Value = submit_NCD_Application.canvas_code;

            pr2[10] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr2[10].Value = Session["login_user"].ToString();

            pr2[11] = new SqlParameter("@renewal_amt", SqlDbType.Float);
            pr2[11].Value = submit_NCD_Application.renewal_amt;

            pr2[12] = new SqlParameter("@nofapplicant", SqlDbType.Int);
            pr2[12].Value = submit_NCD_Application.nofapplicant;

            pr2[13] = new SqlParameter("@ac_type", SqlDbType.Char,1);
            pr2[13].Value = submit_NCD_Application.ac_type;

            pr2[14] = new SqlParameter("@tax_payee", SqlDbType.Char,1);
            pr2[14].Value = "N";//submit_NCD_Application.tax_payee;

            pr2[15] = new SqlParameter("@lienholder", SqlDbType.VarChar,50);
            pr2[15].Value = submit_NCD_Application.lienholder;

            pr2[16] = new SqlParameter("@lien", SqlDbType.Char,1);
            pr2[16].Value = submit_NCD_Application.lien;

            pr2[17] = new SqlParameter("@rpay", SqlDbType.Char, 1);
            pr2[17].Value = submit_NCD_Application.rpay;

            pr2[18] = new SqlParameter("@15_status", SqlDbType.Char,1);
            pr2[18].Value = submit_NCD_Application._15_status;            

            pr2[19] = new SqlParameter("@15_type", SqlDbType.Char, 1);
            pr2[19].Value = submit_NCD_Application._15_type;

            pr2[20] = new SqlParameter("@dp_id", SqlDbType.VarChar,100);
            pr2[20].Value = submit_NCD_Application.dp_id;

            pr2[21] = new SqlParameter("@dp_name", SqlDbType.VarChar,200);
            pr2[21].Value = submit_NCD_Application.dp_name;

            pr2[22] = new SqlParameter("@pancard", SqlDbType.VarChar, 20);
            pr2[22].Value = submit_NCD_Application.pancard;

            pr2[23] = new SqlParameter("@nominee", SqlDbType.VarChar, 100);
            pr2[23].Value = submit_NCD_Application.nominee;

            pr2[24] = new SqlParameter("@nominee_add", SqlDbType.VarChar, 200);
            pr2[24].Value = submit_NCD_Application.nominee_add;

            pr2[25] = new SqlParameter("@relation", SqlDbType.VarChar, 50);
            pr2[25].Value = submit_NCD_Application.relation;

            pr2[26] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr2[26].Direction = ParameterDirection.Output;

            pr2[27] = new SqlParameter("@can_type", SqlDbType.Int);
            pr2[27].Value = submit_NCD_Application.canvas_type;


            dbconnect.Open();
            dbconnect.ExecuteDataset("DEB_Application", pr2);
            //DataSet ds = dbconnect.ExecuteDataset("DEB_Application", pr2);
            dbconnect.Close();

            string message= pr2[26].Value.ToString();

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);

        }
    }
}