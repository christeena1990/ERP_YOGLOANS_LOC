using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Reflection;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_DeclarationController : BaseController
    {
        DB dbconnect = new DB();
        private object img_customer_m;
        private object ImageUrl1;
        private string base64String;

        [HttpGet]
        public ActionResult Declaration()
        {
            declaration_view model = new declaration_view();
            SqlParameter[] pr2 = new SqlParameter[3];

            pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr2[0].Value = 9;

            pr2[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr2[1].Value = Convert.ToInt64(Session["TempCID"]);

            pr2[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr2[2].Value = Session["branch_id"];

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr2);

            if (ds.Tables.Count > 0)
            {
                DataTable dt_personal = ds.Tables[0];

                if (dt_personal.Rows.Count > 0)
                {
                    DataRow row = dt_personal.Rows[0];

                    model.cid = Convert.ToInt32(Session["TempCID"]);
                    model.first_name = row["first_name"].ToString();
                    model.second_name = row["second_name"].ToString();
                    model.mob_no = row["mobile_no"].ToString();
                    model.m_status = row["m_status"].ToString();
                    model.dob = row["dob"].ToString();
                    model.father_name = row["fathers_name"].ToString();
                    model.residence_type = row["residence_type"].ToString();
                    model.spouse_name = row["spouse_name"].ToString();
                    model.mother_name = row["mothers_name"].ToString();
                    model.gender = row["gender"].ToString();
                    model.occupation = row["occupation"].ToString();
                    model.commu_address = row["commu_address"].ToString();
                    model.c_district = row["comm_district"].ToString();
                    model.branch_name = row["branch_name"].ToString();
                    model.education = row["education"].ToString();
                    model.c_pin = row["commu_pin"].ToString();
                    model.c_state = row["commu_state"].ToString();
                  
                    model.p_address = row["per_address"].ToString() == "" ? row["commu_address"].ToString(): row["per_address"].ToString();
                    model.p_state = row["per_state"].ToString() == "" ? row["commu_state"].ToString() : row["per_state"].ToString();
                    model.p_pin = row["perm_pin"].ToString() == "" ? row["commu_pin"].ToString() : row["perm_pin"].ToString();
                   
                    float annualIncome;
                    if (float.TryParse(row["Annual_income"].ToString(), out annualIncome))
                    {
                        model.Annual_income = annualIncome;
                    }
                    else
                    {
                        // Handle parsing error
                    }
                }

                if (ds.Tables[1].Rows.Count > 0)     // For photo
                {
                    DataTable dtphoto = ds.Tables[1];

                    if (dtphoto.Rows.Count > 0)
                    {
                        object photoObject = dtphoto.Rows[0][0];

                        if (photoObject is byte[])
                        {
                            byte[] byte_photo = (byte[])photoObject;
                            string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                            model.photo = "data:image/png;base64," + base64String;
                        }
                        else if (photoObject is string)
                        {
                            string photoString = (string)photoObject;
                            byte[] byte_photo = Convert.FromBase64String(photoString);
                            string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                            model.photo = "data:image/png;base64," + base64String;
                        }
                        else
                        {
                            model.photo = "data:image/png;base64," + "default_base64_encoded_image_string";
                        }
                    }
                }

                DataTable dtKYC = ds.Tables[2];
                if (dtKYC.Rows.Count > 0)
                {
                    foreach (DataRow row in dtKYC.Rows)
                    {
                        int kycId = Convert.ToInt32(row["kyc_id"]);
                        string id_no = Convert.ToString(row["id_no"]);

                        // Populate the model properties based on KYC type
                        switch (kycId)
                        {
                            case 1: // PAN Card
                                model.PANCard = id_no;
                                break;
                            case 2: // PAN Card
                                model.passport = id_no;
                                break;
                            case 3: // Driving License
                                model.DrivingLicense = id_no;
                                break;
                            case 4: // Voters Identity Card
                                model.VotersIdentityCard = id_no;
                                break;
                            case 5: // Ration Card
                                model.RationCard = id_no;
                                break;
                            case 9: // Aadhar Card
                                model.AadharCard = id_no;
                                break;
                            case 15: // Aadhar Unmasked
                                model.AadharUnmasked = id_no;
                                break;
                            default:
                                // Handle other KYC types if necessary
                                break;
                        }
                    }
                }

                return View(model);
            }

            return View();
        }

        [HttpGet]
        public ActionResult form_60()

        {
            

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 7;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            //pr[1].Value = Convert.ToInt64(Session["cid"]);
            pr[1].Value = Convert.ToInt64(Session["TempCID"]);
           

            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr).Tables[0];
            if (dt.Rows.Count > 0)
            {

                //if (dt.Rows[0][0].ToString() == "0")
               // {
                    
                    //string disp_msg = "Already PAN attached,No need of taking Form60.";
                    //ViewBag.AlertMessage = "Already PAN attached. No need of taking Form60.";

                    //return RedirectToAction("kyc_approval", "Customer_KYC");
                    //return View("kyc_approval");


                    //TempData["AlertMessage"] = "Already PAN attached. No need of taking Form60.";
                    //return RedirectToAction("kyc_approval", "Customer_KYC");
               // }
               // else
              //  {
                    string dob = dt.Rows[0]["dob"].ToString();
                    string[] dobParts = dob.Split('-');


                    form_60_model model = new form_60_model
                    {
                        first_name = dt.Rows[0]["first_name"].ToString()==""?"": dt.Rows[0]["first_name"].ToString(),
                        second_name = dt.Rows[0]["second_name"].ToString(),
                        last_name = dt.Rows[0]["last_name"].ToString(),
                        dob = dt.Rows[0]["dob"].ToString(),


                        Year1 = dobParts[2].Length > 3 ? dobParts[2][0].ToString() : "",
                        Year2 = dobParts[2].Length > 3 ? dobParts[2][1].ToString() : "", // Second digit of day
                        Year3 = dobParts[2].Length > 3 ? dobParts[2][2].ToString() : "", // First digit of month
                        Year4 = dobParts[2].Length > 3 ? dobParts[2][3].ToString() : "", // Second digit of month
                        Month1 = dobParts[1].Length > 1 ? dobParts[1][0].ToString() : "", // First digit of year
                        Month2 = dobParts[1].Length > 1 ? dobParts[1][1].ToString() : "", // Second digit of year
                        Day1 = dobParts[0].Length > 1 ? dobParts[0][0].ToString() : "", // Third digit of year
                        Day2 = dobParts[0].Length > 1 ? dobParts[0][1].ToString() : "", // Fourth digit of year

                        fathers_name = dt.Rows[0]["fathers_name"].ToString(),
                        address = dt.Rows[0]["address"].ToString(),
                        land_mark = dt.Rows[0]["land_mark"].ToString(),
                        city = dt.Rows[0]["city"].ToString(),
                        district_name = dt.Rows[0]["district_name"].ToString(),
                        state_name = dt.Rows[0]["state_name"].ToString(),
                        pin_code = dt.Rows[0]["pin_code"].ToString(),
                        mobile_no = dt.Rows[0]["mobile_no"].ToString(),
                        aadhar = dt.Rows[0]["aadhar"].ToString(),
                        todaydate = dt.Rows[0]["todaysdt"].ToString()

                    };
                    return View("form_60", model);
               // }
            }
            return View();
            
        }
    }
}