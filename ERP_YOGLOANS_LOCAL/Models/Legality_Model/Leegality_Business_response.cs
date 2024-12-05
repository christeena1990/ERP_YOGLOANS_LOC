//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace ERP_YOGLOANS_LOCAL.Models.Legality_Model
//{
//    public class Leegality_Business_response
//    {
//    }
//}



public class Leegality_Business_response
{
    public Data data { get; set; }
    public Message[] messages { get; set; }
    public int status { get; set; }
}

public class Data
{
    public string documentId { get; set; }
    public object irn { get; set; }
    public Invitee1[] invitees { get; set; }
}

public class Invitee1
{
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string signUrl { get; set; }
    public bool active { get; set; }
    public string expiryDate { get; set; }
}

public class Message
{
    public string code { get; set; }
    public string message { get; set; }
}
