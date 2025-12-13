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
    public class ProductBatchHistoryController : ControllerBase
    {
        ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
        // POST: api/ProductBatchHistory/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductBatchHistoryVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();

            try
            {
                resultVM = await _productBatchHistoryService.Insert(productBatchHistory);
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
                ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.Update(productBatchHistory);
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
                //ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
                _productBatchHistoryService = new ProductBatchHistoryService();
                //string?[] IDs = null;
                //IDs = new string?[] { productBatchHistory.Id.ToString() };

                resultVM = await _productBatchHistoryService.Delete(vm);
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
                ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.List(new[] { "H.Id" }, new[] { productBatchHistory.Id.ToString() }, null);
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
                ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.ListAsDataTable(new[] { "" }, new[] { "" });
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
                ProductBatchHistoryService _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.Dropdown(); // Adjust if Dropdown requires a different method
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
                _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.GetGridData(options,ProductId);
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


        // POST: api/ProductBatchHistory/GetProductBatchHistoryById
        [HttpPost("GetProductBatchHistoryById")]
        public async Task<ResultVM> GetProductBatchHistoryById(GridOptions options, int productId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productBatchHistoryService = new ProductBatchHistoryService();
                resultVM = await _productBatchHistoryService.GetProductBatchHistoryById(options, productId);
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


    }
}
