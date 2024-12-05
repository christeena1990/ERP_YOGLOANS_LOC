using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.Bond_models;
using System.Globalization;

namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_Scheme_RegistrationController : BaseController
    {
        // GET: Bond_Scheme_Registration
      

        DB dbconnect = new DB();
        
        public ActionResult Bond_Scheme_Registration()
        {

            fillissuenumber();
            fillfacevalue();
            return View();
        }



        public void fillissuenumber()
        {
            try
            {
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 1;

                DataSet ds1 = dbconnect.ExecuteDataset("[dbo].[BOND_issue_registration]", pr);
                dbconnect.Close();

                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    // Store issue number in Session
                    Session["Issue_no_bond"] = ds1.Tables[0].Rows[0]["issue_no"].ToString();
                }
                else
                {
                    Session["Issue_no_bond"] = "No issue number found";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching issue number: " + ex.Message);
            }
        }




        public void fillfacevalue()
        {
            try
            {
                // Define the SQL parameter
                SqlParameter[] pr = new SqlParameter[1];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 2;

                // Execute the stored procedure and retrieve the dataset
                DataSet ds1 = dbconnect.ExecuteDataset("[dbo].[BOND_issue_registration]", pr);
                dbconnect.Close();

                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    // Store facevalue in the Session to be used in the view
                    Session["FaceValue"] = ds1.Tables[0].Rows[0]["facevalue"].ToString();
                }
                else
                {
                    Session["FaceValue"] = "No face value found";
                }
            }
            catch (Exception ex)
            {
                // Handle error using TempData or ViewBag
                TempData["ErrorMessage"] = "Error fetching face value: " + ex.Message;
            }
        }


        // Action for submitting the form
        [HttpPost]
        public ActionResult SubmitSchemeRegistration(bond_scheme_model model)
        {
            try
            {
                
                // Default value handling for senior citizen interest
                if (string.IsNullOrEmpty(model.senior_citizen_interest))
                {
                    model.senior_citizen_interest = "0";
                }

                string period_type = "M";  // Assume fixed period type

                SqlParameter[] prr = new SqlParameter[21];

                prr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                prr[0].Value = 3;


                prr[1] = new SqlParameter("@issue_no", SqlDbType.BigInt);
                prr[1].Value = model.issue_no;

                prr[2] = new SqlParameter("@startdate", SqlDbType.Date);
                prr[2].Value = model.issue_start != null ? (object)model.issue_start : DBNull.Value;

                prr[3] = new SqlParameter("@enddate", SqlDbType.Date);
                prr[3].Value = model.issue_end != null ? (object)model.issue_end : DBNull.Value;

                prr[4] = new SqlParameter("@period", SqlDbType.Int);
                prr[4].Value = model.period;

                prr[5] = new SqlParameter("@interest_type", SqlDbType.Char, 1);
                prr[5].Value = model.interest_type;

                prr[6] = new SqlParameter("@interest_ratefix", SqlDbType.Float);
                prr[6].Value = model.interest_rate_fixed;

                prr[7] = new SqlParameter("@total_bond_amount", SqlDbType.Float);
                prr[7].Value = model.total_fund;

                prr[8] = new SqlParameter("@face_value", SqlDbType.Float);
                prr[8].Value = model.face_value;

                prr[9] = new SqlParameter("@total_bond", SqlDbType.Int);
                prr[9].Value = int.TryParse(model.number_of_debentures, out int totalDeb) ? totalDeb : 0; // Default to 0 if conversion fails

                prr[10] = new SqlParameter("@min_amount", SqlDbType.Float);
                prr[10].Value = model.min_amount;

                prr[11] = new SqlParameter("@period_type", SqlDbType.Char, 1);
                prr[11].Value = period_type;


                prr[12] = new SqlParameter("@locking_period", SqlDbType.Int);
                prr[12].Value = model.locking_period;

                prr[13] = new SqlParameter("@sc_interest", SqlDbType.Float);
                prr[13].Value = model.senior_citizen_interest;

                prr[14] = new SqlParameter("@sci", SqlDbType.Char, 1);
                prr[14].Value = model.senior_citizen;

                prr[15] = new SqlParameter("@meet_date", SqlDbType.VarChar, 10);
                prr[15].Value = model.meet_date.ToString("dd/MM/yyyy");

         
                prr[16] = new SqlParameter("@enter_by", SqlDbType.BigInt);
                prr[16].Value = Session["login_user"]; // Assuming session for user ID

                prr[17] = new SqlParameter("@canvasser_commission", SqlDbType.Char, 1);
                //prr[19].Value = can_type;
                prr[17].Value = model.canvasser_commission;

                prr[18] = new SqlParameter("candata", SqlDbType.VarChar, 4000);
                //prr[20].Value = Request.Form["candata"]; // Assuming form values here
                prr[18].Value = model.canvasser_commission_data;


                prr[19] = new SqlParameter("@interest_ratecu", SqlDbType.Float);
                prr[19].Value = model.interest_rate_cmltv;


                //prr[20] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                //prr[20].Value = ParameterDirection.Output;
                prr[20] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                prr[20].Direction = ParameterDirection.Output;


                // Execute the stored procedure
                dbconnect.ExecuteStoredProcedure("[dbo].[BOND_issue_registration]", prr); // Assuming db is your database helper class

                // Check output message from stored procedure
                string outputMessage = prr[20].Value.ToString();

                //////////////  alert when success or error    /////////



                if (outputMessage == "Confirmed Successfully...!")
                {
                    TempData["AlertType"] = "success"; // Success flag
                }
                else
                {
                    TempData["AlertType"] = "error"; // Error flag
                }

                TempData["Message"] = outputMessage; // Store the output message
            }


            catch (Exception ex)
            {
                TempData["AlertType"] = "error"; // Error flag
                TempData["Message"] = "Error during scheme registration: " + ex.Message; // Store the exception message
            }

            return RedirectToAction("Bond_Scheme_Registration"); // Redirect after submission
        }




        //    TempData["Message"] = "Scheme registration success: " + outputMessage;
        //}
        //catch (Exception ex)
        //{
        //    TempData["ErrorMessage"] = "Error during scheme registration: " + ex.Message;
        //}

        //return RedirectToAction("Bond_Scheme_Registration"); // Redirect to another action after successful registration



    }
}








