using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Web.UI;
using iTextSharp.text.pdf.qrcode;
using ERP_YOGLOANS_LOCAL.Models.personal_loan_models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ERP_YOGLOANS_LOCAL.Controllers.Personal_loan
{
    public class Personal_Loan_DisburseController : Controller
    {
        DB dbconnect = new DB();

        //PERSONAL LOAN - HO DISBURSE (BANK)
        //Pl_loan_application (Paymentmode:1)
        //Need Bank Details,View Bank Details,Select HO bank,Payment Type and Mode Selection (dropdown lists)
        //---------------------------------------------------


        // GET: Personal_Loan_Disburse
        public ActionResult Personal_Loan_DisburseView()
        {
            DataSet ds = new DataSet();

            try
            {
                //-----------Region Fill

                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr[0].Value = 19;

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

                //------------------Bank Fill--------------------------------

                SqlParameter[] bankPr = new SqlParameter[2];
                bankPr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                bankPr[0].Value = 26;
                bankPr[1] = new SqlParameter("@branch_id", SqlDbType.Int); 
                bankPr[1].Value = Session["branch_id"];

                dbconnect.Open();
                DataSet bankDs = dbconnect.ExecuteDataset("[dbo].[pl_queries]", bankPr);
                dbconnect.Close();

                List<SelectListItem> bankList = new List<SelectListItem>();

                if (bankDs != null && bankDs.Tables.Count > 0)
                {
                    foreach (DataRow row in bankDs.Tables[0].Rows)
                    {
                        bankList.Add(new SelectListItem
                        {
                            Value = row["sub_accno"].ToString(),
                            Text = row["sub_name"].ToString()
                        });
                    }
                }
                ViewBag.BankList = bankList;
                //-------------------Payment type fill

                SqlParameter[] modPr = new SqlParameter[1];
                modPr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                modPr[0].Value = 24;

                dbconnect.Open();
                DataSet modDs = dbconnect.ExecuteDataset("[dbo].[pl_queries]", modPr);
                dbconnect.Close();

                

                List<SelectListItem> PaymentTypeList = new List<SelectListItem>();

                if (modDs != null && modDs.Tables.Count > 0)
                {
                    foreach (DataRow row in modDs.Tables[0].Rows)
                    {
                        PaymentTypeList.Add(new SelectListItem
                        {

                            Value = row["value"].ToString(),
                            Text = row["data"].ToString()
                        });
                    }
                }
                ViewBag.PaymentTypeList = PaymentTypeList;


                //--------------Payment Mode fill

                SqlParameter[] PTyp_Pr = new SqlParameter[1];
                PTyp_Pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                PTyp_Pr[0].Value = 25;

                dbconnect.Open();
                DataSet PTyp_Ds = dbconnect.ExecuteDataset("[dbo].[pl_queries]", PTyp_Pr);
                dbconnect.Close();
                
                List<SelectListItem> PaymodeList = new List<SelectListItem>();

                if (PTyp_Ds != null && PTyp_Ds.Tables.Count > 0)
                {
                    foreach (DataRow row in PTyp_Ds.Tables[0].Rows)
                    {
                        PaymodeList.Add(new SelectListItem
                        {
                            Value = row["modeid"].ToString(),
                            Text = row["mode"].ToString()
                        });
                    }
                }
                ViewBag.PaymodeList = PaymodeList;





                var pl_table_values = personal_loan_disburse_grid();
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

      


        private DataTable personal_loan_disburse_grid()
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 20;

            pr[1] = new SqlParameter("@region_id", SqlDbType.BigInt);
            pr[1].Value = -1;
            DataTable dt = new DataTable();
            
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;

        }

        public ActionResult UpdateScrollableContainer(long region_id)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
                pr[0].Value = 20;

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
                               ID: {row["application_id"]}
                            </div>
                            <div class='widget-subheading' style='font-size:small'>
                                {row["branch_name"]}
                              </div>

                        </div>
                    </div>
                </div>
            </li>");
            }

            return Content(htmlContent.ToString(), "text/html");
        }


        [HttpPost]
        public JsonResult PL_Disburse_details(long applicationid)
        {
            //Session["app_id"] = applicationid;
            Session["PL_application_id"] = applicationid;

            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr[0].Value = 21; 

            pr[1] = new SqlParameter("@app_id", SqlDbType.VarChar, 15);
            pr[1].Value = Session["PL_application_id"];

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            dbconnect.Open();
            DataTable dtLoan = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr).Tables[0];
            dbconnect.Close();

           


            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@queryid", SqlDbType.BigInt);
            pr1[0].Value = 15;

            pr1[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr1[1].Value = dtLoan.Rows[0][2];

            dbconnect.Open();
            

            DataTable dtBank = new DataTable();
            if (dtBank != null)
            {
                dtBank.Clear();  // This will remove all rows but keep the schema
            }


            dtBank = dbconnect.ExecuteDataset("[dbo].[pl_queries]", pr1).Tables[0];
            dbconnect.Close();
            

            if (dtLoan.Rows.Count > 0)
            {
                DataRow row = dtLoan.Rows[0];

                var model = new
                {
                    CustomerId = row["customer_id"] != DBNull.Value ? row["customer_id"].ToString() : string.Empty,
                    CustomerName = row["customer_name"] != DBNull.Value ? row["customer_name"].ToString() : string.Empty,
                    ApplicationID = row["application_id"] != DBNull.Value ? row["application_id"].ToString() : string.Empty,
                    LoanNo = row["gl_loan_no"] != DBNull.Value ? row["gl_loan_no"].ToString() : string.Empty,
                    LoanAmt = row["loan_amount"] != DBNull.Value ? Convert.ToDecimal(row["loan_amount"]) : 0.0m,
                    BranchId = row["branch_id"] != DBNull.Value ? row["branch_id"].ToString() : string.Empty,
                    Receivables = row["receivables"] != DBNull.Value ? row["receivables"].ToString() : string.Empty,
                    Disbursement = row["disb_amt"] != DBNull.Value ? row["disb_amt"].ToString() : string.Empty,

                    // Bank details
                    BankDetails = dtBank.AsEnumerable().Select(bankRow => new
                    {
                        AccNo = bankRow["acc_no"] != DBNull.Value ? bankRow["acc_no"].ToString() : string.Empty,
                        IFSC = bankRow["ifsc_code"] != DBNull.Value ? bankRow["ifsc_code"].ToString() : string.Empty,
                        BankName = bankRow["bank_name"] != DBNull.Value ? bankRow["bank_name"].ToString() : string.Empty,
                        BranchName = bankRow["branch_name"] != DBNull.Value ? bankRow["branch_name"].ToString() : string.Empty
                    }).ToList()
                };
                

                Session["customer_id"] = model.CustomerId;
                return Json(model, JsonRequestBehavior.AllowGet);
            }
         

            return Json(null, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public async Task<ActionResult> DisburseLoan(DisbursementData disbursementData)
        {
            string reponsejson = string.Empty;
            string api_response = string.Empty;
            string appId = disbursementData.appId;
            string loanAmt = disbursementData.loanAmt;
            string disAmt = disbursementData.disAmt;
            string bankId = disbursementData.bankId;
            string paymode = disbursementData.paymode;
            string paymenttype = disbursementData.paymenttype;
            string actionType = disbursementData.actionType;
            string custAccount_Number = disbursementData.custAccntNumb;
            string ifsc = disbursementData.ifsc;
            string customerName = disbursementData.custName;

            string outputMessage = string.Empty;

            try
            {
                if (actionType == "Disburse")
                {
                        SqlParameter[] pr = new SqlParameter[8];

                        pr[0] = new SqlParameter("@appid", SqlDbType.VarChar, 15);
                        pr[0].Value = appId;

                        pr[1] = new SqlParameter("@loanamt", SqlDbType.Decimal);
                        pr[1].Value = Convert.ToDecimal(loanAmt);

                        pr[2] = new SqlParameter("@disamt", SqlDbType.Decimal);
                        pr[2].Value = Convert.ToDecimal(disAmt);

                        pr[3] = new SqlParameter("@userid", SqlDbType.BigInt);
                        pr[3].Value = Session["login_user"]; 

                        pr[4] = new SqlParameter("@bankid", SqlDbType.VarChar, 15);
                        pr[4].Value = bankId;
                   
                        pr[5] = new SqlParameter("@paytype", SqlDbType.VarChar, 50);
                        pr[5].Value = paymenttype;
                    
                        pr[6] = new SqlParameter("@paymode", SqlDbType.VarChar, 50);
                        pr[6].Value = paymode;

                        pr[7] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                        pr[7].Direction = ParameterDirection.Output;

                        dbconnect.Open();
                        dbconnect.ExecuteStoredProcedure("[dbo].[pl_proc_disbursement]", pr);
                        dbconnect.Close();

                        outputMessage = pr[7].Value.ToString();
                        TempData["ResultMessage"] = outputMessage;



                    string[] messageParts = outputMessage.Split('!');
                    string message;
                    string resultCode = messageParts.Length > 1 ? messageParts[1] : "";

                   

                    if (messageParts[1] != "")
                    {
                        string[] msg = messageParts[1].Split('~');
                        string new_lno = msg[0];
                        string slno= msg[1];
                        string accountNumber = msg[2];
                        string updateId = msg[3];
                        var acc_name = "";

                        if (paymenttype == "S")
                        {
                            ////production Account number
                            //if (accountNumber == "10044895542")
                            //{
                            //    acc_name = "YOGLOANS";
                            //    source= "YGL";
                            //}
                            //else if (accountNumber == "10156501867")
                            //{
                            //    acc_name = "YOGLOANS3";
                            //    source = "YLL";
                            //}
                            //else
                            //{

                            //}

                            ////UAT Account number
                            accountNumber = "21490576022";

                            acc_name = "Yogloans";
                            string msgType = "";
                            if (paymode == "RTGS")
                            {
                                msgType = "R41";
                            }
                            else
                            {
                                msgType = "";
                            }

                            var rootobject = new List<IDFC_singlepaymentModel.Rootobject>();

                            // Create a new instance of initiateAuthGenericFundTransferAPIReq and assign its properties
                            var initiateAPIReq = new IDFC_singlepaymentModel.Initiateauthgenericfundtransferapireq
                            {
                                transactionID = updateId.Trim(),
                                debitAccountNumber = accountNumber,
                                creditAccountNumber = custAccount_Number.Trim(),
                                remitterName = acc_name,
                                amount = disAmt.Trim(),
                                currency = "INR",
                                transactionType = paymode.Trim(),
                                paymentDescription = "PL Disbursement",
                                beneficiaryIFSC = ifsc.Trim(),
                                beneficiaryName = customerName.Trim(),
                                beneficiaryAddress = "Address",
                                emailId = "hod.accounts@yogloans.com",
                                mobileNo = "8547213907",
                                // brokerCode = "ISEC",
                                brokerCode = "",
                                messageType = msgType
                            };

                            var wrappedObject = new { initiateAuthGenericFundTransferAPIReq = initiateAPIReq };

                            // Serialize the wrapped object to JSON
                            reponsejson = JsonConvert.SerializeObject(wrappedObject); //Json request
                            IDFC_directCustomer iDFC_DirectCustomer = new IDFC_directCustomer();

                            api_response = await iDFC_DirectCustomer.ProcessPayment(reponsejson, updateId);
                            if (api_response != "")
                            {
                                var paymentResponse = JsonConvert.DeserializeObject<IDFC_singlepaymentResponseModel>(api_response);

                                var refNo = paymentResponse.InitiateAuthGenericFundTransferAPIResp.ResourceData.TransactionReferenceNo;
                                var resourse_status = paymentResponse.InitiateAuthGenericFundTransferAPIResp.ResourceData.Status;
                                var metadata_status = paymentResponse.InitiateAuthGenericFundTransferAPIResp.MetaData.Status;
                                var metadata_message = paymentResponse.InitiateAuthGenericFundTransferAPIResp.MetaData.Message;


                                SqlParameter[] pr1 = new SqlParameter[6];
                                pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                                pr1[0].Value = 17;
                                pr1[1] = new SqlParameter("@utrno", SqlDbType.VarChar, 50);
                                pr1[1].Value = refNo;
                                pr1[2] = new SqlParameter("@statusdet", SqlDbType.VarChar, 100);
                                pr1[2].Value = resourse_status;
                                pr1[3] = new SqlParameter("@status", SqlDbType.VarChar, 30);
                                pr1[3].Value = metadata_status;
                                pr1[4] = new SqlParameter("@slno", SqlDbType.BigInt);
                                pr1[4].Value = slno;
                                pr1[5] = new SqlParameter("@message", SqlDbType.NVarChar, 100);
                                pr1[5].Value = metadata_message;

                                dbconnect.Open();
                                dbconnect.ExecuteStoredProcedure("account_idfc_integration_procedure", pr1);
                                dbconnect.Close();
                            }
                        }
                        else //Multi Payment
                        {
                            List<idfcmultipaymentmodel.Payment> payments = new List<idfcmultipaymentmodel.Payment>();
                            idfcmultipaymentmodel.Rootobject abc = new idfcmultipaymentmodel.Rootobject();
                            idfcmultipaymentmodel.Domultipaymentcorpreq rootobject = new idfcmultipaymentmodel.Domultipaymentcorpreq();
                            idfcmultipaymentmodel.Header header = new idfcmultipaymentmodel.Header();
                            //if (accountNumber == "10044895542")
                            //{
                            //    acc_name = "YOGLOANS";
                            //}
                            //else if (accountNumber == "10156501867")
                            //{
                            //    acc_name = "YOGLOANS3";
                            //}
                            //else
                            //{

                            //}

                            ////UAT Account number
                            accountNumber = "21490576022";

                            acc_name = "Yogloans";

                            payments.Add(new idfcmultipaymentmodel.Payment()
                            {
                                RefNo = slno,         //slno
                                Amount = Convert.ToDecimal(disAmt.Trim()),
                                //Debit_Acct_No = "10044895542", // Production Account Number                                
                                //Debit_Acct_Name = "YOGLOANS",
                                Debit_Acct_No = accountNumber, // Production Account Number                                
                                Debit_Acct_Name = acc_name,
                                Debit_Mobile = "8547213907",
                                Ben_IFSC = ifsc.Trim(),
                                Ben_Acct_No = custAccount_Number.Trim(),
                                Ben_Name = customerName.Trim(),
                                Ben_BankName = "",
                                Ben_Email = "hod.accounts@yogloans.com",
                                Ben_Mobile = null,
                                Mode_of_Pay = paymode.Trim(),
                                Nature_of_Pay = "MPYMT",
                                Remarks = "PAYMENT"

                            });

                            header.Approver_ID = "YOGPROD" + updateId.Trim();
                            header.Maker_ID = "PRASADM.5677";
                            header.Checker_ID = "PRASADM.5677";
                            rootobject.Header = header;
                            idfcmultipaymentmodel.Body body = new idfcmultipaymentmodel.Body();
                            body.Payment = payments;
                            rootobject.Body = body;
                            abc.doMultiPaymentCorpReq = rootobject;

                            //create tran_id
                            var ss = "YOGPROD" + updateId.Trim();
                            reponsejson = JsonConvert.SerializeObject(abc); //Json request

                            IDFC_throughIDFC iDFC_ThroughIDFC = new IDFC_throughIDFC();

                            api_response = await iDFC_ThroughIDFC.ProcessPayment(reponsejson, ss);
                        }

                        //message = messageParts[0] + " ( Loan No : " + new_lno.ToString() + " )";
                        message = api_response;


                        return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = messageParts[0];
                        outputMessage = message;

                        return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                    }



                    //outputMessage = message;
                    //return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);

                }

               

            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = "An error occurred during disbursement: " + ex.Message;
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Personal_Loan_DisburseView");

        }


    }
}