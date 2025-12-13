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
    public class SaleOrderDetailController : ControllerBase
    {
        // POST: api/SaleOrderDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleOrderDetailVM saleOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();

            try
            {
                resultVM = await _saleOrderDetailService.Insert(saleOrderDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = saleOrderDetail
                };
            }
        }

        // POST: api/SaleOrderDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleOrderDetailVM saleOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();
                resultVM = await _saleOrderDetailService.Update(saleOrderDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = saleOrderDetail
                };
            }
        }

        // POST: api/SaleOrderDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SaleOrderDetailVM saleOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();

                string?[] IDs = null;
                IDs = new string?[] { saleOrderDetail.Id.ToString() };

                resultVM = await _saleOrderDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = saleOrderDetail
                };
            }
        }

        // POST: api/SaleOrderDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(SaleOrderDetailVM saleOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();
                resultVM = await _saleOrderDetailService.List(new[] { "M.Id" }, new[] { saleOrderDetail.Id.ToString() }, null);
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

        // GET: api/SaleOrderDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleOrderDetailVM saleOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();
                resultVM = await _saleOrderDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SaleOrderDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SaleOrderDetailService _saleOrderDetailService = new SaleOrderDetailService();
                resultVM = await _saleOrderDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
