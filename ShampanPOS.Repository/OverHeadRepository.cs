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
    public class OverHeadRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(OverHeadVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO OverHeads
(
    Code, OverHead,
    Comments, IsArchive, IsActive, CreatedBy, CreatedOn
)
VALUES
(
    @Code, @OverHead, 
    @Comments, @IsArchive, @IsActive, @CreatedBy, @CreatedOn 
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@OverHead", vm.OverHead);

                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
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
        public async Task<ResultVM> Update(OverHeadVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE OverHeads
SET 
    Code = @Code,
    OverHead = @OverHead,
    Comments = @Comments,   
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
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OverHead", vm.OverHead ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);

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

                string query = $" UPDATE OverHeads SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.OverHead, '') AS OverHead,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM OverHeads M
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

                var model = new List<OverHeadVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new OverHeadVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        OverHead = row.Field<string>("OverHead"),
                        Comments = row.Field<string>("Comments"),
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
    Code,
    Name,
    BanglaName,
    CustomerGroupId,
    Address,
    BanglaAddress,
    RouteId,
    AreaId,
    City,
    TelephoneNo,
    FaxNo,
    Email,
    TINNo,
    BINNo,
    NIDNo,
    Comments,
    IsArchive,
    IsActive,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    ISNULL(RegularDiscountRate, 0) AS RegularDiscountRate,

    LastModifiedOn
FROM Customers
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
FROM Customers
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

                var data = new GridEntity<OverHeadVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM OverHeads H 
        WHERE H.IsArchive != 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<OverHeadVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                                        (options.sort.Count > 0 ?
                                            options.sort[0].field + " " + options.sort[0].dir :
                                            "H.Id DESC ") + @") AS rowindex,
        
        ISNULL(H.Id, 0) AS Id,
        ISNULL(H.Code, '') AS Code,
        ISNULL(H.OverHead, '') AS OverHead,
        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.IsArchive, 0) AS IsArchive,
        ISNULL(H.IsActive, 0) AS IsActive,
        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom

        FROM OverHeads H 
        WHERE H.IsArchive != 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<OverHeadVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<OverHeadVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
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

        ISNULL(H.Id, 0) AS Id,
        ISNULL(H.Code, '') AS Code,
        ISNULL(H.Name, '') AS Name,
        ISNULL(H.RouteId, 0) AS RouteId,
        ISNULL(R.Name, '') AS RouteName,
        ISNULL(H.AreaId, 0) AS AreaId,
        ISNULL(A.Name, '') AS AreaName,
        ISNULL(H.BranchId, 0) AS BranchId,
        ISNULL(B.Name, '') AS BranchName,
        ISNULL(H.CustomerGroupId, 0) AS CustomerGroupId,
        ISNULL(C.Code, '') AS CustomerGroupCode,
        ISNULL(C.Name, '') AS CustomerGroupName,
        ISNULL(H.BanglaName, '') AS BanglaName,
        ISNULL(H.Address, '') AS Address,
        ISNULL(H.BanglaAddress, '') AS BanglaAddress,
        ISNULL(H.City, '') AS City,
        ISNULL(H.TelephoneNo, '') AS TelephoneNo,
        ISNULL(H.FaxNo, '') AS FaxNo,
        ISNULL(H.Email, '') AS Email,
        ISNULL(H.TINNo, '') AS TINNo,
        ISNULL(H.BINNo, '') AS BINNo,
        ISNULL(H.NIDNo, '') AS NIDNo,
        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.IsArchive, 0) AS IsArchive,
        ISNULL(H.IsActive, 0) AS IsActive,
        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom

        FROM Customers H 
        left outer join CustomerGroups C ON H.CustomerGroupId = C.Id
        left outer join BranchProfiles B ON H.BranchId = B.Id
        left outer join Areas A ON H.AreaId = A.Id
        left outer join Routes R ON H.RouteId = R.Id
        WHERE 1 = 1 ";

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
