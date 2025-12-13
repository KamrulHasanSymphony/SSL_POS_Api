using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanPOS.Repository
{
    public class BranchCreditLimitRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(BranchCreditLimitVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

INSERT INTO BranchCreditLimits 
(

OtherCreditLimit,
SelfCreditLimit, 
LimitEntryDate, 
BranchId, 
IsApproveed,
CreatedOn,
CreatedBy, 
IsPost, 
IsLatest

)
VALUES 
(
 
@OtherCreditLimit,
@SelfCreditLimit, 
@LimitEntryDate,
@BranchId,
@IsApproveed,
@CreatedOn,
@CreatedBy,
@IsPost, 
@IsLatest

);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    
                    cmd.Parameters.AddWithValue("@OtherCreditLimit", vm.OtherCreditLimit);
                    cmd.Parameters.AddWithValue("@SelfCreditLimit", vm.SelfCreditLimit);
                    cmd.Parameters.AddWithValue("@LimitEntryDate", vm.LimitEntryDate);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@IsApproveed", vm.IsApproved);
                    cmd.Parameters.AddWithValue("@CreatedOn", vm.CreatedOn);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);   
                    cmd.Parameters.AddWithValue("@IsLatest", vm.IsLatest);

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
        public async Task<ResultVM> Update(BranchCreditLimitVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

UPDATE BranchCreditLimits 
SET 

OtherCreditLimit=@OtherCreditLimit,
SelfCreditLimit=@SelfCreditLimit, 
LimitEntryDate=@LimitEntryDate, 
BranchId=@BranchId,
LastModifiedBy=@LastModifiedBy,
LastModifiedOn=GETDATE(),
IsLatest=@IsLatest

WHERE Id = @Id"
;

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                                  
                    cmd.Parameters.AddWithValue("@OtherCreditLimit", vm.OtherCreditLimit);
                    cmd.Parameters.AddWithValue("@SelfCreditLimit", vm.SelfCreditLimit);
                    cmd.Parameters.AddWithValue("@LimitEntryDate", vm.LimitEntryDate);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsLatest", vm.IsLatest);


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

                string query = "UPDATE BranchCreditLimits SET IsArchive = 1 , IsActive = 0 WHERE Id IN (@Ids)";

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

 ISNULL(M.Id,0) Id,
 ISNULL(M.OtherCreditLimit,0) OtherCreditLimit,
 ISNULL(M.SelfCreditLimit,0) SelfCreditLimit,
 ISNULL(FORMAT(M.LimitEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') LimitEntryDate,
 ISNULL(M.BranchId,0) BranchId,
 ISNULL(B.Name,'') BranchName,
 ISNULL(B.Code,'') BranchCode,
 ISNULL(M.IsApproveed,0) IsApproveed,
 ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
 ISNULL(M.CreatedBy,'') CreatedBy,
 
 ISNULL(M.PostedBy,'') PostedBy,
 ISNULL(FORMAT(M.ApproveedDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') ApproveedDate,
 ISNULL(M.ApproveedBy,'') ApproveedBy,
 ISNULL(M.LastModifiedBy,'') LastModifiedBy,
 ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '') LastModifiedOn,
 ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm'), '') PostedOn,
 ISNULL(M.IsLatest,0) IsLatest

FROM BranchCreditLimits M
LEFT OUTER JOIN BranchProfiles B on M.BranchId = B.Id
WHERE 1=1";

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

                var model = new List<BranchCreditLimitVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new BranchCreditLimitVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        BranchName = row.Field<string>("BranchName"),
                        BranchCode = row.Field<string>("BranchCode"),
                        CreatedOn = Convert.ToString(row["CreatedOn"]),
                        CreatedBy = row["CreatedBy"].ToString(),
                        PostedBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"].ToString(),                                   
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"].ToString(),
                        IsLatest = Convert.ToBoolean(row["IsLatest"]),
                        OtherCreditLimit = Convert.ToDecimal(row["OtherCreditLimit"]),
                        SelfCreditLimit = Convert.ToDecimal(row["SelfCreditLimit"]),
                        LimitEntryDate = Convert.ToString(row["LimitEntryDate"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                   
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
 ISNULL(M.Id,0) Id,
 ISNULL(M.CreatedOn,'1900-01-01') CreatedOn,
 ISNULL(M.CreatedBy,'') CreatedBy,
 ISNULL(M.PostedDate,'1900-01-01') PostedDate,
 ISNULL(M.PostedBy,'') PostedBy,
 ISNULL(M.IsPosted,0) IsPosted,
 ISNULL(M.ApprovedDate,'1900-01-01') ApprovedDate,
 ISNULL(M.ApprovedBy,'') ApprovedBy,
 ISNULL(M.LastModifiedBy,'') LastModifiedBy,
 ISNULL(M.LastModifiedOn,'1900-01-01') LastModifiedOn,
 ISNULL(M.IsLatest,0) IsLatest,
 ISNULL(M.OtherCreditLimit,0) OtherCreditLimit,
 ISNULL(M.SelfCreditLimit,0) SelfCreditLimit,
 ISNULL(M.LimitEntryDate,'1900-01-01') LimitEntryDate,
 ISNULL(M.BranchId,0) BranchId,
 ISNULL(M.IsApproved,0) IsApproved
FROM BranchCreditLimits M WHERE 1=1";

                DataTable dataTable = new DataTable();

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

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


        // Dropdown Method
        public async Task<ResultVM> Dropdown(SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @"
SELECT
 ISNULL(Id, 0) Id,
 ISNULL(CreatedBy, '') CreatedBy
FROM BranchCreditLimits
WHERE IsPosted = 1
ORDER BY CreatedBy";

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


        public async Task<ResultVM> GetGridData(GridOptions options,string BranchId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<BranchCreditLimitVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query

            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM BranchCreditLimits H 
		    LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
			WHERE 1 = 1 

            -- Add the filter condition
             " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchCreditLimitVM>.FilterCondition(options.filter) + ")" : "");
                if (!string.IsNullOrEmpty(BranchId) && BranchId != "0")
                {
                    sqlQuery += " AND H.BranchId = " + BranchId; // Directly insert salePersonId in the query
                }
                sqlQuery += @"
            -- Data query with pagination and sorting

            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                ISNULL(H.Id, 0) AS Id
                
                ,ISNULL(FORMAT(H.LimitEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LimitEntryDate              
				,ISNULL(H.SelfCreditLimit,0) SelfCreditLimit
				,ISNULL(H.OtherCreditLimit,0) OtherCreditLimit
				,ISNULL(H.ApproveedBy,'')	ApproveedBy
				,ISNULL(H.IsLatest,0) IsLatest
				,ISNULL(H.IsApproveed,0) IsApproveed		
                ,ISNULL(FORMAT(H.ApproveedDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS ApproveedDate
				,ISNULL(H.IsPost,0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
				,ISNULL(H.PostedBy,'') PostedBy				
                ,ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS PostedOn
				,ISNULL(H.CreatedBy,'') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(H.CreatedFrom,'') CreatedFrom
				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom						
                ,ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn			
                --,ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
		        ,ISNULL(H.BranchId,0) BranchId

				FROM BranchCreditLimits H 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
				
				WHERE 1 = 1

            -- Add the filter condition
             " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchCreditLimitVM>.FilterCondition(options.filter) + ")" : "");
                if (!string.IsNullOrEmpty(BranchId) && BranchId != "0")
                {
                    sqlQuery += " AND H.BranchId = " + BranchId; // Directly insert salePersonId in the query
                }
                sqlQuery += @"
            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<BranchCreditLimitVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                string query = $" UPDATE BranchCreditLimits SET IsPost = 1, PostedBy = @PostedBy ,LastUpdateFrom=@LastUpdateFrom, PostedOn = GETDATE() WHERE Id IN ({inClause}) ";


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

