using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.Legality_Model
{
    public class Annex_model
    {
        // Define properties to match the data from your stored procedure
        public string ApplicationId { get; set; }
        public string LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public string DisburseSchedule { get; set; }
        public string Duration { get; set; }
        public string Instalment { get; set; }
        public string NumberOfEPIs { get; set; }
        public string EPI { get; set; }
        public string CommencementOfRepayment { get; set; }
        public string InterestRateType { get; set; }
        public string RefBenchmark { get; set; }
        public string BenchmarkRate { get; set; }
        public string Spread { get; set; }
        public string FinalRate { get; set; }
        public string ResetPeriodB { get; set; }
        public string ResetPeriodS { get; set; }
        public string BenchEPI { get; set; }
        public string BenchNoEPI { get; set; }

        public string RE_A_Processing_recurring { get; set; }
        public string RE_A_Processing_amount { get; set; }
        public string RE_B_Processing_recurring { get; set; }
        public string RE_B_Processing_amount { get; set; }

        public string RE_A_Insurance_recurring { get; set; }
        public string RE_A_Insurance_amount { get; set; }
        public string RE_B_Insurance_recurring { get; set; }
        public string RE_B_Insurance_amount { get; set; }

        public string RE_A_fee_recurring { get; set; }
        public string RE_A_fee_amount { get; set; }
        public string RE_B_fee_recurring { get; set; }
        public string RE_B_fee_amount { get; set; }

        public string RE_A_other_recurring { get; set; }
        public string RE_A_other_amount { get; set; }
        public string RE_B_other_recurring { get; set; }
        public string RE_B_other_amount { get; set; }

        public string APR { get; set; }
        public decimal Penal_charge { get; set; }
        public decimal OtherPenalCharge { get; set; }
        public decimal ForeclosureCharge { get; set; }
        public decimal SwitchingCharge { get; set; }
        public decimal OtherCharge { get; set; }



        public string ClauseAgent { get; set; }
        public string ClauseRedressalMechanism { get; set; }
        public string PhoneEmailNodal { get; set; }
        public string TransferToRE { get; set; }
        public string NameOfOriginatingRE { get; set; }
        public string NameOfPartnerRE { get; set; }
        public string BlendedRateOfInterest { get; set; }
        public string LookUpPeriod { get; set; }
        public string DetailsOfLSP { get; set; }

        //-----------------------Annex-B--------------------

        public string Tenure { get; set; }
        public string no_of_instalment { get; set; }
        public string EPI_amount { get; set; }
        public string no_of_instalment_int { get; set; }
        public string Commencement_of_repayments { get; set; }
        public string Total_Interest_Amount { get; set; }
        public string Fee_Charges { get; set; }
        public string Payable_to_the_RE { get; set; }
        public string Payable_to_third_party_routed_through_RE { get; set; }
        public string Annual_Percentage_rate { get; set; }
        public string Schedule_of_disbursement { get; set; }
        public string Due_date { get; set; }
        public string Net_disbursed_amount { get; set; }
        public string Total_amount_to_be_paid { get; set; }
        public decimal loanAmount { get; set; }
        public decimal Rate_of_Interest { get; set; }
        public string InterestRateTypeB { get; set; }
    }

    public class PdfRequest
    {
        public HttpPostedFileBase pdfFile { get; set; }
        public string ApplicationId { get; set; }
    }
}