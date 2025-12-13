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
    public class SaleDeliveryReturnService
    {
        CommonRepository _commonRepo = new CommonRepository();
        SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();

        public async Task<ResultVM> Insert(SaleDeliveryReturnVM saleDelivery)
        {
            string CodeGroup = "SaleDeliveryReturn";
            string CodeName = "SaleDeliveryReturn";
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
            SaleOrderRepository _orderRepo = new SaleOrderRepository();
            SaleDeliveryRepository _sdlvRepo = new SaleDeliveryRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            SaleReturnService _serviceSaleReturn = new SaleReturnService();

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
                var totalQuantity = saleDelivery.saleDeliveryReturnDetailList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { saleDelivery.BranchId.ToString() }, null, conn, transaction);

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

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, saleDelivery.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    saleDelivery.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region CustomerData
                var customerData = _repoCustomer.List(new[] { "M.Id" }, new[] { saleDelivery.CustomerId.ToString() }, null, conn, transaction);
                string jsonString = JsonConvert.SerializeObject(customerData.Result.DataVM);
                List<CustomerVM> customer = JsonConvert.DeserializeObject<List<CustomerVM>>(jsonString);

                //CustomerVM vm = JsonConvert.DeserializeObject<List<CustomerVM>>(customerData.Result.DataVM.ToString()).FirstOrDefault();

                //List<CustomerVM> customer = IdentityExtensions.DeserializeJson<List<CustomerVM>>(customerData.Result.DataVM.ToString());

                saleDelivery.RouteId = customer.FirstOrDefault().RouteId;
                #endregion


                var saleReturnVM = MapSaleDeliveryToSale(saleDelivery);
                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, saleDelivery.InvoiceDateTime.ToString(), saleDelivery.BranchId , conn, transaction);
                saleDelivery.Code = code;
                saleReturnVM.Code = code;
                if (!string.IsNullOrEmpty(code))
                {
                    

                    result = await _repo.Insert(saleDelivery, conn, transaction);
                    //saleDelivery.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {
                        int LineNo = 1;
                        foreach (var details in saleDelivery.saleDeliveryReturnDetailList)
                        {
                            details.SaleDeliveryId = Convert.ToInt32(saleDelivery.SaleDeliveryId);
                            details.SaleDeliveryReturnId = Convert.ToInt32(result.Id);
                            details.BranchId = saleDelivery.BranchId;
                            details.Line = LineNo;
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
                                details.VATAmount = (details.SubTotal * details.VATRate) / 100;
                            }

                            details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

                            #endregion

                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                            if (resultDetail.Status.ToLower() == "success")
                            {
                                LineNo++;
                                foreach (var detail in saleReturnVM.saleReturnDetailList)
                                {
                                    detail.SaleReturnId = Convert.ToInt32(result.Id);
                                    detail.SaleId = Convert.ToInt32(result.Id);
                                }
                            }
                            else
                            {
                                result.Message = resultDetail.Message;
                                throw new Exception(result.Message);
                            }
                        }
                        var grndResult = await _repo.UpdateGrandTotal(Convert.ToInt32(result.Id), conn, transaction);

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


        public async Task<ResultVM> Update(SaleDeliveryReturnVM saleDelivery)
        {
            _repo = new SaleDeliveryReturnRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            SaleDeliveryRepository _sdlvRepo = new SaleDeliveryRepository();
            SaleReturnService _serviceSale = new SaleReturnService();
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
                if (saleDelivery.saleDeliveryReturnDetailList == null || !saleDelivery.saleDeliveryReturnDetailList.Any())
                {
                    throw new Exception("Sale Delivery must have at least one detail!");
                }

                // Sum up the quantities if there are multiple items in the list
                var totalQuantity = saleDelivery.saleDeliveryReturnDetailList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { saleDelivery.BranchId.ToString() }, null, conn, transaction);

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

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, saleDelivery.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    saleDelivery.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region CustomerData
                var customerData = _repoCustomer.List(new[] { "M.Id" }, new[] { saleDelivery.CustomerId.ToString() }, null, conn, transaction);
                string jsonString = JsonConvert.SerializeObject(customerData.Result.DataVM);
                List<CustomerVM> customer = JsonConvert.DeserializeObject<List<CustomerVM>>(jsonString);


                saleDelivery.RouteId = customer.FirstOrDefault().RouteId;
                #endregion

                var record = _commonRepo.DetailsDelete("SaleDeleveryReturnDetails", new[] { "SaleDeliveryReturnId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }
                var saleRecord = _commonRepo.DetailsDelete("SaleReturnDetails", new[] { "SaleReturnId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);
                if (saleRecord.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Sale Details Data.");
                }
                var saleVM = MapSaleDeliveryToSale(saleDelivery);

                result = await _repo.Update(saleDelivery, conn, transaction);

                if (isNewConnection && result.Status == "Success")
                {
                    int LineNo = 1;
                    saleVM.Id = Convert.ToInt32(result.Id);
                    _serviceSale.Update(saleVM);

                    foreach (var details in saleDelivery.saleDeliveryReturnDetailList)
                    {
                        details.SaleDeliveryReturnId = saleDelivery.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = saleDelivery.BranchId;
                        details.Line = LineNo;

                        #region UOM Data

                        var uomdata = await new ProductRepository().List(new[] { "M.Id" }, new[] { details.UOMId.ToString() }, null, conn, transaction);
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
                            details.VATAmount = (details.SubTotal * details.VATRate) / 100;
                        }

                        details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

                        #endregion

                        var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                        if (resultDetail.Status.ToLower() == "success")
                        {
                            LineNo++;
                            _sdlvRepo.UpdateSaleDeliveryIsComplete(details.SaleDeliveryId, conn, transaction);
                        }
                        else
                        {
                            throw new Exception(resultDetail.Message);
                        }
                    }
                    await _serviceSale.DetailInsert(saleVM);
                    var grndResult = await _repo.UpdateGrandTotal(saleDelivery.Id, conn, transaction);

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

        public async Task<ResultVM> Delete(string[] IDs)
        {
            _repo = new SaleDeliveryReturnRepository();
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

        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            _repo = new SaleDeliveryReturnRepository();
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

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            _repo = new SaleDeliveryReturnRepository();
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

        public async Task<ResultVM> Dropdown()
        {
            _repo = new SaleDeliveryReturnRepository();
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
            _repo = new SaleDeliveryReturnRepository();
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


        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            _repo = new SaleDeliveryReturnRepository();
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


        public async Task<ResultVM> GetDetailsGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
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


        public async Task<ResultVM> FromSaleDeliveryGridData(GridOptions options)
        {
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
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

                result = await _repo.FromSaleDeliveryGridData(options, conn, transaction);

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

        public async Task<ResultVM> GetSaleDeliveryReturnDetailDataById(GridOptions options, int masterId)
        {
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
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

                result = await _repo.GetSaleDeliveryReturnDetailDataById(options, masterId, conn, transaction);

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
        public SaleReturnVM MapSaleDeliveryToSale(SaleDeliveryReturnVM saleDeliveryReturn)
        {
            return new SaleReturnVM
            {
                Code = saleDeliveryReturn.Code,
                BranchId = saleDeliveryReturn.BranchId ?? 0,
                CustomerId = saleDeliveryReturn.CustomerId ?? 0,
                SalePersonId = saleDeliveryReturn.SalePersonId ?? 0,
                RouteId = saleDeliveryReturn.RouteId ?? 0,
                DeliveryAddress = saleDeliveryReturn.DeliveryAddress,
                VehicleNo = saleDeliveryReturn.VehicleNo,
                VehicleType = saleDeliveryReturn.VehicleType,
                InvoiceDateTime = saleDeliveryReturn.InvoiceDateTime,
                DeliveryDate = saleDeliveryReturn.DeliveryDate,
                GrandTotalAmount = saleDeliveryReturn.GrandTotalAmount ?? 0,
                GrandTotalSDAmount = saleDeliveryReturn.GrandTotalSDAmount ?? 0,
                GrandTotalVATAmount = saleDeliveryReturn.GrandTotalVATAmount ?? 0,
                Comments = saleDeliveryReturn.Comments,
                IsPrint = false, // This should be set to false initially since it's a new sale return
                PrintBy = "",
                PrintOn = "",
                TransactionType = "Sale Return",
                IsPost = false, 
                PostBy = "",
                PostedOn = "",
                FiscalYear = "", 
                PeriodId = "", 
                CurrencyId = saleDeliveryReturn.CurrencyId ?? 0,
                CurrencyRateFromBDT = saleDeliveryReturn.CurrencyRateFromBDT ?? 1,
                CreatedBy = "", 
                CreatedOn = DateTime.Now.ToString(),
                LastModifiedBy = "",
                LastModifiedOn = "",
                CreatedFrom = saleDeliveryReturn.CreatedFrom,
                LastUpdateFrom = "",

                // Mapping SaleDeliveryReturnDetailVM to SaleReturnDetailVM
                saleReturnDetailList = saleDeliveryReturn.saleDeliveryReturnDetailList?.Select(detail => new SaleReturnDetailVM
                {
                    Id = 0, 
                    SaleReturnId = 0, 
                    SaleId = 0,                  
                    BranchId = detail.BranchId ?? 0,
                    Line = detail.Line ?? 0,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitRate = detail.UnitRate ?? 0,
                    SubTotal = detail.SubTotal ?? 0,
                    SD = detail.SD ?? 0, 
                    SDAmount = detail.SDAmount ?? 0,
                    VATRate = detail.VATRate ?? 0,
                    VATAmount = detail.VATAmount ?? 0,
                    LineTotal = detail.LineTotal ?? 0,
                    UOMId = detail.UOMId ?? 0,
                    UOMName = detail.UOMName,
                    UOMFromId = detail.UOMFromId ?? 0,
                    UOMFromName = detail.UOMFromName,
                    UOMConversion = detail.UOMConversion ?? 1,
                    Comments = detail.Comments,
                    TransactionType = "Sale Return", 
                    IsPost = false
                }).ToList() ?? new List<SaleReturnDetailVM>()
            };
        }

        public async Task<ResultVM> SummaryReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
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

                result = await _repo.ProductWiseSaleDeliveryReturn(conditionalFields, conditionalValues, vm, conn, transaction);

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
            SaleDeliveryReturnRepository _repo = new SaleDeliveryReturnRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Sale Delivery Return Invoice" };
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
