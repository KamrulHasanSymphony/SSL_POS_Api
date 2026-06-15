using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShampanPOS.Service;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using System.Security.Claims;

namespace ShampanPOS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        //UserInfoService _UserInfoService = new UserInfoService();

        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly IConfiguration _configuration;


        RegistrationService _RegistrationService = new RegistrationService();
        CommonService _common = new CommonService();

        // POST: api/Registration/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(RegistrationVM registration)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            RegistrationService _RegistrationService = new RegistrationService();

                try
                {
                resultVM = await _RegistrationService.Insert(registration);
                return resultVM;
                }

               
            
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not created.",
                    ExMessage = ex.Message,
                    DataVM = registration
                };
            }


        }



        // POST: api/Registration/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(RegistrationVM registration)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                RegistrationService _RegistrationService = new RegistrationService();
                resultVM = await _RegistrationService.Update(registration);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = "Data not updated.",
                    ExMessage = ex.Message,
                    DataVM = registration
                };
            }
        }

        // POST: api/Registration/Delete
        [HttpPost("Delete")]
        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = null, DataVM = null };
            try
            {
                _RegistrationService = new RegistrationService();

                resultVM = await _RegistrationService.Delete(vm);
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

        // POST: api/Registration/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {


                string[] conditionFields = null;
                string[] conditionValues = null;
                conditionFields = vm.ConditionalFields;
                conditionValues = vm.ConditionalValues;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    conditionFields = new string[] { "M.Id" };
                    conditionValues = new string[] { vm.Id };
                }
                RegistrationService _RegistrationService = new RegistrationService();


                resultVM = await _RegistrationService.List(new[] { "R.Id" }, new[] { vm.Id.ToString() }, null);
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


        // POST: api/Registration/GetGridData
        [HttpPost("GetGridData")]
        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _RegistrationService = new RegistrationService();
                resultVM = await _RegistrationService.GetGridData(options, null, null);
                //resultVM = await _CustomerService.GetGridData(options);
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
    }
}
