using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class RegistrationService
    {

        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(RegistrationVM registration)
        {
            string CodeGroup = "";
            string CodeName = "";
            string code = "";
            CommonRepository _commonRepo = new CommonRepository();
            RegistrationRepository _repo = new RegistrationRepository();

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
                //string[] conditionField = { "TelephoneNo", "IsActive" };
                //string[] conditionValue = { bankinfo.TelephoneNo.Trim(), "1" };

                //bool exist = _commonRepo.CheckExists("BankInformations", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion


                  CodeGroup = "Registration";
                  CodeName = "Registration";

                  code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    registration.Code = code;

                    result = await _repo.Insert(registration, conn, transaction);

                    if (result.Status == "Success")
                    {
                        CompanyProfileVM companyVm = new CompanyProfileVM();
                        CompanyProfileRepository _companyProfileRepo = new CompanyProfileRepository();

                        CodeGroup = "CompanyProfile";
                        CodeName = "CompanyProfile";

                        var companycode = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

                        companyVm.Code = companycode;
                        companyVm.CompanyName = registration.CompanyName;
                        companyVm.Address = registration.CompanyAddress;                     
                        companyVm.CreatedBy = registration.CreatedBy;                     
                        companyVm.CreatedOn = DateTime.Now;                     
                        companyVm.CreatedFrom = registration.CreatedFrom;                     

                        result = await _companyProfileRepo.Insert(companyVm,conn,transaction);

                        int companyId = 0;
                        if (result.Status == "Success")
                        {
                            companyId = Convert.ToInt32(result.Id);

                            BranchProfileVM branchVm = new BranchProfileVM();
                            BranchProfileRepository _branchProfileRepo = new BranchProfileRepository();

                            CodeGroup = "BranchProfile";
                            CodeName = "BranchProfile";

                            code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);
                            branchVm.Code = code;

                            branchVm.Name = registration.CompanyName;
                            branchVm.DistributorCode = code;
                            branchVm.CreatedBy = registration.CreatedBy;
                            branchVm.CreatedFrom = registration.CreatedFrom;
                            branchVm.CompanyId = companyId;
                            branchVm.IsActive = true;

                            result = await _branchProfileRepo.Insert(branchVm,conn,transaction);

                            int branchId = 0;

                            if (result.Status == "Success")
                            {
                                branchId = Convert.ToInt32(result.Id);

                                UserProfileVM userVm = new UserProfileVM();
                                UserProfileRepository _userProfileRepo = new UserProfileRepository();

                                userVm.UserId = registration.UserId;
                                userVm.UserName = registration.EmailAsLoginId;
                                userVm.Email = registration.EmailAsLoginId;
                                userVm.NormalizedPassword = registration.Password;
                                userVm.PhoneNumber = registration.PhoneNumber;
                                userVm.FullName = registration.FullName;
                                userVm.CompanyId = companyId;
                                userVm.BranchId = branchId;
                                userVm.CreatedBy = registration.CreatedBy;
                                userVm.CreatedFrom = registration.CreatedFrom;
                                result = await _userProfileRepo.Insert(userVm, conn, transaction);


                                if (result.Status == "Success")
                                {
                                    MenuAuthorizationRepository _menuRepo = new MenuAuthorizationRepository();

                                    UserBranchProfileVM Vm = new UserBranchProfileVM();

                                    Vm.UserId = registration.EmailAsLoginId;
                                    Vm.CompanyId = companyId;
                                    Vm.CreatedBy = registration.CreatedBy;
                                    Vm.CreatedFrom = registration.CreatedFrom;

                                    var menuResult = await _menuRepo.CopyRoleMenuToUser(Vm,conn,transaction);

                                    if (menuResult.Status != "Success")
                                    {
                                        throw new Exception(menuResult.Message);
                                    }
                                }


                                //if (result.Status != "Success")
                                //{
                                //    throw new Exception(result.Message);
                                //}

                                // ==============================
                                // ✅ USER BRANCH MAP (DEFAULT)
                                // ==============================
                                UserBranchProfileVM mapVm = new UserBranchProfileVM();
                                UserBranchProfileRepository _mapRepo = new UserBranchProfileRepository();

                                mapVm.UserId = registration.UserId;
                                mapVm.BranchId = branchId;
                                mapVm.CreatedBy = registration.CreatedBy;
                                mapVm.CreatedFrom = registration.CreatedFrom;

                                var mapResult = await _mapRepo.Insert(mapVm, conn, transaction);

                                if (mapResult.Status != "Success")
                                {
                                    throw new Exception(mapResult.Message);
                                }

                            }
                        }


                    }
                    if (isNewConnection)
                    {
                        transaction.Commit();
                    }

                    return result;
                }
                else
                {
                    throw new Exception("Code Generation Failed!");
                }
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
        }

        public async Task<ResultVM> Update(RegistrationVM registration)
        {
            RegistrationRepository _repo = new RegistrationRepository();
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
                //string[] conditionField = { "Id not", "TelephoneNo", "IsActive" };
                //string[] conditionValue = { bankinfo.Id.ToString(), bankinfo.TelephoneNo.Trim(), "1" };

                //bool exist = _commonRepo.CheckExists("BankInformations", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion

                result = await _repo.Update(registration, conn, transaction);

                if (isNewConnection)
                {
                    transaction.Commit();
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

        public async Task<ResultVM> Delete(CommonVM vm)
        {
            RegistrationRepository _repo = new RegistrationRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Delete(vm, conn, transaction);

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

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            RegistrationRepository _repo = new RegistrationRepository();
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

                if (isNewConnection)
                {
                    transaction.Commit();
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

        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            RegistrationRepository _repo = new RegistrationRepository();
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

                result = await _repo.GetGridData(options, conditionalFields, conditionalValues, conn, transaction);

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
