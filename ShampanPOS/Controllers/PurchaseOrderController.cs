using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Configuration;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using static ShampanPOS.Configuration.HttpRequestHelper;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        PurchaseOrderService _purchaseOrderService = new PurchaseOrderService();
        CommonService _common = new CommonService();

        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(PurchaseOrderVM purchase)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _purchaseOrderService = new PurchaseOrderService();

            try
            {
                resultVM = await _purchaseOrderService.Insert(purchase);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = purchase
                };
            }
        }

        [HttpPost("Update")]
        public async Task<ResultVM> Update(PurchaseOrderVM purchase)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.Update(purchase);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = purchase
                };
            }
        }


        // POST: api/SaleDelivery/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();

                string?[] IDs = null;
                IDs = new string?[] { vm.Id.ToString() };

                resultVM = await _purchaseOrderService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/PurchaseOrder/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // GET: api/PurchaseOrder/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        //GET: api/PurchaseOrder/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/PurchaseOrder/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();

                resultVM = await _purchaseOrderService.MultiplePost(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/PurchaseOrder/MultipleIsCompleted
        [HttpPost("MultipleIsCompleted")]
        public async Task<ResultVM> MultipleIsCompleted(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();

                resultVM = await _purchaseOrderService.MultipleIsCompleted(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/PurchaseOrder/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.GetGridData(options, new[] { "H.BranchId", "H.IsPost", "H.OrderDate between", "H.OrderDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/PurchaseOrder/GetDetailsGridData
        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.GetDetailsGridData(options, new[] { "H.BranchId", "H.IsPost", "H.OrderDate between", "H.OrderDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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




        //// POST: api/PurchaseOrder/PurchaseOrderList
        //[HttpPost("PurchaseOrderList")]
        //public async Task<ResultVM> PurchaseOrderList(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        _purchaseOrderService = new PurchaseOrderService();
        //        resultVM = await _purchaseOrderService.PurchaseOrderList(vm.IDs);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = vm
        //        };
        //    }
        //}

        // POST: api/PurchaseOrder/GetPurchaseOrderDetailDataById
        [HttpPost("GetPurchaseOrderDetailDataById")]
        public async Task<ResultVM> GetPurchaseOrderDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _purchaseOrderService = new PurchaseOrderService();
                resultVM = await _purchaseOrderService.GetPurchaseOrderDetailDataById(options, masterId);
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




        // POST: api/PurchaseOrder/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            _common = new CommonService();
            ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string baseUrl = "";

                settingResult = await _common.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { "DMSReportUrl", "DMSReportUrl" }, null);

                if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingValue)
                {
                    if (settingValue.Rows.Count > 0)
                    {
                        baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                _purchaseOrderService = new PurchaseOrderService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _purchaseOrderService.ReportPreview(new[] { "M.Id", "M.BranchId", "M.OrderDate between", "M.OrderDate between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Purchase/GetPurchaseOrder", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "PurchaseOrder.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }







    }
}
