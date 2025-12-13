using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using System;
using System.Threading.Tasks;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPurchasePriceBatchHistorieController : ControllerBase
    {
        ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
        // POST: api/ProductBatchHistory/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductBatchHistoryVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();

            try
            {
                resultVM = await _productPurchasePriceBatchHistorieService.Insert(productBatchHistory);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = productBatchHistory
                };
            }
        }

        // POST: api/ProductBatchHistory/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductBatchHistoryVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                resultVM = await _productPurchasePriceBatchHistorieService.Update(productBatchHistory);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = productBatchHistory
                };
            }
        }

        // POST: api/ProductBatchHistory/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                //ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                //string?[] IDs = null;
                //IDs = new string?[] { productBatchHistory.Id.ToString() };

                resultVM = await _productPurchasePriceBatchHistorieService.Delete(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/ProductBatchHistory/List
        [HttpPost("List")]
        public async Task<ResultVM> List(ProductBatchHistoryVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                resultVM = await _productPurchasePriceBatchHistorieService.List(new[] { "H.Id" }, new[] { productBatchHistory.Id.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/ProductBatchHistory/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(ProductBatchHistoryVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                resultVM = await _productPurchasePriceBatchHistorieService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // GET: api/ProductBatchHistory/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                resultVM = await _productPurchasePriceBatchHistorieService.Dropdown(); // Adjust if Dropdown requires a different method
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/ProductBatchHistory/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
                resultVM = await _productPurchasePriceBatchHistorieService.GetGridData(options,ProductId);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        //// POST: api/ProductBatchHistory/GetProductByProductId
        //[HttpPost("GetProductByProductId")]
        //public async Task<ResultVM> GetProductByProductId(int ProductId)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        ProductPurchasePriceBatchHistorieService _productPurchasePriceBatchHistorieService = new ProductPurchasePriceBatchHistorieService();
        //        resultVM = await _productPurchasePriceBatchHistorieService.GetProductByProductId(ProductId);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not fetched.",
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}

    }
}
