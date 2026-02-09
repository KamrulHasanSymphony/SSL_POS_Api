using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class PaymentRepository : CommonRepository
    {
        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        //Insert Method
        public async Task<ResultVM> Insert(PaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
            INSERT INTO Payments
            (
                Code, TransactionDate,UserId, SupplierId, BankAccountId, IsCash, TotalPaymentAmount, Reference,
                Comments, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
            )
            VALUES 
            (
                @Code, @TransactionDate,@UserId, @SupplierId, @BankAccountId, @IsCash, @TotalPaymentAmount, @Reference, 
                @Comments, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom
            );
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BankAccountId", vm.BankAccountId);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@TotalPaymentAmount", vm.TotalPaymentAmount);

                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference", vm.Reference ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCash", vm.IsCash);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? (object)DBNull.Value);

                    object newId = cmd.ExecuteScalar();
                    vm.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = newId.ToString();
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



        // InsertDetails Method
        public async Task<ResultVM> InsertDetails(PaymentDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO PaymentDetails
        (PaymentId, SupplierId,PurchaseId, PurchaseAmount, PaymentAmount, Comments)
        VALUES 
        (@PaymentId, @SupplierId,@PurchaseId, @PurchaseAmount, @PaymentAmount, @Comments);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@PaymentId", details.PaymentId);
                    cmd.Parameters.AddWithValue("@SupplierId", details.SupplierId ?? 0);
                    cmd.Parameters.AddWithValue("@PurchaseId", details.PurchaseId ?? 0);
                    //cmd.Parameters.AddWithValue("@QuanPurchaseAmounttity", details.PurchaseAmount);
                    cmd.Parameters.AddWithValue("@PaymentAmount", details.PaymentAmount);
                    cmd.Parameters.AddWithValue("@PurchaseAmount", details.PurchaseAmount);
                    cmd.Parameters.AddWithValue("@Comments", details.Comments ?? (object)DBNull.Value);



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
                result.Status = "Fail";
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        // Update Method
        public async Task<ResultVM> Update(PaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE Payments
        SET 
            SupplierId = @SupplierId, 
            TransactionDate = @TransactionDate, 
            BankAccountId = @BankAccountId, 
            IsCash = @IsCash, 
            TotalPaymentAmount = @TotalPaymentAmount, 
            Reference = @Reference, 
            Comments = @Comments, 
            IsArchive = @IsArchive,
            IsActive = @IsActive,
            LastModifiedBy = @LastModifiedBy,
            LastModifiedOn = GETDATE(),
            CreatedFrom = @CreatedFrom,
            LastUpdateFrom = @LastUpdateFrom
        WHERE Id = @Id";  // Make sure @Id is in the WHERE clause to specify which record to update

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@BankAccountId", vm.BankAccountId);
                    cmd.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@TotalPaymentAmount", vm.TotalPaymentAmount);
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Reference", vm.Reference ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsCash", vm.IsCash);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", vm.CreatedFrom ?? "Unknown"); // Set a default value if CreatedFrom is null
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.LastUpdateFrom ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", vm.Id); // This is crucial for updating the correct record

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
    ISNULL(M.Code, '') AS Code,
	ISNULL(M.BankAccountId, 0) AS BankAccountId,
	ISNULL(e.AccountNo, 0) AS AccountNo,
    ISNULL(M.SupplierId, 0) AS SupplierId,
	ISNULL(M.TotalPaymentAmount, 0) AS TotalPaymentAmount,
    ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
    ISNULL(M.Comments, '') AS Comments,
	ISNULL(M.Reference, '') AS Reference,
    ISNULL(M.IsArchive, 0) AS IsArchive,
    ISNULL(M.IsCash, 0) AS IsCash,   
	ISNULL(M.IsActive, 0) AS IsActive, 
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom ,   
    ISNULL(S.Name, '') AS SupplierName

FROM 
Payments M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
LEFT OUTER JOIN BankAccounts e on M.BankAccountId= e.AccountNo

WHERE 1 = 1
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

                var lst = new List<PaymentVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new PaymentVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        TotalPaymentAmount = row.Field<decimal>("TotalPaymentAmount"),
                        BankAccountId = row.Field<int>("BankAccountId"),
                        SupplierId = row.Field<int>("SupplierId"),
                        SupplierName = row.Field<string>("SupplierName"),
                        AccountNo = row.Field<string>("AccountNo"),
                        TransactionDate = row.Field<string>("TransactionDate"),
                        Comments = row.Field<string>("Comments"),
                        Reference = row.Field<string>("Reference"),
                        IsArchive = row.Field<bool>("IsArchive"),
                        IsActive = row.Field<bool>("IsActive"),
                        IsCash = row.Field<bool>("IsCash"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                        LastUpdateFrom = row.Field<string?>("LastUpdateFrom")
                    });
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = lst;

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

        public async Task<ResultVM> DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(D.PaymentId, 0) AS PaymentId,
ISNULL(D.SupplierId, 0) AS SupplierId,
ISNULL(D.PurchaseId, 0) AS PurchaseId,
ISNULL(D.Comments, 0) AS Comments,
ISNULL(D.PurchaseAmount,0) AS PurchaseAmount,
ISNULL(D.PaymentAmount,0) AS PaymentAmount


FROM 
PaymentDetails D

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
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(M.BENumber, '') AS BENumber,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(FORMAT(M.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
    ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
    ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
    ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.CurrencyId, 0) AS CurrencyId,
    ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
    ISNULL(M.ImportIDExcel, '') AS ImportIDExcel,
    ISNULL(M.FileName, '') AS FileName,
    ISNULL(M.CustomHouse, '') AS CustomHouse,
    ISNULL(M.FiscalYear, '') AS FiscalYear,
    ISNULL(M.PeriodId, '') AS PeriodId,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Purchases M

WHERE 1 = 1 ";


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
                FROM Purchases
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

                string query = $" UPDATE Purchases SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE PurchaseDetails SET IsPost = 1 WHERE PurchaseId IN ({inClause}) ";

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

        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<PaymentVM>();

                // Define your SQL query string
                string sqlQuery = $@"
                -- Count query
                SELECT COUNT(DISTINCT H.Id) AS totalcount

                FROM Payments H
                LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
				LEFT OUTER JOIN BankAccounts e on h.BankAccountId= e.AccountNo

                WHERE 1 = 1
            -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + $@") AS rowindex,
        
	            ISNULL(H.Id, 0) AS Id,
				ISNULL(H.Code, '') AS Code,
				ISNULL(H.BankAccountId, 0) AS BankAccountId,
				ISNULL(e.AccountNo, 0) AS AccountNo,
				ISNULL(H.SupplierId, 0) AS SupplierId,
				ISNULL(H.TotalPaymentAmount, 0) AS TotalPaymentAmount,
				ISNULL(FORMAT(H.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
				ISNULL(H.Comments, '') AS Comments,
				ISNULL(H.Reference, '') AS Reference,
				ISNULL(H.IsArchive, 0) AS IsArchive,
				ISNULL(H.IsCash, 0) AS IsCash,   
				ISNULL(H.IsActive, 0) AS IsActive, 
				ISNULL(H.CreatedBy, '') AS CreatedBy,
				ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
				ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
				ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
				ISNULL(H.CreatedFrom, '') AS CreatedFrom,
				ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom , 
                ISNULL(S.Name,'') SupplierName



                FROM Payments H
                LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
				LEFT OUTER JOIN BankAccounts e on h.BankAccountId= e.AccountNo

                WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PaymentVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

//        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

//                var data = new GridEntity<PaymentDetailVM>();

//                // Define your SQL query string
//                string sqlQuery = $@"
//    -- Count query
//    SELECT COUNT(DISTINCT H.Id) AS totalcount
//            FROM Purchases H
//            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
//            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
//            LEFT OUTER JOIN PurchaseDetails D on h.Id = D.PurchaseId            
//            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
//			LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

//        WHERE 1 = 1
//    -- Add the filter condition
//        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentDetailVM>.FilterCondition(options.filter) + ")" : "");

//                // Apply additional conditions
//                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

//                sqlQuery += @"
//    -- Data query with pagination and sorting
//    SELECT * 
//    FROM (
//        SELECT 
//        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + $@") AS rowindex,
        

//	            ISNULL(H.Id, 0) AS Id,
//                ISNULL(H.Code, '') AS Code,
//                ISNULL(H.BranchId, 0) AS BranchId,
//				ISNULL(H.CompanyId, 0) AS CompanyId,
//				ISNULL(Br.Name,'') BranchName,
//                ISNULL(H.SupplierId, 0) AS SupplierId,
//				ISNULL(S.Name,'') SupplierName,
//                ISNULL(S.Address, '') AS SupplierAddress,
//                ISNULL(H.BENumber, '') AS BENumber,
//                ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
//                ISNULL(FORMAT(H.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
//                ISNULL(H.Comments, '') AS Comments,
//                ISNULL(H.TransactionType, '') AS TransactionType,
//                ISNULL(H.IsPost, 0) AS IsPost,	            
//                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
//                ISNULL(H.PostedBy, '') AS PostedBy,
//                ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
//                ISNULL(H.FiscalYear, '') AS FiscalYear,
//                ISNULL(H.PeriodId, '') AS PeriodId,
//                ISNULL(H.CreatedBy, '') AS CreatedBy,
//                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
//                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
//                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
//                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
//                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
                
               
//                    -- Sales Details
//            ISNULL(D.Id, 0) AS PurchaseDetailId,
//            ISNULL(D.PurchaseOrderId, 0) AS PurchaseOrderId,
//            ISNULL(D.PurchaseOrderDetailId, 0) AS PurchaseOrderDetailId,
//            ISNULL(D.Line, 0) AS Line,
//            ISNULL(D.ProductId, 0) AS ProductId,
//            ISNULL(P.Name,'') ProductName,
//            ISNULL(D.Quantity, 0) AS Quantity,
//            ISNULL(D.UnitPrice, 0) AS UnitPrice,
//            ISNULL(D.SubTotal, 0) AS SubTotal,
//            ISNULL(D.SD, 0) AS SD,
//            ISNULL(D.SDAmount, 0) AS SDAmount,
//            ISNULL(D.VATRate, 0) AS VATRate,
//            ISNULL(D.VATAmount, 0) AS VATAmount,
//            ISNULL(D.OthersAmount, 0) AS OthersAmount,
//			ISNULL(CP.CompanyName,'') CompanyName



//            FROM Purchases H
//            LEFT OUTER JOIN Suppliers s on h.SupplierId = s.Id
//            LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
//            LEFT OUTER JOIN PurchaseDetails D on h.Id = D.PurchaseId            
//            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
//			LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id

//        WHERE 1 = 1

//    -- Add the filter condition
//        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentDetailVM>.FilterCondition(options.filter) + ")" : "");

//                // Apply additional conditions
//                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

//                sqlQuery += @"
//    ) AS a
//    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
//";

//                data = KendoGrid<PaymentDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

//                result.Status = "Success";
//                result.Message = "Details data retrieved successfully.";
//                result.DataVM = data;

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


      public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {

                var data = new GridEntity<PaymentDetailVM>();


                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT D.Id) AS totalcount
FROM 
PaymentDetails D

WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + $@") AS rowindex,
             SELECT 
ISNULL(D.Id, 0) AS Id,
ISNULL(D.PaymentId, 0) AS PaymentId,
ISNULL(D.SupplierId, 0) AS SupplierId,
ISNULL(D.PurchaseId, 0) AS PurchaseId,
ISNULL(D.Comments, 0) AS Comments,
ISNULL(D.PurchaseAmount,0) AS PurchaseAmount,
ISNULL(D.PaymentAmount,0) AS PaymentAmount


FROM 
PaymentDetails D

WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<PaymentDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<PaymentDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

                result.Status = "Success";
                result.Message = "Details data retrieved successfully.";
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
