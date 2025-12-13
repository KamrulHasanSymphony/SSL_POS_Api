using Microsoft.AspNetCore.Mvc;
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
    public class ProductController : ControllerBase
    {
        ProductService _productService = new ProductService();
        CommonService _common = new CommonService();
        // POST: api/Product/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(ProductVM product)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _productService = new ProductService();

            try
            {
                resultVM = await _productService.Insert(product);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = product
                };
            }
        }

        // POST: api/Product/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(ProductVM product)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.Update(product);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = product
                };
            }
        }

        // POST: api/Product/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _productService = new ProductService();

                resultVM = await _productService.Delete(vm);
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

        // POST: api/Product/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
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

        // GET: api/Product/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(ProductVM product)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = product
                };
            }
        }

        // GET: api/Product/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.Dropdown(); // Adjust if Dropdown requires a different method
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


        // POST: api/Product/GetProductModalData
        [HttpPost("GetProductModalData")]
        public async Task<ResultVM> GetProductModalData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                PeramModel vm = new PeramModel();
                vm = model.PeramModel;

                resultVM = await _productService.GetProductModalData(new[] { "P.Code like", "P.Name like", "P.BanglaName like", "P.HSCodeNo like", "PG.Name like", "UOM.Name like" }, new[] { model.ProductCode, model.ProductName, model.BanglaName, model.HSCodeNo, model.ProductGroupName, model.UOMName }, vm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }


        // POST: api/Product/GetUOMFromNameData
        [HttpPost("GetUOMFromNameData")]
        public async Task<ResultVM> GetUOMFromNameData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();

                resultVM = await _productService.GetUOMFromNameData(new[] { "M.FromId", "uom.Name", "M.ConversationFactor like" }, new[] { model.UOMId.ToString(), model.UOMFromName.ToString(), model.UOMConversion.ToString() }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }


        // POST: api/Product/GetProductGroupModalData
        [HttpPost("GetProductGroupModalData")]
        public async Task<ResultVM> GetProductGroupModalData(ProductDataVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.GetProductGroupModalData(new[] { "" }, new[] { "" }, null);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }


        // POST: api/Product/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _productService = new ProductService();
                resultVM = await _productService.GetGridData(options);
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


        // POST: api/Product/ReportPreview
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

                _productService = new ProductService();
                PeramModel peramModel = new PeramModel();
                peramModel.CompanyId = vm.CompanyId;
                var resultVM = await _productService.ReportPreview(new[] { "H.Id", "H.BranchId" }, new[] { vm.Id, vm.BranchId}, peramModel);

                if (resultVM.Status == "Success" && resultVM.DataVM is DataTable dt && dt.Rows.Count > 0)
                {
                    string json = Extensions.DataTableToJson(dt);
                    HttpRequestHelper httpRequestHelper = new HttpRequestHelper();

                    var authModel = httpRequestHelper.GetAuthentication(new CredentialModel
                    {
                        ApiKey = DatabaseHelper.GetKey(),
                        PathName = baseUrl
                    });

                    var stream = httpRequestHelper.PostDataReport(baseUrl + "/api/Product/GetProduct", authModel, json);

                    if (stream == null)
                    {
                        throw new Exception("Failed to generate report.");
                    }

                    return new FileStreamResult(stream, "application/pdf")
                    {
                        FileDownloadName = "Product.pdf"
                    };
                }

                throw new Exception("No data found.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }


        // POST: api/Purchase/ExportProductExcel
        [HttpPost("ExportProductExcel")]
        public async Task<ResultVM> ExportProductExcel(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductService _service = new ProductService();
                resultVM = await _service.ExportProductExcel(vm);
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


        [HttpPost("ExportProductPursaseExcel")]
        public async Task<ResultVM> ExportProductPursaseExcel(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductService _service = new ProductService();
                resultVM = await _service.ExportProductPursaseExcel(vm);
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
        // POST: api/Product/ImportExcelFileInsert
        [HttpPost("ImportExcelFileInsert")]
        public async Task<ResultVM> ImportExcelFileInsert(ProductVM model)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ProductService _service = new ProductService();

            try
            {

                CommonRepository _commonRepo = new CommonRepository();

                resultVM = await _service.ImportExcelFileInsert(model);

                return resultVM;

            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = model
                };
            }
        }
        // GET: api/Product/UploadImage
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

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Product");

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

                string relativePath = $"/Uploads/Product/{fileName}";

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
        
        // GET: api/Product/PreviewImage
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


        // POST: api/Purchase/ExportProductStockExcel
        [HttpPost("ExportProductStockExcel")]
        public async Task<ResultVM> ExportProductStockExcel(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                ProductService _service = new ProductService();
                resultVM = await _service.ExportProductStockExcel(vm);
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

        // 

        [HttpPost("PurchaseImportExcelFileInsert")]
        public async Task<ResultVM> PurchaseImportExcelFileInsert(ProductPriceGroupVM productPriceGroup)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ProductService _service = new ProductService();

            try
            {
                resultVM = await _service.PurchaseImportExcelFileInsert(productPriceGroup);
                return resultVM;
            }

            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = productPriceGroup
                };
            }

        }

    }
}
