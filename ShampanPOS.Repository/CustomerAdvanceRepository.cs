using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
   
    public class CustomerAdvanceRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CustomerAdvanceVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

INSERT INTO CustomerAdvances 
(

AdvanceEntryDate,
AdvanceAmount, 
PaymentEnumTypeId,
PaymentDate,
DocumentNo,
BankName, 
BankBranchName,
CustomerId,
BranchId, 
IsPost,    
CreatedBy,
CreatedOn

)
VALUES 
( 
@AdvanceEntryDate,
@AdvanceAmount,
@PaymentEnumTypeId,
@PaymentDate,
@DocumentNo,
@BankName,
@BankBranchName,
@CustomerId,
@BranchId, 
@IsPost,  
@CreatedBy,
@CreatedOn

);
SELECT SCOPE_IDENTITY();";
                Console.WriteLine(query);

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {


                    cmd.Parameters.AddWithValue("@AdvanceEntryDate", vm.AdvanceEntryDate);
                    cmd.Parameters.AddWithValue("@AdvanceAmount", vm.AdvanceAmount);
                    cmd.Parameters.AddWithValue("@PaymentEnumTypeId", vm.PaymentEnumTypeId);
                    cmd.Parameters.AddWithValue("@PaymentDate", vm.PaymentDate);
                    cmd.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", vm.BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankBranchName", vm.BankBranchName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);                  
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

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
        public async Task<ResultVM> Update(CustomerAdvanceVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CustomerAdvances

SET 

 
AdvanceEntryDate = @AdvanceEntryDate,
AdvanceAmount = @AdvanceAmount, 
PaymentDate = @PaymentDate,
DocumentNo = @DocumentNo, 
BankName = @BankName, 
BankBranchName = @BankBranchName,
CustomerId = @CustomerId, 
PaymentEnumTypeId = @PaymentEnumTypeId,
LastModifiedBy = @LastModifiedBy,
LastModifiedOn = @LastModifiedOn

WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {

                    cmd.Parameters.AddWithValue("@Id", vm.Id);

                    cmd.Parameters.AddWithValue("@AdvanceEntryDate", vm.AdvanceEntryDate);
                    cmd.Parameters.AddWithValue("@AdvanceAmount", vm.AdvanceAmount);
                    cmd.Parameters.AddWithValue("@PaymentDate", vm.PaymentDate);
                    cmd.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", vm.BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankBranchName", vm.BankBranchName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@PaymentEnumTypeId", vm.PaymentEnumTypeId);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now.ToString());

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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "", DataVM = null };

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

                string query = "UPDATE Currencies SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

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
    ISNULL(FORMAT(M.AdvanceEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AdvanceEntryDate,
	ISNULL(M.AdvanceAmount,0) AdvanceAmount,
    ISNULL(FORMAT(M.PaymentDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') PaymentDate,
	ISNULL(M.DocumentNo,'') DocumentNo,
	ISNULL(M.BankName,'') BankName,
	ISNULL(M.BankBranchName,'') BankBranchName,
	ISNULL(M.CustomerId,0) CustomerId,
    ISNULL(C.Name, 0) AS CustomerName,
	ISNULL(C.Code, 0) AS CustomerCode,
	ISNULL(M.PaymentEnumTypeId,0) PaymentEnumTypeId,
	ISNULL(M.ReceiveByDeliveryPersonId,0) ReceiveByDeliveryPersonId,
	ISNULL(M.ReceiveByEnumTypeId,0) ReceiveByEnumTypeId,
	ISNULL(M.IsApproved,0) IsApproved,
	ISNULL(M.ApproveBy,'') ApproveBy,
    ISNULL(FORMAT(M.ApproveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') ApproveDate,
	ISNULL(M.PostedBy,'') PostedBy,
	ISNULL(M.CreatedBy,'') CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '') CreatedOn,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm'), '') PostedOn,
	ISNULL(M.LastModifiedBy,'') LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '') LastModifiedOn


FROM CustomerAdvances M
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

                var model = new List<CustomerAdvanceVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerAdvanceVM
                    {
                        Id =Convert.ToInt32(row["Id"]),
                        AdvanceEntryDate =row["AdvanceEntryDate"].ToString(),
                        AdvanceAmount = Convert.ToDecimal(row["AdvanceAmount"]),
                        PaymentDate = (row["PaymentDate"]).ToString(),
                        DocumentNo = (row["DocumentNo"]).ToString(),
                        BankName = (row["BankName"]).ToString(),
                        BankBranchName = row["BankBranchName"].ToString(),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        CustomerName = row.Field<string>("CustomerName"),
                        //CustomerCode = row.Field<string>("CustomerCode"),
                        PaymentEnumTypeId = Convert.ToInt32(row["PaymentEnumTypeId"]),
                        //ReceiveByDeliveryPersonId = Convert.ToInt32(row["ReceiveByDeliveryPersonId"]),
                        //ReceiveByEnumTypeId = Convert.ToInt32(row["ReceiveByEnumTypeId"]),
                        //IsApproved = Convert.ToBoolean(row["IsApproved"]),
                        PostedBy = (row["PostedBy"]).ToString(),
                        CreatedBy = (row["CreatedBy"]).ToString(),
                        CreatedOn = (row["CreatedOn"]).ToString(),
                        PostedOn = (row["PostedOn"]).ToString(),
                        LastModifiedBy =(row["LastModifiedBy"]).ToString(),
                        LastModifiedOn = (row["LastModifiedOn"]).ToString(),

                      

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
    CustomerId,
    AdvanceEntryDate,
    AdvanceAmount,
    PaymentEnumTypeId,
    PaymentDate,
    DocumentNo,
    BankName,
    BankBranchName,
    ReceiveByDeliveryPersonId,
    ReceiveByEnumTypeId,
    IsApproved,
    ApproveBy,
    ApproveDate,
    IsPosted,
    PostedBy,
    PostedDate,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn
FROM CustomerAdvances
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
FROM CustomerAdvances
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

                var data = new GridEntity<CustomerAdvanceVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query

            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM CustomerAdvances H 
			LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id 
			LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
			LEFT OUTER JOIN EnumTypes ET ON H.PaymentEnumTypeId = ET.Id

			WHERE 1 = 1 

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerAdvanceVM>.FilterCondition(options.filter) + ")" : "");

               
                if (!string.IsNullOrEmpty(customerId) && customerId != "0")
                {
                    sqlQuery += " AND H.CustomerId = " + customerId; 
                }

                
                sqlQuery += @"    
            -- Data query with pagination and sorting

            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                ISNULL(H.Id, 0) AS Id
                
                ,ISNULL(FORMAT(H.AdvanceEntryDate,'yyyy-MM-dd HH:mm'),'1900-01-01') AdvanceEntryDate           
				,ISNULL(H.AdvanceAmount,0) AdvanceAmount
				,ISNULL(FORMAT(H.PaymentDate,'yyyy-MM-dd HH:mm'),'1900-01-01') PaymentDate				 
				,ISNULL(H.DocumentNo,'')	DocumentNo
				,ISNULL(H.BankName,'')	BankName
				,ISNULL(H.BankBranchName,'')	BankBranchName
				,ISNULL(H.IsPost,0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
				,ISNULL(H.PostedBy,'') PostedBy
				,ISNULL(FORMAT(H.PostedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') PostedOn
				,ISNULL(H.CreatedBy,'') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(H.CreatedFrom,'') CreatedFrom
				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom		
				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				--,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
		        ,ISNULL(H.CustomerId,0) CustomerId
                ,ISNULL(C.Name,'') CustomerName
		        ,ISNULL(H.PaymentEnumTypeId,0) PaymentEnumTypeId


				FROM CustomerAdvances H 
				LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
			    LEFT OUTER JOIN EnumTypes ET ON H.PaymentEnumTypeId = ET.Id

				
				WHERE 1 = 1

            -- Add the filter condition
           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerAdvanceVM>.FilterCondition(options.filter) + ")" : "");


                if (!string.IsNullOrEmpty(customerId) && customerId != "0")
                {
                    sqlQuery += " AND H.CustomerId = " + customerId;
                }


                sqlQuery += @"  

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<CustomerAdvanceVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                string query = $" UPDATE CustomerAdvances SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                

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




    }

}
