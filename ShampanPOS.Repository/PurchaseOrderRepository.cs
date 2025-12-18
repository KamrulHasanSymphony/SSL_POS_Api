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
    public class PurchaseOrderRepository : CommonRepository
    {
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }
        // Insert Method
        public async Task<ResultVM> Insert(PurchaseOrderVM purchaseOrder, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO PurchaseOrders 
                (Code, BranchId, SupplierId, OrderDate, DeliveryDateTime, Comments, TransactionType,
                PeriodId,IsPost, CreatedBy,CreatedOn,CreatedFrom)
                VALUES 
                (@Code, @BranchId, @SupplierId, @OrderDate, @DeliveryDateTime, @Comments, @TransactionType, 
                 @PeriodId,@IsPost, @CreatedBy,@CreatedOn,@CreatedFrom);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", purchaseOrder.Code);
                    cmd.Parameters.AddWithValue("@BranchId", purchaseOrder.BranchId);
                    cmd.Parameters.AddWithValue("@SupplierId", purchaseOrder.SupplierId);
                    cmd.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                    cmd.Parameters.AddWithValue("@DeliveryDateTime", purchaseOrder.DeliveryDateTime);
                    cmd.Parameters.AddWithValue("@Comments", purchaseOrder.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", purchaseOrder.TransactionType);
                    cmd.Parameters.AddWithValue("@PeriodId", purchaseOrder.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@IsPost", purchaseOrder.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", purchaseOrder.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", purchaseOrder.CreatedFrom);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    object newId = cmd.ExecuteScalar();
                    purchaseOrder.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = purchaseOrder.Id.ToString();
                    result.DataVM = purchaseOrder;
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
        public async Task<ResultVM> InsertDetails(PurchaseOrderDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                   INSERT INTO PurchaseOrderDetails (
    
                    PurchaseOrderId, 
                    BranchId, 
                    Line, 
                    ProductId, 
                    Quantity, 
                    UnitPrice, 
                    SubTotal, 
                    SD, 
                    SDAmount, 
                    VATRate, 
                    VATAmount, 
                    OthersAmount, 
                    LineTotal,
                    CompletedQty,
                    RemainQty,
                    IsCompleted
                ) 
                VALUES (
     
                    @PurchaseOrderId, 
                    @BranchId, 
                    @Line, 
                    @ProductId, 
                    @Quantity, 
                    @UnitPrice, 
                    @SubTotal, 
                    @SD, 
                    @SDAmount, 
                    @VATRate, 
                    @VATAmount, 
                    @OthersAmount, 
                    @LineTotal,
                    @CompletedQty,
                    @RemainQty,
                    @IsCompleted
                    );
                SELECT SCOPE_IDENTITY();
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@PurchaseOrderId", details.PurchaseOrderId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@Line", details.Line);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity ?? 0);
                    cmd.Parameters.AddWithValue("@UnitPrice", details.UnitPrice ?? 0);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal ?? 0);
                    cmd.Parameters.AddWithValue("@SD", details.SD ?? 0);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount ?? 0);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount ?? 0);
                    cmd.Parameters.AddWithValue("@OthersAmount", details.OthersAmount ?? 0);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal ?? 0);
                    cmd.Parameters.AddWithValue("@CompletedQty", details.CompletedQty ?? 0);
                    cmd.Parameters.AddWithValue("@RemainQty", details.RemainQty ?? 0);
                    cmd.Parameters.AddWithValue("@IsCompleted", details.IsCompleted ?? false);


                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details data inserted successfully.";
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
        public async Task<ResultVM> Update(PurchaseOrderVM purchaseOrder, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = purchaseOrder.Id.ToString(), DataVM = purchaseOrder };

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
                UPDATE PurchaseOrders 
                SET 
                   SupplierId = @SupplierId, OrderDate = @OrderDate, DeliveryDateTime = @DeliveryDateTime,  
                     Comments = @Comments, TransactionType = @TransactionType, PeriodId = @PeriodId, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn,LastUpdateFrom = @LastUpdateFrom
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", purchaseOrder.Id);
                    cmd.Parameters.AddWithValue("@SupplierId", purchaseOrder.SupplierId);
                    cmd.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                    cmd.Parameters.AddWithValue("@DeliveryDateTime", purchaseOrder.DeliveryDateTime);
                    cmd.Parameters.AddWithValue("@Comments", purchaseOrder.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", purchaseOrder.TransactionType);
                    cmd.Parameters.AddWithValue("@PeriodId", purchaseOrder.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@LastModifiedBy", purchaseOrder.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", purchaseOrder.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);

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
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(S.Address, '') AS SupplierAddress,
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
	ISNULL(M.IsPost, 0) AS IsPost,
	ISNULL(M.PostBy, '') AS PostBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.PeriodId,0) AS PeriodId,   
    
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
    
FROM 
    PurchaseOrders M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
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

                var lst = new List<PurchaseOrderVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new PurchaseOrderVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        SupplierId = Convert.ToInt32(row["SupplierId"]),
                        SupplierName = Convert.ToString(row["SupplierName"]),
                        OrderDate = row["OrderDate"].ToString(),
                        DeliveryDateTime = row["DeliveryDateTime"].ToString(),
                        Comments = row["Comments"].ToString(),
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        PostBy = row["PostBy"].ToString(),
                        PosteOn = row["PostedOn"].ToString(),
                        PeriodId = Convert.ToString(row["PeriodId"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString(),
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"].ToString()
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
                ISNULL(D.PurchaseOrderId, 0) AS PurchaseOrderId,
                ISNULL(D.BranchId, 0) AS BranchId,
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
                ISNULL(D.OthersAmount,0) AS OthersAmount,
                ISNULL(P.Name,'') ProductName,
                ISNULL(P.BanglaName,'') BanglaName, 
                ISNULL(P.Code,'') ProductCode, 
                ISNULL(P.HSCodeNo,'') HSCodeNo,
                ISNULL(P.ProductGroupId,0) ProductGroupId,
                ISNULL(PG.Name,'') ProductGroupName,
                ISNULL(UOM.Name,'') UOMName,
                ISNULL(UOM.Name,'') UOMFromName

                FROM 
                PurchaseOrderDetails D
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
                    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
                    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,
                    ISNULL(M.Comments, '') AS Comments,
                    ISNULL(M.TransactionType, '') AS TransactionType,
	                ISNULL(M.IsPost, 0) AS IsPost,
	                ISNULL(M.PostBy, '') AS PostBy,
                    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
                    ISNULL(M.PeriodId,0) AS PeriodId,   
    
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
                    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
                    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
    
                FROM 
                    PurchaseOrders M
                WHERE  1 = 1";


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
SELECT Id, Code
FROM PurchaseOrders
WHERE IsPost = 1
ORDER BY Code";

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
                result.Message = "Error in Dropdown.";
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

                string query = $" UPDATE PurchaseOrders SET IsPost = 1, PostBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE PurchaseOrderDetails SET IsPost = 1 WHERE PurchaseOrderId IN ({inClause}) ";

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
        // MultipleIsCompleted Method
        public async Task<ResultVM> MultipleIsCompleted(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = $" UPDATE PurchaseOrders SET IsCompleted = 1, LastModifiedBy = @LastModifiedBy , LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn = GETDATE() WHERE Id IN ({inClause}) ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data Completed successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were Completed.");
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

                var data = new GridEntity<PurchaseOrderVM>();

                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

	WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "");

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
	            ISNULL(H.SupplierId, 0) AS SupplierId,
                ISNULL(s.Name, 0) AS SupplierName,
                ISNULL(S.Address, '') AS SupplierAddress,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,                
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
	            ISNULL(H.Comments, '') AS Comments,
	            ISNULL(H.TransactionType, '') AS TransactionType,
	            ISNULL(H.PeriodId, 0) AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,                
	       
                ISNULL(br.Name, '') AS BranchName,                
                ISNULL(br.Address, '') AS BranchAddress



            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

        WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PurchaseOrderVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<PurchaseOrderDetailVM>();

                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
            LEFT OUTER JOIN PurchaseOrderDetails D on h.Id = D.PurchaseOrderId            
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id

	WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderDetailVM>.FilterCondition(options.filter) + ")" : "");

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
	            ISNULL(H.SupplierId, 0) AS SupplierId,
                ISNULL(s.Name, 0) AS SupplierName,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status, 
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
	            ISNULL(H.Comments, '') AS Comments,
	            ISNULL(H.TransactionType, '') AS TransactionType,
	            ISNULL(H.PeriodId, 0) AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,                
	            ISNULL(H.IsPost, 0) AS IsPost,	            
                ISNULL(br.Name, '') AS BranchName,
        -- Sales Details
            ISNULL(D.Id, 0) AS PurchaseOrderDetail,
            ISNULL(D.PurchaseOrderId, 0) AS PurchaseOrderId,
            ISNULL(D.BranchId, 0) AS BranchId,
            ISNULL(D.Line, 0) AS Line,
            ISNULL(D.ProductId, 0) AS ProductId,
            ISNULL(P.Name,'') ProductName,
            ISNULL(D.Quantity, 0) AS Quantity,
            ISNULL(D.UnitPrice, 0) AS UnitPrice,
            ISNULL(D.SubTotal, 0) AS SubTotal,
            ISNULL(D.SD, 0) AS SD,
            ISNULL(D.SDAmount, 0) AS SDAmount,
            ISNULL(D.VATRate, 0) AS VATRate,
            ISNULL(D.VATAmount, 0) AS VATAmount,
            ISNULL(D.OthersAmount, 0) AS OthersAmount


            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
            LEFT OUTER JOIN PurchaseOrderDetails D on h.Id = D.PurchaseOrderId            
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id

        WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PurchaseOrderDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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
        //        public async Task<ResultVM> FromPurchaseOrderGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

        //                var data = new GridEntity<PurchaseOrderVM>();

        //                // Define your SQL query string
        //                string sqlQuery = @"
        //    -- Count query
        //    SELECT COUNT(DISTINCT H.Id) AS totalcount
        //            FROM PurchaseOrders H
        //            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
        //            LEFT OUTER JOIN Currencies c on h.CurrencyId = c.Id
        //            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

        //	        LEFT JOIN 
        //					            (
        //						            SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
        //						            FROM [dbo].[PurchaseOrderDetails] d   
        //						            GROUP BY d.PurchaseOrderId
        //					            ) SD ON H.Id = SD.PurchaseOrderId
        //            WHERE H.IsPost = 1 AND ISNULL(H.IsCompleted,0) = 0 AND  (SD.TotalCompletedQty < SD.TotalQuantity)
        //    -- Add the filter condition
        //    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

        //    -- Data query with pagination and sorting
        //    SELECT * 
        //    FROM (
        //        SELECT 
        //        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,

        //                ISNULL(H.Id, 0) AS Id,
        //                ISNULL(H.Code, '') AS Code,
        //	            ISNULL(H.SupplierId, 0) AS SupplierId,
        //                ISNULL(s.Name, 0) AS SupplierName,
        //                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
        //                ISNULL(H.BENumber, '') AS BENumber,
        //                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
        //                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
        //                (ISNULL(SD.TotalQuantity, 0)-ISNULL(SD.TotalCompletedQty, 0)) AS TotalQuantity,
        //                ISNULL(SD.TotalCompletedQty, 0) AS TotalCompletedQty,
        //                ISNULL(H.GrandTotalAmount, 0) AS GrandTotalAmount,
        //                ISNULL(H.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
        //                ISNULL(H.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
        //	            ISNULL(H.Comments, '') AS Comments,
        //	            ISNULL(H.TransactionType, '') AS TransactionType,
        //	            ISNULL(H.CurrencyId, 0) AS CurrencyId,
        //	            ISNULL(c.Name, '') AS CurrencyName,
        //	            ISNULL(H.PeriodId, 0) AS PeriodId,
        //	            ISNULL(h.FiscalYear,'') AS FiscalYear,
        //                ISNULL(H.CreatedBy, '') AS CreatedBy,
        //                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        //                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,                
        //	            ISNULL(H.IsPost, 'N') AS IsPost,
        //                ISNULL(br.Name, '') AS BranchName


        //            FROM PurchaseOrders H
        //            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
        //            LEFT OUTER JOIN Currencies c on h.CurrencyId = c.Id
        //            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

        //            LEFT JOIN 
        //					    (
        //						    SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
        //						    FROM [dbo].[PurchaseOrderDetails] d   
        //						    GROUP BY d.PurchaseOrderId
        //					    ) SD ON H.Id = SD.PurchaseOrderId
        //            WHERE H.IsPost = 1 AND ISNULL(H.IsCompleted,0) = 0 AND  (SD.TotalCompletedQty < SD.TotalQuantity)

        //    -- Add the filter condition
        //    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

        //    ) AS a
        //    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        //";

        //                data = KendoGrid<PurchaseOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = data;

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
        public async Task<ResultVM> PurchaseOrderList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.Id, 0) AS PurchaseOrderId,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(S.Name, 0) AS SupplierName,
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
	ISNULL(M.IsPost, 0) AS IsPost,
	ISNULL(M.PostBy, '') AS PostBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.PeriodId,0) AS PeriodId,   
    
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
    
FROM 
    PurchaseOrders M
    LEFT OUTER JOIN Suppliers s on M.SupplierId = s.Id
WHERE  1 = 1
 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND M.Id IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

                var lst = new List<PurchaseVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new PurchaseVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        PurchaseOrderId = Convert.ToInt32(row["PurchaseOrderId"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        SupplierId = Convert.ToInt32(row["SupplierId"]),
                        SupplierName = Convert.ToString(row["SupplierName"]),
                        InvoiceDateTime = row["DeliveryDateTime"].ToString(),
                        Comments = row["Comments"].ToString(),
                        Code = row["Code"].ToString(),
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        PostedBy = row["PostBy"].ToString(),
                        PostedOn = row["PostedOn"].ToString(),
                        PeriodId = Convert.ToString(row["PeriodId"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString(),
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"].ToString()
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
        public async Task<ResultVM> PurchaseOrderDetailsList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(D.Id, 0) AS PurchaseOrderDetailId,
                ISNULL(D.PurchaseOrderId, 0) AS PurchaseOrderId,
                ISNULL(D.BranchId, 0) AS BranchId,
                ISNULL(D.Line, 0) AS Line,
                ISNULL(D.ProductId, 0) AS ProductId,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) AS Quantity,
                ISNULL(D.UnitPrice,0.00) AS UnitPrice,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00))*ISNULL(D.UnitPrice,0.00) AS SubTotal,
                ISNULL(D.SD,0.00) AS SD,
                ISNULL(D.SDAmount,0.00) AS SDAmount,
                ISNULL(D.VATRate,0.00) AS VATRate,
                ISNULL(D.VATAmount,0.00) AS VATAmount,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00))*ISNULL(D.UnitPrice,0.00) AS LineTotal,
                ISNULL(FORMAT(D.OthersAmount, 'N2'), '0.00') AS OthersAmount,
                ISNULL(P.Name,'') ProductName,
                ISNULL(P.BanglaName,'') BanglaName, 
                ISNULL(P.Code,'') ProductCode, 
                ISNULL(P.HSCodeNo,'') HSCodeNo,
                ISNULL(P.ProductGroupId,0) ProductGroupId,
                ISNULL(PG.Name,'') ProductGroupName,
                ISNULL(UOM.Name,'') UOMName,
                ISNULL(UOM.Name,'') UOMFromName

                FROM 
                PurchaseOrderDetails D
                LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
                WHERE 1 = 1 AND (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) > 0 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND D.PurchaseOrderId IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

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
        public async Task<ResultVM> GetPurchaseOrderDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<PurchaseOrderDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM PurchaseOrderDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
            LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
            WHERE h.PurchaseOrderId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.ProductId, 0) AS ProductId,
            ISNULL(H.UOMId, 0) AS UOMId,
            ISNULL(H.UOMFromId, 0) AS UOMFromId,
            ISNULL(H.PurchaseOrderId, 0) AS PurchaseOrderId,
            ISNULL(P.Name, '') AS ProductName,
            ISNULL(H.Line, '') AS Line,
            ISNULL(H.Quantity, 0) AS Quantity,
            ISNULL(H.UnitPrice, 0) AS UnitPrice,
            ISNULL(H.SubTotal, 0) AS SubTotal,
            ISNULL(H.SD, 0) AS SD,
            ISNULL(H.SDAmount, 0) AS SDAmount,
            ISNULL(H.VATRate, 0) AS VATRate,
            ISNULL(H.VATAmount, 0) AS VATAmount,
            ISNULL(H.OthersAmount, 0) AS OthersAmount,
            ISNULL(H.LineTotal, 0) AS LineTotal,
            ISNULL(u.Name, '') AS UOMName,
            ISNULL(H.UOMconversion, '') AS UOMconversion,
            ISNULL(H.Comments, '') AS Comments,
            ISNULL(H.VATType, '') AS VATType,
            ISNULL(H.TransactionType, '') AS TransactionType,
            ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
            ISNULL(H.IsFixedVAT, 0) AS IsFixedVAT,
            ISNULL(H.FixedVATAmount, 0) AS FixedVATAmount

            FROM PurchaseOrderDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
            LEFT OUTER JOIN UOMs u ON h.UOMId = u.Id
            WHERE h.PurchaseOrderId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<PurchaseOrderDetailVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
//        public async Task<ResultVM> ProductWisePurchaseOrder(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//                                    -- Details Data
//                                    ISNULL(D.ProductId, 0) AS ProductId,
//                                    ISNULL(PG.Name, '') AS ProductGroupName,
//                                    ISNULL(P.Code, '') AS ProductCode,
//                                    ISNULL(P.Name, '') AS ProductName,
//                                    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
//                                    ISNULL(uom.Name, '') AS UOMName,
//                                    ISNULL(SUM(D.Quantity), 0) AS Quantity

//                                    FROM PurchaseOrders M
//                                    LEFT OUTER JOIN PurchaseOrderDetails D ON ISNULL(M.Id,0) = D.PurchaseOrderId
//                                    LEFT OUTER JOIN Products P ON D.ProductId = P.Id
//                                    LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//                                    LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
//                                    LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id

//                                    WHERE 1=1 

                                
//";

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    query += " AND M.Id = @Id ";
//                }

//                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
//                {
//                    query += " AND CAST(M.OrderDate AS DATE) BETWEEN @FromDate AND @ToDate ";
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

//                var lst = new List<PurchaseOrderReportVM>();
//                int serialNumber = 1;
//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new PurchaseOrderReportVM
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
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount,0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(TRIM(M.Comments), '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
	ISNULL(M.IsPost, 0) AS IsPost,
	ISNULL(M.PostBy, '') AS PostedBy,
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
	ISNULL(p.CtnSizeFactor,0) AS UOMConversion,
	ISNULL(D.OthersAmount,0) AS OthersAmount,
	ISNULL(TRIM(D.Comments), '') AS DetailComments,
	ISNULL(TRIM(P.Name),'') ProductName,
	ISNULL(TRIM(P.BanglaName),'') BanglaName, 
	ISNULL(TRIM(P.Code),'') ProductCode, 
	ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
	ISNULL(P.ProductGroupId,0) ProductGroupId,
	ISNULL(PG.Name,'') ProductGroupName,
	ISNULL(TRIM(p.CtnSize),'') UOMName,
	ISNULL(D.IsFixedVAT,0) IsFixedVAT
    
FROM 
    PurchaseOrders M
LEFT OUTER JOIN PurchaseOrderDetails D ON ISNULL(M.Id,0) = D.PurchaseOrderId
LEFT OUTER JOIN Currencies C ON ISNULL(M.CurrencyId,0) = C.Id
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id

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

        public async Task<ResultVM> FromPurchaseOrderGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<PurchaseOrderVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

	        LEFT JOIN 
					            (
						            SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						            FROM [dbo].[PurchaseOrderDetails] d   
						            GROUP BY d.PurchaseOrderId
					            ) SD ON H.Id = SD.PurchaseOrderId
            WHERE H.IsPost = 1 AND  (SD.TotalCompletedQty < SD.TotalQuantity)
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
	            ISNULL(H.SupplierId, 0) AS SupplierId,
                ISNULL(s.Name, 0) AS SupplierName,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
                (ISNULL(SD.TotalQuantity, 0)-ISNULL(SD.TotalCompletedQty, 0)) AS TotalQuantity,
                ISNULL(SD.TotalCompletedQty, 0) AS TotalCompletedQty,
	            ISNULL(H.Comments, '') AS Comments,
	            ISNULL(H.TransactionType, '') AS TransactionType,
	            ISNULL(H.PeriodId, 0) AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,                
	            ISNULL(H.IsPost, 'N') AS IsPost,
                ISNULL(br.Name, '') AS BranchName


            FROM PurchaseOrders H
            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

            LEFT JOIN 
					    (
						    SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						    FROM [dbo].[PurchaseOrderDetails] d   
						    GROUP BY d.PurchaseOrderId
					    ) SD ON H.Id = SD.PurchaseOrderId
            WHERE H.IsPost = 1 AND  (SD.TotalCompletedQty < SD.TotalQuantity)

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PurchaseOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
