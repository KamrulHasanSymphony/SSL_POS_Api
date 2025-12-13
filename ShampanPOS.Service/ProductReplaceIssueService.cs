using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ShampanPOS.Service
{
    public class ProductReplaceIssueService
    {
        CommonRepository _commonRepo = new CommonRepository();

         //1. Insert Method
    
        public async Task<ResultVM> Insert(ProductReplaceIssueVM model)
        {
            string CodeGroup = "ProductReplaceIssue";
            string CodeName = "ProductReplaceIssue";
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
            _commonRepo = new CommonRepository();
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

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, model.IssueDate, model.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    model.Code = code;

                    result = await _repo.Insert(model, conn, transaction);

                    model.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {

                        CommonVM commonVM = new CommonVM();
                        var idList = new List<string?>();
                        int LineNo = 1;

                        foreach (var details in model.ProductReplaceIssueDetails)
                        {
                            idList.Add(details.ProductReplaceReceiveId != null ? details.ProductReplaceReceiveId.ToString() : "0");
                            commonVM.IDs = idList.ToArray();
                            details.ProductReplaceIssueId = model.Id;
                            details.Line = LineNo;

                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                            if (resultDetail.Status.ToLower() == "success")
                            {
                                LineNo++;
                                if (details.ProductReplaceReceiveDetailId > 0)
                                {
                                    var lineItemResult = await _repo.UpdateLineItem(details, conn, transaction);

                                    if (lineItemResult.Status.ToLower() == "fail")
                                    {
                                        throw new Exception(lineItemResult.Message);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(resultDetail.Message);
                            }
                        }

                        foreach (var item in commonVM.IDs)
                        {
                            PeramModel peramModel = new PeramModel();
                            peramModel.Id = item;
                            model.ProductReplaceReceiveId = Convert.ToInt32(peramModel.Id);

                            var completedQtyResult = await _repo.GetLineItemCompletedQty(null, null, peramModel, conn, transaction);

                            if (completedQtyResult.Status == "Success" && completedQtyResult.DataVM is DataTable statusValue)
                            {
                                if (statusValue.Rows.Count > 0)
                                {
                                    var status = statusValue.Rows[0]["Status"].ToString();

                                    if (status == "True")
                                    {
                                        var updateIsCompletedResult = await _repo.UpdateIsCompleted(model, conn, transaction);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(result.Message);
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


        // 2. Update Method
        public async Task<ResultVM> Update(ProductReplaceIssueVM model)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = model.Id.ToString(), DataVM = model };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                var record = _commonRepo.DetailsDelete("ProductReplaceIssueDetails", new[] { "ProductReplaceIssueId" }, new[] { model.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }


                result = await _repo.Update(model, conn, transaction);


                if (result.Status.ToLower() == "success")
                {
                    int lineNo = 1;
                    foreach (var details in model.ProductReplaceIssueDetails)
                    {
                        details.ProductReplaceIssueId = model.Id;
                        details.Line = lineNo;

                        var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                        if (resultDetail.Status.ToLower() == "success")
                        {
                            lineNo++;
                        }
                        else
                        {
                            throw new Exception(resultDetail.Message);
                        }
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

        // 3. Delete Method
        public async Task<ResultVM> Delete(int[] IDs)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.Delete(IDs, conn, transaction);

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

        // 4. GetGridData Method
        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };
            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                result = await _repo.List(conditionalFields, conditionalValues, null, conn, transaction);

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

        // 5. GetDetailsGridData Method
        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.GetDetailsGridData(options, conditionalFields, conditionalValues, conn, transaction);

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

        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();

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
        public async Task<ResultVM> FromProductReplaceReceiveGridData(GridOptions options)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.FromProductReplaceReceiveGridData(options, conn, transaction);

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


        public async Task<ResultVM> GetProductReplaceIssueDetailDataById(GridOptions options, int masterId)
        {
            ProductReplaceIssueRepository _repo = new ProductReplaceIssueRepository();
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

                result = await _repo.GetProductReplaceIssueDetailDataById(options, masterId, conn, transaction);

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


    }
}
