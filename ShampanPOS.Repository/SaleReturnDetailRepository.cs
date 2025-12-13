using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;

namespace ShampanPOS.Repository
{
    using ShampanPOS.ViewModel.Utility;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class SaleReturnDetailRepository : CommonRepository    
    {
        // Insert Method
        public async Task<ResultVM> Insert(SaleReturnDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO SaleReturnDetails 
        (
            SaleReturnId, SaleId, SaleDetailId, BranchId, Line, ProductId, Quantity, UnitRate, 
            SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, 
            UOMConversion, TransactionType, IsPost, ReasonOfReturn, Comments, CreatedBy, CreatedOn
        )
        VALUES 
        (
            @SaleReturnId, @SaleId, @SaleDetailId, @BranchId, @Line, @ProductId, @Quantity, @UnitRate, 
            @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, 
            @UOMConversion, @TransactionType, @IsPost, @ReasonOfReturn, @Comments, @CreatedBy, @CreatedOn
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleReturnId", vm.SaleReturnId);
                    cmd.Parameters.AddWithValue("@SaleId", vm.SaleId);
                    cmd.Parameters.AddWithValue("@SaleDetailId", vm.SaleDetailId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@Line", vm.Line ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", vm.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", vm.LineTotal);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId);
                    cmd.Parameters.AddWithValue("@UOMFromId", vm.UOMFromId);
                    cmd.Parameters.AddWithValue("@UOMConversion", vm.UOMConversion);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReasonOfReturn", vm.ReasonOfReturn);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
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
        public async Task<ResultVM> Update(SaleReturnDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE SaleReturnDetails 
        SET 
            SaleReturnId=@SaleReturnId, SaleId=@SaleId, SaleDetailId=@SaleDetailId, BranchId=@BranchId, 
            Line=@Line, ProductId=@ProductId, Quantity=@Quantity, UnitRate=@UnitRate, SubTotal=@SubTotal, 
            SD=@SD, SDAmount=@SDAmount, VATRate=@VATRate, VATAmount=@VATAmount, LineTotal=@LineTotal, 
            UOMId=@UOMId, UOMFromId=@UOMFromId, UOMConversion=@UOMConversion, TransactionType=@TransactionType, 
            IsPost=@IsPost, ReasonOfReturn=@ReasonOfReturn, Comments=@Comments, LastModifiedBy=@LastModifiedBy, 
            LastModifiedOn=GETDATE()
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SaleReturnId", vm.SaleReturnId);
                    cmd.Parameters.AddWithValue("@SaleId", vm.SaleId);
                    cmd.Parameters.AddWithValue("@SaleDetailId", vm.SaleDetailId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@Line", vm.Line ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", vm.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", vm.LineTotal);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId);
                    cmd.Parameters.AddWithValue("@UOMFromId", vm.UOMFromId);
                    cmd.Parameters.AddWithValue("@UOMConversion", vm.UOMConversion);
                    cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReasonOfReturn", vm.ReasonOfReturn);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);

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
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    Id,
    SaleReturnId,
    SaleId,
    SaleDetailId,
    BranchId,
    Line,
    ProductId,
    Quantity,
    UnitRate,
    SubTotal,
    SD,
    SDAmount,
    VATRate,
    VATAmount,
    LineTotal,
    UOMId,
    UOMFromId,
    UOMConversion,
    TransactionType,
    IsPost,
    ReasonOfReturn,
    Comments
FROM SaleReturnDetails
WHERE 1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                using (SqlDataAdapter adapter = CreateAdapter(query, conn, transaction))
                {
                    adapter.Fill(dataTable);
                }

                var modelList = dataTable.AsEnumerable().Select(row => new SaleReturnDetailVM
                {
                    Id = row.Field<int>("Id"),
                    SaleReturnId = row.Field<int>("SaleReturnId"),
                    SaleId = row.Field<int>("SaleId"),
                    SaleDetailId = row.Field<int>("SaleDetailId"),
                    BranchId = row.Field<int>("BranchId"),
                    Line = row.Field<int?>("Line"),
                    ProductId = row.Field<int>("ProductId"),
                    Quantity = row.Field<decimal>("Quantity"),
                    UnitRate = row.Field<decimal>("UnitRate"),
                    SubTotal = row.Field<decimal>("SubTotal"),
                    SD = row.Field<decimal>("SD"),
                    SDAmount = row.Field<decimal>("SDAmount"),
                    VATRate = row.Field<decimal>("VATRate"),
                    VATAmount = row.Field<decimal>("VATAmount"),
                    LineTotal = row.Field<decimal>("LineTotal"),
                    UOMId = row.Field<int>("UOMId"),
                    UOMFromId = row.Field<int>("UOMFromId"),
                    UOMConversion = row.Field<decimal>("UOMConversion"),
                    TransactionType = row.Field<string>("TransactionType"),
                    IsPost = row.Field<bool?>("IsPost"),
                    ReasonOfReturn = row.Field<string>("ReasonOfReturn"),
                    Comments = row.Field<string>("Comments")
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
    Id,
    SaleReturnId,
    SaleId,
    SaleDetailId,
    BranchId,
    Line,
    ProductId,
    Quantity,
    UnitRate,
    SubTotal,
    SD,
    SDAmount,
    VATRate,
    VATAmount,
    LineTotal,
    UOMId,
    UOMFromId,
    UOMConversion,
    TransactionType,
    IsPost,
    ReasonOfReturn,
    Comments
FROM SaleReturnDetails
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
FROM SaleReturnDetails
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
