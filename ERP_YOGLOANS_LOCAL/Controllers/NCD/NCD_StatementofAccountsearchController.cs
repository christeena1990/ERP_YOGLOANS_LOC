using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_StatementofAccountsearchController : Controller
    {
        DB dbconnect = new DB();
        // GET: NCD_StatementofAccount
        public ActionResult NCD_StatementofAccountsearch()
        {
            return View();
        }


     

        public ActionResult GetSearchSuggestions(string search_txt)
        {
            SqlParameter[] pr = new SqlParameter[4];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 8 };
            pr[1] = new SqlParameter("@searchdata", SqlDbType.VarChar, 50) { Value = search_txt };
            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = Session["branch_id"] };
            pr[3] = new SqlParameter("@user", SqlDbType.BigInt) { Value = Session["login_user"] };

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr).Tables[0];
            dbconnect.Close();

            var result = dt.AsEnumerable().Select(row => new
            {
                name = row["name"].ToString(),     // Replace "name" with actual column name
                deb_id = row["deb_id"].ToString() // Replace "deb_id" with actual column name
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }



    }

}


//public ActionResult GetSearchSuggestions(string search_txt)
//{

//    SqlParameter[] pr = new SqlParameter[4];

//    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
//    pr[0].Value = 8;
//    pr[1] = new SqlParameter("@searchdata", SqlDbType.VarChar, 50);
//    pr[1].Value = search_txt;
//    pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
//    pr[2].Value = Session["branch_id"];
//    pr[3] = new SqlParameter("@user", SqlDbType.BigInt);
//    pr[3].Value = Session["login_user"];


//    DataTable dt = new DataTable();
//    dbconnect.Open();
//    dt = dbconnect.ExecuteDataset("[dbo].[DEB_report]", pr).Tables[0];
//    dbconnect.Close();

//    var result = dt.AsEnumerable().Select(row => row["name"].ToString()).ToList();

//    return Json(result, JsonRequestBehavior.AllowGet);
//}



