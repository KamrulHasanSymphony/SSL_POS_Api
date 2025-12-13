using Newtonsoft.Json;
using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class PurchaseReturnService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(PurchaseReturnVM model)
        {
            string CodeGroup = "PurchaseReturn";
            string CodeName = "PurchaseReturn";
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                #region Current Fiscal Period Status
                var MonthName = Convert.ToDateTime(model.PurchaseReturnDate).ToString("MMM-yy");
                var periodData = new FiscalYearRepository().DetailsList(new[] { "D.MonthName" }, new[] { MonthName }, null, conn, transaction);

                if (periodData.Status == "Success" && periodData.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<FiscalYearDetailVM>>(json);
                    if (details.Count == 0)
                    {
                        throw new Exception("Fiscal Year data not found!");
                    }
                    var data = details.FirstOrDefault();
                    model.PeriodId = data.FiscalYearId.ToString();
                    model.FiscalYear = data.Year.ToString();

                    if (data.MonthLock)
                    {
                        throw new Exception("This Fiscal Period: " + data.MonthName + " is Locked!");
                    }
                }
                else
                {
                    throw new Exception("Fiscal Year data not found!");
                }
                #endregion

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { model.BranchId.ToString() }, null, conn, transaction);

                if (currencyData.Status == "Success" && currencyData.DataVM is List<BranchProfileVM> branch)
                {
                    CurrencyFromId = branch.FirstOrDefault().CurrencyId.ToString();
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region Currency Conversation Data

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, model.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    model.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, model.PurchaseReturnDate, model.BranchId , conn, transaction);

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
                        foreach (var details in model.purchaseReturnDetailList)
                        {
                            idList.Add(details.PurchaseId != null ? details.PurchaseId.ToString() : "0");
                            commonVM.IDs = idList.ToArray();

                            details.PurchaseReturnId = model.Id;
                            details.SDAmount = 0;
                            details.VATAmount = 0;
                            details.BranchId = model.BranchId;
                            details.TransactionType = model.TransactionType;
                            details.Line = LineNo;
                            details.IsPost = model.IsPost;

                            #region UOM Data

                            var uomdata = await new ProductRepository().List(new[] { "M.Id" }, new[] { details.ProductId.ToString() }, null, conn, transaction);
                            if (uomdata.Status == "Success" && uomdata.DataVM is List<ProductVM> uomList)
                            {
                                details.UOMFromId = uomList.FirstOrDefault()?.UOMId;
                            }
                            else
                            {
                                throw new Exception("UOM data not found!");
                            }

                            #endregion

                            #region UOM Conversation Data

                            var uomConversation = await new UOMConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { details.UOMFromId.ToString(), details.UOMId.ToString() }, null, conn, transaction);
                            if (uomConversation.Status == "Success" && uomConversation.DataVM is List<UOMConversationVM> uomConvList)
                            {
                                details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
                            }
                            else
                            {
                                throw new Exception("UOM Conversation data not found!");
                            }

                            #endregion

                            #region Product Batch History Param with ProductId & BranchId Data

                            var sdVATdata = await new ProductBatchHistoryRepository().List(new[] { "ProductId", "BranchId" }, new[] { details.ProductId.ToString(), details.BranchId.ToString() }, null, conn, transaction);
                            if (sdVATdata.Status == "Success" && sdVATdata.DataVM is List<ProductBatchHistoryVM> sdVatList)
                            {
                                details.SD = sdVatList.FirstOrDefault()?.SD > 0 ? sdVatList.FirstOrDefault()?.SD : 0;
                                details.VATRate = sdVatList.FirstOrDefault()?.VATRate > 0 ? sdVatList.FirstOrDefault()?.VATRate : 0;
                            }
                            else
                            {
                                throw new Exception("UOM data not found!");
                            }

                            #endregion

                            #region Product Batch History Param with ProductId & BranchId=0 Data

                            if (sdVatList.Count == 0)
                            {
                                sdVATdata = await new ProductBatchHistoryRepository().List(new[] { "ProductId", "BranchId" }, new[] { details.ProductId.ToString(), "0" }, null, conn, transaction);
                                if (sdVATdata.Status == "Success" && sdVATdata.DataVM is List<ProductBatchHistoryVM> sdVatsList)
                                {
                                    details.SD = sdVatsList.FirstOrDefault()?.SD > 0 ? sdVatsList.FirstOrDefault()?.SD : 0;
                                    details.VATRate = sdVatsList.FirstOrDefault()?.VATRate > 0 ? sdVatsList.FirstOrDefault()?.VATRate : 0;
                                }
                                else
                                {
                                    throw new Exception("UOM data not found!");
                                }
                            }

                            #endregion

                            #region Line Total Summation
                            if (details.SD > 0)
                            {
                                details.SDAmount = (details.SubTotal * details.SD) / 100;
                            }
                            if (details.VATRate > 0)
                            {
                                details.VATAmount = ((details.SubTotal + details.SDAmount + details.OthersAmount) * details.VATRate) / 100;
                            }

                            details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount + details.OthersAmount;

                            #endregion

                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                            if (resultDetail.Status.ToLower() == "success")
                            {
                                LineNo++;
                                if(details.PurchaseDetailId > 0)
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

                        var grndResult = await _repo.UpdateGrandTotal(model, conn, transaction);

                        if (grndResult.Status.ToLower() == "fail")
                        {
                            throw new Exception(grndResult.Message);
                        }

                        foreach (var item in commonVM.IDs)
                        {
                            PeramModel peramModel = new PeramModel();
                            peramModel.Id = item;
                            model.PurchaseId = Convert.ToInt32(peramModel.Id);

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


        public async Task<ResultVM> Update(PurchaseReturnVM model)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            _commonRepo = new CommonRepository();
            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                #region Current Fiscal Period Status
                var MonthName = Convert.ToDateTime(model.PurchaseReturnDate).ToString("MMM-yy");
                var periodData = new FiscalYearRepository().DetailsList(new[] { "D.MonthName" }, new[] { MonthName }, null, conn, transaction);

                if (periodData.Status == "Success" && periodData.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<FiscalYearDetailVM>>(json);
                    if (details.Count == 0)
                    {
                        throw new Exception("Fiscal Year data not found!");
                    }
                    var data = details.FirstOrDefault();
                    model.PeriodId = data.FiscalYearId.ToString();
                    model.FiscalYear = data.Year.ToString();

                    if (data.MonthLock)
                    {
                        throw new Exception("This Fiscal Period: " + data.MonthName + " is Locked!");
                    }
                }
                else
                {
                    throw new Exception("Fiscal Year data not found!");
                }
                #endregion

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { model.BranchId.ToString() }, null, conn, transaction);

                if (currencyData.Status == "Success" && currencyData.DataVM is List<BranchProfileVM> branch)
                {
                    CurrencyFromId = branch.FirstOrDefault().CurrencyId.ToString();
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region Currency Conversation Data

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, model.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    model.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                var record = _commonRepo.DetailsDelete("PurchaseReturnDetails", new[] { "PurchaseReturnId" }, new[] { model.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                result = await _repo.Update(model, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    foreach (var details in model.purchaseReturnDetailList)
                    {
                        details.PurchaseReturnId = model.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = model.BranchId;
                        details.TransactionType = model.TransactionType;
                        details.Line = LineNo;
                        details.IsPost = model.IsPost;

                        #region UOM Data

                        var uomdata = await new ProductRepository().List(new[] { "M.Id" }, new[] { details.ProductId.ToString() }, null, conn, transaction);
                        if (uomdata.Status == "Success" && uomdata.DataVM is List<ProductVM> uomList)
                        {
                            details.UOMFromId = uomList.FirstOrDefault()?.UOMId;
                        }
                        else
                        {
                            throw new Exception("UOM data not found!");
                        }

                        #endregion

                        #region UOM Conversation Data

                        var uomConversation = await new UOMConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { details.UOMFromId.ToString(), details.UOMId.ToString() }, null, conn, transaction);
                        if (uomConversation.Status == "Success" && uomConversation.DataVM is List<UOMConversationVM> uomConvList)
                        {
                            details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
                        }
                        else
                        {
                            throw new Exception("UOM Conversation data not found!");
                        }

                        #endregion

                        #region Product Batch History Param with ProductId & BranchId Data

                        var sdVATdata = await new ProductBatchHistoryRepository().List(new[] { "ProductId", "BranchId" }, new[] { details.ProductId.ToString(), details.BranchId.ToString() }, null, conn, transaction);
                        if (sdVATdata.Status == "Success" && sdVATdata.DataVM is List<ProductBatchHistoryVM> sdVatList)
                        {
                            details.SD = sdVatList.FirstOrDefault()?.SD > 0 ? sdVatList.FirstOrDefault()?.SD : 0;
                            details.VATRate = sdVatList.FirstOrDefault()?.VATRate > 0 ? sdVatList.FirstOrDefault()?.VATRate : 0;
                        }
                        else
                        {
                            throw new Exception("UOM data not found!");
                        }

                        #endregion

                        #region Product Batch History Param with ProductId & BranchId=0 Data

                        if (sdVatList.Count == 0)
                        {
                            sdVATdata = await new ProductBatchHistoryRepository().List(new[] { "ProductId", "BranchId" }, new[] { details.ProductId.ToString(), "0" }, null, conn, transaction);
                            if (sdVATdata.Status == "Success" && sdVATdata.DataVM is List<ProductBatchHistoryVM> sdVatsList)
                            {
                                details.SD = sdVatsList.FirstOrDefault()?.SD > 0 ? sdVatsList.FirstOrDefault()?.SD : 0;
                                details.VATRate = sdVatsList.FirstOrDefault()?.VATRate > 0 ? sdVatsList.FirstOrDefault()?.VATRate : 0;
                            }
                            else
                            {
                                throw new Exception("UOM data not found!");
                            }
                        }

                        #endregion

                        #region Line Total Summation
                        if (details.SD > 0)
                        {
                            details.SDAmount = (details.SubTotal * details.SD) / 100;
                        }
                        if (details.VATRate > 0)
                        {
                            details.VATAmount = ((details.SubTotal + details.SDAmount + details.OthersAmount) * details.VATRate) / 100;
                        }

                        details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount + details.OthersAmount;

                        #endregion

                        var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                        if (resultDetail.Status.ToLower() == "success")
                        {
                            LineNo++;
                        }
                        else
                        {
                            throw new Exception(resultDetail.Message);
                        }
                    }

                    var grndResult = await _repo.UpdateGrandTotal(model, conn, transaction);

                    if (grndResult.Status.ToLower() == "fail")
                    {
                        throw new Exception(grndResult.Message);
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

        public async Task<ResultVM> Delete(string[] IDs)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                var lst = new List<PurchaseReturnVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<PurchaseReturnVM>>(data);

                var detailsDataList = await _repo.DetailsList(new[] { "D.PurchaseReturnId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<PurchaseReturnDetailVM>>(json);

                    lst.FirstOrDefault().purchaseReturnDetailList = details;
                    lst.FirstOrDefault().PurchaseId = details.FirstOrDefault().PurchaseId;
                    result.DataVM = lst;
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

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> Dropdown()
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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


        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                var companyData = await new CompanyProfileRepository().List(new[] { "H.Id" }, new[] { options.vm.CompanyId }, null, conn, transaction);
                string companyName = string.Empty;
                if (companyData.Status == "Success" && companyData.DataVM is List<CompanyProfileVM> company)
                {
                    companyName = company.FirstOrDefault()?.CompanyName;
                }

                if (result.Status == "Success" && !string.IsNullOrEmpty(companyName) && result.DataVM is GridEntity<PurchaseReturnVM> gridData)
                {
                    var items = gridData.Items;
                    items.ToList().ForEach(item => item.CompanyName = companyName);
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

        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

        public async Task<ResultVM> GetPurchaseReturnDetailDataById(GridOptions options, int masterId)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                result = await _repo.GetPurchaseReturnDetailDataById(options, masterId, conn, transaction);

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

        public async Task<ResultVM> SummaryReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                result = await _repo.ProductWisePurchaseReturn(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseReturnRepository _repo = new PurchaseReturnRepository();
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

                result = await _repo.ReportPreview(conditionalFields, conditionalValues, vm, conn, transaction);

                var companyData = await new CompanyProfileRepository().List(new[] { "H.Id" }, new[] { vm.CompanyId }, null, conn, transaction);
                string companyName = string.Empty;
                if (companyData.Status == "Success" && companyData.DataVM is List<CompanyProfileVM> company)
                {
                    companyName = company.FirstOrDefault()?.CompanyName;
                }

                if (result.Status == "Success" && !string.IsNullOrEmpty(companyName) && result.DataVM is DataTable dataTable)
                {
                    if (!dataTable.Columns.Contains("CompanyName"))
                    {
                        var CompanyName = new DataColumn("CompanyName") { DefaultValue = companyName };
                        dataTable.Columns.Add(CompanyName);
                    }

                    if (!dataTable.Columns.Contains("ReportType"))
                    {
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Purchase Return Invoice" };
                        dataTable.Columns.Add(ReportType);
                    }

                    result.DataVM = dataTable;
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
