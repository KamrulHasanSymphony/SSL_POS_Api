using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShampanPOS.Repository
{
    public class SupplierProductRepository : CommonRepository
    {
        // Insert Method
        //public async Task<ResultVM> Insert(SupplierProductVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        if (transaction == null)
        //        {
        //            transaction = conn.BeginTransaction();
        //        }

        //        string query = @"
        //        INSERT INTO SupplierProduct
        //        (
        //             SupplierId,ProductId,UserId,CompanyId,
        //             IsArchive, IsActive, CreatedBy, CreatedOn
        //        )
        //        VALUES
        //        (
        //            @SupplierId,@ProductId,@UserId,@CompanyId,
        //             @IsArchive, @IsActive, @CreatedBy, @CreatedOn 
        //        );
        //        SELECT SCOPE_IDENTITY();";

        //        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //        {
        //            cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
        //            cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);

        //            cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
        //            cmd.Parameters.AddWithValue("@IsActive", true);
        //            cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        //            cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

        //            vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

        //            result.Status = "Success";
        //            result.Message = "Data inserted successfully.";
        //            result.Id = vm.Id.ToString();
        //            result.DataVM = vm;


        //            if (isNewConnection)
        //            {
        //                transaction.Commit();
        //            }

        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //        {
        //            transaction.Rollback();
        //        }

        //        result.ExMessage = ex.Message;
        //        result.Message = "Error in Insert.";
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        public async Task<ResultVM> InsertOrUpdateSupplierProduct(MasterItemVM details = null,SqlConnection conn = null, SqlTransaction transaction = null,SupplierProductVM supplierproduct = null)
        {
            bool isNewConnection = false;

            ResultVM result = new ResultVM {Status = "Fail",Message = "Error",ExMessage = null,Id = "0",DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                // 1️⃣ Check if the Supplier-Product mapping already exists
                string checkQuery = @"
            SELECT Id
            FROM SupplierProduct
            WHERE SupplierId = @SupplierId
            AND ProductId = @ProductId";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                {
                    checkCmd.Parameters.AddWithValue("@SupplierId", supplierproduct.SupplierId);
                    checkCmd.Parameters.AddWithValue("@ProductId", details.Id);

                    var obj = await checkCmd.ExecuteScalarAsync();

                    if (obj != null)
                    {
                        // 2️⃣ Exists → skip insert
                        result.Status = "Success";
                        result.Message = "SupplierProduct already exists. Skipped.";
                        result.Id = obj.ToString();
                        result.DataVM = details;
                        return result;
                    }
                }

                // 3️⃣ Insert new record if not exists
                string insertQuery = @"
            INSERT INTO SupplierProduct
            (
                SupplierId, ProductId, UserId, CompanyId,
                IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
            )
            VALUES
            (
                @SupplierId, @ProductId, @UserId, @CompanyId,
                @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom
            );
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("@SupplierId", supplierproduct.SupplierId);
                    insertCmd.Parameters.AddWithValue("@ProductId", details.Id);
                    insertCmd.Parameters.AddWithValue("@UserId", supplierproduct.UserId ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@CompanyId", supplierproduct.CompanyId ?? (object)DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@IsArchive", details.IsArchive);
                    insertCmd.Parameters.AddWithValue("@IsActive", true);
                    insertCmd.Parameters.AddWithValue("@CreatedBy", supplierproduct.CreatedBy);
                    insertCmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@CreatedFrom", supplierproduct.CreatedFrom ?? (object)DBNull.Value);

                    var newId = await insertCmd.ExecuteScalarAsync();
                    result.Id = Convert.ToInt32(newId).ToString();
                }

                result.Status = "Success";
                result.Message = "SupplierProduct inserted successfully.";
                result.DataVM = details;

                if (isNewConnection)
                    transaction.Commit();

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                    transaction.Rollback();

                result.ExMessage = ex.ToString();
                result.Message = "Error in InsertOrUpdate.";
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                    conn.Close();
            }
        }
        public async Task<ResultVM> Insert(MasterItemVM details = null, SqlConnection conn = null, SqlTransaction transaction = null, SupplierProductVM supplierproduct = null)
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
             INSERT INTO SupplierProduct
         (
              SupplierId,ProductId,UserId,CompanyId,
              IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
         )
         VALUES
         (
             @SupplierId,@ProductId,@UserId,@CompanyId,
              @IsArchive, @IsActive, @CreatedBy, @CreatedOn ,@CreatedFrom
         );
             SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SupplierId", supplierproduct.SupplierId);
                    cmd.Parameters.AddWithValue("@ProductId", details.Id);
                    cmd.Parameters.AddWithValue("@UserId", supplierproduct.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", supplierproduct.CompanyId ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@SourceType", supplierproduct.SourceType ?? "SP");
                    cmd.Parameters.AddWithValue("@IsArchive", details.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", supplierproduct.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", supplierproduct.CreatedFrom ?? (object)DBNull.Value);

                    details.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                //if (isNewConnection)
                //{
                //    transaction.Commit();
                //}

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.DataVM = details;

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

                result.ExMessage = ex.ToString();
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

        //Update Method
        public async Task<ResultVM> Update(SupplierProductVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE SupplierProduct
SET 

    SupplierId = @SupplierId,
    ProductId = @ProductId,
    UserId = @UserId,
    CompanyId = @CompanyId,
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
                    cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@SourceType", vm.SourceType ?? "SP");
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

                string query = $" UPDATE SupplierProduct SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
	ISNULL(M.SupplierId, 0) AS SupplierId,
	ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(s.Name, '') AS SupplierName,
	ISNULL(M.UserId, 0) AS UserId,
    ISNULL(p.Name, '') AS ProductName,
    ISNULL(M.IsArchive, 0) AS IsArchive,
	ISNULL(M.IsActive, 0) AS IsActive,   
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
	ISNULL(pg.Name, '') AS ProductGroupName
	--ISNULL(pg.Id,0) ProductGroupId

FROM SupplierProduct M
LEFT OUTER JOIN Suppliers s on M.SupplierId = s.Id
LEFT OUTER JOIN Products p on M.ProductId = p.Id
LEFT OUTER JOIN ProductGroups pg ON p.ProductGroupId = pg.Id  

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

                var model = new List<SupplierProductVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SupplierProductVM
                    {
                        Id = row.Field<int>("Id"),
                        SupplierId = row.Field<int>("SupplierId"),
                        //ProductGroupId = row.Field<int>("ProductGroupId"),
                        ProductId = row.Field<int>("ProductId"),
                        SupplierName = row.Field<string>("SupplierName"),
                        ProductName = row.Field<string>("ProductName"),
                        UserId = row.Field<string>("UserId"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        //SourceType = row.Field<string>("SourceType"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string>("LastModifiedOn")
                    });
                }

                foreach (var parent in model)
                {
                    // pass single department id as conditional value
                    var detailsResult = DetailsList(new[] { "M.SupplierId" }, new[] { parent.SupplierId.ToString() }, vm, conn, transaction);

                    if (detailsResult.Status == "Success" && detailsResult.DataVM is DataTable dts)
                    {
                        string json = JsonConvert.SerializeObject(dts);
                        var details = JsonConvert.DeserializeObject<List<MasterItemVM>>(json);

                        parent.MasterItemList = details ?? new List<MasterItemVM>();
                    }
                    else
                    {
                        parent.MasterItemList = new List<MasterItemVM>();
                    }
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


        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail" };
            DataTable dataTable = new DataTable();
            try
            {
                string query = @"
	SELECT
	ISNULL(M.SupplierId, 0) AS SupplierId,
	ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(s.Name, '') AS SupplierName,
	ISNULL(M.UserId, 0) AS UserId,
    ISNULL(p.Name, '') AS Name,
    ISNULL(M.IsArchive, 0) AS IsArchive,
	ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
	--ISNULL(pg.Id,0) ProductGroupId,
    ISNULL(p.Code,'') Code,
	ISNULL(pg.Name, '') AS ProductGroupName,
	ISNULL(p.Id,0) Id

FROM SupplierProduct M
LEFT OUTER JOIN Suppliers s on M.SupplierId = s.Id
LEFT OUTER JOIN Products p on M.ProductId = p.Id
LEFT OUTER JOIN ProductGroups pg ON p.ProductGroupId = pg.Id  

WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter da = CreateAdapter(query, conn, transaction);
                da.SelectCommand = ApplyParameters(da.SelectCommand, conditionalFields, conditionalValues);
                da.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
            }
            catch (Exception ex)
            {
                result.Message = MessageModel.DetailInsertFailed;
                result.ExMessage = ex.ToString();
            }
            return result;
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
FROM SupplierProduct
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

                var data = new GridEntity<SupplierProductVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(*) AS totalcount
FROM (
    SELECT DISTINCT
        M.SupplierId
    FROM SupplierProduct M
    LEFT JOIN Suppliers s ON M.SupplierId = s.Id
    LEFT JOIN MasterSupplier ms ON M.SupplierId = ms.Id
    WHERE 1 = 1

";

                sqlQuery += (options.filter.Filters.Count > 0 ?
                        " AND (" + GridQueryBuilder<SupplierProductVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) t

-- Data query
SELECT *
FROM (
    SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                        (options.sort.Count > 0 ?
                            options.sort[0].field + " " + options.sort[0].dir :
                            "SupplierId DESC") + @") AS rowindex,

        *
    FROM (
        SELECT DISTINCT
    ISNULL(M.SupplierId, 0) AS SupplierId,

    ISNULL(s.Name, ms.Name) AS SupplierName,
    ISNULL(s.Code,'') AS SupplierCode,
    ISNULL(s.City,'-') AS SupplierCity,
    ISNULL(s.Email,'-') AS SupplierEmail,
    ISNULL(s.TelephoneNo,'-') AS SupplierTelephoneNo,

    ISNULL(M.UserId, 0) AS UserId,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsActive, 0) AS IsActive,

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,

    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn

FROM SupplierProduct M
LEFT JOIN Suppliers s ON M.SupplierId = s.Id
LEFT JOIN MasterSupplier ms ON M.SupplierId = ms.Id
WHERE 1 = 1
";

                sqlQuery += (options.filter.Filters.Count > 0 ?
                        " AND (" + GridQueryBuilder<SupplierProductVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) d
) a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";




                data = KendoGrid<SupplierProductVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
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


        public async Task<ResultVM> ReportList(
            string[] conditionalFields,
            string[] conditionalValues,
            SupplierProductReportVM vm = null,
            SqlConnection conn = null,
            SqlTransaction transaction = null,
            bool productWiseSupplier = true)
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

                string query = "";

                if (productWiseSupplier)
                {
                    if (vm.IsSummary)
                    {
                        query = @"
SELECT 
PG.Name AS ProductGroupName,
U.Name AS UOMName,
P.Code AS ProductCode,
P.Name AS ProductName,
P.SDRate,
P.VATRate,
P.PurchasePrice,
STRING_AGG(SG.Name, ', ') AS SupplierGroupName,
STRING_AGG(S.Name, ', ') AS SupplierName,
STRING_AGG(S.Code, ', ') AS SupplierCode,
STRING_AGG(S.TelephoneNo, ', ') AS TelephoneNo,
STRING_AGG(S.Address, ', ') AS SupplierAddress
FROM SupplierProduct SP
INNER JOIN Products P ON SP.ProductId = P.Id
INNER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
INNER JOIN UOMs U ON P.UOMId = U.Id
INNER JOIN Suppliers S ON SP.SupplierId = S.Id
INNER JOIN SupplierGroups SG ON S.SupplierGroupId = SG.Id
WHERE SP.IsActive = 1 AND SP.IsArchive = 0
GROUP BY PG.Name, U.Name, P.Code, P.Name, P.SDRate, P.VATRate, P.PurchasePrice
ORDER BY PG.Name, P.Name;";
                    }
                    else
                    {
                        query = @"
SELECT 
PG.Name AS ProductGroupName,
U.Name AS UOMName,
P.Code AS ProductCode,
P.Name AS ProductName,
P.SDRate,
P.VATRate,
P.PurchasePrice,
SG.Name AS SupplierGroupName,
S.Name AS SupplierName,
S.Code AS SupplierCode,
S.TelephoneNo,
S.Address AS SupplierAddress
FROM SupplierProduct SP
INNER JOIN Products P ON SP.ProductId = P.Id
INNER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
INNER JOIN UOMs U ON P.UOMId = U.Id
INNER JOIN Suppliers S ON SP.SupplierId = S.Id
INNER JOIN SupplierGroups SG ON S.SupplierGroupId = SG.Id
WHERE SP.IsActive = 1 AND SP.IsArchive = 0
ORDER BY PG.Name, P.Name, S.Name;";
                    }
                }
                else
                {
                    if (vm.IsSummary)
                    {
                        query = @"
SELECT 
SG.Name AS SupplierGroupName,
S.Name AS SupplierName,
S.Code AS SupplierCode,
S.TelephoneNo,
S.Address AS SupplierAddress,
STRING_AGG(PG.Name, ', ') AS ProductGroupName,
STRING_AGG(P.Code, ', ') AS ProductCode,
STRING_AGG(P.Name, ', ') AS ProductName,
STRING_AGG(CAST(P.SDRate AS varchar), ', ') AS SDRate,
STRING_AGG(CAST(P.VATRate AS varchar), ', ') AS VATRate,
STRING_AGG(CAST(P.PurchasePrice AS varchar), ', ') AS PurchasePrice
FROM SupplierProduct SP
INNER JOIN Suppliers S ON SP.SupplierId = S.Id
INNER JOIN SupplierGroups SG ON S.SupplierGroupId = SG.Id
INNER JOIN Products P ON SP.ProductId = P.Id
INNER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
WHERE SP.IsActive = 1 AND SP.IsArchive = 0
GROUP BY SG.Name, S.Name, S.Code, S.TelephoneNo, S.Address
ORDER BY SG.Name, S.Name;";
                    }
                    else
                    {
                        query = @"
SELECT 
SG.Name AS SupplierGroupName,
S.Name AS SupplierName,
S.Code AS SupplierCode,
S.TelephoneNo,
S.Address AS SupplierAddress,
PG.Name AS ProductGroupName,
P.Code AS ProductCode,
P.Name AS ProductName,
P.SDRate,
P.VATRate,
P.PurchasePrice
FROM SupplierProduct SP
INNER JOIN Suppliers S ON SP.SupplierId = S.Id
INNER JOIN SupplierGroups SG ON S.SupplierGroupId = SG.Id
INNER JOIN Products P ON SP.ProductId = P.Id
INNER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
WHERE SP.IsActive = 1 AND SP.IsArchive = 0
ORDER BY SG.Name, S.Name, P.Name;";
                    }
                }

                // Apply additional conditions
                if (!query.ToUpper().Contains("WHERE"))
                {
                    query += " WHERE 1=1 ";
                }
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                // Optional: filter by ProductId or SupplierId if sent in vm
                objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId);
                objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", vm.SupplierId);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new SupplierProductReportVM
                {
                    Id = 0, // no PurchaseId here
                    ProductName = dataTable.Columns.Contains("ProductName") ? row["ProductName"]?.ToString() : "",
                    SupplierName = dataTable.Columns.Contains("SupplierName") ? row["SupplierName"]?.ToString() : "",
                    ProductGroupName = dataTable.Columns.Contains("ProductGroupName") ? row["ProductGroupName"]?.ToString() : "",
                    SupplierGroupName = dataTable.Columns.Contains("SupplierGroupName") ? row["SupplierGroupName"]?.ToString() : "",
                    ProductCode = dataTable.Columns.Contains("ProductCode") ? row["ProductCode"]?.ToString() : "",
                    SupplierCode = dataTable.Columns.Contains("SupplierCode") ? row["SupplierCode"]?.ToString() : "",
                    TelephoneNo = dataTable.Columns.Contains("TelephoneNo") ? row["TelephoneNo"]?.ToString() : "",
                    SupplierAddress = dataTable.Columns.Contains("SupplierAddress") ? row["SupplierAddress"]?.ToString() : "",
                    SDRate = dataTable.Columns.Contains("SDRate") ? Convert.ToDecimal(row["SDRate"]) : 0,
                    VATRate = dataTable.Columns.Contains("VATRate") ? Convert.ToDecimal(row["VATRate"]) : 0,
                    //PurchasePrice = dataTable.Columns.Contains("PurchasePrice") ? Convert.ToDecimal(row["PurchasePrice"]) : 0
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


    }
}
