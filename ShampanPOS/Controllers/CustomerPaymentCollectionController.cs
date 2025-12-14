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
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static ShampanPOS.Configuration.HttpRequestHelper;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class CustomerPaymentCollectionController : ControllerBase
    {
        CustomerPaymentCollectionService _customerPaymentCollectionService = new CustomerPaymentCollectionService();
        CommonService _common = new CommonService();
        //POST: api/CustomerPaymentCollection/Insert
       [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CustomerPaymentCollectionVM CustomerPaymentCollection)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _customerPaymentCollectionService = new CustomerPaymentCollectionService();

            try
            {
                resultVM = await _customerPaymentCollectionService.Insert(CustomerPaymentCollection);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = CustomerPaymentCollection
                };
            }
        }
        //[HttpPost("Insert")]
        //public async Task<ResultVM> Insert()
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error" };
        //    _customerPaymentCollectionService = new CustomerPaymentCollectionService();

        //    try
        //    {
        //        var httpRequest = HttpContext.Request;

        //        var modelJson = httpRequest.Form["model"];
        //        var model = JsonConvert.DeserializeObject<CustomerPaymentCollectionVM>(modelJson);

        //        var file = httpRequest.Form.Files["file"];
        //        if (file != null && file.Length > 0)
        //        {
        //            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Content/CustomerPaymentCollection");
        //            if (!Directory.Exists(uploadsFolder))
        //                Directory.CreateDirectory(uploadsFolder);

        //            string fileExtension = Path.GetExtension(file.FileName).ToLower();
        //            string[] validImageTypes = { ".jpg", ".jpeg", ".png", ".gif" };

        //            if (!validImageTypes.Contains(fileExtension))
        //            {
        //                return new ResultVM { Status = "Fail", Message = "Invalid image file type." };
        //            }

        //            string fileName = Guid.NewGuid().ToString() + fileExtension;
        //            string filePath = Path.Combine(uploadsFolder, fileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            model.ImagePath = "/Content/CustomerPaymentCollection/" + fileName;
        //        }

        //        resultVM = await _customerPaymentCollectionService.Insert(model);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM { Status = "Fail", Message = ex.Message };
        //    }
        //}



        // POST: api/CustomerPaymentCollection/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CustomerPaymentCollectionVM CustomerPaymentCollection)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                resultVM = await _customerPaymentCollectionService.Update(CustomerPaymentCollection);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = CustomerPaymentCollection
                };
            }
        }
        //public async Task<ResultVM> Update([FromForm] CustomerPaymentCollectionVM CustomerPaymentCollection)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        _customerPaymentCollectionService = new CustomerPaymentCollectionService();
        //        resultVM = await _customerPaymentCollectionService.Update(CustomerPaymentCollection);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = "Data not updated.",
        //            ExMessage = ex.Message,
        //            DataVM = CustomerPaymentCollection
        //        };
        //    }
        //}

        // POST: api/CustomerPaymentCollection/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                resultVM = await _customerPaymentCollectionService.GetGridData(options);
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
        // POST: api/CustomerPaymentCollection/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                resultVM = await _customerPaymentCollectionService.Delete(vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = vm
                };
            }
        }

        // POST: api/CustomerPaymentCollection/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                resultVM = await _customerPaymentCollectionService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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

        // POST: api/CustomerPaymentCollection/ReportPreview
        [HttpPost("ReportPreview")]
        public async Task<FileStreamResult> ReportPreview(CommonVM vm)
        {
            _customerPaymentCollectionService = new CustomerPaymentCollectionService();
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
                         

                    }
                }
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Report API Url Not Found!");
                }

                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _customerPaymentCollectionService.ReportPreview(new[] { "M.Id", "M.BranchId" }, new[] { vm.Id, vm.BranchId }, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/CustomerPaymentCollection/GetCustomerPaymentCollection", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "CustomerPaymentCollection.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }

        // GET: api/CustomerPaymentCollection/ReportPreview
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

                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
                PeramModel peramModel = new PeramModel();



                // Call the ReportPreview service with query parameters
                var resultVM = await _customerPaymentCollectionService.ReportPreview(
                    new[] { "M.Id" },
                    new[] { id},
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
                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/CustomerPaymentCollection/GetCustomerPaymentCollection", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    // Return the PDF file as a stream
                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "CustomerPaymentCollection.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }

        }

        // GET: api/CustomerPaymentCollection/GetDetailsGridData
        [HttpPost("GetDetailsGridData")]
        public async Task<ResultVM> GetDetailsGridData(GridOptions options)
        {
            if (options == null || options.vm == null)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Invalid request options.",
                    DataVM = null
                };
            }

            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            try
            {
                // Assuming you are using dependency injection
                resultVM = await _customerPaymentCollectionService.GetDetailsGridData(
                    options,
                    new[] { "H.CustomerId", "H.TransactionDate between", "H.TransactionDate between" },
                    new[] { options.vm.CustomerId.ToString(), options.vm.FromDate.ToString(), options.vm.ToDate.ToString() }
                );

                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString(),
                    DataVM = null
                };
            }
        }
        // GET: api/CustomerPaymentCollection/UploadImage
        [HttpPost("UploadImage")]
        public async Task<ResultVM> UploadImage(IFormFile imageFile)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", Id = "0", DataVM = null };

            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    resultVM.Message = "No file uploaded.";
                    return resultVM;
                }

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "CustomerPaymentCollection");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };

                if (!validExtensions.Contains(fileExtension))
                {
                    resultVM.Message = "Invalid file type. Only JPG, PNG, and PDF are allowed.";
                    return resultVM;
                }

                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                string relativePath = $"/Uploads/CustomerPaymentCollection/{fileName}";

                resultVM.Status = "Success";
                resultVM.Message = "File uploaded successfully.";
                resultVM.DataVM = new { ImagePath = relativePath };

                return resultVM;
            }
            catch (Exception ex)
            {
                resultVM.ExMessage = ex.ToString();
                resultVM.Message = "Image upload failed.";
                return resultVM;
            }
        }
        // GET: api/CustomerPaymentCollection/PreviewImage
        [HttpGet("PreviewImage")]
        public IActionResult PreviewImage([FromQuery] string relativePath)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Image not found" };

            if (string.IsNullOrWhiteSpace(relativePath))
                return BadRequest("Path is required.");

            try
            {
                relativePath = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound(result);

                var contentType = GetContentType(fullPath);
                var imageBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM
                {
                    Status = "Fail",
                    Message = "Error loading image",
                    ExMessage = ex.Message
                });
            }
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }


        // POST: api/CustomerPaymentCollection/MultiplePost
        [HttpPost("MultiplePost")]
        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _customerPaymentCollectionService = new CustomerPaymentCollectionService();

                resultVM = await _customerPaymentCollectionService.MultiplePost(vm);
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



        //[HttpPost("MultiplePaymentSettlementProcess")]
        //public async Task<ResultVM> MultiplePaymentSettlementProcess(CommonVM vm)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
        //    try
        //    {
        //        _customerPaymentCollectionService = new CustomerPaymentCollectionService();

        //        resultVM = await _customerPaymentCollectionService.MultiplePaymentSettlementProcess(vm);
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



//        [HttpPost("GetTabGridData")]
//        public async Task<ResultVM> GetTabGridData(GridOptions options)
//        {
//            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
//            try
//            {

//                _customerPaymentCollectionService = new CustomerPaymentCollectionService();
//                List<string> conditionFields = new List<string>
//{
//                "S.BranchId"
//};

//                List<string> conditionValues = new List<string>
//{
//                options.vm.BranchId.ToString()
//};
//                string[] finalConditionFields = conditionFields.ToArray();
//                string[] finalConditionValues = conditionValues.ToArray();


//                resultVM = await _customerPaymentCollectionService.GetTabGridData(options, finalConditionFields, finalConditionValues);
//                return resultVM;
//            }
//            catch (Exception ex)
//            {
//                return new ResultVM
//                {
//                    Status = "Fail",
//                    Message = ex.Message,
//                    ExMessage = ex.Message,
//                    DataVM = null
//                };
//            }
//        }




    }
}
