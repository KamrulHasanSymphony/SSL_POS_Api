using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using Microsoft.VisualBasic;
using System;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    public class ContactPersonRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(ContactPersonVM contactPerson, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO ContactPersons
(Name, Code, Designation, Mobile, Mobile2, Phone, Phone2, EmailAddress, EmailAddress2, Fax, Address, 
CountryId, DivisionId, DistrictId, ThanaId, ZipCode, IsArchive, IsActive, CreatedBy, CreatedOn)
VALUES
(@Name, @Code, @Designation, @Mobile, @Mobile2, @Phone, @Phone2, @EmailAddress, @EmailAddress2, @Fax, @Address, 
@CountryId, @DivisionId, @DistrictId, @ThanaId, @ZipCode, @IsArchive, @IsActive, @CreatedBy, @CreatedOn);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", contactPerson.Name);
                    cmd.Parameters.AddWithValue("@Code", contactPerson.Code);
                    cmd.Parameters.AddWithValue("@Designation", contactPerson.Designation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile2", contactPerson.Mobile2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", contactPerson.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", contactPerson.Phone2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress", contactPerson.EmailAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress2", contactPerson.EmailAddress2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", contactPerson.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", contactPerson.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryId", contactPerson.CountryId ?? 0);
                    cmd.Parameters.AddWithValue("@DivisionId", contactPerson.DivisionId ?? 0);
                    cmd.Parameters.AddWithValue("@DistrictId", contactPerson.DistrictId ?? 0);
                    cmd.Parameters.AddWithValue("@ThanaId", contactPerson.ThanaId ?? 0);
                    cmd.Parameters.AddWithValue("@ZipCode", contactPerson.ZipCode ?? "");
                    cmd.Parameters.AddWithValue("@IsArchive", contactPerson.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive",true);
                    cmd.Parameters.AddWithValue("@CreatedBy", contactPerson.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    object newId = cmd.ExecuteScalar();
                    contactPerson.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = contactPerson;
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
public async Task<ResultVM> Update(ContactPersonVM contactPerson, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = contactPerson.Id.ToString(), DataVM = contactPerson };

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
UPDATE ContactPersons
SET 
    Name = @Name,
    Code = @Code,
    Designation = @Designation,
    Mobile = @Mobile,
    Mobile2 = @Mobile2,
    Phone = @Phone,
    Phone2 = @Phone2,
    EmailAddress = @EmailAddress,
    EmailAddress2 = @EmailAddress2,
    Fax = @Fax,
    Address = @Address,
    CountryId = @CountryId,
    DivisionId = @DivisionId,
    DistrictId = @DistrictId,
    ThanaId = @ThanaId,
    ZipCode = @ZipCode,
    IsArchive = @IsArchive,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = @LastModifiedOn
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", contactPerson.Id);
                    cmd.Parameters.AddWithValue("@Name", contactPerson.Name);
                    cmd.Parameters.AddWithValue("@Code", contactPerson.Code);
                    cmd.Parameters.AddWithValue("@Designation", contactPerson.Designation ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile", contactPerson.Mobile ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mobile2", contactPerson.Mobile2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", contactPerson.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone2", contactPerson.Phone2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress", contactPerson.EmailAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmailAddress2", contactPerson.EmailAddress2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Fax", contactPerson.Fax ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", contactPerson.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CountryId", contactPerson.CountryId ?? 0);
                    cmd.Parameters.AddWithValue("@DivisionId", contactPerson.DivisionId ?? 0);
                    cmd.Parameters.AddWithValue("@DistrictId", contactPerson.DistrictId ?? 0);
                    cmd.Parameters.AddWithValue("@ThanaId", contactPerson.ThanaId ?? 0);
                    cmd.Parameters.AddWithValue("@ZipCode", contactPerson.ZipCode ?? "");
                    cmd.Parameters.AddWithValue("@IsArchive", contactPerson.IsArchive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", contactPerson.LastModifiedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);

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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "", DataVM = null };

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
                string query = $"UPDATE ContactPersons SET IsArchive = 1, IsActive = 0 ,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
                        result.Message = $"Data Deleted Successfully.";
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

                string query = @"
SELECT
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Name, '') AS Name,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.Designation, '') AS Designation,
    ISNULL(M.Mobile, '') AS Mobile,
    ISNULL(M.Mobile2, '') AS Mobile2,
    ISNULL(M.Phone, '') AS Phone,
    ISNULL(M.Phone2, '') AS Phone2,
    ISNULL(M.EmailAddress, '') AS EmailAddress,
    ISNULL(M.EmailAddress2, '') AS EmailAddress2,
    ISNULL(M.Fax, '') AS Fax,
    ISNULL(M.Address, '') AS Address,
    ISNULL(M.CountryId, 0) AS CountryId,
    ISNULL(M.DivisionId, 0) AS DivisionId,
    ISNULL(M.DistrictId, 0) AS DistrictId,
    ISNULL(M.ThanaId, 0) AS ThanaId,
    ISNULL(M.ZipCode, '') AS ZipCode,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActive, 0) AS IsActive,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS LastModifiedOn,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM ContactPersons M
WHERE 1 = 1";



                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ContactPersonVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Code = row["Code"]?.ToString(),
                    Name = row["Name"]?.ToString(),
                    Designation = row["Designation"]?.ToString(),
                    Mobile = row["Mobile"]?.ToString(),
                    Mobile2 = row["Mobile2"]?.ToString(),
                    Phone = row["Phone"]?.ToString(),
                    Phone2 = row["Phone2"]?.ToString(),
                    EmailAddress = row["EmailAddress"]?.ToString(),
                    EmailAddress2 = row["EmailAddress2"]?.ToString(),
                    Fax = row["Fax"]?.ToString(),
                    Address = row["Address"]?.ToString(),
                    CountryId = row["CountryId"] != DBNull.Value ? Convert.ToInt32(row["CountryId"]) : (int?)null,
                    DivisionId = row["DivisionId"] != DBNull.Value ? Convert.ToInt32(row["DivisionId"]) : (int?)null,
                    DistrictId = row["DistrictId"] != DBNull.Value ? Convert.ToInt32(row["DistrictId"]) : (int?)null,
                    ThanaId = row["ThanaId"] != DBNull.Value ? Convert.ToInt32(row["ThanaId"]) : (int?)null,
                    ZipCode = row["ZipCode"]?.ToString(),
                    IsArchive = row["IsArchive"] != DBNull.Value ? Convert.ToBoolean(row["IsArchive"]) : (bool?)null,
                    IsActive = (bool)(row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : (bool?)null),
                    CreatedBy = row["CreatedBy"]?.ToString(),
                    CreatedOn = row["CreatedOn"]?.ToString(),
                    LastModifiedBy = row["LastModifiedBy"]?.ToString(),
                    LastModifiedOn = row["LastModifiedOn"]?.ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"]?.ToString()
                }).ToList();



                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
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
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Name, '') AS Name,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.Designation, '') AS Designation,
    ISNULL(M.Mobile, '') AS Mobile,
    ISNULL(M.Mobile2, '') AS Mobile2,
    ISNULL(M.Phone, '') AS Phone,
    ISNULL(M.Phone2, '') AS Phone2,
    ISNULL(M.EmailAddress, '') AS EmailAddress,
    ISNULL(M.EmailAddress2, '') AS EmailAddress2,
    ISNULL(M.Fax, '') AS Fax,
    ISNULL(M.Address, '') AS Address,
    ISNULL(M.CountryId, 0) AS CountryId,
    ISNULL(M.DivisionId, 0) AS DivisionId,
    ISNULL(M.DistrictId, 0) AS DistrictId,
    ISNULL(M.ThanaId, 0) AS ThanaId,
    ISNULL(M.ZipCode, '') AS ZipCode,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActiveStatus, 0) AS IsActiveStatus,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(M.CreatedOn, '1900-01-01') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(M.LastModifiedOn, '1900-01-01') AS LastModifiedOn
FROM ContactPersons M
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
FROM ContactPersons
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

                var data = new GridEntity<ContactPersonVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT M.Id) AS totalcount
FROM ContactPersons M
    left outer join Locations  LC On M.CountryId=LC.Id
    left outer join Locations  LD On M.DivisionId=LD.Id
    left outer join Locations  LDI On M.DistrictId=LDI.Id
    left outer join Locations  LT On M.ThanaId=LT.Id
WHERE M.IsArchive != 1
and      M.IsActive = 1

" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ContactPersonVM>.FilterCondition(options.filter) + ")" : "") + @"

-- Data query with pagination and sorting
SELECT * 
FROM (
    SELECT 
    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "M.Id DESC") + @") AS rowindex,
     ISNULL(M.Id, 0) AS Id,
     ISNULL(M.Name, '') AS Name,
     ISNULL(M.Code, '') AS Code,
     ISNULL(M.Designation, '') AS Designation,
     ISNULL(M.Mobile, '') AS Mobile,
     ISNULL(M.Mobile2, '') AS Mobile2,
     ISNULL(M.Phone, '') AS Phone,
     ISNULL(M.Phone2, '') AS Phone2,
     ISNULL(M.EmailAddress, '') AS EmailAddress,
     ISNULL(M.EmailAddress2, '') AS EmailAddress2,
     ISNULL(M.Fax, '') AS Fax,
     ISNULL(M.Address, '') AS Address,
     ISNULL(M.CountryId, 0) AS CountryId,
     ISNULL(LC.Name, 0) AS CountryName,
     ISNULL(M.DivisionId, 0) AS DivisionId,
     ISNULL(LD.Name, 0) AS  DivisionName,
     ISNULL(M.DistrictId, 0) AS DistrictId,
     ISNULL(LDI.Name, 0) AS  DistrictName,
     ISNULL(M.ThanaId, 0) AS ThanaId,
     ISNULL(LT.Name, 0) AS ThanaName,
     ISNULL(M.ZipCode, '') AS ZipCode,
     ISNULL(M.IsArchive, 0) AS IsArchive,
     ISNULL(M.IsActive, 0) AS IsActive,
     CASE WHEN ISNULL(M.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
     ISNULL(M.CreatedBy, '') AS CreatedBy,
     ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
     ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS CreatedOn,
     ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS LastModifiedOn,
     ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
    FROM ContactPersons M
    left outer join Locations  LC On M.CountryId=LC.Id
    left outer join Locations  LD On M.DivisionId=LD.Id
    left outer join Locations  LDI On M.DistrictId=LDI.Id
    left outer join Locations  LT On M.ThanaId=LT.Id
    WHERE M.IsArchive != 1
    and  M.IsActive = 1
   " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ContactPersonVM>.FilterCondition(options.filter) + ")" : "") + @"

) AS a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";


                data = KendoGrid<ContactPersonVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
     ISNULL(M.Id, 0) AS Id,
     ISNULL(M.Name, '') AS Name,
     ISNULL(M.Code, '') AS Code,
     ISNULL(M.Designation, '') AS Designation,
     ISNULL(M.Mobile, '') AS Mobile,
     ISNULL(M.Mobile2, '') AS Mobile2,
     ISNULL(M.Phone, '') AS Phone,
     ISNULL(M.Phone2, '') AS Phone2,
     ISNULL(M.EmailAddress, '') AS EmailAddress,
     ISNULL(M.EmailAddress2, '') AS EmailAddress2,
     ISNULL(M.Fax, '') AS Fax,
     ISNULL(M.Address, '') AS Address,
     ISNULL(M.CountryId, 0) AS CountryId,
     ISNULL(LC.Name, 0) AS CountryName,
     ISNULL(M.DivisionId, 0) AS DivisionId,
     ISNULL(LD.Name, 0) AS  DivisionName,
     ISNULL(M.DistrictId, 0) AS DistrictId,
     ISNULL(LDI.Name, 0) AS  DistrictName,
     ISNULL(M.ThanaId, 0) AS ThanaId,
     ISNULL(LT.Name, 0) AS ThanaName,
     ISNULL(M.ZipCode, '') AS ZipCode,
     ISNULL(M.IsArchive, 0) AS IsArchive,
     ISNULL(M.IsActive, 0) AS IsActive,
     CASE WHEN ISNULL(M.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
     ISNULL(M.CreatedBy, '') AS CreatedBy,
     ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
     ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS CreatedOn,
     ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01 00:00') AS LastModifiedOn,
     ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
    FROM ContactPersons M
    left outer join Locations  LC On M.CountryId=LC.Id
    left outer join Locations  LD On M.DivisionId=LD.Id
    left outer join Locations  LDI On M.DistrictId=LDI.Id
    left outer join Locations  LT On M.ThanaId=LT.Id
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


