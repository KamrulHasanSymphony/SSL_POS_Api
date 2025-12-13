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
    public class FiscalYearForSaleController : ControllerBase
    {
        FiscalYearForSaleService _fiscalYearForSaleService = new FiscalYearForSaleService();
        // POST: api/FiscalYearForSale/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(FiscalYearForSaleVM fiscalYearForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _fiscalYearForSaleService = new FiscalYearForSaleService();

            try
            {
                resultVM = await _fiscalYearForSaleService.Insert(fiscalYearForSale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearForSale
                };
            }
        }

        // POST: api/FiscalYearForSale/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(FiscalYearForSaleVM fiscalYearForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                 _fiscalYearForSaleService = new FiscalYearForSaleService();
                resultVM = await _fiscalYearForSaleService.Update(fiscalYearForSale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearForSale
                };
            }
        }

        // POST: api/FiscalYearForSale/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _fiscalYearForSaleService = new FiscalYearForSaleService();

                resultVM = await _fiscalYearForSaleService.Delete(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/FiscalYearForSale/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
             _fiscalYearForSaleService = new FiscalYearForSaleService();
                resultVM = await _fiscalYearForSaleService.List(new[] { "M.Id" }, new[] { vm.Id}, null);
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

        // GET: api/FiscalYearForSale/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(FiscalYearForSaleVM fiscalYearForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _fiscalYearForSaleService = new FiscalYearForSaleService();
                resultVM = await _fiscalYearForSaleService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/FiscalYearForSale/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _fiscalYearForSaleService = new FiscalYearForSaleService();
                resultVM = await _fiscalYearForSaleService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/FiscalYearForSale/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _fiscalYearForSaleService = new FiscalYearForSaleService();
                resultVM = await _fiscalYearForSaleService.GetGridData(options);
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
