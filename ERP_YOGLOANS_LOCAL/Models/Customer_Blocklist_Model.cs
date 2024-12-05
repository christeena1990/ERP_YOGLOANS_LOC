using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models
{
    public class Customer_Blocklist_Model
    {
        public int SlNo { get; set; }
        public string Type { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string RequestBy { get; set; }
        public DateTime? RequestDt { get; set; }
        // Keep as string, formatted in SQL
    }

    public class CustomerBlockMergeViewModel
    {
        public IEnumerable<Customer_Blocklist_Model> BlockList { get; set; }
        public IEnumerable<Customer_Blocklist_Model> MergeList { get; set; }
        public List<ColumnInfo> ColumnInfo { get; set; }
        public List<Region> Regions { get; set; } // Add this property

    }

    public class ColumnInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}