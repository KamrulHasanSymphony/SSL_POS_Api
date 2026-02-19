using Newtonsoft.Json;
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
    public class MasterSupplierItemRepository : CommonRepository
    {

        // Insert Method
        public async Task<ResultVM> Insert( MasterItemVM details = null, SqlConnection conn = null, SqlTransaction transaction = null, MasterSupplierItemVM mastersupplieritem = null)
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
                    INSERT INTO MasterSupplierItem
                    (
                        MasterSupplierId,
                        MasterProductId,
                        UserId,
                        CompanyId,
                        IsArchive,
                        IsActive,
                        CreatedBy,
                        CreatedOn,
                        CreatedFrom
                    )
                    VALUES
                    (
                        @MasterSupplierId,
                        @MasterProductId,
                        @UserId,
                        @CompanyId,
                        @IsArchive,
                        @IsActive,
                        @CreatedBy,
                        @CreatedOn,
                        @CreatedFrom
                    );
                    SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@MasterSupplierId", details.MasterSupplierId);
                        cmd.Parameters.AddWithValue("@MasterProductId", details.ProductId);
                        cmd.Parameters.AddWithValue("@UserId", mastersupplieritem.UserId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompanyId", mastersupplieritem.CompanyId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchive", details.IsArchive);
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@CreatedBy", mastersupplieritem.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedFrom", mastersupplieritem.CreatedFrom ?? (object)DBNull.Value);

                    details.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                //if (isNewConnection)
                //{
                //    transaction.Commit();
                //}

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.DataVM = details;

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

                result.ExMessage = ex.ToString();
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
        public async Task<ResultVM> Update(MasterSupplierItemVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE MasterSupplierItem
SET 

    MasterSupplierId = @MasterSupplierId,
    MasterProductId = @MasterProductId,
    UserId = @UserId,
    CompanyId = @CompanyId,
    IsArchive = @IsArchive,
    IsActive = @IsActive,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = GETDATE(),
    CreatedFrom = @CreatedFrom,
    LastUpdateFrom = @LastUpdateFrom

WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@MasterSupplierId", vm.MasterSupplierId);
                    cmd.Parameters.AddWithValue("@MasterProductId", vm.MasterProductId);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
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

                string query = $" UPDATE MasterSupplierItem SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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


        /// List Method




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
            ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
            ISNULL(M.MasterProductId, 0) AS MasterProductId,
            ISNULL(s.Name, '') AS MasterSupplierName,
            ISNULL(M.UserId, 0) AS UserId,
            ISNULL(p.Name, '') AS MasterProductName,
            ISNULL(M.IsArchive, 0) AS IsArchive,
            ISNULL(M.IsActive, 0) AS IsActive,   
            ISNULL(M.CreatedBy, '') AS CreatedBy,
            FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
            ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
            FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
            ISNULL(pg.Name, '') AS ProductGroupName  
	        --ISNULL(pg.Id,0) MasterItemGroupId

        FROM MasterSupplierItem M
        LEFT OUTER JOIN MasterSupplier s ON M.MasterSupplierId = s.Id
        LEFT OUTER JOIN MasterItem p ON M.MasterProductId = p.Id
        LEFT OUTER JOIN MasterItemGroup pg ON p.MasterItemGroupId = pg.Id  

        WHERE 1 = 1 ";

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

                var lst = new List<MasterSupplierItemVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new MasterSupplierItemVM
                    {
                        Id = row.Field<int>("Id"),
                        MasterSupplierId = row.Field<int>("MasterSupplierId"),
                        MasterProductId = row.Field<int>("MasterProductId"),
                        //MasterItemGroupId = row.Field<int>("MasterItemGroupId"),
                        MasterSupplierName = row.Field<string>("MasterSupplierName"),
                        MasterProductName = row.Field<string>("MasterProductName"),
                        UserId = row.Field<string>("UserId"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string>("LastModifiedOn")
                    });
                }
                foreach (var parent in lst)
                {
                    // pass single department id as conditional value
                    var detailsResult = DetailsList(new[] { "M.MasterSupplierId" }, new[] { parent.MasterSupplierId.ToString() }, vm, conn, transaction);

                    if (detailsResult.Status == "Success" && detailsResult.DataVM is DataTable dts)
                    {
                        string json = JsonConvert.SerializeObject(dts);
                        var details = JsonConvert.DeserializeObject<List<MasterItemVM>>(json);

                        parent.MasterItemList = details ?? new List<MasterItemVM>();
                    }
                    else
                    {
                        parent.MasterItemList = new List<MasterItemVM>();
                    }
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



        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail" };
            DataTable dataTable = new DataTable();
            try
            {
                string query = @"
        SELECT
        ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
        ISNULL(M.MasterProductId, 0) AS MasterProductId,
        ISNULL(s.Name, '') AS MasterSupplierName,
        ISNULL(M.UserId, 0) AS UserId,
    
        ISNULL(M.IsArchive, 0) AS IsArchive,
        ISNULL(M.IsActive, 0) AS IsActive,   
        ISNULL(M.CreatedBy, '') AS CreatedBy,
        FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
        ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
        FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
    
        --ISNULL(pg.Id,0) MasterItemGroupId,
        ISNULL(p.Code,'') Code,
	    ISNULL(p.Name, '') AS Name,
	    ISNULL(pg.Name, '') AS MasterItemGroupName,
        ISNULL(P.Id,0) AS Id

    FROM MasterSupplierItem M
    LEFT OUTER JOIN MasterSupplier s ON M.MasterSupplierId = s.Id
    LEFT OUTER JOIN MasterItem p ON M.MasterProductId = p.Id
    LEFT OUTER JOIN MasterItemGroup pg ON p.MasterItemGroupId = pg.Id  

    WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter da = CreateAdapter(query, conn, transaction);
                da.SelectCommand = ApplyParameters(da.SelectCommand, conditionalFields, conditionalValues);
                da.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
            }
            catch (Exception ex)
            {
                result.Message = MessageModel.DetailInsertFailed;
                result.ExMessage = ex.ToString();
            }
            return result;
        }






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
FROM MasterSupplierItem
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

                var data = new GridEntity<MasterSupplierItemVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(*) AS totalcount
FROM (
    SELECT DISTINCT
        M.MasterSupplierId,
        s.Name,
        M.UserId,
        M.IsArchive,
        M.IsActive,
        M.CreatedBy,
        --M.CreatedOn,
        M.LastModifiedBy,
        M.LastModifiedOn
    FROM MasterSupplierItem M
    LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
    WHERE 1 = 1
";

                sqlQuery += (options.filter.Filters.Count > 0 ?
                        " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) t

-- Data query
SELECT *
FROM (
    SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                        (options.sort.Count > 0 ?
                            options.sort[0].field + " " + options.sort[0].dir :
                            "MasterSupplierId DESC") + @") AS rowindex,

        *
    FROM (
        SELECT DISTINCT
            ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
            ISNULL(s.Name, '') AS MasterSupplierName,
            ISNULL(M.UserId, 0) AS UserId,
            ISNULL(M.IsArchive, 0) AS IsArchive,
            ISNULL(M.IsActive, 0) AS IsActive,   

            ISNULL(M.CreatedBy, '') AS CreatedBy,
            FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
            ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
            FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn

        FROM MasterSupplierItem M
        LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
        WHERE 1 = 1
";

                sqlQuery += (options.filter.Filters.Count > 0 ?
                        " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) d
) a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";




                data = KendoGrid<MasterSupplierItemVM>.GetGridData_CMD(options, sqlQuery, "M.Id");
                //data = KendoGrid<CustomerVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);
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
