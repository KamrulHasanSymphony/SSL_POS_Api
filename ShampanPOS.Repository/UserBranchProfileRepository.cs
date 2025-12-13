using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanPOS.Repository
{
    public class UserBranchProfileRepository : CommonRepository
    {
        // Insert Method
         public async Task<ResultVM> Insert(UserBranchProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string selectQuery = @"SELECT COUNT(Id) FROM UserBranchMap WHERE UserId = @UserId AND BranchId = @BranchId";
                var selectCommand = CreateCommand(selectQuery,conn, transaction);
                selectCommand.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(vm.UserId) ? "" : vm.UserId.Trim();
                selectCommand.Parameters.Add("@BranchId", SqlDbType.BigInt).Value = vm.BranchId;

                int count = Convert.ToInt32(selectCommand.ExecuteScalar());
                if (count > 0)
                {
                    result.Message = "Data Already Exist!";
                    return result;
                }

                string query = @"
INSERT INTO UserBranchMap 
(

 UserId
,BranchId
,CreatedBy
,CreatedOn
,CreatedFrom
 
)
VALUES 
(

 @UserId
,@BranchId
,@CreatedBy
,GETDATE()
,@CreatedFrom

);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value); 
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id =vm.Id.ToString();
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
         public async Task<ResultVM> Update(UserBranchProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string selectQuery = @"SELECT COUNT(Id) FROM UserBranchMap WHERE UserId = @UserId AND BranchId = @BranchId AND Id != @Id ";
                var selectCommand = CreateCommand(selectQuery, conn, transaction);
                selectCommand.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = string.IsNullOrEmpty(vm.UserId) ? "" : vm.UserId.Trim();
                selectCommand.Parameters.Add("@BranchId", SqlDbType.BigInt).Value = vm.BranchId;
                selectCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;

                int count = Convert.ToInt32(selectCommand.ExecuteScalar());
                if (count > 0)
                {
                    result.Message = "Data Already Exist!";
                    return result;
                }

                string query = @"
UPDATE UserBranchMap
SET
    
 BranchId=@BranchId
,LastModifiedBy=@LastModifiedBy
,LastModifiedOn=GETDATE()
,LastUpdateFrom=@LastUpdateFrom

WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.Id;
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }


                string query = $@"
SELECT
UM.Id,
U.UserName AS UserId,
UM.BranchId,
B.Name BranchName,
B.Code BranchCode,
B.IsActive,
ISNULL(UM.CreatedBy, '') AS CreatedBy,
ISNULL(UM.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(UM.CreatedFrom, '') AS CreatedFrom,
ISNULL(UM.LastUpdateFrom, '') AS LastUpdateFrom,
ISNULL(FORMAT(UM.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS LastModifiedOn,
ISNULL(FORMAT(UM.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS CreatedOn

FROM {DatabaseHelper.AuthDbName()}.[dbo].AspNetUsers U
LEFT OUTER JOIN UserBranchMap UM on UM.UserId=U.Id
LEFT OUTER JOIN BranchProfiles B on UM.BranchId=B.Id
WHERE UM.UserId IS NOT NULL

";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND UM.Id = @Id ";
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

                var modelList = dataTable.AsEnumerable().Select(row => new UserBranchProfileVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    BranchId = Convert.ToInt32(row["BranchId"]),
                    UserId = row["UserId"].ToString(),
                    //UserName = row["UserName"].ToString(),
                    BranchName = row["BranchName"].ToString(),
                    BranchCode = row["BranchCode"].ToString(),
                    Name = row["BranchName"].ToString(),
                    Code = row["BranchCode"].ToString(),
                    CreatedBy = row["CreatedBy"].ToString(),
                    CreatedOn = row["CreatedOn"].ToString(),
                    LastModifiedBy = row["LastModifiedBy"].ToString(),
                    LastModifiedOn = row["LastModifiedOn"].ToString(),
                    CreatedFrom = row["CreatedFrom"].ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"].ToString(),                   
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;

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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = $@"
SELECT
UM.Id,
U.UserName,
UM.UserId,
UM.BranchId,
B.Name BranchName,
B.Code BranchCode,
ISNULL(UM.CreatedBy, '') AS CreatedBy,
ISNULL(UM.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(UM.CreatedFrom, '') AS CreatedFrom,
ISNULL(UM.LastUpdateFrom, '') AS LastUpdateFrom,
ISNULL(FORMAT(UM.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS LastModifiedOn,
ISNULL(FORMAT(UM.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS CreatedOn,

FROM [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers U
LEFT OUTER JOIN UserBranchMap UM on UM.UserId=U.Id
LEFT OUTER JOIN BranchProfiles B on UM.BranchId=B.Id
WHERE UM.UserId IS NOT NULL

";

                DataTable dataTable = new DataTable();

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

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
                SELECT Id, UserId Name
                FROM UserBranchMap
                WHERE 1 = 1
                ORDER BY UserId ";

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


        public async Task<ResultVM> GetGridData(GridOptions options, string userId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<UserBranchProfileVM>();

                // Define your SQL query string with dynamic SalePersonId condition
                string sqlQuery = $@"
        -- Count query
                    SELECT COUNT(DISTINCT UM.Id) AS totalcount
                    FROM [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers U
                    LEFT OUTER JOIN UserBranchMap UM ON UM.UserId=U.Id
                    LEFT OUTER JOIN BranchProfiles B ON UM.BranchId=B.Id
                    WHERE UM.UserId IS NOT NULL  
        -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserBranchProfileVM>.FilterCondition(options.filter) + ")" : "");

                if (!string.IsNullOrEmpty(userId))
                {
                    sqlQuery += " AND UM.UserId = '" + userId + "' ";
                }

                // Data query with pagination and sorting
                sqlQuery += @"
        -- Data query with pagination and sorting
        SELECT * 
        FROM (
            SELECT 
            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "UM.Id DESC ") + $@") AS rowindex,
            UM.Id,
            U.UserName,
            UM.UserId,
            UM.BranchId,
            B.Name BranchName,
            B.Code BranchCode
            FROM [{DatabaseHelper.AuthDbName()}].[dbo].AspNetUsers U
            LEFT OUTER JOIN UserBranchMap UM ON UM.UserId=U.Id
            LEFT OUTER JOIN BranchProfiles B ON UM.BranchId=B.Id
            WHERE UM.UserId IS NOT NULL
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserBranchProfileVM>.FilterCondition(options.filter) + ")" : "");

                if (!string.IsNullOrEmpty(userId))
                {
                    sqlQuery += " AND UM.UserId = '" + userId + "' ";
                }

                sqlQuery += @"
        ) AS a
        WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                // Now you can pass options without the SqlParameter
                data = KendoGrid<UserBranchProfileVM>.GetGridData_CMD(options, sqlQuery, "UM.Id");

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
