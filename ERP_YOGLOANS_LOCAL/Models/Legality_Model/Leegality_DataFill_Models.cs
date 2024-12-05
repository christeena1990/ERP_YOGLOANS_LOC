using System;
using System.Collections.Generic;

namespace ERP_YOGLOANS_LOCAL.Models.Legality_Model
{
    public class Leegality_DataFill_Models
    {
        // Application and Branch Details
        public string ApplicationID { get; set; }
        public string BranchName { get; set; }
        public double AppliedLoanAmt { get; set; }
        public string AppliedLoanAmtWords { get; set; }
        public double AppliedLoanAmt1 { get; set; }
        public string AppliedLoanAmtWords1 { get; set; }

        // Primary Applicant Details
        public string PrimaryApplicantName { get; set; }
        public string PrimaryApplicantGuardianType { get; set; }
        public string PrimaryApplicantGuardian { get; set; }
        public string PrimaryApplicantGender { get; set; }
        public string PrimaryApplicantAgeDOB { get; set; }

        public string PrimaryApplicantPresentAddress1 { get; set; }
        public string PrimaryApplicantPresentAddress2 { get; set; }
        public string PrimaryApplicantPresentAddress3 { get; set; }
        public string PrimaryApplicantPresentAddress4 { get; set; }
        public string PrimaryApplicantPresentPost { get; set; }
        public string PrimaryApplicantPresentCity { get; set; }
        public string PrimaryApplicantPresentDistrict { get; set; }
        public string PrimaryApplicantPresentPinCode { get; set; }
        public string PrimaryApplicantPresentState { get; set; }
        public string PrimaryApplicantPermanentAddress1 { get; set; }
        public string PrimaryApplicantPermanentAddress2 { get; set; }
        public string PrimaryApplicantPermanentAddress3 { get; set; }
        public string PrimaryApplicantPermanentAddress4 { get; set; }
        public string PrimaryApplicantPermanentPost { get; set; }
        public string PrimaryApplicantPermanentCity { get; set; }
        public string PrimaryApplicantPermanentDistrict { get; set; }
        public string PrimaryApplicantPermanentPinCode { get; set; }
        public string PrimaryApplicantPermanentState { get; set; }

        // Co-Applicant 1 Details
        public string CoApplicant1_Name { get; set; }
        public string CoApplicant1_AgeDOB { get; set; }
        public string CoApplicant1_Sex { get; set; }
        public string CoApplicant1_Relation { get; set; }
        public string CoApplicant1_Occupation { get; set; }
        public string CoApplicant1_Mobile { get; set; }
        public string CoApplicant1_Email { get; set; }
        public string CoApplicant1_Address1 { get; set; }
        public string CoApplicant1_Address2 { get; set; }
        public string CoApplicant1_Address3 { get; set; }
        public string CoApplicant1_Address4 { get; set; }
        public string CoApplicant1_Post { get; set; }
        public string CoApplicant1_City { get; set; }
        public string CoApplicant1_District { get; set; }
        public string CoApplicant1_PinCode { get; set; }
        public string CoApplicant1_State { get; set; }

        // Co-Applicant 2 Details
        public string CoApplicant2_Name { get; set; }
        public string CoApplicant2_AgeDOB { get; set; }
        public string CoApplicant2_Sex { get; set; }
        public string CoApplicant2_Relation { get; set; }
        public string CoApplicant2_Occupation { get; set; }
        public string CoApplicant2_Mobile { get; set; }
        public string CoApplicant2_Email { get; set; }
        public string CoApplicant2_Address1 { get; set; }
        public string CoApplicant2_Address2 { get; set; }
        public string CoApplicant2_Address3 { get; set; }
        public string CoApplicant2_Address4 { get; set; }
        public string CoApplicant2_Post { get; set; }
        public string CoApplicant2_City { get; set; }
        public string CoApplicant2_District { get; set; }
        public string CoApplicant2_PinCode { get; set; }
        public string CoApplicant2_State { get; set; }

        // Co-Applicant 3 Details
        public string CoApplicant3_Name { get; set; }
        public string CoApplicant3_AgeDOB { get; set; }
        public string CoApplicant3_Sex { get; set; }
        public string CoApplicant3_Relation { get; set; }
        public string CoApplicant3_Occupation { get; set; }
        public string CoApplicant3_Mobile { get; set; }
        public string CoApplicant3_Email { get; set; }
        public string CoApplicant3_Address1 { get; set; }
        public string CoApplicant3_Address2 { get; set; }
        public string CoApplicant3_Address3 { get; set; }
        public string CoApplicant3_Address4 { get; set; }
        public string CoApplicant3_Post { get; set; }
        public string CoApplicant3_City { get; set; }
        public string CoApplicant3_District { get; set; }
        public string CoApplicant3_PinCode { get; set; }
        public string CoApplicant3_State { get; set; }

        // Loan Details
        public string TypeOfLoan { get; set; }
        public double AmountOfLoan { get; set; }
        public int Period { get; set; }
        public string PurposeOfLoan { get; set; }

        // Financial Details
        public string CurrentLiability { get; set; }
        public string PropertyDetails { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PAN_GST { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string KYCType { get; set; }
        public string KYCNumber { get; set; }
        public string BusinessActivity { get; set; }
        public string BusinessAddress1 { get; set; }
        public string BusinessAddress2 { get; set; }
        public string BusinessAddress3 { get; set; }
        public string BusinessAddress4 { get; set; }
        public string BusinessPremises_Own_Rent { get; set; }

        // Family Member Details
        public string FamilyMember1_Name { get; set; }
        public int FamilyMember1_Age { get; set; }
        public string FamilyMember1_Relation { get; set; }
        public string FamilyMember1_Occupation { get; set; }

        public string FamilyMember2_Name { get; set; }
        public int FamilyMember2_Age { get; set; }
        public string FamilyMember2_Relation { get; set; }
        public string FamilyMember2_Occupation { get; set; }

        public string FamilyMember3_Name { get; set; }
        public int FamilyMember3_Age { get; set; }
        public string FamilyMember3_Relation { get; set; }
        public string FamilyMember3_Occupation { get; set; }

        public string FamilyMember4_Name { get; set; }
        public int FamilyMember4_Age { get; set; }
        public string FamilyMember4_Relation { get; set; }
        public string FamilyMember4_Occupation { get; set; }

        // Application Details
        public string Place { get; set; }
        public string ApplicationDate { get; set; }

        // Photos/Images
        public string PrimaryApplicant_Photo_ImageUrl1 { get; set; }
        public string CoApplicant1_Photo_ImageUrl1 { get; set; }
        public string CoApplicant2_Photo_ImageUrl1 { get; set; }
        public string CoApplicant3_Photo_ImageUrl1 { get; set; }

        //--------------------------------Sanction Letter-----------------------------------

        public string ApplicationID_SL { get; set; }
        public string PrimaryApplicantName_SL { get; set; }
        public string CoApplicant1_Name_SL { get; set; }
        public string CoApplicant2_Name_SL { get; set; }
        public string CoApplicant3_Name_SL { get; set; }
        public string SanctionNumber_SL { get; set; }
        public string SanctionDate_SL { get; set; }
        public string SanctionedLoanAmt_SL { get; set; }
        public string SanctionedLoanAmtWords_SL { get; set; }
        public string NoOfMonths_SL { get; set; }
        public string EMIAmt_SL { get; set; }
        public string EMIAmtWords_SL { get; set; }

        //-----------------------------DPN----------------------------------
        public string ApplicationID_DPN { get; set; }
        public string SanctionedLoanAmt_DPN { get; set; }
        public string SanctionedLoanAmtWords_DPN { get; set; }
        public string RateOfInterest_DPN { get; set; }
        public string Place_DPN { get; set; }
        public string DPNDate_DPN { get; set; }
        public string PrimaryApplicantName_DPN { get; set; }
        public string PrimaryApplicantAgeDOB_DPN { get; set; }
        public string PrimaryApplicantGuardianType_DPN { get; set; }
        public string PrimaryApplicantGuardian_DPN { get; set; }
        public string PrimaryApplicantPresentAddress1_DPN { get; set; }
        public string PrimaryApplicantPresentAddress2_DPN { get; set; }
        public string PrimaryApplicantPresentAddress3_DPN { get; set; }
        public string PrimaryApplicantPresentAddress4_DPN { get; set; }
        public string PrimaryApplicantPresentPost_DPN { get; set; }
        public string PrimaryApplicantPresentCity_DPN { get; set; }
        public string PrimaryApplicantPresentDistrict_DPN { get; set; }
        public string PrimaryApplicantPresentPinCode_DPN { get; set; }
        public string PrimaryApplicantPresentState_DPN { get; set; }
        public string CoApplicant1_Name_DPN { get; set; }
        public string CoApplicant1_AgeDOB_DPN { get; set; }
        public string CoApplicant1_GuardianType_DPN { get; set; }
        public string CoApplicant1_Guardian_DPN { get; set; }
        public string CoApplicant1_Address1_DPN { get; set; }
        public string CoApplicant1_Address2_DPN { get; set; }
        public string CoApplicant1_Address3_DPN { get; set; }
        public string CoApplicant1_Address4_DPN { get; set; }
        public string CoApplicant1_Post_DPN { get; set; }
        public string CoApplicant1_City_DPN { get; set; }
        public string CoApplicant1_District_DPN { get; set; }
        public string CoApplicant1_PinCode_DPN { get; set; }
        public string CoApplicant1_State_DPN { get; set; }
        public string CoApplicant2_Name_DPN { get; set; }
        public string CoApplicant2_AgeDOB_DPN { get; set; }
        public string CoApplicant2_GuardianType_DPN { get; set; }
        public string CoApplicant2_Guardian_DPN { get; set; }
        public string CoApplicant2_Address1_DPN { get; set; }
        public string CoApplicant2_Address2_DPN { get; set; }
        public string CoApplicant2_Address3_DPN { get; set; }
        public string CoApplicant2_Address4_DPN { get; set; }
        public string CoApplicant2_Post_DPN { get; set; }
        public string CoApplicant2_City_DPN { get; set; }
        public string CoApplicant2_District_DPN { get; set; }
        public string CoApplicant2_PinCode_DPN { get; set; }
        public string CoApplicant2_State_DPN { get; set; }
        public string CoApplicant3_Name_DPN { get; set; }
        public string CoApplicant3_AgeDOB_DPN { get; set; }
        public string CoApplicant3_GuardianType_DPN { get; set; }
        public string CoApplicant3_Guardian_DPN { get; set; }
        public string CoApplicant3_Address1_DPN { get; set; }
        public string CoApplicant3_Address2_DPN { get; set; }
        public string CoApplicant3_Address3_DPN { get; set; }
        public string CoApplicant3_Address4_DPN { get; set; }
        public string CoApplicant3_Post_DPN { get; set; }
        public string CoApplicant3_City_DPN { get; set; }
        public string CoApplicant3_District_DPN { get; set; }
        public string CoApplicant3_PinCode_DPN { get; set; }
        public string CoApplicant3_State_DPN { get; set; }
        //------------------------------------------Agreement-------------------------------------------
        public string ApplicationID_Agrmnt { get; set; }
        public string BranchName_Agrmnt { get; set; }
        public string AgreementDate_Agrmnt { get; set; }
        public string AgreementYear_Agrmnt { get; set; }
        
        public string AgreementDtInWords { get; set; }
        public string PrimaryApplicantName_Agrmnt { get; set; }
        public string PrimaryApplicantAgeDOB_Agrmnt { get; set; }
        public string PrimaryApplicantGuardianType_Agrmnt { get; set; }
        public string PrimaryApplicantGuardian_Agrmnt { get; set; }
        public string PrimaryApplicantPresentAddress1_Agrmnt { get; set; }
        public string PrimaryApplicantPresentAddress2_Agrmnt { get; set; }
        public string PrimaryApplicantPresentAddress3_Agrmnt { get; set; }
        public string PrimaryApplicantPresentAddress4_Agrmnt { get; set; }
        public string PrimaryApplicantPresentPost_Agrmnt { get; set; }
        public string PrimaryApplicantPresentCity_Agrmnt { get; set; }
        public string PrimaryApplicantPresentDistrict_Agrmnt { get; set; }
        public string PrimaryApplicantPresentPinCode_Agrmnt { get; set; }
        public string PrimaryApplicantPresentState_Agrmnt { get; set; }
        public string CoApplicant1_Name_Agrmnt { get; set; }
        public string CoApplicant1_AgeDOB_Agrmnt { get; set; }
        public string CoApplicant1_GuardianType_Agrmnt { get; set; }
        public string CoApplicant1_Guardian_Agrmnt { get; set; }
        public string CoApplicant1_Address1_Agrmnt { get; set; }
        public string CoApplicant1_Address2_Agrmnt { get; set; }
        public string CoApplicant1_Address3_Agrmnt { get; set; }
        public string CoApplicant1_Address4_Agrmnt { get; set; }
        public string CoApplicant1_Post_Agrmnt { get; set; }
        public string CoApplicant1_City_Agrmnt { get; set; }
        public string CoApplicant1_District_Agrmnt { get; set; }
        public string CoApplicant1_PinCode_Agrmnt { get; set; }
        public string CoApplicant1_State_Agrmnt { get; set; }
        public string CoApplicant2_Name_Agrmnt { get; set; }
        public string CoApplicant2_AgeDOB_Agrmnt { get; set; }
        public string CoApplicant2_GuardianType_Agrmnt { get; set; }
        public string CoApplicant2_Guardian_Agrmnt { get; set; }
        public string CoApplicant2_Address1_Agrmnt { get; set; }
        public string CoApplicant2_Address2_Agrmnt { get; set; }
        public string CoApplicant2_Address3_Agrmnt { get; set; }
        public string CoApplicant2_Address4_Agrmnt { get; set; }
        public string CoApplicant2_Post_Agrmnt { get; set; }
        public string CoApplicant2_City_Agrmnt { get; set; }
        public string CoApplicant2_District_Agrmnt { get; set; }
        public string CoApplicant2_PinCode_Agrmnt { get; set; }
        public string CoApplicant2_State_Agrmnt { get; set; }
        public string CoApplicant3_Name_Agrmnt { get; set; }
        public string CoApplicant3_AgeDOB_Agrmnt { get; set; }
        public string CoApplicant3_GuardianType_Agrmnt { get; set; }
        public string CoApplicant3_Guardian_Agrmnt { get; set; }
        public string CoApplicant3_Address1_Agrmnt { get; set; }
        public string CoApplicant3_Address2_Agrmnt { get; set; }
        public string CoApplicant3_Address3_Agrmnt { get; set; }
        public string CoApplicant3_Address4_Agrmnt { get; set; }
        public string CoApplicant3_Post_Agrmnt { get; set; }
        public string CoApplicant3_City_Agrmnt { get; set; }
        public string CoApplicant3_District_Agrmnt { get; set; }
        public string CoApplicant3_PinCode_Agrmnt { get; set; }
        public string CoApplicant3_State_Agrmnt { get; set; }

        //--------------------------------------Terms & Conditions----------------------------------------

        public string ApplicationID_TC { get; set; }
        public string PurposeOfLoan_TC { get; set; }
        public string RateOfInterest_TC { get; set; }
        public string BokenIntRate_TC { get; set; }
        public string LoanDisbursementBranch_TC { get; set; }
        public string TotalInterestCharge_TC { get; set; }
        public string PenalInterest_TC { get; set; }
        public string TenureOfLoan_TC { get; set; }
        public string RepaymentFrequency_TC { get; set; }
        public string NumberOfEMI_EMIAmount_TC { get; set; }
        public string NumberOfAdvanceInstallment_TC { get; set; }
        public string MannerOfPayment_TC { get; set; }
        public string DueDate_TC { get; set; }
        public string DateOfCommencementEMI_TC { get; set; }
        public string DueDateOfPaymentOfFirstEMI_TC { get; set; }
        public string DueDateOfPaymentOfLastEMI_TC { get; set; }
        public string ModeOfRepayment_TC { get; set; }
        public string PrepaymentCharges_TC { get; set; }
        public string LoanAmount_TC { get; set; }
        public string ProcessingCharges_TC { get; set; }
        public string ServiceCharge_TC { get; set; }
        public string AdvanceEMI_TC { get; set; }
        public string NetDisbursedAmount_TC { get; set; }
        public string TotalAmountPaidByBorrower_TC { get; set; }
        public string ECS_PDC_DishonourCharges_TC { get; set; }
        public string SubmissionACHMandate_TC { get; set; }
        public string RegisteredLetterCharges_TC { get; set; }
        public string NormalLetterCharges_TC { get; set; }
        public string StatementRepaymentAnnexure_TC { get; set; }
        public string OtherCharges_TC { get; set; }
        public string Security_HypothecatedAssets_TC { get; set; }
        public string Jurisdiction_TC { get; set; }
        public string PrimaryApplicantName_TC { get; set; }
        public string PrimaryApplicantGuardianType_TC { get; set; }
        public string PrimaryApplicantGuardian_TC { get; set; }
        public string PrimaryApplicantPresentAddress1_TC { get; set; }
        public string PrimaryApplicantPresentAddress2_TC { get; set; }
        public string PrimaryApplicantPresentAddress3_TC { get; set; }
        public string PrimaryApplicantPresentAddress4_TC { get; set; }
        public string PrimaryApplicantPresentPost_TC { get; set; }
        public string PrimaryApplicantPresentCity_TC { get; set; }
        public string PrimaryApplicantPresentDistrict_TC { get; set; }
        public string PrimaryApplicantPresentPinCode_TC { get; set; }
        public string PrimaryApplicantPresentState_TC { get; set; }
        public string OtherDisclosures { get; set; }
        public string DetailsOfLSP { get; set; }
        public string AmortizationOfLoan { get; set; }




        //------------------------------------Annexure----------------------------------------
        public string ApplicationID_annx { get; set; }
        public string BranchAddress1_annx { get; set; }
        public string BranchAddress2_annx { get; set; }
        public string BranchAddress3_annx { get; set; }
        public string BranchAddress4_annx { get; set; }
        public string PrimaryApplicantName_annx { get; set; }
        public string PrimaryApplicantGuardianType_annx { get; set; }
        public string PrimaryApplicantGuardian_annx { get; set; }
        public string PrimaryApplicantAgeDOB_annx { get; set; }
        public string PrimaryApplicantPresentAddress1_annx { get; set; }
        public string PrimaryApplicantPresentAddress2_annx { get; set; }
        public string PrimaryApplicantPresentAddress3_annx { get; set; }
        public string PrimaryApplicantPresentAddress4_annx { get; set; }
        public string PrimaryApplicantPresentPost_annx { get; set; }
        public string PrimaryApplicantPresentCity_annx { get; set; }
        public string PrimaryApplicantPresentDistrict_annx { get; set; }
        public string PrimaryApplicantPresentPinCode_annx { get; set; }
        public string PrimaryApplicantPresentState_annx { get; set; }
        public string PrimaryApplicantConstitution_annx { get; set; }
        public string PrimaryApplicantMobile_annx { get; set; }
        public string PrimaryApplicantEmail_annx { get; set; }
        public string CoApplicant1_Name_annx { get; set; }
        public string CoApplicant1_AgeDOB_annx { get; set; }
        public string CoApplicant1_FatherName_annx { get; set; }
        public string CoApplicant1_Mobile_annx { get; set; }
        public string CoApplicant1_Email_annx { get; set; }
        public string CoApplicant1_Address1_annx { get; set; }
        public string CoApplicant1_Address2_annx { get; set; }
        public string CoApplicant1_Address3_annx { get; set; }
        public string CoApplicant1_Address4_annx { get; set; }
        public string CoApplicant1_Post_annx { get; set; }
        public string CoApplicant1_City_annx { get; set; }
        public string CoApplicant1_District_annx { get; set; }
        public string CoApplicant1_PinCode_annx { get; set; }
        public string CoApplicant1_State_annx { get; set; }
        public string CoApplicant1_Constitution_annx { get; set; }
        public string CoApplicant2_Name_annx { get; set; }
        public string CoApplicant2_AgeDOB_annx { get; set; }
        public string CoApplicant2_FatherName_annx { get; set; }
        public string CoApplicant2_Mobile_annx { get; set; }
        public string CoApplicant2_Email_annx { get; set; }
        public string CoApplicant2_Address1_annx { get; set; }
        public string CoApplicant2_Address2_annx { get; set; }
        public string CoApplicant2_Address3_annx { get; set; }
        public string CoApplicant2_Address4_annx { get; set; }
        public string CoApplicant2_Post_annx { get; set; }
        public string CoApplicant2_City_annx { get; set; }
        public string CoApplicant2_District_annx { get; set; }
        public string CoApplicant2_PinCode_annx { get; set; }
        public string CoApplicant2_State_annx { get; set; }
        public string CoApplicant2_Constitution_annx { get; set; }
        public string CoApplicant3_Name_annx { get; set; }
        public string CoApplicant3_AgeDOB_annx { get; set; }
        public string CoApplicant3_FatherName_annx { get; set; }
        public string CoApplicant3_Mobile_annx { get; set; }
        public string CoApplicant3_Email_annx { get; set; }
        public string CoApplicant3_Address1_annx { get; set; }
        public string CoApplicant3_Address2_annx { get; set; }
        public string CoApplicant3_Address3_annx { get; set; }
        public string CoApplicant3_Address4_annx { get; set; }
        public string CoApplicant3_Post_annx { get; set; }
        public string CoApplicant3_City_annx { get; set; }
        public string CoApplicant3_District_annx { get; set; }
        public string CoApplicant3_PinCode_annx { get; set; }
        public string CoApplicant3_State_annx { get; set; }
        public string CoApplicant3_Constitution_annx { get; set; }

        //----------------------------------Disbursment Memo--------------------------------------------
        public string ApplicationID_DM { get; set; }
        public string PrimaryApplicantName_DM { get; set; }
        public string PrimaryApplicantPresentAddress1_DM { get; set; }
        public string PrimaryApplicantPresentAddress2_DM { get; set; }
        public string PrimaryApplicantPresentAddress3_DM { get; set; }
        public string PrimaryApplicantPresentAddress4_DM { get; set; }
        public string PrimaryApplicantPresentPost_DM { get; set; }
        public string PrimaryApplicantPresentCity_DM { get; set; }
        public string PrimaryApplicantPresentDistrict_DM { get; set; }
        public string PrimaryApplicantPresentPinCode_DM { get; set; }
        public string PrimaryApplicantPresentState_DM { get; set; }
        public string BranchAddress1_DM { get; set; }
        public string BranchAddress2_DM { get; set; }
        public string BranchAddress3_DM { get; set; }
        public string BranchAddress4_DM { get; set; }
        public string ApplicationDate_DM { get; set; }
        public string SanctionedLoanAmt_DM { get; set; }
        public string SanctionedLoanAmtWords_DM { get; set; }
        public string AgreementDate_DM { get; set; }
        public string BankName_DM { get; set; }
        public string BankBranch_DM { get; set; }
        public string AccountHolder_DM { get; set; }
        public string AccountNumber_DM { get; set; }
        public string IFSC_DM { get; set; }

        //--------------------------Repayment Schedule----------------------------------

        //public string ApplicationID_RS { get; set; }  
        //public int InstNo_RS { get; set; }            
        //public decimal OutstandingPrincipal_RS { get; set; } 
        //public decimal Principal_RS { get; set; }     
        //public decimal Interest_RS { get; set; }     
        //public decimal Amount_RS { get; set; }



    }
    public class RepaymentDetails
    {
        public string ApplicationID_RS { get; set; }
        public int InstNo_RS { get; set; }
        public decimal OutstandingPrincipal_RS { get; set; }
        public decimal Principal_RS { get; set; }
        public decimal Interest_RS { get; set; }
        public decimal Amount_RS { get; set; }
    }

    public class KFSLoanInfo
    {
        // Application and Loan Information
        public string ApplicationId { get; set; }
        public string LoanType { get; set; }
        public string LoanAmount { get; set; }
        public string DisbursementStages { get; set; }
        public string Schedule { get; set; }
        public string Duration { get; set; } // Duration in months
        public string Instalment { get; set; }
        public string NumberOfEPIs { get; set; }
        public string EPI { get; set; }
        public string CommencementOfRepayment { get; set; }
        public string InterestRate { get; set; }
        public string InterestRateType { get; set; }
        public string RefeBenchmark { get; set; }
        public string BenchmarkRate { get; set; }
        public string Spread { get; set; }
        public string FinalRate { get; set; }
        public string ResetPeriodB { get; set; }
        public string ResetPeriodS { get; set; }

        // Recurring and One-time Processing Fees
        public string BenchEPI { get; set; }
        public string BenchNoEPI { get; set; }
        public string REAProcessingRecurring { get; set; }
        public string REAProcessingAmount { get; set; }
        public string REBProcessingRecurring { get; set; }
        public string REBProcessingAmount { get; set; }
        public string REAInsuranceRecurring { get; set; }
        public string REAInsuranceAmount { get; set; }
        public string REBInsuranceRecurring { get; set; }
        public string REBInsuranceAmount { get; set; }
        public string REAServiceRecurring { get; set; }
        public string REAServiceAmount { get; set; }
        public string REBServiceRecurring { get; set; }
        public string REBServiceAmount { get; set; }
        public string REAeSigningRecurring { get; set; }
        public string REAeSigningAmount { get; set; }
        public string REBeSigningRecurring { get; set; }
        public string REBeSigningAmount { get; set; }
        public string REAStampRecurring { get; set; }
        public string REAStampAmount { get; set; }
        public string REBStampRecurring { get; set; }
        public string REBStampAmount { get; set; }
        public string REAAppRecurring { get; set; }
        public string REAAppAmount { get; set; }
        public string REBAppRecurring { get; set; }
        public string REBAppAmount { get; set; }
        public string REAeStamp { get; set; }
        public string REAeStampAmount { get; set; }
        public string REBeStampRecurring { get; set; }
        public string REBeStampAmount { get; set; }
        public string REABureau { get; set; }
        public string REABureauAmount { get; set; }
        public string REBBureauRecurring { get; set; }
        public string REBBureauAmount { get; set; }
        public string REAValuatioRecurring { get; set; }
        public string REAValuatioAmount { get; set; }
        public string REBValuatioRecurring { get; set; }
        public string REBValuatioAmount { get; set; }
        public string REAothRecurring { get; set; }
        public string REAothAmount { get; set; }
        public string REBothRecurring { get; set; }
        public string REBothAmount { get; set; }
        public string REABrokRecurring { get; set; }
        public string REABrokAmount { get; set; }
        public string REBBrokRecurring { get; set; }
        public string REBBrokAmount { get; set; }
        public string REAAdRecurring { get; set; }
        public string REAAdAmount { get; set; }
        public string REBAdRecurring { get; set; }
        public string REBAdAmount { get; set; }

        // Additional Charges and Fees
        public string APR { get; set; }
        public string PenalCharge { get; set; }
        public string OtherPenalCharge { get; set; }
        public string ForeclosureCharge { get; set; }
        public string SwitchingCharge { get; set; }
        public string OtherCharge { get; set; }
        public string NoticeCharge { get; set; }
        public string PostageCharge { get; set; }
        public string AdvertisementCharge { get; set; }
        public string ClauseAgent { get; set; }
        public string ClauseRedressalMechanism { get; set; }
        public string PhoneEmailNodal { get; set; }
        public string TransferToRE { get; set; }

        // Partner and Originating RE Information
        public string NameOfOriginatingRE { get; set; }
        public string NameOfPartnerRE { get; set; }

        // Interest and Rate Information
        public string BlendedRateOfInterest { get; set; }
        public string LookUpPeriod { get; set; }

        // LSP Details
        public string DetailsOfLSP { get; set; }

        // Branch Information
        public string Branch { get; set; }

        // Date and Other Information
        public string PrintDate { get; set; }
        public string MinInt { get; set; } // Minimum interest
        public string Recovery { get; set; } // Recovery Amount
        public string Cheque { get; set; } // Cheque Detail

        //------------------------------------AnnexB Model--------------------

        public string LoanAmt { get; set; }
        public string Tenure { get; set; }
        public string NoOfInstallments { get; set; }
        public string EpiAmount { get; set; }
        public string NoOfInstallmentsInt { get; set; }
        public string CommencementOfRepayments { get; set; }
        public string InterestRateType_AnnexB { get; set; }
        public string RateOfInterest { get; set; }
        public string TotalInterestAmount { get; set; }
        public string FeeCharges { get; set; }
        public string PayableToRE { get; set; }
        public string PayableToThirdPartyRoutedThroughRE { get; set; }
        public string NetDisbursedAmount { get; set; }
        public string TotalAmountToBePaid { get; set; }
        public string AnnualPercentageRate { get; set; }
        public string ScheduleOfDisbursement { get; set; }
        public string DueDate { get; set; }


        public string InstalmentNo { get; set; }
        public string OutstandingPrincipal { get; set; }
        public string Principal { get; set; }
        public string Interest { get; set; }
        public string Instalment_AnnexC { get; set; }






    }

    public class ApplicationAndKFSViewModel
    {
        // Application Data (from Leegality_DataFill_Models)
        public Leegality_DataFill_Models ApplicationFormData { get; set; }

        public Leegality_DataFill_Models DPNApplicationFormData { get; set; }

        public Leegality_DataFill_Models AgreementApplicationFormData { get; set; }

        public Leegality_DataFill_Models TandCApplicationFormData { get; set; }

        public Leegality_DataFill_Models AnnexureApplicationFormData { get; set; }

        public Leegality_DataFill_Models DMApplicationFormData { get; set; }

        public Leegality_DataFill_Models RepaymentDetails { get; set; }



        // KFS Data (from KFSLoanInfo)
        public KFSLoanInfo KFSFormData { get; set; }

        public KFSLoanInfo KFSFormData_AnnexB { get; set; }

        public List<KFSLoanInfo> KFSFormData_AnnexC { get; set; }

        public List<RepaymentDetails> RepaymentSchedule { get; set; }

    }
}
