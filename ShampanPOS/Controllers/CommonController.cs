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

        // POST: api/Common/MasterItemGroupList
        [HttpPost("MasterItemGroupList")]
        public async Task<ResultVM> MasterItemGroupList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                //resultVM = await _commonService.ProductGroupList(new[] { "H.Id" }, new[] { Vm.Value.ToString() == "0" ? null : Vm.Value.ToString() }, null);
                resultVM = await _commonService.MasterItemGroupList(new[] { "" }, new[] { "" }, null);

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


        // POST: api/Common/MasterProductList
        [HttpPost("MasterProductList")]
        public async Task<ResultVM> MasterProductList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.MasterProductList(new[] { "" }, new[] { "" }, null);
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


        [HttpPost("MasterSupplierList")]
        public async Task<ResultVM> MasterSupplierList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.MasterSupplierList(new[] { "" }, new[] { "" }, null);
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




        // POST: api/Common/SaleOrderList
        [HttpPost("SaleOrderList")]
        public async Task<ResultVM> SaleOrderList(CommonVM Vm)
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
                resultVM = await _commonService.SaleOrderList(conditionFields, conditionValues, null);
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



        [HttpPost("GetSaleOrderList")]
        public async Task<ResultVM> GetSaleOrderList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string[] conditionFields = null;
                string[] conditionValues = null;

                if (!string.IsNullOrEmpty(Vm.BranchId))
                {
                    conditionFields = new string[] { "M.BranchId" };
                    conditionValues = new string[] { Vm.BranchId };
                }

                SaleOrderService _sale = new SaleOrderService();
                resultVM = await _sale.List(conditionFields, conditionValues, null);
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



        [HttpPost("BankIdList")]
        public async Task<ResultVM> BankIdList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.BankIdList(new[] { "" }, new[] { "" }, null);
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


        // POST: api/Common/GetPurchaseOrderIdData
        [HttpPost("GetPurchaseOrderIdData")]
        public async Task<ResultVM> GetPurchaseOrderIdData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetPurchaseOrderIdData(new[] { "P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like" }, new[] { model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName }, vm);
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



        [HttpPost("PurchaseOrderList")]
        public async Task<ResultVM> PurchaseOrderList(CommonVM Vm)
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

                PurchaseOrderService _purchase = new PurchaseOrderService();
                resultVM = await _purchase.List(conditionFields, conditionValues, null);
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


        //[HttpPost("PurchaseOrderList")]
        //public async Task<ResultVM> PurchaseOrderList(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.PurchaseOrderList(new[] { "" }, new[] { "" }, null);
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

        [HttpPost("BankAccountList")]
        public async Task<ResultVM> BankAccountList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.BankAccountList(new[] { "" }, new[] { "" }, null);
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


        // POST: api/Common/GetPurchaseData
        [HttpPost("GetPurchaseData")]
        public async Task<ResultVM> GetPurchaseData(PurchaseDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetPurchaseData(new[] { "M.Code like", "S.Name like", "E.Code like"}, new[] { model.Code, model.SupplierName, model.PurchaseOrderCode }, vm);
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



        // POST: api/Common/GetSaleData
        [HttpPost("GetSaleData")]
        public async Task<ResultVM> GetSaleData(SaleDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _commonService.GetSaleData(new[] { "M.Code like", "S.Name like", "E.Code like" }, new[] { model.Code, model.CustomerName, model.SaleOrderCode }, vm);
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

        // POST: api/Common/GetPurchaseDatabysupplier
        [HttpPost("GetPurchaseDatabysupplier")]
        public async Task<ResultVM> GetPurchaseDatabysupplier(PurchaseDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;
                vm.Id = model.SupplierId.ToString();

                resultVM = await _commonService.GetPurchaseDatabysupplier(new[] { "M.Code like", "S.Name like", "E.Code like" }, new[] { model.Code, model.SupplierName, model.PurchaseOrderCode }, vm);
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

        [HttpPost("GetProductReport")]
        public async Task<ResultVM> GetProductReport(CommonVM Vm)
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

                ProductService _service = new ProductService();
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


        [HttpPost("GetItemList")]
        public async Task<ResultVM> GetItemList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetItemList(new[] { "H.MasterItemGroupId" }, new[] { Vm.Value.ToString() }, null);
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

        // POST: api/Common/IsMasterItemGroupMappedWithProductGroup
        //[HttpPost("IsMasterItemGroupMappedWithProductGroup")]
        //public async Task<ResultVM> IsMasterItemGroupMappedWithProductGroup(CommonVM vm)
        //{
        //    ResultVM result = new ResultVM
        //    {
        //        Status = "Fail",
        //        Message = "Group not matched"
        //    };

        //    try
        //    {
        //        if (vm.MasterItemGroupId <= 0)
        //        {
        //            result.Message = "Invalid Master Item Group";
        //            return result;
        //        }

        //        CommonService service = new CommonService();

        //        bool isMapped =
        //            await service.IsMasterItemGroupMappedWithProductGroup(vm.MasterItemGroupId);

        //        if (isMapped)
        //        {
        //            result.Status = "Success";
        //            result.Message = "Group matched";
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Status = "Fail";
        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //}



        // POST: api/Common/MasterSupplierGroupList
        [HttpPost("MasterSupplierGroupList")]
        public async Task<ResultVM> MasterSupplierGroupList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.MasterSupplierGroupList(new[] { "" }, new[] { "" }, null);
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



        [HttpPost("GetSupplierListByGroup")]
        public async Task<ResultVM> GetSupplierListByGroup(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetSupplierListByGroup(new[] { "H.MasterSupplierGroupId" }, new[] { Vm.Value.ToString() }, null);
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
