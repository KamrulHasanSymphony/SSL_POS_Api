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
    public class CampaignDetailByProductValueController : ControllerBase
    {
        // POST: api/CampaignDetailByProductValue/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(CampaignDetailByProductValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();

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

        // POST: api/CampaignDetailByProductValue/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(CampaignDetailByProductValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();
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

        // POST: api/CampaignDetailByProductValue/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CampaignDetailByProductValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();

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

        // POST: api/CampaignDetailByProductValue/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CampaignDetailByProductValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();
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

        // GET: api/CampaignDetailByProductValue/ListAsDataTable
        [HttpGet("ListAsDataTable")]
        public async Task<ResultVM> ListAsDataTable(CampaignDetailByProductValueVM campaignDetail)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();
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


        //// POST: api/Product/GetProductGroupModalDataList
        //[HttpPost("GetProductGroupModalDataList")]
        //public async Task<ResultVM> GetCampaignDetailByProductValueModalDataList(CampaignDetailByProductValueVM model)
        //{
        //    ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    try
        //    {
        //        CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();
        //        resultVM = await _campaignDetailService.GetCampaignDetailByProductValueModalDataList(new[] { "" }, new[] { "" }, null);
        //        return resultVM;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.Message,
        //            DataVM = model
        //        };
        //    }
        //}


        // GET: api/CampaignDetailByProductValue/Dropdown
        [HttpGet("Dropdown")]
        public async Task<ResultVM> Dropdown()
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                CampaignDetailByProductValueService _campaignDetailService = new CampaignDetailByProductValueService();
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
