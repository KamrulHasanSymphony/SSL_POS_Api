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
    public class ProductUOMConversionController : ControllerBase
    {
        ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();
        // POST: api/ProductUOMConversion/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductUOMFactorsVM productUOMFactor)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();

            try
            {
                resultVM = await _productUOMFactorsService.Insert(productUOMFactor);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = productUOMFactor
                };
            }
        }

        // POST: api/ProductUOMConversion/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductUOMFactorsVM productUOMFactors)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();
                resultVM = await _productUOMFactorsService.Update(productUOMFactors);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = productUOMFactors
                };
            }
        }

        // POST: api/ProductUOMConversion/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();

                resultVM = await _productUOMFactorsService.Delete(vm);
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

        // POST: api/ProductUOMConversion/List
        [HttpPost("List")]
        public async Task<ResultVM> List(ProductUOMFactorsVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();
                resultVM = await _productUOMFactorsService.List(new[] { "H.Id" }, new[] { productBatchHistory.Id.ToString() }, null);
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

        // GET: api/ProductUOMConversion/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(ProductUOMFactorsVM productBatchHistory)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();
                resultVM = await _productUOMFactorsService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/ProductUOMConversion/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductUOMFactorsService _productUOMFactorsService = new ProductUOMFactorsService();
                resultVM = await _productUOMFactorsService.Dropdown(); 
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

        // POST: api/ProductUOMConversion/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string ProductId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productUOMFactorsService = new ProductUOMFactorsService();
                resultVM = await _productUOMFactorsService.GetGridData(options,ProductId);
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
