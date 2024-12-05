using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.personal_loan_models
{
    public class personal_loan_create_model
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }        
    }
    public class Pl_application_data
    {
        public string CustomerId { get; set; }
        public float MaxLoanAmt { get; set; }
        public float RequestAmt { get; set; }
        public float IntRate { get; set; }
        public string MaturityDate { get; set; }
        public string LoanNo { get; set; }
    }
    public class DownloadAgreementData
    {
        public string application_id { get; set; }
        public string Name { get; set; }
        public string GlLoanNo { get; set; }
        public float MaxLoanAmt { get; set; }
        public float RequestAmt { get; set; }
        public string otp { get; set; }
        public string otp_status { get; set; }
        public string MobNo { get; set; }

    }

    public class SubmitToCPC
    {
        public string AppId { get; set; }       
        public string paymode { get; set; }       
        public string gl_loanNumber { get; set; }       
        public HttpPostedFileBase Doc_DPN { get; set; }
        public HttpPostedFileBase Doc_agreement { get; set; }
        public HttpPostedFileBase Doc_application { get; set; }

    }

    public class DisbursementData
    {
        public string appId { get; set; }
        public string loanAmt { get; set; }
        public string disAmt { get; set; }
        public string bankId { get; set; }
        public string paymode { get; set; }
        public string paymenttype { get; set; }
        public string actionType { get; set; }    
        public string custAccntNumb { get; set; }    
        public string ifsc { get; set; }                          
        public string custName { get; set; }                          
                       
    }

    public class Selltement_Req
    {
        public string LNo { get; set; }
        public float outstand { get; set; }
        public string susp { get; set; }
        public float Settle_int { get; set; }
        public float f_chg { get; set; }
        public float o_chg { get; set; }

        public float discount { get; set; }
        public float settle_amt { get; set; }
        public int paymode { get; set; }
        public string actionType { get; set; }      

    }
}