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

    public class SaleDeliveryReturnRepository : CommonRepository
    {
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        //Insert Method
         public async Task<ResultVM> Insert(SaleDeliveryReturnVM saleDelivery, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO SaleDeleveryReturns 
                (Code, BranchId, CustomerId, SalePersonId, DeliveryPersonId, DriverPersonId, RouteId,  
                 DeliveryAddress, InvoiceDateTime, DeliveryDate, GrandTotalAmount, GrandTotalSDAmount, 
                GrandTotalVATAmount, Comments, TransactionType, IsCompleted, CurrencyId, CurrencyRateFromBDT, 
                IsPost, CreatedBy, CreatedOn,CreatedFrom)
                VALUES 
                (@Code, @BranchId, @CustomerId, @SalePersonId, @DeliveryPersonId, @DriverPersonId, @RouteId, 
                @DeliveryAddress, @InvoiceDateTime, @DeliveryDate, @GrandTotalAmount, @GrandTotalSDAmount, 
                @GrandTotalVATAmount, @Comments, @TransactionType, @IsCompleted, @CurrencyId, @CurrencyRateFromBDT, 
                @IsPost, @CreatedBy, GETDATE(),@CreatedFrom);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", saleDelivery.Code);
                    cmd.Parameters.AddWithValue("@BranchId", saleDelivery.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", saleDelivery.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleDelivery.SalePersonId);
                    cmd.Parameters.AddWithValue("@DeliveryPersonId", saleDelivery.DeliveryPersonId);
                    cmd.Parameters.AddWithValue("@DriverPersonId", saleDelivery.DriverPersonId);
                    cmd.Parameters.AddWithValue("@RouteId", saleDelivery.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleDelivery.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleDelivery.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleDelivery.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleDelivery.GrandTotalAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleDelivery.GrandTotalSDAmount ?? 0);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleDelivery.GrandTotalVATAmount ?? 0);
                    cmd.Parameters.AddWithValue("@Comments", saleDelivery.Comments ?? "");
                    cmd.Parameters.AddWithValue("@TransactionType", saleDelivery.TransactionType);
                    cmd.Parameters.AddWithValue("@IsCompleted", saleDelivery.IsCompleted);
                    cmd.Parameters.AddWithValue("@CurrencyId", saleDelivery.CurrencyId);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleDelivery.CurrencyRateFromBDT);
                    cmd.Parameters.AddWithValue("@IsPost", saleDelivery.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", saleDelivery.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", saleDelivery.CreatedFrom ?? (object)DBNull.Value);
                    object newId = cmd.ExecuteScalar();
                    saleDelivery.Id = Convert.ToInt32(newId);

                    //int LineNo = 1;
                    //foreach (var details in saleDelivery.saleDeliveryReturnDetailList)
                    //{
                    //    details.SaleDeliveryReturnId = saleDelivery.Id;
                    //    details.SaleDeliveryId = saleDelivery.Id;
                    //    details.SaleDeliveryDetailId = saleDelivery.Id;
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
                    result.Id = newId.ToString();
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
        public async Task<ResultVM> InsertDetails(SaleDeliveryReturnDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO SaleDeleveryReturnDetails
        (SaleDeliveryReturnId, SaleDeliveryId, SaleDeliveryDetailId, BranchId, Line, ProductId,Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount,LineTotal,UOMId,UOMFromId,UOMConversion,IsPost,Comments,ReasonOfReturn)
        VALUES 
        (@SaleDeliveryReturnId, @SaleDeliveryId, @SaleDeliveryDetailId, @BranchId, @Line, @ProductId,@Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount,@LineTotal, @UOMId,@UOMFromId, @UOMConversion, @IsPost, @Comments,@ReasonOfReturn);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleDeliveryReturnId", details.SaleDeliveryReturnId ?? 0);
                    cmd.Parameters.AddWithValue("@SaleDeliveryId", details.SaleDeliveryId ?? 0);
                    cmd.Parameters.AddWithValue("@SaleDeliveryDetailId", details.SaleDeliveryDetailId ?? 0);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Line", details.Line ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
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
                    cmd.Parameters.AddWithValue("@IsPost", details.IsPost ?? false);
                    cmd.Parameters.AddWithValue("@Comments", details.Comments ?? "");
                    cmd.Parameters.AddWithValue("@ReasonOfReturn", details.ReasonOfReturn ?? "");

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
                result.Status = "Fail";
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        // Update Method
        public async Task<ResultVM> Update(SaleDeliveryReturnVM saleDelivery, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE SaleDeleveryReturns 
                SET 
                    CustomerId = @CustomerId, SalePersonId = @SalePersonId, 
                    DeliveryPersonId = @DeliveryPersonId, DriverPersonId = @DriverPersonId, RouteId = @RouteId, 
                    DeliveryAddress = @DeliveryAddress,InvoiceDateTime = @InvoiceDateTime, DeliveryDate = @DeliveryDate, GrandTotalAmount = @GrandTotalAmount, 
                    GrandTotalSDAmount = @GrandTotalSDAmount, GrandTotalVATAmount = @GrandTotalVATAmount, Comments = @Comments, 
                    CurrencyId = @CurrencyId,CurrencyRateFromBDT = @CurrencyRateFromBDT,LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn,LastUpdateFrom = @LastUpdateFrom
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", saleDelivery.Id);
                    cmd.Parameters.AddWithValue("@CustomerId", saleDelivery.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleDelivery.SalePersonId);
                    cmd.Parameters.AddWithValue("@DeliveryPersonId", saleDelivery.DeliveryPersonId);
                    cmd.Parameters.AddWithValue("@DriverPersonId", saleDelivery.DriverPersonId);
                    cmd.Parameters.AddWithValue("@RouteId", saleDelivery.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleDelivery.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleDelivery.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleDelivery.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleDelivery.GrandTotalAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleDelivery.GrandTotalSDAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleDelivery.GrandTotalVATAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", saleDelivery.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyId", saleDelivery.CurrencyId);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleDelivery.CurrencyRateFromBDT);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", saleDelivery.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", saleDelivery.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", saleDelivery.LastUpdateFrom ?? (object)DBNull.Value);

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
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount,0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
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
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    SaleDeleveryReturns M
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

                var model = new List<SaleDeliveryReturnVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleDeliveryReturnVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                        DeliveryPersonId = Convert.ToInt32(row["DeliveryPersonId"]),
                        DriverPersonId = Convert.ToInt32(row["DriverPersonId"]),
                        RouteId = Convert.ToInt32(row["RouteId"]),
                        DeliveryAddress = row["DeliveryAddress"].ToString(),
                        InvoiceDateTime = row["InvoiceDateTime"].ToString(),
                        DeliveryDate = row["DeliveryDate"].ToString(),
                        GrandTotalAmount = Convert.ToDecimal(row["GrandTotalAmount"]),
                        GrandTotalSDAmount = Convert.ToDecimal(row["GrandTotalSDAmount"]),
                        GrandTotalVATAmount = Convert.ToDecimal(row["GrandTotalVATAmount"]),
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
                        LastUpdateFrom = row["LastUpdateFrom"].ToString()
                    });
                }

                var detailsDataList = DetailsList(new[] { "D.SaleDeliveryReturnId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDeliveryReturnDetailVM>>(json);

                    model.FirstOrDefault().saleDeliveryReturnDetailList = details;
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

        public  ResultVM DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(D.SaleDeliveryReturnId, 0) AS SaleDeliveryReturnId,
ISNULL(D.SaleDeliveryId, 0) AS SaleDeliveryId,
ISNULL(D.SaleDeliveryDetailId, 0) AS SaleDeliveryDetailId,
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
ISNULL(D.ReasonOfReturn, '') AS ReasonOfReturn,

ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(UOM.Name,'') UOMName,
ISNULL(UOM.Name,'') UOMFromName

FROM 
SaleDeleveryReturnDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id

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

FROM SaleDeleveryReturns M 

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
                FROM SaleDeleveryReturns
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

                string query = $" UPDATE SaleDeleveryReturns SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE SaleDeleveryReturnDetails SET IsPost = 1 WHERE SaleDeliveryReturnId IN ({inClause}) ";

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
                        result.Message = $"Data posted successfully.";
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

                var data = new GridEntity<SaleDeliveryReturnVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM SaleDeleveryReturns H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex ,
        
         ISNULL(H.Id,0)	Id
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

        FROM SaleDeleveryReturns H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleDeliveryReturnVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryReturnDetails>();

                // Define your SQL query string

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM SaleDeleveryReturns H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        LEFT OUTER JOIN SaleDeleveryReturnDetails D ON H.Id = D.SaleDeliveryReturnId
        LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
        LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
        WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnDetails>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex ,
        
         ISNULL(H.Id,0)	Id
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
        ,ISNULL(PD.Name,'') ProductName
        ,ISNULL(UM.Name,'') UOMName
        ,ISNULL(cus.Name,'') CustomerName
        ,ISNULL(SP.Name,'') SalePersonName
        ,ISNULL(DP.Name,'') DeliveryPersonName
        ,ISNULL(rut.Name,'') RouteName
        ,ISNULL(ET.Name,'') DriverPersonName
        ,ISNULL(con.Name,'') CurrencyName
        ,ISNULL(H.TransactionType, 0) TransactionType
        ,ISNULL(H.CustomerId, 0) CustomerId
        ,ISNULL(H.DeliveryPersonId, 0) DeliveryPersonId
        ,ISNULL(H.DriverPersonId, 0) DriverPersonId
        ,ISNULL(H.RouteId, 0) RouteId
        ,ISNULL(H.SalePersonId, 0) SalePersonId




     -- Detail Information
     ,ISNULL(D.Quantity, 0) AS Quantity
     ,ISNULL(D.UnitRate, 0) AS UnitRate
     ,ISNULL(D.SubTotal, 0) AS SubTotal
     ,ISNULL(D.SD, 0) AS SD
     ,ISNULL(D.SDAmount, 0) AS SDAmount
     ,ISNULL(D.VATRate, 0) AS VATRate
     ,ISNULL(D.VATAmount, 0) AS VATAmount
     ,ISNULL(D.LineTotal, 0) AS LineTotal
     ,ISNULL(D.UOMId, 0) AS UOMId
     ,ISNULL(D.UOMFromId, 0) AS UOMFromId
     ,ISNULL(D.UOMConversion, 0) AS UOMConversion
     ,ISNULL(D.Comments, '') AS DetailComments
     ,ISNULL(D.ReasonOfReturn, '') AS ReasonOfReturn

        FROM SaleDeleveryReturns H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        LEFT OUTER JOIN DeliveryPersons DP ON H.DeliveryPersonId = DP.Id
        LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
        LEFT OUTER JOIN Currencies con ON H.CurrencyId = con.Id  
        LEFT OUTER JOIN EnumTypes ET ON H.DriverPersonId = ET.Id
        LEFT OUTER JOIN SaleDeleveryReturnDetails D ON H.Id = D.SaleDeliveryReturnId
        LEFT OUTER JOIN Products pd ON D.ProductId = pd.Id
        LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
        WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnDetails>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";





                data = KendoGrid<SaleDeliveryReturnDetails>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> FromSaleDeliveryGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
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

        FROM SaleDeleveryReturns H 
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
        public async Task<ResultVM> UpdateGrandTotal(int? deliveryReturnId, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = deliveryReturnId.ToString(), DataVM = deliveryReturnId };

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
                UPDATE SaleDeleveryReturns
                SET 
                    GrandTotalAmount = COALESCE((SELECT SUM(SubTotal) FROM SaleDeleveryReturnDetails WHERE SaleDeliveryReturnId = @Id), 0),
                    GrandTotalSDAmount = COALESCE((SELECT SUM(SDAmount) FROM SaleDeleveryReturnDetails WHERE SaleDeliveryReturnId = @Id), 0),
                    GrandTotalVATAmount = COALESCE((SELECT SUM(VATAmount) FROM SaleDeleveryReturnDetails WHERE SaleDeliveryReturnId = @Id), 0)
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", deliveryReturnId);

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

        public async Task<ResultVM> GetSaleDeliveryReturnDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryReturnDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
             FROM SaleDeleveryReturnDetails h
        LEFT OUTER JOIN Products p ON h.ProductId = p.Id
        LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
        WHERE h.SaleDeliveryReturnId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

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
        ISNULL(H.SaleDeliveryReturnId, 0) AS SaleDeliveryReturnId,
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

        FROM SaleDeleveryReturnDetails h
        LEFT OUTER JOIN Products p ON h.ProductId = p.Id
        LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
        WHERE h.SaleDeliveryReturnId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryReturnDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<SaleDeliveryReturnDetailVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
        public async Task<ResultVM> ProductWiseSaleDeliveryReturn(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                                FROM SaleDeleveryReturns M
                                LEFT OUTER JOIN SaleDeleveryReturnDetails D ON ISNULL(M.Id,0) = D.SaleDeliveryReturnId
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

                var lst = new List<SaleDevliveryReturnReportVM>();
                int serialNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleDevliveryReturnReportVM
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
 
FROM SaleDeleveryReturns M
LEFT OUTER JOIN SaleDeleveryReturnDetails D ON ISNULL(M.Id,0) = D.SaleDeliveryReturnId
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



    }

}
