using System;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using ERP_YOGLOANS_LOCAL.Models.Legality_Model;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.Services.Description;

namespace ERP_YOGLOANS_LOCAL.Controllers.Business_Loan
{
    public class BL_KFS_keralaController : Controller
    {
        DB dbconnect = new DB();
        // GET: BL_KFS_kerala
        public ActionResult BL_DocEsign_First()
        {
            //return GeneratePdf();
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
                    var applicationFormData = MapApplicationFormData(ds1.Tables[0]);

                    // Fetch KFS data
                    SqlParameter[] pr2 = new SqlParameter[3];
                    pr2[0] = new SqlParameter("@module_id", SqlDbType.Int) { Value = 11 };
                    pr2[1] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 3 };
                    pr2[2] = new SqlParameter("@loanno", SqlDbType.BigInt) { Value = 11010423500141 };

                    dbconnect.Open();
                    DataSet ds2 = dbconnect.ExecuteDataset("SP_KFS", pr2);
                    dbconnect.Close();

                    // Create the ViewModel and initialize lists to avoid null values
                    var viewModel = new ApplicationAndKFSViewModel
                    {
                        ApplicationFormData = applicationFormData,
                        KFSFormData = null,  // Initialize the other KFS data properties here
                        KFSFormData_AnnexB = null,
                        KFSFormData_AnnexC = null  // Initialize as null (we will populate it if data is found)
                    };

                    // Check if KFS data is retrieved (making sure Tables[2] exists and has data)
                    if (ds2 != null && ds2.Tables.Count > 2 && ds2.Tables[2].Rows.Count > 0)
                    {
                        // Map KFS data and assign to the ViewModel
                        var KFSFormData = MapKFSFormData(ds2.Tables[0]);
                        var KFSFormData_AnnexB = MapKFSFormData_AnnexB(ds2.Tables[1]);
                        var KFSFormData_AnnexC = MapKFSFormData_AnnexC(ds2.Tables[2]);

                        // Assign data to the ViewModel properties
                        viewModel.KFSFormData = KFSFormData;
                        viewModel.KFSFormData_AnnexB = KFSFormData_AnnexB;
                        viewModel.KFSFormData_AnnexC = KFSFormData_AnnexC ?? new List<KFSLoanInfo>();  // Ensure it's initialized even if empty
                    }

                    return PartialView("_BL_DocEsign_First", viewModel);
                }
                else
                {
                    // Handle case when application data is not found
                    return PartialView(new ApplicationAndKFSViewModel
                    {
                        ApplicationFormData = null,
                        KFSFormData = null,
                        KFSFormData_AnnexB = null,
                        KFSFormData_AnnexC = new List<KFSLoanInfo>()  // Initialize empty list
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle exception and return an empty model
                return PartialView(new ApplicationAndKFSViewModel
                {
                    ApplicationFormData = null,
                    KFSFormData = null,
                    KFSFormData_AnnexB = null,
                    KFSFormData_AnnexC = new List<KFSLoanInfo>()  // Initialize empty list
                });
            }
        }

        private Leegality_DataFill_Models MapApplicationFormData(DataTable table)
        {
            var applicationForm = new Leegality_DataFill_Models();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0]; // Assuming one row of data
                applicationForm.ApplicationID = row["ApplicationID"].ToString();
                applicationForm.BranchName = row["BranchName"].ToString();
                applicationForm.AppliedLoanAmt = Convert.ToDouble(row["AppliedLoanAmt"]);
                applicationForm.AppliedLoanAmtWords = row["AppliedLoanAmtWords"].ToString();
                applicationForm.AppliedLoanAmt1 = Convert.ToDouble(row["AppliedLoanAmt"]);
                applicationForm.AppliedLoanAmtWords1 = row["AppliedLoanAmtWords"].ToString();

                // Primary Applicant Details
                applicationForm.PrimaryApplicantName = row["PrimaryApplicantName"].ToString();
                applicationForm.PrimaryApplicantGuardianType = row["PrimaryApplicantGuardianType"].ToString();
                applicationForm.PrimaryApplicantGuardian = row["PrimaryApplicantGuardian"].ToString();
                applicationForm.PrimaryApplicantGender = row["PrimaryApplicantGender"].ToString();
                applicationForm.PrimaryApplicantAgeDOB = row["PrimaryApplicantAgeDOB"].ToString();
                applicationForm.Mobile = row["Mobile"].ToString();
                applicationForm.PrimaryApplicantPresentAddress1 = row["PrimaryApplicantPresentAddress1"].ToString();
                applicationForm.PrimaryApplicantPresentAddress2 = row["PrimaryApplicantPresentAddress2"].ToString();
                applicationForm.PrimaryApplicantPresentAddress3 = row["PrimaryApplicantPresentAddress3"].ToString();
                applicationForm.PrimaryApplicantPresentAddress4 = row["PrimaryApplicantPresentAddress4"].ToString();
                applicationForm.PrimaryApplicantPresentPost = row["PrimaryApplicantPresentPost"].ToString();
                applicationForm.PrimaryApplicantPresentCity = row["PrimaryApplicantPresentCity"].ToString();
                applicationForm.PrimaryApplicantPresentDistrict = row["PrimaryApplicantPresentDistrict"].ToString();
                applicationForm.PrimaryApplicantPresentPinCode = row["PrimaryApplicantPresentPinCode"].ToString();
                applicationForm.PrimaryApplicantPresentState = row["PrimaryApplicantPresentState"].ToString();
                applicationForm.PrimaryApplicantPermanentAddress1 = row["PrimaryApplicantPermanentAddress1"].ToString();
                applicationForm.PrimaryApplicantPermanentAddress2 = row["PrimaryApplicantPermanentAddress2"].ToString();
                applicationForm.PrimaryApplicantPermanentAddress3 = row["PrimaryApplicantPermanentAddress3"].ToString();
                applicationForm.PrimaryApplicantPermanentAddress4 = row["PrimaryApplicantPermanentAddress4"].ToString();
                applicationForm.PrimaryApplicantPermanentPost = row["PrimaryApplicantPermanentPost"].ToString();
                applicationForm.PrimaryApplicantPermanentCity = row["PrimaryApplicantPermanentCity"].ToString();
                applicationForm.PrimaryApplicantPermanentDistrict = row["PrimaryApplicantPermanentDistrict"].ToString();
                applicationForm.PrimaryApplicantPermanentPinCode = row["PrimaryApplicantPermanentPinCode"].ToString();
                applicationForm.PrimaryApplicantPermanentState = row["PrimaryApplicantPermanentState"].ToString();

                // Co-Applicant 1 Details
                applicationForm.CoApplicant1_Name = row["CoApplicant1_Name"].ToString();
                applicationForm.CoApplicant1_AgeDOB = row["CoApplicant1_AgeDOB"].ToString();
                applicationForm.CoApplicant1_Sex = row["CoApplicant1_Sex"].ToString();
                applicationForm.CoApplicant1_Relation = row["CoApplicant1_Relation"].ToString();
                applicationForm.CoApplicant1_Occupation = row["CoApplicant1_Occupation"].ToString();
                applicationForm.CoApplicant1_Mobile = row["CoApplicant1_Mobile"].ToString();
                applicationForm.CoApplicant1_Email = row["CoApplicant1_Email"].ToString();
                applicationForm.CoApplicant1_Address1 = row["CoApplicant1_Address1"].ToString();
                applicationForm.CoApplicant1_Address2 = row["CoApplicant1_Address2"].ToString();
                applicationForm.CoApplicant1_Address3 = row["CoApplicant1_Address3"].ToString();
                applicationForm.CoApplicant1_Address4 = row["CoApplicant1_Address4"].ToString();
                applicationForm.CoApplicant1_Post = row["CoApplicant1_Post"].ToString();
                applicationForm.CoApplicant1_City = row["CoApplicant1_City"].ToString();
                applicationForm.CoApplicant1_District = row["CoApplicant1_District"].ToString();
                applicationForm.CoApplicant1_PinCode = row["CoApplicant1_PinCode"].ToString();
                applicationForm.CoApplicant1_State = row["CoApplicant1_State"].ToString();

                // Co-Applicant 2 Details
                applicationForm.CoApplicant2_Name = row["CoApplicant2_Name"].ToString();
                applicationForm.CoApplicant2_AgeDOB = row["CoApplicant2_AgeDOB"].ToString();
                applicationForm.CoApplicant2_Sex = row["CoApplicant2_Sex"].ToString();
                applicationForm.CoApplicant2_Relation = row["CoApplicant2_Relation"].ToString();
                applicationForm.CoApplicant2_Occupation = row["CoApplicant2_Occupation"].ToString();
                applicationForm.CoApplicant2_Mobile = row["CoApplicant2_Mobile"].ToString();
                applicationForm.CoApplicant2_Email = row["CoApplicant2_Email"].ToString();
                applicationForm.CoApplicant2_Address1 = row["CoApplicant2_Address1"].ToString();
                applicationForm.CoApplicant2_Address2 = row["CoApplicant2_Address2"].ToString();
                applicationForm.CoApplicant2_Address3 = row["CoApplicant2_Address3"].ToString();
                applicationForm.CoApplicant2_Address4 = row["CoApplicant2_Address4"].ToString();
                applicationForm.CoApplicant2_Post = row["CoApplicant2_Post"].ToString();
                applicationForm.CoApplicant2_City = row["CoApplicant2_City"].ToString();
                applicationForm.CoApplicant2_District = row["CoApplicant2_District"].ToString();
                applicationForm.CoApplicant2_PinCode = row["CoApplicant2_PinCode"].ToString();
                applicationForm.CoApplicant2_State = row["CoApplicant2_State"].ToString();

                // Co-Applicant 3 Details
                applicationForm.CoApplicant3_Name = row["CoApplicant3_Name"].ToString();
                applicationForm.CoApplicant3_AgeDOB = row["CoApplicant3_AgeDOB"].ToString();
                applicationForm.CoApplicant3_Sex = row["CoApplicant3_Sex"].ToString();
                applicationForm.CoApplicant3_Relation = row["CoApplicant3_Relation"].ToString();
                applicationForm.CoApplicant3_Occupation = row["CoApplicant3_Occupation"].ToString();
                applicationForm.CoApplicant3_Mobile = row["CoApplicant3_Mobile"].ToString();
                applicationForm.CoApplicant3_Email = row["CoApplicant3_Email"].ToString();
                applicationForm.CoApplicant3_Address1 = row["CoApplicant3_Address1"].ToString();
                applicationForm.CoApplicant3_Address2 = row["CoApplicant3_Address2"].ToString();
                applicationForm.CoApplicant3_Address3 = row["CoApplicant3_Address3"].ToString();
                applicationForm.CoApplicant3_Address4 = row["CoApplicant3_Address4"].ToString();
                applicationForm.CoApplicant3_Post = row["CoApplicant3_Post"].ToString();
                applicationForm.CoApplicant3_City = row["CoApplicant3_City"].ToString();
                applicationForm.CoApplicant3_District = row["CoApplicant3_District"].ToString();
                applicationForm.CoApplicant3_PinCode = row["CoApplicant3_PinCode"].ToString();
                applicationForm.CoApplicant3_State = row["CoApplicant3_State"].ToString();

                applicationForm.CurrentLiability = row["CurrentLiability"].ToString();
                applicationForm.PropertyDetails = row["ProperytyDetails"].ToString();
                //applicationForm.Mobile = row["Mobile"].ToString();
                applicationForm.Email = row["Email"].ToString();
                applicationForm.PAN_GST = row["PAN_GST"].ToString();
                applicationForm.BankName = row["BankName"].ToString();
                applicationForm.BankBranch = row["BankBranch"].ToString();
                applicationForm.AccountType = row["AccountType"].ToString();
                applicationForm.AccountNumber = row["AccountNumber"].ToString();
                applicationForm.KYCType = row["KYCType"].ToString();
                applicationForm.KYCNumber = row["KYCNumber"].ToString();
                applicationForm.BusinessActivity = row["BusinessActivity"].ToString();
                applicationForm.BusinessAddress1 = row["BusinessAddress1"].ToString();
                applicationForm.BusinessAddress2 = row["BusinessAddress2"].ToString();
                applicationForm.BusinessAddress3 = row["BusinessAddress3"].ToString();
                applicationForm.BusinessAddress4 = row["BusinessAddress4"].ToString();
                applicationForm.BusinessPremises_Own_Rent = row["BusinessPremises_Own_Rent"].ToString();

                applicationForm.TypeOfLoan = row["TypeOfLoan"].ToString();
                applicationForm.PurposeOfLoan = row["PurposeOfLoan"].ToString();
                // Retrieve and assign the AmountOfLoan (double) from the DataRow
                applicationForm.AmountOfLoan = row["AmountOfLoan"] != DBNull.Value
                                                ? Convert.ToDouble(row["AmountOfLoan"])
                                                : 0.0;  // Use a default value if DBNull

                // Retrieve and assign the Period (int) from the DataRow
                applicationForm.Period = row["Period"] != DBNull.Value
                                          ? Convert.ToInt32(row["Period"])
                                          : 0;  // Use a default value if DBNull


                // Family Member 1 Details
                applicationForm.FamilyMember1_Name = row["FamilyMember1_Name"].ToString();
                applicationForm.FamilyMember1_Age = row["FamilyMember1_Age"] != DBNull.Value ? Convert.ToInt32(row["FamilyMember1_Age"]) : 0;
                applicationForm.FamilyMember1_Relation = row["FamilyMember1_Relation"].ToString();
                applicationForm.FamilyMember1_Occupation = row["FamilyMember1_Occupation"].ToString();

                // Family Member 2 Details
                applicationForm.FamilyMember2_Name = row["FamilyMember2_Name"].ToString();
                applicationForm.FamilyMember2_Age = row["FamilyMember2_Age"] != DBNull.Value ? Convert.ToInt32(row["FamilyMember2_Age"]) : 0;
                applicationForm.FamilyMember2_Relation = row["FamilyMember2_Relation"].ToString();
                applicationForm.FamilyMember2_Occupation = row["FamilyMember2_Occupation"].ToString();

                // Family Member 3 Details
                applicationForm.FamilyMember3_Name = row["FamilyMember3_Name"].ToString();
                applicationForm.FamilyMember3_Age = row["FamilyMember3_Age"] != DBNull.Value ? Convert.ToInt32(row["FamilyMember3_Age"]) : 0;
                applicationForm.FamilyMember3_Relation = row["FamilyMember3_Relation"].ToString();
                applicationForm.FamilyMember3_Occupation = row["FamilyMember3_Occupation"].ToString();

                // Family Member 4 Details
                applicationForm.FamilyMember4_Name = row["FamilyMember4_Name"].ToString();
                applicationForm.FamilyMember4_Age = row["FamilyMember4_Age"] != DBNull.Value ? Convert.ToInt32(row["FamilyMember4_Age"]) : 0;
                applicationForm.FamilyMember4_Relation = row["FamilyMember4_Relation"].ToString();
                applicationForm.FamilyMember4_Occupation = row["FamilyMember4_Occupation"].ToString();

                applicationForm.Place = row["Place"].ToString();
                applicationForm.ApplicationDate = row["ApplicationDate"].ToString();

                // For PrimaryApplicant_Photo
                if (table.Rows[0]["PrimaryApplicant_Photo"] != DBNull.Value)
                {
                    byte[] byte_photo = (byte[])table.Rows[0]["PrimaryApplicant_Photo"];
                    string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                    applicationForm.PrimaryApplicant_Photo_ImageUrl1 = "data:image/png;base64," + base64String;
                }
                else
                {
                    applicationForm.PrimaryApplicant_Photo_ImageUrl1 = string.Empty; // Handle the case when photo is DBNull
                }

                // For CoApplicant1_Photo
                if (table.Rows[0]["CoApplicant1_Photo"] != DBNull.Value)
                {
                    byte[] byte_photo1 = (byte[])table.Rows[0]["CoApplicant1_Photo"];
                    string base64String1 = Convert.ToBase64String(byte_photo1, 0, byte_photo1.Length);
                    applicationForm.CoApplicant1_Photo_ImageUrl1 = "data:image/png;base64," + base64String1;
                }
                else
                {
                    applicationForm.CoApplicant1_Photo_ImageUrl1 = string.Empty; // Handle the case when photo is DBNull
                }

                // For CoApplicant2_Photo
                if (table.Rows[0]["CoApplicant2_Photo"] != DBNull.Value)
                {
                    byte[] byte_photo2 = (byte[])table.Rows[0]["CoApplicant2_Photo"];
                    string base64String2 = Convert.ToBase64String(byte_photo2, 0, byte_photo2.Length);
                    applicationForm.CoApplicant2_Photo_ImageUrl1 = "data:image/png;base64," + base64String2;
                }
                else
                {
                    applicationForm.CoApplicant2_Photo_ImageUrl1 = string.Empty; // Handle the case when photo is DBNull
                }

                // For CoApplicant3_Photo
                if (table.Rows[0]["CoApplicant3_Photo"] != DBNull.Value)
                {
                    byte[] byte_photo3 = (byte[])table.Rows[0]["CoApplicant3_Photo"];
                    string base64String3 = Convert.ToBase64String(byte_photo3, 0, byte_photo3.Length);
                    applicationForm.CoApplicant3_Photo_ImageUrl1 = "data:image/png;base64," + base64String3;
                }
                else
                {
                    applicationForm.CoApplicant3_Photo_ImageUrl1 = string.Empty; // Handle the case when photo is DBNull
                }

            }

            return applicationForm;
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
                KFSForm.BenchNoEPI =row["Bench_no_EPI"].ToString();

                KFSForm.REAProcessingRecurring = row["RE_A_Processing_recurring"].ToString();
                KFSForm.REAProcessingAmount = row["RE_A_Processing_amount"].ToString();
                KFSForm.REBProcessingRecurring = row["RE_B_Processing_recurring"].ToString();
                KFSForm.REBProcessingAmount = row["RE_B_Processing_amount"].ToString();

                KFSForm.REAInsuranceRecurring =row["RE_A_Insurance_recurring"].ToString();
                KFSForm.REAInsuranceAmount = row["RE_A_Insurance_amount"].ToString();
                KFSForm.REBInsuranceRecurring = row["RE_B_Insurance_recurring"].ToString();
                KFSForm.REBInsuranceAmount = row["RE_B_Insurance_amount"].ToString();

                KFSForm.REAServiceRecurring = row["RE_A_Service_recurring"].ToString();
                KFSForm.REAServiceAmount = row["RE_A_Service_amount"].ToString();
                KFSForm.REBServiceRecurring = row["RE_B_Service_recurring"].ToString();
                KFSForm.REBServiceAmount = row["RE_B_Service_amount"].ToString();

                KFSForm.REAeSigningRecurring = row["RE_A_eSigning_recurring"].ToString();
                KFSForm.REAeSigningAmount =row["RE_A_eSigning_amount"].ToString();
                KFSForm.REBeSigningRecurring =row["RE_B_eSigning_recurring"].ToString();
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

        [HttpPost]
        public async Task<ActionResult> SendDocToLeegality(HttpPostedFileBase pdfFile, string applicationId)
        {
            if (pdfFile != null && !string.IsNullOrEmpty(applicationId))
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Read PDF file into a memory stream
                    await pdfFile.InputStream.CopyToAsync(memoryStream);
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

        // Example method to fetch invitees, replace with your actual data fetching logic
        private async Task<DataTable> FetchInviteesAsync(string applicationId)
        {
            DataTable dtInvitees = new DataTable();
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 2 };
            pr[1] = new SqlParameter("@application_id", SqlDbType.VarChar, 14) { Value = applicationId };

            dbconnect.Open();
            dtInvitees = dbconnect.ExecuteDataset("[dbo].[Leegality_queries]", pr).Tables[0];
            dbconnect.Close();

            return dtInvitees;
        }


    }
}
