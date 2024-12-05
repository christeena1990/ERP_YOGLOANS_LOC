using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_CustViewController : Controller
    {
        DB dbconnect = new DB();
        private object img_customer_m;
        private object ImageUrl1;
        private string base64String;

        // GET: Customer_CustView
        public ActionResult FillDetails(Customer_Approval_Model model)
        {
       

            int h_type = 1;

            if (h_type == 1 /*Session["Apr_custID"] != null*/)
            {

                //CustomerView_Model Model = new CustomerView_Model();

             

                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 10;

                pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                pr[1].Value = Session["Apr_custID"];


                DataSet ds = new DataSet();
                ds = dbconnect.ExecuteDataset("[dbo].[cust_approve_modify]", pr);
                dbconnect.Close();

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
                        model.mobile_no = dt_personal.Rows[0]["mobile_no"].ToString();
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

                        model.education = dt_personal.Rows[0]["education"].ToString();
                        model.occupation = dt_personal.Rows[0]["occupation"].ToString();
                        model.spouse = dt_personal.Rows[0]["spouse_name"].ToString();

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

                            model.Data.Add(kycItem);
                        }
                        //return View(model); // Pass the model to the view
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
                      
                    }
                  
                }


            }
            return View(model);
        }
    }
}


