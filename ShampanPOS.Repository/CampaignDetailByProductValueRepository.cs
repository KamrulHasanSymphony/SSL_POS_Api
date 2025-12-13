using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;

namespace ShampanPOS.Repository
{
   
    public class CampaignDetailByProductValueRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CampaignDetailByProductValueVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
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
INSERT INTO CampaignDetailByProductValues
(CampaignId, BranchId, CustomerId, ProductId, FromQuantity, ToQuantity, DiscountRateBasedOnUnitPrice, CampaignStartDate, CampaignEndDate,CampaignEntryDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @ProductId, @FromQuantity, @ToQuantity, @DiscountRateBasedOnUnitPrice, @CampaignStartDate, @CampaignEndDate,@CampaignEntryDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", campaignDetail.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", campaignDetail.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", campaignDetail.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", campaignDetail.ProductId);
                    cmd.Parameters.AddWithValue("@FromQuantity", campaignDetail.FromQuantity);
                    cmd.Parameters.AddWithValue("@ToQuantity", campaignDetail.ToQuantity);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnUnitPrice", campaignDetail.DiscountRateBasedOnUnitPrice);
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
        public async Task<ResultVM> Update(CampaignDetailByProductValueVM campaignDetail, SqlConnection conn = null, SqlTransaction transaction = null)
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
UPDATE CampaignDetailByProductValues
SET 
    CampaignId = @CampaignId,
    BranchId = @BranchId,
    CustomerId = @CustomerId,
    ProductId = @ProductId,
    FromQuantity = @FromQuantity,
    ToQuantity = @ToQuantity,
    DiscountRateBasedOnUnitPrice = @DiscountRateBasedOnUnitPrice,
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
                    cmd.Parameters.AddWithValue("@ToQuantity", campaignDetail.ToQuantity);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnUnitPrice", campaignDetail.DiscountRateBasedOnUnitPrice);
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

                string query = "UPDATE CampaignDetailByProductValues SET IsArchive = 1, IsActive = 0 WHERE Id IN (" + string.Join(",", IDs.Select(id => $"'{id}'")) + ")";

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
    ISNULL(M.ProductId, 0) AS ProductId,
    ISNULL(M.FromQuantity, 0.0) AS FromQuantity,
    ISNULL(M.ToQuantity, 0.0) AS ToQuantity,
    ISNULL(M.DiscountRateBasedOnUnitPrice, 0.0) AS DiscountRateBasedOnUnitPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByProductValues M
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

                var model = new List<CampaignDetailByProductValueVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CampaignDetailByProductValueVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        CampaignId = Convert.ToInt32(row["CampaignId"]),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        FromQuantity = Convert.ToDecimal(row["FromQuantity"]),
                        ToQuantity = Convert.ToDecimal(row["ToQuantity"]),
                        DiscountRateBasedOnUnitPrice = Convert.ToDecimal(row["DiscountRateBasedOnUnitPrice"]),
                         CampaignStartDate = row["CampaignStartDate"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CampaignEndDate = row["CampaignEndDate"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
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
    ISNULL(M.ToQuantity, 0.0) AS ToQuantity,
    ISNULL(M.DiscountRateBasedOnUnitPrice, 0.0) AS DiscountRateBasedOnUnitPrice,
    ISNULL(M.CampaignStartDate, '1900-01-01') AS CampaignStartDate,
    ISNULL(M.CampaignEndDate, '1900-01-01') AS CampaignEndDate
FROM CampaignDetailByProductValues M
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
        FROM CampaignDetailByProductValues
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


//        public async Task<ResultVM> GetCampaignDetailByProductValueModalDataList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"
//SELECT 
//ISNULL(P.Id,0)ProductId , 
//ISNULL(P.Name,'') ProductName,
//ISNULL(P.BanglaName,'') BanglaName, 
//ISNULL(P.Code,'') ProductCode, 
//ISNULL(P.HSCodeNo,'') HSCodeNo,
//ISNULL(P.ProductGroupId,0) ProductGroupId,
//ISNULL(PG.Name,'') ProductGroupName,
//ISNULL(P.UOMId,0) UOMId,
//ISNULL(UOM.Name,'') UOMName,
//0 CostPrice , 
//0 SalesPrice , 
//0 PurchasePrice , 
//0 SD , 
//0 VATRate , 

//CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status

//FROM Products P
//LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

//WHERE P.IsActive = 1 
//";

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
//                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
//                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
//                }

//                objComm.Fill(dataTable);

//                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
//                {
//                    ProductId = row.Field<int>("ProductId"),
//                    ProductName = row.Field<string>("ProductName"),
//                    BanglaName = row.Field<string>("BanglaName"),
//                    ProductCode = row.Field<string>("ProductCode"),
//                    HSCodeNo = row.Field<string>("HSCodeNo"),
//                    ProductGroupId = row.Field<int>("ProductGroupId"),
//                    SDRate = row.Field<decimal>("SD"),
//                    VATRate = row.Field<decimal>("VATRate"),
//                    //CostPrice = row.Field<decimal>("CostPrice"),
//                    //SalesPrice = row.Field<decimal>("SalesPrice"),
//                    //PurchasePrice = row.Field<decimal>("PurchasePrice"),
//                    ProductGroupName = row.Field<string>("ProductGroupName"),
//                    UOMId = row.Field<int>("UOMId"),
//                    UOMName = row.Field<string>("UOMName"),
//                    Status = row.Field<string>("Status")

//                }).ToList();


//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = modelList;
//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.ExMessage = ex.Message;
//                result.Message = ex.Message;
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                }
//            }
//        }
    }

}
