public class IDFC_singlePaymentStatusModel
{

    //public class Rootobject
    //{
    //    public Paymenttransactionstatusreq paymentTransactionStatusReq { get; set; }
    //}

    //public class Paymenttransactionstatusreq
    //{
    //    public string transactionType { get; set; }
    //    public string transactionReferenceNumber { get; set; }
    //    public string paymentReferenceNumber { get; set; }
    //    public string transactionDate { get; set; }
    //}


    public class Rootobject
    {
        public Paymenttransactionstatusreq paymentTransactionStatusReq { get; set; }
    }

    public class Paymenttransactionstatusreq
    {
        public string tellerBranch { get; set; }
        public string tellerID { get; set; }
        public string transactionType { get; set; }
        public string transactionReferenceNumber { get; set; }
        public string paymentReferenceNumber { get; set; }
        public string transactionDate { get; set; }
    }


}