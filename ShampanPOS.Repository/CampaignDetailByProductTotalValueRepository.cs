using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    using ShampanPOS.ViewModel.CommonVMs;
    using ShampanPOS.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using ShampanPOS.ViewModel.Utility;

    
        public class CampaignDetailByProductTotalValueRepository : CommonRepository
        {
            // Insert Method
            public async Task<ResultVM> Insert(CampaignDetailByProductTotalValueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO CampaignDetailByProductTotalValues 
(
 CampaignId, BranchId, CustomerId, ProductId, FromAmount, ToAmount, DiscountRateBasedOnTotalPrice, 
 CampaignStartDate, CampaignEndDate,CampaignEntryDate, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
)
VALUES 
(
 @CampaignId, @BranchId, @CustomerId, @ProductId, @FromAmount, @ToAmount, @DiscountRateBasedOnTotalPrice, 
 @CampaignStartDate, @CampaignEndDate,@CampaignEntryDate, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom
);
SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CampaignId", vm.CampaignId);
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                        cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                        cmd.Parameters.AddWithValue("@FromAmount", vm.FromAmount);
                        cmd.Parameters.AddWithValue("@ToAmount", vm.ToAmount);
                        cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", vm.DiscountRateBasedOnTotalPrice);
                        cmd.Parameters.AddWithValue("@CampaignStartDate", vm.CampaignStartDate);
                        cmd.Parameters.AddWithValue("@CampaignEndDate", vm.CampaignEndDate);
                        cmd.Parameters.AddWithValue("@CampaignEntryDate", vm.CampaignEntryDate);
                        cmd.Parameters.AddWithValue("@IsArchive", 0);
                        cmd.Parameters.AddWithValue("@IsActive", 1);
                        cmd.Parameters.AddWithValue("@CreatedBy", "System"); // Replace with actual user
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedFrom", "System"); // Replace with actual source

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
            public async Task<ResultVM> Update(CampaignDetailByProductTotalValueVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CampaignDetailByProductTotalValues 
SET 
 CampaignId=@CampaignId, BranchId=@BranchId, CustomerId=@CustomerId, ProductId=@ProductId, 
 FromAmount=@FromAmount, ToAmount=@ToAmount, DiscountRateBasedOnTotalPrice=@DiscountRateBasedOnTotalPrice, 
 CampaignStartDate=@CampaignStartDate, CampaignEndDate=@CampaignEndDate, 
 IsArchive=@IsArchive, IsActive=@IsActive, LastModifiedBy=@LastModifiedBy, 
 LastModifiedOn=GETDATE(), LastUpdateFrom=@LastUpdateFrom
WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                        cmd.Parameters.AddWithValue("@CampaignId", vm.CampaignId);
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                        cmd.Parameters.AddWithValue("@ProductId", vm.ProductId);
                        cmd.Parameters.AddWithValue("@FromAmount", vm.FromAmount);
                        cmd.Parameters.AddWithValue("@ToAmount", vm.ToAmount);
                        cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", vm.DiscountRateBasedOnTotalPrice);
                        cmd.Parameters.AddWithValue("@CampaignStartDate", vm.CampaignStartDate);
                        cmd.Parameters.AddWithValue("@CampaignEndDate", vm.CampaignEndDate);
                      
                        cmd.Parameters.AddWithValue("@LastModifiedBy", "System"); // Replace with actual user
                        cmd.Parameters.AddWithValue("@LastUpdateFrom", "System"); // Replace with actual source

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
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "", DataVM = null };

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

                // Generate a comma-separated list of IDs
                string idList = string.Join(",", IDs);

                string query = "UPDATE CampaignDetailByProductTotalValue SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
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
    ISNULL(M.CampaignId, 0) AS CampaignId,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(M.FromAmount, 0.0) AS FromAmount,
    ISNULL(M.ToAmount, 0.0) AS ToAmount,
    ISNULL(M.DiscountRateBasedOnTotalPrice, 0.0) AS DiscountRateBasedOnTotalPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByProductTotalValue M
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

                var model = new List<CampaignDetailByInvoiceValueVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CampaignDetailByInvoiceValueVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        CampaignId = Convert.ToInt32(row["CampaignId"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        FromAmount = Convert.ToDecimal(row["FromAmount"]),
                        ToAmount = Convert.ToDecimal(row["ToAmount"]),
                        DiscountRateBasedOnTotalPrice = Convert.ToDecimal(row["DiscountRateBasedOnTotalPrice"]),
                        //CampaignStartDate = Convert.ToDateTime(row["CampaignStartDate"]),
                        //CampaignEndDate = Convert.ToDateTime(row["CampaignEndDate"])
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
    ISNULL(M.CampaignId, 0) AS CampaignId,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(M.FromAmount, 0.0) AS FromAmount,
    ISNULL(M.ToAmount, 0.0) AS ToAmount,
    ISNULL(M.DiscountRateBasedOnTotalPrice, 0.0) AS DiscountRateBasedOnTotalPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByProductTotalValue M
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
        SELECT Id, CampaignId
        FROM CampaignDetailByProductTotalValue
        WHERE FromAmount > 0 AND ToAmount > 0
        ORDER BY CampaignId";

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



      
    

