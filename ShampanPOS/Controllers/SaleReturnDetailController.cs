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
    public class SaleReturnDetailController : ControllerBase
    {
        // POST: api/SaleReturnDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleReturnDetailVM saleReturnDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();

            try
            {
                resultVM = await _saleReturnDetailService.Insert(saleReturnDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = saleReturnDetail
                };
            }
        }

        // POST: api/SaleReturnDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleReturnDetailVM saleReturnDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();
                resultVM = await _saleReturnDetailService.Update(saleReturnDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = saleReturnDetail
                };
            }
        }

        // POST: api/SaleReturnDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SaleReturnDetailVM saleReturnDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();

                string?[] IDs = null;
                IDs = new string?[] { saleReturnDetail.Id.ToString() };

                resultVM = await _saleReturnDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = saleReturnDetail
                };
            }
        }

        // POST: api/SaleReturnDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(SaleReturnDetailVM saleReturnDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();
                resultVM = await _saleReturnDetailService.List(new[] { "M.Id" }, new[] { saleReturnDetail.Id.ToString() }, null);
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

        // GET: api/SaleReturnDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleReturnDetailVM saleReturnDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();
                resultVM = await _saleReturnDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SaleReturnDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleReturnDetailService _saleReturnDetailService = new SaleReturnDetailService();
                resultVM = await _saleReturnDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
