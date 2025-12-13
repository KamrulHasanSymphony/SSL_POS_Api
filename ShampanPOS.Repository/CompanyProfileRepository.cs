using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanPOS.Repository
{
    public class CompanyProfileRepository : CommonRepository
    {
        // Insert Method
         public async Task<ResultVM> Insert(CompanyProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
    INSERT INTO CompanyProfiles 
        (
            Code, CompanyName, CompanyBanglaName, CompanyLegalName, Address, City, ZipCode, 
            TelephoneNo, FaxNo, Email, ContactPerson, ContactPersonDesignation, ContactPersonTelephone, ContactPersonEmail, Comments, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom, BusinessNature, BIN, TIN, FYearStart, FYearEnd
        )
    VALUES 
        (
            @Code, @CompanyName, @CompanyBanglaName, @CompanyLegalName, @Address, @City, @ZipCode,
            @TelephoneNo, @FaxNo, @Email, @ContactPerson, @ContactPersonDesignation, @ContactPersonTelephone, @ContactPersonEmail, @Comments, 
            @IsArchive, @IsActive, @CreatedBy, GETDATE(), @CreatedFrom, @BusinessNature, @BIN, @TIN, @FYearStart, @FYearEnd
        );
    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyBanglaName", vm.CompanyBanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyLegalName", vm.CompanyLegalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIN", vm.TIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearStart", vm.FYearStart ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearEnd", vm.FYearEnd ?? (object)DBNull.Value);

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
         public async Task<ResultVM> Update(CompanyProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CompanyProfiles
SET
    CompanyName = @CompanyName,
    CompanyBanglaName = @CompanyBanglaName,
    CompanyLegalName = @CompanyLegalName,
    Address = @Address,
    City = @City,
    ZipCode = @ZipCode,
    TelephoneNo = @TelephoneNo,
    FaxNo = @FaxNo,
    Email = @Email,
    ContactPerson = @ContactPerson,
    ContactPersonDesignation = @ContactPersonDesignation,
    ContactPersonTelephone = @ContactPersonTelephone,
    ContactPersonEmail = @ContactPersonEmail,
    Comments = @Comments,
    LastModifiedOn = GETDATE(),
    BusinessNature = @BusinessNature,
    BIN = @BIN,
    TIN = @TIN,
    FYearStart = @FYearStart,
    FYearEnd = @FYearEnd,
    LastModifiedBy = @LastModifiedBy,
    LastUpdateFrom = @LastUpdateFrom
WHERE Id = @Id";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyBanglaName", vm.CompanyBanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyLegalName", vm.CompanyLegalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIN", vm.TIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearStart", vm.FYearStart ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearEnd", vm.FYearEnd ?? (object)DBNull.Value);

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

                string query = $" UPDATE CompanyProfiles SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(H.CompanyName, '') AS CompanyName,
    ISNULL(H.CompanyBanglaName, '') AS CompanyBanglaName,
    ISNULL(H.CompanyLegalName, '') AS CompanyLegalName,
    ISNULL(H.Address, '') AS Address,
    ISNULL(H.City, '') AS City,
    ISNULL(H.ZipCode, '') AS ZipCode,
    ISNULL(H.TelephoneNo, '') AS TelephoneNo,
    ISNULL(H.FaxNo, '') AS FaxNo,
    ISNULL(H.Email, '') AS Email,
    ISNULL(H.ContactPerson, '') AS ContactPerson,
    ISNULL(H.ContactPersonDesignation, '') AS ContactPersonDesignation,
    ISNULL(H.ContactPersonTelephone, '') AS ContactPersonTelephone,
    ISNULL(H.ContactPersonEmail, '') AS ContactPersonEmail,
    ISNULL(H.Comments, '') AS Comments,
    ISNULL(H.IsArchive, 0) AS IsArchive,
    ISNULL(H.IsActive, 0) AS IsActive,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS CreatedOn,
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS LastModifiedOn,
    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(FORMAT(H.FYearStart, 'yyyy-MM-dd'), '1900-01-01') AS FYearStart,
    ISNULL(FORMAT(H.FYearEnd, 'yyyy-MM-dd'), '1900-01-01') AS FYearEnd,
    ISNULL(H.BusinessNature, '') AS BusinessNature,
    ISNULL(H.BIN, '') AS BIN,
    ISNULL(H.TIN, '') AS TIN
FROM 
    CompanyProfiles AS H
WHERE 
    1 = 1";


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND H.Id = @Id ";
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

                var modelList = dataTable.AsEnumerable().Select(row => new CompanyProfileVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Code = row["Code"].ToString(),
                    CompanyName = row["CompanyName"].ToString(),
                    CompanyBanglaName = row["CompanyBanglaName"].ToString(),
                    CompanyLegalName = row["CompanyLegalName"].ToString(),
                    Address = row["Address"].ToString(),
                    City = row["City"].ToString(),
                    ZipCode = row["ZipCode"].ToString(),
                    TelephoneNo = row["TelephoneNo"].ToString(),
                    FaxNo = row["FaxNo"].ToString(),
                    Email = row["Email"].ToString(),
                    ContactPerson = row["ContactPerson"].ToString(),
                    ContactPersonDesignation = row["ContactPersonDesignation"].ToString(),
                    ContactPersonTelephone = row["ContactPersonTelephone"].ToString(),
                    ContactPersonEmail = row["ContactPersonEmail"].ToString(),
                    Comments = row["Comments"].ToString(),
                    IsArchive = Convert.ToBoolean(row["IsArchive"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedBy = row["CreatedBy"].ToString(),
                    LastModifiedBy = row["LastModifiedBy"].ToString(),
                    CreatedFrom = row["CreatedFrom"].ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"].ToString(),
                    BusinessNature = row["BusinessNature"].ToString(),
                    BIN = row["BIN"].ToString(),
                    TIN = row["TIN"].ToString(),
                    FYearStart = row["FYearStart"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["FYearStart"]),
                    FYearEnd = row["FYearEnd"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["FYearEnd"]),
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

                string query = @"
SELECT 
    ISNULL(H.Id, 0) AS Id,
    ISNULL(H.Code, '') AS Code,
    ISNULL(H.CompanyName, '') AS CompanyName,
    ISNULL(H.CompanyBanglaName, '') AS CompanyBanglaName,
    ISNULL(H.CompanyLegalName, '') AS CompanyLegalName,
    ISNULL(H.Address, '') AS Address,
    ISNULL(H.City, '') AS City,
    ISNULL(H.ZipCode, '') AS ZipCode,
    ISNULL(H.TelephoneNo, '') AS TelephoneNo,
    ISNULL(H.FaxNo, '') AS FaxNo,
    ISNULL(H.Email, '') AS Email,
    ISNULL(H.ContactPerson, '') AS ContactPerson,
    ISNULL(H.ContactPersonDesignation, '') AS ContactPersonDesignation,
    ISNULL(H.ContactPersonTelephone, '') AS ContactPersonTelephone,
    ISNULL(H.ContactPersonEmail, '') AS ContactPersonEmail,
    ISNULL(H.Comments, '') AS Comments,
    ISNULL(H.IsArchive, 0) AS IsArchive,
    ISNULL(H.IsActive, 0) AS IsActive,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS CreatedOn,
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS LastModifiedOn,
    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(FORMAT(H.FYearStart, 'yyyy-MM-dd'), '1900-01-01') AS FYearStart,
    ISNULL(FORMAT(H.FYearEnd, 'yyyy-MM-dd'), '1900-01-01') AS FYearEnd,
    ISNULL(H.BusinessNature, '') AS BusinessNature,
    ISNULL(H.BIN, '') AS BIN,
	ISNULL(H.TIN, '') AS TIN
FROM 
    CompanyProfiles AS H
WHERE 
    1 = 1

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
                SELECT Id CompanyId, CompanyName
                FROM CompanyProfiles
                WHERE IsActive = 1
                ORDER BY CompanyName ";

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

                var data = new GridEntity<CompanyProfileVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
                    SELECT COUNT(DISTINCT H.Id) AS totalcount
                   FROM CompanyProfiles H 
                    WHERE H.IsArchive != 1
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CompanyProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
    ISNULL(H.Id, 0) AS Id,
    ISNULL(H.Code, '') AS Code,
    ISNULL(H.CompanyName, '') AS CompanyName,
    ISNULL(H.CompanyBanglaName, '') AS CompanyBanglaName,
    ISNULL(H.CompanyLegalName, '') AS CompanyLegalName,
    ISNULL(H.Address, '') AS Address,
    ISNULL(H.City, '') AS City,
    ISNULL(H.ZipCode, '') AS ZipCode,
    ISNULL(H.TelephoneNo, '') AS TelephoneNo,
    ISNULL(H.FaxNo, '') AS FaxNo,
    ISNULL(H.Email, '') AS Email,
    ISNULL(H.ContactPerson, '') AS ContactPerson,
    ISNULL(H.ContactPersonDesignation, '') AS ContactPersonDesignation,
    ISNULL(H.ContactPersonTelephone, '') AS ContactPersonTelephone,
    ISNULL(H.ContactPersonEmail, '') AS ContactPersonEmail,
    ISNULL(H.TIN, '') AS TIN,  -- Remove duplicate TIN
    ISNULL(H.Comments, '') AS Comments,
    ISNULL(H.IsArchive, 0) AS IsArchive,
    ISNULL(H.IsActive, 0) AS IsActive,
    CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(H.CreatedOn, '1900-01-01') AS CreatedOn,  -- DateTime value
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(H.LastModifiedOn, '1900-01-01') AS LastModifiedOn,  -- DateTime value
    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(H.FYearStart, '1900-01-01') AS FYearStart,  -- DateTime value
    ISNULL(H.FYearEnd, '1900-01-01') AS FYearEnd,  -- DateTime value
    ISNULL(H.BusinessNature, '') AS BusinessNature,
    ISNULL(H.BIN, '') AS BIN
FROM 
    CompanyProfiles AS H


                    WHERE H.IsArchive != 1
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CompanyProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<CompanyProfileVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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



        public async Task<ResultVM> AuthCompanyInsert(CompanyProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
                INSERT INTO CompanyProfiles 
                    (
                        Code, CompanyName, CompanyBanglaName, CompanyLegalName, Address1, City, ZipCode,TINNo ,
                        TelephoneNo, FaxNo, Email, ContactPerson, ContactPersonDesignation, ContactPersonTelephone, ContactPersonEmail,Comments, IsArchive, IsActive, CreatedBy, CreatedOn,CreatedFrom, BusinessNature, BIN,FYearStart, FYearEnd
                        
                    )
                    VALUES 
                    (
                      @Code, @CompanyName, @CompanyBanglaName, @CompanyLegalName, @Address, @City, @ZipCode,@TINNo,
                      @TelephoneNo, @FaxNo, @Email, @ContactPerson, @ContactPersonDesignation, @ContactPersonTelephone, @ContactPersonEmail,@Comments, 
                        @IsArchive, @IsActive, @CreatedBy, GETDATE(), @CreatedFrom, @BusinessNature,@BIN,@FYearStart, @FYearEnd
                    );
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyBanglaName", vm.CompanyBanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyLegalName", vm.CompanyLegalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNo", vm.TIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearStart", vm.FYearStart ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearEnd", vm.FYearEnd ?? (object)DBNull.Value);

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
                result.Status = "Fail";
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
        public async Task<ResultVM> AuthCompanyUpdate(CompanyProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.Id.ToString(), DataVM = vm };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
UPDATE CompanyProfiles
SET
    CompanyName = @CompanyName,
    CompanyBanglaName = @CompanyBanglaName,
    CompanyLegalName = @CompanyLegalName,
    Address = @Address,
    City = @City,
    ZipCode = @ZipCode,
    TelephoneNo = @TelephoneNo,
    FaxNo = @FaxNo,
    Email = @Email,
    ContactPerson = @ContactPerson,
    ContactPersonDesignation = @ContactPersonDesignation,
    ContactPersonTelephone = @ContactPersonTelephone,
    ContactPersonEmail = @ContactPersonEmail,
    Comments = @Comments,
    LastModifiedOn = GETDATE(),
    BusinessNature = @BusinessNature,
    BIN = @BIN,
    TIN = @TIN,
    FYearStart = @FYearStart,
    FYearEnd = @FYearEnd,
    LastModifiedBy = @LastModifiedBy,    
    LastUpdateFrom = @LastUpdateFrom,
WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyBanglaName", vm.CompanyBanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyLegalName", vm.CompanyLegalName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ZipCode", vm.ZipCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonDesignation", vm.ContactPersonDesignation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonTelephone", vm.ContactPersonTelephone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPersonEmail", vm.ContactPersonEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BusinessNature", vm.BusinessNature ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIN", vm.TIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearStart", vm.FYearStart ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FYearEnd", vm.FYearEnd ?? (object)DBNull.Value);

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
                result.Status = "Fail";
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
        public async Task<ResultVM> AuthCompanyDelete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.IDs.ToString(), DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                //if (transaction == null)
                //{
                //    transaction = conn.BeginTransaction();
                //}
                //string inClause = string.Join(", ", vm.IDs.Select((id, index) => $"@Id{index}"));

                //string query = $" UPDATE CompanyProfiles SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string inClause = string.Join(", ", vm.IDs.Select((id, index) => $"@Id{index}"));

                string query = $" UPDATE CompanyProfiles SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";
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
                //using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                //{
                //    for (int i = 0; i < vm.IDs.Length; i++)
                //    {
                //        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                //    }

                //    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                //    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

                //    int rowsAffected = cmd.ExecuteNonQuery();


                //    if (rowsAffected > 0)
                //    {
                //        result.Status = "Success";
                //        result.Message = $"CompanyProfiles with Ids {string.Join(", ", string.Join(",", vm.IDs))}  deleted successfully.";
                //    }
                //    else
                //    {
                //        throw new Exception("No rows were updated.");
                //    }
                //}

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
                result.Status = "Fail";
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
                                    ISNULL(H.CompanyName, '') AS CompanyName,
                                    ISNULL(H.CompanyBanglaName, '') AS CompanyBanglaName,
                                    ISNULL(H.CompanyLegalName, '') AS CompanyLegalName,
                                    ISNULL(H.Address1, '') AS Address1,
                                    ISNULL(H.Address2, '') AS Address2,
                                    ISNULL(H.Address3, '') AS Address3,
                                    ISNULL(H.City, '') AS City,
                                    ISNULL(H.ZipCode, '') AS ZipCode,
                                    ISNULL(H.TelephoneNo, '') AS TelephoneNo,
                                    ISNULL(H.FaxNo, '') AS FaxNo,
                                    ISNULL(H.Email, '') AS Email,
                                    ISNULL(H.ContactPerson, '') AS ContactPerson,
                                    ISNULL(H.ContactPersonDesignation, '') AS ContactPersonDesignation,
                                    ISNULL(H.ContactPersonTelephone, '') AS ContactPersonTelephone,
                                    ISNULL(H.ContactPersonEmail, '') AS ContactPersonEmail,
                                    ISNULL(H.TINNo, '') AS TINNo,
                                    ISNULL(H.VatRegistrationNo, '') AS VatRegistrationNo,
                                    ISNULL(H.Comments, '') AS Comments,
                                    ISNULL(H.IsArchive, 0) AS IsArchive,
                                    ISNULL(H.IsActive, 0) AS IsActive,
                                    ISNULL(H.CreatedBy, '') AS CreatedBy,
                                    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS CreatedOn,
                                    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                                    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01') AS LastModifiedOn,
                                    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                                    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
                                    ISNULL(FORMAT(H.FYearStart, 'yyyy-MM-dd'), '1900-01-01') AS FYearStart,
                                    ISNULL(FORMAT(H.FYearEnd, 'yyyy-MM-dd'), '1900-01-01') AS FYearEnd,
                                    ISNULL(H.BusinessNature, '') AS BusinessNature,
                                    ISNULL(H.AccountingNature, '') AS AccountingNature,
                                    ISNULL(H.CompanyTypeId, 0) AS CompanyTypeId,
                                    ISNULL(H.Section, '') AS Section,
                                    ISNULL(H.BIN, '') AS BIN,
                                    ISNULL(H.IsVDSWithHolder, 0) AS IsVDSWithHolder,
                                    ISNULL(H.AppVersion, '') AS AppVersion,
                                    ISNULL(H.License, '') AS License,
	                                ISNULL(FORMAT(H.FYearStartSalePerson, 'yyyy-MM-dd'), '1900-01-01') AS FYearStartSalePerson,
                                    ISNULL(FORMAT(H.FYearEndSalePerson, 'yyyy-MM-dd'), '1900-01-01') AS FYearEndSalePerson
                                FROM 
                                    CompanyProfiles AS H
                                WHERE 
                                    1 = 1 ";

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
