using ShampanPOS.ViewModel;
using System.Data;
using System.Data.SqlClient;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using Microsoft.VisualBasic;
using System;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Repository
{
    public class CustomerJournalRepository : CommonRepository
    {
        public async Task<ResultVM> GetCustomerJournal(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

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
                    SL, a.Code, InvoiceDateTime, CustomerId, 
                    c.Code AS CustomerCode, c.Name AS CustomerName, 
                    c.Address AS CustomerAddress, ISNULL(a.Opening, 0) AS Opening, 
                    DrAmount, 
                    SUM(ISNULL(a.Opening, 0) + ISNULL(DrAmount, 0) - ISNULL(CrAmount, 0))
                        OVER(PARTITION BY a.CustomerId ORDER BY a.InvoiceDateTime, a.Code, SL 
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS CurrentBalance, 
                    Remarks
                FROM (
                    SELECT 'D' AS SL, Code, InvoiceDateTime, CustomerId, 0 AS Opening, 0 AS DrAmount,
                        GrandTotalAmount AS CrAmount, 'Sales' AS Remarks
                    FROM SaleDeleveries

                    UNION ALL

                    SELECT 'B' AS SL, ISNULL(s.Code, 0) AS Code, NULL AS InvoiceDateTime,
                        ISNULL(s.CustomerId, 0) AS CustomerId, 0 AS Opening, 
                        b.Amount AS DrAmount, 0 AS CrAmount, 
                        b.ModeOfPayment + '~ ' + '~ ' AS Remarks
                    FROM CustomerPaymentCollection b
                    LEFT JOIN SaleDeleveries s ON b.CustomerId = s.CustomerId
                ) AS a
                LEFT JOIN Customers c ON a.CustomerId = c.Id
                WHERE c.IsActive = 1";

                SqlDataAdapter adapter = CreateAdapter(query, conn, transaction);
                adapter.Fill(dataTable);

                result.Status = "Success";
                result.DataVM = dataTable;
                return result;
            }
            catch (Exception ex)
            {
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


