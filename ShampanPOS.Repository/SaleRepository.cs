using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using Newtonsoft.Json;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{  
    public class SaleRepository : CommonRepository
    {

        //Insert Method
        //public async Task<ResultVM> Insert(SaleVM sale, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        if (transaction == null)
        //        {
        //            transaction = conn.BeginTransaction();
        //        }

        //        string query = @"
        //        INSERT INTO Sales 
        //        (Code, BranchId, CustomerId, SalePersonId, RouteId, DeliveryAddress, VehicleNo, VehicleType, 
        //         InvoiceDateTime, DeliveryDate, GrandTotalAmount, GrandTotalSDAmount, GrandTotalVATAmount, Comments, 
        //         IsPrint,TransactionType, IsPost,  FiscalYear, PeriodId, 
        //         CurrencyId, CurrencyRateFromBDT, CreatedBy, CreatedOn, CreatedFrom)
        //        VALUES 
        //        (@Code, @BranchId, @CustomerId, @SalePersonId, @RouteId, @DeliveryAddress, @VehicleNo, @VehicleType, 
        //         @InvoiceDateTime, @DeliveryDate, @GrandTotalAmount, @GrandTotalSDAmount, @GrandTotalVATAmount, @Comments, 
        //         @IsPrint, @TransactionType, @IsPost, @FiscalYear, @PeriodId, 
        //         @CurrencyId, @CurrencyRateFromBDT, @CreatedBy, GETDATE(), @CreatedFrom);
        //        SELECT SCOPE_IDENTITY();";


        //        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //        {
        //            // Adding parameters with value checks
        //            //cmd.Parameters.AddWithValue("@Id", sale.Id);
        //            cmd.Parameters.AddWithValue("@Code", sale.Code);
        //            cmd.Parameters.AddWithValue("@BranchId", sale.BranchId);
        //            cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
        //            cmd.Parameters.AddWithValue("@SalePersonId", sale.SalePersonId);
        //            cmd.Parameters.AddWithValue("@RouteId", sale.RouteId);
        //            cmd.Parameters.AddWithValue("@DeliveryAddress", sale.DeliveryAddress ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@VehicleNo", sale.VehicleNo ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@VehicleType", sale.VehicleType ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@InvoiceDateTime", sale.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
        //            cmd.Parameters.AddWithValue("@DeliveryDate", sale.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));

        //            cmd.Parameters.AddWithValue("@GrandTotalAmount", sale.GrandTotalAmount);
        //            cmd.Parameters.AddWithValue("@GrandTotalSDAmount", sale.GrandTotalSDAmount);
        //            cmd.Parameters.AddWithValue("@GrandTotalVATAmount", sale.GrandTotalVATAmount);
        //            cmd.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@IsPrint", false); // Assuming IsPrint is a bit column

        //            cmd.Parameters.AddWithValue("@TransactionType", sale.TransactionType);
        //            cmd.Parameters.AddWithValue("@IsPost", false);

        //            cmd.Parameters.AddWithValue("@FiscalYear", sale.FiscalYear ?? (object)DBNull.Value); // Optional field
        //            cmd.Parameters.AddWithValue("@PeriodId", sale.PeriodId ?? ""); // Optional field
        //            cmd.Parameters.AddWithValue("@CurrencyId", sale.CurrencyId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", sale.CurrencyRateFromBDT ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@CreatedBy", sale.CreatedBy ?? "ERP");
        //            cmd.Parameters.AddWithValue("@CreatedFrom", sale.CreatedFrom ?? (object)DBNull.Value);


        //            // Execute the query and get the new ID (primary key)
        //            object newId = cmd.ExecuteScalar();

        //            result.Status = "Success";
        //            result.Message = "Data inserted successfully.";
        //            result.Id = newId.ToString();
        //            result.DataVM = sale.Id;
        //        }

        //        if (isNewConnection)
        //        {
        //            transaction.Commit();
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //        {
        //            transaction.Rollback();
        //        }

        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}


        // InsertDetails Method
        public async Task<ResultVM> Insert(SaleVM sale, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
        INSERT INTO Sales 
        (Code, BranchId, CustomerId, SalePersonId, RouteId, DeliveryAddress, VehicleNo, VehicleType, 
         InvoiceDateTime, DeliveryDate, GrandTotalAmount, GrandTotalSDAmount, GrandTotalVATAmount, Comments, 
         IsPrint, TransactionType, IsPost, FiscalYear, PeriodId, 
         CurrencyId, CurrencyRateFromBDT, CreatedBy, CreatedOn, CreatedFrom)
        VALUES 
        (@Code, @BranchId, @CustomerId, @SalePersonId, @RouteId, @DeliveryAddress, @VehicleNo, @VehicleType, 
         @InvoiceDateTime, @DeliveryDate, @GrandTotalAmount, @GrandTotalSDAmount, @GrandTotalVATAmount, @Comments, 
         @IsPrint, @TransactionType, @IsPost, @FiscalYear, @PeriodId, 
         @CurrencyId, @CurrencyRateFromBDT, @CreatedBy, GETDATE(), @CreatedFrom);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", sale.Code);
                    cmd.Parameters.AddWithValue("@BranchId", sale.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", sale.SalePersonId);
                    cmd.Parameters.AddWithValue("@RouteId", sale.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", sale.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleNo", sale.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", sale.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", sale.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", sale.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));

                    cmd.Parameters.AddWithValue("@GrandTotalAmount", sale.GrandTotalAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", sale.GrandTotalSDAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", sale.GrandTotalVATAmount);
                    cmd.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPrint", false);
                    cmd.Parameters.AddWithValue("@TransactionType", sale.TransactionType);
                    cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.AddWithValue("@FiscalYear", sale.FiscalYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PeriodId", sale.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@CurrencyId", sale.CurrencyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", sale.CurrencyRateFromBDT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", sale.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedFrom", sale.CreatedFrom ?? (object)DBNull.Value);

                    object newId = cmd.ExecuteScalar();
                    sale.Id = Convert.ToInt32(newId);
                }

                // Insert Sale Details
                if (sale.saleDetailsList != null && sale.saleDetailsList.Count > 0)
                {
                    foreach (var detail in sale.saleDetailsList)
                    {
                        detail.SaleId = sale.Id.ToString(); // Assign SaleId to details
                        ResultVM detailResult = InsertDetails(detail, conn, transaction);

                        if (detailResult.Status != "Success")
                        {
                            throw new Exception("Failed to insert Sale Details: " + detailResult.Message);
                        }
                    }
                }

                // Commit transaction only if everything is successful
                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.Id = sale.Id.ToString();
                result.DataVM = sale.Id;

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public ResultVM InsertDetails(SaleDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO SaleDetails
(SaleId, SaleDeliveryId, SaleDeliveryDetailId, SaleOrderId, SaleOrderDetailId, BranchId, Line, ProductId, 
 Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, 
 UOMconversion, Comments, VATType, TransactionType, IsPost)
VALUES 
(@SaleId, @SaleDeliveryId, @SaleDeliveryDetailId, @SaleOrderId, @SaleOrderDetailId, @BranchId, @Line, @ProductId, 
 @Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, 
 @UOMconversion, @Comments, @VATType, @TransactionType, @IsPost);
SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Adding parameters to the SQL command
                    cmd.Parameters.AddWithValue("@SaleId", details.SaleId );
                    cmd.Parameters.AddWithValue("@SaleDeliveryId", details.SaleDeliveryId );
                    cmd.Parameters.AddWithValue("@SaleDeliveryDetailId", details.SaleDeliveryDetailId ?? 0 );
                    cmd.Parameters.AddWithValue("@SaleOrderId", details.SaleOrderId ?? 0 );
                    cmd.Parameters.AddWithValue("@SaleOrderDetailId", details.SaleOrderDetailId ?? 0 );
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId ?? 0 );
                    cmd.Parameters.AddWithValue("@Line", details.Line );
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity ?? 0);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate ?? 0);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal ?? 0);
                    cmd.Parameters.AddWithValue("@SD", details.SD ?? 0);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount ?? 0 );
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount ?? 0 );
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal ?? 0);
                    cmd.Parameters.AddWithValue("@UOMId", details.UOMId ?? 0 );
                    cmd.Parameters.AddWithValue("@UOMFromId", details.UOMFromId ?? 0 );
                    cmd.Parameters.AddWithValue("@UOMConversion", details.UOMConversion ?? 0 );
                    cmd.Parameters.AddWithValue("@VATType", "VAT");
                    cmd.Parameters.AddWithValue("@TransactionType", details.TransactionType ?? "Sales");
                    cmd.Parameters.AddWithValue("@IsPost",false);
                    cmd.Parameters.AddWithValue("@Comments", details.Comments ?? "");

                    // Execute the query and get the new ID (primary key)
                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        // Update Method
        public async Task<ResultVM> Update(SaleVM sale, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = sale.Id.ToString(), DataVM = sale };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string query = @"
UPDATE Sales
SET 
    BranchId = @BranchId,
    CustomerId = @CustomerId,
    SalePersonId = @SalePersonId,
    RouteId = @RouteId,
    DeliveryAddress = @DeliveryAddress,
    VehicleNo = @VehicleNo,
    VehicleType = @VehicleType,
    InvoiceDateTime = @InvoiceDateTime,
    DeliveryDate = @DeliveryDate,
    GrandTotalAmount = @GrandTotalAmount,
    GrandTotalSDAmount = @GrandTotalSDAmount,
    GrandTotalVATAmount = @GrandTotalVATAmount,
    Comments = @Comments,
    IsPrint = @IsPrint,
    TransactionType = @TransactionType,
    IsPost = @IsPost,
    FiscalYear = @FiscalYear,
    PeriodId = @PeriodId,
    CurrencyId = @CurrencyId,
    CurrencyRateFromBDT = @CurrencyRateFromBDT,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = @LastModifiedOn,
    LastUpdateFrom = @LastUpdateFrom
WHERE Id = @Id;
";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", sale.Id);
                    cmd.Parameters.AddWithValue("@BranchId", sale.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", sale.SalePersonId);
                    cmd.Parameters.AddWithValue("@RouteId", sale.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", sale.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleNo", sale.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", sale.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", sale.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", sale.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", sale.GrandTotalAmount );
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", sale.GrandTotalSDAmount );
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", sale.GrandTotalVATAmount );
                    cmd.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPrint", false);
                    cmd.Parameters.AddWithValue("@TransactionType", sale.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost",false);
                    cmd.Parameters.AddWithValue("@FiscalYear", sale.FiscalYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PeriodId", sale.PeriodId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyId", sale.CurrencyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", sale.CurrencyRateFromBDT ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@LastModifiedBy", sale.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", sale.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", sale.LastUpdateFrom ?? (object)DBNull.Value);

                    // Execute the query (Insert or Update)
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were updated.");
                    }
                }

                int LineNo = 1;

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

       
        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.SalePersonId, 0) AS SalePersonId,
    ISNULL(M.RouteId, 0) AS RouteId,
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(M.VehicleNo, '') AS VehicleNo,
    ISNULL(M.VehicleType, '') AS VehicleType,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(M.Comments, '') AS Comments,     
    ISNULL(M.IsPrint, 0) AS IsPrint,   
	ISNULL(M.PrintBy,'') AS PrintBy,   
    ISNULL(FORMAT(M.PrintOn, 'yyyy-MM-dd'), '1900-01-01') AS       PrintOn,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostBy, 0) AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd'), '1900-01-01') AS      PostedOn, 
    ISNULL(M.FiscalYear, 0) AS FiscalYear,
    ISNULL(M.PeriodId, 0) AS PeriodId,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Sales M
WHERE 
    1 = 1
 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<SaleVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                        RouteId = Convert.ToInt32(row["RouteId"]),
                        DeliveryAddress = row["DeliveryAddress"].ToString(),
                        VehicleNo = row["VehicleNo"].ToString(),
                        VehicleType = row["VehicleType"].ToString(),
                        InvoiceDateTime = row["InvoiceDateTime"] != DBNull.Value ? Convert.ToDateTime(row["InvoiceDateTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        DeliveryDate = row["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(row["DeliveryDate"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalAmount"]) : 0,
                        GrandTotalSDAmount = row["GrandTotalSDAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalSDAmount"]) : 0,
                        GrandTotalVATAmount = row["GrandTotalVATAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalVATAmount"]) : 0,
                        Comments = row["Comments"].ToString(),
                        IsPrint = row["IsPrint"] != DBNull.Value ? Convert.ToBoolean(row["IsPrint"]) : false,
                        PrintBy = row["PrintBy"].ToString(),
                        PrintOn = row["PrintOn"] != DBNull.Value ? Convert.ToDateTime(row["PrintOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = row["IsPost"] != DBNull.Value ? Convert.ToBoolean(row["IsPost"]) : false,
                        PostBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        FiscalYear = row["FiscalYear"].ToString(),
                        PeriodId = row["PeriodId"].ToString(),
                        CurrencyId = row["CurrencyId"] != DBNull.Value ? Convert.ToInt32(row["CurrencyId"]) : 0,
                        CurrencyRateFromBDT = row["CurrencyRateFromBDT"] != DBNull.Value ? Convert.ToDecimal(row["CurrencyRateFromBDT"]) : 0,
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["LastModifiedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CreatedFrom = row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row["LastUpdateFrom"].ToString()
                    });
                }

                conditionalValue = new string[] { model.FirstOrDefault().Code.ToString() };

                var detailsDataList = DetailsList(new[] { "D.SaleId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDetailVM>>(json);

                    model.FirstOrDefault().saleDetailsList = details;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = model;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 
ISNULL(D.Id, 0) AS Id,
ISNULL(D.SaleDeliveryId, 0) AS SaleDeliveryId,
ISNULL(D.saleDeliveryDetailId, 0) AS saleDeliveryDetailId,
ISNULL(D.saleId, '') AS saleId,
ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
ISNULL(D.BranchId, 0) AS BranchId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0.00) AS Quantity,
ISNULL(FORMAT(D.UnitRate, 'N2'), '0.00') AS UnitRate,
ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
ISNULL(D.UOMId, 0) AS UOMId,
ISNULL(D.UOMFromId, 0) AS UOMFromId,
ISNULL(FORMAT(D.UOMConversion, 'N2'), '0.00') AS UOMConversion,
ISNULL(D.IsPost, 0) AS IsPost,
ISNULL(D.Comments, '') AS Comments,
ISNULL(D.Code, '') AS Code,
ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(P.UOMId,0) UOMId,
ISNULL(UOM.Name,'') UOMName,
ISNULL(P.UOMId,0) UOMFromId,
ISNULL(UOM.Name,'') UOMFromName

FROM 
SaleDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Details Data retrieved successfully.";
                result.DataVM = dataTable;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        // ListAsDataTable Method
        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.SalePersonId, 0) AS SalePersonId,
    ISNULL(M.RouteId, 0) AS RouteId,
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(M.VehicleNo, '') AS VehicleNo,
    ISNULL(M.VehicleType, '') AS VehicleType,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(M.Comments, '') AS Comments,     
    ISNULL(M.IsPrint, 0) AS IsPrint,   
	ISNULL(M.PrintBy,'') AS PrintBy,   
    ISNULL(FORMAT(M.PrintOn, 'yyyy-MM-dd'), '1900-01-01') AS       PrintOn,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, 0) AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd'), '1900-01-01') AS      PostedOn, 
    ISNULL(M.FiscalYear, 0) AS FiscalYear,
    ISNULL(M.PeriodId, 0) AS PeriodId,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Sales M
WHERE 
    1 = 1
  ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                DataTable dataTable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }
                    adapter.Fill(dataTable);
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        // Dropdown Method
        public async Task<ResultVM> Dropdown(SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT Id, Code
                FROM SaleDeleveries
                WHERE 1=1
                ORDER BY Id";

                DataTable dropdownData = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }
                    adapter.Fill(dropdownData);
                }

                result.Status = "Success";
                result.Message = "Dropdown data retrieved successfully.";
                result.DataVM = dropdownData;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        // MultiplePost Method
        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.IDs.ToString(), DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string inClause = string.Join(", ", vm.IDs.Select((id, index) => $"@Id{index}"));

                string query = $" UPDATE Sales SET IsPost = 1, PostBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE SaleDetails SET IsPost = 1 WHERE saleId IN ({inClause}) ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    cmd.Parameters.AddWithValue("@PostedBy", vm.ModifyBy);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data Posted successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were posted.");
                    }
                }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<SaleVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
		FROM Sales H 
		LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
		LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
		LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
		LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
		WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                
                ,ISNULL(H.Id,0)	Id
                ,ISNULL(H.Code,'') Code
                ,ISNULL(H.VehicleNo,'') VehicleNo
                ,ISNULL(H.VehicleType,'') VehicleType
                ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
                ,ISNULL(H.Comments,'') Comments
                ,ISNULL(H.IsPost, 0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
                ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
                ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
                ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
                ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
                ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate

                ,ISNULL(Br.Name,'') BranchName
                ,ISNULL(cus.Name,'') CustomerName
                ,ISNULL(SP.Name,'') SalePersonName
                ,ISNULL(rut.Name,'') RouteName

                FROM Sales H 
                LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
                LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
                LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// GetDetailsGridData
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                var data = new GridEntity<SaleDetail>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
		FROM Sales H 
		LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
	    LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
	    LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
	    LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN SaleDetails D ON H.Code = D.SaleId
        LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
        LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
		WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetail>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.DeliveryAddress, '') AS DeliveryAddress,
                ISNULL(H.Comments, '') AS Comments,
                ISNULL(H.IsPost, 0) AS IsPost,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount,
                ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount,
                ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount,
                ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
                ISNULL(FORMAT(H.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
                ISNULL(H.GrandTotalAmount, 0) AS GrandTotalAmount,
                ISNULL(H.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
                ISNULL(H.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
                ISNULL(Br.Name, '') AS BranchName,
                ISNULL(cus.Name, '') AS CustomerName,
                ISNULL(PD.Name, '') AS ProductName,
                ISNULL(UM.Name, '') AS UOMName,
                ISNULL(H.TransactionType, '') AS TransactionType,
                ISNULL(SP.Name, '') AS SalePersonName,
                ISNULL(rut.Name, '-') AS RouteName,
                ISNULL(con.Name, '') AS CurrencyName,
                ISNULL(H.CustomerId, 0) AS CustomerId,
                ISNULL(H.RouteId, 0) AS RouteId,
                ISNULL(H.SalePersonId, 0) AS SalePersonId,
                ISNULL(H.VehicleNo, '') AS VehicleNo,
                ISNULL(H.VehicleType, '') AS VehicleType,
                
                -- Sale Details
                ISNULL(D.Id, 0) AS SaleDeliveryDetailId,
                ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
                ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
                ISNULL(D.BranchId, 0) AS BranchId,
                ISNULL(D.ProductId, 0) AS ProductId,
                ISNULL(D.Quantity, 0) AS Quantity,
                ISNULL(D.UnitRate, 0) AS UnitRate,
                ISNULL(D.SubTotal, 0) AS SubTotal,
                ISNULL(D.SD, 0) AS SD,
                ISNULL(D.SDAmount, 0) AS SDAmount,
                ISNULL(D.VATRate, 0) AS VATRate,
                ISNULL(D.VATAmount, 0) AS VATAmount,
                ISNULL(D.LineTotal, 0) AS LineTotal,
                ISNULL(D.UOMId, 0) AS UOMId,
                ISNULL(D.UOMFromId, 0) AS UOMFromId,
                ISNULL(D.UOMConversion, 0) AS UOMConversion,
                ISNULL(D.IsPost, 0) AS DetailIsPost,
                ISNULL(D.Comments, '') AS DetailComments

                FROM Sales H
                LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
                LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
                LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
                LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id
                LEFT OUTER JOIN SaleDetails D ON H.Code = D.SaleId
                LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
                LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
                
                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetail>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleDetail>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = data;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
        public async Task<ResultVM> InsertDetail(SaleVM saleDetails, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                // SQL Insert query
                string query = @"
        INSERT INTO SaleDetails
        (SaleId, SaleDeliveryId, SaleDeliveryDetailId, SaleOrderId, SaleOrderDetailId, BranchId, Line, ProductId, 
        Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, 
        UOMconversion, Comments, VATType, TransactionType, IsPost)
        VALUES 
        (@SaleId, @SaleDeliveryId, @SaleDeliveryDetailId, @SaleOrderId, @SaleOrderDetailId, @BranchId, @Line, @ProductId, 
        @Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, 
        @UOMconversion, @Comments, @VATType, @TransactionType, @IsPost);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Loop through the sale details list and insert each detail
                    foreach (var detail in saleDetails.saleDetailsList)
                    {
                        // Clear parameters to avoid conflicts
                        cmd.Parameters.Clear();

                        // Adding parameters to the SQL command
                        cmd.Parameters.AddWithValue("@SaleId", saleDetails.Id);
                        cmd.Parameters.AddWithValue("@SaleDeliveryId", detail.SaleDeliveryId ?? 0);
                        cmd.Parameters.AddWithValue("@SaleDeliveryDetailId", detail.SaleDeliveryDetailId ?? 0);
                        cmd.Parameters.AddWithValue("@SaleOrderId", detail.SaleOrderId ?? 0);
                        cmd.Parameters.AddWithValue("@SaleOrderDetailId", detail.SaleOrderDetailId ?? 0);
                        cmd.Parameters.AddWithValue("@BranchId", saleDetails.BranchId);
                        cmd.Parameters.AddWithValue("@Line", detail.Line ?? 0);
                        cmd.Parameters.AddWithValue("@ProductId", detail.ProductId ?? 0);
                        cmd.Parameters.AddWithValue("@Quantity", detail.Quantity ?? 0);
                        cmd.Parameters.AddWithValue("@UnitRate", detail.UnitRate ?? 0);
                        cmd.Parameters.AddWithValue("@SubTotal", detail.SubTotal ?? 0);
                        cmd.Parameters.AddWithValue("@SD", detail.SD ?? 0);
                        cmd.Parameters.AddWithValue("@SDAmount", detail.SDAmount ?? 0);
                        cmd.Parameters.AddWithValue("@VATRate", detail.VATRate ?? 0);
                        cmd.Parameters.AddWithValue("@VATAmount", detail.VATAmount ?? 0);
                        cmd.Parameters.AddWithValue("@LineTotal", detail.LineTotal ?? 0);
                        cmd.Parameters.AddWithValue("@UOMId", detail.UOMId ?? 0);
                        cmd.Parameters.AddWithValue("@UOMFromId", detail.UOMFromId ?? 0);
                        cmd.Parameters.AddWithValue("@UOMConversion", detail.UOMConversion ?? 0);
                        cmd.Parameters.AddWithValue("@VATType", "VAT"); // Assuming VAT is constant
                        cmd.Parameters.AddWithValue("@TransactionType", saleDetails.TransactionType ?? "");
                        cmd.Parameters.AddWithValue("@IsPost", saleDetails.IsPost);
                        cmd.Parameters.AddWithValue("@Comments", detail.Comments ?? "");

                        // Execute the query and get the new ID (primary key)
                        object newId = await cmd.ExecuteScalarAsync();
                        detail.Id = Convert.ToInt32(newId);
                    }

                    // Commit the transaction if everything goes well
                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = saleDetails.Id.ToString();
                    result.DataVM = saleDetails;
                }
            }
            catch (Exception ex)
            {
                // In case of error, rollback the transaction (if needed) and set result messages
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultVM> ProductWiseSale(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                           SELECT     
                            -- Details Data
                            ISNULL(D.ProductId, 0) AS ProductId,
                            ISNULL(PG.Name, '') AS ProductGroupName,
                            ISNULL(P.Code, '') AS ProductCode,
                            ISNULL(P.Name, '') AS ProductName,
                            ISNULL(P.HSCodeNo, '') AS HSCodeNo,
                            ISNULL(uom.Name, '') AS UOMName,
                            ISNULL(SUM(D.Quantity), 0) AS Quantity

                            FROM Sales M
                            LEFT OUTER JOIN SaleDetails D ON ISNULL(M.Id,0) = D.SaleId
                            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                            LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
                            LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
		                    WHERE 1 = 1 
		                  ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    query += " AND CAST(M.DeliveryDate AS DATE) BETWEEN @FromDate AND @ToDate ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);
                query += @" GROUP BY    
                                    D.ProductId,
                                    P.HSCodeNo,
                                    P.Code,
                                    P.Name,
                                    PG.Name,
                                    uom.Name ";
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
                }

                objComm.Fill(dataTable);

                var lst = new List<SaleReportVM>();
                int serialNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleReportVM
                    {
                        SL = serialNumber,
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        ProductGroupName = row["ProductGroupName"].ToString(),
                        ProductCode = row["ProductCode"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        HSCodeNo = row["HSCodeNo"].ToString(),
                        UOMName = row["UOMName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"])
                    });
                    serialNumber++;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = lst;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 

ISNULL(M.Id, 0) AS Id,
ISNULL(M.RouteId, 0) AS RouteId,
ISNULL(R.Name, '') + ', ' + ISNULL(R.Address, '') AS RouteAddress,
ISNULL(M.Code, '') AS Code,
ISNULL(M.BranchId, 0) AS BranchId,
ISNULL(TRIM(branch.Name), '') AS BranchName,
ISNULL(TRIM(branch.Code), '') AS BranchCode,
ISNULL(TRIM(branch.Address), '') AS BranchAddress,
ISNULL(M.CustomerId, 0) AS CustomerId,
ISNULL(C.Name, '') AS CustomerName,
ISNULL(C.Address, '') AS CustomerAddress,
ISNULL(M.SalePersonId, 0) AS SalePersonId,
ISNULL(SP.Name, '') AS SalePersonName,
ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
ISNULL(M.Comments, '') AS Comments,
ISNULL(M.TransactionType, '') AS TransactionType,
ISNULL(M.CurrencyId, 0) AS CurrencyId,
ISNULL(Cur.Name, '') AS CurrencyName,
ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,


ISNULL(D.Id, 0) AS DetailId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0) AS Quantity,
ISNULL(D.UnitRate, 0) AS UnitPrice,
ISNULL(D.SubTotal,0) AS SubTotal,
ISNULL(D.SD,0) AS SD,
ISNULL(D.SDAmount,0) AS SDAmount,
ISNULL(D.VATRate,0) AS VATRate,
ISNULL(D.VATAmount,0) AS VATAmount,
ISNULL(D.LineTotal,0) AS LineTotal,
ISNULL(D.UOMId, 0) AS UOMId,
ISNULL(D.UOMFromId, 0) AS UOMFromId,
ISNULL(D.UOMConversion,0) AS UOMConversion,
ISNULL(TRIM(D.Comments), '') AS DetailComments,
ISNULL(TRIM(P.Name),'') ProductName,
ISNULL(TRIM(P.BanglaName),'') BanglaName, 
ISNULL(TRIM(P.Code),'') ProductCode, 
ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(TRIM(UOM.Name),'') UOMName,
ISNULL(TRIM(UOM.Name),'') UOMFromName
 
FROM Sales M
LEFT OUTER JOIN SaleDetails D ON ISNULL(M.Id,0) = D.SaleId
LEFT OUTER JOIN Routes R on ISNULL(M.RouteId,0) = R.Id
LEFT OUTER JOIN Customers C on ISNULL(M.CustomerId,0) = C.Id
LEFT OUTER JOIN SalesPersons SP on M.SalePersonId = SP.Id
LEFT OUTER JOIN Currencies Cur ON ISNULL(M.CurrencyId,0) = Cur.Id
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
LEFT OUTER JOIN BranchProfiles branch ON M.BranchId = branch.Id

WHERE  1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }



    }

}
