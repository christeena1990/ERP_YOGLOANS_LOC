using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class Personal_Loan_Branch_DisburseController : Controller
    {
        DB dbconnect = new DB();

        //PERSONAL LOAN - BRANCH DISBURSE (CONTROL ACCOUNT)
        //Pl_loan_application (Pay_mode:5)
        //No Need of  Bank Details,View Bank Details,Select HO bank,Payment Type and Mode Selection (dropdown lists)
        //---------------------------------------------------
        
        // GET: Personal_Loan_Branch_Disburse

        public ActionResult Personal_Loan_Branch_Disburse_View()
        {
            try
            {
                var pl_table_values = personal_loan_bankdisburse_grid();
                bool isEmpty = pl_table_values.Rows.Count == 0;
                ViewBag.IsEmpty = isEmpty;

                return View(pl_table_values);
            } 
            catch 
            { 
            }
            return View();
        }
        private DataTable personal_loan_bankdisburse_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 29;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;

        }
        [HttpPost]
        public JsonResult PL_BranchDisburse_details(long applicationid)
        {
            //Session["app_id"] = applicationid;
            Session["PL_application_id"] = applicationid;

            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 21;

            pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 15);
            pr[1].Value = Session["PL_application_id"];

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            dbconnect.Open();
            DataTable dtLoan = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();

            if (dtLoan.Rows.Count > 0)
            {
                DataRow row = dtLoan.Rows[0];

                var model = new
                {
                    CustomerId = row["customer_id"] != DBNull.Value ? row["customer_id"].ToString() : string.Empty,
                    CustomerName = row["customer_name"] != DBNull.Value ? row["customer_name"].ToString() : string.Empty,
                    ApplicationID = row["application_id"] != DBNull.Value ? row["application_id"].ToString() : string.Empty,
                    LoanNo = row["gl_loan_no"] != DBNull.Value ? row["gl_loan_no"].ToString() : string.Empty,
                    LoanAmt = row["loan_amount"] != DBNull.Value ? Convert.ToDecimal(row["loan_amount"]) : 0.0m,
                    BranchId = row["branch_id"] != DBNull.Value ? row["branch_id"].ToString() : string.Empty,
                    Receivables = row["receivables"] != DBNull.Value ? row["receivables"].ToString() : string.Empty,
                    Disbursement = row["disb_amt"] != DBNull.Value ? row["disb_amt"].ToString() : string.Empty,

                   
                };


                Session["customer_id"] = model.CustomerId;
                return Json(model, JsonRequestBehavior.AllowGet);
            }


            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Branch_DisburseLoan(string appId, string loanAmt, string disAmt, string actionType)
        {
            string outputMessage = string.Empty;

            try
            {
                if (actionType == "Disburse")
                {
                    SqlParameter[] pr = new SqlParameter[5];

                    pr[0] = new SqlParameter("@appid", SqlDbType.VarChar, 15);
                    pr[0].Value = appId;

                    pr[1] = new SqlParameter("@loanamt", SqlDbType.Decimal);
                    pr[1].Value = Convert.ToDecimal(loanAmt);

                    pr[2] = new SqlParameter("@disamt", SqlDbType.Decimal);
                    pr[2].Value = Convert.ToDecimal(disAmt);

                    pr[3] = new SqlParameter("@userid", SqlDbType.BigInt);
                    pr[3].Value = Session["login_user"]; // Assuming user ID is stored in session

                    pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    pr[4].Direction = ParameterDirection.Output;

                    dbconnect.Open();
                    dbconnect.ExecuteStoredProcedure("[dbo].[pl_proc_disbursement]", pr);
                    dbconnect.Close();

                    outputMessage = pr[4].Value.ToString();
                    TempData["ResultMessage"] = outputMessage;



                    string[] messageParts = outputMessage.Split('!');
                    string message;
                    string resultCode = messageParts.Length > 1 ? messageParts[1] : "";

                    if (messageParts[1] != "")
                    {
                        string[] msg = messageParts[1].Split('~');
                        string new_lno = msg[0];
                        message = messageParts[0]+" ( Loan No : " + new_lno.ToString()+" )";
                        

                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = messageParts[0];
                        outputMessage = message;

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    //outputMessage = message;
                     //return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = "An error occurred during disbursement: " + ex.Message;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Personal_Loan_Branch_Disburse_View");

        }


    }
}