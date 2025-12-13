using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.Configuration;
using static ShampanPOS.Configuration.HttpRequestHelper;
using System.Data;
using Newtonsoft.Json;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MISReportController : ControllerBase
    {
        // POST: api/MISReport/ExportSaleAndPayment
        [HttpPost("ExportSaleAndPayment")]
        public async Task<ResultVM> ExportSaleAndPayment(SalePaymentVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ExportSaleAndPayment(vm);
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


        [HttpPost("ExportSaleAndPaymentSummary")]
        public async Task<ResultVM> ExportSaleAndPaymentSummary(SalePaymentVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ExportSaleAndPaymentSummary(vm);
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




        [HttpPost("SaleDeleveryInformation")]
        public async Task<ResultVM> SaleDeleveryInformation(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.SaleDeleveryInformation(vm);
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

        [HttpPost("MonthlySalesAndAmountReportProductWise")]
        public async Task<ResultVM> MonthlySalesAndAmountReportProductWise(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.MonthlySalesAndAmountReportProductWise(vm);
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

        [HttpPost("MonthlySalesAndAmountReportCustomerWise")]
        public async Task<ResultVM> MonthlySalesAndAmountReportCustomerWise(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.MonthlySalesAndAmountReportCustomerWise(vm);
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

        [HttpPost("ProductSalesReport")]
        public async Task<ResultVM> ProductSalesReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ProductSalesReport(vm);
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

        [HttpPost("ProductInventoryReport")]
        public async Task<ResultVM> ProductInventoryReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.ProductInventoryReport(vm);
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

        [HttpPost("CustomerBillReport")]
        public async Task<ResultVM> CustomerBillReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.CustomerBillReport(vm);
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

        [HttpPost("CustomerDueListReport")]
        public async Task<ResultVM> CustomerDueListReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.CustomerDueListReport(vm);
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

        [HttpPost("CustomerDueListCustomerWiseReport")]
        public async Task<ResultVM> CustomerDueListCustomerWiseReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.CustomerDueListCustomerWiseReport(vm);
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

        [HttpPost("SinglePorductInventoryReport")]
        public async Task<ResultVM> SinglePorductInventoryReport(MISReportVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                MISReportService _service = new MISReportService();
                resultVM = await _service.SinglePorductInventoryReport(vm);
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

        


        [HttpPost("SaleDeleveryReportPreview")]
        public async Task<FileStreamResult> SaleDeleveryReportPreview(MISReportVM vm)
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

                MISReportService _service = new MISReportService();

                var resultVM = await _service.SaleDeleveryInformation( vm);

                if (resultVM.Status == "Success" && resultVM.DataVM != null)
                {
                    var json = JsonConvert.SerializeObject(resultVM.DataVM);
                    var data = JsonConvert.DeserializeObject<List<SaleDeleveryInformationVM>>(json);
                    //var dt = Extensions.ConvertToDataTable(data);
                    //string json = Extensions.DataTableToJson(dt);
                    string BranchName = "";
                    string CustomerName = "";
                    if (string.IsNullOrWhiteSpace(vm.BranchId))
                    {
                        BranchName = "ALL";
                    }
                    else
                    { 
                        if(data.Count()>0)
                        {
                            BranchName = data.FirstOrDefault().BranchName ?? "ALL";
                        }
                    }
                    if (string.IsNullOrWhiteSpace(vm.CustomerId))
                    {
                         CustomerName = "ALL";
                    }
                    else
                    {
                        if (data.Count() > 0)
                        {
                            CustomerName = data.FirstOrDefault().CustomerName ?? "ALL";
                        }
                    }
                    var Finaljson = JsonConvert.SerializeObject(new
                        {


                            SaleData = resultVM.DataVM,

                            UtilityVm = new { CustomerName = CustomerName, BranchName= BranchName , DateFrom = vm.DateFrom, DateTo= vm.DateTo }
                        });
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });
                

        
                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/MISReport/GetSaleDeleveryInfo", authModel, Finaljson);

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

        [HttpPost("ProductLeatestPricePreview")]
        public async Task<FileStreamResult> ProductLeatestPricePreview(MISReportVM vm)
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


                //string baseUrl = "http://localhost:11910/";

                MISReportService _service = new MISReportService();

                //var resultVM = await _service.SaleDeleveryInformation( vm);
                var resultVM = await _service.ProductLeatestPriceInformation( vm);

                if (resultVM.Status == "Success" && resultVM.DataVM != null)
                {
                    var json = JsonConvert.SerializeObject(resultVM.DataVM);
                    var data = JsonConvert.DeserializeObject<List<ProductBatchHistoryVM>>(json);
                    //var dt = Extensions.ConvertToDataTable(data);
                    //string json = Extensions.DataTableToJson(dt);
                    string BranchName = "";
                    //string CustomerName = "";
                    if (string.IsNullOrWhiteSpace(vm.BranchId))
                    {
                        BranchName = "ALL";
                    }
                    else
                    { 
                        if(data.Count()>0)
                        {
                            BranchName = data.FirstOrDefault().BranchName ?? "ALL";
                        }
                    }
                    //if (string.IsNullOrWhiteSpace(vm.CustomerId))
                    //{
                    //     CustomerName = "ALL";
                    //}
                    //else
                    //{
                    //    if (data.Count() > 0)
                    //    {
                    //        CustomerName = data.FirstOrDefault().CustomerName ?? "ALL";
                    //    }
                    //}
                    var Finaljson = JsonConvert.SerializeObject(new
                        {


                            ProductPriceData = resultVM.DataVM,

                            UtilityVm = new { BranchName= BranchName}
                        });
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });
                

        
                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/MISReport/GetProductLeatestPriceInfo", authModel, Finaljson);

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

    }
}
