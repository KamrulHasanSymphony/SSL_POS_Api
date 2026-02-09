using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Xml;

namespace ShampanPOS.Repository
{
    
    public class ProductRepository : CommonRepository
    {
        // Insert Method
       
        public async Task<ResultVM> Insert(ProductVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO Products
        (
            Code, Name,CompanyId,UserId, ProductGroupId, BanglaName, Description, UOMId, HSCodeNo,VATRate,SDRate,PurchasePrice,SalePrice, 
            IsArchive, IsActive, CreatedBy,CreatedFrom, CreatedOn,ImagePath
        )
        VALUES
        (
            @Code, @Name,@CompanyId,@UserId, @ProductGroupId, @BanglaName, @Description, @UOMId, @HSCodeNo,@VATRate,@SDRate,@PurchasePrice,@SalePrice,
            @IsArchive, @IsActive, @CreatedBy, @CreatedFrom,@CreatedOn,@ImagePath
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyId", vm.CompanyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductGroupId", vm.ProductGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", vm.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId ?? 1);
                    cmd.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CtnSize", vm.CtnSize ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CtnSizeFactor", vm.CtnSizeFactor ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate ?? 0.0m); 
                    cmd.Parameters.AddWithValue("@SDRate", vm.SDRate ?? 0.0m); 
                    cmd.Parameters.AddWithValue("@PurchasePrice", vm.PurchasePrice ?? 0.0m);
                    cmd.Parameters.AddWithValue("@SalePrice", vm.SalePrice ?? 0.0m);


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

        //InsertFactors
        public async Task<ResultVM> InsertProductUOMFactorss(ProductUOMFactorsVM factors, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"

                    INSERT INTO ProductUOMFactors
                    (Name, Packsize, ProductId, ConversationFactor, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom)
                    VALUES 
                    (@Name, @Packsize, @ProductId, @ConversationFactor, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", factors.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Packsize", factors.Packsize ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", factors.ProductId );
                    cmd.Parameters.AddWithValue("@ConversationFactor", factors.ConversationFactor);
                    cmd.Parameters.AddWithValue("@IsArchive", factors.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", factors.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", factors.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", factors.CreatedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", factors.CreatedFrom ?? (object)DBNull.Value);
                    
                    object newId = cmd.ExecuteScalar();
                    factors.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "factors data inserted successfully.";
                    result.DetailId = newId.ToString();
                    result.DataVM = factors;
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

        public async Task<ResultVM> Update(ProductVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE Products
        SET
            Code = @Code, Name = @Name, ProductGroupId = @ProductGroupId, BanglaName = @BanglaName, VATRate= @VATRate,SDRate= @SDRate,PurchasePrice= @PurchasePrice,SalePrice= @SalePrice,
            Description = @Description, UOMId = @UOMId, HSCodeNo = @HSCodeNo,
            IsArchive = @IsArchive, IsActive = @IsActive, LastModifiedBy = @LastModifiedBy, LastUpdateFrom=@LastUpdateFrom,
            LastModifiedOn = GETDATE(),ImagePath = @ImagePath
        WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductGroupId", vm.ProductGroupId);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", vm.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UOMId", vm.UOMId ?? 1);
                    cmd.Parameters.AddWithValue("@HSCodeNo", vm.HSCodeNo ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CtnSize", vm.CtnSize ?? (object)DBNull.Value);
                    //cmd.Parameters.AddWithValue("@CtnSizeFactor", vm.CtnSizeFactor ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);
                    cmd.Parameters.AddWithValue("@VATRate", vm.VATRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@SDRate", vm.SDRate ?? 0.0m);
                    cmd.Parameters.AddWithValue("@PurchasePrice", vm.PurchasePrice ?? 0.0m);
                    cmd.Parameters.AddWithValue("@SalePrice", vm.SalePrice ?? 0.0m);

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

                string query = $" UPDATE Products SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

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
                ISNULL(M.Id, 0) Id,
                ISNULL(M.Code, '') Code,
                ISNULL(M.Name, '') Name,
                ISNULL(M.ProductGroupId, 0) ProductGroupId,
                ISNULL(PG.Name, 0) ProductGroupName ,
                ISNULL(M.BanglaName, '') BanglaName,
                ISNULL(M.Description, '') Description,
                ISNULL(M.UOMId, 0) UOMId,
                ISNULL(UM.Name, '') UOMName,
                ISNULL(M.HSCodeNo, '') HSCodeNo,
                ISNULL(M.IsArchive, 0) IsArchive,
                ISNULL(M.IsActive, 0) IsActive,
                ISNULL(M.CreatedBy, '') CreatedBy,
                ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
                ISNULL(M.LastModifiedBy, '') LastModifiedBy,
                ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
                ISNULL(M.ImagePath,'') AS ImagePath,
            	ISNULL(M.VATRate, 0) AS VATRate,
            	ISNULL(M.SDRate, 0) AS SDRate,
            	ISNULL(M.PurchasePrice, 0) AS PurchasePrice,
            	ISNULL(M.SalePrice, 0) AS SalePrice
            
            FROM Products M
            LEFT OUTER JOIN ProductGroups PG ON M.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UM ON M.UOMId = UM.Id
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

                var modelList = dataTable.AsEnumerable().Select(row => new ProductVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    Description = row.Field<string>("Description"),
                    UOMId = row.Field<int?>("UOMId"), // Nullable field
                    UOMName = row.Field<string>("UOMName"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    IsActive = row.Field<bool>("IsActive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                    ImagePath = row.Field<string>("ImagePath"),
                    VATRate = row.Field<decimal?>("VATRate") ?? 0.0m, 
                    SDRate = row.Field<decimal?>("SDRate") ?? 0.0m,   
                    PurchasePrice = row.Field<decimal?>("PurchasePrice") ?? 0.0m,
                    SalePrice = row.Field<decimal?>("SalePrice") ?? 0.0m

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
    Id, Code, Name, ProductGroupId, BanglaName, Description, UOMId, HSCodeNo, 
    IsArchive, IsActive, CreatedBy, CreatedOn, 
    LastModifiedBy, LastModifiedOn
FROM Products WHERE 1 = 1";



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
SELECT Id, Name
FROM Products
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


        // GetProductModalData Method
//        public async Task<ResultVM> GetProductModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//ISNULL(PBH.CostPrice,0) CostPrice , 
//ISNULL(PBH.SalesPrice,0) SalesPrice , 
//ISNULL(PBH.PurchasePrice,0) PurchasePrice , 
//ISNULL(PBH.SD,0) SD , 
//ISNULL(PBH.VATRate,0) VATRate , 

//CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status

//FROM Products P
//LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId AND PBH.BranchId = @BranchId
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
//                    CostPrice = row.Field<decimal>("CostPrice"),
//                    SalesPrice = row.Field<decimal>("SalesPrice"),
//                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
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

        // GetProductModalCountData Method
//        public async Task<ResultVM> GetProductModalCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

//COALESCE(COUNT(P.Id), 0) AS FilteredCount

//FROM Products P
//LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId AND PBH.BranchId = @BranchId
//LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

//WHERE P.IsActive = 1 
//";

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, true);                

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
//                }

//                objComm.Fill(dataTable);

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
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

        // GetUOMFromNameData Method


        // GetProductGroupModalData Method
//        public async Task<ResultVM> GetProductGroupModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//SELECT DISTINCT
//ISNULL(P.Id,0) ProductGroupId , 
//ISNULL(P.Name,'') ProductGroupName,
//ISNULL(P.Code,'') ProductGroupCode, 
//CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status

//FROM ProductGroups P

//WHERE P.IsActive = 1 

//";

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                objComm.Fill(dataTable);

//                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
//                {
//                    ProductGroupId = row.Field<int>("ProductGroupId"),
//                    ProductGroupName = row.Field<string>("ProductGroupName"),
//                    ProductGroupCode = row.Field<string>("ProductGroupCode"),
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

                var data = new GridEntity<ProductVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM Products H
            LEFT OUTER JOIN ProductGroups PG ON H.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs uom ON H.UOMId = uom.Id
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductVM>.FilterCondition(options.filter) + ")" : "") + @"

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
                CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.ProductGroupId, 0) AS ProductGroupId,
                ISNULL(PG.Name, '') AS ProductGroupName,
                ISNULL(H.UOMId, 0) AS UOMId,
				ISNULL(H.VATRate, 0) AS VATRate,
				ISNULL(H.SDRate, 0) AS SDRate,
				ISNULL(H.PurchasePrice, 0) AS PurchasePrice,
				ISNULL(H.SalePrice, 0) AS SalePrice
            FROM Products H
            LEFT OUTER JOIN ProductGroups PG ON H.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs uom ON H.UOMId = uom.Id
            WHERE H.IsArchive != 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<ProductVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.Name, '') AS Name,
                ISNULL(H.Description, '') AS Description,
                ISNULL(H.IsArchive, 0) AS IsArchive,                
                CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.ProductGroupId, 0) AS ProductGroupId,
                ISNULL(PG.Name, '') AS ProductGroupName,
                ISNULL(H.UOMId, 0) AS UOMId,
                ISNULL(uom.Name, '') AS UOMName
            FROM Products H
            LEFT OUTER JOIN ProductGroups PG ON H.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs uom ON H.UOMId = uom.Id

            WHERE  1 = 1 ";

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




//        public async Task<ResultVM> ExportProductExcel(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"

//                SELECT 
//                ISNULL(M.Id, 0) AS Id,
//                ISNULL(M.Code, '') AS Code,
//                ISNULL(M.Name, '') AS Name

//                FROM Products M

//                WHERE 1=1"

//;

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
            
//                objComm.Fill(dataTable);

//                foreach (DataRow row in dataTable.Select("Id = 0"))
//                {
//                    dataTable.Rows.Remove(row);
//                }

//                var lst = new List<ProductSummaryVM>();

//                int serialNumber = 1;

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new ProductSummaryVM
//                    {
                        
//                        ProductName = row["Name"].ToString(),
//                        ProductCode = row["Code"].ToString(),
//                        EffectDate = DateTime.Now.ToString("yyyy-MM-dd"),
//                        CostPrice = 0,
//                        SalesPrice = 0

//                    });

//                    serialNumber++;
//                }

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = lst;
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

//        public async Task<ResultVM> ExportProductPursaseExcel(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                string query = @"

//                SELECT 
//                ISNULL(M.Id, 0) AS Id,
//                ISNULL(M.Code, '') AS Code,
//                ISNULL(M.Name, '') AS Name

//                FROM Products M

//                WHERE 1=1"

//;

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                objComm.Fill(dataTable);

//                foreach (DataRow row in dataTable.Select("Id = 0"))
//                {
//                    dataTable.Rows.Remove(row);
//                }

//                var lst = new List<ProductSummaryVM>();

//                int serialNumber = 1;

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new ProductSummaryVM
//                    {

//                        ProductName = row["Name"].ToString(),
//                        ProductCode = row["Code"].ToString(),
//                        EffectDate = DateTime.Now.ToString("yyyy-MM-dd"),
//                        CostPrice = 0,

//                    });

//                    serialNumber++;
//                }

//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.DataVM = lst;
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


        public async Task<ResultVM> TempProductList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

ISNULL(D.Id, 0) AS Id,
ISNULL(D.BranchId, 0) AS BranchId,
ISNULL(D.ProductName, '') AS ProductName,
ISNULL(D.ProductCode, '') AS ProductCode,
ISNULL(FORMAT(D.EffectDate, 'yyyy-MM-dd'), '1900-01-01') AS EffectDate,
ISNULL(D.GroupName, '') AS GroupName,
ISNULL(D.SalesPrice, 0) AS SalesPrice,
ISNULL(D.CostPrice, 0) AS CostPrice

FROM 

TempProductData D

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


        //        public async Task<ResultVM> ExportProductStockExcel(CommonVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //        {
        //            bool isNewConnection = false;
        //            DataTable dataTable = new DataTable();
        //            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //            try
        //            {
        //                if (conn == null)
        //                {
        //                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                    conn.Open();
        //                    isNewConnection = true;
        //                }

        //                string query = @"

        //                        SELECT
        //                        ISNULL(M.Id, 0) AS Id,
        //                        ISNULL(M.Code, '') AS ProductCode,
        //                        ISNULL(M.Name, '') AS ProductName,
        //                        ISNULL(POS.OpeningDate, '1900-01-01') AS OpeningDate,
        //                        ISNULL(POS.OpeningQuantity, 0) AS OpeningQuantity,
        //                        ISNULL(POS.OpeningValue, 0) AS OpeningValue
        //                        FROM 
        //                        Products M
        //                        left outer join ProductsOpeningStocks POS ON M.Id = POS.ProductId
        //                    WHERE 1 = 1"

        //;

        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                objComm.Fill(dataTable);

        //                foreach (DataRow row in dataTable.Select("Id = 0"))
        //                {
        //                    dataTable.Rows.Remove(row);
        //                }

        //                var lst = new List<ProductStockSummary>();

        //                int serialNumber = 1;

        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    lst.Add(new ProductStockSummary
        //                    {

        //                        ProductName = row["ProductName"].ToString(),
        //                        ProductCode = row["ProductCode"].ToString(),
        //                        OpeningDate = DateTime.Now.ToString("yyyy-MM-dd"),
        //                        OpeningQuantity = 0,
        //                        OpeningValue = 0

        //                    });

        //                    serialNumber++;
        //                }

        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
        //                result.DataVM = lst;
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

        //public async Task<ResultVM> PurchaseImportExcelFileInsert(ProductPriceGroupVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        if (transaction == null)
        //        {
        //            transaction = conn.BeginTransaction();
        //        }



        //        #region BulKInsert

        //        if (vm.BranchProfileList != null && vm.BranchProfileList.Count > 0)
        //        {
        //            foreach (var branchprofile in vm.BranchProfileList)
        //            {
        //                if (vm.ProductPriceGroupDetails != null && vm.ProductPriceGroupDetails.Count > 0)
        //                {

        //                    DataTable detailsTable = new DataTable();
        //                    detailsTable.Columns.Add("ProductId", typeof(int));
        //                    detailsTable.Columns.Add("BranchId", typeof(int));
        //                    detailsTable.Columns.Add("BatchNo", typeof(string));

        //                    detailsTable.Columns.Add("EntryDate", typeof(string));
        //                    detailsTable.Columns.Add("EffectDate", typeof(string));
        //                    detailsTable.Columns.Add("MFGDate", typeof(string));
        //                    detailsTable.Columns.Add("EXPDate", typeof(string));

        //                    detailsTable.Columns.Add("SD", typeof(decimal));
        //                    detailsTable.Columns.Add("SDAmount", typeof(decimal));
        //                    detailsTable.Columns.Add("VATRate", typeof(decimal));
        //                    detailsTable.Columns.Add("VATAmount", typeof(decimal));
        //                    detailsTable.Columns.Add("CostPrice", typeof(decimal));
        //                    detailsTable.Columns.Add("SalesPrice", typeof(decimal));
        //                    detailsTable.Columns.Add("PurchasePrice", typeof(decimal));
        //                    detailsTable.Columns.Add("CreatedBy", typeof(string));

        //                    detailsTable.Columns.Add("CreatedOn", typeof(string));

        //                    detailsTable.Columns.Add("LastModifiedBy", typeof(string));

        //                    detailsTable.Columns.Add("LastModifiedOn", typeof(string));

        //                    detailsTable.Columns.Add("CreatedFrom", typeof(string));
        //                    detailsTable.Columns.Add("LastUpdateFrom", typeof(string));

        //                    foreach (var detail in vm.ProductPriceGroupDetails)
        //                    {
        //                        detailsTable.Rows.Add(

        //                            detail.ProductId,
        //                            branchprofile.Id,
        //                            DBNull.Value,

        //                            DateTime.Now,
        //                            vm.EffectDate,
        //                            DBNull.Value,
        //                            DBNull.Value,

        //                            0,
        //                            0,
        //                            detail.VATRate,
        //                            0,
        //                            detail.CosePrice,
        //                            detail.SalePrice??0,
        //                            0,
        //                            vm.CreatedBy,

        //                            DateTime.Now,

        //                            DBNull.Value,

        //                            DBNull.Value,

        //                            DBNull.Value,
        //                            DBNull.Value

        //                            );
        //                    }

        //                    var resultt = await BulkInsert("ProductPurchasePriceBatchHistories", detailsTable, conn, transaction);

        //                    if (resultt.Status.ToLower() != "success")
        //                    {
        //                        return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
        //                    }

        //                    if (resultt.Status.ToLower() == "success" && isNewConnection)
        //                    {
        //                        //transaction.Commit();
        //                    }

        //                }
        //            }
        //        }

        //        #endregion

        //        result.Status = "Success";
        //        result.Message = "Data inserted successfully.";
        //        result.Id = vm.Id.ToString();
        //        result.DataVM = vm;

        //        if (isNewConnection)
        //        {
        //            transaction.Commit();
        //        }

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //        {
        //            transaction.Rollback();
        //        }

        //        result.ExMessage = ex.Message;
        //        result.Message = "Error in Insert.";
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        // List Method
        public async Task<ResultVM> ReportList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(M.Id, 0) Id,
                ISNULL(M.Code, '') Code,
                ISNULL(M.Name, '') Name,
                ISNULL(M.ProductGroupId, 0) ProductGroupId,
                ISNULL(PG.Name, 0) ProductGroupName ,
                ISNULL(M.BanglaName, '') BanglaName,
                ISNULL(M.Description, '') Description,
                ISNULL(M.UOMId, 0) UOMId,
                ISNULL(UM.Name, '') UOMName,
                ISNULL(M.HSCodeNo, '') HSCodeNo,
                ISNULL(M.IsArchive, 0) IsArchive,
                ISNULL(M.IsActive, 0) IsActive,
                ISNULL(M.CreatedBy, '') CreatedBy,
                ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
                ISNULL(M.LastModifiedBy, '') LastModifiedBy,
                ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
                ISNULL(M.ImagePath,'') AS ImagePath,
            	ISNULL(M.VATRate, 0) AS VATRate,
            	ISNULL(M.SDRate, 0) AS SDRate,
            	ISNULL(M.PurchasePrice, 0) AS PurchasePrice,
            	ISNULL(M.SalePrice, 0) AS SalePrice
            
            FROM Products M
            LEFT OUTER JOIN ProductGroups PG ON M.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UM ON M.UOMId = UM.Id
            WHERE 1 = 1";

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

                var modelList = dataTable.AsEnumerable().Select(row => new ProductVM
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    Description = row.Field<string>("Description"),
                    UOMId = row.Field<int?>("UOMId"), // Nullable field
                    UOMName = row.Field<string>("UOMName"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    IsArchive = row.Field<bool>("IsArchive"),
                    IsActive = row.Field<bool>("IsActive"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                    ImagePath = row.Field<string>("ImagePath"),
                    VATRate = row.Field<decimal?>("VATRate") ?? 0.0m,
                    SDRate = row.Field<decimal?>("SDRate") ?? 0.0m,
                    PurchasePrice = row.Field<decimal?>("PurchasePrice") ?? 0.0m,
                    SalePrice = row.Field<decimal?>("SalePrice") ?? 0.0m

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





        public bool Exists(string name, SqlConnection conn, SqlTransaction tran)
        {
            string sql = @"
              SELECT COUNT(1)
              FROM Products
              WHERE LOWER(LTRIM(RTRIM(Name))) = LOWER(LTRIM(RTRIM(@Name)))
              AND IsArchive = 0
           ";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.AddWithValue("@Name", name.Trim());
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

    }

}
