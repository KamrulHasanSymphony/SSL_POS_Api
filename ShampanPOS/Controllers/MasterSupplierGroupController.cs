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
    public class MasterSupplierGroupController : ControllerBase
    {
        // POST: api/SupplierGroup/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(MasterSupplierGroupVM masterSupplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            MasterSupplierGroupService _service = new MasterSupplierGroupService();

            try
            {
                resultVM = await _service.Insert(masterSupplierGroup);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = masterSupplierGroup
                };
            }
        }

        // POST: api/SupplierGroup/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(MasterSupplierGroupVM masterSupplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.Update(masterSupplierGroup);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = masterSupplierGroup
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

                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.Delete(vm);
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
        public async Task<ResultVM> List(MasterSupplierGroupVM masterSupplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.List(new[] { "M.Id" }, new[] { masterSupplierGroup.Id.ToString() }, null);
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
        public async Task<ResultVM> ListAsDataTable(MasterSupplierGroupVM masterSupplierGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.ListAsDataTable(new[] { "" }, new[] { "" });
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
                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.Dropdown(); 
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

                MasterSupplierGroupService _service = new MasterSupplierGroupService();
                resultVM = await _service.GetGridData(options);
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
