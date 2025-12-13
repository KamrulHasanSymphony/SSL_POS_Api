using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalePersonMonthlyAchievementController : Controller
    {
        SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(SalePersonMonthlyAchievementVM achievement)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();

            try
            {
                resultVM = await _salePersonMonthlyAchievementService.Insert(achievement);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = achievement
                };
            }
        }

        // POST: api/SalePersonMonthlyAchievement/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(SalePersonMonthlyAchievementVM achievement)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.Update(achievement);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = achievement
                };
            }
        }

        // POST: api/SalePersonMonthlyAchievement/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.Delete(vm);
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

        // POST: api/SalePersonMonthlyAchievement/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.List(new[] { "M.Id" }, new[] { vm.Id.ToString() }, null);
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

        // GET: api/SalePersonMonthlyAchievement/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(SalePersonMonthlyAchievementVM achievement)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/SalePersonMonthlyAchievement/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.Dropdown(); // Adjust if Dropdown requires a different method
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

        // POST: api/SalePersonMonthlyAchievement/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                SalePersonMonthlyAchievementService _salePersonMonthlyAchievementService = new SalePersonMonthlyAchievementService();
                resultVM = await _salePersonMonthlyAchievementService.GetGridData(options);
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

        [HttpPost("ProcessData")]
        public async Task<ActionResult<ResultVM>> ProcessData(SalePersonMonthlyAchievementDataVM model)
        {
            // Initialize a default ResultVM with failure status
            var resultVM = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            try
            {

                // Call the service with the VM
                resultVM = await _salePersonMonthlyAchievementService.ProcessDataInsert(model);

                // Return the result based on the service call
                if (resultVM.Status == "Success")
                {
                    return Ok(resultVM); // Return 200 OK if success
                }
                else
                {
                    return StatusCode(500, resultVM); // Return 500 Internal Server Error if failure
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM
                {
                    Status = "Fail",
                    Message = "An unexpected error occurred.",
                    ExMessage = ex.Message,
                    DataVM = null
                });
            }
        }
    }
}
