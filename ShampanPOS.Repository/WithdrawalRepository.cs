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
    public class WithdrawalRepository : CommonRepository
    {

        // Insert Method
        public async Task<ResultVM> Insert(WithdrawalVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO Withdrawals
(
     Code, TransactionDate,Reference, FromBankAccountId, ChequeNo,ChequeBankName, ChequeDate,ToBankAccountId,IsCash,TotalDepositAmount,
    Comments, IsArchive, IsActive, CreatedBy, CreatedOn
)
VALUES
(
    @Code, @TransactionDate, @Reference,@FromBankAccountId, @ChequeNo,@ChequeBankName, @ChequeDate,@ToBankAccountId,@IsCash, @TotalDepositAmount,@Comments,
     @IsArchive, @IsActive, @CreatedBy, GETDATE() 
);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code);
                    cmd.Parameters.AddWithValue("@ChequeNo", vm.ChequeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromBankAccountId", vm.FromBankAccountId);
                    cmd.Parameters.AddWithValue("@ToBankAccountId", vm.ToBankAccountId);
                    cmd.Parameters.AddWithValue("@ChequeBankName", vm.ChequeBankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@ChequeDate", vm.ChequeDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@TotalDepositAmount", vm.TotalDepositAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference", vm.Reference ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@IsCash", vm.IsCash);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

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
        public async Task<ResultVM> Update(WithdrawalVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE Withdrawals
SET 
    ChequeNo = @ChequeNo,
    ChequeDate = @ChequeDate,
    TransactionDate = @TransactionDate,
    IsCash = @IsCash,
    FromBankAccountId = @FromBankAccountId,
    ToBankAccountId = @ToBankAccountId,
    ChequeBankName = @ChequeBankName,
    TotalDepositAmount = @TotalDepositAmount,
    Comments = @Comments,   
    Reference = @Reference,   
    IsArchive = @IsArchive,
    IsActive = @IsActive,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = GETDATE(),
    CreatedFrom = @CreatedFrom,
    LastUpdateFrom = @LastUpdateFrom

WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Ensure that vm.Code is being passed correctly
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);  // Add this line to pass the @Code parameter

                    cmd.Parameters.AddWithValue("@ChequeNo", vm.ChequeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FromBankAccountId", vm.FromBankAccountId);
                    cmd.Parameters.AddWithValue("@ToBankAccountId", vm.ToBankAccountId);
                    cmd.Parameters.AddWithValue("@ChequeBankName", vm.ChequeBankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@ChequeDate", vm.ChequeDate);
                    cmd.Parameters.AddWithValue("@TotalDepositAmount", vm.TotalDepositAmount);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference", vm.Reference ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@IsCash", vm.IsCash);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "Unknown");
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
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

                string query = $" UPDATE Withdrawals SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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

                string query = @"
SELECT
    ISNULL(M.Id, 0) AS Id,
	ISNULL(M.FromBankAccountId, 0) AS FromBankAccountId,
	ISNULL(M.ToBankAccountId, 0) AS ToBankAccountId,
    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
	ISNULL(M.TotalDepositAmount, 0) AS TotalDepositAmount,
    ISNULL(M.ChequeNo, '') AS ChequeNo,
    ISNULL(M.Comments, '') AS Comments,
	ISNULL(M.Reference, '') AS Reference,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsCash, 0) AS IsCash,   
	ISNULL(M.IsActive, 0) AS IsActive,   
	ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM Withdrawals M
WHERE 1 = 1
 ";


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions
                // 

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<WithdrawalVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new WithdrawalVM
                    {
                        Id = row.Field<int>("Id"),
                        FromBankAccountId = row.Field<int>("FromBankAccountId"),
                        ToBankAccountId = row.Field<int>("ToBankAccountId"),
                        ChequeBankName = row.Field<string>("ChequeBankName"),
                        TransactionDate = row.Field<string>("TransactionDate"),
                        ChequeDate = row.Field<string>("ChequeDate"),
                        TotalDepositAmount = row.Field<decimal>("TotalDepositAmount"),
                        ChequeNo = row.Field<string>("ChequeNo"),
                        Comments = row.Field<string>("Comments"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        IsCash = row.Field<bool>("IsCash"),
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
	ISNULL(M.FromBankAccountId, 0) AS FromBankAccountId,
	ISNULL(M.ToBankAccountId, 0) AS ToBankAccountId,
    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
	ISNULL(M.TotalDepositAmount, 0) AS TotalDepositAmount,
    ISNULL(M.ChequeNo, '') AS ChequeNo,
    ISNULL(M.Comments, '') AS Comments,
	ISNULL(M.Reference, '') AS Reference,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsCash, 0) AS IsCash,   
	ISNULL(M.IsActive, 0) AS IsActive,   
	ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM Withdrawals M
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
FROM Withdrawals
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

                var data = new GridEntity<WithdrawalVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT M.Id) AS totalcount
FROM Withdrawals M
LEFT OUTER JOIN BankAccounts o on M.FromBankAccountId =o.Id
LEFT OUTER JOIN BankAccounts p on M.ToBankAccountId =p.Id

WHERE M.IsArchive != 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<WithdrawalVM>.FilterCondition(options.filter) + ")" : "");

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
	ISNULL(M.Code, '') AS Code,
    ISNULL(M.FromBankAccountId, 0) AS FromBankAccountId,
	ISNULL(o.AccountNo, '') AS AccountNo,
	ISNULL(M.ToBankAccountId, 0) AS ToBankAccountId,
	ISNULL(p.AccountName, '') AS AccountName,
    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
	ISNULL(M.TotalDepositAmount, 0) AS TotalDepositAmount,
    ISNULL(M.ChequeNo, '') AS ChequeNo,
    ISNULL(M.Comments, '') AS Comments,
	ISNULL(M.Reference, '') AS Reference,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsCash, 0) AS IsCash,   
	ISNULL(M.IsActive, 0) AS IsActive,   
	ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM Withdrawals M
LEFT OUTER JOIN BankAccounts o on M.FromBankAccountId =o.Id
LEFT OUTER JOIN BankAccounts p on M.ToBankAccountId =p.Id

WHERE M.IsArchive != 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<WithdrawalVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<WithdrawalVM>.GetGridData_CMD(options, sqlQuery, "M.Id");
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
