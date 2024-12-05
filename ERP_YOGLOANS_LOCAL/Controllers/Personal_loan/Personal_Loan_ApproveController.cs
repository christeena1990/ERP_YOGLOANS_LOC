using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.personal_loan_models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class Personal_Loan_ApproveController : Controller
    {
        DB dbconnect = new DB();

        // GET: Personal_Loan_Approve
        public ActionResult Personal_Loan_ApproveView()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlParameter[] pr = new SqlParameter[1];

                pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr[0].Value = 9;

                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                List<SelectListItem> regionList = new List<SelectListItem>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        regionList.Add(new SelectListItem
                        {
                            Value = row["region_id"].ToString(),
                            Text = row["region_name"].ToString()
                        });
                    }
                }

                ViewBag.RegionList = regionList;

                var pl_table_values = personal_loan_approve_grid();
                bool isEmpty = pl_table_values.Rows.Count == 0;

                ViewBag.IsEmpty = isEmpty;
                return View(pl_table_values);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred while loading regions: " + ex.Message;
            }

            return View();
        }
        private DataTable personal_loan_approve_grid()
        {
            DataTable dt = new DataTable();

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 10;

            pr[1] = new SqlParameter("@region_id", SqlDbType.BigInt);
            pr[1].Value = -1;

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();
            
            return dt;

        }

        [HttpPost]
       
        public JsonResult personal_loan_details(long AppID)
        {
           
            Session["app_id"] = AppID;

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 11;

            
            pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
            pr[1].Value = Session["app_id"];

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            dbconnect.Open();
            DataTable dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                var model = new
                {
                    CustomerId = row["customer_id"] != DBNull.Value ? row["customer_id"].ToString() : string.Empty,
                    CustomerName = row["customer_name"] != DBNull.Value ? row["customer_name"].ToString() : string.Empty,
                    ApplicationID = row["application_id"] != DBNull.Value ? row["application_id"].ToString() : string.Empty,
                    MaxLoan = row["max_loan_amt"] != DBNull.Value ? Convert.ToDecimal(row["max_loan_amt"]) : 0.0m,
                    InterestRt = row["int_rate"] != DBNull.Value ? row["int_rate"].ToString() : string.Empty,
                    MaturityDt = row["mat_dt"] != DBNull.Value ? Convert.ToDateTime(row["mat_dt"]).ToString("dd-MM-yyyy") : string.Empty,
                    AppDt = row["appli_dt"] != DBNull.Value ? Convert.ToDateTime(row["appli_dt"]).ToString("dd-MM-yyyy") : string.Empty,
                    SchemeName = row["scheme_name"] != DBNull.Value ? row["scheme_name"].ToString() : string.Empty,
                    LoanAmt = row["loan_amount"] != DBNull.Value ? Convert.ToDecimal(row["loan_amount"]) : 0.0m,
                    BranchId = row["branch_id"] != DBNull.Value ? row["branch_id"].ToString() : string.Empty,
                    LoanNo= row["gl_loan_no"] != DBNull.Value ? row["gl_loan_no"].ToString() : string.Empty,
                    Doc_Charges = row["doc_charge"] != DBNull.Value ? row["doc_charge"].ToString() : string.Empty,
                    DisbAmt = row["disb_amt"] != DBNull.Value ? row["disb_amt"].ToString() : string.Empty,
                };

                Session["customer_id_his"] = model.CustomerId;
                Session["PL_application_id"] = model.ApplicationID;
                Session["loan-no"] = model.LoanNo;

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            // Return null if no data found
            return Json(null, JsonRequestBehavior.AllowGet);


        }

        public ActionResult UpdateScrollableContainer(long region_id)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr[0].Value = 10;

                pr[1] = new SqlParameter("@region_id", SqlDbType.BigInt);
                pr[1].Value = region_id;

                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
                dbconnect.Close();
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred: " + ex.Message });
            }

            // Create HTML content for the updated loan details
            var htmlContent = new System.Text.StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
              //var customerloanId = row["loan_no"];
                var customerloanId = row["application_id"];
              htmlContent.Append($@"
                <li class='list-group-item'>
                    <div class='todo-indicator bg-primary'></div>
                    <div class='widget-content p-0'>
                        <div class='widget-content-wrapper'>
                            <div class='widget-content-left mr-2'>
                                <div class='custom-checkbox custom-control'>
                                    <input type='checkbox' name='option' class='custom-control-input checkbox-option' data-customer-id='{customerloanId}' id='checkbox-{customerloanId}'>
                                    <label class='custom-control-label' for='checkbox-{customerloanId}'>&nbsp;</label>
                                </div>
                            </div>
                            <div class='widget-content-left'>
                                <div class='widget-heading' style='font-size:small'>
                                 {row["customer_name"]}
                                </div>
                                <div class='widget-subheading' style='font-size:small'>
                                    App ID:{@row["application_id"]}
                                </div>
                                <div class='widget-subheading' style='font-size:small'>
                                 Branch:{@row["branch_name"]}
                                </div>
                            </div>
                        </div>
                    </div>
                </li>");
            }

            return Content(htmlContent.ToString(), "text/html");
        }


        [HttpPost]
        public ActionResult ApproveApplication(string appId, string loanNo, float loanAmt,float charges,float DisbAmt, string actionType)
        {
            string outputMessage = string.Empty;

            if (actionType == "Approve")
            {
                SqlParameter[] pr = new SqlParameter[9];

                pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
                pr[0].Value = 17; 

                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 15);
                pr[1].Value = Session["PL_application_id"];

                pr[2] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                pr[2].Value = Session["login_user"] ;

                pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[3].Direction = ParameterDirection.Output;

                pr[4] = new SqlParameter("@loan_no", SqlDbType.VarChar, 15);
                pr[4].Value = Session["loan-no"];

                pr[5] = new SqlParameter("@loan_amt", SqlDbType.Float);
                pr[5].Value = loanAmt;

                pr[6] = new SqlParameter("@rec_amt", SqlDbType.Float);
                pr[6].Value = charges;

                pr[7] = new SqlParameter("@tfr_amt", SqlDbType.Float);
                pr[7].Value = 0;

                pr[8] = new SqlParameter("@disb_amt", SqlDbType.Float);
                pr[8].Value = DisbAmt;

                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                outputMessage = pr[3].Value.ToString();

                //TempData["ResultMessage"] = outputMessage;
               //return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);

                bool isSuccess = false;

                string[] messageParts = outputMessage.Split('#');
                string message = messageParts[0]; // Part before the '#'
                string resultCode = messageParts.Length > 1 ? messageParts[1] : "";

               
                TempData["ResultMessage"] = message;

                // Check the resultCode for success or failure
                if (resultCode == "1")
                {
                    isSuccess = true; // Indicates success
                }
                else if (resultCode == "2")
                {
                    isSuccess = false; // Indicates failure
                }
                else if (resultCode == "3")
                {
                    isSuccess = false; // Indicates failure
                }

                return Json(new { success = isSuccess, message = message }, JsonRequestBehavior.AllowGet);


            }
            else if (actionType == "Reject")
            {
                SqlParameter[] pr = new SqlParameter[5];

                pr[0] = new SqlParameter("@queryid", SqlDbType.Int);
                pr[0].Value = 18; 

                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 15);
                pr[1].Value = Session["PL_application_id"];

                pr[2] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                pr[2].Value = Session["login_user"];

                pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[3].Direction = ParameterDirection.Output;

                pr[4] = new SqlParameter("@loan_no", SqlDbType.VarChar, 15);
                pr[4].Value = Session["loan-no"];

                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("[dbo].[pl_queries]", pr);
                dbconnect.Close();

                outputMessage = pr[3].Value.ToString();
                TempData["ResultMessage"] = outputMessage;
              
                return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Personal_Loan_ApproveView");
        }

        [HttpPost]
        public ActionResult SaveDisbursementMemo(string app_id, string enter_by, string data1)
        {
            string outmsg = string.Empty;

            try
            {
                // Create SQL parameters
                SqlParameter[] pr = new SqlParameter[5];
                pr[0] = new SqlParameter("@queryid", SqlDbType.Int) { Value = 16 }; // For upload Disbursement Memo
                pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar) { Value = app_id };
                pr[2] = new SqlParameter("@data1", SqlDbType.VarChar) { Value = data1 };
                pr[3] = new SqlParameter("@enter_by", SqlDbType.VarChar) { Value = enter_by };
                pr[4] = new SqlParameter("@curr_dt", SqlDbType.DateTime) { Value = DateTime.Now };

               
                    dbconnect.Open();
                    dbconnect.ExecuteNonQuery("[dbo].[pl_queries]", pr);
                    dbconnect.Close();
                

                outmsg = "Disbursement Memo Saved..";
            }
            catch (Exception ex)
            {
                outmsg = "Error: " + ex.Message;
            }

            return Json(new { message = outmsg });
        }


    }

}