using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using Newtonsoft.Json;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{

    public class SaleOrderRepository : CommonRepository
    {
        // Insert Method

        public async Task<ResultVM> Insert(SaleOrderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO SaleOrders 
                (
                    Code, BranchId, CompanyId, CustomerId, DeliveryAddress, OrderDate, 
                    DeliveryDate, Comments, 
                    TransactionType,CreatedBy, CreatedOn
                )
                VALUES 
                (
                    @Code, @BranchId, @CompanyId,@CustomerId, @DeliveryAddress, @OrderDate, 
                    @DeliveryDate, @Comments, 
                    @TransactionType,@CreatedBy, @CreatedOn
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    //cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", vm.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderDate", vm.OrderDate);
                    cmd.Parameters.AddWithValue("@DeliveryDate", vm.DeliveryDate);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? "");
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int)
                                  .Value = (object?)vm.CompanyId ?? DBNull.Value;
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

        // Update Method

        public async Task<ResultVM> Update(SaleOrderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE SaleOrders 
                SET 
                    BranchId = @BranchId,  CompanyId = @CompanyId, CustomerId = @CustomerId, DeliveryAddress = @DeliveryAddress, OrderDate = @OrderDate, 
                    DeliveryDate = @DeliveryDate, Comments = @Comments, TransactionType = @TransactionType, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = GETDATE()

                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? 0);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", vm.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderDate", vm.OrderDate);
                    cmd.Parameters.AddWithValue("@DeliveryDate", vm.DeliveryDate);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                   
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

        // Delete Method
        public async Task<ResultVM> Delete(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            throw new NotImplementedException();

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
        ISNULL(M.Id, 0) AS SaleOrderId,
        ISNULL(M.Code, '') AS Code,
        ISNULL(M.BranchId, 0) AS BranchId,
	    ISNULL(M.CompanyId, 0) AS CompanyId,
        ISNULL(M.CustomerId, 0) AS CustomerId,
        ISNULL(C.Name, '') AS CustomerName,
        ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
        ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
        ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,

        ISNULL(M.Comments, '') AS Comments,
        ISNULL(M.TransactionType, '') AS TransactionType,
        ISNULL(M.IsPost, 0) AS IsPost,
        ISNULL(M.PostedBy, '') AS PostedBy,
        ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
        ISNULL(M.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
        ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
        ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
	ISNULL(Br.Name,'') BranchName,
    ISNULL(CP.CompanyName,'') CompanyName
    FROM SaleOrders M
    LEFT JOIN Customers C ON M.CustomerId = C.Id
    LEFT OUTER JOIN BranchProfiles Br ON M.BranchId = Br.Id
	LEFT OUTER JOIN CompanyProfiles CP ON M.CompanyId = CP.Id
    WHERE 1 = 1
";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
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

                var model = new List<SaleOrderVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleOrderVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        SaleOrderId = Convert.ToInt32(row["SaleOrderId"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CompanyId = Convert.ToInt32(row["CompanyId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CustomerName = row.Field<string>("CustomerName"),
                        DeliveryAddress = row.Field<string>("DeliveryAddress") ?? string.Empty,
                        OrderDate = row.Field<string>("OrderDate"),
                        DeliveryDate = row.Field<string>("DeliveryDate"),

                        Comments = row.Field<string>("Comments") ?? string.Empty,
                        TransactionType = row.Field<string>("TransactionType") ?? string.Empty,
                        IsPost = row.Field<bool>("IsPost"),
                        PostBy = row.Field<string>("PostedBy"),
                        PosteOn = row.Field<string?>("PostedOn"),
                        CreatedBy = row.Field<string>("CreatedBy") ?? string.Empty,
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy") ?? string.Empty,
                        LastModifiedOn = row.Field<string>("LastModifiedOn") ?? string.Empty,
                    });
                }

                var detailsDataList = DetailsList(new[] { "D.SaleOrderId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleOrderDetailVM>>(json);

                    model.FirstOrDefault().saleOrderDetailsList = details;
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
                                ISNULL(M.Id, 0) AS Id,
                                ISNULL(M.Code, '') AS Code,
                                ISNULL(M.BranchId, 0) AS BranchId,
                                ISNULL(M.CustomerId, 0) AS CustomerId,
                                ISNULL(C.Name, 0) AS CustomerName,
                                ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
                                ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
                                ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,

                                ISNULL(M.Comments, '') AS Comments,
                                ISNULL(M.TransactionType, '') AS TransactionType,
                                ISNULL(M.IsPost, 0) AS IsPost,
                                ISNULL(M.PostedBy, '') AS PostedBy,
                                ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
                                ISNULL(M.CreatedBy, '') AS CreatedBy,
                                ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
                                ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
                                ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
                   
                            FROM SaleOrders M
							LEFT OUTER JOIN Customers C on M.CustomerId = C.Id

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
                    SELECT Id, Code
                    FROM SaleOrders
                    WHERE 1 = 1
                    ORDER BY Id";

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

        public async Task<ResultVM> InsertDetails(SaleOrderDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                    INSERT INTO SaleOrderDetails
                    (SaleOrderId, Line, ProductId, Quantity, UnitRate, SubTotal,SD,CompletedQty,RemainQty, SDAmount, VATRate, VATAmount, LineTotal,CompanyId)
                    VALUES 
                    (@SaleOrderId, @Line, @ProductId, @Quantity, @UnitRate,@CompletedQty,@RemainQty, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal,@CompanyId);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleOrderId", details.SaleOrderId);
                    cmd.Parameters.AddWithValue("@Line", details.Line );
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId ?? 0);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate ?? 0);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal ?? 0);
                    cmd.Parameters.AddWithValue("@SD", details.SD ?? 0);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount ?? 0);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount ?? 0);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal ?? 0);
                    cmd.Parameters.AddWithValue("@Comments", details.Comments ?? "-");
                    cmd.Parameters.AddWithValue("@CompletedQty", details.CompletedQty);

                    cmd.Parameters.AddWithValue("@RemainQty", details.RemainQty);
                    cmd.Parameters.AddWithValue("@CompanyId", details.CompanyId ?? 0);



                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details data inserted successfully.";
                    result.DetailId = newId.ToString();
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
                ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
                ISNULL(D.Line, 0) AS Line,
                ISNULL(D.ProductId, 0) AS ProductId,
                ISNULL(D.CompanyId, 0) AS CompanyId,
                ISNULL(D.Quantity, 0.00) AS Quantity,
                ISNULL(FORMAT(D.UnitRate, 'N2'), '0.00') AS UnitRate,
                ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
                ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
                ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
                ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
                ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
                ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
                ISNULL(FORMAT(D.CompletedQty, 'N2'), '0.00') AS CompletedQty,
                ISNULL(FORMAT(D.RemainQty, 'N2'), '0.00') AS RemainQty,


                ISNULL(P.Name,'') ProductName,
                ISNULL(P.BanglaName,'') BanglaName, 
                ISNULL(P.Code,'') ProductCode, 
                ISNULL(P.HSCodeNo,'') HSCodeNo,
                ISNULL(P.ProductGroupId,0) ProductGroupId,
                ISNULL(PG.Name,'') ProductGroupName,
                ISNULL(CP.CompanyName,'') CompanyName



                FROM 
                SaleOrderDetails D
                LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

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

                string query = $" UPDATE SaleOrders SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                //query += $" UPDATE SaleOrderDetails SET IsPost = 1 WHERE SaleOrderId IN ({inClause}) ";

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

                var data = new GridEntity<SaleOrderVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
				FROM SaleOrders H 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
				LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
				LEFT OUTER JOIN BranchProfiles BR on h.BranchId = BR.Id
				LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
				WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                 ISNULL(H.Id,0)	Id
				,ISNULL(H.Code,'')	Code
				,ISNULL(H.DeliveryAddress,'')	DeliveryAddress				
				,ISNULL(FORMAT(H.OrderDate,'yyyy-MM-dd HH:mm'),'1900-01-01') OrderDate
				,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd HH:mm'),'1900-01-01') DeliveryDate	

				,ISNULL(H.Comments,'')	Comments	
				,ISNULL(H.TransactionType,'')	TransactionType				
				
				,ISNULL(H.CreatedBy,'') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(H.CreatedFrom,'') CreatedFrom
				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom		
				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
		        ,ISNULL(H.BranchId,0) BranchId
		        ,ISNULL(H.CustomerId,0) CustomerId
		        ,ISNULL(C.Name,'') CustomerName
		        ,ISNULL(BR.Name,'-') BranchName
				,ISNULL(CP.CompanyName,'') CompanyName
		        ,ISNULL(H.IsPost,0) IsPost
		        ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Yes' ELSE 'No' END AS PostStatus

				FROM SaleOrders H 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
				LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
				LEFT OUTER JOIN BranchProfiles BR on h.BranchId = BR.Id
				LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
				WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                //data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<SaleOrderVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<SaleOrderDetailVM>();

                // Define your SQL query string


                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM SaleOrders H
    LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
    LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
    LEFT OUTER JOIN BranchProfiles BR ON H.BranchId = BR.Id
    LEFT OUTER JOIN SaleOrderDetails D ON H.Id = D.SaleOrderId
    LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id

    WHERE 1 = 1
-- Add the filter condition
" + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
-- Data query with pagination and sorting
SELECT * 
FROM (
    SELECT 
    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
  ISNULL(H.Id, 0) AS Id,
    ISNULL(H.Code, '') AS Code,
    ISNULL(H.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(H.DeliveryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS DeliveryDate,
    ISNULL(H.Comments, '') AS Comments,
    ISNULL(H.TransactionType, '') AS TransactionType,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
    ISNULL(H.BranchId, 0) AS BranchId,
    ISNULL(H.CustomerId, 0) AS CustomerId,
    ISNULL(C.Name, '') AS CustomerName,
    ISNULL(BR.Name, '-') AS BranchName,
    ISNULL(PD.Name, '') AS ProductName,


    -- Detail Information
    ISNULL(D.Quantity, 0) AS Quantity,
    ISNULL(D.UnitRate, 0) AS UnitRate,
    ISNULL(D.SubTotal, 0) AS SubTotal,
    ISNULL(D.SD, 0) AS SD,
    ISNULL(D.SDAmount, 0) AS SDAmount,
    ISNULL(D.VATRate, 0) AS VATRate,
    ISNULL(D.VATAmount, 0) AS VATAmount,
    ISNULL(D.LineTotal, 0) AS LineTotal
  
    
  

    FROM SaleOrders H
    LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
    LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
    LEFT OUTER JOIN BranchProfiles BR ON H.BranchId = BR.Id
    LEFT OUTER JOIN SaleOrderDetails D ON H.Id = D.SaleOrderId
    LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id

    WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) AS a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take);";

                data = KendoGrid<SaleOrderDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> UpdateGrandTotal(SaleOrderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE SaleOrders
                SET 
                    GrandTotalAmount = (SELECT SUM(SubTotal) FROM SaleOrderDetails WHERE SaleOrderId = @Id)-(RegularDiscountAmount+SpecialDiscountAmount),
                    GrandTotalSDAmount = (SELECT SUM(SDAmount) FROM SaleOrderDetails WHERE SaleOrderId = @Id),
                    GrandTotalVATAmount = (SELECT SUM(VATAmount) FROM SaleOrderDetails WHERE SaleOrderId = @Id)
                WHERE Id = @Id ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);

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

        //public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //        SELECT 

        //         0 AS Id
        //        ,'' AS	Code
        //        ,ISNULL(H.BranchId,'')	BranchId
        //        ,ISNULL(H.CustomerId,'')	CustomerId
        //        ,ISNULL(H.SalePersonId,'')	SalePersonId
        //        ,0 AS	DeliveryPersonId
        //        ,0 AS	DriverPersonId
        //        ,ISNULL(H.RouteId,'')	RouteId
        //        ,'' AS	VehicleNo
        //        ,'' AS	VehicleType
        //        ,ISNULL(H.DeliveryAddress,'')	DeliveryAddress				
        //        ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd HH:mm'),'1900-01-01') InvoiceDateTime
        //        ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd HH:mm'),'1900-01-01') DeliveryDate	
        //        ,ISNULL(H.GrandTotalAmount,0) GrandTotalAmount
        //        ,ISNULL(H.GrandTotalSDAmount,0) GrandTotalSDAmount
        //        ,ISNULL(H.GrandTotalVATAmount,0) GrandTotalVATAmount
        //        ,ISNULL(H.RegularDiscountRate, 0) AS RegularDiscountRate
        //        ,ISNULL(H.RegularDiscountAmount, 0) AS RegularDiscountAmount
        //        ,ISNULL(H.SpecialDiscountRate, 0) AS SpecialDiscountRate
        //        ,ISNULL(H.SpecialDiscountAmount, 0) AS SpecialDiscountAmount
        //        ,ISNULL(H.Comments,'')	Comments	
        //        ,'SaleDelivery' AS	TransactionType				
        //        ,ISNULL(H.IsCompleted,0)	IsCompleted				
        //        ,ISNULL(H.CurrencyId,0)	CurrencyId				
        //        ,ISNULL(H.CurrencyRateFromBDT,0) CurrencyRateFromBDT
        //        ,0 AS IsPost
        //        ,'' AS PostedBy
        //        ,'1900-01-01' AS PostedOn
        //        ,ISNULL(H.CreatedBy,'') CreatedBy
        //        ,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
        //        ,ISNULL(H.LastModifiedBy,'') LastModifiedBy
        //        ,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
        //        ,ISNULL(H.CreatedFrom,'') CreatedFrom
        //        ,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom
        //        ,ISNULL(H.InvoiceDiscountRate,0) InvoiceDiscountRate
        //        ,ISNULL(H.InvoiceDiscountAmount,0) InvoiceDiscountAmount
		

        //        FROM SaleOrders H 
        //        LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
        //        LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
        //        LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
        //        LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
        //        LEFT OUTER JOIN Currencies CR ON H.CurrencyId = CR.Id
        //        LEFT OUTER JOIN BranchProfiles BR on h.BranchId = BR.Id
        //        WHERE  1 = 1
        //         ";

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            query += " AND H.Id = @Id ";
        //        }

        //        // Apply additional conditions
        //        query = ApplyConditions(query, conditionalFields, conditionalValue, false);

        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
        //        }

        //        objComm.Fill(dataTable);

        //        var lst = new List<SaleDeliveryVM>();

        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            lst.Add(new SaleDeliveryVM
        //            {
        //                Id = Convert.ToInt32(row["Id"]),
        //                Code = row["Code"].ToString(),
        //                BranchId = row.IsNull("BranchId") ? 0 : Convert.ToInt32(row["BranchId"]),
        //                CustomerId = row.IsNull("CustomerId") ? 0 : Convert.ToInt32(row["CustomerId"]),
        //                SalePersonId = row.IsNull("SalePersonId") ? 0 : Convert.ToInt32(row["SalePersonId"]),
        //                DeliveryPersonId = 0, // Fixed value as per query
        //                DriverPersonId = 0, // Fixed value as per query
        //                RouteId = row.IsNull("RouteId") ? 0 : Convert.ToInt32(row["RouteId"]),
        //                VehicleNo = string.Empty, // Fixed value as per query
        //                VehicleType = string.Empty, // Fixed value as per query
        //                DeliveryAddress = row.IsNull("DeliveryAddress") ? string.Empty : row["DeliveryAddress"].ToString(),
        //                InvoiceDateTime = row.IsNull("InvoiceDateTime") ? "1900-01-01" : row["InvoiceDateTime"].ToString(),
        //                DeliveryDate = row.IsNull("DeliveryDate") ? "1900-01-01" : row["DeliveryDate"].ToString(),
        //                GrandTotalAmount = row.IsNull("GrandTotalAmount") ? 0 : Convert.ToDecimal(row["GrandTotalAmount"]),
        //                GrandTotalSDAmount = row.IsNull("GrandTotalSDAmount") ? 0 : Convert.ToDecimal(row["GrandTotalSDAmount"]),
        //                GrandTotalVATAmount = row.IsNull("GrandTotalVATAmount") ? 0 : Convert.ToDecimal(row["GrandTotalVATAmount"]),
        //                RegularDiscountRate = row.Field<decimal>("RegularDiscountRate"),
        //                RegularDiscountAmount = row.Field<decimal>("RegularDiscountAmount"),

        //                SpecialDiscountRate = row.Field<decimal>("SpecialDiscountRate"),
        //                SpecialDiscountAmount = row.Field<decimal>("SpecialDiscountAmount"),
        //                Comments = row.IsNull("Comments") ? string.Empty : row["Comments"].ToString(),
        //                TransactionType = row.IsNull("TransactionType") ? string.Empty : row["TransactionType"].ToString(),
        //                IsCompleted = row.IsNull("IsCompleted") ? false : Convert.ToBoolean(row["IsCompleted"]),
        //                CurrencyId = row.IsNull("CurrencyId") ? 0 : Convert.ToInt32(row["CurrencyId"]),
        //                CurrencyRateFromBDT = row.IsNull("CurrencyRateFromBDT") ? 0 : Convert.ToDecimal(row["CurrencyRateFromBDT"]),
        //                IsPost = false, // Fixed value as per query
        //                PostedBy = string.Empty, // Fixed value as per query
        //                PostedOn = "1900-01-01", // Fixed value as per query
        //                CreatedBy = row.IsNull("CreatedBy") ? string.Empty : row["CreatedBy"].ToString(),
        //                CreatedOn = row.IsNull("CreatedOn") ? "1900-01-01" : row["CreatedOn"].ToString(),
        //                LastModifiedBy = row.IsNull("LastModifiedBy") ? string.Empty : row["LastModifiedBy"].ToString(),
        //                LastModifiedOn = row.IsNull("LastModifiedOn") ? "1900-01-01" : row["LastModifiedOn"].ToString(),
        //                CreatedFrom = row.IsNull("CreatedFrom") ? string.Empty : row["CreatedFrom"].ToString(),
        //                LastUpdateFrom = row.IsNull("LastUpdateFrom") ? string.Empty : row["LastUpdateFrom"].ToString()
        //            });
        //        }


        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = lst;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        //public async Task<ResultVM> SaleOrderDetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"
        //         SELECT 
        //        0 AS Id,
        //         0 AS SaleDeliveryId, 
        //         ISNULL(D.Id, 0) AS SaleOrderDetailId,
        //         ISNULL(D.Id, 0) AS FromSaleOrderDetailId,
        //         ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
        //         ISNULL(D.BranchId, 0) AS BranchId,
        //         ISNULL(D.Line, 0) AS Line,
        //         ISNULL(D.ProductId, 0) AS ProductId,

        //         (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) AS Quantity,
        //         --ISNULL(D.Quantity, 0.00) AS Quantity,
        //         ISNULL(D.CtnQuantity, 0.00) AS CtnQuantity,
        //        ISNULL(D.PcsQuantity, 0.00) AS PcsQuantity,
        //         ISNULL(FORMAT(D.UnitRate, 'N2'), '0.00') AS UnitRate,
        //         ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
        //         ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
        //         ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
        //         ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
        //         ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
        //         ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
        //         ISNULL(P.UOMId,0) UOMId,
        //         ISNULL(P.UOMId,0) UOMFromId,
        //         ISNULL(FORMAT(D.UOMConversion, 'N2'), '0.00') AS UOMConversion,
        //        0 AS IsPost ,
        //        ISNULL(D.Comments, '') AS Comments,
        //        ISNULL(P.Name,'') ProductName,
        //        ISNULL(P.BanglaName,'') BanglaName, 
        //        ISNULL(P.Code,'') ProductCode, 
        //        ISNULL(P.HSCodeNo,'') HSCodeNo,
        //        ISNULL(P.ProductGroupId,0) ProductGroupId,
        //        ISNULL(PG.Name,'') ProductGroupName,
        //        ISNULL(UOM.Name,'') UOMName,
        //        ISNULL(UOM.Name,'') UOMFromName,
        //        ISNULL(D.CustomerId,0) CustomerId,
        //        ISNULL(D.FreeProductId,0) FreeProductId,
        //        ISNULL(FP.Name,'') FreeProductName,
        //        ISNULL(D.FreeQuantity,0) FreeQuantity,
        //        ISNULL(D.DiscountRate,0) DiscountRate,
        //        ISNULL(D.DiscountAmount,0) DiscountAmount,
        //        ISNULL(D.SubTotalAfterDiscount,0) SubTotalAfterDiscount,
        //        ISNULL(D.LineDiscountRate,0) LineDiscountRate,
        //        ISNULL(D.LineDiscountAmount,0) LineDiscountAmount,
        //        ISNULL(D.LineTotalAfterDiscount,0) LineTotalAfterDiscount,
        //        ISNULL(D.CampaignId,0) CampaignId,
        //        ISNULL(D.CampaignDetailsId,0) CampaignDetailsId,
        //        ISNULL(D.CampaignTypeId,0) CampaignTypeId,
        //        ISNULL(D.PcsQuantity,0) PcsQuantity,
        //        ISNULL(D.CtnQuantity,0) CtnQuantity

        //         FROM 
        //         SaleOrderDetails D
        //         LEFT OUTER JOIN Products P ON D.ProductId = P.Id
        //         LEFT OUTER JOIN Products FP ON D.FreeProductId = FP.Id
        //         LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //         LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        //         WHERE 1 = 1
        //        ";

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            query += " AND D.Id = @Id ";
        //        }

        //        // Apply additional conditions
        //        query = ApplyConditions(query, conditionalFields, conditionalValue, false);

        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
        //        }

        //        objComm.Fill(dataTable);

        //        result.Status = "Success";
        //        result.Message = "Details Data retrieved successfully.";
        //        result.DataVM = dataTable;

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        public async Task<ResultVM> UpdateSaleOrderIsComplete(int? Id, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = Id?.ToString() };

            try
            {
                if (Id == null)
                {
                    result.Message = "Sale Order Id cannot be null.";
                    return result;
                }

                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();  // Use async for opening connection
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
        UPDATE SaleOrders 
        SET 
            IsCompleted = 1  -- Use 1 for true, assuming it's a bit column in SQL Server
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", Id.Value);  // Safely use Value since Id is non-null here.

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();  // Use async for non-blocking operations
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
                    await transaction.CommitAsync();  // Use async commit for non-blocking operations
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    await transaction.RollbackAsync();  // Use async rollback
                }
                result.Status = "Fail";
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    await conn.CloseAsync();  // Use async close for non-blocking
                }
            }
        }

        public async Task<ResultVM> GetSaleOrderDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleOrderDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM SaleOrderDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
            WHERE h.SaleOrderId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.ProductId, 0) AS ProductId,
            ISNULL(H.SaleOrderId, 0) AS SaleOrderId,
            ISNULL(P.Name, '') AS ProductName,
            ISNULL(H.Line, '') AS Line,
            ISNULL(H.Quantity, 0) AS Quantity,
            ISNULL(H.UnitRate, 0) AS UnitRate,
            ISNULL(H.SubTotal, 0) AS SubTotal,
            ISNULL(H.SD, 0) AS SD,
            ISNULL(H.SDAmount, 0) AS SDAmount,
            ISNULL(H.VATRate, 0) AS VATRate,
            ISNULL(H.VATAmount, 0) AS VATAmount,
            ISNULL(H.LineTotal, 0) AS LineTotal

            FROM SaleOrderDetails h
            LEFT OUTER JOIN Products p ON h.ProductId = p.Id
            WHERE h.SaleOrderId = @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";
                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<SaleOrderDetailVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

//        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

//                var data = new GridEntity<SaleOrderVM>();

//                // Define your SQL query string
//                string sqlQuery = @"
//    -- Count query
//    SELECT COUNT(DISTINCT H.Id) AS totalcount
//					FROM SaleOrders H 
//					LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
//					LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
//					LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
//					LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
//					LEFT OUTER JOIN Currencies CR ON H.CurrencyId = CR.Id
//                    LEFT OUTER JOIN BranchProfiles BR on h.BranchId = BR.Id

//					WHERE 1 = 1
//    -- Add the filter condition
//    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

//    -- Data query with pagination and sorting
//    SELECT * 
//    FROM (
//        SELECT 
//        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
//                ,ISNULL(H.Id,0)	Id
//				,ISNULL(H.Code,'')	Code
//				,ISNULL(H.DeliveryAddress,'')	DeliveryAddress				
//				,ISNULL(FORMAT(H.OrderDate,'yyyy-MM-dd HH:mm'),'1900-01-01') OrderDate
//				,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd HH:mm'),'1900-01-01') DeliveryDate	
//				,ISNULL(H.GrandTotalAmount,0) GrandTotalAmount
//				,ISNULL(H.GrandTotalSDAmount,0) GrandTotalSDAmount
//				,ISNULL(H.GrandTotalVATAmount,0) GrandTotalVATAmount
//				,ISNULL(H.Comments,'')	Comments	
//				,ISNULL(H.TransactionType,'')	TransactionType				
//				,ISNULL(H.CurrencyRateFromBDT,0) CurrencyRateFromBDT
				
//				,ISNULL(H.CreatedBy,'') CreatedBy
//				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
//				,ISNULL(H.CreatedFrom,'') CreatedFrom
//				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom		
//				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
//				,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
//		        ,ISNULL(H.BranchId,0) BranchId
//		        ,ISNULL(H.CustomerId,0) CustomerId
//		        ,ISNULL(C.Name,'') CustomerName
//		        ,ISNULL(H.SalePersonId,0) SalePersonId
//		        ,ISNULL(SP.Name,0) SalePersonName
//		        ,ISNULL(H.RouteId,0) RouteId
//		        ,ISNULL(R.Name,'') RouteName
//		        ,ISNULL(H.CurrencyId,0) CurrencyId
//		        ,ISNULL(CR.Name,0) CurrencyName
//		        ,ISNULL(BR.Name,'-') BranchName
//		        ,ISNULL(H.IsCompleted,0) IsCompleted
//		        ,CASE WHEN ISNULL(H.IsCompleted, 0) = 1 THEN 'Yes' ELSE 'No' END AS Status

//				FROM SaleOrders H 
//				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
//				LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
//				LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
//				LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
//				LEFT OUTER JOIN Currencies CR ON H.CurrencyId = CR.Id
//				LEFT OUTER JOIN BranchProfiles BR on h.BranchId = BR.Id
				
//				WHERE 1 = 1

//    -- Add the filter condition
//    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

//    ) AS a
//    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
//";



//                data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = data;

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

//        public async Task<ResultVM> ProductWiseSaleOrder(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//                           SELECT     
//                                    -- Details Data
//                                    ISNULL(D.ProductId, 0) AS ProductId,
//                                    ISNULL(PG.Name, '') AS ProductGroupName,
//                                    ISNULL(P.Code, '') AS ProductCode,
//                                    ISNULL(P.Name, '') AS ProductName,
//                                    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
//                                    ISNULL(uom.Name, '') AS UOMName,
//                                    ISNULL(SUM(D.Quantity), 0) AS Quantity

//                                    FROM SaleOrders M
//                                    LEFT OUTER JOIN SaleOrderDetails D ON ISNULL(M.Id,0) = D.SaleOrderId
//                                    LEFT OUTER JOIN Products P ON D.ProductId = P.Id
//                                    LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//                                    LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
//                                    LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
//		                                WHERE 1 = 1 

//";

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    query += " AND M.Id = @Id ";
//                }
//                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
//                {
//                    query += " AND CAST(M.DeliveryDate AS DATE) BETWEEN @FromDate AND @ToDate ";
//                }

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValue, false);
//                query += @" GROUP BY    
//                                    D.ProductId,
//                                    P.HSCodeNo,
//                                    P.Code,
//                                    P.Name,
//                                    PG.Name,
//                                    uom.Name ";

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
//                }
//                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
//                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
//                }

//                objComm.Fill(dataTable);

//                var lst = new List<SaleOrderReportVM>();
//                int serialNumber = 1;
//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new SaleOrderReportVM
//                    {
//                        SL = serialNumber,
//                        ProductId = Convert.ToInt32(row["ProductId"]),
//                        ProductGroupName = row["ProductGroupName"].ToString(),
//                        ProductCode = row["ProductCode"].ToString(),
//                        ProductName = row["ProductName"].ToString(),
//                        HSCodeNo = row["HSCodeNo"].ToString(),
//                        UOMName = row["UOMName"].ToString(),
//                        Quantity = Convert.ToInt32(row["Quantity"])
//                    });
//                    serialNumber++;
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
ISNULL(M.RouteId, 0) AS RouteId,
ISNULL(R.Name, '') + ', ' + ISNULL(R.Address, '') AS RouteAddress,
ISNULL(M.Code, '') AS Code,
ISNULL(M.BranchId, 0) AS BranchId,
ISNULL(TRIM(B.Name), '') AS BranchName,
ISNULL(TRIM(B.BanglaName), '') AS BranchBanglaName,
ISNULL(TRIM(B.Code), '') AS BranchCode,
ISNULL(TRIM(B.Address), '') AS BranchAddress,
ISNULL(TRIM(B.TelephoneNo), '') AS TelephoneNo,
ISNULL(M.CustomerId, 0) AS CustomerId,
ISNULL(C.Name, '') AS CustomerName,
ISNULL(C.Address, '') AS CustomerAddress,
ISNULL(M.SalePersonId, 0) AS SalePersonId,
ISNULL(SP.Name, '') AS SalePersonName,
ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
ISNULL(M.Comments, '') AS Comments,
ISNULL(M.TransactionType, '') AS TransactionType,
ISNULL(M.IsCompleted, 0) AS IsCompleted,
ISNULL(M.CurrencyId, 0) AS CurrencyId,
ISNULL(Cur.Name, '') AS CurrencyName,
ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
ISNULL(D.UOMConversion,0) AS UOMConversion,
ISNULL(TRIM(D.Comments), '') AS DetailComments,
ISNULL(TRIM(P.Name),'') ProductName,
ISNULL(TRIM(P.BanglaName),'') BanglaName, 
ISNULL(TRIM(P.Code),'') ProductCode, 
ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,

ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0) AS Quantity,
ISNULL(D.PcsQuantity, 0) AS PcsQuantity,
ISNULL(D.CtnQuantity, 0) AS CtnQuantity,
ISNULL(D.CtnQuantity, 0) AS CtnQuantity,
ISNULL(D.PcsQuantity, 0) AS PcsQuantity,
ISNULL(D.UnitRate, 0) AS UnitPrice,
ISNULL(D.SubTotal,0) AS SubTotal,
ISNULL(D.SD,0) AS SD,
ISNULL(D.SDAmount,0) AS SDAmount,
ISNULL(D.VATRate,0) AS VATRate,
ISNULL(D.VATAmount,0) AS VATAmount,
ISNULL(D.LineTotal,0) AS LineTotal


 
FROM SaleOrderDetails D 
LEFT OUTER JOIN SaleOrders M ON ISNULL(M.Id,0) = D.SaleOrderId
LEFT OUTER JOIN Routes R on ISNULL(M.RouteId,0) = R.Id
LEFT OUTER JOIN Customers C on ISNULL(M.CustomerId,0) = C.Id
LEFT OUTER JOIN SalesPersons SP on M.SalePersonId = SP.Id
LEFT OUTER JOIN Currencies Cur ON ISNULL(M.CurrencyId,0) = Cur.Id
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id

LEFT OUTER JOIN BranchProfiles B ON M.BranchId = B.Id

WHERE  1 = 1 ";

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

        //public async Task<ResultVM> InvoiceUpdate(SaleOrderVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = vm.Id.ToString(), DataVM = vm };

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
        //        UPDATE SaleOrders 
        //        SET 
        //            InvoiceDiscountRate = @InvoiceDiscountRate,InvoiceDiscountAmount = @InvoiceDiscountAmount
        //        WHERE Id = @Id";

        //        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //        {
        //            cmd.Parameters.AddWithValue("@Id", vm.Id);
        //            cmd.Parameters.AddWithValue("@InvoiceDiscountRate", vm.InvoiceDiscountRate ?? (object)DBNull.Value);
        //            cmd.Parameters.AddWithValue("@InvoiceDiscountAmount", vm.InvoiceDiscountAmount ?? (object)DBNull.Value);

        //            int rowsAffected = cmd.ExecuteNonQuery();

        //            if (rowsAffected > 0)
        //            {
        //                result.Status = "Success";
        //                result.Message = "Data updated successfully.";
        //            }
        //            else
        //            {
        //                result.Message = "No rows were updated.";
        //                throw new Exception("No rows were updated.");
        //            }
        //        }

        //        if (isNewConnection)
        //        {
        //            transaction.Commit();
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //        {
        //            transaction.Rollback();
        //        }
        //        result.Status = "Fail";
        //        result.ExMessage = ex.Message;
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
        public async Task<ResultVM> GetOrderNoWiseGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleOrderVM>();

                // Updated SQL Query
                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT d.Id) AS totalcount
FROM SaleOrderDetails d
LEFT JOIN SaleOrders h ON d.SaleOrderId = h.Id
LEFT JOIN BranchProfiles Br ON h.BranchId = Br.Id
LEFT JOIN SalesPersons SP ON h.SalePersonId = SP.Id
LEFT JOIN Customers C ON h.CustomerId = C.Id
LEFT JOIN Routes r ON h.RouteId = r.Id
LEFT JOIN Products P ON d.ProductId = P.Id
WHERE h.IsCompleted = 1 
";

                // Apply Filter Conditions
                if (options.filter.Filters.Count > 0)
                {
                    sqlQuery += " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")";
                }

                // Apply Additional Conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                // Data Query with Pagination & Sorting
                sqlQuery += @"
;WITH OrderedData AS (
    SELECT 
        ROW_NUMBER() OVER (ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "h.BranchId ASC") + @") AS rowindex,
        h.BranchId, 
        Br.Name AS BranchName,
        r.Name AS RouteName,
        ISNULL(FORMAT(h.OrderDate,'yyyy-MM-dd HH:mm'),'1900-01-01') OrderDate,
        P.Name AS ProductName,
        SUM(d.Quantity) AS Quantity
    FROM SaleOrderDetails d
    LEFT JOIN SaleOrders h ON d.SaleOrderId = h.Id
    LEFT JOIN BranchProfiles Br ON h.BranchId = Br.Id
    LEFT JOIN SalesPersons SP ON h.SalePersonId = SP.Id
    LEFT JOIN Customers C ON h.CustomerId = C.Id
    LEFT JOIN Routes r ON h.RouteId = r.Id
    LEFT JOIN Products P ON d.ProductId = P.Id
    WHERE h.IsCompleted = 1 
    ";

                // Apply Filter Conditions
                if (options.filter.Filters.Count > 0)
                {
                    sqlQuery += " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")";
                }

                // Apply Additional Conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    GROUP BY h.BranchId, Br.Name, r.Name,FORMAT(h.OrderDate, 'yyyy-MM-dd HH:mm'), P.Name
)
SELECT * FROM OrderedData
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take);";

                // Retrieve Data
                data = KendoGrid<SaleOrderVM>.GetTransactionalGridData_CMD(options, sqlQuery, "h.BranchId", conditionalFields, conditionalValues);

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


        public async Task<ResultVM> SaleOrderDetailsList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(D.Id, 0) AS PurchaseOrderDetailId,
                ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
                ISNULL(D.Line, 0) AS Line,
                ISNULL(D.ProductId, 0) AS ProductId,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) AS Quantity,
                ISNULL(D.UnitRate,0.00) AS UnitRate,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00))*ISNULL(D.UnitRate,0.00) AS SubTotal,
                ISNULL(D.SD,0.00) AS SD,
                ISNULL(D.SDAmount,0.00) AS SDAmount,
                ISNULL(D.VATRate,0.00) AS VATRate,
                ISNULL(D.VATAmount,0.00) AS VATAmount,
                (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00))*ISNULL(D.UnitRate,0.00) AS LineTotal,
                ISNULL(P.Name,'') ProductName,
                ISNULL(P.BanglaName,'') BanglaName, 
                ISNULL(P.Code,'') ProductCode, 
                ISNULL(P.HSCodeNo,'') HSCodeNo,
                ISNULL(P.ProductGroupId,0) ProductGroupId,
                ISNULL(PG.Name,'') ProductGroupName

                FROM 
                SaleOrderDetails D
                LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                WHERE 1 = 1 AND (ISNULL(D.Quantity, 0.00)-ISNULL(D.CompletedQty, 0.00)) > 0 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND D.SaleOrderId IN ({inClause}) ";
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

        public async Task<ResultVM> SaleOrderList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.Id, 0) AS SaleOrderId,
	ISNULL(M.Code, '') AS SaleOrderCode,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.Code, '') AS PurchaseOrderCode,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(S.Name, 0) AS CustomerName,
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
	ISNULL(M.IsPost, 0) AS IsPost,
	ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.PeriodId,0) AS PeriodId,   
    
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn
    
FROM 
    SaleOrders M
    LEFT OUTER JOIN Customers s on M.CustomerId = s.Id
WHERE  1 = 1
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

                var lst = new List<SaleVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        SaleOrderId = Convert.ToInt32(row["SaleOrderId"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CustomerName = Convert.ToString(row["CustomerName"]),
                        InvoiceDateTime = row["DeliveryDate"].ToString(),
                        Comments = row["Comments"].ToString(),
                        Code = row["Code"].ToString(),
                        SaleOrderCode = row["SaleOrderCode"].ToString(),
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = Convert.ToBoolean(row["IsPost"]),
                        PostedBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"].ToString(),
                        PeriodId = Convert.ToString(row["PeriodId"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"].ToString(),
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"].ToString()
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



        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleOrderVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM SaleOrders H
            LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

            LEFT JOIN 
					    (
						    SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						    FROM [dbo].[PurchaseOrderDetails] d   
						    GROUP BY d.PurchaseOrderId
					    ) SD ON H.Id = SD.PurchaseOrderId
            WHERE H.IsPost != 1 AND  (SD.TotalCompletedQty < SD.TotalQuantity)
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
	            ISNULL(H.CustomerId, 0) AS CustomerId,
                ISNULL(s.Name, 0) AS CustomerName,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd') , '') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDate, 'yyyy-MM-dd') , '') AS DeliveryDate,
                (ISNULL(SD.TotalQuantity, 0)-ISNULL(SD.TotalCompletedQty, 0)) AS TotalQuantity,
                ISNULL(SD.TotalCompletedQty, 0) AS TotalCompletedQty,
	            ISNULL(H.Comments, '') AS Comments,
	            ISNULL(H.TransactionType, '') AS TransactionType,
	            ISNULL(H.PeriodId, 0) AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,                
	            ISNULL(H.IsPost, 'N') AS IsPost,
                ISNULL(br.Name, '') AS BranchName


            FROM SaleOrders H
            LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id

            LEFT JOIN 
					    (
						    SELECT d.PurchaseOrderId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity, SUM(ISNULL(d.CompletedQty,0)) AS TotalCompletedQty
						    FROM [dbo].[PurchaseOrderDetails] d   
						    GROUP BY d.PurchaseOrderId
					    ) SD ON H.Id = SD.PurchaseOrderId
            WHERE H.IsPost != 1 AND  (SD.TotalCompletedQty < SD.TotalQuantity)

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleOrderVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
