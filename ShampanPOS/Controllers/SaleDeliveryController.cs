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
    public class SaleDeliveryController : ControllerBase
    {
        SaleDeliveryService _saleDeliveryService = new SaleDeliveryService();
        CommonService _common = new CommonService();

        // POST: api/SaleDelivery/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleDeliveryVM saleDelivery)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _saleDeliveryService = new SaleDeliveryService();

            try
            {
                resultVM = await _saleDeliveryService.Insert(saleDelivery);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = saleDelivery
                };
            }
        }

        // POST: api/SaleDelivery/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleDeliveryVM saleDelivery)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.Update(saleDelivery);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = saleDelivery
                };
            }
        }

        // POST: api/SaleDelivery/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SaleDeliveryVM saleDelivery)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();

                string?[] IDs = null;
                IDs = new string?[] { saleDelivery.Id.ToString() };

                resultVM = await _saleDeliveryService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = saleDelivery
                };
            }
        }

        // POST: api/SaleDelivery/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
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

        // GET: api/SaleDelivery/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleDeliveryVM saleDelivery)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = saleDelivery
                };
            }
        }

        //GET: api/SaleDelivery/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/SaleDelivery/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();

                resultVM = await _saleDeliveryService.MultiplePost(vm);
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


        // POST: api/SaleDelivery/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.GetGridData(options, new[] { "H.BranchId", "H.IsPost", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
                //resultVM = await _saleDeliveryService.GetGridData(options);
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
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.GetDetailsGridData(options, new[] { "H.BranchId", "H.IsPost", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.IsPost.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/SaleDelivery/FromSaleOrderGridData
        [HttpPost("FromSaleOrderGridData")]
        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.FromSaleOrderGridData(options, new[] { "H.BranchId", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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


        // POST: api/SaleDelivery/SaleDeliveryList
        [HttpPost("SaleDeliveryList")]
        public async Task<ResultVM> SaleDeliveryList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.SaleDeliveryList(new[] { "H.Id" }, new[] { vm.Id }, null);
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


        // POST: api/SaleDelivery/GetSaleDeliveryDetailDataById
        [HttpPost("GetSaleDeliveryDetailDataById")]
        public async Task<ResultVM> GetSaleDeliveryDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.GetSaleDeliveryDetailDataById(options, masterId);
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

        // POST: api/SaleDelivery/SummaryReport
        [HttpPost("SummaryReport")]
        public async Task<ResultVM> SummaryReport(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                PeramModel param = new PeramModel();
                param.FromDate = vm.FromDate;
                param.ToDate = vm.ToDate;
                resultVM = await _saleDeliveryService.SummaryReport(new[] { "M.BranchId", "M.IsPost" }, new[] { vm.BranchId, vm.IsPost }, param);
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

        // POST: api/SaleDelivery/IncoiceReportPreview
        [HttpPost("IncoiceReportPreview")]
        public async Task<FileStreamResult> IncoiceReportPreview(CommonVM vm)
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
                        //baseUrl = "http://localhost:11910/";
                        baseUrl = settingValue.Rows[0]["SettingValue"].ToString();
                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                SaleDeliveryService _saleDeliveryService = new SaleDeliveryService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _saleDeliveryService.IncoiceReportPreview(new[] { "D.SaleDeliveryId", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Invoice/GetSaleOrderInvoice", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "SaleOrderInvoice.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }


        // POST: api/SaleDelivery/ReportPreview
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
                        //baseUrl = "http://localhost:11910/";
                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                _saleDeliveryService = new SaleDeliveryService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _saleDeliveryService.ReportPreview(new[] { "M.Id", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetSaleDelivery", authModel, json);

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

        // GET: api/SaleDelivery/ReportPreview
        [HttpGet("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(string id)
        {
            _common = new CommonService();
            ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string baseUrl = "";

                // Fetch the base URL for the report API
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

                _saleDeliveryService = new SaleDeliveryService();
                PeramModel peramModel = new PeramModel();

                // Call the ReportPreview service with query parameters
                var resultVM = await _saleDeliveryService.ReportPreview(
                    new[] { "M.Id" },
                    new[] { id }, // Passing 'id' as parameter
                    peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    // Get authentication details
                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    // Send the request to the report API
                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetSaleDelivery", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    // Return the PDF file as a stream
                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "SaleDelivery.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }

        [HttpPost("GetDeliveryNoWiseGridData")]
        public async Task<ResultVM> GetDeliveryNoWiseGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.GetDeliveryNoWiseGridData(options, new[] { "H.BranchId", "h.InvoiceDateTime between", "h.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/SaleDelivery/GetSaleDueListByCustomerId
        [HttpPost("GetSaleDueListByCustomerId")]
        public async Task<ResultVM> GetSaleDueListByCustomerId(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleDeliveryService = new SaleDeliveryService();
                resultVM = await _saleDeliveryService.GetSaleDueListByCustomerId(options, new[] { "H.CustomerId" }, new[] { options.vm.Id.ToString() });
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
