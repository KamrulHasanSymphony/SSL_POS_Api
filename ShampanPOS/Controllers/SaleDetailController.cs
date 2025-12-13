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
    public class SaleDetailController : ControllerBase
    {
        // POST: api/SaleDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleDetailVM saleDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SaleDetailService _saleDetailService = new SaleDetailService();

            try
            {
                resultVM = await _saleDetailService.Insert(saleDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = saleDetail
                };
            }
        }

        // POST: api/SaleDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleDetailVM saleDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleDetailService _saleDetailService = new SaleDetailService();
                resultVM = await _saleDetailService.Update(saleDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = saleDetail
                };
            }
        }

        // POST: api/SaleDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SaleDetailVM saleDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SaleDetailService _saleDetailService = new SaleDetailService();

                string?[] IDs = null;
                IDs = new string?[] { saleDetail.Id.ToString() };

                resultVM = await _saleDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = saleDetail
                };
            }
        }

        // POST: api/SaleDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(SaleDetailVM saleDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleDetailService _saleDetailService = new SaleDetailService();
                resultVM = await _saleDetailService.List(new[] { "M.Id" }, new[] { saleDetail.Id.ToString() }, null);
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

        // GET: api/SaleDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleDetailVM saleDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleDetailService _saleDetailService = new SaleDetailService();
                resultVM = await _saleDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SaleDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleDetailService _saleDetailService = new SaleDetailService();
                resultVM = await _saleDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
