using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.personal_loan_models
{
    public class pl_statement_model
    {

        public string CustomerName { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        public string PostOffice { get; set; }
        public string Landmark { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string PinCode { get; set; }
        public string ResidenceNo { get; set; }
        public string OfficeNo { get; set; }
        public string MobileNo { get; set; }
        public DateTime LoanDate { get; set; }
        public string LoanStatus { get; set; }
        public int DueDays { get; set; }
        public DateTime MaturityDate { get; set; }
        public string InstallFrequency { get; set; }
        public decimal SmaNpaDues { get; set; }
        public DateTime FirstInstallDate { get; set; }
        public DateTime LastInstallDate { get; set; }
        public decimal AgreementValue { get; set; }
        public decimal FinancialCharges { get; set; }
        public decimal FinanceAmount { get; set; }
        public string CollectionBookNo { get; set; }
    }

}






