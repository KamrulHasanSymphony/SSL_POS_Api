using Newtonsoft.Json;
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
    public class MasterSupplierItemRepository : CommonRepository
    {

        // Insert Method
        public async Task<ResultVM> Insert( MasterItemVM details = null, SqlConnection conn = null, SqlTransaction transaction = null, MasterSupplierItemVM mastersupplieritem = null)
        {
            bool isNewConnection = false;

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

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

                string query = @"
                    INSERT INTO MasterSupplierItem
                    (
                        MasterSupplierId,
                        MasterProductId,
                        UserId,
                        CompanyId,
                        IsArchive,
                        IsActive,
                        CreatedBy,
                        CreatedOn
                    )
                    VALUES
                    (
                        @MasterSupplierId,
                        @MasterProductId,
                        @UserId,
                        @CompanyId,
                        @IsArchive,
                        @IsActive,
                        @CreatedBy,
                        @CreatedOn
                    );
                    SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@MasterSupplierId", details.MasterSupplierId);
                        cmd.Parameters.AddWithValue("@MasterProductId", details.ProductId);
                        cmd.Parameters.AddWithValue("@UserId", mastersupplieritem.UserId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CompanyId", mastersupplieritem.CompanyId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsArchive", details.IsArchive);
                        cmd.Parameters.AddWithValue("@IsActive", true);
                        cmd.Parameters.AddWithValue("@CreatedBy", mastersupplieritem.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                        details.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.DataVM = details;

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
        // Update Method
        public async Task<ResultVM> Update(MasterSupplierItemVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE MasterSupplierItem
SET 

    MasterSupplierId = @MasterSupplierId,
    MasterProductId = @MasterProductId,
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
                    cmd.Parameters.AddWithValue("@MasterSupplierId", vm.MasterSupplierId);
                    cmd.Parameters.AddWithValue("@MasterProductId", vm.MasterProductId);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
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

                string query = $" UPDATE MasterSupplierItem SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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

                // Base query
                string query = @"
        SELECT
            ISNULL(M.Id, 0) AS Id,
            ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
            ISNULL(M.MasterProductId, 0) AS MasterProductId,
            ISNULL(s.Name, '') AS MasterSupplierName,
            ISNULL(M.UserId, 0) AS UserId,
            ISNULL(p.Name, '') AS MasterProductName,
            ISNULL(M.IsArchive, 0) AS IsArchive,
            ISNULL(M.IsActive, 0) AS IsActive,   
            ISNULL(M.CreatedBy, '') AS CreatedBy,
            FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
            ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
            FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,
            ISNULL(pg.Name, '') AS ProductGroupName  

        FROM MasterSupplierItem M
        LEFT OUTER JOIN MasterSupplier s ON M.MasterSupplierId = s.Id
        LEFT OUTER JOIN MasterItem p ON M.MasterProductId = p.Id
        LEFT OUTER JOIN MasterItemGroup pg ON p.MasterItemGroupId = pg.Id  

        WHERE 1 = 1
        AND M.MasterSupplierId = @MasterSupplierId";  // Ensure this condition is included

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";  // Add additional conditions if vm.Id is provided
                }

                // Apply additional dynamic conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // Set additional conditions for parameters
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                // Ensure the @MasterSupplierId parameter is added
                if (conditionalValues != null && conditionalValues.Length > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@MasterSupplierId", conditionalValues[0]); // Assuming conditionalValues[0] is the MasterSupplierId
                }

                // Add other parameters
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<MasterSupplierItemVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new MasterSupplierItemVM
                    {
                        Id = row.Field<int>("Id"),
                        MasterSupplierId = row.Field<int>("MasterSupplierId"),
                        MasterProductId = row.Field<int>("MasterProductId"),
                        MasterSupplierName = row.Field<string>("MasterSupplierName"),
                        MasterProductName = row.Field<string>("MasterProductName"),
                        UserId = row.Field<string>("UserId"),
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





        //public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null,
        // SqlConnection conn = null, SqlTransaction transaction = null)
        //     {
        //         DataTable dt = new DataTable();
        //         ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

        //         try
        //         {
        //             if (conn == null) throw new Exception("Database connection failed!");

        //             string query = @"
        //         SELECT
        //             ISNULL(M.Id, 0) AS Id,
        //             ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
        //             ISNULL(M.MasterProductId, 0) AS MasterProductId,
        //             ISNULL(s.Name, '') AS MasterSupplierName,
        //             ISNULL(M.UserId, '') AS UserId,
        //             ISNULL(p.Name, '') AS MasterProductName,
        //             ISNULL(p.MasterItemGroupId, 0) AS MasterItemGroupId, -- Added this field
        //             ISNULL(M.IsArchive, 0) AS IsArchive,
        //             ISNULL(M.IsActive, 0) AS IsActive,
        //             ISNULL(M.CreatedBy, '') AS CreatedBy,
        //             FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
        //             ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
        //             FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
        //         FROM MasterSupplierItem M
        //         LEFT OUTER JOIN MasterSupplier s ON M.MasterSupplierId = s.Id
        //         LEFT OUTER JOIN MasterItem p ON M.MasterProductId = p.Id
        //         WHERE 1=1
        //     ";

        //             if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //                 query += " AND M.Id = @Id ";

        //             query = ApplyConditions(query, conditionalFields, conditionalValues, false);

        //             SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
        //             adapter.SelectCommand = ApplyParameters(adapter.SelectCommand, conditionalFields, conditionalValues);

        //             if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //                 adapter.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);

        //             adapter.Fill(dt);

        //             var list = dt.AsEnumerable().Select(row => new MasterSupplierItemVM
        //             {
        //                 Id = row.Field<int>("Id"),
        //                 MasterSupplierId = row.Field<int>("MasterSupplierId"),
        //                 MasterProductId = row.Field<int>("MasterProductId"),
        //                 MasterSupplierName = row.Field<string>("MasterSupplierName"),
        //                 MasterProductName = row.Field<string>("MasterProductName"),
        //                 UserId = row.Field<string>("UserId"),
        //                 MasterItemGroupId = row.Field<int>("MasterItemGroupId"),  // Map it here
        //                 IsArchive = row.Field<bool>("IsArchive"),
        //                 IsActive = row.Field<bool>("IsActive"),
        //                 CreatedBy = row.Field<string>("CreatedBy"),
        //                 CreatedOn = row.Field<string>("CreatedOn"),
        //                 LastModifiedBy = row.Field<string>("LastModifiedBy"),
        //                 LastModifiedOn = row.Field<string>("LastModifiedOn"),
        //                 SabreList = new List<MasterSupplierItemVM>()
        //             }).ToList();

        //             // ─────────── per-parent details call ───────────
        //             foreach (var parent in list)
        //             {
        //                 // Pass MasterSupplierId and MasterItemGroupId instead of DepartmentId
        //                 var detailsResult = DetailsList(
        //                     new[] { "M.MasterSupplierId", "p.MasterItemGroupId" },
        //                     new[] { parent.MasterSupplierId.ToString(), parent.MasterItemGroupId.ToString() },
        //                     vm, conn, transaction);

        //                 if (detailsResult.Status == "Success" && detailsResult.DataVM is DataTable dts)
        //                 {
        //                     string json = JsonConvert.SerializeObject(dts);
        //                     var details = JsonConvert.DeserializeObject<List<MasterSupplierItemVM>>(json);

        //                     parent.SabreList = details ?? new List<MasterSupplierItemVM>();
        //                 }
        //                 else
        //                 {
        //                     parent.SabreList = new List<MasterSupplierItemVM>();
        //                 }
        //             }

        //             result.Status = "Success";
        //             result.Message = "Data retrieved successfully.";
        //             result.DataVM = list;

        //             return result;
        //         }
        //         catch (Exception ex)
        //         {
        //             result.Message = ex.Message;
        //             result.ExMessage = ex.ToString();
        //             return result;
        //         }
        //     }


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
FROM MasterSupplierItem
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

                var data = new GridEntity<MasterSupplierItemVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT M.Id) AS totalcount
FROM MasterSupplierItem M
LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
LEFT OUTER JOIN MasterItem p on M.MasterProductId = p.Id
WHERE 1 = 1 ";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                                        (options.sort.Count > 0 ?
                                            options.sort[0].field + " " + options.sort[0].dir :
                                            "M.Id DESC ") + @") AS rowindex,
        
    ISNULL(M.Id, 0) AS Id,
	ISNULL(M.MasterSupplierId, 0) AS MasterSupplierId,
	ISNULL(M.MasterProductId, 0) AS MasterItemId,
    ISNULL(s.Name, '') AS MasterSupplierName,
	ISNULL(M.UserId, 0) AS UserId,
    ISNULL(p.Name, '') AS MasterProductName,
    ISNULL(M.IsArchive, 0) AS IsArchive,
	ISNULL(M.IsActive, 0) AS IsActive,   

    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn

FROM MasterSupplierItem M
LEFT OUTER JOIN MasterSupplier s on M.MasterSupplierId = s.Id
LEFT OUTER JOIN MasterItem p on M.MasterProductId = p.Id
WHERE 1 = 1
       ";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<MasterSupplierItemVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<MasterSupplierItemVM>.GetGridData_CMD(options, sqlQuery, "M.Id");
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
    }
}
