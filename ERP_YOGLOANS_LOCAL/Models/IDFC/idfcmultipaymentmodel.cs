using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for idfcmultipaymentmodel
/// </summary>
public class idfcmultipaymentmodel
{

    public class Rootobject
    {
        public Domultipaymentcorpreq doMultiPaymentCorpReq { get; set; }
    }

    public class Domultipaymentcorpreq
    {
        public Header Header { get; set; }
        public Body Body { get; set; }
    }

    public class Header
    {
        public string Maker_ID { get; set; }
        public string Checker_ID { get; set; }
        public string Approver_ID { get; set; }
    }

    public class Body
    {
        public List<Payment> Payment { get; set; }
    }

    public class Payment
    {
        public string RefNo { get; set; }
        public decimal Amount { get; set; }
        public string Debit_Acct_No { get; set; }
        public string Debit_Acct_Name { get; set; }
        public string Debit_Mobile { get; set; }
        public string Ben_IFSC { get; set; }
        public string Ben_Acct_No { get; set; }
        public string Ben_Name { get; set; }
        public string Ben_BankName { get; set; }
        public string Ben_Email { get; set; }
        public string Ben_Mobile { get; set; }
        public string Mode_of_Pay { get; set; }
        public string Nature_of_Pay { get; set; }
        public string Remarks { get; set; }
    }

}
