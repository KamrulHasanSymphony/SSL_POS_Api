using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShampanPOS.Repository;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.Configuration;
using System;
using System.Data;
using System.Threading.Tasks;
using static ShampanPOS.Configuration.HttpRequestHelper;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        CampaignService _campaignService = new CampaignService();

        // POST: api/Campaign/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignVM Campaign)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _campaignService = new CampaignService();

            try
            {
                resultVM = await _campaignService.Insert(Campaign);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = Campaign
                };
            }
        }

        // POST: api/Campaign/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.Update(Sale);
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



        // POST: api/Campaign/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.List(new[] { "H.Id" }, new[] { vm.Id }, null);
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

        // GET: api/Campaign/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignVM Sale)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        //GET: api/Campaign/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();
                resultVM = await _campaignService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/Campaign/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.MultiplePost(vm);
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


        // POST: api/Campaign/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options,string EnumId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.GetGridData(options, EnumId, new[] { "H.BranchId", "H.IsPost", "H.CampaignStartDate between", "H.CampaignStartDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/Campaign/GetGridData
        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string EnumId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _campaignService = new CampaignService();

                resultVM = await _campaignService.GetDetailsGridData(options, EnumId, new[] { "H.BranchId", "H.IsPost", "H.CampaignStartDate between", "H.CampaignStartDate between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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



        //// POST: api/Campaign/ReportPreview
        //[HttpPost("ReportPreview")]
        //public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string baseUrl = "http://localhost:11909/";


        //        _campaignService = new CampaignService();

        //        PeramModel peramModel = new PeramModel();
        //        peramModel.CompanyId = vm.CompanyId;

        //        resultVM = await _campaignService.List(new[] { "H.Id" }, new[] { vm.Id }, null);
        //        if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
        //        {
        //            string json = Extensions.DataTableToJson(dt);
        //            HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

        //            var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
        //            {
        //                ApiKey = DatabaseHelper.GetKey(),
        //                PathName = baseUrl
        //            });

        //            var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetSaleOrder", authModel, json);

        //            if (stream == null)
        //            {
        //                throw new Exception("Failed to generate report.");
        //            }

        //            return new FileStreamResult(stream, "application/pdf")
        //            {
        //                FileDownloadName = "SaleOrder.pdf"
        //            };
        //        }

        //        throw new Exception("No data found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error generating report: {ex.Message}");
        //    }
        //}
        //[HttpPost("ReportPreview")]
        //public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        //{
        //    CommonService _common = new CommonService();
        //    ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string baseUrl = "http://localhost:11910/";
        //        //string baseUrl = "";

        //        //settingResult = await _common.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { "DMSReportUrl", "DMSReportUrl" }, null);

        //        //if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingValue)
        //        //{
        //        //    if (settingValue.Rows.Count > 0)
        //        //    {
        //        //        baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
        //        //    }
        //        //}
        //        //if (string.IsNullOrEmpty(baseUrl))
        //        //{
        //        //    throw new Exception("Report API Url Not Found!");
        //        //}

        //        _campaignService = new CampaignService();
        //        PeramModel peramModel = new PeramModel();
        //        peramModel.CompanyId = vm.CompanyId;
        //        //var resultVM = await _campaignService.List(new[] { "M.Id", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);
        //        var resultVM = await _campaignService.List(new[] { "H.Id" }, new[] { vm.Id }, null);
        //        if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
        //        {
        //            string json = Extensions.DataTableToJson(dt);
        //            HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

        //            var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
        //            {
        //                ApiKey = DatabaseHelper.GetKey(),
        //                PathName = baseUrl
        //            });

        //            var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetCampaignUnitPrice", authModel, json);

        //            if (stream == null)
        //            {
        //                throw new Exception("Failed to generate report.");
        //            }

        //            return new FileStreamResult(stream, "application/pdf")
        //            {
        //                FileDownloadName = "SaleOrder.pdf"
        //            };
        //        }

        //        throw new Exception("No data found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error generating report: {ex.Message}");
        //    }
        //}




        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            CommonService _common = new CommonService();
            ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                // Get Report API Base URL
                string baseUrl = "";
                settingResult = await _common.SettingsValue(
                    new[] { "SettingGroup", "SettingName" },
                    new[] { "DMSReportUrl", "DMSReportUrl" },
                    null
                );

                if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingValue && settingValue.Rows.Count > 0)
                {
                    baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
                }

                if (string.IsNullOrEmpty(baseUrl))
                    throw new Exception("Report API Url Not Found!");


                //baseUrl = "http://localhost:11910/";

                CampaignService _campaignService = new CampaignService();
                var resultVM = await _campaignService.List(new[] { "H.Id" }, new[] { vm.Id }, null);

                if (resultVM.Status != "Success" || resultVM.DataVM == null)
                    throw new Exception("No data found.");

                // Deserialize data from result
                var json = JsonConvert.SerializeObject(resultVM.DataVM);
                var data = JsonConvert.DeserializeObject<List<CampaignVM>>(json);

                if (data == null || data.Count == 0)
                    throw new Exception("Invalid campaign data.");

                // Prepare final JSON for report API
                var master = data.FirstOrDefault();
                var details = master.campaignDetailByProductValue
                    .Select(d => new
                    {
                        d.Id,
                        d.CustomerName,
                        d.ProductName,
                        FromQuantity = ((int)d.FromQuantity).ToString(),
                        DiscountRateBasedOnUnitPrice = ((int)d.DiscountRateBasedOnUnitPrice).ToString()
                    })
                    .ToList();

                var finalJson = JsonConvert.SerializeObject(new
                {
                    CampaignMaster = master,
                    CampaignDetails = details
                });

                // Make report request
                HttpRequestHelper httpRequestHelper = new HttpRequestHelper();
                var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                {
                    ApiKey = DatabaseHelper.GetKey(),
                    PathName = baseUrl
                });

                var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetCampaignUnitPrice", authModel, finalJson);

                if (stream == null)
                    throw new Exception("Failed to generate report.");

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = "CampaignReport.pdf"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }
    }
}
