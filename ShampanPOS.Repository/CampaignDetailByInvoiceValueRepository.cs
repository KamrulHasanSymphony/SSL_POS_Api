using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;

namespace ShampanPOS.Repository
{

    public class CampaignDetailByInvoiceValueRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CampaignDetailByInvoiceValueVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO CampaignDetailByInvoiceValues
(CampaignId, BranchId, CustomerId, FromAmount, ToAmount, DiscountRateBasedOnTotalPrice, CampaignStartDate, CampaignEndDate,CampaignEntryDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @FromAmount, @ToAmount, @DiscountRateBasedOnTotalPrice, @CampaignStartDate, @CampaignEndDate,@CampaignEntryDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", campaignDetail.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", campaignDetail.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", campaignDetail.CustomerId);
                    cmd.Parameters.AddWithValue("@FromAmount", campaignDetail.FromAmount);
                    cmd.Parameters.AddWithValue("@ToAmount", campaignDetail.ToAmount);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", campaignDetail.DiscountRateBasedOnTotalPrice);
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
        public async Task<ResultVM> Update(CampaignDetailByInvoiceValueVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CampaignDetailByInvoiceValues
SET 
    CampaignId = @CampaignId,
    BranchId = @BranchId,
    CustomerId = @CustomerId,
    FromAmount = @FromAmount,
    ToAmount = @ToAmount,
    DiscountRateBasedOnTotalPrice = @DiscountRateBasedOnTotalPrice,
    CampaignStartDate = @CampaignStartDate,
    CampaignEndDate = @CampaignEndDate
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", campaignDetail.Id);
                    cmd.Parameters.AddWithValue("@CampaignId", campaignDetail.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", campaignDetail.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", campaignDetail.CustomerId);
                    cmd.Parameters.AddWithValue("@FromAmount", campaignDetail.FromAmount);
                    cmd.Parameters.AddWithValue("@ToAmount", campaignDetail.ToAmount);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", campaignDetail.DiscountRateBasedOnTotalPrice);
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
    ISNULL(M.FromAmount, 0.0) AS FromAmount,
    ISNULL(M.ToAmount, 0.0) AS ToAmount,
    ISNULL(M.DiscountRateBasedOnTotalPrice, 0.0) AS DiscountRateBasedOnTotalPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate,
    ISNULL(M.CampaignEntryDate, '1900-01-01') AS CampaignEntryDate
FROM CampaignDetailByInvoiceValues M
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
                        CampaignEntryDate = row["CampaignEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["CampaignEntryDate"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00"
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
    ISNULL(M.FromAmount, 0.0) AS FromAmount,
    ISNULL(M.ToAmount, 0.0) AS ToAmount,
    ISNULL(M.DiscountRateBasedOnTotalPrice, 0.0) AS DiscountRateBasedOnTotalPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate,
    ISNULL(M.CampaignEntryDate, '1900-01-01') AS CampaignEntryDate
FROM CampaignDetailByInvoiceValues M
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
        FROM CampaignDetailByInvoiceValues
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



        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(M.Code, 0) AS CampaignCode,
ISNULL(M.Code, '') AS Code,
ISNULL(M.BranchId, 0) AS BranchId,
ISNULL(TRIM(B.Name), '') AS BranchName,
ISNULL(TRIM(B.BanglaName), '') AS BranchBanglaName,
ISNULL(TRIM(B.Code), '') AS BranchCode,
ISNULL(TRIM(B.Address), '') AS BranchAddress,
ISNULL(TRIM(B.TelephoneNo), '') AS TelephoneNo,
ISNULL(FORMAT(M.CampaignStartDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignStartDate,
ISNULL(FORMAT(M.CampaignEndDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignEndDate,
ISNULL(M.Description, '') AS Description,
ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,

ISNULL(D.DiscountRateBasedOnTotalPrice,0) AS DiscountRateBasedOnTotalPrice



 
FROM CampaignDetailByInvoiceValues D 
LEFT OUTER JOIN Campaigns M ON ISNULL(M.Id,0) = D.CampaignId
LEFT OUTER JOIN BranchProfiles B ON M.BranchId = B.Id
WHERE  1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
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
