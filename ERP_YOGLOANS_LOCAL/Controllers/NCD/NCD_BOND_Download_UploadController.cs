using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;

namespace ERP_YOGLOANS_LOCAL.Controllers.NCD
{
    public class NCD_BOND_Download_UploadController : Controller
    {
        DB dbconnect = new DB();

        // GET: NCD_Download_Upload
        public ActionResult NCD_BOND_Download_Upload_View()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetGridData()
        {
            try
            {
                
                var branchId = Session["branch_id"];
                if (branchId == null || Convert.ToInt32(branchId) <= 0)
                {
                    return Json(new { success = false, message = "Branch ID is required and not found in the session." });
                }
                DataTable dt = new DataTable();

                SqlParameter[] pr = new SqlParameter[2];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 13 };
                pr[1] = new SqlParameter("@branch_id", SqlDbType.Int) { Value = Convert.ToInt32(branchId) };



                dbconnect.Open();

                if (Session["module_id"]?.ToString() == "15")
                    dt = dbconnect.ExecuteDataset("[dbo].[DEB_transfer_queries]", pr).Tables[0];
                if (Session["module_id"]?.ToString() == "16")
                    dt = dbconnect.ExecuteDataset("[dbo].[BOND_transfer_queries]", pr).Tables[0];

                dbconnect.Close();

                if (dt.Rows.Count > 0)
                {
                    var result = dt.AsEnumerable().Select(row => new
                    {
                        TransferID = row["Transfer ID"],
                        Issue = row["Issue"],
                        Certificate = row["Certificate"],
                        TransferType = row["TransferType"],
                        Transferee = row["Transferee"],
                        TransfereeID = row["Transferee ID"],
                        UploadStatus = row["Upload Status"],
                        ID = row["ID"],  // ID - DebID or BondID
                        Transfer_Type = row["transfer_type"]


                    }).ToList();

                    return Json(new { success = true, data = result });
                }
                else
                {
                    return Json(new { success = false, message = "No data found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public JsonResult HandleFileUpload()
        {
            try
            {

                var file = Request.Files["file"];
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "No file selected." });
                }

                string docType = Request.Form["docType"];
                int docId = Convert.ToInt32(Request.Form["docId"]);
                int transferId = Convert.ToInt32(Request.Form["transferId"]);
                string fileExtension = Path.GetExtension(file.FileName).ToLower();


                byte[] fileData;
                using (var reader = new BinaryReader(file.InputStream))
                {
                    fileData = reader.ReadBytes(file.ContentLength);
                }


                SqlParameter[] pr = new SqlParameter[6];
                pr[0] = new SqlParameter("@query_id", SqlDbType.BigInt) { Value = 14 }; // Hard-coded query ID for the insert
                pr[1] = new SqlParameter("@transtype", SqlDbType.BigInt) { Value = docId }; // Use docId as transtype
                pr[2] = new SqlParameter("@tranferid", SqlDbType.BigInt) { Value = transferId }; // Transfer ID
                pr[3] = new SqlParameter("@data", SqlDbType.VarBinary) { Value = fileData }; // File data
                pr[4] = new SqlParameter("@enter_by", SqlDbType.BigInt) { Value = Session["login_user"] }; // Entered by user
                pr[5] = new SqlParameter("@outmsg", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output }; // Output message


                dbconnect.Open();

                if (Session["module_id"]?.ToString() == "15")
                    dbconnect.ExecuteStoredProcedure("[dbo].[DEB_transfer_queries]", pr);
                if (Session["module_id"]?.ToString() == "16")
                    dbconnect.ExecuteStoredProcedure("[dbo].[BOND_transfer_queries]", pr);

                

                dbconnect.Close();

                string message = pr[5].Value.ToString();

                return Json(new { success = true, message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
