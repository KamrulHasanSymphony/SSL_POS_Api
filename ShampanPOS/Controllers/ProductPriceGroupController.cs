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
    public class ProductPriceGroupController : ControllerBase
    {
        ProductPriceGroupService _productPriceGroupService = new ProductPriceGroupService();
        // POST: api/ProductPriceGroup/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductPriceGroupVM productPriceGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _productPriceGroupService = new ProductPriceGroupService();

            try
            {
                resultVM = await _productPriceGroupService.Insert(productPriceGroup);
                return resultVM;
            }

            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = productPriceGroup
                };
            }

        }

        // POST: api/ProductPriceGroup/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductPriceGroupVM ProductPriceGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPriceGroupService _productPriceGroupService = new ProductPriceGroupService();
                resultVM = await _productPriceGroupService.Update(ProductPriceGroup);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = ProductPriceGroup
                };
            }
        }

       
        // POST: api/ProductPriceGroup/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPriceGroupService _productPriceGroupService = new ProductPriceGroupService();
                resultVM = await _productPriceGroupService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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


        // POST: api/SaleDelivery/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductPriceGroupService _productPriceGroupService = new ProductPriceGroupService();
                resultVM = await _productPriceGroupService.GetGridData(options);
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
