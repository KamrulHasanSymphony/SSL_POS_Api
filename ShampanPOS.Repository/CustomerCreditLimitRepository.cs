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

    public class CustomerCreditLimitRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CustomerCreditLimitVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO CustomerCreditLimits 
(

CustomerId,
BranchId, 
LimitEntryDate,
CreditLimit,
IsLatest, 
IsApproveed, 
IsPost, 
CreatedBy,
CreatedOn

)
VALUES 
(
@CustomerId, 
@BranchId,
@LimitEntryDate,
@CreditLimit,
@IsLatest, 
@IsApproveed, 
@IsPost,  
@CreatedBy,
@CreatedOn
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@LimitEntryDate", vm.LimitEntryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreditLimit", vm.CreditLimit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsLatest", vm.IsLatest);
                    cmd.Parameters.AddWithValue("@IsApproveed", vm.IsApproved);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);

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
        public async Task<ResultVM> Update(CustomerCreditLimitVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CustomerCreditLimits 
SET 


    CustomerId = @CustomerId,
    BranchId = @BranchId,
    LimitEntryDate = @LimitEntryDate,
    CreditLimit = @CreditLimit,
    IsLatest = @IsLatest,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = @LastModifiedOn

    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@LimitEntryDate", vm.LimitEntryDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreditLimit", vm.CreditLimit ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsLatest", vm.IsLatest);            
                    cmd.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedOn ?? (object)DBNull.Value);
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
        public async Task<ResultVM> Delete(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = IDs, DataVM = null };

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

                string query = "UPDATE CustomerCreditLimits SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    string ids = string.Join(",", IDs);
                    cmd.Parameters.AddWithValue("@Ids", ids);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data deleted successfully.";
                    }
                    else
                    {
                        result.Message = "No rows were deleted.";
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
    ISNULL(M.CustomerId, 0) AS CustomerId,
	 ISNULL(C.Name, 0) AS CustomerName,
	  ISNULL(C.Code, 0) AS CustomerCode,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(FORMAT(M.LimitEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') LimitEntryDate,
    ISNULL(M.CreditLimit, 0.0) AS CreditLimit,
    ISNULL(M.IsLatest, 0) AS IsLatest,  
    ISNULL(M.IsApproveed, 0) AS IsApproveed,

    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
    ISNULL(M.CreatedBy,'') CreatedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm'), '') PostedOn,
    ISNULL(M.PostedBy,'') PostedBy,
    ISNULL(M.LastModifiedBy,'') LastModifiedBy,   
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '') LastModifiedOn


    FROM CustomerCreditLimits M
	left outer join Customers C ON M.CustomerId=C.Id

WHERE 1 = 1";


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

                var model = new List<CustomerCreditLimitVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerCreditLimitVM
                    {

                        Id = Convert.ToInt32(row["Id"]),
                        LimitEntryDate = row["LimitEntryDate"].ToString(),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CustomerName = row.Field<string>("CustomerName"),
                        CustomerCode = row.Field<string>("CustomerCode"),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CreditLimit = Convert.ToDecimal(row["CreditLimit"]),                        
                        IsApproved = Convert.ToBoolean(row["IsApproveed"]),
                        IsLatest = Convert.ToBoolean(row["IsLatest"]),
                        CreatedOn = (row["CreatedOn"]).ToString(),
                        CreatedBy =(row["CreatedBy"]).ToString(),
                        PostedBy = (row["PostedBy"]).ToString(),
                        PostedOn = (row["PostedOn"]).ToString(),
                        LastModifiedBy = (row["LastModifiedBy"]).ToString(),
                        LastModifiedOn = (row["LastModifiedOn"]).ToString()

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
        // GetLimitEntryDateExist Method
        public async Task<ResultVM> GetLimitEntryDateExist(CustomerCreditLimitVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT 1
FROM CustomerCreditLimits
WHERE CustomerId = @CustomerId  AND BranchId = @BranchId AND CAST(LimitEntryDate AS DATE) = @LimitEntryDate ";

                if (vm.Id > 0)
                {
                    query += " AND Id != @Id ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                        adapter.SelectCommand.Parameters.AddWithValue("@LimitEntryDate", vm.LimitEntryDate);
                        adapter.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                        adapter.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
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
    CustomerId,
    BranchId,
    LimitEntryDate,
    CreditLimit,
    IsLatest,
    IsApproved,
    ApprovedBy,
    ApprovedDate,
    IsPosted,
    PostedBy,
    PostedDate,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn

FROM CustomerCreditLimits
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
FROM CustomerCreditLimits
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
        public async Task<ResultVM> GetGridData(GridOptions options, string customerId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<CustomerCreditLimitVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query

            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM CustomerCreditLimits H 
			LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
			LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
			WHERE 1 = 1 

 -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerCreditLimitVM>.FilterCondition(options.filter) + ")" : "");

                // Conditionally add SalePersonId filter if salePersonId is not "0"
                if (!string.IsNullOrEmpty(customerId) && customerId != "0")
                {
                    sqlQuery += " AND H.CustomerId = " + customerId; // Directly insert salePersonId in the query
                }

                // Data query with pagination and sorting
                sqlQuery += @"          

            -- Data query with pagination and sorting

            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                 ISNULL(H.Id, 0) AS Id
                ,ISNULL(FORMAT(H.LimitEntryDate,'yyyy-MM-dd HH:mm'),'1900-01-01') LimitEntryDate
				,ISNULL(H.CreditLimit,0) CreditLimit
				,ISNULL(H.ApproveedBy,'')	ApproveedBy
				,ISNULL(H.IsLatest,0) IsLatest
				,ISNULL(H.IsApproveed,0) IsApproveed
				,ISNULL(FORMAT(H.ApproveedDate,'yyyy-MM-dd HH:mm'),'1900-01-01') ApproveedDate
				,ISNULL(H.IsPost,0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
				,ISNULL(H.PostedBy,'') PostedBy
				,ISNULL(FORMAT(H.PostedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') PostedOn
				,ISNULL(H.CreatedBy,'''') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(H.CreatedFrom,'') CreatedFrom
				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom		
				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn				
		        ,ISNULL(H.CustomerId,0) CustomerId
                ,ISNULL(C.Name,'') CustomerName
                ,ISNULL(H.BranchId,0) BranchId
                ,ISNULL(BF.Name,'') BranchName

				FROM CustomerCreditLimits H 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
				LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id	
				
				WHERE 1 = 1

           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerCreditLimitVM>.FilterCondition(options.filter) + ")" : "");

                // Conditionally add SalePersonId filter if salePersonId is not "0"
                if (!string.IsNullOrEmpty(customerId) && customerId != "0")
                {
                    sqlQuery += " AND H.CustomerId = " + customerId; // Directly insert customerId in the query
                }

                sqlQuery += @"
        ) AS a
        WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<CustomerCreditLimitVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = $" UPDATE CustomerCreditLimits SET IsPost = 1, PostedBy = @PostedBy ,LastUpdateFrom=@LastUpdateFrom, PostedOn = GETDATE() WHERE Id IN ({inClause}) ";


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

        public async Task<ResultVM> BranchWiseCustomerCreditLimit(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
         OVER (PARTITION BY a.CustomerId 
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
        WHERE 1=1

        UNION ALL

        SELECT 
            'B' AS SL,
            ISNULL(s.Code, 0) AS Code,
            NULL AS InvoiceDateTime,  -- Placeholder for InvoiceDateTime
            ISNULL(s.CustomerId, 0) AS CustomerId,
            0 AS Opening,
            b.Amount AS DrAmount,
            0 AS CrAmount,
            b.ModeOfPayment + '~ ' + '~ ' AS Remarks
        FROM CustomerPaymentCollection b
        LEFT OUTER JOIN SaleDeleveries s ON b.CustomerId = s.CustomerId
        WHERE 1=1
    ) AS a
) AS a
LEFT OUTER JOIN Customers c ON a.CustomerId = c.Id
WHERE c.IsActive = 1
ORDER BY c.Id, a.InvoiceDateTime, a.Code, SL                              
";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND c.Id = @Id ";
                }

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    query += " AND CAST(c.InvoiceDateTime AS DATE) BETWEEN @FromDate AND @ToDate ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                query += @" GROUP BY    
                                    c.BreanchId,
                                    c.CustomerId,
                                    c.CustomerCode,
                                    c.CustomerName,
                                    h.CustomerId,
                                    uom.Name ";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
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

                var lst = new List<PurchaseOrderReportVM>();
                int serialNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new PurchaseOrderReportVM
                    {
                        SL = serialNumber,
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        ProductGroupName = row["ProductGroupName"].ToString(),
                        ProductCode = row["ProductCode"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        HSCodeNo = row["HSCodeNo"].ToString(),
                        UOMName = row["UOMName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"])
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
         OVER (PARTITION BY a.CustomerId 
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
        WHERE 1=1

        UNION ALL

        SELECT 
            'B' AS SL,
            ISNULL(s.Code, 0) AS Code,
            NULL AS InvoiceDateTime,  -- Placeholder for InvoiceDateTime
            ISNULL(s.CustomerId, 0) AS CustomerId,
            0 AS Opening,
            b.Amount AS DrAmount,
            0 AS CrAmount,
            b.ModeOfPayment + '~ ' + '~ ' AS Remarks
        FROM CustomerPaymentCollection b
        LEFT OUTER JOIN SaleDeleveries s ON b.CustomerId = s.CustomerId
        WHERE 1=1
    ) AS a
) AS a
LEFT OUTER JOIN Customers c ON a.CustomerId = c.Id
WHERE c.IsActive = 1
ORDER BY c.Id, a.InvoiceDateTime, a.Code, SL";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND c.Id = @Id ";
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
