using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml;
using Newtonsoft.Json;
using System.Transactions;

namespace ShampanPOS.Repository
{


    public class ProductPriceGroupRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(ProductPriceGroupVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                INSERT INTO ProductPriceGroups
                (
                 Name, EffectDate,IsArchive,IsActive,CreatedBy, CreatedOn
                )
                VALUES 
                (
                 @Name, @EffectDate,@IsArchive,@IsActive, @CreatedBy, @CreatedOn
                );
                SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", vm.Name);
                    cmd.Parameters.AddWithValue("@EffectDate", vm.EffectDate);
                    cmd.Parameters.AddWithValue("@IsArchive", false);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Insert ProductPriceGroup Details
                if (vm.ProductPriceGroupDetails != null && vm.ProductPriceGroupDetails.Count > 0)
                {
                    foreach (var detail in vm.ProductPriceGroupDetails)
                    {
                        detail.ProductPriceGroupId = vm.Id;

                        ResultVM detailResult = InsertDetails(detail, conn, transaction);

                        if (detailResult.Status != "Success")
                        {
                            throw new Exception("Failed to insert Sale Details: " + detailResult.Message);
                        }
                    }
                }

                #region BulKInsert

                if (vm.BranchProfileList != null && vm.BranchProfileList.Count > 0)
                {
                    foreach (var branchprofile in vm.BranchProfileList)
                    {
                        if (vm.ProductPriceGroupDetails != null && vm.ProductPriceGroupDetails.Count > 0)
                        {

                            DataTable detailsTable = new DataTable();
                            detailsTable.Columns.Add("ProductId", typeof(int));
                            detailsTable.Columns.Add("BranchId", typeof(int));
                            detailsTable.Columns.Add("BatchNo", typeof(string));
                            detailsTable.Columns.Add("PriceCategory", typeof(string));

                            detailsTable.Columns.Add("EntryDate", typeof(string));
                            detailsTable.Columns.Add("EffectDate", typeof(string));
                            detailsTable.Columns.Add("MFGDate", typeof(string));
                            detailsTable.Columns.Add("EXPDate", typeof(string));

                            detailsTable.Columns.Add("SD", typeof(decimal));
                            detailsTable.Columns.Add("SDAmount", typeof(decimal));
                            detailsTable.Columns.Add("VATRate", typeof(decimal));
                            detailsTable.Columns.Add("VATAmount", typeof(decimal));
                            detailsTable.Columns.Add("CostPrice", typeof(decimal));
                            detailsTable.Columns.Add("SalesPrice", typeof(decimal));
                            detailsTable.Columns.Add("PurchasePrice", typeof(decimal));
                            detailsTable.Columns.Add("CreatedBy", typeof(string));

                            detailsTable.Columns.Add("CreatedOn", typeof(string));

                            detailsTable.Columns.Add("LastModifiedBy", typeof(string));

                            detailsTable.Columns.Add("LastModifiedOn", typeof(string));

                            detailsTable.Columns.Add("CreatedFrom", typeof(string));
                            detailsTable.Columns.Add("LastUpdateFrom", typeof(string));

                            foreach (var detail in vm.ProductPriceGroupDetails)
                            {
                                detailsTable.Rows.Add(

                                    detail.ProductId,
                                    branchprofile.Id,
                                    DBNull.Value,
                                    vm.Name,

                                    DBNull.Value,
                                    vm.EffectDate,
                                    DBNull.Value,
                                    DBNull.Value,

                                    0,
                                    0,
                                    detail.VATRate,
                                    0,
                                    detail.CosePrice,
                                    detail.SalePrice,
                                    0,
                                    vm.CreatedBy,

                                    DateTime.Now,

                                    DBNull.Value,

                                    DBNull.Value,

                                    DBNull.Value,
                                    DBNull.Value

                                    );
                            }

                            var resultt = await BulkInsert("ProductSalePriceBatchHistories", detailsTable, conn, transaction);

                            if (resultt.Status.ToLower() != "success")
                            {
                                return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
                            }

                            if (resultt.Status.ToLower() == "success" && isNewConnection)
                            {
                                //transaction.Commit();
                            }

                        }
                    }
                }

                #endregion

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.Id = vm.Id.ToString();
                result.DataVM = vm;

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

        public ResultVM InsertDetails(ProductPriceGroupDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"

INSERT INTO ProductPriceGroupDetails
(ProductPriceGroupId, ProductId, CosePrice, SalePrice, VATRate)
VALUES 
(@ProductPriceGroupId, @ProductId, @CosePrice, @SalePrice, @VATRate);
SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {

                    cmd.Parameters.AddWithValue("@ProductPriceGroupId", details.ProductPriceGroupId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@CosePrice", details.CosePrice ?? 0);
                    cmd.Parameters.AddWithValue("@SalePrice", details.SalePrice ?? 0);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }



        // Update Method
        public async Task<ResultVM> Update(ProductPriceGroupVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                UPDATE ProductPriceGroups
                SET 

                --Name = @Name,
                EffectDate = @EffectDate, 
                LastModifiedBy = @LastModifiedBy,
                LastModifiedOn = GETDATE()

                WHERE Id = @Id"

        ;

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    //cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EffectDate", vm.EffectDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();


        //            foreach (ProductPriceGroupDetailVM item in vm.ProductPriceGroupDetails)
        //            {
        //                query = @"
        //                      Update ProductPriceGroupDetails set
        //                       CosePrice =@CosePrice
        //                      ,SalePrice=@SalePrice
        //                      ,VATRate=@VATRate
        //                       where Id=@Id"
        //;
        //                SqlCommand cmdDetails = new SqlCommand(query, conn, transaction);
        //                cmdDetails.Parameters.AddWithValue("@Id", item.Id);
        //                cmdDetails.Parameters.AddWithValue("@CosePrice", item.CosePrice ?? 0);
        //                cmdDetails.Parameters.AddWithValue("@SalePrice", item.SalePrice ?? 0);
        //                cmdDetails.Parameters.AddWithValue("@VATRate", item.VATRate ?? 0);
        //                cmdDetails.ExecuteNonQuery();
        //            }



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


                // Insert ProductPriceGroup Details
                if (vm.ProductPriceGroupDetails != null && vm.ProductPriceGroupDetails.Count > 0)
                {
                    foreach (var detail in vm.ProductPriceGroupDetails)
                    {
                        detail.ProductPriceGroupId = vm.Id;

                        ResultVM detailResult = InsertDetails(detail, conn, transaction);

                        if (detailResult.Status != "Success")
                        {
                            throw new Exception("Failed to insert Sale Details: " + detailResult.Message);
                        }
                    }
                }



                #region BulKInsert

                if (vm.BranchProfileList != null && vm.BranchProfileList.Count > 0)
                {
                    foreach (var branchprofile in vm.BranchProfileList)
                    {
                        if (vm.ProductPriceGroupDetails != null && vm.ProductPriceGroupDetails.Count > 0)
                        {

                            DataTable detailsTable = new DataTable();
                            detailsTable.Columns.Add("ProductId", typeof(int));
                            detailsTable.Columns.Add("BranchId", typeof(int));
                            detailsTable.Columns.Add("BatchNo", typeof(string));
                            detailsTable.Columns.Add("PriceCategory", typeof(string));

                            detailsTable.Columns.Add("EntryDate", typeof(string));
                            detailsTable.Columns.Add("EffectDate", typeof(string));
                            detailsTable.Columns.Add("MFGDate", typeof(string));
                            detailsTable.Columns.Add("EXPDate", typeof(string));

                            detailsTable.Columns.Add("SD", typeof(decimal));
                            detailsTable.Columns.Add("SDAmount", typeof(decimal));
                            detailsTable.Columns.Add("VATRate", typeof(decimal));
                            detailsTable.Columns.Add("VATAmount", typeof(decimal));
                            detailsTable.Columns.Add("CostPrice", typeof(decimal));
                            detailsTable.Columns.Add("SalesPrice", typeof(decimal));
                            detailsTable.Columns.Add("PurchasePrice", typeof(decimal));
                            detailsTable.Columns.Add("CreatedBy", typeof(string));

                            detailsTable.Columns.Add("CreatedOn", typeof(string));

                            detailsTable.Columns.Add("LastModifiedBy", typeof(string));

                            detailsTable.Columns.Add("LastModifiedOn", typeof(string));

                            detailsTable.Columns.Add("CreatedFrom", typeof(string));
                            detailsTable.Columns.Add("LastUpdateFrom", typeof(string));

                            foreach (var detail in vm.ProductPriceGroupDetails)
                            {
                                detailsTable.Rows.Add(

                                    detail.ProductId,
                                    branchprofile.Id,
                                    DBNull.Value,
                                    vm.Name,

                                    DBNull.Value,
                                    vm.EffectDate,
                                    DBNull.Value,
                                    DBNull.Value,

                                    0,
                                    0,
                                    detail.VATRate,
                                    0,
                                    detail.CosePrice,
                                    detail.SalePrice,
                                    0,
                                    vm.LastModifiedBy,

                                    DateTime.Now,

                                    DBNull.Value,

                                    DBNull.Value,

                                    DBNull.Value,
                                    DBNull.Value

                                    );
                            }

                            var resultt = await BulkInsert("ProductSalePriceBatchHistories", detailsTable, conn, transaction);

                            if (resultt.Status.ToLower() != "success")
                            {
                                return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
                            }

                            if (resultt.Status.ToLower() == "success" && isNewConnection)
                            {
                                //transaction.Commit();
                            }

                        }
                    }
                }

                #endregion




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

                string deleteFiscalYearsQuery = $"DELETE FROM FiscalYears WHERE Id IN ({inClause})";

                string deleteFiscalYearDetailsQuery = $"DELETE FROM FiscalYearDetails WHERE FiscalYearId IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(deleteFiscalYearsQuery, conn, transaction))
                {

                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("No rows were deleted.");
                    }

                }


                using (SqlCommand cmd = new SqlCommand(deleteFiscalYearDetailsQuery, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("No rows were deleted.");
                    }
                }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = $"Data deleted successfully.";

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
            ISNULL(FORMAT(M.EffectDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') EffectDate,
            ISNULL(M.IsArchive, 0) AS IsArchive,
            ISNULL(M.IsActive, 0) AS IsActive,       
            ISNULL(M.CreatedBy, '') AS CreatedBy,
            ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,           
            ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn

            FROM ProductPriceGroups M

            WHERE 1=1"

        ;

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

                var model = new List<ProductPriceGroupVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductPriceGroupVM
                    {
                        Id = row.Field<int>("Id"),
                        Name = row.Field<string>("Name"),
                        EffectDate = row.Field<string>("EffectDate"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedBy = row.Field<string?>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string?>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string?>("LastModifiedOn")

                    });
                }

                var detailsDataList = DetailsList(new[] { "D.ProductPriceGroupId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<ProductPriceGroupDetailVM>>(json);

                    model.FirstOrDefault().ProductPriceGroupDetails = details;
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
            ISNULL(M.Id, 0) AS Id,
            ISNULL(M.Year, 0) AS Year,
            ISNULL(M.YearStart, '1900-01-01') AS YearStart,
            ISNULL(M.YearEnd, '1900-01-01') AS YearEnd,
            ISNULL(M.YearLock, 0) AS YearLock,
            ISNULL(M.Remarks, '') AS Remarks,
            ISNULL(M.CreatedBy, '') AS CreatedBy,
            ISNULL(M.CreatedOn, '1900-01-01') AS CreatedOn,
            ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(M.LastModifiedOn, '1900-01-01') AS LastModifiedOn
        FROM FiscalYears M
        WHERE 1=1";

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
FROM FiscalYears
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

                var data = new GridEntity<ProductPriceGroupVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
             SELECT COUNT(DISTINCT H.Id) AS totalcount
             FROM  ProductPriceGroups H
            Where 1=1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductPriceGroupVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.Name, '') AS Name,	
            ISNULL(FORMAT(H.EffectDate, 'yyyy-MM-dd'),'1900-01-01') AS EffectDate, 

            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(H.CreatedOn, '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '''') AS LastModifiedBy,
            ISNULL(H.LastModifiedOn, '1900-01-01') AS LastModifiedOn,
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom

            FROM ProductPriceGroups H 

             Where 1=1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductPriceGroupVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductPriceGroupVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                Select

                       D.Id
                      ,D.[ProductPriceGroupId]
                      ,D.[ProductId]
                      ,D.CosePrice
                      ,D.SalePrice
                      ,D.VATRate

                      ,P.Code
	                  ,P.Name
	                  ,P.BanglaName


                FROM ProductPriceGroupDetails D
                Left Outer Join Products P On P.Id = D.ProductId

                where 1=1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Details Data retrieved successfully.";
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

        public bool DuplicateFiscal(int? year, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            bool exists = false;

            try
            {
                // Check if connection is passed in; if not, create a new one
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                // SQL query to retrieve fiscal year record by id
                string query = @"
            SELECT COUNT(1)
            FROM FiscalYears M
            WHERE Year = @year";

                // Create and configure SQL command
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@year", year);

                // If a transaction is provided, assign it to the command
                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                // Execute the query and check if any record exists
                exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;

            }
            catch (Exception ex)
            {
                // Handle errors (log exception if needed)
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Close the connection if it was opened in this method
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            // Return true if fiscal year exists, otherwise false
            return exists;
        }

    }

}
