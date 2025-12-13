using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;

namespace ShampanPOS.Repository
{
    
    public class CampaignDetailByQuantityRepository : CommonRepository
    {
        public async Task<ResultVM> Insert(CampaignDetailByQuantityVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO CampaignDetailByQuantities
(CampaignId, BranchId, CustomerId, ProductId, FromQuantity, FreeProductId, FreeQuantity, CampaignStartDate, CampaignEndDate,CampaignEntryDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @ProductId, @FromQuantity, @FreeProductId, @FreeQuantity, @CampaignStartDate, @CampaignEndDate,@CampaignEntryDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", campaignDetail.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", campaignDetail.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", campaignDetail.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", campaignDetail.ProductId);
                    cmd.Parameters.AddWithValue("@FromQuantity", campaignDetail.FromQuantity);
                    //cmd.Parameters.AddWithValue("@FreeProductId", campaignDetail.FreeProductrId);
                    cmd.Parameters.AddWithValue("@FreeQuantity", campaignDetail.FreeQuantity);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", campaignDetail.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", campaignDetail.CampaignEndDate);
                    cmd.Parameters.AddWithValue("@CampaignEntryDate", campaignDetail.CampaignEntryDate);

                    object newId = cmd.ExecuteScalar();
                    campaignDetail.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = campaignDetail;
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
        public async Task<ResultVM> Update(CampaignDetailByQuantityVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = campaignDetail.Id.ToString(), DataVM = campaignDetail };

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
UPDATE CampaignDetailByQuantities
SET 
    CampaignId = @CampaignId,
    BranchId = @BranchId,
    CustomerId = @CustomerId,
    ProductId = @ProductId,
    FromQuantity = @FromQuantity,
    FreeProductId = @FreeProductId,
    FreeQuantity = @FreeQuantity,
    CampaignStartDate = @CampaignStartDate,
    CampaignEndDate = @CampaignEndDate
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", campaignDetail.Id);
                    cmd.Parameters.AddWithValue("@CampaignId", campaignDetail.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", campaignDetail.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", campaignDetail.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", campaignDetail.ProductId);
                    cmd.Parameters.AddWithValue("@FromQuantity", campaignDetail.FromQuantity);
                    //cmd.Parameters.AddWithValue("@FreeProductId", campaignDetail.FreeProductrId);
                    cmd.Parameters.AddWithValue("@FreeQuantity", campaignDetail.FreeQuantity);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", campaignDetail.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", campaignDetail.CampaignEndDate);

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

                string query = "UPDATE CampaignDetailByInvoiceValue SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Ids", string.Join(",", IDs));

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
    ISNULL(M.CampaignId, 0) AS CampaignId,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(M.FromQuantity, 0.0) AS FromQuantity,
    ISNULL(M.FreeProductId, 0) AS FreeProductId,
    ISNULL(M.FreeQuantity, 0.0) AS FreeQuantity,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByQuantities M
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

                var model = new List<CampaignDetailByQuantityVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CampaignDetailByQuantityVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        CampaignId = Convert.ToInt32(row["CampaignId"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        FromQuantity = Convert.ToDecimal(row["FromQuantity"]),
                        //FreeProductrId = Convert.ToInt32(row["FreeProductId"]),
                        //FreeQuantity = Convert.ToDecimal(row["FreeQuantity"]),
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
    ISNULL(M.FromQuantity, 0.0) AS FromQuantity,
    ISNULL(M.FreeProductrId, 0) AS FreeProductrId,
    ISNULL(M.FreeQuantity, 0.0) AS FreeQuantity,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByQuantities M
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
        FROM CampaignDetailByQuantities
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
