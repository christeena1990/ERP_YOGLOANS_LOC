using ERP_YOGLOANS_LOCAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using ERP_YOGLOANS_LOCAL.Models.Equifax_Model;
using System.IO.Ports;
using System.Reflection;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_Canvaser_RegistrationController : Controller
    {
        // GET: NCD_Canvaser_Registration
        DB dbconnect = new DB();
        public ActionResult Canvaser_Registration_View()
        {
            return View();
        }

        public JsonResult GetCategories()
        {
            List<SelectListItem> categoryList = new List<SelectListItem>();

            try
            {
                // Fetch categories from the database
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 6 };

                dbconnect.Open();
                DataSet categoryDataSet = dbconnect.ExecuteDataset("[dbo].[DEB_canvasser]", parameters);
                dbconnect.Close();

                // Populate the categoryList from the DataSet
                if (categoryDataSet != null && categoryDataSet.Tables.Count > 0)
                {
                    foreach (DataRow row in categoryDataSet.Tables[0].Rows)
                    {
                        categoryList.Add(new SelectListItem
                        {
                            Value = row["category_canvasser_id"].ToString(),
                            Text = row["category_name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the error (consider logging it)
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(categoryList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetSuggestions(string searchType, string keyword)
        {
            List<canvasser_model> suggestionList = new List<canvasser_model>();

            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
            parameters[0].Value = 1; // Adjust as necessary for your logic

            // Configure parameters based on search type
            if (searchType == "ccode")
            {
                parameters[1] = new SqlParameter("@cancode", SqlDbType.BigInt);
                parameters[1].Value = keyword;
            }
            else if (searchType == "cname")
            {
                parameters[1] = new SqlParameter("@name", SqlDbType.VarChar, 150);
                parameters[1].Value = keyword;
            }
            else if (searchType == "mobileNo")
            {
                parameters[1] = new SqlParameter("@mob", SqlDbType.VarChar, 20);
                parameters[1].Value = keyword;
            }
            else if (searchType == "pancard")
            {
                parameters[1] = new SqlParameter("@pan", SqlDbType.VarChar, 20);
                parameters[1].Value = keyword;
            }

            DataTable dt = dbconnect.ExecuteDataset("[dbo].[DEB_canvasser]", parameters).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    suggestionList.Add(new canvasser_model
                    {
                        can_list = row["can_list"].ToString(),
                        can_code = Convert.ToInt32(row["can_code"]),
                    });
                }
            }
            suggestionList.Add(new canvasser_model
            {
                can_list = "",
                can_code = 0
            });

            return Json(suggestionList, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDataByCanCode(int canCode)
        {

            List<canvasser_model> data = new List<canvasser_model>();


            SqlParameter[] parameters = new SqlParameter[2];

            parameters[0] = new SqlParameter("@query_id", SqlDbType.Int);
            parameters[0].Value = 3; // Adjust as necessary for your logic
            parameters[1] = new SqlParameter("@cancode", SqlDbType.Int);
            parameters[1].Value = canCode;

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[DEB_canvasser]", parameters);

            string c_house = "", c_street = "", c_city = "", c_pin = "", c_post = "", Is_perm_add = "1";

            // Check if the first table exists and has rows
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                DataTable cdt = new DataTable();
                cdt = ds.Tables[0];
                var row = ds.Tables[0].Rows[0]; // Get the first row of the first table

                // Assign values to variables from the first table's columns
                c_house = row["c_house_name"] != DBNull.Value ? row["c_house_name"].ToString().ToUpper () : " ";
               

                c_street = row["c_street_land_location"] != DBNull.Value ? row["c_street_land_location"].ToString().ToUpper() : " ";
                c_city = row["c_city"] != DBNull.Value ? row["c_city"].ToString().ToUpper() : " ";
                c_pin = row["pin_code"] != DBNull.Value ? row["pin_code"].ToString() : " ";
                c_post = row["c_post_id"] != DBNull.Value ? row["c_post_id"].ToString() : "0";

               

                Is_perm_add = "1";

                //c_post = row["post_name"] != DBNull.Value ? row["post_name"].ToString() : "0";

            }
            string p_house = "", p_street = "", p_city = "", p_pin = "", p_post = "";
            if (ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            {

                DataTable pdt = new DataTable();
                pdt = ds.Tables[1];
                var row1 = ds.Tables[1].Rows[0]; // Get the first row of the first table
              


                // Assign values to variables from the first table's columns
                p_house = row1["p_house_name"] != DBNull.Value ? row1["p_house_name"].ToString().ToUpper() : "";
                p_street = row1["p_street_land_location"] != DBNull.Value ? row1["p_street_land_location"].ToString().ToUpper() : "";
                p_city = row1["p_city"] != DBNull.Value ? row1["p_city"].ToString().ToUpper() : "";
                p_pin = row1["pin_code"] != DBNull.Value ? row1["pin_code"].ToString() : "";
                p_post = row1["p_post_id"] != DBNull.Value ? row1["p_post_id"].ToString() : "0";
                //p_post = row1["post_name"] != DBNull.Value ? row1["post_name"].ToString() : "0";
                if (p_post == "0")

                { Is_perm_add = "0"; 
                }
                
                
            }
            string nomin_add = "", nomin_name = "", realtion = "";

            if (ds.Tables.Count > 0 && ds.Tables[2].Rows.Count > 0)
            {

                DataTable nomini_dt = new DataTable();
                nomini_dt = ds.Tables[2];
                var row2 = ds.Tables[2].Rows[0]; // Get the first row of the first table

                nomin_add = row2["Nominee_add"] != DBNull.Value ? row2["Nominee_add"].ToString().ToUpper() : "";
                nomin_name = row2["Nominee"] != DBNull.Value ? row2["Nominee"].ToString().ToUpper() : " ";
                realtion = row2["relation"] != DBNull.Value ? row2["relation"].ToString().ToUpper() : " ";

            }

            string caregoryid = "", categoryname = "";

            if (ds.Tables.Count > 0 && ds.Tables[3].Rows.Count > 0)
            {

                DataTable category_dt = new DataTable();
                category_dt = ds.Tables[3];
                var row3 = ds.Tables[3].Rows[0]; // Get the first row of the first table

                caregoryid = row3["category_canvasser_id"] != DBNull.Value ? row3["category_canvasser_id"].ToString() : "0";
                categoryname = row3["category_name"] != DBNull.Value ? row3["category_name"].ToString() : "------Select Category------";


            }


            string ifsc_code = "", bank_name = "", branch_name = "", bank_accno = "", payeename = "",bank_id="";


            if (ds.Tables.Count > 0 && ds.Tables[4].Rows.Count > 0)
            {

                DataTable bnk_dt = new DataTable();
                bnk_dt = ds.Tables[4];
                var row4 = ds.Tables[4].Rows[0];
                

                ifsc_code = row4["ifsc_code"] != DBNull.Value ? row4["ifsc_code"].ToString().ToUpper() : " ";
                bank_name = row4["bank_name"] != DBNull.Value ? row4["bank_name"].ToString().ToUpper() : " ";
                bank_id = row4["bank_id"] != DBNull.Value ? row4["bank_id"].ToString() : "0";
                branch_name = row4["branch_name"] != DBNull.Value ? row4["branch_name"].ToString().ToUpper() : " ";
                bank_accno = row4["bank_ac"] != DBNull.Value ? row4["bank_ac"].ToString() : " ";
                payeename = row4["payeename"] != DBNull.Value ? row4["payeename"].ToString().ToUpper() : " ";
            }



            string canvasser = "", can_code = "", mobile = "", pancard = "", address = "";

            if (ds.Tables.Count > 0 && ds.Tables[5].Rows.Count > 0)
            {

                DataTable canvas_dt = new DataTable();
                canvas_dt = ds.Tables[5];
                var row5 = ds.Tables[5].Rows[0];

                canvasser = row5["canvasser"] != DBNull.Value ? row5["canvasser"].ToString().ToUpper() : " ";
                can_code = row5["can_code"] != DBNull.Value ? row5["can_code"].ToString() : "";
                mobile = row5["mobile"] != DBNull.Value ? row5["mobile"].ToString() : "";
                pancard = row5["pancard"] != DBNull.Value ? row5["pancard"].ToString().ToUpper() : "";
                address = row5["address"] != DBNull.Value ? row5["address"].ToString().ToUpper() : "";
            }

            var model = new
            {
                c_house,
                c_street,
                c_city,
                c_pin,
                c_post,

                p_house,
                p_street,
                p_city,
                p_pin,
                p_post,

                Is_perm_add,

                nomin_add,
                nomin_name,
                realtion,

                caregoryid,
                categoryname,


                ifsc_code,
                bank_name,
                branch_name,
                bank_id,
                bank_accno,
                payeename,

                canvasser,
                can_code,
                mobile,
                pancard,
                address


            };
            return Json(model, JsonRequestBehavior.AllowGet);


        }
        private string GetValueFromDataSet(DataSet ds, int tableIndex, string columnName)
        {
            if (ds.Tables.Count > tableIndex && ds.Tables[tableIndex].Rows.Count > 0)
            {
                var row = ds.Tables[tableIndex].Rows[0];
                return row[columnName] != DBNull.Value ? row[columnName].ToString() : "0";
            }
            return "0";
        }
        public ActionResult c_PincodeChanged(string pincode)
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[1].Value = pincode.Trim();

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_pincode]", pr);

            // Assuming you have two DataTables in your DataSet

            List<SelectListItem> c_postOfficeList = new List<SelectListItem>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    c_postOfficeList.Add(new SelectListItem
                    {

                        Value = row["post_id"].ToString(),
                        Text = row["post_name"].ToString()
                    });
                }
            }
            return Json(c_postOfficeList, JsonRequestBehavior.AllowGet);





        }

        public ActionResult p_PincodeChanged(string pincode)
        {
            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[1].Value = pincode.Trim();

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_pincode]", pr);



            List<SelectListItem> p_postOfficeList = new List<SelectListItem>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    p_postOfficeList.Add(new SelectListItem
                    {

                        Value = row["post_id"].ToString(),
                        Text = row["post_name"].ToString()
                    });
                }
            }
            return Json(p_postOfficeList, JsonRequestBehavior.AllowGet);





        }


        public ActionResult ProcessIfsc(string ifsc)
        {
            SqlParameter[] pr1 = new SqlParameter[2];
            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 8;
            pr1[1] = new SqlParameter("@ifsc", SqlDbType.VarChar, 20);
            pr1[1].Value = ifsc.ToString();

            DataTable dt_bk = new DataTable();
            dbconnect.Open();
            dt_bk = dbconnect.ExecuteDataset("[dbo].[DEB_canvasser]", pr1).Tables[0];
            dbconnect.Close();
            string bank_id = "", bank_name = "", branch_name = "";

            if (dt_bk.Rows.Count > 0)
            {
                bank_id = dt_bk.Rows[0]["bank_id"].ToString();
                bank_name = dt_bk.Rows[0]["bank_name"].ToString();
                branch_name = dt_bk.Rows[0]["branch_name"].ToString();

            }
            var model = new
            {
                bank_id,
                bank_name,
                branch_name


            };
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult NCD_Canvaser_Create_Edit(FormCollection form)
        {
            try
            {
                // Extract form data
                //int canvasserCode = Convert.ToInt32(form["CanvasserName"]);
                string canvasserName = form["CanvasserName"];
                string canvasserType = form["canvasser_type"];
                string mobile = form["Mobile"];
                string landNo = form["LandNo"];
                string panCard = form["PanCard"];
                string nomineeName = form["NomineeName"];
                string nomineeRelation = form["NomineeRelation"];
                string nomineeAddress = form["NomineeAddress"];
                string cHousename = form["c_housename"];
                string cLandmark = form["c_landmark"];
                string cPin = form["c_pin"];
                string cPost = form["c_post"];
                string cCity = form["c_city"];
                string pHousename = form["p_housename"];
                string pLandmark = form["p_landmark"];
                string pPin = form["p_pin"];
                string pPost = form["p_post"];
                string pCity = form["p_city"];
                string ifsc = form["ifsc"];
                string bankName = form["bank_name"];
                string bankBranch = form["bank_branch"];
                string payee = form["payee"];
                string accNo = form["acc_no"];
                string bankId = form["bank_id"];
                string canvasserCode = form["cancassercode"];
                string checkbox = form["checkbox"];

                int type = 0;
                string outputMessage = string.Empty;

                string add_typ = string.Empty;

                // type 1 ->Canvasser  Creation
                // type 2 ->Canvasser  Modification

                type = Convert.ToInt32((canvasserCode == "0") ? "1" : "2");

                add_typ = (add_typ == "false") ? "S" : "O";

                SqlParameter[] pr_save = new SqlParameter[27];

                pr_save[0] = new SqlParameter("@type", SqlDbType.Int);
                pr_save[0].Value = type;

                pr_save[1] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr_save[1].Value = 9;

                pr_save[2] = new SqlParameter("@can_name", SqlDbType.VarChar, 100);
                pr_save[2].Value = canvasserName;

                pr_save[3] = new SqlParameter("@can_type", SqlDbType.Int);
                pr_save[3].Value =Convert.ToInt32( canvasserType);

                pr_save[4] = new SqlParameter("@mob", SqlDbType.VarChar, 15);
                pr_save[4].Value = mobile;

                pr_save[5] = new SqlParameter("@land_line", SqlDbType.VarChar, 15);
                pr_save[5].Value = landNo;

                pr_save[6] = new SqlParameter("@pan", SqlDbType.VarChar, 20);
                pr_save[6].Value = panCard;

                pr_save[7] = new SqlParameter("@nominee", SqlDbType.VarChar, 100);
                pr_save[7].Value = nomineeName;

                pr_save[8] = new SqlParameter("@relation", SqlDbType.VarChar, 100);
                pr_save[8].Value = nomineeRelation;

                pr_save[9] = new SqlParameter("@addnominee", SqlDbType.VarChar, 500);
                pr_save[9].Value = nomineeAddress;

                pr_save[10] = new SqlParameter("@c_house", SqlDbType.VarChar, 100);
                pr_save[10].Value = cHousename;

                pr_save[11] = new SqlParameter("@c_str", SqlDbType.VarChar, 100);
                pr_save[11].Value = cLandmark;

                pr_save[12] = new SqlParameter("@c_postid", SqlDbType.Int);
                pr_save[12].Value =Convert.ToInt32( cPost);

                pr_save[13] = new SqlParameter("@c_city", SqlDbType.VarChar, 100);
                pr_save[13].Value = cCity;


                if (checkbox == "true")
                {
                    pr_save[14] = new SqlParameter("@p_house", SqlDbType.VarChar, 100);
                    pr_save[14].Value = "";

                    pr_save[15] = new SqlParameter("@p_str", SqlDbType.VarChar, 100);
                    pr_save[15].Value = "";

                    pr_save[16] = new SqlParameter("@p_postid", SqlDbType.Int);
                    pr_save[16].Value = 0;

                    pr_save[17] = new SqlParameter("@p_city", SqlDbType.VarChar, 100);
                    pr_save[17].Value = "";
                }
                else
                {
                    pr_save[14] = new SqlParameter("@p_house", SqlDbType.VarChar, 100);
                    pr_save[14].Value = pHousename;

                    pr_save[15] = new SqlParameter("@p_str", SqlDbType.VarChar, 100);
                    pr_save[15].Value = pLandmark;

                    pr_save[16] = new SqlParameter("@p_postid", SqlDbType.Int);
                    pr_save[16].Value =Convert.ToInt32 (pPost);

                    pr_save[17] = new SqlParameter("@p_city", SqlDbType.VarChar, 100);
                    pr_save[17].Value = pCity;
                }

                

                pr_save[18] = new SqlParameter("@ifsc", SqlDbType.VarChar, 20);
                pr_save[18].Value = ifsc;

                pr_save[19] = new SqlParameter("@payee", SqlDbType.VarChar, 100);
                pr_save[19].Value = payee;

                pr_save[20] = new SqlParameter("@bank_ac", SqlDbType.VarChar, 100);
                pr_save[20].Value = accNo;

                pr_save[21] = new SqlParameter("@bank_id", SqlDbType.Int);
                pr_save[21].Value =Convert.ToInt32( bankId);

                pr_save[22] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr_save[22].Value = Session["branch_id"];

                pr_save[23] = new SqlParameter("@enter_by", SqlDbType.BigInt);
                pr_save[23].Value = Session["login_user"];

                pr_save[24] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr_save[24].Direction = ParameterDirection.Output;

                if (checkbox == "true")
                {
                    pr_save[25] = new SqlParameter("@add_type", SqlDbType.Char);
                    pr_save[25].Value = "S";

                }
                else { 

                pr_save[25] = new SqlParameter("@add_type", SqlDbType.Char);
                pr_save[25].Value = "O";
                }

                pr_save[26] = new SqlParameter("@can_code", SqlDbType.BigInt);
                pr_save[26].Value = Convert.ToInt32(canvasserCode);


                dbconnect.ExecuteStoredProcedure("[dbo].[DEB_canvasser]", pr_save); // Update the stored procedure name
                outputMessage = pr_save[24].Value.ToString();


                TempData["ResultMessage"] = outputMessage;

                if (outputMessage == "Confirmed Sucessfully...!")
                {
                    return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                }
                else if (outputMessage == "Updated Sucessfully...!")
                {
                    return Json(new { success = true, message = outputMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "An error occurred while saving the data. Please try again." }, JsonRequestBehavior.AllowGet);
                }

                // return Json(new { success = true, message = "Canvasser details saved successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log exception (implement logging as needed)
                return Json(new { success = false, message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}