using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models.Bond_models;
using System.Web.UI.WebControls;
namespace ERP_YOGLOANS_LOCAL.Controllers.Bond
{
    public class Bond_ApplicationFormController : Controller
    {
        DB dbconnect = new DB();


        // GET: Bond_ApplicationForm
        public ActionResult Bond_ApplicationForm()
        {
     
                Bond_ApplicationFormfill();
                FillTermsOfIssue();
                return View();
        }


            public ActionResult Bond_ApplicationFormfill()
            {
                string appid = Session["appid"] as string;
                string custid = Session["custid"] as string;
                var model = new DataFillViewModel();
                try
                {
                    // Set up parameters for the new stored procedure [dbo].[DEB_application_form]
                    SqlParameter[] pr = new SqlParameter[3];

                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    pr[0].Value = 4;

                    pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt);
                    pr[1].Value = appid; // Application ID

                    pr[2] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                    pr[2].Value = custid; // Customer ID

                    // Execute the stored procedure [dbo].[DEB_application_form]
                    DataTable dt = new DataTable();
                    dbconnect.Open();
                    DataSet ds = dbconnect.ExecuteDataset("[dbo].[BOND_application_form]", pr);
                    dbconnect.Close();

                    // Populate the model with necessary values
                    model.ApplicationId = appid;
                    //model.Date = DateTime.Now.ToString("dd MMMM, yyyy");
                    model.Date = DateTime.Now.ToString("dd-MMM-yyyy");


                    if (ds != null && ds.Tables.Count >= 0)
                    {
                        DataTable personal_dt = ds.Tables[0];
                        DataTable second_aplcnt = ds.Tables[1];
                        DataTable dt3 = ds.Tables[2];
                        DataTable pincode_dt = ds.Tables[3];
                        DataTable repayment_bank_dt = ds.Tables[4];
                        DataTable category_dt = ds.Tables[5];
                        DataTable declaration_dt = ds.Tables[6];
                        DataTable adrs_dt = ds.Tables[7];
                        DataTable kyc_dt = ds.Tables[8];
                        DataTable interest_bank_dt = ds.Tables[9];
                        DataTable cheque_dt = ds.Tables[10];
                        /*DataTable dp_dt = ds.Tables[11]*/;
                        DataTable aplcnt_photo_dt = ds.Tables[11];
                        DataTable coaplcnt_photo_dt = ds.Tables[12];

                        if (personal_dt.Rows.Count > 0)
                        {
                            var row = personal_dt.Rows[0];
                            model.CustomerId = row["customer_id"].ToString();
                            model.FirstApplicant = row["custname"].ToString();
                            model.FatherName = row["fathers_name"].ToString();
                            model.Email = row["email_id"].ToString();
                            model.Age = row["age"].ToString();
                            model.MobileNo = row["mobile_no"].ToString();
                            model.Contact = row["land_no"].ToString();
                            model.Dob = row["dob"].ToString();
                            model.PanCharacters = row["pancardnumber"].ToString().ToCharArray().Select(c => c.ToString()).ToArray();
                            model.Duration = row["period"].ToString();
                            model.DurationType = row["period_type"].ToString();
                            model.InterestRate = row["interest_rate"].ToString() + " % pa";
                            model.InterestTypeFixed = row["Interest_type"].ToString() == "F";
                            model.InterestTypeCompound = row["Interest_type"].ToString() == "C";
                            model.Nominee_Name = row["nominee_Name"].ToString();
                            model.Nominee_Relation = row["relation"].ToString();

                        
                            
                            
                        }

                        if (second_aplcnt.Rows.Count > 0)
                        {
                            var row = second_aplcnt.Rows[0];

                            model.SecondApplicant = row["custname"].ToString();

                        }




                        if (adrs_dt.Rows.Count > 0)
                        {
                            var row = adrs_dt.Rows[0];

                            model.Address = row["address"].ToString();

                        }


                        if (kyc_dt.Rows.Count > 0)
                        {
                            var row = kyc_dt.Rows[0];

                            model.KycDocName = row["id_names"].ToString();
                            model.KycNo = row["id_nos"].ToString();

                        }


                    


                        if (repayment_bank_dt.Rows.Count > 0)
                    {
                        var row = repayment_bank_dt.Rows[0];

                        model.repayment_payee_name = row["payee_name"].ToString();
                        model.repayment_banki_ac = row["banki_ac"].ToString();
                        model.repayment_ifsc_code = row["ifsc_code"].ToString();
                        model.repayment_bank_name = row["bank_name"].ToString();
                        model.repayment_branch_name = row["branch_name"].ToString();
                        
                    }

                    if (interest_bank_dt.Rows.Count > 0)
                    {
                        var row = interest_bank_dt.Rows[0];

                        model.interest_payee_name = row["payee_name"].ToString();
                        model.interest_banki_ac = row["banki_ac"].ToString();
                        model.interest_ifsc_code = row["ifsc_code"].ToString();
                        model.interest_bank_name = row["bank_name"].ToString();
                        model.interest_branch_name = row["branch_name"].ToString();
                    }



                    if (category_dt.Rows.Count > 0)
                        {
                            var row = category_dt.Rows[0];

                            model.Category = row["category"].ToString();

                        }

                       
                        if (cheque_dt.Rows.Count > 0)
                        {
                            model.GridDatacheque = cheque_dt.AsEnumerable()
                                .Select(row => cheque_dt.Columns.Cast<DataColumn>()
                                .ToDictionary(col => col.ColumnName, col => row[col]))
                                .ToList();

                            // Get column names to send with JSON
                            model.GridDatachequeColumns = cheque_dt.Columns.Cast<DataColumn>()
                                .Select(col => col.ColumnName)
                                .ToList();
                        }





                        if (declaration_dt.Rows.Count > 0)
                        {
                            var row = declaration_dt.Rows[0];

                            model.Place = row["branch_name"].ToString();
                            model.DeclarationDate = DateTime.Now.ToString("dd-MMM-yyyy");
                            model.Declaration = row["declaration"].ToString();

                        }

                        if (aplcnt_photo_dt.Rows.Count > 0)
                        {
                            var row = aplcnt_photo_dt.Rows[0];

                            // Convert photo bytes to Base64
                            byte[] photoBytes = (byte[])(row["photo"]);
                            model.Base64Photo = Convert.ToBase64String(photoBytes);


                        }

                    if (coaplcnt_photo_dt.Rows.Count > 0)
                    {
                        var row = coaplcnt_photo_dt.Rows[0];

                        // Convert photo bytes to Base64
                        byte[] photoBytes = (byte[])(row["photo"]);
                        model.Base64CoApplicantPhoto = Convert.ToBase64String(photoBytes);


                    }




                }
                    // Return the model as JSON for use in the AJAX call
                    return Json(new { success = true, model = model }, JsonRequestBehavior.AllowGet);

                    //return Json(model);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }



            ///////////////////////////////////////////

            public ActionResult FillTermsOfIssue()
            {
                // Initialize a combined model to pass both DataFill and TableModel
                var combinedModel = new CombinedViewModel
                {
                    DataFill = new DataFillViewModel(),
                    TableData = new TableModel()
                };

                string app_id = Session["appid"] as string;
                string custid = Session["custid"] as string;

                try
                {
                    // Prepare SQL parameters
                    SqlParameter[] pr = new SqlParameter[2];
                    pr[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = 3 };
                    pr[1] = new SqlParameter("@app_id", SqlDbType.BigInt) { Value = app_id };

                    // Execute the stored procedure to retrieve data
                    DataSet ds = dbconnect.ExecuteDataset("[dbo].[Bond_application_form]", pr);

                    // Process and assign data from the DataSet to the combined model
                    if (ds.Tables.Count > 0)
                    {
                        // Process Table 0 data for Body1Data
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            combinedModel.TableData.Body1Data = ds.Tables[0].AsEnumerable()
                                .Select(row => new TableRowModel
                                {
                                    RuleNo = row.Field<int>("rule_no"),
                                    RuleName = row.Field<string>("rule_name"),
                                    Data = row.Field<string>("data")
                                }).ToList();
                        }


                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        combinedModel.TableData.Body2Data = ds.Tables[1].AsEnumerable()
                            .Select(row => new TableRowModel
                            {
                                RuleName = row.Field<string>("declar"),
                                //RuleName = row.Field<string>("signat"),

                            }).ToList();
                    }


                }

                // Return the view with the populated model
                return View(combinedModel);
                }
                catch (Exception ex)
                {
                    // Handle error (e.g., log the error and show a user-friendly message)
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToAction("Error", "Home");
                }
            }




            ///////////////////////////////////////////////////////////////////////////









        }
    }









