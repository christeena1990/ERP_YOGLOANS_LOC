using ERP_YOGLOANS_LOCAL.Models.Legality_Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Drawing.Drawing2D;

namespace ERP_YOGLOANS_LOCAL.Controllers.Legality
{
    public class AnnexController : Controller
    {
        DB dbconnect = new DB();

        // GET: Annex
        public ActionResult Annex()
        {
            Annex_model Model = new Annex_model();

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            pr[1] = new SqlParameter("@module_id", SqlDbType.Int);
            pr[1].Value = 21;

            pr[2] = new SqlParameter("@application_id", SqlDbType.VarChar, 14);
            pr[2].Value = 510001504809;


            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[SP_KFS]", pr);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_annexA = ds.Tables[0];



                    Model.ApplicationId = dt_annexA.Rows[0]["application_id"].ToString();
                    Model.LoanType = dt_annexA.Rows[0]["loan_type"].ToString();
                    decimal loanAmount;
                    if (decimal.TryParse(dt_annexA.Rows[0]["loan_amt"].ToString(), out loanAmount))
                    {
                        Model.LoanAmount = loanAmount;
                    }
                    Model.DisburseSchedule = dt_annexA.Rows[0]["Schedule"].ToString();
                    Model.Duration = dt_annexA.Rows[0]["Duration"].ToString();
                    Model.Instalment = dt_annexA.Rows[0]["Instalment"].ToString();
                    Model.NumberOfEPIs = dt_annexA.Rows[0]["Number_of_EPIs"].ToString();
                    Model.EPI = dt_annexA.Rows[0]["EPI"].ToString();
                    Model.CommencementOfRepayment = dt_annexA.Rows[0]["Commencement_of_repayment"].ToString();
                    Model.InterestRateType = dt_annexA.Rows[0]["Interest_rate_type"].ToString();
                    Model.RefBenchmark = dt_annexA.Rows[0]["refeBenchMark"].ToString();

                    Model.BenchmarkRate = dt_annexA.Rows[0]["benchMarkRate"].ToString();
                    Model.Spread = dt_annexA.Rows[0]["spread"].ToString();
                    Model.FinalRate = dt_annexA.Rows[0]["finalRate"].ToString();
                    Model.ResetPeriodB = dt_annexA.Rows[0]["resetPeriod_B"].ToString();
                    Model.ResetPeriodS = dt_annexA.Rows[0]["resetPeriod_S"].ToString();
                    Model.BenchEPI = dt_annexA.Rows[0]["Bench_EPI"].ToString();
                    Model.BenchNoEPI = dt_annexA.Rows[0]["Bench_no_EPI"].ToString();
                    Model.RE_A_Processing_recurring = dt_annexA.Rows[0]["RE_A_Processing_recurring"].ToString();

                    Model.RE_A_fee_recurring = dt_annexA.Rows[0]["RE_A_Processing_amount"].ToString();
                    Model.RE_A_fee_amount = dt_annexA.Rows[0]["RE_B_Processing_recurring"].ToString();
                    Model.RE_B_fee_recurring = dt_annexA.Rows[0]["RE_B_Processing_amount"].ToString();
                    Model.RE_B_fee_amount = dt_annexA.Rows[0]["RE_A_Insurance_recurring"].ToString();
                    Model.RE_A_other_recurring = dt_annexA.Rows[0]["RE_A_Insurance_amount"].ToString();
                    Model.RE_A_other_amount = dt_annexA.Rows[0]["RE_B_Insurance_recurring"].ToString();
                    Model.RE_B_other_recurring = dt_annexA.Rows[0]["RE_B_Insurance_amount"].ToString();
                    Model.RE_B_other_amount = dt_annexA.Rows[0]["RE_A_fee_recurring"].ToString();

                    Model.APR = dt_annexA.Rows[0]["APR"].ToString();

                    decimal Penal_charge;
                    if (decimal.TryParse(dt_annexA.Rows[0]["Penal_charge"].ToString(), out Penal_charge))
                    {
                        Model.Penal_charge = Penal_charge;
                    }
                    decimal other_penal_charge;
                    if (decimal.TryParse(dt_annexA.Rows[0]["Penal_charge"].ToString(), out other_penal_charge))
                    {
                        Model.OtherPenalCharge = other_penal_charge;
                    }
                    decimal foreclosure_charge;
                    if (decimal.TryParse(dt_annexA.Rows[0]["Penal_charge"].ToString(), out foreclosure_charge))
                    {
                        Model.ForeclosureCharge = foreclosure_charge;
                    }
                    decimal switching_charge;
                    if (decimal.TryParse(dt_annexA.Rows[0]["Penal_charge"].ToString(), out switching_charge))
                    {
                        Model.SwitchingCharge = switching_charge;
                    }
                    decimal other_charge;
                    if (decimal.TryParse(dt_annexA.Rows[0]["Penal_charge"].ToString(), out other_charge))
                    {
                        Model.OtherCharge = other_charge;
                    }
                    Model.ClauseAgent = dt_annexA.Rows[0]["Clause_agent"].ToString();
                    Model.ClauseRedressalMechanism = dt_annexA.Rows[0]["Clause_redressal_mechanism"].ToString();

                    Model.PhoneEmailNodal = dt_annexA.Rows[0]["Phone_email_nodal"].ToString();
                    Model.TransferToRE = dt_annexA.Rows[0]["transfer_to_RE"].ToString();
                    Model.NameOfOriginatingRE = dt_annexA.Rows[0]["Name_of_originating_RE"].ToString();
                    Model.NameOfPartnerRE = dt_annexA.Rows[0]["Name_of_partner_RE"].ToString();
                    Model.BlendedRateOfInterest = dt_annexA.Rows[0]["Blended_rate_of_interest"].ToString();
                    Model.LookUpPeriod = dt_annexA.Rows[0]["look_up_period"].ToString();
                    Model.DetailsOfLSP = dt_annexA.Rows[0]["Details_of_LSP"].ToString();




                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    DataTable dt_annexB = ds.Tables[1];

                    //-----------------------AnnexB---------------


                    decimal loanAmount;
                    if (decimal.TryParse(dt_annexB.Rows[0]["loan_amt"].ToString(), out loanAmount))
                    {
                        Model.LoanAmount = loanAmount;
                    }

                    Model.Tenure = dt_annexB.Rows[0]["Tenure"].ToString();
                    Model.no_of_instalment = dt_annexB.Rows[0]["no_of_instalment"].ToString();
                    Model.EPI_amount = dt_annexB.Rows[0]["EPI_amount"].ToString();
                    Model.no_of_instalment_int = dt_annexB.Rows[0]["no_of_instalment_int"].ToString();
                    Model.Commencement_of_repayments = dt_annexB.Rows[0]["Commencement_of_repayments"].ToString();
                    Model.InterestRateTypeB = dt_annexB.Rows[0]["Interest_rate_type"].ToString();

                    decimal Rate_of_Interest;
                    if (decimal.TryParse(dt_annexB.Rows[0]["loan_amt"].ToString(), out Rate_of_Interest))
                    {
                        Model.Rate_of_Interest = Rate_of_Interest;
                    }

                    Model.Total_Interest_Amount = dt_annexB.Rows[0]["Total_Interest_Amount"].ToString();
                    Model.Fee_Charges = dt_annexB.Rows[0]["Fee_Charges"].ToString();


                    Model.Payable_to_the_RE = dt_annexB.Rows[0]["Payable_to_the_RE"].ToString();
                    Model.Payable_to_third_party_routed_through_RE = dt_annexB.Rows[0]["Payable_to_third_party_routed_through_RE"].ToString();
                    Model.Net_disbursed_amount = dt_annexB.Rows[0]["Net_disbursed_amount"].ToString();
                    Model.Total_amount_to_be_paid = dt_annexB.Rows[0]["Total_amount_to_be_paid"].ToString();
                    Model.Annual_Percentage_rate = dt_annexB.Rows[0]["Annual_Percentage_rate"].ToString();
                    Model.Schedule_of_disbursement = dt_annexB.Rows[0]["Schedule_of_disbursement"].ToString();
                    Model.Due_date = dt_annexB.Rows[0]["Due_date"].ToString();







                }


            }



            return View(Model);
        }
    }
}