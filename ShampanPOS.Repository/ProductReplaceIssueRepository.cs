using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{
    public class ProductReplaceIssueRepository : CommonRepository
    {
        // 1. Insert Method
        public async Task<ResultVM> Insert(ProductReplaceIssueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO ProductReplaceIssue 
                (
                    Code, BranchId,IssueDate, CustomerId,IsPost, CreatedBy, CreatedOn, CreatedFrom
                )
                VALUES 
                (
                    @Code, @BranchId,@IssueDate, @CustomerId,@IsPost, @CreatedBy, @CreatedOn, @CreatedFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@IssueDate", vm.IssueDate);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "ERP");
        

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
        public async Task<ResultVM> Update(ProductReplaceIssueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE ProductReplaceIssue 
                

SET                  
IssueDate = @IssueDate,
CustomerId = @CustomerId,                
LastModifiedBy = @LastModifiedBy, 
LastModifiedOn = GETDATE()             
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id); 
                    cmd.Parameters.AddWithValue("@IssueDate", vm.IssueDate);
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

                    ISNULL(M.Id, 0) AS Id,
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(FORMAT(M.IssueDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS IssueDate,
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.CustomerId, 0) AS CustomerId,
                    ISNULL(M.IsPost, 0) AS IsPost,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
                    FROM ProductReplaceIssue M

                    WHERE 1 = 1

 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
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

                var model = new List<ProductReplaceIssueVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new ProductReplaceIssueVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        IssueDate = row["IssueDate"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString()
                    });
                }

                var detailsDataList = DetailsList(new[] { "M.ProductReplaceIssueId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {

                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<ProductReplaceIssueDetailsVM>>(json);
                    model.FirstOrDefault().ProductReplaceIssueDetails = details;

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
                FROM ProductReplaceIssues M
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
        public async Task<ResultVM> InsertDetails(ProductReplaceIssueDetailsVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                    INSERT INTO ProductReplaceIssueDetails
                    (ProductReplaceIssueId,ProductReplaceReceiveId,ProductReplaceReceiveDetailId, ProductId, Quantity)
                    VALUES 
                    (@ProductReplaceIssueId,@ProductReplaceReceiveId,@ProductReplaceReceiveDetailId, @ProductId, @Quantity);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductReplaceIssueId", details.ProductReplaceIssueId);
                    cmd.Parameters.AddWithValue("@ProductReplaceReceiveId", details.ProductReplaceReceiveId);
                    cmd.Parameters.AddWithValue("@ProductReplaceReceiveDetailId", details.ProductReplaceReceiveDetailId);
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


        public async Task<ResultVM> UpdateLineItem(ProductReplaceIssueDetailsVM detail, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = detail.Id.ToString(), DataVM = detail };

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

                string query = $@"
                UPDATE ProductReplaceReceiveDetails SET CompletedQty = (SELECT ISNULL(CompletedQty,0) + CAST(@Quantity AS DECIMAL(10, 2)) FROM ProductReplaceReceiveDetails WHERE Id = @Id )  
                WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", detail.ProductReplaceReceiveDetailId != null ? detail.ProductReplaceReceiveDetailId : 0);
                    cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);

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


        public async Task<ResultVM> GetLineItemCompletedQty(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
CASE WHEN SUM(D.Quantity) = SUM(D.CompletedQty) THEN 'True' ELSE 'False' END Status
FROM ProductReplaceReceiveDetails D
WHERE D.ProductReplaceReceiveId = @Id
GROUP BY D.ProductReplaceReceiveId
 ";


                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                        adapter.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
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

        public async Task<ResultVM> UpdateIsCompleted(ProductReplaceIssueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @" UPDATE ProductReplaceReceive SET IsCompleted = 1 WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.ProductReplaceReceiveId);

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



        // 7. DetailsList Method
        public  ResultVM DetailsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.ProductReplaceIssueId, 0) AS ProductReplaceIssueId,
    ISNULL(M.ProductReplaceReceiveId, 0) AS ProductReplaceReceiveId,
    ISNULL(M.ProductReplaceReceiveDetailId, 0) AS ProductReplaceReceiveDetailId,
    ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(P.Name,'') ProductName,
    ISNULL(P.CtnSize,'') UOM,
    ISNULL(M.Quantity, 0) AS Quantity,
    ISNULL(PRR.Code,'') ReceiveCode


    FROM ProductReplaceIssueDetails M 
 
    LEFT OUTER JOIN Products P ON M.ProductId = P.Id
    LEFT OUTER JOIN ProductReplaceReceive PRR ON M.ProductReplaceReceiveId = PRR.Id

    WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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

                string query = $" UPDATE ProductReplaceIssue SET IsPost = 1, PostedBy = @PostedBy , LastPostedFrom = @LastPostedFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    cmd.Parameters.AddWithValue("@PostedBy", vm.ModifyBy);
                    cmd.Parameters.AddWithValue("@LastPostedFrom", vm.ModifyFrom);

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


        public async Task<ResultVM> FromProductReplaceReceiveGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

	LEFT JOIN 
			(

				SELECT d.ProductReplaceReceiveId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
                FROM [dbo].[ProductReplaceReceiveDetails] d   
                GROUP BY d.ProductReplaceReceiveId

			) SD ON H.Id = SD.ProductReplaceReceiveId

WHERE H.IsPost = 1 AND SD.TotalCompletedQty < SD.TotalQuantity

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveVM>.FilterCondition(options.filter) + ")" : "") + @"

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
		,ISNULL(FORMAT(H.ReceiveDate,'yyyy-MM-dd HH:mm'),'1900-01-01') ReceiveDate
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

       	LEFT JOIN 
			(

				SELECT d.ProductReplaceReceiveId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
                FROM [dbo].[ProductReplaceReceiveDetails] d   
                GROUP BY d.ProductReplaceReceiveId

			) SD ON H.Id = SD.ProductReplaceReceiveId

        WHERE H.IsPost = 1 AND SD.TotalCompletedQty < SD.TotalQuantity


    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceReceiveVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceReceiveVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                var data = new GridEntity<ProductReplaceIssueVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM ProductReplaceIssue H
    WHERE 1 = 1
";

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
         ISNULL(H.Id, 0) AS Id
        ,ISNULL(H.Code, '') AS Code
        ,ISNULL(H.IssueDate, '') AS IssueDate
        ,ISNULL(H.BranchId, 0) AS BranchId
        ,ISNULL(H.CustomerId, 0) AS CustomerId
        ,ISNULL(C.Name, '') AS CustomerName
		,ISNULL(H.IsPost, 0) IsPost
		,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
        ,ISNULL(H.CreatedFrom,'') CreatedFrom
        ,ISNULL(H.CreatedBy,'') CreatedBy
		,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
		,ISNULL(H.LastModifiedBy,'') LastModifiedBy
		,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
		,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom
		,ISNULL(H.PostedBy, '') PostedBy
		,ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') PostedOn

        FROM ProductReplaceIssue H
		LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
		LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id

        WHERE 1 = 1

" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceIssueVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<ProductReplaceIssueDetailsVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT D.Id) AS totalcount
FROM ProductReplaceIssueDetails D
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
    ISNULL(D.ProductReplaceIssueId, 0) AS ProductReplaceIssueId,
    ISNULL(D.ProductId, 0) AS ProductId,
    ISNULL(P.Name, '') AS ProductName,  -- Assuming Product Name is from Products table
    ISNULL(D.CtnQuantity, 0) AS CtnQuantity,
    ISNULL(D.PcsQuantity, 0) AS PcsQuantity,
    ISNULL(D.Quantity, 0) AS Quantity,


FROM ProductReplaceIssueDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
WHERE 1 = 1
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueDetailsVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) AS a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<ProductReplaceIssueDetailsVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

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


        public async Task<ResultVM> UpdateProductReplaceReceiveIsComplete(int? id, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = null, DataVM = id };

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
                    IsCompleted = 1

                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

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



        public async Task<ResultVM> GetProductReplaceIssueDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<ProductReplaceIssueDetailsVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT D.Id) AS totalcount
            FROM 
            ProductReplaceIssueDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            WHERE D.ProductReplaceIssueId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
        
           ISNULL(D.Id, 0) AS Id,
           ISNULL(D.ProductReplaceIssueId, 0) AS ProductReplaceIssueId,
           ISNULL(D.ProductId, 0) AS ProductId,
           ISNULL(P.Name, '') AS ProductName,  
           ISNULL(D.Quantity, 0) AS Quantity,
           ISNULL(P.CtnSize,'') UOM

           FROM ProductReplaceIssueDetails D
           LEFT OUTER JOIN Products P ON D.ProductId = P.Id
           WHERE D.ProductReplaceIssueId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductReplaceIssueDetailsVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");

                data = KendoGrid<ProductReplaceIssueDetailsVM>.GetGridData_CMD(options, sqlQuery, "D.Id");

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
