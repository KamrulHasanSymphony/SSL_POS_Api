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
    [Route("api/[controller]")]
    [ApiController]
    public class MenuAuthorizationController : ControllerBase
    {
        MenuAuthorizationService _menuAuthorization = new MenuAuthorizationService();


        // POST: api/MenuAuthorizationController/Insert
        [HttpPost("Insert")]
        public async Task<ResultVM> Insert(UserRoleMVM urm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _menuAuthorization = new MenuAuthorizationService();

            try
            {
                resultVM = await _menuAuthorization.Insert(urm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = urm
                };
            }
        }
        // POST: api/MenuAuthorizationController/Update
        [HttpPost("Update")]
        public async Task<ResultVM> Update(UserRoleMVM urm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _menuAuthorization = new MenuAuthorizationService();

            try
            {
                resultVM = await _menuAuthorization.Update(urm);
                return resultVM;
            }
            catch (Exception ex)
            {
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.Message,
                    DataVM = urm
                };
            }
        }
        // POST: api/MenuAuthorizationController/GetRoleIndexData
        [HttpPost("GetRoleIndexData")]
        public async Task<ResultVM> GetRoleIndexData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();
                resultVM = await _menuAuthorization.GetRoleIndexData(options);
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

        // POST: api/MenuAuthorization/RoleMenuInsert
        [HttpPost("RoleMenuInsert")]
        public async Task<ResultVM> RoleMenuInsert(RoleMenuVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _menuAuthorization = new MenuAuthorizationService();

            try
            {
                resultVM = await _menuAuthorization.RoleMenuInsert(vm);
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
        // POST: api/MenuAuthorization/List
        [HttpPost("List")]
        public async Task<ResultVM> List(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();
                resultVM = await _menuAuthorization.List(new[] { "H.Id" }, new[] { vm.Id }, null);
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
        [HttpPost("GetMenuAccessData")]
        public async Task<ResultVM> GetMenuAccessData(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();

                var peram = new PeramModel
                {
                    Id = vm.Id // map only the needed property
                };

                resultVM = await _menuAuthorization.GetMenuAccessData(null, null, peram);
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
        // POST: api/MenuAuthorizationController/GetUserMenuIndexData
        [HttpPost("GetUserMenuIndexData")]
        public async Task<ResultVM> GetUserMenuIndexData(GridOptions options)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();
                resultVM = await _menuAuthorization.GetUserMenuIndexData(options);
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

        [HttpPost("GetUserMenuAccessData")]
        public async Task<ResultVM> GetUserMenuAccessData(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();

                var peram = new PeramModel
                {
                    Id = vm.Id,
                    UserLogInId = vm.UserId// map only the needed property
                };

                resultVM = await _menuAuthorization.GetUserMenuAccessData(null, null, peram);
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
        // POST: api/MenuAuthorization/UserMenuInsert
        [HttpPost("UserMenuInsert")]
        public async Task<ResultVM> UserMenuInsert(UserMenuVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _menuAuthorization = new MenuAuthorizationService();

            try
            {
                resultVM = await _menuAuthorization.UserMenuInsert(vm);
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
        // POST: api/MenuAuthorization/GetUserRoleWiseMenuAccessData
        [HttpPost("GetUserRoleWiseMenuAccessData")]
        public async Task<ResultVM> GetUserRoleWiseMenuAccessData(CommonVM vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();

                var peram = new PeramModel
                {
                    Id = vm.Id // map only the needed property
                };

                resultVM = await _menuAuthorization.GetUserRoleWiseMenuAccessData(null, null, peram);
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

        // POST: api/MenuAuthorization/GetRoleData
        [HttpPost("GetRoleData")]
        public async Task<ResultVM> GetRoleData(CommonVM Vm)
        {
            ResultVM resultVM = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                _menuAuthorization = new MenuAuthorizationService();
                resultVM = await _menuAuthorization.GetRoleData(new[] { "" }, new[] { "" }, null);
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
