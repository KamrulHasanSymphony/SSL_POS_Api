using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class ReportsService
    {
        public async Task<ResultVM> BankTransactionReportList(BankTransactionReportVM vm = null)
        {
            ReportsRepository _repo = new ReportsRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            bool isNewConnection = false;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                if (vm != null)
                {
                    vm.Operation = vm.IsSummary ? "SUMMARY" : "DETAILS";
                }

                result = await _repo.BankTransactionReportList(null, null, vm, conn, transaction);

                if (result.Status == "Success")
                    transaction.Commit();
                else
                    transaction.Rollback();

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                    transaction.Rollback();

                result.ExMessage = ex.ToString();
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (conn != null && isNewConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        public async Task<ResultVM> CustomerSaleCollectionReportList(
    CustomerSaleCollectionReportVM vm = null)
        {
            ReportsRepository _repo = new ReportsRepository();
            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            bool isNewConnection = false;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                if (vm != null)
                    vm.Operation = vm.IsSummary ? "SUMMARY" : "DETAILS";

                result = await _repo.CustomerSaleCollectionReportList(vm, conn, transaction);

                if (result.Status == "Success")
                    transaction.Commit();
                else
                    transaction.Rollback();

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                    transaction.Rollback();
                result.ExMessage = ex.ToString();
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (conn != null && isNewConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<ResultVM> SupplierPurchasePaymentReportList(
    SupplierPurchasePaymentReportVM vm = null)
        {
            ReportsRepository _repo = new ReportsRepository();
            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            bool isNewConnection = false;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();
                if (vm != null)
                    vm.Operation = vm.IsSummary ? "SUMMARY" : "DETAILS";
                result = await _repo.SupplierPurchasePaymentReportList(vm, conn, transaction);
                if (result.Status == "Success")
                    transaction.Commit();
                else
                    transaction.Rollback();
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                    transaction.Rollback();
                result.ExMessage = ex.ToString();
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (conn != null && isNewConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }


        public async Task<ResultVM> GetSupplierPaymentDueList(SupplierPaymentDueVM vm = null)
        {
            ReportsRepository _repo = new ReportsRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            SqlConnection conn = null;
            bool isNewConnection = false;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                string[] conditionalFields = new string[] { "PUR.BranchId" };
                string[] conditionalValues = new string[] { vm.BranchId.ToString() };

                //string[] conditionalFields = new string[] { "PUR.CompanyId", "PUR.BranchId" };
                //string[] conditionalValues = new string[] { vm.CompanyId.ToString(), vm.BranchId.ToString() };

                result = await _repo.GetSupplierPaymentDueList( conditionalFields, conditionalValues, vm, conn, null);

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.ToString();
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (conn != null && isNewConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public async Task<ResultVM> GetCustomerCollectionDueList(CustomerCollectionDueVM vm = null)
        {
            ReportsRepository _repo = new ReportsRepository();
            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };
            SqlConnection conn = null;
            bool isNewConnection = false;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                string[] conditionalFields = new string[] { "S.BranchId" };
                string[] conditionalValues = new string[] {  vm.BranchId.ToString() };

                //string[] conditionalFields = new string[] { "S.CompanyId", "S.BranchId" };
                //string[] conditionalValues = new string[] { vm.CompanyId.ToString(), vm.BranchId.ToString() };

                result = await _repo.GetCustomerCollectionDueList(conditionalFields, conditionalValues, vm, conn, null);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.ToString();
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                if (conn != null && isNewConnection)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

    }
}
