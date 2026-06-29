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
            ResultVM resultVM = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId
                };

                SupplierService _service = new SupplierService();

                resultVM = await _service.SupplierReport(conditionFields, conditionValues, param);

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

                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId
                };

                CustomerService _service = new CustomerService();
                resultVM = await _service.CustomerReport(conditionFields, conditionValues, param);

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
                if (customer == null || string.IsNullOrEmpty(customer.BranchId?.ToString()) || string.IsNullOrEmpty(customer.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                conditionFields.Add("M.BranchId");
                conditionValues.Add(customer.BranchId.ToString());

                conditionFields.Add("M.CompanyId");
                conditionValues.Add(customer.CompanyId.ToString());

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

                resultVM = await _service.ReportList(
                    conditionFields.ToArray(),
                    conditionValues.ToArray(),
                    null
                );

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.ToString() 
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
                    conditionFields = new string[] { "M.Id", "M.CompanyId" };
                    conditionValues = new string[] { Vm.Id, Vm.CompanyId };
                }
                else {
                    conditionFields = new string[] { "M.CompanyId" };
                    conditionValues = new string[] { Vm.CompanyId };
                }

                    //var param = new PeramModel
                    //{
                    //    Id = Vm.Id,
                    //    CompanyId = Vm.CompanyId
                    //};

                    PurchaseService _service = new PurchaseService();
                resultVM = await _service.GetPurchaseReport(conditionFields, conditionValues, null);
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
                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId
                };

                SaleService _service = new SaleService();
                resultVM = await _service.SaleReport(conditionFields, conditionValues, param);
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


        [HttpPost("GetReport")]
        public async Task<ResultVM> GetReport(CommonVM Vm)
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
                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId
                };

                SaleService _service = new SaleService();
                resultVM = await _service.GetSaleReport(conditionFields, conditionValues, param);
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

        //[HttpPost("GetSaleReturnReport")]
        //public async Task<ResultVM> GetSaleReturnReport(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.Id))
        //        {
        //            conditionFields = new string[] { "M.Id" };
        //            conditionValues = new string[] { Vm.Id };
        //        }

        //        var param = new PeramModel
        //        {
        //            Id = Vm.Id,
        //            CompanyId = Vm.CompanyId
        //        };

        //        SaleReturnService _service = new SaleReturnService();
        //        resultVM = await _service.SaleReturnReport(conditionFields, conditionValues, param);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not fetched.",
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}

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
                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId
                };

                SaleReturnService _service = new SaleReturnService();
                resultVM = await _service.SaleReturnReport(conditionFields, conditionValues, param);
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
                    conditionFields = new string[] { "M.Id", "M.CompanyId" };
                    conditionValues = new string[] { Vm.Id, Vm.CompanyId };
                }
                else
                {
                    conditionFields = new string[] { "M.CompanyId" };
                    conditionValues = new string[] { Vm.CompanyId };
                }

                PurchaseOrderService _service = new PurchaseOrderService();
                resultVM = await _service.PurchaseOrderReport(conditionFields, conditionValues, null);
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
                    conditionFields = new string[] { "M.Id", "M.CompanyId" };
                    conditionValues = new string[] { Vm.Id, Vm.CompanyId };
                }
                else
                {
                    conditionFields = new string[] { "M.CompanyId" };
                    conditionValues = new string[] { Vm.CompanyId };
                }


                PurchaseReturnService _service = new PurchaseReturnService();
                resultVM = await _service.PurchaseReturnReport(conditionFields, conditionValues, null);
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
                if (product == null || string.IsNullOrEmpty(product.BranchId?.ToString()) || string.IsNullOrEmpty(product.CompanyId?.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                conditionFields.Add("M.BranchId");
                conditionValues.Add(product.BranchId.ToString());

                conditionFields.Add("M.CompanyId");
                conditionValues.Add(product.CompanyId.ToString());

                if (product.Id != null && product.Id != 0)
                {
                    conditionFields.Add("M.ProductGroupId");
                    conditionValues.Add(product.Id.ToString());
                }

                if (!string.IsNullOrEmpty(product.Name))
                {
                    conditionFields.Add("M.Name");
                    conditionValues.Add(product.Name);
                }

                if (!string.IsNullOrEmpty(product.Code))
                {
                    conditionFields.Add("M.Code");
                    conditionValues.Add(product.Code);
                }

                ProductService _service = new ProductService();
                resultVM = await _service.ReportList(
                    conditionFields.ToArray(),
                    conditionValues.ToArray(),
                    null
                );

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.ToString() 
                };
            }
        }

        [HttpPost("GetSupplierByCategory")]
        public async Task<ResultVM> GetSupplierByCategory(SupplierVM supplier)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (supplier == null || string.IsNullOrEmpty(supplier.BranchId?.ToString()) || string.IsNullOrEmpty(supplier.CompanyId?.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                List<string> conditionFields = new List<string>();
                List<string> conditionValues = new List<string>();

                conditionFields.Add("M.BranchId");
                conditionValues.Add(supplier.BranchId.ToString());

                conditionFields.Add("M.CompanyId");
                conditionValues.Add(supplier.CompanyId.ToString());

                if (supplier.Id != null && supplier.Id != 0)
                {
                    conditionFields.Add("M.SupplierGroupId");
                    conditionValues.Add(supplier.Id.ToString());
                }

                if (!string.IsNullOrEmpty(supplier.Name))
                {
                    conditionFields.Add("M.Name");
                    conditionValues.Add(supplier.Name);
                }

                if (!string.IsNullOrEmpty(supplier.Code))
                {
                    conditionFields.Add("M.Code");
                    conditionValues.Add(supplier.Code);
                }

                SupplierService _service = new SupplierService();

                resultVM = await _service.ReportList(
                    conditionFields.ToArray(),
                    conditionValues.ToArray(),
                    null
                );

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.ToString() 
                };
            }
        }

        [HttpPost("GetProductReport")]
        public async Task<ResultVM> GetProductReport(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { Vm.Id };
                }

                var param = new PeramModel
                {
                    Id = Vm.Id,
                    CompanyId = Vm.CompanyId  
                };

                ProductService _service = new ProductService();

                resultVM = await _service.ProductReport(conditionFields, conditionValues, param);

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

        //[HttpPost("GetSaleReturnReport")]
        //public async Task<ResultVM> GetSaleReturnReport(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.Id))
        //        {
        //            conditionFields = new string[] { "M.Id" };
        //            conditionValues = new string[] { Vm.Id };
        //        }

        //        SaleReturnService _service = new SaleReturnService();
        //        resultVM = await _service.SaleReturnReport(conditionFields, conditionValues, null);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not fetched.",
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}
        //[HttpPost("GetPurchaseByList")]
        //public async Task<ResultVM> GetPurchaseByList(PurchaseVM purchase)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

        //    try
        //    {
        //        List<string> conditionFields = new List<string>();
        //        List<string> conditionValues = new List<string>();

        //        if (!string.IsNullOrEmpty(purchase.SupplierName))
        //        {
        //            conditionFields.Add("M.SupplierName");
        //            conditionValues.Add(purchase.SupplierName);
        //        }

        //        if (!string.IsNullOrEmpty(purchase.PurchaseFromDate))
        //        {
        //            conditionFields.Add("M.PurchaseFromDate");
        //            conditionValues.Add(purchase.PurchaseFromDate);
        //        }

        //        if (!string.IsNullOrEmpty(purchase.PurchaseToDate))
        //        {
        //            conditionFields.Add("M.PurchaseToDate");
        //            conditionValues.Add(purchase.PurchaseToDate);
        //        }

        //        if (!string.IsNullOrEmpty(purchase.InvoiceFromDate))
        //        {
        //            conditionFields.Add("M.InvoiceFromDate");
        //            conditionValues.Add(purchase.InvoiceFromDate);
        //        }
        //        if (!string.IsNullOrEmpty(purchase.InvoiceToDate))
        //        {
        //            conditionFields.Add("M.InvoiceToDate");
        //            conditionValues.Add(purchase.InvoiceToDate);
        //        }

        //        PurchaseService _service = new PurchaseService();

        //        if (conditionFields.Count == 0)
        //        {
        //            resultVM = await _service.ReportList(null, null, null);
        //        }
        //        else
        //        {
        //            resultVM = await _service.ReportList(
        //                conditionFields.ToArray(),
        //                conditionValues.ToArray(),
        //                null
        //            );
        //        }

        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not fetched.",
        //            ExMessage = ex.Message
        //        };
        //    }
        //}



        [HttpPost("GetSaleByList")]
        public async Task<ResultVM> GetSaleByList(SaleReportVM sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (string.IsNullOrEmpty(sale.BranchId.ToString()) || string.IsNullOrEmpty(sale.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "S.BranchId", "S.CompanyId" };
                string[] conditionValues = new string[] { sale.BranchId.ToString(), sale.CompanyId.ToString() };


                SaleService _service = new SaleService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, sale);
                

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



        [HttpPost("GetSaleOrderByList")]
        public async Task<ResultVM> GetSaleOrderByList(SaleOrderReportVM saleorder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (string.IsNullOrEmpty(saleorder.BranchId.ToString()) || string.IsNullOrEmpty(saleorder.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "S.BranchId", "S.CompanyId" };
                string[] conditionValues = new string[] { saleorder.BranchId.ToString(), saleorder.CompanyId.ToString() };

                SaleOrderService _service = new SaleOrderService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, saleorder);


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



        [HttpPost("GetSaleReturnByList")]
        public async Task<ResultVM> GetSaleReturnByList(SaleReturnReportVM salereturn)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {

                if (string.IsNullOrEmpty(salereturn.BranchId.ToString()) || string.IsNullOrEmpty(salereturn.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "S.BranchId", "S.CompanyId" };
                string[] conditionValues = new string[] { salereturn.BranchId.ToString(), salereturn.CompanyId.ToString() };

                SaleReturnService _service = new SaleReturnService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, salereturn);


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



        [HttpPost("GetPurchaseByList")]
        public async Task<ResultVM> GetPurchaseByList(PurchaseReportVM purchase)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (string.IsNullOrEmpty(purchase.BranchId.ToString()) || string.IsNullOrEmpty(purchase.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "P.BranchId", "P.CompanyId" };
                string[] conditionValues = new string[] { purchase.BranchId.ToString(), purchase.CompanyId.ToString() };

                PurchaseService _service = new PurchaseService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, purchase);


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



        [HttpPost("GetPurchaseOrderByList")]
        public async Task<ResultVM> GetPurchaseOrderByList(PurchaseOrderReportVM purchaseorder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (string.IsNullOrEmpty(purchaseorder.BranchId.ToString()) || string.IsNullOrEmpty(purchaseorder.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "P.BranchId", "P.CompanyId" };
                string[] conditionValues = new string[] { purchaseorder.BranchId.ToString(), purchaseorder.CompanyId.ToString() };


                PurchaseOrderService _service = new PurchaseOrderService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, purchaseorder);


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




        [HttpPost("GetPurchaseReturnByList")]
        public async Task<ResultVM> GetPurchaseReturnByList(PurchaseReturnReportVM purchasereturn)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                if (string.IsNullOrEmpty(purchasereturn.BranchId.ToString()) || string.IsNullOrEmpty(purchasereturn.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "P.BranchId", "P.CompanyId" };
                string[] conditionValues = new string[] { purchasereturn.BranchId.ToString(), purchasereturn.CompanyId.ToString() };

                PurchaseReturnService _service = new PurchaseReturnService();

                resultVM = await _service.ReportList(conditionFields, conditionValues, purchasereturn);


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


        [HttpPost("GetPurchaseOrdervsPurchaseByList")]
        public async Task<ResultVM> GetPurchaseOrdervsPurchaseByList(PurchaseReportVM purchase)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {

                PurchaseService _service = new PurchaseService();

                resultVM = await _service.PurchaseOrdervsPurchaseReportList(purchase);


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




        [HttpPost("GetSupplierProductByList")]
        public async Task<ResultVM> GetSupplierProductByList(SupplierProductReportVM supplierProduct)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                SupplierProductService _service = new SupplierProductService();

                resultVM = await _service.ReportList(supplierProduct);


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




        [HttpPost("GetBankTransactionReportList")]
        public async Task<ResultVM> GetBankTransactionReportList(BankTransactionReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.BankTransactionReportList(vm);
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

        [HttpPost("PurchaseReturnvsPurchaseReportList")]
        public async Task<ResultVM> PurchaseReturnvsPurchaseReportList(PurchaseReportVM purchase)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                PurchaseService _service = new PurchaseService();

                resultVM = await _service.PurchaseReturnvsPurchaseReportList(purchase);


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



        [HttpPost("GetCustomerSaleCollectionReportList")]
        public async Task<ResultVM> GetCustomerSaleCollectionReportList(
    CustomerSaleCollectionReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.CustomerSaleCollectionReportList(vm);
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


        [HttpPost("GetSupplierPurchasePaymentReportList")]
        public async Task<ResultVM> GetSupplierPurchasePaymentReportList(
    SupplierPurchasePaymentReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.SupplierPurchasePaymentReportList(vm);
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

        [HttpPost("GetSupplierPaymentDueList")]
        public async Task<ResultVM> GetSupplierPaymentDueList(SupplierPaymentDueVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.GetSupplierPaymentDueList(vm);
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

        [HttpPost("GetCustomerCollectionDueList")]
        public async Task<ResultVM> GetCustomerCollectionDueList(CustomerCollectionDueVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.GetCustomerCollectionDueList(vm);
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
        [HttpPost("GetSaleOrderStatusList")]
        public async Task<ResultVM> GetSaleOrderStatusList(SaleOrderStatusVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                ReportsService _service = new ReportsService();
                resultVM = await _service.GetSaleOrderStatusList(vm);
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

        

        [HttpPost("SalevsSaleReturnReportList")]
        public async Task<ResultVM> SalevsSaleReturnReportList(SaleReportVM sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                SaleService _service = new SaleService();

                resultVM = await _service.SalevsSaleReturnReportList(sale);


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

        [HttpPost("GetSaleOrdervsSaleByList")]
        public async Task<ResultVM> GetSaleOrdervsSaleByList(SaleReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };

            try
            {
                SaleService _service = new SaleService();

                resultVM = await _service.SaleOrdervsSaleReportList(vm);

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
