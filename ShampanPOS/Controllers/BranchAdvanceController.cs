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
    public class BranchAdvanceController : ControllerBase
    {
        // POST: api/BranchAdvance/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(BranchAdvanceVM branchAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();

            try
            {
                resultVM = await _branchAdvanceService.Insert(branchAdvance);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = branchAdvance
                };
            }
        }

        // POST: api/BranchAdvance/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(BranchAdvanceVM branchAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();
                resultVM = await _branchAdvanceService.Update(branchAdvance);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = branchAdvance
                };
            }
        }

        // POST: api/BranchAdvance/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(BranchAdvanceVM branchAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();

                string?[] IDs = null;
                IDs = new string?[] { branchAdvance.Id.ToString() };

                resultVM = await _branchAdvanceService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = branchAdvance
                };
            }
        }

        // POST: api/BranchAdvance/List
        [HttpPost("List")]
        public async Task<ResultVM> List(BranchAdvanceVM branchAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();
                resultVM = await _branchAdvanceService.List(new[] { "M.Id" }, new[] { branchAdvance.Id.ToString() }, null);
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

        // GET: api/BranchAdvance/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(BranchAdvanceVM branchAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();
                resultVM = await _branchAdvanceService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/BranchAdvance/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();
                resultVM = await _branchAdvanceService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/BranchAdvance/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string BranchId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();             
                resultVM = await _branchAdvanceService.GetGridData(options, BranchId);
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
                
                BranchAdvanceService _branchAdvanceService = new BranchAdvanceService();

                resultVM = await _branchAdvanceService.MultiplePost(vm);
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
