using ShampanPOS.Repository;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using static ShampanPOS.ViewModel.KendoCommon.UtilityCommon;
using Microsoft.EntityFrameworkCore;

namespace ShampanPOS.Service
{
    public class MenuAuthorizationService
    {
        CommonRepository _commonRepo = new CommonRepository();
        public async Task<ResultVM> Insert(UserRoleMVM urm)
        {
          
            CommonRepository _commonRepo = new CommonRepository();
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                #region Check Exist Data
                string[] conditionField = { "Name" };
                string[] conditionValue = { urm.Name.Trim()};

                bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion
                // string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);
                result = await _repo.Insert(urm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
                
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
        public async Task<ResultVM> Update(UserRoleMVM urm)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                #region Check Exist Data
                string[] conditionField = { "Id not", "Name" };
                string[] conditionValue = { urm.Id.ToString(), urm.Name.Trim()};

                bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion
                result = await _repo.Update(urm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
        public async Task<ResultVM> GetRoleIndexData(GridOptions options)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetRoleIndexData(options, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> RoleMenuInsert(RoleMenuVM vm)
        {

            CommonRepository _commonRepo = new CommonRepository();
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                //#region Check Exist Data
                //string[] conditionField = { "Name" };
                //string[] conditionValue = { urm.Name.Trim() };

                //bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion


                result = await _repo.RoleMenuDelete(vm);

                RoleMenuVM master = new RoleMenuVM();
                bool isSave = false;
                foreach (var item in vm.roleMenuList)
                {
                    if (item.MenuId > 0 && item.IsChecked)
                    {
                        isSave = true;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.CreatedFrom = vm.CreatedFrom;
                        item.RoleId = vm.RoleId;
                        result = await _repo.RoleMenuInsert(item);
                    }
                }

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;

            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }
        //public ResultModel<RoleMenuVM> Insert(RoleMenuVM model)
        //{
        //    using (var context = _unitOfWork.Create())
        //    {
        //        try
        //        {
        //            if (model == null)
        //            {
        //                return new ResultModel<RoleMenuVM>()
        //                {
        //                    Status = Status.Warning,
        //                    Message = MessageModel.NotFoundForSave,
        //                };
        //            }
        //            else if (!model.roleMenuList.Any())
        //            {
        //                return new ResultModel<RoleMenuVM>()
        //                {
        //                    Status = Status.Warning,
        //                    Message = MessageModel.SubmissionFail,
        //                };
        //            }

        //            var delete = context.Repositories.MenuAuthorizationRepository.RoleMenuDelete(model);

        //            RoleMenuVM master = new RoleMenuVM();
        //            bool isSave = false;
        //            foreach (var item in model.roleMenuList)
        //            {
        //                if (item.MenuId > 0 && item.IsChecked)
        //                {
        //                    isSave = true;
        //                    item.Audit.CreatedBy = model.Audit.CreatedBy;
        //                    item.Audit.CreatedOn = model.Audit.CreatedOn;
        //                    item.Audit.CreatedFrom = model.Audit.CreatedFrom;
        //                    item.RoleId = model.RoleId;
        //                    master = context.Repositories.MenuAuthorizationRepository.Insert(item);
        //                }
        //            }

        //            context.SaveChanges();

        //            if (isSave)
        //            {
        //                return new ResultModel<RoleMenuVM>()
        //                {
        //                    Status = Status.Success,
        //                    Message = MessageModel.SubmissionSuccess,
        //                    Data = master,
        //                    DataVM = master
        //                };
        //            }
        //            else
        //            {
        //                return new ResultModel<RoleMenuVM>()
        //                {
        //                    Status = Status.Warning,
        //                    Message = MessageModel.NotFoundForSave,
        //                };
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            context.RollBack();

        //            return new ResultModel<RoleMenuVM>()
        //            {
        //                Status = Status.Fail,
        //                Message = e.Message,
        //                Data = null
        //            };
        //        }

        //    }
        //}

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }


        public async Task<ResultVM> GetMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetMenuAccessData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetUserMenuIndexData(GridOptions options)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetUserMenuIndexData(options, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetUserMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetUserMenuAccessData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> UserMenuInsert(UserMenuVM vm)
        {

            CommonRepository _commonRepo = new CommonRepository();
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                //#region Check Exist Data
                //string[] conditionField = { "Name" };
                //string[] conditionValue = { urm.Name.Trim() };

                //bool exist = _commonRepo.CheckExists("Role", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion


                result = await _repo.UserMenuDelete(vm);

                UserMenuVM master = new UserMenuVM();
                bool isSave = false;
                foreach (var item in vm.userMenuList)
                {
                    if (item.MenuId > 0 && item.IsChecked)
                    {
                        isSave = true;
                        item.CreatedBy = vm.CreatedBy;
                        item.CreatedOn = vm.CreatedOn;
                        item.CreatedFrom = vm.CreatedFrom;
                        item.RoleId = vm.RoleId;
                        item.UserId = vm.UserId;

                        result = await _repo.UserMenuInsert(item, conn, transaction);
                    }
                }

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;

            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetUserRoleWiseMenuAccessData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetUserRoleWiseMenuAccessData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetRoleData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            MenuAuthorizationRepository _repo = new MenuAuthorizationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.GetRoleData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.Message = ex.ToString();
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

    }


}
