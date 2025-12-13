using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanPOS.Repository
{
    public class BranchAdvanceRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(BranchAdvanceVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO BranchAdvances
(

BranchId,
AdvanceEntryDate,
AdvanceAmount,
PaymentEnumTypeId,
PaymentDate, 
DocumentNo, 
BankName,
BankBranchName,
ReceiveByEnumTypeId, 
IsApproved, 
IsPost

)
VALUES
(
@BranchId,
@AdvanceEntryDate, 
@AdvanceAmount, 
@PaymentEnumTypeId,
@PaymentDate,
@DocumentNo, 
@BankName,
@BankBranchName,
@ReceiveByEnumTypeId,
@IsApproved,
@IsPost

);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@AdvanceEntryDate", vm.AdvanceEntryDate);
                    cmd.Parameters.AddWithValue("@AdvanceAmount", vm.AdvanceAmount);
                    cmd.Parameters.AddWithValue("@PaymentEnumTypeId", vm.PaymentEnumTypeId);
                    cmd.Parameters.AddWithValue("@PaymentDate", vm.PaymentDate);
                    cmd.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", vm.BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankBranchName", vm.BankBranchName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReceiveByEnumTypeId", vm.ReceiveByEnumTypeId);
                    cmd.Parameters.AddWithValue("@IsApproved", vm.IsApproved);
                    cmd.Parameters.AddWithValue("@IsPost", false);


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
        public async Task<ResultVM> Update(BranchAdvanceVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE BranchAdvances
SET
 BranchId = @BranchId, 
AdvanceEntryDate = @AdvanceEntryDate,
AdvanceAmount = @AdvanceAmount, 
 PaymentEnumTypeId = @PaymentEnumTypeId, 
PaymentDate = @PaymentDate,
DocumentNo = @DocumentNo, 
 BankName = @BankName,
BankBranchName = @BankBranchName,
ReceiveByEnumTypeId = @ReceiveByEnumTypeId, 
IsApproved = @IsApproved



WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@AdvanceEntryDate", vm.AdvanceEntryDate);
                    cmd.Parameters.AddWithValue("@AdvanceAmount", vm.AdvanceAmount);
                    cmd.Parameters.AddWithValue("@PaymentEnumTypeId", vm.PaymentEnumTypeId);
                    cmd.Parameters.AddWithValue("@PaymentDate", vm.PaymentDate);
                    cmd.Parameters.AddWithValue("@DocumentNo", vm.DocumentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankName", vm.BankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankBranchName", vm.BankBranchName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReceiveByEnumTypeId", vm.ReceiveByEnumTypeId);
                    cmd.Parameters.AddWithValue("@IsApproved", vm.IsApproved);
                    //cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);


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

                string query = "UPDATE Areas SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

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

    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.BranchId, 0) AS BranchId,  
     ISNULL(B.Name,'') BranchName,
     ISNULL(B.Code,'') BranchCode,
    ISNULL(FORMAT(M.AdvanceEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AdvanceEntryDate,
    ISNULL(M.AdvanceAmount, 0.0) AS AdvanceAmount,
    ISNULL(M.PaymentEnumTypeId, 0) AS PaymentEnumTypeId,   
    ISNULL(FORMAT(M.PaymentDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') PaymentDate,
    ISNULL(M.DocumentNo, '') AS DocumentNo,
    ISNULL(M.BankName, '') AS BankName,
    ISNULL(M.BankBranchName, '') AS BankBranchName,
    ISNULL(M.ReceiveByEnumTypeId, 0) AS ReceiveByEnumTypeId,
    ISNULL(M.IsApproved, 0) AS IsApproved,
    ISNULL(M.ApproveById, 0) AS ApproveById,   
    ISNULL(FORMAT(M.ApproveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') ApproveDate,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,   
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm'), '') PostedOn
   
FROM BranchAdvances M
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

                var model = new List<BranchAdvanceVM>();


              
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new BranchAdvanceVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        BranchName = row.Field<string>("BranchName"),
                        BranchCode = row.Field<string>("BranchCode"),
                        AdvanceEntryDate =(row["AdvanceEntryDate"]).ToString(),
                        AdvanceAmount = Convert.ToDecimal(row["AdvanceAmount"]),
                        PaymentEnumTypeId = Convert.ToInt32(row["PaymentEnumTypeId"]),
                        PaymentDate = (row["PaymentDate"]).ToString(),
                        DocumentNo = row["DocumentNo"].ToString(),
                        BankName = row["BankName"].ToString(),
                        BankBranchName = row["BankBranchName"].ToString(),
                        ReceiveByEnumTypeId = Convert.ToInt32(row["ReceiveByEnumTypeId"]),
                        IsApproved = Convert.ToBoolean(row["IsApproved"]),
                        ApproveById = Convert.ToString(row["ApproveById"]),
                        ApproveDate = (row["ApproveDate"]).ToString(),
                        IsPost = (bool)row["IsPost"],
                        PostedBy = row["PostedBy"].ToString(),
                        PostedOn = (row["PostedOn"]).ToString(),
                      

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
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.AdvanceEntryDate, '1900-01-01') AS AdvanceEntryDate,
    ISNULL(M.AdvanceAmount, 0.0) AS AdvanceAmount,
    ISNULL(M.PaymentEnumTypeId, 0) AS PaymentEnumTypeId,
    ISNULL(M.PaymentDate, '1900-01-01') AS PaymentDate,
    ISNULL(M.DocumentNo, '') AS DocumentNo,
    ISNULL(M.BankName, '') AS BankName,
    ISNULL(M.BankBranchName, '') AS BankBranchName,
    ISNULL(M.ReceiveByEnumTypeId, 0) AS ReceiveByEnumTypeId,
    ISNULL(M.IsApproved, 0) AS IsApproved,
    ISNULL(M.ApproveById, 0) AS ApproveById,
    ISNULL(M.ApproveDate, '1900-01-01') AS ApproveDate,
    ISNULL(M.IsPosted, 0) AS IsPosted,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(M.PostedDate, '1900-01-01') AS PostedDate
FROM BranchAdvances M
WHERE 1=1";

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
                SELECT Id, Name
                FROM BranchAdvances
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


        public async Task<ResultVM> GetGridData(GridOptions options, string BranchId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<BranchAdvanceVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query

            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM BranchAdvances H
            LEFT OUTER JOIN BranchProfiles BP ON H.BranchId = BP.Id
            LEFT OUTER JOIN EnumTypes ET ON H.PaymentEnumTypeId = ET.Id
            LEFT OUTER JOIN EnumTypes ETD ON H.ReceiveByEnumTypeId = ETD.Id
            WHERE 1= 1

            -- Add the filter condition
             " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchAdvanceVM>.FilterCondition(options.filter) + ")" : "");
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
                ISNULL(H.Id, 0) AS Id,
                ISNULL(FORMAT(H.AdvanceEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS AdvanceEntryDate,
                ISNULL(FORMAT(H.PaymentDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS PaymentDate,
                ISNULL(FORMAT(H.ApproveDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS ApproveDate,
                ISNULL(H.AdvanceAmount, 0) AS AdvanceAmount,
                ISNULL(H.DocumentNo, '') AS DocumentNo,
                ISNULL(H.BankName, '') AS BankName,
                ISNULL(H.BankBranchName, '') AS BankBranchName,

                ISNULL(H.IsApproved, 0) AS IsApproved,
                ISNULL(H.IsPost, 0) AS IsPost,
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                ISNULL(H.PostedBy, '') AS PostedBy,
               
                ISNULL(H.BranchId, 0) AS BranchId,
                ISNULL(H.PaymentEnumTypeId, 0) AS PaymentEnumTypeId,
                ISNULL(H.ReceiveByEnumTypeId, 0) AS ReceiveByEnumTypeId
               
                FROM BranchAdvances H
                LEFT OUTER JOIN BranchProfiles BP ON H.BranchId = BP.Id
                LEFT OUTER JOIN EnumTypes ET ON H.PaymentEnumTypeId = ET.Id
                LEFT OUTER JOIN EnumTypes ETD ON H.ReceiveByEnumTypeId = ETD.Id
                
                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchAdvanceVM>.FilterCondition(options.filter) + ")" : "");
                if (!string.IsNullOrEmpty(BranchId) && BranchId != "0")
                {
                    sqlQuery += " AND H.BranchId = " + BranchId; // Directly insert salePersonId in the query
                }
                sqlQuery += @"
            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<BranchAdvanceVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                string query = $" UPDATE BranchAdvances SET IsPost = 1, PostedBy = @PostedBy , PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
               

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
    

