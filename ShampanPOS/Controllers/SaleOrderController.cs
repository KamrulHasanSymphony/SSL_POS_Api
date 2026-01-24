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
    public class SaleOrderController : ControllerBase
    {
        SaleOrderService _saleOrderService = new SaleOrderService();
        CommonService _common = new CommonService();

        // POST: api/SaleOrder/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SaleOrderVM saleOrder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _saleOrderService = new SaleOrderService();

            try
            {
                resultVM = await _saleOrderService.Insert(saleOrder);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = saleOrder
                };
            }
        }

        // POST: api/SaleOrder/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SaleOrderVM saleOrder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.Update(saleOrder);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = saleOrder
                };
            }
        }

        // POST: api/SaleOrder/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(SaleOrderVM saleOrder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();

                string?[] IDs = null;
                IDs = new string?[] { saleOrder.Id.ToString() };

                resultVM = await _saleOrderService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = saleOrder
                };
            }
        }

        // POST: api/SaleOrder/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }





        // POST: api/SaleOrder/OrderList
        [HttpPost("OrderList")]
        public async Task<ResultVM> OrderList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();

                resultVM = await _saleOrderService.OrderList(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not fetched.",
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }






        // GET: api/SaleOrder/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SaleOrderVM saleOrder)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SaleOrder/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/SaleOrder/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();

                resultVM = await _saleOrderService.MultiplePost(vm);
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

        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {

                List<string> conditionFields = new List<string>
                 {
                     "H.BranchId",
                     "H.OrderDate between",
                     "H.OrderDate between"
                 };

                List<string> conditionValues = new List<string>
                 {
                     options.vm.BranchId.ToString(),
                     options.vm.FromDate.ToString(),
                     options.vm.ToDate.ToString()
                 };

                if (!string.IsNullOrEmpty(options.vm.UserId))
                {
                    conditionFields.Add("H.SalePersonId");
                    conditionValues.Add(options.vm.UserId);

                }

                string[] finalConditionFields = conditionFields.ToArray();
                string[] finalConditionValues = conditionValues.ToArray();

                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.GetGridData(options, finalConditionFields, finalConditionValues);
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

        [HttpPost("GetOrderNoWiseGridData")]
        public async Task<ResultVM> GetOrderNoWiseGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.GetOrderNoWiseGridData(options, new[] { "h.BranchId", "h.InvoiceDateTime between", "h.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.GetDetailsGridData(options, new[] { "H.BranchId", "H.InvoiceDateTime between", "H.InvoiceDateTime between" }, new[] { options.vm.BranchId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() });
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

        // POST: api/SaleOrder/FromSaleOrderGridData
        [HttpPost("FromSaleOrderGridData")]
        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.FromSaleOrderGridData(options);
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

        // POST: api/PurchaseOrder/SaleOrderList
        [HttpPost("SaleOrderList")]
        public async Task<ResultVM> SaleOrderList(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.SaleOrderList(vm.IDs);
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




        // POST: api/SaleOrder/GetSaleOrderDetailDataById
        [HttpPost("GetSaleOrderDetailDataById")]
        public async Task<ResultVM> GetSaleOrderDetailDataById(GridOptions options, int masterId)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _saleOrderService = new SaleOrderService();
                resultVM = await _saleOrderService.GetSaleOrderDetailDataById(options, masterId);
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


        // POST: api/SaleOrder/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            _common = new CommonService();
            ResultVM settingResult = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                //string baseUrl = "http://localhost:11909/";
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

                _saleOrderService = new SaleOrderService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _saleOrderService.ReportPreview(new[] { "M.Id", "M.BranchId", "M.InvoiceDateTime between", "M.InvoiceDateTime between" }, new[] { vm.Id, vm.BranchId, vm.FromDate, vm.ToDate }, peramModel);

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


        // GET: api/SaleOrder/ReportPreview
        [HttpGet("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(string id/*, string branchId*/)
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
                //// Get the logged-in user's BranchId from the session or authentication context
                //string loggedInBranchId = Session["BranchId"]?.ToString(); // Assuming BranchId is stored in the session
                //if (string.IsNullOrEmpty(loggedInBranchId))
                //{
                //    throw new Exception("Logged-in user's BranchId not found.");
                //}

                _saleOrderService = new SaleOrderService();
                PeramModel peramModel = new PeramModel();



                // Call the ReportPreview service with query parameters
                var resultVM = await _saleOrderService.ReportPreview(
                    new[] { "M.Id" },
                    new[] { id/*,branchId*/},
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
                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Sale/GetSaleOrder", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    // Return the PDF file as a stream
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
