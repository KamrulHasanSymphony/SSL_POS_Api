using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                (
                    Code, BranchId, CompanyId,UserId, SupplierId, OrderDate, DeliveryDateTime, Comments,
                    TransactionType, PeriodId, IsPost, CreatedBy, CreatedOn, CreatedFrom
                )
                VALUES 
                (
                    @Code, @BranchId, @CompanyId,@UserId, @SupplierId, @OrderDate, @DeliveryDateTime, @Comments,
                    @TransactionType, @PeriodId, @IsPost, @CreatedBy, @CreatedOn, @CreatedFrom
                );
                SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", purchaseOrder.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", purchaseOrder.BranchId ?? (object)DBNull.Value);

                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = (object?)purchaseOrder.CompanyId ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@UserId", purchaseOrder.UserId ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@SupplierId", purchaseOrder.SupplierId);
                    cmd.Parameters.AddWithValue("@OrderDate", purchaseOrder.OrderDate);
                    cmd.Parameters.AddWithValue("@DeliveryDateTime", purchaseOrder.DeliveryDateTime);
                    cmd.Parameters.AddWithValue("@Comments", purchaseOrder.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", purchaseOrder.TransactionType);
                    cmd.Parameters.AddWithValue("@PeriodId", purchaseOrder.PeriodId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", purchaseOrder.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", purchaseOrder.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", purchaseOrder.CreatedFrom);

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
                    CompanyId,
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
                    @CompanyId, 
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
                    cmd.Parameters.AddWithValue("@CompanyId", details.CompanyId ?? 0);


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
                   SupplierId = @SupplierId,CompanyId = @CompanyId, OrderDate = @OrderDate, DeliveryDateTime = @DeliveryDateTime,  
                     Comments = @Comments, TransactionType = @TransactionType, PeriodId = @PeriodId, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn,LastUpdateFrom = @LastUpdateFrom
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", purchaseOrder.Id);
                    cmd.Parameters.AddWithValue("@CompanyId", purchaseOrder.CompanyId ?? 0);
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




        //public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)



        public async Task<ResultVM> List(string[] conditionalFields,string[] conditionalValues,int companyId,int branchId,PeramModel vm = null,SqlConnection conn = null,SqlTransaction transaction = null)

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

    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(S.Address, '') AS SupplierAddress,

    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,

    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,

    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostBy, '') AS PostBy,

    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '') AS PostedOn,

    ISNULL(M.PeriodId, 0) AS PeriodId,

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '') AS CreatedOn,

    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '') AS LastModifiedOn,

    ISNULL(Br.Name, '') AS BranchName,
    ISNULL(CP.CompanyName, '') AS CompanyName

FROM PurchaseOrders M

LEFT OUTER JOIN Suppliers S
    ON ISNULL(M.SupplierId, 0) = S.Id

LEFT OUTER JOIN CompanyProfiles CP
    ON M.CompanyId = CP.Id

LEFT OUTER JOIN BranchProfiles Br
    ON M.BranchId = Br.Id

WHERE 1 = 1 AND

M.CompanyId = @CompanyId
AND (@BranchId = 0 OR M.BranchId = @BranchId)
 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                // Apply additional conditions
                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand.Parameters.AddWithValue("@CompanyId", companyId);
                objComm.SelectCommand.Parameters.AddWithValue("@BranchId", branchId);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

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
                        BranchName = row.Field<string>("BranchName"),
                        CompanyId = Convert.ToInt32(row["CompanyId"]),
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
        ISNULL(D.CompanyId, 0) AS CompanyId,
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
        ISNULL(CP.CompanyName,'') CompanyName

        FROM 
        PurchaseOrderDetails D
        LEFT OUTER JOIN Products P ON D.ProductId = P.Id
        LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id
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
                //query += $" UPDATE PurchaseOrderDetails SET IsPost = 1 WHERE PurchaseOrderId IN ({inClause}) ";

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
				LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

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
	            ISNULL(H.IsPost, 0) AS IsPost,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,                
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
	            ISNULL(H.Comments, '') AS Comments,
	            ISNULL(H.TransactionType, '') AS TransactionType,
	            ISNULL(H.PeriodId, 0) AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,  
				ISNULL(CP.CompanyName,'') CompanyName,

	       
                ISNULL(br.Name, '') AS BranchName

				FROM PurchaseOrders H
				LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
				LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
				LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

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



//        public async Task<ResultVM> GetGridData(
//           GridOptions options,
//           string[] conditionalFields,
//           string[] conditionalValues,
//           SqlConnection conn = null,
//           SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;

//            ResultVM result = new ResultVM
//            {
//                Status = "Fail",
//                Message = "Error",
//                ExMessage = null,
//                Id = "0",
//                DataVM = null
//            };

//            try
//            {
//                int companyId = Convert.ToInt32(options.vm.CompanyId);

//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string filterCondition = "";
//                if (options?.filter?.Filters != null && options.filter.Filters.Count > 0)
//                {
//                    filterCondition = " AND (" +
//                        GridQueryBuilder<PurchaseOrderVM>.FilterCondition(options.filter) +
//                        ")";
//                }

//                string sortExpression = "H.Id DESC";
//                if (options?.sort != null && options.sort.Count > 0)
//                {
//                    sortExpression = options.sort[0].field + " " + options.sort[0].dir;
//                }

//                string companyCondition = $" AND H.CompanyId = {companyId} ";

//                string sqlQuery = $@"

//-- =========================
//-- COUNT QUERY
//-- =========================
//SELECT COUNT(DISTINCT H.Id) AS totalcount
//FROM PurchaseOrders H
//LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
//LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
//LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

//WHERE 1 = 1
//{companyCondition}
//{filterCondition}

//-- =========================
//-- DATA QUERY
//-- =========================
//SELECT *
//FROM
//(
//    SELECT
//        ROW_NUMBER() OVER(ORDER BY {sortExpression}) AS rowindex,

//                ISNULL(H.Id, 0) AS Id,
//                ISNULL(H.Code, '') AS Code,
//	            ISNULL(H.SupplierId, 0) AS SupplierId,
//                ISNULL(s.Name, 0) AS SupplierName,
//                ISNULL(S.Address, '') AS SupplierAddress,
//	            ISNULL(H.IsPost, 0) AS IsPost,
//                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,                
//                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
//                ISNULL(FORMAT(H.DeliveryDateTime, 'yyyy-MM-dd') , '') AS DeliveryDateTime,
//	            ISNULL(H.Comments, '') AS Comments,
//	            ISNULL(H.TransactionType, '') AS TransactionType,
//	            ISNULL(H.PeriodId, 0) AS PeriodId,
//                ISNULL(H.CreatedBy, '') AS CreatedBy,
//                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
//                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,  
//				ISNULL(CP.CompanyName,'') CompanyName,

	       
//                ISNULL(br.Name, '') AS BranchName

//				FROM PurchaseOrders H
//				LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
//				LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
//				LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

//				WHERE 1 = 1
//    {companyCondition}
//    {filterCondition}

//) A
//WHERE A.rowindex > @skip
//AND (@take = 0 OR A.rowindex <= @take);
//";

//                var data = KendoGrid<PurchaseOrderVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues, "", "", "", "", "", "", "", "", "", "");

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = data;

//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.Status = "Fail";
//                result.Message = ex.Message;
//                result.ExMessage = ex.ToString();
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                    conn.Dispose();
//                }
//            }
//        }






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
				LEFT OUTER JOIN Suppliers s ON h.SupplierId = s.Id
				LEFT OUTER JOIN BranchProfiles br ON h.BranchId = br.Id
				LEFT OUTER JOIN PurchaseOrderDetails D ON h.Id = D.PurchaseOrderId
				LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id
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
                ISNULL(H.Code, '') AS PurchaseOrderCode,
	            ISNULL(H.SupplierId, 0) AS SupplierId,
				ISNULL(s.Code, '') AS SupplierCode,
				ISNULL(D.CompanyId, 0) AS CompanyId,
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
			    ISNULL(P.Code, '') AS ProductCode,

				ISNULL(D.Quantity, 0) AS Quantity,
				ISNULL(D.UnitPrice, 0) AS UnitPrice,
				ISNULL(D.SubTotal, 0) AS SubTotal,
				ISNULL(D.SD, 0) AS SD,
				ISNULL(D.SDAmount, 0) AS SDAmount,
				ISNULL(D.VATRate, 0) AS VATRate,
				ISNULL(D.VATAmount, 0) AS VATAmount,
				ISNULL(D.OthersAmount, 0) AS OthersAmount,
				ISNULL(CP.CompanyName,'') CompanyName

				FROM PurchaseOrders H
				LEFT OUTER JOIN Suppliers s ON h.SupplierId = s.Id
				LEFT OUTER JOIN BranchProfiles br ON h.BranchId = br.Id
				LEFT OUTER JOIN PurchaseOrderDetails D ON h.Id = D.PurchaseOrderId
				LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id
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
    ISNULL(P.Code, '') AS Code,
    ISNULL(M.Code, '') AS PurchaseOrderCode,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(S.Name, '') AS SupplierName,
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
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,

    ISNULL(P.BENumber, '') AS BENumber,
    ISNULL(FORMAT(P.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
	ISNULL(FORMAT(P.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime

FROM 
    PurchaseOrders M
    LEFT OUTER JOIN Suppliers S ON M.SupplierId = S.Id
    LEFT OUTER JOIN Purchases P ON M.Id = P.PurchaseOrderId  
WHERE 
    1 = 1
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
                        BENumber = Convert.ToString(row["BENumber"]),
                        DeliveryDateTime = row["DeliveryDateTime"].ToString(),
                        PurchaseDate = row["PurchaseDate"].ToString(),
                        OrderDate = row["OrderDate"].ToString(),  
                        InvoiceDateTime = row["InvoiceDateTime"].ToString(), 
                        Comments = row["Comments"].ToString(),
                        Code = row["Code"].ToString(),
                        PurchaseOrderCode = row["PurchaseOrderCode"].ToString(),
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
               (
                    ((ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) * ISNULL(D.UnitPrice,0.00))
                    + ISNULL(D.SDAmount,0.00)
                    + ISNULL(D.VATAmount,0.00)
                    + ISNULL(D.OthersAmount,0.00)
                ) AS LineTotal,
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
            ISNULL(H.LineTotal, 0) AS LineTotal

            FROM PurchaseOrderDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
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

        public async Task<ResultVM> PurchaseOrderReport(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
	ISNULL(Br.Name,'') BranchName,
	ISNULL(Br.Address,'') BranchAddress,
    ISNULL(CP.CompanyName,'') CompanyName
    
FROM 
    PurchaseOrders M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
LEFT OUTER JOIN CompanyProfiles CP ON M.CompanyId = CP.Id
    LEFT OUTER JOIN BranchProfiles Br ON M.BranchId = Br.Id

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
                        BranchName = row.Field<string>("BranchName"),
                        BranchAddress = row.Field<string>("BranchAddress"),
                        CompanyId = Convert.ToInt32(row["CompanyId"]),
                        CompanyName = Convert.ToString(row["CompanyName"]),
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
        }

        //        public async Task<ResultVM> ReportList(string[] conditionalFields, string[] conditionalValues, PurchaseOrderReportVM vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //        {
        //            bool isNewConnection = false;
        //            DataTable dataTable = new DataTable();
        //            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //            try
        //            {
        //                if (conn == null)
        //                {
        //                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                    conn.Open();
        //                    isNewConnection = true;
        //                }

        //                string query = "";

        //                if (vm != null && vm.IsSummary)
        //                {

        //                    //Summary
        //                    query = @"
        //SELECT
        //    ISNULL(M.Code,'') AS PurchaseCode,
        //    ISNULL(M.SupplierId,0) AS SupplierId,
        //    ISNULL(S.Name,'') AS SupplierName,

        //    ISNULL(FORMAT(M.OrderDate, 'dd/MM/yyyy'), '') AS OrderDate,
        //    ISNULL(FORMAT(M.DeliveryDateTime, 'dd/MM/yyyy'), '') AS DeliveryDateTime,

        //    ISNULL(D.ProductId,0) AS ProductId,
        //    ISNULL(P.Name,'') AS ProductName,

        //    SUM(ISNULL(D.Quantity,0)) AS Quantity,
        //    SUM(ISNULL(D.SubTotal,0)) AS SubTotal,
        //    SUM(ISNULL(D.VATAmount,0)) AS VATAmount,
        //    SUM(ISNULL(D.LineTotal,0)) AS LineTotal

        //FROM PurchaseOrders M
        //LEFT JOIN PurchaseOrderDetails D ON M.Id = D.PurchaseOrderId
        //LEFT JOIN Suppliers S ON M.SupplierId = S.Id
        //LEFT JOIN Products P ON D.ProductId = P.Id

        //WHERE 1=1

        //AND (@SupplierId = 0 OR M.SupplierId = @SupplierId)

        //AND (@OrderFromDate IS NULL OR M.OrderDate >= @OrderFromDate)
        //AND (@OrderToDate IS NULL OR M.OrderDate <= @OrderToDate)

        //AND (@DeliveryFromDate IS NULL OR M.DeliveryDateTime >= @DeliveryFromDate)
        //AND (@DeliveryToDate IS NULL OR M.DeliveryDateTime <= @DeliveryToDate)

        //GROUP BY
        //    M.Code,
        //    M.SupplierId,
        //    S.Name,
        //    M.OrderDate,
        //    M.DeliveryDateTime,
        //    D.ProductId,
        //    P.Name
        //";
        //                }
        //                else
        //                {
        //                    //Details
        //                    query = @"
        //SELECT
        //    ISNULL(M.Code,'') AS PurchaseCode,
        //    ISNULL(M.SupplierId,0) AS SupplierId,
        //    ISNULL(S.Name,'') AS SupplierName,

        //    ISNULL(FORMAT(M.OrderDate, 'dd/MM/yyyy'), '') AS OrderDate,
        //    ISNULL(FORMAT(M.DeliveryDateTime, 'dd/MM/yyyy'), '') AS DeliveryDateTime,

        //    ISNULL(D.Id,0) AS DetailId,
        //    ISNULL(D.ProductId,0) AS ProductId,
        //    ISNULL(P.Name,'') AS ProductName,
        //    ISNULL(P.Code,'') AS ProductCode,

        //    ISNULL(D.Quantity,0) Quantity,
        //    ISNULL(D.UnitPrice,0) UnitPrice,
        //    ISNULL(D.SubTotal,0) SubTotal,
        //    ISNULL(D.VATAmount,0) VATAmount,
        //    ISNULL(D.LineTotal,0) LineTotal

        //FROM PurchaseOrders M
        //LEFT JOIN PurchaseOrderDetails D ON M.Id = D.PurchaseOrderId
        //LEFT JOIN Suppliers S ON M.SupplierId = S.Id
        //LEFT JOIN Products P ON D.ProductId = P.Id

        //WHERE 1=1

        //AND (@SupplierId = 0 OR M.SupplierId = @SupplierId)

        //AND (@OrderFromDate IS NULL OR M.OrderDate >= @OrderFromDate)
        //AND (@OrderToDate IS NULL OR M.OrderDate <= @OrderToDate)

        //AND (@DeliveryFromDate IS NULL OR M.DeliveryDateTime >= @DeliveryFromDate)
        //AND (@DeliveryToDate IS NULL OR M.DeliveryDateTime <= @DeliveryToDate)
        //";
        //                }

        //                // Apply additional conditions
        //                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                // SET additional conditions param
        //                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);


        //                objComm.SelectCommand.Parameters.AddWithValue(
        //                    "@SupplierId",
        //                    vm.SupplierId
        //                );

        //                objComm.SelectCommand.Parameters.AddWithValue(
        //                    "@OrderFromDate",
        //                    string.IsNullOrEmpty(vm.OrderFromDate)
        //                        ? (object)DBNull.Value
        //                        : DateTime.Parse(vm.OrderFromDate)
        //                );

        //                objComm.SelectCommand.Parameters.AddWithValue(
        //                    "@OrderToDate",
        //                    string.IsNullOrEmpty(vm.OrderToDate)
        //                        ? (object)DBNull.Value
        //                        : DateTime.Parse(vm.OrderToDate)
        //                );

        //                objComm.SelectCommand.Parameters.AddWithValue(
        //                    "@DeliveryFromDate",
        //                    string.IsNullOrEmpty(vm.DeliveryFromDate)
        //                        ? (object)DBNull.Value
        //                        : DateTime.Parse(vm.DeliveryFromDate)
        //                );

        //                objComm.SelectCommand.Parameters.AddWithValue(
        //                    "@DeliveryToDate",
        //                    string.IsNullOrEmpty(vm.DeliveryToDate)
        //                        ? (object)DBNull.Value
        //                        : DateTime.Parse(vm.DeliveryToDate)
        //                );

        //                objComm.Fill(dataTable);


        //                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseOrderReportVM
        //                {
        //                    Id = dataTable.Columns.Contains("Id") ? row.Field<int>("Id") : 0,

        //                    Code = dataTable.Columns.Contains("PurchaseCode")
        //                        ? row.Field<string>("PurchaseCode")
        //                        : "",

        //                    SupplierName = dataTable.Columns.Contains("SupplierName")
        //                        ? row.Field<string>("SupplierName")
        //                        : "",

        //                    ProductName = dataTable.Columns.Contains("ProductName")
        //                        ? row.Field<string>("ProductName")
        //                        : "",

        //                    Quantity = row.Field<decimal?>("Quantity") ?? 0.0m,

        //                    UnitPrice = dataTable.Columns.Contains("UnitPrice")
        //                        ? row.Field<decimal?>("UnitPrice") ?? 0.0m
        //                        : 0.0m,

        //                    SubTotal = row.Field<decimal?>("SubTotal") ?? 0.0m,

        //                    SD = dataTable.Columns.Contains("SD")
        //                        ? row.Field<decimal?>("SD") ?? 0.0m
        //                        : 0.0m,

        //                    SDAmount = dataTable.Columns.Contains("SDAmount")
        //                        ? row.Field<decimal?>("SDAmount") ?? 0.0m
        //                        : 0.0m,

        //                    VATRate = dataTable.Columns.Contains("VATRate")
        //                        ? row.Field<decimal?>("VATRate") ?? 0.0m
        //                        : 0.0m,

        //                    VATAmount = row.Field<decimal?>("VATAmount") ?? 0.0m,

        //                    LineTotal = row.Field<decimal?>("LineTotal") ?? 0.0m,

        //                    DeliveryDateTime = dataTable.Columns.Contains("DeliveryDateTime")
        //                        ? row.Field<string>("DeliveryDateTime")
        //                        : "",

        //                    OrderDate = dataTable.Columns.Contains("OrderDate")
        //                        ? row.Field<string>("OrderDate")
        //                        : ""
        //                }).ToList();

        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = modelList;
        //                return result;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.Message = ex.Message;
        //                result.ExMessage = ex.Message;
        //                return result;
        //            }
        //        }



        public async Task<ResultVM> ReportList(string[] conditionalFields, string[] conditionalValues, PurchaseOrderReportVM vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = "";

                if (vm != null && vm.IsSummary)
                {
                    // =========================
                    // SUMMARY MODE (ReportType Wise)
                    // =========================
                    switch (vm.ReportType)
                    {
                        case "Day Wise":
                            query = @"
SELECT 
FORMAT(CAST(P.OrderDate AS DATE),'dd-MMM-yyyy') AS OrderDate,
FORMAT(CAST(P.DeliveryDateTime AS DATE),'dd-MMM-yyyy') AS DeliveryDateTime,
COUNT(DISTINCT P.Id) AS TotalInvoice,
SUM(PD.Quantity) AS Quantity,
SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.BranchId AS BranchId,
P.CompanyId AS CompanyId,
C.CompanyName AS CompanyName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles C ON P.CompanyId = C.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
CAST(P.OrderDate AS DATE),
CAST(P.DeliveryDateTime AS DATE),
P.CompanyId,
P.BranchId,
C.CompanyName,
B.Name
ORDER BY 
CAST(P.DeliveryDateTime AS DATE),
CAST(P.OrderDate AS DATE)";
                            break;

                        case "Monthly":
                            query = @"
SELECT 
DATENAME(MONTH, P.OrderDate) + '-' + CAST(YEAR(P.OrderDate) AS VARCHAR(4)) AS MonthYear,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
COUNT(DISTINCT P.Id) AS TotalInvoice,

SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.CompanyId,
P.BranchId,
C.CompanyName,
S.Name AS SupplierName,
PR.Name AS ProductName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles C ON P.CompanyId = C.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
YEAR(P.OrderDate),
MONTH(P.OrderDate),
DATENAME(MONTH, P.OrderDate),
P.OrderDate,
P.DeliveryDateTime,
P.CompanyId,
P.BranchId,
C.CompanyName,
S.Name,
PR.Name,
B.Name
ORDER BY 
P.DeliveryDateTime,
P.OrderDate";
                            break;

                        case "Supplier Wise":
                            query = @"
SELECT 
S.Id,
S.Name AS SupplierName,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
COUNT(DISTINCT P.Id) AS TotalInvoice,
SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM Suppliers S
INNER JOIN PurchaseOrders P ON S.Id = P.SupplierId
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
S.Id,
S.Name,
P.OrderDate,
P.DeliveryDateTime,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name
ORDER BY LineTotal DESC";
                            break;

                        case "Product Wise":
                            query = @"
SELECT 
PR.Id,
PR.Name AS ProductName,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
SUM(PD.Quantity) AS Quantity,
SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM Products PR
INNER JOIN PurchaseOrderDetails PD ON PR.Id = PD.ProductId
INNER JOIN PurchaseOrders P ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
PR.Id,
PR.Name,
P.OrderDate,
P.DeliveryDateTime,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name
ORDER BY LineTotal DESC";
                            break;

                        case "Invoice Wise":
                            query = @"
SELECT 
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
S.Name AS SupplierName,
SUM(PD.Quantity) AS Quantity,
SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
P.Code,
P.OrderDate,
P.DeliveryDateTime,
S.Name,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name
ORDER BY 
P.DeliveryDateTime,
P.OrderDate";
                            break;


                        case "Purchase Order List":
                            query = @"
SELECT 
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
S.Name AS SupplierName,
PR.Name AS ProductName,
SUM(PD.Quantity) AS Quantity,
SUM(PD.SubTotal) AS SubTotal,
SUM(PD.SDAmount) AS SDAmount,
SUM(PD.VATAmount) AS VAT,
SUM(PD.LineTotal) AS LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
P.Code,
P.OrderDate,
P.DeliveryDateTime,
S.Name,
PR.Name,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name
ORDER BY 
P.DeliveryDateTime,
P.OrderDate";
                            break;





                        default:
                            query = @"
SELECT 
    P.Code AS PurchaseOrderCode,
    FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
    FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
    S.Name AS SupplierName,
    PR.Name AS ProductName,
    PD.Quantity,
    PD.UnitPrice,
    PD.SubTotal,
    PD.SDAmount,
    PD.VATAmount,
    PD.LineTotal,
    P.CompanyId,
    P.BranchId,
    Co.CompanyName,
    B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
    AND P.OrderDate >= @fromDate
    AND P.OrderDate <= @toDate
    AND P.DeliveryDateTime >= @deliveryFromDate
    AND P.DeliveryDateTime <= @deliveryToDate
    AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
    AND (@ProductId = 0 OR PD.ProductId = @ProductId)
ORDER BY P.OrderDate, P.Code, PD.Line";
                            break;
                    }
                }

                else
                {
                    // =========================
                    // DETAILS MODE (ReportType Wise)
                    // =========================
                    switch (vm.ReportType)
                    {

                        case "Day Wise":
                            query = @"
SELECT 
FORMAT(CAST(P.OrderDate AS DATE),'dd-MMM-yyyy') AS OrderDate,
FORMAT(CAST(P.DeliveryDateTime AS DATE),'dd-MMM-yyyy') AS DeliveryDateTime,
P.Id AS PurchaseOrderId,
P.Code AS PurchaseOrderCode,
PD.Quantity,
PD.SubTotal,
PD.SDAmount,
PD.VATAmount,
PD.LineTotal,
P.CompanyId AS CompanyId,
P.BranchId AS BranchId,
Co.CompanyName,  
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
ORDER BY 
P.DeliveryDateTime,
P.OrderDate";
                            break;

                        case "Monthly":
                            query = @"
SELECT 
DATENAME(MONTH, P.OrderDate) + '-' + CAST(YEAR(P.OrderDate) AS VARCHAR(4)) AS MonthYear,
P.Id AS PurchaseOrderId,
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
PD.Quantity,
PD.SDAmount,
PD.VATAmount,
PD.LineTotal,
P.CompanyId AS CompanyId,
P.BranchId AS BranchId,
Co.CompanyName,  
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
GROUP BY 
YEAR(P.OrderDate),
MONTH(P.OrderDate),
DATENAME(MONTH, P.OrderDate),
P.Id, P.Code, P.OrderDate, P.DeliveryDateTime,
PD.Quantity, PD.LineTotal,
P.CompanyId, P.BranchId,
Co.CompanyName, B.Name
ORDER BY 
P.DeliveryDateTime,
P.OrderDate";
                            break;

                        case "Supplier Wise":
                            query = @"
SELECT 
S.Id,
P.Code AS PurchaseOrderCode,
S.Name AS SupplierName,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
PD.Quantity,
PD.UnitPrice AS UnitPrice,
PD.SubTotal,
PD.SDAmount,
PD.VATAmount,

PD.LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM Suppliers S
INNER JOIN PurchaseOrders P ON S.Id = P.SupplierId
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)";
                            break;

                        case "Product Wise":
                            query = @"
SELECT 
PR.Id,
PR.Name AS ProductName,
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
PD.Quantity,
PD.UnitPrice AS UnitRate,
PD.SubTotal,
PD.SDAmount,
PD.VATAmount,

PD.LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM Products PR
INNER JOIN PurchaseOrderDetails PD ON PR.Id = PD.ProductId
INNER JOIN PurchaseOrders P ON P.Id = PD.PurchaseOrderId
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)";
                            break;

                        case "Invoice Wise":
                            query = @"
SELECT 
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
S.Name AS SupplierName,
PR.Name AS ProductName,
PD.Quantity,
PD.UnitPrice AS UnitPrice,
PD.SubTotal,
PD.SDAmount,
PD.VATAmount,

PD.LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)";
                            break;


                        case "Purchase Order List":
                            query = @"
SELECT
P.Code AS PurchaseOrderCode,

FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,

S.Name AS SupplierName,
PR.Name AS ProductName,

PD.Quantity,
PD.UnitPrice,

PD.SubTotal,

PD.SDAmount,

PD.VATAmount,

PD.LineTotal,

P.CompanyId,
P.BranchId,

Co.CompanyName,
B.Name AS BranchName

FROM PurchaseOrders P

INNER JOIN Suppliers S
    ON P.SupplierId = S.Id

INNER JOIN PurchaseOrderDetails PD
    ON P.Id = PD.PurchaseOrderId

INNER JOIN Products PR
    ON PD.ProductId = PR.Id

INNER JOIN CompanyProfiles Co
    ON P.CompanyId = Co.Id

INNER JOIN BranchProfiles B
    ON P.BranchId = B.Id

WHERE 1 = 1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate

AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate

AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)

ORDER BY
P.OrderDate,
P.Code";
                            break;




                        case "Details":
                            query = @"
SELECT 
P.Id AS PurchaseOrderId,
P.Code AS PurchaseOrderCode,
FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
CAST(P.OrderDate AS DATE) AS OrderDate,
CAST(P.DeliveryDateTime AS DATE) AS DeliveryDateTime,
S.Id AS SupplierId,
S.Name AS SupplierName,
PR.Id AS ProductId,
PR.Name AS ProductName,
PD.Quantity,
PD.UnitPrice AS UnitRate,
PD.SubTotal,
PD.SDAmount,
PD.VATAmount,

PD.LineTotal,
P.CompanyId,
P.BranchId,
Co.CompanyName,
B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)";
                            break;

                        default:
                            query = @"
SELECT 
    P.Code AS PurchaseOrderCode,
    FORMAT(P.OrderDate,'dd-MMM-yyyy') AS OrderDate,
    FORMAT(P.DeliveryDateTime,'dd-MMM-yyyy') AS DeliveryDateTime,
    S.Name AS SupplierName,
    PR.Name AS ProductName,
    PD.Quantity,
    PD.UnitPrice,
    PD.SubTotal,
    PD.SDAmount,
    PD.VATAmount,
    PD.LineTotal,
    P.CompanyId,
    P.BranchId,
    Co.CompanyName,
    B.Name AS BranchName
FROM PurchaseOrders P
INNER JOIN Suppliers S ON P.SupplierId = S.Id
INNER JOIN PurchaseOrderDetails PD ON P.Id = PD.PurchaseOrderId
INNER JOIN Products PR ON PD.ProductId = PR.Id
INNER JOIN CompanyProfiles Co ON P.CompanyId = Co.Id
INNER JOIN BranchProfiles B ON P.BranchId = B.Id
WHERE 1=1
AND P.OrderDate >= @fromDate
AND P.OrderDate <= @toDate
AND P.DeliveryDateTime >= @deliveryFromDate
AND P.DeliveryDateTime <= @deliveryToDate
AND (@SupplierId = 0 OR P.SupplierId = @SupplierId)
AND (@ProductId = 0 OR PD.ProductId = @ProductId)
ORDER BY P.OrderDate, P.Code, PD.Line";
                            break;
                    }
                }
                // Apply additional conditions
                if (!query.ToUpper().Contains("WHERE"))
                {
                    query += " WHERE 1=1 ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                //query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                // Ensure correct date formats are passed to SQL query
                objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId ?? 0);
                objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", (vm.SupplierId));
                objComm.SelectCommand.Parameters.AddWithValue("@fromDate", DateTime.Parse(vm.OrderFromDate));
                objComm.SelectCommand.Parameters.AddWithValue("@toDate", DateTime.Parse(vm.OrderToDate));
                objComm.SelectCommand.Parameters.AddWithValue("@deliveryFromDate", DateTime.Parse(vm.DeliveryFromDate));
                objComm.SelectCommand.Parameters.AddWithValue("@deliveryToDate", DateTime.Parse(vm.DeliveryToDate));

                objComm.Fill(dataTable);


                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseOrderReportVM
                {
                    //Id = row.Field<int>("Id"),
                    Id = dataTable.Columns.Contains("PurchaseOrderId")
                        ? row.Field<int>("PurchaseOrderId")
                        : 0,

                    //Code = row.Field<string>("PurchaseCode"),
                    Code = dataTable.Columns.Contains("PurchaseOrderCode")
                        ? row["PurchaseOrderCode"]?.ToString()
                        : "",

                    SupplierName = dataTable.Columns.Contains("SupplierName")
                        ? row["SupplierName"]?.ToString()
                        : "",

                    ProductName = dataTable.Columns.Contains("ProductName")
                        ? row["ProductName"]?.ToString()
                        : "",

                    BranchId = dataTable.Columns.Contains("BranchId")
                        ? Convert.ToInt32(row["BranchId"])
                        : 0,

                    CompanyId = dataTable.Columns.Contains("CompanyId")
                        ? Convert.ToInt32(row["CompanyId"])
                        : 0,

                    BranchName = dataTable.Columns.Contains("BranchName")
                        ? row["BranchName"]?.ToString()
                        : "",

                    CompanyName = dataTable.Columns.Contains("CompanyName")
                        ? row["CompanyName"]?.ToString()
                        : "",

                    Quantity = dataTable.Columns.Contains("Quantity")
                        ? row.Field<decimal?>("Quantity") ?? 0.0m
                        : 0.0m,

                    UnitPrice = dataTable.Columns.Contains("UnitPrice")
                        ? row.Field<decimal?>("UnitPrice") ?? 0.0m
                        : 0.0m,

                    SubTotal = dataTable.Columns.Contains("SubTotal")
                        ? row.Field<decimal?>("SubTotal") ?? 0.0m
                        : 0.0m,

                    TotalInvoice = dataTable.Columns.Contains("TotalInvoice")
                        ? row.Field<int?>("TotalInvoice") ?? 0
                        : 0,

                    SD = dataTable.Columns.Contains("SD")
                        ? row.Field<decimal?>("SD") ?? 0.0m
                        : 0.0m,

                    SDAmount = dataTable.Columns.Contains("SDAmount")
                        ? row.Field<decimal?>("SDAmount") ?? 0.0m
                        : 0.0m,

                    VATRate = dataTable.Columns.Contains("VATRate")
                        ? row.Field<decimal?>("VATRate") ?? 0.0m
                        : 0.0m,

                    VATAmount = dataTable.Columns.Contains("VATAmount")
                        ? row.Field<decimal?>("VATAmount") ?? 0.0m
                        : dataTable.Columns.Contains("VAT")
                            ? row.Field<decimal?>("VAT") ?? 0.0m
                            : 0.0m,

                    LineTotal = dataTable.Columns.Contains("LineTotal")
                        ? row.Field<decimal?>("LineTotal") ?? 0.0m
                        : 0.0m,

                    OrderDate = dataTable.Columns.Contains("OrderDate")
                        ? row["OrderDate"]?.ToString()
                        : dataTable.Columns.Contains("MonthYear")
                            ? row["MonthYear"]?.ToString()
                            : "",

                    DeliveryDateTime = dataTable.Columns.Contains("DeliveryDateTime")
                        ? row["DeliveryDateTime"]?.ToString()
                        : ""
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
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




    }
}
