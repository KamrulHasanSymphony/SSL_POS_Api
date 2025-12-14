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

    public class PurchaseDetailRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(PurchaseDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO PurchaseDetails
        (
            PurchaseId, PurchaseOrderId, PurchaseOrderDetailId, BranchId, Line, ProductId, 
            Quantity, UnitPrice, SubTotal, SD, SDAmount, VATRate, VATAmount, OthersAmount, 
            LineTotal, UOMId, UOMFromId, UOMConversion, Comments, VATType, TransactionType, 
            IsPost, IsFixedVAT, FixedVATAmount
        )
        VALUES
        (
            @PurchaseId, @PurchaseOrderId, @PurchaseOrderDetailId, @BranchId, @Line, @ProductId,
            @Quantity, @UnitPrice, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @OthersAmount, 
            @LineTotal, @UOMId, @UOMFromId, @UOMConversion, @Comments, @VATType, @TransactionType, 
            @IsPost, @IsFixedVAT, @FixedVATAmount
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@PurchaseId", vm.PurchaseId);
                    //cmd.Parameters.AddWithValue("@PurchaseOrderId", vm.PurchaseOrderId);
                    cmd.Parameters.AddWithValue("@PurchaseOrderDetailId", vm.PurchaseOrderDetailId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@Line", vm.Line);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", vm.UnitPrice);
                    cmd.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmd.Parameters.AddWithValue("@OthersAmount", vm.OthersAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", vm.LineTotal);
                    //cmd.Parameters.AddWithValue("@UOMId", vm.UOMId);
                    //cmd.Parameters.AddWithValue("@UOMFromId", vm.UOMFromId);
                    //cmd.Parameters.AddWithValue("@UOMConversion", vm.UOMConversion);
                    //cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@VATType", vm.VATType ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    //cmd.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT);
                    //cmd.Parameters.AddWithValue("@FixedVATAmount", vm.FixedVATAmount);

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
        public async Task<ResultVM> Update(PurchaseDetailVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE PurchaseDetails
        SET 
            PurchaseId = @PurchaseId, PurchaseOrderId = @PurchaseOrderId, PurchaseOrderDetailId = @PurchaseOrderDetailId, 
            BranchId = @BranchId, Line = @Line, ProductId = @ProductId, Quantity = @Quantity, UnitPrice = @UnitPrice, 
            SubTotal = @SubTotal, SD = @SD, SDAmount = @SDAmount, VATRate = @VATRate, VATAmount = @VATAmount, 
            OthersAmount = @OthersAmount, LineTotal = @LineTotal, UOMId = @UOMId, UOMFromId = @UOMFromId, 
            UOMConversion = @UOMConversion, Comments = @Comments, VATType = @VATType, TransactionType = @TransactionType, 
            IsPost = @IsPost, IsFixedVAT = @IsFixedVAT, FixedVATAmount = @FixedVATAmount
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@PurchaseId", vm.PurchaseId);
                    //cmd.Parameters.AddWithValue("@PurchaseOrderId", vm.PurchaseOrderId);
                    cmd.Parameters.AddWithValue("@PurchaseOrderDetailId", vm.PurchaseOrderDetailId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@Line", vm.Line);
                    cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", vm.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", vm.UnitPrice);
                    cmd.Parameters.AddWithValue("@SubTotal", vm.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", vm.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", vm.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", vm.VATAmount);
                    cmd.Parameters.AddWithValue("@OthersAmount", vm.OthersAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", vm.LineTotal);
                    //cmd.Parameters.AddWithValue("@UOMId", vm.UOMId);
                    //cmd.Parameters.AddWithValue("@UOMFromId", vm.UOMFromId);
                    //cmd.Parameters.AddWithValue("@UOMConversion", vm.UOMConversion);
                    //cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@VATType", vm.VATType ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@TransactionType", vm.TransactionType ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    //cmd.Parameters.AddWithValue("@IsFixedVAT", vm.IsFixedVAT);
                    //cmd.Parameters.AddWithValue("@FixedVATAmount", vm.FixedVATAmount);

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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = IDs.ToString(), DataVM = null };

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
                    cmd.Parameters.AddWithValue("@Id", IDs);

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
    Id, PurchaseId, PurchaseOrderId, PurchaseOrderDetailId, BranchId, Line, ProductId, 
    Quantity, UnitPrice, SubTotal, SD, SDAmount, VATRate, VATAmount, OthersAmount, 
    LineTotal, UOMId, UOMFromId, UOMConversion, Comments, VATType, TransactionType, 
    IsPost, IsFixedVAT, FixedVATAmount
FROM PurchaseDetails WHERE 1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                using (SqlDataAdapter adapter = CreateAdapter(query, conn, transaction))
                {
                    adapter.Fill(dataTable);
                }

                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseDetailVM
                {
                    Id = row.Field<int>("Id"),
                    PurchaseId = row.Field<int>("PurchaseId"),
                    //PurchaseOrderId = row.Field<int>("PurchaseOrderId"),
                    PurchaseOrderDetailId = row.Field<int>("PurchaseOrderDetailId"),
                    BranchId = row.Field<int>("BranchId"),
                    Line = row.Field<int>("Line"),
                    ProductId = row.Field<int>("ProductId"),
                    Quantity = row.Field<decimal>("Quantity"),
                    UnitPrice = row.Field<decimal>("UnitPrice"),
                    SubTotal = row.Field<decimal>("SubTotal"),
                    SD = row.Field<decimal>("SD"),
                    SDAmount = row.Field<decimal>("SDAmount"),
                    VATRate = row.Field<decimal>("VATRate"),
                    VATAmount = row.Field<decimal>("VATAmount"),
                    OthersAmount = row.Field<decimal>("OthersAmount"),
                    LineTotal = row.Field<decimal>("LineTotal"),
                    //UOMId = row.Field<int>("UOMId"),
                    //UOMFromId = row.Field<int>("UOMFromId"),
                    //UOMConversion = row.Field<decimal>("UOMConversion"),
                    //Comments = row.Field<string>("Comments"),
                    //VATType = row.Field<string>("VATType"),
                    //TransactionType = row.Field<string>("TransactionType"),
                    //IsPost = row.Field<bool>("IsPost"),
                    //IsFixedVAT = row.Field<bool>("IsFixedVAT"),
                    //FixedVATAmount = row.Field<decimal>("FixedVATAmount")
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
    Id, PurchaseId, PurchaseOrderId, PurchaseOrderDetailId, BranchId, Line, ProductId, 
    Quantity, UnitPrice, SubTotal, SD, SDAmount, VATRate, VATAmount, OthersAmount, 
    LineTotal, UOMId, UOMFromId, UOMConversion, Comments, VATType, TransactionType, 
    IsPost, IsFixedVAT, FixedVATAmount
FROM PurchaseDetails WHERE 1 = 1";


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
FROM PurchaseDetails
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
