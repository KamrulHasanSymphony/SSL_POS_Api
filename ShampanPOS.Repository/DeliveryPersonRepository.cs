using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    

    public class DeliveryPersonRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(DeliveryPersonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO DeliveryPersons 
(
    Code, Name, BranchId, EnumTypeId, BanglaName, Comments, City, FaxNo, 
    NIDNo, Mobile, Mobile2, Phone, Phone2, EmailAddress, EmailAddress2, Fax, 
    Address, ZipCode, CreatedBy, CreatedOn, CreatedFrom
)
VALUES 
(
    @Code, @Name, @BranchId, @EnumTypeId, @BanglaName, @Comments, @City, @FaxNo, 
    @NIDNo, @Mobile, @Mobile2, @Phone, @Phone2, @EmailAddress, @EmailAddress2, @Fax, 
    @Address, @ZipCode, @CreatedBy, GETDATE(), @CreatedFrom
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@EnumTypeId", vm.EnumTypeId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", vm.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile2", vm.Mobile2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", vm.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", vm.Phone2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress", vm.EmailAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress2", vm.EmailAddress2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", vm.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
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
        public async Task<ResultVM> Update(DeliveryPersonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE DeliveryPersons 
SET 
    Code = @Code, Name = @Name, BranchId = @BranchId, EnumTypeId = @EnumTypeId, 
    BanglaName = @BanglaName, Comments = @Comments, City = @City, FaxNo = @FaxNo, 
    NIDNo = @NIDNo, Mobile = @Mobile, Mobile2 = @Mobile2, Phone = @Phone, Phone2 = @Phone2, 
    EmailAddress = @EmailAddress, EmailAddress2 = @EmailAddress2, Fax = @Fax, Address = @Address, 
    ZipCode = @ZipCode, LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE(), 
    LastUpdateFrom = @LastUpdateFrom
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@EnumTypeId", vm.EnumTypeId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", vm.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile2", vm.Mobile2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", vm.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", vm.Phone2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress", vm.EmailAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress2", vm.EmailAddress2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", vm.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    result.Status = rowsAffected > 0 ? "Success" : "Fail";
                    result.Message = rowsAffected > 0 ? "Data updated successfully." : "No rows were updated.";
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

                string query = $" UPDATE DeliveryPersons SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.BranchId, 0)  BranchId,
    ISNULL(M.EnumTypeId, 0) EnumTypeId,
    ISNULL(M.BanglaName, '') BanglaName,
    ISNULL(M.Comments, '') Comments,
    ISNULL(M.City, '') City,
    ISNULL(M.FaxNo, '') FaxNo,
    ISNULL(M.Mobile, '') Mobile,
    ISNULL(M.Mobile2, '') Mobile2,
    ISNULL(M.Phone, '') Phone,
    ISNULL(M.Phone2, '') Phone2,
    ISNULL(M.NIDNo, '') NIDNo,
    ISNULL(M.EmailAddress, '') EmailAddress,
    ISNULL(M.EmailAddress2, '') EmailAddress2,
    ISNULL(M.Fax, '') Fax,
    ISNULL(M.Address, '') Address,
    ISNULL(M.ZipCode, '') ZipCode,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(M.CreatedBy, '') CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
    ISNULL(M.LastModifiedBy, '') LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn

        FROM DeliveryPersons M
        WHERE 1 = 1";


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);


                var modelList = dataTable.AsEnumerable().Select(row => new DeliveryPersonVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    BranchId = row.Field<int>("BranchId"),
                    EnumTypeId = row.Field<int>("EnumTypeId"),
                    BanglaName = row.Field<string>("BanglaName"),
                    Comments = row.Field<string>("Comments"),
                    City = row.Field<string>("City"),
                    FaxNo = row.Field<string>("FaxNo"),
                    NIDNo = row.Field<string>("NIDNo"),
                    Mobile = row.Field<string>("Mobile"),
                    Mobile2 = row.Field<string>("Mobile2"),
                    Phone = row.Field<string>("Phone"),
                    Phone2 = row.Field<string>("Phone2"),
                    EmailAddress = row.Field<string>("EmailAddress"),
                    EmailAddress2 = row.Field<string>("EmailAddress2"),
                    Fax = row.Field<string>("Fax"),
                    Address = row.Field<string>("Address"),
                    ZipCode = row.Field<string>("ZipCode"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string>("LastModifiedOn"),
                    CreatedFrom = row.Field<string>("CreatedFrom"),
                    LastUpdateFrom = row.Field<string>("LastUpdateFrom")
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
    Code,
    Name,
    BranchId,
    EnumTypeId,
    BanglaName,
    Comments,
    City,
    FaxNo,
    NIDNo,
    Mobile,
    Mobile2,
    Phone,
    Phone2,
    EmailAddress,
    EmailAddress2,
    Fax,
    Address,
    ZipCode,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn,
    CreatedFrom,
    LastUpdateFrom
FROM DeliveryPersons
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
FROM DeliveryPersons
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

                var data = new GridEntity<DeliveryPersonVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
             SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM DeliveryPersons H
            where 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<DeliveryPersonVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,            
                ISNULL(H.Name, '') AS Name,
                ISNULL(H.City, '') AS City,
                ISNULL(H.Mobile, '') AS Mobile,
                ISNULL(H.Mobile2, '') AS Mobile2,
                ISNULL(H.Phone, '') AS Phone,
                ISNULL(H.Phone2, '') AS Phone2,
                ISNULL(H.EmailAddress, '') AS EmailAddress,
                ISNULL(H.EmailAddress2, '') AS EmailAddress2,
                ISNULL(H.Address, '') AS Address,
                ISNULL(H.ZipCode, '') AS ZipCode,
                ISNULL(H.NIDNo, '') AS NIDNo,
                ISNULL(H.FaxNo, '') AS FaxNo,
                ISNULL(H.Comments, '') AS Comments,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
            FROM DeliveryPersons H
     where
            1= 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<DeliveryPersonVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                //data = KendoGrid<DeliveryPersonVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<DeliveryPersonVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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
                ISNULL(H.City, '') AS City,
                ISNULL(H.Mobile, '') AS Mobile,
                ISNULL(H.Mobile2, '') AS Mobile2,
                ISNULL(H.Phone, '') AS Phone,
                ISNULL(H.Phone2, '') AS Phone2,
                ISNULL(H.EmailAddress, '') AS EmailAddress,
                ISNULL(H.EmailAddress2, '') AS EmailAddress2,
                ISNULL(H.Address, '') AS Address,
                ISNULL(H.ZipCode, '') AS ZipCode,
                ISNULL(H.NIDNo, '') AS NIDNo,
                ISNULL(H.FaxNo, '') AS FaxNo,
                ISNULL(H.Comments, '') AS Comments,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
        FROM DeliveryPersons H
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
