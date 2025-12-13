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
    public class PurchaseDetailController : ControllerBase
    {
        // POST: api/PurchaseDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(PurchaseDetailVM purchaseDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();

            try
            {
                resultVM = await _purchaseDetailService.Insert(purchaseDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = purchaseDetail
                };
            }
        }

        // POST: api/PurchaseDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(PurchaseDetailVM purchaseDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();
                resultVM = await _purchaseDetailService.Update(purchaseDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = purchaseDetail
                };
            }
        }

        // POST: api/PurchaseDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(PurchaseDetailVM purchaseDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();

                string?[] IDs = null;
                IDs = new string?[] { purchaseDetail.Id.ToString() };

                resultVM = await _purchaseDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = purchaseDetail
                };
            }
        }

        // POST: api/PurchaseDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(PurchaseDetailVM purchaseDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();
                resultVM = await _purchaseDetailService.List(new[] { "M.Id" }, new[] { purchaseDetail.Id.ToString() }, null);
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

        // GET: api/PurchaseDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(PurchaseDetailVM purchaseDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();
                resultVM = await _purchaseDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/PurchaseDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseDetailService _purchaseDetailService = new PurchaseDetailService();
                resultVM = await _purchaseDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
