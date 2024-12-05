using ERP_YOGLOANS_LOCAL.Models.Legality_Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.Business_Loan
{
    public class BL_DocEsign_SecondController : Controller
    {
        DB dbconnect = new DB();


        // GET: BL_DocEsign_Second
        public ActionResult BL_DocEsign_Second()
        {
            return View();
        }
        public ActionResult fillGrid()
        {
            SqlParameter[] pr = new SqlParameter[3];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[1].Value = Session["branch_id"];
            pr[2] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[2].Value = Session["login_user"];

            DataTable dt = new DataTable();
            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[Leegality_queries]", pr).Tables[0];
            dbconnect.Close();
            List<Dictionary<string, object>> BL_customerList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> customers = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    customers[col.ColumnName] = row[col];
                }
                BL_customerList.Add(customers);
            }

            // Return the data as JSON
            return Json(BL_customerList, JsonRequestBehavior.AllowGet);
        }

       
        private string RenderViewToString(string viewName)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.ToString();
            }
        }


        [HttpPost]
        public ActionResult Fillform(string applicationId)
        {
            try
            {
                // Fetch application data
                SqlParameter[] pr1 = new SqlParameter[2];
                pr1[0] = new SqlParameter("@module_id", SqlDbType.BigInt) { Value = 11 };
                pr1[1] = new SqlParameter("@application_id", SqlDbType.BigInt) { Value = applicationId };

                dbconnect.Open();
                DataSet ds1 = dbconnect.ExecuteDataset("application_merge", pr1);
                dbconnect.Close();

                // If application data is found, proceed to get KFS data
                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    // Map application data to the model                   
                    var applicationFormData = MapSanctionLetterFormData(ds1.Tables[1]);
                    var DPNapplicationFormData = MapDPNFormData(ds1.Tables[2]);
                    var AgreementapplicationFormData = MapAgrmntFormData(ds1.Tables[3]);
                    var RepaymentapplicationFormData = MapRepaymentSchedule(ds1.Tables[4]);
                    var TermsndconditionsapplicationFormData = MapTCFormData(ds1.Tables[5]);
                    var AnnexureapplicationFormData = MapAnnxFormData(ds1.Tables[6]);
                    var DMapplicationFormData = MapDMFormData(ds1.Tables[7]);

                    // Fetch KFS data
                    SqlParameter[] pr2 = new SqlParameter[3];
                    pr2[0] = new SqlParameter("@module_id", SqlDbType.Int) { Value = 11 };
                    pr2[1] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                    pr2[2] = new SqlParameter("@loanno", SqlDbType.BigInt) { Value = 11010423500141 };

                    dbconnect.Open();
                    DataSet ds2 = dbconnect.ExecuteDataset("SP_KFS", pr2);
                    dbconnect.Close();

                    // Create the ViewModel and populate with data
                    var viewModel = new ApplicationAndKFSViewModel
                    {
                        ApplicationFormData = applicationFormData,
                        DPNApplicationFormData = DPNapplicationFormData,
                        AgreementApplicationFormData = AgreementapplicationFormData,
                        TandCApplicationFormData = TermsndconditionsapplicationFormData,
                        AnnexureApplicationFormData = AnnexureapplicationFormData,
                        DMApplicationFormData = DMapplicationFormData,
                        RepaymentSchedule = RepaymentapplicationFormData, // Set the mapped repayment data
                        KFSFormData = null,  // Initialize the other KFS data properties here
                        KFSFormData_AnnexB = null,
                        KFSFormData_AnnexC = null  // Initialize as null (we will populate it if data is found)
                    };

                    // Check if KFS data is retrieved
                    if (ds2 != null && ds2.Tables.Count > 2 && ds2.Tables[2].Rows.Count > 0)
                    {
                        // Map KFS data and assign to the ViewModel
                        var KFSFormData = MapKFSFormData(ds2.Tables[0]);
                        var KFSFormData_AnnexB = MapKFSFormData_AnnexB(ds2.Tables[1]);
                        var KFSFormData_AnnexC = MapKFSFormData_AnnexC(ds2.Tables[2]);

                        // Assign data to the ViewModel properties
                        viewModel.KFSFormData = KFSFormData;
                        viewModel.KFSFormData_AnnexB = KFSFormData_AnnexB;
                        viewModel.KFSFormData_AnnexC = KFSFormData_AnnexC ?? new List<KFSLoanInfo>(); // Ensure it's initialized even if empty
                    }

                    return PartialView("_BL_DocEsign_Second", viewModel);
                }
                else
                {
                    // Handle case when application data is not found
                    return PartialView(new ApplicationAndKFSViewModel
                    {
                        ApplicationFormData = null,
                        RepaymentSchedule = null,
                        KFSFormData = null,
                        KFSFormData_AnnexB = null,
                        KFSFormData_AnnexC = new List<KFSLoanInfo>() // Initialize empty list
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (use your preferred logging method)
                // LogError(ex);

                // Handle exception and return an empty model
                return PartialView(new ApplicationAndKFSViewModel
                {
                    ApplicationFormData = null,
                    RepaymentSchedule = null,
                    KFSFormData = null,
                    KFSFormData_AnnexB = null,
                    KFSFormData_AnnexC = new List<KFSLoanInfo>() // Initialize empty list
                });
            }
        }
        private List<RepaymentDetails> MapRepaymentSchedule(DataTable table)
        {
            var repaymentDetailsList = new List<RepaymentDetails>();

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    var repaymentDetails = new RepaymentDetails
                    {
                        ApplicationID_RS = row["ApplicationID"].ToString(),
                        InstNo_RS = row["InstNo"] != DBNull.Value ? Convert.ToInt32(row["InstNo"]) : 0,
                        OutstandingPrincipal_RS = row["OutstandingPrincipal"] != DBNull.Value ? Convert.ToDecimal(row["OutstandingPrincipal"]) : 0,
                        Principal_RS = row["Principal"] != DBNull.Value ? Convert.ToDecimal(row["Principal"]) : 0,
                        Interest_RS = row["Interest"] != DBNull.Value ? Convert.ToDecimal(row["Interest"]) : 0,
                        Amount_RS = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0
                    };

                    repaymentDetailsList.Add(repaymentDetails);
                }
                catch (Exception ex)
                {
                    // Log row-specific errors if needed
                    // LogError(ex);
                }
            }

            return repaymentDetailsList;
        }

        private Leegality_DataFill_Models MapDMFormData(DataTable table)
        {
            var DMapplicationFormData = new Leegality_DataFill_Models();


            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data

                DMapplicationFormData.ApplicationID_DM = row["ApplicationID"].ToString();
                DMapplicationFormData.PrimaryApplicantName_DM = row["PrimaryApplicantName"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentAddress1_DM = row["PrimaryApplicantPresentAddress1"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentAddress2_DM = row["PrimaryApplicantPresentAddress2"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentAddress3_DM = row["PrimaryApplicantPresentAddress3"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentAddress4_DM = row["PrimaryApplicantPresentAddress4"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentPost_DM = row["PrimaryApplicantPresentPost"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentCity_DM = row["PrimaryApplicantPresentCity"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentDistrict_DM = row["PrimaryApplicantPresentDistrict"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentPinCode_DM = row["PrimaryApplicantPresentPinCode"].ToString();
                DMapplicationFormData.PrimaryApplicantPresentState_DM = row["PrimaryApplicantPresentState"].ToString();
                DMapplicationFormData.BranchAddress1_DM = row["BranchAddress1"].ToString();
                DMapplicationFormData.BranchAddress2_DM = row["BranchAddress2"].ToString();
                DMapplicationFormData.BranchAddress3_DM = row["BranchAddress3"].ToString();
                DMapplicationFormData.BranchAddress4_DM = row["BranchAddress4"].ToString();
                DMapplicationFormData.ApplicationDate_DM = row["ApplicationDate"].ToString();
                DMapplicationFormData.SanctionedLoanAmt_DM = row["SanctionedLoanAmt"].ToString();
                DMapplicationFormData.SanctionedLoanAmtWords_DM = row["SanctionedLoanAmtWords"].ToString();
                DMapplicationFormData.AgreementDate_DM = row["AgreementDate"].ToString();
                DMapplicationFormData.BankName_DM = row["BankName"].ToString();
                DMapplicationFormData.BankBranch_DM = row["BankBranch"].ToString();
                DMapplicationFormData.AccountHolder_DM = row["AccountHolder"].ToString();
                DMapplicationFormData.AccountNumber_DM = row["AccountNumber"].ToString();
                DMapplicationFormData.IFSC_DM = row["IFSC"].ToString();


            }

            return DMapplicationFormData;

        }
        private Leegality_DataFill_Models MapAnnxFormData(DataTable table)
        {
            var AnnexureapplicationFormData = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data

                AnnexureapplicationFormData.ApplicationID_annx = row["ApplicationID"].ToString();
                AnnexureapplicationFormData.BranchAddress1_annx = row["BranchAddress1"].ToString();
                AnnexureapplicationFormData.BranchAddress2_annx = row["BranchAddress2"].ToString();
                AnnexureapplicationFormData.BranchAddress3_annx = row["BranchAddress3"].ToString();
                AnnexureapplicationFormData.BranchAddress4_annx = row["BranchAddress4"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantName_annx = row["PrimaryApplicantName"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantGuardianType_annx = row["PrimaryApplicantGuardianType"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantGuardian_annx = row["PrimaryApplicantGuardian"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantAgeDOB_annx = row["PrimaryApplicantAgeDOB"].ToString();

                AnnexureapplicationFormData.PrimaryApplicantPresentAddress1_annx = row["PrimaryApplicantPresentAddress1"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentAddress2_annx = row["PrimaryApplicantPresentAddress2"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentAddress3_annx = row["PrimaryApplicantPresentAddress3"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentAddress4_annx = row["PrimaryApplicantPresentAddress4"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentPost_annx = row["PrimaryApplicantPresentPost"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentCity_annx = row["PrimaryApplicantPresentCity"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentDistrict_annx = row["PrimaryApplicantPresentDistrict"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentPinCode_annx = row["PrimaryApplicantPresentPinCode"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantPresentState_annx = row["PrimaryApplicantPresentState"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantConstitution_annx = row["PrimaryApplicantConstitution"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantMobile_annx = row["PrimaryApplicantMobile"].ToString();
                AnnexureapplicationFormData.PrimaryApplicantEmail_annx = row["PrimaryApplicantEmail"].ToString();

                AnnexureapplicationFormData.CoApplicant1_Name_annx = row["CoApplicant1_Name"].ToString();
                AnnexureapplicationFormData.CoApplicant1_AgeDOB_annx = row["CoApplicant1_AgeDOB"].ToString();
                AnnexureapplicationFormData.CoApplicant1_FatherName_annx = row["CoApplicant1_FatherName"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Mobile_annx = row["CoApplicant1_Mobile"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Email_annx = row["CoApplicant1_Email"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Address1_annx = row["CoApplicant1_Address1"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Address2_annx = row["CoApplicant1_Address2"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Address3_annx = row["CoApplicant1_Address3"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Address4_annx = row["CoApplicant1_Address4"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Post_annx = row["CoApplicant1_Post"].ToString();
                AnnexureapplicationFormData.CoApplicant1_City_annx = row["CoApplicant1_City"].ToString();
                AnnexureapplicationFormData.CoApplicant1_District_annx = row["CoApplicant1_District"].ToString();
                AnnexureapplicationFormData.CoApplicant1_PinCode_annx = row["CoApplicant1_PinCode"].ToString();
                AnnexureapplicationFormData.CoApplicant1_State_annx = row["CoApplicant1_State"].ToString();
                AnnexureapplicationFormData.CoApplicant1_Constitution_annx = row["CoApplicant1_Constitution"].ToString();

                AnnexureapplicationFormData.CoApplicant2_Name_annx = row["CoApplicant2_Name"].ToString();
                AnnexureapplicationFormData.CoApplicant2_AgeDOB_annx = row["CoApplicant2_AgeDOB"].ToString();
                AnnexureapplicationFormData.CoApplicant2_FatherName_annx = row["CoApplicant2_FatherName"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Mobile_annx = row["CoApplicant2_Mobile"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Email_annx = row["CoApplicant2_Email"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Address1_annx = row["CoApplicant2_Address1"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Address2_annx = row["CoApplicant2_Address2"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Address3_annx = row["CoApplicant2_Address3"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Address4_annx = row["CoApplicant2_Address4"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Post_annx = row["CoApplicant2_Post"].ToString();
                AnnexureapplicationFormData.CoApplicant2_City_annx = row["CoApplicant2_City"].ToString();
                AnnexureapplicationFormData.CoApplicant2_District_annx = row["CoApplicant2_District"].ToString();
                AnnexureapplicationFormData.CoApplicant2_PinCode_annx = row["CoApplicant2_PinCode"].ToString();
                AnnexureapplicationFormData.CoApplicant2_State_annx = row["CoApplicant2_State"].ToString();
                AnnexureapplicationFormData.CoApplicant2_Constitution_annx = row["CoApplicant2_Constitution"].ToString();

                AnnexureapplicationFormData.CoApplicant3_Name_annx = row["CoApplicant3_Name"].ToString();
                AnnexureapplicationFormData.CoApplicant3_AgeDOB_annx = row["CoApplicant3_AgeDOB"].ToString();
                AnnexureapplicationFormData.CoApplicant3_FatherName_annx = row["CoApplicant3_FatherName"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Mobile_annx = row["CoApplicant3_Mobile"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Email_annx = row["CoApplicant3_Email"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Address1_annx = row["CoApplicant3_Address1"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Address2_annx = row["CoApplicant3_Address2"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Address3_annx = row["CoApplicant3_Address3"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Address4_annx = row["CoApplicant3_Address4"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Post_annx = row["CoApplicant3_Post"].ToString();
                AnnexureapplicationFormData.CoApplicant3_City_annx = row["CoApplicant3_City"].ToString();
                AnnexureapplicationFormData.CoApplicant3_District_annx = row["CoApplicant3_District"].ToString();
                AnnexureapplicationFormData.CoApplicant3_PinCode_annx = row["CoApplicant3_PinCode"].ToString();
                AnnexureapplicationFormData.CoApplicant3_State_annx = row["CoApplicant3_State"].ToString();
                AnnexureapplicationFormData.CoApplicant3_Constitution_annx = row["CoApplicant3_Constitution"].ToString();

            }

            return AnnexureapplicationFormData;

        }
        private Leegality_DataFill_Models MapTCFormData(DataTable table)
        {
            var TermsndconditionsapplicationFormData = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data

                TermsndconditionsapplicationFormData.ApplicationID_TC = row["ApplicationID"].ToString();
                TermsndconditionsapplicationFormData.PurposeOfLoan_TC = row["PurposeOfLoan"].ToString();
                TermsndconditionsapplicationFormData.RateOfInterest_TC = row["RateOfInterest"].ToString();
                TermsndconditionsapplicationFormData.BokenIntRate_TC = row["BokenIntRate"].ToString();
                TermsndconditionsapplicationFormData.LoanDisbursementBranch_TC = row["LoanDisbursementBranch"].ToString();
                TermsndconditionsapplicationFormData.TotalInterestCharge_TC = row["TotalInterestCharge"].ToString();
                TermsndconditionsapplicationFormData.PenalInterest_TC = row["PenalInterest"].ToString();
                TermsndconditionsapplicationFormData.TenureOfLoan_TC = row["TenureOfLoan"].ToString();
                TermsndconditionsapplicationFormData.RepaymentFrequency_TC = row["RepaymentFrequency"].ToString();
                TermsndconditionsapplicationFormData.NumberOfEMI_EMIAmount_TC = row["NumberOfEMI_EMIAmount"].ToString();
                TermsndconditionsapplicationFormData.NumberOfAdvanceInstallment_TC = row["NumberOfAdvanceInstallment"].ToString();
                TermsndconditionsapplicationFormData.MannerOfPayment_TC = row["MannerOfPayment"].ToString();
                TermsndconditionsapplicationFormData.DueDate_TC = row["DueDate"].ToString();
                TermsndconditionsapplicationFormData.DateOfCommencementEMI_TC = row["DateOfCommencementEMI"].ToString();
                TermsndconditionsapplicationFormData.DueDateOfPaymentOfFirstEMI_TC = row["DueDateOfPaymentOfFirstEMI"].ToString();
                TermsndconditionsapplicationFormData.DueDateOfPaymentOfLastEMI_TC = row["DueDateOfPaymentOfLastEMI"].ToString();
                TermsndconditionsapplicationFormData.ModeOfRepayment_TC = row["ModeOfRepayment"].ToString();
                TermsndconditionsapplicationFormData.PrepaymentCharges_TC = row["PrepaymentCharges"].ToString();
                TermsndconditionsapplicationFormData.LoanAmount_TC = row["LoanAmount"].ToString();
                TermsndconditionsapplicationFormData.ProcessingCharges_TC = row["ProcessingCharges"].ToString();
                TermsndconditionsapplicationFormData.ServiceCharge_TC = row["ServiceCharge"].ToString();
                TermsndconditionsapplicationFormData.AdvanceEMI_TC = row["AdvanceEMI"].ToString();
                TermsndconditionsapplicationFormData.NetDisbursedAmount_TC = row["NetDisbursedAmount"].ToString();
                TermsndconditionsapplicationFormData.TotalAmountPaidByBorrower_TC = row["TotalAmountPaidByBorrower"].ToString();
                TermsndconditionsapplicationFormData.ECS_PDC_DishonourCharges_TC = row["ECS_PDC_DishonourCharges"].ToString();
                TermsndconditionsapplicationFormData.SubmissionACHMandate_TC = row["SubmissionACHMandate"].ToString();
                TermsndconditionsapplicationFormData.RegisteredLetterCharges_TC = row["RegisteredLetterCharges"].ToString();
                TermsndconditionsapplicationFormData.NormalLetterCharges_TC = row["NormalLetterCharges"].ToString();
                TermsndconditionsapplicationFormData.StatementRepaymentAnnexure_TC = row["StatementRepaymentAnnexure"].ToString();
                TermsndconditionsapplicationFormData.OtherCharges_TC = row["OtherCharges"].ToString();
                TermsndconditionsapplicationFormData.Security_HypothecatedAssets_TC = row["Security_HypothecatedAssets"].ToString();
                TermsndconditionsapplicationFormData.Jurisdiction_TC = row["Jurisdiction"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantName_TC = row["PrimaryApplicantName"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantGuardianType_TC = row["PrimaryApplicantGuardianType"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantGuardian_TC = row["PrimaryApplicantGuardian"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentAddress1_TC = row["PrimaryApplicantPresentAddress1"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentAddress2_TC = row["PrimaryApplicantPresentAddress2"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentAddress3_TC = row["PrimaryApplicantPresentAddress3"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentAddress4_TC = row["PrimaryApplicantPresentAddress4"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentPost_TC = row["PrimaryApplicantPresentPost"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentCity_TC = row["PrimaryApplicantPresentCity"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentDistrict_TC = row["PrimaryApplicantPresentDistrict"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentPinCode_TC = row["PrimaryApplicantPresentPinCode"].ToString();
                TermsndconditionsapplicationFormData.PrimaryApplicantPresentState_TC = row["PrimaryApplicantPresentState"].ToString();

                TermsndconditionsapplicationFormData.OtherDisclosures = row["OtherDisclosures"].ToString();
                TermsndconditionsapplicationFormData.DetailsOfLSP = row["DetailsOfLSP"].ToString();
                TermsndconditionsapplicationFormData.AmortizationOfLoan = row["AmortizationOfLoan"].ToString();

            }

            return TermsndconditionsapplicationFormData;

        }
        private Leegality_DataFill_Models MapAgrmntFormData(DataTable table)
        {
            var AgreementapplicationFormData = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data

                AgreementapplicationFormData.ApplicationID_Agrmnt = row["ApplicationID"].ToString();
                AgreementapplicationFormData.BranchName_Agrmnt = row["BranchName"].ToString();
                AgreementapplicationFormData.AgreementDate_Agrmnt = row["AgreementDate"].ToString();
                AgreementapplicationFormData.AgreementYear_Agrmnt = row["AgreementYear"].ToString();
                AgreementapplicationFormData.AgreementDtInWords = row["AgreementDtInWords"].ToString();
                AgreementapplicationFormData.PrimaryApplicantName_Agrmnt = row["PrimaryApplicantName"].ToString();
                AgreementapplicationFormData.PrimaryApplicantAgeDOB_Agrmnt = row["PrimaryApplicantAgeDOB"].ToString();
                AgreementapplicationFormData.PrimaryApplicantGuardianType_Agrmnt = row["PrimaryApplicantGuardianType"].ToString();
                AgreementapplicationFormData.PrimaryApplicantGuardian_Agrmnt = row["PrimaryApplicantGuardian"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentAddress1_Agrmnt = row["PrimaryApplicantPresentAddress1"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentAddress2_Agrmnt = row["PrimaryApplicantPresentAddress2"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentAddress3_Agrmnt = row["PrimaryApplicantPresentAddress3"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentAddress4_Agrmnt = row["PrimaryApplicantPresentAddress4"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentPost_Agrmnt = row["PrimaryApplicantPresentPost"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentCity_Agrmnt = row["PrimaryApplicantPresentCity"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentDistrict_Agrmnt = row["PrimaryApplicantPresentDistrict"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentPinCode_Agrmnt = row["PrimaryApplicantPresentPinCode"].ToString();
                AgreementapplicationFormData.PrimaryApplicantPresentState_Agrmnt = row["PrimaryApplicantPresentState"].ToString();
                AgreementapplicationFormData.CoApplicant1_Name_Agrmnt = row["CoApplicant1_Name"].ToString();
                AgreementapplicationFormData.CoApplicant1_AgeDOB_Agrmnt = row["CoApplicant1_AgeDOB"].ToString();
                AgreementapplicationFormData.CoApplicant1_GuardianType_Agrmnt = row["CoApplicant1_GuardianType"].ToString();
                AgreementapplicationFormData.CoApplicant1_Guardian_Agrmnt = row["CoApplicant1_Guardian"].ToString();
                AgreementapplicationFormData.CoApplicant1_Address1_Agrmnt = row["CoApplicant1_Address1"].ToString();
                AgreementapplicationFormData.CoApplicant1_Address2_Agrmnt = row["CoApplicant1_Address2"].ToString();
                AgreementapplicationFormData.CoApplicant1_Address3_Agrmnt = row["CoApplicant1_Address3"].ToString();
                AgreementapplicationFormData.CoApplicant1_Address4_Agrmnt = row["CoApplicant1_Address4"].ToString();
                AgreementapplicationFormData.CoApplicant1_Post_Agrmnt = row["CoApplicant1_Post"].ToString();
                AgreementapplicationFormData.CoApplicant1_City_Agrmnt = row["CoApplicant1_City"].ToString();
                AgreementapplicationFormData.CoApplicant1_District_Agrmnt = row["CoApplicant1_District"].ToString();
                AgreementapplicationFormData.CoApplicant1_PinCode_Agrmnt = row["CoApplicant1_PinCode"].ToString();
                AgreementapplicationFormData.CoApplicant1_State_Agrmnt = row["CoApplicant1_State"].ToString();
                AgreementapplicationFormData.CoApplicant2_Name_Agrmnt = row["CoApplicant2_Name"].ToString();
                AgreementapplicationFormData.CoApplicant2_AgeDOB_Agrmnt = row["CoApplicant2_AgeDOB"].ToString();
                AgreementapplicationFormData.CoApplicant2_GuardianType_Agrmnt = row["CoApplicant2_GuardianType"].ToString();
                AgreementapplicationFormData.CoApplicant2_Guardian_Agrmnt = row["CoApplicant2_Guardian"].ToString();
                AgreementapplicationFormData.CoApplicant2_Address1_Agrmnt = row["CoApplicant2_Address1"].ToString();
                AgreementapplicationFormData.CoApplicant2_Address2_Agrmnt = row["CoApplicant2_Address2"].ToString();
                AgreementapplicationFormData.CoApplicant2_Address3_Agrmnt = row["CoApplicant2_Address3"].ToString();
                AgreementapplicationFormData.CoApplicant2_Address4_Agrmnt = row["CoApplicant2_Address4"].ToString();
                AgreementapplicationFormData.CoApplicant2_Post_Agrmnt = row["CoApplicant2_Post"].ToString();
                AgreementapplicationFormData.CoApplicant2_City_Agrmnt = row["CoApplicant2_City"].ToString();
                AgreementapplicationFormData.CoApplicant2_District_Agrmnt = row["CoApplicant2_District"].ToString();
                AgreementapplicationFormData.CoApplicant2_PinCode_Agrmnt = row["CoApplicant2_PinCode"].ToString();
                AgreementapplicationFormData.CoApplicant2_State_Agrmnt = row["CoApplicant2_State"].ToString();
                AgreementapplicationFormData.CoApplicant3_Name_Agrmnt = row["CoApplicant3_Name"].ToString();
                AgreementapplicationFormData.CoApplicant3_AgeDOB_Agrmnt = row["CoApplicant3_AgeDOB"].ToString();
                AgreementapplicationFormData.CoApplicant3_GuardianType_Agrmnt = row["CoApplicant3_GuardianType"].ToString();
                AgreementapplicationFormData.CoApplicant3_Guardian_Agrmnt = row["CoApplicant3_Guardian"].ToString();
                AgreementapplicationFormData.CoApplicant3_Address1_Agrmnt = row["CoApplicant3_Address1"].ToString();
                AgreementapplicationFormData.CoApplicant3_Address2_Agrmnt = row["CoApplicant3_Address2"].ToString();
                AgreementapplicationFormData.CoApplicant3_Address3_Agrmnt = row["CoApplicant3_Address3"].ToString();
                AgreementapplicationFormData.CoApplicant3_Address4_Agrmnt = row["CoApplicant3_Address4"].ToString();
                AgreementapplicationFormData.CoApplicant3_Post_Agrmnt = row["CoApplicant3_Post"].ToString();
                AgreementapplicationFormData.CoApplicant3_City_Agrmnt = row["CoApplicant3_City"].ToString();
                AgreementapplicationFormData.CoApplicant3_District_Agrmnt = row["CoApplicant3_District"].ToString();
                AgreementapplicationFormData.CoApplicant3_PinCode_Agrmnt = row["CoApplicant3_PinCode"].ToString();
                AgreementapplicationFormData.CoApplicant3_State_Agrmnt = row["CoApplicant3_State"].ToString();



            }

            return AgreementapplicationFormData;

        }
        private Leegality_DataFill_Models MapDPNFormData(DataTable table)
        {
            var DPNapplicationFormData = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data
                DPNapplicationFormData.ApplicationID_DPN = row["ApplicationID"].ToString();
                DPNapplicationFormData.SanctionedLoanAmt_DPN = row["SanctionedLoanAmt"].ToString();
                DPNapplicationFormData.SanctionedLoanAmtWords_DPN = row["SanctionedLoanAmtWords"].ToString();
                DPNapplicationFormData.RateOfInterest_DPN = row["RateOfInterest"].ToString();
                DPNapplicationFormData.Place_DPN = row["Place"].ToString();
                DPNapplicationFormData.DPNDate_DPN = row["DPNDate"].ToString();
                DPNapplicationFormData.PrimaryApplicantName_DPN = row["PrimaryApplicantName"].ToString();
                DPNapplicationFormData.PrimaryApplicantAgeDOB_DPN = row["PrimaryApplicantAgeDOB"].ToString();
                DPNapplicationFormData.PrimaryApplicantGuardianType_DPN = row["PrimaryApplicantGuardianType"].ToString();
                DPNapplicationFormData.PrimaryApplicantGuardian_DPN = row["PrimaryApplicantGuardian"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentAddress1_DPN = row["PrimaryApplicantPresentAddress1"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentAddress2_DPN = row["PrimaryApplicantPresentAddress2"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentAddress3_DPN = row["PrimaryApplicantPresentAddress3"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentAddress4_DPN = row["PrimaryApplicantPresentAddress4"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentPost_DPN = row["PrimaryApplicantPresentPost"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentCity_DPN = row["PrimaryApplicantPresentCity"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentDistrict_DPN = row["PrimaryApplicantPresentDistrict"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentPinCode_DPN = row["PrimaryApplicantPresentPinCode"].ToString();
                DPNapplicationFormData.PrimaryApplicantPresentState_DPN = row["PrimaryApplicantPresentState"].ToString();

                DPNapplicationFormData.CoApplicant1_Name_DPN = row["CoApplicant1_Name"].ToString();
                DPNapplicationFormData.CoApplicant1_AgeDOB_DPN = row["CoApplicant1_AgeDOB"].ToString();
                DPNapplicationFormData.CoApplicant1_GuardianType_DPN = row["CoApplicant1_GuardianType"].ToString();
                DPNapplicationFormData.CoApplicant1_Guardian_DPN = row["CoApplicant1_Guardian"].ToString();
                DPNapplicationFormData.CoApplicant1_Address1_DPN = row["CoApplicant1_Address1"].ToString();
                DPNapplicationFormData.CoApplicant1_Address2_DPN = row["CoApplicant1_Address2"].ToString();
                DPNapplicationFormData.CoApplicant1_Address3_DPN = row["CoApplicant1_Address3"].ToString();
                DPNapplicationFormData.CoApplicant1_Address4_DPN = row["CoApplicant1_Address4"].ToString();
                DPNapplicationFormData.CoApplicant1_Post_DPN = row["CoApplicant1_Post"].ToString();
                DPNapplicationFormData.CoApplicant1_City_DPN = row["CoApplicant1_City"].ToString();
                DPNapplicationFormData.CoApplicant1_District_DPN = row["CoApplicant1_District"].ToString();
                DPNapplicationFormData.CoApplicant1_PinCode_DPN = row["CoApplicant1_PinCode"].ToString();
                DPNapplicationFormData.CoApplicant1_State_DPN = row["CoApplicant1_State"].ToString();

                DPNapplicationFormData.CoApplicant2_Name_DPN = row["CoApplicant2_Name"].ToString();
                DPNapplicationFormData.CoApplicant2_AgeDOB_DPN = row["CoApplicant2_AgeDOB"].ToString();
                DPNapplicationFormData.CoApplicant2_GuardianType_DPN = row["CoApplicant2_GuardianType"].ToString();
                DPNapplicationFormData.CoApplicant2_Guardian_DPN = row["CoApplicant2_Guardian"].ToString();
                DPNapplicationFormData.CoApplicant2_Address1_DPN = row["CoApplicant2_Address1"].ToString();
                DPNapplicationFormData.CoApplicant2_Address2_DPN = row["CoApplicant2_Address2"].ToString();
                DPNapplicationFormData.CoApplicant2_Address3_DPN = row["CoApplicant2_Address3"].ToString();
                DPNapplicationFormData.CoApplicant2_Address4_DPN = row["CoApplicant2_Address4"].ToString();
                DPNapplicationFormData.CoApplicant2_Post_DPN = row["CoApplicant2_Post"].ToString();
                DPNapplicationFormData.CoApplicant2_City_DPN = row["CoApplicant2_City"].ToString();
                DPNapplicationFormData.CoApplicant2_District_DPN = row["CoApplicant2_District"].ToString();
                DPNapplicationFormData.CoApplicant2_PinCode_DPN = row["CoApplicant2_PinCode"].ToString();
                DPNapplicationFormData.CoApplicant2_State_DPN = row["CoApplicant2_State"].ToString();

                DPNapplicationFormData.CoApplicant3_Name_DPN = row["CoApplicant3_Name"].ToString();
                DPNapplicationFormData.CoApplicant3_AgeDOB_DPN = row["CoApplicant3_AgeDOB"].ToString();
                DPNapplicationFormData.CoApplicant3_GuardianType_DPN = row["CoApplicant3_GuardianType"].ToString();
                DPNapplicationFormData.CoApplicant3_Guardian_DPN = row["CoApplicant3_Guardian"].ToString();
                DPNapplicationFormData.CoApplicant3_Address1_DPN = row["CoApplicant3_Address1"].ToString();
                DPNapplicationFormData.CoApplicant3_Address2_DPN = row["CoApplicant3_Address2"].ToString();
                DPNapplicationFormData.CoApplicant3_Address3_DPN = row["CoApplicant3_Address3"].ToString();
                DPNapplicationFormData.CoApplicant3_Address4_DPN = row["CoApplicant3_Address4"].ToString();
                DPNapplicationFormData.CoApplicant3_Post_DPN = row["CoApplicant3_Post"].ToString();
                DPNapplicationFormData.CoApplicant3_City_DPN = row["CoApplicant3_City"].ToString();
                DPNapplicationFormData.CoApplicant3_District_DPN = row["CoApplicant3_District"].ToString();
                DPNapplicationFormData.CoApplicant3_PinCode_DPN = row["CoApplicant3_PinCode"].ToString();
                DPNapplicationFormData.CoApplicant3_State_DPN = row["CoApplicant3_State"].ToString();



            }

            return DPNapplicationFormData;

        }
        private Leegality_DataFill_Models MapSanctionLetterFormData(DataTable table)
        {
            var applicationFormData = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data
                applicationFormData.ApplicationID_SL = row["ApplicationID"].ToString();
                applicationFormData.PrimaryApplicantName_SL = row["PrimaryApplicantName"].ToString();
                applicationFormData.CoApplicant1_Name_SL = row["CoApplicant1_Name"].ToString();
                applicationFormData.CoApplicant2_Name_SL = row["CoApplicant2_Name"].ToString();
                applicationFormData.CoApplicant3_Name_SL = row["CoApplicant3_Name"].ToString();
                applicationFormData.SanctionNumber_SL = row["SanctionNumber"].ToString();
                applicationFormData.SanctionDate_SL = row["SanctionDate"].ToString();
                applicationFormData.SanctionedLoanAmt_SL = row["SanctionedLoanAmt"].ToString();
                applicationFormData.SanctionedLoanAmtWords_SL = row["SanctionedLoanAmtWords"].ToString();
                applicationFormData.NoOfMonths_SL = row["NoOfMonths"].ToString();
                applicationFormData.EMIAmt_SL = row["EMIAmt"].ToString();
                applicationFormData.EMIAmtWords_SL = row["EMIAmtWords"].ToString();


        
             }

            return applicationFormData;

        }
        // Map KFS data into KFSLoanInfo model
        private KFSLoanInfo MapKFSFormData(DataTable table)
        {
            var KFSForm = new KFSLoanInfo();

            if (table.Rows.Count > 0)
            {
                // Assuming that table has appropriate columns and mapping to KFSLoanInfo
                var row = table.Rows[0];

                // Application and Loan Information
                KFSForm.ApplicationId = row["application_id"].ToString();
                KFSForm.LoanType = row["loan_type"].ToString();
                //KFSForm.LoanAmount = Convert.ToDecimal(row["loan_amt"]);
                KFSForm.LoanAmount = row["loan_amt"].ToString();
                KFSForm.DisbursementStages = row["Disbursement_stages"].ToString();
                KFSForm.Schedule = row["Schedule"].ToString();
                //KFSForm.Duration = Convert.ToInt32(row["Duration"]);
                KFSForm.Duration = row["Duration"].ToString();
                KFSForm.Instalment = row["Instalment"].ToString();
                KFSForm.NumberOfEPIs = row["Number_of_EPIs"].ToString();
                KFSForm.EPI = row["EPI"].ToString();
                KFSForm.CommencementOfRepayment = row["Commencement_of_repayment"].ToString();
                KFSForm.InterestRate = row["Interest_rate"].ToString();
                KFSForm.InterestRateType = row["Interest_rate_type"].ToString();
                KFSForm.RefeBenchmark = row["refeBenchMark"].ToString();

                KFSForm.BenchmarkRate = row["benchMarkRate"].ToString();
                KFSForm.Spread = row["spread"].ToString();

                KFSForm.EPI = row["EPI"].ToString();
                KFSForm.FinalRate = row["finalRate"].ToString();
                KFSForm.EPI = row["EPI"].ToString();
                KFSForm.ResetPeriodB = row["resetPeriod_B"].ToString();
                KFSForm.ResetPeriodS = row["resetPeriod_S"].ToString();

                // Recurring and One-time Processing Fees
                KFSForm.BenchEPI = row["Bench_EPI"].ToString();
                KFSForm.BenchNoEPI = row["Bench_no_EPI"].ToString();

                KFSForm.REAProcessingRecurring = row["RE_A_Processing_recurring"].ToString();
                KFSForm.REAProcessingAmount = row["RE_A_Processing_amount"].ToString();
                KFSForm.REBProcessingRecurring = row["RE_B_Processing_recurring"].ToString();
                KFSForm.REBProcessingAmount = row["RE_B_Processing_amount"].ToString();

                KFSForm.REAInsuranceRecurring = row["RE_A_Insurance_recurring"].ToString();
                KFSForm.REAInsuranceAmount = row["RE_A_Insurance_amount"].ToString();
                KFSForm.REBInsuranceRecurring = row["RE_B_Insurance_recurring"].ToString();
                KFSForm.REBInsuranceAmount = row["RE_B_Insurance_amount"].ToString();

                KFSForm.REAServiceRecurring = row["RE_A_Service_recurring"].ToString();
                KFSForm.REAServiceAmount = row["RE_A_Service_amount"].ToString();
                KFSForm.REBServiceRecurring = row["RE_B_Service_recurring"].ToString();
                KFSForm.REBServiceAmount = row["RE_B_Service_amount"].ToString();

                KFSForm.REAeSigningRecurring = row["RE_A_eSigning_recurring"].ToString();
                KFSForm.REAeSigningAmount = row["RE_A_eSigning_amount"].ToString();
                KFSForm.REBeSigningRecurring = row["RE_B_eSigning_recurring"].ToString();
                KFSForm.REBeSigningAmount = row["RE_B_eSigning_amount"].ToString();

                KFSForm.REAStampRecurring = row["RE_A_stamp_recurring"].ToString();
                KFSForm.REAStampAmount = row["RE_A_stamp_amount"].ToString();
                KFSForm.REBStampRecurring = row["RE_B_stamp_recurring"].ToString();
                KFSForm.REBStampAmount = row["RE_B_stamp_amount"].ToString();

                KFSForm.REAAppRecurring = row["RE_A_app_recurring"].ToString();
                KFSForm.REAAppAmount = row["RE_A_app_amount"].ToString();
                KFSForm.REBAppRecurring = row["RE_B_app_recurring"].ToString();
                KFSForm.REBAppAmount = row["RE_B_app_amount"].ToString();

                KFSForm.REAeStamp = row["RE_A_eStamp"].ToString();
                KFSForm.REAeStampAmount = row["RE_A_eStamp_amount"].ToString();
                KFSForm.REBeStampRecurring = row["RE_B_eStamp_recurring"].ToString();
                KFSForm.REBeStampAmount = row["RE_B_eStamp_amount"].ToString();

                KFSForm.REABureau = row["RE_A_Bureau"].ToString();
                KFSForm.REABureauAmount = row["RE_A_Bureau_amount"].ToString();
                KFSForm.REBBureauRecurring = row["RE_B_Bureau_recurring"].ToString();
                KFSForm.REBBureauAmount = row["RE_B_Bureau_amount"].ToString();

                KFSForm.REAValuatioRecurring = row["RE_A_Valuatio_recurring"].ToString();
                KFSForm.REAValuatioAmount = row["RE_A_Valuatio_amount"].ToString();
                KFSForm.REBValuatioRecurring = row["RE_B_Valuatio_recurring"].ToString();
                KFSForm.REBValuatioAmount = row["RE_B_Valuatio_amount"].ToString();

                KFSForm.REAothRecurring = row["RE_A_oth_recurring"].ToString();
                KFSForm.REAothAmount = row["RE_A_oth_amount"].ToString();
                KFSForm.REBothRecurring = row["RE_B_oth_recurring"].ToString();
                KFSForm.REBothAmount = row["RE_B_oth_amount"].ToString();

                KFSForm.REABrokRecurring = row["RE_A_brok_recurring"].ToString();
                KFSForm.REABrokAmount = row["RE_A_brok_amount"].ToString();
                KFSForm.REBBrokRecurring = row["RE_B_brok_recurring"].ToString();
                KFSForm.REBBrokAmount = row["RE_B_brok_amount"].ToString();

                KFSForm.REAAdRecurring = row["RE_A_ad_recurring"].ToString();
                KFSForm.REAAdAmount = row["RE_A_ad_amount"].ToString();
                KFSForm.REBAdRecurring = row["RE_B_ad_recurring"].ToString();
                KFSForm.REBAdAmount = row["RE_B_ad_amount"].ToString();


                // Additional Charges and Fees
                KFSForm.APR = row["APR"].ToString();
                KFSForm.PenalCharge = row["Penal_charge"].ToString();
                KFSForm.OtherPenalCharge = row["other_penal_charge"].ToString();
                KFSForm.ForeclosureCharge = row["foreclosure_charge"].ToString();
                KFSForm.SwitchingCharge = row["switching_charge"].ToString();
                KFSForm.OtherCharge = row["other_charge"].ToString();
                KFSForm.NoticeCharge = row["notice_charge"].ToString();
                KFSForm.PostageCharge = row["Postage_charge"].ToString();
                KFSForm.AdvertisementCharge = row["Advertisement_charge"].ToString();
                KFSForm.MinInt = row["minint"].ToString();
                KFSForm.Recovery = row["recovery"].ToString();
                KFSForm.Cheque = row["cheque"].ToString();

                //Part-2
                KFSForm.ClauseAgent = row["Clause_agent"].ToString();
                KFSForm.ClauseRedressalMechanism = row["Clause_redressal_mechanism"].ToString();
                KFSForm.PhoneEmailNodal = row["Phone_email_nodal"].ToString();
                KFSForm.TransferToRE = row["transfer_to_RE"].ToString();
                // Partner and Originating RE Information
                KFSForm.NameOfOriginatingRE = row["Name_of_originating_RE"].ToString();
                KFSForm.NameOfPartnerRE = row["Name_of_partner_RE"].ToString();
                // Interest and Rate Information
                KFSForm.BlendedRateOfInterest = row["Blended_rate_of_interest"].ToString();
                KFSForm.LookUpPeriod = row["look_up_period"].ToString();
                // LSP Details
                KFSForm.DetailsOfLSP = row["Details_of_LSP"].ToString();

                // Branch Information
                KFSForm.Branch = row["branch"].ToString();

                // Date and Other Information
                KFSForm.PrintDate = row["printdt"].ToString();


            }

            return KFSForm;
        }
        private KFSLoanInfo MapKFSFormData_AnnexB(DataTable table)
        {
            var KFSForm = new KFSLoanInfo();

            if (table.Rows.Count > 0)
            {
                // Assuming that table has appropriate columns and mapping to KFSLoanInfo
                var row = table.Rows[0];

                KFSForm.LoanAmt = row["loan_amt"].ToString();
                KFSForm.Tenure = row["Tenure"].ToString();
                KFSForm.NoOfInstallments = row["no_of_instalment"].ToString();
                KFSForm.EpiAmount = row["EPI_amount"].ToString();
                KFSForm.NoOfInstallmentsInt = row["no_of_instalment_int"].ToString();
                KFSForm.CommencementOfRepayments = row["Commencement_of_repayments"].ToString();
                KFSForm.InterestRateType = row["Interest_rate_type"].ToString();
                KFSForm.RateOfInterest = row["Rate_of_Interest"].ToString();
                KFSForm.TotalInterestAmount = row["Total_Interest_Amount"].ToString();
                KFSForm.FeeCharges = row["Fee_Charges"].ToString();
                KFSForm.PayableToRE = row["Payable_to_the_RE"].ToString();
                KFSForm.PayableToThirdPartyRoutedThroughRE = row["Payable_to_third_party_routed_through_RE"].ToString();
                KFSForm.NetDisbursedAmount = row["Net_disbursed_amount"].ToString();
                KFSForm.TotalAmountToBePaid = row["Total_amount_to_be_paid"].ToString();
                KFSForm.AnnualPercentageRate = row["Annual_Percentage_rate"].ToString();
                KFSForm.ScheduleOfDisbursement = row["Schedule_of_disbursement"].ToString();
                KFSForm.DueDate = row["Due_date"].ToString();

            }

            return KFSForm;
        }
        public List<KFSLoanInfo> MapKFSFormData_AnnexC(DataTable dt)
        {
            var list = new List<KFSLoanInfo>();

            foreach (DataRow row in dt.Rows)
            {
                var data = new KFSLoanInfo
                {
                    InstalmentNo = row["Instalment No."].ToString(),
                    OutstandingPrincipal = row["Outstanding Principal (in Rupees)"].ToString(),
                    Principal = row["Principal (in Rupees)"].ToString(),
                    Interest = row["Interest (in Rupees)"].ToString(),
                    Instalment = row["Instalment (in Rupees)"].ToString()
                };

                list.Add(data);
            }

            return list;
        }

        private List<Dictionary<string, object>> GetTableData(DataTable table)
        {
            return table.AsEnumerable().Select(row =>
            {
                var dictionary = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    dictionary[column.ColumnName] = row[column] ?? DBNull.Value; // Handle null values
                }
                return dictionary;
            }).ToList();
        }



        //[HttpPost]
        //public async Task<ActionResult> SendDocChunkToLeegality(HttpPostedFileBase chunk, string filename, int chunkNumber, int totalChunks, string applicationId)
        //{
        //    try
        //    {
        //        var chunkDir = Path.Combine(Server.MapPath("~/App_Data/Chunks"), applicationId);
        //        if (!Directory.Exists(chunkDir))
        //        {
        //            Directory.CreateDirectory(chunkDir);
        //        }

        //        var chunkPath = Path.Combine(chunkDir, $"{chunkNumber}-{filename}");
        //        chunk.SaveAs(chunkPath);

        //        // Check if all chunks have been received
        //        var chunkFiles = Directory.GetFiles(chunkDir);
        //        if (chunkFiles.Length == totalChunks)
        //        {
        //            // Combine chunks into a single file
        //            var finalFilePath = Path.Combine(Server.MapPath("~/App_Data/Uploads"), filename);
        //            using (var finalFile = new FileStream(finalFilePath, FileMode.Create))
        //            {
        //                foreach (var chunkFile in chunkFiles.OrderBy(f => int.Parse(Path.GetFileName(f).Split('-')[0])))
        //                {
        //                    var bytes = System.IO.File.ReadAllBytes(chunkFile);
        //                    finalFile.Write(bytes, 0, bytes.Length);
        //                    System.IO.File.Delete(chunkFile); // Delete chunk after appending
        //                }
        //            }

        //            // Delete chunk directory after completion
        //            Directory.Delete(chunkDir);

        //            // Respond with success
        //            return Json(new { success = true, message = "PDF received successfully." });
        //        }

        //        return Json(new { success = true, message = "Chunk uploaded successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exception
        //        return Json(new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}

        //public async Task<ActionResult> SendDocChunkToLeegality(HttpPostedFileBase chunk, string filename, int chunkNumber, int totalChunks, string applicationId)

        [HttpPost]
        //public async Task<ActionResult> SendDocChunkToLeegality(HttpPostedFileBase pdfFile, string applicationId)
       public async Task<ActionResult> SendDocChunkToLeegality(HttpPostedFileBase chunk, string applicationId)

        {
            if (chunk != null && !string.IsNullOrEmpty(applicationId))
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Read PDF file into a memory stream
                    await chunk.InputStream.CopyToAsync(memoryStream);
                    byte[] pdfBytes = memoryStream.ToArray();

                    // Convert to base64 if required
                    string base64String = Convert.ToBase64String(pdfBytes);

                    // Create the Leegality request
                    Leegality_Business_request leegality_Business_Request = new Leegality_Business_request
                    {
                        profileId = "KZI4WJD",
                        file = new DocumentFile
                        {
                            name = "GeneratedPDF",
                            file = base64String // PDF as base64 string
                        },
                        irn = applicationId // Use the application ID
                    };

                    // Process invitees if needed (assuming you're fetching them from a database)
                    //DataTable dtInvitees = await FetchInviteesAsync(applicationId);
                    //if (dtInvitees != null && dtInvitees.Rows.Count > 0)
                    //{
                    //    leegality_Business_Request.invitees = dtInvitees.AsEnumerable().Select((row, index) => new Invitee
                    //    {
                    //        name = row["coappli_name"].ToString(),
                    //        email = row["email_id"].ToString(),
                    //        phone = index == 0 ? "9446332086" : "9567427023" // Example phone numbers
                    //    }).ToArray();
                    //}

                    // Manually set two invitees for testing
                    leegality_Business_Request.invitees = new[]
                    {
                        new Invitee
                        {
                            name = "SIGI RAJU",
                            email = "sigi@yogloans.com",
                            phone = "9446332086"
                        },
                        new Invitee
                        {
                            name = "ABHILA K S",
                            email = "abhila.chittilappilly@gmail.com",
                            phone = "9567427023"
                        },
                         new Invitee
                        {
                            name = "Anu",
                            email = "itdev@yogloans.com",
                            phone = "7034597062"
                        },
                          new Invitee
                        {
                            name = "Renya K",
                            email = "itdev@yogloans.com",
                            phone = "9061050308"
                        }
                    };


                    // Serialize the request for API use
                    string inputJson = JsonConvert.SerializeObject(leegality_Business_Request);
                    LeegalityApiClient leegalityApiClient = new LeegalityApiClient();

                    // Send request to Leegality
                    string response = await leegalityApiClient.SendLeegalityRequestAsync(leegality_Business_Request);
                    //string response = "{\r\n    \"data\": {\r\n        \"documentId\": \"yixJlag\",\r\n        \"irn\": \"11017623500101\",\r\n        \"invitees\": [\r\n            {\r\n                \"name\": \"SIGI RAJU\",\r\n                \"email\": \"sigi@yogloans.com\",\r\n                \"phone\": \"9446332086\",\r\n                \"signUrl\": \"https://sandbox.leegality.com/sign/d02efdf8-848d-4cc5-8851-cbf0bdc8a75c\",\r\n                \"active\": true,\r\n                \"expiryDate\": \"2024-11-23T18:29:59Z\"\r\n            },\r\n            {\r\n                \"name\": \"ABHILA K S\",\r\n                \"email\": \"abhila.chittilappilly@gmail.com\",\r\n                \"phone\": \"9567427023\",\r\n                \"signUrl\": \"https://sandbox.leegality.com/sign/e2d4442b-ece9-44f7-8d10-60f652c8a425\",\r\n                \"active\": false,\r\n                \"expiryDate\": \"2024-11-23T18:29:59Z\"\r\n            },\r\n            {\r\n                \"name\": \"Anu\",\r\n                \"email\": \"itdev@yogloans.com\",\r\n                \"phone\": \"7034597062\",\r\n                \"signUrl\": \"https://sandbox.leegality.com/sign/ccd34858-48a4-4bec-bdf4-21542b563251\",\r\n                \"active\": false,\r\n                \"expiryDate\": \"2024-11-23T18:29:59Z\"\r\n            },\r\n            {\r\n                \"name\": \"Renya K\",\r\n                \"email\": \"itdev@yogloans.com\",\r\n                \"phone\": \"9061050308\",\r\n                \"signUrl\": \"https://sandbox.leegality.com/sign/aeaab9a4-3adf-46ae-8a8e-16f52562007a\",\r\n                \"active\": false,\r\n                \"expiryDate\": \"2024-11-23T18:29:59Z\"\r\n            }\r\n        ]\r\n    },\r\n    \"messages\": [\r\n        {\r\n            \"code\": \"simpleWorkFlow.success\",\r\n            \"message\": \"Invitations sent successfully.\"\r\n        }\r\n    ],\r\n    \"status\": 1\r\n}";
                    //StringBuilder outMessage = new StringBuilder();
                    string message_out = "";
                    if (response != null)
                    {
                        var rootObject = JsonConvert.DeserializeObject<Leegality_Business_response>(response);
                        string doc_id = rootObject.data.documentId;
                        var app_id = rootObject.data.irn;
                        string invitee1_name = "", invitee1_number = "", invitee1_url = "", invitee2_name = "", invitee2_number = "", invitee2_url = "";
                        string invitee3_name = "", invitee3_number = "", invitee3_url = "", invitee4_name = "", invitee4_number = "", invitee4_url = "";
                        bool invitee1_status, invitee2_status, invitee3_status, invitee4_status;
                        invitee1_name = (rootObject.data.invitees[0].name == null) ? "" : rootObject.data.invitees[0].name;
                        invitee1_number = (rootObject.data.invitees[0].phone == null) ? "" : rootObject.data.invitees[0].phone;
                        invitee1_url = (rootObject.data.invitees[0].signUrl == null) ? "" : rootObject.data.invitees[0].signUrl;
                        invitee1_status = rootObject.data.invitees[0].active; //(rootObject.data.invitees[0].active == null) ? "" : rootObject.data.invitees[0].active;
                        invitee2_name = (rootObject.data.invitees[1].name == null) ? "" : rootObject.data.invitees[1].name;
                        invitee2_number = (rootObject.data.invitees[1].phone == null) ? "" : rootObject.data.invitees[1].phone;
                        invitee2_url = (rootObject.data.invitees[1].signUrl == null) ? "" : rootObject.data.invitees[1].signUrl;
                        invitee2_status = rootObject.data.invitees[1].active;
                        invitee3_name = (rootObject.data.invitees[2].name == null) ? "" : rootObject.data.invitees[2].name;
                        invitee3_number = (rootObject.data.invitees[2].phone == null) ? "" : rootObject.data.invitees[2].phone;
                        invitee3_url = (rootObject.data.invitees[2].signUrl == null) ? "" : rootObject.data.invitees[2].signUrl;
                        invitee3_status = rootObject.data.invitees[2].active;
                        invitee4_name = (rootObject.data.invitees[3].name == null) ? "" : rootObject.data.invitees[3].name;
                        invitee4_number = (rootObject.data.invitees[3].phone == null) ? "" : rootObject.data.invitees[3].phone;
                        invitee4_url = (rootObject.data.invitees[3].signUrl == null) ? "" : rootObject.data.invitees[3].signUrl;
                        invitee4_status = rootObject.data.invitees[3].active;

                        int status_code = (Convert.ToInt16(rootObject.status));
                        message_out = (rootObject.messages[0].message == null) ? "" : rootObject.messages[0].message;

                        //Insert the data into leegality_document_dtl table
                        SqlParameter[] prr = new SqlParameter[23];
                        prr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                        prr[0].Value = 4;
                        prr[1] = new SqlParameter("@application_id", SqlDbType.VarChar, 14);
                        prr[1].Value = app_id;
                        prr[2] = new SqlParameter("@module_id", SqlDbType.Int);
                        prr[2].Value = Session["module_id"].ToString();
                        prr[3] = new SqlParameter("@user", SqlDbType.BigInt);
                        prr[3].Value = Session["login_user"];
                        prr[4] = new SqlParameter("@document_id", SqlDbType.VarChar, 20);
                        prr[4].Value = doc_id;
                        prr[5] = new SqlParameter("@invitee1_name", SqlDbType.VarChar, 50);
                        prr[5].Value = invitee1_name;
                        prr[6] = new SqlParameter("@invitee1_number", SqlDbType.VarChar, 20);
                        prr[6].Value = invitee1_number;
                        prr[7] = new SqlParameter("@invitee1_url", SqlDbType.NVarChar, 100);
                        prr[7].Value = invitee1_url;
                        prr[8] = new SqlParameter("@invitee2_name", SqlDbType.VarChar, 50);
                        prr[8].Value = invitee2_name;
                        prr[9] = new SqlParameter("@invitee2_number", SqlDbType.VarChar, 20);
                        prr[9].Value = invitee2_number;
                        prr[10] = new SqlParameter("@invitee2_url", SqlDbType.NVarChar, 100);
                        prr[10].Value = invitee2_url;
                        prr[11] = new SqlParameter("@invitee3_name", SqlDbType.VarChar, 50);
                        prr[11].Value = invitee3_name;
                        prr[12] = new SqlParameter("@invitee3_number", SqlDbType.VarChar, 20);
                        prr[12].Value = invitee3_number;
                        prr[13] = new SqlParameter("@invitee3_url", SqlDbType.NVarChar, 100);
                        prr[13].Value = invitee3_url;
                        prr[14] = new SqlParameter("@invitee4_name", SqlDbType.VarChar, 50);
                        prr[14].Value = invitee4_name;
                        prr[15] = new SqlParameter("@invitee4_number", SqlDbType.VarChar, 20);
                        prr[15].Value = invitee4_number;
                        prr[16] = new SqlParameter("@invitee4_url", SqlDbType.NVarChar, 100);
                        prr[16].Value = invitee4_url;
                        prr[17] = new SqlParameter("@status_code", SqlDbType.Int);
                        prr[17].Value = status_code;
                        prr[18] = new SqlParameter("@status_msg", SqlDbType.VarChar, 200);
                        prr[18].Value = message_out;
                        prr[19] = new SqlParameter("@invitee1_status", SqlDbType.VarChar, 10);
                        prr[19].Value = (invitee1_name == null) ? "" : Convert.ToString(invitee1_status);
                        prr[20] = new SqlParameter("@invitee2_status", SqlDbType.VarChar, 10);
                        prr[20].Value = (invitee2_name == null) ? "" : Convert.ToString(invitee2_status);
                        prr[21] = new SqlParameter("@invitee3_status", SqlDbType.VarChar, 10);
                        prr[21].Value = (invitee3_name == null) ? "" : Convert.ToString(invitee3_status);
                        prr[22] = new SqlParameter("@invitee4_status", SqlDbType.VarChar, 10);
                        prr[22].Value = (invitee4_name == null) ? "" : Convert.ToString(invitee4_status);

                        dbconnect.Open();
                        dbconnect.ExecuteStoredProcedure("[dbo].[Leegality_queries]", prr);
                        dbconnect.Close();


                        //if (invitee1_name != null && invitee1_name != "")
                        //{
                        //    string invite1Message = "";
                        //    if (invitee1_status)
                        //        invite1Message = "SMS to '" + invitee1_number + "' of '" + invitee1_name + "' sent successfully.";
                        //    else
                        //        invite1Message = "SMS to '" + invitee1_number + "' of '" + invitee1_name + "' not sent!";
                        //    outMessage.AppendLine(invite1Message);
                        //}
                        //if (invitee2_name != null && invitee2_name != "")
                        //{
                        //    string invite2Message = "";
                        //    if (invitee2_status)
                        //        invite2Message = "SMS to '" + invitee2_number + "' of '" + invitee2_name + "' sent successfully.";
                        //    else
                        //        invite2Message = "SMS to '" + invitee2_number + "' of '" + invitee2_name + "' not sent!";
                        //    outMessage.AppendLine(invite2Message);
                        //}
                        //if (invitee3_name != null && invitee3_name != "")
                        //{
                        //    string invite3Message = "";
                        //    if (invitee3_status)
                        //        invite3Message = "SMS to '" + invitee3_number + "' of '" + invitee3_name + "' sent successfully.";
                        //    else
                        //        invite3Message = "SMS to '" + invitee3_number + "' of '" + invitee3_name + "' not sent!";
                        //    outMessage.AppendLine(invite3Message);
                        //}
                        //if (invitee4_name != null && invitee4_name != "")
                        //{
                        //    string invite4Message = "";
                        //    if (invitee4_status)
                        //        invite4Message = "SMS to '" + invitee4_number + "' of '" + invitee4_name + "' sent successfully.";
                        //    else
                        //        invite4Message = "SMS to '" + invitee4_number + "' of '" + invitee4_name + "' not sent!";
                        //    outMessage.AppendLine(invite4Message);
                        //}
                        //string moreDtl = "Further reference note the document Id: " + doc_id;
                        //outMessage.AppendLine(moreDtl);


                    }
                    // Return response back to the frontend
                    return Json(new { success = true, message = message_out });
                }
            }

            return Json(new { success = false, message = "File or application ID is missing" });
        }






    }
}