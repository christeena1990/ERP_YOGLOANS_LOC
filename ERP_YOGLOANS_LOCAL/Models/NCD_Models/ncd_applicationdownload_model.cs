using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.NCD_Models
{
    public class CombinedViewModel
    {
        public DataFillViewModel DataFill { get; set; }
        public TableModel TableData { get; set; }
       
    }
    public class DataFillViewModel
    {

        public string ApplicationId { get; set; }
        public string Date { get; set; }
        public string Dob { get; set; }
        public string CustomerId { get; set; }
        public string FirstApplicant { get; set; }
        public string SecondApplicant { get; set; }
        public string FatherName { get; set; }
        public string Email { get; set; }
        
        public string Age { get; set; }
        public string MobileNo { get; set; }
        public string Contact { get; set; }
        public string KycDocName { get; set; }
        public string KycNo { get; set; }
        public string[] PanCharacters { get; set; }
        public string Duration { get; set; }
        public string DurationType { get; set; }
        public string InterestRate { get; set; }
        public bool InterestTypeFixed { get; set; }
        public bool InterestTypeCompound { get; set; }
        public string Category { get; set; }
        public string Place { get; set; }
        public string DeclarationDate { get; set; }
        public string Declaration { get; set; }
        public string Address { get; set; }
        public string DebitAmount { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public string ChequeBank { get; set; }
        public string DpId { get; set; }
        public string DpName { get; set; }
        public string Base64Photo { get; set; }
        public bool TaxYes { get; set; }
        public bool TaxNo { get; set; }
        public string Base64CoApplicantPhoto { get; set; }
        //public DataTable GridDatacheque { get; set; }



        // Properties for Grid Data
        public List<string> GridDatachequeColumns { get; set; }  // Column names
        public List<Dictionary<string, object>> GridDatacheque { get; set; } // Row data

        public DataFillViewModel()
        {
            GridDatachequeColumns = new List<string>();
            GridDatacheque = new List<Dictionary<string, object>>();
        }
    }
        


        public class TableModel
    {
        public List<TableRowModel> Body1Data { get; set; }
        public List<TableRowModel> Body2Data { get; set; }
        //public string Body3Data { get; set; }
        public List<TableRowModel> Body3Data { get; set; }
        public List<TableRowModel> Body4Data { get; set; }
        public DataTable GridData { get; set; }
    }



public class TableRowModel
    {
        public int RuleNo { get; set; }
        public string RuleName { get; set; }
        public string Data { get; set; }
    }


    public class NCD_Oversubscriptionmodel
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Place { get; set; }
        public string Post { get; set; }
        public string AggrAmtFrom { get; set; }
        public string AggrAmtTo { get; set; }
        public string AggrAmtTo1000 { get; set; } // Converted to char array
        public string AggrAmtFrom1000 { get; set; }
        public string IssueNo { get; set; }
        public string Date { get; set; }
    }






}