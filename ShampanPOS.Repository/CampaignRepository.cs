using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace ShampanPOS.Repository
{
    using Newtonsoft.Json;
    using ShampanPOS.ViewModel.KendoCommon;
    using ShampanPOS.ViewModel.Utility;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO.Compression;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Xml.Linq;

    public class CampaignRepository : CommonRepository
    {
        public async Task<ResultVM> Insert(CampaignVM campaign, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO Campaigns 
        (BranchId, Code, Name, Description, CampaignStartDate, CampaignEndDate,CampaignEntryDate, EnumTypeId,IsPost,IsArchive, IsActive, CreatedBy, CreatedOn,CreatedFrom)
        VALUES
        (@BranchId, @Code, @Name, @Description, @CampaignStartDate, @CampaignEndDate,@CampaignEntryDate, @EnumTypeId,@IsPost,@IsArchive, @IsActive, @CreatedBy, @CreatedOn,@CreatedFrom);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", campaign.BranchId);
                    cmd.Parameters.AddWithValue("@Code", campaign.Code);
                    cmd.Parameters.AddWithValue("@Name", campaign.Name);
                    cmd.Parameters.AddWithValue("@Description", campaign.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", campaign.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", campaign.CampaignEndDate);
                    cmd.Parameters.AddWithValue("@CampaignEntryDate", campaign.CampaignEntryDate);
                    cmd.Parameters.AddWithValue("@EnumTypeId", campaign.EnumTypeId);
                    cmd.Parameters.AddWithValue("@IsPost", campaign.IsPost);


                    cmd.Parameters.AddWithValue("@IsArchive", false);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", campaign.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", campaign.CreatedOn);
                    cmd.Parameters.AddWithValue("@CreatedFrom", campaign.CreatedFrom);

                    object newId = cmd.ExecuteScalar();
                    campaign.Id = Convert.ToInt32(newId);



                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = campaign.Id.ToString();
                    result.DataVM = campaign;
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


        // InsertDetails Method
        public async Task<ResultVM> InsertCampaignByQuantityDetails(CampaignDetailByQuantityVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO CampaignDetailByQuantities
(CampaignId, BranchId, CustomerId, ProductId, FromQuantity, FreeProductId, FreeQuantity, CampaignStartDate, CampaignEndDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @ProductId, @FromQuantity, @FreeProductId, @FreeQuantity, @CampaignStartDate, @CampaignEndDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", details.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@FromQuantity", details.FromQuantity);
                    cmd.Parameters.AddWithValue("@FreeProductId", details.FreeProductId);
                    cmd.Parameters.AddWithValue("@FreeQuantity", details.FreeQuantity);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", details.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", details.CampaignEndDate);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);



                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultVM> InsertCampaignByProductValueDetails(CampaignDetailByProductValueVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO CampaignDetailByProductValues
(CampaignId, BranchId, CustomerId, ProductId, FromQuantity, ToQuantity, DiscountRateBasedOnUnitPrice, CampaignStartDate, CampaignEndDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @ProductId, @FromQuantity, @ToQuantity, @DiscountRateBasedOnUnitPrice, @CampaignStartDate, @CampaignEndDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", details.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@FromQuantity", details.FromQuantity);
                    cmd.Parameters.AddWithValue("@ToQuantity", details.ToQuantity);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnUnitPrice", details.DiscountRateBasedOnUnitPrice);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", details.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", details.CampaignEndDate);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);



                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultVM> InsertCampaignByProductTotalValueDetails(CampaignDetailByProductTotalValueVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO CampaignDetailByProductTotalValues
(CampaignId, BranchId, CustomerId, ProductId, FromAmount, ToAmount, DiscountRateBasedOnTotalPrice, CampaignStartDate, CampaignEndDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @ProductId, @FromAmount, @ToAmount, @DiscountRateBasedOnTotalPrice, @CampaignStartDate, @CampaignEndDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", details.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@FromAmount", details.FromAmount);
                    cmd.Parameters.AddWithValue("@ToAmount", details.ToAmount);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", details.DiscountRateBasedOnTotalPrice);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", details.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", details.CampaignEndDate);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);



                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultVM> InsertCampaignDetailByInvoiceValueDetails(CampaignDetailByInvoiceValueVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
INSERT INTO CampaignDetailByInvoiceValues
(CampaignId, BranchId, CustomerId,  FromAmount, ToAmount, DiscountRateBasedOnTotalPrice, CampaignStartDate, CampaignEndDate)
VALUES
(@CampaignId, @BranchId, @CustomerId, @FromAmount, @ToAmount, @DiscountRateBasedOnTotalPrice, @CampaignStartDate, @CampaignEndDate);
SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CampaignId", details.CampaignId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId);
                    cmd.Parameters.AddWithValue("@FromAmount", details.FromAmount);
                    cmd.Parameters.AddWithValue("@ToAmount", details.ToAmount);
                    cmd.Parameters.AddWithValue("@DiscountRateBasedOnTotalPrice", details.DiscountRateBasedOnTotalPrice);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", details.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", details.CampaignEndDate);

                    object newId = cmd.ExecuteScalar();
                    details.Id = Convert.ToInt32(newId);



                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }
        // Update Method
        public async Task<ResultVM> Update(CampaignVM campaign, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = campaign.Id.ToString(), DataVM = campaign };

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
UPDATE Campaigns
SET 
   Name = @Name,
   Description = @Description,
   CampaignStartDate = @CampaignStartDate,
   CampaignEndDate = @CampaignEndDate,   


    IsActive = @IsActive,
    --IsActive = @IsActive,
    LastModifiedBy = @LastModifiedBy,
    LastModifiedOn = @LastModifiedOn,
    LastUpdateFrom = @LastUpdateFrom
WHERE Id = @Id;
";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", campaign.Id);
                    cmd.Parameters.AddWithValue("@Code", campaign.Code);
                    cmd.Parameters.AddWithValue("@Name", campaign.Name);
                    cmd.Parameters.AddWithValue("@Description", campaign.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CampaignStartDate", campaign.CampaignStartDate);
                    cmd.Parameters.AddWithValue("@CampaignEndDate", campaign.CampaignEndDate);
                    cmd.Parameters.AddWithValue("@EnumTypeId", campaign.EnumTypeId);
                    cmd.Parameters.AddWithValue("@IsPost", campaign.IsPost);
                    cmd.Parameters.AddWithValue("@IsActive", campaign.IsActive);

                    cmd.Parameters.AddWithValue("@LastModifiedBy", campaign.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", campaign.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", campaign.LastUpdateFrom ?? (object)DBNull.Value);

                    // Execute the query (Insert or Update)
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Data updated successfully.";
                    }
                    else
                    {
                        throw new Exception("No rows were updated.");
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


        // List Method
        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
            ISNULL(H.Id, 0) AS Id,
            ISNULL(H.BranchId, 0) AS BranchId,
            ISNULL(H.Code, '') AS Code,
            ISNULL(H.Name, '') AS Name,
            ISNULL(H.Description, '') AS Description,
            ISNULL(FORMAT(H.CampaignStartDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignStartDate,
            ISNULL(FORMAT(H.CampaignEndDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignEndDate,
            ISNULL(FORMAT(H.CampaignEntryDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignEntryDate,
            ISNULL(H.EnumTypeId, 0) AS EnumTypeId,
			enm.Name EnumName,
            ISNULL(H.IsPost, 0) AS IsPost,
            ISNULL(H.PostedBy, '') AS PostedBy,
            ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS PostedOn,
            ISNULL(H.IsArchive, 0) AS IsArchive,
            ISNULL(H.IsActive, 0) AS IsActive,
            ISNULL(H.CreatedBy, '') AS CreatedBy,
            ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
            ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
            ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
            ISNULL(H.CreatedFrom, '') AS CreatedFrom,
            ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
        FROM 
            Campaigns H
		left outer join EnumTypes enm on  enm.Id=H.EnumTypeId
        
WHERE 
    1 = 1
 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND H.Id = @Id ";
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

                var model = new List<CampaignVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    string encodedHtml = row["Description"].ToString();
                    string decodedHtml = HttpUtility.HtmlDecode(encodedHtml);

                    model.Add(new CampaignVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CampaignStartDate = row["CampaignStartDate"] != DBNull.Value ? Convert.ToDateTime(row["CampaignStartDate"]).ToString("yyyy-MM-dd") : "1900-01-01",
                        CampaignEndDate = row["CampaignEndDate"] != DBNull.Value ? Convert.ToDateTime(row["CampaignEndDate"]).ToString("yyyy-MM-dd") : "1900-01-01",
                        CampaignEntryDate = row["CampaignEntryDate"] != DBNull.Value ? Convert.ToDateTime(row["CampaignEntryDate"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        Name = row["Name"].ToString(),

                        Description = decodedHtml,


                        ////Description = row["Description"].ToString(),

                        EnumTypeId = Convert.ToInt32(row["EnumTypeId"]),
                        EnumName = row["EnumName"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),

                        IsPost = row["IsPost"] != DBNull.Value ? Convert.ToBoolean(row["IsPost"]) : false,
                        PostedBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["LastModifiedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CreatedFrom = row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row["LastUpdateFrom"].ToString()
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

        public async Task<ResultVM> CampaignByQuantityDetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT D.[Id]
       ,D.CampaignId
	    ,C.EnumTypeId AS CampaignTypeId
      ,[CustomerId]
      ,cus.Name CustomerName
      ,[ProductId]
	  ,p.Name ProductName
      ,[FromQuantity]
      ,[FreeProductId]
	  ,FreeP.Name FreeProductName
      ,[FreeQuantity]
 
  FROM [CampaignDetailByQuantities] D
  left outer join Customers cus on cus.id=d.CustomerId
  left outer join Products P on P.id=D.ProductId
  left outer join Products FreeP on FreeP.id=D.[FreeProductId]
  left outer join Campaigns C on D.CampaignId = C.Id

WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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
                result.Message = "Details Data retrieved successfully.";
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


        public async Task<ResultVM> CampaignProductValueDetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT D.[Id]
      ,D.CampaignId
		,C.EnumTypeId AS CampaignTypeId
      ,[CustomerId]
      ,csus.Name CustomerName
      ,[ProductId]
	  ,p.Name ProductName
     
      ,[FromQuantity]     
,[ToQuantity]
,[DiscountRateBasedOnUnitPrice]


 
  FROM [CampaignDetailByProductValues] D
  left outer join Customers csus on csus.id=d.CustomerId
  left outer join Products P on P.id=D.ProductId
  left outer join Campaigns C on D.CampaignId = C.Id

WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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
                result.Message = "Details Data retrieved successfully.";
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

        public async Task<ResultVM> CampaignByProductTotalValueList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT D.[Id]
		,C.EnumTypeId AS CampaignTypeId
       ,D.CampaignId
      ,[CustomerId]
      ,cus.Name CustomerName
      ,[ProductId]
	  ,p.Name ProductName
     
      ,[FromAmount]     
,[ToAmount]
,[DiscountRateBasedOnTotalPrice]


 
  FROM [CampaignDetailByProductTotalValues] D
  left outer join Customers Cus on Cus.id=d.CustomerId
  left outer join Products P on P.id=D.ProductId
  left outer join Campaigns C on D.CampaignId = C.Id

WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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
                result.Message = "Details Data retrieved successfully.";
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

        // ListAsDataTable Method


        public async Task<ResultVM> CampaignDetailByInvoiceValueList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
SELECT D.[Id]
       ,C.EnumTypeId AS CampaignTypeId

      ,[CustomerId]
      ,cus.Name CustomerName

     
      ,[FromAmount]     
,[ToAmount]
,[DiscountRateBasedOnTotalPrice]


 
  FROM [CampaignDetailByInvoiceValues] D
  left outer join Customers Cus on Cus.id=d.CustomerId
  left outer join Campaigns C on D.CampaignId = C.Id

WHERE 1 = 1 ";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND D.Id = @Id ";
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
                result.Message = "Details Data retrieved successfully.";
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
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.SalePersonId, 0) AS SalePersonId,
    ISNULL(M.RouteId, 0) AS RouteId,
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(M.VehicleNo, '') AS VehicleNo,
    ISNULL(M.VehicleType, '') AS VehicleType,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
    ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(M.Comments, '') AS Comments,     
    ISNULL(M.IsPrint, 0) AS IsPrint,   
	ISNULL(M.PrintBy,'') AS PrintBy,   
    ISNULL(FORMAT(M.PrintOn, 'yyyy-MM-dd'), '1900-01-01') AS       PrintOn,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, 0) AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd'), '1900-01-01') AS      PostedOn, 
    ISNULL(M.FiscalYear, 0) AS FiscalYear,
    ISNULL(M.PeriodId, 0) AS PeriodId,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Sales M
WHERE 
    1 = 1
  ";


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
                SELECT Id, Code
                FROM SaleDeleveries
                WHERE 1=1
                ORDER BY Id";

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


        // MultiplePost Method
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

                string query = $" UPDATE Campaigns SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";

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
                        result.Message = $"Data posted successfully.";
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


        public async Task<ResultVM> GetGridData(GridOptions options, string EnumId, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<CampaignVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
                    SELECT COUNT(DISTINCT H.Id) AS totalcount
                    FROM Campaigns H

                    WHERE H.IsArchive != 1 AND H.EnumTypeId = @EnumId
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CampaignVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                         ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,
                        ISNULL(FORMAT(H.CampaignEntryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CampaignEntryDate,
                        ISNULL(FORMAT(H.CampaignStartDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignStartDate,
                        ISNULL(FORMAT(H.CampaignEndDate, 'yyyy-MM-dd'), '1900-01-01') AS CampaignEndDate,
                        ISNULL(H.Name, '') AS Name,
                        ISNULL(H.Description, '') AS Description,
                        ISNULL(H.IsArchive, 0) AS IsArchive,
                        ISNULL(H.IsActive, 0) AS IsActive,                
                        ISNULL(H.IsPost, 0) AS IsPost,

                        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                        ISNULL(H.CreatedBy, '') AS CreatedBy,
                        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                        ISNULL(H.BranchId, 0) AS BranchId

                    FROM Campaigns H

                    WHERE H.IsArchive != 1 AND H.EnumTypeId = @EnumId
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CampaignVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";
                sqlQuery = sqlQuery.Replace("@EnumId", "'" + EnumId + "'");


                data = KendoGrid<CampaignVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string EnumId, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<CampaignDetail>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
                    SELECT COUNT(DISTINCT H.Id) AS totalcount
                    FROM Campaigns H
                    LEFT OUTER JOIN CampaignDetailByInvoiceValues D ON H.Id = D.CampaignId
                    LEFT OUTER JOIN CampaignDetailByProductTotalValues DP ON H.Id = DP.CampaignId
                    LEFT OUTER JOIN CampaignDetailByProductValues DPV ON H.Id = DPV.CampaignId
                    LEFT OUTER JOIN CampaignDetailByQuantities DQ ON H.Id = DQ.CampaignId
                    LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id

                    WHERE H.IsArchive != 1 AND H.EnumTypeId = @EnumId
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CampaignDetail>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,
                        ISNULL(H.Name, '') AS Name,
                        ISNULL(H.Description, '') AS Description,
                        ISNULL(H.IsArchive, 0) AS IsArchive,
                        ISNULL(H.IsActive, 0) AS IsActive,                
                        ISNULL(H.IsPost, 0) AS IsPost,
                        CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                        ISNULL(H.CreatedBy, '') AS CreatedBy,
                        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                        ISNULL(H.BranchId, 0) AS BranchId,
                        ISNULL(Br.Name, '') AS BranchName,
	                -- CampaignDetailByInvoiceValues
		                 ISNULL(D.Id, 0) AS InvoiceValueId,		              
		                 ISNULL(D.FromAmount, 0) AS InvoiceValueFromAmount,
		                 ISNULL(D.ToAmount, 0) AS InvoiceValueToAmount,
		                 ISNULL(D.DiscountRateBasedOnTotalPrice, 0) AS InvoiceValueDiscountRateBasedOnTotalPrice,
		                 -- CampaignDetailByProductTotalValue
		                 ISNULL(DP.Id, 0) AS ProductTotalValueId,
		                 ISNULL(DP.FromAmount, 0) AS ProductTotalValueFromAmount,
		                 ISNULL(DP.ToAmount, 0) AS ProductTotalValueToAmount,
		                 ISNULL(DP.DiscountRateBasedOnTotalPrice, 0) AS ProductTotalValueDiscountRateBasedOnTotalPrice,
		                 -- CampaignDetailByProductValues
		                 ISNULL(DPV.Id, 0) AS ProductValueId,
		                  ISNULL(DPV.FromQuantity, 0) AS ProductValueFromQuantity,
		                 ISNULL(DPV.ToQuantity, 0) AS ProductValueToQuantity,
		
		                  -- CampaignDetailByQuantities
		                 ISNULL(DQ.Id, 0) AS DetailByQuantitieId,
		                 ISNULL(DQ.FromQuantity, 0) AS DetailByQuantitieFromQuantity,
		                 ISNULL(DQ.FreeQuantity, 0) AS DetailByQuantitieFreeQuantity
                   
            
                    FROM Campaigns H
                    LEFT OUTER JOIN CampaignDetailByInvoiceValues D ON H.Id = D.CampaignId
                    LEFT OUTER JOIN CampaignDetailByProductTotalValues DP ON H.Id = DP.CampaignId
                    LEFT OUTER JOIN CampaignDetailByProductValues DPV ON H.Id = DPV.CampaignId
                    LEFT OUTER JOIN CampaignDetailByQuantities DQ ON H.Id = DQ.CampaignId
                    LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
               
                    WHERE H.IsArchive != 1 AND H.EnumTypeId = @EnumId
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CampaignDetail>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";
                sqlQuery = sqlQuery.Replace("@EnumId", "'" + EnumId + "'");


                data = KendoGrid<CampaignDetail>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<(bool, int)> CampaignCheckExists(CampaignVM campaign, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;

            try
            {
                string sqlText = @"
            SELECT TOP 1 c.id 
            FROM Campaigns c
            WHERE c.IsActive = 1
            AND c.BranchId = @BranchId
            --AND @InvoiceDate BETWEEN c.CampaignEntryDate
            AND c.CampaignEntryDate = @InvoiceDate
            AND c.EnumTypeId = @EnumTypeId";

                using (var command = CreateCommand(sqlText, conn, transaction))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@InvoiceDate", campaign.CampaignEntryDate);
                    command.Parameters.AddWithValue("@BranchId", campaign.BranchId);
                    command.Parameters.AddWithValue("@EnumTypeId", campaign.EnumTypeId);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int campaignId = Convert.ToInt32(result);
                        return (true, campaignId); // Campaign exists, return Id
                    }

                    return (false, 0); // No campaign found
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CampaignDataExists(CampaignVM campaign, SqlConnection conn, SqlTransaction transaction)
        {
            string sqlText = @"
        SELECT TOP 1 c.Id
        FROM Campaigns c
        WHERE c.IsActive = 1
          AND c.CampaignEntryDate = @CampaignEntryDate
          AND c.CampaignEndDate = @CampaignEndDate
          AND c.Name = @Name
          AND c.BranchId = @BranchId";

            using (var command = CreateCommand(sqlText, conn, transaction))
            {
                // Add parameters
                command.Parameters.AddWithValue("@CampaignEntryDate", campaign.CampaignEntryDate);
                command.Parameters.AddWithValue("@CampaignEndDate", campaign.CampaignEndDate);
                command.Parameters.AddWithValue("@Name", campaign.Name ?? string.Empty);
                command.Parameters.AddWithValue("@BranchId", campaign.BranchId);

                var result = await command.ExecuteScalarAsync();

                // Return true if any record exists, otherwise false
                return result != null && int.TryParse(result.ToString(), out _);
            }
        }



    }

}
