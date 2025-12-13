using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Repository
{
    public class ProductStockRepository
    {


        public async Task<ResultVM> Insert(ProductStockVM productStockVM, SqlConnection conn = null, SqlTransaction transaction = null)
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
                                    INSERT INTO ProductsOpeningStocks
                                    (
                                        ProductId,   BranchId, OpeningQuantity, OpeningValue, OpeningDate, 
                                        CreatedBy, CreatedOn, CreatedFrom)
                                    VALUES 
                                    (
                                        @ProductId,  @BranchId, @OpeningQuantity, @OpeningValue, @OpeningDate, 
                                        @CreatedBy, @CreatedOn, @CreatedFrom
                                    );
                                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ProductId", productStockVM.ProductId);
                            cmd.Parameters.AddWithValue("@BranchId", productStockVM.BranchId);
                            cmd.Parameters.AddWithValue("@OpeningQuantity", productStockVM.OpeningQuantity ?? 0);
                            cmd.Parameters.AddWithValue("@OpeningValue", productStockVM.OpeningValue ?? 0);
                            cmd.Parameters.AddWithValue("@OpeningDate", productStockVM.OpeningDate);
                            cmd.Parameters.AddWithValue("@CreatedBy", productStockVM.CreatedBy);
                            cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            cmd.Parameters.AddWithValue("@CreatedFrom", productStockVM.CreatedFrom ?? "System");

                    productStockVM.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }


                result.Status = "Success";
                result.Message = "Product stock data inserted successfully.";
                result.Id = productStockVM.Id.ToString();
                result.DataVM = productStockVM;

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



        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId, SqlConnection conn = null, SqlTransaction transaction = null)
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

                var data = new GridEntity<ProductStockVM>();

                string sqlQuery = @"
          SELECT COUNT(DISTINCT H.Id) AS totalcount
            FROM ProductsOpeningStocks H
            LEFT OUTER JOIN Products P on H.ProductId = p.Id
            WHERE 1 = 1
    -- Add the filter condition
    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductStockVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
                {
                    sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
                }
                sqlQuery += @"
                -- Data query with pagination and sorting
    SELECT * 
    FROM (
        SELECT 
            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
            ISNULL(H.Id, 0) AS Id,
    ISNULL(H.ProductId, 0) AS ProductId,
    ISNULL(P.Name, '') AS ProductName,
	ISNULL(P.Code, '') AS ProductCode,
    ISNULL(H.BranchId, 0) AS BranchId,
	ISNULL(H.OpeningQuantity, 0) AS OpeningQuantity,
	ISNULL(H.OpeningValue, 0) AS OpeningValue,
    ISNULL(FORMAT(H.OpeningDate, 'yyyy-MM-dd'), '') AS OpeningDate,
    ISNULL(H.CreatedBy, '') AS CreatedBy,
    ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
    ISNULL(H.CreatedFrom, '') AS CreatedFrom
FROM ProductsOpeningStocks H
LEFT OUTER JOIN Products P on H.ProductId = p.Id
WHERE 1 = 1


        -- Add the filter condition
           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductStockVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
                {

                    //sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
                    sqlQuery += " AND H.ProductId = " + ProductId + " AND H.BranchId = " + options.vm.BranchId;

                }
                sqlQuery += @"

    ) AS a
    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
";


                data = KendoGrid<ProductStockVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

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





        //        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId, SqlConnection conn = null, SqlTransaction transaction = null)
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

        //                var data = new GridEntity<ProductStockVM>();

        //                string sqlQuery = @"
        //    -- Count query
        //    SELECT COUNT(DISTINCT H.Id) AS totalcount
        //    FROM ProductsOpeningStocks H
        //    LEFT OUTER JOIN Products P on H.ProductId = p.Id
        //    WHERE 1 = 1
        //    -- Add the filter condition
        //    " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductStockVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
        //                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
        //                {
        //                    sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
        //                }
        //                sqlQuery += @"
        //                -- Data query with pagination and sorting
        //    SELECT * 
        //    FROM (
        //        SELECT 
        //            ROW_NUMBER() OVER(ORDER BY " + (options.sort.Count > 0 ? options.sort[0].field + " " + options.sort[0].dir : "H.Id DESC ") + @") AS rowindex,
        //        ISNULL(H.Id, 0) AS Id,
        //        ISNULL(H.ProductId, 0) AS ProductId,
        //        ISNULL(P.Name, '') AS ProductName,
        //	    ISNULL(P.Code, '') AS ProductCode,
        //        ISNULL(H.BranchId, 0) AS BranchId,
        //	    ISNULL(H.OpeningQuantity, 0) AS OpeningQuantity,
        //	    ISNULL(H.OpeningValue, 0) AS OpeningValue,
        //        ISNULL(FORMAT(H.OpeningDate, 'yyyy-MM-dd'), '') AS OpeningDate,
        //        ISNULL(H.CreatedBy, '') AS CreatedBy,
        //        ISNULL(FORMAT(H.CreatedOn, 'yyyy-MM-dd HH:mm'), '1900-01-01') AS CreatedOn,
        //        ISNULL(H.CreatedFrom, '') AS CreatedFrom
        //    FROM ProductsOpeningStocks H
        //    LEFT OUTER JOIN Products P on H.ProductId = p.Id
        //    WHERE 1 = 1
        //        -- Add the filter condition
        //           " + (options.filter.Filters.Count > 0 ? " AND (" + GridQueryBuilder<ProductStockVM>.FilterCondition(options.filter) + ")" : "");/* + @"*/
        //                if (!string.IsNullOrEmpty(ProductId) && ProductId != "0")
        //                {

        //                    //sqlQuery += " AND H.ProductId = " + ProductId; // Directly insert salePersonId in the query
        //                    sqlQuery += " AND H.ProductId = " + ProductId + " AND H.BranchId = " + options.vm.BranchId;

        //                }
        //                sqlQuery += @"

        //    ) AS a
        //    WHERE rowindex > @skip AND (@take = 0 OR rowindex <= @take)
        //";


        //                data = KendoGrid<ProductStockVM>.GetGridData_CMD(options, sqlQuery, "H.Id");

        //                result.Status = "Success";
        //                result.Message = "Data retrieved successfully.";
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









    }
}
