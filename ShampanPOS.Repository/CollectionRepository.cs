using Newtonsoft.Json;
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
    public class CollectionRepository : CommonRepository
    {

        public async Task<ResultVM> Delete(string[] IDs, SqlConnection conn, SqlTransaction transaction)
        {
            throw new NotImplementedException();
        }

        //Insert Method
        public async Task<ResultVM> Insert(CollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
            INSERT INTO Collections
            (
                Code, TransactionDate, CustomerId, BankAccountId, IsCash, TotalCollectAmount, ChequeNo,ChequeBankName, ChequeDate,
                Comments, IsArchive, IsActive, CreatedBy, CreatedOn, CreatedFrom
            )
            VALUES 
            (
                @Code, @TransactionDate, @CustomerId, @BankAccountId, @IsCash, @TotalCollectAmount, @ChequeNo,@ChequeBankName, @ChequeDate,
                @Comments, @IsArchive, @IsActive, @CreatedBy, @CreatedOn, @CreatedFrom
            );
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@BankAccountId", vm.BankAccountId);
                    cmd.Parameters.AddWithValue("@IsCash", vm.IsCash);
                    cmd.Parameters.AddWithValue("@TotalCollectAmount", vm.TotalCollectAmount);
                    cmd.Parameters.AddWithValue("@ChequeNo", vm.ChequeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChequeBankName", vm.ChequeBankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChequeDate", vm.ChequeDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsArchive", vm.IsArchive);
                    cmd.Parameters.AddWithValue("@IsActive", vm.IsActive);
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
        public async Task<ResultVM> InsertDetails(CollectionDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO CollectionDetails
        (CollectionId, CustomerId,SaleId, SaleAmount, CollectionAmount, Comments)
        VALUES 
        (@CollectionId, @CustomerId,@SaleId, @SaleAmount, @CollectionAmount, @Comments);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CollectionId", details.CollectionId);
                    cmd.Parameters.AddWithValue("@CustomerId", details.CustomerId ?? 0);
                    cmd.Parameters.AddWithValue("@SaleId", details.SaleId ?? 0);
                    //cmd.Parameters.AddWithValue("@QuanPurchaseAmounttity", details.PurchaseAmount);
                    cmd.Parameters.AddWithValue("@SaleAmount", details.SaleAmount);
                    cmd.Parameters.AddWithValue("@CollectionAmount", details.CollectionAmount);
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
        public async Task<ResultVM> Update(CollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        UPDATE Collections
        SET 

            BankAccountId = @BankAccountId,
            CustomerId = @CustomerId, 
            TransactionDate = @TransactionDate, 
            TotalCollectAmount = @TotalCollectAmount, 
            ChequeNo = @ChequeNo, 
            ChequeBankName = @ChequeBankName, 
            ChequeDate = @ChequeDate, 
            Comments = @Comments,
            IsCash = @IsCash, 
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
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@TotalCollectAmount", vm.TotalCollectAmount);
                    cmd.Parameters.AddWithValue("@ChequeNo", vm.ChequeNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChequeBankName", vm.ChequeBankName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ChequeDate", vm.ChequeDate + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Comments", vm.Comments ?? (object)DBNull.Value);
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
//        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//SELECT 
//    ISNULL(M.Id, 0) AS Id,
//    ISNULL(M.Code, '') AS Code,
//	ISNULL(M.BankAccountId, 0) AS BankAccountId,
//	ISNULL(e.AccountNo, 0) AS AccountNo,
//	ISNULL(M.ChequeNo, '') AS ChequeNo,
//    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
//    ISNULL(M.CustomerId, 0) AS CustomerId,
//	ISNULL(S.Name, 0) AS CustomerName,
//	ISNULL(M.TotalCollectAmount, 0) AS TotalCollectAmount,
//    ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
//	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
//    ISNULL(M.Comments, '') AS Comments,
//    ISNULL(M.IsArchive, 0) AS IsArchive,
//    ISNULL(M.IsCash, 0) AS IsCash,   
//	ISNULL(M.IsActive, 0) AS IsActive, 
//    ISNULL(M.CreatedBy, '') AS CreatedBy,
//    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
//    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
//    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
//    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
//    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom ,   
//    ISNULL(S.Name, '') AS SupplierName

//FROM 
//Collections M
//LEFT OUTER JOIN Customers S ON ISNULL(M.CustomerId,0) = S.Id
//LEFT OUTER JOIN BankAccounts e on M.BankAccountId= e.AccountNo

//WHERE 1 = 1
// ";

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    query += " AND M.Id = @Id ";
//                }

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
//                }

//                objComm.Fill(dataTable);

//                var lst = new List<CollectionVM>();

//                foreach (DataRow row in dataTable.Rows)
//                {
//                    lst.Add(new CollectionVM
//                    {
//                        Id = row.Field<int>("Id"),
//                        Code = row.Field<string>("Code"),
//                        TotalCollectAmount = row.Field<decimal>("TotalCollectAmount"),
//                        BankAccountId = row.Field<int>("BankAccountId"),
//                        CustomerId = row.Field<int>("CustomerId"),
//                        CustomerName = row.Field<string>("CustomerName"),
//                        AccountNo = row.Field<string>("AccountNo"),
//                        TransactionDate = row.Field<string>("TransactionDate"),
//                        Comments = row.Field<string>("Comments"),
//                        IsArchive = row.Field<bool>("IsArchive"),
//                        IsActive = row.Field<bool>("IsActive"),
//                        IsCash = row.Field<bool>("IsCash"),
//                        CreatedBy = row.Field<string>("CreatedBy"),
//                        CreatedOn = row.Field<string>("CreatedOn"),
//                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
//                        LastModifiedOn = row.Field<string?>("LastModifiedOn"),
//                        LastUpdateFrom = row.Field<string?>("LastUpdateFrom")
//                    });
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
    ISNULL(M.Code, '') AS Code,
	ISNULL(M.BankAccountId, 0) AS BankAccountId,
	ISNULL(e.AccountNo, 0) AS AccountNo,
	ISNULL(M.ChequeNo, '') AS ChequeNo,
    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
    ISNULL(M.CustomerId, 0) AS CustomerId,
	ISNULL(S.Name, 0) AS CustomerName,
	ISNULL(M.TotalCollectAmount, 0) AS TotalCollectAmount,
    ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
    ISNULL(M.Comments, '') AS Comments,
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
Collections M
LEFT OUTER JOIN Customers S ON ISNULL(M.CustomerId,0) = S.Id
LEFT OUTER JOIN BankAccounts e on M.BankAccountId= e.AccountNo

WHERE 1 = 1
";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<CollectionVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CollectionVM
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        ChequeNo = row.Field<string>("ChequeNo"),
                        ChequeBankName = row.Field<string>("ChequeBankName"),
                        TotalCollectAmount = row.Field<decimal>("TotalCollectAmount"),
                        BankAccountId = row.Field<int>("BankAccountId"),
                        CustomerId = row.Field<int>("CustomerId"),
                        CustomerName = row.Field<string>("CustomerName"),
                        AccountNo = row.Field<string>("AccountNo"),
                        TransactionDate = row.Field<string>("TransactionDate"),
                        Comments = row.Field<string>("Comments"),
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

                //var detailsDataList = DetailsList(new[] { "D.SaleId" }, conditionalValues, vm, conn, transaction);

                //if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                //{
                //    string json = JsonConvert.SerializeObject(dt);
                //    var details = JsonConvert.DeserializeObject<List<CollectionDetailVM>>(json);

                //    model.FirstOrDefault().collectionDetailList = details;
                //}

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





//        public async Task<ResultVM> DetailsList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
//SELECT 

//ISNULL(D.Id, 0) AS Id,
//ISNULL(D.CollectionId, 0) AS CollectionId,
//ISNULL(D.CustomerId, 0) AS CustomerId,
//ISNULL(D.SaleId, 0) AS SaleId,
//ISNULL(D.Comments, 0) AS Comments,
//ISNULL(D.CollectionAmount,0) AS CollectionAmount,
//ISNULL(D.SaleAmount,0) AS SaleAmount


//FROM 
//CollectionDetails D

//WHERE 1 = 1 ";

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    query += " AND D.Id = @Id ";
//                }

//                // Apply additional conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValue, false);

//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

//                // SET additional conditions param
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

//                if (vm != null && !string.IsNullOrEmpty(vm.Id))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
//                }

//                objComm.Fill(dataTable);

//                result.Status = "Success";
//                result.Message = "Details Data retrieved successfully.";
//                result.DataVM = dataTable;

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
ISNULL(D.CollectionId, 0) AS CollectionId,
ISNULL(D.CustomerId, 0) AS CustomerId,
ISNULL(D.SaleId, 0) AS SaleId,
ISNULL(S.Code,0) AS SaleCode,
ISNULL(D.Comments, 0) AS Comments,
ISNULL(D.CollectionAmount,0) AS CollectionAmount,
ISNULL(D.SaleAmount,0) AS SaleAmount


FROM 
CollectionDetails D
LEFT OUTER JOIN Sales S ON D.SaleId= S.Id
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
	ISNULL(M.BankAccountId, 0) AS BankAccountId,
	ISNULL(e.AccountNo, 0) AS AccountNo,
	ISNULL(M.ChequeNo, '') AS ChequeNo,
    ISNULL(M.ChequeBankName, '') AS ChequeBankName,
    ISNULL(M.CustomerId, 0) AS CustomerId,
	ISNULL(S.Name, 0) AS CustomerName,
	ISNULL(M.TotalCollectAmount, 0) AS TotalCollectAmount,
    ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
	ISNULL(FORMAT(M.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
    ISNULL(M.Comments, '') AS Comments,
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
Collections M
LEFT OUTER JOIN Customers S ON ISNULL(M.CustomerId,0) = S.Id
LEFT OUTER JOIN BankAccounts e on M.BankAccountId= e.AccountNo

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
                FROM Collections
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

                string query = $" UPDATE Collections SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE CollectionDetails SET IsPost = 1 WHERE PurchaseId IN ({inClause}) ";

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

                var data = new GridEntity<CollectionVM>();

                // Define your SQL query string
                string sqlQuery = $@"
                -- Count query
                SELECT COUNT(DISTINCT H.Id) AS totalcount

                FROM Collections H
                LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
				LEFT OUTER JOIN BankAccounts e on h.BankAccountId= e.AccountNo

                WHERE 1 = 1
            -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CollectionVM>.FilterCondition(options.filter) + ")" : "");

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
				ISNULL(e.AccountName, 0) AS AccountName,
				ISNULL(H.ChequeNo, '') AS ChequeNo,
                ISNULL(H.ChequeBankName, '') AS ChequeBankName,
				ISNULL(H.CustomerId, 0) AS CustomerId,
				ISNULL(S.Name, 0) AS CustomerName,
				ISNULL(H.TotalCollectAmount, 0) AS TotalCollectAmount,
				ISNULL(FORMAT(H.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
				ISNULL(FORMAT(H.ChequeDate, 'yyyy-MM-dd'), '1900-01-01') AS ChequeDate,
				ISNULL(H.Comments, '') AS Comments,
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



                FROM Collections H
                LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
				LEFT OUTER JOIN BankAccounts e on h.BankAccountId= e.AccountNo

                WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CollectionVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<CollectionVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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


        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {

                var data = new GridEntity<CollectionDetailVM>();


                // Define your SQL query string
                string sqlQuery = $@"
    -- Count query
    SELECT COUNT(DISTINCT D.Id) AS totalcount
FROM 
CollectionDetails D

WHERE 1 = 1
    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CollectionDetailVM>.FilterCondition(options.filter) + ")" : "");

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
ISNULL(D.CollectionId, 0) AS CollectionId,
ISNULL(D.CustomerId, 0) AS CustomerId,
ISNULL(D.SaleId, 0) AS SaleId,
ISNULL(D.Comments, 0) AS Comments,
ISNULL(D.CollectionAmount,0) AS CollectionAmount,
ISNULL(D.SaleAmount,0) AS SaleAmount


FROM 
CollectionDetails D

WHERE 1 = 1

    -- Add the filter condition
        " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CollectionDetailVM>.FilterCondition(options.filter) + ")" : "");

                // Apply additional conditions
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<CollectionDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "D.Id", conditionalFields, conditionalValues);

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
