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
    public class CommonController : ControllerBase
    {

        [HttpPost("NextPrevious")]
        public async Task<ResultVM> NextPrevious(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.NextPrevious(vm.Id, vm.Status, vm.TableName, vm.Type);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }

        // POST: api/Common/GetSettingsValue
        [HttpPost("GetSettingsValue")]
        public async Task<ResultVM> GetSettingsValue(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { vm.Group, vm.Name }, null);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable settingValue)
                {
                    resultVM.DataVM = null;
                    resultVM.DataVM = Extensions.ConvertDataTableToList(settingValue);
                }

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

        // POST: api/Common/EnumList
        [HttpPost("EnumList")]
        public async Task<ResultVM> EnumList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.EnumList(new[] { "EnumType" }, new[] { Vm.Value.ToString() }, null);
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

        // POST: api/Common/AreaLocationList
        //[HttpPost("AreaLocationList")]
        //public async Task<ResultVM> AreaLocationList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();

        //        var columns = new List<string> { "L.EnumType" };
        //        var values = new List<string> { Vm.Value.ToString() };
        //        if (!string.IsNullOrEmpty(Vm.ParentId))
        //        {
        //            columns.Add("L.ParentId");
        //            values.Add(Vm.ParentId.ToString());
        //        }

        //        resultVM = await _commonService.AreaLocationList(columns.ToArray(), values.ToArray(), null);
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

        // POST: api/Common/ParentAreaList
        [HttpPost("ParentAreaList")]
        public async Task<ResultVM> ParentAreaList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.ParentAreaList(new[] { " " }, new[] { "" }, null);
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

        //[HttpPost("GetSalesPersonList")]
        //public async Task<ResultVM> GetSalesPersonList(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        SalesPersonService _salesPersonService = new SalesPersonService();

        //        string[] conditionFields = null;
        //        string[] conditionValues = null;
        //        conditionFields = vm.ConditionalFields;
        //        conditionValues = vm.ConditionalValues;
        //        _salesPersonService = new SalesPersonService();
        //        if (!string.IsNullOrEmpty(vm.Id))
        //        {
        //            conditionFields = new string[] { "M.Id" };
        //            conditionValues = new string[] { vm.Id };
        //        }
        //        resultVM = await _salesPersonService.List(conditionFields, conditionValues, null);
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

        // POST: api/Common/ParentBranchProfileList
        [HttpPost("ParentBranchProfileList")]
        public async Task<ResultVM> ParentBranchProfileList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.ParentBranchProfileList(new[] { "" }, new[] { "" }, null);
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

        // POST: api/Common/AssingedBranchList
        [HttpPost("AssingedBranchList")]
        public async Task<ResultVM> AssingedBranchList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.AssingedBranchList(new[] { "U.UserName" }, new[] { Vm.UserId }, null);
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


        // POST: api/Common/ProductGroupList
        [HttpPost("ProductGroupList")]
        public async Task<ResultVM> ProductGroupList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                //resultVM = await _commonService.ProductGroupList(new[] { "H.Id" }, new[] { Vm.Value.ToString() == "0" ? null : Vm.Value.ToString() }, null);
                resultVM = await _commonService.ProductGroupList(new[] { "" }, new[] { "" }, null);

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
        // POST: api/Common/UOMList
        [HttpPost("UOMList")]
        public async Task<ResultVM> UOMList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.UOMList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/EnumTypeList
        [HttpPost("EnumTypeList")]
        public async Task<ResultVM> EnumTypeList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.EnumTypeList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/SalePersonList
        //[HttpPost("SalePersonList")]
        //public async Task<ResultVM> SalePersonList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.BranchId))
        //        {
        //            conditionFields = new string[] { "H.BranchId" };
        //            conditionValues = new string[] { Vm.BranchId };
        //        }

        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.SalePersonList(conditionFields, conditionValues, null);
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


        // POST: api/Common/SaleOrderList
        //[HttpPost("SaleOrderList")]
        //public async Task<ResultVM> SaleOrderList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.BranchId))
        //        {
        //            conditionFields = new string[] { "H.BranchId" };
        //            conditionValues = new string[] { Vm.BranchId };
        //        }

        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.SaleOrderList(conditionFields, conditionValues, null);
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



        // POST: api/Common/GetSalePersonParentList
        //[HttpPost("GetSalePersonParentList")]
        //public async Task<ResultVM> GetSalePersonParentList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.BranchId))
        //        {
        //            conditionFields = new string[] { "H.BranchId" };
        //            conditionValues = new string[] { Vm.BranchId };
        //        }

        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetSalePersonParentList(conditionFields, conditionValues, null);
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




        // POST: api/Common/CurrencieList
        //[HttpPost("CurrencieList")]
        //public async Task<ResultVM> CurrencieList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CurrencieList(new[] { "" }, new[] { "" }, null);
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



        // POST: api/Common/CustomerCategoryList
        [HttpPost("CustomerCategoryList")]
        public async Task<ResultVM> CustomerCategoryList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.CustomerCategoryList(new[] { "" }, new[] { "" }, null);
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


        // POST: api/Common/ProductList
        [HttpPost("ProductList")]
        public async Task<ResultVM> ProductList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.ProductList(new[] { "" }, new[] { "" }, null);
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


        // POST: api/Common/DeliveryPersonList
        //[HttpPost("DeliveryPersonList")]
        //public async Task<ResultVM> DeliveryPersonList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.DeliveryPersonList(new[] { "" }, new[] { "" }, null);
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



        // POST: api/Common/CustomerList
        [HttpPost("CustomerList")]
        public async Task<ResultVM> CustomerList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.BranchId))
                {
                    conditionFields = new string[] { "H.BranchId" };
                    conditionValues = new string[] { Vm.BranchId };
                }
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.CustomerList(conditionFields, conditionValues, null);
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
        // POST: api/Common/ReceiveByDeliveryPersonList
        [HttpPost("ReceiveByDeliveryPersonList")]
        public async Task<ResultVM> ReceiveByDeliveryPersonList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.ReceiveByDeliveryPersonList(new[] { "" }, new[] { "" }, null);
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
        //// POST: api/Common/CustomerList
        //[HttpPost("CustomerList")]
        //public async Task<ResultVM> CustomerList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CustomerList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/CustomerGroupList
        [HttpPost("CustomerGroupList")]
        public async Task<ResultVM> CustomerGroupList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.CustomerGroupList(new[] { "" }, new[] { "" }, null);
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

        // POST: api/Common/CustomerRouteList
        //[HttpPost("CustomerRouteList")]
        //public async Task<ResultVM> CustomerRouteList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CustomerRouteList(new[] { "H.BranchId" }, new[] { Vm.BranchId }, null);
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
        // POST: api/Common/SupplierList
        [HttpPost("SupplierList")]
        public async Task<ResultVM> SupplierList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SupplierList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/SupplierGroupList
        [HttpPost("SupplierGroupList")]
        public async Task<ResultVM> SupplierGroupList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SupplierGroupList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/CampaignTargetList
        //[HttpPost("CampaignTargetList")]
        //public async Task<ResultVM> CampaignTargetList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CampaignTargetList(new[] { "" }, new[] { "" }, null);
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
        // POST: api/Common/GetProductModalForSaleData
        [HttpPost("GetProductModalForSaleData")]
        public async Task<ResultVM> GetProductModalForSaleData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetProductModalForSaleData(new[] { "P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like", "P.ImagePath like" }, new[] { model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName, model.ImagePath }, vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }

        // POST: api/Common/GetProductModalForPurchaseData
        [HttpPost("GetProductModalForPurchaseData")]
        public async Task<ResultVM> GetProductModalForPurchaseData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetProductModalForPurchaseData(new[] { "P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like" }, new[] { model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName }, vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }

        // POST: api/Common/ParentSalePersonList
        //[HttpPost("ParentSalePersonList")]
        //public async Task<ResultVM> ParentSalePersonList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.ParentSalePersonList(new[] { "M.BranchId" }, new[] { Vm.BranchId.ToString() }, null);
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

        // POST: api/Common/GetFiscalYearForSaleList
        //[HttpPost("GetFiscalYearForSaleList")]
        //public async Task<ResultVM> GetFiscalYearForSaleList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetFiscalYearForSaleList(new[] { "" }, new[] { "" }, null);
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

        //[HttpPost("SalePersonList")]
        //public async Task<ResultVM> SalePersonList(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;
        //        conditionFields = vm.ConditionalFields;
        //        conditionValues = vm.ConditionalValues;
        //        SalesPersonService _salesPersonService = new SalesPersonService();

        //        conditionFields = new string[] { "M.Id", "M.BranchId" };
        //        conditionValues = new string[] { vm.Id, vm.BranchId };

        //        resultVM = await _salesPersonService.List(conditionFields, conditionValues, null);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}

        [HttpPost("GetAreaList")]
        public async Task<ResultVM> GetAreaList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;
                conditionFields = vm.ConditionalFields;
                conditionValues = vm.ConditionalValues;
                AreaService _areaService = new AreaService();
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { vm.Id };
                }

                resultVM = await _areaService.List(conditionFields, conditionValues, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }


        [HttpPost("GetCustomersBySalePersonAndBranch")]
        public async Task<ResultVM> GetCustomersBySalePersonAndBranch([FromBody] CustomerData requestParam)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                // Assuming CustomerService and your method is correctly set up
                CustomerService _customerService = new CustomerService();
                resultVM = await _customerService.GetCustomersBySalePersonAndBranch(requestParam.salePersonId, requestParam.branchId);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }




        //[HttpPost("GetRouteBySalePersonAndBranch")]
        //public async Task<ResultVM> GetRouteBySalePersonAndBranch([FromBody] CustomerData requestParam)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {

        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetRouteBySalePersonAndBranch(requestParam.salePersonId, requestParam.branchId);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}




        [HttpPost("GetProductModalData")]
        public async Task<ResultVM> GetProductModalData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetProductModalData(new[] { "P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like" }, new[] { model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName }, vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }

        // POST: api/Common/GetTop10Customers
        [HttpPost("GetTop10Customers")]
        public async Task<ResultVM> GetTop10Customers(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetTop10Customers(Vm.BranchId);
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

        // POST: api/Common/GetBottom10Customers
        [HttpPost("GetBottom10Customers")]
        public async Task<ResultVM> GetBottom10Customers(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetBottom10Customers(Vm.BranchId);
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

        // POST: api/Common/GetTop10Products
        //[HttpPost("GetTop10Products")]
        //public async Task<ResultVM> GetTop10Products(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetTop10Products(Vm.BranchId);
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

        // POST: api/Common/GetBottom10Products
        //[HttpPost("GetBottom10Products")]
        //public async Task<ResultVM> GetBottom10Products(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetBottom10Products(Vm.BranchId);
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

        // POST: api/Common/GetTop10SalePersons
        //[HttpPost("GetTop10SalePersons")]
        //public async Task<ResultVM> GetTop10SalePersons(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetTop10SalePersons(Vm.BranchId);
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

        // POST: api/Common/GetBottom10SalePersons
        //[HttpPost("GetBottom10SalePersons")]
        //public async Task<ResultVM> GetBottom10SalePersons(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetBottom10SalePersons(Vm.BranchId);
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

        // POST: api/Common/GetOrderPurchasePOReturnData
        [HttpPost("GetOrderPurchasePOReturnData")]
        public async Task<ResultVM> GetOrderPurchasePOReturnData(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetOrderPurchasePOReturnData(Vm.BranchId);
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

        // POST: api/Common/GetSalesData
        //[HttpPost("GetSalesData")]
        //public async Task<ResultVM> GetSalesData(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetSalesData(Vm.BranchId);
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

        // POST: api/Common/GetPendingSales
        [HttpPost("GetPendingSales")]
        public async Task<ResultVM> GetPendingSales(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetPendingSales(Vm.BranchId);
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

        // POST: api/Common/CampaignMudularityCalculation
        //[HttpPost("CampaignMudularityCalculation")]
        //public async Task<ResultVM> CampaignMudularityCalculation(CampaignUtilty vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CampaignMudularityCalculation(vm);
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

        //// POST: api/Common/CampaignInvoiceCalculation
        //[HttpPost("CampaignInvoiceCalculation")]
        //public async Task<ResultVM> CampaignInvoiceCalculation(CampaignUtilty vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.CampaignInvoiceCalculation(vm);
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

        //// POST: api/Common/GetCampaignList
        //[HttpPost("GetCampaignList")]
        //public async Task<ResultVM> GetCampaignList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.GetCampaignList(new[] { "" }, new[] { "" }, null);
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

        // POST: api/Common/PaymentTypeList
        [HttpPost("PaymentTypeList")]
        public async Task<ResultVM> PaymentTypeList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {


                CommonService _commonService = new CommonService();
                resultVM = await _commonService.PaymentTypeList(new[] { "" }, new[] { "" }, null);
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

        //[HttpPost("GetSaleDeleveryByCustomerAndBranch")]
        //public async Task<ResultVM> GetSaleDeleveryByCustomerAndBranch([FromBody] CustomerData requestParam)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        // Assuming CustomerService and your method is correctly set up
        //        SaleDeliveryService _saleDeliveryService = new SaleDeliveryService();
        //        resultVM = await _saleDeliveryService.GetSaleDeleveryByCustomerAndBranch(requestParam.salePersonId, requestParam.branchId);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = null
        //        };
        //    }
        //}

        //// POST: api/Common/SaleDeleveryList
        //[HttpPost("SaleDeleveryList")]
        //public async Task<ResultVM> SaleDeleveryList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        if (!string.IsNullOrEmpty(Vm.BranchId))
        //        {
        //            conditionFields = new string[] { "H.BranchId" };
        //            conditionValues = new string[] { Vm.BranchId };
        //        }
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.SaleDeleveryList(conditionFields, conditionValues, null);
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

        //// POST: api/Common/GetSaleDeleveryModal
        //[HttpPost("GetSaleDeleveryModal")]
        //public async Task<ResultVM> GetSaleDeleveryModal(SaleDeliveryVM model)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        PeramModel vm = new PeramModel();
        //        vm = model.PeramModel;

        //        resultVM = await _commonService.GetSaleDeleveryModal(new[] { "P.Code like", }, new[] { model.Code }, vm);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = model
        //        };
        //    }
        //}

        // POST: api/Common/SaleDeleveryList
        //[HttpPost("GetFocalPointList")]
        //public async Task<ResultVM> GetFocalPointList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;
        //        conditionFields = Vm.ConditionalFields;
        //        conditionValues = Vm.ConditionalValues;
        //        if (!string.IsNullOrEmpty(Vm.Id))
        //        {
        //            conditionFields = new string[] { "M.Id" };
        //            conditionValues = new string[] { Vm.Id };
        //        }
        //        if (!string.IsNullOrEmpty(Vm.BranchId))
        //        {
        //            conditionFields = new string[] { "M.BranchId" };
        //            conditionValues = new string[] { Vm.BranchId.ToString() };
        //        }

        //        FocalPointService _focalPoint = new FocalPointService();
        //        resultVM = await _focalPoint.List(conditionFields, conditionValues, null);
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

        // POST: api/Branch/CheckDayEnd
        [HttpPost]
        [Route("CheckDayEnd")]
        public async Task<ResultVM> CheckDayEnd(CommonVM vm)
        {
            CommonService _commonService = new CommonService();

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
                if (vm == null || string.IsNullOrEmpty(vm.Id))
                {
                    resultVM.Message = "Branch ID is required.";
                    return resultVM;
                }

                // Call your service/repo method
                resultVM = await _commonService.HasDayEndData(vm.Id); // returns ResultVM

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }
    }
}
