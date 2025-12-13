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
    public class ProductReplaceIssueController : ControllerBase
    {
        private readonly ProductReplaceIssueService _service;

        public ProductReplaceIssueController()
        {
            _service = new ProductReplaceIssueService();
        }

        // POST: api/ProductReplaceIssue/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductReplaceIssueVM model)
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

        // POST: api/ProductReplaceIssue/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductReplaceIssueVM model)
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

        // POST: api/ProductReplaceIssue/Delete
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

        // POST: api/ProductReplaceIssue/List
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


        // POST: api/ProductReplaceIssue/GetGridData
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

        // POST: api/ProductReplaceIssue/GetDetailsGridData
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

       
        [HttpPost("FromProductReplaceReceiveGridData")]
        public async Task<ResultVM> FromProductReplaceReceiveGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {

                resultVM = await _service.FromProductReplaceReceiveGridData(options);
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


        // POST: api/ProductReplaceIssue/GetProductReplaceIssueDetailDataById
        [HttpPost("GetProductReplaceIssueDetailDataById")]
        public async Task<ResultVM> GetProductReplaceIssueDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {

                resultVM = await _service.GetProductReplaceIssueDetailDataById(options, masterId);

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
