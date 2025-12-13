using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{
    public class MISReportRepository : CommonRepository
    {

        public async Task<ResultVM> ExportSaleAndPayment(SalePaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
    SL,
    c.Name CustomerName,
    Date,
    [Invoice No.],
    [Payment No.],
    [Payment Mode],
    ISNULL(Opening,0) Opening,
    [Receive Amount],
    [Sales Amount],
    SUM(ISNULL(Opening,0)  + [Receive Amount]- [Sales Amount]) 
        OVER (PARTITION BY a.CustomerID ORDER BY a.Date, SL ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS [Closing Balance],
    Remarks
FROM (
    SELECT a.*
    FROM (
        SELECT 'C' SL, 
               InvoiceDateTime Date,
               Code [Invoice No.], 
               '' [Payment No.],
               CustomerID,
               '' [Payment Mode],
               0 Opening,
               0 [Receive Amount],
               GrandTotalAmount [Sales Amount], 
               'Sales' Remarks 
        FROM SaleDeleveries h
        WHERE 1=1
       AND IsPost=1
";
                if(!string.IsNullOrEmpty(vm.BranchId)&& vm.BranchId!="0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.InvoiceDateTime >=@DateFrom ";
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    query += " AND h.InvoiceDateTime <=@DateTo ";
                }

                query += @"
           
     
        
        UNION ALL
        
        SELECT 'B' SL, 
               TransactionDate Date,
               '' [Invoice No.], 
               Code [Payment No.],
               CustomerID,
               ModeOfPayment [Payment Mode],
               0 Opening,
               Amount [Receive Amount],
               0 [Sales Amount], 
               'Payment' Remarks 
        FROM CustomerPaymentCollection h
        WHERE 1=1
      AND IsPost=1";

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.TransactionDate >=@DateFrom ";
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    query += " AND h.TransactionDate <=@DateTo ";
                }

                query += @"
 
         
    ) AS a
    
    UNION ALL
    
    SELECT 'A' SL,
           @Date Date,
           '' [Invoice No.],
           '' [Payment No.], 
           a.CustomerID,
           '' [Payment Mode],
           SUM(a.Opening) Opening,
           0 [Receive Amount],
           0 [Sales Amount], 
           'Opening' Remarks
    FROM (
        SELECT DISTINCT CustomerID, SUM(-1*GrandTotalAmount) Opening 
        FROM SaleDeleveries h
        WHERE 1=1
       AND IsPost=1
";
                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.InvoiceDateTime <@DateFrom ";
                }

                query += @"
      
        GROUP BY h.CustomerID
        
        UNION ALL
        
        SELECT DISTINCT CustomerID, SUM(Amount) Opening 
        FROM CustomerPaymentCollection h
        WHERE 1=1
       AND IsPost=1

";
                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.TransactionDate <@DateFrom ";
                }

                query += @"
               
        GROUP BY h.CustomerID
    ) AS a
    GROUP BY a.CustomerID
) AS a 
LEFT OUTER JOIN Customers c ON a.CustomerID=c.Id
ORDER BY c.Code, a.Date, SL
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // --- Add parameters for every @Variable in your query ---
                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId );
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom);
                    objComm.SelectCommand.Parameters.AddWithValue("@Date", vm.DateFrom );

                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }
                // --------------------------------------------------------

                objComm.Fill(dataTable);

                

                var lst = new List<SalePaymentVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                   
                    lst.Add(new SalePaymentVM
                    {
                        SL = serialNumber.ToString(),
                        CustomerName = row["CustomerName"].ToString(),
                        Date = row["Date"].ToString(),
                        InvoiceNo = row["Invoice No."].ToString(),
                        PaymentNo = row["Payment No."].ToString(),
                        PaymentMode = row["Payment Mode"].ToString(),
                        Opening = row["Opening"].ToString(),
                        ReceiveAmount = Convert.ToInt32(row["Receive Amount"]),
                        SalesAmount = Convert.ToInt32(row["Sales Amount"]),
                        ClosingBalance = Convert.ToInt32(row["Closing Balance"]),
                        Remarks = row["Remarks"].ToString(),
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

        public async Task<ResultVM> ExportSaleAndPaymentSummary(SalePaymentVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
    c.Name CustomerName,
    SUM(ISNULL(Opening,0)) Opening,
    SUM([Receive Amount]) [Receive Amount],
    SUM([Sales Amount]) [Sales Amount],
    SUM(ISNULL(Opening,0)) + SUM([Receive Amount]) - SUM([Sales Amount]) AS [Closing Balance]
FROM (
    ------------------------------- Transaction --------------------------------------------
    SELECT a.*
    FROM (
        ------------------------------- Transaction --------------------------------------------
        SELECT 'C' SL, 
               InvoiceDateTime Date,
               Code [Invoice No.], 
               '' [Payment No.],
               CustomerID,
               '' [Payment Mode],
               0 Opening,
               0 [Receive Amount],
               GrandTotalAmount [Sales Amount], 
               'Sales' Remarks 
        FROM SaleDeleveries h
        WHERE 1=1
            AND IsPost=1
";
                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.InvoiceDateTime >=@DateFrom ";
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    query += " AND h.InvoiceDateTime <=@DateTo ";
                }

                query += @"
           
        UNION ALL
        
        SELECT 'B' SL, 
               TransactionDate Date,
               '' [Invoice No.], 
               Code [Payment No.],
               CustomerID,
               ModeOfPayment [Payment Mode],
               0 Opening,
               Amount [Receive Amount],
               0 [Sales Amount], 
               'Payment' Remarks 
        FROM CustomerPaymentCollection h
        WHERE 1=1 AND IsPost=1
";
                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.TransactionDate >=@DateFrom ";
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    query += " AND h.TransactionDate <=@DateTo ";
                }

                query += @"
    ) AS a
    ------------------------------- Transaction --------------------------------------------
    
    ------------------------------- Opening --------------------------------------------
    UNION ALL
    
    SELECT 'A' SL,
           @Date Date,
           '' [Invoice No.],
           '' [Payment No.], 
           a.CustomerID,
           '' [Payment Mode],
           SUM(a.Opening) Opening,
           0 [Receive Amount],
           0 [Sales Amount], 
           'Opening' Remarks
    FROM (
        SELECT DISTINCT CustomerID, SUM(-1*GrandTotalAmount) Opening 
        FROM SaleDeleveries h
        WHERE 1=1
            AND IsPost=1

";

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.InvoiceDateTime <@DateFrom ";
                }



                query += @"
        GROUP BY h.CustomerID
        
        UNION ALL
        
        SELECT DISTINCT CustomerID, SUM(Amount) Opening 
        FROM CustomerPaymentCollection h
        WHERE 1=1
       AND IsPost=1
";

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    query += " AND BranchId=@BranchId ";
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    query += " AND CustomerId=@CustomerId ";
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    query += " AND h.TransactionDate <@DateFrom ";
                }
             
     

              query += @"
        GROUP BY h.CustomerID
    ) AS a
    GROUP BY a.CustomerID
) AS a 
LEFT OUTER JOIN Customers c ON a.CustomerID=c.Id
GROUP BY c.Name, a.CustomerID
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom);
                    objComm.SelectCommand.Parameters.AddWithValue("@Date", vm.DateFrom);

                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }

                objComm.Fill(dataTable);
                var lst = new List<SalePaymentVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SalePaymentVM
                    {
                        CustomerName = row["CustomerName"].ToString(),
                        Opening = row["Opening"].ToString(),
                        ReceiveAmount = Convert.ToInt32(row["Receive Amount"]),
                        SalesAmount = Convert.ToInt32(row["Sales Amount"]),
                        ClosingBalance = Convert.ToInt32(row["Closing Balance"])
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

        public async Task<ResultVM> SaleDeleveryInformation(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
Select 
 h.Code
,b.Name BranchName
,b.BanglaName BranchBanglaName
,r.Name RouteName
,r.BanglaName RouteBanglaName
,sp.Name  SalesPersonName
,sp.BanglaName  SalesPersonBanglaName
,c.Name CustomerName
,c.BanglaName CustomerBanglaName
,c.Address CustomerAddress
,c.BanglaAddress CustomerBanglaAddress
,h.InvoiceDateTime
,h.DeliveryDate
,h.SpecialDiscountAmount
,h.RegularDiscountAmount
,h.GrandTotalAmount
,p.Name ProductName
,p.BanglaName ProductBanglaName
,p.CtnSize
,p.CtnSizeFactor
,d.ProductId
,d.CtnQuantity
,d.PcsQuantity
,d.Quantity
,d.UnitRate
,d.UOMConversion
,d.SubTotal
From SaleDeleveryDetails d
left outer join SaleDeleveries h on h.Id=d.SaleDeliveryId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1

";

                if (!string.IsNullOrEmpty(vm.BranchId))
                { query += " AND h.BranchId = @BranchId "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }

                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }
                objComm.Fill(dataTable);
                var lst = new List<SaleDeleveryInformationVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SaleDeleveryInformationVM
                    {
                        Code = row["Code"]?.ToString(),

                        BranchName = row["BranchName"]?.ToString(),
                        BranchBanglaName = row["BranchBanglaName"]?.ToString(),

                        RouteName = row["RouteName"]?.ToString(),
                        RouteBanglaName = row["RouteBanglaName"]?.ToString(),

                        SalesPersonName = row["SalesPersonName"]?.ToString(),
                        SalesPersonBanglaName = row["SalesPersonBanglaName"]?.ToString(),

                        CustomerName = row["CustomerName"]?.ToString(),
                        CustomerBanglaName = row["CustomerBanglaName"]?.ToString(),
                        CustomerAddress = row["CustomerAddress"]?.ToString(),
                        CustomerBanglaAddress = row["CustomerBanglaAddress"]?.ToString(),

                        InvoiceDateTime = row["InvoiceDateTime"]?.ToString(),
                        DeliveryDate = row["DeliveryDate"]?.ToString(),

                        SpecialDiscountAmount = row["SpecialDiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["SpecialDiscountAmount"]) : 0,
                        RegularDiscountAmount = row["RegularDiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["RegularDiscountAmount"]) : 0,
                        GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalAmount"]) : 0,

                        ProductName = row["ProductName"]?.ToString(),
                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        CtnSize = row["CtnSize"]?.ToString(),
                        CtnSizeFactor = row["CtnSizeFactor"] != DBNull.Value ? Convert.ToDecimal(row["CtnSizeFactor"]) : 0,

                        ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt32(row["ProductId"]) : 0,

                        CtnQuantity = row["CtnQuantity"] != DBNull.Value ? Convert.ToDecimal(row["CtnQuantity"]) : 0,
                        PcsQuantity = row["PcsQuantity"] != DBNull.Value ? Convert.ToDecimal(row["PcsQuantity"]) : 0,
                        Quantity = row["Quantity"] != DBNull.Value ? Convert.ToDecimal(row["Quantity"]) : 0,
                        UnitRate = row["UnitRate"] != DBNull.Value ? Convert.ToDecimal(row["UnitRate"]) : 0,
                        UOMConversion = row["UOMConversion"] != DBNull.Value ? Convert.ToDecimal(row["UOMConversion"]) : 0,
                        SubTotal = row["SubTotal"] != DBNull.Value ? Convert.ToDecimal(row["SubTotal"]) : 0,
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
        public async Task<ResultVM> ProductLeatestPriceInformation(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
WITH LatestPrice AS (
    SELECT
        psph.ProductId,
        psph.PriceCategory,
        psph.EffectDate,
        psph.SalesPrice,
        ROW_NUMBER() OVER (PARTITION BY psph.ProductId ORDER BY psph.EffectDate DESC) AS rn
    FROM ProductSalePriceBatchHistories psph
	where BranchId = @BranchId
)
SELECT
    lp.PriceCategory,
    p.Name as ProductName,
    lp.EffectDate,
    lp.SalesPrice
FROM LatestPrice lp
LEFT JOIN Products p ON p.Id = lp.ProductId
WHERE lp.rn = 1

";

                //if (!string.IsNullOrEmpty(vm.BranchId))
                //{ query += " AND BranchId = @BranchId "; }

                //if (!string.IsNullOrEmpty(vm.CustomerId))
                //{ query += " AND h.CustomerId = @CustomerId "; }

                //if (!string.IsNullOrEmpty(vm.DateFrom))
                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

                //if (!string.IsNullOrEmpty(vm.DateTo))
                //{ query += " AND h.InvoiceDateTime <= @DateTo "; }

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                //if (!string.IsNullOrEmpty(vm.CustomerId))
                //{
                //    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                //}
                //if (!string.IsNullOrEmpty(vm.DateFrom))
                //{
                //    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                //}

                //if (!string.IsNullOrEmpty(vm.DateTo))
                //{
                //    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                //}
                objComm.Fill(dataTable);
                var lst = new List<ProductBatchHistoryVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new ProductBatchHistoryVM
                    {
                        PriceCategory = row["PriceCategory"]?.ToString(),

                        //BranchName = row["BranchName"]?.ToString(),
                        ProductName = row["ProductName"]?.ToString(),
                        SalesPrice = row["SalesPrice"] != DBNull.Value ? Convert.ToDecimal(row["SalesPrice"]) : 0,
                        EffectDate = row["EffectDate"]?.ToString(),
                        //BranchBanglaName = row["BranchBanglaName"]?.ToString(),

                        //RouteName = row["RouteName"]?.ToString(),
                        //RouteBanglaName = row["RouteBanglaName"]?.ToString(),

                        //SalesPersonName = row["SalesPersonName"]?.ToString(),
                        //SalesPersonBanglaName = row["SalesPersonBanglaName"]?.ToString(),

                        //CustomerName = row["CustomerName"]?.ToString(),
                        //CustomerBanglaName = row["CustomerBanglaName"]?.ToString(),
                        //CustomerAddress = row["CustomerAddress"]?.ToString(),
                        //CustomerBanglaAddress = row["CustomerBanglaAddress"]?.ToString(),

                        //InvoiceDateTime = row["InvoiceDateTime"]?.ToString(),
                        //DeliveryDate = row["DeliveryDate"]?.ToString(),

                        //SpecialDiscountAmount = row["SpecialDiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["SpecialDiscountAmount"]) : 0,
                        //RegularDiscountAmount = row["RegularDiscountAmount"] != DBNull.Value ? Convert.ToDecimal(row["RegularDiscountAmount"]) : 0,
                        //GrandTotalAmount = row["GrandTotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["GrandTotalAmount"]) : 0,

                        //ProductName = row["ProductName"]?.ToString(),
                        //ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        //CtnSize = row["CtnSize"]?.ToString(),
                        //CtnSizeFactor = row["CtnSizeFactor"] != DBNull.Value ? Convert.ToDecimal(row["CtnSizeFactor"]) : 0,

                        //ProductId = row["ProductId"] != DBNull.Value ? Convert.ToInt32(row["ProductId"]) : 0,

                        //CtnQuantity = row["CtnQuantity"] != DBNull.Value ? Convert.ToDecimal(row["CtnQuantity"]) : 0,
                        //PcsQuantity = row["PcsQuantity"] != DBNull.Value ? Convert.ToDecimal(row["PcsQuantity"]) : 0,
                        //Quantity = row["Quantity"] != DBNull.Value ? Convert.ToDecimal(row["Quantity"]) : 0,
                        //UnitRate = row["UnitRate"] != DBNull.Value ? Convert.ToDecimal(row["UnitRate"]) : 0,
                        //UOMConversion = row["UOMConversion"] != DBNull.Value ? Convert.ToDecimal(row["UOMConversion"]) : 0,
                        //SubTotal = row["SubTotal"] != DBNull.Value ? Convert.ToDecimal(row["SubTotal"]) : 0,
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

        public async Task<ResultVM> MonthlySalesAndAmountReportProductWise(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

 Select 
 ProductName
,ProductBanglaName

,sum(SaleCtnQuantity)SaleCtnQuantity
,sum(RetrunCtnQuantity)RetrunCtnQuantity
,sum(SaleCtnQuantity-RetrunCtnQuantity)ActualCtnQuantity

,sum(SalePcsQuantity)SalePcsQuantity
,sum(RetrunPcsQuantity)RetrunPcsQuantity
,sum(SalePcsQuantity-RetrunPcsQuantity)ActualPcsQuantity

,sum(SaleQuantity)TotalSaleQuantity
,sum(RetrunQuantity)TotalRetrunQuantity
,sum(SaleQuantity-RetrunQuantity)TotalQuantity

,sum(SaleSubTotal-RetrunSubTotal) NetAmount

from (

Select 
p.Name ProductName
,p.BanglaName ProductBanglaName

,d.CtnQuantity SaleCtnQuantity
,d.PcsQuantity SalePcsQuantity
,d.Quantity    SaleQuantity
,d.SubTotal    SaleSubTotal

,0    RetrunCtnQuantity
,0    RetrunPcsQuantity
,0    RetrunQuantity
,0    RetrunUnitRate
,0    RetrunSubTotal

From SaleDeleveryDetails d
left outer join SaleDeleveries h on h.Id=d.SaleDeliveryId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId

where 1=1


";


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.ProductId))
                { query += " AND d.ProductId = @ProductId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }



                query += @" 

Union All 

Select 
 p.Name ProductName
,p.BanglaName ProductBanglaName


,0    SaleCtnQuantity
,0    SalePcsQuantity
,0    SaleQuantity
,0    SaleSubTotal

, FLOOR(CAST(d.Quantity AS decimal(18, 2)) / NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0)) AS RetrunCtnQuantity
,CAST(d.Quantity AS decimal(18, 2)) % NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0) AS RetrunPcsQuantity
,d.Quantity RetrunQuantity
,d.UnitRate RetrunUnitRate
,d.SubTotal RetrunSubTotal

From SaleDeleveryReturnDetails d
left outer join SaleDeleveryReturns h on h.Id=d.SaleDeliveryReturnId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1

";



                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.ProductId))
                { query += " AND d.ProductId = @ProductId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += @"

) as result 
group by ProductName,ProductBanglaName

";


                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.ProductId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.Post))
                {
                    if (vm.Post == "Y")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", 1);
                    }
                    else
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", 0);
                    }
                }

                objComm.Fill(dataTable);

                var lst = new List<MonthlySalesAndAmountProductWiseVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new MonthlySalesAndAmountProductWiseVM
                    {

                        SL = serialNumber,
                        ProductName = row["ProductName"]?.ToString(),
                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        SaleCtnQuantity = Convert.ToDecimal(row["SaleCtnQuantity"]),
                        RetrunCtnQuantity = Convert.ToDecimal(row["RetrunCtnQuantity"]),
                        ActualCtnQuantity = Convert.ToDecimal(row["ActualCtnQuantity"]),
                        SalePcsQuantity = Convert.ToDecimal(row["SalePcsQuantity"]),
                        RetrunPcsQuantity = Convert.ToDecimal(row["RetrunPcsQuantity"]),
                        ActualPcsQuantity = Convert.ToDecimal(row["ActualPcsQuantity"]),
                        TotalSaleQuantity = Convert.ToDecimal(row["TotalSaleQuantity"]),
                        TotalRetrunQuantity = Convert.ToDecimal(row["TotalRetrunQuantity"]),
                        TotalQuantity = Convert.ToDecimal(row["TotalQuantity"]),
                        NetAmount = Convert.ToDecimal(row["NetAmount"]),

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

        public async Task<ResultVM> MonthlySalesAndAmountReportCustomerWise(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

Select
ISNULL(c.Name, '') AS CustomerName,
SUM(d.Quantity) AS TotalQty,
SUM(h.GrandTotalAmount) AS GrandTotalAmount,
SUM(h.SpecialDiscountAmount) AS Discount,
SUM(h.GrandTotalAmount) - (SUM(h.SpecialDiscountAmount)) AS TotalAmount,

ISNULL(MAX(b.Name), '-') AS BranchName,
ISNULL(MAX(b.Address), '-') AS BranchAddress


From SaleDeleveryDetails d
left outer join SaleDeleveries h on h.Id=d.SaleDeliveryId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1

";


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += " Group By c.Name ";


                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }

                if (!string.IsNullOrEmpty(vm.Post))
                {
                    if (vm.Post == "Y")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", 1);
                    }
                    else
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", 0);
                    }
                }

                objComm.Fill(dataTable);

                var lst = new List<MonthlySalesAndAmountCustomerWiseVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new MonthlySalesAndAmountCustomerWiseVM
                    {

                        SL = serialNumber,
                        CustomerName = row["CustomerName"].ToString(),
                        TotalQty = Convert.ToDecimal(row["TotalQty"]),
                        GrandTotalAmount = Convert.ToDecimal(row["GrandTotalAmount"]),
                        Discount = Convert.ToDecimal(row["Discount"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"]),

                        BranchName = row["BranchName"].ToString(),
                        BranchAddress = row["BranchAddress"].ToString()

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

        public async Task<ResultVM> ProductSalesReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

Select 
ProductName
,ProductBanglaName

,sum(SaleQuantity)SaleQuantity
,sum(RetrunQuantity)TotalRetrunQuantity
,sum(SaleQuantity-RetrunQuantity)TotalQuantity

from (

Select 
p.Name ProductName
,p.BanglaName ProductBanglaName


,d.CtnQuantity SaleCtnQuantity
,d.PcsQuantity SalePcsQuantity
,d.Quantity    SaleQuantity
,d.SubTotal    SaleSubTotal

,0    RetrunCtnQuantity
,0    RetrunPcsQuantity
,0    RetrunQuantity
,0    RetrunUnitRate
,0    RetrunSubTotal

From SaleDeleveryDetails d
left outer join SaleDeleveries h on h.Id=d.SaleDeliveryId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1


";


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.ProductId))
                { query += " AND d.ProductId = @ProductId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += @"

Union All 

Select 
p.Name ProductName
,p.BanglaName ProductBanglaName

,0    SaleCtnQuantity
,0    SalePcsQuantity
,0    SaleQuantity
,0    SaleSubTotal

, FLOOR(CAST(d.Quantity AS decimal(18, 2)) / NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0)) AS RetrunCtnQuantity
,CAST(d.Quantity AS decimal(18, 2)) % NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0) AS RetrunPcsQuantity
,d.Quantity RetrunQuantity
,d.UnitRate RetrunUnitRate
,d.SubTotal RetrunSubTotal

From SaleDeleveryReturnDetails d
left outer join SaleDeleveryReturns h on h.Id=d.SaleDeliveryReturnId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1


";

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.ProductId))
                { query += " AND d.ProductId = @ProductId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += @" 

) as result 

group by ProductName,ProductBanglaName

";


                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.ProductId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }

                if (!string.IsNullOrEmpty(vm.Post))
                {
                    if (vm.Post == "Y")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", true);
                    }
                    else
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", false);
                    }
                }

                objComm.Fill(dataTable);

                var lst = new List<ProductSalesReportVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new ProductSalesReportVM
                    {
                        SL = serialNumber,
                        ProductName = row["ProductName"]?.ToString(),
                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        SaleQuantity = Convert.ToDecimal(row["SaleQuantity"]),
                        TotalRetrunQuantity = Convert.ToDecimal(row["TotalRetrunQuantity"]),
                        TotalQuantity = Convert.ToDecimal(row["TotalQuantity"]),
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
        public async Task<ResultVM> ProductInventoryReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
WITH StockData AS (
    -- Opening Balance
    SELECT  
        Opening.ProductName,
        Opening.ProductBanglaName,
        SUM(Opening.OpeningQty + Opening.PurchaseQuantity + Opening.SaleQuantity + Opening.RetrunQuantity) AS OpeningQuantity,
        0 AS PurchaseQuantity,
        0 AS SaleQuantity,
        0 AS RetrunQuantity
    FROM (
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            SUM(d.OpeningQuantity) AS OpeningQty,
            0 AS PurchaseQuantity,
            0 AS SaleQuantity,
            0 AS RetrunQuantity
        FROM ProductsOpeningStocks d
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE d.OpeningDate < @DateFrom
        GROUP BY p.Name, p.BanglaName

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS OpeningQty,
            SUM(d.Quantity) AS PurchaseQuantity,
            0 AS SaleQuantity,
            0 AS RetrunQuantity
        FROM PurchaseDetails d
        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.PurchaseDate <= @DateFrom
        GROUP BY p.Name, p.BanglaName

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS OpeningQty,
            SUM(d.Quantity * -1) AS PurchaseQuantity,
            0 AS SaleQuantity,
            0 AS RetrunQuantity
        FROM PurchaseReturnDetails d
        LEFT JOIN PurchaseReturns h ON h.Id = d.PurchaseReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.PurchaseReturnDate <= @DateFrom
        GROUP BY p.Name, p.BanglaName

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS OpeningQty,
            0 AS PurchaseQuantity,
            SUM(d.Quantity * -1) AS SaleQuantity,
            0 AS RetrunQuantity
        FROM SaleDeleveryDetails d
        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.DeliveryDate <= @DateFrom
        GROUP BY p.Name, p.BanglaName

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS OpeningQty,
            0 AS PurchaseQuantity,
            0 AS SaleQuantity,
            SUM(d.Quantity) AS RetrunQuantity
        FROM SaleDeleveryReturnDetails d
        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.InvoiceDateTime >= @DateFrom
        GROUP BY p.Name, p.BanglaName
    ) AS Opening
    GROUP BY Opening.ProductName, Opening.ProductBanglaName

    UNION ALL

    -- Transactions
    SELECT  
        Result.ProductName,
        Result.ProductBanglaName,
        0 AS OpeningQuantity,
        SUM(Result.PurchaseQuantity) AS PurchaseQuantity,
        SUM(Result.SaleQuantity) AS SaleQuantity,
        SUM(Result.RetrunQuantity) AS RetrunQuantity
    FROM (
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.Quantity AS PurchaseQuantity,
            0 AS SaleQuantity,
            0 AS RetrunQuantity
        FROM PurchaseDetails d
        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.PurchaseDate BETWEEN @DateFrom AND @DateTo

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.Quantity * -1 AS PurchaseQuantity,
            0 AS SaleQuantity,
            0 AS RetrunQuantity
        FROM PurchaseReturnDetails d
        LEFT JOIN PurchaseReturns h ON h.Id = d.PurchaseReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.PurchaseReturnDate BETWEEN @DateFrom AND @DateTo

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS PurchaseQuantity,
            d.Quantity AS SaleQuantity,
            0 AS RetrunQuantity
        FROM SaleDeleveryDetails d
        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.DeliveryDate BETWEEN @DateFrom AND @DateTo

        UNION ALL

        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            0 AS PurchaseQuantity,
            0 AS SaleQuantity,
            d.Quantity AS RetrunQuantity
        FROM SaleDeleveryReturnDetails d
        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId AND h.IsPost = @Post AND h.InvoiceDateTime BETWEEN @DateFrom AND @DateTo
    ) AS Result
    GROUP BY Result.ProductName, Result.ProductBanglaName
)

SELECT 
    ProductName,
    ProductBanglaName,
    SUM(OpeningQuantity) AS OpeningQuantity,
    SUM(PurchaseQuantity) AS PurchaseQuantity,
    SUM(SaleQuantity) AS SaleQuantity,
    SUM(RetrunQuantity) AS RetrunQuantity,
    SUM(OpeningQuantity + PurchaseQuantity - SaleQuantity + RetrunQuantity) AS TotalQty
FROM StockData
GROUP BY ProductName, ProductBanglaName;
";

                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);

                adapter.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? "0");
                adapter.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                adapter.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                adapter.SelectCommand.Parameters.AddWithValue("@Post", vm.Post == "Y");

                if (!string.IsNullOrEmpty(vm.ProductId))
                    adapter.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId);

                adapter.Fill(dataTable);

                var list = new List<ProductInventoryReportVM>();
                int sl = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(new ProductInventoryReportVM
                    {
                        SL = sl++,
                        ProductName = row["ProductName"]?.ToString(),
                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        OpeningQuantity = Convert.ToDecimal(row["OpeningQuantity"]),
                        PurchaseQuantity = Convert.ToDecimal(row["PurchaseQuantity"]),
                        SaleQuantity = Convert.ToDecimal(row["SaleQuantity"]),
                        RetrunQuantity = Convert.ToDecimal(row["RetrunQuantity"]),
                        TotalQty = Convert.ToDecimal(row["TotalQty"])
                    });
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = list;
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

        //        public async Task<ResultVM> ProductInventoryReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //WITH StockData AS (
        //    -- Opening Balance
        //    SELECT  
        //        Opening.ProductName,
        //        Opening.ProductBanglaName,
        //        SUM(Opening.OpeningQty+Opening.PurchaseQuantity + Opening.SaleQuantity + Opening.RetrunQuantity) AS OpeningQuantity,
        //        0 AS PurchaseQuantity,
        //        0 AS SaleQuantity,
        //        0 AS RetrunQuantity
        //    FROM (

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            SUM(d.OpeningQuantity) AS OpeningQty,
        //            0 AS PurchaseQuantity,
        //            0 AS SaleQuantity,
        //            0 AS RetrunQuantity
        //        FROM ProductsOpeningStocks d

        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1 = 1 

        //";


        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND d.OpeningDate < @DateFrom "; }


        //                query += @"

        //        GROUP BY p.Name, p.BanglaName

        //		UNION ALL

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            0 AS OpeningQty,
        //            SUM(d.Quantity) AS PurchaseQuantity,
        //            0 AS SaleQuantity,
        //            0 AS RetrunQuantity
        //        FROM PurchaseDetails d
        //        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1


        //";

        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                //if (!string.IsNullOrEmpty(vm.DateFrom))
        //                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.PurchaseDate <= @DateFrom "; }


        //                query += @" 
        //        GROUP BY p.Name, p.BanglaName

        //        UNION ALL

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            0 AS OpeningQty,
        //            0 AS PurchaseQuantity,
        //            SUM(d.Quantity * -1) AS SaleQuantity,
        //            0 AS RetrunQuantity
        //        FROM SaleDeleveryDetails d
        //        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                //if (!string.IsNullOrEmpty(vm.DateFrom))
        //                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.DeliveryDate <= @DateFrom "; }



        //                query += @" 

        //        GROUP BY p.Name, p.BanglaName

        //        UNION ALL

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            0 AS OpeningQty,
        //            0 AS PurchaseQuantity,
        //            0 AS SaleQuantity,
        //            SUM(d.Quantity) AS RetrunQuantity
        //        FROM SaleDeleveryReturnDetails d
        //        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1 

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                //if (!string.IsNullOrEmpty(vm.DateTo))
        //                //{ query += " AND h.InvoiceDateTime <= @DateTo "; }



        //                query += @" 

        //    GROUP BY p.Name, p.BanglaName

        //    ) AS Opening
        //    GROUP BY Opening.ProductName, Opening.ProductBanglaName

        //    UNION ALL

        //    -- Transactions
        //    SELECT  
        //        Result.ProductName,
        //        Result.ProductBanglaName,
        //        0 AS OpeningQuantity,
        //        SUM(Result.PurchaseQuantity) AS PurchaseQuantity,
        //        SUM(Result.SaleQuantity) AS SaleQuantity,
        //        SUM(Result.RetrunQuantity) AS RetrunQuantity
        //    FROM (
        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            d.Quantity AS PurchaseQuantity,
        //            0 AS SaleQuantity,
        //            0 AS RetrunQuantity
        //        FROM PurchaseDetails d
        //        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.PurchaseDate >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.PurchaseDate <= @DateTo "; }


        //                query += @" 

        //        UNION ALL

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            0 AS PurchaseQuantity,
        //            d.Quantity AS SaleQuantity,
        //            0 AS RetrunQuantity
        //        FROM SaleDeleveryDetails d
        //        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId

        //		WHERE 1=1

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.DeliveryDate >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.DeliveryDate <= @DateTo "; }


        //                query += @" 

        //        UNION ALL

        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            0 AS PurchaseQuantity,
        //            0 AS SaleQuantity,
        //            d.Quantity AS RetrunQuantity
        //        FROM SaleDeleveryReturnDetails d
        //        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        //        LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1


        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.InvoiceDateTime <= @DateTo "; }



        //                query += @" 

        //    ) AS Result
        //    GROUP BY Result.ProductName, Result.ProductBanglaName
        //)

        //-- Final Select with TotalQty Calculation
        //SELECT 
        //    ProductName,
        //    ProductBanglaName,
        //    SUM(OpeningQuantity) AS OpeningQuantity,
        //    SUM(PurchaseQuantity) AS PurchaseQuantity,
        //    SUM(SaleQuantity) AS SaleQuantity,
        //    SUM(RetrunQuantity) AS RetrunQuantity,
        //    -- TotalQty = Opening + Purchases - Sales + Returns
        //    SUM(OpeningQuantity + PurchaseQuantity - SaleQuantity + RetrunQuantity) AS TotalQty
        //FROM StockData
        //GROUP BY ProductName, ProductBanglaName;

        //";



        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.Post))
        //                {
        //                    if (vm.Post == "Y")
        //                    {
        //                        objComm.SelectCommand.Parameters.AddWithValue("@Post", true);
        //                    }
        //                    else
        //                    {
        //                        objComm.SelectCommand.Parameters.AddWithValue("@Post", false);

        //                    }
        //                }


        //                objComm.Fill(dataTable);

        //                var lst = new List<ProductInventoryReportVM>();

        //                int serialNumber = 1;

        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    lst.Add(new ProductInventoryReportVM
        //                    {
        //                        SL = serialNumber,
        //                        ProductName = row["ProductName"]?.ToString(),
        //                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
        //                        OpeningQuantity = Convert.ToDecimal(row["OpeningQuantity"]),
        //                        PurchaseQuantity = Convert.ToDecimal(row["PurchaseQuantity"]),
        //                        SaleQuantity = Convert.ToDecimal(row["SaleQuantity"]),
        //                        RetrunQuantity = Convert.ToDecimal(row["RetrunQuantity"]),
        //                        TotalQty = Convert.ToDecimal(row["TotalQty"]),
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


        public async Task<ResultVM> CustomerBillReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

WITH SalesData AS (
    SELECT 
        C.Name AS CustomerName,
        C.BanglaName AS CustomerBanglaName,

        SUM(d.CtnQuantity) AS SaleCtnQuantity,
        SUM(d.PcsQuantity) AS SalePcsQuantity,
        SUM(d.Quantity) AS SaleQuantity,
        SUM(d.SubTotal) AS SaleSubTotal,
        SUM(0) AS RetrunCtnQuantity,
        SUM(0) AS RetrunPcsQuantity,
        SUM(0) AS RetrunQuantity,
        SUM(0) AS RetrunSubTotal,
        SUM(0) AS TotalDiscountAmount
    FROM SaleDeleveryDetails d
    LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
    LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
    LEFT JOIN Customers c ON h.CustomerId = c.Id
    LEFT JOIN Products p ON p.Id = d.ProductId

	Where 1 = 1 

";



                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += @" 

    GROUP BY C.Name, C.BanglaName

    UNION ALL

    SELECT 
        C.Name AS CustomerName,
        C.BanglaName AS CustomerBanglaName,

        SUM(0) AS SaleCtnQuantity,
        SUM(0) AS SalePcsQuantity,
        SUM(0) AS SaleQuantity,
        SUM(0) AS SaleSubTotal,
        SUM(FLOOR(CAST(d.Quantity AS decimal(18, 2)) / NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0))) AS RetrunCtnQuantity,
        SUM(CAST(d.Quantity AS decimal(18, 2)) % NULLIF(CAST(p.CtnSizeFactor AS decimal(18, 2)), 0)) AS RetrunPcsQuantity,
        SUM(d.Quantity) AS RetrunQuantity,
        SUM(d.SubTotal) AS RetrunSubTotal,
        SUM(0) AS TotalDiscountAmount
    FROM SaleDeleveryReturnDetails d
    LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
    LEFT JOIN BranchProfiles B ON B.Id = h.BranchId
    LEFT JOIN Customers c ON h.CustomerId = c.Id
    LEFT JOIN Products p ON p.Id = d.ProductId

	Where 1=1

";


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }



                query += @" 

 GROUP BY C.Name, C.BanglaName

), DiscountData AS (
    SELECT 
        C.Name AS CustomerName,
        C.BanglaName AS CustomerBanglaName,
        SUM(h.SpecialDiscountAmount + h.RegularDiscountAmount) AS TotalDiscountAmount
    FROM SaleDeleveryDetails d
    LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
    LEFT JOIN Customers c ON h.CustomerId = c.Id

	where 1=1

";


                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                { query += " AND h.BranchId = @BranchId "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }

                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }


                query += @"  

GROUP BY C.Name, C.BanglaName
)

SELECT 
    s.CustomerName,
    s.CustomerBanglaName,
    SUM(s.SaleQuantity - s.RetrunQuantity) AS TotalQuantity,
    SUM(s.SaleCtnQuantity - s.RetrunCtnQuantity) AS TotalCtnQuantity,
    SUM(s.SaleSubTotal - s.RetrunSubTotal) AS GrossAmount,
    COALESCE(d.TotalDiscountAmount, 0) AS TotalDiscountAmount,
    SUM(s.SaleSubTotal - s.RetrunSubTotal) - COALESCE(d.TotalDiscountAmount, 0) AS NetAmount
FROM SalesData s
LEFT JOIN DiscountData d ON s.CustomerName = d.CustomerName AND s.CustomerBanglaName = d.CustomerBanglaName
GROUP BY s.CustomerName, s.CustomerBanglaName, d.TotalDiscountAmount
HAVING SUM(s.SaleQuantity - s.RetrunQuantity) > 0
ORDER BY s.CustomerName;

";


                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.Post))
                {
                    if (vm.Post == "Y")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", true);
                    }
                    else
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", false);

                    }
                }

                objComm.Fill(dataTable);

                var lst = new List<CustomerBillVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new CustomerBillVM
                    {
                        SL = serialNumber,
                        CustomerName = row["CustomerName"]?.ToString(),
                        CustomerBanglaName = row["CustomerBanglaName"]?.ToString(),
                        TotalQuantity = Convert.ToDecimal(row["TotalQuantity"]),
                        TotalCtnQuantity = Convert.ToDecimal(row["TotalCtnQuantity"]),
                        GrossAmount = Convert.ToDecimal(row["GrossAmount"]),
                        TotalDiscountAmount = Convert.ToDecimal(row["TotalDiscountAmount"]),
                        NetAmount = Convert.ToDecimal(row["NetAmount"]),
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


        public async Task<ResultVM> CustomerDueListReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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
        C.Name AS CustomerName,
	    H.DeliveryDate AS [Date],
		H.Code AS [BillNo],
        C.BanglaName AS CustomerBanglaName,
        D.LineTotal AS [GrossAmount],
        h.SpecialDiscountAmount + h.RegularDiscountAmount AS [Discount],
	    D.LineTotal -h.SpecialDiscountAmount - h.RegularDiscountAmount AS [NetAmount]
        FROM SaleDeleveryDetails d
        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        LEFT JOIN Customers c ON h.CustomerId = c.Id
	    where 1=1

";


                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (vm.Post == "Y" || vm.Post == "N")
                { query += " AND h.IsPost = @Post "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }

                if (!string.IsNullOrEmpty(vm.CustomerId))
                { query += " AND h.CustomerId = @CustomerId "; }


                query += @" 

	    and isnull(Processed,0)=0
		and RestAmount>0
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.CustomerId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.Post))
                {
                    if (vm.Post == "Y")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", true);
                    }
                    else
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@Post", false);

                    }
                }

                objComm.Fill(dataTable);

                var lst = new List<CustomerDueListVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new CustomerDueListVM
                    {

                        SL = serialNumber,
                        CustomerName = row["CustomerName"]?.ToString(),
                        CustomerBanglaName = row["CustomerBanglaName"]?.ToString(),
                        Date = row["Date"]?.ToString(),
                        BillNo = row["BillNo"]?.ToString(),
                        GrossAmount = Convert.ToDecimal(row["GrossAmount"]),
                        DiscountAmount = Convert.ToDecimal(row["Discount"]),
                        NetAmount = Convert.ToDecimal(row["NetAmount"]),

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


        public async Task<ResultVM> CustomerDueListCustomerWiseReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

 Select
 
 c.Name AS CustomerName
,0 AS DueAmount
,'' AS FirstDueBill
,FORMAT(h.InvoiceDateTime, 'dd-MMM-yyyy') AS FirstDueDate

,ISNULL(b.Name,'-') BranchName
,ISNULL(b.Address,'-') BranchAddress


From SaleDeleveryDetails d
left outer join SaleDeleveries h on h.Id=d.SaleDeliveryId
left outer join SalesPersons sp on h.SalePersonId=sp.Id
left outer join BranchProfiles B on B.Id=h.BranchId
left outer join Customers c on h.CustomerId=c.Id
left outer join Routes R on c.RouteId=R.Id
left outer join Products p on p.Id=d.ProductId
where 1=1

";


                if (!string.IsNullOrEmpty(vm.DateFrom))
                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

                if (!string.IsNullOrEmpty(vm.DateTo))
                { query += " AND h.InvoiceDateTime <= @DateTo "; }

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                if (!string.IsNullOrEmpty(vm.DateFrom))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
                }
                if (!string.IsNullOrEmpty(vm.DateTo))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
                }

                objComm.Fill(dataTable);

                var lst = new List<CustomerDueListCustomerWiseVM>();

                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new CustomerDueListCustomerWiseVM
                    {

                        SL = serialNumber,
                        CustomerName = row["CustomerName"]?.ToString(),
                        DueAmount = Convert.ToDecimal(row["DueAmount"]),
                        FirstDueBill = row["FirstDueBill"]?.ToString(),
                        FirstDueDate = row["FirstDueDate"].ToString(),

                        BranchName = row["BranchName"].ToString(),
                        BranchAddress = row["BranchAddress"].ToString()

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

        public async Task<ResultVM> SinglePorductInventoryReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
WITH StockData AS (
    -- Opening Balance
    SELECT 
        'Opening' AS [Invoice/Rcv ID/Rtn Id],
        @DateFrom AS [Date],
        Opening.ProductName,
        Opening.ProductBanglaName,
        'Opening' AS TranType,
        SUM(OpeningQty) AS OpeningQuantity,
        0 AS PurchaseQuantity,
        0 AS SaleQuantity,
        0 AS RetrunQuantity
    FROM (
        -- Opening stock before DateFrom
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.OpeningQuantity AS OpeningQty
        FROM ProductsOpeningStocks d
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE d.OpeningDate < @DateFrom

        UNION ALL

        -- Purchases before @DateFrom
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.Quantity AS OpeningQty
        FROM PurchaseDetails d
        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId  
          AND h.IsPost = @Post  
          AND d.ProductId = @ProductId  
          AND p.Id = @ProductId  
          AND h.PurchaseDate <= @DateFrom  

        UNION ALL

        -- Purchase Returns before @DateFrom
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.Quantity * -1 AS OpeningQty
        FROM PurchaseReturnDetails d
        LEFT JOIN PurchaseReturns h ON h.Id = d.PurchaseReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId  
          AND h.IsPost = @Post  
          AND d.ProductId = @ProductId  
          AND p.Id = @ProductId  
          AND h.PurchaseReturnDate <= @DateFrom  

        UNION ALL

        -- Sales before @DateFrom 
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            -1 * d.Quantity AS OpeningQty
        FROM SaleDeleveryDetails d
        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId  
          AND h.IsPost = @Post  
          AND h.CustomerId = @CustomerId  
          AND d.ProductId = @ProductId  
          AND p.Id = @ProductId  
          AND h.DeliveryDate < @DateFrom  

        UNION ALL

        -- Returns before @DateFrom 
        SELECT 
            p.Name AS ProductName,
            p.BanglaName AS ProductBanglaName,
            d.Quantity AS OpeningQty
        FROM SaleDeleveryReturnDetails d
        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        LEFT JOIN Products p ON p.Id = d.ProductId
        WHERE h.BranchId = @BranchId  
          AND h.IsPost = @Post  
          AND h.CustomerId = @CustomerId  
          AND d.ProductId = @ProductId  
          AND p.Id = @ProductId  
          AND h.InvoiceDateTime < @DateFrom 
    ) AS Opening
    GROUP BY Opening.ProductName, Opening.ProductBanglaName

    UNION ALL

    -- Purchases during date range
    SELECT 
        h.Code AS [Invoice/Rcv ID/Rtn Id],
        h.PurchaseDate AS [Date],
        p.Name AS ProductName,
        p.BanglaName AS ProductBanglaName,
        'Purchase' AS TranType,
        0 AS OpeningQuantity,
        SUM(d.Quantity) AS PurchaseQuantity,
        0 AS SaleQuantity,
        0 AS RetrunQuantity
    FROM PurchaseDetails d
    LEFT JOIN Purchases h ON h.Id = d.PurchaseId
    LEFT JOIN Products p ON p.Id = d.ProductId
    WHERE h.BranchId = @BranchId  
      AND h.IsPost = @Post  
      AND d.ProductId = @ProductId  
      AND p.Id = @ProductId  
      AND h.PurchaseDate >= @DateFrom  
      AND h.PurchaseDate <= @DateTo  
    GROUP BY h.Code, h.PurchaseDate, p.Name, p.BanglaName

    UNION ALL

    -- Purchase Returns during date range
    SELECT 
        h.Code AS [Invoice/Rcv ID/Rtn Id],
        h.PurchaseReturnDate AS [Date],
        p.Name AS ProductName,
        p.BanglaName AS ProductBanglaName,
        'Purchase Return' AS TranType,
        0 AS OpeningQuantity,
        SUM(d.Quantity * -1) AS PurchaseQuantity,
        0 AS SaleQuantity,
        0 AS RetrunQuantity
    FROM PurchaseReturnDetails d
    LEFT JOIN PurchaseReturns h ON h.Id = d.PurchaseReturnId
    LEFT JOIN Products p ON p.Id = d.ProductId
    WHERE h.BranchId = @BranchId  
      AND h.IsPost = @Post  
      AND d.ProductId = @ProductId  
      AND p.Id = @ProductId  
      AND h.PurchaseReturnDate >= @DateFrom  
      AND h.PurchaseReturnDate <= @DateTo  
    GROUP BY h.Code, h.PurchaseReturnDate, p.Name, p.BanglaName

    UNION ALL

    -- Sales during date range
    SELECT 
        h.Code AS [Invoice/Rcv ID/Rtn Id],
        h.DeliveryDate AS [Date],
        p.Name AS ProductName,
        p.BanglaName AS ProductBanglaName,
        'Sale' AS TranType,
        0 AS OpeningQuantity,
        0 AS PurchaseQuantity,
        SUM(d.Quantity) AS SaleQuantity,
        0 AS RetrunQuantity
    FROM SaleDeleveryDetails d
    LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
    LEFT JOIN Products p ON p.Id = d.ProductId
    WHERE h.BranchId = @BranchId  
      AND h.IsPost = @Post  
      AND h.CustomerId = @CustomerId  
      AND d.ProductId = @ProductId  
      AND p.Id = @ProductId  
      AND h.DeliveryDate >= @DateFrom  
      AND h.DeliveryDate <= @DateTo  
    GROUP BY h.Code, h.DeliveryDate, p.Name, p.BanglaName

    UNION ALL

    -- Returns during date range
    SELECT 
        h.Code AS [Invoice/Rcv ID/Rtn Id],
        h.InvoiceDateTime AS [Date],
        p.Name AS ProductName,
        p.BanglaName AS ProductBanglaName,
        'Return' AS TranType,
        0 AS OpeningQuantity,
        0 AS PurchaseQuantity,
        0 AS SaleQuantity,
        SUM(d.Quantity) AS RetrunQuantity
    FROM SaleDeleveryReturnDetails d
    LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
    LEFT JOIN Products p ON p.Id = d.ProductId
    WHERE h.BranchId = @BranchId  
      AND h.IsPost = @Post  
      AND h.CustomerId = @CustomerId  
      AND d.ProductId = @ProductId  
      AND p.Id = @ProductId  
      AND h.InvoiceDateTime >= @DateFrom  
      AND h.InvoiceDateTime <= @DateTo  
    GROUP BY h.Code, h.InvoiceDateTime, p.Name, p.BanglaName
)

SELECT 
    [Invoice/Rcv ID/Rtn Id] AS InvoiceRcvIDRtnId,
    [Date],
    ProductName,
    ProductBanglaName,
    TranType,
    OpeningQuantity,
    PurchaseQuantity,
    SaleQuantity,
    RetrunQuantity,
    SUM(
        ISNULL(OpeningQuantity,0) + ISNULL(PurchaseQuantity,0) 
        - ISNULL(SaleQuantity,0) + ISNULL(RetrunQuantity,0)
    ) OVER (
        ORDER BY 
            [Invoice/Rcv ID/Rtn Id],
            CASE TranType 
                WHEN 'Opening' THEN 1
                WHEN 'Purchase' THEN 2
                WHEN 'Purchase Return' THEN 3
                WHEN 'Sale' THEN 4
                WHEN 'Return' THEN 5
                ELSE 6
            END
        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
    ) AS TotalQty
FROM StockData
ORDER BY 
    [Date], 
    CASE TranType 
        WHEN 'Opening' THEN 1
        WHEN 'Purchase' THEN 2
        WHEN 'Purchase Return' THEN 3
        WHEN 'Sale' THEN 4
        WHEN 'Return' THEN 5
        ELSE 6
    END;
";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // Add parameters
                objComm.SelectCommand.Parameters.AddWithValue("@BranchId", string.IsNullOrEmpty(vm.BranchId) ? (object)DBNull.Value : vm.BranchId);
                objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", string.IsNullOrEmpty(vm.CustomerId) ? (object)DBNull.Value : vm.CustomerId);
                objComm.SelectCommand.Parameters.AddWithValue("@ProductId", string.IsNullOrEmpty(vm.ProductId) ? (object)DBNull.Value : vm.ProductId);
                objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", string.IsNullOrEmpty(vm.DateFrom) ? (object)DBNull.Value : vm.DateFrom);
                objComm.SelectCommand.Parameters.AddWithValue("@DateTo", string.IsNullOrEmpty(vm.DateTo) ? (object)DBNull.Value : vm.DateTo);
                if (!string.IsNullOrEmpty(vm.Post))
                    objComm.SelectCommand.Parameters.AddWithValue("@Post", vm.Post == "Y");

                objComm.Fill(dataTable);

                var lst = new List<SinglePorductInventoryVM>();
                int serialNumber = 1;

                foreach (DataRow row in dataTable.Rows)
                {
                    lst.Add(new SinglePorductInventoryVM
                    {
                        SL = serialNumber++,
                        InvoiceRcvIDRtnId = row["InvoiceRcvIDRtnId"]?.ToString(),
                        Date = row["Date"]?.ToString(),
                        ProductName = row["ProductName"]?.ToString(),
                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
                        TranType = row["TranType"]?.ToString(),
                        OpeningQuantity = Convert.ToDecimal(row["OpeningQuantity"]),  
                        PurchaseQuantity = Convert.ToDecimal(row["PurchaseQuantity"]),
                        SaleQuantity = Convert.ToDecimal(row["SaleQuantity"]),
                        RetrunQuantity = Convert.ToDecimal(row["RetrunQuantity"]),
                        TotalQty = Convert.ToDecimal(row["TotalQty"]),
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
                    conn.Close();
            }
        }

        //        public async Task<ResultVM> SinglePorductInventoryReport(MISReportVM vm, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //WITH StockData AS (
        //    -- Opening Balance
        //    SELECT 
        //        'Opening' AS [Invoice/Rcv ID/Rtn Id],
        //        @DateFrom AS [Date],
        //        Opening.ProductName,
        //        Opening.ProductBanglaName,
        //        'Opening' AS TranType,
        //        SUM(OpeningQty) AS OpeningQuantity,
        //        0 AS PurchaseQuantity,
        //        0 AS SaleQuantity,
        //        0 AS RetrunQuantity
        //    FROM (



        //--declare opening
        //	   SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            d.OpeningQuantity AS OpeningQty

        //        FROM ProductsOpeningStocks d
        //		LEFT JOIN Products p ON p.Id = d.ProductId
        //		WHERE 1=1

        //";



        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND d.OpeningDate < @DateFrom "; }




        //                query += @"

        //        UNION ALL
        //        -- Purchases before @DateFrom
        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            d.Quantity AS OpeningQty
        //        FROM PurchaseDetails d
        //        LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1 


        //";



        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                //if (!string.IsNullOrEmpty(vm.DateFrom))
        //                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.PurchaseDate <= @DateFrom "; }


        //                query += @" 

        // UNION ALL

        //        -- Sales before @DateFrom 
        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            -1 * d.Quantity AS OpeningQty
        //        FROM SaleDeleveryDetails d
        //        LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1 

        //";

        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.CustomerId))
        //                { query += " AND h.CustomerId = @CustomerId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                //if (!string.IsNullOrEmpty(vm.DateFrom))
        //                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.DeliveryDate < @DateFrom "; }


        //                query += @" 

        //        UNION ALL

        //        -- Returns before @DateFrom 
        //        SELECT 
        //            p.Name AS ProductName,
        //            p.BanglaName AS ProductBanglaName,
        //            d.Quantity AS OpeningQty
        //        FROM SaleDeleveryReturnDetails d
        //        LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        //        LEFT JOIN Products p ON p.Id = d.ProductId
        //        WHERE 1=1  

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.CustomerId))
        //                { query += " AND h.CustomerId = @CustomerId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                //if (!string.IsNullOrEmpty(vm.DateFrom))
        //                //{ query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.InvoiceDateTime < @DateFrom "; }


        //                query += @"

        // ) AS Opening
        //    GROUP BY Opening.ProductName, Opening.ProductBanglaName

        //    UNION ALL

        //    -- Purchases during date range
        //    SELECT 
        //        h.Code AS [Invoice/Rcv ID/Rtn Id],
        //        h.PurchaseDate AS [Date],
        //        p.Name AS ProductName,
        //        p.BanglaName AS ProductBanglaName,
        //        'Purchase' AS TranType,
        //        0,
        //        SUM(d.Quantity),
        //        0,
        //        0
        //    FROM PurchaseDetails d
        //    LEFT JOIN Purchases h ON h.Id = d.PurchaseId
        //    LEFT JOIN Products p ON p.Id = d.ProductId
        //    WHERE 1=1 

        //";



        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.PurchaseDate >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.PurchaseDate <= @DateTo "; }


        //                query += @" 

        // GROUP BY h.Code, h.PurchaseDate, p.Name, p.BanglaName

        //    UNION ALL

        //    -- Sales during date range
        //    SELECT 
        //        h.Code AS [Invoice/Rcv ID/Rtn Id],
        //        h.DeliveryDate AS [Date],
        //        p.Name AS ProductName,
        //        p.BanglaName AS ProductBanglaName,
        //        'Sale' AS TranType,
        //        0,
        //        0,
        //        SUM(d.Quantity),
        //        0
        //    FROM SaleDeleveryDetails d
        //    LEFT JOIN SaleDeleveries h ON h.Id = d.SaleDeliveryId
        //    LEFT JOIN Products p ON p.Id = d.ProductId
        //    WHERE 1=1 

        //";


        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.CustomerId))
        //                { query += " AND h.CustomerId = @CustomerId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.DeliveryDate >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.DeliveryDate <= @DateTo "; }


        //                query += @" 

        //    GROUP BY h.Code, h.DeliveryDate, p.Name, p.BanglaName

        //    UNION ALL

        //    -- Returns during date range
        //    SELECT 
        //        h.Code AS [Invoice/Rcv ID/Rtn Id],
        //        h.InvoiceDateTime AS [Date],
        //        p.Name AS ProductName,
        //        p.BanglaName AS ProductBanglaName,
        //        'Return' AS TranType,
        //        0,
        //        0,
        //        0,
        //        SUM(d.Quantity)
        //    FROM SaleDeleveryReturnDetails d
        //    LEFT JOIN SaleDeleveryReturns h ON h.Id = d.SaleDeliveryReturnId
        //    LEFT JOIN Products p ON p.Id = d.ProductId
        //    WHERE 1=1 


        //";



        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                { query += " AND h.BranchId = @BranchId "; }

        //                if (vm.Post == "Y" || vm.Post == "N")
        //                { query += " AND h.IsPost = @Post "; }

        //                if (!string.IsNullOrEmpty(vm.CustomerId))
        //                { query += " AND h.CustomerId = @CustomerId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND d.ProductId = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                { query += " AND p.Id = @ProductId "; }

        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                { query += " AND h.InvoiceDateTime >= @DateFrom "; }

        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                { query += " AND h.InvoiceDateTime <= @DateTo "; }


        //                query += @" 


        // GROUP BY h.Code, h.InvoiceDateTime, p.Name, p.BanglaName
        //)

        //SELECT 
        //    [Invoice/Rcv ID/Rtn Id] InvoiceRcvIDRtnId,
        //    [Date],
        //    ProductName,
        //    ProductBanglaName,
        //    TranType,
        //    OpeningQuantity,
        //    PurchaseQuantity,
        //    SaleQuantity,
        //    RetrunQuantity,
        //    SUM(
        //        ISNULL(OpeningQuantity,0) + ISNULL(PurchaseQuantity,0) 
        //        - ISNULL(SaleQuantity,0) + ISNULL(RetrunQuantity,0)
        //    ) OVER (
        //        ORDER BY 
        //            [Invoice/Rcv ID/Rtn Id],
        //            CASE TranType 
        //                WHEN 'Opening' THEN 1
        //                WHEN 'Purchase' THEN 2
        //                WHEN 'Sale' THEN 3
        //                WHEN 'Return' THEN 4
        //                ELSE 5
        //            END
        //        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
        //    ) AS TotalQty
        //FROM StockData
        //ORDER BY [Date], 
        //         CASE TranType 
        //            WHEN 'Opening' THEN 1
        //            WHEN 'Purchase' THEN 2
        //            WHEN 'Sale' THEN 3
        //            WHEN 'Return' THEN 4
        //            ELSE 5
        //         END;


        //                    ";



        //                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

        //                if (!string.IsNullOrEmpty(vm.BranchId) && vm.BranchId != "0")
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.CustomerId))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.ProductId))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@ProductId", vm.ProductId ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.DateFrom))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@DateFrom", vm.DateFrom ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.DateTo))
        //                {
        //                    objComm.SelectCommand.Parameters.AddWithValue("@DateTo", vm.DateTo ?? (object)DBNull.Value);
        //                }
        //                if (!string.IsNullOrEmpty(vm.Post))
        //                {
        //                    if (vm.Post == "Y")
        //                    {
        //                        objComm.SelectCommand.Parameters.AddWithValue("@Post", true);
        //                    }
        //                    else
        //                    {
        //                        objComm.SelectCommand.Parameters.AddWithValue("@Post", false);

        //                    }
        //                }

        //                objComm.Fill(dataTable);

        //                var lst = new List<SinglePorductInventoryVM>();

        //                int serialNumber = 1;

        //                foreach (DataRow row in dataTable.Rows)
        //                {
        //                    lst.Add(new SinglePorductInventoryVM
        //                    {

        //                        SL = serialNumber,
        //                        InvoiceRcvIDRtnId = row["InvoiceRcvIDRtnId"]?.ToString(),
        //                        Date = row["Date"]?.ToString(),
        //                        ProductName = row["ProductName"]?.ToString(),
        //                        ProductBanglaName = row["ProductBanglaName"]?.ToString(),
        //                        TranType = row["TranType"]?.ToString(),
        //                        OpeningQuantity = Convert.ToDecimal(row["OpeningQuantity"]),
        //                        PurchaseQuantity = Convert.ToDecimal(row["PurchaseQuantity"]),
        //                        SaleQuantity = Convert.ToDecimal(row["SaleQuantity"]),
        //                        RetrunQuantity = Convert.ToDecimal(row["RetrunQuantity"]),
        //                        TotalQty = Convert.ToDecimal(row["TotalQty"]),

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





    }
}
