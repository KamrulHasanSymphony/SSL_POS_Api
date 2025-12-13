using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShampanPOS.uploads;

namespace ShampanPOS.Uploads
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        public IActionResult UploadFile(IFormFile file) {
        return Ok(new UploadHandler().Upload(file));
        }
    }
}
