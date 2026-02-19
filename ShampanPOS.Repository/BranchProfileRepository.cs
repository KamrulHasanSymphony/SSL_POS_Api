using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShampanPOS.Repository
{
 

    public class BranchProfileRepository:CommonRepository
    {
        // Insert Method
         public async Task<ResultVM> Insert(BranchProfileVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO BranchProfiles 
                (Code, DistributorCode, Name,BanglaName,  TelephoneNo, Email, VATRegistrationNo, BIN, TINNO, Comments, 
                 IsArchive, IsActive, CreatedBy, CreatedOn,Address)
                VALUES 
                (@Code, @DistributorCode, @Name,@BanglaName,  @TelephoneNo, @Email, @VATRegistrationNo, @BIN, @TINNO, @Comments, 
                 @IsArchive, @IsActive, @CreatedBy, GETDATE(), @Address);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistributorCode", vm.DistributorCode);
                    cmd.Parameters.AddWithValue("@Name", vm.Name);
                    cmd.Parameters.AddWithValue("@BanglaName", vm.BanglaName);
                    cmd.Parameters.AddWithValue("@TelephoneNo", vm.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", vm.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VATRegistrationNo", vm.VATRegistrationNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", vm.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNO", vm.TINNO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", vm.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    //cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);

                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id =vm.Id.ToString();
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
         public async Task<ResultVM> Update(BranchProfileVM branchProfile, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = branchProfile.Id.ToString(), DataVM = branchProfile };

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
                UPDATE BranchProfiles 
                SET 
                    Code = @Code, DistributorCode = @DistributorCode, Name = @Name,BanglaName = @BanglaName, 
                    AreaId = @AreaId, TelephoneNo = @TelephoneNo, Email = @Email, VATRegistrationNo = @VATRegistrationNo, BIN = @BIN, 
                    TINNO = @TINNO, Comments = @Comments, IsArchive = @IsArchive, IsActive = @IsActiveStatus, 
                    LastModifiedBy = @LastModifiedBy, LastModifiedOn = @LastModifiedOn,Address=@Address
                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", branchProfile.Id);
                    cmd.Parameters.AddWithValue("@Code", branchProfile.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DistributorCode", branchProfile.DistributorCode);
                    cmd.Parameters.AddWithValue("@Name", branchProfile.Name);
                    cmd.Parameters.AddWithValue("@BanglaName", branchProfile.BanglaName);
                    //cmd.Parameters.AddWithValue("@ParentId", branchProfile.ParentId);
                    //cmd.Parameters.AddWithValue("@EnumTypeId", branchProfile.EnumTypeId);
                    cmd.Parameters.AddWithValue("@TelephoneNo", branchProfile.TelephoneNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", branchProfile.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VATRegistrationNo", branchProfile.VATRegistrationNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BIN", branchProfile.BIN ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TINNO", branchProfile.TINNO ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", branchProfile.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comments", branchProfile.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", branchProfile.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActiveStatus", branchProfile.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", branchProfile.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", branchProfile.LastModifiedOn ?? (object)DBNull.Value);

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

                string query = $"UPDATE BranchProfiles SET IsArchive = 1, IsActive = 0,LastModifiedBy = @LastModifiedBy,LastUpdateFrom = @LastUpdateFrom ,LastModifiedOn =GETDATE() WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    //string ids = string.Join(",", IDs);
                    //cmd.Parameters.AddWithValue("@Ids", ids);
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
    ISNULL(H.Id, 0) AS Id
";
                if (vm != null && !string.IsNullOrEmpty(vm.UserLogInId))
                {
                    query += @"
    ,@UserId AS UserId";
                }
                else {
                    query += @"
    ,'' AS UserId";
                }

query += @"
    
    ,ISNULL(H.Code, '') AS Code
    ,ISNULL(H.DistributorCode, '') AS DistributorCode
    ,ISNULL(H.Name, '') AS Name
    ,ISNULL(H.BanglaName, '') AS BanglaName
    ,ISNULL(H.AreaId, 0) AS AreaId
	
    ,ISNULL(H.TelephoneNo, '') AS TelephoneNo
    ,ISNULL(H.Address, '') AS Address
    ,ISNULL(H.Email, '') AS Email
    ,ISNULL(H.VATRegistrationNo, '') AS VATRegistrationNo
    ,ISNULL(H.BIN, '') AS BIN
    --,ISNULL(H.TINNO, '') AS TINNO
    ,ISNULL(H.Comments, '') AS Comments
    ,ISNULL(H.IsArchive, 0) AS IsArchive
    ,ISNULL(H.IsActive, 0) AS IsActive
    ,ISNULL(H.CreatedBy, '') AS CreatedBy
    ,ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
    ,ISNULL(H.LastModifiedBy, '') AS LastModifiedBy
    ,ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
    ,ISNULL(H.CreatedFrom, '') AS CreatedFrom
    ,ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    BranchProfiles H
	
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
                if (vm != null && !string.IsNullOrEmpty(vm.UserLogInId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@UserId", vm.UserLogInId);
                }
                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new BranchProfileVM
                {
                    
                Id = Convert.ToInt32(row["Id"]),
                    Code = row["Code"].ToString(),
                    UserId = row["UserId"].ToString(),
                    DistributorCode = row["DistributorCode"].ToString(),
                    Name = row["Name"].ToString(),
                    BanglaName = row["BanglaName"].ToString(),
                    TelephoneNo = row["TelephoneNo"].ToString(),
                    Email = row["Email"].ToString(),
                    Address = row["Address"].ToString(),
                    VATRegistrationNo = row["VATRegistrationNo"].ToString(),
                    BIN = row["BIN"].ToString(),
                    Comments = row["Comments"].ToString(),
                    IsArchive = Convert.ToBoolean(row["IsArchive"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedBy = row["CreatedBy"].ToString(),
                    LastModifiedBy = row["LastModifiedBy"].ToString(),
                    CreatedOn = row["CreatedOn"].ToString(),
                    LastModifiedOn = row["LastModifiedOn"].ToString(),
                    CreatedFrom = row["CreatedFrom"].ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"].ToString(),
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;

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

        public async Task<ResultVM> UserWiseBranchList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(H.Id, 0) AS Id
";
                if (vm != null && !string.IsNullOrEmpty(vm.UserLogInId))
                {
                    query += @"
    ,@UserId AS UserId";
                }
                else
                {
                    query += @"
    ,'' AS UserId";
                }

                query += @"
    

	,ISNULL(H.Code, '') AS Code
    ,ISNULL(H.DistributorCode, '') AS DistributorCode
    ,ISNULL(H.Name, '') AS Name
    ,ISNULL(H.BanglaName, '') AS BanglaName
    ,ISNULL(H.AreaId, 0) AS AreaId
	
    ,ISNULL(H.TelephoneNo, '') AS TelephoneNo
    ,ISNULL(H.Address, '') AS Address
    ,ISNULL(H.Email, '') AS Email
    ,ISNULL(H.VATRegistrationNo, '') AS VATRegistrationNo
    ,ISNULL(H.BIN, '') AS BIN
    --,ISNULL(H.TINNO, '') AS TINNO
    ,ISNULL(H.Comments, '') AS Comments
    ,ISNULL(H.IsArchive, 0) AS IsArchive
    ,ISNULL(H.IsActive, 0) AS IsActive
    ,ISNULL(H.CreatedBy, '') AS CreatedBy
    ,ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn
    ,ISNULL(H.LastModifiedBy, '') AS LastModifiedBy
    ,ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn
    ,ISNULL(H.CreatedFrom, '') AS CreatedFrom
    ,ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
FROM [dbo].[UserBranchMap] b
LEFT OUTER JOIN BranchProfiles H on b.BranchId=H.Id
	
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
                if (vm != null && !string.IsNullOrEmpty(vm.UserLogInId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@UserId", vm.UserLogInId);
                }
                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new BranchProfileVM
                {

                    Id = Convert.ToInt32(row["Id"]),
                    Code = row["Code"].ToString(),
                    UserId = row["UserId"].ToString(),
                    DistributorCode = row["DistributorCode"].ToString(),
                    Name = row["Name"].ToString(),
                    BanglaName = row["BanglaName"].ToString(),
                    TelephoneNo = row["TelephoneNo"].ToString(),
                    Email = row["Email"].ToString(),
                    Address = row["Address"].ToString(),
                    VATRegistrationNo = row["VATRegistrationNo"].ToString(),
                    BIN = row["BIN"].ToString(),
                    Comments = row["Comments"].ToString(),
                    IsArchive = Convert.ToBoolean(row["IsArchive"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedBy = row["CreatedBy"].ToString(),
                    LastModifiedBy = row["LastModifiedBy"].ToString(),
                    CreatedOn = row["CreatedOn"].ToString(),
                    LastModifiedOn = row["LastModifiedOn"].ToString(),
                    CreatedFrom = row["CreatedFrom"].ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"].ToString(),
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;

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
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.AdvanceEntryDate, '1900-01-01') AS AdvanceEntryDate,
    ISNULL(M.AdvanceAmount, 0.0) AS AdvanceAmount,
    ISNULL(M.PaymentEnumTypeId, 0) AS PaymentEnumTypeId,
    ISNULL(M.PaymentDate, '1900-01-01') AS PaymentDate,
    ISNULL(M.DocumentNo, '') AS DocumentNo,
    ISNULL(M.BankName, '') AS BankName,
    ISNULL(M.BankBranchName, '') AS BankBranchName,
    ISNULL(M.ReceiveByEnumTypeId, 0) AS ReceiveByEnumTypeId,
    ISNULL(M.IsApproved, 0) AS IsApproved,
    ISNULL(M.ApproveById, 0) AS ApproveById,
    ISNULL(M.ApproveDate, '1900-01-01') AS ApproveDate,
    ISNULL(M.IsPosted, 0) AS IsPosted,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(M.PostedDate, '1900-01-01') AS PostedDate
FROM BranchAdvances M
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
                SELECT Id, Name
                FROM BranchAdvances
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

                var data = new GridEntity<BranchProfileVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
                    SELECT COUNT(DISTINCT H.Id) AS totalcount
                    FROM BranchProfiles H
						left outer join Areas A on A.Id=h.AreaId
	--left outer join EnumTypes EParent on EParent.Id=H.ParentId
	--left outer join EnumTypes EEnumType on EEnumType.Id=H.EnumTypeId
                    WHERE H.IsArchive != 1
                    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

                    -- Data query with pagination and sorting
                    SELECT * 
                    FROM (
                        SELECT 
                         ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex
   
                        ,ISNULL(H.Id, 0) AS Id
                        ,ISNULL(H.Code, '') AS Code
                        ,ISNULL(H.DistributorCode, '') AS DistributorCode
                        ,ISNULL(H.Name, '') AS Name
                        ,ISNULL(H.BanglaName, '') AS BanglaName
                        --,ISNULL(H.ParentId, 0) AS ParentId    
                        --,ISNULL(EParent.Name,'N/A') AS EnumType
                        
                        --,ISNULL(H.CurrencyId, 0) AS CurrencyId
                        --,ISNULL(H.EnumTypeId, 0) AS EnumTypeId
                        --,ISNULL(EEnumType.Name,'N/A') AS EnumName
                        ,ISNULL(H.AreaId, 0) AS AreaId
                        ,ISNULL(A.Name, 'N/A') AS AreaName
				        ,ISNULL(H.TelephoneNo,'') TelephoneNo
				        ,ISNULL(H.Email,'') Email
				        ,ISNULL(H.VATRegistrationNo,'''') VATRegistrationNo
				        ,ISNULL(H.BIN,'') BIN
				        ,ISNULL(H.TINNO,'') TINNO
				        ,ISNULL(H.Comments,'') Comments
				        ,ISNULL(H.IsArchive,0)	IsArchive
				        ,ISNULL(H.IsActive,0)	IsActive
				        ,CASE WHEN ISNULL(H.IsActive,0) = 1 THEN 'Active' ELSE 'Inactive'	END Status
				        ,ISNULL(H.CreatedBy,'') CreatedBy
				        ,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				        ,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				        ,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn

                    FROM BranchProfiles H
						left outer join Areas A on A.Id=h.AreaId
	--left outer join EnumTypes EParent on EParent.Id=H.ParentId
	--left outer join EnumTypes EEnumType on EEnumType.Id=H.EnumTypeId
                    WHERE H.IsArchive != 1
                  
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<BranchProfileVM>.FilterCondition(options.filter) + ")" : "") + @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<BranchProfileVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

                        ISNULL(H.Id, 0) AS Id
                        ,ISNULL(H.Code, '') AS Code
                        ,ISNULL(H.DistributorCode, '') AS DistributorCode
                        ,ISNULL(H.Name, '') AS Name
                        ,ISNULL(H.ParentId, 0) AS ParentId    
                        ,ISNULL(EParent.Name,'N/A') AS EnumType
                        
                        ,ISNULL(H.CurrencyId, 0) AS CurrencyId
                        ,ISNULL(H.EnumTypeId, 0) AS EnumTypeId
                        ,ISNULL(EEnumType.Name,'N/A') AS EnumName
                        ,ISNULL(H.AreaId, 0) AS AreaId
                        ,ISNULL(A.Name, 'N/A') AS AreaName
				        ,ISNULL(H.TelephoneNo,'') TelephoneNo
				        ,ISNULL(H.Email,'') Email
				        ,ISNULL(H.VATRegistrationNo,'''') VATRegistrationNo
				        ,ISNULL(H.BIN,'') BIN
				        ,ISNULL(H.TINNO,'') TINNO
				        ,ISNULL(H.CreatedBy,'') CreatedBy
				        ,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				        ,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				        ,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn

                       FROM BranchProfiles H
						left outer join Areas A on A.Id=H.AreaId
	                    left outer join EnumTypes EParent on EParent.Id=H.ParentId
	                    left outer join EnumTypes EEnumType on EEnumType.Id=H.EnumTypeId

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

        public async Task<DataTable> GetProductList(SqlConnection conn, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();

            string sqlText = @"
               WITH LatestEffectDatePerName AS (
    SELECT 
        Name,
        MAX(EffectDate) AS LatestEffectDate
    FROM ProductPriceGroups
    GROUP BY Name
)
SELECT  
    d.ProductId,
    --0 AS BranchId,                
    NULL AS BatchNo,                      
    h.Name AS PriceCategory,              
    GETDATE() AS EntryDate,              
    h.EffectDate,                         
    NULL AS MFGDate,                      
    NULL AS EXPDate,                     
    0.00 AS SD,                           
    NULL AS SDAmount,                     
    d.VATRate,
    NULL AS VATAmount,                    
    d.CosePrice AS CostPrice,
    d.SalePrice AS SalesPrice,
    0.00 AS PurchasePrice,                
    --@CreatedBy AS CreatedBy,             
    GETDATE() AS CreatedOn,              
    NULL AS LastModifiedBy,
    NULL AS LastModifiedOn,
    --@CreatedFrom AS CreatedFrom,          
    NULL AS LastUpdateFrom
FROM ProductPriceGroupDetails d
INNER JOIN ProductPriceGroups h 
    ON h.Id = d.ProductPriceGroupId
INNER JOIN LatestEffectDatePerName l 
    ON h.Name = l.Name AND h.EffectDate = l.LatestEffectDate
WHERE h.IsActive = 1
ORDER BY h.Name;";

            using (var command = CreateCommand(sqlText, conn, transaction))
            {
                //command.Parameters.AddWithValue("@BranchId", branchAdvance.Id);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable)); // Fill DataTable
                }
            }

            return dataTable;
        }

        public async Task<DataTable> GetPurchaseList(SqlConnection conn, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();

            string sqlText = @"
               SELECT 
    Id,
    ProductId,
    BranchId,
    BatchNo,
    EntryDate,
    EffectDate,
    MFGDate,
    EXPDate,
    SD,
    SDAmount,
    VATRate,
    VATAmount,
    CostPrice,
    SalesPrice,
    PurchasePrice,
    CreatedBy,
    CreatedOn,
    LastModifiedBy,
    LastModifiedOn,
    CreatedFrom,
    LastUpdateFrom
FROM [ProductPurchasePriceBatchHistories]
WHERE BranchId = (
    SELECT TOP 1 BranchId
    FROM [ProductPurchasePriceBatchHistories]
    ORDER BY BranchId
)
AND EffectDate = (
    SELECT MAX(EffectDate)
    FROM [ProductPurchasePriceBatchHistories]
    WHERE BranchId = (
        SELECT TOP 1 BranchId
        FROM [ProductPurchasePriceBatchHistories]
        ORDER BY BranchId
    )
);";

            using (var command = CreateCommand(sqlText, conn, transaction))
            {
                //command.Parameters.AddWithValue("@BranchId", branchAdvance.Id);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable)); // Fill DataTable
                }
            }

            return dataTable;
        }
    }


}
