using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.personal_loan_models;
using System.Net;
using ERP_YOGLOANS_LOCAL.Models;


namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_Report_StatementofAccountController : Controller
    {
        DB dbconnect = new DB();
        
        // GET: PL_Report_StatementofAccount
        public ActionResult PL_StatementofAccount()
        {
            return View();
        }

        public ActionResult GetSearchSuggestions(string search_txt)
        {

            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;
            pr[1] = new SqlParameter("@search_txt", SqlDbType.VarChar, 50);
            pr[1].Value = search_txt;
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];
            pr[3] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[3].Value = Session["login_user"];


            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr).Tables[0];
            dbconnect.Close();

            var result = dt.AsEnumerable().Select(row => row["Loan"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public ActionResult GetLoanStatementData(string loanId)
        {
            if (string.IsNullOrEmpty(loanId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Loan number is required.");
            }

            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr1[0].Value = 6;
            pr1[1] = new SqlParameter("@loan_no", SqlDbType.VarChar, 50);
            pr1[1].Value = loanId;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_reports]", pr1).Tables[0];
            dbconnect.Close();


            if (dt.Rows.Count == 0)
            {
                return Json(new { success = false, message = "No data found for the selected loan number." });
            }

           

            var statementData = dt.AsEnumerable().Select(row => new
            {
                BranchName = row.Field<string>("branch_name") ?? "N/A",
                LoanNumber = row.Field<string>("loan_no") ?? "N/A",
                CustomerName = row.Field<string>("customer_name") ?? "N/A",
                CustomerId = row.Field<long?>("customer_id") ?? 0, // Use long since customer_id is bigint
                LoanDate = row.Field<object>("loan_dt") != DBNull.Value ? DateTime.Parse(row.Field<string>("loan_dt")) : DateTime.MinValue,
                MaturityDate = row.Field<object>("MaturityDt") != DBNull.Value ? DateTime.Parse(row.Field<string>("MaturityDt")) : DateTime.MinValue,
                GlLoanNo = row.Field<string>("gl_loan_no") ?? "N/A",
                InterestRate = row.Field<object>("int_rate") != DBNull.Value ? row.Field<double>("int_rate") : 0.0, // Use double for float
               //Status = GetStatusDescription(row.Field<int?>("status_id")),
                Status = row.Field<string>("status_id") ?? "N/A",
                Address = row.Field<string>("address") ?? "N/A",
                Landmark = row.Field<string>("land_mark") ?? "N/A",
                City = row.Field<string>("city") ?? "N/A",
                PostOffice = row.Field<string>("post_name") ?? "N/A",
                PinCode = row.Field<object>("pin_code") != DBNull.Value ? row.Field<int>("pin_code").ToString() : "N/A", // Ensure pin_code is handled correctly
                State = row.Field<string>("state_name") ?? "N/A",
                District = row.Field<string>("district_name") ?? "N/A",
                MobileNo = row.Field<string>("mobile_no") ?? "N/A",
                LandNo = row.Field<string>("land_no") ?? "N/A",
                OfficeNo = row.Field<string>("office_no") ?? "N/A",
                ResidenceNo = row.Field<string>("residence_no") ?? "N/A",
                ResidenceType = row.Field<string>("residence_type") ?? "N/A"
            }).ToList();



            return Json(statementData, JsonRequestBehavior.AllowGet);



        }







       
    }

    }
