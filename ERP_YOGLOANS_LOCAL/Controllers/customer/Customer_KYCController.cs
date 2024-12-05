
using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Description;
using System.Reflection;
using WebGrease.Activities;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web.UI;
using System.Web.Helpers;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.qrcode;
using Org.BouncyCastle.Asn1.Ocsp;
using iTextSharp.text;



namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_KYCController : BaseController
    {

        private object dbcontext;
        private object command;
        DB dbconnect = new DB();
        FileCompressor doc = new FileCompressor();

        doc_compression ImageCompress = new doc_compression();
        public ActionResult kyc_approval()  
        {
            //Session["login"] = "10566";
            ViewBag.AlertMessage = TempData["AlertMessage"] as string;
            tbl_customer_photo model = new tbl_customer_photo();

            byte[] photoData = GetExistingPhotoData();
            if(photoData != null)
            {
                string base64Photo = Convert.ToBase64String(photoData);

                // Pass Base64 string to the view
                ViewBag.PhotoData = base64Photo;
            }
            // Convert photo data to Base64 string
            

            DataTable kyc_types = Fillkyc_type();
            DataTable dt_kycGrid = new DataTable();

            ViewData["kyc_typeList"] = kyc_types;

            tbl_customer_photo mod = new tbl_customer_photo();

            mod.CustomerName = Session["CustomerName"].ToString();
           
            SqlParameter[] pr1 = new SqlParameter[2];

            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 4;

            pr1[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr1[1].Value = Session["TempCID"];
            dbconnect.Open();
            dt_kycGrid = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr1).Tables[0];
            dbconnect.Close();
            List<kyc_upload> model2 = new List<kyc_upload>();

            foreach (DataRow row in dt_kycGrid.Rows)
            {
                kyc_upload item = new kyc_upload
                {
                    sno = Convert.ToInt32(row["sno"]),
                    KYC = row["KYC"].ToString(),
                    kyc_id = Convert.ToInt32(row["kyc_id"]),
                    id_no = row["id_no"].ToString(),
                    Proof_Type = row["Proof_Type"].ToString(),
                    //cid = Convert.ToInt32(Session["TempCID"]),
                    FromDate = row["from_dt"].ToString(),
                    ToDate = row["to_dt"].ToString(),
                    ImageUrl = GetImageUrl(row["image"] as byte[]),
                    //pan_customer_name = row["father"].ToString(),
                    //pan_dob = row["name"].ToString(),
                    //pan_father_name = row["dob_kyc"].ToString()
                    pan_customer_name = row["name"].ToString(),
                    pan_father_name = row["father"].ToString(),
                    pan_dob = row["dob_kyc"].ToString()
                };

                model2.Add(item);
                model.kycgrid = model2;

                SqlParameter[] pr6 = new SqlParameter[2];

                pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
                pr6[0].Value = 12;

                pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr6[1].Value = Convert.ToInt64(Session["TempCID"]);

                DataTable dt_validation = new DataTable();
                dbconnect.Open();
                dt_validation = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr6).Tables[0];
                dbconnect.Close();
                string[] msgArray = dt_validation.Rows[0][0].ToString().Split(' ');
                string date = msgArray[0].ToString();
                string time = msgArray[1].ToString();
                model.dob_validation = date;

            }
            return View(model);

        }
        

        [HttpGet]
        private byte[] GetExistingPhotoData()
        {
            byte[] photoData = null; // Initialize photoData variable

            SqlParameter[] pr5 = new SqlParameter[2];
            pr5[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr5[0].Value = 2;

            pr5[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr5[1].Value = Session["TempCID"];
            dbconnect.Open();
            DataTable dt = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr5).Tables[0];
            dbconnect.Close();
            // Check if the DataTable contains any rows
            if (dt.Rows.Count > 0)
            {
                // Assuming "photo" is the column name in the result set
                if(dt.Rows[0][0].ToString() != "1")
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
        public ActionResult UploadImage_add(HttpPostedFileBase file)
        {
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
                    



                    SqlParameter[] prr = new SqlParameter[5];

                    prr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    prr[0].Value = 1;

                    prr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                    prr[1].Value = Convert.ToInt64 ( Session["TempCID"].ToString());

                    prr[2] = new SqlParameter("@image", SqlDbType.Binary);
                    prr[2].Value = fileBytes;

                    prr[3] = new SqlParameter("@user", SqlDbType.BigInt);
                    prr[3].Value = Convert.ToInt64(Session["login_user"].ToString());

                    prr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200);
                    prr[4].Direction = ParameterDirection.Output;

                    dbconnect.Open();
                    dbconnect.ExecuteStoredProcedure("cust_photo_kyc", prr);
                    dbconnect.Close();

                    string msg = prr[4].Value.ToString();

                    
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
        public ActionResult SaveCapturedImage(string imageData)
        {
            try
            {

                byte[] imageBytes_photo = Convert.FromBase64String(imageData.Replace("data:image/jpeg;base64,", ""));
                SqlParameter[] pr = new SqlParameter[5];


                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr[0].Value = 1;

                pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr[1].Value = Session["TempCID"].ToString();

                pr[2] = new SqlParameter("@image", SqlDbType.Binary);
                pr[2].Value = imageBytes_photo;

                pr[3] = new SqlParameter("@user", SqlDbType.BigInt);
                pr[3].Value = Session["login_user"].ToString();

                pr[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 200);
                pr[4].Direction = ParameterDirection.Output;

                dbconnect.Open();
                dbconnect.ExecuteStoredProcedure("cust_photo_kyc", pr);
                dbconnect.Close();

                string msg = pr[4].Value.ToString();

                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading image: " + ex.Message });
            }
        }

        public DataTable Fillkyc_type()
        {

            SqlParameter[] pr = new SqlParameter[2];

            pr[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr[0].Value = 8;

            pr[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr[1].Value = Session["TempCID"].ToString();

            DataTable dt = new DataTable();

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr).Tables[0];
            dbconnect.Close();
            return dt;

        }




        //[HttpPost]
        //public ActionResult document_upload(tbl_customer_photo model)
        //{
        //    try
        //    {


        //        byte[] imageBytes = null, imageBytesMasked = null;

        //        // Check and process the first image (model.image)
        //        if (model.image != null && model.image.ContentLength > 0)
        //        {
        //            if (model.image.ContentType == "application/pdf")
        //            {
        //                //using (PdfReader pdfReader = new PdfReader(model.image.InputStream))
        //                //{
        //                using (PdfReader pdfReader = new PdfReader(model.image.InputStream))
        //                {
        //                    using (MemoryStream stream = new MemoryStream())
        //                    {
        //                        using (PdfStamper pdfStamper = new PdfStamper(pdfReader, stream))
        //                        {
        //                            // Optionally, you can add compression or other modifications here
        //                        }

        //                        imageBytes = stream.ToArray();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                using (var reader = new BinaryReader(model.image.InputStream))
        //                {
        //                    imageBytes = reader.ReadBytes(model.image.ContentLength);
        //                }
        //            }
        //        }

        //        // Check and process the second image (model.image1)
        //        if (model.image1 != null && model.image1.ContentLength > 0)
        //        {
        //            if (model.image1.ContentType == "application/pdf")
        //            {
        //                using (PdfReader pdfReader = new PdfReader(model.image1.InputStream))
        //                {
        //                    using (MemoryStream stream = new MemoryStream())
        //                    {
        //                        using (PdfStamper pdfStamper = new PdfStamper(pdfReader, stream))
        //                        {
        //                            // Optionally, you can add compression or other modifications here
        //                        }

        //                        imageBytesMasked = stream.ToArray();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                using (var reader = new BinaryReader(model.image1.InputStream))
        //                {
        //                    imageBytesMasked = reader.ReadBytes(model.image1.ContentLength);
        //                }
        //            }
        //        }




        //        SqlParameter[] pr2 = new SqlParameter[15]; // Increase the array size to accommodate the new parameters

        //        pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
        //        pr2[0].Value = 3;

        //        pr2[1] = new SqlParameter("@cid", SqlDbType.BigInt);
        //        pr2[1].Value = Convert.ToInt64(Session["TempCID"]);

        //        pr2[2] = new SqlParameter("@image", SqlDbType.Binary);
        //        pr2[2].Value = imageBytes;

        //        pr2[3] = new SqlParameter("@status_id", SqlDbType.Int);
        //        pr2[3].Value = 1;

        //        pr2[4] = new SqlParameter("@user", SqlDbType.BigInt);
        //        pr2[4].Value = Convert.ToInt64(Session["login_user"]);

        //        pr2[5] = new SqlParameter("@id_no", SqlDbType.VarChar);
        //        pr2[5].Value = model.id_no;

        //        pr2[6] = new SqlParameter("@kyc_id", SqlDbType.Int);
        //        pr2[6].Value = model.kyc_id;

        //        pr2[7] = new SqlParameter("@branch_id", SqlDbType.BigInt);
        //        pr2[7].Value = Session["branch_id"];

        //        pr2[8] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
        //        pr2[8].Direction = ParameterDirection.Output;

        //        pr2[9] = new SqlParameter("@image1", SqlDbType.Binary);
        //        pr2[9].Value = imageBytesMasked;

        //        pr2[10] = new SqlParameter("@from_dt", SqlDbType.Date);
        //        pr2[10].Value = model.from_date;

        //        pr2[11] = new SqlParameter("@To_dt", SqlDbType.Date);
        //        pr2[11].Value = model.to_date;

        //        pr2[12] = new SqlParameter("@pan_name", SqlDbType.VarChar, 500);
        //        pr2[12].Value = model.pan_customer_name;

        //        pr2[13] = new SqlParameter("@pan_father", SqlDbType.VarChar, 500);
        //        pr2[13].Value = model.pan_father_name;

        //        pr2[14] = new SqlParameter("@pan_dob", SqlDbType.Date);
        //        pr2[14].Value = model.pan_dob;

        //        // ... (remaining parameters remain the same)

        //        dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr2);

        //        string msg = pr2[8].Value.ToString();
        //        string[] msg_string = msg.Split('#');

        //        string msg1 = msg_string[0].ToString();

        //        string msg2 = msg_string[1].ToString();


        //        tbl_customer_photo kyc_dt = new tbl_customer_photo();
        //        if (msg2 == "0")
        //        {
        //            return Json(new { success = false, message = msg1 });
        //        }
        //        else if (msg2 == "1")
        //        {
        //            DataTable dt_kycGrid = new DataTable();
        //            SqlParameter[] pr1 = new SqlParameter[2];
        //            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
        //            pr1[0].Value = 4;
        //            pr1[1] = new SqlParameter("@cid", SqlDbType.BigInt);
        //            pr1[1].Value = Session["TempCID"];
        //            dt_kycGrid = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr1).Tables[0];

        //            List<kyc_upload> model2 = new List<kyc_upload>();
        //            //model2.Data = new List<Dictionary<string, object>>();
        //            foreach (DataRow row in dt_kycGrid.Rows)
        //            {
        //                kyc_upload item = new kyc_upload
        //                {
        //                    sno = Convert.ToInt32(row["sno"]),
        //                    KYC = row["KYC"].ToString(),
        //                    kyc_id = Convert.ToInt32(row["kyc_id"]),
        //                    id_no = row["id_no"].ToString(),
        //                    Proof_Type = row["Proof_Type"].ToString(),
        //                    // cid = Convert.ToInt32(Session["TempCID"]),
        //                    FromDate = row["from_dt"].ToString(),
        //                    ToDate = row["to_dt"].ToString(),
        //                    ImageUrl = GetImageUrl(row["image"] as byte[]),
        //                    pan_customer_name = row["father"].ToString(),
        //                    pan_dob = row["name"].ToString(),
        //                    pan_father_name = row["dob_kyc"].ToString()

        //                    //Data= GetImageUrl(row["image"] as byte[])
        //                };

        //                model2.Add(item);
        //            }

        //            kyc_dt.kycgrid = model2;
        //        }
        //        else
        //        {
        //            kyc_dt.kycgrid = new List<kyc_upload>();
        //        }

        //        DataTable kyc_types = Fillkyc_type();
        //        ViewData["kyc_typeList"] = kyc_types;
        //        model.image = null;
        //        model.image1 = null;
        //        imageBytes = null; imageBytesMasked = null;

        //        byte[] photoData = GetExistingPhotoData();
        //        if (photoData != null)
        //        {
        //            string base64Photo = Convert.ToBase64String(photoData);

        //            // Pass Base64 string to the view
        //            ViewBag.PhotoData = base64Photo;
        //        }

        //        // return View("kyc_approval", kyc_dt);
        //        return Json(new { success = true, message = "KYC Saved", redirectTo = Url.Action("kyc_approval", kyc_dt) });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "error uploading file: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public ActionResult document_upload(tbl_customer_photo model)
        {
            try
            {
              
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
                               ? doc.CompressFile(pdfData,".pdf")
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

                SqlParameter[] pr2 = new SqlParameter[15]; // Increase the array size to accommodate the new parameters

                pr2[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr2[0].Value = 3;

                pr2[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr2[1].Value = Convert.ToInt64(Session["TempCID"]);

                pr2[2] = new SqlParameter("@image", SqlDbType.Binary);
                pr2[2].Value = imageBytes;

                pr2[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr2[3].Value = 1;

                pr2[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr2[4].Value = Convert.ToInt64(Session["login_user"]);

                pr2[5] = new SqlParameter("@id_no", SqlDbType.VarChar);
                pr2[5].Value = model.id_no;

                pr2[6] = new SqlParameter("@kyc_id", SqlDbType.Int);
                pr2[6].Value = model.kyc_id;

                pr2[7] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr2[7].Value = Session["branch_id"];

                pr2[8] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr2[8].Direction = ParameterDirection.Output;

                pr2[9] = new SqlParameter("@image1", SqlDbType.Binary);
                pr2[9].Value = imageBytesMasked;

                pr2[10] = new SqlParameter("@from_dt", SqlDbType.Date);
                pr2[10].Value = model.from_date;

                pr2[11] = new SqlParameter("@To_dt", SqlDbType.Date);
                pr2[11].Value = model.to_date;

                pr2[12] = new SqlParameter("@pan_name", SqlDbType.VarChar, 500);
                pr2[12].Value = model.pan_customer_name;

                pr2[13] = new SqlParameter("@pan_father", SqlDbType.VarChar, 500);
                pr2[13].Value = model.pan_father_name;

                pr2[14] = new SqlParameter("@pan_dob", SqlDbType.Date);
                pr2[14].Value = model.pan_dob;

                // ... (remaining parameters remain the same)
                dbconnect.Open();
                dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr2);
                dbconnect.Close();
                string msg = pr2[8].Value.ToString();
                string[] msg_string = msg.Split('#');

                string msg1 = msg_string[0].ToString();

                string msg2 = msg_string[1].ToString();


                tbl_customer_photo kyc_dt = new tbl_customer_photo();
                if (msg2 == "0")
                {
                    return Json(new { success = false, message = msg1 });
                }
                else if (msg2 == "1")
                {
                    DataTable dt_kycGrid = new DataTable();
                    SqlParameter[] pr1 = new SqlParameter[2];
                    pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                    pr1[0].Value = 4;
                    pr1[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                    pr1[1].Value = Session["TempCID"];
                    dbconnect.Open();
                    dt_kycGrid = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr1).Tables[0];
                    dbconnect.Close() ; 
                    List<kyc_upload> model2 = new List<kyc_upload>();
                    //model2.Data = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dt_kycGrid.Rows)
                    {
                        kyc_upload item = new kyc_upload
                        {
                            sno = Convert.ToInt32(row["sno"]),
                            KYC = row["KYC"].ToString(),
                            kyc_id = Convert.ToInt32(row["kyc_id"]),
                            id_no = row["id_no"].ToString(),
                            Proof_Type = row["Proof_Type"].ToString(),
                            // cid = Convert.ToInt32(Session["TempCID"]),
                            FromDate = row["from_dt"].ToString(),
                            ToDate = row["to_dt"].ToString(),
                            ImageUrl = GetImageUrl(row["image"] as byte[]),
                            //pan_customer_name = row["father"].ToString(),
                            //pan_dob = row["name"].ToString(),
                            //pan_father_name = row["dob_kyc"].ToString()
                            pan_customer_name = row["name"].ToString(),
                            pan_father_name = row["father"].ToString(),
                            pan_dob = row["dob_kyc"].ToString()

                            //Data= GetImageUrl(row["image"] as byte[])
                        };

                        model2.Add(item);
                    }

                    kyc_dt.kycgrid = model2;
                }
                else
                {
                    kyc_dt.kycgrid = new List<kyc_upload>();
                }

                DataTable kyc_types = Fillkyc_type();
                ViewData["kyc_typeList"] = kyc_types;
                model.image = null;
                model.image1 = null;
                imageBytes = null; imageBytesMasked = null;

                byte[] photoData = GetExistingPhotoData();
                if (photoData != null)
                {
                    string base64Photo = Convert.ToBase64String(photoData);

                    // Pass Base64 string to the view
                    ViewBag.PhotoData = base64Photo;
                }

                // SQL Parameters and Database Insertion Logic Here (No Change)

                return Json(new { success = true, message = "KYC Saved", redirectTo = Url.Action("kyc_approval") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }

       


        [HttpPost]
        public ActionResult kyc_attach_delet(int kyc_id)
        {

            SqlParameter[] pr3 = new SqlParameter[4];

            pr3[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr3[0].Value = 5;

            pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr3[1].Value = Convert.ToInt64(Session["TempCID"]);

            pr3[2] = new SqlParameter("@kyc_id", SqlDbType.Int);
            pr3[2].Value = kyc_id;

            pr3[3] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
            pr3[3].Direction = ParameterDirection.Output;

            dbconnect.Open();   
            dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr3);
            dbconnect.Close();
            string msg = pr3[3].Value.ToString();

            DataTable kyc_types = Fillkyc_type();
            DataTable dt_kycGrid = new DataTable();
            
            ViewData["kyc_typeList"] = kyc_types;

            tbl_customer_photo model = new tbl_customer_photo();
            tbl_customer_photo mod = new tbl_customer_photo();

            mod.CustomerName = Session["CustomerName"].ToString();


            SqlParameter[] pr1 = new SqlParameter[2];

            pr1[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
            pr1[0].Value = 4;

            pr1[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr1[1].Value = Session["TempCID"];
            dbconnect.Open();
            dt_kycGrid = dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr1).Tables[0];
            dbconnect.Close() ;
            List<kyc_upload> model2 = new List<kyc_upload>();

            foreach (DataRow row in dt_kycGrid.Rows)
            {
                kyc_upload item = new kyc_upload
                {
                    sno = Convert.ToInt32(row["sno"]),
                    KYC = row["KYC"].ToString(),
                    kyc_id = Convert.ToInt32(row["kyc_id"]),
                    id_no = row["id_no"].ToString(),
                    Proof_Type = row["Proof_Type"].ToString(),
                   // cid = Convert.ToInt32(Session["TempCID"]),
                    FromDate = row["from_dt"].ToString(),
                    ToDate = row["to_dt"].ToString(),
                    ImageUrl = GetImageUrl(row["image"] as byte[])

                };


                model2.Add(item);
                model.kycgrid = model2;
            }

            if (msg == "1")

                return Json(new { success = true });              
               

            else

                return Json(new { success = false });
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


        [HttpPost]
        public ActionResult dec_upload(tbl_customer_photo model)
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

                // Check and process the second image (model.image1)

                SqlParameter[] pr3 = new SqlParameter[7]; // Increase the array size to accommodate the new parameters

                pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr3[0].Value = 8;

                pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr3[1].Value = Convert.ToInt64(Session["TempCID"]);

                pr3[2] = new SqlParameter("@image3", SqlDbType.Binary);
                pr3[2].Value = imageBytes_dec;

                pr3[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr3[3].Value = 1;

                pr3[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr3[4].Value = Convert.ToInt64(Session["login_user"]);


                pr3[5] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr3[5].Value = Session["branch_id"];

                pr3[6] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr3[6].Direction = ParameterDirection.Output;
                dbconnect.Open();   
                dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr3);
                dbconnect.Close();
                string msg_dis = pr3[6].Value.ToString();

                return RedirectToAction("kyc_approval", model);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error uploading file: " + ex.Message });
            }

        }

        [HttpPost]
        public ActionResult Send_Apprv()
        {

            SqlParameter[] pr4 = new SqlParameter[3];

            pr4[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr4[0].Value = 10;

            pr4[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr4[1].Value = Session["TempCID"].ToString();

            pr4[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr4[2].Direction = ParameterDirection.Output;

            dbconnect.Open();
            dbconnect.ExecuteStoredProcedure("[dbo].[cust_photo_kyc]", pr4);
            string msg = pr4[2].Value.ToString();
            dbconnect.Close() ;
            string[] msg_string = msg.Split('.');
          
            if (msg_string[0] != null && msg_string[0] == "Successfully Send for Approval")
            {
               // TempData["SuccessMessage"] = msg;
                TempData["SuccessMessage"] = msg_string[0].ToString();
            }
            else
            {
                //TempData["ErrorMessage"] = msg;
                TempData["ErrorMessage"] = msg_string[0].ToString();

            }

            return Json(new { success = true, message = msg_string[0].ToString() });
        }

        [HttpPost]
        public ActionResult Declaration_check()
        {
            SqlParameter[] pr3 = new SqlParameter[3];

            pr3[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr3[0].Value = 11;

            pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr3[1].Value = Convert.ToInt64(Session["TempCID"]);

            //pr3[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            //pr3[2].Value = Session["branch_id"];

            pr3[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr3[2].Direction = ParameterDirection.Output;

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr3);
            dbconnect.Close();
            string msg1 = pr3[2].Value.ToString();

            //string[] msg_string = msg1.Split('.');

            return Json(new { success = true, message = msg1 });

        }

        [HttpPost]
        public ActionResult form_60_check()
        {
            SqlParameter[] pr6 = new SqlParameter[3];

            pr6[0] = new SqlParameter("@query_id", SqlDbType.Int);
            pr6[0].Value = 13;

            pr6[1] = new SqlParameter("@cid", SqlDbType.BigInt);
            pr6[1].Value = Convert.ToInt64(Session["TempCID"]);

            //pr3[2] = new SqlParameter("@branch_id", SqlDbType.BigInt);
            //pr3[2].Value = Session["branch_id"];

            pr6[2] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100);
            pr6[2].Direction = ParameterDirection.Output;

            dbconnect.Open();
            dbconnect.ExecuteDataset("[dbo].[cust_temp_create]", pr6);
            dbconnect.Close();
            string msg1 = pr6[2].Value.ToString();
            
            return Json(new { success = true, message = msg1 });

        }

        [HttpPost]
        public ActionResult form60_upload(tbl_customer_photo model)
        {
            try
            {


                byte[] imageBytes_dec = null;

                // Check and process the first image (model.image)
                if (model.image4 != null && model.image4.ContentLength > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.image4.InputStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        byte[] pdfData = memoryStream.ToArray();

                        if (model.image4.ContentType == "application/pdf")
                        {
                            imageBytes_dec = memoryStream.Length > 256 * 1024
                               ? doc.CompressFile(pdfData, ".pdf")
                               : memoryStream.ToArray();
                        }
                        else if (model.image4.ContentType == "image/png" ||
                              model.image4.ContentType == "image/jpg" ||
                              model.image4.ContentType == "image/jpeg" ||
                              model.image4.ContentType == "image/bmp" ||
                              model.image4.ContentType == "image/gif")
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

                // Check and process the second image (model.image1)

                SqlParameter[] pr3 = new SqlParameter[7]; // Increase the array size to accommodate the new parameters

                pr3[0] = new SqlParameter("@query_id", SqlDbType.BigInt);
                pr3[0].Value = 13;

                pr3[1] = new SqlParameter("@cid", SqlDbType.BigInt);
                pr3[1].Value = Convert.ToInt64(Session["TempCID"]);

                pr3[2] = new SqlParameter("@image3", SqlDbType.Binary);
                pr3[2].Value = imageBytes_dec;

                pr3[3] = new SqlParameter("@status_id", SqlDbType.Int);
                pr3[3].Value = 1;

                pr3[4] = new SqlParameter("@user", SqlDbType.BigInt);
                pr3[4].Value = Convert.ToInt64(Session["login_user"]);


                pr3[5] = new SqlParameter("@branch_id", SqlDbType.BigInt);
                pr3[5].Value = Session["branch_id"];

                pr3[6] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500);
                pr3[6].Direction = ParameterDirection.Output;
                dbconnect.Open();
                dbconnect.ExecuteDataset("[dbo].[cust_photo_kyc]", pr3);
                dbconnect.Close();
                string msg_dis = pr3[6].Value.ToString();

                //  return RedirectToAction("kyc_approval", model);
               // return Json(new { success = true, message = msg_dis });
                return Json(new { success = true, message = msg_dis, redirectTo = Url.Action("kyc_approval", model) });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "error uploading file: " + ex.Message });
            }

        }

    }
}





























