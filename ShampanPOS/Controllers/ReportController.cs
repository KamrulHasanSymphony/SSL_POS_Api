using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        [HttpPost("GetSupplierReport")]
        public async Task<ResultVM> GetSupplierReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                SupplierService _service = new SupplierService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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

        [HttpPost("GetCustomerReport")]
        public async Task<ResultVM> GetCustomerReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                CustomerService _service = new CustomerService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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

        [HttpPost("GetPurchaseReport")]
        public async Task<ResultVM> GetPurchaseReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                PurchaseService _service = new PurchaseService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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

        [HttpPost("GetSaleReport")]
        public async Task<ResultVM> GetSaleReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                SaleService _service = new SaleService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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

        [HttpPost("GetSaleOrderReport")]
        public async Task<ResultVM> GetSaleOrderReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                SaleOrderService _service = new SaleOrderService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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

        [HttpPost("GetSaleReturnReport")]
        public async Task<ResultVM> GetSaleReturnReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                SaleReturnService _service = new SaleReturnService();
                resultVM = await _service.List(conditionFields, conditionValues, null);
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
