using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;

namespace ShampanPOS.Repository
{
    using ShampanPOS.ViewModel.KendoCommon;
    using ShampanPOS.ViewModel.Utility;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class CustomerRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CustomerVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO Customers
(
    Code, Name, CustomerGroupId, BanglaName, Address, BanglaAddress, 
     TelephoneNo, FaxNo, Email, TINNo, BINNo, NIDNo, 
    Comments, IsArchive, IsActive, CreatedBy, CreatedOn,ImagePath
)
VALUES
(
    @Code, @Name, @CustomerGroupId, @BanglaName, @Address, @BanglaAddress, 
     @TelephoneNo, @FaxNo, @Email, @TINNo, @BINNo, @NIDNo,
    @Comments, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @ImagePath
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@Name", vm.Name);
                    
                    cmd.Parameters.AddWithValue("@CustomerGroupId", vm.CustomerGroupId);
                    
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNo", vm.TINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BINNo", vm.BINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
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
        public async Task<ResultVM> Update(CustomerVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE Customers
SET 
    Code = @Code,
    Name = @Name,
    CustomerGroupId = @CustomerGroupId,
    BanglaName = @BanglaName,
    Address = @Address,
    BanglaAddress = @BanglaAddress,

    TelephoneNo = @TelephoneNo,
    FaxNo = @FaxNo,
    Email = @Email,
    TINNo = @TINNo,
    BINNo = @BINNo,
    NIDNo = @NIDNo,
    Comments = @Comments,   

    IsArchive = @IsArchive,
    
    IsActive = @IsActive,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = GETDATE(),
    CreatedFrom = @CreatedFrom,
    LastUpdateFrom = @LastUpdateFrom,

    ImagePath = @ImagePath
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerGroupId", vm.CustomerGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BanglaAddress", vm.BanglaAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FaxNo", vm.FaxNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNo", vm.TINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BINNo", vm.BINNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NIDNo", vm.NIDNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
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

                string query = $" UPDATE Customers SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.Name, '') AS Name,
    ISNULL(M.BanglaName, '') AS BanglaName,
    ISNULL(M.CustomerGroupId, 0) AS CustomerGroupId,
    ISNULL(M.Address, '') AS Address,
    ISNULL(M.BanglaAddress, '') AS BanglaAddress,
    ISNULL(M.TelephoneNo, '') AS TelephoneNo,
    ISNULL(M.FaxNo, '') AS FaxNo,
    ISNULL(M.Email, '') AS Email,
    ISNULL(M.TINNo, '') AS TINNo,
    ISNULL(M.BINNo, '') AS BINNo,
    ISNULL(M.NIDNo, '') AS NIDNo,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
    ISNULL(M.ImagePath,'') AS ImagePath
FROM Customers M
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

                var model = new List<CustomerVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        BanglaName = row.Field<string>("BanglaName"),
                        CustomerGroupId = row.Field<int>("CustomerGroupId"),
                        Address = row.Field<string>("Address"),
                        BanglaAddress = row.Field<string>("BanglaAddress"),
                        TelephoneNo = row.Field<string>("TelephoneNo"),
                        FaxNo = row.Field<string>("FaxNo"),
                        Email = row.Field<string>("Email"),
                        TINNo = row.Field<string>("TINNo"),
                        BINNo = row.Field<string>("BINNo"),
                        NIDNo = row.Field<string>("NIDNo"),
                        Comments = row.Field<string>("Comments"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string>("LastModifiedOn"),
                        ImagePath = row.Field<string>("ImagePath")
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
        public async Task<ResultVM> GetCustomerListByRoute(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(Id, 0) AS Id,
    ISNULL(Code, '') AS Code,
    ISNULL(Name, '') AS Name,
    ISNULL(BanglaName, '') AS BanglaName,
    ISNULL(CustomerGroupId, 0) AS CustomerGroupId,
    ISNULL(Address, '') AS Address,
    ISNULL(BanglaAddress, '') AS BanglaAddress,
    ISNULL(RouteId, 0) AS RouteId,
    ISNULL(AreaId, 0) AS AreaId,
    ISNULL(City, '') AS City,
    ISNULL(TelephoneNo, '') AS TelephoneNo,
    ISNULL(FaxNo, '') AS FaxNo,
    ISNULL(Email, '') AS Email,
    ISNULL(TINNo, '') AS TINNo,
    ISNULL(BINNo, '') AS BINNo,
    ISNULL(NIDNo, '') AS NIDNo,
    ISNULL(Comments, '') AS Comments,
    ISNULL(IsArchive, 0) AS IsArchive,
    ISNULL(IsActive, 0) AS IsActive,
    ISNULL(CreatedBy, '') AS CreatedBy,
    ISNULL(RegularDiscountRate, 0) AS RegularDiscountRate,

    FORMAT(ISNULL(CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM Customers 
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


                var model = new List<CustomerVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        BanglaName = row.Field<string>("BanglaName"),
                        CustomerGroupId = row.Field<int>("CustomerGroupId"),
                        Address = row.Field<string>("Address"),
                        BanglaAddress = row.Field<string>("BanglaAddress"),
                        TelephoneNo = row.Field<string>("TelephoneNo"),
                        FaxNo = row.Field<string>("FaxNo"),
                        Email = row.Field<string>("Email"),
                        TINNo = row.Field<string>("TINNo"),
                        BINNo = row.Field<string>("BINNo"),
                        NIDNo = row.Field<string>("NIDNo"),
                        Comments = row.Field<string>("Comments"),
                        //RegularDiscountRate = row.Field<decimal>("RegularDiscountRate"),

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

                var data = new GridEntity<CustomerVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM Customers H 
    LEFT OUTER JOIN CustomerGroups C ON H.CustomerGroupId = C.Id
    WHERE H.IsArchive != 1 ";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<CustomerVM>.FilterCondition(options.filter) + ")" : "");

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
        ISNULL(H.Name, '') AS Name,
        ISNULL(H.CustomerGroupId, 0) AS CustomerGroupId,
        ISNULL(C.Name, '') AS CustomerGroupName,
        ISNULL(H.BanglaName, '') AS BanglaName,
        ISNULL(H.Address, '') AS Address,
        ISNULL(H.BanglaAddress, '') AS BanglaAddress,
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
        LEFT OUTER JOIN CustomerGroups C ON H.CustomerGroupId = C.Id
        WHERE H.IsArchive != 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<CustomerVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<CustomerVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
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
//        public async Task<ResultVM> GetCustomerModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"
//SELECT DISTINCT
//    ISNULL(Cus.Id, 0) AS CustomerId, 
//    ISNULL(Cus.Name, '') AS CustomerName,
//    ISNULL(Cus.BanglaName, '') AS BanglaName, 
//    ISNULL(Cus.Code, '') AS CustomerCode,
//    'Active' AS Status
//FROM Customers Cus
//WHERE Cus.Name = 'ALL'

//UNION

//SELECT DISTINCT
//    ISNULL(Cus.Id,0)CustomerId , 
//    ISNULL(Cus.Name,'') CustomerName,
//    ISNULL(Cus.BanglaName,'') BanglaName, 
//    ISNULL(Cus.Code,'') CustomerCode, 
//    CASE WHEN Cus.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status
//FROM Customers Cus
//WHERE Cus.IsActive = 1 
//";

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                objComm.Fill(dataTable);

//                var modelList = dataTable.AsEnumerable().Select(row => new CustomerDataVM
//                {
//                    CustomerId = row.Field<int>("CustomerId"),
//                    CustomerName = row.Field<string>("CustomerName"),
//                    BanglaName = row.Field<string>("BanglaName"),
//                    CustomerCode = row.Field<string>("CustomerCode"),
                
//                    Status = row.Field<string>("Status")

//                }).ToList();


//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = modelList;
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
        public async Task<ResultVM> ListCustomersBySalePersonAndBranch(int salePersonId, int branchId, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                // If no connection is passed, create a new one
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                // SQL query to get the customers based on SalePersonId and BranchId
                string query = @"
            SELECT
M.Id, 
M.Name,
sp.BranchId,
M.Code,
M.Address,
M.Email,
isnull(M.RegularDiscountRate,0)RegularDiscountRate

            
FROM Customers M
            
LEFT OUTER JOIN SalePersonRoutes sp ON sp.RouteId = M.RouteId
            
WHERE 
M.IsArchive != 1 AND M.BranchId = @CusBranchId AND  sp.BranchId = @BranchId AND sp.SalePersonId = @SalePersonId
        ";

                // Create the SQL command and set parameters
                SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@SalePersonId", salePersonId);
                cmd.Parameters.AddWithValue("@BranchId", branchId);
                cmd.Parameters.AddWithValue("@CusBranchId", branchId);

                // Create a data adapter to fill the DataTable
                SqlDataAdapter objComm = new SqlDataAdapter(cmd);
                objComm.Fill(dataTable);

                // Convert the DataTable rows to a list of CustomerDataVM objects
                var modelList = dataTable.AsEnumerable().Select(row => new CustomerVM
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    Code = row.Field<string>("Code"),
                    Address = row.Field<string>("Address"),
                    Email = row.Field<string>("Email"),
                    BranchId= row.Field<int>("BranchId"),

                    //RegularDiscountRate = row.Field<decimal>("RegularDiscountRate")

                }).ToList();

                // If there are any customers, set the status to Success
                if (modelList.Any())
                {
                    result.Status = "Success";
                    result.Message = "Data retrieved successfully.";
                    result.DataVM = modelList;
                }
                else
                {
                    result.Status = "Fail";
                    result.Message = "No customers found for the given SalePersonId and BranchId.";
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception and return the error message
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                // Close the connection if it was opened in this method
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> BranchWiseDevitCrdit(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
            SL,
            a.Code,
            InvoiceDateTime,
            CustomerId,
            c.Code AS CustomerCode,
            c.Name AS CustomerName,
            c.Address AS CustomerAddress,
            ISNULL(a.Opening, 0) AS Opening,
            DrAmount,
            SUM(ISNULL(a.Opening, 0) + ISNULL(DrAmount, 0) - ISNULL(CrAmount, 0))
                OVER(PARTITION BY a.CustomerId
                      ORDER BY a.InvoiceDateTime, a.Code, SL
                      ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CurrentBalance,
            Remarks
        FROM (
            SELECT *
            FROM (
                SELECT
                    'D' AS SL,
                    Code,
                    InvoiceDateTime,
                    CustomerId,
                    0 AS Opening,
                    0 AS DrAmount,
                    GrandTotalAmount AS CrAmount,
                    'Sales' AS Remarks
                FROM SaleDeleveries h
                WHERE 1 = 1

                UNION ALL

                SELECT
                    'B' AS SL,
                    ISNULL(s.Code, 0) AS Code,
                    NULL AS InvoiceDateTime, -- Placeholder for InvoiceDateTime
                    ISNULL(s.CustomerId, 0) AS CustomerId,
                    0 AS Opening,
                    b.Amount AS DrAmount,
                    0 AS CrAmount,
                    b.ModeOfPayment + '~ ' + '~ ' AS Remarks
                FROM CustomerPaymentCollection b
                LEFT OUTER JOIN SaleDeleveries s ON b.CustomerId = s.CustomerId
                WHERE 1 = 1
            ) AS a
        ) AS a
        LEFT OUTER JOIN Customers c ON a.CustomerId = c.Id
        WHERE c.IsActive = 1";

                // Apply filters for Id and Date range
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND c.Id = @Id ";
                }

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    query += " AND CAST(a.InvoiceDateTime AS DATE) BETWEEN @FromDate AND @ToDate ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                query += @" ORDER BY c.Id, a.InvoiceDateTime, a.Code, SL";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions parameters
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
                }

                objComm.Fill(dataTable);

                var lst = new List<CustomerVM>();
                int serialNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new CustomerVM
                    {
                        SL = serialNumber,
                        Id = Convert.ToInt32(row["CustomerId"]),
                        Name = row["CustomerName"].ToString(),
                        Code = row["CustomerCode"].ToString(),
                        BranchId = Convert.ToInt32(row["CustomerId"])
                    });
                    serialNumber++;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = lst;
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

        public async Task<ResultVM> ReportList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.Name, '') AS Name,
    ISNULL(M.BanglaName, '') AS BanglaName,
    ISNULL(M.CustomerGroupId, 0) AS CustomerGroupId,
    ISNULL(M.Address, '') AS Address,
    ISNULL(M.BanglaAddress, '') AS BanglaAddress,
    ISNULL(M.TelephoneNo, '') AS TelephoneNo,
    ISNULL(M.FaxNo, '') AS FaxNo,
    ISNULL(M.Email, '') AS Email,
    ISNULL(M.TINNo, '') AS TINNo,
    ISNULL(M.BINNo, '') AS BINNo,
    ISNULL(M.NIDNo, '') AS NIDNo,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
    ISNULL(M.ImagePath,'') AS ImagePath
FROM Customers M
WHERE 1 = 1
and Code!='ALL'

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

                var model = new List<CustomerVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        BanglaName = row.Field<string>("BanglaName"),
                        CustomerGroupId = row.Field<int>("CustomerGroupId"),
                        Address = row.Field<string>("Address"),
                        BanglaAddress = row.Field<string>("BanglaAddress"),
                        TelephoneNo = row.Field<string>("TelephoneNo"),
                        FaxNo = row.Field<string>("FaxNo"),
                        Email = row.Field<string>("Email"),
                        TINNo = row.Field<string>("TINNo"),
                        BINNo = row.Field<string>("BINNo"),
                        NIDNo = row.Field<string>("NIDNo"),
                        Comments = row.Field<string>("Comments"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string>("LastModifiedOn"),
                        ImagePath = row.Field<string>("ImagePath")
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

    }

}
