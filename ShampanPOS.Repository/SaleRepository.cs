using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using Newtonsoft.Json;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{

    public class SaleRepository : CommonRepository
    {
        public async Task<ResultVM> Insert(SaleVM sale, SqlConnection conn = null, SqlTransaction transaction = null)
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
        INSERT INTO Sales 
        (Code, BranchId,CompanyId, CustomerId, SaleOrderId, DeliveryAddress,
         InvoiceDateTime, Comments,SubTotal, TotalSD, TotalVAT, GrandTotal, PaidAmount, 
        TransactionType, IsPost, PeriodId, CreatedBy, CreatedOn, CreatedFrom)
        VALUES 
        (@Code, @BranchId,@CompanyId, @CustomerId, @SaleOrderId,@DeliveryAddress, 
         @InvoiceDateTime, @Comments,@SubTotal, @TotalSD, @TotalVAT, @GrandTotal, @PaidAmount, @TransactionType, @IsPost, @PeriodId, 
         @CreatedBy, GETDATE(), @CreatedFrom);
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Add all the parameters as before
                    cmd.Parameters.AddWithValue("@Code", sale.Code);
                    cmd.Parameters.AddWithValue("@BranchId", sale.BranchId);
                    cmd.Parameters.AddWithValue("@CompanyId", sale.CompanyId);
                    cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);

                    // Ensure that SaleOrderId is provided and not null
                    cmd.Parameters.AddWithValue("@SaleOrderId", sale.SaleOrderId ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@DeliveryAddress", sale.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", sale.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                    cmd.Parameters.AddWithValue("@TotalSD", sale.TotalSD);
                    cmd.Parameters.AddWithValue("@TotalVAT", sale.TotalVAT);
                    cmd.Parameters.AddWithValue("@GrandTotal", sale.GrandTotal);
                    cmd.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                    cmd.Parameters.AddWithValue("@TransactionType", sale.TransactionType);
                    cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.AddWithValue("@PeriodId", sale.PeriodId ?? "");
                    cmd.Parameters.AddWithValue("@CreatedBy", sale.CreatedBy ?? "ERP");
                    cmd.Parameters.AddWithValue("@CreatedFrom", sale.CreatedFrom ?? (object)DBNull.Value);

                    // Execute the query
                    object newId = cmd.ExecuteScalar();
                    sale.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Data inserted successfully.";
                    result.Id = sale.Id.ToString();
                    result.DataVM = sale;
                }


                // Commit transaction only if everything is successful
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

        public async Task<ResultVM> InsertDetails(SaleDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                INSERT INTO SaleDetails
                (SaleId,SaleOrderId, SaleOrderDetailId, Line, ProductId, CompanyId,
                 Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal)
                VALUES 
                (@SaleId, @SaleOrderId, @SaleOrderDetailId, @Line, @ProductId, @CompanyId,
                 @Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal );
                SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Adding parameters to the SQL command
                    cmd.Parameters.AddWithValue("@SaleId", details.SaleId);
                    cmd.Parameters.AddWithValue("@SaleOrderId", details.SaleOrderId ?? 0);
                    cmd.Parameters.AddWithValue("@SaleOrderDetailId", details.SaleOrderDetailId ?? 0);
                    cmd.Parameters.AddWithValue("@Line", details.Line);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", details.SD ?? 0);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount ?? 0);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate ?? 0);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount ?? 0);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal ?? 0);
                    //cmd.Parameters.AddWithValue("@VATType", "VAT");
                    //cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int)
                                  .Value = (object?)details.CompanyId ?? DBNull.Value;


                    // Execute the query and get the new ID (primary key)
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
        public async Task<ResultVM> Update(SaleVM sale, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = sale.Id.ToString(), DataVM = sale };

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
        UPDATE Sales
        SET 
            BranchId = @BranchId,
            CompanyId = @CompanyId,
            CustomerId = @CustomerId,
            SaleOrderId = @SaleOrderId,
            DeliveryAddress = @DeliveryAddress,
            InvoiceDateTime = @InvoiceDateTime,
            Comments = @Comments,
            SubTotal = @SubTotal, 
            TotalSD = @TotalSD,
            TotalVAT = @TotalVAT,
            GrandTotal = @GrandTotal,
            PaidAmount = @PaidAmount,
            TransactionType = @TransactionType,
            IsPost = @IsPost,
            PeriodId = @PeriodId,
            LastModifiedBy = @LastModifiedBy,
            LastModifiedOn = @LastModifiedOn,
            LastUpdateFrom = @LastUpdateFrom
        WHERE Id = @Id;
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", sale.Id);
                    cmd.Parameters.AddWithValue("@BranchId", sale.BranchId);
                    cmd.Parameters.AddWithValue("@CompanyId", sale.CompanyId);
                    cmd.Parameters.AddWithValue("@CustomerId", sale.CustomerId);

                    // Ensure SaleOrderId is not null, use DBNull.Value if it's null
                    cmd.Parameters.AddWithValue("@SaleOrderId", sale.SaleOrderId ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@DeliveryAddress", sale.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", sale.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubTotal", sale.SubTotal);
                    cmd.Parameters.AddWithValue("@TotalSD", sale.TotalSD);
                    cmd.Parameters.AddWithValue("@TotalVAT", sale.TotalVAT);
                    cmd.Parameters.AddWithValue("@GrandTotal", sale.GrandTotal);
                    cmd.Parameters.AddWithValue("@PaidAmount", sale.PaidAmount);
                    cmd.Parameters.AddWithValue("@IsPrint", false);
                    cmd.Parameters.AddWithValue("@TransactionType", sale.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", false);
                    cmd.Parameters.AddWithValue("@PeriodId", sale.PeriodId ?? (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@LastModifiedBy", sale.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedOn", sale.LastModifiedOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", sale.LastUpdateFrom ?? (object)DBNull.Value);

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
				ISNULL(M.Id, 0) AS Id,
				ISNULL(M.Code, '') AS Code,
				ISNULL(M.BranchId, 0) AS BranchId,
				ISNULL(M.CompanyId, 0) AS CompanyId,
				ISNULL(M.CustomerId, 0) AS CustomerId,
				ISNULL(M.SaleOrderId, 0) AS SaleOrderId,
				ISNULL(S.Code, 0) AS SaleOrderCode,
				ISNULL(M.SubTotal, 0) AS SubTotal,
				ISNULL(M.TotalSD, 0) AS TotalSD,
				ISNULL(M.TotalVAT, 0) AS TotalVAT,
				ISNULL(M.GrandTotal, 0) AS GrandTotal,
				ISNULL(M.PaidAmount, 0) AS PaidAmount,
				ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
				ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
				ISNULL(M.Comments, '') AS Comments,     
				ISNULL(M.TransactionType, '') AS TransactionType,
				ISNULL(M.IsPost, 0) AS IsPost,
				ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd'), '1900-01-01') AS      PostedOn, 
				ISNULL(M.PeriodId, 0) AS PeriodId,
				ISNULL(M.CreatedBy, '') AS CreatedBy,
				ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
				ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
				ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
				ISNULL(M.CreatedFrom, '') AS CreatedFrom,
				ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
				ISNULL(Br.Name,'') BranchName,
				ISNULL(CP.CompanyName,'') CompanyName,
				ISNULL(cus.Name,'') CustomerName
			FROM 
				Sales M
	            LEFT OUTER JOIN BranchProfiles Br ON M.BranchId = Br.Id
				LEFT OUTER JOIN CompanyProfiles CP ON M.CompanyId = CP.Id
                LEFT OUTER JOIN Customers cus ON M.CustomerId = cus.Id
				LEFT OUTER JOIN SaleOrders S ON M.SaleOrderId = S.Id
			WHERE 
				1 = 1
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

                var model = new List<SaleVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        SubTotal = row.Field<decimal>("SubTotal"),
                        TotalSD = row.Field<decimal>("TotalSD"),
                        TotalVAT = row.Field<decimal>("TotalVAT"),
                        GrandTotal = row.Field<decimal>("GrandTotal"),
                        PaidAmount = row.Field<decimal>("PaidAmount"),
                        Code = row["Code"].ToString(),
                        SaleOrderCode = row["SaleOrderCode"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CompanyId = Convert.ToInt32(row["CompanyId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        SaleOrderId = Convert.ToInt32(row["SaleOrderId"]),
                        DeliveryAddress = row["DeliveryAddress"].ToString(),
                        InvoiceDateTime = row["InvoiceDateTime"] != DBNull.Value ? Convert.ToDateTime(row["InvoiceDateTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        Comments = row["Comments"].ToString(),
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = row["IsPost"] != DBNull.Value ? Convert.ToBoolean(row["IsPost"]) : false,
                        PostedOn = row["PostedOn"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        PeriodId = row["PeriodId"].ToString(),
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["LastModifiedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CreatedFrom = row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row["LastUpdateFrom"].ToString()
                    });
                }

                //conditionalValue = new string[] { model.FirstOrDefault().Code.ToString() };

                var detailsDataList = DetailsList(new[] { "D.SaleId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDetailVM>>(json);

                    model.FirstOrDefault().saleDetailsList = details;
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
ISNULL(D.SaleId, 0) AS SaleId,
ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
ISNULL(D.CompanyId, 0) AS CompanyId,
ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0.00) AS Quantity,
ISNULL(FORMAT(D.UnitRate, 'N2'), '0.00') AS UnitRate,
ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(CP.CompanyName,'') CompanyName


FROM 
SaleDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

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

                if (vm != null && int.TryParse(vm.Id, out int id))
                {
                    query += " AND D.Id = @Id ";
                    objComm.SelectCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;
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
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.SaleOrderId, 0) AS SaleOrderId,
    ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(M.Comments, '') AS Comments,     
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd'), '1900-01-01') AS      PostedOn, 
    ISNULL(M.PeriodId, 0) AS PeriodId,
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

                string query = $" UPDATE Sales SET IsPost = 1, PostedBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                //query += $" UPDATE SaleDetails SET IsPost = 1 WHERE saleId IN ({inClause}) ";

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
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SaleVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
                FROM Sales H
                LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
                LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
		        LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
			    LEFT OUTER JOIN SaleOrders P ON H.SaleOrderId = P.Id 

                WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                
				ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.BranchId, 0) AS BranchId,
                ISNULL(H.CompanyId, 0) AS CompanyId,
                ISNULL(H.CustomerId, 0) AS CustomerId,
				ISNULL(s.Name, 0) AS CustomerName,
				ISNULL(H.SaleOrderId, 0) AS SaleOrderId,
				ISNULL(P.Code, 0) AS SaleOrderCode,
				ISNULL(H.SubTotal, 0) AS SubTotal,
				ISNULL(H.TotalSD, 0) AS TotalSD,
				ISNULL(H.TotalVAT, 0) AS TotalVAT,
				ISNULL(H.GrandTotal, 0) AS GrandTotal,
				ISNULL(H.PaidAmount, 0) AS PaidAmount,
                ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
                ISNULL(H.Comments, '') AS Comments,
                ISNULL(H.TransactionType, '') AS TransactionType,
                ISNULL(H.IsPost, 0) AS IsPost,	            
                CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
                ISNULL(H.PostedBy, '') AS PostedBy,
                ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
                ISNULL(H.PeriodId, '') AS PeriodId,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
				ISNULL(P.Code, '') AS PurchaseOrderCode,
                ISNULL(Br.Name,'') BranchName,               
                ISNULL(Br.Address, '') AS BranchAddress,
                ISNULL(S.Name,'') SupplierName,
                ISNULL(S.Address, '') AS SupplierAddress,
				ISNULL(CP.CompanyName,'') CompanyName



                FROM Sales H
                LEFT OUTER JOIN Customers s on h.CustomerId = s.Id
                LEFT OUTER JOIN BranchProfiles br on h.BranchId = br.Id
		        LEFT OUTER JOIN CompanyProfiles CP ON H.CompanyId = CP.Id
			    LEFT OUTER JOIN SaleOrders P ON H.SaleOrderId = P.Id 

                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        /// GetDetailsGridData
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SaleDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
                FROM SaleOrders H
                LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
                LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
                LEFT OUTER JOIN BranchProfiles BR ON H.BranchId = BR.Id
                LEFT OUTER JOIN SaleOrderDetails D ON H.Id = D.SaleOrderId
                LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
	            LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

                WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetailVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        
                
                ISNULL(H.Id, 0) AS Id,
                ISNULL(H.Code, '') AS Code,
                ISNULL(H.DeliveryAddress, '') AS DeliveryAddress,
                ISNULL(FORMAT(H.OrderDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS OrderDate,
                ISNULL(FORMAT(H.DeliveryDate, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS DeliveryDate,
                ISNULL(H.Comments, '') AS Comments,
                ISNULL(H.TransactionType, '') AS TransactionType,
                ISNULL(H.CreatedBy, '') AS CreatedBy,
                ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
                ISNULL(H.CreatedFrom, '') AS CreatedFrom,
                ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,
                ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
                ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS LastModifiedOn,
                ISNULL(H.BranchId, 0) AS BranchId,
                ISNULL(H.CustomerId, 0) AS CustomerId,
                ISNULL(C.Name, '') AS CustomerName,
                ISNULL(BR.Name, '-') AS BranchName,
                ISNULL(PD.Name, '') AS ProductName,
	            ISNULL(CP.CompanyName,'') CompanyName,

                -- Detail Information
                ISNULL(D.Quantity, 0) AS Quantity,
                ISNULL(D.UnitRate, 0) AS UnitRate,
                ISNULL(D.SubTotal, 0) AS SubTotal,
                ISNULL(D.SD, 0) AS SD,
                ISNULL(D.SDAmount, 0) AS SDAmount,
                ISNULL(D.VATRate, 0) AS VATRate,
                ISNULL(D.VATAmount, 0) AS VATAmount,
                ISNULL(D.LineTotal, 0) AS LineTotal
  
    
  

                FROM SaleOrders H
                LEFT OUTER JOIN BranchProfiles BF ON H.BranchId = BF.Id
                LEFT OUTER JOIN Customers C ON H.CustomerId = C.Id
                LEFT OUTER JOIN BranchProfiles BR ON H.BranchId = BR.Id
                LEFT OUTER JOIN SaleOrderDetails D ON H.Id = D.SaleOrderId
                LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
	            LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetailVM>.FilterCondition(options.filter) + ")" : "");

                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleDetailVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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
        //public async Task<ResultVM> InsertDetail(SaleVM saleDetails, SqlConnection conn, SqlTransaction transaction)
        //{
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    try
        //    {
        //        // SQL Insert query
        //        string query = @"
        //INSERT INTO SaleDetails
        //(SaleId, SaleDeliveryId, SaleDeliveryDetailId, SaleOrderId, SaleOrderDetailId, BranchId, Line, ProductId, 
        //Quantity, UnitRate, SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, 
        //UOMconversion, Comments, VATType, TransactionType, IsPost)
        //VALUES 
        //(@SaleId, @SaleDeliveryId, @SaleDeliveryDetailId, @SaleOrderId, @SaleOrderDetailId, @BranchId, @Line, @ProductId, 
        //@Quantity, @UnitRate, @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, 
        //@UOMconversion, @Comments, @VATType, @TransactionType, @IsPost);
        //SELECT SCOPE_IDENTITY();";

        //        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //        {
        //            // Loop through the sale details list and insert each detail
        //            foreach (var detail in saleDetails.saleDetailsList)
        //            {
        //                // Clear parameters to avoid conflicts
        //                cmd.Parameters.Clear();

        //                // Adding parameters to the SQL command
        //                cmd.Parameters.AddWithValue("@SaleId", saleDetails.Id);
        //                cmd.Parameters.AddWithValue("@SaleDeliveryId", detail.SaleDeliveryId ?? 0);
        //                cmd.Parameters.AddWithValue("@SaleDeliveryDetailId", detail.SaleDeliveryDetailId ?? 0);
        //                cmd.Parameters.AddWithValue("@SaleOrderId", detail.SaleOrderId ?? 0);
        //                cmd.Parameters.AddWithValue("@SaleOrderDetailId", detail.SaleOrderDetailId ?? 0);
        //                cmd.Parameters.AddWithValue("@BranchId", saleDetails.BranchId);
        //                cmd.Parameters.AddWithValue("@Line", detail.Line ?? 0);
        //                cmd.Parameters.AddWithValue("@ProductId", detail.ProductId ?? 0);
        //                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity ?? 0);
        //                cmd.Parameters.AddWithValue("@UnitRate", detail.UnitRate ?? 0);
        //                cmd.Parameters.AddWithValue("@SubTotal", detail.SubTotal ?? 0);
        //                cmd.Parameters.AddWithValue("@SD", detail.SD ?? 0);
        //                cmd.Parameters.AddWithValue("@SDAmount", detail.SDAmount ?? 0);
        //                cmd.Parameters.AddWithValue("@VATRate", detail.VATRate ?? 0);
        //                cmd.Parameters.AddWithValue("@VATAmount", detail.VATAmount ?? 0);
        //                cmd.Parameters.AddWithValue("@LineTotal", detail.LineTotal ?? 0);
        //                cmd.Parameters.AddWithValue("@UOMId", detail.UOMId ?? 0);
        //                cmd.Parameters.AddWithValue("@UOMFromId", detail.UOMFromId ?? 0);
        //                cmd.Parameters.AddWithValue("@UOMConversion", detail.UOMConversion ?? 0);
        //                cmd.Parameters.AddWithValue("@VATType", "VAT"); // Assuming VAT is constant
        //                cmd.Parameters.AddWithValue("@TransactionType", saleDetails.TransactionType ?? "");
        //                cmd.Parameters.AddWithValue("@IsPost", saleDetails.IsPost);
        //                cmd.Parameters.AddWithValue("@Comments", detail.Comments ?? "");

        //                // Execute the query and get the new ID (primary key)
        //                object newId = await cmd.ExecuteScalarAsync();
        //                detail.Id = Convert.ToInt32(newId);
        //            }

        //            // Commit the transaction if everything goes well
        //            result.Status = "Success";
        //            result.Message = "Details Data inserted successfully.";
        //            result.Id = saleDetails.Id.ToString();
        //            result.DataVM = saleDetails;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // In case of error, rollback the transaction (if needed) and set result messages
        //        result.ExMessage = ex.Message;
        //        result.Message = ex.Message;
        //    }

        //    return result;
        //}

        //public async Task<ResultVM> ProductWiseSale(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
        //                   SELECT     
        //                    -- Details Data
        //                    ISNULL(D.ProductId, 0) AS ProductId,
        //                    ISNULL(PG.Name, '') AS ProductGroupName,
        //                    ISNULL(P.Code, '') AS ProductCode,
        //                    ISNULL(P.Name, '') AS ProductName,
        //                    ISNULL(P.HSCodeNo, '') AS HSCodeNo,
        //                    ISNULL(uom.Name, '') AS UOMName,
        //                    ISNULL(SUM(D.Quantity), 0) AS Quantity

        //                    FROM Sales M
        //                    LEFT OUTER JOIN SaleDetails D ON ISNULL(M.Id,0) = D.SaleId
        //                    LEFT OUTER JOIN Products P ON D.ProductId = P.Id
        //                    LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //                    LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
        //                    LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
		      //              WHERE 1 = 1 
		      //            ";

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            query += " AND M.Id = @Id ";
        //        }
        //        if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
        //        {
        //            query += " AND CAST(M.DeliveryDate AS DATE) BETWEEN @FromDate AND @ToDate ";
        //        }

        //        // Apply additional conditions
        //        query = ApplyConditions(query, conditionalFields, conditionalValue, false);
        //        query += @" GROUP BY    
        //                            D.ProductId,
        //                            P.HSCodeNo,
        //                            P.Code,
        //                            P.Name,
        //                            PG.Name,
        //                            uom.Name ";
        //        SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //        // SET additional conditions param
        //        objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

        //        if (vm != null && !string.IsNullOrEmpty(vm.Id))
        //        {
        //            objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
        //        }
        //        if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
        //        {
        //            objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
        //            objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
        //        }

        //        objComm.Fill(dataTable);

        //        var lst = new List<SaleReportVM>();
        //        int serialNumber = 1;
        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            lst.Add(new SaleReportVM
        //            {
        //                SL = serialNumber,
        //                ProductId = Convert.ToInt32(row["ProductId"]),
        //                ProductGroupName = row["ProductGroupName"].ToString(),
        //                ProductCode = row["ProductCode"].ToString(),
        //                ProductName = row["ProductName"].ToString(),
        //                HSCodeNo = row["HSCodeNo"].ToString(),
        //                UOMName = row["UOMName"].ToString(),
        //                Quantity = Convert.ToInt32(row["Quantity"])
        //            });
        //            serialNumber++;
        //        }

        //        result.Status = "Success";
        //        result.Message = "Data retrieved successfully.";
        //        result.DataVM = lst;
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

ISNULL(M.Id, 0) AS Id,
ISNULL(M.RouteId, 0) AS RouteId,
ISNULL(R.Name, '') + ', ' + ISNULL(R.Address, '') AS RouteAddress,
ISNULL(M.Code, '') AS Code,
ISNULL(M.BranchId, 0) AS BranchId,
ISNULL(TRIM(branch.Name), '') AS BranchName,
ISNULL(TRIM(branch.Code), '') AS BranchCode,
ISNULL(TRIM(branch.Address), '') AS BranchAddress,
ISNULL(M.CustomerId, 0) AS CustomerId,
ISNULL(C.Name, '') AS CustomerName,
ISNULL(C.Address, '') AS CustomerAddress,
ISNULL(M.SalePersonId, 0) AS SalePersonId,
ISNULL(SP.Name, '') AS SalePersonName,
ISNULL(M.DeliveryAddress, '') AS DeliveryAddress,
ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
ISNULL(FORMAT(M.DeliveryDate, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDate,
ISNULL(M.GrandTotalAmount, 0) AS GrandTotalAmount,
ISNULL(M.GrandTotalSDAmount, 0) AS GrandTotalSDAmount,
ISNULL(M.GrandTotalVATAmount, 0) AS GrandTotalVATAmount,
ISNULL(M.Comments, '') AS Comments,
ISNULL(M.TransactionType, '') AS TransactionType,
ISNULL(M.CurrencyId, 0) AS CurrencyId,
ISNULL(Cur.Name, '') AS CurrencyName,
ISNULL(M.CurrencyRateFromBDT, 0) AS CurrencyRateFromBDT,
ISNULL(M.CreatedBy, '') AS CreatedBy,
ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,


ISNULL(D.Id, 0) AS DetailId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0) AS Quantity,
ISNULL(D.UnitRate, 0) AS UnitPrice,
ISNULL(D.SubTotal,0) AS SubTotal,
ISNULL(D.SD,0) AS SD,
ISNULL(D.SDAmount,0) AS SDAmount,
ISNULL(D.VATRate,0) AS VATRate,
ISNULL(D.VATAmount,0) AS VATAmount,
ISNULL(D.LineTotal,0) AS LineTotal,
ISNULL(D.UOMId, 0) AS UOMId,
ISNULL(D.UOMFromId, 0) AS UOMFromId,
ISNULL(D.UOMConversion,0) AS UOMConversion,
ISNULL(TRIM(D.Comments), '') AS DetailComments,
ISNULL(TRIM(P.Name),'') ProductName,
ISNULL(TRIM(P.BanglaName),'') BanglaName, 
ISNULL(TRIM(P.Code),'') ProductCode, 
ISNULL(TRIM(P.HSCodeNo),'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(TRIM(UOM.Name),'') UOMName,
ISNULL(TRIM(UOM.Name),'') UOMFromName
 
FROM Sales M
LEFT OUTER JOIN SaleDetails D ON ISNULL(M.Id,0) = D.SaleId
LEFT OUTER JOIN Routes R on ISNULL(M.RouteId,0) = R.Id
LEFT OUTER JOIN Customers C on ISNULL(M.CustomerId,0) = C.Id
LEFT OUTER JOIN SalesPersons SP on M.SalePersonId = SP.Id
LEFT OUTER JOIN Currencies Cur ON ISNULL(M.CurrencyId,0) = Cur.Id
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
LEFT OUTER JOIN BranchProfiles branch ON M.BranchId = branch.Id

WHERE  1 = 1 ";

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




        public async Task<ResultVM> GetSaleDetailDataById(GridOptions options, int masterId, SqlConnection conn, SqlTransaction transaction)
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

                var data = new GridEntity<SaleDetailVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
            SELECT COUNT(DISTINCT D.Id) AS totalcount
            FROM 
            SaleDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
            WHERE D.SaleId= @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "D.Id DESC") + @") AS rowindex,
        
            ISNULL(D.Id, 0) AS Id,
            ISNULL(D.SaleId, 0) AS SaleId,
            ISNULL(D.Line, 0) AS Line,
            ISNULL(D.ProductId, 0) AS ProductId,
            ISNULL(D.Quantity, 0) AS Quantity,
            ISNULL(D.UnitRate,0) AS UnitRate,
            ISNULL(D.SubTotal,0) AS SubTotal,
            ISNULL(D.SD,0) AS SD,
            ISNULL(D.SDAmount,0) AS SDAmount,
            ISNULL(D.VATRate,0) AS VATRate,
            ISNULL(D.VATAmount,0) AS VATAmount,
            ISNULL(D.LineTotal,0) AS LineTotal,

            ISNULL(P.Name, '') AS ProductName,
            ISNULL(P.BanglaName, '') AS BanglaName,
            ISNULL(P.Code, '') AS ProductCode,
            ISNULL(P.HSCodeNo, '') AS HSCodeNo,
            ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
            ISNULL(PG.Name, '') AS ProductGroupName,
            ISNULL(UOM.Name, '') AS UOMName,
            ISNULL(UOM.Name, '') AS UOMFromName
            FROM 
            SaleDetails D
            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
            LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
            WHERE D.SaleId= @masterId

    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleDetailVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                sqlQuery = sqlQuery.Replace("@masterId", "" + masterId + "");
                data = KendoGrid<SaleDetailVM>.GetGridData_CMD(options, sqlQuery, "D.Id");

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



        public async Task<ResultVM> SaleList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.PeriodId, '') AS PeriodId,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom
FROM 
    Sales M
WHERE 1 = 1
 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND M.Id IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

                var lst = new List<SaleReturnVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleReturnVM
                    {
                        Id = row.Field<int>("Id"),
                        //SaleId = row.Field<int>("SaleId"),
                        BranchId = row.Field<int>("BranchId"),
                        CustomerId = row.Field<int>("CustomerId"),
                        InvoiceDateTime = row.Field<string>("InvoiceDateTime"),
                        Comments = row.Field<string>("Comments"),
                        TransactionType = row.Field<string>("TransactionType"),
                        IsPost = row.Field<bool>("IsPost"),
                        PostBy = row.Field<string>("PostedBy"),
                        PostedOn = row.Field<string?>("PostedOn"),
                        PeriodId = row.Field<string>("PeriodId"),
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




        public async Task<ResultVM> SaleDetailsList(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(D.SaleId, 0) AS SaleId,
ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
ISNULL(D.CompanyId, 0) AS CompanyId,
ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(D.Quantity, 0.00) AS Quantity,
ISNULL(FORMAT(D.UnitRate, 'N2'), '0.00') AS UnitRate,
ISNULL(FORMAT(D.SubTotal, 'N2'), '0.00') AS SubTotal,
ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
ISNULL(FORMAT(D.LineTotal, 'N2'), '0.00') AS LineTotal,
ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(CP.CompanyName,'') CompanyName


FROM 
SaleDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN CompanyProfiles CP ON D.CompanyId = CP.Id

WHERE 1 = 1";


                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND D.SaleId IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

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



        //        public async Task<ResultVM> SaleListForPayment(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
        //    ISNULL(M.Id, 0) AS PurchaseId,
        //    ISNULL(M.Code, '') AS Code,
        //    ISNULL(M.BranchId, 0) AS BranchId,
        //    ISNULL(M.SupplierId, 0) AS SupplierId,
        //    ISNULL(M.BENumber, '') AS BENumber,
        //    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
        //    ISNULL(FORMAT(M.PurchaseDate, 'yyyy-MM-dd'), '1900-01-01') AS PurchaseDate,
        //    ISNULL(M.Comments, '') AS Comments,
        //    ISNULL(M.TransactionType, '') AS TransactionType,
        //    ISNULL(M.IsPost, 0) AS IsPost,
        //    ISNULL(M.PostedBy, '') AS PostedBy,
        //    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
        //    ISNULL(M.FiscalYear, '') AS FiscalYear,
        //    ISNULL(M.PeriodId, '') AS PeriodId,
        //    ISNULL(M.CreatedBy, '') AS CreatedBy,
        //    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
        //    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
        //    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
        //    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
        //    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
        //    ISNULL(M.GrandTotal, 0) AS GrandTotal

        //FROM 
        //    Purchases M
        //WHERE 1 = 1
        // ";

        //                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

        //                if (IDs.Length > 0)
        //                {
        //                    query += $" AND M.Id IN ({inClause}) ";
        //                }

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
        //                {
        //                    if (transaction != null)
        //                    {
        //                        adapter.SelectCommand.Transaction = transaction;
        //                    }

        //                    for (int i = 0; i < IDs.Length; i++)
        //                    {
        //                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
        //                    }

        //                    adapter.Fill(dataTable);
        //                }

        //                var lst = new List<PaymentVM>();

        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    lst.Add(new PaymentVM
        //                    {
        //                        Id = row.Field<int>("Id"),
        //                        GrandTotal = row.Field<decimal>("GrandTotal"),
        //                        Code = row.Field<string>("Code"),
        //                        //BranchId = row.Field<int>("BranchId"),
        //                        SupplierId = row.Field<int>("SupplierId"),
        //                        //BENumber = row.Field<string>("BENumber"),
        //                        //InvoiceDateTime = row.Field<string>("InvoiceDateTime"),
        //                        //PurchaseDate = row.Field<string>("PurchaseDate"),
        //                        Comments = row.Field<string>("Comments"),
        //                        //TransactionType = row.Field<string>("TransactionType"),
        //                        //IsPost = row.Field<bool>("IsPost"),
        //                        //PostedBy = row.Field<string>("PostedBy"),
        //                        //PostedOn = row.Field<string?>("PostedOn"),
        //                        //FiscalYear = row.Field<string>("FiscalYear"),
        //                        //PeriodId = row.Field<string>("PeriodId"),
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



        //        public async Task<ResultVM> SaleDetailsListForPayment(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
        //ISNULL(Pur.Code,'') PurchaseCode,
        //ISNULL(Pur.Id,0) PurchaseId,
        //ISNULL(FORMAT(Pur.GrandTotal, 'N2'), '0.00') AS PurchaseAmount,
        //ISNULL(D.Id, 0) AS Id,
        //ISNULL(D.Id, 0) AS PurchaseDetailId,
        //ISNULL(D.PurchaseId, 0) AS PurchaseId,
        //ISNULL(D.BranchId, 0) AS BranchId,
        //ISNULL(D.Line, 0) AS Line,
        //ISNULL(D.ProductId, 0) AS ProductId,
        //ISNULL(FORMAT(D.UnitPrice, 'N2'), 0.00) AS UnitPrice,
        //ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
        //ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
        //ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
        //ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,
        //ISNULL(FORMAT(D.OthersAmount, 'N2'), '0.00') AS OthersAmount,

        //ISNULL(P.Name,'') ProductName,
        //ISNULL(P.BanglaName,'') BanglaName,
        //ISNULL(P.Code,'') ProductCode, 
        //ISNULL(P.HSCodeNo,'') HSCodeNo,
        //ISNULL(P.ProductGroupId,0) ProductGroupId,
        //ISNULL(PG.Name,'') ProductGroupName


        //FROM 
        //PurchaseDetails D
        //LEFT OUTER JOIN Products P ON D.ProductId = P.Id
        //LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
        //LEFT OUTER JOIN Purchases Pur ON D.PurchaseId = Pur.Id

        //WHERE 1 = 1";


        //                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

        //                if (IDs.Length > 0)
        //                {
        //                    query += $" AND D.PurchaseId IN ({inClause}) ";
        //                }

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
        //                {
        //                    if (transaction != null)
        //                    {
        //                        adapter.SelectCommand.Transaction = transaction;
        //                    }

        //                    for (int i = 0; i < IDs.Length; i++)
        //                    {
        //                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
        //                    }

        //                    adapter.Fill(dataTable);
        //                }

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


        public async Task<ResultVM> SaleListForPayment(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
    ISNULL(M.Id, 0) AS PurchaseId,
    ISNULL(M.BranchId, 0) AS BranchId,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(FORMAT(M.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.TransactionType, '') AS TransactionType,
    ISNULL(M.IsPost, 0) AS IsPost,
    ISNULL(M.PostedBy, '') AS PostedBy,
    ISNULL(FORMAT(M.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
    ISNULL(M.PeriodId, '') AS PeriodId,
    ISNULL(M.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(M.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
    ISNULL(M.LastModifiedBy, '') AS LastModifiedBy,
    ISNULL(FORMAT(M.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
    ISNULL(M.CreatedFrom, '') AS CreatedFrom,
    ISNULL(M.LastUpdateFrom, '') AS LastUpdateFrom,
    ISNULL(M.GrandTotal, 0) AS GrandTotal

FROM 
    Sales M
WHERE 1 = 1
 ";

                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND M.Id IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

                var lst = new List<CollectionVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new CollectionVM
                    {
                        Id = row.Field<int>("Id"),
                        GrandTotal = row.Field<decimal>("GrandTotal"),
                        //BranchId = row.Field<int>("BranchId"),
                        CustomerId = row.Field<int>("CustomerId"),
                        //BENumber = row.Field<string>("BENumber"),
                        //InvoiceDateTime = row.Field<string>("InvoiceDateTime"),
                        //PurchaseDate = row.Field<string>("PurchaseDate"),
                        Comments = row.Field<string>("Comments"),
                        //TransactionType = row.Field<string>("TransactionType"),
                        //IsPost = row.Field<bool>("IsPost"),
                        //PostedBy = row.Field<string>("PostedBy"),
                        //PostedOn = row.Field<string?>("PostedOn"),
                        //FiscalYear = row.Field<string>("FiscalYear"),
                        //PeriodId = row.Field<string>("PeriodId"),
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



        public async Task<ResultVM> SaleDetailsListForPayment(string?[] IDs, SqlConnection conn = null, SqlTransaction transaction = null)
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
ISNULL(Pur.Code,'') SaleCode,
ISNULL(Pur.Id,0) SaleId,
ISNULL(FORMAT(Pur.GrandTotal, 'N2'), '0.00') AS SaleAmount,
ISNULL(D.Id, 0) AS Id,
ISNULL(D.Id, 0) AS SaleDetailId,
ISNULL(D.SaleId, 0) AS SaleId,
ISNULL(D.Line, 0) AS Line,
ISNULL(D.ProductId, 0) AS ProductId,
ISNULL(FORMAT(D.UnitRate, 'N2'), 0.00) AS UnitRate,
ISNULL(FORMAT(D.SD, 'N2'), '0.00') AS SD,
ISNULL(FORMAT(D.SDAmount, 'N2'), '0.00') AS SDAmount,
ISNULL(FORMAT(D.VATRate, 'N2'), '0.00') AS VATRate,
ISNULL(FORMAT(D.VATAmount, 'N2'), '0.00') AS VATAmount,

ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName,
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName


FROM 
SaleDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN Sales Pur ON D.SaleId = Pur.Id

WHERE 1 = 1";


                string inClause = string.Join(", ", IDs.Select((id, index) => $"@Id{index}"));

                if (IDs.Length > 0)
                {
                    query += $" AND D.SaleId IN ({inClause}) ";
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    if (transaction != null)
                    {
                        adapter.SelectCommand.Transaction = transaction;
                    }

                    for (int i = 0; i < IDs.Length; i++)
                    {
                        adapter.SelectCommand.Parameters.AddWithValue($"@Id{i}", IDs[i]);
                    }

                    adapter.Fill(dataTable);
                }

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


        public async Task<ResultVM> FromSaleGridData(GridOptions options, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<SaleVM>();

                // Define your SQL query string
                string sqlQuery = @"
    -- Count query
    SELECT COUNT(DISTINCT H.Id) AS totalcount
        FROM Sales H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers S ON H.CustomerId = S.Id
        LEFT JOIN 
					(
						SELECT d.PurchaseId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity
						FROM [dbo].[PurchaseDetails] d   
						GROUP BY d.PurchaseId
					) SD ON H.Id = SD.PurchaseId
        WHERE H.IsPost = 1 
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "") + @"

    -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
        ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex,
        

        ISNULL(H.Id, 0) AS Id,
        ISNULL(H.Code, '') AS Code,
        ISNULL(H.BranchId, 0) AS BranchId,
        ISNULL(H.CustomerId, 0) AS CustomerId,
        ISNULL(S.Name, '') AS CustomerName,
        ISNULL(FORMAT(H.InvoiceDateTime, 'yyyy-MM-dd'), '1900-01-01') AS InvoiceDateTime,
        ISNULL(H.Comments, '') AS Comments,
        ISNULL(H.TransactionType, '') AS TransactionType,
        ISNULL(H.IsPost, 0) AS IsPost,
        CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status,
        ISNULL(H.PostedBy, '') AS PostedBy,
        ISNULL(FORMAT(H.PostedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS PostedOn,
        ISNULL(H.PeriodId, '') AS PeriodId,
        ISNULL(H.CreatedBy, '') AS CreatedBy,
        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS CreatedOn,
        ISNULL(H.LastModifiedBy, '') AS LastModifiedBy,
        ISNULL(FORMAT(H.LastModifiedOn, 'yyyy-MM-dd HH:mm:ss'), '1900-01-01 00:00:00') AS LastModifiedOn,
        ISNULL(H.CreatedFrom, '') AS CreatedFrom,
        ISNULL(H.LastUpdateFrom, '') AS LastUpdateFrom,

        ISNULL(Br.Name,'') BranchName,
        ISNULL(S.Name,'') SupplierName

        FROM Sales H 
        LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
        LEFT OUTER JOIN Customers S ON H.CustomerId = S.Id
        LEFT JOIN 
					(
						SELECT d.PurchaseId, SUM(ISNULL(d.Quantity,0)) AS TotalQuantity
						FROM [dbo].[PurchaseDetails] d   
						GROUP BY d.PurchaseId
					) SD ON H.Id = SD.PurchaseId
        WHERE H.IsPost = 1 


    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleVM>.FilterCondition(options.filter) + ")" : "") + @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";

                data = KendoGrid<SaleVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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
