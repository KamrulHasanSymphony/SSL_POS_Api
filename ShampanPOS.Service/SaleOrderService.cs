using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class SaleOrderService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(SaleOrderVM saleOrder)
        {
            string CodeGroup = "SaleOrder";
            string CodeName = "SaleOrder";
            SaleOrderRepository _repo = new SaleOrderRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            CampaignRepository _repoCampaign = new CampaignRepository();
            ProductRepository _repoProduct = new ProductRepository();
            PeramModel paramModel = new PeramModel();

            CommonVM commonVM = new CommonVM();

            commonVM.Group = "StockControl";
            commonVM.Name = "StockControl";

            _commonRepo = new CommonRepository();


            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            bool CampaignByQuantitiesExist = false;
            int CampaignByQuantitiesId = 0;
            bool CampaignByProductValuesExist = false;
            int CampaignByProductValuesId = 0;

            bool CampaignByProductTotalValueExist = false;
            int CampaignByProductTotalValueId = 0;

            bool CampaignByInvoiceValueExist = false;
            int CampaignByInvoiceValueId = 0;

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();
                if (saleOrder.CustomerId == 0 || saleOrder.CustomerId == null)
                {
                    throw new Exception("Customer Is Required!");
                }

                if (saleOrder.SalePersonId == 0 || saleOrder.SalePersonId == null)
                {
                    throw new Exception("SalePerson Is Required!");
                }

                if (saleOrder.saleOrderDetailsList == null || !saleOrder.saleOrderDetailsList.Any())
                {
                    throw new Exception("Sale Order must have at least one detail!");
                }

                // Sum up the quantities if there are multiple items in the list
                var totalQuantity = saleOrder.saleOrderDetailsList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Stock Check

                paramModel.FromDate = saleOrder.InvoiceDateTime;
                paramModel.CustomerId = saleOrder.CustomerId;
                paramModel.BranchId = saleOrder.BranchId.ToString();

                result = await _repo.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { commonVM.Group, commonVM.Name }, new PeramModel(), conn, transaction);
                string settingValue = result.Value.ToString();

                if (settingValue.ToLower() == "y")
                {
                    foreach (var product in saleOrder.saleOrderDetailsList)
                    {
                        paramModel.ProductId = product.ProductId;

                        result = await _repo.GetProductStockQuantity(new[] { "" }, new[] { "" }, paramModel, conn, transaction);

                        if (result.Status == "Success" && result.DataVM != null)
                        {
                            try
                            {
                                var data = result.DataVM as List<ProductDataVM>;
                                var firstItem = data.First();

                                var ProductName = firstItem.ProductName;
                                var QuantityInHand = firstItem.QuantityInHand;

                                if (data == null || data.Count == 0)
                                    throw new Exception("Data conversion failed or no data found.");

                                if(product.Quantity > QuantityInHand)
                                    throw new Exception($"Product '{ProductName}' quantity is higher than available stock ({QuantityInHand}).");

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        else
                        {
                            throw new Exception("Product Storck data not found!");
                        }
                    }

                }

                #endregion


                #region Campaign Exist Check
                #region CampaignByQuantitiesExist
                (CampaignByQuantitiesExist, CampaignByQuantitiesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 25, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductValuesExist, CampaignByProductValuesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 26, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductTotalValueExist, CampaignByProductTotalValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 27, conn, transaction);
                #endregion

                #region CampaignByInvoiceValueExist
                (CampaignByInvoiceValueExist, CampaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 24, conn, transaction);
                #endregion

                #endregion

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { saleOrder.BranchId.ToString() }, null, conn, transaction);

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

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, saleOrder.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    saleOrder.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region CustomerData
                var customerData = _repoCustomer.List(new[] { "M.Id" }, new[] { saleOrder.CustomerId.ToString() }, null, conn, transaction);
                string jsonString = JsonConvert.SerializeObject(customerData.Result.DataVM);
                List<CustomerVM> customer = JsonConvert.DeserializeObject<List<CustomerVM>>(jsonString);

                //CustomerVM vm = JsonConvert.DeserializeObject<List<CustomerVM>>(customerData.Result.DataVM.ToString()).FirstOrDefault();

                //List<CustomerVM> customer = IdentityExtensions.DeserializeJson<List<CustomerVM>>(customerData.Result.DataVM.ToString());

                saleOrder.RouteId = customer.FirstOrDefault().RouteId;
                #endregion

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, saleOrder.InvoiceDateTime, saleOrder.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    saleOrder.Code = code;

                    result = await _repo.Insert(saleOrder, conn, transaction);
                    saleOrder.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {



                        int LineNo = 1;

                        int campaignByProductTotalValueId = 0;
                        bool campaignByInvoiceValueExist = false;
                        int campaignByInvoiceValueId = 0;
                        decimal subtotal = 0;
                        decimal DiscountGain = 0;
                        decimal SumtotalAfterDisCount = 0;
                        decimal VATAmountAfterDisCount = 0;
                        decimal LineDiscountGain = 0;
                        decimal InvoiceDiscount = 0;
                        decimal totalInvoiceValue = 0;

                        foreach (var details in saleOrder.saleOrderDetailsList)
                        {
                            #region Campaign set up
                            CampaignDetailByQuantityVM CampaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
                            CampaignDetailByProductValueVM CampaignDetailByproductValueVM = new CampaignDetailByProductValueVM();
                            CampaignDetailByProductTotalValueVM CampaignDetailByproductTotalValueVM = new CampaignDetailByProductTotalValueVM();

                            #region  CampaignByQuantity Discount
                            if (CampaignByQuantitiesExist)
                            {
                                var CampaignByQuantity = await _repoCampaign.CampaignByQuantityDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByQuantitiesId.ToString() }, null, conn, transaction);

                                if (CampaignByQuantity.Status == "Success" && CampaignByQuantity.DataVM is DataTable dt)
                                {
                                    string json = JsonConvert.SerializeObject(dt);
                                    var CampaignByQuantitydetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);
                                    CampaignDetailByQuantityVM = CampaignByQuantitydetails
                              .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId);

                                    if (CampaignDetailByQuantityVM == null)
                                    {
                                        CampaignDetailByQuantityVM = CampaignByQuantitydetails
                               .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId);
                                    }

                                    #region Calculation 

                                    if (CampaignDetailByQuantityVM != null)
                                    {
                                        details.CampaignHeaderId = CampaignDetailByQuantityVM.CampaignId;
                                        details.CampaignDetailsId = CampaignDetailByQuantityVM.Id;
                                        details.CampaignTypeId = CampaignDetailByQuantityVM.CampaignTypeId;

                                        int Quantityabilty = (int)Math.Floor((decimal)details.Quantity / CampaignDetailByQuantityVM.FromQuantity);
                                        int FreeQuantityGain = Convert.ToInt32(CampaignDetailByQuantityVM.FreeQuantity * Quantityabilty);
                                        DataTable campaignByQuantityDiscount = new DataTable("SaleOrderCampaignByQuantityDiscount");

                                        // Define columns with their default values
                                        var columns = new[]
                                         {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 25 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.ProductId },
                                            new DataColumn("FreeProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.FreeProductId },
                                            new DataColumn("FreeQuantity", typeof(int)) { DefaultValue = FreeQuantityGain }
                                        };

                                        // Add columns to DataTable
                                        campaignByQuantityDiscount.Columns.AddRange(columns);

                                        // Add a row with the default values
                                        campaignByQuantityDiscount.Rows.Add();
                                        if (Quantityabilty != 0)
                                        {
                                            var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByQuantityDiscount", campaignByQuantityDiscount, conn, transaction);

                                            if (resultt.Status.ToLower() != "success")
                                            {
                                                throw new Exception(result.ExMessage.ToString());
                                            }
                                        }

                                    }


                                    #endregion
                                }


                            }
                            #endregion

                            #region  CampaignByProductValues Discount
                            if (CampaignByProductValuesExist)
                            {
                                var CampaignByProductValues = await _repoCampaign.CampaignProductValueDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByProductValuesId.ToString() }, null, conn, transaction);

                                if (CampaignByProductValues.Status == "Success" && CampaignByProductValues.DataVM is DataTable dt)
                                {
                                    string json = JsonConvert.SerializeObject(dt);
                                    var CampaignByProductValuesdetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);
                                    CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                              .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);

                                    if (CampaignDetailByproductValueVM == null)
                                    {
                                        CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                               .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);
                                    }

                                    #region Calculation 

                                    if (CampaignDetailByproductValueVM != null)
                                    {
                                        details.CampaignHeaderId = CampaignDetailByproductValueVM.CampaignId;
                                        details.CampaignDetailsId = CampaignDetailByproductValueVM.Id;
                                        details.CampaignTypeId = CampaignDetailByproductValueVM.CampaignTypeId;
                                        DiscountGain = Convert.ToDecimal(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice * details.SubTotal / 100);

                                        DataTable campaignByproductValueDiscount = new DataTable("campaignByproductValueDiscount");


                                        SumtotalAfterDisCount = (details.SubTotal ?? 0) - DiscountGain;
                                        VATAmountAfterDisCount = ((SumtotalAfterDisCount + details.SDAmount ?? 0) * details.VATRate ?? 0) / 100;
                                        InvoiceDiscount += SumtotalAfterDisCount + VATAmountAfterDisCount;

                                        // Define columns with their default values
                                        var columns = new[]
                                         {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 26 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductValueVM.ProductId },
                                            new DataColumn("DiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice)},
                                            new DataColumn("DiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(DiscountGain) }
                                        };

                                        // Add columns to DataTable
                                        campaignByproductValueDiscount.Columns.AddRange(columns);

                                        // Add a row with the default values
                                        campaignByproductValueDiscount.Rows.Add();

                                        var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByProductUnitRate", campaignByproductValueDiscount, conn, transaction);
                                        if (resultt.Status.ToLower() != "success")
                                        {
                                            throw new Exception(result.ExMessage.ToString());
                                        }


                                    }


                                    #endregion
                                }


                            }
                            #endregion

                            #region  CampaignByProductTotalValue Discount
                            if (CampaignByProductTotalValueExist)
                            {
                                var CampaignDetailByproductTotalValue = await _repoCampaign.CampaignByProductTotalValueList(new[] { "D.CampaignId" }, new[] { CampaignByProductTotalValueId.ToString() }, null, conn, transaction);

                                if (CampaignDetailByproductTotalValue.Status == "Success" && CampaignDetailByproductTotalValue.DataVM is DataTable dt)
                                {
                                    string json = JsonConvert.SerializeObject(dt);
                                    var CampaignByProductTotalValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);

                                    CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                              .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);

                                    if (CampaignDetailByproductTotalValueVM == null)
                                    {
                                        CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                               .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);
                                    }

                                    #region Calculation 

                                    if (CampaignDetailByproductTotalValueVM != null)
                                    {
                                        details.CampaignHeaderId = CampaignDetailByproductTotalValueVM.CampaignId;
                                        details.CampaignDetailsId = CampaignDetailByproductTotalValueVM.Id;
                                        details.CampaignTypeId = CampaignDetailByproductTotalValueVM.CampaignTypeId;
                                        LineDiscountGain = Convert.ToDecimal(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice * SumtotalAfterDisCount / 100);

                                        InvoiceDiscount += -LineDiscountGain;

                                        DataTable campaignByproductTotalValueDiscount = new DataTable("campaignByproductTotalValueDiscount");

                                        // Define columns with their default values
                                        var columns = new[]
                                         {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 27 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductTotalValueVM.ProductId },
                                            new DataColumn("LineDiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice)},
                                            new DataColumn("LineDiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(LineDiscountGain) }
                                        };

                                        // Add columns to DataTable
                                        campaignByproductTotalValueDiscount.Columns.AddRange(columns);

                                        // Add a row with the default values
                                        campaignByproductTotalValueDiscount.Rows.Add();

                                        var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByProductTotalPrice", campaignByproductTotalValueDiscount, conn, transaction);
                                        if (resultt.Status.ToLower() != "success")
                                        {
                                            throw new Exception(result.ExMessage.ToString());
                                        }


                                    }


                                    #endregion
                                }


                            }
                            #endregion



                            #endregion


                            details.SaleOrderId = saleOrder.Id;
                            details.SDAmount = 0;
                            details.VATAmount = 0;
                            details.BranchId = saleOrder.BranchId;
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
                                //details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
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
                                details.VATAmount = ((details.SubTotal + details.SDAmount) * details.VATRate) / 100;
                                //details.VATAmount = (details.SubTotal * details.VATRate) / 100;
                            }

                            details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

                            #endregion

                            details.CustomerId = saleOrder.CustomerId;
                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                            if (resultDetail.Status.ToLower() == "success")
                            {

                                LineNo++;
                            }
                            else
                            {
                                result.Message = resultDetail.Message;
                                throw new Exception(result.Message);
                            }
                        }

                        #region  Campaign DetailBy InvoiceValues Discount
                        CampaignDetailByInvoiceValueVM CampaignDetailByInvoiceValue = new CampaignDetailByInvoiceValueVM();

                        if (CampaignByInvoiceValueExist)
                        {
                            var CampaignByInvoiceValue = await _repoCampaign.CampaignDetailByInvoiceValueList(new[] { "D.CampaignId" }, new[] { CampaignByInvoiceValueId.ToString() }, null, conn, transaction);

                            if (CampaignByInvoiceValue.Status == "Success" && CampaignByInvoiceValue.DataVM is DataTable dt)
                            {
                                string json = JsonConvert.SerializeObject(dt);
                                var CampaignByInvoicedetails = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);
                                CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                          .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId);

                                if (CampaignDetailByInvoiceValue == null)
                                {
                                    CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                           .FirstOrDefault(record => record.CustomerId == 0);
                                }

                                #region Calculation 

                                if (CampaignDetailByInvoiceValue != null)
                                {
                                    saleOrder.InvoiceDiscountRate = CampaignByInvoicedetails.FirstOrDefault().DiscountRateBasedOnTotalPrice;

                                    _repo.InvoiceUpdate(saleOrder);

                                }


                                #endregion
                            }


                        }
                        #endregion

                        var grndResult = await _repo.UpdateGrandTotal(saleOrder, conn, transaction);


                        if (grndResult.Status.ToLower() == "fail")
                        {
                            throw new Exception(grndResult.Message);
                        }
                    }
                    else
                    {
                        throw new Exception(result.Message);
                    }


                    #region Sale persion Visit histry auto check
                    var VisitHistriedata = await new SalePersonVisitHistrieRepository().List(new[] { "M.SalePersonId", "M.RouteId", "M.DATE" }, new[] { saleOrder.SalePersonId.ToString(), saleOrder.RouteId.ToString(), saleOrder.InvoiceDateTime }, null, conn, transaction);
                    if (VisitHistriedata.Status == "Success" && VisitHistriedata.DataVM is List<SalePersonVisitHistrieVM> VisitHistrieList)
                    {
                        SalePersonVisitHistrieVM vm = new SalePersonVisitHistrieVM();

                        if (VisitHistrieList.Count > 0)
                        {
                            SalePersonVisitHistrieVM data = VisitHistrieList.FirstOrDefault();

                            var customerVisits = data.SalePersonVisitHistrieDetails
           .Where(v => v.CustomerId == saleOrder.CustomerId)
           .ToList();

                            if (!customerVisits.FirstOrDefault().IsVisited)
                            {
                                customerVisits.FirstOrDefault().IsVisited = true;
                                customerVisits.FirstOrDefault().SaleOrderId = saleOrder.Id;

                                var resultTt = await new SalePersonVisitHistrieRepository().DetailsUpdate(new List<SalePersonVisitHistrieDetailVM> { customerVisits.FirstOrDefault() }, conn, transaction);

                            }
                        }
                        else
                        {
                            vm.Date = saleOrder.InvoiceDateTime;
                            vm.BranchId = saleOrder.BranchId;
                            vm.SalePersonId = saleOrder.SalePersonId;
                            vm.RouteId = saleOrder.RouteId;
                            var resultVisitHistrie = await new SalePersonVisitHistrieRepository().Insert(vm, conn, transaction);
                            if (resultVisitHistrie.Status == "Success")
                            {
                                ResultVM customerdata = await new SalePersonVisitHistrieRepository().CostomerList(vm, conn, transaction);

                                string customerjson = JsonConvert.SerializeObject(customerdata.DataVM);
                                DataTable dt = JsonConvert.DeserializeObject<DataTable>(customerjson);

                                if (dt == null || dt.Rows.Count == 0)
                                {
                                    throw new Exception("No Customer Found For This Route!");
                                }

                                if (customerdata.Status == "Success" && dt.Rows.Count > 0)
                                {
                                    string json = JsonConvert.SerializeObject(dt);
                                    var details = JsonConvert.DeserializeObject<List<CustomerVM>>(json);

                                    List<SalePersonVisitHistrieDetailVM> SalePersonVisitHistrieDetails = new List<SalePersonVisitHistrieDetailVM>();

                                    foreach (var item in details)
                                    {
                                        SalePersonVisitHistrieDetailVM SalePersondetails = new SalePersonVisitHistrieDetailVM();
                                        SalePersondetails.SalePersonVisitHistroyId = Convert.ToInt32(resultVisitHistrie.Id);
                                        SalePersondetails.BranchId = vm.BranchId;
                                        SalePersondetails.CustomerId = item.Id;
                                        SalePersondetails.IsVisited = false;
                                        SalePersondetails.SaleOrderId = saleOrder.Id;
                                        SalePersonVisitHistrieDetails.Add(SalePersondetails);
                                    }
                                    vm.SalePersonVisitHistrieDetails = SalePersonVisitHistrieDetails;

                                }
                                foreach (var details in vm.SalePersonVisitHistrieDetails)
                                {
                                    var resultVisitHistri = await new SalePersonVisitHistrieRepository().DetailsInsert(new List<SalePersonVisitHistrieDetailVM> { details }, conn, transaction);


                                    if (resultVisitHistri.Status == "Success")
                                    {
                                        string VisitHistrijson = JsonConvert.SerializeObject(resultVisitHistri.DataVM);
                                        DataTable dtt = JsonConvert.DeserializeObject<DataTable>(VisitHistrijson);

                                        string json = JsonConvert.SerializeObject(dtt);
                                        var model = JsonConvert.DeserializeObject<List<SalePersonVisitHistrieDetailVM>>(json);
                                        if (model.FirstOrDefault().CustomerId == saleOrder.CustomerId)
                                        {
                                            model.FirstOrDefault().IsVisited = true;
                                            var resultT = await new SalePersonVisitHistrieRepository().DetailsUpdate(new List<SalePersonVisitHistrieDetailVM> { model.FirstOrDefault() }, conn, transaction);

                                        }
                                    }

                                }
                            }
                        }

                    }
                    #endregion

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

        public async Task<ResultVM> Update(SaleOrderVM saleOrder)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
            CampaignRepository _repoCampaign = new CampaignRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PeramModel paramModel = new PeramModel();
            CommonVM commonVM = new CommonVM();

            commonVM.Group = "StockControl";
            commonVM.Name = "StockControl";

            bool isNewConnection = false;
            bool CampaignByQuantitiesExist = false;
            int CampaignByQuantitiesId = 0;
            bool CampaignByProductValuesExist = false;
            int CampaignByProductValuesId = 0;

            bool CampaignByProductTotalValueExist = false;
            int CampaignByProductTotalValueId = 0;

            bool CampaignByInvoiceValueExist = false;
            int CampaignByInvoiceValueId = 0;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                if (saleOrder.saleOrderDetailsList == null || !saleOrder.saleOrderDetailsList.Any())
                {
                    throw new Exception("Sale Order must have at least one detail!");
                }

                // Sum up the quantities if there are multiple items in the list
                var totalQuantity = saleOrder.saleOrderDetailsList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Stock Check

                paramModel.FromDate = saleOrder.InvoiceDateTime;
                paramModel.CustomerId = saleOrder.CustomerId;
                paramModel.BranchId = saleOrder.BranchId.ToString();

                result = await _repo.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { commonVM.Group, commonVM.Name }, new PeramModel(), conn, transaction);
                string settingValue = result.Value.ToString();

                if (settingValue.ToLower() == "y")
                {
                    foreach (var product in saleOrder.saleOrderDetailsList)
                    {
                        paramModel.ProductId = product.ProductId;

                        result = await _repo.GetProductStockQuantity(new[] { "" }, new[] { "" }, paramModel, conn, transaction);

                        if (result.Status == "Success" && result.DataVM != null)
                        {
                            try
                            {
                                var data = result.DataVM as List<ProductDataVM>;
                                var firstItem = data.First();

                                var ProductName = firstItem.ProductName;
                                var QuantityInHand = firstItem.QuantityInHand;

                                if (data == null || data.Count == 0)
                                    throw new Exception("Data conversion failed or no data found.");

                                if (product.Quantity > QuantityInHand)
                                    throw new Exception($"Product '{ProductName}' quantity is higher than available stock ({QuantityInHand}).");

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                        else
                        {
                            throw new Exception("Product Storck data not found!");
                        }
                    }

                }

                #endregion

                #region Campaign Exist Check
                #region CampaignByQuantitiesExist
                (CampaignByQuantitiesExist, CampaignByQuantitiesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 25, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductValuesExist, CampaignByProductValuesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 26, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductTotalValueExist, CampaignByProductTotalValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 27, conn, transaction);
                #endregion

                #region CampaignByInvoiceValueExist
                (CampaignByInvoiceValueExist, CampaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleOrder.DeliveryDate), saleOrder.BranchId ?? 1, 24, conn, transaction);
                #endregion

                #endregion

                #region Currency Data

                string CurrencyFromId = "";
                var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { saleOrder.BranchId.ToString() }, null, conn, transaction);

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

                var currencyConversation = await new CurrencyConversationRepository().List(new[] { "M.FromId", "M.ToId" }, new[] { CurrencyFromId, saleOrder.CurrencyId.ToString() }, null, conn, transaction);
                if (currencyConversation.Status == "Success" && currencyConversation.DataVM is List<CurrencyConversionVM> currencyList)
                {
                    saleOrder.CurrencyRateFromBDT = currencyList.FirstOrDefault()?.ConversationFactor > 0 ? currencyList.FirstOrDefault()?.ConversationFactor : 1;
                }
                else
                {
                    throw new Exception("Currency data not found!");
                }

                #endregion

                #region CustomerData
                var customerData = _repoCustomer.List(new[] { "M.Id" }, new[] { saleOrder.CustomerId.ToString() }, null, conn, transaction);
                string jsonString = JsonConvert.SerializeObject(customerData.Result.DataVM);
                List<CustomerVM> customer = JsonConvert.DeserializeObject<List<CustomerVM>>(jsonString);

                //CustomerVM vm = JsonConvert.DeserializeObject<List<CustomerVM>>(customerData.Result.DataVM.ToString()).FirstOrDefault();

                //List<CustomerVM> customer = IdentityExtensions.DeserializeJson<List<CustomerVM>>(customerData.Result.DataVM.ToString());

                saleOrder.RouteId = customer.FirstOrDefault().RouteId;
                #endregion

                var record = _commonRepo.DetailsDelete("SaleOrderDetails", new[] { "SaleOrderId" }, new[] { saleOrder.Id.ToString() }, conn, transaction);


                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }


                result = await _repo.Update(saleOrder, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    int campaignByProductTotalValueId = 0;
                    bool campaignByInvoiceValueExist = false;
                    int campaignByInvoiceValueId = 0;
                    decimal subtotal = 0;
                    decimal DiscountGain = 0;
                    decimal SumtotalAfterDisCount = 0;
                    decimal VATAmountAfterDisCount = 0;
                    decimal LineDiscountGain = 0;
                    decimal InvoiceDiscount = 0;
                    decimal totalInvoiceValue = 0;



                    foreach (var details in saleOrder.saleOrderDetailsList)
                    {
                        ResultVM Deleterecord = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

                        Deleterecord = _commonRepo.DetailsDelete("SaleOrderCampaignByQuantityDiscount", new[] { "SaleDetailId" }, new[] { saleOrder.Id.ToString() }, conn, transaction);
                        Deleterecord = _commonRepo.DetailsDelete("SaleOrderCampaignByProductUnitRate", new[] { "SaleDetailId" }, new[] { saleOrder.Id.ToString() }, conn, transaction);
                        Deleterecord = _commonRepo.DetailsDelete("SaleOrderCampaignByProductTotalPrice", new[] { "SaleDetailId" }, new[] { saleOrder.Id.ToString() }, conn, transaction);
                        if (Deleterecord.Status == "Fail")
                        {
                            throw new Exception("Error in Delete for Details Data.");
                        }


                        #region Campaign set up
                        CampaignDetailByQuantityVM CampaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
                        CampaignDetailByProductValueVM CampaignDetailByproductValueVM = new CampaignDetailByProductValueVM();
                        CampaignDetailByProductTotalValueVM CampaignDetailByproductTotalValueVM = new CampaignDetailByProductTotalValueVM();

                        #region  CampaignByQuantity Discount
                        if (CampaignByQuantitiesExist)
                        {
                            var CampaignByQuantity = await _repoCampaign.CampaignByQuantityDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByQuantitiesId.ToString() }, null, conn, transaction);

                            if (CampaignByQuantity.Status == "Success" && CampaignByQuantity.DataVM is DataTable dt)
                            {
                                string json = JsonConvert.SerializeObject(dt);
                                var CampaignByQuantitydetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);
                                CampaignDetailByQuantityVM = CampaignByQuantitydetails
                          .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId);

                                if (CampaignDetailByQuantityVM == null)
                                {
                                    CampaignDetailByQuantityVM = CampaignByQuantitydetails
                           .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId);
                                }

                                #region Calculation 

                                if (CampaignDetailByQuantityVM != null)
                                {
                                    details.CampaignHeaderId = CampaignDetailByQuantityVM.CampaignId;
                                    details.CampaignDetailsId = CampaignDetailByQuantityVM.Id;
                                    details.CampaignTypeId = CampaignDetailByQuantityVM.CampaignTypeId;

                                    int Quantityabilty = (int)Math.Floor((decimal)details.Quantity / CampaignDetailByQuantityVM.FromQuantity);
                                    int FreeQuantityGain = Convert.ToInt32(CampaignDetailByQuantityVM.FreeQuantity * Quantityabilty);
                                    DataTable campaignByQuantityDiscount = new DataTable("SaleOrderCampaignByQuantityDiscount");

                                    // Define columns with their default values
                                    var columns = new[]
                                     {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 25 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.ProductId },
                                            new DataColumn("FreeProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.FreeProductId },
                                            new DataColumn("FreeQuantity", typeof(int)) { DefaultValue = FreeQuantityGain }
                                        };

                                    // Add columns to DataTable
                                    campaignByQuantityDiscount.Columns.AddRange(columns);

                                    // Add a row with the default values
                                    campaignByQuantityDiscount.Rows.Add();
                                    if (Quantityabilty != 0)
                                    {
                                        var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByQuantityDiscount", campaignByQuantityDiscount, conn, transaction);

                                        if (resultt.Status.ToLower() != "success")
                                        {
                                            throw new Exception(result.ExMessage.ToString());
                                        }
                                    }

                                }


                                #endregion
                            }


                        }
                        #endregion

                        #region  CampaignByProductValues Discount
                        if (CampaignByProductValuesExist)
                        {
                            var CampaignByProductValues = await _repoCampaign.CampaignProductValueDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByProductValuesId.ToString() }, null, conn, transaction);

                            if (CampaignByProductValues.Status == "Success" && CampaignByProductValues.DataVM is DataTable dt)
                            {
                                string json = JsonConvert.SerializeObject(dt);
                                var CampaignByProductValuesdetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);
                                CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                          .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);

                                if (CampaignDetailByproductValueVM == null)
                                {
                                    CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                           .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);
                                }

                                #region Calculation 

                                if (CampaignDetailByproductValueVM != null)
                                {
                                    details.CampaignHeaderId = CampaignDetailByproductValueVM.CampaignId;
                                    details.CampaignDetailsId = CampaignDetailByproductValueVM.Id;
                                    details.CampaignTypeId = CampaignDetailByproductValueVM.CampaignTypeId;
                                    DiscountGain = Convert.ToDecimal(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice * details.SubTotal / 100);

                                    DataTable campaignByproductValueDiscount = new DataTable("campaignByproductValueDiscount");


                                    SumtotalAfterDisCount = (details.SubTotal ?? 0) - DiscountGain;
                                    VATAmountAfterDisCount = ((SumtotalAfterDisCount + details.SDAmount ?? 0) * details.VATRate ?? 0) / 100;
                                    InvoiceDiscount += SumtotalAfterDisCount + VATAmountAfterDisCount;

                                    // Define columns with their default values
                                    var columns = new[]
                                     {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 26 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductValueVM.ProductId },
                                            new DataColumn("DiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice)},
                                            new DataColumn("DiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(DiscountGain) }
                                        };

                                    // Add columns to DataTable
                                    campaignByproductValueDiscount.Columns.AddRange(columns);

                                    // Add a row with the default values
                                    campaignByproductValueDiscount.Rows.Add();

                                    var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByProductUnitRate", campaignByproductValueDiscount, conn, transaction);
                                    if (resultt.Status.ToLower() != "success")
                                    {
                                        throw new Exception(result.ExMessage.ToString());
                                    }


                                }


                                #endregion
                            }


                        }
                        #endregion

                        #region  CampaignByProductTotalValue Discount
                        if (CampaignByProductTotalValueExist)
                        {
                            var CampaignDetailByproductTotalValue = await _repoCampaign.CampaignByProductTotalValueList(new[] { "D.CampaignId" }, new[] { CampaignByProductTotalValueId.ToString() }, null, conn, transaction);

                            if (CampaignDetailByproductTotalValue.Status == "Success" && CampaignDetailByproductTotalValue.DataVM is DataTable dt)
                            {
                                string json = JsonConvert.SerializeObject(dt);
                                var CampaignByProductTotalValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);

                                CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                          .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);

                                if (CampaignDetailByproductTotalValueVM == null)
                                {
                                    CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                           .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);
                                }

                                #region Calculation 

                                if (CampaignDetailByproductTotalValueVM != null)
                                {
                                    details.CampaignHeaderId = CampaignDetailByproductTotalValueVM.CampaignId;
                                    details.CampaignDetailsId = CampaignDetailByproductTotalValueVM.Id;
                                    details.CampaignTypeId = CampaignDetailByproductTotalValueVM.CampaignTypeId;
                                    LineDiscountGain = Convert.ToDecimal(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice * SumtotalAfterDisCount / 100);

                                    InvoiceDiscount += -LineDiscountGain;

                                    DataTable campaignByproductTotalValueDiscount = new DataTable("campaignByproductTotalValueDiscount");

                                    // Define columns with their default values
                                    var columns = new[]
                                     {
                                            new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleOrder.Id },
                                            new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleOrder.CustomerId },
                                            new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                                            new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 27 },
                                            new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductTotalValueVM.ProductId },
                                            new DataColumn("LineDiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice)},
                                            new DataColumn("LineDiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(LineDiscountGain) }
                                        };

                                    // Add columns to DataTable
                                    campaignByproductTotalValueDiscount.Columns.AddRange(columns);

                                    // Add a row with the default values
                                    campaignByproductTotalValueDiscount.Rows.Add();

                                    var resultt = await _commonRepo.BulkInsert("SaleOrderCampaignByProductTotalPrice", campaignByproductTotalValueDiscount, conn, transaction);
                                    if (resultt.Status.ToLower() != "success")
                                    {
                                        throw new Exception(result.ExMessage.ToString());
                                    }


                                }


                                #endregion
                            }


                        }
                        #endregion



                        #endregion

                        details.SaleOrderId = saleOrder.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = saleOrder.BranchId;
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
                            //details.UOMConversion = uomConvList.FirstOrDefault()?.ConversationFactor > 0 ? uomConvList.FirstOrDefault()?.ConversationFactor : 1;
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
                            details.VATAmount = ((details.SubTotal + details.SDAmount) * details.VATRate) / 100;
                            // details.VATAmount = (details.SubTotal * details.VATRate) / 100;
                        }

                        details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

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
                    #region  Campaign DetailBy InvoiceValues Discount
                    CampaignDetailByInvoiceValueVM CampaignDetailByInvoiceValue = new CampaignDetailByInvoiceValueVM();

                    if (CampaignByInvoiceValueExist)
                    {
                        var CampaignByInvoiceValue = await _repoCampaign.CampaignDetailByInvoiceValueList(new[] { "D.CampaignId" }, new[] { CampaignByInvoiceValueId.ToString() }, null, conn, transaction);

                        if (CampaignByInvoiceValue.Status == "Success" && CampaignByInvoiceValue.DataVM is DataTable dt)
                        {
                            string json = JsonConvert.SerializeObject(dt);
                            var CampaignByInvoicedetails = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);
                            CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                      .FirstOrDefault(record => record.CustomerId == saleOrder.CustomerId);

                            if (CampaignDetailByInvoiceValue == null)
                            {
                                CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                       .FirstOrDefault(record => record.CustomerId == 0);
                            }

                            #region Calculation 

                            if (CampaignDetailByInvoiceValue != null)
                            {
                                saleOrder.InvoiceDiscountRate = CampaignByInvoicedetails.FirstOrDefault().DiscountRateBasedOnTotalPrice;

                                _repo.InvoiceUpdate(saleOrder);

                            }


                            #endregion
                        }


                    }
                    #endregion

                    var grndResult = await _repo.UpdateGrandTotal(saleOrder, conn, transaction);

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
            SaleOrderRepository _repo = new SaleOrderRepository();
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
            SaleOrderRepository _repo = new SaleOrderRepository();
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

        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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
            SaleOrderRepository _repo = new SaleOrderRepository();
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

        public async Task<ResultVM> MultiplePost(CommonVM vm)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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
            SaleOrderRepository _repo = new SaleOrderRepository();
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
            SaleOrderRepository _repo = new SaleOrderRepository();
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
        public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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

                var lst = new List<SaleDeliveryVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<SaleDeliveryVM>>(data);

                var detailsDataList = await _repo.SaleOrderDetailsList(new[] { "D.SaleOrderId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDeliveryDetailVM>>(json);

                    lst.FirstOrDefault().saleDeliveryDetailList = details;
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

        public async Task<ResultVM> GetSaleOrderDetailDataById(GridOptions options, int masterId)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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

                result = await _repo.GetSaleOrderDetailDataById(options, masterId, conn, transaction);

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

        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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

                result = await _repo.FromSaleOrderGridData(options, conn, transaction);

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
            SaleOrderRepository _repo = new SaleOrderRepository();
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

                result = await _repo.ProductWiseSaleOrder(conditionalFields, conditionalValues, vm, conn, transaction);

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
            SaleOrderRepository _repo = new SaleOrderRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Sale Order Invoice" };
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

        public async Task<ResultVM> GetOrderNoWiseGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
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

                result = await _repo.GetOrderNoWiseGridData(options, conditionalFields, conditionalValues, conn, transaction);

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
