using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using ERP_YOGLOANS_LOCAL.Models;
using Newtonsoft.Json;

namespace ERP_YOGLOANS_LOCAL.Controllers.customer
{
    public class Customer_BlockApproveListController : Controller
    {
        DB dbconnect = new DB();

        [HttpGet]
        public async Task<ActionResult> BlockList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FillDropdown(int type)
        {
            DataTable dt = null;
            var jsonResult = new List<object>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@query_id", SqlDbType.Int) { Value = 4 },
                new SqlParameter("@type", SqlDbType.Int) { Value = type }
            };

            dbconnect.Open();
            dt = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", parameters).Tables[0];
            dbconnect.Close();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    jsonResult.Add(new
                    {
                        region_id = row["region_id"],
                        region_name = row["region_name"]
                    });
                }
            }

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetCustomerBlockList(int region_id, int type)
        {
            var customerBlockList = GetCustomerBlockListInternal(region_id, type);
            return Json(customerBlockList, JsonRequestBehavior.AllowGet);
        }

        private List<Customer_Blocklist_Model> GetCustomerBlockListInternal(int region_id, int type)
        {
            var customerBlockList = new List<Customer_Blocklist_Model>();

            SqlParameter[] parameters = new SqlParameter[3]
            {
                    new SqlParameter("@query_id", SqlDbType.Int) { Value = 6 },
                    new SqlParameter("@region_id", SqlDbType.Int) { Value = region_id },
                    new SqlParameter("@type", SqlDbType.Int) { Value = type }
            };


            dbconnect.Open();
            var ds = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", parameters);
            dbconnect.Close();

            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                customerBlockList = dt.AsEnumerable().Select(row => new Customer_Blocklist_Model
                {
                    SlNo = Convert.ToInt32(row["SlNo"]),
                    Type = Convert.ToString(row["Type"]),
                    CustomerID = Convert.ToInt32(row["CustomerID"]),

                    Name = Convert.ToString(row["Name"]),
                    Comments = Convert.ToString(row["Comments"]),
                    RequestBy = Convert.ToString(row["RequestBy"]),
                    RequestDt = row["RequestDt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["RequestDt"]),
                    //Mobile = Convert.ToString(row["Mobile No"]),
                }).ToList();
            }


            return customerBlockList;
        }

        [HttpGet]
        public JsonResult GetCustomerDetails(int SlNo)
        {
            if (SlNo <= 0)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Invalid customer ID"
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DataSet ds = null;

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@query_id", SqlDbType.Int) { Value = 7 },
            new SqlParameter("@slno", SqlDbType.Int) { Value = SlNo }
                };

                dbconnect.Open();
                ds = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", parameters);
                dbconnect.Close();

                // Initialize response objects
                var existingCustomer = (object)null;
                var newCustomerDetails = (object)null;
                var imageData = (object)null;
                var imageType = (string)null;

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Assuming the third table is for image data
                    DataTable imageTable = ds.Tables.Count > 2 ? ds.Tables[2] : null;

                    if (imageTable != null && imageTable.Rows.Count > 0)
                    {
                        var imageRow = imageTable.Rows[0];
                        byte[] imageBytes = imageRow["image"] as byte[];

                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            string base64Data = Convert.ToBase64String(imageBytes);

                            // Check for PDF magic numbers
                            if (imageBytes.Length > 4 &&
                                imageBytes[0] == 0x25 && // '%'
                                imageBytes[1] == 0x50 && // 'P'
                                imageBytes[2] == 0x44 && // 'D'
                                imageBytes[3] == 0x46 && // 'F'
                                imageBytes[4] == 0x2D)   // '-'
                            {
                                imageData = $"data:application/pdf;base64,{base64Data}";
                                imageType = "pdf";
                            }
                            else
                            {
                                // Determine MIME type based on file signature or use default image MIME type
                                string mimeType = GetImageMimeType(imageBytes);
                                imageData = $"data:{mimeType};base64,{base64Data}";
                                imageType = "image";
                            }
                        }
                    }

                    // Existing and New Customer Data Extraction Logic
                    var existingCustomerTable = ds.Tables[0];
                    var newCustomerTable = ds.Tables[1];

                    if (existingCustomerTable.Rows.Count > 0)
                    {

                        existingCustomer = new
                        {
                            Success = true,
                            Message = "Existing customer details retrieved successfully",
                            CustomerID = existingCustomerTable.Rows[0]["customer_id"] != DBNull.Value ? (int?)Convert.ToInt32(existingCustomerTable.Rows[0]["customer_id"]) : null,
                            Name = existingCustomerTable.Rows[0]["ExistingCustomer"].ToString(),
                            MobileNo = existingCustomerTable.Rows[0]["MobileNo"].ToString(),
                            Comment = existingCustomerTable.Rows[0]["comment"].ToString(),
                            BranchName = existingCustomerTable.Rows[0]["branch_name"].ToString()
                        };


                    }


                    if (newCustomerTable.Rows.Count > 0)
                    {
                        if (newCustomerTable.Rows[0]["new_custid"].ToString() != "0")
                        {

                            newCustomerDetails = new
                            {
                                Success = true,
                                Message = "New customer details retrieved successfully",
                                NewCustomerID = newCustomerTable.Rows[0]["new_custid"] != DBNull.Value ? (int?)Convert.ToInt32(newCustomerTable.Rows[0]["new_custid"]) : null,
                                NewCustomerName = newCustomerTable.Rows[0]["NewCustomer"].ToString(),
                                NewCustomerMobile = newCustomerTable.Rows[0]["mobile_no"].ToString(),
                                NewCustomerBranchName = newCustomerTable.Rows[0]["branch_name"].ToString(),
                                Image = imageData,
                                ImageType = imageType
                            };


                        }
                    }


                    return Json(new
                    {
                        Success = true,
                        ExistingCustomer = existingCustomer,
                        NewCustomer = newCustomerDetails,


                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No customer details found"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if needed
                return Json(new
                {
                    Success = false,
                    Message = "Error fetching customer details"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Helper method to get MIME type of an image
        private string GetImageMimeType(byte[] imageBytes)
        {
            // A simple approach to determine the MIME type based on the image signature
            if (imageBytes.Length > 1)
            {
                if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                {
                    return "image/jpeg";
                }
                else if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50)
                {
                    return "image/png";
                }
                else if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49)
                {
                    return "image/tiff";
                }
                else if (imageBytes[0] == 0x49 && imageBytes[1] == 0x49)
                {
                    return "image/tiff";
                }
                else if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                {
                    return "image/bmp";
                }
                // Add more image formats as needed
            }
            return "image/unknown";
        }



        [HttpGet]
        public JsonResult GetLoanDetails(int customerID)
        {
            try
            {
                DataTable dt = null;

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@query_id", SqlDbType.Int) { Value = 2 },
            new SqlParameter("@customer_id", SqlDbType.Int) { Value = customerID }
                };

                dbconnect.Open();
                dt = dbconnect.ExecuteDataset("[dbo].[cust_merge_proc]", parameters).Tables[0];
                dbconnect.Close();

                if (dt.Rows.Count > 0)
                {
                    var loanDetails = dt.AsEnumerable().Select(row => new
                    {
                        Module = row["Module"] != DBNull.Value ? row["Module"].ToString() : "N/A",
                        AccountNo = row["Account No"] != DBNull.Value ? row["Account No"].ToString() : "N/A",
                        StartDate = row["Start Dt"] != DBNull.Value ? row["Start Dt"].ToString() : "N/A",
                        MaturityDate = row["Maturity Dt"] != DBNull.Value ? row["Maturity Dt"].ToString() : "N/A",
                        Amount = row["Amount"] != DBNull.Value ? row["Amount"].ToString() : "N/A"
                    }).ToList();

                    return Json(new
                    {
                        Success = true,
                        Message = "Loan details retrieved successfully",
                        LoanDetails = loanDetails
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "No Active Accounts"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error fetching loan details",
                    Error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult SetSessionValue(int customerID)
        {
            Session["customer_id_his"] = customerID;
            return Json(new
            {
                Success = true,
                Message = 1
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult RejectCustomer(int customerID, long sl_no, string rejectReason)
        {
            // Define the query ID for rejection
            int queryId = 8;

            // Prepare SQL parameters
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = queryId };
            parameters[1] = new SqlParameter("@slno", SqlDbType.BigInt) { Value = sl_no };
            parameters[2] = new SqlParameter("@comments", SqlDbType.NVarChar, 500) { Value = rejectReason };
            parameters[3] = new SqlParameter("@user", SqlDbType.VarChar, 50) { Value = Session["login_user"] };
            parameters[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            parameters[5] = new SqlParameter("@customer_id", SqlDbType.Int) { Value = customerID };
            try
            {
                // Call the stored procedure
                dbconnect.ExecuteStoredProcedure("[dbo].[cust_merge_proc]", parameters);

                // Retrieve the result message
                string outmsg = parameters[4].Value?.ToString() ?? "No message returned.";

                // Return a JSON result for the AJAX call
                return Json(new { success = true, message = outmsg });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // LogException(ex); // Implement your logging here

                // Return an error result for the AJAX call
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ApproveCustomer(int customerID, long sl_no, int secondCustomerID, int type)
        {
            int queryId = 9; // Use the appropriate query ID for approval
            //Session["Enter_by"] = Session["login_user"];

            SqlParameter[] parameters = new SqlParameter[7];
            parameters[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = queryId };
            parameters[1] = new SqlParameter("@slno", SqlDbType.BigInt) { Value = sl_no };
            parameters[2] = new SqlParameter("@customer_id", SqlDbType.Int) { Value = customerID };
            parameters[3] = new SqlParameter("@user", SqlDbType.VarChar, 50) { Value = Session["login_user"] };
            parameters[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            parameters[5] = new SqlParameter("@new_custid", SqlDbType.Int) { Value = secondCustomerID };
            parameters[6] = new SqlParameter("@type", SqlDbType.Int) { Value = type };

            try
            {
                dbconnect.ExecuteStoredProcedure("[dbo].[cust_merge_proc]", parameters);

                string outmsg = parameters[4].Value?.ToString() ?? "No message returned.";

                return Json(new { success = true, message = outmsg });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // LogException(ex); // Implement your logging here

                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult BlockCustomer(int customerID, long sl_no, int type)
        {
            int queryId = 9; // Use the appropriate query ID for approval
            //Session["Enter_by"] = Session["login_user"];

            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@query_id", SqlDbType.Int) { Value = queryId };
            parameters[1] = new SqlParameter("@slno", SqlDbType.BigInt) { Value = sl_no };
            parameters[2] = new SqlParameter("@customer_id", SqlDbType.Int) { Value = customerID };
            parameters[3] = new SqlParameter("@user", SqlDbType.VarChar, 50) { Value = Session["login_user"] };
            parameters[4] = new SqlParameter("@outmsg", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output };
            // parameters[5] = new SqlParameter("@new_custid", SqlDbType.Int) { Value = secondCustomerID };
            parameters[5] = new SqlParameter("@type", SqlDbType.Int) { Value = type };

            try
            {
                dbconnect.ExecuteStoredProcedure("[dbo].[cust_merge_proc]", parameters);

                string outmsg = parameters[4].Value?.ToString() ?? "No message returned.";

                return Json(new { success = true, message = outmsg });
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // LogException(ex); // Implement your logging here

                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

    }
}
