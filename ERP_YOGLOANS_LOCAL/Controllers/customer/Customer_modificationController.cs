using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Security.Cryptography;
using System.Globalization;
using System.Net.NetworkInformation;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using iTextSharp.text.pdf;
using System.IO.Ports;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{

    public class Customer_modificationController : BaseController


    {
        //GET: Customer_modification
        DB dbconnect = new DB();
      
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();
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




        public ActionResult customer_modification()
        {

            ViewBag.AlertMessage = TempData["AlertMessage"] as string;
            string sl_no = TempData["HiddenValue"] != null ? TempData["HiddenValue"].ToString() : "";

            SqlParameter[] pr6 = new SqlParameter[2];


            modify_model model = new modify_model();
            byte[] photoData;

            string return_slno = Session["cust_slno"].ToString() == "" ? "" : Session["cust_slno"].ToString();


            if (sl_no == "" && return_slno == "")
            {


                SqlParameter[] pr = new SqlParameter[2];

                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 25;

                pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                DataTable dt1 = new DataTable();
                dbconnect.Open();   
                dt1 = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr).Tables[0];
                dbconnect.Close();
                if (dt1 != null && dt1.Rows.Count > 0 && dt1.Rows[0]["image"] != DBNull.Value)
                {
                    byte[] imageData = dt1.Rows[0]["image"] as byte[];
                    string base64Image = Convert.ToBase64String(imageData);
                    string metadata = "data:image;base64," + base64Image;  // Assuming it's an image
                    model.ImageUrl_pattach = metadata;
                    Session["imageurl_pattach"] = metadata;
                }

                photoData = GetExistingPhotoData_modification();
                if (photoData != null)
                {
                    string base64Photo = Convert.ToBase64String(photoData);

                    // Pass Base64 string to the view
                    ViewBag.PhotoData = base64Photo;
                }

                pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr6[0].Value = 1;

                pr6[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                pr6[1].Value = Session["CustomerID_M"].ToString();

                DataSet ds = new DataSet();
                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr6);
                dbconnect.Close();

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        DataTable dt_customer = ds.Tables[0];
                        model.customer_id = Session["CustomerID_M"] != null ? Convert.ToInt32(Session["CustomerID_M"]) : 0;


                        model.first_name = dt_customer.Rows[0]["first_name"]?.ToString() ?? "";
                        model.second_name = dt_customer.Rows[0]["second_name"]?.ToString() ?? "";
                        model.last_name = dt_customer.Rows[0]["last_name"]?.ToString() ?? "";
                        model.Customer_name = dt_customer.Rows[0]["customer_name"]?.ToString() ?? "";
                        Session["Customer_fullname"] = model.Customer_name;
                        model.category_id = dt_customer.Rows[0]["customer_category"]?.ToString() ?? "";
                        model.branch_name = dt_customer.Rows[0]["branch_name"]?.ToString() ?? "";
                        //model.create_date = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;


                        //DateTime create_datee = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;
                        //string create_dateee = create_datee.ToString();
                        //string[] msg = create_dateee.Split(' ');
                        //string date = msg[0].ToString();
                        //string time = msg[1].ToString();
                        //model.create_date = date;


                        DateTime create_datee = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;

                        // Format the date to string without time part
                        string create_dateee = create_datee.ToString("dd-MMM-yyyy");

                        // Assign the formatted string to the model
                        model.create_date = create_dateee;



                        model.status_id = dt_customer.Rows[0]["Status"]?.ToString() ?? "";
                        model.mob_no = dt_customer.Rows[0]["mobile_no"]?.ToString() ?? "";
                        Session["OriginalMobileNo"] = model.mob_no;
                        model.land_no = dt_customer.Rows[0]["land_no"]?.ToString() ?? "";
                        model.office_no = dt_customer.Rows[0]["office_no"]?.ToString() ?? "";
                        model.email = dt_customer.Rows[0]["email_id"]?.ToString() ?? "";
                        //model.dob = dt_customer.Rows[0]["dob"].ToString() == "" ? "" : dt_customer.Rows[0]["dob"].ToString();

                        model.dob = dt_customer.Rows[0]["dob"].ToString();

                        model.gender = dt_customer.Rows[0]["gender"]?.ToString() ?? "";
                        model.marital_status = dt_customer.Rows[0]["m_status"]?.ToString() ?? "";
                        model.spause_name = dt_customer.Rows[0]["spouse_name"]?.ToString() ?? "";
                        model.father_name = dt_customer.Rows[0]["fathers_name"]?.ToString() ?? "";
                        model.mother_name = dt_customer.Rows[0]["mothers_name"]?.ToString() ?? "";
                        model.guardian = dt_customer.Rows[0]["guardiantype"]?.ToString() ?? "";
                        model.residence_type = dt_customer.Rows[0]["residence_type"]?.ToString() ?? "";
                        Session["residence_type"] = model.residence_type;
                        model.yearofstay = dt_customer.Rows[0]["y_stay"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["y_stay"]) : 0;
                        model.dist_from_branch = dt_customer.Rows[0]["distance_frm_branch"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["distance_frm_branch"]) : 0;
                        Session["distance_from_bramch"] = model.dist_from_branch;
                        model.branch_id = dt_customer.Rows[0]["branch_id"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["branch_id"]) : 0;
                        model.residence_no = dt_customer.Rows[0]["residence_no"]?.ToString() ?? "";
                        model.annual_income = dt_customer.Rows[0]["Annual_income"] != DBNull.Value ? Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]) : 0f;
                        model.education = dt_customer.Rows[0]["education"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["education"]) : 0;
                        model.occupation = dt_customer.Rows[0]["occupation"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["occupation"]) : 0;

                    }


                    if (ds.Tables[2].Rows.Count > 0)    //For Communication Address
                    {
                        DataTable dt_comm = ds.Tables[2];
                        if (dt_comm.Rows.Count > 0)
                        {

                            model.house_name = dt_comm.Rows[0]["c_add"].ToString() == "" ? "" : dt_comm.Rows[0]["c_add"].ToString();
                            model.landmark = dt_comm.Rows[0]["c_land"].ToString() == "" ? "" : dt_comm.Rows[0]["c_land"].ToString();
                            model.City = dt_comm.Rows[0]["c_city"].ToString().ToString() == "" ? "" : dt_comm.Rows[0]["c_city"].ToString();
                            model.Pincode = dt_comm.Rows[0]["c_pincode"].ToString() == "" ? "0" : dt_comm.Rows[0]["c_pincode"].ToString();


                            model.PostOffice = dt_comm.Rows[0]["c_postid"].ToString() == "0" ? "" : dt_comm.Rows[0]["c_postid"].ToString();
                            model.Post_name = dt_comm.Rows[0]["c_postname"].ToString() == "" ? "" : dt_comm.Rows[0]["c_postname"].ToString();

                            model.District = dt_comm.Rows[0]["c_district"].ToString() == "" ? "" : dt_comm.Rows[0]["c_district"].ToString();
                            model.State = dt_comm.Rows[0]["c_state"].ToString() == "" ? "" : dt_comm.Rows[0]["c_state"].ToString();

                        }
                    }
                    if (ds.Tables[3].Rows.Count > 0)    //For Permananent Address
                    {
                        DataTable dt_permenant = ds.Tables[3];
                        if (dt_permenant.Rows.Count > 0)
                        {

                            model.p_house_name = dt_permenant.Rows[0]["p_add"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_add"].ToString();
                            model.p_landmark = dt_permenant.Rows[0]["p_land"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_land"].ToString();
                            model.p_City = dt_permenant.Rows[0]["p_city"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_city"].ToString();

                            model.p_Pincode = dt_permenant.Rows[0]["p_pincode"].ToString() == "" ? "0" : dt_permenant.Rows[0]["p_pincode"].ToString();



                            model.p_PostOffice = dt_permenant.Rows[0]["p_postid"].ToString() == "0" ? "" : dt_permenant.Rows[0]["p_postid"].ToString();
                            model.p_Post_name = dt_permenant.Rows[0]["p_postname"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_postname"].ToString();


                            model.p_District = dt_permenant.Rows[0]["p_district"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_district"].ToString();
                            model.p_State = dt_permenant.Rows[0]["p_state"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_state"].ToString();

                        }
                    }

                    try
                    {
                        if (Session["cust_slno"] != null && Session["cust_slno"].ToString() != "")
                        {
                            List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                            DataTable dt_Grid = new DataTable();
                            SqlParameter[] pr30 = new SqlParameter[3];
                            pr30[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                            pr30[0].Value = 7;
                            pr30[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                            pr30[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                            pr30[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                            pr30[2].Value = Convert.ToUInt64(Session["cust_slno"]);


                            dt_Grid = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr30).Tables[0];


                            //model2.Data = new List<Dictionary<string, object>>();
                            foreach (DataRow row in dt_Grid.Rows)
                            {
                                kyc_upload_M item = new kyc_upload_M
                                {
                                    sno = Convert.ToInt32(row["sno"]),
                                    KYC = row["KYC"].ToString(),
                                    kyc_id = Convert.ToInt32(row["kyc_id"]),
                                    id_no = row["id_no"].ToString(),
                                    //Proof_Type = row["Proof_Type"].ToString(),
                                    cid = Convert.ToInt32(Session["CustomerID_M"]),
                                    FromDate = row["from_dt"].ToString(),
                                    ToDate = row["to_dt"].ToString(),
                                    ImageUrl = GetImageUrl(row["image"] as byte[])
                                    //Data= GetImageUrl(row["image"] as byte[])
                                };

                                model2.Add(item);
                            }
                            model.kycgrids = model2;
                            // return PartialView("grid_view_modifi", model2);
                        }
                        else
                        {
                            if (ds.Tables[4].Rows.Count > 0)
                            {
                                DataTable dt_kyc_dtl = ds.Tables[4];
                                if (dt_kyc_dtl.Rows.Count > 0)
                                {
                                    List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                                    //model2.Data = new List<Dictionary<string, object>>();
                                    foreach (DataRow row in dt_kyc_dtl.Rows)
                                    {
                                        kyc_upload_M item = new kyc_upload_M
                                        {
                                            sno = Convert.ToInt32(row["sno"]),
                                            KYC = row["KYC"].ToString(),
                                            kyc_id = Convert.ToInt32(row["kyc_id"]),
                                            id_no = row["id_no"].ToString(),
                                            Proof_Type = row["Proof_Type"].ToString(),
                                            cid = Convert.ToInt32(Session["CustomerID_M"]),
                                            FromDate = row["from_dt"].ToString(),
                                            ToDate = row["to_dt"].ToString(),
                                            ImageUrl = GetImageUrl(row["image"] as byte[]),
                                            Father = row["father"].ToString(),
                                            Name = row["name"].ToString(),
                                            DOB = row["dob_kyc"].ToString()
                                        };

                                        model2.Add(item);
                                        model.kycgrids = model2;
                                    }
                                }


                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                    }




                }
            }

            else if (sl_no != "")
            {
                model.hidden_kyc_status = 0;
                photoData = GetExistingPhotoData_modification();
                if (photoData != null)
                {
                    string base64Photo = Convert.ToBase64String(photoData);

                    // Pass Base64 string to the view
                    ViewBag.PhotoData = base64Photo;
                }


                SqlParameter[] pr7 = new SqlParameter[3];

                pr7[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr7[0].Value = 14;

                pr7[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                pr7[1].Value = Session["CustomerID_M"].ToString();


                pr7[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                pr7[2].Value = sl_no;


                DataSet ds_modify = new DataSet();

                ds_modify = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr7);

                if (ds_modify.Tables.Count > 0)
                {
                    if (ds_modify.Tables[1].Rows.Count > 0)
                    {

                        DataTable dt_customer = ds_modify.Tables[1];



                        model.customer_id = Session["CustomerID_M"] != null ? Convert.ToInt32(Session["CustomerID_M"]) : 0;
                        model.Customer_name = dt_customer.Rows[0]["customer_name"]?.ToString() ?? "";
                        model.category_id = dt_customer.Rows[0]["customer_category"]?.ToString() ?? "";
                        model.branch_name = dt_customer.Rows[0]["branch_name"]?.ToString() ?? "";

                        //model.create_date = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;

                        DateTime create_datee = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;

                        // Format the date to string without time part
                        string create_dateee = create_datee.ToString("dd-MMM-yyyy");

                        // Assign the formatted string to the model
                        model.create_date = create_dateee;



                        model.status_id = dt_customer.Rows[0]["Status"]?.ToString() ?? "";
                        model.mob_no = dt_customer.Rows[0]["mobile_no"]?.ToString() ?? "";
                        model.land_no = dt_customer.Rows[0]["land_no"]?.ToString() ?? "";
                        model.office_no = dt_customer.Rows[0]["office_no"]?.ToString() ?? "";
                        model.email = dt_customer.Rows[0]["email_id"]?.ToString() ?? "";
                        model.dob = dt_customer.Rows[0]["dob"].ToString() == "" ? "" : dt_customer.Rows[0]["dob"].ToString();
                        model.gender = dt_customer.Rows[0]["gender"]?.ToString() ?? "";
                        model.marital_status = dt_customer.Rows[0]["m_status"]?.ToString() ?? "";
                        model.spause_name = dt_customer.Rows[0]["spouse_name"]?.ToString() ?? "";
                        model.father_name = dt_customer.Rows[0]["fathers_name"]?.ToString() ?? "";
                        model.mother_name = dt_customer.Rows[0]["mothers_name"]?.ToString() ?? "";
                        model.guardian = dt_customer.Rows[0]["guardiantype"]?.ToString() ?? "";
                        model.residence_type = string.IsNullOrEmpty(dt_customer.Rows[0]["residence_type"]?.ToString()) ?
                        (Session["residence_type"]?.ToString() ?? "") : dt_customer.Rows[0]["residence_type"].ToString();
                        Session["residence_type"] = model.residence_type;
                        model.yearofstay = dt_customer.Rows[0]["y_stay"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["y_stay"]) : 0;

                        //  model.dist_from_branch = Convert.ToInt32(dt_customer.Rows[0]["distance_frm_branch"]);
                        model.dist_from_branch = dt_customer.Rows[0]["distance_frm_branch"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["distance_frm_branch"]) : 0;

                        if (model.dist_from_branch == 0 && Session["distance_from_bramch"] != null)
                        {
                            model.dist_from_branch = Convert.ToInt32(Session["distance_from_bramch"]);
                        }
                        Session["distance_from_bramch"] = model.dist_from_branch;

                        model.branch_id = Convert.ToInt32(dt_customer.Rows[0]["branch_id"]);

                        model.residence_no = dt_customer.Rows[0]["residence_no"]?.ToString() ?? "";
                        model.annual_income = dt_customer.Rows[0]["Annual_income"] != DBNull.Value ? Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]) : 0f;
                        model.education = dt_customer.Rows[0]["education"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["education"]) : 0;
                        model.occupation = dt_customer.Rows[0]["occupation"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["occupation"]) : 0;

                        model.first_name = dt_customer.Rows[0]["first_name"]?.ToString() ?? "";
                        model.second_name = dt_customer.Rows[0]["second_name"]?.ToString() ?? "";
                        model.last_name = dt_customer.Rows[0]["last_name"]?.ToString() ?? "";


                    }



                    if (ds_modify.Tables[3].Rows.Count > 0)
                    {
                        List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                        DataTable dtkyc = ds_modify.Tables[3];
                        foreach (DataRow row in dtkyc.Rows)
                        {
                            kyc_upload_M item = new kyc_upload_M
                            {
                                sno = Convert.ToInt32(row["sno"]),
                                KYC = row["KYC"].ToString(),
                                kyc_id = Convert.ToInt32(row["kyc_id"]),
                                id_no = row["ID-No"].ToString(),

                                cid = Convert.ToInt32(Session["CustomerID_M"]),
                                FromDate = row["from_dt"].ToString(),
                                ToDate = row["to_dt"].ToString(),
                                Father = row["father"].ToString(),
                                Name = row["name"].ToString(),
                                DOB = row["dob_kyc"].ToString(),
                                ImageUrl = GetImageUrl(row["image"] as byte[])

                            };

                            model2.Add(item);
                        }
                        model.kycgrids = model2;

                    }


                    if (ds_modify.Tables[4].Rows.Count > 0)    //For Communication Address
                    {
                        DataTable dt_comm = ds_modify.Tables[4];
                        if (dt_comm.Rows.Count > 0)
                        {

                            model.house_name = dt_comm.Rows[0]["c_add"].ToString() == "" ? "" : dt_comm.Rows[0]["c_add"].ToString();
                            model.landmark = dt_comm.Rows[0]["c_land"].ToString() == "" ? "" : dt_comm.Rows[0]["c_land"].ToString();
                            model.City = dt_comm.Rows[0]["c_city"].ToString().ToString() == "" ? "" : dt_comm.Rows[0]["c_city"].ToString();
                            model.Pincode = dt_comm.Rows[0]["c_pincode"].ToString() == "" ? "0" : dt_comm.Rows[0]["c_pincode"].ToString();

                            model.PostOffice = dt_comm.Rows[0]["c_postid"].ToString() == "0" ? "" : dt_comm.Rows[0]["c_postid"].ToString();
                            model.Post_name = dt_comm.Rows[0]["c_postname"].ToString() == "" ? "" : dt_comm.Rows[0]["c_postname"].ToString();

                            model.District = dt_comm.Rows[0]["c_district"].ToString() == "" ? "" : dt_comm.Rows[0]["c_district"].ToString();
                            model.State = dt_comm.Rows[0]["c_state"].ToString() == "" ? "" : dt_comm.Rows[0]["c_state"].ToString();

                        }
                    }
                    if (ds_modify.Tables[5].Rows.Count > 0)    //For Permananent Address
                    {
                        DataTable dt_permenant = ds_modify.Tables[5];
                        if (dt_permenant.Rows.Count > 0)
                        {

                            model.p_house_name = dt_permenant.Rows[0]["p_add"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_add"].ToString();
                            model.p_landmark = dt_permenant.Rows[0]["p_land"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_land"].ToString();
                            model.p_City = dt_permenant.Rows[0]["p_city"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_city"].ToString();

                            model.p_Pincode = dt_permenant.Rows[0]["p_pincode"].ToString() == "" ? "0" : dt_permenant.Rows[0]["p_pincode"].ToString();



                            model.p_PostOffice = dt_permenant.Rows[0]["p_postid"].ToString() == "0" ? "" : dt_permenant.Rows[0]["p_postid"].ToString();
                            model.p_Post_name = dt_permenant.Rows[0]["p_postname"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_postname"].ToString();


                            model.p_District = dt_permenant.Rows[0]["p_district"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_district"].ToString();
                            model.p_State = dt_permenant.Rows[0]["p_state"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_state"].ToString();

                        }
                    }


                }

            }

            else
            {

                if (return_slno != "")

                {


                    SqlParameter[] pr3 = new SqlParameter[3];
                    pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr3[0].Value = 24;

                    pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                    pr3[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                    pr3[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                    pr3[2].Value = Convert.ToInt64(Session["cust_slno"]);

                    DataTable dt = new DataTable();
                    dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr3).Tables[0];



                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["image"] != DBNull.Value)
                    {
                        byte[] imageData = dt.Rows[0]["image"] as byte[];
                        string base64Image = Convert.ToBase64String(imageData);
                        string metadata = "data:image;base64," + base64Image;  // Assuming it's an image
                        model.ImageUrl_pattach = metadata;
                        Session["imageurl_pattach"] = metadata;
                    }


                    model.hidden_kyc_status = 0;
                    TempData["HiddenValue"] = return_slno;

                    photoData = GetExistingPhotoData_modification();
                    if (photoData != null)
                    {
                        string base64Photo = Convert.ToBase64String(photoData);


                        ViewBag.PhotoData = base64Photo;
                    }


                    SqlParameter[] pr7 = new SqlParameter[3];

                    pr7[0] = new SqlParameter("@query_id", SqlDbType.Int);
                    pr7[0].Value = 14;

                    pr7[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
                    pr7[1].Value = Session["CustomerID_M"].ToString();


                    pr7[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                    pr7[2].Value = return_slno;


                    DataSet ds_modify = new DataSet();

                    ds_modify = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr7);

                    if (ds_modify.Tables.Count > 0)
                    {
                        if (ds_modify.Tables[1].Rows.Count > 0)
                        {

                            DataTable dt_customer = ds_modify.Tables[1];



                            model.customer_id = Session["CustomerID_M"] != null ? Convert.ToInt32(Session["CustomerID_M"]) : 0;
                            model.Customer_name = dt_customer.Rows[0]["customer_name"]?.ToString() ?? "";
                            model.category_id = dt_customer.Rows[0]["customer_category"]?.ToString() ?? "";
                            model.branch_name = dt_customer.Rows[0]["branch_name"]?.ToString() ?? "";

                            //model.create_date = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;

                            DateTime create_datee = dt_customer.Rows[0]["create_dt"] != DBNull.Value ? Convert.ToDateTime(dt_customer.Rows[0]["create_dt"]) : DateTime.MinValue;

                            // Format the date to string without time part
                            string create_dateee = create_datee.ToString("dd-MMM-yyyy");

                            // Assign the formatted string to the model
                            model.create_date = create_dateee;




                            model.status_id = dt_customer.Rows[0]["Status"]?.ToString() ?? "";
                            model.mob_no = dt_customer.Rows[0]["mobile_no"]?.ToString() ?? "";
                            model.land_no = dt_customer.Rows[0]["land_no"]?.ToString() ?? "";
                            model.office_no = dt_customer.Rows[0]["office_no"]?.ToString() ?? "";
                            model.email = dt_customer.Rows[0]["email_id"]?.ToString() ?? "";
                            model.dob = dt_customer.Rows[0]["dob"]?.ToString() ?? "";
                            model.gender = dt_customer.Rows[0]["gender"]?.ToString() ?? "";
                            model.marital_status = dt_customer.Rows[0]["m_status"]?.ToString() ?? "";
                            model.spause_name = dt_customer.Rows[0]["spouse_name"]?.ToString() ?? "";
                            model.father_name = dt_customer.Rows[0]["fathers_name"]?.ToString() ?? "";
                            model.mother_name = dt_customer.Rows[0]["mothers_name"]?.ToString() ?? "";
                            model.guardian = dt_customer.Rows[0]["guardiantype"]?.ToString() ?? "";
                            model.residence_type = string.IsNullOrEmpty(dt_customer.Rows[0]["residence_type"]?.ToString()) ?
                                                 (Session["residence_type"]?.ToString() ?? "") : dt_customer.Rows[0]["residence_type"].ToString();
                            Session["residence_type"] = model.residence_type;
                            model.yearofstay = dt_customer.Rows[0]["y_stay"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["y_stay"]) : 0;

                            model.dist_from_branch = dt_customer.Rows[0]["distance_frm_branch"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["distance_frm_branch"]) : 0;
                            if (model.dist_from_branch == 0 && Session["distance_from_bramch"] != null)
                            {
                                model.dist_from_branch = Convert.ToInt32(Session["distance_from_bramch"]);
                            }
                            Session["distance_from_bramch"] = model.dist_from_branch;

                            model.branch_id = dt_customer.Rows[0]["branch_id"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["branch_id"]) : 0;

                            model.residence_no = dt_customer.Rows[0]["residence_no"]?.ToString() ?? "";
                            model.annual_income = dt_customer.Rows[0]["Annual_income"] != DBNull.Value ? Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]) : 0f;
                            model.education = dt_customer.Rows[0]["education"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["education"]) : 0;
                            model.occupation = dt_customer.Rows[0]["occupation"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["occupation"]) : 0;
                            model.first_name = dt_customer.Rows[0]["first_name"]?.ToString() ?? "";
                            model.second_name = dt_customer.Rows[0]["second_name"]?.ToString() ?? "";
                            model.last_name = dt_customer.Rows[0]["last_name"]?.ToString() ?? "";

                        }


                        if (ds_modify.Tables[3].Rows.Count > 0)
                        {
                            List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                            DataTable dtkyc = ds_modify.Tables[3];
                            foreach (DataRow row in dtkyc.Rows)
                            {
                                kyc_upload_M item = new kyc_upload_M
                                {
                                    sno = Convert.ToInt32(row["sno"]),
                                    KYC = row["KYC"].ToString(),
                                    kyc_id = Convert.ToInt32(row["kyc_id"]),
                                    id_no = row["ID-No"].ToString(),
                                    //Proof_Type = row["Proof_Type"].ToString(),
                                    cid = Convert.ToInt32(Session["CustomerID_M"]),
                                    FromDate = row["from_dt"].ToString(),
                                    ToDate = row["to_dt"].ToString(),
                                    Father = row["father"].ToString(),
                                    Name = row["name"].ToString(),
                                    DOB = row["dob_kyc"].ToString(),
                                    ImageUrl = GetImageUrl(row["image"] as byte[])
                                    //Data= GetImageUrl(row["image"] as byte[])
                                };



                                model2.Add(item);
                            }
                            model.kycgrids = model2;

                        }


                        if (ds_modify.Tables[4].Rows.Count > 0)    //For Communication Address
                        {
                            DataTable dt_comm = ds_modify.Tables[4];
                            if (dt_comm.Rows.Count > 0)
                            {

                                model.house_name = dt_comm.Rows[0]["c_add"].ToString() == "" ? "" : dt_comm.Rows[0]["c_add"].ToString();
                                model.landmark = dt_comm.Rows[0]["c_land"].ToString() == "" ? "" : dt_comm.Rows[0]["c_land"].ToString();
                                model.City = dt_comm.Rows[0]["c_city"].ToString().ToString() == "" ? "" : dt_comm.Rows[0]["c_city"].ToString();
                                model.Pincode = dt_comm.Rows[0]["c_pincode"].ToString() == "" ? "0" : dt_comm.Rows[0]["c_pincode"].ToString();


                                model.PostOffice = dt_comm.Rows[0]["c_postid"].ToString() == "0" ? "" : dt_comm.Rows[0]["c_postid"].ToString();
                                model.Post_name = dt_comm.Rows[0]["c_postname"].ToString() == "" ? "" : dt_comm.Rows[0]["c_postname"].ToString();

                                model.District = dt_comm.Rows[0]["c_district"].ToString() == "" ? "" : dt_comm.Rows[0]["c_district"].ToString();
                                model.State = dt_comm.Rows[0]["c_state"].ToString() == "" ? "" : dt_comm.Rows[0]["c_state"].ToString();

                            }
                        }
                        if (ds_modify.Tables[5].Rows.Count > 0)    //For Permananent Address
                        {
                            DataTable dt_permenant = ds_modify.Tables[5];
                            if (dt_permenant.Rows.Count > 0)
                            {

                                model.p_house_name = dt_permenant.Rows[0]["p_add"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_add"].ToString();
                                model.p_landmark = dt_permenant.Rows[0]["p_land"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_land"].ToString();
                                model.p_City = dt_permenant.Rows[0]["p_city"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_city"].ToString();

                                model.p_Pincode = dt_permenant.Rows[0]["p_pincode"].ToString() == "" ? "0" : dt_permenant.Rows[0]["p_pincode"].ToString();



                                model.p_PostOffice = dt_permenant.Rows[0]["p_postid"].ToString() == "0" ? "" : dt_permenant.Rows[0]["p_postid"].ToString();
                                model.p_Post_name = dt_permenant.Rows[0]["p_postname"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_postname"].ToString();


                                model.p_District = dt_permenant.Rows[0]["p_district"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_district"].ToString();
                                model.p_State = dt_permenant.Rows[0]["p_state"].ToString() == "" ? "" : dt_permenant.Rows[0]["p_state"].ToString();

                            }
                        }

                        if (ds_modify.Tables[7].Rows.Count > 0)    //Return reason
                        {
                            DataTable dt_returnreason = ds_modify.Tables[7];
                            if (dt_returnreason.Rows.Count > 0)
                            {

                                model.return_reason = dt_returnreason.Rows[0]["return_reason"]?.ToString() ?? "";
                            }
                        }

                    }



                }


            }


            DataTable categories = categorydropdown();
            ViewData["CategoryList"] = categories;

            DataTable gender = FillGender();
            ViewData["genderList"] = gender;

            DataTable m_status = Fillmarital_status();
            ViewData["m_statusList"] = m_status;

            DataTable kyc_types = Fillkyc_type();
            DataTable dt_kycGrid = new DataTable();
            ViewData["kyc_typeList"] = kyc_types;

            DataTable edu = educationdropdown();
            ViewData["EducationList"] = edu;

            DataTable occu = occupationdropdown();
            ViewData["OccupationList"] = occu;

            SqlParameter[] pr11 = new SqlParameter[2];

            pr11[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr11[0].Value = 30;

            pr11[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr11[1].Value = Session["CustomerID_M"].ToString();

            DataTable dt_rekyc = new DataTable();

            dt_rekyc = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr11).Tables[0];

            model.rekyc_status = Convert.ToInt32(dt_rekyc.Rows[0][0]);

            return View(model);

        }



        private string GetImageUrl(byte[] imageBytes)
        {


            string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

            if (IsPdf(imageBytes))
            {

                string base64Pdf = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

                return $"data:application/pdf;base64,{base64String}";

            }
            else
            {

                string base64Image = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);

                return $"data:image;base64,{base64String}";

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



        public DataTable Fillkyc_type()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 18;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);

            pr[1].Value = Session["CustomerID_M"].ToString();
            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr).Tables[0];
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



        [HttpPost]
        public ActionResult ModifyDeclarationDelete()
        {
            //Session["Hidden_kyc_edit"] = "1";
            //Session["hid_kyc_edit_status"] = "1";
            // Ensure you have proper error handling here
            //DataTable dt = null;
            SqlParameter[] prr = new SqlParameter[3];
            prr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            prr[0].Value = 20;

            if (Session["CustomerID_M"] != null && Session["cust_slno"] != null)
            {
                prr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                prr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                prr[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                prr[2].Value = Convert.ToInt64(Session["cust_slno"]);
                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", prr);

                //dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", prr).Tables[0];
                return Json(new { success = true });
            }
            else
            {
                // Handle the case where session values are missing
                return Json(new { success = false, message = "Session values are missing." });
            }
        }





        public ActionResult ModifyphotoDeclarationDelete()
        {
            //Session["Hidden_kyc_edit"] = "1";
            //Session["hid_kyc_edit_status"] = "1";
            // Ensure you have proper error handling here
            //DataTable dt = null;
            SqlParameter[] prr = new SqlParameter[3];
            prr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            prr[0].Value = 27;

            if (Session["CustomerID_M"] != null && Session["cust_slno"] != null)
            {
                prr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                prr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                prr[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                prr[2].Value = Convert.ToInt64(Session["cust_slno"]);
                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", prr);

                //dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", prr).Tables[0];
                return Json(new { success = true });
            }
            else
            {
                // Handle the case where session values are missing
                return Json(new { success = false, message = "Session values are missing." });
            }
        }


        [HttpPost]
        public JsonResult kyc_modifi_attach_delet(int kyc_id)
        {

            SqlParameter[] pr3 = new SqlParameter[5];

            pr3[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr3[0].Value = 12;

            pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr3[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr3[2] = new SqlParameter("@kyc_id", SqlDbType.Int);
            pr3[2].Value = kyc_id;

            pr3[3] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr3[3].Value = Convert.ToUInt64(Session["cust_slno"]);

            pr3[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr3[4].Direction = ParameterDirection.Output;


            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr3);
            string msg = pr3[4].Value.ToString();
            modify_model kyc_dt = new modify_model();

            DataTable kyc_types = Fillkyc_type();
            ViewData["kyc_typeList"] = kyc_types;

            Session["Hidden_kyc_edit"] = "1";
            Session["hid_kyc_edit_status"] = "1";


            if (msg == "1")

                return Json(new { success = true });

            else

                return Json(new { success = false });
        }



        [HttpPost]
        public ActionResult modify_checkbox()
        {
            if (Session["cust_slno"].ToString() != "")
            {
                string cust_Slno = Session["cust_slno"].ToString();

                SqlParameter[] pr = new SqlParameter[6];
                modify_model model = new modify_model();

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 16;

                pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr[1].Value = Session["CustomerID_M"].ToString();

                pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr[2].Value = Session["branch_id"];

                pr[3] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[3].Value = Session["login_user"];

                pr[4] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                pr[4].Value = Convert.ToInt64(Session["cust_slno"]);

                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[5].Direction = ParameterDirection.Output;

                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);
                string msg = pr[5].Value.ToString();

                if (msg == "Previous modification is pending")
                {

                    return Json(new { success = false, message = msg });
                }

               else if (msg == "You are not allowed to modify the blocked Customer!")
                {
                    return Json(new { success = false, message = msg });
                }

                else
                    return Json(new { success = true, message = msg });
            }
            else
            {
                SqlParameter[] pr = new SqlParameter[5];
                modify_model model = new modify_model();

                pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr[0].Value = 19;

                pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr[1].Value = Session["CustomerID_M"].ToString();

                pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr[2].Value = Session["branch_id"];

                pr[3] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[3].Value = Session["login_user"];

                pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr[4].Direction = ParameterDirection.Output;

                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);
                string msg = pr[4].Value.ToString();


                if (msg == "Previous modification is pending in branch")
                {
                    return Json(new { success = false, message = msg });
                }
                else if (msg == "You are not allowed to modify this Customer!")
                {
                    return Json(new { success = false, message = msg });
                }
                else if (msg == "Previous modification is pending for approval in H.O.")
                {
                    return Json(new { success = false, message = msg });
                }
                else if (msg == "You are not allowed to modify the blocked Customer!")
                {
                    return Json(new { success = false, message = msg });
                }
                else
                {
                    Session["cust_slno"] = msg;
                    return Json(new { success = true, message = "Modification successful." });
                }
            }
        }


        [HttpPost]
        public ActionResult sent_modi_approval(modify_model model)
        {


            SqlParameter[] pr = new SqlParameter[6];


            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 4;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr[1].Value = Session["CustomerID_M"].ToString();

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Session["branch_id"];

            pr[3] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr[3].Value = Convert.ToInt64(Session["cust_slno"]);

            pr[4] = new SqlParameter("@user", SqlDbType.BigInt);
            pr[4].Value = Convert.ToInt64(Session["login_user"]);

            pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr[5].Direction = ParameterDirection.Output;

            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);

            string msg = pr[5].Value.ToString();

            string[] msg_string = msg.Split('#');

            if (msg_string[0] != null && msg_string[0] == "Successfully Sent for Approval")
            {
                TempData["SuccessMessage"] = msg_string[0].ToString();
            }
            else
            {
                TempData["ErrorMessage"] = msg_string[0].ToString();
            }

            return Json(new { success = true, message = msg_string[0].ToString() });
            // return View();

        }




        [HttpGet]
        private byte[] GetExistingPhotoData_modification()
        {
            byte[] photoData = null; // Initialize photoData variable

            SqlParameter[] pr5 = new SqlParameter[3];
            pr5[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr5[0].Value = 15;

            pr5[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr5[1].Value = Session["CustomerID_M"];

            pr5[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr5[2].Value = TempData["HiddenValue"];




            DataTable dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr5).Tables[0];

            // Check if the DataTable contains any rows
            if (dt.Rows.Count > 0)
            {
                // Assuming "photo" is the column name in the result set
                if (dt.Rows[0][0].ToString() != "1")
                {
                    object photoObject = dt.Rows[0][0];

                    if (photoObject is byte[])
                    {
                        photoData = (byte[])photoObject;
                    }
                    else if (photoObject is string)
                    {
                        string photoString = (string)photoObject;
                        photoData = Convert.FromBase64String(photoString);
                    }

                }

                // You can add additional handling for other data types here if needed
            }

            return photoData;
        }


        [HttpPost]
        public ActionResult SaveCapturedImage_modification(string imageData)
        {
            try
            {
                // Session["login_user"] = "10566";
                byte[] imageBytes_photo = Convert.FromBase64String(imageData.Replace("data:image/jpeg;base64,", ""));
                SqlParameter[] pr6 = new SqlParameter[6];


                pr6[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr6[0].Value = 3;

                pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr6[1].Value = Session["CustomerID_M"].ToString();

                pr6[2] = new SqlParameter("@image", SqlDbType.Binary);
                pr6[2].Value = imageBytes_photo;

                pr6[3] = new SqlParameter("@user", SqlDbType.BigInt);
                pr6[3].Value = Session["login_user"].ToString();

                pr6[4] = new SqlParameter("@sl_no", SqlDbType.BigInt);
                pr6[4].Value = Convert.ToUInt64(Session["cust_slno"]);



                pr6[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200);
                pr6[5].Direction = ParameterDirection.Output;

                dbconnect.ExecuteStoredProcedure("cust_modify_info", pr6);


                string msg = pr6[5].Value.ToString();

                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading image: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UploadImage_modification(HttpPostedFileBase file)

        {
            // Session["login_user"] = "10566";
            modify_model model = new modify_model();
            if (file != null && file.ContentLength > 0)
            {
                try
                {


                    byte[] fileBytes;



                    //using (var reader = new BinaryReader(file.InputStream))
                    //{
                    //    fileBytes = reader.ReadBytes(file.ContentLength);
                    //}

                    using (var memoryStream = new MemoryStream())
                    {
                        file.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (file.ContentType == "application/pdf")
                        {
                            fileBytes = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (file.ContentType == "image/png" ||
                              file.ContentType == "image/jpg" ||
                              file.ContentType == "image/jpeg" ||
                              file.ContentType == "image/bmp" ||
                              file.ContentType == "image/gif")
                        {
                            fileBytes = memoryStream.Length > 256 * 1024
                                ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                                : memoryStream.ToArray();
                        }
                        else
                        {
                            // Handle unsupported content types or throw an error
                            throw new NotSupportedException("Unsupported file type.");
                        }
                    }

                    SqlParameter[] pr7 = new SqlParameter[6];

                    pr7[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr7[0].Value = 3;

                    pr7[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                    pr7[1].Value = Session["CustomerID_M"].ToString();

                    pr7[2] = new SqlParameter("@image", SqlDbType.Binary);
                    pr7[2].Value = fileBytes;


                    pr7[3] = new SqlParameter("@user", SqlDbType.BigInt);
                    pr7[3].Value = Session["login_user"].ToString();

                    pr7[4] = new SqlParameter("@sl_no", SqlDbType.BigInt);
                    pr7[4].Value = Convert.ToUInt64(Session["cust_slno"]);

                    pr7[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200);
                    pr7[5].Direction = ParameterDirection.Output;

                    dbconnect.ExecuteStoredProcedure("[cust_modify_info]", pr7);


                    string msg = pr7[5].Value.ToString();


                    return Json(new { success = true, message = msg });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error uploading file: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "No file specified" });
            }
        }

        [HttpPost]
        public ActionResult document_upload_M(FormCollection form, modify_model model)
        {
            try
            {
                

                string kycId = form["kyc_id"];
                string idNo = form["id_no"];


                HttpPostedFileBase image1 = Request.Files["image1"];
                HttpPostedFileBase image = Request.Files["image"];

                byte[] imageBytes = null, imageBytesMasked = null;



                if (model.image != null && model.image.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image.ContentType == "application/pdf")
                        {
                            imageBytes = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (model.image.ContentType == "image/png" ||
                              model.image.ContentType == "image/jpg" ||
                              model.image.ContentType == "image/jpeg" ||
                              model.image.ContentType == "image/bmp" ||
                              model.image.ContentType == "image/gif")
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


                // Compress and process the second image
                if (model.image1 != null && model.image1.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image1.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image1.ContentType == "application/pdf")
                        {
                            imageBytesMasked = memoryStream.Length > 256 * 1024
                                ? doc.CompressFile(pdfData, ".pdf")
                                : memoryStream.ToArray();

                        }
                        else if (model.image.ContentType == "image/png" ||
                             model.image.ContentType == "image/jpg" ||
                             model.image.ContentType == "image/jpeg" ||
                             model.image.ContentType == "image/bmp" ||
                             model.image.ContentType == "image/gif")
                        {
                            imageBytesMasked = memoryStream.Length > 256 * 1024
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

                SqlParameter[] pr2 = new SqlParameter[16]; // Increase the array size to accommodate the new parameters

                pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr2[0].Value = 5;

                pr2[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr2[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                pr2[2] = new SqlParameter("@image", SqlDbType.Binary);
                pr2[2].Value = imageBytes;

                pr2[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr2[3].Value = 1;

                pr2[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr2[4].Value = Convert.ToInt64(Session["login_user"]);

                pr2[5] = new SqlParameter("@id_no", SqlDbType.VarChar);
                pr2[5].Value = idNo;

                pr2[6] = new SqlParameter("@kyc_id", SqlDbType.Int);
                pr2[6].Value = kycId;

                pr2[7] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr2[7].Value = Session["branch_id"];

                pr2[8] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr2[8].Direction = ParameterDirection.Output;

                pr2[9] = new SqlParameter("@image1", SqlDbType.Binary);
                pr2[9].Value = imageBytesMasked;

                pr2[10] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                pr2[10].Value = Convert.ToInt64(Session["cust_slno"]);

                pr2[11] = new SqlParameter("@from_dt", SqlDbType.Date);
                pr2[11].Value = model.from_date;

                pr2[12] = new SqlParameter("@To_dt", SqlDbType.Date);
                pr2[12].Value = model.to_date;

                pr2[13] = new SqlParameter("@pan_dob", SqlDbType.Date);
                pr2[13].Value = model.pan_dob;

                pr2[14] = new SqlParameter("@pan_name", SqlDbType.VarChar);
                pr2[14].Value = model.pan_name;

                pr2[15] = new SqlParameter("@pan_father", SqlDbType.VarChar);
                pr2[15].Value = model.pan_father_name;

                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr2);

                string msg = pr2[8].Value.ToString();
                string[] msg_string = msg.Split('@');

                string msg1 = msg_string[0].ToString();

                string msg2 = msg_string[1].ToString();


                //List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                if (msg2 == "0")
                {
                    return Json(new { success = false, message = msg1 });
                }


                else if (msg2 == "1")
                {
                    List<kyc_upload_M> model2 = new List<kyc_upload_M>();
                    DataTable dt_kycGrid = new DataTable();
                    SqlParameter[] pr3 = new SqlParameter[3];
                    pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr3[0].Value = 7;
                    pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                    pr3[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

                    pr3[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                    pr3[2].Value = Convert.ToUInt64(Session["cust_slno"]);


                    dt_kycGrid = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr3).Tables[0];


                    //model2.Data = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dt_kycGrid.Rows)
                    {
                        kyc_upload_M item = new kyc_upload_M
                        {
                            sno = Convert.ToInt32(row["sno"]),
                            KYC = row["KYC"].ToString(),
                            kyc_id = Convert.ToInt32(row["kyc_id"]),
                            id_no = row["id_no"].ToString(),
                            //Proof_Type = row["Proof_Type"].ToString(),
                            cid = Convert.ToInt32(Session["CustomerID_M"]),
                            FromDate = row["from_dt"].ToString(),
                            ToDate = row["to_dt"].ToString(),
                            Father = row["father"].ToString(),
                            Name = row["name"].ToString(),
                            DOB = row["dob_kyc"].ToString(),
                            ImageUrl = GetImageUrl(row["image"] as byte[])
                            //Data= GetImageUrl(row["image"] as byte[])
                        };

                        model2.Add(item);
                    }
                    DataTable categories = categorydropdown();
                    ViewData["CategoryList"] = categories;


                    DataTable gender = FillGender();
                    ViewData["genderList"] = gender;

                    DataTable m_status = Fillmarital_status();
                    ViewData["m_statusList"] = m_status;


                    DataTable kyc_types1 = Fillkyc_type();

                    ViewData["kyc_typeList"] = kyc_types1;

                    DataTable edu = educationdropdown();
                    ViewData["EducationList"] = edu;

                    //Function call for fill occupation
                    DataTable occu = occupationdropdown();
                    ViewData["OccupationList"] = occu;

                    Session["Hidden_kyc_edit"] = "1";

                    model.kycgrids = model2;
                    //model.kycgrids = model2;
                }
                else
                {
                    model.kycgrids = new List<kyc_upload_M>();
                }

                DataTable kyc_types = Fillkyc_type();
                ViewData["kyc_typeList"] = kyc_types;

                Session["hid_kyc_edit_status"] = "1";

                // return RedirectToAction("customer_modification", model);
                return Json(new { success = true, message = "KYC Saved", redirectTo = Url.Action("customer_modification", model) });


            }




            catch (Exception ex)
            {
                // Log the inner exception for debugging
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    // Log inner exception details here
                    innerException = innerException.InnerException;
                }

                return Json(new { success = false, message = "error uploading file: " + ex.Message });
            }
        }


        [HttpPost]
        public ActionResult kyc_edit_pencil_click()
        {
            //Session["branch_id"] = "1";

            //SqlParameter[] pr = new SqlParameter[3];
            //modify_model model = new modify_model();

            //pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            //pr[0].Value = 6;

            //pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            //pr[1].Value = Session["CustomerID_M"].ToString();

            //pr[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            //pr[2].Value = Convert.ToInt64(Session["cust_slno"]);





            //dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);

            //Session["Hidden_kyc_edit"] = "1";
            //Session["hid_kyc_edit_status"] = "1";

            return View();

        }

        [HttpPost]
        public ActionResult declaration_upload_M(modify_model model)
        {
            try
            {

                byte[] imageBytes_dec = null;

                // Check and process the first image (model.image)
                if (model.image3 != null && model.image3.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image3.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image3.ContentType == "application/pdf")
                        {
                            imageBytes_dec = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (model.image3.ContentType == "image/png" ||
                              model.image3.ContentType == "image/jpg" ||
                              model.image3.ContentType == "image/jpeg" ||
                              model.image3.ContentType == "image/bmp" ||
                              model.image3.ContentType == "image/gif")
                        {
                            imageBytes_dec = memoryStream.Length > 256 * 1024
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

                //        // Check and process the second image (model.image1)


                SqlParameter[] pr3 = new SqlParameter[8]; // Increase the array size to accommodate the new parameters

                pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr3[0].Value = 9;

                pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr3[1].Value = Convert.ToInt64(Session["CustomerID_M"]);


                pr3[2] = new SqlParameter("@image3", SqlDbType.Binary);
                pr3[2].Value = imageBytes_dec;

                pr3[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr3[3].Value = 1;

                pr3[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr3[4].Value = Convert.ToInt64(Session["login_user"]);


                pr3[5] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr3[5].Value = Session["branch_id"];


                pr3[6] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                pr3[6].Value = Convert.ToInt64(Session["cust_slno"]);

                pr3[7] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr3[7].Direction = ParameterDirection.Output;

                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr3);

                string msg_dis = pr3[7].Value.ToString();

                //ViewBag.HiddenValue = Session["cust_slno"];


                TempData["HiddenValue"] = Session["cust_slno"];

                // or "Rent", depending on your logic


                return RedirectToAction("customer_modification", model);



            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error uploading file: " + ex.Message });
            }

        }

        [HttpPost]
        public ActionResult form60_upload_M(modify_model model)
        {
            try
            {

                byte[] imageBytes_dec = null;

                // Check and process the first image (model.image)
                if (model.image5 != null && model.image5.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image5.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image5.ContentType == "application/pdf")
                        {
                            imageBytes_dec = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (model.image5.ContentType == "image/png" ||
                              model.image5.ContentType == "image/jpg" ||
                              model.image5.ContentType == "image/jpeg" ||
                              model.image5.ContentType == "image/bmp" ||
                              model.image5.ContentType == "image/gif")
                        {
                            imageBytes_dec = memoryStream.Length > 256 * 1024
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

                //        // Check and process the second image (model.image1)


                SqlParameter[] pr3 = new SqlParameter[8]; // Increase the array size to accommodate the new parameters

                pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr3[0].Value = 26;

                pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr3[1].Value = Convert.ToInt64(Session["CustomerID_M"]);


                pr3[2] = new SqlParameter("@image3", SqlDbType.Binary);
                pr3[2].Value = imageBytes_dec;

                pr3[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr3[3].Value = 1;

                pr3[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr3[4].Value = Convert.ToInt64(Session["login_user"]);


                pr3[5] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr3[5].Value = Session["branch_id"];


                pr3[6] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
                pr3[6].Value = Convert.ToInt64(Session["cust_slno"]);

                pr3[7] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr3[7].Direction = ParameterDirection.Output;

                dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr3);

                string msg_dis = pr3[7].Value.ToString();

                //ViewBag.HiddenValue = Session["cust_slno"];


                TempData["HiddenValue"] = Session["cust_slno"];

                // or "Rent", depending on your logic


                return RedirectToAction("customer_modification", model);



            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error uploading file: " + ex.Message });
            }

        }

        [HttpGet]
        public void SetSession(string sessionval)
        {
            Session["otpVerified"] = sessionval;
        }



        [HttpPost]
        public ActionResult declaration_modification_view(modify_model model)
        {
            // Initial setup and session checks
            Session["datachange"] = 0;
            string originalMobileNo = Session["OriginalMobileNo"] != null ? Session["OriginalMobileNo"].ToString() : null;
            string changedMobileNo = model.mob_no;
            int mobileNoStatus;

            if (changedMobileNo != originalMobileNo && Session["otpVerified"]?.ToString() == "1")
            {
                mobileNoStatus = 1;
            }
            else
            {
                mobileNoStatus = 0;
                if (Session["otpVerified"]?.ToString() == "0")
                {
                    return Json(new { success = false, message = "Please Verify Entered Mobile Number." });
                }
            }

            string Customer_fullname = Session["Customer_fullname"] != null ? Session["Customer_fullname"].ToString() : null;
            string newCustomerName = model.first_name + " " + model.second_name + " " + model.last_name;
            if (string.IsNullOrEmpty(newCustomerName.Trim()))
            {
                newCustomerName = Customer_fullname;
            }

            int NameStatus = (Customer_fullname == newCustomerName) ? 0 : 1;

            byte[] imageBytes = null;

            // Convert uploaded file to byte array
            if (model.image4 != null && model.image4.ContentLength > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    model.image4.InputStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    byte[] pdfData = memoryStream.ToArray();

                    if (model.image4.ContentType == "application/pdf")
                    {
                        imageBytes = memoryStream.Length > 256 * 1024
                           ? doc.CompressFile(pdfData, ".pdf")
                           : memoryStream.ToArray();
                    }
                    else if (model.image4.ContentType == "image/png" ||
                          model.image4.ContentType == "image/jpg" ||
                          model.image4.ContentType == "image/jpeg" ||
                          model.image4.ContentType == "image/bmp" ||
                          model.image4.ContentType == "image/gif")
                    {
                        imageBytes = memoryStream.Length > 256 * 1024
                            ? ImageCompress.CompressImageToSize(memoryStream, ImageFormat.Jpeg)
                            : memoryStream.ToArray();
                    }
                    else
                    {
                        // Handle unsupported content types or throw an error
                        //throw new NotSupportedException("Unsupported file type.");
                        return Json(new { success = false, message = "Unsupported permenant address proof file type!" });
                    }
                }
            }

            // If imageBytes is null, use model.ImageUrl_pattach
            if (imageBytes == null && Session["imageurl_pattach"] != null)
            {
                string base64String = Session["imageurl_pattach"].ToString();
                if (!string.IsNullOrEmpty(base64String))
                {
                    if (base64String.StartsWith("data:image;base64,"))
                    {
                        base64String = base64String.Replace("data:image;base64,", string.Empty);
                        imageBytes = Convert.FromBase64String(base64String);
                    }
                    else if (base64String.StartsWith("data:application/pdf;base64,"))
                    {
                        base64String = base64String.Replace("data:application/pdf;base64,", string.Empty);
                        imageBytes = Convert.FromBase64String(base64String);
                    }
                }
            }


            // Prepare SQL parameters
            SqlParameter[] pr = new SqlParameter[30];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 10;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr[2].Value = Convert.ToInt64(Session["branch_id"]);

            pr[3] = new SqlParameter("@dob1", SqlDbType.Date);
            pr[3].Value = model.dob;

            pr[4] = new SqlParameter("@fath_name", SqlDbType.VarChar);
            pr[4].Value = model.father_name;

            pr[5] = new SqlParameter("@mother_name", SqlDbType.VarChar);
            pr[5].Value = model.mother_name;

            pr[6] = new SqlParameter("@gender", SqlDbType.VarChar);
            pr[6].Value = model.gender;

            pr[7] = new SqlParameter("@res_type", SqlDbType.VarChar);
            pr[7].Value = model.residence_type;

            pr[8] = new SqlParameter("@mstatus", SqlDbType.VarChar);
            pr[8].Value = model.marital_status;

            pr[9] = new SqlParameter("@spouse_name", SqlDbType.VarChar);
            pr[9].Value = model.spause_name;

            pr[10] = new SqlParameter("@email", SqlDbType.VarChar);
            pr[10].Value = model.email;

            pr[11] = new SqlParameter("@annual_income", SqlDbType.VarChar);
            pr[11].Value = model.annual_income;

            pr[12] = new SqlParameter("@distance", SqlDbType.VarChar);
            pr[12].Value = model.dist_from_branch;

            pr[13] = new SqlParameter("@residence", SqlDbType.VarChar);
            pr[13].Value = model.residence_no;

            pr[14] = new SqlParameter("@office", SqlDbType.VarChar);
            pr[14].Value = model.office_no;

            pr[15] = new SqlParameter("@education", SqlDbType.VarChar);
            pr[15].Value = model.education;

            pr[16] = new SqlParameter("@occupation", SqlDbType.VarChar);
            pr[16].Value = model.occupation;

            pr[17] = new SqlParameter("@commuaddress", SqlDbType.VarChar);
            pr[17].Value = $"{model.house_name}#{model.landmark}#{model.City}#{model.PostOffice}";

            pr[18] = new SqlParameter("@ystay", SqlDbType.VarChar);
            pr[18].Value = model.yearofstay;

            pr[19] = new SqlParameter("@permaddress", SqlDbType.VarChar);
            pr[19].Value = string.IsNullOrEmpty(model.p_house_name) ? "0" : $"{model.p_house_name}#{model.p_landmark}#{model.p_City}#{model.p_PostOffice}";

            pr[20] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr[20].Value = Convert.ToInt64(Session["cust_slno"]);

            pr[21] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr[21].Direction = ParameterDirection.Output;

            pr[22] = new SqlParameter("@first_name", SqlDbType.VarChar, 50);
            pr[22].Value = model.first_name;

            pr[23] = new SqlParameter("@second_name", SqlDbType.VarChar, 50);
            pr[23].Value = model.second_name;

            pr[24] = new SqlParameter("@last_name", SqlDbType.VarChar, 50);
            pr[24].Value = model.last_name;

            pr[25] = new SqlParameter("@Mob_change", SqlDbType.Int);
            pr[25].Value = mobileNoStatus;

            if (model.mob_no == "0")
            {
                return Json(new { success = false, message = "Please Enter Mobile No and Verify otp!" });
            }
            else
            {
                pr[26] = new SqlParameter("@Mobile", SqlDbType.VarChar, 20);
                pr[26].Value = model.mob_no;
            }


            pr[27] = new SqlParameter("@add_doc_img", SqlDbType.Binary);
            pr[27].Value = (object)imageBytes ?? DBNull.Value;

            pr[28] = new SqlParameter("@nam_change", SqlDbType.Int);
            pr[28].Value = NameStatus;

            pr[29] = new SqlParameter("@user", SqlDbType.Int);
            pr[29].Value = Session["login_user"];

            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);

            string msg = pr[21].Value.ToString();




            if (msg == "1")
            {
                Session["datachange"] = 1;
                return Json(new { success = true, datachange = Session["datachange"] });
            }
            else
            {
                return Json(new { success = false, message = msg });
            }
        }




        public ActionResult declaration_modification_view_new(modify_model model)
        {
            //declaration_view model1 = new F();
            SqlParameter[] pr2 = new SqlParameter[5];

            pr2[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr2[0].Value = 8;

            pr2[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr2[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr2[2] = new SqlParameter("@dt_change_status", SqlDbType.Int);
            pr2[2].Value = Session["datachange"];

            pr2[3] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr2[3].Value = Convert.ToInt64(Session["cust_slno"]);

            pr2[4] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            pr2[4].Value = Session["branch_id"];

            DataSet ds = new DataSet();

            ds = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr2);
            //  declaration_view model1 = new declaration_view();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt_customer = ds.Tables[0];
                    model.branch_name = dt_customer.Rows[0]["branch_name"].ToString();
                    model.customer_id = Convert.ToInt32(Session["CustomerID_M"]);


                    model.mob_no = dt_customer.Rows[0]["mobile"].ToString();

                    //model.dob = Convert.ToDateTime(dt_customer.Rows[0]["dob"]).ToString("dd-MMM-yyyy");
                    model.dob = dt_customer.Rows[0]["dob"].ToString();
                    //model.cust_create_date = dt_personal.Rows[0]["create_dt"].ToString();
                    //model.dob = Convert.ToDateTime(dt_customer.Rows[0]["dob"]);

                    model.todaydate = dt_customer.Rows[0]["todaysdt"].ToString();
                    model.father_name = dt_customer.Rows[0]["fathers_name"].ToString();
                    model.residence_type = dt_customer.Rows[0]["residence_type"].ToString();
                    model.spause_name = dt_customer.Rows[0]["spouse_name"].ToString();
                    model.mother_name = dt_customer.Rows[0]["mothers_name"].ToString();
                    model.gender = dt_customer.Rows[0]["gender"].ToString();
                    //model.annual_income = Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]);
                    //model.annual_income = Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]).ToString("F");
                    model.annual_income = Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]);


                    //model.annual_income = String.Format("{0:F}", Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]));


                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    model.education = dt_customer.Rows[0]["education"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["education"]) : 0;
                    //model.occupation = dt_customer.Rows[0]["occupation"] != DBNull.Value ? Convert.ToInt32(dt_customer.Rows[0]["occupation"]) : 0;
                    model.occupation_d = dt_customer.Rows[0]["occupation"]?.ToString() ?? "";
                    model.first_name = dt_customer.Rows[0]["first_name1"]?.ToString() ?? "";
                    model.second_name = dt_customer.Rows[0]["second_name"]?.ToString() ?? "";
                    model.Customer_name = dt_customer.Rows[0]["customer_name"]?.ToString() ?? "";

                    //if (dt_customer.Rows[0]["Annual_income"] != DBNull.Value)
                    //{
                    //    float annualIncome;
                    //    if (float.TryParse(dt_customer.Rows[0]["Annual_income"].ToString(), out annualIncome))
                    //    {
                    //        model.annual_income = annualIncome;
                    //    }

                    //}
                    //else
                    //{
                    //    model.annual_income = 0;
                    //}

                    //////////////////////////////////////////////////////////////////////////////////////////////////////

                    //model.education = Convert.ToInt32(dt_customer.Rows[0]["education"]);
                    //model.occupation = Convert.ToInt32(dt_customer.Rows[0]["occupation"]);
                    //model.first_name = dt_customer.Rows[0]["first_name"].ToString();
                    //model.second_name = dt_customer.Rows[0]["second_name"].ToString();

                    ///////////////////////////////////////////////////////////////////////////////

                    model.house_name = dt_customer.Rows[0]["commu_address"].ToString();
                    model.Pincode = dt_customer.Rows[0]["commu_pin"].ToString();

                    model.State = dt_customer.Rows[0]["commu_state"].ToString();
                    model.p_house_name = dt_customer.Rows[0]["per_address"].ToString();

                    model.p_Pincode = dt_customer.Rows[0]["perm_pin"].ToString();
                    model.p_State = dt_customer.Rows[0]["per_state"].ToString();

                    if (dt_customer.Rows[0]["Annual_income"] != DBNull.Value)
                    {
                        float annualIncome;
                        if (float.TryParse(dt_customer.Rows[0]["Annual_income"].ToString(), out annualIncome))
                        {
                            model.annual_income = annualIncome;
                        }

                    }
                    else
                    {
                        model.annual_income = 0;
                    }


                    //model.annual_income = dt_customer.Rows[0]["Annual_income"] != DBNull.Value ? Convert.ToSingle(dt_customer.Rows[0]["Annual_income"]) : 0;


                }

                if (ds.Tables[1].Rows.Count > 0)     // For photo
                {

                    DataTable dtphoto = ds.Tables[1];

                    if (dtphoto.Rows.Count > 0)
                    {
                        // Assuming "photo" is the column name in the result set
                        object photoObject = dtphoto.Rows[0][0];

                        if (photoObject is byte[])
                        {
                            byte[] byte_photo = (byte[])photoObject;
                            string base64String = Convert.ToBase64String(byte_photo, 0, byte_photo.Length);
                            model.photo = "data:image/png;base64," + base64String;
                        }
                        else if (photoObject is string)
                        {
                            // Handle the case where the photo is stored as a string
                            string photoString = (string)photoObject;
                            // Convert the string to a byte array (assuming it's a Base64-encoded string)
                            byte[] byte_photo = Convert.FromBase64String(photoString);
                            // Convert the byte array back to a Base64 string
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



                // return View();

            }
            //  Session["residence_type"] = model.residence_type;
            return View("declaration_modification_view_new", model);
            // return View("~/Views/Customer_Declaration/Declaration.cshtml", model1);
            // return View(Url.Action("Declaration", "Customer_Declaration"), model1);


        }

        [HttpPost]
        public ActionResult PincodeChanged(string pincode)
        {

            DataTable categories = categorydropdown();
            ViewData["CategoryList"] = categories;

            //Function call for fill guardian type
            //DataTable guardians = FillGuardianType();
            //ViewData["guardianList"] = guardians;

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
        [HttpPost]
        public ActionResult permanent_PincodeChanged(string pincode)
        {




            SqlParameter[] pr = new SqlParameter[2];
            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 1;
            pr[1] = new SqlParameter("@pincode", SqlDbType.BigInt);
            pr[1].Value = pincode.Trim();

            DataSet ds = dbconnect.ExecuteDataset("[dbo].[cust_pincode]", pr);




            if (ds != null && ds.Tables.Count > 1)
            {
                DataTable dt1 = ds.Tables[0];
                DataTable dt2 = ds.Tables[1];



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
        [HttpPost]
        public ActionResult SavePostIDpermanent(int selectedPostID)
        {
            try
            {
                Customer_Registration_model model = new Customer_Registration_model();

                TempData["per_postid"] = selectedPostID;
                model.PostOffice = selectedPostID.ToString();

                return Json("Success");
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult form60_modify_view_m()
        {

            SqlParameter[] pr = new SqlParameter[3];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 17;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr[2].Value = Convert.ToInt64(Session["cust_slno"]);





            DataTable dt = new DataTable();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr).Tables[0];
            if (dt.Rows.Count > 0)
            {


                string dob = dt.Rows[0]["dob"].ToString();
                string[] dobParts = dob.Split('-');


                form_60_model model = new form_60_model
                {

                    first_name = dt.Rows[0]["first_name"].ToString() == "" ? "" : dt.Rows[0]["first_name"].ToString(),
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
                    mobile_no = dt.Rows[0]["mobile"].ToString(),
                    aadhar = dt.Rows[0]["aadhar"].ToString(),
                    todaydate = dt.Rows[0]["todaysdt"].ToString()

                };
                return View("form60_modify_view_m", model);

            }
            return View();
        }


        [HttpPost]
        public ActionResult print_declaration_check_modi()
        {
            SqlParameter[] pr6 = new SqlParameter[4];

            pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr6[0].Value = 21;

            pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr6[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr6[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr6[2].Value = Convert.ToInt64(Session["cust_slno"]);

            pr6[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr6[3].Direction = ParameterDirection.Output;

            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr6);

            string msg1 = pr6[3].Value.ToString();

            //if (msg1 == "Send for Approval")
            //{
            //    // Call your additional function here

            //    return RedirectToAction("declaration_modification_view");

            //   // return RedirectToAction("declaration_modification_view", "Customer_modification");
            //}

            return Json(new { success = true, message = msg1 });
        }



        [HttpPost]
        public ActionResult form_60_check_modi()
        {
            SqlParameter[] pr6 = new SqlParameter[4];

            pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr6[0].Value = 22;

            pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr6[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr6[2] = new SqlParameter("@cust_sl_no", SqlDbType.BigInt);
            pr6[2].Value = Convert.ToInt64(Session["cust_slno"]);

            pr6[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr6[3].Direction = ParameterDirection.Output;

            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr6);

            string msg1 = pr6[3].Value.ToString();

            return Json(new { success = true, message = msg1 });

        }
        [HttpPost]
        public ActionResult modi_cancelled_btnclick()
        {
            SqlParameter[] pr6 = new SqlParameter[4];

            pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr6[0].Value = 28;

            pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr6[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            pr6[2] = new SqlParameter("@sl_no", SqlDbType.BigInt);
            pr6[2].Value = Convert.ToInt64(Session["cust_slno"]);

            pr6[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr6[3].Direction = ParameterDirection.Output;

            dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr6);

            string msg1 = pr6[3].Value.ToString();

            return Json(new { success = true, message = msg1 });

        }
     

        public ActionResult DownloadAllDocuments() // For viewing all documents
        {
            Customer_Approval_Model model = new Customer_Approval_Model();

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 29;

            pr[1] = new SqlParameter("@customer_id", SqlDbType.BigInt);
            pr[1].Value = Convert.ToInt64(Session["CustomerID_M"]);

            DataSet ds = new DataSet();
            ds = dbconnect.ExecuteDataset("[dbo].[cust_modify_info]", pr);

           
                DataTable dtDoc = ds.Tables[0];
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

                return View("DownloadAllDocuments", model);
            }
          
        }

    
}










