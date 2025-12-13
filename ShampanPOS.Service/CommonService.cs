using ShampanPOS.Repository;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;

namespace ShampanPOS.Service
{
    public class CommonService
    {

        public async Task<ResultVM> SettingsValue(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SettingsValue(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> NextPrevious(string id, string status, string tableName, string type)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.NextPrevious(id, status, tableName, type, conn, transaction);

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

        public async Task<ResultVM> EnumList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.EnumList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> AreaLocationList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.AreaLocationList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> ParentAreaList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ParentAreaList(conditionalFields, conditionalValues, vm, conn, transaction);

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



        public async Task<ResultVM> ParentBranchProfileList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ParentBranchProfileList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> ParentSalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ParentSalePersonList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> GetFiscalYearForSaleList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetFiscalYearForSaleList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> AssingedBranchList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            UserBranchProfileRepository _repo = new UserBranchProfileRepository();
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




        public async Task<ResultVM> ProductGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ProductGroupList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> UOMList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.UOMList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> EnumTypeList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.EnumTypeList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> SalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SalePersonList(conditionalFields, conditionalValues, vm, conn, transaction);

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



        public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SaleOrderList(conditionalFields, conditionalValues, vm, conn, transaction);

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







        public async Task<ResultVM> GetSalePersonParentList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetSalePersonParentList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> CurrencieList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CurrencieList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> CustomerCategoryList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CustomerCategoryList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> ProductList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ProductList(conditionalFields, conditionalValues, vm, conn, transaction);

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


        public async Task<ResultVM> DeliveryPersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.DeliveryPersonList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> CustomerList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CustomerList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> ReceiveByDeliveryPersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ReceiveByDeliveryPersonList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> CustomerGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CustomerGroupList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> CustomerRouteList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CustomerRouteList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> SupplierList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SupplierList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> SupplierGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SupplierGroupList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        //public async Task<ResultVM> SalePersonList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    CommonRepository _repo = new CommonRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;
        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        conn.Open();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        result = await _repo.SalePersonList(conditionalFields, conditionalValues, vm, conn, transaction);

        //        if (isNewConnection && result.Status == "Success")
        //        {
        //            transaction.Commit();
        //        }
        //        else
        //        {
        //            throw new Exception(result.Message);
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //        {
        //            transaction.Rollback();
        //        }
        //        result.Message = ex.ToString();
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}
        public async Task<ResultVM> CampaignTargetList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.CampaignTargetList(conditionalFields, conditionalValues, vm, conn, transaction);

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
        public async Task<ResultVM> GetProductModalForPurchaseData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetProductModalForPurchaseData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    var countResult = await _repo.GetProductModalForPurchaseCountData(conditionalFields, conditionalValues, vm, conn, transaction);

                    if (countResult.Status.ToLower() == "success")
                    {
                        result.Count = countResult.Count;
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
                result.Status = "Fail";
                result.Message = ex.Message.ToString();
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

        public async Task<ResultVM> GetProductModalForSaleData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetProductModalForSaleData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    var countResult = await _repo.GetProductModalForSaleCountData(conditionalFields, conditionalValues, vm, conn, transaction);

                    if (countResult.Status.ToLower() == "success")
                    {
                        result.Count = countResult.Count;
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
                result.Status = "Fail";
                result.Message = ex.Message.ToString();
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

        public async Task<ResultVM> GetProductModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetProductModalData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    var countResult = await _repo.GetProductModalCountData(conditionalFields, conditionalValues, vm, conn, transaction);

                    if (countResult.Status.ToLower() == "success")
                    {
                        result.Count = countResult.Count;
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
                result.Status = "Fail";
                result.Message = ex.Message.ToString();
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

        public async Task<ResultVM> GetTop10Customers(string branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetTop10Customers(branchId, conn, transaction);

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

        public async Task<ResultVM> GetBottom10Customers(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetBottom10Customers(branchId, conn, transaction);

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

        public async Task<ResultVM> GetTop10Products(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetTop10Products(branchId, conn, transaction);

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

        public async Task<ResultVM> GetBottom10Products(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetBottom10Products(branchId, conn, transaction);

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

        public async Task<ResultVM> GetTop10SalePersons(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetTop10SalePersons(branchId, conn, transaction);

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

        public async Task<ResultVM> GetBottom10SalePersons(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetBottom10SalePersons(branchId, conn, transaction);

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

        public async Task<ResultVM> GetOrderPurchasePOReturnData(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetOrderPurchasePOReturnData(branchId, conn, transaction);

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

        public async Task<ResultVM> GetSalesData(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetSalesData(branchId, conn, transaction);

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

        public async Task<ResultVM> GetPendingSales(string? branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetPendingSales(branchId, conn, transaction);

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


        public async Task<ResultVM> CampaignMudularityCalculation(CampaignUtilty campaign)
        {
            CampaignManagerVM campaignManager = new CampaignManagerVM();
            CampaignRepository _repoCampaign = new CampaignRepository();
            CommonRepository _commonRepo = new CommonRepository();
            CustomerRepository _CustomerRepo = new CustomerRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            CampaignDetailByQuantityVM campaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
            CampaignDetailByProductValueVM campaignDetailByProductValueVM = new CampaignDetailByProductValueVM();
            CampaignDetailByProductTotalValueVM campaignDetailByProductTotalValueVM = new CampaignDetailByProductTotalValueVM();
            CampaignDetailByInvoiceValueVM campaignDetailByInvoiceValueVM = new CampaignDetailByInvoiceValueVM();

            #region Variable Declaration
            bool campaignByQuantitiesExist = false;
            int campaignByQuantitiesId = 0;
            bool campaignByProductValuesExist = false;
            int campaignByProductValuesId = 0;
            bool campaignByProductTotalValueExist = false;
            int campaignByProductTotalValueId = 0;
            bool campaignByInvoiceValueExist = false;
            int campaignByInvoiceValueId = 0;
            decimal subtotal = 0;
            decimal discountGain = 0;
            decimal sumTotalAfterDiscount = 0;
            decimal lineDiscountGain = 0;
            decimal invoiceDiscount = 0;
            //string  PriceGroup= "";

            #endregion

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                // Get product modal for sale data

                var productResult = await _commonRepo.GetProductModalForSaleData(
                    new[] { "P.Id" },
                    new[] { campaign.ProductId.ToString() },
                    new PeramModel
                    {
                        BranchId = campaign.BranchId.ToString(),
                        FromDate = campaign.Date,
                        CustomerId = campaign.CustomerId,
                        OrderName = "P.Id",
                        orderDir = "ASC",
                        startRec = 0,
                        pageSize = 10
                    },
                    conn,
                    transaction
                );

                if (productResult.Status == "Success" && productResult.DataVM is List<ProductDataVM> productData)
                {
                    var firstProduct = productData.FirstOrDefault();
                    if (firstProduct != null)
                    {
                        subtotal = campaign.Quantity * (firstProduct.SalesPrice ?? 0);
                    }
                }

                #region Campaign Exist Check
                (campaignByQuantitiesExist, campaignByQuantitiesId) = _commonRepo.CampaignExists(Convert.ToDateTime(campaign.Date), campaign.BranchId, 25, conn, transaction);
                (campaignByProductValuesExist, campaignByProductValuesId) = _commonRepo.CampaignExists(Convert.ToDateTime(campaign.Date), campaign.BranchId, 26, conn, transaction);
                (campaignByProductTotalValueExist, campaignByProductTotalValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(campaign.Date), campaign.BranchId, 27, conn, transaction);
                (campaignByInvoiceValueExist, campaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(campaign.Date), campaign.BranchId, 24, conn, transaction);
                #endregion

                #region Campaign By Quantity Discount
                if (campaignByQuantitiesExist)
                {
                    var campaignByQuantityResult = await _repoCampaign.CampaignByQuantityDetailsList(
                        new[] { "D.CampaignId" },
                        new[] { campaignByQuantitiesId.ToString() },
                        null,
                        conn,
                        transaction
                    );

                    if (campaignByQuantityResult.Status == "Success" && campaignByQuantityResult.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var campaignByQuantityDetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);
                        campaignDetailByQuantityVM = campaignByQuantityDetails
                            .FirstOrDefault(record => record.CustomerId == campaign.CustomerId && record.ProductId == campaign.ProductId)
                            ?? campaignByQuantityDetails.FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == campaign.ProductId);


                        //campaignDetailByQuantityVM = campaignByQuantityDetails
                        //.FirstOrDefault(record =>
                        //{
                        //    bool match = record.CustomerId == campaign.CustomerId && record.ProductId == campaign.ProductId;
                        //    if (!match)
                        //    {
                        //        Console.WriteLine($"Skipping record: CustomerId={record.CustomerId}, ProductId={record.ProductId}");
                        //    }
                        //    return match;
                        //})
                        //?? campaignByQuantityDetails.FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == campaign.ProductId);


                        if (campaignDetailByQuantityVM != null)
                        {
                            int quantityAbility = (int)Math.Floor((decimal)campaign.Quantity / campaignDetailByQuantityVM.FromQuantity);
                            int freeQuantityGain = Convert.ToInt32(campaignDetailByQuantityVM.FreeQuantity * quantityAbility);
                            campaignManager.FreeQuantity = freeQuantityGain;
                            campaignManager.FreeProductId = campaignDetailByQuantityVM.FreeProductId;
                            campaignManager.FreeProductName = campaignDetailByQuantityVM.ProductName;
                            if (quantityAbility == 0)
                            {
                                campaignManager.FreeQuantity = 0;
                                campaignManager.FreeProductId = 0;
                                campaignManager.FreeProductName = "";
                            }
                        }
                    }
                }
                #endregion

                #region Campaign By Product Values Discount
                if (campaignByProductValuesExist)
                {
                    var campaignByProductValuesResult = await _repoCampaign.CampaignProductValueDetailsList(
                        new[] { "D.CampaignId" },
                        new[] { campaignByProductValuesId.ToString() },
                        null,
                        conn,
                        transaction
                    );

                    if (campaignByProductValuesResult.Status == "Success" && campaignByProductValuesResult.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var campaignByProductValuesDetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);
                        campaignDetailByProductValueVM = campaignByProductValuesDetails
                            .FirstOrDefault(record => record.CustomerId == campaign.CustomerId &&
                                                      record.ProductId == campaign.ProductId &&
                                                      record.FromQuantity <= campaign.Quantity &&
                                                      record.ToQuantity >= campaign.Quantity)
                            ?? campaignByProductValuesDetails.FirstOrDefault(record => record.CustomerId == 0 &&
                                                                                       record.ProductId == campaign.ProductId &&
                                                                                       record.FromQuantity <= campaign.Quantity &&
                                                                                       record.ToQuantity >= campaign.Quantity);

                        if (campaignDetailByProductValueVM != null)
                        {
                            discountGain = Convert.ToDecimal(campaignDetailByProductValueVM.DiscountRateBasedOnUnitPrice * subtotal / 100);
                            campaignManager.DiscountAmount = discountGain;
                            campaignManager.DiscountRate = campaignDetailByProductValueVM.DiscountRateBasedOnUnitPrice;
                        }
                    }
                }
                #endregion

                // Calculate total after applying product value discount
                sumTotalAfterDiscount = subtotal - discountGain;
                campaignManager.SubTotalAfterDiscount = sumTotalAfterDiscount;

                #region Campaign By Product Total Value Discount
                if (campaignByProductTotalValueExist)
                {
                    var campaignByProductTotalValueResult = await _repoCampaign.CampaignByProductTotalValueList(
                        new[] { "D.CampaignId" },
                        new[] { campaignByProductTotalValueId.ToString() },
                        null,
                        conn,
                        transaction
                    );

                    if (campaignByProductTotalValueResult.Status == "Success" && campaignByProductTotalValueResult.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var campaignByProductTotalValueDetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);
                        campaignDetailByProductTotalValueVM = campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == campaign.CustomerId &&
                                                      record.ProductId == campaign.ProductId &&
                                                      record.FromAmount <= sumTotalAfterDiscount &&
                                                      record.ToAmount >= sumTotalAfterDiscount)
                            ??
                            campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == 0 &&
                                                      record.ProductId == campaign.ProductId &&
                                                      record.FromAmount <= sumTotalAfterDiscount &&
                                                      record.ToAmount >= sumTotalAfterDiscount);
                        ;

                        if (campaignDetailByProductTotalValueVM != null)
                        {
                            lineDiscountGain = Convert.ToDecimal(campaignDetailByProductTotalValueVM.DiscountRateBasedOnTotalPrice * sumTotalAfterDiscount / 100);
                            invoiceDiscount -= lineDiscountGain;
                            campaignManager.LineDiscountRate = campaignDetailByProductTotalValueVM.DiscountRateBasedOnTotalPrice;
                            campaignManager.LineDiscountAmount = lineDiscountGain;
                        }
                    }
                }
                // Calculate total after applying product value discount
                var sumLineTotalAfterDiscount = sumTotalAfterDiscount - lineDiscountGain;
                campaignManager.LineTotalAfterDiscount = sumLineTotalAfterDiscount;
                #endregion

                #region Campaign By Total Invoice
                if (campaignByProductTotalValueExist)
                {
                    var campaignByProductTotalValueResult = await _repoCampaign.CampaignByProductTotalValueList(
                        new[] { "D.CampaignId" },
                        new[] { campaignByProductTotalValueId.ToString() },
                        null,
                        conn,
                        transaction
                    );

                    if (campaignByProductTotalValueResult.Status == "Success" && campaignByProductTotalValueResult.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var campaignByProductTotalValueDetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);
                        campaignDetailByProductTotalValueVM = campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == campaign.CustomerId &&
                                                      record.ProductId == campaign.ProductId &&
                                                      record.FromAmount <= sumLineTotalAfterDiscount &&
                                                      record.ToAmount >= sumLineTotalAfterDiscount)

                            ??

                            campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == 0 &&
                                                      record.ProductId == campaign.ProductId &&
                                                      record.FromAmount <= sumLineTotalAfterDiscount &&
                                                      record.ToAmount >= sumLineTotalAfterDiscount);

                        if (campaignDetailByProductTotalValueVM != null)
                        {
                            lineDiscountGain = Convert.ToDecimal(campaignDetailByProductTotalValueVM.DiscountRateBasedOnTotalPrice * sumTotalAfterDiscount / 100);
                            invoiceDiscount -= lineDiscountGain;
                            campaignManager.LineDiscountRate = campaignDetailByProductTotalValueVM.DiscountRateBasedOnTotalPrice;
                            campaignManager.LineDiscountAmount = lineDiscountGain;
                        }
                    }
                }
                // Calculate total after applying product value discount
                campaignManager.LineTotalAfterDiscount = sumLineTotalAfterDiscount;
                #endregion

                transaction.Commit();

                result.Status = "Success";
                result.Message = "Campaign successfully Calculated";
                result.DataVM = campaignManager;
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }

        public async Task<ResultVM> CampaignInvoiceCalculation(CampaignUtilty campaign)
        {
            CampaignManagerVM campaignManager = new CampaignManagerVM();
            CampaignRepository _repoCampaign = new CampaignRepository();
            CommonRepository _commonRepo = new CommonRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            //CampaignDetailByQuantityVM campaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
            //CampaignDetailByProductValueVM campaignDetailByProductValueVM = new CampaignDetailByProductValueVM();
            //CampaignDetailByProductTotalValueVM campaignDetailByProductTotalValueVM = new CampaignDetailByProductTotalValueVM();
            CampaignDetailByInvoiceValueVM campaignDetailByInvoiceValueVM = new CampaignDetailByInvoiceValueVM();
            campaignManager.TotalInvoiceValue = campaign.Quantity;
            campaignManager.DiscountAmount = 0;
            campaignManager.DiscountRateBasedOnTotalPrice = 0;
            #region Variable Declaration

            int campaignByProductTotalValueId = 0;
            bool campaignByInvoiceValueExist = false;
            int campaignByInvoiceValueId = 0;
            decimal subtotal = 0;
            decimal sumTotalAfterDiscount = 0;
            decimal lineDiscountGain = 0;
            decimal invoiceDiscount = 0;
            decimal totalInvoiceValue = 0;
            #endregion

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                #region Campaign Exist Check
                (campaignByInvoiceValueExist, campaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(campaign.Date), campaign.BranchId, 24, conn, transaction);
                #endregion


                #region Campaign By Total Invoice
                if (campaignByInvoiceValueExist)
                {
                    var campaignByProductTotalValueResult = await _repoCampaign.CampaignDetailByInvoiceValueList(
                        new[] { "D.CampaignId" },
                        new[] { campaignByInvoiceValueId.ToString() },
                        null,
                        conn,
                        transaction
                    );

                    if (campaignByProductTotalValueResult.Status == "Success" && campaignByProductTotalValueResult.DataVM is DataTable dt)
                    {
                        string json = JsonConvert.SerializeObject(dt);
                        var campaignByProductTotalValueDetails = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);
                        campaignDetailByInvoiceValueVM = campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == campaign.CustomerId &&
                                                      record.FromAmount <= campaign.Quantity &&
                                                      record.ToAmount >= campaign.Quantity)
                        ?? campaignByProductTotalValueDetails
                            .FirstOrDefault(record => record.CustomerId == 0 &&
                                                      record.FromAmount <= campaign.Quantity &&
                                                      record.ToAmount >= campaign.Quantity);



                        if (campaignDetailByInvoiceValueVM != null)
                        {
                            totalInvoiceValue = Convert.ToDecimal(campaignDetailByInvoiceValueVM.DiscountRateBasedOnTotalPrice * campaign.Quantity / 100);
                            campaign.Quantity = campaign.Quantity - totalInvoiceValue;
                            campaignManager.TotalInvoiceValue = campaign.Quantity;
                            campaignManager.DiscountAmount = totalInvoiceValue;


                            campaignManager.DiscountRateBasedOnTotalPrice = campaignByProductTotalValueDetails.FirstOrDefault().DiscountRateBasedOnTotalPrice;
                        }
                    }
                }
                #endregion

                transaction.Commit();

                result.Status = "Success";
                result.Message = "Campaign successfully Calculated";
                result.DataVM = campaignManager;
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection) transaction.Rollback();
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                if (isNewConnection && conn != null) conn.Close();
            }
        }


        public async Task<ResultVM> GetRouteBySalePersonAndBranch(int salePersonId, int branchId)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.ListRouteBySalePersonAndBranch(salePersonId, branchId, conn, transaction);

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

        public async Task<ResultVM> GetCampaignList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetCampaignList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> PaymentTypeList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.PaymentTypeList(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> SaleDeleveryList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.SaleDeleveryList(conditionalFields, conditionalValues, vm, conn, transaction);

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


        public async Task<ResultVM> GetSaleDeleveryModal(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CommonRepository _repo = new CommonRepository();
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

                result = await _repo.GetSaleDeleveryModal(conditionalFields, conditionalValues, vm, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    var countResult = await _repo.getSaleDeleveryModalCountData(conditionalFields, conditionalValues, vm, conn, transaction);

                    if (countResult.Status.ToLower() == "success")
                    {
                        result.Count = countResult.Count;
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
                result.Status = "Fail";
                result.Message = ex.Message.ToString();
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

        public async Task<ResultVM> HasDayEndData(string branchId)
        {
            CommonRepository _repo = new CommonRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                // Call repository method (assumes you have _repo.HasDayEndData implemented)
                result = await _repo.HasDayEndData(branchId, conn, transaction);

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
