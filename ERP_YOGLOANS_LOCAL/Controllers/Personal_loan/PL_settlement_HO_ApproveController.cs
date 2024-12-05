using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class PL_settlement_HO_ApproveController : Controller
    {
        DB dbconnect = new DB();

        //PERSONAL LOAN -SETTLEMENT  HO APPROVE 
        //---------------------------------------
       
        // GET: PL_settlement_HO_Approve
        public ActionResult PL_Settle_HO_Approve_View()
        {
            try
            {
                var pl_table_values = PL_Settle_HO_BranchGrid();
                bool isEmpty = pl_table_values.Rows.Count == 0;
                ViewBag.IsEmpty = isEmpty;

                return View(pl_table_values);
            }
            catch
            {
            }
            return View();
        }
        private DataTable PL_Settle_HO_BranchGrid()
        {
            SqlParameter[] pr = new SqlParameter[1];
            
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 6;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;

        }
        [HttpPost]
        public JsonResult PL_HO_settle_details(String Loanno, string slno)
        {
            Session["loanno"] = Loanno;
            Session["sl_no"] = slno;

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

            pr1[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr1[0].Value = 9;

            pr1[1] = new SqlParameter("@loan_no", SqlDbType.VarChar, 50);
            pr1[1].Value = Loanno;

            dbconnect.Open();
            dtSettlement = dbconnect.ExecuteDataset("[dbo].[pl_settlement_queries]", pr1).Tables[0];
            dbconnect.Close();

            //---------------------------------------------



            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string Out = "0", Susp = "0", Interest = "0", F_Charge = "0", O_charge = "0", discount = "0", Settle_Amt = "0", Paymode = string.Empty;


                if (dtSettlement.Rows.Count > 0)
                {
                    DataRow row_settle = dtSettlement.Rows[0];




                    Out = row_settle["outstanding"] != DBNull.Value ? row_settle["outstanding"].ToString() : "0";
                    Susp = row_settle["suspense_amt"] != DBNull.Value ? row_settle["suspense_amt"].ToString() : "0";
                    Interest = row_settle["int_amt"] != DBNull.Value ? row_settle["int_amt"].ToString() : "0";
                    F_Charge = row_settle["forclose_chrg"] != DBNull.Value ? row_settle["forclose_chrg"].ToString() : "0";
                    O_charge = row_settle["other_charges"] != DBNull.Value ? row_settle["other_charges"].ToString() : "0";
                    discount = row_settle["discount_amt"] != DBNull.Value ? row_settle["discount_amt"].ToString() : "0";
                    Settle_Amt = row_settle["settlement_amt"] != DBNull.Value ? row_settle["settlement_amt"].ToString() : "0";
                    Paymode = row_settle["PaymentMode"] != DBNull.Value ? row_settle["PaymentMode"].ToString() : string.Empty;


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
                    Settle_Int = Interest,
                    Fore_chrg = F_Charge,
                    Other_chrg = O_charge,
                    Discount = discount,
                    Settle_Amt = Settle_Amt,
                    Paymode = Paymode,

                };
                return Json(model, JsonRequestBehavior.AllowGet);


                // System.Diagnostics.Debug.WriteLine("Returned LoanNo: " + model.LoanNo);

            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Settlement_HO_Approve(string actionType)

       

        {
            string outputMessage = string.Empty;
            // string actionType = Selltement_Req.actionType;
            try
            {
                if (actionType == "Approve")
                {
                    SqlParameter[] pr = new SqlParameter[5];

                    pr[0] = new SqlParameter("@user", SqlDbType.BigInt);
                    pr[0].Value = Session["login_user"];

                    pr[1] = new SqlParameter("@loan_no", SqlDbType.VarChar,20);
                    pr[1].Value = Session["loanno"];

                    pr[2] = new SqlParameter("@slno", SqlDbType.BigInt);
                    pr[2].Value = Session["sl_no"];

                    pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    pr[3].Direction = ParameterDirection.Output;

                    pr[4] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr[4].Value = 7;

                    dbconnect.Open();
                    dbconnect.ExecuteStoredProcedure("[dbo].[pl_settlement_queries]", pr);
                    dbconnect.Close();

                    outputMessage = pr[3].Value.ToString();
                    TempData["ResultMessage"] = outputMessage;

                    //if (outputMessage == "Confirmed Successfully..!")
                    //{
                    //    return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                    //}
                    //else { return Json(new { success = false, message = outputMessage }, JsonRequestBehavior.AllowGet); }
                    return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                }
                else if (actionType == "Reject")
                {
                    SqlParameter[] pr = new SqlParameter[5];

                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    pr[0].Value = 8;

                    pr[1] = new SqlParameter("@loan_no", SqlDbType.VarChar, 15);
                    pr[1].Value = Session["loanno"];

                    pr[2] = new SqlParameter("@slno", SqlDbType.BigInt);
                    pr[2].Value = Session["sl_no"];

                    pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    pr[3].Direction = ParameterDirection.Output;

                    pr[4] = new SqlParameter("@user", SqlDbType.BigInt);
                    pr[4].Value = Session["login_user"];



                    dbconnect.Open();
                    dbconnect.ExecuteStoredProcedure("[dbo].[pl_settlement_queries]", pr);
                    dbconnect.Close();

                    outputMessage = pr[3].Value.ToString();
                    TempData["ResultMessage"] = outputMessage;

                    return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = "An error occurred during disbursement: " + ex.Message;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("PL_Settle_Branch_Approve_View");
        }
    }
}