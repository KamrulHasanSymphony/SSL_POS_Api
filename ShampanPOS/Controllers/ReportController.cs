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

        [HttpPost("GetCustomerByCategory")]
        public async Task<ResultVM> GetCustomerByCategory(CustomerVM customer)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                // Customer Group
                if (customer.Id != null && customer.Id != 0)
                {
                    conditionFields.Add("M.CustomerGroupId");
                    conditionValues.Add(customer.Id.ToString());
                }

                // Customer Name
                if (!string.IsNullOrEmpty(customer.Name))
                {
                    conditionFields.Add("M.Name");
                    conditionValues.Add(customer.Name);
                }

                // Customer Code
                if (!string.IsNullOrEmpty(customer.Code))
                {
                    conditionFields.Add("M.Code");
                    conditionValues.Add(customer.Code);
                }

                CustomerService _service = new CustomerService();

                if (conditionFields.Count == 0)
                {
                    resultVM = await _service.ReportList(null, null, null);
                }
                else
                {
                    resultVM = await _service.ReportList(
                        conditionFields.ToArray(),
                        conditionValues.ToArray(),
                        null
                    );
                }

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message
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

        [HttpPost("GetPurchaseOrderReport")]
        public async Task<ResultVM> GetPurchaseOrderReport(CommonVM Vm)
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

                PurchaseOrderService _service = new PurchaseOrderService();
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

        [HttpPost("GetPurchaseReturnReport")]
        public async Task<ResultVM> GetPurchaseReturnReport(CommonVM Vm)
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

                PurchaseReturnService _service = new PurchaseReturnService();
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

        [HttpPost("GetProductByCategory")]
        public async Task<ResultVM> GetProductByCategory(ProductVM product)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                // Customer Group
                if (product.Id != null && product.Id != 0)
                {
                    conditionFields.Add("M.ProductGroupId");
                    conditionValues.Add(product.Id.ToString());
                }

                // Customer Name
                if (!string.IsNullOrEmpty(product.Name))
                {
                    conditionFields.Add("M.Name");
                    conditionValues.Add(product.Name);
                }

                // Customer Code
                if (!string.IsNullOrEmpty(product.Code))
                {
                    conditionFields.Add("M.Code");
                    conditionValues.Add(product.Code);
                }

                ProductService _service = new ProductService();

                if (conditionFields.Count == 0)
                {
                    resultVM = await _service.ReportList(null, null, null);
                }
                else
                {
                    resultVM = await _service.ReportList(
                        conditionFields.ToArray(),
                        conditionValues.ToArray(),
                        null
                    );
                }

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message
                };
            }
        }

        [HttpPost("GetSupplierByCategory")]
        public async Task<ResultVM> GetSupplierByCategory(SupplierVM supplier)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                // Customer Group
                if (supplier.Id != null && supplier.Id != 0)
                {
                    conditionFields.Add("M.SupplierGroupId");
                    conditionValues.Add(supplier.Id.ToString());
                }

                // Customer Name
                if (!string.IsNullOrEmpty(supplier.Name))
                {
                    conditionFields.Add("M.Name");
                    conditionValues.Add(supplier.Name);
                }

                // Customer Code
                if (!string.IsNullOrEmpty(supplier.Code))
                {
                    conditionFields.Add("M.Code");
                    conditionValues.Add(supplier.Code);
                }

                SupplierService _service = new SupplierService();

                if (conditionFields.Count == 0)
                {
                    resultVM = await _service.ReportList(null, null, null);
                }
                else
                {
                    resultVM = await _service.ReportList(
                        conditionFields.ToArray(),
                        conditionValues.ToArray(),
                        null
                    );
                }

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message
                };
            }
        }

    }
}
