using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class UPIController : Controller
    {
        // GET: UPI

        //for punlish
        //DB dbconnect = new DB();

        DB dbconnect = new DB();

        public ActionResult UPI_View()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SearchData(string fromDate, string toDate, string dueCount)
        {
            List<object> resultData = new List<object>();

            try
            {
                DateTime fromDt = DateTime.MinValue;  // Default value when fromDate is empty or invalid
                if (!string.IsNullOrEmpty(fromDate))
                {
                    fromDt = Convert.ToDateTime(fromDate);
                }

                DateTime toDt = DateTime.MaxValue; // Default value when toDate is empty or invalid
                if (!string.IsNullOrEmpty(toDate))
                {
                    toDt = Convert.ToDateTime(toDate);
                }

                int DCount = string.IsNullOrEmpty(dueCount) ? 0 : Convert.ToInt32(dueCount);



                SqlParameter[] pr = new SqlParameter[4];

                if(Session["module_id"]?.ToString()=="10") //---VL
                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 5 };

                else if(Session["module_id"]?.ToString() == "11")//--BL
                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 };

                    pr[1] = new SqlParameter("@From", SqlDbType.Date) { Value = fromDt };
                    pr[2] = new SqlParameter("@To", SqlDbType.Date) { Value = toDt };
                    pr[3] = new SqlParameter("@duecount", SqlDbType.Int) { Value = DCount };

                dbconnect.Open();
                DataTable dt = dbconnect.ExecuteDataset("[dbo].[UPI_link_List]", pr).Tables[0];
                dbconnect.Close();

                foreach (DataRow row in dt.Rows)
                {
                    resultData.Add(new
                    {
                        No = row["No"].ToString(),
                        LoanNo = row["LoanNo"].ToString(),
                        DueCount = row["DueCount"].ToString(),
                        CustomerID = row["CustomerID"].ToString(),
                        CustomerName = row["CustomerName"].ToString(),
                        MobileNo = row["MobileNo"].ToString(),
                        DueAmount = row["DueAmount"].ToString(),
                        DueDate = row["DueDate"].ToString(),
                        AmtEdit = row["DueAmount"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the error here
            }

            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]


        public ActionResult SubmitSelectedRows(string selectedData, string type, DateTime validDate)
        {
            string outputMessage = string.Empty;
            int amount_type = Convert.ToInt32(type.ToString());
            string remark = string.Empty;

            if (string.IsNullOrEmpty(selectedData))
            {
                return RedirectToAction("UPI_LinkGenerate_View");
            }

            var selectedRowData = JsonConvert.DeserializeObject<List<EMIDetailsRow>>(selectedData);

            DataTable dt = new DataTable();
            dt.Columns.Add("LoanNo", typeof(string));      // Correct data type for LoanNo (nvarchar(50))
            dt.Columns.Add("CustomerID", typeof(long));    // Correct data type for CustomerID (bigint)
            dt.Columns.Add("Customer", typeof(string));    // Correct data type for Customer (nvarchar(500))
            dt.Columns.Add("Mobile", typeof(string));      // Correct data type for Mobile (nvarchar(15))
            dt.Columns.Add("EMI", typeof(float));          // Correct data type for EMI (float)
            dt.Columns.Add("Installment", typeof(int));    // Correct data type for Installment (int)
            dt.Columns.Add("Msged_EMI_Amt", typeof(float));


            foreach (var item in selectedRowData)
            {
                dt.Rows.Add(item.LoanNo, item.CustomerID, item.CustomerName, item.MobileNo, item.DueAmount, item.DueCount, item.Msged_EMI_Amt);
            }


            SqlParameter[] pr = new SqlParameter[8];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 2 };
            pr[1] = new SqlParameter("@UPIRecordList", SqlDbType.Structured) { Value = dt };
            pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            pr[3] = new SqlParameter("@Amttype", SqlDbType.Int) { Value = amount_type };

            if (amount_type == 0)
                remark = "Msg Send with amt Zero";
            else
                remark = "Msg Send with Emi Amount";

            pr[4] = new SqlParameter("@Remark", SqlDbType.NVarChar, 100) { Value = remark };
            pr[5] = new SqlParameter("@Validity_dt", SqlDbType.Date) { Value = validDate };
            pr[6] = new SqlParameter("@module_id", SqlDbType.Int) { Value = Convert.ToInt32(Session["module_id"]?.ToString()) };
            pr[7] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"] };


            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[UPI_link_List]", pr);
            dbconnect.Close();

            if (pr[2].Value != DBNull.Value && pr[2].Value != null)
            {
                outputMessage = pr[2].Value.ToString();
            }
            else
            {
                outputMessage = "No message returned from stored procedure.";
            }


            TempData["OutputMessage"] = outputMessage;


            if (outputMessage == "Successfully Send")
            {
                return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = false, message = "An error occurred while saving the data. Please try again." }, JsonRequestBehavior.AllowGet);
            }

           
        }

        public class EMIDetailsRow
        {
            public string LoanNo { get; set; }          
            public long CustomerID { get; set; }       
            public string CustomerName { get; set; }    
            public string MobileNo { get; set; }        
            public float DueAmount { get; set; }       
            public int DueCount { get; set; }         
            public float Msged_EMI_Amt { get; set; }

        }

    }

}