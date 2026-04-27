using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;

namespace ShampanPOS
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableSectionController : Controller
    {
        TableSectionService _TableSectionService = new TableSectionService();
        CommonService _common = new CommonService();

        // POST: api/TableSection/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(TableSectionVM tablesection)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            TableSectionService _TableSectionService = new TableSectionService();

            try
            {
                resultVM = await _TableSectionService.Insert(tablesection);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = tablesection
                };
            }
        }

        // POST: api/TableSection/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(TableSectionVM tablesection)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                TableSectionService _TableSectionService = new TableSectionService();
                resultVM = await _TableSectionService.Update(tablesection);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = tablesection
                };
            }
        }

        // POST: api/TableSection/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _TableSectionService = new TableSectionService();

                resultVM = await _TableSectionService.Delete(vm);
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

        // POST: api/TableSection/List
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
                TableSectionService _TableSectionService = new TableSectionService();


                resultVM = await _TableSectionService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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


        // POST: api/TableSection/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _TableSectionService = new TableSectionService();
                resultVM = await _TableSectionService.GetGridData(options, null, null);
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
