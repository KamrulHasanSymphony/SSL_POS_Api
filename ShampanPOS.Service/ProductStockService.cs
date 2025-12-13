using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class ProductStockService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(List<ProductStockVM> productStockVM)
        {
      

            CommonRepository _commonRepo = new CommonRepository();
            ProductStockRepository _repo = new ProductStockRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {

                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();


                #region Check Exist Data

                //string[] conditionField = { "Name" };
                //string[] conditionValue = { productPriceGroup.Name.ToString() };
                //bool exist = _commonRepo.CheckExists("ProductPriceGroups", conditionField, conditionValue, conn, transaction);
                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}

                #endregion
                foreach (var productStock in productStockVM)
                {

                    string[] conditionField = { "ProductId", "BranchId" };
                    string[] conditionValue = { productStock.ProductId.ToString() };
                    bool exist = _commonRepo.CheckExists("ProductsOpeningStocks", conditionField, conditionValue, conn, transaction);
                    if (exist)
                    {
                        var record = _commonRepo.DetailsDelete("ProductsOpeningStocks", new[] { "ProductId" , "BranchId" }, new[] { productStock.ProductId.ToString(), productStock.BranchId.ToString() }, conn, transaction);
                    }



                    result = await _repo.Insert(productStock, conn, transaction);
                }



                if (result.Status == "Success" && isNewConnection)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                //result.Message = "Error in inserting fiscal year.";
                result.Message = result.Message;
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



        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId)
        {
            ProductStockRepository _repo = new ProductStockRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetGridData(options, ProductId, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
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
