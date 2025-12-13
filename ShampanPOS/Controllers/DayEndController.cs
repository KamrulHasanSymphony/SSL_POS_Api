using Microsoft.AspNetCore.Mvc;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel;
using ShampanPOS.Service;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DayEndController : ControllerBase
    {
        DayEndService _service = new DayEndService();

        [HttpPost("ProcessData")]
        public async Task<ActionResult<ResultVM>> ProcessData(DayEndHeadersVM model)
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
                resultVM = await _service.ProcessDataInsert(model);

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
