using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static ShampanPOS.ViewModel.KendoCommon.UtilityCommon;

namespace ShampanPOS.Repository
{
   

    public class MenuAuthorizationRepository : CommonRepository
    {

        //Insert Method
        public async Task<ResultVM> Insert(UserRoleMVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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


                string query = @" INSERT INTO [dbo].[Role] (



 Name
,CreatedBy
,CreatedOn
,CreatedFrom
) VALUES (

 @Name
,@CreatedBy
,@CreatedOn
,@CreatedFrom

); 
SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);

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

        public async Task<ResultVM> Update(UserRoleMVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @"  UPDATE [dbo].[Role] SET  

 Name=@Name
,LastModifiedBy=@LastModifiedBy
,LastModifiedOn=@LastModifiedOn
,LastUpdateFrom=@LastUpdateFrom
                       
WHERE  Id = @Id ";
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@Name", vm.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

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
        public async Task<ResultVM> GetRoleIndexData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<UserRoleMVM>();

                // Define your SQL query string
                string sqlQuery = @"
                                 -- Count query
                  SELECT COUNT(DISTINCT H.Id) AS totalcount
					FROM Role H
					WHERE 1 = 1

                  -- Add the filter condition
                  " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserRoleMVM>.FilterCondition(options.filter) + ")" : "") + @"

                  -- Data query with pagination and sorting
                   SELECT * 
                  FROM (
                      SELECT 
                     ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex
			    ,ISNULL(H.Id,0)	Id
				,ISNULL(H.Name,'') Name
				,ISNULL(H.CreatedBy,'') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn

				FROM Role H 
				WHERE 1 = 1

                  -- Add the filter condition
                  " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<UserRoleMVM>.FilterCondition(options.filter) + ")" : "") + @"

                  ) AS a
                  WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
            ";

                data = KendoGrid<UserRoleMVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> RoleMenuDelete(RoleMenuVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @"DELETE FROM RoleMenu WHERE RoleId = @RolesId";
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@RolesId", SqlDbType.NChar).Value = vm.RoleId;
                    int res = cmd.ExecuteNonQuery();

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
                throw ex;
            }

        }
        public async Task<ResultVM> RoleMenuInsert(RoleMenuVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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


                string query = @" INSERT INTO RoleMenu (



 RoleId
,MenuId
,List
,[Insert]
,[Delete]
,Post
,CreatedBy
,CreatedOn
,CreatedFrom
,LastModifiedBy
,LastModifiedOn
,LastUpdateFrom

) VALUES (

 @RoleId
,@MenuId
,@List
,@Insert
,@Delete
,@Post
,@CreatedBy
,@CreatedOn
,@CreatedFrom
,@LastModifiedBy
,@LastModifiedOn
,@LastUpdateFrom



); 
SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@RoleId", SqlDbType.NChar).Value = vm.RoleId;
                    cmd.Parameters.AddWithValue("@MenuId", SqlDbType.NChar).Value = vm.MenuId;
                    cmd.Parameters.AddWithValue("@List", SqlDbType.NChar).Value = vm.List;
                    cmd.Parameters.AddWithValue("@Insert", SqlDbType.NChar).Value = vm.Insert;
                    cmd.Parameters.AddWithValue("@Delete", SqlDbType.NChar).Value = vm.Delete;
                    cmd.Parameters.AddWithValue("@Post", SqlDbType.NChar).Value = vm.Post;


                    cmd.Parameters.AddWithValue("@CreatedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedBy) ? (object)DBNull.Value : vm.CreatedBy.Trim();
                    cmd.Parameters.AddWithValue("@CreatedOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedOn.ToString()) ? (object)DBNull.Value : vm.CreatedOn.ToString();
                    cmd.Parameters.AddWithValue("@CreatedFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedFrom) ? (object)DBNull.Value : vm.CreatedFrom.Trim();

                    cmd.Parameters.AddWithValue("@LastModifiedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.LastModifiedBy) ? (object)DBNull.Value : vm.LastModifiedBy.Trim();
                    cmd.Parameters.AddWithValue("@LastModifiedOn", SqlDbType.VarChar).Value = string.IsNullOrEmpty(vm.LastModifiedOn) ? (object)DBNull.Value : vm.LastModifiedOn;
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.LastUpdateFrom) ? (object)DBNull.Value : vm.LastUpdateFrom.Trim();

                

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
    ISNULL(H.Id, 0) AS Id,
    ISNULL(H.Name, '') AS Name,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
    ISNULL(H.CreatedFrom, '') AS CreatedFrom,
    ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Role H
WHERE 
    1 = 1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);


                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserRoleVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    CreatedBy = row["CreatedBy"]?.ToString(),
                    CreatedOn = row["CreatedOn"]?.ToString(),
                    LastModifiedBy = row["LastModifiedBy"]?.ToString(),
                    LastModifiedOn = row["LastModifiedOn"]?.ToString(),
                    CreatedFrom = row["CreatedFrom"]?.ToString(),
                    LastUpdateFrom = row["LastUpdateFrom"]?.ToString(),

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

        public async Task<ResultVM> GetMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
            DECLARE @Id INT = @RoleId;

            WITH MenuHierarchy AS (
                SELECT 
                    Id, 
                    [Name], 
                    ParentId, 
                    CAST([Name] AS NVARCHAR(MAX)) AS MenuName
                FROM 
                    Menu
                WHERE  
                    IsActive = 1 AND ParentId = 0
                UNION ALL
                SELECT 
                    m.Id, 
                    m.[Name], 
                    m.ParentId, 
                    CAST(mc.MenuName + ' > ' + m.[Name] AS NVARCHAR(MAX)) AS MenuName
                FROM 
                    Menu m    
                INNER JOIN 
                    MenuHierarchy mc ON m.ParentId = mc.Id
                WHERE  
                    m.IsActive = 1 
            ),
            MenuData AS (
                SELECT 
                    1 AS IsChecked,
                    RM.RoleId,
                    RM.MenuId,
                    RM.List,
                    RM.[Insert],
                    RM.[Delete],
                    RM.Post,
                    M.ParentId,
                    M.DisplayOrder,
                    ISNULL(M.[Url], '') AS [Url],
                    MH.MenuName,
                    ISNULL(M.Controller, '') AS Controller
                FROM 
                    RoleMenu RM
                LEFT OUTER JOIN  
                    [dbo].[Menu] M ON RM.MenuId = M.Id
                JOIN
                    MenuHierarchy MH ON RM.MenuId = MH.Id
                WHERE 
                    M.IsActive = 1 AND RM.RoleId = @Id

                UNION ALL

                SELECT 
                    0 AS IsChecked,
                    0 AS RoleId,
                    M.Id AS MenuId,
                    0 AS List,
                    0 AS [Insert],
                    0 AS [Delete],
                    0 AS Post,
                    M.ParentId,
                    M.DisplayOrder,
                    ISNULL(M.[Url], '') AS [Url],
                    MH.MenuName,
                    ISNULL(M.Controller, '') AS Controller
                FROM 
                    [dbo].[Menu] M
                JOIN
                    MenuHierarchy MH ON M.Id = MH.Id
                WHERE 
                    M.IsActive = 1 AND M.Id NOT IN (SELECT MenuId FROM RoleMenu WHERE RoleId = @Id)
            )

            SELECT DISTINCT
                MenuId AS Id,
                IsChecked,
                RoleId,
                '0' AS UserGroupId,
                MenuId,
                ParentId,
                Url,
                MenuName,
                Controller,
                DisplayOrder,
                MAX(List) AS List,
                MAX([Insert]) AS [Insert],
                MAX([Delete]) AS [Delete],
                MAX(Post) AS Post
            FROM 
                MenuData 
            GROUP BY 
                IsChecked,
                RoleId,
                MenuId,
                ParentId,
                Url,
                MenuName,
                Controller,
                DisplayOrder
        ";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

       
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@RoleId", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new RoleMenuVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IsChecked = Convert.ToBoolean(row["IsChecked"]),
                    RoleId = row["RoleId"].ToString(),
                    //UserGroupId = row["UserGroupId"].ToString(),
                    MenuId = Convert.ToInt32(row["MenuId"]),
                    ParentId = Convert.ToInt32(row["ParentId"]),
                    Url = row["Url"].ToString(),
                    MenuName = row["MenuName"].ToString(),
                    Controller = row["Controller"].ToString(),
                    //DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
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

        public async Task<ResultVM> GetUserMenuIndexData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var auhConn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                string auithDBName = auhConn.Database;


                var data = new GridEntity<UserMenuVM>();

                // Define your SQL query string
                string sqlQuery = $@"
                    -- Create temporary table
                    CREATE TABLE #Temp (
                        UserId VARCHAR(255),
                        RoleId VARCHAR(255),
                        RoleName VARCHAR(255),
                        FullName VARCHAR(255)
                    );

                    -- Populate the table
                    INSERT INTO #Temp (UserId, RoleId, RoleName, FullName)
                    SELECT DISTINCT UM.UserId, UM.RoleId, R.Name AS RoleName, '' AS FullName
                    FROM [dbo].[UserMenu] UM
                    LEFT OUTER JOIN [dbo].[Role] R ON UM.RoleId = R.Id
                    WHERE 1 = 1
                    UNION ALL
                    SELECT UM.UserName AS UserId, '0' AS RoleId, '' AS RoleName, UM.FullName
                    FROM [{auithDBName}].[dbo].[AspNetUsers] UM
                    WHERE UM.UserName NOT IN (
                        SELECT DISTINCT UserId FROM [dbo].[UserMenu]
                    )
                    AND 1 = 1;

                    -- Get total count
                    SELECT COUNT(*) AS totalcount 
                    FROM #Temp
                    WHERE 1 = 1
                    " + (options.filter.Filters.Count > 0
                                         ? " AND (" + GridQueryBuilder<UserMenuVM>.FilterCondition(options.filter) + ")"
                                         : "") + @" ;

                    -- Get paged data
                    SELECT * 
                    FROM (
                        SELECT 
                            ROW_NUMBER() OVER (ORDER BY " + (options.sort.Count > 0
                                                 ? options.sort[0].field + " " + options.sort[0].dir
                                                 : "RoleId ASC") + @") AS rowindex,
                            *
                        FROM #Temp
                        WHERE 1 = 1
                        " + (options.filter.Filters.Count > 0
                                             ? " AND (" + GridQueryBuilder<UserMenuVM>.FilterCondition(options.filter) + ")"
                                             : "") + @"
                    ) AS a
                    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take);

                    -- Clean up
                    DROP TABLE #Temp;
                ";

                data = KendoGrid<UserMenuVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> GetUserMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                var auhConn = new SqlConnection(DatabaseHelper.GetAuthConnectionString());
                string auithDBName = auhConn.Database;

                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = $@"
            DECLARE @Id INT = @RoleId;
--DECLARE @UserId NVARCHAR(50) = 'ERP'; -- Example UserId

WITH MenuHierarchy AS (
    SELECT 
        Id, 
        [Name], 
        ParentId, 
        CAST([Name] AS NVARCHAR(MAX)) AS MenuName
    FROM 
        Menu
    WHERE  
        IsActive = 1 AND ParentId = 0 

    UNION ALL

    SELECT 
        m.Id, 
        m.[Name], 
        m.ParentId, 
        CAST(mc.MenuName + ' > ' + m.[Name] AS NVARCHAR(MAX)) AS MenuName
    FROM 
        Menu m
    INNER JOIN 
        MenuHierarchy mc ON m.ParentId = mc.Id
    WHERE 
        m.IsActive = 1 
),
MenuData AS (
    SELECT 
        CAST(1 AS BIT) AS IsChecked,
        UM.RoleId,
        UM.MenuId,
        M.ParentId,
        ISNULL(M.[Url], '') AS [Url],
        MH.MenuName,
        ISNULL(M.Controller, '') AS Controller,
        M.DisplayOrder,
        CAST(UM.List AS INT) AS List,
        CAST(UM.[Insert] AS INT) AS [Insert],
        CAST(UM.[Delete] AS INT) AS [Delete],
        CAST(UM.Post AS INT) AS Post
    FROM 
        UserMenu UM   
    LEFT JOIN  
        [dbo].[Menu] M ON UM.MenuId = M.Id
    LEFT JOIN  
        [dbo].[Role] R ON UM.RoleId = R.Id
    LEFT JOIN  
         [{auithDBName}].[dbo].[AspNetUsers] U ON UM.UserId = U.UserName
    JOIN
        MenuHierarchy MH ON UM.MenuId = MH.Id
    WHERE 
        M.IsActive = 1 
        AND UM.RoleId = @Id 
        AND UM.UserId = @UserId 

    UNION ALL

    SELECT 
        CAST(0 AS BIT) AS IsChecked,
        CAST(0 AS INT) AS RoleId,
        M.Id AS MenuId,
        M.ParentId,
        ISNULL(M.[Url], '') AS [Url],
        MH.MenuName,
        ISNULL(M.Controller, '') AS Controller,
        M.DisplayOrder,
        0 AS List,
        0 AS [Insert],
        0 AS [Delete],
        0 AS Post
    FROM 
        [dbo].[Menu] M
    JOIN
        MenuHierarchy MH ON M.Id = MH.Id
    WHERE 
        M.IsActive = 1 
        AND M.Id NOT IN (
            SELECT MenuId 
            FROM UserMenu 
            WHERE RoleId = @Id 
              AND UserId = @UserId
        )
)

SELECT DISTINCT
    MenuId AS Id,
    IsChecked,
    RoleId,
    MenuId,
    ParentId,
    Url,
    MenuName,
    Controller,
    DisplayOrder,
    MAX(List) AS List,
    MAX([Insert]) AS [Insert],
    MAX([Delete]) AS [Delete],
    MAX(Post) AS Post
FROM 
    MenuData
GROUP BY 
    IsChecked,
    RoleId,
    MenuId,
    ParentId,
    Url,
    MenuName,
    Controller,
    DisplayOrder
ORDER BY 
    DisplayOrder;

        ";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@RoleId", vm.Id);
                    objComm.SelectCommand.Parameters.AddWithValue("@UserId", vm.UserLogInId);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserMenuVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IsChecked = Convert.ToBoolean(row["IsChecked"]),
                    RoleId = row["RoleId"].ToString(),
                    //UserGroupId = row["UserGroupId"].ToString(),
                    MenuId = Convert.ToInt32(row["MenuId"]),
                    ParentId = Convert.ToInt32(row["ParentId"]),
                    Url = row["Url"].ToString(),
                    MenuName = row["MenuName"].ToString(),
                    Controller = row["Controller"].ToString(),
                    //DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
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
        //public async Task<ResultVM> GetMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        //{
        //    bool isNewConnection = false;
        //    DataTable dataTable = new DataTable();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        if (conn == null)
        //        {
        //            conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //            conn.Open();
        //            isNewConnection = true;
        //        }

        //        string query = @"

        //         -- DECLARE @Id INT = @RoleId;

        //        WITH MenuHierarchy AS (
        //            SELECT 
        //                Id, 
        //                [Name], 
        //                ParentId, 
        //                CAST([Name] AS NVARCHAR(MAX)) AS MenuName
        //            FROM 
        //                Menu
        //            WHERE  
        //                IsActive = 1 AND ParentId = 0
        //            UNION ALL
        //            SELECT 
        //                m.Id, 
        //                m.[Name], 
        //                m.ParentId, 
        //                CAST(mc.MenuName + ' > ' + m.[Name] AS NVARCHAR(MAX)) AS MenuName
        //            FROM 
        //                Menu m    
        //            INNER JOIN 
        //                MenuHierarchy mc ON m.ParentId = mc.Id
        //            WHERE  
        //                m.IsActive = 1 
        //        ),
        //        MenuData AS (
        //            SELECT 
        //                1 AS IsChecked,
        //                RM.RoleId,
        //                RM.MenuId,
        //                RM.List,
        //                RM.[Insert],
        //                RM.[Delete],
        //                RM.Post,
        //                M.ParentId,
        //                M.DisplayOrder,
        //                ISNULL(M.[Url], '') AS [Url],
        //                MH.MenuName,
        //                ISNULL(M.Controller, '') AS Controller
        //            FROM 
        //                RoleMenu RM
        //            LEFT OUTER JOIN  
        //                [dbo].[Menu] M ON RM.MenuId = M.Id
        //            JOIN
        //                MenuHierarchy MH ON RM.MenuId = MH.Id
        //            WHERE 
        //                M.IsActive = 1 AND RM.RoleId = @Id

        //            UNION ALL

        //            SELECT 
        //                0 AS IsChecked,
        //                0 AS RoleId,
        //                M.Id AS MenuId,
        //                0 AS List,
        //                0 AS [Insert],
        //                0 AS [Delete],
        //                0 AS Post,
        //                M.ParentId,
        //                M.DisplayOrder,
        //                ISNULL(M.[Url], '') AS [Url],
        //                MH.MenuName,
        //                ISNULL(M.Controller, '') AS Controller
        //            FROM 
        //                [dbo].[Menu] M
        //            JOIN
        //                MenuHierarchy MH ON M.Id = MH.Id
        //            WHERE 
        //                M.IsActive = 1 AND M.Id NOT IN (SELECT MenuId FROM RoleMenu WHERE RoleId = @Id)
        //        )

        //        SELECT DISTINCT
        //            MenuId AS Id,
        //            IsChecked,
        //            RoleId,
        //            '0' AS UserGroupId,
        //            MenuId,
        //            ParentId,
        //            Url,
        //            MenuName,
        //            Controller,
        //            DisplayOrder,
        //            MAX(List) AS List,
        //            MAX([Insert]) AS [Insert],
        //            MAX([Delete]) AS [Delete],
        //            MAX(Post) AS Post
        //        FROM 
        //            MenuData 
        //        GROUP BY 
        //            IsChecked,
        //            RoleId,
        //            MenuId,
        //            ParentId,
        //            Url,
        //            MenuName,
        //            Controller,
        //            DisplayOrder
        //       ";




        //        //if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        //{
        //        //    query += " AND RoleId = @RoleId ";
        //        //}

        //        query = ApplyConditions(query, conditionalFields, conditionalValues, false);


        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            objComm.SelectCommand.Parameters.AddWithValue("@RoleId", vm.Id);
        //        }

        //        objComm.Fill(dataTable);

        //        var modelList = dataTable.AsEnumerable().Select(row => new RoleMenuVM
        //        {





        //            Id = Convert.ToInt32(row["Id"]),
        //            IsChecked = Convert.ToBoolean(row["IsChecked"]),
        //            RoleId = row["RoleId"].ToString(),
        //            UserGroupId = row["UserGroupId"].ToString(),
        //            MenuId = Convert.ToInt32(row["MenuId"]),
        //            ParentId = Convert.ToInt32(row["ParentId"]),
        //            Url = row["Url"].ToString(),
        //            MenuName = row["MenuName"].ToString(),
        //            Controller = row["Controller"].ToString(),
        //            DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),



        //        }).ToList();

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = modelList;

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
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

        public async Task<ResultVM> UserMenuDelete(UserMenuVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string query = @"DELETE FROM UserMenu WHERE UserId = @UsersId";
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.Add("@UsersId", SqlDbType.NChar).Value = vm.UserId;
                    int res = cmd.ExecuteNonQuery();

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
                throw ex;
            }

        }
        public async Task<ResultVM> UserMenuInsert(UserMenuVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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


                string query = @" INSERT INTO UserMenu (



                 UserId
                 ,RoleId
                ,MenuId
                ,List
                ,[Insert]
                ,[Delete]
                ,Post
                ,CreatedBy
                ,CreatedOn
                ,CreatedFrom
                ,LastModifiedBy
                ,LastModifiedOn
                ,LastUpdateFrom


                ) VALUES (
                @UserId
                ,@RoleId
                ,@MenuId
                ,@List
                ,@Insert
                ,@Delete
                ,@Post
                ,@CreatedBy
                ,@CreatedOn
                ,@CreatedFrom
                ,@LastModifiedBy
                ,@LastModifiedOn
                ,@LastUpdateFrom



); 
SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@UserId", SqlDbType.NChar).Value = vm.UserId;
                    cmd.Parameters.AddWithValue("@RoleId", SqlDbType.NChar).Value = vm.RoleId;
                    cmd.Parameters.AddWithValue("@MenuId", SqlDbType.NChar).Value = vm.MenuId;
                    cmd.Parameters.AddWithValue("@List", SqlDbType.NChar).Value = vm.List;
                    cmd.Parameters.AddWithValue("@Insert", SqlDbType.NChar).Value = vm.Insert;
                    cmd.Parameters.AddWithValue("@Delete", SqlDbType.NChar).Value = vm.Delete;
                    cmd.Parameters.AddWithValue("@Post", SqlDbType.NChar).Value = vm.Post;


                    cmd.Parameters.AddWithValue("@CreatedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedBy) ? (object)DBNull.Value : vm.CreatedBy.Trim();
                    cmd.Parameters.AddWithValue("@CreatedOn", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedOn.ToString()) ? (object)DBNull.Value : vm.CreatedOn.ToString();
                    cmd.Parameters.AddWithValue("@CreatedFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.CreatedFrom) ? (object)DBNull.Value : vm.CreatedFrom.Trim();

                    cmd.Parameters.AddWithValue("@LastModifiedBy", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.LastModifiedBy) ? (object)DBNull.Value : vm.LastModifiedBy.Trim();
                    cmd.Parameters.AddWithValue("@LastModifiedOn", SqlDbType.VarChar).Value = string.IsNullOrEmpty(vm.LastModifiedOn) ? (object)DBNull.Value : vm.LastModifiedOn;
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", SqlDbType.NChar).Value = string.IsNullOrEmpty(vm.LastUpdateFrom) ? (object)DBNull.Value : vm.LastUpdateFrom.Trim();




                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());

                    result.Status = "Success";
                    result.Message = "Data Submited successfully.";
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

        public async Task<ResultVM> GetUserRoleWiseMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
            DECLARE @Id INT = @RoleId;
WITH MenuHierarchy AS (
    SELECT 
        Id, 
        [Name], 
        ParentId, 
        CAST([Name] AS NVARCHAR(MAX)) AS MenuName
    FROM 
        Menu
    WHERE  
        IsActive = 1 AND ParentId = 0 -- Top-level menus
    UNION ALL
    SELECT 
        m.Id, 
        m.[Name], 
        m.ParentId, 
        CAST(mc.MenuName + ' > ' + m.[Name] AS NVARCHAR(MAX)) AS MenuName
    FROM 
        Menu m
    INNER JOIN 
        MenuHierarchy mc ON m.ParentId = mc.Id
    WHERE 
        m.IsActive = 1
),
MenuData AS (
    SELECT 
        CAST(1 AS BIT) AS IsChecked,
        RM.RoleId,
        RM.MenuId,
        CAST(RM.List AS INT) AS List,
        CAST(RM.[Insert] AS INT) AS [Insert],
        CAST(RM.[Delete] AS INT) AS [Delete],
        CAST(RM.Post AS INT) AS Post,
        M.ParentId,
        M.DisplayOrder,
        ISNULL(M.[Url],'') AS [Url],
        MH.MenuName,
        ISNULL(M.Controller,'') AS Controller
    FROM 
        RoleMenu RM
    LEFT JOIN  
        Menu M ON RM.MenuId = M.Id
    JOIN
        MenuHierarchy MH ON RM.MenuId = MH.Id
    WHERE 
        M.IsActive = 1 
        AND RM.RoleId = @Id

    UNION ALL

    SELECT 
        CAST(0 AS BIT) AS IsChecked,
        CAST(0 AS INT) AS RoleId,
        M.Id AS MenuId,
        0 AS List,
        0 AS [Insert],
        0 AS [Delete],
        0 AS Post,
        M.ParentId,
        M.DisplayOrder,
        ISNULL(M.[Url],'') AS [Url],
        MH.MenuName,
        ISNULL(M.Controller,'') AS Controller
    FROM 
        Menu M
    JOIN
        MenuHierarchy MH ON M.Id = MH.Id
    WHERE 
        M.IsActive = 1 
        AND M.Id NOT IN (SELECT MenuId FROM UserMenu WHERE RoleId = @Id)
        AND M.Id NOT IN (SELECT MenuId FROM RoleMenu WHERE RoleId = @Id)
)

SELECT 
    MenuId AS Id,
    IsChecked,
    RoleId,
    MenuId,
    ParentId,
    Url,
    MenuName,
    Controller,
    DisplayOrder,
    MAX(List) AS List,
    MAX([Insert]) AS [Insert],
    MAX([Delete]) AS [Delete],
    MAX(Post) AS Post
FROM 
    MenuData
GROUP BY 
    IsChecked,
    RoleId,
    MenuId,
    ParentId,
    Url,
    MenuName,
    Controller,
    DisplayOrder
ORDER BY 
    DisplayOrder;

        ";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);


                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@RoleId", vm.Id);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserMenuVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    IsChecked = Convert.ToBoolean(row["IsChecked"]),
                    RoleId = row["RoleId"].ToString(),
                    //UserGroupId = row["UserGroupId"].ToString(),
                    MenuId = Convert.ToInt32(row["MenuId"]),
                    ParentId = Convert.ToInt32(row["ParentId"]),
                    Url = row["Url"].ToString(),
                    MenuName = row["MenuName"].ToString(),
                    Controller = row["Controller"].ToString(),
                    //DisplayOrder = Convert.ToInt32(row["DisplayOrder"]),
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

        public async Task<ResultVM> GetRoleData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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

                string sqlQuery = @"
                    SELECT Id, Name RoleName FROM [dbo].[Role] WHERE 1 = 1
                ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UserVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    RoleName = row["RoleName"]?.ToString(),
                    

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
    }

}
