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
    public class CustomerAdvanceController : ControllerBase
    {
        // POST: api/CustomerAdvance/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CustomerAdvanceVM customerAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();

            try
            {
                resultVM = await _customerAdvanceService.Insert(customerAdvance);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = customerAdvance
                };
            }
        }

        // POST: api/CustomerAdvance/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CustomerAdvanceVM customerAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();
                resultVM = await _customerAdvanceService.Update(customerAdvance);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = customerAdvance
                };
            }
        }

        // POST: api/CustomerAdvance/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CustomerAdvanceVM customerAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();

                string?[] IDs = null;
                IDs = new string?[] { customerAdvance.Id.ToString() };

                resultVM = await _customerAdvanceService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = customerAdvance
                };
            }
        }

        // POST: api/CustomerAdvance/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM customerAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();
                resultVM = await _customerAdvanceService.List(new[] { "M.Id" }, new[] { customerAdvance.Id.ToString() }, null);
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

        // GET: api/CustomerAdvance/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CustomerAdvanceVM customerAdvance)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();
                resultVM = await _customerAdvanceService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/CustomerAdvance/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();
                resultVM = await _customerAdvanceService.Dropdown(); // Adjust if Dropdown requires a different method
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



        // POST: api/CustomerAdvance/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string CustomerId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();
                resultVM = await _customerAdvanceService.GetGridData(options, CustomerId);
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
                CustomerAdvanceService _customerAdvanceService = new CustomerAdvanceService();


                resultVM = await _customerAdvanceService.MultiplePost(vm);
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
