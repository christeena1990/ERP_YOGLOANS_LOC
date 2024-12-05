using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.Bond_models
{
    public class bond_scheme_model
    {

            public string issue_no { get; set; }
            public string interest_type { get; set; }
            public string total_fund { get; set; }
            public string period { get; set; }
            public string face_value { get; set; }
            public string issue_start { get; set; }
            public string issue_end { get; set; }
            public string min_amount { get; set; }
            public string number_of_debentures { get; set; }
            public string locking_period { get; set; }
            public string interest_rate_fixed { get; set; }
        public string interest_rate_cmltv { get; set; }
        public string senior_citizen { get; set; }
            public string senior_citizen_interest { get; set; }
            public string canvasser_commission { get; set; }
            public string canvasser_commission_data { get; set; }
            public DateTime meet_date { get; set; }
            public DateTime roc_date { get; set; }
            public DateTime trust_date { get; set; }
           }

    public class Bond_Interest
    {
        public string issue_no { get; set; }
        public string amount { get; set; }
        public string int_type { get; set; }


    }

    public class Submit_BOND_Application
    {
        public string issue_no { get; set; }
        public string amount { get; set; }
        public string matamt { get; set; }
        public string totint { get; set; }
        public string matdate { get; set; }
        public string nod { get; set; }
        public string canvas_code { get; set; }
        public string renewal_amt { get; set; }
        public string nofapplicant { get; set; }
        public string ac_type { get; set; }
        public string tax_payee { get; set; }
        public string lienholder { get; set; }
        public string lien { get; set; }
        public string rpay { get; set; }
        public string _15_status { get; set; }
        public string _15_type { get; set; }
        public string dp_id { get; set; }
        public string dp_name { get; set; }
        public string pancard { get; set; }
        public string nominee { get; set; }
        public string nominee_add { get; set; }
        public string relation { get; set; }
        public string canvas_type { get; set; }
    }
}



    
