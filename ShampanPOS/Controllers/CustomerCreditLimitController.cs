using Microsoft.AspNetCore.Mvc;
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
using static System.Net.WebRequestMethods;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerCreditLimitController : ControllerBase
    {
        CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
        // POST: api/CustomerCreditLimit/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CustomerCreditLimitVM customerCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();

            try
            {
                resultVM = await _customerCreditLimitService.Insert(customerCreditLimit);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = customerCreditLimit
                };
            }
        }

        // POST: api/CustomerCreditLimit/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CustomerCreditLimitVM customerCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                resultVM = await _customerCreditLimitService.Update(customerCreditLimit);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = customerCreditLimit
                };
            }
        }

        // POST: api/CustomerCreditLimit/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CustomerCreditLimitVM customerCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();

                string?[] IDs = null;
                IDs = new string?[] { customerCreditLimit.Id.ToString() };

                resultVM = await _customerCreditLimitService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = customerCreditLimit
                };
            }
        }

        // POST: api/CustomerCreditLimit/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CustomerCreditLimitVM customerCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                resultVM = await _customerCreditLimitService.List(new[] { "M.Id" }, new[] { customerCreditLimit.Id.ToString() }, null);
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

        // GET: api/CustomerCreditLimit/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CustomerCreditLimitVM customerCreditLimit)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                resultVM = await _customerCreditLimitService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/CustomerCreditLimit/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                resultVM = await _customerCreditLimitService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/CustomerCreditLimit/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options, string CustomerId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _customerCreditLimitService = new CustomerCreditLimitService();
                resultVM = await _customerCreditLimitService.GetGridData(options, CustomerId);
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


        // POST: api/CustomerCreditLimit/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();

               

                resultVM = await _customerCreditLimitService.MultiplePost(vm);
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



        // POST: api/CustomerCreditLimit/SummaryReport
        [HttpPost("SummaryReport")]
        public async Task<ResultVM> SummaryReport(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                PeramModel param = new PeramModel();
                param.FromDate = vm.FromDate;
                param.ToDate = vm.ToDate;
                resultVM = await _customerCreditLimitService.SummaryReport(new[] { "c.BranchId", "c.IsPost" }, new[] { vm.BranchId, vm.IsPost.ToLower() == "false" ? "0" : "1" }, param);
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


        // POST: api/CustomerCreditLimit/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            CommonService _common = new CommonService();
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
                       //baseUrl = "http://localhost:11910/";
                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                CustomerCreditLimitService _customerCreditLimitService = new CustomerCreditLimitService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _customerCreditLimitService.ReportPreview(new[] { "c.Id", "c.BranchId", "c.InvoiceDateTime between", "c.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/CustomerCreditLimit/GetCustomerCreditLimit", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "CustomerCreditLimit.pdf"
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
