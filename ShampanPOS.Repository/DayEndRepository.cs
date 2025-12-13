using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShampanPOS.Repository
{
    public class DayEndRepository : CommonRepository
    {
        //Insert Method
        public async Task<ResultVM> Insert(DayEndHeadersVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
                INSERT INTO DayEndHeaders 
                ([Date], BranchId, IsLocked)
                VALUES 
                (@Date,@BranchId,@IsLocked);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Date", vm.Date);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);
                    cmd.Parameters.AddWithValue("@IsLocked", vm.IsLocked);

                    object newId = cmd.ExecuteScalar();
                    vm.Id = Convert.ToInt32(newId);

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
        public async Task<ResultVM> SaleProcessDataList(DayEndHeadersVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                                 WITH Sales AS (
    SELECT 
        CONVERT(DATE, s.DeliveryDate)DeliveryDate,
        sd.BranchId,
        sd.ProductId,
        SUM(sd.Quantity) AS SalesQuantity
    FROM SaleDeleveryDetails sd
    LEFT OUTER JOIN SaleDeleveries s 
        ON sd.SaleDeliveryId = s.Id
    WHERE sd.IsPost = 1 
      AND s.DeliveryDate >= @Date 
      AND s.DeliveryDate < DATEADD(DAY, 1, @Date)	
      AND sd.BranchId = @BranchId
    GROUP BY sd.BranchId,CONVERT(DATE, s.DeliveryDate), sd.ProductId
),
Returns AS (
    SELECT 
        CONVERT(DATE, s.DeliveryDate)DeliveryDate,
        sd.BranchId,
        sd.ProductId,
        SUM(sd.Quantity) AS ReturnQuantity
    FROM SaleDeleveryReturnDetails sd
    LEFT OUTER JOIN SaleDeleveryReturns s 
        ON sd.SaleDeliveryReturnId = s.Id
    WHERE s.DeliveryDate >= @Date 
      AND s.DeliveryDate < DATEADD(DAY, 1, @Date)	
      AND sd.BranchId = @BranchId
    GROUP BY sd.BranchId,CONVERT(DATE, s.DeliveryDate), sd.ProductId
)
SELECT 
    COALESCE(s.DeliveryDate, r.DeliveryDate) AS DeliveryDate,
    COALESCE(s.BranchId, r.BranchId) AS BranchId,
    COALESCE(s.ProductId, r.ProductId) AS ProductId,
    COALESCE(s.SalesQuantity, 0) - COALESCE(r.ReturnQuantity, 0) AS Quantity,
    'Sales' AS Type
FROM Sales s
FULL OUTER JOIN Returns r 
    ON s.ProductId = r.ProductId 
   AND s.BranchId = r.BranchId
WHERE COALESCE(s.SalesQuantity, 0) - COALESCE(r.ReturnQuantity, 0) <> 0;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.CommandType = CommandType.Text;

                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Date", vm.Date);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new DayEndHeadersVM
                {         
                    Date = vm.Date,
                    BranchId = vm.BranchId,
                    ProductId = row.Field<int>("ProductId"),
                    Quantity = row.Field<decimal>("Quantity"),
                    Type = "Sales"
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
                    await conn.CloseAsync();
                }
            }
        }

        public async Task<ResultVM> InsertDetails(DayEndDetailsVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                INSERT INTO DayEndDetails
                (DayEndHeaderId, [Date],BranchId,ProductId, Quantity,[Type])
                VALUES 
                (@DayEndHeaderId, @Date, @BranchId,@ProductId, @Quantity, @Type);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@DayEndHeaderId", details.DayEndHeaderId);
                    cmd.Parameters.AddWithValue("@Date", details.Date ?? "");
                    cmd.Parameters.AddWithValue("@BranchId", details.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", details.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", details.Quantity);
                    cmd.Parameters.AddWithValue("@UnitRate", details.Type ?? "");                   


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

        public async Task<ResultVM> PurchaseProcessDataList(DayEndHeadersVM vm, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
                                WITH Sales AS (
                                SELECT 
                                    s.PurchaseDate AS Date,
                                    sd.BranchId,
                                    sd.ProductId,
                                    SUM(sd.Quantity) AS PurchaseQuantity
                                FROM PurchaseDetails sd
                                LEFT OUTER JOIN Purchases s 
                                    ON sd.PurchaseId = s.Id
                                WHERE sd.IsPost = 1 
                                  AND s.PurchaseDate >= @Date 
                                  AND s.PurchaseDate < DATEADD(DAY, 1, @Date)                                  
                                  AND sd.BranchId = @BranchId
                                GROUP BY sd.BranchId, sd.ProductId,s.PurchaseDate
                            ),
                            Returns AS (
                                SELECT 
                                    s.PurchaseReturnDate AS Date,
                                    sd.BranchId,
                                    sd.ProductId,
                                    SUM(sd.Quantity) AS ReturnQuantity
                                FROM PurchaseReturnDetails sd
                                LEFT OUTER JOIN PurchaseReturns s 
                                    ON sd.PurchaseReturnId = s.Id
                                WHERE s.PurchaseReturnDate >= @Date 
                                  AND s.PurchaseReturnDate < DATEADD(DAY, 1, @Date)  
                                  AND sd.BranchId = @BranchId
                                GROUP BY sd.BranchId, sd.ProductId,s.PurchaseReturnDate
                            )
                            SELECT 
                                COALESCE(s.Date, r.Date) AS Date,
                                COALESCE(s.BranchId, r.BranchId) AS BranchId,
                                COALESCE(s.ProductId, r.ProductId) AS ProductId,
                                COALESCE(s.PurchaseQuantity, 0) - COALESCE(r.ReturnQuantity, 0) AS Quantity,
                                'Purchase' AS Type
                            FROM Sales s
                            FULL OUTER JOIN Returns r 
                                ON s.ProductId = r.ProductId 
                               AND s.BranchId = r.BranchId
                                AND s.Date = r.Date
                            WHERE COALESCE(s.PurchaseQuantity, 0) - COALESCE(r.ReturnQuantity, 0) <> 0;";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.CommandType = CommandType.Text;

                    // Add parameters to prevent SQL injection
                    cmd.Parameters.AddWithValue("@Date", vm.Date);
                    cmd.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }

                var modelList = dataTable.AsEnumerable().Select(row => new DayEndHeadersVM
                {
                    Date = vm.Date,
                    BranchId = vm.BranchId,
                    ProductId = row.Field<int>("ProductId"),
                    Quantity = row.Field<decimal>("Quantity"),
                    Type = "Purchase"
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
                    await conn.CloseAsync();
                }
            }
        }

        public async Task<ResultVM> InsertDayEndDetails(DayEndHeadersVM processedData, SqlConnection conn, SqlTransaction transaction)
        {
            bool isNewConnection = false;
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
                INSERT INTO DayEndDetails
                (DayEndHeaderId, [Date],BranchId,ProductId, Quantity,[Type])
                VALUES 
                (@DayEndHeaderId, @Date, @BranchId,@ProductId, @Quantity, @Type);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@DayEndHeaderId", processedData.Id);
                    cmd.Parameters.AddWithValue("@Date", processedData.Date ?? "");
                    cmd.Parameters.AddWithValue("@BranchId", processedData.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", processedData.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", processedData.Quantity);
                    cmd.Parameters.AddWithValue("@Type", processedData.Type ?? "");


                    object newId = cmd.ExecuteScalar();
                    processedData.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Details Data inserted successfully.";
                    result.Id = newId.ToString();
                    result.DataVM = processedData;
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

        public ResultVM DayEndList(DayEndHeadersVM vm)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT * FROM DayEndHeaders WHERE Date = @Date";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", vm.Date);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            List<DayEndHeadersVM> dayEndList = new List<DayEndHeadersVM>();

                            while (reader.Read())
                            {
                                dayEndList.Add(new DayEndHeadersVM
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")).ToString(),
                                    // Add other necessary properties here
                                });
                            }

                            return new ResultVM
                            {
                                Status = "Success",
                                Message = "Data retrieved successfully.",
                                DataVM = dayEndList
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Error fetching data.",
                    ExMessage = ex.Message
                };
            }
        }

        public async Task<ResultVM> UpdateDayEndDetails(DayEndHeadersVM saleData, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                string query = @"
        UPDATE DayEndDetails
        SET 
            [Date] = @Date,
            BranchId = @BranchId,
            ProductId = @ProductId,
            Quantity = @Quantity,
            [Type] = @Type
        WHERE 
            DayEndHeaderId = @DayEndHeaderId 
            AND ProductId = @ProductId
            AND Type = @Type
;";  // Ensures the correct record is updated
    
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@DayEndHeaderId", saleData.Id);
                    cmd.Parameters.AddWithValue("@Date", saleData.Date ?? "");
                    cmd.Parameters.AddWithValue("@BranchId", saleData.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductId", saleData.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", saleData.Quantity);
                    cmd.Parameters.AddWithValue("@Type", saleData.Type ?? "");

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        result.Status = "Success";
                        result.Message = "Details updated successfully.";
                        result.Id = saleData.Id.ToString();
                        result.DataVM = saleData;
                    }                    
                }
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error updating DayEndDetails.";
            }

            return result;
        }

        public DataSet SalesDataList(DayEndHeadersVM vm)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.GetConnectionString()))
                {
                    conn.Open();
                    string query = @"
                WITH Delivery AS (
                    SELECT 
                        sdd.ProductId, 
                        SUM(sdd.Quantity) AS TotalDelivery,
                        sdd.SaleDeliveryId,
                        sd.Code,
                        sd.BranchId,
                        sdd.UnitRate,
                        sdd.SD,        
                        SUM((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100) AS SDAmount, 
                        sdd.VATRate,        
                        SUM(((sdd.Quantity * sdd.UnitRate) + ((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100)) * sdd.VATRate / 100) AS VATAmount,         
                        SUM((sdd.Quantity * sdd.UnitRate) + ((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100) +
                            (((sdd.Quantity * sdd.UnitRate) + ((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100)) * sdd.VATRate / 100)) AS LineTotal,
                        sdd.UOMId,
                        sdd.UOMFromId,
                        sdd.UOMConversion,
                        sdd.Line,
                        sdd.CustomerId,
                        sd.SalePersonId,
                        sd.RouteId,
                        sd.DeliveryAddress,
                        sd.VehicleNo,
                        sd.VehicleType,
                        sd.InvoiceDateTime,
                        sd.DeliveryDate,		
                        SUM(sdd.Quantity * sdd.UnitRate) AS SubTotal,        
                        SUM((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100) AS GrandTotalSDAmount,        
                        SUM(((sdd.Quantity * sdd.UnitRate) + ((sdd.Quantity * sdd.UnitRate * sdd.SD) / 100)) * sdd.VATRate / 100) AS GrandTotalVATAmount, 
                        sd.Comments,
                        'Sales' AS TransactionType,
                        '' AS FiscalYear,
                        '' AS PeriodId,
                        sd.CurrencyId,
                        sd.CurrencyRateFromBDT,
                        'Others' VatType
                    FROM SaleDeleveryDetails sdd
                    LEFT OUTER JOIN SaleDeleveries sd ON sdd.SaleDeliveryId = sd.Id
                    WHERE sd.DeliveryDate >= @Date 
                      AND sd.DeliveryDate < DATEADD(DAY, 1, @Date)
                    GROUP BY 
                        sdd.ProductId,
                        sdd.SaleDeliveryId,
                        sd.Code, 
                        sd.BranchId, 
                        sdd.UnitRate, 
                        sdd.SD, 
                        sdd.VATRate, 
                        sdd.UOMId, 
                        sdd.UOMFromId, 
                        sdd.UOMConversion, 
                        sdd.Line,
                        sdd.CustomerId, 
                        sd.SalePersonId, 
                        sd.RouteId, 
                        sd.DeliveryAddress, 
                        sd.VehicleNo, 
                        sd.VehicleType, 
                        sd.InvoiceDateTime, 
                        sd.DeliveryDate, 
                        sd.Comments, 
                        sd.CurrencyId, 
                        sd.CurrencyRateFromBDT
                ),
                DeliveryReturn AS (
                    SELECT 
                        d.ProductId, 
                        SUM(d.Quantity) AS TotalReturn
                    FROM SaleDeleveryReturnDetails d
                    LEFT OUTER JOIN SaleDeleveries sd ON d.SaleDeliveryId = sd.Id
                    WHERE d.IsPost = 1 
                      AND sd.DeliveryDate >= @Date 
                      AND sd.DeliveryDate < DATEADD(DAY, 1, @Date)
                    GROUP BY d.ProductId
                )
                SELECT 
                    d.ProductId, 
                    d.SaleDeliveryId,
                    COALESCE(d.TotalDelivery, 0) - COALESCE(dr.TotalReturn, 0) AS Quantity,
                    d.Code,
                    d.BranchId,
                    d.UnitRate,
                    d.SD,
                    d.SDAmount,
                    d.VATRate,
                    d.VATAmount,
                    d.LineTotal,
                    d.UOMId,
                    d.UOMFromId,
                    d.UOMConversion,
                    d.Line,
                    d.CustomerId,
                    d.SalePersonId,
                    d.RouteId,
                    d.DeliveryAddress,
                    d.VehicleNo,
                    d.VehicleType,
                    d.InvoiceDateTime,
                    d.DeliveryDate,
                    d.SubTotal,
                    d.GrandTotalSDAmount,
                    d.GrandTotalVATAmount,                                    
                    SUM(d.SubTotal) OVER(PARTITION BY d.SaleDeliveryId) AS GrandTotalAmount,
                    d.Comments,
                    d.TransactionType,
                    d.FiscalYear,
                    d.PeriodId,
                    d.CurrencyId,
                    d.CurrencyRateFromBDT,
                    d.VatType
                FROM Delivery d
                LEFT JOIN DeliveryReturn dr ON d.ProductId = dr.ProductId;
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", vm.Date);

                        DataTable dtTableResult = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtTableResult);
                        }

                        // Create a DataView from the result DataTable.
                        var dataView = new DataView(dtTableResult);

                        // Extract master data table. Adjust the column names as necessary.
                        DataTable dtSaleMaster = dataView.ToTable(
                            true,
                            "Code", "BranchId", "CustomerId", "SalePersonId", "RouteId",
                            "DeliveryAddress", "VehicleNo", "VehicleType", "InvoiceDateTime", "DeliveryDate",
                            "Comments", "TransactionType", "FiscalYear", "PeriodId", "CurrencyId", "CurrencyRateFromBDT", "GrandTotalAmount"
                        );

                        // Extract detail data table. Adjust the column names as necessary.
                        DataTable dtSaleDetail = dataView.ToTable(
                            false,
                            "SaleDeliveryId", "BranchId", "Line", "ProductId", "Quantity", "UnitRate", "SubTotal",
                            "SD", "SDAmount", "VATRate", "VATAmount", "LineTotal", "UOMId", "UOMFromId", "UOMConversion", "Comments", "VatType", "TransactionType", "Code"
                        );

                        // Add the master and detail tables to the DataSet.
                        ds.Tables.Add(dtSaleMaster);
                        ds.Tables.Add(dtSaleDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception as needed. For this example, we'll rethrow it.
                throw new Exception("Error fetching data: " + ex.Message, ex);
            }

            return ds;
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
                                0 AS SaleId,
                                ISNULL(D.SaleDeliveryId, 0) AS SaleDeliveryId,
                                0 AS SaleDeliveryDetailId,
	                            ISNULL(D.SaleOrderId, 0) AS SaleOrderId,
                                ISNULL(D.SaleOrderDetailId, 0) AS SaleOrderDetailId,    
                                ISNULL(D.BranchId, 0) AS BranchId,
                                ISNULL(D.Line, 0) AS Line,
                                ISNULL(D.ProductId, 0) AS ProductId,
                                ISNULL(D.Quantity, 0.00) AS Quantity,
                                CAST(ISNULL(D.UnitRate, 0.00) AS DECIMAL(18,2)) AS UnitRate,
                                CAST(ISNULL(D.SubTotal, 0.00) AS DECIMAL(18,2)) AS SubTotal,
                                CAST(ISNULL(D.SD, 0.00) AS DECIMAL(18,2)) AS SD,
                                CAST(ISNULL(D.SDAmount, 0.00) AS DECIMAL(18,2)) AS SDAmount,
                                CAST(ISNULL(D.VATRate, 0.00) AS DECIMAL(18,2)) AS VATRate,
                                CAST(ISNULL(D.VATAmount, 0.00) AS DECIMAL(18,2)) AS VATAmount,
                                CAST(ISNULL(D.LineTotal, 0.00) AS DECIMAL(18,2)) AS LineTotal,
                                ISNULL(D.UOMId, 0) AS UOMId,
                                ISNULL(D.UOMFromId, 0) AS UOMFromId,
                                CAST(ISNULL(D.UOMConversion, 0.00) AS DECIMAL(18,2)) AS UOMConversion,
	                            ISNULL(D.Comments, '') AS Comments,
	                            'VAT' AS VATType,
	                            'Others' AS TransactionType,
                                ISNULL(D.IsPost, 0) AS IsPost
                            FROM 
                                SaleDeleveryDetails D
                            WHERE 1 = 1 ";

                // **Check for valid conditions**
                if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        query += $" AND {conditionalFields[i]} = @param{i} ";
                    }
                }

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // **Add condition parameters**
                    if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
                    {
                        for (int i = 0; i < conditionalFields.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@param{i}", conditionalValue[i]);
                        }
                    }

                    // **Check for additional filters**
                    if (vm != null && !string.IsNullOrEmpty(vm.Id))
                    {
                        query += " AND D.Id = @Id ";
                        cmd.Parameters.AddWithValue("@Id", vm.Id);
                    }

                    SqlDataAdapter objComm = new SqlDataAdapter(cmd);
                    objComm.Fill(dataTable);
                }

                result.Status = "Success";
                result.Message = "Details Data retrieved successfully.";
                result.DataVM = dataTable;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = "Error fetching details data.";
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }

            return result;
        }

    }
}
