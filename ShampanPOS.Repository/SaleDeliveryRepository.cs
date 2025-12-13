using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;


namespace ShampanPOS.Repository
{

    public class SaleDeliveryRepository : CommonRepository
    {
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        //Insert Method
        public async Task<ResultVM> Insert(SaleDeliveryVM saleDelivery, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO SaleDeleveries 
                (Code, BranchId, CustomerId, SalePersonId, DeliveryPersonId, DriverPersonId, RouteId, VehicleNo, 
                VehicleType, DeliveryAddress, InvoiceDateTime, DeliveryDate, GrandTotalAmount, GrandTotalSDAmount, 
                RegularDiscountRate,RegularDiscountAmount,SpecialDiscountRate,SpecialDiscountAmount,
                GrandTotalVATAmount, Comments, TransactionType, IsCompleted, CurrencyId, CurrencyRateFromBDT, 
                IsPost, CreatedBy, CreatedOn,CreatedFrom,InvoiceDiscountRate,InvoiceDiscountAmount,Processed)
                VALUES 
                (@Code, @BranchId, @CustomerId, @SalePersonId, @DeliveryPersonId, @DriverPersonId, @RouteId, @VehicleNo, 
                @VehicleType, @DeliveryAddress, @InvoiceDateTime, @DeliveryDate, @GrandTotalAmount, @GrandTotalSDAmount,
                @RegularDiscountRate,@RegularDiscountAmount,@SpecialDiscountRate,@SpecialDiscountAmount,
                @GrandTotalVATAmount, @Comments, @TransactionType, @IsCompleted, @CurrencyId, @CurrencyRateFromBDT, 
                @IsPost, @CreatedBy, GETDATE(),@CreatedFrom,@InvoiceDiscountRate,@InvoiceDiscountAmount,0);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", saleDelivery.Code);
                    cmd.Parameters.AddWithValue("@BranchId", saleDelivery.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", saleDelivery.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleDelivery.SalePersonId);
                    cmd.Parameters.AddWithValue("@DeliveryPersonId", saleDelivery.DeliveryPersonId ?? 0);
                    cmd.Parameters.AddWithValue("@DriverPersonId", saleDelivery.DriverPersonId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RouteId", saleDelivery.RouteId);
                    cmd.Parameters.AddWithValue("@VehicleNo", saleDelivery.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", saleDelivery.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleDelivery.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleDelivery.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleDelivery.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleDelivery.GrandTotalAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleDelivery.GrandTotalSDAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleDelivery.GrandTotalVATAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@RegularDiscountRate", saleDelivery.RegularDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegularDiscountAmount", saleDelivery.RegularDiscountAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@SpecialDiscountRate", saleDelivery.SpecialDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SpecialDiscountAmount", saleDelivery.SpecialDiscountAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Comments", saleDelivery.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", saleDelivery.TransactionType);
                    cmd.Parameters.AddWithValue("@IsCompleted", saleDelivery.IsCompleted);
                    cmd.Parameters.AddWithValue("@CurrencyId", saleDelivery.CurrencyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleDelivery.CurrencyRateFromBDT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", saleDelivery.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", saleDelivery.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", saleDelivery.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountRate", 0);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountAmount", 0);

                    object newId = cmd.ExecuteScalar();
                    saleDelivery.Id = Convert.ToInt32(newId);

                    //int LineNo = 1;
                    //foreach (var details in saleDelivery.saleDeliveryDetailList)
                    //{
                    //    details.SaleDeliveryId = saleDelivery.Id;
                    //    details.SaleOrderDetailId = saleDelivery.Id;
                    //    details.SaleOrderId = saleDelivery.Id;
                    //    details.BranchId = saleDelivery.BranchId;
                    //    details.Line = LineNo;
                    //    details.IsPost = saleDelivery.IsPost;

                    //    var respone = InsertDetails(details, conn, transaction);

                    //    if (respone.Id == "0")
                    //    {
                    //        throw new Exception("Error in Insert for Details Data.");
                    //    }

                    //    LineNo++;
                    //}

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = saleDelivery.Id.ToString();
                    result.DataVM = saleDelivery;
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


        // InsertDetails Method
        public async Task<ResultVM> InsertDetails(SaleDeliveryDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO SaleDeleveryDetails
(SaleDeliveryId, SaleOrderDetailId, SaleOrderId, BranchId, Line, ProductId,CtnQuantity,PcsQuantity, Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, UOMConversion, CustomerId, FreeProductId, FreeQuantity, DiscountRate, DiscountAmount, SubTotalAfterDiscount, LineDiscountRate, LineDiscountAmount, LineTotalAfterDiscount, IsPost, Comments,CampaignId,CampaignDetailsId,CampaignTypeId)
VALUES 
(@SaleDeliveryId, @SaleOrderDetailId, @SaleOrderId, @BranchId, @Line, @ProductId, @CtnQuantity,@PcsQuantity,@Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, @UOMConversion, @CustomerId, @FreeProductId, @FreeQuantity, @DiscountRate, @DiscountAmount, @SubTotalAfterDiscount, @LineDiscountRate, @LineDiscountAmount, @LineTotalAfterDiscount, @IsPost, @Comments,@CampaignId,@CampaignDetailsId,@CampaignTypeId);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleDeliveryId", details.SaleDeliveryId);
                    cmd.Parameters.AddWithValue("@SaleOrderDetailId", details.SaleOrderDetailId ?? 0);
                    cmd.Parameters.AddWithValue("@SaleOrderId", details.SaleOrderId ?? 0);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Line", details.Line ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@CtnQuantity", details.CtnQuantity);
                    cmd.Parameters.AddWithValue("@PcsQuantity", details.PcsQuantity);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", details.SD ?? 0);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount ?? 0);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount ?? 0);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal ?? 0);
                    cmd.Parameters.AddWithValue("@UOMId", details.UOMId ?? 0);
                    cmd.Parameters.AddWithValue("@UOMFromId", details.UOMFromId ?? 0);
                    cmd.Parameters.AddWithValue("@UOMConversion", details.UOMConversion ?? 0);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId ?? 0);
                    cmd.Parameters.AddWithValue("@FreeProductId", details.FreeProductId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FreeQuantity", details.FreeQuantity ?? 0);
                    cmd.Parameters.AddWithValue("@DiscountRate", details.DiscountRate ?? 0);
                    cmd.Parameters.AddWithValue("@DiscountAmount", details.DiscountAmount ?? 0);
                    cmd.Parameters.AddWithValue("@SubTotalAfterDiscount", details.SubTotalAfterDiscount ?? 0);
                    cmd.Parameters.AddWithValue("@LineDiscountRate", details.LineDiscountRate ?? 0);
                    cmd.Parameters.AddWithValue("@LineDiscountAmount", details.LineDiscountAmount ?? 0);
                    cmd.Parameters.AddWithValue("@LineTotalAfterDiscount", details.LineTotalAfterDiscount ?? 0);
                    cmd.Parameters.AddWithValue("@IsPost", details.IsPost ?? false);
                    cmd.Parameters.AddWithValue("@Comments", details.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CampaignId", details.CampaignHeaderId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CampaignDetailsId", details.CampaignDetailsId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CampaignTypeId", details.CampaignTypeId ?? (object)DBNull.Value);


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
        public async Task<ResultVM> Update(SaleDeliveryVM saleDelivery, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = saleDelivery.Id.ToString(), DataVM = saleDelivery };

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
                UPDATE SaleDeleveries 
                SET 
                    CustomerId = @CustomerId, SalePersonId = @SalePersonId, 
                    DeliveryPersonId = @DeliveryPersonId, DriverPersonId = @DriverPersonId, RouteId = @RouteId, 
                    VehicleNo = @VehicleNo, VehicleType = @VehicleType, DeliveryAddress = @DeliveryAddress, 
                    InvoiceDateTime = @InvoiceDateTime, DeliveryDate = @DeliveryDate, GrandTotalAmount = @GrandTotalAmount, 
                    RegularDiscountRate=@RegularDiscountRate,RegularDiscountAmount=@RegularDiscountAmount,SpecialDiscountRate=SpecialDiscountRate,SpecialDiscountAmount=SpecialDiscountAmount,
                    GrandTotalSDAmount = @GrandTotalSDAmount, GrandTotalVATAmount = @GrandTotalVATAmount, Comments = @Comments, 
                    CurrencyId = @CurrencyId,CurrencyRateFromBDT = @CurrencyRateFromBDT,LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn,LastUpdateFrom = @LastUpdateFrom,
                    InvoiceDiscountRate = @InvoiceDiscountRate,InvoiceDiscountAmount = @InvoiceDiscountAmount
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", saleDelivery.Id);
                    cmd.Parameters.AddWithValue("@CustomerId", saleDelivery.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleDelivery.SalePersonId);
                    cmd.Parameters.AddWithValue("@DeliveryPersonId", saleDelivery.DeliveryPersonId);
                    cmd.Parameters.AddWithValue("@DriverPersonId", saleDelivery.DriverPersonId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RouteId", saleDelivery.RouteId);
                    cmd.Parameters.AddWithValue("@VehicleNo", saleDelivery.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", saleDelivery.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleDelivery.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleDelivery.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleDelivery.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleDelivery.GrandTotalAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleDelivery.GrandTotalSDAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleDelivery.GrandTotalVATAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@RegularDiscountRate", saleDelivery.RegularDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegularDiscountAmount", saleDelivery.RegularDiscountAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@SpecialDiscountRate", saleDelivery.SpecialDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SpecialDiscountAmount", saleDelivery.SpecialDiscountAmount ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@Comments", saleDelivery.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyId", saleDelivery.CurrencyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleDelivery.CurrencyRateFromBDT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", saleDelivery.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", saleDelivery.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", saleDelivery.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountRate", saleDelivery.InvoiceDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountAmount", saleDelivery.InvoiceDiscountAmount ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
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
                result.Status = "Fail";
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

        public async Task<ResultVM> InvoiceUpdate(SaleDeliveryVM saleDelivery, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = saleDelivery.Id.ToString(), DataVM = saleDelivery };

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
                UPDATE SaleDeleveries 
                SET 
                    InvoiceDiscountRate = @InvoiceDiscountRate,InvoiceDiscountAmount = @InvoiceDiscountAmount
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", saleDelivery.Id);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountRate", saleDelivery.InvoiceDiscountRate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDiscountAmount", saleDelivery.InvoiceDiscountAmount ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
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
                result.Status = "Fail";
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
    ISNULL(M.DeliveryPersonId, 0) AS DeliveryPersonId,
    ISNULL(M.DriverPersonId, 0) AS DriverPersonId,
    ISNULL(M.RouteId, 0) AS RouteId,
    ISNULL(M.VehicleNo, '') AS VehicleNo,
    ISNULL(M.VehicleType, '') AS VehicleType,
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount,0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,

    ISNULL(M.RegularDiscountRate, 0) AS RegularDiscountRate,
    ISNULL(M.RegularDiscountAmount, 0) AS RegularDiscountAmount,

    ISNULL(M.SpecialDiscountRate, 0) AS SpecialDiscountRate,
    ISNULL(M.SpecialDiscountAmount, 0) AS SpecialDiscountAmount,

    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsCompleted, 0) AS IsCompleted,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT,0) AS CurrencyRateFromBDT,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(M.InvoiceDiscountRate,0) AS InvoiceDiscountRate,
	ISNULL(M.InvoiceDiscountAmount,0) AS InvoiceDiscountAmount
FROM 
    SaleDeleveries M
WHERE  1 = 1
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

                var model = new List<SaleDeliveryVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleDeliveryVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                        DeliveryPersonId = Convert.ToInt32(row["DeliveryPersonId"]),
                        DriverPersonId = Convert.ToInt32(row["DriverPersonId"]),
                        RouteId = Convert.ToInt32(row["RouteId"]),
                        VehicleNo = row["VehicleNo"].ToString(),
                        VehicleType = row["VehicleType"].ToString(),
                        DeliveryAddress = row["DeliveryAddress"].ToString(),
                        InvoiceDateTime = row["InvoiceDateTime"].ToString(),
                        DeliveryDate = row["DeliveryDate"].ToString(),
                        GrandTotalAmount = Convert.ToDecimal(row["GrandTotalAmount"]),
                        GrandTotalSDAmount = Convert.ToDecimal(row["GrandTotalSDAmount"]),
                        GrandTotalVATAmount = Convert.ToDecimal(row["GrandTotalVATAmount"]),

                        RegularDiscountRate = Convert.ToDecimal(row["RegularDiscountRate"]),
                        RegularDiscountAmount = Convert.ToDecimal(row["RegularDiscountAmount"]),

                        SpecialDiscountRate = Convert.ToDecimal(row["SpecialDiscountRate"]),
                        SpecialDiscountAmount = Convert.ToDecimal(row["SpecialDiscountAmount"]),

                        Comments = row["Comments"].ToString(),
                        TransactionType = row["TransactionType"].ToString(),
                        IsCompleted = Convert.ToBoolean(row["IsCompleted"]),
                        CurrencyId = Convert.ToInt32(row["CurrencyId"]),
                        CurrencyRateFromBDT = Convert.ToDecimal(row["CurrencyRateFromBDT"]),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        PostedBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"].ToString(),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString(),
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"].ToString(),
                        CreatedFrom = row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row["LastUpdateFrom"].ToString(),
                        InvoiceDiscountRate = Convert.ToDecimal(row["InvoiceDiscountRate"]),
                        InvoiceDiscountAmount = Convert.ToDecimal(row["InvoiceDiscountAmount"])
                    });
                }

                var detailsDataList = DetailsList(new[] { "D.SaleDeliveryId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDeliveryDetailVM>>(json);

                    model.FirstOrDefault().saleDeliveryDetailList = details;
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
            ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
            ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
            ISNULL(D.BranchId, 0) AS BranchId,
            ISNULL(D.Line, 0) AS Line,
            ISNULL(D.ProductId, 0) AS ProductId,
            ISNULL(D.Quantity, 0.00) AS Quantity,

            ISNULL(D.CtnQuantity, 0.00) AS CtnQuantity,

            ISNULL(D.PcsQuantity, 0.00) AS PcsQuantity,

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

            ISNULL(P.Name,'') ProductName,
            ISNULL(P.BanglaName,'') BanglaName, 
            ISNULL(P.Code,'') ProductCode, 
            ISNULL(P.HSCodeNo,'') HSCodeNo,
            ISNULL(P.ProductGroupId,0) ProductGroupId,
            ISNULL(PG.Name,'') ProductGroupName,
            ISNULL(UOM.Name,'') UOMName,
            ISNULL(UOM.Name,'') UOMFromName,
            ISNULL(D.CustomerId, 0) AS CustomerId,
            ISNULL(C.Name,'') CustomerName,
            ISNULL(D.FreeProductId, 0) AS FreeProductId,
            ISNULL(PC.Name, 0) AS FreeProductName,
            ISNULL(D.FreeQuantity, 0.00) AS FreeQuantity,
            ISNULL(D.DiscountRate, 0.00) AS DiscountRate,
            ISNULL(D.DiscountAmount, 0.00) AS DiscountAmount,
            ISNULL(D.SubTotalAfterDiscount, 0.00) AS SubTotalAfterDiscount,
            ISNULL(D.LineDiscountRate, 0.00) AS LineDiscountRate,
            ISNULL(D.LineDiscountAmount, 0.00) AS LineDiscountAmount,
            ISNULL(D.LineTotalAfterDiscount, 0.00) AS LineTotalAfterDiscount,
            ISNULL(D.CampaignId, 0) AS CampaignId,
            ISNULL(D.CampaignDetailsId, 0) AS CampaignDetailsId,
            ISNULL(D.CampaignTypeId, 0) AS CampaignTypeId

            FROM 
            SaleDeleveryDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
            LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
            LEFT OUTER JOIN Customers C on D.CustomerId = C.Id
            LEFT OUTER JOIN Products PC on D.FreeProductId = PC.Id


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

 ISNULL(M.Id,0)	Id
,ISNULL(M.Code,'') Code
,ISNULL(M.VehicleNo,'') VehicleNo
,ISNULL(M.VehicleType,'') VehicleType
,ISNULL(M.DeliveryAddress,'') DeliveryAddress
,ISNULL(M.Comments,'') Comments
,ISNULL(FORMAT(M.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
,ISNULL(FORMAT(M.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
,ISNULL(FORMAT(M.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
,ISNULL(FORMAT(M.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
,ISNULL(FORMAT(M.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate

,ISNULL(M.BranchId	,0)BranchId	
,ISNULL(M.CustomerId	,0)CustomerId	
,ISNULL(M.SalePersonId	,0)SalePersonId	
,ISNULL(M.DeliveryPersonId,0)DeliveryPersonId	
,ISNULL(M.DriverPersonId	,0)DriverPersonId	
,ISNULL(M.RouteId,0)RouteId

,ISNULL(M.CreatedBy,'')	CreatedBy
,ISNULL(M.LastModifiedBy,'')	LastModifiedBy
,ISNULL(FORMAT(M.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01')	CreatedOn
,ISNULL(FORMAT(M.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01')	LastModifiedOn

FROM SaleDeleveries M 

WHERE 1 = 1 ";


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

                string query = $" UPDATE SaleDeleveries SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE SaleDeleveryDetails SET IsPost = 1 WHERE SaleDeliveryId IN ({inClause}) ";

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
                        result.Message = $"Sale Delivery posted successfully.";
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

                var data = new GridEntity<SaleDeliveryVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
	FROM SaleDeleveries H 
	LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
	LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
	LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
	LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
	LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
    LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
	LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
	WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "");
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
        ,ISNULL(H.Id,0)	Id
        ,ISNULL(H.Code,'') Code
        ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
        ,ISNULL(H.Comments,'') Comments
        ,ISNULL(H.IsPost, 0) IsPost
        ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
        ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
        ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
        ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
        ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
        ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate
        ,ISNULL(H.GrandTotalAmount, 0) GrandTotalAmount
        ,ISNULL(H.GrandTotalSDAmount, 0) GrandTotalSDAmount
        ,ISNULL(H.GrandTotalVATAmount, 0) GrandTotalVATAmount
        ,ISNULL(Br.Name,'') BranchName
        ,ISNULL(cus.Name,'') CustomerName
        ,ISNULL(SP.Name,'') SalePersonName
        ,ISNULL(DP.Name,'') DeliveryPersonName
        ,ISNULL(rut.Name,'-') RouteName
        ,ISNULL(ET.Name,'') DriverPersonName
        ,ISNULL(con.Name,'') CurrencyName
        ,ISNULL(H.CustomerId, 0) CustomerId
        ,ISNULL(H.DeliveryPersonId, 0) DeliveryPersonId
        ,ISNULL(H.DriverPersonId, 0) DriverPersonId
        ,ISNULL(H.RouteId, 0) RouteId
        ,ISNULL(H.SalePersonId, 0) SalePersonId
        ,ISNULL(H.VehicleNo,'') VehicleNo
        ,ISNULL(H.VehicleType,'') VehicleType

        FROM SaleDeleveries H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "");
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleDeliveryVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);


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

                var data = new GridEntity<SaleDeliveryDetail>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
	FROM SaleDeleveries H 
	LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
	LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
	LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
	LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
	LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
    LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
	LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
    LEFT OUTER JOIN SaleDeleveryDetails D ON H.Id = D.SaleDeliveryId
    LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
    LEFT OUTER JOIN UOMs U ON D.UOMId = U.Id
    LEFT OUTER JOIN Campaigns CP ON D.CampaignId = CP.Id
	WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryDetail>.FilterCondition(options.filter) + ")" : "");
                // Apply additional conditions
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
    ISNULL(PD.Name, '') AS ProductName,
    ISNULL(U.Name, '') AS UOMName,
    ISNULL(CP.Name, '') AS CampaignName,
    ISNULL(PD.Name, '') AS FreeProductName,
    ISNULL(cus.Name, '') AS CustomerName,
    ISNULL(SP.Name, '') AS SalePersonName,
    ISNULL(DP.Name, '') AS DeliveryPersonName,
    ISNULL(rut.Name, '-') AS RouteName,
    ISNULL(ET.Name, '') AS DriverPersonName,
    ISNULL(con.Name, '') AS CurrencyName,
    ISNULL(H.CustomerId, 0) AS CustomerId,
    ISNULL(H.DeliveryPersonId, 0) AS DeliveryPersonId,
    ISNULL(H.DriverPersonId, 0) AS DriverPersonId,
    ISNULL(H.RouteId, 0) AS RouteId,
    ISNULL(H.SalePersonId, 0) AS SalePersonId,
    ISNULL(H.VehicleNo, '') AS VehicleNo,
    ISNULL(H.VehicleType, '') AS VehicleType,
    
    -- Sale Delivery Details
    ISNULL(D.Id, 0) AS SaleDeliveryDetailId,
    ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
    ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
    ISNULL(D.BranchId, 0) AS BranchId,
    ISNULL(D.ProductId, 0) AS ProductId,
    ISNULL(D.CampaignId, 0) AS CampaignId,
    ISNULL(D.FreeProductId, 0) AS FreeProductId,
    ISNULL(D.Quantity, 0) AS Quantity,
    ISNULL(D.FreeQuantity, 0) AS FreeQuantity,
    ISNULL(D.UnitRate, 0) AS UnitRate,
    ISNULL(D.DiscountRate, 0) AS DiscountRate,
    ISNULL(D.DiscountAmount, 0) AS DiscountAmount,
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

FROM SaleDeleveries H
LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id
LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
LEFT OUTER JOIN SaleDeleveryDetails D ON H.Id = D.SaleDeliveryId
LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
LEFT OUTER JOIN UOMs U ON D.UOMId = U.Id
LEFT OUTER JOIN Campaigns CP ON D.CampaignId = CP.Id

WHERE 1 = 1


    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryDetail>.FilterCondition(options.filter) + ")" : "");
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleDeliveryDetail>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);


                result.Status = "Success";
                result.Message = "Dtails data retrieved successfully.";
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

        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SaleOrderVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
	FROM SaleOrders H 
         LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
         LEFT OUTER JOIN Customers S ON H.CustomerId = S.Id
         LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
         LEFT OUTER JOIN Routes R ON H.RouteId = R.Id


        LEFT JOIN 
					 (
						    SELECT d.SaleOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						    FROM [dbo].[SaleOrderDetails] d   
						    GROUP BY d.SaleOrderId
					) SD ON H.Id = SD.SaleOrderId
        WHERE H.IsPost = 1 AND  ISNULL(H.IsCompleted,0) = 0 AND  (SD.TotalCompletedQty < SD.TotalQuantity)


         --LEFT JOIN 
				        --(
					        --SELECT d.SaleOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity
					        --FROM [dbo].[SaleOrderDetails] d   
					        --GROUP BY d.SaleOrderId
				        --) SD ON H.Id = SD.SaleOrderId
         --WHERE H.IsCompleted = 0 



    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        ISNULL(H.Id, '') AS Id,
        ISNULL(H.Code, '') AS Code,
         ISNULL(H.BranchId, 0) AS BranchId,
         ISNULL(H.CustomerId, 0) AS CustomerId,
         ISNULL(S.Name, 0) AS CustomerName,
         ISNULL(H.SalePersonId, 0) AS SalePersonId,
         ISNULL(SP.Name, 0) AS SalePersonName,
         ISNULL(H.RouteId, 0) AS RouteId,
         ISNULL(R.Name, 0) AS RouteName,
         ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
         ISNULL(FORMAT(H.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,

        (ISNULL(SD.TotalQuantity, 0)-ISNULL(SD.TotalCompletedQty, 0)) AS TotalQuantity,
        ISNULL(SD.TotalCompletedQty, 0) AS TotalCompletedQty,

         ISNULL(H.GrandTotalAmount, 0) AS GrandTotalAmount,
         ISNULL(H.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
         ISNULL(H.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
         ISNULL(H.Comments, '') AS Comments,
         ISNULL(H.TransactionType, '') AS TransactionType,
         --ISNULL(H.IsCompleted, 0) AS IsCompleted,
         CASE WHEN ISNULL(H.IsCompleted, 0) = 1 THEN 'Completed' ELSE 'Not-Completed' END AS Status, 
         ISNULL(H.CurrencyId, 0) AS CurrencyId,
         ISNULL(H.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
         ISNULL(H.CreatedBy, '') AS CreatedBy,
         ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
         ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
         ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
         ISNULL(H.CreatedFrom, '') AS CreatedFrom,
         ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
         ISNULL(Br.Name,'') BranchName,
         ISNULL(S.Name,'') SupplierName

         FROM SaleOrders H 
         LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
         LEFT OUTER JOIN Customers S ON H.CustomerId = S.Id
         LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
         LEFT OUTER JOIN Routes R ON H.RouteId = R.Id


         LEFT JOIN 
					    (
						    SELECT d.SaleOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						    FROM [dbo].[SaleOrderDetails] d   
						    GROUP BY d.SaleOrderId
					    ) SD ON H.Id = SD.SaleOrderId
            WHERE H.IsPost = 1 AND  ISNULL(H.IsCompleted,0) = 0 AND  (SD.TotalCompletedQty < SD.TotalQuantity)


         --LEFT JOIN 
				        --(
					        --SELECT d.SaleOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity
					        --FROM [dbo].[SaleOrderDetails] d   
					        --GROUP BY d.SaleOrderId
				        --) SD ON H.Id = SD.SaleOrderId
         --WHERE H.IsCompleted = 0 

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                //data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<SaleOrderVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);
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

        public async Task<ResultVM> UpdateGrandTotal(SaleDeliveryVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.Id.ToString(), DataVM = vm };

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
                UPDATE SaleDeleveries
                SET 
                    GrandTotalAmount = (SELECT SUM(SubTotal) FROM SaleDeleveryDetails WHERE SaleDeliveryId = @Id)-(RegularDiscountAmount+SpecialDiscountAmount),
                    RestAmount = (SELECT SUM(SubTotal) FROM SaleDeleveryDetails WHERE SaleDeliveryId = @Id)-(RegularDiscountAmount+SpecialDiscountAmount),
                    GrandTotalSDAmount = (SELECT SUM(SDAmount) FROM SaleDeleveryDetails WHERE SaleDeliveryId = @Id),
                    GrandTotalVATAmount = (SELECT SUM(VATAmount) FROM SaleDeleveryDetails WHERE SaleDeliveryId = @Id)
                WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
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
                result.Status = "Fail";
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

        public async Task<ResultVM> GetSDGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
	FROM SaleDeleveries H 
	LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
	LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
	LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
	LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
	LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
    LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
	LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
	WHERE H.IsCompleted = 0
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
        ,ISNULL(H.Id,0)	Id
        ,ISNULL(H.Code,'') Code
        ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
        ,ISNULL(H.Comments,'') Comments
        ,ISNULL(H.IsPost, 0) IsPost
        ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
        ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
        ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
        ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
        ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
        ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate
        ,ISNULL(H.GrandTotalAmount, 0) GrandTotalAmount
        ,ISNULL(H.GrandTotalSDAmount, 0) GrandTotalSDAmount
        ,ISNULL(H.GrandTotalVATAmount, 0) GrandTotalVATAmount
        ,ISNULL(Br.Name,'') BranchName
        ,ISNULL(cus.Name,'') CustomerName
        ,ISNULL(SP.Name,'') SalePersonName
        ,ISNULL(DP.Name,'') DeliveryPersonName
        ,ISNULL(rut.Name,'') RouteName
        ,ISNULL(ET.Name,'') DriverPersonName
        ,ISNULL(con.Name,'') CurrencyName

        ,ISNULL(H.CustomerId, 0) CustomerId
        ,ISNULL(H.DeliveryPersonId, 0) DeliveryPersonId
        ,ISNULL(H.DriverPersonId, 0) DriverPersonId
        ,ISNULL(H.RouteId, 0) RouteId
        ,ISNULL(H.SalePersonId, 0) SalePersonId

        FROM SaleDeleveries H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        WHERE H.IsCompleted = 0

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleDeliveryVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> SaleDeliveryList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    H.Id AS Id,
    H.Code AS Code,
    ISNULL(H.BranchId, 0) AS BranchId,
    ISNULL(H.CustomerId, 0) AS CustomerId,
    ISNULL(H.SalePersonId, 0) AS SalePersonId,
    ISNULL(H.DeliveryPersonId, 0) AS DeliveryPersonId,
    ISNULL(H.DriverPersonId, 0) AS DriverPersonId,
    ISNULL(H.RouteId, 0) AS RouteId,
    ISNULL(H.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(CONVERT(varchar, H.InvoiceDateTime, 120), '1900-01-01') AS InvoiceDateTime, -- Converted to valid date format
    ISNULL(CONVERT(varchar, H.DeliveryDate, 120), '1900-01-01') AS DeliveryDate, -- Converted to valid date format
    ISNULL(H.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(H.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(H.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(H.Comments, '') AS Comments,
    'SaleDeliveryReturn' AS TransactionType,
    ISNULL(H.IsCompleted, 0) AS IsCompleted,
    ISNULL(H.CurrencyId, 0) AS CurrencyId,
    ISNULL(H.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(H.IsPost, 0) AS IsPost,
    ISNULL(H.PostedBy, 0) AS PostedBy,
    ISNULL(CONVERT(varchar, H.PostedOn, 120), '1900-01-01') AS PostedOn, -- Converted to valid date format
    ISNULL(H.CreatedBy, 'ERP') AS CreatedBy,
    ISNULL(CONVERT(varchar, H.CreatedOn, 120), '1900-01-01') AS CreatedOn, -- Converted to valid date format
    ISNULL(H.LastModifiedBy, 'ERP') AS LastModifiedBy,
    ISNULL(CONVERT(varchar, H.LastModifiedOn, 120), '1900-01-01') AS LastModifiedOn, -- Converted to valid date format
    ISNULL(H.CreatedFrom, 'LOCAL') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, 'LOCAL') AS LastUpdateFrom,
    ISNULL(H.Id,0) AS SaleDeliveryId
FROM SaleDeleveries H
LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
LEFT OUTER JOIN Currencies CR ON H.CurrencyId = CR.Id
LEFT OUTER JOIN BranchProfiles BR ON H.BranchId = BR.Id
WHERE 1 = 1

                 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND H.Id = @Id ";
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

                var lst = new List<SaleDeliveryReturnVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleDeliveryReturnVM
                    {
                        Id = row.IsNull("Id") ? 0 : Convert.ToInt32(row["Id"]),
                        Code = row.IsNull("Code") ? string.Empty : row["Code"].ToString(),
                        BranchId = row.IsNull("BranchId") ? 0 : Convert.ToInt32(row["BranchId"]),
                        CustomerId = row.IsNull("CustomerId") ? 0 : Convert.ToInt32(row["CustomerId"]),
                        SalePersonId = row.IsNull("SalePersonId") ? 0 : Convert.ToInt32(row["SalePersonId"]),
                        DeliveryPersonId = row.IsNull("DeliveryPersonId") ? 0 : Convert.ToInt32(row["DeliveryPersonId"]),
                        DriverPersonId = row.IsNull("DriverPersonId") ? 0 : Convert.ToInt32(row["DriverPersonId"]),
                        RouteId = row.IsNull("RouteId") ? 0 : Convert.ToInt32(row["RouteId"]),
                        DeliveryAddress = row.IsNull("DeliveryAddress") ? string.Empty : row["DeliveryAddress"].ToString(),
                        InvoiceDateTime = row.IsNull("InvoiceDateTime") ? "1900-01-01" : row["InvoiceDateTime"].ToString(),
                        DeliveryDate = row.IsNull("DeliveryDate") ? "1900-01-01" : row["DeliveryDate"].ToString(),
                        GrandTotalAmount = row.IsNull("GrandTotalAmount") ? 0 : Convert.ToDecimal(row["GrandTotalAmount"]),
                        GrandTotalSDAmount = row.IsNull("GrandTotalSDAmount") ? 0 : Convert.ToDecimal(row["GrandTotalSDAmount"]),
                        GrandTotalVATAmount = row.IsNull("GrandTotalVATAmount") ? 0 : Convert.ToDecimal(row["GrandTotalVATAmount"]),
                        Comments = row.IsNull("Comments") ? string.Empty : row["Comments"].ToString(),
                        TransactionType = row.IsNull("TransactionType") ? string.Empty : row["TransactionType"].ToString(),
                        IsCompleted = row.IsNull("IsCompleted") ? false : Convert.ToBoolean(row["IsCompleted"]),
                        CurrencyId = row.IsNull("CurrencyId") ? 0 : Convert.ToInt32(row["CurrencyId"]),
                        CurrencyRateFromBDT = row.IsNull("CurrencyRateFromBDT") ? 0 : Convert.ToDecimal(row["CurrencyRateFromBDT"]),
                        IsPost = row.IsNull("IsPost") ? false : Convert.ToBoolean(row["IsPost"]),
                        PostedBy = row.IsNull("PostedBy") ? "0" : row["PostedBy"].ToString(),
                        PostedOn = row.IsNull("PostedOn") ? "1900-01-01" : row["PostedOn"].ToString(),
                        CreatedBy = row.IsNull("CreatedBy") ? "1900-01-01" : row["CreatedBy"].ToString(),
                        CreatedOn = row.IsNull("CreatedOn") ? "1900-01-01" : row["CreatedOn"].ToString(),
                        LastModifiedBy = row.IsNull("LastModifiedBy") ? string.Empty : row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row.IsNull("LastModifiedOn") ? "1900-01-01" : row["LastModifiedOn"].ToString(),
                        CreatedFrom = row.IsNull("CreatedFrom") ? string.Empty : row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row.IsNull("LastUpdateFrom") ? string.Empty : row["LastUpdateFrom"].ToString()
                    });
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

        public async Task<ResultVM> SaleDeliveryDetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                 0 AS Id,
                 ISNULL(D.SaleDeliveryId, 0) AS SaleDeliveryId,
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
                 ISNULL(P.UOMId,0) UOMId,
                 ISNULL(P.UOMId,0) UOMFromId,
                 ISNULL(FORMAT(D.UOMConversion, 'N2'), '0.00') AS UOMConversion,
                ISNULL(D.IsPost,0) IsPost,
                ISNULL(D.Comments, '') AS Comments,
                ISNULL(P.Name,'') ProductName,
                ISNULL(P.BanglaName,'') BanglaName, 
                ISNULL(P.Code,'') ProductCode, 
                ISNULL(P.HSCodeNo,'') HSCodeNo,
                ISNULL(P.ProductGroupId,0) ProductGroupId,
                ISNULL(PG.Name,'') ProductGroupName,
                ISNULL(UOM.Name,'') UOMName,
                ISNULL(UOM.Name,'') UOMFromName

                 FROM 
                 SaleDeleveryDetails D
                 LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                 LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                 LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
                 WHERE 1 = 1
                ";

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

        public async Task<ResultVM> UpdateSaleOrderIsComplete(int? Id, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = Id?.ToString() };

            try
            {
                if (Id == null)
                {
                    result.Message = "Sale Order Id cannot be null.";
                    return result;
                }

                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();  // Use async for opening connection
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
        UPDATE SaleOrders 
        SET 
            IsCompleted = 1  -- Use 1 for true, assuming it's a bit column in SQL Server
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", Id.Value);  // Safely use Value since Id is non-null here.

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();  // Use async for non-blocking operations
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
                    }
                }

                if (isNewConnection)
                {
                    await transaction.CommitAsync();  // Use async commit for non-blocking operations
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    await transaction.RollbackAsync();  // Use async rollback
                }
                result.Status = "Fail";
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    await conn.CloseAsync(); 
                }
            }
        }

        public async Task<ResultVM> UpdateSaleDeliveryIsComplete(int? saleDeliveryId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = saleDeliveryId?.ToString() };

            try
            {
                if (saleDeliveryId == null)
                {
                    result.Message = "Sale Delivery Id cannot be null.";
                    return result;
                }

                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();  // Use async for opening connection
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
        UPDATE SaleDeleveries 
        SET 
            IsCompleted = 1  -- Use 1 for true, assuming it's a bit column in SQL Server
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", saleDeliveryId.Value);  // Safely use Value since Id is non-null here.

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();  // Use async for non-blocking operations
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
                    }
                }

                if (isNewConnection)
                {
                    await transaction.CommitAsync();  // Use async commit for non-blocking operations
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    await transaction.RollbackAsync();  // Use async rollback
                }
                result.Status = "Fail";
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    await conn.CloseAsync();  // Use async close for non-blocking
                }
            }
        }

        public async Task<ResultVM> GetSaleDeliveryDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
             FROM SaleDeleveryDetails h
             LEFT OUTER JOIN Products p ON h.ProductId = p.Id
             LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
             WHERE h.SaleDeliveryId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
         ISNULL(H.Id, 0) AS Id,
         ISNULL(H.SaleDeliveryId, 0) AS SaleDeliveryId,
         ISNULL(H.ProductId, 0) AS ProductId,
         ISNULL(H.UOMId, 0) AS UOMId,
         ISNULL(H.UOMFromId, 0) AS UOMFromId,
         ISNULL(H.SaleOrderId, 0) AS SaleOrderId,
         ISNULL(P.Name, '') AS ProductName,
         ISNULL(H.Line, '') AS Line,
         ISNULL(H.Quantity, 0) AS Quantity,
         ISNULL(H.UnitRate, 0) AS UnitRate,
         ISNULL(H.SubTotal, 0) AS SubTotal,
         ISNULL(H.SD, 0) AS SD,
         ISNULL(H.SDAmount, 0) AS SDAmount,
         ISNULL(H.VATRate, 0) AS VATRate,
         ISNULL(H.VATAmount, 0) AS VATAmount,
         ISNULL(H.LineTotal, 0) AS LineTotal,
         ISNULL(u.Name, '') AS UOMName,
         ISNULL(H.UOMconversion, '') AS UOMconversion,
         ISNULL(H.Comments, '') AS Comments

         FROM SaleDeleveryDetails h
         LEFT OUTER JOIN Products p ON h.ProductId = p.Id
         LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
         WHERE h.SaleDeliveryId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<SaleDeliveryDetailVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> ProductWiseSaleDelivery(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                                FROM SaleDeleveries M
                                LEFT OUTER JOIN SaleDeleveryDetails D ON ISNULL(M.Id,0) = D.SaleDeliveryId
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

        public async Task<ResultVM> IncoiceReportPreview(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(TRIM(B.Name), '') AS BranchName,
ISNULL(TRIM(B.BanglaName), '') AS BranchBanglaName,
ISNULL(TRIM(B.TelephoneNo), '') AS TelephoneNo,
ISNULL(TRIM(B.Code), '') AS BranchCode,
ISNULL(TRIM(B.Address), '') AS BranchAddress,
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
ISNULL(M.IsCompleted, 0) AS IsCompleted,
ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
ISNULL(D.UOMConversion,0) AS UOMConversion,
ISNULL(TRIM(D.Comments), '') AS DetailComments,
ISNULL(TRIM(P.Name),'') ProductName,
ISNULL(TRIM(P.BanglaName),'') BanglaName, 
ISNULL(TRIM(P.Code),'') ProductCode, 
ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,

ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0) AS Quantity,
ISNULL(D.CtnQuantity, 0) AS CtnQuantity,
ISNULL(D.PcsQuantity, 0) AS PcsQuantity,
ISNULL(D.UnitRate, 0) AS UnitPrice,
ISNULL(D.SubTotal,0) AS SubTotal,
ISNULL(D.SD,0) AS SD,
ISNULL(D.SDAmount,0) AS SDAmount,
ISNULL(D.VATRate,0) AS VATRate,
ISNULL(D.VATAmount,0) AS VATAmount,
ISNULL(D.LineTotal,0) AS LineTotal,
ISNULL(M.SpecialDiscountRate, 0) AS SpecialDiscountRate,
ISNULL(M.SpecialDiscountAmount, 0) AS SpecialDiscountAmount,
ISNULL(M.RegularDiscountRate, 0) AS RegularDiscountRate,
ISNULL(M.RegularDiscountAmount, 0) AS RegularDiscountAmount,
ISNULL(D.DiscountRate, 0) AS DiscountRate,
ISNULL(D.DiscountAmount, 0) AS DiscountAmount



 
FROM SaleDeleveryDetails D 

LEFT OUTER JOIN SaleDeleveries M ON ISNULL(M.Id,0) = D.SaleDeliveryId
LEFT OUTER JOIN Routes R on ISNULL(M.RouteId,0) = R.Id
LEFT OUTER JOIN Customers C on ISNULL(M.CustomerId,0) = C.Id
LEFT OUTER JOIN SalesPersons SP on M.SalePersonId = SP.Id
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN BranchProfiles B ON M.BranchId = B.Id

WHERE  1 = 1 ";

           

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.SaleDeliveryId = @Id ";
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
ISNULL(SP.Address, '') AS SalePersonAddress,
ISNULL(M.DeliveryPersonId, 0) AS DeliveryPersonId,
ISNULL(DP.Name, '') AS DeliveryPersonName,
ISNULL(M.DriverPersonId, 0) AS DriverPersonId,
ISNULL(enum.Name, '') AS DriverPersonName,
ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
ISNULL(M.Comments, '') AS Comments,
ISNULL(M.TransactionType, '') AS TransactionType,
ISNULL(M.IsCompleted, 0) AS IsCompleted,
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
 
FROM SaleDeleveries M
LEFT OUTER JOIN SaleDeleveryDetails D ON ISNULL(M.Id,0) = D.SaleDeliveryId
LEFT OUTER JOIN Routes R on ISNULL(M.RouteId,0) = R.Id
LEFT OUTER JOIN Customers C on ISNULL(M.CustomerId,0) = C.Id
LEFT OUTER JOIN SalesPersons SP on M.SalePersonId = SP.Id
LEFT OUTER JOIN DeliveryPersons DP on M.DeliveryPersonId = DP.Id
LEFT OUTER JOIN EnumTypes enum on M.DriverPersonId = enum.Id
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

        public async Task<ResultVM> GetDeliveryNoWiseGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryVM>();

                // Base SQL Query (Count Query)
                string sqlQuery = @"
                -- Count query
                SELECT COUNT(DISTINCT d.Id) AS totalcount
                FROM SaleOrderDetails d
                LEFT JOIN SaleOrders h ON d.SaleOrderId = h.Id
                LEFT JOIN BranchProfiles Br ON h.BranchId = Br.Id
                LEFT JOIN SalesPersons SP ON h.SalePersonId = SP.Id
                LEFT JOIN Customers C ON h.CustomerId = C.Id
                LEFT JOIN Routes r ON h.RouteId = r.Id
                LEFT JOIN Products P ON d.ProductId = P.Id
                LEFT JOIN (
                    SELECT DISTINCT SaleOrderDetailId, SaleOrderId, h.Code AS DeliveryCode, ProductId, 
                           SUM(Quantity) AS DeliveryQuantity
                    FROM SaleDeleveryDetails d
                    LEFT JOIN SaleDeleveries h ON d.SaleDeliveryId = h.Id
                    GROUP BY SaleOrderDetailId, SaleOrderId, h.Code, ProductId
                ) del ON d.Id = del.SaleOrderDetailId 
                      AND h.Id = del.SaleOrderId 
                      AND d.ProductId = del.ProductId
                WHERE h.IsCompleted = 1 ";

                // Apply Filter Conditions
                if (options.filter.Filters.Count > 0)
                {
                    sqlQuery += " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")";
                }

                // Apply Additional Conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                // Data Query with Pagination & Sorting
                sqlQuery += @"
                ;WITH OrderedData AS (
                    SELECT 
                        ROW_NUMBER() OVER (ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "d.Id DESC") + @") AS rowindex,
                        d.Id,
                        h.Code AS Code, 
                        h.BranchId, 
                        br.Name AS BranchName,
                        r.Name AS RouteName,
                        ISNULL(FORMAT(h.InvoiceDateTime,'yyyy-MM-dd HH:mm'),'1900-01-01') InvoiceDateTime,                
                        p.Name AS ProductName,
                        d.Quantity AS Quantity,
                        ISNULL(del.DeliveryCode,'-') AS DeliveryCode,
                        ISNULL(del.DeliveryQuantity,0) AS DeliveryQuantity
                    FROM SaleOrderDetails d
                    LEFT JOIN SaleOrders h ON d.SaleOrderId = h.Id
                    LEFT JOIN BranchProfiles Br ON h.BranchId = Br.Id
                    LEFT JOIN SalesPersons SP ON h.SalePersonId = SP.Id
                    LEFT JOIN Customers C ON h.CustomerId = C.Id
                    LEFT JOIN Routes r ON h.RouteId = r.Id
                    LEFT JOIN Products P ON d.ProductId = P.Id
                    LEFT JOIN (
                        SELECT DISTINCT SaleOrderDetailId, SaleOrderId, h.Code AS DeliveryCode, ProductId, 
                               SUM(Quantity) AS DeliveryQuantity
                        FROM SaleDeleveryDetails d
                        LEFT JOIN SaleDeleveries h ON d.SaleDeliveryId = h.Id
                        GROUP BY SaleOrderDetailId, SaleOrderId, h.Code, ProductId
                    ) del ON d.Id = del.SaleOrderDetailId 
                          AND h.Id = del.SaleOrderId 
                          AND d.ProductId = del.ProductId
                    WHERE h.IsCompleted = 1 ";

                // Apply Filter Conditions
                if (options.filter.Filters.Count > 0)
                {
                    sqlQuery += " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")";
                }

                // Apply Additional Conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
                )
                SELECT * FROM OrderedData
                WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take);";

                // Retrieve Data
                data = KendoGrid<SaleDeliveryVM>.GetTransactionalGridData_CMD(options, sqlQuery, "d.Id", conditionalFields, conditionalValues);

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



        // UpdateLineItem Method
        public async Task<ResultVM> UpdateLineItem(SaleDeliveryDetailVM detail, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = detail.Id.ToString(), DataVM = detail };

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

                string query = $@"
                UPDATE SaleOrderDetails SET CompletedQty = (SELECT ISNULL(CompletedQty,0) + CAST(@Quantity AS DECIMAL(10, 2)) FROM SaleOrderDetails WHERE Id = @Id )  
                WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    //cmd.Parameters.AddWithValue("@Id", detail.Id != null ? detail.Id : 0);
                    cmd.Parameters.AddWithValue("@Id", detail.SaleOrderDetailId != null ? detail.SaleOrderDetailId : 0);
                    cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
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
                result.Status = "Fail";
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

        public async Task<ResultVM> ListGetSaleDeleveryByCustomerAndBranch(int CustomerId, int branchId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                // If no connection is passed, create a new one
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                // SQL query to get the customers based on SalePersonId and BranchId
                string query = @"
            SELECT M.Id,M.DeliveryId M.DeliveryAddress, sp.BranchId,M.Code
            FROM SaleDeleveries M
            LEFT OUTER JOIN Customers sp ON sp.Id = M.CustomerId
            WHERE sp.BranchId = @BranchId
            AND sp.CustomerId = @CustomerId
        ";

                // Create the SQL command and set parameters
                SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                cmd.Parameters.AddWithValue("@BranchId", branchId);

                // Create a data adapter to fill the DataTable
                SqlDataAdapter objComm = new SqlDataAdapter(cmd);
                objComm.Fill(dataTable);

                // Convert the DataTable rows to a list of CustomerDataVM objects
                var modelList = dataTable.AsEnumerable().Select(row => new SaleDeliveryVM
                {
                    Id = row.Field<int>("Id"),
                    DeliveryPersonId = row.Field<int>("DeliveryId"),
                    Code = row.Field<string>("Code"),
                    

                }).ToList();

                // If there are any customers, set the status to Success
                if (modelList.Any())
                {
                    result.Status = "Success";
                    result.Message = "Data retrieved successfully.";
                    result.DataVM = modelList;
                }
                else
                {
                    result.Status = "Fail";
                    result.Message = "No customers found for the given SalePersonId and BranchId.";
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception and return the error message
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                // Close the connection if it was opened in this method
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> GetSaleDueListByCustomerId(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
	FROM SaleDeleveries H 
	LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
	LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
	LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
	LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
	LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
    LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
	LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
	WHERE isnull(H.Processed,0 )= 0
    and h.IsPost='1'
";
                if( !string.IsNullOrEmpty( options.vm.Id) && options.vm.Id!="0")
                {
                    sqlQuery += " AND H.CustomerId = " + options.vm.Id + " ";
                }

                sqlQuery += @"
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
        ,ISNULL(H.Id,0)	Id
        ,ISNULL(H.Code,'') Code
        ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
        ,ISNULL(H.Comments,'') Comments
        ,ISNULL(H.IsPost, 0) IsPost
        ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
        ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
        ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
        ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
        ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
        ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate
        ,ISNULL(H.GrandTotalAmount, 0) GrandTotalAmount
        ,ISNULL(H.GrandTotalSDAmount, 0) GrandTotalSDAmount
        ,ISNULL(H.GrandTotalVATAmount, 0) GrandTotalVATAmount
        ,ISNULL(Br.Name,'') BranchName
        ,ISNULL(cus.Name,'') CustomerName
        ,ISNULL(SP.Name,'') SalePersonName
        ,ISNULL(DP.Name,'') DeliveryPersonName
        ,ISNULL(rut.Name,'') RouteName
        ,ISNULL(ET.Name,'') DriverPersonName
        ,ISNULL(con.Name,'') CurrencyName

        ,ISNULL(H.CustomerId, 0) CustomerId
        ,ISNULL(H.DeliveryPersonId, 0) DeliveryPersonId
        ,ISNULL(H.DriverPersonId, 0) DriverPersonId
        ,ISNULL(H.RouteId, 0) RouteId
        ,ISNULL(H.SalePersonId, 0) SalePersonId
		,ISNULL(H.RestAmount,0) RestAmount

        FROM SaleDeleveries H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        WHERE isnull(H.Processed,0 )= 0
        and h.IsPost='1'
";

 if( !string.IsNullOrEmpty( options.vm.Id) && options.vm.Id!="0")
                {
                    sqlQuery += " AND H.CustomerId = " + options.vm.Id + " ";

                }

                sqlQuery += @"
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleDeliveryVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
    }

}
