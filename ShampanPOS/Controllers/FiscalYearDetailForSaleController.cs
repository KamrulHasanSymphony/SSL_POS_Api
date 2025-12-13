using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using System;
using System.Threading.Tasks;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiscalYearDetailForSaleController : ControllerBase
    {
        // POST: api/FiscalYearDetailForSale/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(FiscalYearDetailForSaleVM fiscalYearDetailForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();

            try
            {
                resultVM = await _fiscalYearDetailForSaleService.Insert(fiscalYearDetailForSale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetailForSale
                };
            }
        }

        // POST: api/FiscalYearDetailForSale/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(FiscalYearDetailForSaleVM fiscalYearDetailForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();
                resultVM = await _fiscalYearDetailForSaleService.Update(fiscalYearDetailForSale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetailForSale
                };
            }
        }

        // POST: api/FiscalYearDetailForSale/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(FiscalYearDetailForSaleVM fiscalYearDetailForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();

                string?[] IDs = null;
                IDs = new string?[] { fiscalYearDetailForSale.Id.ToString() };

                resultVM = await _fiscalYearDetailForSaleService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = fiscalYearDetailForSale
                };
            }
        }

        // POST: api/FiscalYearDetailForSale/List
        [HttpPost("List")]
        public async Task<ResultVM> List(FiscalYearDetailForSaleVM fiscalYearDetailForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();
                resultVM = await _fiscalYearDetailForSaleService.List(new[] { "M.Id" }, new[] { fiscalYearDetailForSale.Id.ToString() }, null);
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

        // GET: api/FiscalYearDetailForSale/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(FiscalYearDetailForSaleVM fiscalYearDetailForSale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();
                resultVM = await _fiscalYearDetailForSaleService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/FiscalYearDetailForSale/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                FiscalYearDetailForSaleService _fiscalYearDetailForSaleService = new FiscalYearDetailForSaleService();
                resultVM = await _fiscalYearDetailForSaleService.Dropdown(); // Adjust if Dropdown requires a different method
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
    }
}
