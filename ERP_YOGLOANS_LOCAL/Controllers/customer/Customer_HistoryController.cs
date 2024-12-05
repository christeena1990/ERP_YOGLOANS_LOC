using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_HistoryController : Controller
    {
        DB dbconnect = new DB();

        // GET: Customer_History
        public ActionResult cus_history()
        {
           
            return View();
        }
     
        [HttpPost]
        public JsonResult select_cus_id(int customerID)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = customerID;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            Session["customer_id_his"] = dt.Rows[0][0].ToString();

            return Json(new { success = true });
        }
        [HttpGet]
        public JsonResult select_cus_name()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 26;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            string name = dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : string.Empty;
            string status = dt.Rows.Count > 0 ? dt.Rows[0][1].ToString() : string.Empty;
            string customerId = Session["customer_id_his"].ToString();

            return Json(new { success = true, name = name, status = status, customeridshow = customerId }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult select_commu_address()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            string address = dt.Rows[0][0].ToString();

            return Json(new { success = true, address = address }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult select_per_address()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 25;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            string address = dt.Rows[0][0].ToString();

            return Json(new { success = true, address = address }, JsonRequestBehavior.AllowGet);
        }

       


        [HttpGet]
        public JsonResult GetCustomerPhoto()
        {
            string base64String = null;
            byte[] photoData = null;
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 8;

            pr[1] = new SqlParameter("@cid", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            dbconnect.Open();
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[gl_applicant_information]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {
                // Assuming "photo" is the column name in the result set
                if (dt.Rows[0][0].ToString() != "1")
                {
                    object photoObject = dt.Rows[0][0];

                    if (photoObject is byte[])
                    {
                        photoData = (byte[])photoObject;
                    }
                    else if (photoObject is string)
                    {
                        string photoString = (string)photoObject;
                        photoData = Convert.FromBase64String(photoString);
                    }

                    base64String = photoData != null ? Convert.ToBase64String(photoData) : null;

                    return Json(new
                    {
                        success = true,
                        photo = base64String
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Photo is not available"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "No data found"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult Gold_limit()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 11;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            string gold_rate = dt.Rows[0][0].ToString();

            return Json(new { success = true, gold_rate = gold_rate }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult fraud_Dtl()
        {
            string branch_name = "";
            string fraud_desc = "";
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 12;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt == null || dt.Rows.Count == 0)
            {
                return Json(new { success = false });
            }
            else
            {
                branch_name = dt.Rows[0][0] != DBNull.Value ? dt.Rows[0][0].ToString() : string.Empty;
                fraud_desc = dt.Rows[0][2] != DBNull.Value ? dt.Rows[0][2].ToString() : string.Empty;



                return Json(new { success = true, branch_name = branch_name, fraud_desc = fraud_desc }, JsonRequestBehavior.AllowGet);

            }

        }



        [HttpGet]
        public JsonResult kyc_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            var data = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                dict["Column1"] = row["KYC Name"];
                dict["Column2"] = row["ID Number"];
                dict["Column3"] = row["kyc_id"];
                // No need to add an attachment URL
                data.Add(dict);
            }

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Bankdtl_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 23;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            var data = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                dict["Column1"] = row["Acc No"];
                dict["Column2"] = row["IFSC"];
                dict["Column3"] = row["Bank"];
                dict["Column4"] = row["Branch"];
                // No need to add an attachment URL
                data.Add(dict);
            }

            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult bankdtl_grid_attachment( History_model model)
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 24;

            pr[1] = new SqlParameter("@cust_id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return Json(new { success = false });
            }

            model.ImageUrl2 = GetImageUrl(dt.Rows[0]["attachment_data"] as byte[]);

            return Json(new { success = true, data = model.ImageUrl2 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult kyc_grid_attachment(int kyc_id, History_model model)
        {
            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 9;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            pr[2] = new SqlParameter("@kyc_id", SqlDbType.Int);
            pr[2].Value = kyc_id; // Use the kyc_id passed from the client

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value || string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return Json(new { success = false });
            }

            model.ImageUrl1 = GetImageUrl(dt.Rows[0]["image"] as byte[]);

            return Json(new { success = true, data = model.ImageUrl1 }, JsonRequestBehavior.AllowGet);
        }
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
        private string GetImageUrl(byte[] imageBytes)
        {


            string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

            if (IsPdf(imageBytes))
            {
                // It's a PDF
                string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                //kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                return $"data:application/pdf;base64,{base64String}";

            }
            else
            {
                // It's an image
                string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                // kycItem.Add("image", "data:image/png;base64," + base64Image);
                return $"data:image;base64,{base64String}";
            }
        }
        [HttpGet]
        public JsonResult history_tables()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@id", SqlDbType.BigInt);
            pr[1].Value = Session["customer_id_his"];

            DataSet ds = new DataSet();

            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr);
            dbconnect.Close();
            History_model model = new History_model();
            if (ds.Tables.Count > 0)
            
            
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    model.total_1 = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    model.settled_1 = Convert.ToInt32(ds.Tables[0].Rows[0][1]);
                    model.live_1 = Convert.ToInt32(ds.Tables[0].Rows[0][2]);
                    model.module_1 = Convert.ToInt32(ds.Tables[0].Rows[0][3]);
                   // Session["module_1"] = model.module_1;
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    model.total_2 = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                    model.settled_2 = Convert.ToInt32(ds.Tables[1].Rows[0][1]);
                    model.live_2 = Convert.ToInt32(ds.Tables[1].Rows[0][2]);
                    model.module_2 = Convert.ToInt32(ds.Tables[1].Rows[0][3]);
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    model.total_3 = Convert.ToInt32(ds.Tables[2].Rows[0][0]);
                    model.settled_3 = Convert.ToInt32(ds.Tables[2].Rows[0][1]);
                    model.live_3 = Convert.ToInt32(ds.Tables[2].Rows[0][2]);
                    model.module_3 = Convert.ToInt32(ds.Tables[2].Rows[0][3]);
                }
                if (ds.Tables[3].Rows.Count > 0)
                {
                    model.total_4 = Convert.ToInt32(ds.Tables[3].Rows[0][0]);
                    model.settled_4 = Convert.ToInt32(ds.Tables[3].Rows[0][1]);
                    model.live_4 = Convert.ToInt32(ds.Tables[3].Rows[0][2]);
                    model.module_4 = Convert.ToInt32(ds.Tables[3].Rows[0][3]);
                }
                if (ds.Tables[4].Rows.Count > 0)
                {
                    model.total_5 = Convert.ToInt32(ds.Tables[4].Rows[0][0]);
                    model.settled_5 = Convert.ToInt32(ds.Tables[4].Rows[0][1]);
                    model.live_5 = Convert.ToInt32(ds.Tables[4].Rows[0][2]);
                    model.module_5 = Convert.ToInt32(ds.Tables[4].Rows[0][3]);
                }
                if (ds.Tables[5].Rows.Count > 0)
                {
                    model.total_6 = Convert.ToInt32(ds.Tables[5].Rows[0][0]);
                    model.settled_6 = Convert.ToInt32(ds.Tables[5].Rows[0][1]);
                    model.live_6 = Convert.ToInt32(ds.Tables[5].Rows[0][2]);
                    model.module_6 = Convert.ToInt32(ds.Tables[5].Rows[0][3]);
                }
                if (ds.Tables[6].Rows.Count > 0)
                {
                    model.total_7 = Convert.ToInt32(ds.Tables[6].Rows[0][0]);
                    model.settled_7 = Convert.ToInt32(ds.Tables[6].Rows[0][1]);
                    model.live_7 = Convert.ToInt32(ds.Tables[6].Rows[0][2]);
                    model.module_7 = Convert.ToInt32(ds.Tables[6].Rows[0][3]);
                }
                if (ds.Tables[7].Rows.Count > 0)
                {
                    model.total_8 = Convert.ToInt32(ds.Tables[7].Rows[0][0]);
                    model.settled_8 = Convert.ToInt32(ds.Tables[7].Rows[0][1]);
                    model.live_8 = Convert.ToInt32(ds.Tables[7].Rows[0][2]);
                    model.module_8 = Convert.ToInt32(ds.Tables[7].Rows[0][3]);
                }
                if (ds.Tables[8].Rows.Count > 0)
                {
                    model.total_9 = Convert.ToInt32(ds.Tables[8].Rows[0][0]);
                    model.settled_9 = Convert.ToInt32(ds.Tables[8].Rows[0][1]);
                    model.live_9 = Convert.ToInt32(ds.Tables[8].Rows[0][2]);
                    model.module_9 = Convert.ToInt32(ds.Tables[8].Rows[0][3]);
                }
             
            }

            // return Json(new { success = true });
            //return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                data = model,
               // sessionModule = Session["module_1"]
            }, JsonRequestBehavior.AllowGet);
        }
    
        [HttpGet]
        public JsonResult history_tables_select(int buttonValue)
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[3];

                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 5 };
                pr[1] = new SqlParameter("@id", SqlDbType.BigInt) { Value = Session["customer_id_his"] ?? (object)DBNull.Value };
                pr[2] = new SqlParameter("@module_id", SqlDbType.BigInt) { Value = buttonValue };
                dbconnect.Open();
                DataSet ds = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr);
                dbconnect.Close();
                var tables = new Dictionary<string, object>();

                foreach (DataTable table in ds.Tables)
                {
                    var rows = table.AsEnumerable().Select(row => table.Columns.Cast<DataColumn>().ToDictionary(
                        col => col.ColumnName,
                        col => row[col]
                    )).ToList();

                    tables[table.TableName] = new
                    {
                        TableName = table.TableName,
                        Rows = rows
                    };
                }

                return Json(new { success = true, tables }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public JsonResult gold_inventory_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 15;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt != null && dt.Rows.Count > 0)
            {
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row["BRANCH_NAME"];
                    dict["Column2"] = row["AUDIT ID"];
                    dict["Column3"] = row["AUDIT DATE"];
                    dict["Column4"] = row["LOAN NO"];
                    dict["Column5"] = row["REMARKS"];
                    dict["Column6"] = row["CATEGORY"];
                    dict["Column7"] = row["STATUS"];
                    dict["Column8"] = row["CLOSE REMARKS"];
                    dict["Column9"] = row["RM REMARKS"];
                    dict["Column10"] = row["RM REJECT REASON"];

                    // No need to add an attachment URL
                    data.Add(dict);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false });
            }
        }
        [HttpGet]
        public JsonResult EXCESS_DETAILS_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 14;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt != null && dt.Rows.Count > 0)
            {
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row["loan_no"];
                    dict["Column2"] = row["erp_gwt"];
                    dict["Column3"] = row["audit_gwt"];
                    dict["Column4"] = row["gwt_diff"];
                    dict["Column5"] = row["erp_swt"];
                    dict["Column6"] = row["audit_swt"];
                    dict["Column7"] = row["swt_diff"];
                    dict["Column8"] = row["erp_nwt"];
                    dict["Column9"] = row["nwt_diff"];
                    dict["Column10"] = row["erp_prt"];
                    dict["Column11"] = row["audit_prt"];
                    dict["Column12"] = row["prt_diff"];
                    //dict["Column13"] = row["loan_amt"];
                    //dict["Column14"] = row["audit_amt"];
                    //dict["Column15"] = row["Excess"];
                    dict["Column13"] = Convert.ToDecimal(row["loan_amt"]).ToString("F2");   // Convert to decimal and format to 2 decimal places
                    dict["Column14"] = Convert.ToDecimal(row["audit_amt"]).ToString("F2");  // Convert to decimal and format to 2 decimal places
                    dict["Column15"] = Convert.ToDecimal(row["Excess"]).ToString("F2");
                    dict["Column16"] = row["remarks"];

                    // No need to add an attachment URL
                    data.Add(dict);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false });
            }
        }
      

        [HttpGet]
        public JsonResult GOLD_LOAN_CLOSED_AUDIT_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 17;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt != null && dt.Rows.Count > 0)
            {
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row["BRANCH NAME"];
                    dict["Column2"] = row["AUDIT ID"];
                    dict["Column3"] = row["AUDIT DATE"];
                    dict["Column4"] = row["LOAN NO"];
                    dict["Column5"] = row["REMARKS"];
                    dict["Column6"] = row["CATEGORY"];
                    dict["Column7"] = row["status"];
                    dict["Column8"] = row["close_remarks"];
                    dict["Column9"] = row["rm_remarks"];
                    dict["Column10"] = row["rm_reject_reason"];

                    // No need to add an attachment URL
                    data.Add(dict);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false });
            }
        }
     


        [HttpGet]
        public JsonResult SURPLUS_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 13;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataSet ds = new DataSet();
            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr);
            dbconnect.Close();

            // First table data
            var dataTable1 = new List<Dictionary<string, object>>();
            // Second table data
            var dataTable2 = new List<Dictionary<string, object>>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Process the first table (ds.Tables[0])
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row["AUCTION ID"];
                    dict["Column2"] = row["AUCTION DATE"];
                    dict["Column3"] = row["LOAN NO"];
                    dict["Column4"] = row["SURPLUS"];
                    dict["Column5"] = row["TO LOSS"];
                    dict["Column6"] = row["TO LOAN"];
                    dict["Column7"] = row["TO CHEQUE"];
                    dict["Column8"] = row["OTHERS"];
                    dict["Column9"] = row["BALANCE"];

                    dataTable1.Add(dict);
                }
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                // Process the second table (ds.Tables[1])
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row["AUCTION ID"];
                    dict["Column2"] = row["AUCTION DATE"];
                    dict["Column3"] = row["LOAN NO"];
                    dict["Column4"] = row["LOSS"];
                    dict["Column5"] = row["SURPLUS"];
                    dict["Column6"] = row["LOAN"];
                    dict["Column7"] = row["SETTLE"];
                    dict["Column8"] = row["OTHERS"];
                    dict["Column9"] = row["BALANCE"];

                    dataTable2.Add(dict);
                }
            }

            return Json(new
            {
                success = true,
                table1 = dataTable1,
                table2 = dataTable2
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CUSTOMER_KYC_AUDIT_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 16;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt != null && dt.Rows.Count > 0)
            {
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row[0];
                    dict["Column2"] = row[1];
                    dict["Column3"] = row[2];
                    dict["Column4"] = row[3];
                    dict["Column5"] = row[4];
                    dict["Column6"] = row[5];
                    dict["Column7"] = row[6];
                    dict["Column8"] = row[7];
                              
                    // No need to add an attachment URL
                    data.Add(dict);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false });
            }
        }
        [HttpGet]
        public JsonResult AUCTION_HISTORY_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 18;

            pr[1] = new SqlParameter("@id", SqlDbType.Int);
            pr[1].Value = Session["customer_id_his"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Cust_History]", pr).Tables[0];
            dbconnect.Close();
            if (dt != null && dt.Rows.Count > 0)
            {
                var data = new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();

                    dict["Column1"] = row[0];
                    dict["Column2"] = row[1];
                    dict["Column3"] = row[2];
                    dict["Column4"] = row[3];
                    dict["Column5"] = row[4];
                    dict["Column6"] = row[5];
                    dict["Column7"] = row[6];
                    dict["Column8"] = row[7];

                    // No need to add an attachment URL
                    data.Add(dict);
                }

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false });
            }
        }

    }
}