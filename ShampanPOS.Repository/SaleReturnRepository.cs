using Newtonsoft.Json;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{
    public class SaleReturnRepository : CommonRepository
    {
        //Insert Method
        public async Task<ResultVM> Insert(SaleReturnVM saleReturn, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO SaleReturns 
                (
                    Id,Code, BranchId, CustomerId, SalePersonId, RouteId, DeliveryAddress, VehicleNo, 
                    VehicleType, InvoiceDateTime, DeliveryDate, GrandTotalAmount, GrandTotalSDAmount, 
                    GrandTotalVATAmount, Comments, IsPrint, TransactionType, FiscalYear, 
                    PeriodId, CurrencyId, CurrencyRateFromBDT, IsPost, CreatedBy, CreatedOn,CreatedFrom
                )
                VALUES 
                (
                    @Id,@Code, @BranchId, @CustomerId, @SalePersonId, @RouteId, @DeliveryAddress, @VehicleNo, 
                    @VehicleType, @InvoiceDateTime, @DeliveryDate, @GrandTotalAmount, @GrandTotalSDAmount, 
                    @GrandTotalVATAmount, @Comments, @IsPrint, @TransactionType, @FiscalYear, 
                    @PeriodId, @CurrencyId, @CurrencyRateFromBDT, @IsPost,  @CreatedBy,  GETDATE(),@CreatedFrom
                );
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Adding parameters with value checks
                    cmd.Parameters.AddWithValue("@Id", saleReturn.Id);
                    cmd.Parameters.AddWithValue("@Code", saleReturn.Code);
                    cmd.Parameters.AddWithValue("@BranchId", saleReturn.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", saleReturn.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleReturn.SalePersonId);
                    cmd.Parameters.AddWithValue("@RouteId", saleReturn.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleReturn.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleNo", saleReturn.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", saleReturn.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleReturn.InvoiceDateTime + " " + DateTime.Now.ToString("HH:mm"));
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleReturn.DeliveryDate + " " + DateTime.Now.ToString("HH:mm"));

                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleReturn.GrandTotalAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleReturn.GrandTotalSDAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleReturn.GrandTotalVATAmount);
                    cmd.Parameters.AddWithValue("@Comments", saleReturn.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPrint", false); // Assuming IsPrint is a bit column

                    cmd.Parameters.AddWithValue("@TransactionType", saleReturn.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost", false);

                    cmd.Parameters.AddWithValue("@FiscalYear", saleReturn.FiscalYear ?? (object)DBNull.Value); // Optional field
                    cmd.Parameters.AddWithValue("@PeriodId", saleReturn.PeriodId ?? (object)DBNull.Value); // Optional field
                    cmd.Parameters.AddWithValue("@CurrencyId", saleReturn.CurrencyId);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleReturn.CurrencyRateFromBDT);
                    cmd.Parameters.AddWithValue("@CreatedBy", saleReturn.CreatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedFrom", saleReturn.CreatedFrom ?? (object)DBNull.Value);


                    // Execute the query and get the new ID (primary key)
                    object newId = cmd.ExecuteScalar();


                    if (isNewConnection)
                    {
                        transaction.Commit();
                    }

                    return result;
                }
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
        public ResultVM InsertDetails(SaleReturnDetailVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        INSERT INTO SaleReturnDetails 
        (
            SaleReturnId, SaleId, SaleDetailId, BranchId, Line, ProductId, Quantity, UnitRate, 
            SubTotal, SD, SDAmount, VATRate, VATAmount, LineTotal, UOMId, UOMFromId, 
            UOMConversion, TransactionType, IsPost, ReasonOfReturn, Comments
        )
        VALUES 
        (
            @SaleReturnId, @SaleId, @SaleDetailId, @BranchId, @Line, @ProductId, @Quantity, @UnitRate, 
            @SubTotal, @SD, @SDAmount, @VATRate, @VATAmount, @LineTotal, @UOMId, @UOMFromId, 
            @UOMConversion, @TransactionType, @IsPost, @ReasonOfReturn, @Comments
        );
        SELECT SCOPE_IDENTITY();";


                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleReturnId", details.SaleReturnId);
                    cmd.Parameters.AddWithValue("@SaleId", details.SaleId);
                    cmd.Parameters.AddWithValue("@SaleDetailId", details.SaleDetailId);
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId);
                    cmd.Parameters.AddWithValue("@Line", details.Line ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.UnitRate);
                    cmd.Parameters.AddWithValue("@SubTotal", details.SubTotal);
                    cmd.Parameters.AddWithValue("@SD", details.SD);
                    cmd.Parameters.AddWithValue("@SDAmount", details.SDAmount);
                    cmd.Parameters.AddWithValue("@VATRate", details.VATRate);
                    cmd.Parameters.AddWithValue("@VATAmount", details.VATAmount);
                    cmd.Parameters.AddWithValue("@LineTotal", details.LineTotal);
                    cmd.Parameters.AddWithValue("@UOMId", details.UOMId);
                    cmd.Parameters.AddWithValue("@UOMFromId", details.UOMFromId);
                    cmd.Parameters.AddWithValue("@UOMConversion", details.UOMConversion);
                    cmd.Parameters.AddWithValue("@TransactionType", details.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPost",  false);
                    cmd.Parameters.AddWithValue("@ReasonOfReturn", details.ReasonOfReturn ?? (object)DBNull.Value);
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
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        // Update Method
        public async Task<ResultVM> Update(SaleReturnVM saleReturn, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = saleReturn.Id.ToString(), DataVM = saleReturn };

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
UPDATE SaleReturns 
SET 
     BranchId=@BranchId, CustomerId=@CustomerId, SalePersonId=@SalePersonId, 
    RouteId=@RouteId, DeliveryAddress=@DeliveryAddress, VehicleNo=@VehicleNo, 
    VehicleType=@VehicleType, InvoiceDateTime=@InvoiceDateTime, DeliveryDate=@DeliveryDate, 
    GrandTotalAmount=@GrandTotalAmount, GrandTotalSDAmount=@GrandTotalSDAmount, 
    GrandTotalVATAmount=@GrandTotalVATAmount, Comments=@Comments, IsPrint=@IsPrint, 
    TransactionType=@TransactionType, FiscalYear=@FiscalYear, 
    PeriodId=@PeriodId, CurrencyId=@CurrencyId, CurrencyRateFromBDT=@CurrencyRateFromBDT, 
    IsPost=@IsPost,  LastModifiedBy=@LastModifiedBy, LastUpdateFrom=@LastUpdateFrom,
    LastModifiedOn=GETDATE()
WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", saleReturn.Id);
                    cmd.Parameters.AddWithValue("@BranchId", saleReturn.BranchId);
                    cmd.Parameters.AddWithValue("@CustomerId", saleReturn.CustomerId);
                    cmd.Parameters.AddWithValue("@SalePersonId", saleReturn.SalePersonId);
                    cmd.Parameters.AddWithValue("@RouteId", saleReturn.RouteId);
                    cmd.Parameters.AddWithValue("@DeliveryAddress", saleReturn.DeliveryAddress ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleNo", saleReturn.VehicleNo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VehicleType", saleReturn.VehicleType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", saleReturn.InvoiceDateTime);
                    cmd.Parameters.AddWithValue("@DeliveryDate", saleReturn.DeliveryDate);
                    cmd.Parameters.AddWithValue("@GrandTotalAmount", saleReturn.GrandTotalAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalSDAmount", saleReturn.GrandTotalSDAmount);
                    cmd.Parameters.AddWithValue("@GrandTotalVATAmount", saleReturn.GrandTotalVATAmount);
                    cmd.Parameters.AddWithValue("@Comments", saleReturn.Comments ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsPrint", false);

                    cmd.Parameters.AddWithValue("@TransactionType", saleReturn.TransactionType ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FiscalYear", saleReturn.FiscalYear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PeriodId", saleReturn.PeriodId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CurrencyId", saleReturn.CurrencyId);
                    cmd.Parameters.AddWithValue("@CurrencyRateFromBDT", saleReturn.CurrencyRateFromBDT);
                    cmd.Parameters.AddWithValue("@IsPost",false);
                    cmd.Parameters.AddWithValue("@LastModifiedBy", saleReturn.LastModifiedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastUpdateFrom", saleReturn.LastUpdateFrom ?? (object)DBNull.Value);

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
                foreach (var details in saleReturn.saleReturnDetailList)
                {
                    details.SaleId = saleReturn.Id;
                    details.SaleReturnId = saleReturn.Id;
                    details.SaleDetailId = saleReturn.Id;
                    details.BranchId = saleReturn.BranchId;
                    details.Line = LineNo;
                    details.IsPost = saleReturn.IsPost;
                    details.TransactionType = saleReturn.TransactionType;
                    details.ReasonOfReturn = "SaleReturn";//Need to be change 

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
    ISNULL(M.PostBy, 0) AS PostedBy,
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
    SaleReturns M
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

                var model = new List<SaleReturnVM>();

                foreach (DataRow row in dataTable.Rows)
                {
                    model.Add(new SaleReturnVM
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Code = row["Code"].ToString(),
                        BranchId = Convert.ToInt32(row["BranchId"]),
                        CustomerId = Convert.ToInt32(row["CustomerId"]),
                        SalePersonId = Convert.ToInt32(row["SalePersonId"]),
                        RouteId = Convert.ToInt32(row["RouteId"]),
                        DeliveryAddress = row["DeliveryAddress"].ToString(),
                        VehicleNo = row["VehicleNo"].ToString(),
                        VehicleType = row["VehicleType"].ToString(),
                        InvoiceDateTime = row["InvoiceDateTime"] != DBNull.Value ? Convert.ToDateTime(row["InvoiceDateTime"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        DeliveryDate = row["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(row["DeliveryDate"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalAmount"]) : 0,
                        GrandTotalSDAmount = row["GrandTotalSDAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalSDAmount"]) : 0,
                        GrandTotalVATAmount = row["GrandTotalVATAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalVATAmount"]) : 0,
                        Comments = row["Comments"].ToString(),
                        IsPrint = row["IsPrint"] != DBNull.Value ? Convert.ToBoolean(row["IsPrint"]) : false,
                        PrintBy = row["PrintBy"].ToString(),
                        PrintOn = row["PrintOn"] != DBNull.Value ? Convert.ToDateTime(row["PrintOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        TransactionType = row["TransactionType"].ToString(),
                        IsPost = row["IsPost"] != DBNull.Value ? Convert.ToBoolean(row["IsPost"]) : false,
                        PostBy = row["PostedBy"].ToString(),
                        PostedOn = row["PostedOn"] != DBNull.Value ? Convert.ToDateTime(row["PostedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        FiscalYear = row["FiscalYear"].ToString(),
                        PeriodId = row["PeriodId"].ToString(),
                        CurrencyId = row["CurrencyId"] != DBNull.Value ? Convert.ToInt32(row["CurrencyId"]) : 0,
                        CurrencyRateFromBDT = row["CurrencyRateFromBDT"] != DBNull.Value ? Convert.ToDecimal(row["CurrencyRateFromBDT"]) : 0,
                        CreatedBy = row["CreatedBy"].ToString(),
                        CreatedOn = row["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(row["CreatedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        LastModifiedBy = row["LastModifiedBy"].ToString(),
                        LastModifiedOn = row["LastModifiedOn"] != DBNull.Value ? Convert.ToDateTime(row["LastModifiedOn"]).ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01 00:00:00",
                        CreatedFrom = row["CreatedFrom"].ToString(),
                        LastUpdateFrom = row["LastUpdateFrom"].ToString()
                    });
                }



                var detailsDataList = DetailsList(new[] { "D.saleId" }, conditionalValue, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleReturnDetailVM>>(json);

                    model.FirstOrDefault().saleReturnDetailList = details;
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
ISNULL(D.saleId, 0) AS saleId,
ISNULL(D.SaleDetailId, 0) AS saleId,
ISNULL(D.BranchId, 0) AS BranchId,
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
ISNULL(D.UOMId, 0) AS UOMId,
ISNULL(D.UOMFromId, 0) AS UOMFromId,
ISNULL(FORMAT(D.UOMConversion, 'N2'), '0.00') AS UOMConversion,
ISNULL(D.IsPost, 0) AS IsPost,
ISNULL(D.Comments, '') AS Comments,

ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(P.UOMId,0) UOMId,
ISNULL(UOM.Name,'') UOMName,
ISNULL(P.UOMId,0) UOMFromId,
ISNULL(UOM.Name,'') UOMFromName

FROM 
SaleReturnDetails D
LEFT OUTER JOIN Products P ON D.ProductId = P.Id
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
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

                string query = $" UPDATE SaleReturns SET IsPost = 1, PostBy = @PostedBy , LastUpdateFrom = @LastUpdateFrom ,PostedOn = GETDATE() WHERE Id IN ({inClause}) ";
                query += $" UPDATE SaleReturnDetails SET IsPost = 1 WHERE saleId IN ({inClause}) ";

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

                var data = new GridEntity<SaleReturnVM>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
		FROM SaleReturns H 
		LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
		LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
		LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
		LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
		WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleReturnVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                
                ,ISNULL(H.Id,0)	Id
                ,ISNULL(H.Code,'') Code
                ,ISNULL(H.VehicleNo,'') VehicleNo
                ,ISNULL(H.VehicleType,'') VehicleType
                ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
                ,ISNULL(H.Comments,'') Comments
                ,ISNULL(H.IsPost, 0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
                ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
                ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
                ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
                ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
                ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate

                ,ISNULL(Br.Name,'') BranchName
                ,ISNULL(cus.Name,'') CustomerName
                ,ISNULL(SP.Name,'') SalePersonName
                ,ISNULL(rut.Name,'') RouteName

                  FROM SaleReturns H 
                LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
                LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
                LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleReturnVM>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleReturnVM>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

                var data = new GridEntity<SaleReturnDetail>();

                // Define your SQL query string
                string sqlQuery = @"
            -- Count query
            SELECT COUNT(DISTINCT H.Id) AS totalcount
		FROM SaleReturns H 
		LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
		LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
		LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
		LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
		LEFT OUTER JOIN SaleReturnDetails D ON H.Id = D.SaleReturnId
        LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
        LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
		WHERE 1 = 1
            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleReturnDetail>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            -- Data query with pagination and sorting
            SELECT * 
            FROM (
                SELECT 
                ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC") + @") AS rowindex
        
                
                ,ISNULL(H.Id,0)	Id
                ,ISNULL(H.Code,'') Code
                ,ISNULL(H.VehicleNo,'') VehicleNo
                ,ISNULL(H.VehicleType,'') VehicleType
                ,ISNULL(H.DeliveryAddress,'') DeliveryAddress
                ,ISNULL(H.Comments,'') Comments
                ,ISNULL(H.IsPost, 0) IsPost
                ,CASE WHEN ISNULL(H.IsPost, 0) = 1 THEN 'Posted' ELSE 'Not-posted' END AS Status
                ,ISNULL(FORMAT(H.GrandTotalAmount, 'N2'), '0.00') AS GrdTotalAmount
                ,ISNULL(FORMAT(H.GrandTotalSDAmount, 'N2'), '0.00') AS GrdTotalSDAmount
                ,ISNULL(FORMAT(H.GrandTotalVATAmount, 'N2'), '0.00') AS GrdTotalVATAmount
                ,ISNULL(FORMAT(H.InvoiceDateTime,'yyyy-MM-dd'),'1900-01-01') InvoiceDateTime
                ,ISNULL(FORMAT(H.DeliveryDate,'yyyy-MM-dd'),'1900-01-01') DeliveryDate
                ,ISNULL(Br.Name,'') BranchName
                ,ISNULL(cus.Name,'') CustomerName
                ,ISNULL(SP.Name,'') SalePersonName
                ,ISNULL(rut.Name,'') RouteName
                 -- Sales Details
                ,ISNULL(D.Id, 0) AS SaleReturnDetailId
                ,ISNULL(D.SaleReturnId, 0) AS SaleReturnId
                ,ISNULL(D.SaleId, 0) AS SaleId
                ,ISNULL(D.SaleDetailId, 0) AS SaleDetailId
                ,ISNULL(D.ProductId, 0) AS ProductId
                ,ISNULL(PD.Name, 0) AS ProductName
                ,ISNULL(UM.Name, 0) AS UOMName
                ,ISNULL(D.BranchId, 0) AS BranchId
                ,ISNULL(D.Quantity, 0) AS Quantity
                ,ISNULL(D.UnitRate, 0) AS UnitRate
                ,ISNULL(D.SubTotal, 0) AS SubTotal
                ,ISNULL(D.SD, 0) AS SD
                ,ISNULL(D.SDAmount, 0) AS SDAmount
                ,ISNULL(D.VATRate, 0) AS VATRate
                ,ISNULL(D.VATAmount, 0) AS VATAmount
                ,ISNULL(D.UOMId, 0) AS UOMId
                ,ISNULL(D.UOMFromId, 0) AS UOMFromId
                ,ISNULL(D.UOMConversion, 0) AS UOMConversion
                ,ISNULL(D.Comments, '') AS DetailComments
                ,ISNULL(D.ReasonOfReturn, '') AS ReasonOfReturn
                ,ISNULL(D.TransactionType, '') AS TransactionType
                ,ISNULL(D.IsPost, 0) AS DetailIsPost

                FROM SaleReturns H 
                LEFT OUTER JOIN BranchProfiles Br ON H.BranchId = Br.Id
                LEFT OUTER JOIN Customers cus ON H.CustomerId = cus.Id
                LEFT OUTER JOIN SalesPersons SP ON H.SalePersonId = SP.Id
                LEFT OUTER JOIN Routes rut ON H.RouteId = rut.Id
                LEFT OUTER JOIN SaleReturnDetails D ON H.Id = D.SaleReturnId
                LEFT OUTER JOIN Products PD ON D.ProductId = PD.Id
                LEFT OUTER JOIN UOMs UM ON D.UOMId = UM.Id
                WHERE 1 = 1

            -- Add the filter condition
            " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<SaleReturnDetail>.FilterCondition(options.filter) + ")" : "");
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                sqlQuery += @"

            ) AS a
            WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        ";

                data = KendoGrid<SaleReturnDetail>.GetTransactionalGridData_CMD(options, sqlQuery, "H.Id", conditionalFields, conditionalValues);

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

        public async Task<ResultVM> InsertDetail(SaleReturnVM details, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                // SQL Insert query
                string query = @"
       INSERT INTO SaleReturnDetails
(
    SaleReturnId,
    SaleId,
    SaleDetailId,
    BranchId,
    Line,
    ProductId,
    Quantity,
    UnitRate,
    SubTotal,
    SD,
    SDAmount,
    VATRate,
    VATAmount,
    LineTotal,
    UOMId,
    UOMFromId,
    UOMconversion,
    TransactionType,
    IsPost,
    ReasonOfReturn,
    Comments
)
VALUES
(
    @SaleReturnId,      
    @SaleId,            
    @SaleDetailId,      
    @BranchId,          
    @Line,              
    @ProductId,         
    @Quantity,          
    @UnitRate,          
    @SubTotal,          
    @SD,                
    @SDAmount,          
    @VATRate,           
    @VATAmount,         
    @LineTotal,         
    @UOMId,             
    @UOMFromId,        
    @UOMconversion,     
    @TransactionType,  
    @IsPost,          
    @ReasonOfReturn,  
    @Comments       
);  SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Loop through the sale details list and insert each detail
                    foreach (var detail in details.saleReturnDetailList)
                    {
                        // Clear parameters to avoid conflicts
                        cmd.Parameters.Clear();

                        // Adding parameters to the SQL command
                        cmd.Parameters.AddWithValue("@SaleReturnId", detail.SaleReturnId);
                        cmd.Parameters.AddWithValue("@SaleId", detail.SaleId);
                        cmd.Parameters.AddWithValue("@SaleDetailId", detail.SaleDetailId);
                        cmd.Parameters.AddWithValue("@BranchId", detail.BranchId);
                        cmd.Parameters.AddWithValue("@Line", detail.Line);
                        cmd.Parameters.AddWithValue("@ProductId", detail.ProductId);
                        cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                        cmd.Parameters.AddWithValue("@UnitRate", detail.UnitRate);
                        cmd.Parameters.AddWithValue("@SubTotal", detail.SubTotal);
                        cmd.Parameters.AddWithValue("@SD", detail.SD);
                        cmd.Parameters.AddWithValue("@SDAmount", detail.SDAmount);
                        cmd.Parameters.AddWithValue("@VATRate", detail.VATRate);
                        cmd.Parameters.AddWithValue("@VATAmount", detail.VATAmount);
                        cmd.Parameters.AddWithValue("@LineTotal", detail.LineTotal);
                        cmd.Parameters.AddWithValue("@UOMId", detail.UOMId);
                        cmd.Parameters.AddWithValue("@UOMFromId", detail.UOMFromId);
                        cmd.Parameters.AddWithValue("@UOMconversion", detail.UOMConversion);
                        cmd.Parameters.AddWithValue("@TransactionType", detail.TransactionType);
                        cmd.Parameters.AddWithValue("@IsPost", detail.IsPost);
                        cmd.Parameters.AddWithValue("@ReasonOfReturn", detail.ReasonOfReturn);
                        cmd.Parameters.AddWithValue("@Comments", detail.Comments);


                        // Execute the query and get the new ID (primary key)
                        object newId = await cmd.ExecuteScalarAsync();
                        detail.Id = Convert.ToInt32(newId);
                    }

                    // Commit the transaction if everything goes well
                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = details.Id.ToString();
                    result.DataVM = details;
                }
            }
            catch (Exception ex)
            {
                // In case of error, rollback the transaction (if needed) and set result messages
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultVM> ProductWiseSaleReturn(string[] conditionalFields, string[] conditionalValue, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
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
                            -- Details Data
                            ISNULL(D.ProductId, 0) AS ProductId,
                            ISNULL(PG.Name, '') AS ProductGroupName,
                            ISNULL(P.Code, '') AS ProductCode,
                            ISNULL(P.Name, '') AS ProductName,
                            ISNULL(P.HSCodeNo, '') AS HSCodeNo,
                            ISNULL(uom.Name, '') AS UOMName,
                            ISNULL(SUM(D.Quantity), 0) AS Quantity

                            FROM SaleReturns M
                            LEFT OUTER JOIN SaleReturnDetails D ON ISNULL(M.Id,0) = D.SaleReturnId
                            LEFT OUTER JOIN Products P ON D.ProductId = P.Id
                            LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
                            LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id
                            LEFT OUTER JOIN UOMConversations uomCon ON D.UOMFromId = uomCon.Id
		                        WHERE 1 = 1 
";

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    query += " AND M.Id = @Id ";
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    query += " AND CAST(M.DeliveryDate AS DATE) BETWEEN @FromDate AND @ToDate ";
                }
                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValue, false);
                query += @" GROUP BY    
                                    D.ProductId,
                                    P.HSCodeNo,
                                    P.Code,
                                    P.Name,
                                    PG.Name,
                                    uom.Name ";
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValue);

                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@Id", vm.Id);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate) && !string.IsNullOrEmpty(vm.ToDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", vm.ToDate);
                }

                objComm.Fill(dataTable);

                var lst = new List<SaleReturnReportVM>();
                int serialNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleReturnReportVM
                    {
                        SL = serialNumber,
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        ProductGroupName = row["ProductGroupName"].ToString(),
                        ProductCode = row["ProductCode"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        HSCodeNo = row["HSCodeNo"].ToString(),
                        UOMName = row["UOMName"].ToString(),
                        Quantity = Convert.ToInt32(row["Quantity"])
                    });
                    serialNumber++;
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
 
FROM SaleReturns M
LEFT OUTER JOIN SaleReturnDetails D ON ISNULL(M.Id,0) = D.SaleReturnId
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


    }

}
