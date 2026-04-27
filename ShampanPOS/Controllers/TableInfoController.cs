using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableInfoController : Controller
    {
        TableInfoService _TableInfoService = new TableInfoService();
        CommonService _common = new CommonService();

        // POST: api/TableInfo/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(TableInfoVM bankinfo)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            TableInfoService _TableInfoService = new TableInfoService();

            try
            {
                resultVM = await _TableInfoService.Insert(bankinfo);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = bankinfo
                };
            }
        }

        // POST: api/TableInfo/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(TableInfoVM bankinfo)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                TableInfoService _TableInfoService = new TableInfoService();
                resultVM = await _TableInfoService.Update(bankinfo);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = bankinfo
                };
            }
        }

        // POST: api/TableInfo/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _TableInfoService = new TableInfoService();

                resultVM = await _TableInfoService.Delete(vm);
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

        // POST: api/TableInfo/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {


                string[] conditionFields = null;
                string[] conditionValues = null;
                conditionFields = vm.ConditionalFields;
                conditionValues = vm.ConditionalValues;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { vm.Id };
                }
                TableInfoService _TableInfoService = new TableInfoService();


                resultVM = await _TableInfoService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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


        // POST: api/TableInfo/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _TableInfoService = new TableInfoService();
                resultVM = await _TableInfoService.GetGridData(options, null, null);
                //resultVM = await _CustomerService.GetGridData(options);
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
