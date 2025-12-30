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
    public class BankInformationController : ControllerBase
    {

        BankInformationService _BankInformationService = new BankInformationService();
        CommonService _common = new CommonService();

        // POST: api/BankInformation/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(BankInformationVM bankinfo)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            BankInformationService _BankInformationService = new BankInformationService();

            try
            {
                resultVM = await _BankInformationService.Insert(bankinfo);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = bankinfo
                };
            }
        }

        // POST: api/BankInformation/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(BankInformationVM bankinfo)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BankInformationService _BankInformationService = new BankInformationService();
                resultVM = await _BankInformationService.Update(bankinfo);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = bankinfo
                };
            }
        }

        // POST: api/BankInformation/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _BankInformationService = new BankInformationService();

                resultVM = await _BankInformationService.Delete(vm);
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

        // POST: api/BankInformation/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {


                string[] conditionFields = null;
                string[] conditionValues = null;
                conditionFields = vm.ConditionalFields;
                conditionValues = vm.ConditionalValues;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { vm.Id };
                }
                BankInformationService _BankInformationService = new BankInformationService();


                resultVM = await _BankInformationService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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


        //// POST: api/BankInformation/List
        //[HttpPost("GetCustomerListByRoute")]
        //public async Task<ResultVM> GetCustomerListByRoute(CommonVM customer)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        string[] conditionFields = null;
        //        string[] conditionValues = null;

        //        conditionFields = customer.ConditionalFields;
        //        conditionValues = customer.ConditionalValues;

        //        BankInformationService _BankInformationService = new BankInformationService();
        //        resultVM = await _BankInformationService.GetCustomerListByRoute(conditionFields, conditionValues, null);
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


        // GET: api/BankInformation/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(BankInformationVM bankinfo)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BankInformationService _BankInformationService = new BankInformationService();
                resultVM = await _BankInformationService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/BankInformation/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                BankInformationService _BankInformationService = new BankInformationService();
                resultVM = await _BankInformationService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/BankInformation/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _BankInformationService = new BankInformationService();
                resultVM = await _BankInformationService.GetGridData(options, null, null);
                //resultVM = await _CustomerService.GetGridData(options);
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


        // POST: api/BankInformation/ReportPreview
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

                _BankInformationService = new BankInformationService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _BankInformationService.ReportPreview(new[] { "H.Id", "H.BranchId" }, new[] { vm.Id, vm.BranchId }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Customer/GetCustomer", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "Customer.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }

        //[HttpPost("DevitCredit")]
        //public async Task<ResultVM> DevitCredit(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        _BankInformationService = new BankInformationService();
        //        PeramModel param = new PeramModel();
        //        param.FromDate = vm.FromDate;
        //        param.ToDate = vm.ToDate;
        //        resultVM = await _BankInformationService.DevitCredit(new[] { "c.BranchId" }, new[] { vm.BranchId }, param);
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
    }
}
