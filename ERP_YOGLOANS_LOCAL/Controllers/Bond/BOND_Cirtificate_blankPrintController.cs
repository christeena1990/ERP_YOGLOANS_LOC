using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class BOND_Cirtificate_blankPrintController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult BOND_Cirtificate_blankPrint_View()
        {
            string bond_id = Session["Bond_cirtificate_BondId"].ToString();
            return View();
        }
        [HttpPost]
        public JsonResult text_box_fill()
        {
          

            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 };
                pr[1] = new SqlParameter("@id", SqlDbType.BigInt) { Value = Session["Bond_cirtificate_BondId"] };
                       
                dbconnect.Open();
                DataTable dt = dbconnect.ExecuteDataset("[dbo].[BOND_certificate]", pr).Tables[0];
                dbconnect.Close();

                var resultData = new List<object>();


                resultData.Add(new
                {

                    customer_id = dt.Rows[0]["customer_id"]?.ToString() ?? string.Empty,
                    issue_no = dt.Rows[0]["issue_no"]?.ToString() ?? string.Empty,
                    certificate_no = dt.Rows[0]["certificate_no"]?.ToString() ?? string.Empty,
                    allotment_date = dt.Rows[0]["allotment_date"]?.ToString() ?? string.Empty,
                    customer_name = dt.Rows[0]["customer_name"]?.ToString() ?? string.Empty,
                    applicant_name = dt.Rows[0]["applicant_name"]?.ToString() ?? string.Empty,
                    ADDRESS = dt.Rows[0]["ADDRESS"]?.ToString() ?? string.Empty,
                    district_pin = dt.Rows[0]["district_pin"]?.ToString() ?? string.Empty,
                    amount = dt.Rows[0]["amount"]?.ToString() ?? string.Empty,
                    amount_in_words = dt.Rows[0]["amount_in_words"]?.ToString() ?? string.Empty,
                    interest_rate = dt.Rows[0]["interest_rate"]?.ToString() ?? string.Empty,
                    int_type = dt.Rows[0]["int_type"]?.ToString() ?? string.Empty,
                    maturity_amount = dt.Rows[0]["maturity_amount"]?.ToString() ?? string.Empty,
                    nominee_name = dt.Rows[0]["nominee_name"]?.ToString() ?? string.Empty,
                    maturity_date = dt.Rows[0]["maturity_date"]?.ToString() ?? string.Empty,
                    reg_startdate = dt.Rows[0]["reg_startdate"]?.ToString() ?? string.Empty,
                    customer_id1 = dt.Rows[0]["customer_id1"]?.ToString() ?? string.Empty,
                    issue_no1 = dt.Rows[0]["issue_no1"]?.ToString() ?? string.Empty,
                    certificate_no1 = dt.Rows[0]["certificate_no1"]?.ToString() ?? string.Empty,
                    allotment_date1 = dt.Rows[0]["allotment_date1"]?.ToString() ?? string.Empty,
                    branch_name = dt.Rows[0]["branch_name"]?.ToString() ?? string.Empty,

                    matamt = dt.Rows[0]["matamt"]?.ToString() ?? string.Empty,


                }); ; ;

                return Json(new { success = true, data = resultData }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}