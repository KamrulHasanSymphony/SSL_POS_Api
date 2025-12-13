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
using System.Text.Json;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{

    public class AreaRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(AreaVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO Areas 
(
 Code, Name, BanglaName, ParentId, EnumTypeId, CountryId, DivisionId, 
 DistrictId, ThanaId, ZipCode, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
)
VALUES 
(
 @Code, @Name, @BanglaName, @ParentId, @EnumTypeId, @CountryId, @DivisionId, 
 @DistrictId, @ThanaId, @ZipCode, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentId", vm.ParentId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnumTypeId", vm.EnumTypeId);
                    cmd.Parameters.AddWithValue("@CountryId", vm.CountryId);
                    cmd.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                    cmd.Parameters.AddWithValue("@DistrictId", vm.DistrictId);
                    cmd.Parameters.AddWithValue("@ThanaId", vm.ThanaId);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", 0);
                    cmd.Parameters.AddWithValue("@IsActive", 1);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

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
        public async Task<ResultVM> Update(AreaVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE Areas 
SET 
 Name=@Name, BanglaName=@BanglaName, ParentId=@ParentId, EnumTypeId=@EnumTypeId, 
 CountryId=@CountryId, DivisionId=@DivisionId, DistrictId=@DistrictId, ThanaId=@ThanaId, 
 ZipCode=@ZipCode, IsArchive=@IsArchive, IsActive=@IsActive, 
 LastModifiedBy=@LastModifiedBy, LastModifiedOn=GETDATE(), LastUpdateFrom=@LastUpdateFrom
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentId", vm.ParentId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnumTypeId", vm.EnumTypeId);
                    cmd.Parameters.AddWithValue("@CountryId", vm.CountryId);
                    cmd.Parameters.AddWithValue("@DivisionId", vm.DivisionId);
                    cmd.Parameters.AddWithValue("@DistrictId", vm.DistrictId);
                    cmd.Parameters.AddWithValue("@ThanaId", vm.ThanaId);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
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
                string query = $"UPDATE Areas SET IsArchive = 1, IsActive = 0 WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

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
                result.Message = "Error in Delete.";
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



                //                string query = @"
                //SELECT
                // Id, Code, Name, BanglaName, ParentId, EnumTypeId, CountryId, DivisionId,
                // DistrictId, ThanaId, ZipCode, IsArchive, IsActive, CreatedBy, ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn, 
                // LastModifiedBy, ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
                //FROM Areas M WHERE 1 = 1 ";


                string query = @"SELECT 
                                    Id, 
                                    Code, 
                                    Name, 
                                    BanglaName, 
                                    ParentId, 
                                    EnumTypeId, 
                                    CountryId, 
                                    DivisionId,
                                    DistrictId, 
                                    ThanaId, 
                                    ZipCode, 
                                    IsArchive, 
                                    IsActive, 
                                    CreatedBy, 
                                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn, 
                                    LastModifiedBy, 
                                    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
                                FROM 
                                    Areas M 
                                WHERE 1=1
                                    --IsActive = 1 
                                    AND IsArchive = 0 
                                    ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);
                var model = new List<AreaVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new AreaVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        BanglaName = row.Field<string>("BanglaName"),
                        ParentId = row.Field<string>("ParentId"),
                        EnumTypeId = row.Field<int>("EnumTypeId"),
                        CountryId = row.Field<int>("CountryId"),
                        DivisionId = row.Field<int>("DivisionId"),
                        DistrictId = row.Field<int>("DistrictId"),
                        ThanaId = row.Field<int>("ThanaId"),
                        ZipCode = row.Field<string>("ZipCode"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string?>("LastModifiedOn")
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
 Id, Code, Name, BanglaName, ParentId, EnumTypeId, CountryId, DivisionId,
 DistrictId, ThanaId, ZipCode, IsArchive, IsActive, CreatedBy, CreatedOn, 
 LastModifiedBy, LastModifiedOn
FROM Areas WHERE 1 = 1";

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
SELECT Id, Name , Code
FROM Areas
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

        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<AreaVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
         SELECT COUNT(DISTINCT H.Id) AS totalcount
               FROM  Areas H
             LEFT OUTER JOIN EnumTypes e on h.EnumTypeId = e.Id
			 LEFT OUTER JOIN Locations L on H.CountryId = L.Id
			 LEFT OUTER JOIN Locations Ld on H.DivisionId = Ld.Id
			 LEFT OUTER JOIN Locations Li on H.DistrictId = Li.Id
			 LEFT OUTER JOIN Locations Lt on H.ThanaId = Lt.Id
         Where H.IsArchive != 1
  
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<AreaVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
           ISNULL(H.Id, 0) AS Id,
            ISNULL(H.ParentId, '') AS ParentId,
            ISNULL(H.Code, 0) AS Code,
            ISNULL(H.Name, 0) AS Name,
            ISNULL(H.BanglaName, 0) AS BanglaName,
            ISNULL(H.EnumTypeId, 0) AS EnumTypeId,
            ISNULL(e.Name, 0) AS EnumTypeName,
            ISNULL(H.CountryId, 0) AS CountryId,
			ISNULL(L.Name, '') AS CountryName,
            ISNULL(H.DivisionId, 0) AS DivisionId,
			ISNULL(Ld.name, '') AS DivisionName,
            ISNULL(H.DistrictId, 0) AS DistrictId,
			ISNULL(Li.name, '') AS DistrictName,
            ISNULL(H.ThanaId, 0) AS ThanaId,
			 ISNULL(Lt.Name, 0) AS ThanaName,
            ISNULL(H.ZipCode, '') AS ZipCode,
            CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'In-Active' END AS Status,       
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(H.CreatedOn, '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '''') AS LastModifiedBy,
            ISNULL(H.LastModifiedOn, '1900-01-01') AS LastModifiedOn,
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
            FROM Areas H 

             LEFT OUTER JOIN EnumTypes e on h.EnumTypeId = e.Id
			 LEFT OUTER JOIN Locations L on H.CountryId = L.Id
			 LEFT OUTER JOIN Locations Ld on H.DivisionId = Ld.Id
			 LEFT OUTER JOIN Locations Li on H.DistrictId = Li.Id
			 LEFT OUTER JOIN Locations Lt on H.ThanaId = Lt.Id

                        Where H.IsArchive != 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<AreaVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<AreaVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
            ISNULL(H.ParentId, '') AS ParentId,
            ISNULL(H.Code, 0) AS Code,
            ISNULL(H.Name, 0) AS Name,
            ISNULL(H.BanglaName, 0) AS BanglaName,
            ISNULL(H.EnumTypeId, 0) AS EnumTypeId,
            ISNULL(e.Name, 0) AS EnumTypeName,
            ISNULL(H.CountryId, 0) AS CountryId,
			ISNULL(L.Name, '') AS CountryName,
            ISNULL(H.DivisionId, 0) AS DivisionId,
			ISNULL(Ld.name, '') AS DivisionName,
            ISNULL(H.DistrictId, 0) AS DistrictId,
			ISNULL(Li.name, '') AS DistrictName,
            ISNULL(H.ThanaId, 0) AS ThanaId,
			 ISNULL(Lt.Name, 0) AS ThanaName,
            ISNULL(H.ZipCode, '') AS ZipCode,
            CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'In-Active' END AS Status,       
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(H.CreatedOn, '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '''') AS LastModifiedBy,
            ISNULL(H.LastModifiedOn, '1900-01-01') AS LastModifiedOn,
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
            FROM Areas H 

             LEFT OUTER JOIN EnumTypes e on h.EnumTypeId = e.Id
			 LEFT OUTER JOIN Locations L on H.CountryId = L.Id
			 LEFT OUTER JOIN Locations Ld on H.DivisionId = Ld.Id
			 LEFT OUTER JOIN Locations Li on H.DistrictId = Li.Id
			 LEFT OUTER JOIN Locations Lt on H.ThanaId = Lt.Id

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
