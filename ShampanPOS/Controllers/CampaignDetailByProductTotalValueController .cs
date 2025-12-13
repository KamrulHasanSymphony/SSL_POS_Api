using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using System;
using System.Threading.Tasks;

namespace ShampanPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignDetailByProductTotalValueController : ControllerBase
    {
        // POST: api/CampaignDetailByProductTotalValue/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignDetailByProductTotalValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();

            try
            {
                resultVM = await _campaignDetailService.Insert(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByProductTotalValue/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignDetailByProductTotalValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();
                resultVM = await _campaignDetailService.Update(campaignDetail);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByProductTotalValue/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CampaignDetailByProductTotalValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();

                string?[] IDs = null;
                IDs = new string?[] { campaignDetail.Id.ToString() };

                resultVM = await _campaignDetailService.Delete(IDs);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not deleted.",
                    ExMessage = ex.Message,
                    DataVM = campaignDetail
                };
            }
        }

        // POST: api/CampaignDetailByProductTotalValue/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CampaignDetailByProductTotalValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();
                resultVM = await _campaignDetailService.List(new[] { "M.Id" }, new[] { campaignDetail.Id.ToString() }, null);
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

        // GET: api/CampaignDetailByProductTotalValue/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignDetailByProductTotalValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();
                resultVM = await _campaignDetailService.ListAsDataTable(new[] { "" }, new[] { "" });
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

        // GET: api/CampaignDetailByProductTotalValue/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductTotalValueService _campaignDetailService = new CampaignDetailByProductTotalValueService();
                resultVM = await _campaignDetailService.Dropdown(); // Adjust if Dropdown requires a different method
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
    }
}
