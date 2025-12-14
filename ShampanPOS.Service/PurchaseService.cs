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
    public class PurchaseService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(PurchaseVM model)
        {
            string CodeGroup = "Purchase";
            string CodeName = "Purchase";
            PurchaseRepository _repo = new PurchaseRepository();
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

                #region Date Check
                if (Convert.ToDateTime(model.PurchaseDate) < Convert.ToDateTime(model.InvoiceDateTime))
                {
                    throw new Exception("Purchase Date cannot be smaller then Invoice Date!");
                }
                #endregion

                #region Current Fiscal Period Status
                var MonthName = Convert.ToDateTime(model.InvoiceDateTime).ToString("MMM-yy");
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

                //#region Currency Conversation Data

                //var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, model.CurrencyId.ToString() }, null, conn, transaction);
                //if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                //{
                //    model.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                //}
                //else
                //{
                //    throw new Exception("Currency data not found!");
                //}

                //#endregion

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, model.InvoiceDateTime, model.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    model.Code = code;
                    model.TransactionType = "Purchase";

                    result = await _repo.Insert(model, conn, transaction);
                    model.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {
                        CommonVM commonVM = new CommonVM();
                        var idList = new List<string?>();
                        int LineNo = 1;
                        foreach (var details in model.purchaseDetailList)
                        {
                            idList.Add(details.PurchaseOrderId != null ? details.PurchaseOrderId.ToString() : "0");
                            commonVM.IDs = idList.ToArray();

                            details.PurchaseId = model.Id;
                            details.SDAmount = 0;
                            details.VATAmount = 0;
                            details.BranchId = model.BranchId;
                            //details.TransactionType = model.TransactionType;
                            details.Line = LineNo;
                            //details.IsPost = model.IsPost;

                            //#region UOM Data

                            //var uomdata = await new ProductRepository().List(new[] { "M.Id" }, new[] { details.ProductId.ToString() }, null, conn, transaction);
                            //if (uomdata.Status == "Success" && uomdata.DataVM is List<ProductVM> uomList)
                            //{
                            //    details.UOMFromId = uomList.FirstOrDefault()?.UOMId;
                            //}
                            //else
                            //{
                            //    throw new Exception("UOM data not found!");
                            //}

                            //#endregion

                            //#region UOM Conversation Data

                            //var uomConversation = await new UOMConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { details.UOMFromId.ToString(), details.UOMId.ToString() }, null, conn, transaction);
                            //if (uomConversation.Status == "Success" && uomConversation.DataVM is List<UOMConversationVM> uomConvList)
                            //{
                            //    details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
                            //}
                            //else
                            //{
                            //    throw new Exception("UOM Conversation data not found!");
                            //}

                            //#endregion

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
                                if (details.PurchaseOrderDetailId > 0)
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
                            model.PurchaseOrderId = Convert.ToInt32(peramModel.Id);

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

        public async Task<ResultVM> Update(PurchaseVM model)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                #region Date Check
                if (Convert.ToDateTime(model.PurchaseDate) < Convert.ToDateTime(model.InvoiceDateTime))
                {
                    throw new Exception("Purchase Date cannot be smaller then Invoice Date!");
                }
                #endregion

                #region Current Fiscal Period Status
                var MonthName = Convert.ToDateTime(model.InvoiceDateTime).ToString("MMM-yy");
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

                //#region Currency Conversation Data

                //var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, model.CurrencyId.ToString() }, null, conn, transaction);
                //if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                //{
                //    model.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                //}
                //else
                //{
                //    throw new Exception("Currency data not found!");
                //}

                //#endregion

                var record = _commonRepo.DetailsDelete("PurchaseDetails", new[] { "PurchaseId" }, new[] { model.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                result = await _repo.Update(model, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    foreach (var details in model.purchaseDetailList)
                    {
                        details.PurchaseId = model.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = model.BranchId;
                        //details.TransactionType = model.TransactionType;
                        details.Line = LineNo;
                        //details.IsPost = model.IsPost;

                        //#region UOM Data

                        //var uomdata = await new ProductRepository().List(new[] { "M.Id" }, new[] { details.UOMId.ToString() }, null, conn, transaction);
                        //if (uomdata.Status == "Success" && uomdata.DataVM is List<ProductVM> uomList)
                        //{
                        //    details.UOMFromId = uomList.FirstOrDefault()?.UOMId;
                        //}
                        //else
                        //{
                        //    throw new Exception("UOM data not found!");
                        //}

                        //#endregion

                        //#region UOM Conversation Data

                        //var uomConversation = await new UOMConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { details.UOMFromId.ToString(), details.UOMId.ToString() }, null, conn, transaction);
                        //if (uomConversation.Status == "Success" && uomConversation.DataVM is List<UOMConversationVM> uomConvList)
                        //{
                        //    details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
                        //}
                        //else
                        //{
                        //    throw new Exception("UOM Conversation data not found!");
                        //}

                        //#endregion

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

        public async Task<ResultVM> Delete(CommonVM model)
        {
            PurchaseRepository _repo = new PurchaseRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = model.IDs, DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                result = await _repo.Delete(model, conn, transaction);

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

        //Add
        public async Task<ResultVM> ImportExcelFileInsert(PurchaseVM model)
        {
            SupplierRepository _repoSupplier = new SupplierRepository();
            BranchProfileRepository _repoBranch = new BranchProfileRepository();
            ProductRepository _repoProduct = new ProductRepository();
            UOMRepository _repoUOM = new UOMRepository();
            CommonService _commonService = new CommonService();
            PurchaseRepository _repo = new PurchaseRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };
            DataTable TempPurchaseData = new DataTable("TempPurchaseData");
            PeramModel vm = new PeramModel();
            vm.OrderName = "P.Id";
            vm.startRec = 0;
            vm.pageSize = 10;
            var lst = new PurchaseVM();


            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();


                #region TempTable Delete AND Set Value

                var record = _commonRepo.DetailsDelete("TempPurchaseData", new[] { "" }, new[] { "" }, conn, transaction);

                if (record.Status == "Success")
                {
                    var columns = new[]
                    {
                                 new DataColumn("PurchaseCode", typeof(string)),
                                 new DataColumn("BranchCode", typeof(string)),
                                 new DataColumn("SupplierCode", typeof(string)),
                                 new DataColumn("InvoiceDateTime", typeof(DateTime)),
                                 new DataColumn("ProductCode", typeof(string)),
                                 new DataColumn("UOMCode", typeof(string)),
                                 new DataColumn("Quantity", typeof(decimal))
                    };

                    TempPurchaseData.Columns.AddRange(columns);

                    foreach (var item in model.purchaseDetailExportList)
                    {
                        TempPurchaseData.Rows.Add(
                            item.PurchaseCode,
                            item.BranchCode,
                            item.SupplierCode,
                            item.InvoiceDateTime,
                            item.ProductCode,
                            item.UOMCode,
                            item.Quantity
                        );
                    }
                }

                #endregion


                #region BulKInsert

                var resultt = await _commonRepo.BulkInsert("TempPurchaseData", TempPurchaseData, conn, transaction);

                if (resultt.Status.ToLower() != "success")
                {
                    return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
                }

                if (resultt.Status.ToLower() == "success" && isNewConnection)
                {
                    transaction.Commit();
                }

                #endregion



                #region Get TempPurchase Data

                var exportDetailsDataList = await _repo.ExportDetailsList(new[] { "" }, new[] { "" }, vm, conn, transaction);

                if (exportDetailsDataList.Status == "Success" && exportDetailsDataList.DataVM is DataTable dt)
                {

                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<PurchaseDetailExportVM>>(json);
                    lst.purchaseDetailExportList = details;
                    result.DataVM = lst;

                }

                #endregion

                #region Branch Code Check Exist Data

                bool branchExist = true;

                foreach (var item in lst.purchaseDetailExportList)
                {
                    string[] conditionField = { "Code" };
                    string[] conditionValue = { item.BranchCode.Trim() };

                    branchExist = _commonRepo.CheckExists("BranchProfiles", conditionField, conditionValue, conn, transaction);

                    if (!branchExist)
                    {
                        return new ResultVM { Status = "Fail", Message = $"This Branch Code {item.BranchCode.Trim()} does not exist", DataVM = null };
                    }
                }

                #endregion

                #region Supplier Code Check Exist Data

                bool supplierExist = true;

                foreach (var item in lst.purchaseDetailExportList)
                {
                    string[] conditionField = { "Code" };
                    string[] conditionValue = { item.SupplierCode.Trim() };

                    supplierExist = _commonRepo.CheckExists("Suppliers", conditionField, conditionValue, conn, transaction);

                    if (!supplierExist)
                    {
                        return new ResultVM { Status = "Fail", Message = $"This Supplier Code {item.SupplierCode.Trim()} does not exist", DataVM = null };
                    }
                }

                #endregion

                #region UOMs Code Check Exist Data

                bool UOMsExist = true;

                foreach (var item in lst.purchaseDetailExportList)
                {
                    string[] conditionField = { "Code" };
                    string[] conditionValue = { item.UOMCode.Trim() };

                    UOMsExist = _commonRepo.CheckExists("UOMs", conditionField, conditionValue, conn, transaction);

                    if (!UOMsExist)
                    {
                        return new ResultVM { Status = "Fail", Message = $"This UOMs Code {item.UOMCode.Trim()} does not exist", DataVM = null };

                    }
                }

                #endregion


                #region Distinct List by PurchaseCode,BranchCode,SupplierCode,InvoiceDateTime

                var distinctList = lst.purchaseDetailExportList
    .GroupBy(x => new { x.PurchaseCode, x.BranchCode, x.SupplierCode, x.InvoiceDateTime })
    .Select(g => g.First())
    .ToList();

                #endregion


                #region Check ImportIdExcel  

                bool importIDExcelExist = false;
                foreach (var importIdExcel in distinctList)
                {
                    string[] conditionField = { "ImportIDExcel" };
                    string[] conditionValue = { importIdExcel.PurchaseCode.Trim() };
                    importIDExcelExist = _commonRepo.CheckExists("Purchases", conditionField, conditionValue, conn, transaction);
                    if (importIDExcelExist)
                    {
                        return new ResultVM { Status = "Fail", Message = $"This PurchaseCode/Import ID {importIdExcel.PurchaseCode.Trim()} Is Already  exist", DataVM = null };
                    }
                }

                #endregion


                #region Set Header And Details Value 

                foreach (var item in distinctList)
                {

                    #region Set Header Value 

                    result = await _repoSupplier.List(new[] { "Code" }, new[] { item.SupplierCode }, vm, conn, transaction);
                    var jsonString = JsonConvert.SerializeObject(result.DataVM);
                    var purchaseList = JsonConvert.DeserializeObject<List<PurchaseVM>>(jsonString);


                    result = await _repoBranch.List(new[] { "H.Code" }, new[] { item.BranchCode }, vm, conn, transaction);
                    jsonString = JsonConvert.SerializeObject(result.DataVM);
                    var branchList = JsonConvert.DeserializeObject<List<PurchaseVM>>(jsonString);


                    PurchaseVM vmData = new PurchaseVM();
                    //vmData.ImportIDExcel = item.PurchaseCode;
                    vmData.InvoiceDateTime = item.InvoiceDateTime;
                    vmData.PurchaseDate = item.InvoiceDateTime;
                    vmData.SupplierId = purchaseList.FirstOrDefault().Id;
                    vmData.BranchId = branchList.FirstOrDefault().Id;
                    vmData.CurrencyId = branchList.FirstOrDefault().CurrencyId;

                    vmData.CreatedBy = model.CreatedBy;
                    vmData.CreatedOn = model.CreatedOn;
                    vmData.CreatedFrom = model.CreatedFrom;

                    #endregion

                    #region Get Details Data 

                    //result = await _repo.ExportDetailsList(new[] { "BranchCode", "SupplierCode", "InvoiceDateTime" }, new[] { item.BranchCode, item.SupplierCode, item.InvoiceDateTime }, vm, conn, transaction);
                    result = await _repo.ExportDetailsList(new[] { "PurchaseCode", "BranchCode", "SupplierCode", "InvoiceDateTime" }, new[] { item.PurchaseCode, item.BranchCode, item.SupplierCode, item.InvoiceDateTime }, vm, conn, transaction);
                    var jsonTempBranchCodeString = JsonConvert.SerializeObject(result.DataVM);
                    var TempDetailList = JsonConvert.DeserializeObject<List<PurchaseDetailExportVM>>(jsonTempBranchCodeString);

                    #endregion


                    #region Sum Quantity by PurchaseCode,ProductCode  

                    var finalList = TempDetailList
                                    .GroupBy(x => new { x.PurchaseCode, x.ProductCode })
                                    .Select(g => new PurchaseDetailExportVM
                                    {
                                        Id = g.First().Id,
                                        PurchaseCode = g.Key.PurchaseCode,
                                        ProductCode = g.Key.ProductCode,
                                        BranchCode = g.First().BranchCode,
                                        SupplierCode = g.First().SupplierCode,
                                        InvoiceDateTime = g.First().InvoiceDateTime,
                                        UOMCode = g.First().UOMCode,
                                        Quantity = g.Sum(x => x.Quantity)
                                    })
                                    .ToList();


                    //var finalList = TempDetailList
                    //.GroupBy(x => new { x.PurchaseCode, x.BranchCode, x.SupplierCode, x.InvoiceDateTime, x.ProductCode })
                    //.Select(g => new PurchaseDetailExportVM
                    //{
                    //    PurchaseCode = g.Key.PurchaseCode,
                    //    BranchCode = g.Key.BranchCode,
                    //    SupplierCode = g.Key.SupplierCode,
                    //    InvoiceDateTime = g.Key.InvoiceDateTime,
                    //    ProductCode = g.Key.ProductCode,
                    //    Quantity = g.Sum(x => x.Quantity)
                    //})
                    //.ToList();

                    #endregion


                    //foreach (var itemData in TempDetailList)
                    foreach (var itemData in finalList)
                    {

                        PurchaseDetailVM detail = new PurchaseDetailVM();

                        //result = await _repo.ExportDetailsList(new[] { "Id" }, new[] { itemData.Id.ToString() }, vm, conn, transaction);
                        //var jsonTempQuantityString = JsonConvert.SerializeObject(result.DataVM);
                        //var TempQuantityList = JsonConvert.DeserializeObject<List<PurchaseDetailExportVM>>(jsonTempQuantityString);

                        result = await _repoProduct.List(new[] { "M.Code" }, new[] { itemData.ProductCode }, vm, conn, transaction);
                        var jsonProductString = JsonConvert.SerializeObject(result.DataVM);
                        var ProductList = JsonConvert.DeserializeObject<List<PurchaseDetailVM>>(jsonProductString);

                        result = await _repoUOM.List(new[] { "H.Code" }, new[] { itemData.UOMCode }, vm, conn, transaction);
                        var jsonUOMString = JsonConvert.SerializeObject(result.DataVM);
                        var UOMList = JsonConvert.DeserializeObject<List<PurchaseDetailExportVM>>(jsonUOMString);

                        #region Get UnitPrice

                        vm.FromDate = vmData.PurchaseDate;
                        vm.BranchId = vmData.BranchId.ToString();
                        result = await _commonService.GetProductModalForPurchaseData(new[] { "PBH.ProductId" }, new[] { ProductList.FirstOrDefault().Id.ToString() }, vm);
                        var jsonUnitPriceString = JsonConvert.SerializeObject(result.DataVM);
                        var settings = new JsonSerializerSettings
                        {
                            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        var UnitPriceList = JsonConvert.DeserializeObject<List<PurchaseDetailExportVM>>(jsonUnitPriceString, settings) ?? new List<PurchaseDetailExportVM>();
                        if (UnitPriceList.Count == 0)
                        {
                            //return new ResultVM { Status = "Fail", Message = $"Unit Price is 0 for this product {ProductList.FirstOrDefault().ProductName.ToString()}", DataVM = null };
                            return new ResultVM { Status = "Fail", Message = $"Please insert Product code in ProductPurchasePriceBatchHistories for this {itemData.ProductCode.ToString()}", DataVM = null };
                        }

                        #endregion

                        #region Set Detail Value

                        detail.ProductId = ProductList.FirstOrDefault().Id;
                        detail.UnitPrice = UnitPriceList.FirstOrDefault().CostPrice;
                        //detail.UOMId = UOMList.FirstOrDefault().Id;
                        //detail.UOMName = UOMList.FirstOrDefault().Name;
                        //detail.Quantity = TempQuantityList.FirstOrDefault().Quantity;
                        detail.Quantity = itemData.Quantity;
                        detail.SubTotal = detail.UnitPrice * detail.Quantity;
                        detail.OthersAmount = 0;

                        #endregion

                        vmData.purchaseDetailList.Add(detail);

                    }

                    #region Insert Purchase

                    Task<ResultVM> rData = Insert(vmData);

                    if (rData != null)
                    {
                        if (rData.Result.Status.ToLower() != "success")
                        {
                            return new ResultVM { Status = "Fail", Message = rData.Result.Message, DataVM = null };
                        }
                    }

                    #endregion

                }

                #endregion

                return new ResultVM { Status = "Success", Message = "Data Inserted Successfully", DataVM = null };

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


        //End

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                var lst = new List<PurchaseVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<PurchaseVM>>(data);

                var detailsDataList = await _repo.DetailsList(new[] { "D.PurchaseId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<PurchaseDetailVM>>(json);

                    lst.FirstOrDefault().purchaseDetailList = details;
                    lst.FirstOrDefault().PurchaseOrderId = details.FirstOrDefault().PurchaseOrderId;
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
            PurchaseRepository _repo = new PurchaseRepository();
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
            PurchaseRepository _repo = new PurchaseRepository();
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
            PurchaseRepository _repo = new PurchaseRepository();
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

        public async Task<ResultVM> MultipleIsCompleted(CommonVM vm)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                result = await _repo.MultipleIsCompleted(vm, conn, transaction);

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
            PurchaseRepository _repo = new PurchaseRepository();
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

                if (result.Status == "Success" && !string.IsNullOrEmpty(companyName) && result.DataVM is GridEntity<PurchaseVM> gridData)
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
            PurchaseRepository _repo = new PurchaseRepository();
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

        public async Task<ResultVM> FromPurchaseGridData(GridOptions options)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                result = await _repo.FromPurchaseGridData(options, conn, transaction);

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

        public async Task<ResultVM> PurchaseList(string?[] IDs)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                result = await _repo.PurchaseList(IDs, conn, transaction);

                var lst = new List<PurchaseReturnVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<PurchaseReturnVM>>(data);

                bool allSame = lst.Select(p => p.SupplierId).Distinct().Count() == 1;
                if (!allSame)
                {
                    throw new Exception("Supplier is not distinct!");
                }

                //allSame = lst.Select(p => p.CurrencyId).Distinct().Count() == 1;
                if (!allSame)
                {
                    throw new Exception("Currency is not distinct!");
                }

                var detailsDataList = await _repo.PurchaseDetailsList(IDs, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<PurchaseReturnDetailVM>>(json);
                    //details.ToList().ForEach(item => item.POCode = lst.FirstOrDefault().Code);
                    lst.FirstOrDefault().purchaseReturnDetailList = details;
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

        public async Task<ResultVM> GetPurchaseDetailDataById(GridOptions options, int masterId)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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

                result = await _repo.GetPurchaseDetailDataById(options, masterId, conn, transaction);

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
            PurchaseRepository _repo = new PurchaseRepository();
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

                result = await _repo.ProductWisePurchase(conditionalFields, conditionalValues, vm, conn, transaction);

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

        //public async Task<ResultVM> ExportPurchaseExcel(CommonVM vm)
        //{
        //    PurchaseRepository _repo = new PurchaseRepository();
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

        //        result = await _repo.ExportPurchaseExcel(vm, conn, transaction);

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
        //        result.Message = ex.Message.ToString();
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


        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            PurchaseRepository _repo = new PurchaseRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Purchase Invoice" };
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
