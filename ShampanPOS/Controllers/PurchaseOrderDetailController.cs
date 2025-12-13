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
    public class PurchaseOrderDetailController : ControllerBase
    {
        // POST: api/PurchaseOrderDetail/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(PurchaseOrderDetailVM purchaseOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();

            try
            {
                resultVM = await _purchaseOrderDetailService.Insert(purchaseOrderDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = purchaseOrderDetail
                };
            }
        }

        // POST: api/PurchaseOrderDetail/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(PurchaseOrderDetailVM purchaseOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();
                resultVM = await _purchaseOrderDetailService.Update(purchaseOrderDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = purchaseOrderDetail
                };
            }
        }

        // POST: api/PurchaseOrderDetail/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(PurchaseOrderDetailVM purchaseOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();

                string?[] IDs = null;
                IDs = new string?[] { purchaseOrderDetail.Id.ToString() };

                resultVM = await _purchaseOrderDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = purchaseOrderDetail
                };
            }
        }

        // POST: api/PurchaseOrderDetail/List
        [HttpPost("List")]
        public async Task<ResultVM> List(PurchaseOrderDetailVM purchaseOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();
                resultVM = await _purchaseOrderDetailService.List(new[] { "M.Id" }, new[] { purchaseOrderDetail.Id.ToString() }, null);
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

        // GET: api/PurchaseOrderDetail/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(PurchaseOrderDetailVM purchaseOrderDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();
                resultVM = await _purchaseOrderDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/PurchaseOrderDetail/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                PurchaseOrderDetailService _purchaseOrderDetailService = new PurchaseOrderDetailService();
                resultVM = await _purchaseOrderDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
