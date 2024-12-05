
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static ERP_YOGLOANS_LOCAL.Models.personal_loan_models.PLForms_Model;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class Application_FormController : Controller
    {

        // GET: Application_Form
        public ActionResult ApplctnForm_PL()
        {
            DB dbconnect = new DB();
            LoanApplicationViewModel Model = new LoanApplicationViewModel();
            /* string applicationId = "28000124500014";*/ // Hardcoded application ID
                                                          // string applicationId = "Session["application_id"]";

            try
            {
                // First Query
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 30 };
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14) { Value = Session["application_id"] };

                DataSet ds = new DataSet();
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                // Check results from the first query
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_ApplctnForm = ds.Tables[0];
                    Model.BranchName = dt_ApplctnForm.Rows[0]["branch_name"].ToString();
                    Model.BranchCode = Convert.ToInt32(dt_ApplctnForm.Rows[0]["branch_id"]);
                    Model.CustomerName = dt_ApplctnForm.Rows[0]["customer_name"].ToString();
                    Model.CustomerId = Convert.ToInt32(dt_ApplctnForm.Rows[0]["customer_id"]);
                    Model.FathersName = dt_ApplctnForm.Rows[0]["father"].ToString();
                    Model.SpousesName = dt_ApplctnForm.Rows[0]["spouse"].ToString();
                    Model.Address = dt_ApplctnForm.Rows[0]["app_address"].ToString();
                    Model.PAN = dt_ApplctnForm.Rows[0]["pan"].ToString();
                    Model.MobileNo = dt_ApplctnForm.Rows[0]["mobile_no"].ToString();
                    Model.EmailId = dt_ApplctnForm.Rows[0]["email_id"].ToString();
                    Model.Purpose = dt_ApplctnForm.Rows[0]["purpose"].ToString();
                    Model.RepaymentMode = dt_ApplctnForm.Rows[0]["RepaymentMode"].ToString();
                    Model.Duration = Convert.ToInt32(dt_ApplctnForm.Rows[0]["duration"]);

                    // Additional fields
                    Model.GLLoanNo = dt_ApplctnForm.Rows[0]["gl_loan_no"].ToString();
                    Model.InventoryId = dt_ApplctnForm.Rows[0]["inventory_id"].ToString();
                    Model.NetWeight = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["net_wt"]);
                    Model.GLLoanAmount = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["gl_loan_amt"]);
                    Model.PLLoanAmount = Convert.ToDecimal(dt_ApplctnForm.Rows[0]["Pl_loan_amt"]);
                    Model.Date = dt_ApplctnForm.Rows[0]["Date"].ToString();
                }

                // Second Query: Sanction Letter
                SqlParameter[] pr1 = new SqlParameter[2];
                pr1[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 31 };
                pr1[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14) { Value = Session["application_id"] };

                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr1);
                dbconnect.Close();

                // Check results from the second query
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_SanctionLetter = ds.Tables[0];
                    Model.BranchName = dt_SanctionLetter.Rows[0]["branch_name"].ToString();
                    Model.BranchCode = Convert.ToInt32(dt_SanctionLetter.Rows[0]["branch_id"]);
                    Model.CustomerName = dt_SanctionLetter.Rows[0]["customer_name"].ToString();
                    Model.Address = dt_SanctionLetter.Rows[0]["address"].ToString();
                    Model.GLLoanNo = dt_SanctionLetter.Rows[0]["gl_loan_no"].ToString();
                    Model.InventoryId = dt_SanctionLetter.Rows[0]["inventory_id"].ToString();
                    Model.NetWeight = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["net_wt"]);
                    Model.GLLoanAmount = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["gl_loan_amt"]);
                    Model.PLLoanAmount = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["Pl_loan_amt"]);
                    Model.GLdate = dt_SanctionLetter.Rows[0]["gl_loan_dt"].ToString();
                    Model.GLMaturitydate = dt_SanctionLetter.Rows[0]["gl_maturity_dt"].ToString();
                    Model.TotalLoan = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["Total_loan"]);
                    Model.PLinterestRate = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["Pl_int_rt"]);
                    Model.PLMaturitydate = dt_SanctionLetter.Rows[0]["Pl_maturity_dt"].ToString();
                    Model.Total_ltv = dt_SanctionLetter.Rows[0]["Total_ltv"].ToString();
                    Model.GL_ltv = dt_SanctionLetter.Rows[0]["Gl_ltv"].ToString();
                    Model.GLoutstanding = Convert.ToDecimal(dt_SanctionLetter.Rows[0]["gl_outstand"]);
                    Model.BankName = dt_SanctionLetter.Rows[0]["bank_name"].ToString();
                    Model.Payeename = dt_SanctionLetter.Rows[0]["payee_name"].ToString();
                    Model.AcctNo = dt_SanctionLetter.Rows[0]["acc_no"].ToString();
                    Model.IFSC = dt_SanctionLetter.Rows[0]["ifsc_code"].ToString();
                }

                // Third Query: Loan Agreement
                SqlParameter[] pr2 = new SqlParameter[2];
                pr2[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 32 };
                pr2[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 14) { Value = Session["application_id"] };

                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr2);
                dbconnect.Close();

                // Check results from the third query
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_LoanAgreement = ds.Tables[0];
                    Model.BranchName = dt_LoanAgreement.Rows[0]["branch_name"].ToString();
                    Model.BranchAddress = dt_LoanAgreement.Rows[0]["branch_address"].ToString();
                    Model.CustomerName = dt_LoanAgreement.Rows[0]["customer_name"].ToString();
                    Model.Address = dt_LoanAgreement.Rows[0]["app_address"].ToString();
                    Model.ApplctnDate = dt_LoanAgreement.Rows[0]["applicationDt"].ToString();
                    Model.PLLoanAmount = Convert.ToDecimal(dt_LoanAgreement.Rows[0]["Pl_loan_amt"]);
                    Model.PLMaturitydate = dt_LoanAgreement.Rows[0]["Pl_maturity_dt"].ToString();
                    Model.GLLoanNo = dt_LoanAgreement.Rows[0]["gl_loan_no"].ToString();
                    Model.InventoryId = dt_LoanAgreement.Rows[0]["inventory_id"].ToString();
                    Model.NetWeight = Convert.ToDecimal(dt_LoanAgreement.Rows[0]["net_wt"]);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework or simply write to console)
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return View(Model);
        }
    }



        
}