using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
   

    public class SalesPersonRepository : CommonRepository
    {


        // Insert Method
        public async Task<ResultVM> Insert(SalesPersonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO SalesPersons 
(
    Code, Name, BranchId, ParentId, EnumTypeId, BanglaName, Comments, City, FaxNo, 
    NIDNo, Mobile, Mobile2, Phone, Phone2, EmailAddress, EmailAddress2, Fax, 
    Address, ZipCode, IsArchive, IsActive, CreatedBy, CreatedOn,ImagePath
)
VALUES 
(
    @Code, @Name, @BranchId, @ParentId, @EnumTypeId, @BanglaName, @Comments, @City, @FaxNo, 
    @NIDNo, @Mobile, @Mobile2, @Phone, @Phone2, @EmailAddress, @EmailAddress2, @Fax, 
    @Address, @ZipCode, @IsArchive, @IsActive, @CreatedBy, @CreatedOn,@ImagePath
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@Name", vm.Name);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@ParentId", vm.ParentId);
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
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
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

        public async Task<ResultVM> Update(SalesPersonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE SalesPersons 
SET 
    Code = @Code, Name = @Name, BranchId = @BranchId, ParentId = @ParentId, EnumTypeId = @EnumTypeId,
    BanglaName = @BanglaName, Comments = @Comments, City = @City, FaxNo = @FaxNo, NIDNo = @NIDNo,
    Mobile = @Mobile, Mobile2 = @Mobile2, Phone = @Phone, Phone2 = @Phone2, EmailAddress = @EmailAddress,
    EmailAddress2 = @EmailAddress2, Fax = @Fax, Address = @Address, ZipCode = @ZipCode, 
    IsArchive = @IsArchive, IsActive= @IsActive, LastModifiedBy = @LastModifiedBy, 
    LastModifiedOn = GETDATE(),ImagePath = @ImagePath
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@Name", vm.Name);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@ParentId", vm.ParentId);
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
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);

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

                string query = $"UPDATE SalesPersons SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
                    ISNULL(M.BranchId, 0) BranchId,
                    ISNULL(M.ParentId, 0) ParentId,
                    ISNULL(M.EnumTypeId, 0) EnumTypeId,
                    ISNULL(M.BanglaName, '') BanglaName,
                    ISNULL(M.Comments, '') Comments,
                    ISNULL(M.City, '') City,
                    ISNULL(M.FaxNo, '') FaxNo, 
                    ISNULL(M. NIDNo, '')  NIDNo,
                    ISNULL(M.Mobile, '') Mobile,
                    ISNULL(M.Mobile2, '') Mobile2,  
                    ISNULL(M.Phone, '') Phone,
                    ISNULL(M.Phone2, '') Phone2,
                    ISNULL(M.EmailAddress,'') EmailAddress,
                    ISNULL(M.EmailAddress2,'') EmailAddress2,
                    ISNULL(M. Address, '')  Address,
                    ISNULL(M.Fax, '') Fax,
  
                    ISNULL(M.ZipCode, '') ZipCode,  
                    ISNULL(M.IsArchive, 0) IsArchive,
                    ISNULL(M.IsActive, 0) IsActive,
                    ISNULL(M.CreatedBy, '') CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
                    ISNULL(M.LastModifiedBy, '') LastModifiedBy,
                    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
                    ISNULL(M.ImagePath,'') AS ImagePath
                FROM SalesPersons M
                WHERE 1 = 1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);


                var modelList = dataTable.AsEnumerable().Select(row => new SalesPersonVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    BranchId = row.Field<int>("BranchId"),
                    ParentId = row.Field<int>("ParentId"),
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
                    IsArchive = row.Field<bool>("IsArchive"),
                    IsActive = row.Field<bool>("IsActive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                    ImagePath = row.Field<string?>("ImagePath")
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
    ParentId,
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
    IsArchive,
    IsActiveStatus,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn
FROM SalesPersons
WHERE 1 = 1";

                DataTable dataTable = new DataTable();
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
FROM SalesPersons
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

                var data = new GridEntity<SalesPersonVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
           SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM SalesPersons H
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalesPersonVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
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
        ISNULL(H.BranchId, 0) AS BranchId,
        ISNULL(H.ParentId, 0) AS ParentId,
        ISNULL(H.EnumTypeId, 0) AS EnumTypeId,

        ISNULL( E.Name, '') AS Type,       
        ISNULL(H.BanglaName, '') AS BanglaName,

        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.City, '') AS City,
        ISNULL(H.FaxNo, '') AS FaxNo,
        ISNULL(H.NIDNo, '') AS NIDNo,
        ISNULL(H.Mobile, '') AS Mobile,
        ISNULL(H.Mobile2, '') AS Mobile2,
        ISNULL(H.Phone, '') AS Phone,
        ISNULL(H.Phone2, '') AS Phone2,
        ISNULL(H.EmailAddress, '') AS EmailAddress,
        ISNULL(H.EmailAddress2, '') AS EmailAddress2,
        ISNULL(H.Fax, '') AS Fax,
        ISNULL(H.Address, '') AS Address,
        ISNULL(H.ZipCode, '') AS ZipCode,
        ISNULL(H.IsArchive, 0) AS IsArchive,
        
        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
         ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
    FROM SalesPersons H
            left outer join EnumTypes E on  E.Id=H.EnumTypeId
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalesPersonVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SalesPersonVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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


        // ReportPreview Method
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
        ISNULL(H.BranchId, 0) AS BranchId,
        ISNULL(H.ParentId, 0) AS ParentId,
        ISNULL(H.EnumTypeId, 0) AS EnumTypeId,
        ISNULL(H.BanglaName, '') AS BanglaName,
        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.City, '') AS City,
        ISNULL(H.FaxNo, '') AS FaxNo,
        ISNULL(H.NIDNo, '') AS NIDNo,
        ISNULL(H.Mobile, '') AS Mobile,
        ISNULL(H.Mobile2, '') AS Mobile2,
        ISNULL(H.Phone, '') AS Phone,
        ISNULL(H.Phone2, '') AS Phone2,
        ISNULL(H.EmailAddress, '') AS EmailAddress,
        ISNULL(H.EmailAddress2, '') AS EmailAddress2,
        ISNULL(H.Fax, '') AS Fax,
        ISNULL(H.Address, '') AS Address,
        ISNULL(H.ZipCode, '') AS ZipCode,
        ISNULL(H.IsArchive, 0) AS IsArchive,
        
        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
         ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
    FROM SalesPersons H
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

        public async Task<ResultVM> GetVisitHistoryGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SalePersonVisitHistrieVM>();

                // Define your SQL query string
                string sqlQuery = @"
                -- Count query
                SELECT COUNT(DISTINCT d.Id) AS totalcount
					            from SalePersonVisitHistoryDetails d
                    LEFT OUTER join SalePersonVisitHistries h on d.SalePersonVisitHistroyId=h.Id
                    LEFT OUTER JOIN SalesPersons SP on h.SalePersonId = SP.Id
                    LEFT OUTER JOIN Customers C on d.CustomerId = C.Id
                    LEFT OUTER JOIN Routes r on h.RouteId= r.Id
                    LEFT OUTER JOIN BranchProfiles Br on H.BranchId = Br.Id

					            WHERE 1 = 1
                -- Add the filter condition
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                            // Apply additional conditions
                            sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                            sqlQuery += @"
                -- Data query with pagination and sorting
                SELECT * 
                FROM (
                    SELECT 
                    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "d.Id DESC") + @") AS rowindex
                ,ISNULL(br.Name,'') BranchName
                ,ISNULL(sp.Name,'') SalePersonName
                ,ISNULL(FORMAT(h.[date],'yyyy-MM-dd HH:mm'),'1900-01-01') VisitDate
                ,ISNULL(r.Name,'') RouteName
                ,ISNULL(c.Name,'') CustomerName
                ,CASE WHEN ISNULL(d.IsVisited, 0) = 1 THEN 'Yes' ELSE 'No' END AS Visited
                from SalePersonVisitHistoryDetails d
                left outer join SalePersonVisitHistries h on d.SalePersonVisitHistroyId=h.Id
                 LEFT OUTER JOIN SalesPersons SP on h.SalePersonId = SP.Id
                LEFT OUTER JOIN Customers C on d.CustomerId = C.Id
                LEFT OUTER JOIN Routes r on h.RouteId= r.Id
                LEFT OUTER JOIN BranchProfiles Br on H.BranchId = Br.Id
				
				WHERE 1 = 1

                -- Add the filter condition
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                            // Apply additional conditions
                            sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                            sqlQuery += @"

                ) AS a
                WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
                ";

                //data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<SalePersonVisitHistrieVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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
