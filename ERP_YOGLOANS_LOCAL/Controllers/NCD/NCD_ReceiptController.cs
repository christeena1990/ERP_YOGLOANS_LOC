using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_ReceiptController : Controller
    {
        DB dbconnect = new DB();

        public ActionResult NCD_Receipt_View()
        {
            return View();
        }
        public ActionResult NCD_Receipt_select()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 7 };
            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = Session["application_id_br_appro"] };
           // pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = 15001112021002 };
            DataSet ds=new DataSet();
            string m = Session["module_id"].ToString();
            if (Session["module_id"].ToString() == "15")
            {
                dbconnect.Open();
                 ds = dbconnect.ExecuteDataset("[dbo].[DEB_branch_approval]", pr);
                dbconnect.Close();
            }
            else
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[BOND_branch_approval]", pr);
                dbconnect.Close();
            }
        
            string date_time = ds.Tables[0].Rows[0][3].ToString();
            string[] dateParts = date_time.Split(' ');

            // Now you can access date and time separately
            string date = dateParts[0]; // This will contain the date part
            string time = dateParts[1];

            var resultData = new List<object>();
            resultData.Add(new
            {
               
                Branch = ds.Tables[0].Rows[0][0].ToString(),
                deb_amount = ds.Tables[0].Rows[0][1].ToString(),
                emp_name = ds.Tables[0].Rows[0][2].ToString(),
                date= date,
                time= time,
                cus_name = ds.Tables[0].Rows[0][4].ToString(),
                phon_no = ds.Tables[0].Rows[0][5].ToString(),
                cus_id = ds.Tables[0].Rows[0][6].ToString(),
                cusamount_in_words = ds.Tables[0].Rows[0][7].ToString(),

                check_ref_no_amiunt_line = ds.Tables[1].Rows[0][0].ToString(),

                CIN = ds.Tables[2].Rows[0][0].ToString(),
                GSTIN = ds.Tables[2].Rows[0][1].ToString(),

                type_tr = ds.Tables[3].Rows[0][0].ToString()
            }); ; ;
       
            return Json(resultData = resultData , JsonRequestBehavior.AllowGet);
        }
    }
}