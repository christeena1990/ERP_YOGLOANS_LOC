using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.Legality_Model
{
    public class Leegality_Business_request
    {
        public string profileId { get; set; }
        public DocumentFile file { get; set; }
        public Invitee[] invitees { get; set; }
        public string irn { get; set; }
    }

    public class DocumentFile
    {
        public string name { get; set; }
        public string file { get; set; }
    } 

    public class Invitee
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
}



