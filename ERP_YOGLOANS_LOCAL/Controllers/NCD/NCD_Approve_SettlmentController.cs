using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Approve_SettlmentController : Controller
    {
        DB dbconnect = new DB();
        public ActionResult NCD_Approve_Settlment_View()
        {
            var customerBankDetails = NCD_transfer_Approval_Grid();

            // Check if DataTable has rows and columns
            if (customerBankDetails != null && customerBankDetails.Rows.Count > 0)
            {
                Console.WriteLine("DataTable Rows: " + customerBankDetails.Rows.Count);
                Console.WriteLine("DataTable Columns: " + customerBankDetails.Columns.Count);
            }
            else
            {
                Console.WriteLine("No data found in DataTable");
            }

            ViewBag.CustomerBankDetails = customerBankDetails;
            return View();
        }
        private DataTable NCD_transfer_Approval_Grid()
        {
            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 8;

            pr[1] = new SqlParameter("@branch_id", SqlDbType.Int);
            pr[1].Value = Session["branch_id"];

            pr[2] = new SqlParameter("@sett", SqlDbType.Char);
            pr[2].Value = 'H';

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            return dt;
        }
        [HttpPost]
        public JsonResult textBox_fill(string deb_id)
        {
            Session["deb_id"] = deb_id;

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 9;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = deb_id;

            DataSet ds = new DataSet();

            try
            {
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr);

                DataTable dt0 = new DataTable();
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                dt0 = ds.Tables[0];
                dt1 = ds.Tables[1];
                dt2 = ds.Tables[2];

            }
            finally
            {
                dbconnect.Close();
            }
            if (ds == null || ds.Tables.Count == 0)
            {
                return Json(new { success = false, message = "No data found" }, JsonRequestBehavior.AllowGet);
            }

            ncd_branch_approval_model model = new ncd_branch_approval_model();

            var resultData = new List<object>();


            resultData.Add(new
            {

                // total_int = ds.Tables[0].Rows[0]["settle_amt"].ToString(),
                total_int = ds.Tables[0].Rows[0]["total_int"].ToString(),
                eranint = ds.Tables[0].Rows[0]["total_eraned_interest"].ToString(),
                tot_tds = ds.Tables[0].Rows[0]["total_tds"].ToString(),

                balanceInput = ds.Tables[0].Rows[0]["settle_balance"].ToString(),

                payamt = ds.Tables[0].Rows[0]["total_paidamt"].ToString(),
                //  payamt = ds.Tables[0].Rows[0]["payamt"].ToString(),
                balpay = ds.Tables[0].Rows[0]["total_payable"].ToString(),
                // pan = ds.Tables[0].Rows[0]["pan"].ToString(),
                settpay = ds.Tables[0].Rows[0]["settle_amt"].ToString(),
                //  trid = ds.Tables[0].Rows[0]["trid"].ToString(),
                pantrack = ds.Tables[1].Rows[0]["pan_trackid"].ToString(),
                //   start = ds.Tables[0].Rows[0]["start"].ToString(),
                //  lastintamt = ds.Tables[0].Rows[0]["lastintamt"].ToString(),
                lasttds = ds.Tables[0].Rows[0]["lasttds"].ToString(),
                short_recovery = ds.Tables[0].Rows[0]["short_recovery"].ToString(),
                //   tds_accrued = ds.Tables[0].Rows[0]["tds_accrued"].ToString(),

                maturity_amount = ds.Tables[2].Rows[0]["maturity_amount"].ToString(),
                // lien = ds.Tables[1].Rows[0]["lien"].ToString(),
            }); ; ;
            //  TempData["start"] = ds.Tables[0].Rows[0]["start"].ToString();
            // TempData["lastintamt"] = ds.Tables[0].Rows[0]["lastintamt"].ToString();
            return Json(new { success = true, data = resultData, }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult panORform60_status()
        {


            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["@deb_id"];

            DataSet ds = new DataSet();


            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr);
            dbconnect.Close();

            var resultData = new List<object>();


            resultData.Add(new
            {

                //panorform60_type = ds.Tables[0].Rows[0]["type"].ToString(),
                //type_form = ds.Tables[0].Rows[0]["type_form"].ToString(),
                //idno = ds.Tables[0].Rows[0]["idno"].ToString(),
                //return_reason = ds.Tables[0].Rows[0]["ReturnReason"].ToString(),

                panorform60_type = ds.Tables[0].Rows[0]["type"]?.ToString() ?? string.Empty,
                type_form = ds.Tables[0].Rows[0]["type_form"]?.ToString() ?? string.Empty,
                idno = ds.Tables[0].Rows[0]["idno"]?.ToString() ?? string.Empty,
                // return_reason = ds.Tables[0].Rows[0]["ReturnReason"]?.ToString() ?? string.Empty,


            });

            return Json(new { success = true, data = resultData }, JsonRequestBehavior.AllowGet);
        }
        private bool IsPdf(byte[] byteData)
        {
            // Check if the byte array represents a PDF file
            return byteData.Length > 4 &&
                   byteData[0] == 0x25 &&
                   byteData[1] == 0x50 &&
                   byteData[2] == 0x44 &&
                   byteData[3] == 0x46 &&
                   byteData[4] == 0x2D;
        }
        private string GetImageUrl(byte[] imageBytes)
        {


            string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

            if (IsPdf(imageBytes))
            {
                // It's a PDF
                string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                //kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                return $"data:application/pdf;base64,{base64String}";

            }
            else
            {
                // It's an image
                string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                // kycItem.Add("image", "data:image/png;base64," + base64Image);
                return $"data:image;base64,{base64String}";
            }
        }

        public JsonResult viewBankAttach()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 12;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["deb_id"];

            DataTable dt = new DataTable();
            ncd_branch_approval_model model = new ncd_branch_approval_model();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count <= 0)
            {
                return Json(new { sucess = false, message = "No document available!" });

            }
            else
            {
                model.ImageUrl2 = GetImageUrl(dt.Rows[0]["attachment"] as byte[]);

            }

            return Json(new { data = model.ImageUrl2 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult bank_table()
        {
            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 11;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["deb_id"];

            DataTable dt = new DataTable();

            ncd_branch_approval_model model = new ncd_branch_approval_model();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", pr).Tables[0];
            dbconnect.Close();

            var table4Data = new
            {
                headings = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                rows = dt.Rows.Cast<DataRow>().Select(row => row.ItemArray.Select(item => item.ToString()).ToList()).ToList()
            };

            return Json(new { success = true, table4Data = table4Data }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCategories()
        {
            List<SelectListItem> categoryList = new List<SelectListItem>();

            try
            {
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 10 };
                parameters[1] = new SqlParameter("@branch_id", SqlDbType.BigInt) { Value = Session["branch_id"] ?? 0 };

                dbconnect.Open();
                DataSet categoryDataSet = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_queries]", parameters);
                dbconnect.Close();

                if (categoryDataSet != null && categoryDataSet.Tables.Count > 0)
                {
                    foreach (DataRow row in categoryDataSet.Tables[0].Rows)
                    {
                        categoryList.Add(new SelectListItem
                        {
                            Value = row["sub_accno"].ToString(),
                            Text = row["sub_name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Approve_BtnClick(Disbursement_model disbursement_Model)
        {
            string reponsejson = string.Empty;
            string api_response = string.Empty;
            string source = string.Empty;
            string paymenttype = disbursement_Model.paymenttype;
            string paymode = disbursement_Model.paymode;
            string custAccount_Number = disbursement_Model.custAccntNumb;
            string ifsc = disbursement_Model.ifsc;
            string customerName = disbursement_Model.custName;
            string disAmt = disbursement_Model.loanAmt;
            string benf_BankName = disbursement_Model.bankName.Trim();

            SqlParameter[] pr = new SqlParameter[6];

            pr[0] = new SqlParameter("branchbank", SqlDbType.Int);
            pr[0].Value = disbursement_Model.bankId;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["deb_id"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"].ToString() };

            pr[3] = new SqlParameter("@br_id", SqlDbType.Int);
            pr[3].Value = Session["branch_id"];

            pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 150) { Direction = ParameterDirection.Output };

            pr[5] = new SqlParameter("@paymode", SqlDbType.VarChar, 10);
            pr[5].Value = disbursement_Model.paymode;

            DataTable dt = new DataTable();

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[DEB_settlement_approval_save]", pr);
            dbconnect.Close();

            //String msg = pr[4].Value.ToString();

            //if (msg == "Approval Successfully")
            //{
            //    return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            //}

            string outputMessage = pr[4].Value.ToString();

            TempData["ResultMessage"] = outputMessage;



            string[] messageParts = outputMessage.Split('!');
            string message="";
            string resultCode = messageParts.Length > 1 ? messageParts[1] : "";
           


            if (messageParts[1] != "")
            {
                string[] msg_string = messageParts[1].Split('~');
                string new_lno = msg_string[0];
                string slno = msg_string[1];
                string accountNumber = msg_string[2];
                string updateId = msg_string[3];
                var acc_name = "";
                if (updateId != "0")
                {
                    if (paymenttype == "S") //Single Payment (Direct Customer Account)
                    {
                        //production Account number
                        if (accountNumber == "10044895542")
                        {
                            acc_name = "YOGLOANS";
                            source = "YGL";
                        }
                        else if (accountNumber == "10156501867")
                        {
                            acc_name = "YOGLOANS3";
                            source = "YLL";
                        }
                        else
                        {

                        }

                        //////UAT Account number
                        //accountNumber = "21490576022";

                        //acc_name = "Yogloans";
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
                            paymentDescription = "BOND Disbursement",
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
                        if (accountNumber == "10044895542")
                        {
                            acc_name = "YOGLOANS";
                        }
                        else if (accountNumber == "10156501867")
                        {
                            acc_name = "YOGLOANS3";
                        }
                        else
                        {

                        }

                        ////UAT Account number
                        //accountNumber = "21490576022";

                        //acc_name = "Yogloans";

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
                            Ben_BankName = benf_BankName.Trim(),
                            Ben_Email = "hod.accounts@yogloans.com",
                            Ben_Mobile = null,
                            Mode_of_Pay = paymode.Trim(),
                            Nature_of_Pay = "MPYMT",
                            Remarks = "BOND Disbursement"

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
                    return Json(new { success = false, message = "Payment is not worked. Some issue occured.!" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
                message = messageParts[0];
                outputMessage = message;

                return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult return_click(string rejectReason)
        {

            SqlParameter[] pr = new SqlParameter[5];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@deb_id", SqlDbType.BigInt);
            pr[1].Value = Session["deb_id"];

            pr[2] = new SqlParameter("@enter_by", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];

            pr[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr[3].Direction = ParameterDirection.Output;

            pr[4] = new SqlParameter("@reason", SqlDbType.NVarChar, 500);  // New parameter for rejection reason
            pr[4].Value = rejectReason;

            DataSet ds = new DataSet();

            dbconnect.Open();
            ds = dbconnect.ExecuteDataset("[dbo].[DEB_settlement_save]", pr);
            dbconnect.Close();

            String msg = pr[3].Value.ToString();

            if (msg == "Returned to B&D Department Successfully ")
            {
                return Json(new { success = true, data = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, data = msg }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult FillDropdowns()
        {
            SqlParameter[] pr2 = new SqlParameter[1];
            pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr2[0].Value = 13;

            dbconnect.Open();
            DataTable dt_type = dbconnect.ExecuteDataset("DEB_settlement_queries", pr2).Tables[0];
            dbconnect.Close();

            SqlParameter[] pr3 = new SqlParameter[1];
            pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr3[0].Value = 14;

            dbconnect.Open();
            DataTable dt_mode = dbconnect.ExecuteDataset("DEB_settlement_queries", pr3).Tables[0];
            dbconnect.Close();

            if (dt_type.Rows.Count > 0 && dt_mode.Rows.Count > 0)
            {
                // Assuming the dataset returns different tables for different dropdowns
                var dropdownData = new
                {
                    TypeOptions = dt_type.AsEnumerable().Select(row => new
                    {
                        Id = row["value"],
                        Name = row["data"]
                    }).ToList(),

                    ModeOptions = dt_mode.AsEnumerable().Select(row => new
                    {
                        Id = row["modeid"],
                        Name = row["mode"]
                    }).ToList()
                };

                return Json(new { success = true, data = dropdownData }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}