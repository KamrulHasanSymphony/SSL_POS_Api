using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class UserInfoRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(UserInfoVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO UserInformations
                (
                    UserId,
                    UserName,
                    FullName,
                    PhoneNumber,
                    Email,
                    BranchId,
                    CompanyId,
                    CreatedBy,
                    CreatedAt,
                    CreatedFrom
                )
                VALUES
                (
                    @UserId,
                    @UserName,
                    @FullName,
                    @PhoneNumber,
                    @Email,
                    @BranchId,
                    @CompanyId,
                    @CreatedBy,
                    @CreatedAt,
                    @CreatedFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserName", vm.UserName);
                    cmd.Parameters.AddWithValue("@FullName", vm.FullName);

                    cmd.Parameters.AddWithValue("@PhoneNumber", vm.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

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
    }
}
