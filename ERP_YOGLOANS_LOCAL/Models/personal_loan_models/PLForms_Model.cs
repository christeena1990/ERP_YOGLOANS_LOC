using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.personal_loan_models
{
    public class PLForms_Model
    {
        public class LoanApplicationViewModel
        {
            public string ApplctnId { get; set; }
            public string BranchName { get; set; }
            public int BranchCode { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public string FathersName { get; set; }
            public string SpousesName { get; set; }
            public string Address { get; set; }
            public string MobileNo { get; set; }
            public string EmailId { get; set; }
            public string PAN { get; set; }
            public string GLLoanNo { get; set; }
            public string InventoryId { get; set; }
            public decimal NetWeight { get; set; }
            public decimal GLLoanAmount { get; set; }
            public decimal PLLoanAmount { get; set; }
            public string Purpose { get; set; }
            public int Duration { get; set; }
            public string RepaymentMode { get; set; }
            public string Date { get; set; }
            public string GLdate { get; set; }
            public string GLMaturitydate { get; set; }
            public decimal GLoutstanding { get; set; }
            public decimal TotalLoan { get; set; }
            public decimal PLinterestRate { get; set; }
            public string PLMaturitydate { get; set; }
            public int Duration2 { get; set; }
            public string Total_ltv { get; set; }
            public string GL_ltv { get; set; }

            public string BankName { get; set; }
            public string Payeename { get; set; }
            public string AcctNo { get; set; }
            public string IFSC { get; set; }

            public string BranchAddress { get; set; }
            public string ApplctnDate { get; set; }

            //------------------------Disbursement Memo--------------------------------

            public string BankBranch { get; set; }
            public decimal LoanAmountProposed { get; set; }
            public string receivables { get; set; }
            public decimal TransferAmount { get; set; }
            public decimal DisbursementAmount { get; set; }
            public string PenalCharges { get; set; }

            public string Username { get; set; }
            public string usercode { get; set; }
            public string userDesignation { get; set; }
            public string UserBranchName { get; set; }
            public string UserDate { get; set; }


            public string HOUsername { get; set; }
            public string HOusercode { get; set; }
            public string HOuserDesignation { get; set; }
            public string HOUserBranchName { get; set; }
            public string HOUserDate { get; set; }






        }



    }
}