using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using Newtonsoft.Json;

namespace ShampanPOS.Repository
{
    
    public class SalePersonYearlyTargetRepository : CommonRepository
    {


        // Insert Method
        public async Task<ResultVM> Insert(SalePersonYearlyTargetVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO SalePersonYearlyTargets 
        (


SalePersonId,
BranchId, 
YearlyTarget,
SelfSaleCommissionRate, 
OtherSaleCommissionRate,
FiscalYearForSaleId, 
Year, 
YearStart, 
YearEnd,
IsApproveed,
IsPost, 
CreatedBy,
CreatedOn,
CreatedFrom


        )
        VALUES 
        (

@SalePersonId,
@BranchId,
@YearlyTarget,
@SelfSaleCommissionRate, 
@OtherSaleCommissionRate,
@FiscalYearForSaleId,
@Year, 
@YearStart, 
@YearEnd,
@IsApproveed,
@IsPost,  
@CreatedBy,
@CreatedOn,
@CreatedFrom
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {

                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@YearlyTarget", vm.YearlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@FiscalYearForSaleId", vm.FiscalYearForSaleId);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@YearStart", vm.YearStart);
                    cmd.Parameters.AddWithValue("@YearEnd", vm.YearEnd);
                    cmd.Parameters.AddWithValue("@IsApproveed", vm.IsApproveed);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom);
                    vm.Id = Convert.ToInt32(cmd.ExecuteScalar());


                    int LineNo = 1;
                    foreach (var details in vm.SalePersonYearlyTargetDetailList)
                    {

                        details.SalePersonYearlyTargetId = vm.Id;
                        details.FiscalYearDetailForSaleId = vm.Id;
                        details.FiscalYearForSaleId = vm.Id;                        
                        details.Line = LineNo;
                        details.BranchId = vm.BranchId;
                        details.SalePersonId = vm.SalePersonId;
                        details.Year = 1;
                        details.MonthId = 1;

                        var respone = InsertDetails(details, conn, transaction);

                        if (respone.Id == "0")
                        {
                            throw new Exception("Error in Insert for Details Data.");
                        }

                        LineNo++;
                    }

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


        public ResultVM InsertDetails(SalePersonYearlyTargetDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO SalePersonYearlyTargetDetails

        (

BranchId,
SalePersonId, 
SalePersonYearlyTargetId,
FiscalYearDetailForSaleId, 
FiscalYearForSaleId, 
MonthlyTarget,
SelfSaleCommissionRate,
OtherSaleCommissionRate,
Year,
MonthId,
MonthStart,
MonthEnd

)
VALUES 
(

@BranchId,
@SalePersonId, 
@SalePersonYearlyTargetId,
@FiscalYearDetailForSaleId, 
@FiscalYearForSaleId, 
@MonthlyTarget,
@SelfSaleCommissionRate,
@OtherSaleCommissionRate,
@Year,
@MonthId,
@MonthStart,
@MonthEnd

);

        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@SalePersonId", details.SalePersonId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalePersonYearlyTargetId", details.SalePersonYearlyTargetId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FiscalYearDetailForSaleId", details.FiscalYearDetailForSaleId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FiscalYearForSaleId", details.FiscalYearForSaleId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MonthlyTarget", details.MonthlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", details.SelfSaleCommissionRate);                   
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", details.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@Year", details.Year);
                    cmd.Parameters.AddWithValue("@MonthId", details.MonthId);
                    cmd.Parameters.AddWithValue("@MonthStart", details.MonthStart);
                    cmd.Parameters.AddWithValue("@MonthEnd", details.MonthEnd);


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
        

        public async Task<ResultVM> Update(SalePersonYearlyTargetVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                UPDATE SalePersonYearlyTargets 

SET 
SalePersonId = @SalePersonId, 
BranchId = @BranchId, 
YearlyTarget = @YearlyTarget, 
SelfSaleCommissionRate = @SelfSaleCommissionRate, 
OtherSaleCommissionRate = @OtherSaleCommissionRate, 
FiscalYearForSaleId = @FiscalYearForSaleId,
Year = @Year,
YearStart = @YearStart, 
YearEnd = @YearEnd,
IsApproveed = @IsApproveed, 
IsPost = @IsPost, 
LastModifiedBy = @LastModifiedBy,
LastUpdateFrom = @LastUpdateFrom,
LastModifiedOn = GETDATE()

                WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@YearlyTarget", vm.YearlyTarget);
                    cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                    cmd.Parameters.AddWithValue("@FiscalYearForSaleId", vm.FiscalYearForSaleId);
                    cmd.Parameters.AddWithValue("@Year", vm.Year);
                    cmd.Parameters.AddWithValue("@YearStart", vm.YearStart);
                    cmd.Parameters.AddWithValue("@YearEnd", vm.YearEnd);
                    cmd.Parameters.AddWithValue("@IsApproveed", vm.IsApproveed);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);

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

                int LineNo = 1;
                foreach (var details in vm.SalePersonYearlyTargetDetailList)
                {
                    details.SalePersonYearlyTargetId = vm.Id;
                    details.FiscalYearDetailForSaleId = vm.Id;
                    details.FiscalYearForSaleId = vm.Id;
                    details.Line = LineNo;
                    details.BranchId = vm.BranchId;
                    details.SalePersonId = vm.SalePersonId;
                    details.Year = 1;
                    details.MonthId = 1;

                    var respone = InsertDetails(details, conn, transaction);

                    if (respone.Id == "0")
                    {
                        throw new Exception("Error in Insert for Details Data.");
                    }

                    LineNo++;
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
                result.Status = "Fail";
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

        // Delete Method
        public async Task<ResultVM> Delete(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };

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

                string query = "UPDATE SalePersonYearlyTargets SET IsArchive = 0, IsActive = 1 WHERE Id IN (@Ids)";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    string ids = string.Join(",", IDs);
                    cmd.Parameters.AddWithValue("@Ids", ids);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = $"Data deleted successfully.";
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

ISNULL(M.Id, 0) AS Id,
ISNULL(M.SalePersonId, 0) AS SalePersonId,
ISNULL(M.BranchId, 0) AS BranchId,
ISNULL(M.YearlyTarget, 0) AS YearlyTarget,
ISNULL(M.SelfSaleCommissionRate, 0) AS SelfSaleCommissionRate,
ISNULL(M.OtherSaleCommissionRate, 0) AS OtherSaleCommissionRate,
ISNULL(M.FiscalYearForSaleId, 0) AS FiscalYearForSaleId,
ISNULL(M.Year, 0) AS Year,
ISNULL(M.IsPost, 0) AS IsPost,


ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn,
ISNULL(FORMAT(CAST(M.YearStart AS DATE), 'yyyy-MM-dd'), '1900-01-01') AS YearStart,
ISNULL(FORMAT(CAST(M.YearEnd AS DATE), 'yyyy-MM-dd'), '1900-01-01') AS YearEnd

FROM SalePersonYearlyTargets M

WHERE  1 = 1
 ";

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
                var model = dataTable.AsEnumerable().Select(row => new SalePersonYearlyTargetVM
                {
                    Id = row.Field<int>("Id"),
                    SalePersonId = row.Field<int>("SalePersonId"),
                    BranchId = row.Field<int>("BranchId"),
                    YearlyTarget = row.Field<decimal>("YearlyTarget"),
                    SelfSaleCommissionRate = row.Field<decimal>("SelfSaleCommissionRate"),
                    OtherSaleCommissionRate = row.Field<decimal>("OtherSaleCommissionRate"),
                    FiscalYearForSaleId = row.Field<int>("FiscalYearForSaleId"),
                    Year = row.Field<int>("Year"),
                    YearStart = row.Field<string>("YearStart"),
                    YearEnd = row.Field<string>("YearEnd"),
                    IsPost = row.Field<bool>("IsPost"),
                    CreatedBy = row.Field<string>("CreatedBy"),
                    CreatedOn = row.Field<string>("CreatedOn"),
                    LastModifiedBy = row.Field<string>("LastModifiedBy"),
                    LastModifiedOn = row.Field<string?>("LastModifiedOn")
                }).ToList();
                //var model = new List<SalePersonYearlyTargetVM>();

                //foreach (DataRow row in dataTable.Rows)
                //{
                //    model.Add(new SalePersonYearlyTargetVM
                //    {

                //        Id = Convert.ToInt32(row["Id"]),
                //        SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                //        BranchId = Convert.ToInt32(row["BranchId"]),
                //        YearlyTarget = Convert.ToInt32(row["YearlyTarget"]),
                //        SelfSaleCommissionRate = Convert.ToInt32(row["SelfSaleCommissionRate"]),
                //        OtherSaleCommissionRate = Convert.ToInt32(row["OtherSaleCommissionRate"]),
                //        FiscalYearForSaleId = Convert.ToInt32(row["FiscalYearForSaleId"]),
                //        Year = Convert.ToInt32(row["Year"]),
                //        YearStart = (row["YearStart"]).ToString(),
                //        YearEnd = (row["YearEnd"]).ToString(),
                //        //IsApproveed =Convert.ToBoolean(row["IsApproveed"]),
                //        //ApprovedBy = row["ApprovedBy"].ToString(),
                //        IsPost = Convert.ToBoolean(row["IsPost"]),
                //        //PostedBy = (row["PostedBy"]).ToString(),
                //        CreatedBy = row.Field<string>("CreatedBy"),
                //        CreatedOn = row.Field<string>("CreatedOn"),
                //        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                //        LastModifiedOn = row.Field<string?>("LastModifiedOn"),


                //    });
                //}

                var detailsDataList = DetailsList(new[] { "D.SalePersonYearlyTargetId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SalePersonYearlyTargetDetailVM>>(json);

                    model.FirstOrDefault().SalePersonYearlyTargetDetailList = details;
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


        public ResultVM DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                ISNULL(D.SalePersonId, 0) AS SalePersonId,
                ISNULL(D.SalePersonYearlyTargetId, 0) AS SalePersonYearlyTargetId,
                ISNULL(D.FiscalYearDetailForSaleId, 0) AS FiscalYearDetailForSaleId,
                ISNULL(D.FiscalYearForSaleId, 0) AS FiscalYearForSaleId,
                ISNULL(D.MonthlyTarget, 0) AS MonthlyTarget,
                ISNULL(D.OtherSaleCommissionRate, 0) AS OtherSaleCommissionRate,
                ISNULL(D.SelfSaleCommissionRate, 0) AS SelfSaleCommissionRate,
                ISNULL(D.Year, 0) AS Year,
                ISNULL(D.MonthId, 0) AS MonthId,
                ISNULL(fd.MonthName, 0) AS MonthName,
                ISNULL(FORMAT(CAST(D.MonthStart AS DATE), 'dd-MMM-yyyy'), '1900-01-01') AS MonthStart,
                ISNULL(FORMAT(CAST(D.MonthEnd AS DATE), 'dd-MMM-yyyy'), '1900-01-01') AS MonthEnd


                FROM 
                SalePersonYearlyTargetDetails D
                LEFT OUTER JOIN FiscalYearDetailForSales fd on d.FiscalYearDetailForSaleId = fd.Id




                WHERE 1 = 1";

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
    Id, SalePersonId, BranchId, YearlyTarget, SelfSaleCommissionRate, 
    OtherSaleCommissionRate, FiscalYearForSaleId, Year, YearStart, 
    YearEnd, IsApproveed, ApprovedBy, ApprovedDate, IsPosted, 
    PostedBy, PostedDate, CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn
FROM SalePersonYearlyTargets
WHERE 1 = 1";


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
FROM SalePersonYearlyTargets
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


        public async Task<ResultVM> GetGridData(GridOptions options, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SalePersonYearlyTargetVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
					FROM SalePersonYearlyTargets H 
					LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
					LEFT OUTER JOIN FiscalYearDetailForSales F ON H.FiscalYearForSaleId = F.Id
					LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id

					

					WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonYearlyTargetVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                ,ISNULL(H.Id,0)	Id
				,ISNULL(H.YearlyTarget,0)	YearlyTarget
				,ISNULL(H.Year,0)	Year
				,ISNULL(H.SelfSaleCommissionRate,0)	SelfSaleCommissionRate	
				,ISNULL(H.OtherSaleCommissionRate,0) OtherSaleCommissionRate	
				,ISNULL(FORMAT(H.YearStart,'yyyy-MM-dd HH:mm'),'1900-01-01') YearStart
				,ISNULL(FORMAT(H.YearEnd,'yyyy-MM-dd HH:mm'),'1900-01-01') YearEnd	

				,ISNULL(H.IsPost,0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status


				,ISNULL(H.CreatedBy,'') CreatedBy
				,ISNULL(H.LastModifiedBy,'') LastModifiedBy
				,ISNULL(H.CreatedFrom,'') CreatedFrom
				,ISNULL(H.LastUpdateFrom,'') LastUpdateFrom		
				,ISNULL(FORMAT(H.CreatedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') CreatedOn
				,ISNULL(FORMAT(H.LastModifiedOn,'yyyy-MM-dd HH:mm'),'1900-01-01') LastModifiedOn
		        ,ISNULL(H.BranchId,0) BranchId
		        

		

				FROM SalePersonYearlyTargets H 
				LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
				LEFT OUTER JOIN FiscalYearDetailForSales F ON H.FiscalYearForSaleId = F.Id
				LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
				
				WHERE 1 = 1

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SalePersonYearlyTargetVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SalePersonYearlyTargetVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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

        public async Task<ResultVM> MultiplePost(CommonVM vm, SqlConnection conn, SqlTransaction transaction)
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

                string query = $" UPDATE SalePersonYearlyTargets SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                

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

        public async Task<ResultVM> DetailsInsert(List<SalePersonYearlyTargetDetailVM> salePersonYearlyTargetDetailVMs, SqlConnection conn, SqlTransaction transaction)
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
        INSERT INTO SalePersonYearlyTargetDetails

        (   BranchId,
            SalePersonId, 
            SalePersonYearlyTargetId,
            FiscalYearDetailForSaleId, 
            FiscalYearForSaleId, 
            MonthlyTarget,
            SelfSaleCommissionRate,
            OtherSaleCommissionRate,
            Year,
            MonthId,
            MonthStart,
            MonthEnd

            )
            VALUES 
            (

            @BranchId,
            @SalePersonId, 
            @SalePersonYearlyTargetId,
            @FiscalYearDetailForSaleId, 
            @FiscalYearForSaleId, 
            @MonthlyTarget,
            @SelfSaleCommissionRate,
            @OtherSaleCommissionRate,
            @Year,
            @MonthId,
            @MonthStart,
            @MonthEnd

            );

        SELECT SCOPE_IDENTITY();      
        ";

                foreach (var vm in salePersonYearlyTargetDetailVMs)
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                        cmd.Parameters.AddWithValue("@SalePersonId", vm.SalePersonId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@SalePersonYearlyTargetId", vm.SalePersonYearlyTargetId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FiscalYearDetailForSaleId", vm.FiscalYearDetailForSaleId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FiscalYearForSaleId", vm.FiscalYearForSaleId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MonthlyTarget", vm.MonthlyTarget);
                        cmd.Parameters.AddWithValue("@SelfSaleCommissionRate", vm.SelfSaleCommissionRate);
                        cmd.Parameters.AddWithValue("@OtherSaleCommissionRate", vm.OtherSaleCommissionRate);
                        cmd.Parameters.AddWithValue("@Year", vm.Year);
                        cmd.Parameters.AddWithValue("@MonthId", vm.MonthId);
                        cmd.Parameters.AddWithValue("@MonthStart", vm.MonthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", vm.MonthEnd);

                        vm.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }
                }

                result.Status = "Success";
                result.Message = "Data inserted successfully.";
                result.Id = salePersonYearlyTargetDetailVMs.Last().Id.ToString();
                result.DataVM = salePersonYearlyTargetDetailVMs;

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
    }

}
