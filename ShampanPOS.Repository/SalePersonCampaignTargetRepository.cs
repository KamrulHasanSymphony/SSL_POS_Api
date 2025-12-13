using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    
    public class SalePersonCampaignTargetRepository : CommonRepository
    {
        // Insert Method
       
        public async Task<ResultVM> Insert(SalePersonCampaignTargetVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    INSERT INTO SalePersonCampaignTargets 
                    (
                    BranchId, SalePersonId, CampaignId, TotalTarget, TotalSale, SelfSaleCommissionRate, 
                    OtherSaleCommissionRate, StartDate, EndDate,IsApproved,IsPost, CreatedBy,CreatedFrom, CreatedOn
                    )
                    VALUES 
                    (
                    @BranchId, @SalePersonId, @CampaignId, @TotalTarget, @TotalSale, @SelfSaleCommissionRate, 
                    @OtherSaleCommissionRate, @StartDate, @EndDate, @IsApproved,@IsPost, @CreatedBy,@CreatedFrom, @CreatedOn
                    );
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@CampaignId", vm.CampaignId);
                    cmd.Parameters.AddWithValue("@TotalTarget", vm.TotalTarget);
                    cmd.Parameters.AddWithValue("@TotalSale", vm.TotalSale);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    cmd.Parameters.AddWithValue("@IsApproved", vm.IsApproved);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
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


        // Update Method
       
        public async Task<ResultVM> Update(SalePersonCampaignTargetVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                UPDATE SalePersonCampaignTargets 
                SET 
                 SalePersonId=@SalePersonId, CampaignId=@CampaignId, TotalTarget=@TotalTarget, 
                 TotalSale=@TotalSale, SelfSaleCommissionRate=@SelfSaleCommissionRate, OtherSaleCommissionRate=@OtherSaleCommissionRate,
                 StartDate=@StartDate, EndDate=@EndDate,
                 LastModifiedBy=@LastModifiedBy,LastUpdateFrom=@LastUpdateFrom, LastModifiedOn=GETDATE()
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@CampaignId", vm.CampaignId);
                    cmd.Parameters.AddWithValue("@TotalTarget", vm.TotalTarget);
                    cmd.Parameters.AddWithValue("@TotalSale", vm.TotalSale);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@StartDate", vm.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", vm.EndDate);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        throw new Exception(result.Message);
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

                string query = $" UPDATE SalePersonCampaignTargets SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
    ISNULL(M.CampaignId, 0) AS CampaignId,
    ISNULL(M.TotalTarget, 0.00) AS TotalTarget,
    ISNULL(M.TotalSale, 0.00) AS TotalSale,
    ISNULL(M.SelfSaleCommissionRate, 0.00) AS SelfSaleCommissionRate,
    ISNULL(M.OtherSaleCommissionRate, 0.00) AS OtherSaleCommissionRate,
    FORMAT(ISNULL(M.StartDate, '1900-01-01'), 'yyyy-MM-dd') AS StartDate,
    FORMAT(ISNULL(M.EndDate, '1900-01-01'), 'yyyy-MM-dd') AS EndDate,
    ISNULL(M.IsApproved, 0) AS IsApproved,
    ISNULL(M.ApprovedBy, '') AS ApprovedBy,
    FORMAT(ISNULL(M.ApprovedDate, '1900-01-01'), 'yyyy-MM-dd') AS ApprovedDate,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,    
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    FORMAT(ISNULL(M.PostedOn, '1900-01-01'), 'yyyy-MM-dd') AS PostedOn,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM SalePersonCampaignTargets M
WHERE 1 = 1
";

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

                var modelList = dataTable.AsEnumerable().Select(row => new SalePersonCampaignTargetVM
                {
                    Id = row.Field<int>("Id"),
                    BranchId = row.Field<int>("BranchId"),
                    SalePersonId = row.Field<int>("SalePersonId"),
                    CampaignId = row.Field<int>("CampaignId"),
                    TotalTarget = row.Field<decimal>("TotalTarget"),
                    TotalSale = row.Field<decimal>("TotalSale"),
                    SelfSaleCommissionRate = row.Field<decimal>("SelfSaleCommissionRate"),
                    OtherSaleCommissionRate = row.Field<decimal>("OtherSaleCommissionRate"),
                    IsApproved = row.Field<bool>("IsApproved"),
                    ApprovedBy = row.Field<string>("ApprovedBy"),
                    ApprovedDate = row.Field<string>("ApprovedDate"),
                    IsPost = row.Field<bool>("IsPost"),
                    PostedBy = row.Field<string>("PostedBy"),
                    PostedOn = row.Field<string>("PostedOn"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    CreatedFrom = row.Field<string>("CreatedFrom"),
                    LastUpdateFrom = row.Field<string>("LastUpdateFrom"),
                    LastModifiedOn = row.Field<string>("LastModifiedOn")
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
    ISNULL(M.CampaignId, 0) AS CampaignId,
    ISNULL(M.TotalTarget, 0.00) AS TotalTarget,
    ISNULL(M.TotalSale, 0.00) AS TotalSale,
    ISNULL(M.SelfSaleCommissionRate, 0.00) AS SelfSaleCommissionRate,
    ISNULL(M.OtherSaleCommissionRate, 0.00) AS OtherSaleCommissionRate,
    FORMAT(ISNULL(M.StartDate, '1900-01-01'), 'yyyy-MM-dd') AS StartDate,
    FORMAT(ISNULL(M.EndDate, '1900-01-01'), 'yyyy-MM-dd') AS EndDate,
    ISNULL(M.IsApproved, 0) AS IsApproved,
    ISNULL(M.ApprovedBy, '') AS ApprovedBy,
    FORMAT(ISNULL(M.ApprovedDate, '1900-01-01'), 'yyyy-MM-dd') AS ApprovedDate,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,    
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    FORMAT(ISNULL(M.PostedOn, '1900-01-01'), 'yyyy-MM-dd') AS PostedOn,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    FORMAT(ISNULL(M.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    FORMAT(ISNULL(M.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn
FROM SalePersonCampaignTargets M
WHERE 1 = 1
";


                DataTable dataTable = new DataTable();

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

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = dataTable;
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
SELECT Id, TotalTarget Name
FROM SalePersonCampaignTargets
WHERE IsActive = 1
ORDER BY Id ";

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

        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = $" UPDATE SalePersonCampaignTargets SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    cmd.Parameters.AddWithValue("@PostedBy", vm.ModifyBy);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data Posted successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were posted.");
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
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
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
        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SalePersonCampaignTargetVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
    FROM SalePersonCampaignTargets  H
    LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
    LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
    LEFT OUTER JOIN Campaigns cam ON H.CampaignId = cam.Id
	WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonCampaignTargetVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex ,
        
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(H.SalePersonId, 0) AS SalePersonId,
            ISNULL(H.CampaignId, 0) AS CampaignId,
            ISNULL(H.TotalTarget, 0.00) AS TotalTarget,
            ISNULL(H.TotalSale, 0.00) AS TotalSale,
            ISNULL(H.SelfSaleCommissionRate, 0.00) AS SelfSaleCommissionRate,
            ISNULL(H.OtherSaleCommissionRate, 0.00) AS OtherSaleCommissionRate,
            FORMAT(ISNULL(H.StartDate, '1900-01-01'), 'yyyy-MM-dd') AS StartDate,
            FORMAT(ISNULL(H.EndDate, '1900-01-01'), 'yyyy-MM-dd') AS EndDate,
            ISNULL(H.IsApproved, 0) AS IsApproved,
            ISNULL(H.ApprovedBy, '') AS ApprovedBy,
            FORMAT(ISNULL(H.ApprovedDate, '1900-01-01'), 'yyyy-MM-dd') AS ApprovedDate,
            ISNULL(H.IsPost, 0) AS IsPost,
            CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
            ISNULL(H.PostedBy, '') AS PostedBy,    
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
            FORMAT(ISNULL(H.PostedOn, '1900-01-01'), 'yyyy-MM-dd') AS PostedDate,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            FORMAT(ISNULL(H.CreatedOn, '1900-01-01'), 'yyyy-MM-dd') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            FORMAT(ISNULL(H.LastModifiedOn, '1900-01-01'), 'yyyy-MM-dd') AS LastModifiedOn,

            ISNULL(Br.Name,'') BranchName,
            ISNULL(SP.Name,'') SalePersonName,
            ISNULL(cam.Name,'') CampaignName


            FROM SalePersonCampaignTargets  H
            LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
            LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
            LEFT OUTER JOIN Campaigns cam ON H.CampaignId = cam.Id
            WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonCampaignTargetVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SalePersonCampaignTargetVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
