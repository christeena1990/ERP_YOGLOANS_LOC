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

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_Settlement_RequestController : Controller
    {
        DB dbconnect = new DB();
        // GET: PL_Settlement_Request
        public ActionResult PL_Settlement_View()
        {
            DataSet ds = new DataSet();

            try
            {
                
                //--------------Payment Mode fill

                SqlParameter[] PTyp_Pr = new SqlParameter[1];
                PTyp_Pr[0] = new SqlParameter("query_id", SqlDbType.BigInt);
                PTyp_Pr[0].Value = 3;

                dbconnect.Open();
                DataSet PTyp_Ds = dbconnect.ExecuteDataset("[dbo].[pl_settlement_queries]", PTyp_Pr);
                dbconnect.Close();

                List<SelectListItem> PaymodeList = new List<SelectListItem>();

                if (PTyp_Ds != null && PTyp_Ds.Tables.Count > 0)
                {
                    foreach (DataRow row in PTyp_Ds.Tables[0].Rows)
                    {
                        PaymodeList.Add(new SelectListItem
                        {
                            Value = row["id"].ToString(),
                            Text = row["pay_mode"].ToString()
                        });
                    }
                }
                ViewBag.PaymodeList = PaymodeList;

            }
            catch (Exception ex)
            {

                ViewBag.Message = "An error occurred while loading regions: " + ex.Message;
            }
            return View();

        }
       

        public ActionResult GetSearchSuggestions(string search_txt)
        {

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);

            pr[0].Value = 1;
            pr[1] = new SqlParameter("@search_txt", SqlDbType.VarChar, 50) { Value = search_txt };

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            var result = dt.AsEnumerable().Select(row => row["name"].ToString()).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Get_Settlement_details(string Loanno)
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;
            pr[1] = new SqlParameter("@loan_no", SqlDbType.VarChar, 50);
            pr[1].Value = Loanno;

            DataTable dt = new DataTable();
            DataTable dtSettlement = new DataTable();

            dt.Clear();
            dtSettlement.Clear();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            //-------------------------------------

            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@loan_no", SqlDbType.VarChar,50);
            pr1[0].Value = Loanno;

            pr1[1] = new SqlParameter("@branchid", SqlDbType.Int);
            pr1[1].Value =Session["branch_id"];

            dbconnect.Open();
            dtSettlement = dbconnect.ExecuteDataset("[dbo].[pl_settlement_display_data]", pr1).Tables[0];
            dbconnect.Close();

            //---------------------------------------------

            

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string Out="0" , Susp = "0", Settle_Interest = "0", F_Charge = "0", O_charge = "0", Net = "0";
                
                
                if (dtSettlement.Rows.Count > 0)
                {
                    DataRow row_settle = dtSettlement.Rows[0];

                    Out = row_settle["outs"] != DBNull.Value ? row_settle["outs"].ToString() : "0";
                    Susp = row_settle["suspense"] != DBNull.Value ? row_settle["suspense"].ToString() : "0";
                    Settle_Interest = row_settle["interest"] != DBNull.Value ? row_settle["interest"].ToString() : "0";
                    F_Charge = row_settle["fore_chrg"] != DBNull.Value ? row_settle["fore_chrg"].ToString() : "0";
                    O_charge = row_settle["other_chrg"] != DBNull.Value ? row_settle["other_chrg"].ToString() : "0";
                    Net = row_settle["net"] != DBNull.Value ? row_settle["net"].ToString() : "0";
                    
                }
                var model = new
                {
                    LoanNo = row["loan_no"] != DBNull.Value ? row["loan_no"].ToString() : string.Empty,
                    CustomerID = row["customer_id"] != DBNull.Value ? row["customer_id"].ToString() : string.Empty,
                    CustomerName = row["customer_name"] != DBNull.Value ? row["customer_name"].ToString() : string.Empty,
                    LoanAmt = row["loan_amount"] != DBNull.Value ? row["loan_amount"].ToString() : "0",
                    LoanDt = row["loandt"] != DBNull.Value ? row["loandt"].ToString() : string.Empty,
                    MaturityDt = row["maturitydt"] != DBNull.Value ? row["maturitydt"].ToString() : string.Empty,
                    LastIntDt = row["intuptodt"] != DBNull.Value ? row["intuptodt"].ToString() : string.Empty,
                    IntRate = row["int_rate"] != DBNull.Value ? row["int_rate"].ToString() : "0",
                    GLNo = row["gl_loan_no"] != DBNull.Value ? row["gl_loan_no"].ToString() : string.Empty,
                    InventoryID = row["inventory_id"] != DBNull.Value ? row["inventory_id"].ToString() : string.Empty,
                    NetWt = row["netwt"] != DBNull.Value ? row["netwt"].ToString() : string.Empty,
                    GLAmt = row["gl_loanamt"] != DBNull.Value ? row["gl_loanamt"].ToString() : "0",
                    Scheme = row["scheme_name"] != DBNull.Value ? row["scheme_name"].ToString() : string.Empty,
                    Status = row["Gl_status"] != DBNull.Value ? row["Gl_status"].ToString() : string.Empty,

                    Outstanding = Out,
                    Suspense = Susp,
                    Settle_Int = Settle_Interest,
                    Fore_chrg =F_Charge,
                    Other_chrg = O_charge,
                    Net = Net,

                    };
                    return Json(model, JsonRequestBehavior.AllowGet);

                
                // System.Diagnostics.Debug.WriteLine("Returned LoanNo: " + model.LoanNo);
  
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult Settlement_Request(String LNo,string outstand, string susp,string Settle_Int, string f_chg,string o_chg,string discount,string settle_amt,string paymode,string actionType)

        //public ActionResult Settlement_Request(Selltement_Req Selltement_Req)

        {
            string outputMessage = string.Empty;
           // string actionType = Selltement_Req.actionType;
            try 
            {
                if (actionType == "Confirm")
                {
                    SqlParameter[] pr = new SqlParameter[12];

                    pr[0] = new SqlParameter("@loan_no", SqlDbType.VarChar, 15);
                    pr[0].Value = LNo;

                    pr[1] = new SqlParameter("@outs", SqlDbType.Decimal);
                    pr[1].Value = Convert.ToDecimal(outstand);

                    pr[2] = new SqlParameter("@suspense", SqlDbType.Decimal);
                    pr[2].Value = Convert.ToDecimal(susp);

                    pr[3] = new SqlParameter("@int_amt", SqlDbType.Float);
                    pr[3].Value = Settle_Int;

                    pr[4] = new SqlParameter("@forclose_chrg", SqlDbType.Float);
                    pr[4].Value = f_chg;

                    pr[5] = new SqlParameter("@other_charge", SqlDbType.Float);
                    pr[5].Value = o_chg;

                    pr[6] = new SqlParameter("@discount_amt", SqlDbType.Float);
                    pr[6].Value = discount;

                    pr[7] = new SqlParameter("@settlement_amt", SqlDbType.Float);
                    pr[7].Value = settle_amt;


                    pr[8] = new SqlParameter("@user", SqlDbType.BigInt);
                    pr[8].Value = Session["login_user"];


                    pr[9] = new SqlParameter("@branch", SqlDbType.BigInt);
                    pr[9].Value = Session["branch_id"];


                    pr[10] = new SqlParameter("@pay_mode", SqlDbType.BigInt);
                    pr[10].Value = paymode;


                    pr[11] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    pr[11].Direction = ParameterDirection.Output;

                    dbconnect.Open();
                    dbconnect.ExecuteStoredProcedure("[dbo].[pl_settlemet_tmp_save]", pr);
                    dbconnect.Close();

                    outputMessage = pr[11].Value.ToString();
                    TempData["ResultMessage"] = outputMessage;

                    if (outputMessage == "Confirmed Successfully..!")
                    {
                        return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else { return Json(new { success = false, message = outputMessage }, JsonRequestBehavior.AllowGet); }

                }
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = "An error occurred during disbursement: " + ex.Message;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("PL_Settlement_View");
        }


    }
}