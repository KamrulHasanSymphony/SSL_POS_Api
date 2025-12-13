using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class CustomerJournalController : ControllerBase
    {
        CustomerJournalService _customerJournalService = new CustomerJournalService();
        CommonService _common = new CommonService();

        // POST: api/CustomerJournal/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            _customerJournalService = new CustomerJournalService();
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
                        //baseUrl = settingValue.Rows[0]["SettingValue"].ToString();

                        baseUrl = "http://localhost:11903";
                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                _customerJournalService = new CustomerJournalService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _customerJournalService.GetCustomerJournal(new[] { "CustomerId" }, new[] { vm.Id });

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/CustomerJournal/GetCustomerJournal", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "CustomerJournal.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }





            //_customerJournalService = new CustomerJournalService();
            //_common = new CommonService();

            //string baseUrl = "http://localhost:11903"; // Report API URL

            //var resultVM = await _customerJournalService.GetCustomerJournal(new[] { "CustomerId" }, new[] { vm.Id });

            //if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt)
            //{
            //    string json = Extensions.DataTableToJson(dt);
            //    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();
            //    var stream = httpRequestHelper.PostDataReport($"{baseUrl}/api/CustomerJournal/GetCustomerJournal", json);

            //    return new FileStreamResult(stream, "application/pdf")
            //    {
            //        FileDownloadName = "CustomerJournal.pdf"
            //    };
            //}

            //throw new Exception("No data found.");
        }
    }
}
