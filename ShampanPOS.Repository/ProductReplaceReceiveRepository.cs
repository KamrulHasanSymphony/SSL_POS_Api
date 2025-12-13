using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{
    public class ProductReplaceReceiveRepository : CommonRepository
    {
        // 1. Insert Method
        public async Task<ResultVM> Insert(ProductReplaceReceiveVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO ProductReplaceReceive 
                (
                    Code, BranchId, CustomerId,IsCompleted, CreatedBy, CreatedOn, CreatedFrom, LastUpdateFrom
                )
                VALUES 
                (
                    @Code, @BranchId, @CustomerId,@IsCompleted, @CreatedBy, @CreatedOn, @CreatedFrom, @LastUpdateFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@IsCompleted", false);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "ERP");
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    object newId = cmd.ExecuteScalar();
                    vm.Id = Convert.ToInt32(newId);

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

        // 2. Update Method
        public async Task<ResultVM> Update(ProductReplaceReceiveVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE ProductReplaceReceive 
                SET 
                    BranchId = @BranchId, CustomerId = @CustomerId, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE()
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.CreatedBy ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were updated.";
                        throw new Exception("No rows were updated.");
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

        // 3. Delete Method
        public async Task<ResultVM> Delete(int[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        // 4. List Method


        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,                
                    ISNULL(M.CreatedBy, '') AS CreatedBy,          
                    ISNULL(M.IsPost, 0) AS IsPost,

                    ISNULL(FORMAT(M.ReceiveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS ReceiveDate

                    FROM ProductReplaceReceive M
                    WHERE 1 = 1

 ";

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

                var model = new List<ProductReplaceReceiveVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        ReceiveDate = row["ReceiveDate"].ToString()
                    });
                }

                var detailsDataList = DetailsList(new[] { "D.ProductReplaceReceiveId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<ProductReplaceReceiveDetailsVM>>(json);

                    model.FirstOrDefault().ProductReplaceReceiveDetails = details;
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = model;

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
        // 5. ListAsDataTable Method
        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
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
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.CreatedBy, '') AS CreatedBy
                FROM ProductReplaceReceive M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

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

        // 6. InsertDetails Method
        public async Task<ResultVM> InsertDetails(ProductReplaceReceiveDetailsVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                    INSERT INTO ProductReplaceReceiveDetails
                    (ProductReplaceReceiveId, ProductId, Quantity)
                    VALUES 
                    (@ProductReplaceReceiveId, @ProductId, @Quantity);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductReplaceReceiveId", details.ProductReplaceReceiveId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);


                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details inserted successfully.";
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

        // 7. DetailsList Method
        public async Task<ResultVM> DetailsList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(M.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
                    ISNULL(M.ProductId, 0) AS ProductId,
                    ISNULL(M.Quantity, 0) AS Quantity,

                FROM ProductReplaceReceiveDetails M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveDetailsVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveDetailsVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceReceiveId = Convert.ToInt32(row["ProductReplaceReceiveId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Quantity = Convert.ToDecimal(row["Quantity"]),
                     
                    });
                }

                result.Status = "Success";
                result.Message = "Details retrieved successfully.";
                result.DataVM = model;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error in DetailsList.";
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

        // 8. GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<ProductReplaceReceiveVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
					FROM ProductReplaceReceive H 
					LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
					LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id

					WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                ,ISNULL(H.Id,0)	Id
			,ISNULL(H.Code,'')	Code
		,ISNULL(H.BranchId,0) BranchId
		,ISNULL(BF.Name,0) BranchName
		,ISNULL(H.CustomerId,0) CustomerId
		,ISNULL(C.Name,'') CustomerName
		,ISNULL(H.CreatedBy,'') CreatedBy
		,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
		,ISNULL(H.LastModifiedBy,'') LastModifiedBy
		,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
		,ISNULL(H.CreatedFrom,'') CreatedFrom
		,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom
		,ISNULL(H.IsPost, 0) IsPost
		  ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
		,ISNULL(H.PostedBy, '') PostedBy
		,ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') PostedOn
		FROM ProductReplaceReceive H
		LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
		LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id

		WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"    
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                //data = KendoGrid<ProductReplaceReceiveVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<ProductReplaceReceiveVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        // 9. GetDetailsGridData Method
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<ProductReplaceReceiveDetailsVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT D.Id) AS totalcount
    FROM ProductReplaceReceiveDetails D
    LEFT OUTER JOIN Products P ON D.ProductId = P.Id
    WHERE 1 = 1
";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
        ISNULL(D.Id, 0) AS Id,
        ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
        ISNULL(D.ProductId, 0) AS ProductId,
        ISNULL(D.Quantity, 0) AS Quantity

    FROM ProductReplaceReceiveDetails D
    WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveDetailsVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceReceiveDetailsVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

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

        // 10. ProductReplaceReceiveList Method
        public async Task<ResultVM> ProductReplaceReceiveList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
                FROM ProductReplaceReceive M
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString()
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
                result.Message = "Error in ProductReplaceReceiveList.";
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

        // 11. ProductReplaceReceiveDetailsList Method
        public async Task<ResultVM> ProductReplaceReceiveDetailsList(string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    ISNULL(D.Id, 0) AS Id,
                    ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
                    ISNULL(D.ProductId, 0) AS ProductId,
                    ISNULL(D.Quantity, 0) AS Quantity,


                FROM ProductReplaceReceiveDetails D
                WHERE 1 = 1";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.Fill(dataTable);

                var model = new List<ProductReplaceReceiveDetailsVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceReceiveDetailsVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceReceiveId = Convert.ToInt32(row["ProductReplaceReceiveId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Quantity = Convert.ToDecimal(row["Quantity"]),
                       
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
                result.Message = "Error in ProductReplaceReceiveDetailsList.";
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


        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn, SqlTransaction transaction)
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

                string query = $" UPDATE ProductReplaceReceive SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    cmd.Parameters.AddWithValue("@PostedBy", vm.ModifyBy);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data Posted successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were posted.");
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
            SELECT 

   ISNULL(D.Id, 0) AS Id,
   ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
   ISNULL(D.ProductId, 0) AS ProductId,
   ISNULL(D.Quantity, 0.00) AS Quantity,
   ISNULL(P.Name,'') ProductName,
   ISNULL(P.BanglaName,'') BanglaName, 
   ISNULL(P.CtnSize,'') UOM, 
   ISNULL(P.Code,'') ProductCode
   FROM 
   ProductReplaceReceiveDetails D
   LEFT OUTER JOIN Products P ON D.ProductId = P.Id



   WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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

        public ResultVM ProductReplaceReceiveDetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
   --SELECT 
   --ISNULL(D.Id, 0) AS Id,
   --ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
   --ISNULL(D.ProductId, 0) AS ProductId,
   --ISNULL(D.Quantity, 0.00) AS Quantity,
   --ISNULL(P.Name,'') ProductName,
   --ISNULL(P.BanglaName,'') BanglaName, 
   --ISNULL(P.CtnSize,'') UOM, 
   --ISNULL(P.Code,'') ProductCode
   --FROM 
   --ProductReplaceReceiveDetails D
   --LEFT OUTER JOIN Products P ON D.ProductId = P.Id


SELECT 

ISNULL(RRD.Id, 0) AS Id,
ISNULL(RRD.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
ISNULL(RRD.ProductId, 0) AS ProductId,
ISNULL(RRD.Quantity, 0.00) AS Quantity,
ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.CtnSize,'') UOM, 
ISNULL(P.Code,'') ProductCod
FROM ProductReplaceReceiveDetails RRD
LEFT OUTER JOIN Products P ON RRD.ProductId = P.Id

WHERE RRD.ProductReplaceReceiveId = (
    SELECT Id FROM ProductReplaceReceive WHERE Code = @Code
)
AND RRD.ProductId NOT IN (
    SELECT PRID.ProductId
    FROM ProductReplaceIssueDetails PRID
    WHERE PRID.ProductReplaceIssueId = (
        SELECT Id FROM ProductReplaceIssue WHERE ReceiveCode = @Code
    )
)


--WHERE 1 = 1

";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
                }

                // Apply additional conditions
                //query = ApplyConditions(query, conditionalFields, conditionalValue, false);
                query = ApplyConditions(query, null, null, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                //objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, null, null);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.SearchValue))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Code", vm.SearchValue);
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

        public async Task<ResultVM> ProductReplaceReceiveList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(M.Code, '') AS Code,
                ISNULL(M.BranchId, 0) AS BranchId,
                ISNULL(M.CustomerId, 0) AS CustomerId,                
                ISNULL(M.CreatedBy, '') AS CreatedBy,          
                ISNULL(M.IsPost, 0) AS IsPost,

                ISNULL(FORMAT(M.ReceiveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS ReceiveDate
                FROM ProductReplaceReceive M
                WHERE 1 = 1

                 ";

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

                var lst = new List<ProductReplaceReceiveVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new ProductReplaceReceiveVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        ReceiveDate = row["ReceiveDate"].ToString()
                    });
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

        public async Task<ResultVM> ReplaceReceiveList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.Id, 0) AS ProductReplaceReceiveId,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,                
    ISNULL(M.CreatedBy, '') AS CreatedBy,          
    ISNULL(M.IsPost, 0) AS IsPost,

    ISNULL(FORMAT(M.ReceiveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS ReceiveDate
    FROM ProductReplaceReceive M
    WHERE 1 = 1

 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND M.Id IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);

                }

                var lst = new List<ProductReplaceReceiveVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new ProductReplaceReceiveVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        ProductReplaceReceiveId = Convert.ToInt32(row["ProductReplaceReceiveId"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        ReceiveDate = row["ReceiveDate"].ToString()

                    });
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

        public async Task<ResultVM> ReplaceReceiveDetailsList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
   ISNULL(D.Id, 0) AS Id,

   ISNULL(D.Id, 0) AS ProductReplaceReceiveDetailId,
   ISNULL(D.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,

   ISNULL(D.ProductId, 0) AS ProductId,
   --ISNULL(D.Quantity, 0.00) AS Quantity,
   (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) AS Quantity,
   ISNULL(P.Name,'') ProductName,
   ISNULL(P.BanglaName,'') BanglaName, 
   ISNULL(P.CtnSize,'') UOM, 
   ISNULL(P.Code,'') ProductCode
   FROM 
   ProductReplaceReceiveDetails D
   LEFT OUTER JOIN Products P ON D.ProductId = P.Id

   WHERE 1 = 1  AND (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) > 0 

";


                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND D.ProductReplaceReceiveId IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);

                }

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



        public async Task<ResultVM> GetProductReplaceReceiveDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<ProductReplaceReceiveDetailsVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM ProductReplaceReceiveDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
 
            WHERE h.ProductReplaceReceiveId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
             ISNULL(H.Id, 0) AS Id,
             ISNULL(H.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
             ISNULL(H.ProductId, 0) AS ProductId,
             ISNULL(H.Quantity, 0.00) AS Quantity,
             ISNULL(P.Name,'') ProductName,
             ISNULL(P.BanglaName,'') BanglaName, 
             ISNULL(P.CtnSize,'') UOM, 
             ISNULL(P.Code,'') ProductCode

            FROM ProductReplaceReceiveDetails H
            LEFT OUTER JOIN Products p ON H.ProductId = p.Id

            WHERE h.ProductReplaceReceiveId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");


                data = KendoGrid<ProductReplaceReceiveDetailsVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
