using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Configuration;
using ERP_YOGLOANS_LOCAL.Models.Equifax_Model;
using ERP_YOGLOANS_LOCAL.Models.personal_loan_models;
using System.Web.Services.Description;
using System.Numerics;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers
{
    public class UPI_LinkGenerateController : Controller
    {
        // GET: UPI_LinkGenerate

        DB dbconnect = new DB();
        public ActionResult UPI_LinkGenerate_View()
        {
            var EMI_RecordList = EMI_RecordList_Grid();

            // Check if DataTable has rows and columns
            if (EMI_RecordList != null && EMI_RecordList.Rows.Count > 0)
            {
                Console.WriteLine("DataTable Rows: " + EMI_RecordList.Rows.Count);
                Console.WriteLine("DataTable Columns: " + EMI_RecordList.Columns.Count);
            }
            else
            {
                Console.WriteLine("No data found in DataTable");
            }

            ViewBag.EMI_RecordList = EMI_RecordList;
            return View();

        }
        private DataTable EMI_RecordList_Grid()
        {
            SqlParameter[] pr = new SqlParameter[1];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[UPI_link_Transactions]", pr).Tables[0];
            dbconnect.Close();
            return dt;
        }
        [HttpPost]


        //public ActionResult SubmitSelectedRows(string selectedRows, string selectedData)
        //{
        //    if (string.IsNullOrEmpty(selectedData))
        //    {
        //        return RedirectToAction("UPI_LinkGenerate_View");
        //    }

        //    // Deserialize the selected data
        //    var selectedRowData = JsonConvert.DeserializeObject<List<YourDetailsType>>(selectedData);

        //    // Create a DataTable to hold the details
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("LoanNo", typeof(string));

        //    dt.Columns.Add("CustomerID", typeof(int));
        //    dt.Columns.Add("Customer", typeof(string));
        //    dt.Columns.Add("Mobile", typeof(string));
        //    dt.Columns.Add("EMI", typeof(float));
        //    dt.Columns.Add("Installment", typeof(int));

        //    // Populate the DataTable with the selected row data
        //    foreach (var item in selectedRowData)
        //    {
        //        dt.Rows.Add(item.LoanNo, item.CustomerID, item.Customer, item.Mobile, item.EMI, item.Installment);
        //    }
        //    SqlParameter[] pr = new SqlParameter[3];

        //    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
        //    pr[0].Value = 2;

        //    pr[1] = new SqlParameter("@UPIRecordList", SqlDbType.Structured);
        //    pr[1].Value = dt;

        //    pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
        //    pr[2].Direction = ParameterDirection.Output;


        //    // Execute the stored procedure
        //    dbconnect.Open();
        //    dbconnect.ExecuteStoredProcedure("[dbo].[UPI_link_Transactions]", pr);
        //    dbconnect.Close();





        //    return RedirectToAction("UPI_LinkGenerate_View");
        //}

        public ActionResult SubmitSelectedRows(string selectedRows, string selectedData)
        {
            if (string.IsNullOrEmpty(selectedData))
            {
                return RedirectToAction("UPI_LinkGenerate_View");
            }

            // Deserialize the selected data
            var selectedRowData = JsonConvert.DeserializeObject<List<YourDetailsType>>(selectedData);

            // Create a DataTable to hold the details
            DataTable dt = new DataTable();
            dt.Columns.Add("loan_no", typeof(string));       // Match with your user-defined table type
            dt.Columns.Add("customer_id", typeof(long));     // Use long for BIGINT
            dt.Columns.Add("customer_name", typeof(string)); // Match with your SQL procedure
            dt.Columns.Add("cust_mob", typeof(string));      // Match with your SQL procedure
            dt.Columns.Add("emi_amt", typeof(float));        // Match with your SQL procedure
            dt.Columns.Add("inst_no", typeof(int));          // Match with your SQL procedure

            // Populate the DataTable with the selected row data
            foreach (var item in selectedRowData)
            {
                dt.Rows.Add(item.LoanNo, item.CustomerID, item.Customer, item.Mobile, item.EMI, item.Installment);
            }

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@UPIRecordList", SqlDbType.Structured);
            pr[1].Value = dt;

            pr[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr[2].Direction = ParameterDirection.Output;

            // Execute the stored procedure
            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[UPI_link_Transactions]", pr);
            dbconnect.Close();

            // Check the output message
            //var outMessage = (string)pr[2].Value;
            //if (outMessage != null)
            //{
            //    // Log or handle the output message if necessary
            //}

            return RedirectToAction("UPI_LinkGenerate_View");
        }



        // Define the class to hold the details
        private class YourDetailsType
        {
            public string LoanNo { get; set; }

            public int CustomerID { get; set; }
            public string Customer { get; set; }
            public string Mobile { get; set; }
            public float EMI { get; set; }
            public string Installment { get; set; }


        }









    }
}
