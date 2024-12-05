using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class print_voucher_generalController : Controller
    {
        DB dbconnect = new DB();

        public ActionResult print_voucher_general_blank_view()
        {
            return View();
        }
      
       

        [HttpGet]
        public ActionResult FT_voucher1_select()
        {
        if (Session["branch_id"] == null || Session["voucher_part0"] == null)
        {
            return Json(new { success = false, message = "Session data is missing." }, JsonRequestBehavior.AllowGet);
        }

        SqlParameter[] qr = new SqlParameter[2];
        qr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 4 };
        qr[1] = new SqlParameter("@branchid", SqlDbType.BigInt) { Value = Session["branch_id"] };
        dbconnect.Open();
        DataSet ds1223 = dbconnect.ExecuteDataset("[dbo].[FT_voucher1]", qr);
        dbconnect.Close();  
        string CIN = string.Empty;
        string GSTIN = string.Empty;

        if (ds1223.Tables.Count > 0)
        {
            if (ds1223.Tables[0].Rows.Count > 0)
            {
                // Assuming CIN is in the first table
                CIN = ds1223.Tables[0].Rows[0][0].ToString(); // Check the correct column index
            }
            else
            {
                return Json(new { success = false, message = "No data in CIN table." }, JsonRequestBehavior.AllowGet);
            }

            if (ds1223.Tables.Count > 1 && ds1223.Tables[1].Rows.Count > 0)
            {
                // Assuming GSTIN is in the second table
                GSTIN = ds1223.Tables[1].Rows[0][0].ToString(); // Check the correct column index
            }
            else
            {
                return Json(new { success = false, message = "No data in GSTIN table." }, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            return Json(new { success = false, message = "No tables returned in the dataset." }, JsonRequestBehavior.AllowGet);
        }

            // First, split the voucher_part0 by '*'
            string part0 = Session["voucher_part0"].ToString();
            string[] voucherParts = part0.Split('*');

            // Initialize lists to store extracted data
            var cash_ids = new List<string>();
            var branches = new List<string>();
            var msgs = new List<string>();
            var amounts = new List<string>();
            var login_users = new List<string>();

        // Iterate over each part split by '*'
            foreach (var part in voucherParts)
            {
                string[] subParts = part.Split(new[] { '$' }, 5); // Split each part by '$'

                if (subParts.Length < 5)
                {
                    return Json(new { success = false, message = "Invalid voucher part format." }, JsonRequestBehavior.AllowGet);
                }

                // Store each extracted value in its respective list
                cash_ids.Add(subParts[0]);
                branches.Add(subParts[1]);
                msgs.Add(subParts[2]);
                amounts.Add(subParts[3]);
                login_users.Add(subParts[4]);
            }

        // Return the data as a JSON object with multiple entries
            return Json(new
            {
                success = true,
                cash_ids,
                branches,
                msgs,
                amounts,
                login_users,
                CIN,
                GSTIN
            }, JsonRequestBehavior.AllowGet);
        }

    }
}