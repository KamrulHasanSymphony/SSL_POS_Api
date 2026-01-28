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

    public class MasterItemController : ControllerBase
    {
        MasterItemService _masterItemService = new MasterItemService();
        CommonService _common = new CommonService();

        // POST: api/MasterItem/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(MasterItemVM masteritem)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _masterItemService = new MasterItemService();

            try
            {
                resultVM = await _masterItemService.Insert(masteritem);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = masteritem
                };
            }
        }

        // POST: api/MasterItem/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(MasterItemVM masteritem)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();
                resultVM = await _masterItemService.Update(masteritem);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = masteritem
                };
            }
        }

        // POST: api/MasterItem/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();

                resultVM = await _masterItemService.Delete(vm);
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

        // POST: api/MasterItem/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();
                resultVM = await _masterItemService.List(new[] { "M.Id" }, new[] { vm.Id }, null);
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

        // GET: api/MasterItem/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(MasterItemVM masteritem)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();
                resultVM = await _masterItemService.ListAsDataTable(new[] { "" }, new[] { "" });
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = masteritem
                };
            }
        }

        // GET: api/MasterItem/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();
                resultVM = await _masterItemService.Dropdown(); // Adjust if Dropdown requires a different method
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




        // POST: api/MasterItem/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _masterItemService = new MasterItemService();
                resultVM = await _masterItemService.GetGridData(options);
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

        // GET: api/MasterItem/PreviewImage
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

    }
}
