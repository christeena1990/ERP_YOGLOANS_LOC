using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_monthly_interest_approvalController : BaseController
    {
        DB dbconnect = new DB();
        // GET: Bond_monthly_interest_approval
        public ActionResult Bond_monthly_interest_approval()
        {
            
    
                //Function call for fill  loan category
                DataTable issue_types = fillissuedropdown();
                ViewData["IssueList"] = issue_types;


                DataTable bank_type = fillbankdropdown();
                ViewData["BankList"] = bank_type;



                return View();
            }

            public DataTable fillissuedropdown()
            {

                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 3;
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_int_payment_approve]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }


            public DataTable fillbankdropdown()
            {

                DataTable dt = new DataTable();
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 2;
                pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr[1].Value = Session["branch_id"];
                dt = dbconnect.ExecuteDataset("[dbo].[BOND_int_payment_approve]", pr).Tables[0];
                dbconnect.Close();
                return dt;

            }




            ///////////////////////////old type ///////////////////////////////////
            [HttpPost]
            public JsonResult GetgridData(int bankId, int? issueNo = null)
            {
                Session["bankId_interestaprvl"] = bankId;
                Session["issueNo_interestaprvl"] = issueNo;

                try
                {
                 SqlParameter[] pr = new SqlParameter[4];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int) ;
                pr[0].Value = 1;
                pr[1] = new SqlParameter("@brbranch", SqlDbType.BigInt);
                pr[1].Value = bankId ;
                pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr[2].Value = Session["branch_id"];
                pr[3] = new SqlParameter("@issue_no", SqlDbType.BigInt);

                    if (issueNo != null)
                    {
                        pr[3].Value = issueNo;
                    }
                    else
                    {
                        pr[3].Value = null;
                    }

                    DataSet ds = dbconnect.ExecuteDataset("[dbo].[BOND_int_payment_approve]", pr);
                    DataTable dt = ds != null && ds.Tables.Count > 0 ? ds.Tables[1] : null;
                    DataTable dt1 = ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;

                    // Check if the DataTable is not null and contains rows
                    if (dt1 != null && dt1.Rows.Count > 0)
                    {
                        // Assuming the value you need is in the first row and first column of dt1
                        var debentureCount = dt1.Rows[0][0];

                        // Store this single value in the session
                        Session["countofdebentures"] = debentureCount;
                    }
                    else
                    {
                        // If dt1 is null or has no rows, handle accordingly
                        Session["countofdebentures"] = null; // Or any default value you prefer
                    }

                    // Check if the DataTable is null or empty
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
                    }


                    var result = dt.AsEnumerable().Select(row => new
                    {
                        Issue_No = Convert.ToString(row["Issue No"]),
                        Interest = row["Interest Amount"] != DBNull.Value ? Convert.ToString(row["Interest Amount"]) : null,
                        //TDS = Convert.ToString(row["TDS"]),
                        Short_Recovery = row["Short Recovery"] != DBNull.Value ? Convert.ToString(row["Short Recovery"]) : null,
                        TotalTDS = row["Total TDs"] != DBNull.Value ? Convert.ToString(row["Total TDs"]) : null,
                        PayAmount = row["Pay Amount"] != DBNull.Value ? Convert.ToString(row["Pay Amount"]) : null,
                        //DebentureAmount = row["Debenture Amount"] != DBNull.Value ? Convert.ToString(row["Debenture Amount"]) : null,
                        NoofDebentures = row["No.of Debentures"] != DBNull.Value ? Convert.ToString(row["No.of Debentures"]) : null,
                    }).ToList();


                    //return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);

                    // Send back JSON with session value and result data
                    return Json(new
                    {
                        success = true,
                        data = result,
                        countofdebentures = Session["countofdebentures"] // Include session value here
                    }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }


            /// ////////////////////////////////

            public ActionResult btn_reject_Click()
            {
                SqlParameter[] pr = new SqlParameter[6];
                {
                    pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr[0].Value = 5;
                    pr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                    pr[1].Value = Session["issueNo_interestaprvl"];
                    pr[2] = new SqlParameter("@brbranch", SqlDbType.BigInt);
                    pr[2].Value = Session["bankId_interestaprvl"];
                    pr[3] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                    pr[3].Value = Session["branch_id"];
                    pr[4] = new SqlParameter("@enter_by", SqlDbType.VarChar);
                    pr[4].Value = Session["login_user"];
                    pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                    pr[5].Direction = ParameterDirection.Output;

                    dbconnect.ExecuteStoredProcedure("[dbo].[BOND_int_payment_approve]", pr);
                    string msg = pr[5].Value.ToString();

                    if (msg == "Rejected Successfully...!")
                    {
                        return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
                    }




                    //return Json(new { success = true, message = "Rejected successfully" });

                }
            }

            /// ////////////////////////////////

            public ActionResult btn_approve_Click()
            {


                SqlParameter[] pr = new SqlParameter[6];
                {
                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    pr[0].Value = 4;

                    pr[1] = new SqlParameter("@brbranch", SqlDbType.BigInt);
                    pr[1].Value = Session["bankId_interestaprvl"];

                    pr[2] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                    pr[2].Value = Session["issueNo_interestaprvl"];

                    pr[3] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                    pr[3].Value = Session["branch_id"];

                    pr[4] = new SqlParameter("@enter_by", SqlDbType.BigInt);
                    pr[4].Value = Session["login_user"];

                    pr[5] = pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                    pr[5].Direction = ParameterDirection.Output;

                    dbconnect.ExecuteStoredProcedure("[dbo].[BOND_int_payment_approve]", pr);
                    string msg = pr[5].Value.ToString();

                    if (msg == "Confirmed Successfully...!")
                    {
                        return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
                    }



                    //return Json(new { success = true, message = "Approved successfully" });

                }
            }


        }





    }