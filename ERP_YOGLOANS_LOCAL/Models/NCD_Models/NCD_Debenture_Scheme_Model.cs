using ERP_YOGLOANS_LOCAL.Models.Equifax_Model;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ERP_YOGLOANS_LOCAL.Models.NCD_Models.NCD_Debenture_Scheme_Model;

namespace ERP_YOGLOANS_LOCAL.Models.NCD_Models
{
    public class NCD_Debenture_Scheme_Model
    {
        public List<string> Columns { get; set; }
        public List<string> Column { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
        public List<Bank> Banks { get; set; } = new List<Bank>();
        public class Bank
        {
            public string SubAccNo { get; set; }
            public string SubName { get; set; }
        }

        public class NCDDetails
        {
            public float FaceValue { get; set; }
            public int No_of_deb { get; set; }
            public string TotalDebAmount { get; set; }
            public float MinAmount { get; set; }
            public int LockingPeriod { get; set; }
            public string MeetDate { get; set; }
            public string RocDate { get; set; }
            public string TrustDate { get; set; }
            public string SeniorCitizenInterest { get; set; }
            public string CanvasserCommission { get; set; }
            public float aggregate_amount_from { get; set; }
            public float aggregate_amount_to { get; set; }
            public string ppo_date { get; set; }
            //-----------------------Bond----------------------------------
            public string TotalBondAmount { get; set; }
            public float FaceValue_Bond { get; set; }
            public int No_of_bond { get; set; }
            public float MinAmount_Bond { get; set; }
            public int LockingPeriod_Bond { get; set; }
            public string SeniorCitizenInterest_Bond { get; set; }
            public string MeetDate_Bond { get; set; }
            public string RocDate_bond { get; set; }
            public string TrustDate_Bond { get; set; }
            public string CanvasserCommission_Bond { get; set; }
            //-----------------------Cheque Presented----------------------------------
            public string branch_name { get; set; }
            public string application_id { get; set; }
            public string bank_name { get; set; }
            public string appmoney_no { get; set; }
            public string customer_name { get; set; }
            public string customer_id { get; set; }
            public string deb_amount { get; set; }
            // public int No_of_deb { get; set; }
            public int track_id { get; set; }
            public string reference_no { get; set; }
            public string amount { get; set; }
            public string sub_name { get; set; }
            public string cheque_date { get; set; }
        }

        public class CanvasserModel
        {
            public string AmountRange { get; set; }
            public decimal Commission { get; set; }
            public string AmountRange_Bond { get; set; }
            public decimal Commission_Bond { get; set; }
        }

        public class PrematureModel
        {
            public string Years { get; set; }
            public decimal Interest { get; set; }
        }

        public class TrusteeModel
        {
            public string TrusteeName { get; set; }
            public string TrusteePAN { get; set; }
        }

        //-----------------------Bond----------------------------------
        public int TotalBondAmount { get; set; }
        public float Total_AmountBond { get; set; }

        public decimal InterestRate_Mon { get; set; } 
        public decimal InterestRate_Cum { get; set; } 
        //---------------------------------------------------------


        public int IssueNo { get; set; } // Assuming issue_no is a bigint
        public string Duration { get; set; } // 'X-Months' format
        public string InterestType { get; set; } // 'Cumulative' or 'Monthly'
        public decimal InterestRate { get; set; } // 'X %' format
        public int TotalDebAmount { get; set; } // Assuming total_deb_amount is a decimal
        public DateTime RegStartDate { get; set; } // 'DD/MM/YYYY' format
        public DateTime RegEndDate { get; set; } // 'DD/MM/YYYY' format


        public long StartDate { get; set; } // Assuming you will format date appropriately
        public long EndDate { get; set; }
        public int Period { get; set; }
        public char InterestTypeChar { get; set; }
        public float InterestRateValue { get; set; }
        public float FaceValue { get; set; }
        public int TotalDeb { get; set; }
        public float MinAmount { get; set; }
        public char PrematureClosing { get; set; }
        public int LockingPeriod { get; set; }
        public float ScInterest { get; set; }
        public string SeniorCitizenInterest { get; set; }
        public string MeetDate { get; set; }
        public string RocDate { get; set; }
        public string TrustDate { get; set; }
        public string CanvasserCommission { get; set; }
        public long EnterBy { get; set; }
        public string CanData { get; set; }
        public string PreData { get; set; }
        public string OutMessage { get; set; } // Assuming this is used for output messages
        public string PeriodType { get; set; }
        public string TrusteeName { get; set; }
        public string TrusteePanNo { get; set; }
        public string Trustee { get; set; }
        public NCD_Debenture_Scheme_Model() // Constructor
        {
            Data = new List<Dictionary<string, object>>(); // Initialize the Data list
          
        }

    }

    public class ChequeRealizationViewModel
    {
        public List<ChequeRealizationViewModel> Data { get; set; } = new List<ChequeRealizationViewModel>();
        public string BranchName { get; set; }
        public string ApplicationId { get; set; }
        public string AppMoneyNo { get; set; }
        public string CustomerName { get; set; }
        public string BankName { get; set; }
        public string IFSCCode { get; set; }
        public decimal DebAmount { get; set; }
        public int TrackId { get; set; }
        public string ChequeNumber { get; set; }
        public decimal Amount { get; set; }
        public string SubName { get; set; }
        public DateTime ChequeDate { get; set; }
        public DateTime ClearingDate { get; set; }

        //-----------------------Bond----------------------------------

        public string BranchName_Bond { get; set; }
        public string ApplicationId_Bond { get; set; }
        public string AppMoneyNo_Bond { get; set; }
        public string CustomerName_Bond { get; set; }
        public string BankName_Bond { get; set; }
        public string IFSCCode_Bond { get; set; }
        public decimal DebAmount_Bond { get; set; }
        public int TrackId_Bond { get; set; }
        public string ChequeNumber_Bond { get; set; }
        public decimal Amount_Bond { get; set; }
        public string SubName_Bond { get; set; }
        public DateTime ChequeDate_Bond { get; set; }
        public DateTime ClearingDate_Bond { get; set; }




    }
    // Define a model to bind the incoming data
    public class ApprovalData
    {
        public int trackId { get; set; }
        public string applicationId { get; set; }
        public string clearingDate { get; set; }
        //-----------------------Bond----------------------------------
        public int trackId_Bond { get; set; }
        public string applicationId_Bond { get; set; }
        public string clearingDate_Bond { get; set; }
    }



    public class SchemeRegistrationModel
    {
   
            public string issue_no { get; set; }
            public string interest_type { get; set; }
            public string total_fund { get; set; }
            public string period { get; set; }
            public string face_value { get; set; }
            public string issue_start { get; set; }
            public string issue_end { get; set; }
            public string min_amount { get; set; }
            public string number_of_debentures { get; set; }
            public string locking_period { get; set; }
            public string interest_rate { get; set; }
            public string senior_citizen { get; set; }
            public string senior_citizen_interest { get; set; }
            public string canvasser_commission { get; set; }
        public string canvasser_commission_data { get; set; }
        public string premature_closing { get; set; }
        public string premature_closing_data { get; set; }

        public DateTime meet_date { get; set; }
            public DateTime roc_date { get; set; }
            public DateTime trust_date { get; set; }
            public string trusteeName {get; set; }
            public string trusteePan { get; set; }

        public float agg_amt_from { get; set; }
        public float agg_amt_to { get; set; }
        public DateTime ppo_date { get; set; }

    }

    public class ncd_bank_add_model
    {
        public string ImageUrl1 { get; internal set; }
    }
    public class ncd_branch_approval_model
    {
        public string ImageUrl1 { get; internal set; }
        public string ImageUrl2 { get; internal set; }
        public string ImageUrl3 { get; internal set; }
    }
    public class NCD_Interest
    {
        public string issue_no { get; set; }
        public string amount { get; set; }
        //public string min_amount { get; set; }
        //public string balance_fund { get; set; }
        //public string face_value { get; set; }

    }

    public class Submit_NCD_Application
    { 
        public string issue_no { get; set; }
        public string transfer_ID { get; set; }
        
        public string amount { get; set; }
        public string matamt { get; set; }
        public string totint { get; set; }
        public string matdate { get; set; }
        public string nod { get; set; }
        public string canvas_code { get; set; }
        public string renewal_amt { get; set; }
        public string nofapplicant { get; set; }
        public string ac_type { get; set; }
        public string tax_payee { get; set; }
        public string lienholder { get; set; }
        public string lien { get; set; }
        public string rpay { get; set; }
        public string _15_status { get; set; }
        public string _15_type { get; set; }
        public string dp_id { get; set; }
        public string dp_name { get; set; }
        public string pancard { get; set; }
        public string nominee { get; set; }
        public string nominee_add { get; set; }
        public string relation { get; set; }
        public string canvas_type { get; set; }
        public HttpPostedFileBase Document { get; set; }


    }
  


}