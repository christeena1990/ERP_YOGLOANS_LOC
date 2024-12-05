using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;  // Make sure you have the correct namespace
using ERP_YOGLOANS_LOCAL.Models.Legality_Model;

namespace ERP_YOGLOANS_LOCAL.Controllers.Business_Loan
{
    public class BLKerala_ApplicationFormController : Controller
    {
        DB dbconnect = new DB();
        private object ImageUrl1;
        private string base64String;

        // GET: BLKerala_ApplicationForm
        public ActionResult BLKerala_ApplicationForm(string applicationId)
        {
            // If the applicationId is not provided, you can either return an empty view or handle it
            //if (string.IsNullOrEmpty(applicationId))
            //{
            //    return View();  // return empty view or handle accordingly
            //}

            // Now fetch data using the applicationId
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@module_id", SqlDbType.BigInt) { Value = 11 };
            pr[1] = new SqlParameter("@application_id", SqlDbType.BigInt) { Value = 11000623500454 };

            dbconnect.Open();
            DataSet ds = dbconnect.ExecuteDataset("application_merge", pr);  // Assuming `ExecuteDataset` runs the stored procedure
            dbconnect.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
                // Map the application data to the model
                var applicationFormData = MapApplicationFormData(ds.Tables[0]);

                // Pass the data to the view
                return View(applicationFormData);  // Pass the mapped applicationFormData to the view
            }
            else
            {
                // Handle the case where no data is found
                return View(); // or return some error message view if no data found
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
                applicationForm.AppliedLoanAmt1 = Convert.ToDouble(row["AppliedLoanAmt1"]);
                applicationForm.AppliedLoanAmtWords1 = row["AppliedLoanAmtWords1"].ToString();

                // Primary Applicant Details
                applicationForm.PrimaryApplicantName = row["PrimaryApplicantName"].ToString();
                applicationForm.PrimaryApplicantGuardianType = row["PrimaryApplicantGuardianType"].ToString();
                applicationForm.PrimaryApplicantGuardian = row["PrimaryApplicantGuardian"].ToString();
                applicationForm.PrimaryApplicantGender = row["PrimaryApplicantGender"].ToString();
                applicationForm.PrimaryApplicantAgeDOB = row["PrimaryApplicantAgeDOB"].ToString();
                applicationForm.PrimaryApplicantAgeDOB = row["Mobile"].ToString();
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
                applicationForm.Mobile = row["Mobile"].ToString();
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
    }
}

