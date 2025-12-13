using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
   
    public class SalePersonRouteRepository : CommonRepository
    {


        // Insert Method
        public async Task<ResultVM> Insert(SalePersonRouteVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO SalePersonRoutes 
        (
            SalePersonId, BranchId, RouteId, IsArchive, IsActive, 
            CreatedBy, CreatedOn
        )
        VALUES 
        (
            @SalePersonId, @BranchId, @RouteId, @IsArchive, @IsActive, 
            @CreatedBy, @CreatedOn
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@RouteId", vm.RouteId);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
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


        // Update Method
        public async Task<ResultVM> Update(SalePersonRouteVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE SalePersonRoutes 
        SET 
            SalePersonId = @SalePersonId, BranchId = @BranchId, RouteId = @RouteId, 
            IsArchive = @IsArchive, IsActive = @IsActive, 
            LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE()
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@RouteId", vm.RouteId);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        throw new Exception(result.Message);
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

                string query = $" UPDATE SalePersonRoutes SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.Id, 0) Id,
    --ISNULL(M.Code, '') Code,
    ISNULL(M.SalePersonId, 0) SalePersonId,
    ISNULL(SP.Name, '') AS SalePersonName,  
    ISNULL(M.BranchId, 0) BranchId,
    ISNULL(M.RouteId, 0) RouteId,
    ISNULL(M.IsArchive, 0) IsArchive,
    ISNULL(M.IsActive, 0) IsActive,
    ISNULL(M.CreatedBy, '') CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
    ISNULL(M.LastModifiedBy, '') LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
FROM SalePersonRoutes M
LEFT JOIN SalesPersons SP ON M.SalePersonId = SP.Id
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

                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonRouteVM
                {
                    Id = row.Field<int>("Id"),
                    SalePersonId = row.Field<int>("SalePersonId"),
                    BranchId = row.Field<int>("BranchId"),
                    RouteId = row.Field<int>("RouteId"),
                    SalePersonName = row.Field<string>("SalePersonName"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    IsActive = row.Field<bool>("IsActive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn")
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
    Id,
    SalePersonId,
    BranchId,
    RouteId,
    IsArchive,
    IsActive,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn
FROM SalePersonRoute
WHERE 1 = 1";

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
FROM SalePersonRoute
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
        public async Task<ResultVM> GetGridData(GridOptions options,string salePersonId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SalePersonRouteVM>();

                // Define your SQL query string with dynamic SalePersonId condition
                string sqlQuery = @"
        -- Count query
        SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM SalePersonRoutes H
        LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        LEFT OUTER JOIN BranchProfiles B ON H.BranchId = B.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        WHERE H.IsArchive != 1  
        -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonRouteVM>.FilterCondition(options.filter) + ")" : "");

                // Conditionally add SalePersonId filter if salePersonId is not "0"
                if (!string.IsNullOrEmpty(salePersonId) && salePersonId != "0")
                {
                    sqlQuery += " AND H.SalePersonId = " + salePersonId; // Directly insert salePersonId in the query
                }

                // Data query with pagination and sorting
                sqlQuery += @"
        -- Data query with pagination and sorting
        SELECT * 
        FROM (
            SELECT 
            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.SalePersonId, 0) AS SalePersonId,
            ISNULL(SP.Name, '') AS SalePersonName,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(B.Name, '') AS BranchName,
            ISNULL(H.RouteId, 0) AS RouteId,
            ISNULL(R.Name, '') AS RouteName,
            ISNULL(H.IsArchive, 0) AS IsArchive,
            CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
            ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
        FROM SalePersonRoutes H
        LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        LEFT OUTER JOIN BranchProfiles B ON H.BranchId = B.Id
        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        WHERE H.IsArchive != 1
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonRouteVM>.FilterCondition(options.filter) + ")" : "");

                // Conditionally add SalePersonId filter if salePersonId is not "0"
                if (!string.IsNullOrEmpty(salePersonId) && salePersonId != "0")
                {
                    sqlQuery += " AND H.SalePersonId = " + salePersonId; // Directly insert salePersonId in the query
                }

                sqlQuery += @"
        ) AS a
        WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                // Now you can pass options without the SqlParameter
                data = KendoGrid<SalePersonRouteVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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



        public async Task<bool> IsRouteAlreadyAssigned(SalePersonRouteVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;

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
                                SELECT COUNT(*) 
                                FROM SalePersonRoutes 
                                WHERE
                                SalePersonId = @SalePersonId
                                AND RouteId = @RouteId 
                                AND BranchId = @BranchId
                                AND IsActive = 1;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@RouteId", vm.RouteId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                    return count > 0; 
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        //public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null,string salePersonId)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        var data = new GridEntity<SalePersonRouteVM>();

        //        // Define your SQL query string
        //        string sqlQuery = @"
        //    -- Count query
        //    SELECT COUNT(DISTINCT H.Id) AS totalcount
        //    FROM SalePersonRoutes H
        //    LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        //    LEFT OUTER JOIN BranchProfiles B ON H.BranchId = B.Id
        //    LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        //    WHERE H.IsArchive != 1  
        //    -- Add the filter condition
        //    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonRouteVM>.FilterCondition(options.filter) + ")" : "") + @"

        //    -- Data query with pagination and sorting
        //    SELECT * 
        //    FROM (
        //        SELECT 
        //        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
        //       ISNULL(H.Id, 0) AS Id,

        //        ISNULL(H.SalePersonId, 0) AS SalePersonId,
        //        ISNULL(SP.Name, '') AS SalePersonName,
        //        ISNULL(H.BranchId, 0) AS BranchId,
        //        ISNULL(B.Name, '') AS BranchName,
        //        ISNULL(H.RouteId, 0) AS RouteId,
        //        ISNULL(R.Name, '') AS RouteName,
        //        ISNULL(H.IsArchive, 0) AS IsArchive,
        //        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        //        ISNULL(H.CreatedBy, '') AS CreatedBy,
        //        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        //        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        //        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
        //    FROM SalePersonRoutes H
        //    LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        //    LEFT OUTER JOIN BranchProfiles B ON H.BranchId = B.Id
        //    LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        //    WHERE H.IsArchive != 1
        //    -- Add the filter condition
        //    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonRouteVM>.FilterCondition(options.filter) + ")" : "") + @"

        //    ) AS a
        //    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        //";

        //        data = KendoGrid<SalePersonRouteVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = data;

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
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



    }

}
