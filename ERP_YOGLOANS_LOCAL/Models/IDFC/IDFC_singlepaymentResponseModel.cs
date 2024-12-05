public class MetaData
{
    public string Status { get; set; }
    public string Message { get; set; }
    public string Version { get; set; }
    public string Time { get; set; }
}

public class ResourceData
{
    public string Status { get; set; }
    public string TransactionID { get; set; }
    public string TransactionReferenceNo { get; set; }
    public string BeneficiaryName { get; set; }
}

public class InitiateAuthGenericFundTransferAPIResp
{
    public MetaData MetaData { get; set; }
    public ResourceData ResourceData { get; set; }
}

public class IDFC_singlepaymentResponseModel
{
    public InitiateAuthGenericFundTransferAPIResp InitiateAuthGenericFundTransferAPIResp { get; set; }
}