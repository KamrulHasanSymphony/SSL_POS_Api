using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class PurchaseReturnRepository : CommonRepository
    {
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        //Insert Method
        public async Task<ResultVM> Insert(PurchaseReturnVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    INSERT INTO PurchasesReturn
                    (
                        Code, BranchId,CompanyId, SupplierId, BENumber, PurchaseDate, InvoiceDateTime,Comments, 
                        TransactionType, IsPost, FiscalYear, PeriodId, CreatedBy, CreatedOn,CreatedFrom
                    )
                    VALUES 
                    (
                        @Code, @BranchId,@CompanyId, @SupplierId, @BENumber, @PurchaseDate,@InvoiceDateTime,@Comments, 
                        @TransactionType, @IsPost,@FiscalYear, @PeriodId, @CreatedBy, @CreatedOn,@CreatedFrom
                    );
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                    cmd.Parameters.AddWithValue("@BENumber", vm.BENumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PurchaseDate", vm.PurchaseDate);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", vm.InvoiceDateTime);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? "");
                    cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int)
                                 .Value = (object?)vm.CompanyId ?? DBNull.Value;

                    object newId = cmd.ExecuteScalar();
                    vm.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = vm;
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
        public async Task<ResultVM> InsertDetails(PurchaseReturnDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO PurchaseReturnDetails
        (PurchasesReturnId, Line,CompanyId, ProductId,Quantity, UnitPrice, SubTotal,IsPost, SD, SDAmount, VATRate, VATAmount,LineTotal,OthersAmount)
        VALUES 
        (@PurchasesReturnId, @Line,@CompanyId, @ProductId,@Quantity, @UnitPrice, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount,@LineTotal,@IsPost, @OthersAmount);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                                   
                   
                    cmd.Parameters.AddWithValue("@PurchasesReturnId", details.PurchaseReturnId);
                    cmd.Parameters.AddWithValue("@Line", details.Line);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", details.UnitPrice);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);
                    cmd.Parameters.AddWithValue("@OthersAmount", details.OthersAmount);
                    cmd.Parameters.AddWithValue("@SD", details.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal);
                    cmd.Parameters.AddWithValue("@IsPost", details.IsPost);
                    cmd.Parameters.AddWithValue("@CompanyId", details.CompanyId ?? 0);


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
        public async Task<ResultVM> Update(PurchaseReturnVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE PurchasesReturn
                SET 
                    SupplierId = @SupplierId,CompanyId = @CompanyId, BENumber = @BENumber,InvoiceDateTime=@InvoiceDateTime, 
                    PurchaseDate = @PurchaseDate, Comments = @Comments,
                    FiscalYear = @FiscalYear, PeriodId = @PeriodId, LastModifiedBy = @LastModifiedBy, LastUpdateFrom = @LastUpdateFrom, LastModifiedOn = GETDATE()
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? 0);
                    cmd.Parameters.AddWithValue("@BENumber", vm.BENumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PurchaseDate", vm.PurchaseDate);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", vm.InvoiceDateTime);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FiscalYear", vm.FiscalYear ?? "");
                    cmd.Parameters.AddWithValue("@PeriodId", vm.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

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

        // UpdateGrandTotal Method
        public async Task<ResultVM> UpdateGrandTotal(PurchaseReturnVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE PurchasesReturn
                SET 
                    GrandTotalAmount = (SELECT SUM(SubTotal) FROM PurchaseReturnDetails WHERE PurchaseReturnId = @Id),
                    GrandTotalSDAmount = (SELECT SUM(SDAmount) FROM PurchaseReturnDetails WHERE PurchaseReturnId = @Id),
                    GrandTotalVATAmount = (SELECT SUM(VATAmount) FROM PurchaseReturnDetails WHERE PurchaseReturnId = @Id)
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

        // UpdateLineItem Method


        public async Task<ResultVM> UpdateLineItem(PurchaseReturnDetailVM detail, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE PurchaseDetails SET CompletedQty = (SELECT ISNULL(CompletedQty,0) + CAST(@Quantity AS DECIMAL(10, 2)) FROM PurchaseDetails WHERE Id = @Id )  
                WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", detail.PurchaseDetailId != null ? detail.PurchaseDetailId : 0);
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


        // GetLineItemCompletedQty Method
        public async Task<ResultVM> GetLineItemCompletedQty(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
CASE WHEN SUM(D.Quantity) = SUM(D.CompletedQty) THEN 'True' ELSE 'False' END Status
FROM PurchaseDetails D
WHERE D.PurchaseId = @Id
GROUP BY D.PurchaseId
 ";


                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                        adapter.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
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

        // UpdateIsCompleted Method
        public async Task<ResultVM> UpdateIsCompleted(PurchaseReturnVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @" UPDATE Purchases SET IsCompleted = 1 WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.PurchaseId);

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
	ISNULL(M.CompanyId, 0) AS CompanyId,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(M.BENumber, '') AS BENumber,
    ISNULL(FORMAT(M.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.FiscalYear, '') AS FiscalYear,
    ISNULL(M.PeriodId, '') AS PeriodId,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(S.Name, '') AS SupplierName,
	ISNULL(Br.Name,'') BranchName,
    ISNULL(CP.CompanyName,'') CompanyName
FROM 
    PurchasesReturn M
    LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
    LEFT OUTER JOIN BranchProfiles Br ON M.BranchId = Br.Id
	LEFT OUTER JOIN CompanyProfiles CP ON M.CompanyId = CP.Id
WHERE 1 = 1
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

                var lst = new List<PurchaseReturnVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new PurchaseReturnVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        BranchId = row.Field<int>("BranchId"),
                        BranchName = row.Field<string>("BranchName"),
                        CompanyId = row.Field<int>("CompanyId"),
                        SupplierId = row.Field<int>("SupplierId"),
                        BENumber = row.Field<string>("BENumber"),
                        PurchaseDate = row.Field<string>("PurchaseDate"),
                        Comments = row.Field<string>("Comments"),
                        TransactionType = row.Field<string>("TransactionType"),
                        IsPost = row.Field<bool>("IsPost"),
                        PostedBy = row.Field<string>("PostedBy"),
                        PostedOn = row.Field<string?>("PostedOn"),
                        FiscalYear = row.Field<string>("FiscalYear"),
                        PeriodId = row.Field<string>("PeriodId"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                        LastUpdateFrom = row.Field<string?>("LastUpdateFrom")
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

        public async Task<ResultVM> DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(D.PurchasesReturnId, 0) AS PurchasesReturnId,
    ISNULL(D.Line, 0) AS Line,
    ISNULL(D.ProductId, 0) AS ProductId,
    ISNULL(D.Quantity, 0.00) AS Quantity,
    ISNULL(FORMAT(D.UnitPrice, 'N2'), '0.00') AS UnitPrice,
    ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
    ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
    ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
    ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
    ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
    ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
    ISNULL(FORMAT(D.OthersAmount, 'N2'), '0.00') AS OthersAmount,
    ISNULL(CAST(D.IsPost AS BIT), 0) AS IsPost, 

    ISNULL(P.Name, '') AS ProductName,
    ISNULL(P.BanglaName, '') AS BanglaName,
    ISNULL(P.Code, '') AS ProductCode,
    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
    ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
    ISNULL(PG.Name, '') AS ProductGroupName,
    ISNULL(CP.CompanyName,'') CompanyName


FROM 
    PurchaseReturnDetails D
    LEFT OUTER JOIN Products P ON D.ProductId = P.Id
    LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
 LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

WHERE 
    1 = 1 ";

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
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(M.BENumber, '') AS BENumber,
    ISNULL(FORMAT(M.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
    ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(M.ImportIDExcel, '') AS ImportIDExcel,
    ISNULL(M.FileName, '') AS FileName,
    ISNULL(M.FiscalYear, '') AS FiscalYear,
    ISNULL(M.PeriodId, '') AS PeriodId,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    PurchasesReturn M

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
                FROM PurchaseReturns
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

                string query = $" UPDATE PurchasesReturn SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                //query += $" UPDATE PurchaseReturnDetails SET IsPost = 1 WHERE PurchaseReturnId IN ({inClause}) ";

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

        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<PurchaseReturnVM>();

                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount

                    FROM PurchasesReturn H 
                    LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                    LEFT OUTER JOIN Suppliers S ON H.SupplierId = S.Id
				    LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
                    WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + $@") AS rowindex,
        
                    ISNULL(H.Id, 0) AS Id,
                    ISNULL(H.Code, '') AS Code,
                    ISNULL(H.BranchId, 0) AS BranchId,
                    ISNULL(H.CompanyId, 0) AS CompanyId,
                    ISNULL(H.SupplierId, 0) AS SupplierId,
                    ISNULL(H.BENumber, '') AS BENumber,
                    ISNULL(FORMAT(H.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
                    ISNULL(H.Comments, '') AS Comments,
                    ISNULL(H.TransactionType, '') AS TransactionType,
                    ISNULL(H.IsPost, 0) AS IsPost,
                    CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                    ISNULL(H.PostedBy, '') AS PostedBy,
                    ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
                    ISNULL(H.FiscalYear, '') AS FiscalYear,
                    ISNULL(H.PeriodId, '') AS PeriodId,
                    ISNULL(H.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
                    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
                    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,

                    ISNULL(Br.Name,'') BranchName,
                    ISNULL(Br.Address, '') AS BranchAddress,
                    ISNULL(S.Name,'') SupplierName,
                    ISNULL(S.Address, '') AS SupplierAddress,
				    ISNULL(CP.CompanyName,'') CompanyName


                    FROM PurchasesReturn H 
                    LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                    LEFT OUTER JOIN Suppliers S ON H.SupplierId = S.Id
				    LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
                    WHERE 1 = 1

                -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PurchaseReturnVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<PurchaseReturnDetailVM>();

                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM PurchasesReturn H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Suppliers S ON H.SupplierId = S.Id
        LEFT OUTER JOIN PurchaseReturnDetails D ON H.Id = D.PurchasesReturnId            
        LEFT OUTER JOIN Products P ON D.ProductId = P.Id
		LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

        WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + $@") AS rowindex,
        
	    ISNULL(H.Id, 0) AS Id,
        ISNULL(H.Code, '') AS PurchasesReturnCode,
        ISNULL(H.BranchId, 0) AS BranchId,
		ISNULL(H.CompanyId, 0) AS CompanyId,
        ISNULL(Br.Name,'') BranchName,
        ISNULL(H.SupplierId, 0) AS SupplierId,
        ISNULL(S.Name,'') SupplierName,
        ISNULL(S.Code,'') SupplierCode,
        ISNULL(S.Address, '') AS SupplierAddress,
        ISNULL(H.BENumber, '') AS BENumber,
        ISNULL(FORMAT(H.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.TransactionType, '') AS TransactionType,
        ISNULL(H.IsPost, 0) AS IsPost,
        CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
        ISNULL(H.PostedBy, '') AS PostedBy,
        ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
        ISNULL(H.FiscalYear, '') AS FiscalYear,
        ISNULL(H.PeriodId, '') AS PeriodId,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
      
            -- Sales Details
            ISNULL(D.Id, 0) AS PurchaseReturnId,
            ISNULL(D.Line, 0) AS Line,
            ISNULL(D.ProductId, 0) AS ProductId,
            ISNULL(P.Name,'') ProductName,
            ISNULL(P.Code,'') ProductCode,
            ISNULL(D.Quantity, 0) AS Quantity,
            ISNULL(D.UnitPrice, 0) AS UnitPrice,
            ISNULL(D.SubTotal, 0) AS SubTotal,
            ISNULL(D.SD, 0) AS SD,
            ISNULL(D.SDAmount, 0) AS SDAmount,
            ISNULL(D.VATRate, 0) AS VATRate,
            ISNULL(D.VATAmount, 0) AS VATAmount,
            ISNULL(D.OthersAmount, 0) AS OthersAmount,
            ISNULL(D.IsPost, 0) AS DetailIsPost,
			ISNULL(CP.CompanyName,'') CompanyName


        FROM PurchasesReturn H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Suppliers S ON H.SupplierId = S.Id
        LEFT OUTER JOIN PurchaseReturnDetails D ON H.Id = D.PurchasesReturnId            
        LEFT OUTER JOIN Products P ON D.ProductId = P.Id
		LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

        WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PurchaseReturnDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> GetPurchaseReturnDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<PurchaseReturnDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT D.Id) AS totalcount
            FROM 
            PurchaseReturnDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
            WHERE D.PurchasesReturnId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
        
            ISNULL(D.Id, 0) AS Id,
            ISNULL(D.PurchasesReturnId, 0) AS PurchasesReturnId,
            ISNULL(D.Line, 0) AS Line,
            ISNULL(D.ProductId, 0) AS ProductId,
            ISNULL(D.Quantity, 0) AS Quantity,
            ISNULL(D.UnitPrice,0) AS UnitPrice,
            ISNULL(D.SubTotal,0) AS SubTotal,
            ISNULL(D.SD,0) AS SD,
            ISNULL(D.SDAmount,0) AS SDAmount,
            ISNULL(D.VATRate,0) AS VATRate,
            ISNULL(D.VATAmount,0) AS VATAmount,
            ISNULL(D.LineTotal,0) AS LineTotal,
            ISNULL(D.OthersAmount,0) AS OthersAmount,
            ISNULL(CAST(D.IsPost AS BIT), 0) AS IsPost, 

            ISNULL(P.Name, '') AS ProductName,
            ISNULL(P.BanglaName, '') AS BanglaName,
            ISNULL(P.Code, '') AS ProductCode,
            ISNULL(P.HSCodeNo, '') AS HSCodeNo,
            ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
            ISNULL(PG.Name, '') AS ProductGroupName,
            ISNULL(UOM.Name, '') AS UOMName,
            ISNULL(UOM.Name, '') AS UOMFromName
            FROM 
            PurchaseReturnDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
            WHERE D.PurchasesReturnId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseReturnDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<PurchaseReturnDetailVM>.GetGridData_CMD(options, sqlQuery, "D.Id");

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

        //        public async Task<ResultVM> ProductWisePurchaseReturn(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //        {
        //            bool isNewConnection = false;
        //            DataTable dataTable = new DataTable();
        //            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //            try
        //            {
        //                if (conn == null)
        //                {
        //                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                    conn.Open();
        //                    isNewConnection = true;
        //                }

        //                string query = @"
        //                           SELECT     
        //                                -- Details Data
        //                                ISNULL(D.ProductId, 0) AS ProductId,
        //                                ISNULL(PG.Name, '') AS ProductGroupName,
        //                                ISNULL(P.Code, '') AS ProductCode,
        //                                ISNULL(P.Name, '') AS ProductName,
        //                                ISNULL(P.HSCodeNo, '') AS HSCodeNo,
        //                                ISNULL(uom.Name, '') AS UOMName,
        //                                ISNULL(SUM(D.Quantity), 0) AS Quantity

        //                                FROM PurchasesReturn M
        //                                LEFT OUTER JOIN PurchaseReturnDetails D ON ISNULL(M.Id,0) = D.PurchaseReturnId
        //                                LEFT OUTER JOIN Products P ON D.ProductId = P.Id
        //                                LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //                                LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        //                                LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id

        //                            WHERE 1 = 1 

        //";

        //                if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //                {
        //                    query += " AND M.Id = @Id ";
        //                }
        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
        //                {
        //                    query += " AND CAST(M.PurchaseReturnDate AS DATE) BETWEEN @FromDate AND @ToDate ";
        //                }

        //                // Apply additional conditions
        //                query = ApplyConditions(query, conditionalFields, conditionalValue, false);
        //                query += @" GROUP BY    
        //                                    D.ProductId,
        //                                    P.HSCodeNo,
        //                                    P.Code,
        //                                    P.Name,
        //                                    PG.Name,
        //                                    uom.Name ";
        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                // SET additional conditions param
        //                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

        //                if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
        //                }
        //                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
        //                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
        //                }
        //                objComm.Fill(dataTable);

        //                var lst = new List<PurchaseReturnReportVM>();
        //                int serialNumber = 1;
        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    lst.Add(new PurchaseReturnReportVM
        //                    {
        //                        SL = serialNumber,
        //                        ProductId = Convert.ToInt32(row["ProductId"]),
        //                        ProductGroupName = row["ProductGroupName"].ToString(),
        //                        ProductCode = row["ProductCode"].ToString(),
        //                        ProductName = row["ProductName"].ToString(),
        //                        HSCodeNo = row["HSCodeNo"].ToString(),
        //                        UOMName = row["UOMName"].ToString(),
        //                        Quantity = Convert.ToInt32(row["Quantity"])
        //                    });
        //                    serialNumber++;
        //                }

        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = lst;
        //                return result;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.ExMessage = ex.Message;
        //                result.Message = ex.Message;
        //                return result;
        //            }
        //            finally
        //            {
        //                if (isNewConnection && conn != null)
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }

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
    ISNULL(TRIM(M.Code), '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(TRIM(branch.Name), '') AS BranchName,
    ISNULL(TRIM(branch.Code), '') AS BranchCode,
    ISNULL(TRIM(branch.Address), '') AS BranchAddress,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(TRIM(C.Name), '') AS CurrencyName,
    ISNULL(TRIM(S.Name), '') AS SupplierName,
    ISNULL(S.Address, '') AS SupplierAddress,
    ISNULL(M.BENumber, 0) AS BENumber,
    ISNULL(FORMAT(M.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(M.GrandTotalAmount,0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(TRIM(M.Comments), '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
	ISNULL(M.IsPost, 0) AS IsPost,
	ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT,0) AS CurrencyRateFromBDT,
    ISNULL(M.FiscalYear,0) AS FiscalYear,
    ISNULL(M.PeriodId,0) AS PeriodId,   
    
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,


	ISNULL(D.Id, 0) AS DetailId,
	ISNULL(D.Line, 0) AS Line,
	ISNULL(D.ProductId, 0) AS ProductId,
	ISNULL(D.Quantity, 0) AS Quantity,
	ISNULL(D.UnitPrice, 0) AS UnitPrice,
	ISNULL(D.SubTotal,0) AS SubTotal,
	ISNULL(D.SD,0) AS SD,
	ISNULL(D.SDAmount,0) AS SDAmount,
	ISNULL(D.VATRate,0) AS VATRate,
	ISNULL(D.VATAmount,0) AS VATAmount,
	ISNULL(D.LineTotal,0) AS LineTotal,
	ISNULL(D.UOMId, 0) AS UOMId,
	ISNULL(D.UOMFromId, 0) AS UOMFromId,
	ISNULL(D.UOMConversion,0) AS UOMConversion,
	ISNULL(D.OthersAmount,0) AS OthersAmount,
	ISNULL(TRIM(D.Comments), '') AS DetailComments,
	ISNULL(TRIM(P.Name),'') ProductName,
	ISNULL(TRIM(P.BanglaName),'') BanglaName, 
	ISNULL(TRIM(P.Code),'') ProductCode, 
	ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
	ISNULL(P.ProductGroupId,0) ProductGroupId,
	ISNULL(PG.Name,'') ProductGroupName,
	ISNULL(TRIM(UOM.Name),'') UOMName,
	ISNULL(TRIM(UOM.Name),'') UOMFromName,
    ISNULL(TRIM(D.ReturnReason),'') ReasonOfReturn,
	ISNULL(D.IsFixedVAT,0) IsFixedVAT
    
FROM 
    PurchasesReturn M
LEFT OUTER JOIN PurchaseReturnDetails D ON ISNULL(M.Id,0) = D.PurchaseReturnId
LEFT OUTER JOIN Currencies C ON ISNULL(M.CurrencyId,0) = C.Id
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
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
