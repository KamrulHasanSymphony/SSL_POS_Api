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
    public class SalePersonYearlyTargetController : ControllerBase
    {
        // POST: api/SalePersonYearlyTarget/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SalePersonYearlyTargetVM salePersonYearlyTarget)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();

            try
            {
                resultVM = await _salePersonYearlyTargetService.Insert(salePersonYearlyTarget);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTarget
                };
            }
        }

        // POST: api/SalePersonYearlyTarget/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SalePersonYearlyTargetVM salePersonYearlyTarget)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();
                resultVM = await _salePersonYearlyTargetService.Update(salePersonYearlyTarget);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTarget
                };
            }
        }

        // POST: api/SalePersonYearlyTarget/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SalePersonYearlyTargetVM salePersonYearlyTarget)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();

                string?[] IDs = null;
                IDs = new string?[] { salePersonYearlyTarget.Id.ToString() };

                resultVM = await _salePersonYearlyTargetService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTarget
                };
            }
        }

        // POST: api/SaleDeliveryReturn/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {                
                 SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();

                resultVM = await _salePersonYearlyTargetService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
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

        // GET: api/SalePersonYearlyTarget/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SalePersonYearlyTargetVM salePersonYearlyTarget)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();
                resultVM = await _salePersonYearlyTargetService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SalePersonYearlyTarget/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();
                resultVM = await _salePersonYearlyTargetService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/SalePersonYearlyTarget/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();
                resultVM = await _salePersonYearlyTargetService.GetGridData(options);
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

        // POST: api/SaleDelivery/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SalePersonYearlyTargetService _salePersonYearlyTargetService = new SalePersonYearlyTargetService();

                resultVM = await _salePersonYearlyTargetService.MultiplePost(vm);
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

    }
}
