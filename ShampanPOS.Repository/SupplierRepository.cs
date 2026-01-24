using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    
    public class SupplierRepository : CommonRepository
    {
        //Insert Method
        public async Task<ResultVM> Insert(SupplierVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO Suppliers 
(
 Code, Name, SupplierGroupId, BanglaName, Address, City, TelephoneNo, Email, 
 ContactPerson, Comments, IsArchive, IsActive, CreatedBy, CreatedOn,ImagePath
)
VALUES 
(
 @Code, @Name, @SupplierGroupId, @BanglaName, @Address, @City, @TelephoneNo, 
 @Email, @ContactPerson, @Comments, @IsArchive, @IsActive, @CreatedBy, GETDATE(),@ImagePath
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SupplierGroupId", vm.SupplierGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
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

        //Update Method
        public async Task<ResultVM> Update(SupplierVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE Suppliers 
SET 
    Name = @Name, SupplierGroupId = @SupplierGroupId, BanglaName = @BanglaName, 
    Address = @Address, City = @City, TelephoneNo = @TelephoneNo, Email = @Email, 
    ContactPerson = @ContactPerson, Comments = @Comments, IsArchive = @IsArchive, 
    IsActive = @IsActive, LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE(),ImagePath = @ImagePath

WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SupplierGroupId", vm.SupplierGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", vm.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactPerson", vm.ContactPerson ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
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

        //Delete Method
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

                string query = $" UPDATE Suppliers SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.SupplierGroupId, 0) SupplierGroupId,
    ISNULL(M.BanglaName, '') BanglaName,
    ISNULL(M.Address, '') Address,
    ISNULL(M.City, '') City,
    ISNULL(M.TelephoneNo, '') TelephoneNo,
    ISNULL(M.Email, '')  Email,
    ISNULL(M.ContactPerson, '') ContactPerson,
    ISNULL(M.Comments, '') Comments,
    ISNULL(M.IsArchive, 0) IsArchive,
    ISNULL(M.IsActive, 0) IsActive,
    ISNULL(M.CreatedBy, '') CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
    ISNULL(M.LastModifiedBy, '') LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
    ISNULL(M.ImagePath,'') AS ImagePath
   
FROM Suppliers M
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

                var modelList = dataTable.AsEnumerable().Select(row => new SupplierVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    SupplierGroupId = row.Field<int>("SupplierGroupId"),
                    BanglaName = row.Field<string>("BanglaName"),
                    Address = row.Field<string>("Address"),
                    City = row.Field<string>("City"),
                    TelephoneNo = row.Field<string>("TelephoneNo"),
                    Email = row.Field<string>("Email"),
                    ContactPerson = row.Field<string>("ContactPerson"),
                    Comments = row.Field<string>("Comments"),
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
    SupplierGroupId,
    BanglaName,
    Address,
    City,
    TelephoneNo,
    Email,
    ContactPerson,
    Comments,
    IsArchive,
    IsActive,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn
FROM Suppliers
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
FROM Suppliers
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

                var data = new GridEntity<SupplierVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM Suppliers H
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SupplierVM>.FilterCondition(options.filter) + ")" : "") + @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.Name, '') AS Name,
                ISNULL(H.SupplierGroupId, 0) SupplierGroupId,
                ISNULL(H.BanglaName, '') BanglaName,
                ISNULL(H.Address, '') Address,
                ISNULL(H.City, '') City,
                ISNULL(H.TelephoneNo, '') TelephoneNo,
                ISNULL(H.Email, '')  Email,
                ISNULL(H.ContactPerson, '') ContactPerson,
                ISNULL(H.Comments, '') Comments,
                ISNULL(H.IsArchive, 0) AS IsArchive,
                ISNULL(H.IsActive, 0) AS IsActive,
                CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn

            FROM Suppliers H
           
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SupplierVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SupplierVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
                ISNULL(H.SupplierGroupId, 0) SupplierGroupId,
                ISNULL(H.BanglaName, '') BanglaName,
                ISNULL(H.Address, '') Address,
                ISNULL(H.City, '') City,
                ISNULL(H.TelephoneNo, '') TelephoneNo,
                ISNULL(H.Email, '')  Email,
                ISNULL(H.ContactPerson, '') ContactPerson,
                ISNULL(H.Comments, '') Comments,
                ISNULL(H.IsArchive, 0) AS IsArchive,
                ISNULL(H.IsActive, 0) AS IsActive,
                CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn

            FROM Suppliers H
        
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


//        public async Task<ResultVM> GetPurchaseBySupplier(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"
//SELECT 
//    ISNULL(M.Id, 0) AS Id,
//    ISNULL(M.Id, 0) AS PurchaseId,
//    ISNULL(M.BranchId, 0) AS BranchId,
//    ISNULL(M.CustomerId, 0) AS CustomerId,
//    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
//    ISNULL(M.Comments, '') AS Comments,
//    ISNULL(M.TransactionType, '') AS TransactionType,
//    ISNULL(M.IsPost, 0) AS IsPost,
//    ISNULL(M.PostedBy, '') AS PostedBy,
//    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
//    ISNULL(M.PeriodId, '') AS PeriodId,
//    ISNULL(M.CreatedBy, '') AS CreatedBy,
//    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
//    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
//    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
//    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
//    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
//    ISNULL(M.GrandTotal, 0) AS GrandTotal

//FROM 
//    Sales M
//WHERE 1 = 1
// ";

//                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

//                if (IDs.Length > 0)
//                {
//                    query += $" AND M.Id IN ({inClause}) ";
//                }

//                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
//                {
//                    if (transaction != null)
//                    {
//                        adapter.SelectCommand.Transaction = transaction;
//                    }

//                    for (int i = 0; i < IDs.Length; i++)
//                    {
//                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
//                    }

//                    adapter.Fill(dataTable);
//                }

//                var lst = new List<CollectionVM>();

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new CollectionVM
//                    {
//                        Id = row.Field<int>("Id"),
//                        GrandTotal = row.Field<decimal>("GrandTotal"),
//                        //BranchId = row.Field<int>("BranchId"),
//                        CustomerId = row.Field<int>("CustomerId"),
//                        //BENumber = row.Field<string>("BENumber"),
//                        //InvoiceDateTime = row.Field<string>("InvoiceDateTime"),
//                        //PurchaseDate = row.Field<string>("PurchaseDate"),
//                        Comments = row.Field<string>("Comments"),
//                        //TransactionType = row.Field<string>("TransactionType"),
//                        //IsPost = row.Field<bool>("IsPost"),
//                        //PostedBy = row.Field<string>("PostedBy"),
//                        //PostedOn = row.Field<string?>("PostedOn"),
//                        //FiscalYear = row.Field<string>("FiscalYear"),
//                        //PeriodId = row.Field<string>("PeriodId"),
//                        CreatedBy = row.Field<string>("CreatedBy"),
//                        CreatedOn = row.Field<string>("CreatedOn"),
//                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
//                        LastModifiedOn = row.Field<string?>("LastModifiedOn"),
//                        LastUpdateFrom = row.Field<string?>("LastUpdateFrom")
//                    });
//                }

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = lst;

//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.ExMessage = ex.Message;
//                result.Message = ex.Message;
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                }
//            }
//        }





//        public async Task<ResultVM> GetDetails(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"
//SELECT 
//ISNULL(Pur.Code,'') SaleCode,
//ISNULL(Pur.Id,0) SaleId,
//ISNULL(FORMAT(Pur.GrandTotal, 'N2'), '0.00') AS SaleAmount,
//ISNULL(D.Id, 0) AS Id,
//ISNULL(D.Id, 0) AS SaleDetailId,
//ISNULL(D.SaleId, 0) AS SaleId,
//ISNULL(D.Line, 0) AS Line,
//ISNULL(D.ProductId, 0) AS ProductId,
//ISNULL(FORMAT(D.UnitRate, 'N2'), 0.00) AS UnitRate,
//ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
//ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
//ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
//ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,

//ISNULL(P.Name,'') ProductName,
//ISNULL(P.BanglaName,'') BanglaName,
//ISNULL(P.Code,'') ProductCode, 
//ISNULL(P.HSCodeNo,'') HSCodeNo,
//ISNULL(P.ProductGroupId,0) ProductGroupId,
//ISNULL(PG.Name,'') ProductGroupName


//FROM 
//SaleDetails D
//LEFT OUTER JOIN Products P ON D.ProductId = P.Id
//LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//LEFT OUTER JOIN Sales Pur ON D.SaleId = Pur.Id

//WHERE 1 = 1";


//                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

//                if (IDs.Length > 0)
//                {
//                    query += $" AND D.SaleId IN ({inClause}) ";
//                }

//                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
//                {
//                    if (transaction != null)
//                    {
//                        adapter.SelectCommand.Transaction = transaction;
//                    }

//                    for (int i = 0; i < IDs.Length; i++)
//                    {
//                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
//                    }

//                    adapter.Fill(dataTable);
//                }

//                result.Status = "Success";
//                result.Message = "Details Data retrieved successfully.";
//                result.DataVM = dataTable;

//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.ExMessage = ex.Message;
//                result.Message = ex.Message;
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                }
//            }
//        }


    }

}
