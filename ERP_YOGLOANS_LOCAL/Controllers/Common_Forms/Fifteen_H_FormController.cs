using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Common_Forms
{
    public class Fifteen_H_FormController : Controller
    {
          DB dbconnect = new DB();
        public ActionResult Fifteen_H_Form_View()
        {
           //string pan= Session["pan_no"].ToString();
           //string trid =Session["Track_ID"] .ToString();
          // string appid= Session["appid"]?.ToString();
            return View();
        }
      
       
        [HttpGet]
        public JsonResult fifteen_H_details()
        {


            // Get the Application ID from the session
            var applicationId = Session["appid"]?.ToString();

            string sqlQuery = string.Empty;
            DataSet ds = new DataSet();

            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@panid", SqlDbType.VarChar, 15);
            pr[1].Value = Session["pan_no"];

            pr[2] = new SqlParameter("@pantrack", SqlDbType.VarChar, 15);
            pr[2].Value = Session["Track_ID"];

            
            pr[3] = new SqlParameter("@application_id", SqlDbType.VarChar, 15);
            if (applicationId != null)
            {
                pr[3].Value = applicationId;
            }
            else
            {
                pr[3].Value = null;
                //pr[3].Value = 0;
            }

            pr[4] = new SqlParameter("@module_id", SqlDbType.VarChar, 15);
            pr[4].Value = Session["module_id"];


            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr);
            dbconnect.Close();

            

            List<Dictionary<string, object>> tableData = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> table1Data = new List<Dictionary<string, object>>(); // For table1

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {

                    var pantrack = Session["Track_ID"] ?? string.Empty;

                    var customerName = row["customer_name"]?.ToString() ?? string.Empty;
                    var mobileNo = row["mobile_no"]?.ToString() ?? string.Empty;
                    var emailId = row["email_id"]?.ToString() ?? string.Empty;
                    var cHouseName = row["c_housename"]?.ToString() ?? string.Empty;
                    var cStreetName = row["c_street_name"]?.ToString() ?? string.Empty;
                    var cCity = row["c_city"]?.ToString() ?? string.Empty;
                    var postName = row["post_name"]?.ToString() ?? string.Empty;
                    var pinCode = row["pin_code"]?.ToString() ?? string.Empty;
                    var districtName = row["district_name"]?.ToString() ?? string.Empty;
                    var stateName = row["state_name"]?.ToString() ?? string.Empty;
                    var panCardNo = row["PancARdno"]?.ToString() ?? string.Empty;

                    // Using nullable types to allow null values
                    int? totalNo15Field = null;
                    int? totalIntCif = null;
                    decimal? aggregateAmt = null;
                    decimal? estimatedIncome = null;
                    decimal? estimatedTotalIncome = null;

                    //// Safely handle integer and decimal conversions
                    //int.TryParse(row["tot_no_15_field"]?.ToString(), out var tempTotalNo15Field);
                    //totalNo15Field = tempTotalNo15Field != 0 ? tempTotalNo15Field : (int?)null;

                    //int.TryParse(row["tot_int_cif"]?.ToString(), out var tempTotalIntCif);
                    //totalIntCif = tempTotalIntCif != 0 ? tempTotalIntCif : (int?)null;

                    //decimal.TryParse(row["aggregate_amt"]?.ToString(), out var tempAggregateAmt);
                    //aggregateAmt = tempAggregateAmt != 0 ? tempAggregateAmt : (decimal?)null;

                    //decimal.TryParse(row["estincome"]?.ToString(), out var tempEstimatedIncome);
                    //estimatedIncome = tempEstimatedIncome != 0 ? tempEstimatedIncome : (decimal?)null;

                    //decimal.TryParse(row["esttotincom"]?.ToString(), out var tempEstimatedTotalIncome);
                    //estimatedTotalIncome = tempEstimatedTotalIncome != 0 ? tempEstimatedTotalIncome : (decimal?)null;




                    totalNo15Field = !string.IsNullOrEmpty(row["tot_no_15_field"]?.ToString())
                 ? Convert.ToInt32(row["tot_no_15_field"])
                 : (int?)null;

                    totalIntCif = !string.IsNullOrEmpty(row["tot_int_cif"]?.ToString())
                                  ? Convert.ToInt32(row["tot_int_cif"])
                                  : (int?)null;

                    aggregateAmt = !string.IsNullOrEmpty(row["aggregate_amt"]?.ToString())
                                   ? Convert.ToDecimal(row["aggregate_amt"])
                                   : (decimal?)null;

                    estimatedIncome = !string.IsNullOrEmpty(row["estincome"]?.ToString())
                                      ? Convert.ToDecimal(row["estincome"])
                                      : (decimal?)null;

                    estimatedTotalIncome = !string.IsNullOrEmpty(row["esttotincom"]?.ToString())
                                           ? Convert.ToDecimal(row["esttotincom"])
                                           : (decimal?)null;







                    var status = row["status"]?.ToString() ?? string.Empty;
                    var fy = row["fy"]?.ToString() ?? string.Empty;
                    var premises = row["premises"]?.ToString() ?? string.Empty;
                    var residentialStatus = row["resstatus"]?.ToString() ?? string.Empty;
                    var assessmentYear = row["assyr"]?.ToString() ?? string.Empty;

                    // Safe date parsing
                    DateTime formattedDate_endDate = DateTime.MinValue;
                    DateTime.TryParse(row["enddt"]?.ToString(), out formattedDate_endDate);
                    string endDate = formattedDate_endDate != DateTime.MinValue ? formattedDate_endDate.ToString("dd-MMM-yyyy") : string.Empty;

                    var panTrack = row["pantrack"]?.ToString() ?? string.Empty;
                    var city = row["city"]?.ToString() ?? string.Empty;

                    DateTime formate_dob = DateTime.MinValue;
                    DateTime.TryParse(row["dob"]?.ToString(), out formate_dob);
                    string dob = formate_dob != DateTime.MinValue ? formate_dob.ToString("dd-MMM-yyyy") : string.Empty;

                    DateTime formate_date = DateTime.MinValue;
                    DateTime.TryParse(row["dt"]?.ToString(), out formate_date);
                    string date = formate_date != DateTime.MinValue ? formate_date.ToString("dd-MMM-yyyy") : string.Empty;

                    DateTime formate_otpDate = DateTime.MinValue;
                    DateTime.TryParse(row["otpdt"]?.ToString(), out formate_otpDate);
                    string otpDate = formate_otpDate != DateTime.MinValue ? formate_otpDate.ToString("dd-MMM-yyyy") : string.Empty;

                    var signature = row["signature"]?.ToString() ?? string.Empty;




                    Dictionary<string, object> rowData = new Dictionary<string, object>
            {

                { "pantrack", pantrack },
                { "customer_name", customerName },
                { "mobile_no", mobileNo },
                { "email_id", emailId },
                { "c_housename", cHouseName },
                { "c_street_name", cStreetName },
                { "c_city", cCity },
                { "post_name", postName },
                { "pin_code", pinCode },
                { "district_name", districtName },
                { "state_name", stateName },
                { "PancARdno", panCardNo },
                { "tot_no_15_field", totalNo15Field },
                { "tot_int_cif", totalIntCif },
                { "aggregate_amt", aggregateAmt },
                { "status", status },
                { "fy", fy },
                { "premises", premises },
                { "estincome", estimatedIncome },
                { "esttotincom", estimatedTotalIncome },
                { "resstatus", residentialStatus },
                { "assyr", assessmentYear },
                { "enddt", endDate },
                //{ "pantrack", panTrack },
                { "city", city },
                { "dob", dob },
                { "dt", date },
                { "otpdt", otpDate }   ,
                 { "signature", signature }
            };

                    tableData.Add(rowData);
                }

                // Assuming you have a second table in your query for "table1"
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                       
                        var a1 = row["a1"].ToString();
                        var a2 = row["a2"].ToString();
                        var a3 = row["a3"].ToString();
                        var a4 = row["a4"].ToString();

                        Dictionary<string, object> row1Data = new Dictionary<string, object>
                {
                  
                    { "a1", a1 },
                    { "a2", a2 },
                    { "a3", a3 },
                    { "a4", a4 }
                };

                        table1Data.Add(row1Data);
                    }
                }
            }

            return Json(new
            {
                success = true,
                data = tableData,
                table1 = table1Data // Returning table1 data as well
            }, JsonRequestBehavior.AllowGet);
        }

    }

}