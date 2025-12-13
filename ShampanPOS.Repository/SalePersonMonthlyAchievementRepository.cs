using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class SalePersonMonthlyAchievementRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(SalePersonMonthlyAchievementVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO SalePersonMonthlyAchievements
                (
                BranchId, 
                SalePersonId, 
                MonthlySales, 
                MonthlyTarget, 
                SelfSaleCommissionRate, 
                OtherSaleCommissionRate, 
                Year, 
                MonthId, 
                MonthStart, 
                MonthEnd, 
                BonusAmount,
                OtherCommissionBonus,
                TotalBonus,
                CreatedBy, 
                CreatedOn, 
                CreatedFrom
                )
                VALUES 
                (
                @BranchId, 
                @SalePersonId, 
                @MonthlySales, 
                @MonthlyTarget, 
                @SelfSaleCommissionRate, 
                @OtherSaleCommissionRate, 
                @Year, 
                @MonthId, 
                @MonthStart, 
                @MonthEnd, 
                @BonusAmount,
                @OtherCommissionBonus,
                @TotalBonus,
                @CreatedBy, 
                @CreatedOn, 
                @CreatedFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@MonthlySales", vm.MonthlySales);
                    cmd.Parameters.AddWithValue("@MonthlyTarget", vm.MonthlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@MonthId", vm.MonthId);
                    cmd.Parameters.AddWithValue("@MonthStart", vm.MonthStart);
                    cmd.Parameters.AddWithValue("@MonthEnd", vm.MonthEnd);
                    cmd.Parameters.AddWithValue("@BonusAmount", vm.BonusAmount);
                    cmd.Parameters.AddWithValue("@OtherCommissionBonus", vm.OtherCommissionBonus);
                    cmd.Parameters.AddWithValue("@TotalBonus", vm.TotalBonus);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "");

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
        public async Task<ResultVM> Update(SalePersonMonthlyAchievementVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    Update SalePersonMonthlyAchievements
                        SET
                        BranchId = @BranchId,
                        SalePersonId = @SalePersonId,
                        MonthlySales = @MonthlySales,
                        MonthlyTarget = @MonthlyTarget,
                        SelfSaleCommissionRate = @SelfSaleCommissionRate,
                        OtherSaleCommissionRate = @OtherSaleCommissionRate,
                        Year = @Year,
                        MonthId = @MonthId,
                        MonthStart = @MonthStart,
                        MonthEnd = @MonthEnd,
                        LastModifiedBy = @LastModifiedBy,
                        LastModifiedOn = @LastModifiedOn,
                        LastUpdateFrom = @LastUpdateFrom
                    WHERE Id = @Id;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@MonthlySales", vm.MonthlySales);
                    cmd.Parameters.AddWithValue("@MonthlyTarget", vm.MonthlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@MonthId", vm.MonthId);
                    cmd.Parameters.AddWithValue("@MonthStart", vm.MonthStart);
                    cmd.Parameters.AddWithValue("@MonthEnd", vm.MonthEnd);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);  
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);  
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
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

                string query = $" Delete From SalePersonMonthlyAchievements WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

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
                    ISNULL(M.MonthlySales, 0) AS MonthlySales,
                    ISNULL(M.MonthlyTarget, 0) AS MonthlyTarget,
                    ISNULL(M.SelfSaleCommissionRate, 0) AS SelfSaleCommissionRate,
                    ISNULL(M.OtherSaleCommissionRate, 0) AS OtherSaleCommissionRate,
                    ISNULL(M.Year, 0) AS Year,
                    ISNULL(M.MonthId, 0) AS MonthId,
                    ISNULL(M.MonthStart, '1900-01-01') AS MonthStart,
                    ISNULL(M.MonthEnd, '1900-01-01') AS MonthEnd,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
                    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
                    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
                FROM SalePersonMonthlyAchievements M
                WHERE 1 = 1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);
                var model = new List<SalePersonMonthlyAchievementVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SalePersonMonthlyAchievementVM
                    {
                        Id = row.Field<int>("Id"),
                        BranchId = row.Field<int>("BranchId"),
                        SalePersonId = row.Field<int>("SalePersonId"),
                        MonthlySales = row.Field<decimal>("MonthlySales"),
                        MonthlyTarget = row.Field<decimal>("MonthlyTarget"),
                        SelfSaleCommissionRate = row.Field<decimal>("SelfSaleCommissionRate"),
                        OtherSaleCommissionRate = row.Field<decimal>("OtherSaleCommissionRate"),
                        Year = row.Field<int>("Year"),
                        MonthId = row.Field<int>("MonthId"),
                        MonthStart = row.Field<string>("MonthStart"),
                        MonthEnd = row.Field<string>("MonthEnd"),  
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),   
                        CreatedFrom = row.Field<string>("CreatedFrom"),
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
                    ISNULL(M.BranchId, 0) AS BranchId,
                    ISNULL(M.SalePersonId, 0) AS SalePersonId,
                    ISNULL(M.MonthlySales, 0) AS MonthlySales,
                    ISNULL(M.MonthlyTarget, 0) AS MonthlyTarget,
                    ISNULL(M.SelfSaleCommissionRate, 0) AS SelfSaleCommissionRate,
                    ISNULL(M.OtherSaleCommissionRate, 0) AS OtherSaleCommissionRate,
                    ISNULL(M.Year, 0) AS Year,
                    ISNULL(M.MonthId, 0) AS MonthId,
                    ISNULL(M.MonthStart, '1900-01-01') AS MonthStart,
                    ISNULL(M.MonthEnd, '1900-01-01') AS MonthEnd,
                    ISNULL(M.CreatedBy, '') AS CreatedBy,
                    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
                    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
                    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
                FROM SalePersonMonthlyAchievements M
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
FROM SalePersonMonthlyAchievements
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

        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SalePersonMonthlyAchievementVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
             SELECT COUNT(DISTINCT H.Id) AS totalcount
             FROM SalePersonMonthlyAchievements H
        LEFT OUTER JOIN SalesPersons P on H.SalePersonId = P.Id
        Where 1=1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonMonthlyAchievementVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
            ,ISNULL(H.Id, 0) AS Id,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(H.SalePersonId, 0) AS SalePersonId,
            ISNULL(P.Name, 0) AS SalePersonName,
            ISNULL(H.MonthlySales, 0) AS MonthlySales,
            ISNULL(H.MonthlyTarget, 0) AS MonthlyTarget,
            ISNULL(H.SelfSaleCommissionRate, 0) AS SelfSaleCommissionRate,
            ISNULL(H.OtherSaleCommissionRate, 0) AS OtherSaleCommissionRate,
            ISNULL(H.Year, 0) AS Year,
            ISNULL(H.MonthId, 0) AS MonthId,
            ISNULL(H.MonthStart, '1900-01-01') AS MonthStart,
            ISNULL(H.MonthEnd, '1900-01-01') AS MonthEnd,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
        FROM SalePersonMonthlyAchievements H
        LEFT OUTER JOIN SalesPersons P on H.SalePersonId = P.Id
        Where 1=1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonMonthlyAchievementVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SalePersonMonthlyAchievementVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> ProcessDataList(SalePersonMonthlyAchievementDataVM vm, SqlConnection conn, SqlTransaction transaction)
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

                using (SqlCommand cmd = new SqlCommand("SalePersonMonthlyAchievementProcess", conn, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the stored procedure
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@FiscalYear", vm.Year);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonMonthlyAchievementVM
                {
                    BranchId = row.Field<int>("BranchId"),
                    SalePersonId = row.Field<int>("SalePersonId"),
                    MonthlySales = row.Field<decimal>("MonthlySales"),
                    MonthlyTarget = row.Field<decimal>("MonthlyTarget"),
                    SelfSaleCommissionRate = row.Field<decimal>("SelfSaleCommissionRate"),
                    BonusAmount = row.Field<decimal>("BonusAmount"),
                    OtherSaleCommissionRate = row.Field<decimal>("OtherSaleCommissionRate"),
                    OtherCommissionBonus = row.Field<decimal>("OtherCommissionBonus"),
                    TotalBonus = row.Field<decimal>("TotalBonus"),
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
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    await conn.CloseAsync();
                }
            }
        }
    }
}
