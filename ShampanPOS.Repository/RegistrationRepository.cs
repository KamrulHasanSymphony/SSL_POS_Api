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
    public class RegistrationRepository : CommonRepository
    {


        // Insert Method
        public async Task<ResultVM> Insert(RegistrationVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO Registration
(
            Code,
            EmailAsLoginId,
            Password,
            FullName,
            PhoneNumber,
            CompanyName,
            CompanyAddress,
            CreatedBy,
            CreatedOn,
            CreatedFrom
)
VALUES
(
            @Code,
            @EmailAsLoginId,
            @Password,
            @FullName,
            @PhoneNumber,
            @CompanyName,
            @CompanyAddress,
            @CreatedBy,
            @CreatedOn,
            @CreatedFrom
);

SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@RoleId", 1008);
                    cmd.Parameters.AddWithValue("@EmailAsLoginId", vm.EmailAsLoginId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", vm.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FullName", vm.FullName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyAddress", vm.CompanyAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy??"System");
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

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
        public async Task<ResultVM> Update(RegistrationVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE Registration
        SET
            Code = @Code,
            EmailAsLoginId = @EmailAsLoginId,
            Password = @Password,
            FullName = @FullName,
            PhoneNumber = @PhoneNumber,
            CompanyName = @CompanyName,
            CompanyAddress = @CompanyAddress,
            LastModifiedBy = @LastModifiedBy,
            LastModifiedOn = GETDATE(),
            LastUpdateFrom = @LastUpdateFrom
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    //cmd.Parameters.AddWithValue("@Id", vm.Id);

                    //cmd.Parameters.AddWithValue("@UserName", vm.UserName ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@FullName", vm.FullName ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@Email", vm.EmailAsLoginId ?? (object)DBNull.Value);

                    //cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CompanyLegalName", vm.CompanyLegalName ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CompanyAddress", vm.CompanyAddress ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);

                    //cmd.Parameters.AddWithValue("@LastUpdateBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);


                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@RoleId", 1008);
                    cmd.Parameters.AddWithValue("@EmailAsLoginId", vm.EmailAsLoginId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", vm.Password ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FullName", vm.FullName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyName", vm.CompanyName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyAddress", vm.CompanyAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

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

                string query = $" UPDATE Registration SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(R.Id,0) AS Id,
    ISNULL(R.Code,'') AS Code,

    ISNULL(R.EmailAsLoginId,'') AS EmailAsLoginId,
    ISNULL(R.Password,'') AS Password,

    ISNULL(R.FullName,'') AS FullName,
    ISNULL(R.PhoneNumber,'') AS PhoneNumber,

    ISNULL(R.CompanyName,'') AS CompanyName,
    ISNULL(R.CompanyAddress,'') AS CompanyAddress,

    ISNULL(R.CreatedBy,'') AS CreatedBy,
    FORMAT(ISNULL(R.CreatedOn,'1900-01-01'),'yyyy-MM-dd') AS CreatedOn,

    ISNULL(R.LastModifiedBy,'') AS LastModifiedBy,
    FORMAT(ISNULL(R.LastModifiedOn,'1900-01-01'),'yyyy-MM-dd') AS LastModifiedOn,

    ISNULL(R.CreatedFrom,'') AS CreatedFrom,
    ISNULL(R.LastUpdateFrom,'') AS LastUpdateFrom

FROM Registration R
LEFT JOIN UserInformations AR ON AR.Id = R.RoleId

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

                var model = new List<RegistrationVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new RegistrationVM
                    {
                        Id = row.Field<int>("Id"),

                        Code = row.Field<string>("Code"),
                        EmailAsLoginId = row.Field<string>("EmailAsLoginId"),
                        Password = row.Field<string>("Password"),


                        FullName = row.Field<string>("FullName"),
                        PhoneNumber = row.Field<string>("PhoneNumber"),
                        //EmailAsLoginId = row.Field<string>("Email"),

                        CompanyName = row.Field<string>("CompanyName"),
                        CompanyAddress = row.Field<string>("CompanyAddress"),

                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),

                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string>("LastModifiedOn"),

                        CreatedFrom = row.Field<string>("CreatedFrom"),
                        LastUpdateFrom = row.Field<string>("LastUpdateFrom")
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

                var data = new GridEntity<RegistrationVM>();

                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM Registration H
    LEFT JOIN UserInformations R ON R.Id = H.RoleId

    WHERE 1 = 1";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<RegistrationVM>.FilterCondition(options.filter) + ")" : "");

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
        

    ISNULL(H.Id,0) AS Id,
    ISNULL(H.UserName,'') AS UserName,

    ISNULL(H.RoleId,0) AS RoleId,

    ISNULL(H.FullName,'') AS FullName,
    ISNULL(H.PhoneNumber,'') AS PhoneNumber,
    ISNULL(H.Email,'') AS Email,

    ISNULL(H.CompanyName,'') AS CompanyName,
    ISNULL(H.CompanyLegalName,'') AS CompanyLegalName,
    ISNULL(H.CompanyAddress,'') AS CompanyAddress,
    ISNULL(H.BIN,'') AS BIN,

    ISNULL(H.CreatedBy,'') AS CreatedBy,
    ISNULL(H.LastUpdateBy,'') AS LastUpdateBy,

    ISNULL(FORMAT(H.CreatedAt,'yyyy-MM-dd HH:mm'),'') AS CreatedOn,
    ISNULL(FORMAT(H.LastUpdateAt,'yyyy-MM-dd HH:mm'),'') AS LastModifiedOn,

    ISNULL(H.CreatedFrom,'') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom,'') AS LastUpdateFrom

    FROM Registration H
    LEFT JOIN UserInformations R ON R.Id = H.RoleId

    WHERE 1 = 1
";

                sqlQuery = sqlQuery + (options.filter.Filters.Count > 0 ?
                    " AND (" + GridQueryBuilder<TableInfoVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";



                data = KendoGrid<RegistrationVM>.GetGridData_CMD(options, sqlQuery, "H.Id");
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
