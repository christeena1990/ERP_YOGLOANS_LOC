using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using ERP_YOGLOANS_LOCAL.Models.NCD_Models;
using System.Globalization;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Scheme_registrationController : BaseController
    {
        DB dbconnect = new DB();
        // GET: NCD_Scheme_registration
        public ActionResult NCD_Scheme_registration()
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

                DataSet ds1 = dbconnect.ExecuteDataset("[dbo].[DEB_issue_registration]", pr);
                dbconnect.Close();

                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    // Store issue number in Session
                    Session["Issue_no"] = ds1.Tables[0].Rows[0]["issue_no"].ToString();
                }
                else
                {
                    Session["Issue_no"] = "No issue number found";
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
                DataSet ds1 = dbconnect.ExecuteDataset("[dbo].[DEB_issue_registration]", pr);
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
        public ActionResult SubmitSchemeRegistration(SchemeRegistrationModel model)
        {
            try
            {
               
                // Default value handling for senior citizen interest
                if (string.IsNullOrEmpty(model.senior_citizen_interest))
                {
                    model.senior_citizen_interest = "0";
                }

                string period_type = "M";  // Assume fixed period type

                SqlParameter[] prr = new SqlParameter[26];

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

                prr[6] = new SqlParameter("@interest_rate", SqlDbType.Float);
                prr[6].Value = model.interest_rate;

                prr[7] = new SqlParameter("@total_deb_amount", SqlDbType.Float);
                prr[7].Value = model.total_fund;

                prr[8] = new SqlParameter("@face_value", SqlDbType.Float);
                prr[8].Value = model.face_value;

                prr[9] = new SqlParameter("@total_deb", SqlDbType.Int);
                prr[9].Value = int.TryParse(model.number_of_debentures, out int totalDeb) ? totalDeb : 0; // Default to 0 if conversion fails

                prr[10] = new SqlParameter("@min_amount", SqlDbType.Float);
                prr[10].Value = model.min_amount;

                prr[11] = new SqlParameter("@premature_closing", SqlDbType.Char, 1);
                prr[11].Value = model.premature_closing;

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
                prr[17].Value = model.canvasser_commission;

                prr[18] = new SqlParameter("candata", SqlDbType.VarChar, 4000);
                prr[18].Value = model.canvasser_commission_data;
               
                prr[19] = new SqlParameter("@predata", SqlDbType.VarChar, 4000);
                prr[19].Value = model.premature_closing_data;

                prr[20] = new SqlParameter("@period_type", SqlDbType.Char, 1);
                prr[20].Value = period_type;

                prr[21] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
                prr[21].Direction = ParameterDirection.Output;

                prr[22] = new SqlParameter("@trustee", SqlDbType.VarChar, 4000);
                prr[22].Value = model.trusteeName;

                prr[23] = new SqlParameter("@aggr_amt_from", SqlDbType.Float);
                prr[23].Value = model.agg_amt_from;

                prr[24] = new SqlParameter("@aggr_amt_to", SqlDbType.Float);
                prr[24].Value = model.agg_amt_to;

                prr[25] = new SqlParameter("@ppo_date", SqlDbType.Date);
                prr[25].Value = model.ppo_date; // No need to convert to string


                // Execute the stored procedure
                dbconnect.ExecuteStoredProcedure("[dbo].[DEB_issue_registration]", prr); // Assuming db is your database helper class

                // Check output message from stored procedure
                string outputMessage = prr[21].Value.ToString();
                // TempData["Message"] = "Scheme registration success: " + outputMessage;


                //////////////  alert when success or error    /////////

                if (outputMessage == "Confirmed Sucessfully...!")
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

    return RedirectToAction("NCD_Scheme_registration"); // Redirect after submission
}
            
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = "Error during scheme registration: " + ex.Message;
            //}

            //return RedirectToAction("NCD_Scheme_registration"); // Redirect to another action after successful registration
        
        
        }


    }








