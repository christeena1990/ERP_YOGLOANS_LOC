using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_approveController : BaseController
    {
        // private readonly db dbconnect = new db();


        DB dbconnect = new DB();
        private object img_customer_m;
        private object ImageUrl1;
        private string base64String;

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

        // GET: Customer_approve
        public ActionResult Cus_approve()
        {
            Customer_Approval_Model model = new Customer_Approval_Model();

            Session["Enter_by"] = Session["login_user"];
            //Session["status_id"] = "10";
            //if (Session["Apr_custID"] != null && Session["Approve_type"] = 1)
            if (Session["Apr_custID"] != null && (int)Session["Approve_type"] == 1)
            {


                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 5;

                pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                pr[1].Value = Session["Apr_custID"];


                DataSet ds = new DataSet();
                ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_new]", pr);
                dbconnect.Close();


                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt_personal = ds.Tables[0];

                        model.customer_temp_id = Convert.ToInt32(Session["Apr_custID"].ToString());
                        model.cust_create_date = dt_personal.Rows[0]["create_dt"].ToString();
                        model.cust_status = dt_personal.Rows[0]["Status"].ToString();
                        model.cust_branch = dt_personal.Rows[0]["branch_name"].ToString();
                        model.category = dt_personal.Rows[0]["Category"].ToString();
                        model.customer_name = dt_personal.Rows[0]["customer_name"].ToString();
                        model.mob_no = dt_personal.Rows[0]["mobile_no"].ToString();
                        model.residence_no = ds.Tables[0].Rows[0]["residence_no"].ToString();
                        model.office_no = ds.Tables[0].Rows[0]["office_no"].ToString();
                        model.email = dt_personal.Rows[0]["email_id"].ToString();
                        model.dob = dt_personal.Rows[0]["dob"].ToString();
                        model.gender = dt_personal.Rows[0]["gender"].ToString();
                        model.marital_status = dt_personal.Rows[0]["m_satus"].ToString();
                        model.father_name = dt_personal.Rows[0]["fathers_name"].ToString();
                        model.mother_name = dt_personal.Rows[0]["mothers_name"].ToString();
                        model.guardiantype = dt_personal.Rows[0]["guardiantype"].ToString();
                        model.guardian_name = dt_personal.Rows[0]["guardian_name"].ToString();
                        model.residence_type = dt_personal.Rows[0]["residence_type"].ToString();
                        model.yearofstay = dt_personal.Rows[0]["y_stay"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["y_stay"].ToString());
                        model.dist_from_branch = dt_personal.Rows[0]["distance_frm_branch"].ToString();
                       
                        model.education = dt_personal.Rows[0]["education"].ToString();
                        model.occupation = dt_personal.Rows[0]["occupation"].ToString();
                        model.spouse = dt_personal.Rows[0]["spouse_name"].ToString();

                        model.first_name = dt_personal.Rows[0]["first_name"].ToString();
                        model.second_name = dt_personal.Rows[0]["second_name"].ToString();
                        model.last_name = dt_personal.Rows[0]["last_name"].ToString();
                        if (dt_personal.Rows[0]["Annual_income"] == DBNull.Value)   //annual income as string for get point values also
                        {
                            model.annual_income = "0"; // or any default string for zero value
                        }
                        else
                        {
                            model.annual_income = dt_personal.Rows[0]["Annual_income"].ToString();
                        }



                    }


                    if (ds.Tables[1].Rows.Count > 0) // For photo
                    {
                        DataTable dtphoto = ds.Tables[1];
                        if (dtphoto.Rows.Count > 0)
                        {
                            // Assuming "photo" is the column name in the result set
                            byte[] byte_photo = (byte[])dtphoto.Rows[0]["photo"];
                            string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                            model.ImageUrl1 = "data:image/png;base64," + base64String;
                        }
                    }

                    if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    {
                        model.Data = new List<Dictionary<string, object>>();

                        foreach (DataRow data in ds.Tables[2].Rows)
                        {
                            var kycItem = new Dictionary<string, object>
                                {
                                    { "proof_type", data["Proof Type"].ToString() },
                                    { "kyc", data["KYC"].ToString() },
                                    { "id_no", data["ID-No"].ToString() },
                                    { "valid_from", data["Valid-From"].ToString() },
                                    { "valid_to", data["Valid-To"].ToString() },
                                    { "name", data["Name"].ToString() },
                                    { "father", data["FatherName"].ToString() },
                                    { "dob_kyc", data["DOB"].ToString() },
                                };

                            if (data["image"] != DBNull.Value)
                            {
                                byte[] byteImage = (byte[])data["image"];
                                string base64String = Convert.ToBase64String(byteImage, 0, byteImage.Length);

                                if (IsPdf(byteImage))
                                {
                                    // It's a PDF
                                    string base64Pdf = Convert.ToBase64String(byteImage, 0, byteImage.Length);
                                    kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);


                                }
                                else
                                {
                                    // It's an image
                                    string base64Image = Convert.ToBase64String(byteImage, 0, byteImage.Length);
                                    kycItem.Add("image", "data:image/png;base64," + base64Image);
                                }
                            }

                            model.Data.Add(kycItem);
                        }
                    }

                    if (ds.Tables[3].Rows.Count > 0)    //For Communication Address
                    {
                        DataTable dt_comm = ds.Tables[3];
                        if (dt_comm.Rows.Count > 0)
                        {

                            model.commu_address = dt_comm.Rows[0]["address"].ToString();
                            model.land_mark = dt_comm.Rows[0]["land_mark"].ToString();
                            model.c_city = dt_comm.Rows[0]["city"].ToString();
                            model.c_pin = dt_comm.Rows[0]["pin_code"].ToString();
                            model.c_post = dt_comm.Rows[0]["post_name"].ToString();
                            model.c_district = dt_comm.Rows[0]["district_name"].ToString();
                            model.c_state = dt_comm.Rows[0]["state_name"].ToString();
                        }
                    }
                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        DataTable dt_per = ds.Tables[4];
                        // Assuming there is only one row for the permanent address
                        DataRow perAddressRow = dt_per.Rows[0];

                        // Check if the permanent address has values
                        if (!string.IsNullOrEmpty(perAddressRow["address"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["land_mark"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["city"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["pin_code"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["post_name"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["district_name"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["state_name"].ToString()))
                        {
                            // If the permanent address has values, populate the permanent address in the model
                            model.IsPermanentSameAsCommunication = false;
                            model.p_address = perAddressRow["address"].ToString();
                            model.p_mark = perAddressRow["land_mark"].ToString();
                            model.p_city = perAddressRow["city"].ToString();
                            model.p_pin = perAddressRow["pin_code"].ToString();
                            model.p_post = perAddressRow["post_name"].ToString();
                            model.p_district = perAddressRow["district_name"].ToString();
                            model.p_state = perAddressRow["state_name"].ToString();
                        }
                        else
                        {
                            // If the permanent address has no values, set the flag in the model to true
                            model.IsPermanentSameAsCommunication = true;
                        }
                    }
                    else
                    {
                        // If there are no rows in the permanent address DataTable, set the flag in the model to true
                        model.IsPermanentSameAsCommunication = true;
                    }

                }
                ViewBag.ApprovalList = true;
                return View(model);
               

            }
            else
            {

                SqlParameter[] pr = new SqlParameter[3];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 2;

                pr[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
                pr[1].Value = Session["Apr_custID"];

                pr[2] = new SqlParameter("@cust_slno", SqlDbType.BigInt);
                pr[2].Value = Session["Slno"];


                DataSet ds = new DataSet();
                ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt_personal = ds.Tables[0];
                        // Convert rekyc_status to int and assign to model.rekyc_status
                        if (dt_personal.Rows[0]["rekyc_status"] != DBNull.Value)
                        {
                            model.rekyc_status = Convert.ToInt32(dt_personal.Rows[0]["rekyc_status"]);
                        }
                        else
                        {
                            model.rekyc_status = 0; // Default value if rekyc_status is null
                        }
                        model.rekyc_reason = dt_personal.Rows[0]["rekyc_reason"].ToString();
                        model.customer_id = Convert.ToInt32(Session["Apr_custID"].ToString());
                        model.cust_create_date = dt_personal.Rows[0]["create_dt"].ToString();
                        model.cust_status = dt_personal.Rows[0]["Status"].ToString();
                        model.cust_branch = dt_personal.Rows[0]["branch_name"].ToString();
                        model.category = dt_personal.Rows[0]["Category"].ToString();
                        model.customer_name = dt_personal.Rows[0]["customer_name"].ToString();

                        //model.first_name = dt_personal.Rows[0]["first_name"].ToString();
                        //model.second_name = dt_personal.Rows[0]["second_name"].ToString();
                        //model.last_name = dt_personal.Rows[0]["last_name"].ToString();

                        //model.mob_no = dt_personal.Rows[0]["mobile_no"].ToString();
                        model.residence_no = dt_personal.Rows[0]["residence_no"].ToString();
                        model.office_no = dt_personal.Rows[0]["office_no"].ToString();
                        model.email = dt_personal.Rows[0]["email_id"].ToString();
                        model.dob = dt_personal.Rows[0]["dob"].ToString();
                        model.gender = dt_personal.Rows[0]["gender"].ToString();
                        model.marital_status = dt_personal.Rows[0]["m_satus"].ToString();
                        model.father_name = dt_personal.Rows[0]["fathers_name"].ToString();
                        model.mother_name = dt_personal.Rows[0]["mothers_name"].ToString();
                        model.guardiantype = dt_personal.Rows[0]["guardiantype"].ToString();
                        model.guardian_name = dt_personal.Rows[0]["guardian_name"].ToString();
                        model.residence_type = dt_personal.Rows[0]["residence_type"].ToString();
                        model.yearofstay = dt_personal.Rows[0]["y_stay"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["y_stay"].ToString());
                        model.dist_from_branch = dt_personal.Rows[0]["distance_frm_branch"].ToString();
                        // model.annual_income = Convert.ToInt32(dt_personal.Rows[0]["Annual_income"].ToString());
                        //model.annual_income = dt_personal.Rows[0]["Annual_income"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["Annual_income"].ToString());
                        //model.education = dt_personal.Rows[0]["education"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["education"].ToString());
                        //model.occupation = dt_personal.Rows[0]["occupation"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["occupation"].ToString());

                        model.education = dt_personal.Rows[0]["education"].ToString();
                        model.occupation = dt_personal.Rows[0]["occupation"].ToString();
                        model.spouse = dt_personal.Rows[0]["spouse_name"].ToString();

                        //model.occupation = dt_personal.Rows[0]["occupation"] == DBNull.Value ? 0 : Convert.ToInt32(dt_personal.Rows[0]["occupation"].ToString());


                        if (dt_personal.Rows[0]["Annual_income"] == DBNull.Value)
                        {
                            model.annual_income = "0"; // or any default string for zero value
                        }
                        else
                        {
                            model.annual_income = dt_personal.Rows[0]["Annual_income"].ToString();

                        }



                    }


                    if (ds.Tables[1].Rows.Count > 0) // For photo
                    {
                        DataTable dtphoto = ds.Tables[1];
                        if (dtphoto.Rows.Count > 0)
                        {
                            // Assuming "photo" is the column name in the result set
                            byte[] byte_photo = (byte[])dtphoto.Rows[0]["photo"];
                            string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                            model.ImageUrl1 = "data:image/png;base64," + base64String;
                        }
                    }

                    if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                    {
                        model.Data = new List<Dictionary<string, object>>();

                        foreach (DataRow data in ds.Tables[2].Rows)
                        {
                            var kycItem = new Dictionary<string, object>
                                {
                                    { "proof_type", data["ProofType"].ToString() },
                                    { "kyc", data["KYC"].ToString() },
                                    { "id_no", data["id_no"].ToString() },
                                    { "valid_from", data["Valid_From"].ToString() },
                                    { "valid_to", data["Valid_To"].ToString() },
                                    { "name", data["Name"].ToString() },
                                    { "father", data["FatherName"].ToString() },
                                    { "dob_kyc", data["DOB"].ToString() },
                                };

                            if (data["image"] != DBNull.Value)
                            {
                                byte[] byteImage = (byte[])data["image"];
                                string base64String = Convert.ToBase64String(byteImage, 0, byteImage.Length);

                                if (IsPdf(byteImage))
                                {
                                    // It's a PDF
                                    string base64Pdf = Convert.ToBase64String(byteImage, 0, byteImage.Length);
                                    kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);


                                }
                                else
                                {
                                    // It's an image
                                    string base64Image = Convert.ToBase64String(byteImage, 0, byteImage.Length);
                                    kycItem.Add("image", "data:image/png;base64," + base64Image);
                                }
                            }

                            model.Data.Add(kycItem);
                        }
                    }

                    if (ds.Tables[3].Rows.Count > 0)    //For Communication Address
                    {
                        DataTable dt_comm = ds.Tables[3];
                        if (dt_comm.Rows.Count > 0)
                        {

                            model.commu_address = dt_comm.Rows[0]["address"].ToString();
                            model.land_mark = dt_comm.Rows[0]["land_mark"].ToString();
                            model.c_city = dt_comm.Rows[0]["city"].ToString();
                            model.c_pin = dt_comm.Rows[0]["pin_code"].ToString();
                            model.c_post = dt_comm.Rows[0]["post_name"].ToString();
                            model.c_district = dt_comm.Rows[0]["district_name"].ToString();
                            model.c_state = dt_comm.Rows[0]["state_name"].ToString();
                        }
                    }
                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        DataTable dt_per = ds.Tables[4];
                        // Assuming there is only one row for the permanent address
                        DataRow perAddressRow = dt_per.Rows[0];

                        // Check if the permanent address has values
                        if (!string.IsNullOrEmpty(perAddressRow["address"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["land_mark"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["city"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["pin_code"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["post_name"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["district_name"].ToString()) ||
                            !string.IsNullOrEmpty(perAddressRow["state_name"].ToString()))
                        {
                            // If the permanent address has values, populate the permanent address in the model
                            model.IsPermanentSameAsCommunication = false;
                            model.p_address = perAddressRow["address"].ToString();
                            model.p_mark = perAddressRow["land_mark"].ToString();
                            model.p_city = perAddressRow["city"].ToString();
                            model.p_pin = perAddressRow["pin_code"].ToString();
                            model.p_post = perAddressRow["post_name"].ToString();
                            model.p_district = perAddressRow["district_name"].ToString();
                            model.p_state = perAddressRow["state_name"].ToString();
                        }
                        else
                        {
                            // If the permanent address has no values, set the flag in the model to true
                            model.IsPermanentSameAsCommunication = true;
                        }
                    }
                    else
                    {
                        // If there are no rows in the permanent address DataTable, set the flag in the model to true
                        model.IsPermanentSameAsCommunication = true;
                    }
                    // Ensure dbContext is accessible in this scope
                    // Ensure customer_id is defined and has a value
                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        // Handle mobile number change details from table 5
                        DataTable dtMobChange = ds.Tables[5];
                        DataRow mobChangeRow = dtMobChange.Rows[0];
                        if (mobChangeRow["mobile"] != null)
                        {
                            model.mobile_no = mobChangeRow["mobile"].ToString();
                        }
                    }
                    //else
                    //{
                    //    // If no mobile number change details found, fetch mobile number from original customer table
                    //    var customer = dbContext.Customers.FirstOrDefault(c => c.CustomerId == customer_id);
                    //    if (customer != null)
                    //    {
                    //        model.mobile_no = customer.MobileNumber;
                    //    }
                    //    else
                    //    {
                    //        // Handle the case where the customer with the specified ID is not found
                    //        // For example, you can set a default mobile number or handle the error as per your requirement
                    //        model.mobile_no = "DefaultMobileNumber";
                    //    }
                    //}



                    if (ds.Tables[6].Rows.Count > 0)
                    {
                        // Handle name change details from table 6
                        DataTable dtNameChange = ds.Tables[6];
                        DataRow nameChangeRow = dtNameChange.Rows[0];
                        if (nameChangeRow["customer_name"] != null)
                        {
                            model.first_name = dtNameChange.Rows[0]["first_name"].ToString();
                            model.second_name = dtNameChange.Rows[0]["second_name"].ToString();
                            model.last_name = dtNameChange.Rows[0]["last_name"].ToString();
                        }
                    }


                    //if (ds.Tables[7].Rows.Count > 0)    //For Modification Status
                    //{
                    //    DataTable dt_comm = ds.Tables[7];
                    //    if (dt_comm.Rows.Count > 0)
                    //    {

                    //        model.modi_status = dt_comm.Rows[0]["photo_change"].ToString() + "   " + dt_comm.Rows[0]["kyc_change"].ToString() + " " + dt_comm.Rows[0]["mobile_change"].ToString() + " " +
                    //            dt_comm.Rows[0]["name_change"].ToString() + "   " + dt_comm.Rows[0]["data_change"].ToString();



                    //    }
                    //}
                    if (ds.Tables[7].Rows.Count > 0)    //For Modification Status
                    {
                        DataTable dt_comm = ds.Tables[7];
                        if (dt_comm.Rows.Count > 0)
                        {
                            List<string> changes = new List<string>();

                            // Check each change individually and add to the list if not empty
                            if (!string.IsNullOrEmpty(dt_comm.Rows[0]["photo_change"].ToString()))
                                changes.Add(dt_comm.Rows[0]["photo_change"].ToString());

                            if (!string.IsNullOrEmpty(dt_comm.Rows[0]["kyc_change"].ToString()))
                                changes.Add(dt_comm.Rows[0]["kyc_change"].ToString());

                            if (!string.IsNullOrEmpty(dt_comm.Rows[0]["mobile_change"].ToString()))
                                changes.Add(dt_comm.Rows[0]["mobile_change"].ToString());

                            if (!string.IsNullOrEmpty(dt_comm.Rows[0]["name_change"].ToString()))
                                changes.Add(dt_comm.Rows[0]["name_change"].ToString());

                            if (!string.IsNullOrEmpty(dt_comm.Rows[0]["data_change"].ToString()))
                                changes.Add(dt_comm.Rows[0]["data_change"].ToString());

                            // Join the changes with commas
                            model.modi_status = string.Join(", ", changes);
                        }
                    }


                    ViewBag.ApprovalList = false;
                }
                return View(model);
                //return RedirectToAction("Error");
            }
        }
        public ActionResult Cus_approvelist()
        {
            // Retrieve the approval result from TempData
            var approvalResult = TempData["ApprovalResult"] as string;

            // Your existing code for the action
            Customer_Approval_Model model = new Customer_Approval_Model();
            // ... populate model and perform other actions ...

            // Pass the approval result to the view, if needed
            ViewBag.ApprovalResult = approvalResult;

            return View(model);
        }



        public ActionResult ViewAllDocuments()             // For viewing all documents
        {
            Customer_Approval_Model model = new Customer_Approval_Model();

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 2;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = Session["Apr_custID"];

            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_new]", pr);

            if (ds.Tables.Count > 0)
            {
                DataTable dtDoc = ds.Tables[0];
                if (dtDoc.Rows.Count > 0)
                {
                    model.Data = new List<Dictionary<string, object>>();

                    foreach (DataRow data in ds.Tables[0].Rows)
                    {
                        var kycItem = new Dictionary<string, object>

                            {
                                { "Doc_name", data["Doc_name"].ToString() },
                            };


                        if (data["Doc"] != DBNull.Value)
                        {
                            byte[] byteDoc = (byte[])data["Doc"];
                            string base64String = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);

                            if (IsPdf(byteDoc))
                            {
                                // It's a PDF
                                string base64Pdf = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                            }
                            else
                            {
                                // It's an image
                                string base64Image = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                kycItem.Add("image", "data:image/png;base64," + base64Image);
                            }
                        }

                        model.Data.Add(kycItem);
                    }
                }
            }

            return View("ViewAllDocuments", model);
        }


        public ActionResult ViewAllDocuments_modi()             // For viewing all documents
        {
            Customer_Approval_Model model = new Customer_Approval_Model();

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 6;

            pr[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
            pr[1].Value = Session["Apr_custID"];

            pr[2] = new SqlParameter("@cust_slno", SqlDbType.BigInt);
            pr[2].Value = Session["Slno"];


            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr);

            if (ds.Tables.Count > 0)
            {
                DataTable dtDoc = ds.Tables[0];
                if (dtDoc.Rows.Count > 0)
                {
                    // Assuming model.Data1 is of type List<Dictionary<string, object>>
                    model.Data1 = new List<Dictionary<string, object>>();

                    foreach (DataRow data in ds.Tables[0].Rows)
                    {
                        var document = new Dictionary<string, object>
                            {
                                { "Doc_name", data["Doc_name"].ToString() }
                            };

                        if (data["Doc"] != DBNull.Value)
                        {
                            byte[] byteDoc = (byte[])data["Doc"];

                            if (IsPdf(byteDoc))
                            {
                                // It's a PDF
                                string base64Pdf = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                document.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                            }
                            else
                            {
                                // It's an image
                                string base64Image = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                document.Add("image", "data:image/png;base64," + base64Image);
                            }
                        }

                        model.Data1.Add(document);
                    }

                }
            }

            return View("ViewAllDocuments_modi", model);
        }


        public ActionResult ViewAllExistingKYC()             // For viewing all documents
        {
            Customer_Approval_Model model = new Customer_Approval_Model();

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 9;

            pr[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
            pr[1].Value = Session["Apr_custID"];


            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr);

            if (ds.Tables.Count > 0)
            {
                DataTable dtDoc = ds.Tables[0];
                if (dtDoc.Rows.Count > 0)
                {
                    // Assuming model.Data1 is of type List<Dictionary<string, object>>
                    model.Data1 = new List<Dictionary<string, object>>();

                    foreach (DataRow data in ds.Tables[0].Rows)
                    {
                        var document = new Dictionary<string, object>
                            {
                                { "Doc_name", data["Doc_name"].ToString() }
                            };

                        if (data["Doc"] != DBNull.Value)
                        {
                            byte[] byteDoc = (byte[])data["Doc"];

                            if (IsPdf(byteDoc))
                            {
                                // It's a PDF
                                string base64Pdf = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                document.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                            }
                            else
                            {
                                // It's an image
                                string base64Image = Convert.ToBase64String(byteDoc, 0, byteDoc.Length);
                                document.Add("image", "data:image/png;base64," + base64Image);
                            }
                        }

                        model.Data1.Add(document);
                    }

                }
            }

            return View("ViewAllExistingKYC", model);
        }

        [HttpPost]
        public ActionResult ApproveCustomer()
        {
            try
            {
                if (Session["Apr_custID"] != null && (int)Session["Approve_type"] == 1)
                {
                    DB dbconnect = new DB();

                    int queryId = 6;

                    SqlParameter[] parameters = new SqlParameter[4];

                    parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameters[0].Value = queryId;

                    parameters[1] = new SqlParameter("@custid_temp", SqlDbType.BigInt);
                    parameters[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    parameters[2].Direction = ParameterDirection.Output;

                    parameters[3] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                    parameters[3].Value = Session["Enter_by"].ToString();

                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_new]", parameters);

                    // Retrieve the result message from the output parameter
                    string msg = parameters[2].Value != DBNull.Value ? parameters[2].Value.ToString() : string.Empty;

                    // Return the result as JSON
                    return Json(new { result = msg });
                }
                else
                {
                    DB dbconnect = new DB();

                    int queryId = 3;

                    SqlParameter[] parameters = new SqlParameter[5];

                    parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameters[0].Value = queryId;

                    parameters[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
                    parameters[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                    parameters[2].Direction = ParameterDirection.Output;

                    parameters[3] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);
                    parameters[3].Value = Session["Enter_by"].ToString();

                    parameters[4] = new SqlParameter("@cust_slno", SqlDbType.BigInt);
                    parameters[4].Value = Session["Slno"];

                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_modify]", parameters);

                    // Retrieve the result message from the output parameter
                    string msg = parameters[2].Value != DBNull.Value ? parameters[2].Value.ToString() : string.Empty;

                    return Json(new { result = msg });


                    //string[] disp_msg = msg.Split('#');
                    //// Return the result as JSON
                    //return Json(new { result = disp_msg[0] });

                    //return RedirectToAction("Cus_approvelist");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");

                // Set a TempData message for the error case
                TempData["ApprovalResult"] = "Error occurred during approval process.";

                return RedirectToAction("Cus_approvelist");
            }
        }


        [HttpPost]
        public ActionResult RejectCustomer(string rejectReason)
        {
            try
            {
                if (Session["Apr_custID"] != null && (int)Session["Approve_type"] == 1)
                {
                    DB dbconnect = new DB();

                    int queryId = 7; // Use the appropriate query ID for rejection

                    SqlParameter[] parameter = new SqlParameter[5];

                    parameter[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameter[0].Value = queryId;

                    parameter[1] = new SqlParameter("@custid_temp", SqlDbType.BigInt);
                    parameter[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameter[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);  // Adjust the size accordingly
                    parameter[2].Direction = ParameterDirection.Output;

                    parameter[3] = new SqlParameter("@reject_reason", SqlDbType.NVarChar, 500);  // New parameter for rejection reason
                    parameter[3].Value = rejectReason; // Pass the rejection reason from the UI

                    parameter[4] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);  // New parameter for approve_reject_by
                    parameter[4].Value = Session["Enter_by"].ToString(); // Assuming it's a string, adjust accordingly


                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_new]", parameter);

                    // Retrieve the result message from the output parameter
                    string outmsg = parameter[2].Value != null ? parameter[2].Value.ToString() : string.Empty;
                    //string outmsg =  parameter[2].Value.ToString();


                    // Assuming you have a model or ViewBag for passing data to the view
                    ViewBag.ApprovalResult = outmsg;

                    return RedirectToAction("Cus_approvelist", "Customer_approvelist");
                }
                else
                {
                    DB dbconnect = new DB();

                    int queryId = 5; // Use the appropriate query ID for rejection

                    SqlParameter[] parameter = new SqlParameter[6];

                    parameter[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameter[0].Value = queryId;

                    parameter[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
                    parameter[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameter[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);  // Adjust the size accordingly
                    parameter[2].Direction = ParameterDirection.Output;

                    parameter[3] = new SqlParameter("@reject_reason", SqlDbType.NVarChar, 500);  // New parameter for rejection reason
                    parameter[3].Value = rejectReason; // Pass the rejection reason from the UI

                    parameter[4] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);  // New parameter for approve_reject_by
                    parameter[4].Value = Session["Enter_by"].ToString(); // Assuming it's a string, adjust accordingly

                    parameter[5] = new SqlParameter("@cust_slno", SqlDbType.BigInt);
                    parameter[5].Value = Session["Slno"];

                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_modify]", parameter);

                    // Retrieve the result message from the output parameter
                    string outmsg = parameter[2].Value != null ? parameter[2].Value.ToString() : string.Empty;
                    //string outmsg =  parameter[2].Value.ToString();


                    // Assuming you have a model or ViewBag for passing data to the view
                    ViewBag.ApprovalResult = outmsg;

                    return RedirectToAction("Cus_modiapprovelist", "Customer_approvelistModify");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");

                // Set a TempData message for the error case
                TempData["ApprovalResult"] = "Error occurred during rejection process.";

                return RedirectToAction("Cus_approvelist");
            }
        }
        [HttpPost]
        public ActionResult ReturnCustomer(string returnReason)
        {
            try
            {
                if (Session["Apr_custID"] != null && (int)Session["Approve_type"] == 1)
                {
                    DB dbconnect = new DB();

                    int queryId = 8; // Use the appropriate query ID for returning a customer

                    SqlParameter[] parameters = new SqlParameter[5];

                    parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameters[0].Value = queryId;

                    parameters[1] = new SqlParameter("@custid_temp", SqlDbType.BigInt);
                    parameters[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);  // Adjust the size accordingly
                    parameters[2].Direction = ParameterDirection.Output;

                    parameters[3] = new SqlParameter("@return_reason", SqlDbType.NVarChar, 500);  // New parameter for return reason
                    parameters[3].Value = returnReason; // Pass the return reason from the UI

                    parameters[4] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);  // New parameter for approve_reject_by
                    parameters[4].Value = Session["Enter_by"].ToString();

                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_new]", parameters);

                    // Retrieve the result message from the output parameter                  
                    string outmsg = parameters[2].Value != null ? parameters[2].Value.ToString() : string.Empty;

                    // Assuming you have a model or ViewBag for passing data to the view
                    ViewBag.ReturnResult = outmsg;

                    return RedirectToAction("Cus_approvelist", "Customer_approvelist");
                }
                else
                {
                    DB dbconnect = new DB();

                    int queryId = 4; // Use the appropriate query ID for returning a customer

                    SqlParameter[] parameters = new SqlParameter[6];

                    parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    parameters[0].Value = queryId;

                    parameters[1] = new SqlParameter("@cus_id", SqlDbType.BigInt);
                    parameters[1].Value = Convert.ToInt64(Session["Apr_custID"]);

                    parameters[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);  // Adjust the size accordingly
                    parameters[2].Direction = ParameterDirection.Output;

                    parameters[3] = new SqlParameter("@return_reason", SqlDbType.NVarChar, 500);  // New parameter for return reason
                    parameters[3].Value = returnReason; // Pass the return reason from the UI

                    parameters[4] = new SqlParameter("@enter_by", SqlDbType.VarChar, 50);  // New parameter for approve_reject_by
                    parameters[4].Value = Session["Enter_by"].ToString();

                    parameters[5] = new SqlParameter("@cust_slno", SqlDbType.BigInt);
                    parameters[5].Value = Session["Slno"];


                    dbconnect.ExecuteStoredProcedure("[dbo].[cust_approve_modify]", parameters);

                    // Retrieve the result message from the output parameter                  
                    string outmsg = parameters[2].Value != null ? parameters[2].Value.ToString() : string.Empty;

                    // Assuming you have a model or ViewBag for passing data to the view
                    ViewBag.ReturnResult = outmsg;

                    return RedirectToAction("Cus_modiapprovelist", "Customer_approvelistModify");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");

                // Set a TempData message for the error case
                TempData["ReturnResult"] = "Error occurred during customer return process.";

                return RedirectToAction("Cus_approvelist");
            }
        }



    }
}






