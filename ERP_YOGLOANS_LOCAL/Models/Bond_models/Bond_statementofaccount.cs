using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.Bond_models
{
 
    public class StatementOfAccountModel
    {
        // Header Section
        public string CurrentDate { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }

        //// Company Details
        //public string CompanyName { get; set; } = "YOGAKSHEMAM LOANS LTD.";
        //public string CompanyAddress { get; set; } = "Regd. Office, 3rd Floor, Ottappath Tower, Aswini Junction, P.O. Thiruvambady, Thrissur, Kerala, Pin - 680 022, Tel: 0487-2320102";

        // Customer Details
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string PanNumber { get; set; }
        public string Status { get; set; }
        public string acstatus { get; set; } 
        public string Address { get; set; }
        public string NomineeName { get; set; }
        public string NomineeRelation { get; set; }
        public string NomineeAddress { get; set; }

        // Account Details
        public string ApplicationId { get; set; }
        public string IssueNumber { get; set; }
        public string CertificateNo { get; set; }
        public string DistinctiveNoFrom { get; set; }
        public string DistinctiveNoTo { get; set; }
        public string InterestType { get; set; }
        public string Period { get; set; }
        public decimal InterestRate { get; set; }
        public string IssueDate { get; set; }
        public string MaturityDate { get; set; }
        public decimal Amount { get; set; }
        public decimal MaturityAmount { get; set; }
        public string TaxPayee { get; set; }
        public string ShortRecoveryMarked { get; set; }
        public string ClosingDate { get; set; }
        public string AccountType { get; set; }
        public int NumberOfApplicants { get; set; }
        public string DpId { get; set; }
        public string DpName { get; set; }
        public string IsinNumber { get; set; }
        public string PanTrackID { get; set; }
        public string UpdateDate { get; set; }
        public string Branch_tel { get; set; }

        // Add a DataTable for joint applicants
        public DataTable JointApplicants { get; set; }
        public DataTable Bankdetails { get; set; }
        public DataTable Transferdetails { get; set; }
        public DataTable Transactiondetails { get; set; }

        
    }
}
