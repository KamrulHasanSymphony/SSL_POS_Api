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
        public async Task<ResultVM> Insert(MasterSupplierItemVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                     MasterSupplierId,MasterProductId,UserId,CompanyId,
                     IsArchive, IsActive, CreatedBy, CreatedOn
                )
                VALUES
                (
                    @MasterSupplierId,@MasterProductId,@UserId,@CompanyId,
                     @IsArchive, @IsActive, @CreatedBy, @CreatedOn 
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@MasterSupplierId", vm.MasterSupplierId);
                    cmd.Parameters.AddWithValue("@MasterProductId", vm.MasterProductId);

                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = vm.Id.ToString();
                    result.DataVM = vm;


                    if (isNewConnection)
                    {
                        transaction.Commit();
                    }

                    return result;
                }
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
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn

FROM MasterSupplierItem M
LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
LEFT OUTER JOIN MasterItem p on M.MasterProductId = p.Id
WHERE 1 = 1
 ";


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions
                // 

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<MasterSupplierItemVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new MasterSupplierItemVM
                    {
                        Id = row.Field<int>("Id"),
                        MasterSupplierId = row.Field<int>("MasterSupplierId"),
                        MasterProductId = row.Field<int>("MasterProductId"),
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


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = model;
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
    SELECT COUNT(DISTINCT M.Id) AS totalcount
FROM MasterSupplierItem M
LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
LEFT OUTER JOIN MasterItem p on M.MasterProductId = p.Id
WHERE 1 = 1 ";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                                        (options.sort.Count > 0 ?
                                            options.sort[0].field + " " + options.sort[0].dir :
                                            "M.Id DESC ") + @") AS rowindex,
        
    ISNULL(M.Id, 0) AS Id,
	ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
	ISNULL(M.MasterProductId, 0) AS MasterItemId,
    ISNULL(s.Name, '') AS MasterSupplierName,
	ISNULL(M.UserId, 0) AS UserId,
    ISNULL(p.Name, '') AS MasterProductName,
    ISNULL(M.IsArchive, 0) AS IsArchive,
	ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn

FROM MasterSupplierItem M
LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
LEFT OUTER JOIN MasterItem p on M.MasterProductId = p.Id
WHERE 1 = 1
       ";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
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
