public class IDFC_singlepaymentModel
{
    public class Rootobject
    {
        public Initiateauthgenericfundtransferapireq initiateAuthGenericFundTransferAPIReq { get; set; }
    }

    public class Initiateauthgenericfundtransferapireq
    {
        public string transactionID { get; set; }
        public string debitAccountNumber { get; set; }
        public string creditAccountNumber { get; set; }
        public string remitterName { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string transactionType { get; set; }
        public string paymentDescription { get; set; }
        public string beneficiaryIFSC { get; set; }
        public string beneficiaryName { get; set; }
        public string beneficiaryAddress { get; set; }
        public string emailId { get; set; }
        public string mobileNo { get; set; }
        public string brokerCode { get; set; }
        public string messageType { get; set; }
    }
}

