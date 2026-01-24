using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.Configuration;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Threading.Tasks;
using static ShampanPOS.Configuration.HttpRequestHelper;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        SaleService _SaleService = new SaleService();
        CommonService _common = new CommonService();


        // POST: api/Sale/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _SaleService = new SaleService();

            try
            {
                resultVM = await _SaleService.Insert(Sale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Sale
                };
            }
        }

        // POST: api/Sale/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.Update(Sale);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Sale
                };
            }
        }



        // POST: api/Sale/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
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

        // GET: api/Sale/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Sale
                };
            }
        }

        //GET: api/Sale/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.Dropdown(); // Adjust if Dropdown requires a different method
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


        //POST: api/Sale/MultiplePost
       [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _SaleService = new SaleService();

                resultVM = await _SaleService.MultiplePost(vm);
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


        // POST: api/Sale/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.GetGridData(options, new[] { "H.BranchId", "H.IsPost", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });

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

        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.GetDetailsGridData(options, new[] { "H.BranchId", "H.IsPost", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
                // resultVM = await _saleOrderService.GetGridData(options);
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

        //// POST: api/Sale/SummaryReport
        //[HttpPost("SummaryReport")]
        //public async Task<ResultVM> SummaryReport(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        _SaleService = new SaleService();
        //        PeramModel param = new PeramModel();
        //        param.FromDate = vm.FromDate;
        //        param.ToDate = vm.ToDate;
        //        resultVM = await _SaleService.SummaryReport(new[] { "M.BranchId", "M.IsPost" }, new[] { vm.BranchId, vm.IsPost }, param);
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

        // POST: api/Sale/ReportPreview
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

                _SaleService = new SaleService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _SaleService.ReportPreview(new[] { "M.Id", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetSaleOrder", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "SaleOrder.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }


        // POST: api/Sale/GetSaleDetailDataById
        [HttpPost("GetSaleDetailDataById")]
        public async Task<ResultVM> GetSaleDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.GetSaleDetailDataById(options, masterId);
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


        // POST: api/Purchase/SaleList
        [HttpPost("SaleList")]
        public async Task<ResultVM> SaleList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.SaleList(vm.IDs);
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

        // POST: api/Purchase/SaleListForPayment
        [HttpPost("SaleListForPayment")]
        public async Task<ResultVM> SaleListForPayment(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.SaleListForPayment(vm.IDs);
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


        // POST: api/Purchase/FromSaleGridData
        [HttpPost("FromSaleGridData")]
        public async Task<ResultVM> FromSaleGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _SaleService = new SaleService();
                resultVM = await _SaleService.FromSaleGridData(options);
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
