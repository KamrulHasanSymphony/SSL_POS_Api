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
using System.Text.Json;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{

    public class CustomerPaymentCollectionRepository : CommonRepository
    {
        // Insert Method
        public async Task<ResultVM> Insert(CustomerPaymentCollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

                // Commented out file handling for now
                // if (vm.File != null && vm.File.Length > 0)
                // {
                //     var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
                //     var uploadsDirectory = Path.Combine("Uploads","Payment");
                //     if (!Directory.Exists(uploadsDirectory))
                //     {
                //         Directory.CreateDirectory(uploadsDirectory);
                //     }
                //     var filePath = Path.Combine(uploadsDirectory, uniqueFileName);
                //     using (var stream = new FileStream(filePath, FileMode.Create))
                //     {
                //         await vm.File.CopyToAsync(stream);
                //     }
                //     vm.FileName = filePath;
                // }

                string query = @"
        INSERT INTO CustomerPaymentCollection 
        (
            Code, TransactionDate, Amount, RestAmount, UserId, BranchId, CustomerId, ModeOfPayment, ModeOfPaymentNo, 
            ModeOfPaymentDate, IsPost, ImagePath, Attachment, FileName, CreatedBy, CreatedOn
        )
        VALUES 
        (
            @Code, @TransactionDate, @Amount, @RestAmount, @UserId, @BranchId, @CustomerId, @ModeOfPayment, 
            @ModeOfPaymentNo, @ModeOfPaymentDate, @IsPost, @ImagePath, @Attachment, @FileName, @CreatedBy, @CreatedOn
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@Amount", vm.Amount ?? 0);
                    cmd.Parameters.AddWithValue("@RestAmount", vm.RestAmount ?? 0);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? "");
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@ModeOfPayment", vm.ModeOfPayment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeOfPaymentNo", vm.ModeOfPaymentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeOfPaymentDate", vm.ModeOfPaymentDate ?? (object)DBNull.Value); // Ensure ModeOfPaymentDate is nullable
                    cmd.Parameters.AddWithValue("@Attachment", vm.Attachment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileName", vm.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);
                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    vm.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

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

        //        public async Task<ResultVM> Insert(CustomerPaymentCollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //        {
        //            bool isNewConnection = false;
        //            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //            try
        //            {
        //                if (conn == null)
        //                {
        //                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                    conn.Open();
        //                    isNewConnection = true;
        //                }

        //                if (transaction == null)
        //                {
        //                    transaction = conn.BeginTransaction();
        //                }
        //                //if (vm.File != null && vm.File.Length > 0)
        //                //{
        //                //    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
        //                //    var uploadsDirectory = Path.Combine("Uploads","Payment");
        //                //    if (!Directory.Exists(uploadsDirectory))
        //                //    {
        //                //        Directory.CreateDirectory(uploadsDirectory);
        //                //    }
        //                //    var filePath = Path.Combine(uploadsDirectory, uniqueFileName);
        //                //    using (var stream = new FileStream(filePath, FileMode.Create))
        //                //    {
        //                //        await vm.File.CopyToAsync(stream);
        //                //    }
        //                //    vm.FileName = filePath;
        //                //}

        //                string query = @"
        //INSERT INTO CustomerPaymentCollection 
        //(
        // Code, TransactionDate, Amount,RestAmount, UserId,BranchId ,CustomerId, ModeOfPayment, ModeOfPaymentNo, ModeOfPaymentDate, IsPost, ImagePath,Attachment,FileName, CreatedBy, CreatedOn
        //)
        //VALUES 
        //(
        // @Code, @TransactionDate, @Amount,@RestAmount, @UserId,@BranchId, @CustomerId, @ModeOfPayment, @ModeOfPaymentNo, @ModeOfPaymentDate,@IsPost,@ImagePath,@Attachment,@FileName, @CreatedBy, @CreatedOn
        //);
        //SELECT SCOPE_IDENTITY();";

        //                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //                {
        //                    cmd.Parameters.AddWithValue("@Code", vm.Code ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
        //                    cmd.Parameters.AddWithValue("@Amount", vm.Amount ?? 0);
        //                    cmd.Parameters.AddWithValue("@RestAmount", vm.RestAmount ?? 0);
        //                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
        //                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
        //                    cmd.Parameters.AddWithValue("@ModeOfPayment", vm.ModeOfPayment ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@ModeOfPaymentNo", vm.ModeOfPaymentNo ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@ModeOfPaymentDate", vm.ModeOfPaymentDate);
        //                    cmd.Parameters.AddWithValue("@Attachment", vm.Attachment ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@FileName", vm.FileName ?? (object)DBNull.Value);
        //                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost );
        //                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);
        //                    cmd.Parameters.AddWithValue("@CreatedBy", vm.CreatedBy);
        //                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

        //                    vm.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        //                    result.Status = "Success";
        //                    result.Message = "Data inserted successfully.";
        //                    result.Id = vm.Id.ToString();
        //                    result.DataVM = vm;
        //                }

        //                if (isNewConnection)
        //                {
        //                    transaction.Commit();
        //                }

        //                return result;
        //            }
        //            catch (Exception ex)
        //            {
        //                if (transaction != null && isNewConnection)
        //                {
        //                    transaction.Rollback();
        //                }

        //                result.ExMessage = ex.Message;
        //                result.Message = "Error in Insert.";
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

        // Update Method
        public async Task<ResultVM> Update(CustomerPaymentCollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
            UPDATE CustomerPaymentCollection 
            SET 
                TransactionDate = @TransactionDate, 
                Amount = @Amount,              
                RestAmount = @Amount, 
                UserId = @UserId, 
                CustomerId = @CustomerId, 
                ModeOfPayment = @ModeOfPayment,
                ModeOfPaymentNo = @ModeOfPaymentNo, 
                ModeOfPaymentDate = @ModeOfPaymentDate, 
                Attachment = @Attachment,  
                FileName = @FileName,  
                IsPost = @IsPost,  
                LastModifiedBy = @LastModifiedBy,
                LastModifiedOn = GETDATE(),
                ImagePath = @ImagePath
            WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    cmd.Parameters.AddWithValue("@TransactionDate", vm.TransactionDate);
                    cmd.Parameters.AddWithValue("@Amount", vm.Amount ?? 0);
                    cmd.Parameters.AddWithValue("@UserId", vm.UserId ?? "");
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@ModeOfPayment", vm.ModeOfPayment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", vm.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeOfPaymentNo", vm.ModeOfPaymentNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModeOfPaymentDate", vm.ModeOfPaymentDate ?? (object)DBNull.Value); // Ensure ModeOfPaymentDate can be null
                    cmd.Parameters.AddWithValue("@Attachment", vm.Attachment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileName", vm.FileName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", vm.IsPost);
                    cmd.Parameters.AddWithValue("@SaleDeleveryId", vm.SaleDeleveryId ?? (object)DBNull.Value); // Ensure SaleDeleveryId is handled correctly
                    cmd.Parameters.AddWithValue("@LastModifiedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(vm.ImagePath) ? (object)DBNull.Value : vm.ImagePath);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
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

                var data = new GridEntity<CustomerPaymentCollectionVM>();

                // Define your SQL query string
                string sqlQuery = @"
   -- Count query
   SELECT COUNT(DISTINCT M.Id) AS totalcount
   FROM CustomerPaymentCollection M 
   LEFT OUTER JOIN Customers C ON M.CustomerId = C.Id
   -- LEFT OUTER JOIN SalesPersons SP ON M.UserId = SP.Id
   LEFT OUTER JOIN PaymentTypes MP ON M.ModeOfPayment = MP.Name
   WHERE 1 = 1
   -- Add the filter condition
   " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerPaymentCollectionVM>.FilterCondition(options.filter) + ")" : "") + @"

   -- Data query with pagination and sorting
   SELECT * 
   FROM (
       SELECT 
       ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "M.Id DESC ") + @") AS rowindex,
       ISNULL(M.Id, 0) AS Id,
       ISNULL(M.Code, '') AS Code,
       ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') TransactionDate, 
       ISNULL(M.Amount, 0) AS Amount,
       ISNULL(M.UserId, '') AS UserId,
       -- ISNULL(SP.Name, 0) AS UserName,
       ISNULL(M.CustomerId, 0) AS CustomerId,
       ISNULL(M.BranchId, 0) AS BranchId,
       ISNULL(C.Name, 0) AS CustomerName,
       -- ISNULL(M.SaleDeleveryId, 0) AS SaleDeleveryId,
       ISNULL(M.ModeOfPayment, '') AS ModeOfPayment, 
       ISNULL(MP.Name, '') AS ModeOfPaymentName, 
       ISNULL(M.ModeOfPaymentNo, '') AS ModeOfPaymentNo,
       ISNULL(FORMAT(M.ModeOfPaymentDate,'yyyy-MM-dd HH:mm'), '1900-01-01') ModeOfPaymentDate, 
       ISNULL(M.Attachment, '') AS Attachment,
       ISNULL(M.ImagePath, '') AS ImagePath,
       ISNULL(M.CreatedBy, '') AS CreatedBy, 
       ISNULL(M.IsPost, 0) IsPost, 
       CASE WHEN ISNULL(M.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,   
       ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn, 
       ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
       ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
       FROM CustomerPaymentCollection M 
       LEFT OUTER JOIN Customers C ON M.CustomerId = C.Id
       -- LEFT OUTER JOIN SalesPersons SP ON M.UserId = SP.Id
       LEFT OUTER JOIN PaymentTypes MP ON M.ModeOfPayment = MP.Name
       WHERE 1 = 1
       -- Add the filter condition
       " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<CustomerPaymentCollectionVM>.FilterCondition(options.filter) + ")" : "") + @"
   ) AS a
   WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";


                data = KendoGrid<CustomerPaymentCollectionVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
                string query = $"UPDATE CustomerPaymentCollection SET IsArchive = 1, IsActive = 0 WHERE Id IN ({inClause})";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    //cmd.Parameters.AddWithValue("@LastModifiedBy", vm.ModifyBy);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

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
ISNULL(M.Id, '') AS Id,
ISNULL(M.Code, '') AS Code,
ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') TransactionDate, 
ISNULL(M.Amount, 0) AS Amount,
ISNULL(M.UserId, '') AS UserId,
ISNULL(M.CustomerId, 0) AS CustomerId,
ISNULL(M.ModeOfPayment, '') AS ModeOfPayment, 
ISNULL(M.ModeOfPaymentNo, '') AS ModeOfPaymentNo,
ISNULL(FORMAT(M.ModeOfPaymentDate,'yyyy-MM-dd HH:mm'), '1900-01-01') ModeOfPaymentDate, 
ISNULL(M.Attachment, '') AS Attachment,
ISNULL(M.IsPost, '') AS IsPost,
ISNULL(M.BranchId, 0) AS BranchId,

ISNULL(M.ImagePath, '') AS ImagePath,

ISNULL(M.CreatedBy, '') AS CreatedBy, 
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn, 
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
FROM CustomerPaymentCollection M 
WHERE 1 = 1";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }

                objComm.Fill(dataTable);

                var model = new List<CustomerPaymentCollectionVM>();
                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new CustomerPaymentCollectionVM
                    {
                        Id = row.Field<int>("Id"),
                        UserId = row.Field<string>("UserId"),
                        TransactionDate = row.Field<string>("TransactionDate"),
                        Amount = row.Field<decimal>("Amount"),
                        Code = row.Field<string>("Code"),
                        CustomerId = row.Field<int>("CustomerId"),
                        ModeOfPayment = row.Field<string>("ModeOfPayment"),
                        ModeOfPaymentNo = row.Field<string>("ModeOfPaymentNo"),
                        ModeOfPaymentDate = row.Field<string>("ModeOfPaymentDate"),
                        Attachment = row.Field<string>("Attachment"),
                        //FileName = row.Field<string>("FileName"),
                        IsPost = row.Field<bool>("IsPost"),
                        //IsProcessed = row.Field<bool>("Processed"),
                        //SaleDeleveryId = row.Field<int>("SaleDeleveryId"),
                        //SaleDeleveryNo = row.Field<string>("SaleDeleveryNo"),
                        ImagePath = row.Field<string>("ImagePath"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        CreatedOn = row.Field<string>("CreatedOn"),
                        LastModifiedBy = row.Field<string>("LastModifiedBy"),
                        LastModifiedOn = row.Field<string?>("LastModifiedOn"),
                        BranchId = row.Field<int>("BranchId")

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
        // Report Method
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
ISNULL(FORMAT(M.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') TransactionDate, 
ISNULL(M.Amount, 0) AS Amount,
ISNULL(M.UserId, '') AS UserId,
ISNULL(M.CustomerId, 0) AS CustomerId,
ISNULL(M.ModeOfPayment, '') AS ModeOfPayment, 
ISNULL(M.ModeOfPaymentNo, '') AS ModeOfPaymentNo,
ISNULL(FORMAT(M.ModeOfPaymentDate,'yyyy-MM-dd HH:mm'), '1900-01-01') ModeOfPaymentDate, 
ISNULL(M.Attachment, '') AS Attachment,
ISNULL(M.FileName, '') AS FileName,
ISNULL(M.IsPost, '') AS IsPost,
ISNULL(M.SaleDeleveryId, '') AS SaleDeleveryId,

ISNULL(M.CreatedBy, '') AS CreatedBy, 
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') CreatedOn, 
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') LastModifiedOn
FROM CustomerPaymentCollection M 
WHERE 1 = 1";

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


        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<CustomerPaymentCollectionVM>();

                string sqlQuery = @"
-- Count query
SELECT COUNT(DISTINCT H.Id) AS totalcount
FROM CustomerPaymentCollection H
LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
WHERE 1 = 1
" + (options.filter?.Filters?.Count > 0 ? " AND (" + GridQueryBuilder<CustomerPaymentCollectionVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
-- Data query with pagination and sorting
SELECT * 
FROM (
    SELECT 
    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
    ISNULL(H.Id, 0) AS Id,
    ISNULL(H.Code, '') AS Code,
    ISNULL(H.CustomerId, 0) AS CustomerId,
    ISNULL(H.SaleDeleveryId, 0) AS SaleDeleveryId,
    ISNULL(C.Name, '') AS Name,
    ISNULL(FORMAT(H.TransactionDate, 'yyyy-MM-dd'), '1900-01-01') AS TransactionDate,
    ISNULL(H.ModeOfPayment, '') AS ModeOfPayment,
    ISNULL(H.ModeOfPaymentNo, '') AS ModeOfPaymentNo,
    ISNULL(FORMAT(H.ModeOfPaymentDate, 'yyyy-MM-dd'), '1900-01-01') AS ModeOfPaymentDate,
    ISNULL(H.UserId, '') AS UserId,
    ISNULL(H.Attachment, '') AS Attachment,
    ISNULL(H.Amount, 0) AS Amount,
    ISNULL(H.ImagePath, '') AS ImagePath,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
    ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
    ISNULL(H.FileName, '') AS FileName,
    ISNULL(H.IsPost, '') AS IsPost


    FROM CustomerPaymentCollection H
    LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id

    WHERE 1 = 1
    " + (options.filter?.Filters?.Count > 0 ? " AND (" + GridQueryBuilder<CustomerPaymentCollectionVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"
) AS a
WHERE rowindex > @skip AND (@take = 0 OR rowindex <= (@skip + @take));";

                data = KendoGrid<CustomerPaymentCollectionVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                string query = $" UPDATE CustomerPaymentCollection SET IsPost = 1  WHERE Id IN ({inClause}) ";
               

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    for (int i = 0; i < vm.IDs.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@Id{i}", vm.IDs[i]);
                    }

                    //cmd.Parameters.AddWithValue("@PostedBy", vm.ModifyBy);
                    //cmd.Parameters.AddWithValue("@LastUpdateFrom", vm.ModifyFrom);

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



        //        public async Task<ResultVM> MultiplePaymentSettlementProcess(CustomerPaymentCollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        //        {
        //            bool isNewConnection = false;
        //            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = null, DataVM = null };

        //            try
        //            {
        //                if (conn == null)
        //                {
        //                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //                    await conn.OpenAsync();
        //                    isNewConnection = true;
        //                }

        //                if (transaction == null)
        //                {
        //                    transaction = conn.BeginTransaction();
        //                }

        //                string query = @"
        //-- Step 1: Prepare temporary tables for easier control
        //SELECT 
        //    Id, 
        //    CAST(RestAmount AS DECIMAL(10,2)) AS RestAmount, 
        //    CreatedOn AS PaymentDate 
        //INTO #Payments
        //FROM CustomerPaymentCollection
        //WHERE ISNULL(Processed, 0) = 0 
        //  AND ISNULL(IsPost, 0) = 1 
        //  AND CustomerId = @CustomerId
        //  AND Id = @Id
        //ORDER BY Id;

        //SELECT 
        //    Id, 
        //    CAST(RestAmount AS DECIMAL(10,2)) AS RestAmount, 
        //    DeliveryDate AS InvoiceDate, 
        //    CAST(0.00 AS DECIMAL(10,2)) AS PaidAmount,
        //    CAST(GrandTotalAmount AS DECIMAL(10,2)) AS GrandTotalAmount,
        //    CustomerId
        //INTO #Invoices
        //FROM SaleDeleveries
        //WHERE ISNULL(IsPost, 0) = 1 
        //  AND ISNULL(Processed, 0) = 0 
        //  AND ISNULL(RestAmount, 0) > 0 
        //  AND CustomerId = @CustomerId";

        //                if (vm.SaleDeleveryId > 0)
        //                {
        //                    query += @" AND Id = @SaleDeleveryId ";
        //                }
        //                        query = @"
        //ORDER BY Id;

        //-- Early exit if no payments or invoices to process
        //IF NOT EXISTS (SELECT 1 FROM #Payments WHERE RestAmount > 0)
        //BEGIN

        //    DROP TABLE #Payments;
        //    DROP TABLE #Invoices;
        //    RETURN;
        //END

        //IF NOT EXISTS (SELECT 1 FROM #Invoices WHERE RestAmount > 0)
        //BEGIN

        //    DROP TABLE #Payments;
        //    DROP TABLE #Invoices;
        //    RETURN;
        //END

        //-- Step 2: Loop through payments and settle invoices FIFO
        //DECLARE @PayId INT, @PayAmount DECIMAL(10,2), @PayDate DATE;
        //DECLARE @InvId INT, @InvCustomerId INT, @InvAmount DECIMAL(10,2), @InvDate DATE, @ActualInvoiceAmount DECIMAL(10,2);
        //DECLARE @SettleAmount DECIMAL(10,2);
        //DECLARE @Progress BIT;

        //WHILE EXISTS (SELECT 1 FROM #Payments WHERE RestAmount > 0)
        //BEGIN
        //    SET @Progress = 0;  -- Reset progress flag at start of each outer loop iteration

        //    SELECT TOP 1 
        //        @PayId = Id, 
        //        @PayAmount = RestAmount, 
        //        @PayDate = PaymentDate 
        //    FROM #Payments 
        //    WHERE RestAmount > 0 
        //    ORDER BY Id;

        //    WHILE @PayAmount > 0 AND EXISTS (SELECT 1 FROM #Invoices WHERE RestAmount > 0)
        //    BEGIN
        //        SELECT TOP 1 
        //            @InvId = Id, 
        //            @InvAmount = RestAmount, 
        //            @InvDate = InvoiceDate,
        //            @ActualInvoiceAmount = GrandTotalAmount,
        //            @InvCustomerId = CustomerId
        //        FROM #Invoices 
        //        WHERE RestAmount > 0 
        //        ORDER BY Id;

        //        SET @SettleAmount = CASE 
        //                                WHEN @PayAmount >= @InvAmount THEN @InvAmount 
        //                                ELSE @PayAmount 
        //                            END;

        //        -- If settle amount is zero or negative, break inner loop to avoid infinite loop
        //        IF (@SettleAmount <= 0)
        //        BEGIN
        //            BREAK;
        //        END

        //        INSERT INTO PaymentCollectionSettlementHistory (
        //            InvoiceId, CustomerId, InvoiceDate, InvoiceAmount, ActualInvoiceAmount, InvoiceRestAmount,
        //            PaymentId, PaymentDate, PaymentAmount, PaymentRestAmount
        //        )
        //        SELECT 
        //            @InvId,
        //            @InvCustomerId,
        //            @InvDate,
        //            CAST(@InvAmount AS DECIMAL(10,2)),
        //            @ActualInvoiceAmount,
        //            CAST(@InvAmount - @SettleAmount AS DECIMAL(10,2)),
        //            @PayId,
        //            @PayDate,
        //            CAST(@PayAmount AS DECIMAL(10,2)),
        //            CAST(@PayAmount - @SettleAmount AS DECIMAL(10,2))
        //        FROM #Invoices
        //        WHERE Id = @InvId;

        //        UPDATE #Invoices
        //        SET 
        //            RestAmount = RestAmount - @SettleAmount,
        //            PaidAmount = PaidAmount + @SettleAmount
        //        WHERE Id = @InvId;

        //        UPDATE #Payments
        //        SET RestAmount = RestAmount - @SettleAmount
        //        WHERE Id = @PayId;

        //        SET @PayAmount = @PayAmount - @SettleAmount;

        //        SET @Progress = 1; -- Mark progress to avoid infinite loop
        //    END;

        //    IF (@Progress = 0)
        //    BEGIN
        //        -- No progress in this iteration, break outer loop to prevent infinite looping
        //        BREAK;
        //    END
        //END;

        //-- Step 3: Update original tables
        //UPDATE sd
        //SET 
        //    sd.RestAmount = inv.RestAmount,
        //    sd.PaidAmount = inv.PaidAmount,
        //    sd.Processed = CASE WHEN inv.RestAmount = 0 THEN 1 ELSE 0 END
        //FROM SaleDeleveries sd
        //JOIN #Invoices inv ON sd.Id = inv.Id;

        //UPDATE cp
        //SET 
        //    cp.RestAmount = pay.RestAmount,
        //    cp.Processed = CASE WHEN pay.RestAmount = 0 THEN 1 ELSE 0 END
        //FROM CustomerPaymentCollection cp
        //JOIN #Payments pay ON cp.Id = pay.Id;

        //DROP TABLE #Payments;
        //DROP TABLE #Invoices;
        //;
        //";

        //                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //                {
        //                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
        //                    cmd.Parameters.AddWithValue("@Id", vm.Id);
        //                        if (vm.SaleDeleveryId > 0)
        //                        {
        //                            cmd.Parameters.AddWithValue("@SaleDeleveryId", vm.SaleDeleveryId);
        //                        }
        //                        await cmd.ExecuteNonQueryAsync();
        //                }

        //                if (isNewConnection)
        //                {
        //                    transaction.Commit();
        //                }

        //                result.Status = "Success";
        //                result.Message = "Payment settlement processed successfully.";
        //                return result;
        //            }
        //            catch (Exception ex)
        //            {
        //                if (transaction != null && isNewConnection)
        //                {
        //                    transaction.Rollback();
        //                }
        //                result.Message = ex.Message;
        //                result.ExMessage = ex.Message;
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

        public async Task<ResultVM> MultiplePaymentSettlementProcess(CustomerPaymentCollectionVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
-- Step 1: Prepare temporary tables for easier control
SELECT 
    Id, 
    CAST(RestAmount AS DECIMAL(10,2)) AS RestAmount, 
    CreatedOn AS PaymentDate 
INTO #Payments
FROM CustomerPaymentCollection
WHERE ISNULL(Processed, 0) = 0 
  AND ISNULL(IsPost, 0) = 1 
  AND CustomerId = @CustomerId
  AND Id = @Id
ORDER BY Id;

SELECT 
    Id, 
    CAST(RestAmount AS DECIMAL(10,2)) AS RestAmount, 
    DeliveryDate AS InvoiceDate, 
    CAST(0.00 AS DECIMAL(10,2)) AS PaidAmount,
    CAST(GrandTotalAmount AS DECIMAL(10,2)) AS GrandTotalAmount,
    CustomerId
INTO #Invoices
FROM SaleDeleveries
WHERE ISNULL(IsPost, 0) = 1 
  AND ISNULL(Processed, 0) = 0 
  AND ISNULL(RestAmount, 0) > 0 
  AND CustomerId = @CustomerId";

                if (vm.SaleDeleveryId > 0)
                {
                    query += @" AND Id = @SaleDeleveryId";
                }

                query += @"
ORDER BY Id;

-- Early exit if no payments or invoices to process
IF NOT EXISTS (SELECT 1 FROM #Payments WHERE RestAmount > 0)
BEGIN
    DROP TABLE #Payments;
    DROP TABLE #Invoices;
    RETURN;
END

IF NOT EXISTS (SELECT 1 FROM #Invoices WHERE RestAmount > 0)
BEGIN
    DROP TABLE #Payments;
    DROP TABLE #Invoices;
    RETURN;
END

-- Step 2: Loop through payments and settle invoices FIFO
DECLARE @PayId INT, @PayAmount DECIMAL(10,2), @PayDate DATE;
DECLARE @InvId INT, @InvCustomerId INT, @InvAmount DECIMAL(10,2), @InvDate DATE, @ActualInvoiceAmount DECIMAL(10,2);
DECLARE @SettleAmount DECIMAL(10,2);
DECLARE @Progress BIT;

WHILE EXISTS (SELECT 1 FROM #Payments WHERE RestAmount > 0)
BEGIN
    SET @Progress = 0;  -- Reset progress flag at start of each outer loop iteration
    
    SELECT TOP 1 
        @PayId = Id, 
        @PayAmount = RestAmount, 
        @PayDate = PaymentDate 
    FROM #Payments 
    WHERE RestAmount > 0 
    ORDER BY Id;

    WHILE @PayAmount > 0 AND EXISTS (SELECT 1 FROM #Invoices WHERE RestAmount > 0)
    BEGIN
        SELECT TOP 1 
            @InvId = Id, 
            @InvAmount = RestAmount, 
            @InvDate = InvoiceDate,
            @ActualInvoiceAmount = GrandTotalAmount,
            @InvCustomerId = CustomerId
        FROM #Invoices 
        WHERE RestAmount > 0 
        ORDER BY Id;

        SET @SettleAmount = CASE 
                                WHEN @PayAmount >= @InvAmount THEN @InvAmount 
                                ELSE @PayAmount 
                            END;

        -- If settle amount is zero or negative, break inner loop to avoid infinite loop
        IF (@SettleAmount <= 0)
        BEGIN
            BREAK;
        END

        INSERT INTO PaymentCollectionSettlementHistory (
            InvoiceId, CustomerId, InvoiceDate, InvoiceAmount, ActualInvoiceAmount, InvoiceRestAmount,
            PaymentId, PaymentDate, PaymentAmount, PaymentRestAmount
        )
        SELECT 
            @InvId,
            @InvCustomerId,
            @InvDate,
            CAST(@InvAmount AS DECIMAL(10,2)),
            @ActualInvoiceAmount,
            CAST(@InvAmount - @SettleAmount AS DECIMAL(10,2)),
            @PayId,
            @PayDate,
            CAST(@PayAmount AS DECIMAL(10,2)),
            CAST(@PayAmount - @SettleAmount AS DECIMAL(10,2))
        FROM #Invoices
        WHERE Id = @InvId;

        UPDATE #Invoices
        SET 
            RestAmount = RestAmount - @SettleAmount,
            PaidAmount = PaidAmount + @SettleAmount
        WHERE Id = @InvId;

        UPDATE #Payments
        SET RestAmount = RestAmount - @SettleAmount
        WHERE Id = @PayId;

        SET @PayAmount = @PayAmount - @SettleAmount;

        SET @Progress = 1; -- Mark progress to avoid infinite loop
    END;

    IF (@Progress = 0)
    BEGIN
        -- No progress in this iteration, break outer loop to prevent infinite looping
        BREAK;
    END
END;

-- Step 3: Update original tables
UPDATE sd
SET 
    sd.RestAmount = inv.RestAmount,
    sd.PaidAmount = inv.PaidAmount,
    sd.Processed = CASE WHEN inv.RestAmount = 0 THEN 1 ELSE 0 END
FROM SaleDeleveries sd
JOIN #Invoices inv ON sd.Id = inv.Id;

UPDATE cp
SET 
    cp.RestAmount = pay.RestAmount,
    cp.Processed = CASE WHEN pay.RestAmount = 0 THEN 1 ELSE 0 END
FROM CustomerPaymentCollection cp
JOIN #Payments pay ON cp.Id = pay.Id;

DROP TABLE #Payments;
DROP TABLE #Invoices;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                    cmd.Parameters.AddWithValue("@Id", vm.Id);
                    if (vm.SaleDeleveryId > 0)
                    {
                        cmd.Parameters.AddWithValue("@SaleDeleveryId", vm.SaleDeleveryId);
                    }
                    await cmd.ExecuteNonQueryAsync();
                }

                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Payment settlement processed successfully.";
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


        public async Task<ResultVM> GetList(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
Select * FROM CustomerPaymentCollection
WHERE ISNULL(Processed, 0) = 0 

and RestAmount>0

 ";

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

        public async Task<ResultVM> GetTabGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDeliveryVM>();

                // Define your SQL query string
                string sqlQuery = @"
               -- Count query
               SELECT COUNT(DISTINCT S.Id) AS totalcount
			            FROM [SaleDeleveries] S
                          left outer join Customers C on c.Id=s.CustomerId
                          left outer join Routes R on R.Id=s.RouteId
                          where 1 = 1
                -- Add the filter condition
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

                -- Data query with pagination and sorting
                SELECT * 
                FROM (
                    SELECT 
                    ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "S.Id DESC ") + @") AS rowindex,
                              s.Code
	                          ,c.Name CustomerName
                              ,R.Name RouteName
                              ,s.DeliveryAddress
                              ,ISNULL(FORMAT(s.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') InvoiceDateTime
                              ,ISNULL(FORMAT(s.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') DeliveryDate
	                          ,ISNULL(s.GrandTotalAmount,0) InvoiceAmount
                              ,ISNULL(s.PaidAmount,0) PaidAmount   
                              ,ISNULL(s.RestAmount,0) RestAmount
                              ,CASE WHEN ISNULL(s.Processed, 0) = 1 THEN 'Paid' ELSE 'UnPaid' END AS Status

                          FROM SaleDeleveries S
                          left outer join Customers C on c.Id=s.CustomerId
                          left outer join Routes R on R.Id=s.RouteId
                          where 1 = 1
                -- Add the filter condition
                " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDeliveryVM>.FilterCondition(options.filter) + ")" : "") + @"

                ) AS a
                WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
            ";

                data = KendoGrid<SaleDeliveryVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
