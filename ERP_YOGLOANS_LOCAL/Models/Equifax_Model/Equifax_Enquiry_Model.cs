using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_YOGLOANS_LOCAL.Models.Equifax_Model
{
    public class Equifax_Enquiry_Model
    {
        public List<Address> AddressList { get; set; }
        public List<Scores> ScoreList { get; set; }
        public List<Microfinance> MicrofinanceList { get; set; }
        public List<Retail> RetailList { get; set; }
        public List<Digital> DigitalList { get; set; }

        public Equifax_Enquiry_Model()
        {
            AddressList = new List<Address>();
            ScoreList = new List<Scores>();
            MicrofinanceList = new List<Microfinance>();
            RetailList = new List<Retail>();
            DigitalList = new List<Digital>();
        }

        public string LoanType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public long txt_cust_pincode { get; set; } // Confirm that this should be long
        public long PostOffice { get; set; } // Confirm that this should be long
        public long state_name { get; set; } // Confirm that this should be long
        public decimal LoanAmount { get; set; }
        public string Remarks { get; set; }
        public string VehicleType { get; set; }
        public string PANNumber { get; set; }
        public string AadharNo { get; set; }
        public string VotersID { get; set; }
        public string Passport { get; set; }
        public string DrivingLicence { get; set; }
        public string RationCard { get; set; }
        public string ID { get; set; }
        // ReportModel could be moved outside if used elsewhere
        public class ReportModel
        {
            public dynamic InquiryResponse { get; set; }
            public dynamic InquiryRequestInfo { get; set; }
            public dynamic CCRResponse { get; set; }
        }

        public string ClientId { get; set; }
        public string OrderNo { get; set; }
        public string RefNo { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string lbl_name { get; set; }
        public string lbl_pname { get; set; }
        public string lbl_dob { get; set; }
        public string lbl_age { get; set; }
        public string lbl_gender { get; set; }
        public string lbl_income { get; set; }
        public string lbl_occupation { get; set; }
        public string lbl_marital { get; set; }
        public string lbl_pan { get; set; }
        public string lbl_voter { get; set; }
        public string lbl_passport { get; set; }
        public string lbl_uid { get; set; }
        public string lbl_Dlicense { get; set; }
        public string lbl_ration { get; set; }
        public string lbl_otherid { get; set; }
        public string lbl_home { get; set; }
        public string lbl_office { get; set; }
        public string lbl_mob { get; set; }
        public string Label17 { get; set; }
        public string Label18 { get; set; }
        public string Label19 { get; set; }
        public string lbl_email { get; set; }
        public string mfi_score { get; set; }
        public string lbl_retail { get; set; }
        public string lbl_micro { get; set; }
        public string Literal1 { get; set; }
        public string Literal2 { get; set; }
        public string lbl_enquery { get; set; }
        public string lbl_equi_score { get; set; }
        public string MfiScore { get; set; }
        public string lbl_digital { get; set; }
        public string grid_address { get; set; }
    }

    public class Address
    {
        public int Seq { get; set; }
        public string Address1 { get; set; }
        public string ReportedDate { get; set; }
        public string Postal { get; set; }
        public string State { get; set; }
    }

    //public class Score
    //{
    //    public string Name { get; set; }
    //    public string Value { get; set; }
    //    public string Type { get; set; }
    //    public List<ScoreElement> Elements { get; set; }

    //    public Score()
    //    {
    //        Elements = new List<ScoreElement>();
    //    }
    //}

    //public class ScoreElement
    //{
    //    public int Seq { get; set; }
    //    public string Description { get; set; }
    //}

    public class Scores
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public List<ScoreElement> Elements { get; set; }

        public Scores()
        {
            Elements = new List<ScoreElement>();
        }
    }

    public class ScoreElement
    {
        public int Seq { get; set; }
        public string Description { get; set; }
    }


    public class Microfinance
    {
        public string Id { get; set; }
        public int NoOfActiveAccounts { get; set; }
        public decimal TotalPastDue { get; set; }
        public int NoOfPastDueAccounts { get; set; }
        public string RecentAccount { get; set; }
        public decimal TotalBalanceAmount { get; set; }
        public decimal TotalMonthlyPaymentAmount { get; set; }
        public decimal TotalWrittenOffAmount { get; set; }
    }

    public class Retail
    {
        public int NoOfAccounts { get; set; }
        public int NoOfActiveAccounts { get; set; }
        public int NoOfWriteOffs { get; set; }
        public decimal TotalWrittenOffAmount { get; set; }
        public decimal TotalBalanceAmount { get; set; }
    }

    public class Digital
    {
        public string BankName { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal AccountBalance { get; set; }
    }








    //-----------------------------------------  model for api elements ----------------------------------------------------------

    public class Rootobject
    {

        public Inquiryresponseheader InquiryResponseHeader { get; set; }
        public Inquiryrequestinfo InquiryRequestInfo { get; set; }
        public Score1[] Score { get; set; }
        public Ccrresponse CCRResponse { get; set; }
        public Error Error { get; set; }
    }

    public class Rootrequestobject
    {
        public Requestheader RequestHeader { get; set; }
        public Requestbody RequestBody { get; set; }
        public Score[] Score { get; set; }
    }

    public class Requestheader
    {
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string MemberNumber { get; set; }
        public string SecurityCode { get; set; }
        public string CustRefField { get; set; }
        public string[] ProductCode { get; set; }
    }

    public class Requestbody
    {
        public string InquiryPurpose { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public Inquiryaddress[] InquiryAddresses { get; set; }
        public Inquiryphone[] InquiryPhones { get; set; }
        public Iddetail[] IDDetails { get; set; }
        public Mfidetails MFIDetails { get; set; }
    }

    public class Inquiryresponseheader
    {
        public string ClientID { get; set; }
        public string CustRefField { get; set; }
        public string ReportOrderNO { get; set; }
        public string[] ProductCode { get; set; }
        public string SuccessCode { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public class Inquiryrequestinfo
    {
        public string InquiryPurpose { get; set; }
        public string FirstName { get; set; }
        public Inquiryaddress[] InquiryAddresses { get; set; }
        public Inquiryphone[] InquiryPhones { get; set; }
        public Iddetail[] IDDetails { get; set; }
        public string DOB { get; set; }
        public Mfidetails MFIDetails { get; set; }
    }

    public class Mfidetails
    {
        public Familydetail[] FamilyDetails { get; set; }
    }

    public class Familydetail
    {
        public string seq { get; set; }
        public string AdditionalNameType { get; set; }
        public string AdditionalName { get; set; }
    }

    public class Inquiryaddress
    {
        public string seq { get; set; }
        public string[] AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
    }

    public class Inquiryphone
    {
        public string seq { get; set; }
        public string[] PhoneType { get; set; }
        public string Number { get; set; }
    }

    public class Iddetail
    {
        public string seq { get; set; }
        public string IDType { get; set; }
        public string Source { get; set; }
        public string IDValue { get; set; }
    }

    public class Ccrresponse
    {
        public string Status { get; set; }
        public Cirreportdatalst[] CIRReportDataLst { get; set; }
    }

    public class Cirreportdatalst
    {
        public Inquiryresponseheader1 InquiryResponseHeader { get; set; }
        public Inquiryrequestinfo1 InquiryRequestInfo { get; set; }
        public Score[] Score { get; set; }
        public Cirreportdata CIRReportData { get; set; }

    }

    public class Inquiryresponseheader1
    {
        public string CustomerCode { get; set; }
        public string CustRefField { get; set; }
        public string ReportOrderNO { get; set; }
        public string[] ProductCode { get; set; }
        public string SuccessCode { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string HitCode { get; set; }
        public string CustomerName { get; set; }
    }

    public class Inquiryrequestinfo1
    {
        public string InquiryPurpose { get; set; }
        public string FirstName { get; set; }
        public Inquiryaddress1[] InquiryAddresses { get; set; }
        public Inquiryphone1[] InquiryPhones { get; set; }
        public Iddetail1[] IDDetails { get; set; }
        public string DOB { get; set; }
        public Mfidetails1 MFIDetails { get; set; }
    }

    public class Mfidetails1
    {
        public Familydetail1[] FamilyDetails { get; set; }
    }

    public class Familydetail1
    {
        public string seq { get; set; }
        public string AdditionalNameType { get; set; }
    }

    public class Inquiryaddress1
    {
        public string seq { get; set; }
        public string[] AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
    }

    public class Inquiryphone1
    {
        public string seq { get; set; }
        public string[] PhoneType { get; set; }
        public string Number { get; set; }
    }

    public class Iddetail1
    {
        public string seq { get; set; }
        public string IDType { get; set; }
        public string IDValue { get; set; }
        public string Source { get; set; }
    }

    public class Cirreportdata
    {
        public Idandcontactinfo IDAndContactInfo { get; set; }
        public Retailaccountdetail[] RetailAccountDetails { get; set; }
        public Retailaccountssummary RetailAccountsSummary { get; set; }
        public Scoredetail[] ScoreDetails { get; set; }
        public Enquiry[] Enquiries { get; set; }
        public Enquirysummary EnquirySummary { get; set; }
        public Otherkeyind OtherKeyInd { get; set; }
        public Recentactivities RecentActivities { get; set; }
        public Microfinanceaccountdetail[] MicrofinanceAccountDetails { get; set; }
        public Microfinanceaccountssummary MicrofinanceAccountsSummary { get; set; }
        public Incomedetail[] IncomeDetails { get; set; }
        public Familydetailsinfo familyDetailsInfo { get; set; }
    }

    public class Scoredetail
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public Scoringelement[] ScoringElements { get; set; }
    }

    public class Idandcontactinfo
    {
        public Personalinfo PersonalInfo { get; set; }
        public Identityinfo IdentityInfo { get; set; }
        public Addressinfo[] AddressInfo { get; set; }
        public Phoneinfo[] PhoneInfo { get; set; }
    }

    public class Personalinfo
    {
        public Name Name { get; set; }
        public Aliasname AliasName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Age Age { get; set; }
        public Placeofbirthinfo PlaceOfBirthInfo { get; set; }
        public string Occupation { get; set; }
        public string MaritalStatus { get; set; }
    }

    public class Name
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    public class Aliasname
    {
    }

    public class Age
    {
        public string age { get; set; }
    }

    public class Placeofbirthinfo
    {
    }

    public class Identityinfo
    {
        public Panid[] PANId { get; set; }
        public Voterid[] VoterID { get; set; }
        public Nationalidcard[] NationalIDCard { get; set; }
        public Otherid[] OtherId { get; set; }
        public Rationcard[] RationCard { get; set; }
    }

    public class Panid
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }

    public class Voterid
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }

    public class Nationalidcard
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }
    public class Rationcard
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }

    public class Otherid
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string IdNumber { get; set; }
    }

    public class Addressinfo
    {
        public string Seq { get; set; }
        public string ReportedDate { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public string Type { get; set; }
    }

    public class Phoneinfo
    {
        public string seq { get; set; }
        public string typeCode { get; set; }
        public string ReportedDate { get; set; }
        public string Number { get; set; }
    }

    public class Retailaccountssummary
    {
        public string NoOfAccounts { get; set; }
        public string NoOfActiveAccounts { get; set; }
        public string NoOfWriteOffs { get; set; }
        public string TotalPastDue { get; set; }
        public string MostSevereStatusWithIn24Months { get; set; }
        public string SingleHighestCredit { get; set; }
        public string SingleHighestSanctionAmount { get; set; }
        public string TotalHighCredit { get; set; }
        public string AverageOpenBalance { get; set; }
        public string SingleHighestBalance { get; set; }
        public string NoOfPastDueAccounts { get; set; }
        public string NoOfZeroBalanceAccounts { get; set; }
        public string RecentAccount { get; set; }
        public string OldestAccount { get; set; }
        public string TotalBalanceAmount { get; set; }
        public string TotalSanctionAmount { get; set; }
        public string TotalCreditLimit { get; set; }
        public string TotalMonthlyPaymentAmount { get; set; }
    }

    public class Enquirysummary
    {
        public string Purpose { get; set; }
        public string Total { get; set; }
        public string Past30Days { get; set; }
        public string Past12Months { get; set; }
        public string Past24Months { get; set; }
        public string Recent { get; set; }
    }

    public class Otherkeyind
    {
        public string AgeOfOldestTrade { get; set; }
        public string NumberOfOpenTrades { get; set; }
        public string AllLinesEVERWritten { get; set; }
        public string AllLinesEVERWrittenIn9Months { get; set; }
        public string AllLinesEVERWrittenIn6Months { get; set; }
    }

    public class Recentactivities
    {
        public string AccountsDeliquent { get; set; }
        public string AccountsOpened { get; set; }
        public string TotalInquiries { get; set; }
        public string AccountsUpdated { get; set; }
    }

    public class Microfinanceaccountssummary
    {
        public string id { get; set; }
        public string NoOfActiveAccounts { get; set; }
        public string TotalPastDue { get; set; }
        public string NoOfPastDueAccounts { get; set; }
        public string RecentAccount { get; set; }
        public string TotalBalanceAmount { get; set; }
        public string TotalMonthlyPaymentAmount { get; set; }
        public string TotalWrittenOffAmount { get; set; }
        public string Id { get; set; }
    }

    public class Familydetailsinfo
    {
        public string numberOfDependents { get; set; }
        public Relative[] relatives { get; set; }
    }

    public class Relative
    {
        public string AdditionalNameType { get; set; }
        public string AdditionalName { get; set; }
    }

    public class Retailaccountdetail
    {
        public string seq { get; set; }
        public string AccountNumber { get; set; }
        public string Institution { get; set; }
        public string AccountType { get; set; }
        public string OwnershipType { get; set; }
        public string Balance { get; set; }
        public string PastDueAmount { get; set; }
        public string Open { get; set; }
        public string SanctionAmount { get; set; }
        public string DateReported { get; set; }
        public string DateOpened { get; set; }
        public string InterestRate { get; set; }
        public string CollateralType { get; set; }
        public string RepaymentTenure { get; set; }
        public string AccountStatus { get; set; }
        public string AssetClassification { get; set; }
        public string source { get; set; }
        public History48months[] History48Months { get; set; }
        public string LastPayment { get; set; }
        public string LastPaymentDate { get; set; }
        public string DateClosed { get; set; }
        public string Reason { get; set; }
        public string InstallmentAmount { get; set; }
    }

    public class History48months
    {
        public string key { get; set; }
        public string PaymentStatus { get; set; }
        public string SuitFiledStatus { get; set; }
        public string AssetClassificationStatus { get; set; }
        public string AccountStatus { get; set; }
    }



    public class Scoringelement
    {
        public string type { get; set; }
        public string seq { get; set; }
        public string Description { get; set; }
        public string code { get; set; }
    }

    public class Enquiry
    {
        public string seq { get; set; }
        public string Institution { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string RequestPurpose { get; set; }
        public string Amount { get; set; }
    }

    public class Microfinanceaccountdetail
    {
        public string branchIDMFI { get; set; }
        public string kendraIDMFI { get; set; }
        public string seq { get; set; }
        public string id { get; set; }
        public string AccountNumber { get; set; }
        public string CurrentBalance { get; set; }
        public string Institution { get; set; }
        public string InstitutionType { get; set; }
        public string PastDueAmount { get; set; }
        public string DisbursedAmount { get; set; }
        public string LoanCategory { get; set; }
        public string LoanPurpose { get; set; }
        public string Open { get; set; }
        public string SanctionAmount { get; set; }
        public string LastPayment { get; set; }
        public string LastPaymentDate { get; set; }
        public string DateReported { get; set; }
        public string DateOpened { get; set; }
        public string LoanCycleID { get; set; }
        public string DateSanctioned { get; set; }
        public string DateApplied { get; set; }
        public string AppliedAmount { get; set; }
        public string NoOfInstallments { get; set; }
        public string RepaymentTenure { get; set; }
        public string InstallmentAmount { get; set; }
        public Keyperson KeyPerson { get; set; }
        public string AccountStatus { get; set; }
        public string DaysPastDue { get; set; }
        public string MaxDaysPastDue { get; set; }
        public string TypeOfInsurance { get; set; }
        public string InsurancePolicyAmount { get; set; }
        public string NumberOfMeetingsHeld { get; set; }
        public string NumberOfMeetingsMissed { get; set; }
        public string source { get; set; }
        public Additionalmfidetails AdditionalMFIDetails { get; set; }
        public string BranchIDMFI { get; set; }
        public string KendraIDMFI { get; set; }
        public History24months[] History24Months { get; set; }
        public string DateClosed { get; set; }
        public string Reason { get; set; }
        public string WriteOffAmount { get; set; }
        public Nominee Nominee { get; set; }
    }

    public class Keyperson
    {
        public string Name { get; set; }
        public string associationType { get; set; }
        public string RelationType { get; set; }
    }

    public class Additionalmfidetails
    {
        public string MFIClientFullname { get; set; }
        public string MFIDOB { get; set; }
        public string MFIGender { get; set; }
        public string MemberId { get; set; }
        public Mfiidentification MFIIdentification { get; set; }
        public Mfiaddress[] MFIAddress { get; set; }
        public Mfiphone[] MFIPhones { get; set; }
    }

    public class Mfiidentification
    {
        public Voterid1[] VoterID { get; set; }
        public Nationalidcard1[] NationalIDCard { get; set; }
        public Otherid1[] OtherId { get; set; }
    }

    public class Voterid1
    {
        public string IdNumber { get; set; }
    }

    public class Nationalidcard1
    {
        public string IdNumber { get; set; }
    }

    public class Otherid1
    {
        public string IdNumber { get; set; }
    }

    public class Mfiaddress
    {
        public string Seq { get; set; }
        public string ReportedDate { get; set; }
        public string Address { get; set; }
        public string Postal { get; set; }
        public string State { get; set; }
    }

    public class Mfiphone
    {
        public string seq { get; set; }
        public string ReportedDate { get; set; }
        public string Number { get; set; }
    }

    public class Nominee
    {
        public string Name { get; set; }
        public string RelationType { get; set; }
        public string associationType { get; set; }
    }

    public class History24months
    {
        public string key { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class Incomedetail
    {
        public string occupation { get; set; }
        public string monthlyIncome { get; set; }
        public string monthlyExpense { get; set; }
        public int seq { get; set; }
        public string reportedDate { get; set; }
        public string assetOwnership { get; set; }
    }

    //public class Score
    //{
    //    public string Type { get; set; }
    //    public string Version { get; set; }
    //}

    public class Score1
    {
        public string Type { get; set; }
        public string Version { get; set; }
    }

    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
    }

}



