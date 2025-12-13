using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{

    public class SalePersonVisitHistrieRepository : CommonRepository
    {
        //Insert Method
        public async Task<ResultVM> Insert(SalePersonVisitHistrieVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO SalePersonVisitHistries 
(
 BranchId, SalePersonId, RouteId, Date
)
VALUES 
(
 @BranchId, @SalePersonId, @RouteId,@Date
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RouteId", vm.RouteId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", vm.Date);

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



        // Update Method
        public async Task<ResultVM> Update(SalePersonVisitHistrieVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE SalePersonVisitHistries
SET 
    BranchId = @BranchId,
    SalePersonId = @SalePersonId,
    RouteId = @RouteId
   
 
WHERE Id = @Id;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RouteId", vm.RouteId ?? (object)DBNull.Value);

                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

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

        // Delete Method
        public async Task<ResultVM> Delete(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

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

                string query = $" UPDATE SalePersonVisitHistries SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                        cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                        cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);
                    }



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
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.SalePersonId, 0) AS SalePersonId,
    ISNULL(SP.Name, '') AS SalePersonName, 
    ISNULL(M.RouteId, 0) AS RouteId, 
    ISNULL(FORMAT(M.Date, 'yyyy-MM-dd'), '1900-01-01') AS Date
FROM SalePersonVisitHistries M
LEFT JOIN SalesPersons SP ON M.SalePersonId = SP.Id
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

                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonVisitHistrieVM
                {
                    Id = row.Field<int>("Id"),
                    BranchId = row.Field<int?>("BranchId"),
                    SalePersonId = row.Field<int?>("SalePersonId"),
                    SalePersonName = row.Field<string>("SalePersonName"),
                    RouteId = row.Field<int?>("RouteId")

                }).ToList();
                if (modelList.Count > 0)
                {
                    if (vm == null)
                    {
                        conditionalValues = new[] { modelList.Count > 0 ? modelList.FirstOrDefault().Id.ToString() : "" };
                    }

                    var detailsDataList = DetailsList(new[] { "D.SalePersonVisitHistroyId" }, conditionalValues, vm, conn, transaction);
                    if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var details = JsonConvert.DeserializeObject<List<SalePersonVisitHistrieDetailVM>>(json);

                        modelList.FirstOrDefault().SalePersonVisitHistrieDetails = details;
                    }
                }

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
                                ISNULL(D.SalePersonVisitHistroyId, 0) AS SalePersonVisitHistroyId,
                                ISNULL(D.BranchId, 0) AS BranchId,
                                ISNULL(D.CustomerId, 0) AS CustomerId,
                                ISNULL(C.Code, '') AS Code,
                                ISNULL(C.Name, '') AS Name,
                                ISNULL(C.Address, '') AS Address,
                                ISNULL(D.IsVisited, 0) AS IsVisited,
                                ISNULL(D.Latitude, 0) AS Latitude,
                                ISNULL(D.Longitude, 0) AS Longitude,
                                ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
                                ISNULL(Sa.Code, '') AS SaleOrderCode
                            FROM SalePersonVisitHistoryDetails D
                            LEFT JOIN Customers C ON C.Id = D.CustomerId
                            LEFT JOIN SaleOrders Sa ON Sa.Id = D.SaleOrderId
                            WHERE 1 = 1
";

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
                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonVisitHistrieDetailVM
                {
                    Id = row.Field<int>("Id"),
                    BranchId = row.Field<int?>("BranchId"),
                    Name = row.Field<string>("Name"),
                    Code = row.Field<string>("code"),
                    Address = row.Field<string?>("Address"),
                    SaleOrderCode = row.Field<string?>("SaleOrderCode"),
                    SaleOrderId = row.Field<int?>("SaleOrderId"),

                }).ToList();

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


        public async Task<ResultVM> CostomerList(SalePersonVisitHistrieVM salePersonVisitHistrie, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
        SELECT M.Id, M.Name, sp.BranchId
        FROM Customers M
        LEFT OUTER JOIN SalePersonRoutes sp ON sp.RouteId = M.RouteId
        WHERE sp.BranchId = @BranchId
            AND sp.RouteId = @RouteId
            AND sp.SalePersonId = @SalePersonId";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", salePersonVisitHistrie.BranchId);
                    cmd.Parameters.AddWithValue("@RouteId", salePersonVisitHistrie.RouteId);
                    cmd.Parameters.AddWithValue("@SalePersonId", salePersonVisitHistrie.SalePersonId);

                    using (SqlDataAdapter objComm = new SqlDataAdapter(cmd))
                    {
                        objComm.Fill(dataTable);
                    }
                }

                if (dataTable.Rows.Count > 0)
                {
                    var modelList = dataTable.AsEnumerable().Select(row => new CustomerVM
                    {
                        Id = row.Field<int>("Id"),
                        Name = row.Field<string>("Name"),
                        BranchId = row.Field<int>("BranchId")
                    }).ToList();

                    result.Status = "Success";
                    result.Message = "Data retrieved successfully.";
                    result.DataVM = modelList;
                }
                else
                {
                    result.Message = "No records found.";
                }
            }
            catch (Exception ex)
            {
                result.Message = "An error occurred.";
                result.ExMessage = ex.Message;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

        public async Task<ResultVM> DetailsInsert(List<SalePersonVisitHistrieDetailVM> visitDetails, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            int lastid = 0;
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
        INSERT INTO SalePersonVisitHistoryDetails (
            SalePersonVisitHistroyId,
            BranchId,
            CustomerId,
            IsVisited,
            Latitude,
            Longitude
        )
        VALUES (
            @SalePersonVisitHistroyId,  
            @BranchId,          
            @CustomerId,       
            @IsVisited,       
            @Latitude,    
            @Longitude      
        );

SELECT SCOPE_IDENTITY();";

                foreach (var vm in visitDetails)
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@SalePersonVisitHistroyId", vm.SalePersonVisitHistroyId);
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsVisited", false);
                        cmd.Parameters.AddWithValue("@Latitude", vm.Latitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Longitude", vm.Longitude ?? (object)DBNull.Value);

                        vm.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        lastid = vm.Id;
                    }
                }

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.Id = lastid.ToString();
                result.DataVM = visitDetails;

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

        public async Task<ResultVM> DetailsUpdate(List<SalePersonVisitHistrieDetailVM> visitDetails, SqlConnection conn = null, SqlTransaction transaction = null)
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

Update SalePersonVisitHistoryDetails set
                              IsVisited =@IsVisited,
                              Latitude=@Latitude,
                              Longitude=@Longitude,
                              SaleOrderId=@SaleOrderId
                              where Id=@Id
";

                foreach (var vm in visitDetails)
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                        //cmd.Parameters.AddWithValue("@SalePersonVisitHistroyId", vm.SalePersonVisitHistroyId);
                        //cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                        //cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsVisited", vm.IsVisited); // Assuming `IsVisited` is part of the `SalePersonVisitHistrieDetailVM`
                        cmd.Parameters.AddWithValue("@Latitude", vm.Latitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Longitude", vm.Longitude ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SaleOrderId", vm.SaleOrderId ?? (object)DBNull.Value);
                        //cmd.Parameters.AddWithValue("@Id", vm.Id); // Unique identifier for update

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                result.Status = "Success";
                result.Message = "Details data updated successfully.";
                result.Id = visitDetails.Last().Id.ToString();
                result.DataVM = visitDetails;

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
FROM SalePersonVisitHistries
WHERE 1 = 1
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
    BranchId, 
    SalePersonId, 
    RouteId, 
    CustomerId,
    Date,
    IsVisited 

FROM SalePersonVisitHistries
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



        // GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string salePersonId, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SalePersonVisitHistrieVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
        	FROM SalePersonVisitHistries H 
                LEFT OUTER JOIN BranchProfiles BP ON H.BranchId = Bp.Id
	            LEFT OUTER JOIN SalesPersons SO ON H.SalePersonId = SO.Id
	            LEFT OUTER JOIN Routes R ON H.BranchId = R.Id
	            
        	WHERE 1= 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : "");

                // Conditionally add SalePersonId filter if salePersonId is not "0"
                //if (!string.IsNullOrEmpty(salePersonId) && salePersonId != "0")
                //{
                //    sqlQuery += " AND H.SalePersonId = " + salePersonId; // Directly insert salePersonId in the query
                //}

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);
                // Data query with pagination and sorting
                sqlQuery += @"
            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex
	            ,ISNULL(H.Id,0)	Id
	            ,ISNULL(H.BranchId,0)	BranchId
	            ,ISNULL(BP.Code,'') BranchCode
	            ,ISNULL(BP.Name,'') BranchName
	            ,ISNULL(H.SalePersonId,0) SalePersonId
                ,ISNULL(SO.Name,'') SalePersonName
	            ,ISNULL(SO.Code,'') SalePersonCode
	            ,ISNULL(H.RouteId, 0)RouteId
	            ,ISNULL(R.Code,'') RouteCode
	            ,ISNULL(R.Name,'') RouteName
                ,ISNULL(H.Date, '1900-01-01') AS VisitDate
	            
	            FROM SalePersonVisitHistries H 
	            LEFT OUTER JOIN BranchProfiles BP ON H.BranchId = Bp.Id
	            LEFT OUTER JOIN SalesPersons SO ON H.SalePersonId = SO.Id
	            LEFT OUTER JOIN Routes R ON H.RouteId = R.Id
	            

	            WHERE 1 = 1
            -- Add the filter condition
             " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : "");

                //if (!string.IsNullOrEmpty(salePersonId) && salePersonId != "0")
                //{
                //    sqlQuery += " AND H.SalePersonId = " + salePersonId; 
                //}
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SalePersonVisitHistrieVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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


        public async Task<ResultVM> GetAllGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SalePersonVisitHistrieVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
                     SELECT COUNT(*) AS totalcount
                    FROM SalePersonVisitHistoryDetails D
                    LEFT OUTER JOIN SalePersonVisitHistries H ON H.Id = D.SalePersonVisitHistroyId
					WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                    ,ISNULL(H.[Date], '1900-01-01') AS [Date]
                    ,ISNULL(R.Name, '') AS [RouteName]
                    ,ISNULL(R.Address, '') AS [RouteAddress]
                    ,ISNULL(Sp.[Name], '') AS SalesPersonName
                    ,ISNULL(C.Name, '') AS [CustomerName]
                    ,ISNULL(C.BanglaName, '') AS [CustomerBanglaName]
                    ,ISNULL(C.Address, '') AS [CustomerAddress]
                    ,ISNULL(C.BanglaAddress, '') AS [CustomerBanglaAddress]
                    ,CASE WHEN ISNULL(D.IsVisited, 0) = 1 THEN 'Visited' ELSE 'Not-Visited' END AS Status
                    ,ISNULL(SO.Code, '') AS [SaleOrderNo]

				  from SalePersonVisitHistoryDetails D
                  Left outer join  SalePersonVisitHistries H on h.Id=d.SalePersonVisitHistroyId
                  Left outer join  SalesPersons Sp on Sp.Id=H.SalePersonId
                  Left outer join  SaleOrders SO on SO.Id=D.SaleOrderId
                  Left outer join  Customers C on C.Id=D.CustomerId
                  Left outer join   Routes R on R.Id=C.RouteId
				  where 1=1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonVisitHistrieVM>.FilterCondition(options.filter) + ")" : ""); /*+ @"*/
                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                //data = KendoGrid<SaleOrderVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
                data = KendoGrid<SalePersonVisitHistrieVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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





        public async Task<bool> SalePersonVisitHistrieCheckExists(SalePersonVisitHistrieVM salePersonVisitHistrie, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            bool isExists = false;
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
            ISNULL(H.Id, 0) AS Id
    
        FROM 
            SalePersonVisitHistries H

        
WHERE 
    1 = 1
 ";

                if (salePersonVisitHistrie != null && salePersonVisitHistrie.Id > 0)
                {
                    query += " AND H.Id != @Id ";
                }
                if (salePersonVisitHistrie != null && !string.IsNullOrEmpty(salePersonVisitHistrie.Date))
                {
                    query += " AND H.Date = @Date ";
                }

                if (salePersonVisitHistrie != null && salePersonVisitHistrie.SalePersonId > 0)
                {
                    query += " AND H.SalePersonId = @SalePersonId ";
                }
                if (salePersonVisitHistrie != null && salePersonVisitHistrie.BranchId > 0)
                {
                    query += " AND H.BranchId = @BranchId ";
                }
                if (salePersonVisitHistrie != null && salePersonVisitHistrie.RouteId > 0)
                {
                    query += " AND H.RouteId = @RouteId ";
                }

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param

                if (salePersonVisitHistrie != null && salePersonVisitHistrie.Id > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", salePersonVisitHistrie.Id);
                }
                if (salePersonVisitHistrie != null && !string.IsNullOrEmpty(salePersonVisitHistrie.Date))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Date", salePersonVisitHistrie.Date);
                }

                if (salePersonVisitHistrie != null && salePersonVisitHistrie.BranchId > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", salePersonVisitHistrie.BranchId);
                }
                if (salePersonVisitHistrie != null && salePersonVisitHistrie.SalePersonId > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@SalePersonId", salePersonVisitHistrie.SalePersonId);
                }
                if (salePersonVisitHistrie != null && salePersonVisitHistrie.RouteId > 0)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@RouteId", salePersonVisitHistrie.RouteId);
                }
                objComm.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    isExists = true;
                }

                return isExists;
            }
            catch (Exception ex)
            {

                return isExists;
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
