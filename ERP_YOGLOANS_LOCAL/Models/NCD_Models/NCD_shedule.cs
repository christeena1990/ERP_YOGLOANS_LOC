using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.NCD_Models
{
    public class NCD_shedule
    {
        public string scheme_type { get; set; }
        public string toDate { get; set; }
        public int issue_no { get; set; }
    }

    public class NCD_Interest_release
    {
        public string deb_id { get; set; }
        public string Comments { get; set; }        
    }

    public class NCD_Interest_Block : NCD_Interest_release
    {
        public HttpPostedFileBase Document { get; set; }
    }



}