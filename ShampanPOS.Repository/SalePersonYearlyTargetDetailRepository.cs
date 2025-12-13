using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;

namespace ShampanPOS.Repository
{
    

    public class SalePersonYearlyTargetDetailRepository : CommonRepository
    {


        // Insert Method
        public async Task<ResultVM> Insert(SalePersonYearlyTargetDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO SalePersonYearlyTargetDetails 
        (
            SalePersonId, BranchId, SalePersonYearlyTargetId, FiscalYearDetailForSaleId, FiscalYearForSaleId, 
            MonthlyTarget, SelfSaleCommissionRate, OtherSaleCommissionRate, Year, MonthId, MonthStart, 
            MonthEnd, CreatedBy, CreatedOn
        )
        VALUES 
        (
            @SalePersonId, @BranchId, @SalePersonYearlyTargetId, @FiscalYearDetailForSaleId, @FiscalYearForSaleId, 
            @MonthlyTarget, @SelfSaleCommissionRate, @OtherSaleCommissionRate, @Year, @MonthId, @MonthStart, 
            @MonthEnd, @CreatedBy, @CreatedOn
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonYearlyTargetId", vm.SalePersonYearlyTargetId);
                    cmd.Parameters.AddWithValue("@FiscalYearDetailForSaleId", vm.FiscalYearDetailForSaleId);
                    cmd.Parameters.AddWithValue("@FiscalYearForSaleId", vm.FiscalYearForSaleId);
                    cmd.Parameters.AddWithValue("@MonthlyTarget", vm.MonthlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@MonthId", vm.MonthId);
                    cmd.Parameters.AddWithValue("@MonthStart", vm.MonthStart);
                    cmd.Parameters.AddWithValue("@MonthEnd", vm.MonthEnd);
                    //cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
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
        public async Task<ResultVM> Update(SalePersonYearlyTargetDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE SalePersonYearlyTargetDetails 
        SET 
            SalePersonId=@SalePersonId, BranchId=@BranchId, SalePersonYearlyTargetId=@SalePersonYearlyTargetId, 
            FiscalYearDetailForSaleId=@FiscalYearDetailForSaleId, FiscalYearForSaleId=@FiscalYearForSaleId, 
            MonthlyTarget=@MonthlyTarget, SelfSaleCommissionRate=@SelfSaleCommissionRate, 
            OtherSaleCommissionRate=@OtherSaleCommissionRate, Year=@Year, MonthId=@MonthId, 
            MonthStart=@MonthStart, MonthEnd=@MonthEnd, LastModifiedBy=@LastModifiedBy, LastModifiedOn=GETDATE() 
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonYearlyTargetId", vm.SalePersonYearlyTargetId);
                    cmd.Parameters.AddWithValue("@FiscalYearDetailForSaleId", vm.FiscalYearDetailForSaleId);
                    cmd.Parameters.AddWithValue("@FiscalYearForSaleId", vm.FiscalYearForSaleId);
                    cmd.Parameters.AddWithValue("@MonthlyTarget", vm.MonthlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@MonthId", vm.MonthId);
                    cmd.Parameters.AddWithValue("@MonthStart", vm.MonthStart);
                    cmd.Parameters.AddWithValue("@MonthEnd", vm.MonthEnd);
                   // cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);

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

                string query = "UPDATE SalePersonYearlyTargetDetails SET IsArchive = 0, IsActiveStatus = 1 WHERE Id IN (@Ids)";

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
    Id, BranchId, SalePersonId, SalePersonYearlyTargetId, FiscalYearDetailForSaleId, 
    FiscalYearForSaleId, MonthlyTarget, SelfSaleCommissionRate, OtherSaleCommissionRate, 
    Year, MonthId, MonthStart, MonthEnd
FROM SalePersonYearlyTargetDetails
WHERE 1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                using (SqlDataAdapter adapter = CreateAdapter(query, conn, transaction))
                {
                    adapter.Fill(dataTable);
                }

                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonYearlyTargetDetailVM
                {
                    Id = row.Field<int>("Id"),
                    BranchId = row.Field<int>("BranchId"),
                    SalePersonId = row.Field<int>("SalePersonId"),
                    SalePersonYearlyTargetId = row.Field<int>("SalePersonYearlyTargetId"),
                    FiscalYearDetailForSaleId = row.Field<int>("FiscalYearDetailForSaleId"),
                    FiscalYearForSaleId = row.Field<int>("FiscalYearForSaleId"),
                    MonthlyTarget = row.Field<decimal>("MonthlyTarget"),
                    SelfSaleCommissionRate = row.Field<decimal>("SelfSaleCommissionRate"),
                    OtherSaleCommissionRate = row.Field<decimal>("OtherSaleCommissionRate"),
                    Year = row.Field<int>("Year"),
                    MonthId = row.Field<int>("MonthId"),
                    MonthStart = row.Field<string>("MonthStart"),
                    MonthEnd = row.Field<string>("MonthEnd")
                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
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
    Id, BranchId, SalePersonId, SalePersonYearlyTargetId, FiscalYearDetailForSaleId, 
    FiscalYearForSaleId, MonthlyTarget, SelfSaleCommissionRate, OtherSaleCommissionRate, 
    Year, MonthId, MonthStart, MonthEnd
FROM SalePersonYearlyTargetDetails
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
FROM SalePersonYearlyTargetDetails
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

    }

}
