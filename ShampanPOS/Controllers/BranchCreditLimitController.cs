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
    public class BranchCreditLimitController : ControllerBase
    {
        // POST: api/BranchCreditLimit/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(BranchCreditLimitVM branchCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();

            try
            {
                resultVM = await _branchCreditLimitService.Insert(branchCreditLimit);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = branchCreditLimit
                };
            }
        }

        // POST: api/BranchCreditLimit/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(BranchCreditLimitVM branchCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();
                resultVM = await _branchCreditLimitService.Update(branchCreditLimit);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = branchCreditLimit
                };
            }
        }

        // POST: api/BranchCreditLimit/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(BranchCreditLimitVM branchCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();

                string?[] IDs = null;
                IDs = new string?[] { branchCreditLimit.Id.ToString() };

                resultVM = await _branchCreditLimitService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = branchCreditLimit
                };
            }
        }

        // POST: api/BranchCreditLimit/List
        [HttpPost("List")]
        public async Task<ResultVM> List(BranchCreditLimitVM branchCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();
                resultVM = await _branchCreditLimitService.List(new[] { "M.Id" }, new[] { branchCreditLimit.Id.ToString() }, null);
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

        // GET: api/BranchCreditLimit/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(BranchCreditLimitVM branchCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();
                resultVM = await _branchCreditLimitService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/BranchCreditLimit/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();
                resultVM = await _branchCreditLimitService.Dropdown(); // Adjust if Dropdown requires a different method
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



        // POST: api/Product/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options,string BranchId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();

                resultVM = await _branchCreditLimitService.GetGridData(options,BranchId);
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


        // POST: api/Sale/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
               
                BranchCreditLimitService _branchCreditLimitService = new BranchCreditLimitService();

                resultVM = await _branchCreditLimitService.MultiplePost(vm);
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
