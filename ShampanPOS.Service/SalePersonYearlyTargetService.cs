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

namespace ShampanPOS.Service
{
    public class SalePersonYearlyTargetService
    {
        public async Task<ResultVM> Insert(SalePersonYearlyTargetVM yearlyTarget)
        {
            string CodeGroup = "SalePersonYearlyTarget";
            string CodeName = "SalePersonYearlyTarget";
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PeramModel vm = null;
            CommonRepository _commonRepo = new CommonRepository();
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
            FiscalYearForSaleRepository _frepo = new FiscalYearForSaleRepository();
                       

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Insert(yearlyTarget, conn, transaction);
                List<SalePersonYearlyTargetDetailVM> salePersonYearlyTargetDetailVMs = new List<SalePersonYearlyTargetDetailVM>();

                if (result.Status == "Success")
                {
                    var detailsDataList = _frepo.DetailsList(new[] { "D.FiscalYearForSaleId" }, new[] { yearlyTarget.FiscalYearForSaleId.ToString() }, vm, conn, transaction);

                    if (detailsDataList != null && detailsDataList.DataVM != null)
                    {
                        DataTable detailsTable = (DataTable)detailsDataList.DataVM;

                        foreach (DataRow row in detailsTable.Rows)
                        {
                            SalePersonYearlyTargetDetailVM salePersonYearlyTargetDetailVM = new SalePersonYearlyTargetDetailVM();

                            salePersonYearlyTargetDetailVM.BranchId = yearlyTarget.BranchId;
                            salePersonYearlyTargetDetailVM.SalePersonId = yearlyTarget.SalePersonId;
                            salePersonYearlyTargetDetailVM.SalePersonYearlyTargetId = yearlyTarget.Id;
                            salePersonYearlyTargetDetailVM.FiscalYearDetailForSaleId = Convert.ToInt32(row["Id"]);
                            salePersonYearlyTargetDetailVM.FiscalYearForSaleId = Convert.ToInt32(row["FiscalYearForSaleId"]);
                            salePersonYearlyTargetDetailVM.MonthlyTarget = yearlyTarget.YearlyTarget / 12.0m;
                            salePersonYearlyTargetDetailVM.SelfSaleCommissionRate = yearlyTarget.SelfSaleCommissionRate;
                            salePersonYearlyTargetDetailVM.OtherSaleCommissionRate = yearlyTarget.OtherSaleCommissionRate;
                            salePersonYearlyTargetDetailVM.Year = Convert.ToInt32(row["Year"]);
                            salePersonYearlyTargetDetailVM.MonthId = Convert.ToInt32(row["MonthId"]);
                            salePersonYearlyTargetDetailVM.MonthStart = Convert.ToDateTime(row["MonthStart"]).ToString("yyyy-MM-dd");
                            salePersonYearlyTargetDetailVM.MonthEnd = Convert.ToDateTime(row["MonthEnd"]).ToString("yyyy-MM-dd");
                            salePersonYearlyTargetDetailVMs.Add(salePersonYearlyTargetDetailVM);
                        }
                    }

                    // Insert the details into the repository
                     await _repo.DetailsInsert(salePersonYearlyTargetDetailVMs, conn, transaction);
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

        public async Task<ResultVM> Update(SalePersonYearlyTargetVM yearlyTarget)
        {
            CommonRepository _commonRepo = new CommonRepository();
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                decimal totalMonthlyTarget = Math.Round(yearlyTarget.SalePersonYearlyTargetDetailList.Sum(detail => detail.MonthlyTarget ?? 0));

                // 2. Compare the total MonthlyTarget with YearlyTarget
                
                if (yearlyTarget.YearlyTarget.HasValue && totalMonthlyTarget > yearlyTarget.YearlyTarget.Value || yearlyTarget.YearlyTarget.HasValue && totalMonthlyTarget < yearlyTarget.YearlyTarget.Value)
                {
                    result.Message = "Monthly target exceeds/less the Yearly Target.";
                    return result;
                }

                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                var record = _commonRepo.DetailsDelete("SalePersonYearlyTargetDetails", new[] { "SalePersonYearlyTargetId" }, new[] { yearlyTarget.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                result = await _repo.Update(yearlyTarget, conn, transaction);

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

        public async Task<ResultVM> Delete(string[] IDs)
        {
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = IDs, DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

               result = await _repo.Delete(IDs, conn, transaction);

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

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
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

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
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

                result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm,conn, transaction);

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

        public async Task<ResultVM> Dropdown()
        {
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
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

            result = await _repo.Dropdown(conn, transaction);

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


        public async Task<ResultVM> GetGridData(GridOptions options)
        {
           
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();

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

                result = await _repo.GetGridData(options, conn, transaction);

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

        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            SalePersonYearlyTargetRepository _repo = new SalePersonYearlyTargetRepository();
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

                result = await _repo.MultiplePost(vm, conn, transaction);

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
    }

}
