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
    public class TableReservationRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(TableReservationVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO TableReservations
(
    TableId,
    CustomerName,
    PhoneNumber,
    ReservationTime,
    Status,
    CreatedBy,
    CreatedOn,
    CreatedFrom,
    BranchId,
    CompanyId
)
VALUES
(
    @TableId,
    @CustomerName,
    @PhoneNumber,
    @ReservationTime,
    @Status,
    @CreatedBy,
    @CreatedOn,
    @CreatedFrom,
    @BranchId,
    @CompanyId
);

SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@TableId", vm.TableId);
                    cmd.Parameters.AddWithValue("@CustomerName", vm.CustomerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@ReservationTime", vm.ReservationTime);
                    cmd.Parameters.AddWithValue("@Status", vm.Status ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);

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
        public async Task<ResultVM> Update(TableReservationVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE TableReservations
SET 
    TableId = @TableId,
    CustomerName = @CustomerName,
    PhoneNumber = @PhoneNumber,
    ReservationTime = @ReservationTime,
    Status = @Status,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = GETDATE(),
    LastUpdateFrom = @LastUpdateFrom,
    BranchId = @BranchId,
    CompanyId = @CompanyId
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);

                    cmd.Parameters.AddWithValue("@TableId", vm.TableId);
                    cmd.Parameters.AddWithValue("@CustomerName", vm.CustomerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@ReservationTime", vm.ReservationTime);
                    cmd.Parameters.AddWithValue("@Status", vm.Status ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);

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

                string query = $" UPDATE TableReservations SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.TableId, 0) AS TableId,
    ISNULL(M.CustomerName, '') AS CustomerName,
    ISNULL(M.PhoneNumber, '') AS PhoneNumber,

    ISNULL(M.ReservationTime, '1900-01-01') AS ReservationTime,

    ISNULL(M.Status, '') AS Status,
	ISNULL(T.Name, '') AS StatusName,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    CONVERT(VARCHAR(16), ISNULL(M.CreatedOn, '1900-01-01'), 120) AS CreatedOn,

    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    CONVERT(VARCHAR(16), ISNULL(M.LastModifiedOn, '1900-01-01'), 120) AS LastModifiedOn,

    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,

    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CompanyId, 0) AS CompanyId

FROM TableReservations M
LEFT OUTER JOIN EnumTypes T ON M.Status = T.Id
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

                var model = new List<TableReservationVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new TableReservationVM
                    {
                        Id = row.Field<int?>("Id") ?? 0,

                        TableId = row.Field<int?>("TableId") ?? 0,
                        CustomerName = row.Field<string>("CustomerName") ?? "",
                        StatusName = row.Field<string>("StatusName") ?? "",
                        PhoneNumber = row.Field<string>("PhoneNumber") ?? "",

                        ReservationTime = row.Field<DateTime>("ReservationTime"),
                        Status = row.Field<string>("Status") ?? "",

                        CreatedBy = row.Field<string>("CreatedBy") ?? "",
                        CreatedOn = row.Field<string>("CreatedOn") ?? "",

                        LastModifiedBy = row.Field<string>("LastModifiedBy") ?? "",
                        LastModifiedOn = row.Field<string>("LastModifiedOn") ?? "",

                        CreatedFrom = row.Field<string>("CreatedFrom") ?? "",
                        LastUpdateFrom = row.Field<string>("LastUpdateFrom") ?? "",

                        BranchId = row.Field<int?>("BranchId"),
                        CompanyId = row.Field<int?>("CompanyId"),
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

                var data = new GridEntity<TableReservationVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
FROM TableReservations H
LEFT OUTER JOIN TableInfo T ON H.TableId = T.Id
LEFT OUTER JOIN EnumTypes K ON H.Status = K.Id

WHERE 1 = 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<TableReservationVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " +
                                        (options.sort.Count > 0 ?
                                            options.sort[0].field + " " + options.sort[0].dir :
                                            "H.Id DESC ") + @") AS rowindex,
        
    ISNULL(H.Id, 0) AS Id,
    ISNULL(H.TableId, 0) AS TableId,
    ISNULL(T.TableNumber, 0) AS TableNumber,
    ISNULL(H.CustomerName, '') AS CustomerName,
    ISNULL(H.PhoneNumber, '') AS PhoneNumber,

    ISNULL(H.ReservationTime, '1900-01-01') AS ReservationTime,

    ISNULL(H.Status, '') AS Status,
		ISNULL(K.Name, '') AS StatusName,

    ISNULL(H.CreatedBy, '') AS CreatedBy,
    CONVERT(VARCHAR(16), ISNULL(H.CreatedOn, '1900-01-01'), 120) AS CreatedOn,

    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    CONVERT(VARCHAR(16), ISNULL(H.LastModifiedOn, '1900-01-01'), 120) AS LastModifiedOn,

    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,

    ISNULL(H.BranchId, 0) AS BranchId,
    ISNULL(H.CompanyId, 0) AS CompanyId

FROM TableReservations H
LEFT OUTER JOIN TableInfo T ON H.TableId = T.Id
LEFT OUTER JOIN EnumTypes K ON H.Status = K.Id

WHERE 1 = 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<TableReservationVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<TableReservationVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
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
