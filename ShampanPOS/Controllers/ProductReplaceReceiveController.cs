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
    public class ProductReplaceReceiveController : ControllerBase
    {
        private readonly ProductReplaceReceiveService _service;

        public ProductReplaceReceiveController()
        {
            _service = new ProductReplaceReceiveService();
        }

        // POST: api/ProductReplaceReceive/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductReplaceReceiveVM model)
        {
            try
            {
                return await _service.Insert(model);
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }

        // POST: api/ProductReplaceReceive/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductReplaceReceiveVM model)
        {
            try
            {
                return await _service.Update(model);
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }

        // POST: api/ProductReplaceReceive/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete([FromBody] int[] ids)
        {
            try
            {
                return await _service.Delete(ids);
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/ProductReplaceReceive/List
        [HttpPost("List")]
        public async Task<ResultVM> List([FromBody] CommonVM vm)
        {
            try
            {
                return await _service.List(new[] { "M.Id" }, new[] { vm.Id.ToString() });
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // GET: api/ProductReplaceReceive/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable([FromQuery] ProductReplaceReceiveVM saleOrder)
        {
            try
            {
                return await _service.ListAsDataTable(new[] { "" }, new[] { "" });
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


      
        // POST: api/ProductReplaceReceive/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData([FromBody] GridOptions options)
        {
            try
            {
                var conditionFields = new[] { "H.BranchId"};
                var conditionValues = new[] { options.vm.BranchId.ToString() };



                return await _service.GetGridData(options, conditionFields, conditionValues);
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

        // POST: api/ProductReplaceReceive/GetDetailsGridData
        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData([FromBody] GridOptions options)
        {
            try
            {
                var conditionFields = new[] { "H.BranchId", "H.InvoiceDateTime between", "H.InvoiceDateTime between" };
                var conditionValues = new[] { options.vm.BranchId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() };

                return await _service.GetDetailsGridData(options, conditionFields, conditionValues);
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

        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {

                resultVM = await _service.MultiplePost(vm);
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


        // POST: api/ProductReplaceReceive/ProductReplaceReceiveList
        [HttpPost("ProductReplaceReceiveList")]
        public async Task<ResultVM> ProductReplaceReceiveList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PeramModel model = new PeramModel();
                model.SearchValue = vm.Value;
                resultVM = await _service.ProductReplaceReceiveList(new[] { "M.Id" }, new[] { vm.Id }, model);
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

        // POST: api/Purchase/ReplaceReceiveList
        [HttpPost("ReplaceReceiveList")]
        public async Task<ResultVM> ReplaceReceiveList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                resultVM = await _service.ReplaceReceiveList(vm.IDs);
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


        // POST: api/ProductReplaceReceive/GetProductReplaceReceiveDetailDataById
        [HttpPost("GetProductReplaceReceiveDetailDataById")]
        public async Task<ResultVM> GetProductReplaceReceiveDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                resultVM = await _service.GetProductReplaceReceiveDetailDataById(options, masterId);
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
