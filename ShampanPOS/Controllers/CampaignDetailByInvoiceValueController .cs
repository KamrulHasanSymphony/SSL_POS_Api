using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
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
    public class CampaignDetailByInvoiceValueController : ControllerBase
    {

        CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
        CommonService _common = new CommonService();
        // POST: api/CampaignDetailByInvoiceValue/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();

            try
            {
                resultVM = await _campaignDetailService.Insert(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.Update(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();

                string?[] IDs = null;
                IDs = new string?[] { campaignDetail.Id.ToString() };

                resultVM = await _campaignDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByInvoiceValue/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.List(new[] { "M.Id" }, new[] { campaignDetail.Id.ToString() }, null);
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

        // GET: api/CampaignDetailByInvoiceValue/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignDetailByInvoiceValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/CampaignDetailByInvoiceValue/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByInvoiceValueService _campaignDetailService = new CampaignDetailByInvoiceValueService();
                resultVM = await _campaignDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/CampaignDetailByInvoiceValue/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            _common = new CommonService();
            ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string baseUrl = "http://localhost:11909/";
                //string baseUrl = "";

                //settingResult = await _common.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { "DMSReportUrl", "DMSReportUrl" }, null);

                //if (settingResult.Status == "Success" && settingResult.DataVM is DataTable settingValue)
                //{
                //    if (settingValue.Rows.Count > 0)
                //    {
                //        baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
                //    }
                //}
                //if (string.IsNullOrEmpty(baseUrl))
                //{
                //    throw new Exception("Report API Url Not Found!");
                //}

                _campaignDetailService = new CampaignDetailByInvoiceValueService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _campaignDetailService.ReportPreview(new[] { "M.Id", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetCampaignDetailByInvoiceValue", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "CampaignDetailByInvoiceValue.pdf"
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
