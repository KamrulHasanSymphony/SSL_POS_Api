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
    public class MasterItemRepository : CommonRepository
    {

        // Insert Method

        public async Task<ResultVM> Insert(MasterItemVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO MasterItem
        (
            Code, Name, MasterItemGroupId, BanglaName, Description, UOMId, HSCodeNo,VATRate,SDRate, IsArchive, IsActive, CreatedBy,CreatedFrom, CreatedOn,ImagePath
        )
        VALUES
        (
            @Code, @Name, @MasterItemGroupId, @BanglaName, @Description, @UOMId, @HSCodeNo,@VATRate,@SDRate,
            @IsArchive, @IsActive, @CreatedBy, @CreatedFrom,@CreatedOn,@ImagePath
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MasterItemGroupId", vm.MasterItemGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", vm.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId ?? 1);
                    cmd.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@SDRate", vm.SDRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);
                  


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

        public async Task<ResultVM> Update(MasterItemVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE MasterItem
        SET
            Code = @Code, Name = @Name, MasterItemGroupId = @MasterItemGroupId, BanglaName = @BanglaName, VATRate= @VATRate,SDRate= @SDRate,
            Description = @Description, UOMId = @UOMId, HSCodeNo = @HSCodeNo,
            IsArchive = @IsArchive, IsActive = @IsActive, LastModifiedBy = @LastModifiedBy, LastUpdateFrom=@LastUpdateFrom,
            LastModifiedOn = GETDATE(),ImagePath = @ImagePath
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MasterItemGroupId", vm.MasterItemGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@SDRate", vm.SDRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@Description", vm.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId ?? 1);
                    cmd.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);

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

                string query = $" UPDATE Products SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.Code, '') Code,
    ISNULL(M.Name, '') Name,
    ISNULL(M.MasterItemGroupId, 0) MasterItemGroupId,
    ISNULL(M.BanglaName, '') BanglaName,
    ISNULL(M.Description, '') Description,
    ISNULL(M.UOMId, 0) UOMId,
    ISNULL(M.HSCodeNo, '') HSCodeNo,
    ISNULL(M.IsArchive, 0) IsArchive,
    ISNULL(M.IsActive, 0) IsActive,
    ISNULL(M.CreatedBy, '') CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
    ISNULL(M.LastModifiedBy, '') LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
    ISNULL(M.ImagePath,'') AS ImagePath,
	ISNULL(M.VATRate, 0) AS VATRate,
	ISNULL(M.SDRate, 0) AS SDRate

FROM MasterItem M
WHERE 1 = 1
";

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

                var modelList = dataTable.AsEnumerable().Select(row => new MasterItemVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    MasterItemGroupId = row.Field<int>("MasterItemGroupId"),
                    BanglaName = row.Field<string>("BanglaName"),
                    Description = row.Field<string>("Description"),
                    UOMId = row.Field<int?>("UOMId"), // Nullable field
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    IsActive = row.Field<bool>("IsActive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                    ImagePath = row.Field<string>("ImagePath"),
                    VATRate = row.Field<decimal?>("VATRate") ?? 0.0m,
                    SDRate = row.Field<decimal?>("SDRate") ?? 0.0m

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
    Id, Code, Name, MasterItemGroupId, BanglaName, Description, UOMId, HSCodeNo, 
    IsArchive, IsActive, CreatedBy, CreatedOn, 
    LastModifiedBy, LastModifiedOn
FROM MasterItem WHERE 1 = 1";



                DataTable dataTable = new DataTable();

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
FROM MasterItem
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

        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<MasterItemVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM MasterItem H
            LEFT OUTER JOIN MasterItemGroup MG ON H.MasterItemGroupId = MG.Id
            LEFT OUTER JOIN UOMs uom ON H.UOMId = uom.Id
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<MasterItemVM>.FilterCondition(options.filter) + ")" : "") + @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.Name, '') AS Name,
                ISNULL(H.Description, '') AS Description,
                ISNULL(H.IsArchive, 0) AS IsArchive,                
                CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(MG.Id, 0) AS MasterItemGroupId,
                ISNULL(MG.Name, '') AS ProductGroupName,
                ISNULL(H.UOMId, 0) AS UOMId,
				ISNULL(H.VATRate, 0) AS VATRate,
				ISNULL(H.SDRate, 0) AS SDRate
            FROM MasterItem H
            LEFT OUTER JOIN MasterItemGroup MG ON H.MasterItemGroupId = MG.Id
            LEFT OUTER JOIN UOMs uom ON H.UOMId = uom.Id
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<MasterItemVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<MasterItemVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
