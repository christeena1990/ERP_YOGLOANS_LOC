using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using WebGrease.Css.Ast.Selectors;
using System.Reflection;
using System.Web.UI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Policy;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.text.pdf;
using System.IO;
using Org.BouncyCastle.Asn1.X509;
using System.IO.Ports;
using System.Web.Helpers;
using System.Drawing.Imaging;



namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{

    public class Customer_RegistrationController : BaseController
    {
        DB dbconnect = new DB();
        // GET: Customer_Registration
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();

        public ActionResult Customer_index()
        {
            return View();
        }
        public ActionResult Cust_Registration()
        {
            Customer_Registration_model customer_Registration_Model = new Customer_Registration_model();
            if (Session["MobileNo"] != null)
            {
                customer_Registration_Model.mob_no = Session["MobileNo"].ToString();

            }

            if (Session["CustomerName"] != null)
            {
                customer_Registration_Model.Customer_name = Session["CustomerName"].ToString();


            }

            if (Session["TempCID"] != null)
            {
                customer_Registration_Model.customer_id = Session["TempCID"].ToString();
            }


            //Function call for fill category
            DataTable categories = categorydropdown();
            ViewData["CategoryList"] = categories;

            //Function call for fill education
            DataTable edu = educationdropdown();
            ViewData["EducationList"] = edu;

            //Function call for fill occupation
            DataTable occu = occupationdropdown();
            ViewData["OccupationList"] = occu;

            //Function call for fill guardian type
            DataTable guardians = FillGuardianType();
            ViewData["guardianList"] = guardians;

            //Function call for fill gender
            DataTable gender = FillGender();
            ViewData["genderList"] = gender;

            //Function call for fill marital status
            DataTable m_status = Fillmarital_status();
            ViewData["m_statusList"] = m_status;

            string cid = Session["TempCID"].ToString();
        
                if ((Session["TempCID"].ToString() != null) && ((Session["cust_status"].ToString() == "0") || (Session["cust_status"].ToString() == "2")))

                {
                    string tempid = Session["TempCID"].ToString();
                string status_id = Session["cust_status"].ToString();

                SqlParameter[] pr = new SqlParameter[3];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 6;

                pr[1] = new SqlParameter("@custid_temp", SqlDbType.BigInt);
                pr[1].Value = tempid;

                pr[2] = new SqlParameter("@status_id", SqlDbType.BigInt);
                pr[2].Value = status_id;



                // Rest of your code for the if part goes here

                DataTable dt = new DataTable();
                dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];

                if (dt.Rows.Count > 0)
                {
                   
                    // Create a model instance and populate it with the data
                    Customer_Registration_model model = new Customer_Registration_model
                    {
                        
                        mob_no = Session["MobileNo"].ToString(),
                   
                        father_name = dt.Rows[0]["fathers_name"] != DBNull.Value ? dt.Rows[0]["fathers_name"].ToString() : "",
                        mother_name = dt.Rows[0]["mothers_name"] != DBNull.Value ? dt.Rows[0]["mothers_name"].ToString() : "",                      
                        spouse_name = dt.Rows[0]["spouse_name"]?.ToString() ?? "",
                        email = dt.Rows[0]["email_id"]?.ToString() ?? "",
                        yearofstay = dt.Rows[0]["y_stay"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["y_stay"]) : (int?)null,
                        residence_no = dt.Rows[0]["residence_no"]?.ToString() ?? "",
                        office_no = dt.Rows[0]["office_no"]?.ToString() ?? "",
                        house_name = dt.Rows[0]["address"]?.ToString() ?? "",
                        landmark = dt.Rows[0]["land_mark"]?.ToString() ?? "",
                        City = dt.Rows[0]["city"]?.ToString() ?? "",           
                        dob = string.IsNullOrEmpty(dt.Rows[0]["dob"].ToString()) ? (DateTime?)null : DateTime.ParseExact(dt.Rows[0]["dob"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        gender = dt.Rows.Count > 0 ? dt.Rows[0]["gender"].ToString() : string.Empty,
                        residence_type = dt.Rows.Count > 0 ? dt.Rows[0]["residence_type"].ToString() : string.Empty,                         
                        dist_from_branch = dt.Rows[0]["distance_frm_branch"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["distance_frm_branch"]) : (int?)null,
                        marital_status = dt.Rows.Count > 0 ? dt.Rows[0]["m_status"].ToString() : string.Empty,

                        annualincome = dt.Rows[0]["Annual_income"] != DBNull.Value ? Convert.ToSingle(dt.Rows[0]["Annual_income"]) : (float?)null,

          
                        Pincode = dt.Rows[0]["pin_code"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["pin_code"]) : (long?)null,

                     
                    PostOffice = dt.Rows.Count > 0 ? dt.Rows[0]["post_id"]?.ToString() : string.Empty,
                     Post_name = dt.Rows.Count > 0 ? dt.Rows[0]["post_name"]?.ToString() : string.Empty,
                     District = dt.Rows.Count > 0 ? dt.Rows[0]["district_name"]?.ToString() : string.Empty,
                     State = dt.Rows.Count > 0 ? dt.Rows[0]["state_name"]?.ToString() : string.Empty,
                   education = dt.Rows.Count > 0 && int.TryParse(dt.Rows[0]["education"]?.ToString(), out int educationResult) ? educationResult : 0,
                    occupation = dt.Rows.Count > 0 && int.TryParse(dt.Rows[0]["occupation"]?.ToString(), out int occupationResult) ? occupationResult : 0,


                       p_house_name = dt.Rows.Count > 0 && dt.Rows[0]["pr_address"] != DBNull.Value ? dt.Rows[0]["pr_address"].ToString() : string.Empty,
                    p_landmark = dt.Rows.Count > 0 && dt.Rows[0]["pr_land_mark"] != DBNull.Value ? dt.Rows[0]["pr_land_mark"].ToString() : string.Empty,
                   p_City = dt.Rows.Count > 0 && dt.Rows[0]["pr_city"] != DBNull.Value ? dt.Rows[0]["pr_city"].ToString() : string.Empty,
                    // p_Pincode = dt.Rows.Count > 0 && dt.Rows[0]["pr_pincode"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["pr_pincode"]) : 0,
                        p_Pincode = dt.Rows[0]["pr_pincode"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["pr_pincode"]) : (long?)null,



                        p_PostOffice = dt.Rows.Count > 0 && dt.Rows[0]["pr_post_id"] != DBNull.Value ? dt.Rows[0]["pr_post_id"].ToString() : string.Empty,
                     p_District = dt.Rows.Count > 0 && dt.Rows[0]["pr_district"] != DBNull.Value ? dt.Rows[0]["pr_district"].ToString() : string.Empty,
                     p_State = dt.Rows.Count > 0 && dt.Rows[0]["pr_state"] != DBNull.Value ? dt.Rows[0]["pr_state"].ToString() : string.Empty,
                     p_Post_name = dt.Rows.Count > 0 && dt.Rows[0]["pr_postname"] != DBNull.Value ? dt.Rows[0]["pr_postname"].ToString() : string.Empty,


                };

                    if (dt.Rows.Count > 0 && dt.Rows[0]["customer_category"] != DBNull.Value)
                    {
                        if (int.TryParse(dt.Rows[0]["customer_category"].ToString(), out int categoryId))
                        {
                            
                            model.category_id = categoryId;
                        }
                        else
                        {
                     
                            model.category_id = 0;
                        }
                    }
                    else
                    {
                       
                        model.category_id = 0; // or any default value you prefer
                    }



                    if (dt.Rows[0].IsNull("image"))
                    {
                        return View(model);
                    }
                    else
                    {
                        model.ImageUrl1 = GetImageUrl(dt.Rows[0]["image"] as byte[]);

                        byte[] imageData = dt.Rows[0]["image"] as byte[];
                        //string base64String = Convert.ToBase64String(imageData);
                        //string imageUrlBase64 = "data:image/png;base64," + base64String;
                        //Session["saved_img"] = imageUrlBase64;

                        
                        Session["saved_img"] = imageData;

                    }


                    return View(model);
                }
                else
                {
                    return View(); 
                }

            }

                else
                { 
                    return View(); 
                }


        }


        public DataTable categorydropdown()
        {
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            return dt;
        }



        public DataTable educationdropdown()
        {
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 9;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            return dt;
        }

        public DataTable occupationdropdown()
        {
            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 10;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            return dt;
        }



        public DataTable FillGuardianType()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 3;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            return dt;

        }


        public DataTable FillGender()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            return dt;

        }

        public DataTable Fillmarital_status()
        {

            SqlParameter[] pr = new SqlParameter[1];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 5;


            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];

            return dt;

        }

        // entering communication address pincode : post office,district,state automatically fill

        [HttpPost]
        public ActionResult PincodeChanged(string pincode)
        {

            DataTable categories = categorydropdown();
            ViewData["CategoryList"] = categories;

            //Function call for fill guardian type
            DataTable guardians = FillGuardianType();
            ViewData["guardianList"] = guardians;

            //Function call for fill gender
            DataTable gender = FillGender();
            ViewData["genderList"] = gender;

            //Function call for fill marital status
            DataTable m_status = Fillmarital_status();
            ViewData["m_statusList"] = m_status;

            // Your existing logic to fetch data from the database           

            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[1].Value = pincode.Trim();

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_pincode]", pr);

            // Assuming you have two DataTables in your DataSet


            if (ds != null && ds.Tables.Count > 1)
            {
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];

                if (dt1.Rows.Count > 0 && dt2.Rows.Count>0)
                {
                    List<SelectListItem> postOfficeList = dt1.AsEnumerable()
                    .Select(row => new SelectListItem
                    {
                        Value = row["post_id"].ToString(),
                        Text = row["post_name"].ToString()
                    })
                    .ToList();

                    // Convert DataTable dt2 to a list of SelectListItem for District dropdown
                    List<SelectListItem> districtList = dt2.AsEnumerable()
                        .Select(row => new SelectListItem
                        {
                            Value = "0",
                            Text = row["district_name"].ToString()
                        })
                        .ToList();
                    List<SelectListItem> stateList = dt2.AsEnumerable()
                        .Select(row => new SelectListItem
                        {
                            Value = "1",
                            Text = row["state_name"].ToString()
                        })
                        .ToList();

                    // Create an anonymous object to hold both lists
                    var result = new
                    {
                        PostOffices = postOfficeList,
                        Districts = districtList,
                        States = stateList
                    };

                    return Json(result);
                }
                else
                {
                    var result = new { };
                    return Json(result);
                }
              


                
            }


            else
            {
                var result = new { };
                return Json(result);
            }
            


        }

        // entering permanent address pincode : post office,district,state automatically fill


        [HttpPost]
        public ActionResult permanent_PincodeChanged(string pincode)
        {

          

                // Your existing logic to fetch data from the database           

                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 1;
                pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
                pr[1].Value = pincode.Trim();

                DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_pincode]", pr);

                // Assuming you have two DataTables in your DataSet


                if (ds != null && ds.Tables.Count > 1)
                {
                    DataTable dt1 = ds.Tables[0];
                    DataTable dt2 = ds.Tables[1];

                    // ... rest of your code


                    //        DataTable dt1 = ds.Tables[0];
                    //DataTable dt2 = ds.Tables[1];


                    List<SelectListItem> postOfficeList = dt1.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = row["post_id"].ToString(),
                    Text = row["post_name"].ToString()
                })
                .ToList();

                    // Convert DataTable dt2 to a list of SelectListItem for District dropdown
                    List<SelectListItem> districtList = dt2.AsEnumerable()
                        .Select(row => new SelectListItem
                        {
                            Value = "0",
                            Text = row["district_name"].ToString()
                        })
                        .ToList();
                    List<SelectListItem> stateList = dt2.AsEnumerable()
                .Select(row => new SelectListItem
                {
                    Value = "1",
                    Text = row["state_name"].ToString()
                })
                .ToList();

                    // Create an anonymous object to hold both lists
                    var result = new
                    {
                        PostOffices = postOfficeList,
                        Districts = districtList,
                        States = stateList
                    };

                    return Json(result);
                }


                else
                {
                    return Json(null);
                }
           


        }

        // for saving postid from the post office dropdown id in communication address


        [HttpPost]
        public ActionResult SavePostID(int selectedPostID)
        {
            try
            {
                Customer_Registration_model model=new Customer_Registration_model();
                // Store the selectedPostID in a temporary variable
                TempData["postid"] = selectedPostID;
                model.PostOffice = selectedPostID.ToString();

                // You can add additional logic here if needed

                // Return a success message or any desired response
                return Json("Success");
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                return Json(new { error = ex.Message });
            }
        }


        // for saving postid from the post office dropdown id in communication address


        [HttpPost]
        public ActionResult SavePostIDpermanent(int selectedPostID)
        {
            try
            {
                Customer_Registration_model model = new Customer_Registration_model();
                // Store the selectedPostID in a temporary variable
                TempData["per_postid"] = selectedPostID;
                model.PostOffice = selectedPostID.ToString();

                // You can add additional logic here if needed

                // Return a success message or any desired response
                return Json("Success");
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                return Json(new { error = ex.Message });
            }
        }





       
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
        private string GetImageUrl(byte[] imageBytes)
        {


            string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

            if (IsPdf(imageBytes))
            {
                // It's a PDF
                string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                //kycItem.Add("pdf", "data:application/pdf;base64," + base64Pdf);
                return $"data:application/pdf;base64,{base64String}";

            }
            else
            {
                // It's an image
                string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                // kycItem.Add("image", "data:image/png;base64," + base64Image);
                return $"data:image;base64,{base64String}";
            }
        }





        public ActionResult RegisterTempCustomer(Customer_Registration_model modelObj)
        
        {
            try
            {


                byte[] imageBytes = null;
                modelObj.Customer_name = Session["CustomerName"].ToString();
                modelObj.mob_no = Session["MobileNo"].ToString();

              

                if (modelObj.image != null && modelObj.image.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        modelObj.image.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (modelObj.image.ContentType == "application/pdf")
                        {
                            imageBytes = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (modelObj.image.ContentType == "image/png" ||
                              modelObj.image.ContentType == "image/jpg" ||
                              modelObj.image.ContentType == "image/jpeg" ||
                              modelObj.image.ContentType == "image/bmp" ||
                              modelObj.image.ContentType == "image/gif")
                        {
                            imageBytes = memoryStream.Length > 256 * 1024
                                ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                                : memoryStream.ToArray();
                        }
                        else
                        {
                            // Handle unsupported content types or throw an error
                            throw new NotSupportedException("Unsupported file type.");
                        }
                    }
                }


                SqlParameter[] pr2 = new SqlParameter[32];

                pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr2[0].Value = 12;

                pr2[1] = new SqlParameter("@custcategory", SqlDbType.Int);
                pr2[1].Value = modelObj.category_id;

                string msg = modelObj.dob.ToString();
                string[] parts = msg.Split(' ');
                string part1 = parts[0].ToString();
                string part2 = parts[1].ToString();

                pr2[2] = new SqlParameter("@dob", SqlDbType.Date);         
                pr2[2].Value = part1;

                pr2[3] = new SqlParameter("@mstatus", SqlDbType.VarChar, 25);
                pr2[3].Value = modelObj. marital_status;

                pr2[4] = new SqlParameter("@gender", SqlDbType.Char,1);
                pr2[4].Value =modelObj. gender;

                pr2[5] = new SqlParameter("@residence", SqlDbType.VarChar, 15);
                pr2[5].Value =modelObj.residence_no;

                pr2[6] = new SqlParameter("@office", SqlDbType.VarChar, 15);
                pr2[6].Value =modelObj.office_no;

                pr2[7] = new SqlParameter("@distance", SqlDbType.BigInt);
                pr2[7].Value = modelObj.dist_from_branch;

                pr2[8] = new SqlParameter("@guardian_type", SqlDbType.Char, 10);
                pr2[8].Value = modelObj.guardian;

                pr2[9] = new SqlParameter("@fath_name", SqlDbType.VarChar, 200);
                pr2[9].Value = modelObj.father_name;

                pr2[10] = new SqlParameter("@mother_name", SqlDbType.VarChar, 200);
                pr2[10].Value =modelObj. mother_name;

                pr2[11] = new SqlParameter("@guardian_name", SqlDbType.VarChar, 200);
                pr2[11].Value = modelObj.guardian_name;

                pr2[12] = new SqlParameter("@res_type", SqlDbType.Char, 1);
                pr2[12].Value = modelObj. residence_type;


                pr2[13] = new SqlParameter("@commuaddress", SqlDbType.VarChar, 100);
                pr2[13].Value = modelObj.house_name;

                pr2[14] = new SqlParameter("@c_land", SqlDbType.VarChar, 100);
                pr2[14].Value = modelObj.landmark;

                pr2[15] = new SqlParameter("@c_city", SqlDbType.VarChar, 100);
                pr2[15].Value = modelObj.City;

                pr2[16] = new SqlParameter("@c_postid", SqlDbType.BigInt);
                pr2[16].Value = modelObj.PostOffice;


                pr2[17] = new SqlParameter("@permaddress", SqlDbType.VarChar, 100);
                pr2[17].Value = modelObj.p_house_name;

                pr2[18] = new SqlParameter("@p_land", SqlDbType.VarChar, 100);
                pr2[18].Value = modelObj.p_landmark;

                pr2[19] = new SqlParameter("@p_city", SqlDbType.VarChar, 100);
                pr2[19].Value = modelObj.p_City;

                pr2[20] = new SqlParameter("@p_postid", SqlDbType.BigInt);
                pr2[20].Value = modelObj.p_PostOffice;

                pr2[21] = new SqlParameter("@email", SqlDbType.VarChar, 200);
                pr2[21].Value = modelObj.email;

                pr2[22] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr2[22].Value = Session["TempCID"];

                pr2[23] = new SqlParameter("@ystay", SqlDbType.Int);
                pr2[23].Value = modelObj.yearofstay;

                pr2[24] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr2[24].Direction = ParameterDirection.Output;

                pr2[25] = new SqlParameter("@annual_income", SqlDbType.Float);
                pr2[25].Value = modelObj.annualincome;



                pr2[26] = new SqlParameter("@spouse_name", SqlDbType.VarChar, 50);
                pr2[26].Value = modelObj.spouse_name;

                pr2[27] = new SqlParameter("@education", SqlDbType.Int);
                pr2[27].Value = modelObj.education;

                pr2[28] = new SqlParameter("@occupation", SqlDbType.Int);
                pr2[28].Value = modelObj.occupation;

                Session["per_attach_img"] = imageBytes;
                if (string.IsNullOrEmpty(Session["per_attach_img"]?.ToString()))
                

                {
                    pr2[29] = new SqlParameter("@add_doc_img", SqlDbType.Binary);
                    pr2[29].Value = Session["saved_img"];

                }
                else
                {
                    pr2[29] = new SqlParameter("@add_doc_img", SqlDbType.Binary);
                    pr2[29].Value = Session["per_attach_img"];
                   
                }

                if (modelObj.p_PostOffice == null|| modelObj.p_PostOffice == "")
                {
                    pr2[30] = new SqlParameter("@permaddress_status", SqlDbType.Int);
                    pr2[30].Value = 0;

                }
                else
                {
                    pr2[30] = new SqlParameter("@permaddress_status", SqlDbType.Int);
                    pr2[30].Value = 1;
                }

                pr2[31] = new SqlParameter("@user", SqlDbType.Int);
                pr2[31].Value = Session["login_user"];

                dbconnect.ExecuteStoredProcedure("[cust_temp_create]", pr2);

                string registrationMessage = pr2[24].Value.ToString();
                return RedirectToAction("kyc_approval", "Customer_KYC");

            }
            catch
            {
                return View("_error_partial");
            }
        }


    }


}



