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
    public class SupplierGroupController : ControllerBase
    {
        // POST: api/SupplierGroup/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SupplierGroupVM supplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SupplierGroupService _supplierGroupService = new SupplierGroupService();

            try
            {
                resultVM = await _supplierGroupService.Insert(supplierGroup);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = supplierGroup
                };
            }
        }

        // POST: api/SupplierGroup/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SupplierGroupVM supplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SupplierGroupService _supplierGroupService = new SupplierGroupService();
                resultVM = await _supplierGroupService.Update(supplierGroup);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = supplierGroup
                };
            }
        }

        
        // POST: api/ProductGroup/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {

                SupplierGroupService _supplierGroupService = new SupplierGroupService();               
                resultVM = await _supplierGroupService.Delete(vm);
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

        // POST: api/SupplierGroup/List
        [HttpPost("List")]
        public async Task<ResultVM> List(SupplierGroupVM supplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SupplierGroupService _supplierGroupService = new SupplierGroupService();
                resultVM = await _supplierGroupService.List(new[] { "M.Id" }, new[] { supplierGroup.Id.ToString() }, null);
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

        // GET: api/SupplierGroup/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SupplierGroupVM supplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SupplierGroupService _supplierGroupService = new SupplierGroupService();
                resultVM = await _supplierGroupService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SupplierGroup/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SupplierGroupService _supplierGroupService = new SupplierGroupService();
                resultVM = await _supplierGroupService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/SupplierGroup/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
               
                SupplierGroupService _supplierGroupService = new SupplierGroupService();
                resultVM = await _supplierGroupService.GetGridData(options);
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
