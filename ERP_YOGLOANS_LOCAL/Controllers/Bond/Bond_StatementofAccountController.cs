using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.Bond_models;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_StatementofAccountController : Controller
    {
        DB dbconnect = new DB();
        // GET: Bond_StatementofAccount
      
            [HttpGet]
            public ActionResult Bond_StatementofAccount()
            {

                fillStatementOfAccount();
                //Transactiondetails();
                return View();
            }




            [HttpPost]
            public ActionResult SaveBondId(string bond_id)
            {
                if (string.IsNullOrEmpty(bond_id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Debenture ID is required.");
                }

                // Save debid in TempData

                Session["bond_id"] = bond_id;
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }




            [HttpPost]
            public ActionResult fillStatementOfAccount()
            {
                var model = new StatementOfAccountModel();


                SqlParameter[] pr1 = new SqlParameter[2];
                pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr1[0].Value = 9;
                pr1[1] = new SqlParameter("@bond_id", SqlDbType.VarChar, 50);
                pr1[1].Value = Session["bond_id"];


                DataSet ds = new DataSet();
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[BOND_report]", pr1);
                dbconnect.Close();

                if (ds != null && ds.Tables.Count >= 0)
                {

                    DataTable personal_dt = ds.Tables[0];
                    DataTable adrs = ds.Tables[1];
                    DataTable jointaplcnt = ds.Tables[2];
                    DataTable bank_dt = ds.Tables[3];
                    DataTable transfer_dt = ds.Tables[4];


                    if (personal_dt.Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];
                        model.BranchName = row["branch_name"]?.ToString() ?? null;
                        model.CustomerName = row["customer_name"]?.ToString() ?? null;
                        model.CustomerId = row["customer_id"]?.ToString() ?? null;
                        model.IssueNumber = row["issue_no"]?.ToString() ?? null;
                        model.CertificateNo = row["certificate_no"]?.ToString() ?? null;
                        model.DistinctiveNoFrom = row["folionofrom"]?.ToString() ?? null;
                        model.DistinctiveNoTo = row["folionoto"]?.ToString() ?? null;
                        model.ApplicationId = row["application_id"]?.ToString() ?? null;
                        model.InterestType = row["inttype"]?.ToString() ?? null;
                        model.InterestRate = row["interest_rate"] != DBNull.Value ? Convert.ToDecimal(row["interest_rate"]) : 0;
                        model.Period = row["period"]?.ToString() ?? null;
                        model.IssueDate = row["allotment_date"] != DBNull.Value ? Convert.ToDateTime(row["allotment_date"]).ToString("dd-MMM-yyyy") : null;
                        model.MaturityDate = row["maturity_date"] != DBNull.Value ? Convert.ToDateTime(row["maturity_date"]).ToString("dd-MMM-yyyy") : null;
                        model.ClosingDate = row["closed_dt"] != DBNull.Value ? Convert.ToDateTime(row["closed_dt"]).ToString("dd-MMM-yyyy") : null;
                        model.UpdateDate = row["n15updatedate"] != DBNull.Value ? Convert.ToDateTime(row["n15updatedate"]).ToString("dd-MMM-yyyy") : null;
                        model.PanTrackID = row["pan_trackid"]?.ToString() ?? null;
                        model.Amount = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0;
                        model.PanNumber = row["pancard_no"]?.ToString() ?? null;
                        model.Status = row["n15status"]?.ToString() ?? null;
                        model.TaxPayee = row["tax_payee"]?.ToString() ?? null;
                        model.ShortRecoveryMarked = row["tot_reco_amt"]?.ToString() ?? null;
                        model.NomineeName = row["nominee_name"]?.ToString() ?? null;
                        model.NomineeRelation = row["relation"]?.ToString() ?? null;
                        model.NomineeAddress = row["nominee_address"]?.ToString() ?? null;
                        model.AccountType = row["ac_type"]?.ToString() ?? null;
                        model.NumberOfApplicants = row["no_applicant"] != DBNull.Value ? Convert.ToInt32(row["no_applicant"]) : 0;
                        model.MaturityAmount = row["maturity_amount"] != DBNull.Value ? Convert.ToDecimal(row["maturity_amount"]) : 0;
                        model.CurrentDate = row["getdt"]?.ToString() ?? null;
                        model.Branch_tel = row["branch_tel"]?.ToString() ?? null;
                        model.acstatus = row["status"]?.ToString() ?? null;

                    }

                    if (adrs.Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[1].Rows[0];
                        model.Address = row["address"]?.ToString() ?? null;
                    }



                    if (jointaplcnt.Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[2].Rows[0];
                        model.JointApplicants = jointaplcnt; // Assign the table directly to the model
                    }

                    if (bank_dt.Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[3].Rows[0];
                        model.Bankdetails = bank_dt; // Assign the table directly to the model
                    }

                    if (transfer_dt.Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[4].Rows[0];
                        model.Transferdetails = transfer_dt; // Assign the table directly to the model
                    }
                }

                // Fill data for query_id = 10 (Transaction Details)

                SqlParameter[] pr2 = new SqlParameter[2];
                pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr2[0].Value = 10;
                pr2[1] = new SqlParameter("@bond_id", SqlDbType.VarChar, 50);
                pr2[1].Value = Session["bond_id"];



                DataSet transactionDataSet = dbconnect.ExecuteDataset("[dbo].[BOND_report]", pr2);
                if (transactionDataSet != null && transactionDataSet.Tables.Count > 0)
                {
                    model.Transactiondetails = transactionDataSet.Tables[0];
                }


                return View(model); // Return the populated model to the view
            }
        }
    }

