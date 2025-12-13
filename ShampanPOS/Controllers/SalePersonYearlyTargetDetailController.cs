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
    public class SalePersonYearlyTargetDetailController : ControllerBase
    {
        // POST: api/SalePersonYearlyTargetDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();

            try
            {
                resultVM = await _salePersonYearlyTargetDetailService.Insert(salePersonYearlyTargetDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTargetDetail
                };
            }
        }

        // POST: api/SalePersonYearlyTargetDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();
                resultVM = await _salePersonYearlyTargetDetailService.Update(salePersonYearlyTargetDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTargetDetail
                };
            }
        }

        // POST: api/SalePersonYearlyTargetDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();

                string?[] IDs = null;
                IDs = new string?[] { salePersonYearlyTargetDetail.Id.ToString() };

                resultVM = await _salePersonYearlyTargetDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = salePersonYearlyTargetDetail
                };
            }
        }

        // POST: api/SalePersonYearlyTargetDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();
                resultVM = await _salePersonYearlyTargetDetailService.List(new[] { "M.Id" }, new[] { salePersonYearlyTargetDetail.Id.ToString() }, null);
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

        // GET: api/SalePersonYearlyTargetDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();
                resultVM = await _salePersonYearlyTargetDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SalePersonYearlyTargetDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonYearlyTargetDetailService _salePersonYearlyTargetDetailService = new SalePersonYearlyTargetDetailService();
                resultVM = await _salePersonYearlyTargetDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
