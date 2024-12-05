public class Leegality_docStatusResponse
{
    public Data1 data { get; set; }
    public object[] messages { get; set; }
    public int status { get; set; }
}

public class Data1
{
    public Document1 document { get; set; }
    public Invitation[] invitations { get; set; }
}

public class Document1
{
    public string id { get; set; }
    public string name { get; set; }
    public string irn { get; set; }
    public string status { get; set; }
    public bool coordinatePicker { get; set; }
    public bool deleted { get; set; }
    public string creationDate { get; set; }
    public object completionDate { get; set; }
}

public class Invitation
{
    public string name { get; set; }
    public object email { get; set; }
    public string phone { get; set; }
    public bool enableWhatsApp { get; set; }
    public string inviteeType { get; set; }
    public string invitationUrl { get; set; }
    public string[] allowedSignatures { get; set; }
    public object usedSignatureType { get; set; }
    public Invitationstatus invitationStatus { get; set; }
}

public class Invitationstatus
{
    public bool active { get; set; }
    public bool signed { get; set; }
    public object reviewed { get; set; }
    public object approved { get; set; }
    public bool rejected { get; set; }
    public bool expired { get; set; }
    public string creationDate { get; set; }
    public string expiryDate { get; set; }
    public object signDate { get; set; }
}



