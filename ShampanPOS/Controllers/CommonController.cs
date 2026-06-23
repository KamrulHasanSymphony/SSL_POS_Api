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

                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.ProductGroupList(conditionFields, conditionValues, null);
                return resultVM;
                //CommonService _commonService = new CommonService();
                ////resultVM = await _commonService.ProductGroupList(new[] { "H.Id" }, new[] { Vm.Value.ToString() == "0" ? null : Vm.Value.ToString() }, null);
                //resultVM = await _commonService.ProductGroupList(new[] { "" }, new[] { "" }, null);

                //return resultVM;
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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.CompanyId" };
                string[] conditionValues = new string[] {  Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.UOMList(conditionFields, conditionValues, null);
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
                resultVM = await _commonService.ProductList( new[] { "H.ProductGroupId" }, new[] { Vm.Value }, null);
                //resultVM = await _commonService.ProductList(new[] { "H.Id" }, Vm.Value, null);
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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.CustomerGroupList(conditionFields, conditionValues, null);
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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SupplierList(conditionFields, conditionValues, null);
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




        [HttpPost("GetSupplierProductList")]
        public async Task<ResultVM> GetSupplierProductList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetSupplierProductList(new[] { "SI.MasterSupplierId" }, new[] { Vm.Value }, null);
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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SupplierGroupList(conditionFields, conditionValues, null);
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

                if (string.IsNullOrEmpty(model.BranchId.ToString()) || string.IsNullOrEmpty(model.CompanyId.ToString()))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "H.BranchId", "H.CompanyId" ,"P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like" };
                string[] conditionValues = new string[] { model.BranchId.ToString(), model.CompanyId.ToString(), model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName };


                resultVM = await _commonService.GetProductModalForPurchaseData(conditionFields, conditionValues, vm);
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



        [HttpPost("GetProductModal")]
        public async Task<ResultVM> GetProductModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();

                //resultVM = await _commonService.GetProductModal(new[] { "" }, new[] { "" }, null);

                //string[] conditionalFields = { "P.CompanyId" };

                //string[] conditionalValues = { Vm.CompanyId ?? "0" };

                //resultVM = await _commonService.GetProductModal(
                //    conditionalFields,
                //    conditionalValues,
                //    new PeramModel
                //    {
                //        CompanyId = Vm.CompanyId
                //    });


                return await _commonService.GetProductModal(null,null,new PeramModel{CompanyId = Vm.CompanyId, BranchId = Vm.BranchId });

                //return resultVM;
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



        [HttpPost("GetProductByBarcode")]
        public async Task<ResultVM> GetProductByBarcode(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                DataVM = null
            };

            try
            {
                CommonService _commonService = new CommonService();

                // ✅ FIXED HERE
                //resultVM = await _commonService.GetProductByBarcode(
                //    new[] { "" },
                //    new[] { "" },
                //    vm
                //);

                string[] conditionalFields = { "P.CompanyId" };

                string[] conditionalValues = { vm.CompanyId ?? "0" };

                resultVM = await _commonService.GetProductByBarcode(
                    conditionalFields,
                    conditionalValues,
                    new CommonVM
                    {
                        CompanyId = vm.CompanyId,
                        Value = vm.Value
                    });



                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    DataVM = null
                };
            }
        }



        [HttpPost("ProductModal")]
        public async Task<ResultVM> ProductModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();


                resultVM = await _commonService.ProductModal(null, null, new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId });


                //resultVM = await _commonService.ProductModal(new[] { "" }, new[] { "" }, null);
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



        [HttpPost("GetProductModalPurchase")]
        public async Task<ResultVM> GetProductModalPurchase(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                //resultVM = await _commonService.GetProductModalPurchase(new[] { "" }, new[] { "" }, null);


                return await _commonService.GetProductModalPurchase(null, null, new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId });



                //return resultVM;
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



        //[HttpPost("SaleModal")]
        //public async Task<ResultVM> SaleModal(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.SaleModal(new[] { "M.CustomerId" }, new[] { Vm.Value }, null);
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




        [HttpPost("SaleModal")]
        public async Task<ResultVM> SaleModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                // Normalize Vm.Value: if null, empty, 0, or negative, set to ""
                string normalizedValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(Vm.Value))
                {
                    // Try parse as number to check for 0 or negative
                    if (decimal.TryParse(Vm.Value, out decimal val))
                    {
                        if (val > 0)
                        {
                            normalizedValue = Vm.Value;
                        }
                    }
                    else
                    {
                        // If not a number, just keep the string as is
                        normalizedValue = Vm.Value;
                    }
                }

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.SaleModal(
                    new[] { "M.CustomerId" },
                    new[] { normalizedValue },
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
                    ExMessage = ex.Message,
                    DataVM = null
                };
            }
        }





        //[HttpPost("PurchaseModal")]
        //public async Task<ResultVM> PurchaseModal(CommonVM Vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CommonService _commonService = new CommonService();
        //        resultVM = await _commonService.PurchaseModal(new[] { "M.SupplierId" }, new[] { Vm.Value }, null);
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




        [HttpPost("PurchaseModal")]
        public async Task<ResultVM> PurchaseModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                // Normalize Vm.Value: if null, empty, 0, or negative, set to ""
                string normalizedValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(Vm.Value))
                {
                    // Try parse as number to check for 0 or negative
                    if (decimal.TryParse(Vm.Value, out decimal val))
                    {
                        if (val > 0)
                        {
                            normalizedValue = Vm.Value;
                        }
                    }
                    else
                    {
                        // If not a number, keep the string as is
                        normalizedValue = Vm.Value;
                    }
                }

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.PurchaseModal(
                    new[] { "M.SupplierId" },
                    new[] { normalizedValue },
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
                    ExMessage = ex.Message,
                    DataVM = null
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

                //resultVM = await _sale.List(conditionFields, conditionValues, null, new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId });

                  resultVM = await _sale.List(
                      conditionFields,
                      conditionValues,
                      new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId }
                  );


                //return await _commonService.GetProductModal(null, null, new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId });


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



        [HttpPost("GetSectionList")]
        public async Task<ResultVM> GetSectionList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetSectionList(new[] { "" }, new[] { "" }, null);
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



        [HttpPost("GetStatusList")]
        public async Task<ResultVM> GetStatusList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetStatusList(new[] { "" }, new[] { "" }, null);
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



        [HttpPost("GetReportTypeList")]
        public async Task<ResultVM> GetReportTypeList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetReportTypeList(new[] { "M.EnumType" },  new[] { Vm.Value }, null);
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




        [HttpPost("GetTableList")]
        public async Task<ResultVM> GetTableList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetTableList(new[] { "" }, new[] { "" }, null);
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
                    conditionFields = new string[] { "M.BranchId" };
                    conditionValues = new string[] { Vm.BranchId };
                }

                PurchaseOrderService _purchase = new PurchaseOrderService();

                resultVM = await _purchase.List(
                    conditionFields,
                    conditionValues,
                    new PeramModel { CompanyId = Vm.CompanyId, BranchId = Vm.BranchId }
                );

                //resultVM = await _purchase.List(conditionFields, conditionValues, null);

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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "M.BranchId", "M.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };


                CommonService _commonService = new CommonService();
                resultVM = await _commonService.BankAccountList(conditionFields, conditionValues, null);
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




        [HttpPost("GetPaymentTypeList")]
        public async Task<ResultVM> GetPaymentTypeList(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CommonService _commonService = new CommonService();

                resultVM = await _commonService.GetPaymentTypeList(new[] { "" }, new[] { "" }, null);
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


        [HttpPost("GetTopProductsLast3MonthsTotalSale")]
        public async Task<ResultVM> GetTopProductsLast3MonthsTotalSale(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                CommonService _commonService = new CommonService();

                resultVM = await _commonService.GetTopProductsLast3MonthsTotalSale(new[] { "CompanyId", "Top" }, new[] { Vm.CompanyId ?? "0", Vm.Value ?? "10" }, null);

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


        [HttpPost("GetTop10ProductsCurrentMonthQty")]
        public async Task<ResultVM> GetTop10ProductsCurrentMonthQty(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                CommonService _commonService = new CommonService();

                resultVM = await _commonService.GetTop10ProductsCurrentMonthQty(new[] { "CompanyId", "Top" }, new[] { Vm.CompanyId ?? "0", Vm.Value ?? "10" }, null);

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





        [HttpPost("GetLowSellingProductsCurrentMonthQty")]
        public async Task<ResultVM> GetLowSellingProductsCurrentMonthQty(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                CommonService _commonService = new CommonService();

                resultVM = await _commonService.GetLowSellingProductsCurrentMonthQty(new[] { "CompanyId", "Top" }, new[] { Vm.CompanyId ?? "0", Vm.Value ?? "10" }, null);

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



        [HttpPost("GetTotalPurchasesLast3Months")]
        public async Task<ResultVM> GetTotalPurchasesLast3Months(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                // Initialize the common service to fetch purchase data
                CommonService _commonService = new CommonService();

                // Call the service to fetch total purchases for the last 3 months
                resultVM = await _commonService.GetTotalPurchasesLast3Months(new[] { "Top", "CompanyId" },
                    new[] { Vm.Value ?? "10", Vm.CompanyId ?? "0" }, null);

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



        [HttpPost("GetSaleOrderStatusStats")]
        public async Task<ResultVM> GetSaleOrderStatusStats(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                // Initialize the common service to fetch purchase data
                CommonService _commonService = new CommonService();

                // Call the service to fetch total purchases for the last 3 months
                resultVM = await _commonService.GetSaleOrderStatusStats(new[] { "Top", "CompanyId" },
                    new[] { Vm.Value ?? "10", Vm.CompanyId ?? "0" }, null);

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

        [HttpPost("GetPurchaseModal")]
        public async Task<ResultVM> GetPurchaseModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "M.BranchId", "M.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };

                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetPurchaseModal(conditionFields, conditionValues, null);
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

        [HttpPost("GetPurchaseOrderModal")]
        public async Task<ResultVM> GetPurchaseOrderModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "M.BranchId", "M.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };
                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetPurchaseOrderModal(conditionFields, conditionValues, null);
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
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "M.BranchId" };
                string[] conditionValues = new string[] { Vm.BranchId};


                CommonService _commonService = new CommonService();
                resultVM = await _commonService.BankIdList(conditionFields, conditionValues, null);
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


        [HttpPost("GetBankAccountModal")]
        public async Task<ResultVM> GetBankAccountModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }
                CommonService _commonService = new CommonService();
                if (!string.IsNullOrEmpty(vm.Value)) { 
                    string[] conditionFields = new string[] { "ba.BankId", "ba.BranchId", "ba.CompanyId" };
                    string[] conditionValues = new string[] {  vm.Value,vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetBankAccountModal(conditionFields, conditionValues, null);
                }
                else { 
                    string[] conditionFields = new string[] { "ba.BranchId", "ba.CompanyId" };
                    string[] conditionValues = new string[] {  vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetBankAccountModal(conditionFields, conditionValues, null);
                }

                //var fields = new List<string>();
                //var values = new List<string>();
                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("BankId");
                //    values.Add(vm.Value);
                //}
                //resultVM = await _commonService.GetBankAccountModal(conditionFields,conditionValues, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }

        [HttpPost("GetDepositModal")]
        public async Task<ResultVM> GetDepositModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }
                CommonService _commonService = new CommonService();
                if (!string.IsNullOrEmpty(vm.Value)&& !string.IsNullOrEmpty(vm.Value2))
                {
                    string[] conditionFields = new string[] { "ba.BankId", "d.BankAccountId", "d.BranchId" };
                    string[] conditionValues = new string[] { vm.Value, vm.Value2, vm.BranchId};
                    resultVM = await _commonService.GetDepositModal(conditionFields, conditionValues, null);
                }
                else if (!string.IsNullOrEmpty(vm.Value))
                {
                    string[] conditionFields = new string[] { "ba.BankId", "d.BranchId" };
                    string[] conditionValues = new string[] { vm.Value, vm.BranchId };
                    resultVM = await _commonService.GetDepositModal(conditionFields, conditionValues, null);
                }
                else if (!string.IsNullOrEmpty(vm.Value2))
                {
                    string[] conditionFields = new string[] { "d.BankAccountId", "d.BranchId" };
                    string[] conditionValues = new string[] { vm.Value2, vm.BranchId };
                    resultVM = await _commonService.GetDepositModal(conditionFields, conditionValues, null);
                }
                else
                {
                    string[] conditionFields = new string[] { "d.BranchId" };
                    string[] conditionValues = new string[] { vm.BranchId };
                    resultVM = await _commonService.GetDepositModal(conditionFields, conditionValues, null);
                }


                //CommonService _commonService = new CommonService();
                //var fields = new List<string>();
                //var values = new List<string>();
                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("BankId");
                //    values.Add(vm.Value);
                //}
                //if (!string.IsNullOrEmpty(vm.Value2))
                //{
                //    fields.Add("BankAccountId");
                //    values.Add(vm.Value2);
                //}
                //resultVM = await _commonService.GetDepositModal(fields.ToArray(), values.ToArray(), null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }

        [HttpPost("GetWithdrawalModal")]
        public async Task<ResultVM> GetWithdrawalModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {

                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }
                CommonService _commonService = new CommonService();
                if (!string.IsNullOrEmpty(vm.Value) && !string.IsNullOrEmpty(vm.Value2))
                {
                    string[] conditionFields = new string[] { "ba.BankId", "w.BankAccountId", "w.BranchId" };
                    string[] conditionValues = new string[] { vm.Value, vm.Value2, vm.BranchId };
                    resultVM = await _commonService.GetWithdrawalModal(conditionFields, conditionValues, null);
                }
                else if (!string.IsNullOrEmpty(vm.Value))
                {
                    string[] conditionFields = new string[] { "ba.BankId", "w.BranchId" };
                    string[] conditionValues = new string[] { vm.Value, vm.BranchId };
                    resultVM = await _commonService.GetWithdrawalModal(conditionFields, conditionValues, null);
                }
                else if (!string.IsNullOrEmpty(vm.Value2))
                {
                    string[] conditionFields = new string[] { "w.BankAccountId", "w.BranchId" };
                    string[] conditionValues = new string[] { vm.Value2, vm.BranchId };
                    resultVM = await _commonService.GetWithdrawalModal(conditionFields, conditionValues, null);
                }
                else
                {
                    string[] conditionFields = new string[] { "w.BranchId" };
                    string[] conditionValues = new string[] { vm.BranchId };
                    resultVM = await _commonService.GetWithdrawalModal(conditionFields, conditionValues, null);
                }
                //CommonService _commonService = new CommonService();
                //var fields = new List<string>();
                //var values = new List<string>();
                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("BankId");
                //    values.Add(vm.Value);
                //}
                //if (!string.IsNullOrEmpty(vm.Value2))
                //{
                //    fields.Add("BankAccountId");
                //    values.Add(vm.Value2);
                //}
                //resultVM = await _commonService.GetWithdrawalModal(fields.ToArray(), values.ToArray(), null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }

        [HttpPost("GetCustomerModal")]
        public async Task<ResultVM> GetCustomerModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }
                CommonService _commonService = new CommonService();
                if (!string.IsNullOrEmpty(vm.Value))
                {
                    string[] conditionFields = new string[] { "c.CustomerGroupId", "c.BranchId", "c.CompanyId" , "cg.BranchId", "cg.CompanyId" };
                    string[] conditionValues = new string[] { vm.Value, vm.BranchId , vm.CompanyId , vm.BranchId , vm.CompanyId };
                    resultVM = await _commonService.GetCustomerModal(conditionFields, conditionValues, null);
                }
                else
                {
                    string[] conditionFields = new string[] { "c.BranchId" , "c.CompanyId" , "cg.BranchId" , "cg.CompanyId" };
                    string[] conditionValues = new string[] { vm.BranchId , vm.CompanyId , vm.BranchId , vm.CompanyId };
                    resultVM = await _commonService.GetCustomerModal(conditionFields, conditionValues, null);
                }

                //CommonService _commonService = new CommonService();
                //var fields = new List<string>();
                //var values = new List<string>();
                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("CustomerGroupId");
                //    values.Add(vm.Value);
                //}
                //resultVM = await _commonService.GetCustomerModal(fields.ToArray(), values.ToArray(), null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }

        [HttpPost("GetSupplierModal")]
        public async Task<ResultVM> GetSupplierModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }
                CommonService _commonService = new CommonService();
                if (!string.IsNullOrEmpty(vm.Value))
                {
                    string[] conditionFields = new string[] { "s.SupplierGroupId", "s.BranchId", "s.CompanyId", "sg.BranchId", "sg.CompanyId" };
                    string[] conditionValues = new string[] { vm.Value, vm.BranchId, vm.CompanyId, vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetSupplierModal(conditionFields, conditionValues, null);
                }
                else
                {
                    string[] conditionFields = new string[] { "s.BranchId", "s.CompanyId", "sg.BranchId", "sg.CompanyId" };
                    string[] conditionValues = new string[] { vm.BranchId, vm.CompanyId, vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetSupplierModal(conditionFields, conditionValues, null);
                }

                //CommonService _commonService = new CommonService();
                //var fields = new List<string>();
                //var values = new List<string>();
                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("SupplierGroupId");
                //    values.Add(vm.Value);
                //}
                //resultVM = await _commonService.GetSupplierModal(fields.ToArray(), values.ToArray(), null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }


        [HttpPost("GetPurchaseReturnModal")]
        public async Task<ResultVM> GetPurchaseReturnModal(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                if (string.IsNullOrEmpty(Vm.BranchId) || string.IsNullOrEmpty(Vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "M.BranchId", "M.CompanyId" };
                string[] conditionValues = new string[] { Vm.BranchId, Vm.CompanyId };


                CommonService _commonService = new CommonService();
                resultVM = await _commonService.GetPurchaseReturnModal(conditionFields, conditionValues, null);

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

        [HttpPost("GetNewProductModal")]
        public async Task<ResultVM> GetNewProductModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                CommonService _commonService = new CommonService();

                if (!string.IsNullOrEmpty(vm.Value))
                {
                    string[] conditionFields = new string[] { "P.ProductGroupId", "P.BranchId", "P.CompanyId" };
                    string[] conditionValues = new string[] { vm.Value , vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetNewProductModal(conditionFields, conditionValues, null);

                }
                {
                    string[] conditionFields = new string[] { "P.BranchId", "P.CompanyId" };
                    string[] conditionValues = new string[] { vm.BranchId, vm.CompanyId };
                    resultVM = await _commonService.GetNewProductModal(conditionFields, conditionValues, null);

                }


                //CommonService _commonService = new CommonService();
                //var fields = new List<string>();
                //var values = new List<string>();

                //fields.Add("P.BranchId");
                //values.Add(vm.Value2);

                //if (!string.IsNullOrEmpty(vm.Value))
                //{
                //    fields.Add("P.ProductGroupId");
                //    values.Add(vm.Value);
                //}

                //resultVM = await _commonService.GetNewProductModal(fields.ToArray(), values.ToArray(), null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }


        [HttpPost("GetSaleOrderModal")]
        public async Task<ResultVM> GetSaleOrderModal(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
            try
            {
                if (string.IsNullOrEmpty(vm.BranchId) || string.IsNullOrEmpty(vm.CompanyId))
                {
                    resultVM.Status = "Fail";
                    resultVM.Message = "Branch and Company are required.";
                    return resultVM;
                }

                string[] conditionFields = new string[] { "so.BranchId", "so.CompanyId" };
                string[] conditionValues = new string[] { vm.BranchId, vm.CompanyId };

                CommonService _commonService = new CommonService();
                //var fields = new List<string> { "BranchId" };
                //var values = new List<string> { vm.Value };

                resultVM = await _commonService.GetSaleOrderModal(conditionFields, conditionValues, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM { Status = "Fail", Message = "Data not fetched.", ExMessage = ex.Message };
            }
        }

    }
}
