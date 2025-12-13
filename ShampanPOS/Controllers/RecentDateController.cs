using Microsoft.AspNetCore.Diagnostics;
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

namespace ShampanPOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecentDateController : ControllerBase
    {
        private readonly IRecentDateService _recentDateService;

        public RecentDateController(IRecentDateService recentDateService)
        {
            _recentDateService = recentDateService;
        }

        [HttpGet]
        public ActionResult<RecentDateVM> GetRecentDate()
        {
            try
            {
                var recentDate = _recentDateService.GetRecentDate();
                return Ok(recentDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
