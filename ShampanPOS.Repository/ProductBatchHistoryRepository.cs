using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    
    public class ProductBatchHistoryRepository : CommonRepository
    {
        // Insert Method
        
        public async Task<ResultVM> Insert(ProductBatchHistoryVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO ProductSalePriceBatchHistories
        (
            ProductId, BranchId, BatchNo,PriceCategory, EntryDate, MFGDate, EXPDate,EffectDate, 
            CostPrice, SalesPrice, PurchasePrice,SD,VATRate, CreatedBy, CreatedOn,CreatedFrom
        )
        VALUES 
        (
            @ProductId, @BranchId, @BatchNo,@PriceCategory, @EntryDate, @MFGDate, @EXPDate, @EffectDate,
            @CostPrice, @SalesPrice, @PurchasePrice,@SD,@VATRate, @CreatedBy, @CreatedOn,@CreatedFrom
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@BatchNo", vm.BatchNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PriceCategory", vm.PriceCategory ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EntryDate", vm.EntryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MFGDate", vm.MFGDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EXPDate", vm.EXPDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EffectDate", vm.EffectDate);
                    cmd.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                    cmd.Parameters.AddWithValue("@SalesPrice", vm.SalesPrice);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmd.Parameters.AddWithValue("@PurchasePrice", vm.PurchasePrice);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = vm.Id.ToString();
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
                result.Message = "Error in Insert.";
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


        // Update Method

        public async Task<ResultVM> Update(ProductBatchHistoryVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE ProductSalePriceBatchHistories
SET 
    --ProductId = @ProductId, 
    --BranchId = @BranchId, 
    BatchNo = @BatchNo, 
    PriceCategory = @PriceCategory, 
    EntryDate = @EntryDate, 
    MFGDate = @MFGDate, 
    EXPDate = @EXPDate, 
    EffectDate = @EffectDate, 
    CostPrice = @CostPrice, 
    SalesPrice = @SalesPrice, 
    PurchasePrice = @PurchasePrice, 
    SD=@SD,
    VATRate=@VATRate,
     
    LastModifiedBy = @LastModifiedBy, 
    LastModifiedOn = GETDATE(),
    LastUpdateFrom = @LastUpdateFrom
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Non-nullable fields
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@EntryDate", vm.EntryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MFGDate", vm.MFGDate ?? (object)DBNull.Value );
                    cmd.Parameters.AddWithValue("@EXPDate", vm.EXPDate ?? (object)DBNull.Value );
                    cmd.Parameters.AddWithValue("@EffectDate", vm.EffectDate);
                    cmd.Parameters.AddWithValue("@CostPrice", vm.CostPrice);
                    cmd.Parameters.AddWithValue("@SalesPrice", vm.SalesPrice);
                    cmd.Parameters.AddWithValue("@PurchasePrice", vm.PurchasePrice);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);


                    // Add nullable fields with null checks
                    cmd.Parameters.AddWithValue("@BatchNo", vm.BatchNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PriceCategory", vm.PriceCategory ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                   

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

                result.ExMessage = ex.Message;
                result.Message = "Error in Update.";
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


        // Delete Method
        
        public async Task<ResultVM> Delete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

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

                string query = $" UPDATE ProductSalePriceBatchHistories SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
                        result.Message = $"Data deleted successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were deleted.");
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

        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @"
                SELECT
                ISNULL(H.Id, 0) AS Id, 
                ISNULL(H.ProductId, 0) AS ProductId, 
                ISNULL(P.Name, '') AS ProductName,
                ISNULL(P.Code, '') AS ProductCode,
                ISNULL(H.BranchId, 0) AS BranchId, 
                ISNULL(H.BatchNo, '') AS BatchNo,
                ISNULL(H.PriceCategory, '') AS PriceCategory,
                ISNULL(FORMAT(H.EntryDate, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS EntryDate,
                ISNULL(FORMAT(H.MFGDate, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS MFGDate,
                ISNULL(FORMAT(H.EXPDate, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS EXPDate,
                ISNULL(FORMAT(H.EffectDate, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS EffectDate,
                ISNULL(H.CostPrice, 0) AS CostPrice, 
                ISNULL(H.SalesPrice, 0) AS SalesPrice, 
                ISNULL(H.PurchasePrice, 0) AS PurchasePrice, 
 
                H.CreatedBy, 
                H.LastModifiedBy, 
                ISNULL(H.SD,0) SD,
                ISNULL(H.SDAmount,0) SDAmount,
                ISNULL(H.VATAmount,0) VATAmount,
                ISNULL(H.VATRate,0) VATRate,

                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
 
                FROM ProductSalePriceBatchHistories H  
                LEFT OUTER JOIN Products P on H.ProductId = p.Id
                WHERE 1 = 1";


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductBatchHistoryVM
                {
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    ProductName = row.Field<string>("ProductName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    BranchId = row.Field<int>("BranchId"),
                    BatchNo = row.Field<string>("BatchNo"),
                    PriceCategory = row.Field<string>("PriceCategory"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    SD = row.Field<decimal>("SD"),
                    SDAmount = row.Field<decimal>("SDAmount"),
                    VATRate = row.Field<decimal>("VATRate"),
                    VATAmount = row.Field<decimal>("VATAmount"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedOn = row.Field<string>("LastModifiedOn"),
                    EntryDate = row.Field<string>("EntryDate"),
                    MFGDate = row.Field<string>("MFGDate"),
                    EXPDate = row.Field<string>("EXPDate"),
                    EffectDate = row.Field<string>("EffectDate"),

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error in List.";
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

        // GetEffectDateExist Method
        public async Task<ResultVM> GetEffectDateExist(ProductBatchHistoryVM productBatchHistory, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT 1
FROM ProductSalePriceBatchHistories
WHERE CAST(EffectDate AS DATE) = @EffectDate
AND ProductId = @ProductId  AND BranchId = @BranchId ";

                if(productBatchHistory.Id > 0)
                {
                    query += " AND Id != @Id ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                        adapter.SelectCommand.Parameters.AddWithValue("@EffectDate", productBatchHistory.EffectDate);
                        adapter.SelectCommand.Parameters.AddWithValue("@ProductId", productBatchHistory.ProductId);
                        adapter.SelectCommand.Parameters.AddWithValue("@BranchId", productBatchHistory.BranchId);
                        adapter.SelectCommand.Parameters.AddWithValue("@Id", productBatchHistory.Id);
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
    Id, ProductId, BranchId, BatchNo, EntryDate, MFGDate, EXPDate,EffectDate, 
    CostPrice, SalesPrice, PurchasePrice, CreatedBy, CreatedOn, 
    LastModifiedBy, LastModifiedOn
FROM ProductSalePriceBatchHistories WHERE 1 = 1";


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
                result.Message = "Error in ListAsDataTable.";
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
SELECT Id, Name
FROM  ProductSalePriceBatchHistories WHERE 1 = 1"";

WHERE IsActive = 1
ORDER BY Name";

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

        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<ProductBatchHistoryVM>();

                string sqlQuery = @"
    -- Count query
       SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM ProductSalePriceBatchHistories H
        LEFT OUTER JOIN Products P on H.ProductId = p.Id
        LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductBatchHistoryVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
                {
                    sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
                }
                sqlQuery += @"
                -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.ProductId, 0) AS ProductId,
	        ISNULL(PG.Code,'') ProductGroupCode,
	        ISNULL(PG.Name,'') ProductGroupName,
            ISNULL(P.Code, '') AS ProductCode,
            ISNULL(P.Name, '') AS ProductName,
	        ISNULL(P.BanglaName,'') BanglaName, 
	        ISNULL(P.HSCodeNo,'') HSCodeNo,
	        ISNULL(P.Description,'') ProductDescription,
	        ISNULL(UOM.Name,'') UOMName,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(H.BatchNo, '') AS BatchNo,
            ISNULL(FORMAT(H.EntryDate, 'yyyy-MM-dd'), '') AS EntryDate,
            ISNULL(FORMAT(H.MFGDate, 'yyyy-MM-dd'), '') AS MFGDate,
            ISNULL(FORMAT(H.EXPDate, 'yyyy-MM-dd'), '') AS EXPDate,
            ISNULL(FORMAT(H.EffectDate, 'yyyy-MM-dd'), '') AS EffectDate,
            ISNULL(H.CostPrice, 0) AS CostPrice,
            ISNULL(H.SalesPrice, 0) AS SalesPrice,
            ISNULL(H.PurchasePrice, 0) AS PurchasePrice,
            ISNULL(H.SD,0) SD,
            ISNULL(H.SDAmount,0) SDAmount,
            ISNULL(H.VATAmount,0) VATAmount,
            ISNULL(H.VATRate,0) VATRate,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
    
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom

        FROM ProductSalePriceBatchHistories H
        LEFT OUTER JOIN Products P on H.ProductId = p.Id
        LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        WHERE 1 = 1
        -- Add the filter condition
           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductBatchHistoryVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
                {
                    //sqlQuery += " AND H.ProductId = " + ProductId;
                    sqlQuery += " AND H.ProductId = " + ProductId + " AND H.BranchId = " + options.vm.BranchId;

                }
                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";


                data = KendoGrid<ProductBatchHistoryVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> GetProductBatchHistoryById(GridOptions options, int ProductId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<ProductBatchHistoryVM>();

                string sqlQuery = $@"
    -- Count query
       SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM {options.vm.TableName} H
        LEFT OUTER JOIN Products P on H.ProductId = p.Id
        LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductBatchHistoryVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
                if (ProductId > 0 )
                {
                    sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
                }
                sqlQuery += $@"
                -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + $@") AS rowindex,
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.ProductId, 0) AS ProductId,
	        ISNULL(PG.Code,'') ProductGroupCode,
	        ISNULL(PG.Name,'') ProductGroupName,
            ISNULL(P.Code, '') AS ProductCode,
            ISNULL(P.Name, '') AS ProductName,
	        ISNULL(P.BanglaName,'') BanglaName, 
            CASE WHEN ISNULL(P.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
	        ISNULL(P.HSCodeNo,'') HSCodeNo,
	        ISNULL(P.Description,'') ProductDescription,
	        ISNULL(UOM.Name,'') UOMName,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(H.BatchNo, '') AS BatchNo,
            ISNULL(FORMAT(H.EntryDate, 'yyyy-MM-dd'), '') AS EntryDate,
            ISNULL(FORMAT(H.MFGDate, 'yyyy-MM-dd'), '') AS MFGDate,
            ISNULL(FORMAT(H.EXPDate, 'yyyy-MM-dd'), '') AS EXPDate,
            ISNULL(FORMAT(H.EffectDate, 'yyyy-MM-dd'), '') AS EffectDate,
            ISNULL(H.CostPrice, 0) AS CostPrice,
            ISNULL(H.SalesPrice, 0) AS SalesPrice,
            ISNULL(H.PurchasePrice, 0) AS PurchasePrice,
            ISNULL(H.SD,0) SD,
            ISNULL(H.SDAmount,0) SDAmount,
            ISNULL(H.VATAmount,0) VATAmount,
            ISNULL(H.VATRate,0) VATRate,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
    
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom


            FROM {options.vm.TableName} H
            LEFT OUTER JOIN Products P on H.ProductId = p.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
            WHERE 1 = 1
        -- Add the filter condition
           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductBatchHistoryVM>.FilterCondition(options.filter) + ")" : "");
                if (ProductId > 0)
                {
                    sqlQuery += " AND H.ProductId = " + ProductId;
                }
                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";


                data = KendoGrid<ProductBatchHistoryVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
