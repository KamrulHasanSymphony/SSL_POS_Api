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
    public class SaleDeliveryService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(SaleDeliveryVM saleDelivery)
        {
            string CodeGroup = "SaleDelivery";
            string CodeName = "SaleDelivery";
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
            SaleOrderRepository _orderRepo = new SaleOrderRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            SaleService _serviceSale = new SaleService();
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
                //if (saleDelivery.DeliveryPersonId == 0 || saleDelivery.DeliveryPersonId == null)
                //{
                //    throw new Exception("Delivery Person Is Required!");
                //}
                //if (saleDelivery.DriverPersonId == 0 || saleDelivery.DriverPersonId == null)
                //{
                //    throw new Exception("Driver Person Is Required!");
                //}
                if (saleDelivery.SalePersonId == 0 || saleDelivery.SalePersonId == null)
                {
                    throw new Exception("Sale Person Is Required!");
                }
                if (saleDelivery.CustomerId == 0 || saleDelivery.CustomerId == null)
                {
                    throw new Exception("Customer Is Required!");
                }

                // Sum up the quantities if there are multiple items in the list
                var totalQuantity = saleDelivery.saleDeliveryDetailList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Stock Check

                paramModel.FromDate = saleDelivery.InvoiceDateTime;
                paramModel.CustomerId = saleDelivery.CustomerId;
                paramModel.BranchId = saleDelivery.BranchId.ToString();

                result = await _repo.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { commonVM.Group, commonVM.Name }, new PeramModel(), conn, transaction);
                string settingValue = result.Value.ToString();

                if (settingValue.ToLower() == "y")
                {
                    foreach (var product in saleDelivery.saleDeliveryDetailList)
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

                //#region Campaign Exist Check
                //#region CampaignByQuantitiesExist
                //(CampaignByQuantitiesExist, CampaignByQuantitiesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 25, conn, transaction);
                //#endregion

                //#region CampaignByByProductValuesExist
                //(CampaignByProductValuesExist, CampaignByProductValuesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 26, conn, transaction);
                //#endregion

                //#region CampaignByByProductValuesExist
                //(CampaignByProductTotalValueExist, CampaignByProductTotalValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 27, conn, transaction);
                //#endregion

                //#region CampaignByInvoiceValueExist
                //(CampaignByInvoiceValueExist, CampaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 24, conn, transaction);
                //#endregion

                //#endregion

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


                var saleVM = MapSaleDeliveryToSale(saleDelivery);
                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, saleDelivery.InvoiceDateTime, saleDelivery.BranchId, conn, transaction);
                saleVM.Code = code;
                if (!string.IsNullOrEmpty(code))
                {
                    saleDelivery.Code = code;


                    result = await _repo.Insert(saleDelivery, conn, transaction);

                    if (result.Status.ToLower() == "success")
                    {

                        #region  CampaignDetailByInvoiceValues Discount
                        CampaignDetailByInvoiceValueVM CampaignDetailByInvoiceValue = new CampaignDetailByInvoiceValueVM();

                        if (CampaignByInvoiceValueExist)
                        {
                            var CampaignByInvoiceValue = await _repoCampaign.CampaignDetailByInvoiceValueList(new[] { "D.CampaignId" }, new[] { CampaignByInvoiceValueId.ToString() }, null, conn, transaction);

                            if (CampaignByInvoiceValue.Status == "Success" && CampaignByInvoiceValue.DataVM is DataTable dt)
                            {
                                string json = JsonConvert.SerializeObject(dt);
                                var CampaignByInvoicedetails = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);
                                CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                          .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId);

                                if (CampaignDetailByInvoiceValue == null)
                                {
                                    CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                           .FirstOrDefault(record => record.CustomerId == 0);
                                }

                                #region Calculation 

                                if (CampaignDetailByInvoiceValue != null)
                                {
                                    saleDelivery.InvoiceDiscountRate = CampaignByInvoicedetails.FirstOrDefault().DiscountRateBasedOnTotalPrice;

                                    _repo.InvoiceUpdate(saleDelivery);

                                }


                                #endregion
                            }


                        }
                        #endregion

                        //saleVM.Id = Convert.ToInt32(result.Id);
                        //_serviceSale.Insert(saleVM);
                        int LineNo = 1;

                        int campaignByProductTotalValueId = 0;
                        bool campaignByInvoiceValueExist = false;
                        int campaignByInvoiceValueId = 0;
                        decimal subtotal = 0;
                        decimal DiscountGain = 0;
                        decimal SumtotalAfterDisCount = 0;
                        decimal LineDiscountGain = 0;
                        decimal InvoiceDiscount = 0;
                        decimal totalInvoiceValue = 0;

                        foreach (var details in saleDelivery.saleDeliveryDetailList)
                        {

                            //#region Campaign set up
                            //CampaignDetailByQuantityVM CampaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
                            //CampaignDetailByProductValueVM CampaignDetailByproductValueVM = new CampaignDetailByProductValueVM();
                            //CampaignDetailByProductTotalValueVM CampaignDetailByproductTotalValueVM = new CampaignDetailByProductTotalValueVM();

                            //#region  CampaignByQuantity Discount
                            //if (CampaignByQuantitiesExist)
                            //{
                            //    var CampaignByQuantity = await _repoCampaign.CampaignByQuantityDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByQuantitiesId.ToString() }, null, conn, transaction);

                            //    if (CampaignByQuantity.Status == "Success" && CampaignByQuantity.DataVM is DataTable dt)
                            //    {
                            //        string json = JsonConvert.SerializeObject(dt);
                            //        var CampaignByQuantitydetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);
                            //        CampaignDetailByQuantityVM = CampaignByQuantitydetails
                            //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId);

                            //        if (CampaignDetailByQuantityVM == null)
                            //        {
                            //            CampaignDetailByQuantityVM = CampaignByQuantitydetails
                            //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId);
                            //        }

                            //        #region Calculation 

                            //        if (CampaignDetailByQuantityVM != null)
                            //        {
                            //            details.CampaignHeaderId = CampaignDetailByQuantityVM.CampaignId;
                            //            details.CampaignDetailsId = CampaignDetailByQuantityVM.Id;
                            //            details.CampaignTypeId = CampaignDetailByQuantityVM.CampaignTypeId;

                            //            int Quantityabilty = (int)Math.Floor((decimal)details.Quantity / CampaignDetailByQuantityVM.FromQuantity);
                            //            int FreeQuantityGain = Convert.ToInt32(CampaignDetailByQuantityVM.FreeQuantity * Quantityabilty);
                            //            DataTable campaignByQuantityDiscount = new DataTable("SaleDeliveryCampaignByQuantityDiscount");

                            //            // Define columns with their default values
                            //            var columns = new[]
                            //             {
                            //                new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleVM.Id },
                            //                new DataColumn("CustomerId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.CustomerId },
                            //                new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                            //                new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 25 },
                            //                new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.ProductId },
                            //                new DataColumn("FreeProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.FreeProductId },
                            //                new DataColumn("FreeQuantity", typeof(int)) { DefaultValue = FreeQuantityGain }
                            //            };

                            //            // Add columns to DataTable
                            //            campaignByQuantityDiscount.Columns.AddRange(columns);

                            //            // Add a row with the default values
                            //            campaignByQuantityDiscount.Rows.Add();
                            //            if (Quantityabilty != 0)
                            //            {
                            //                var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByQuantityDiscount", campaignByQuantityDiscount, conn, transaction);
                            //                if (resultt.Status.ToLower() != "success")
                            //                {
                            //                    throw new Exception(result.ExMessage.ToString());
                            //                }
                            //            }

                            //        }


                            //        #endregion
                            //    }


                            //}
                            //#endregion

                            //#region  CampaignByProductValues Discount
                            //if (CampaignByProductValuesExist)
                            //{
                            //    var CampaignByProductValues = await _repoCampaign.CampaignProductValueDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByProductValuesId.ToString() }, null, conn, transaction);

                            //    if (CampaignByProductValues.Status == "Success" && CampaignByProductValues.DataVM is DataTable dt)
                            //    {
                            //        string json = JsonConvert.SerializeObject(dt);
                            //        var CampaignByProductValuesdetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);
                            //        CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                            //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);

                            //        if (CampaignDetailByproductValueVM == null)
                            //        {
                            //            CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                            //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);
                            //        }

                            //        #region Calculation 

                            //        if (CampaignDetailByproductValueVM != null)
                            //        {
                            //            details.CampaignHeaderId = CampaignDetailByproductValueVM.CampaignId;
                            //            details.CampaignDetailsId = CampaignDetailByproductValueVM.Id;
                            //            details.CampaignTypeId = CampaignDetailByproductValueVM.CampaignTypeId;
                            //            DiscountGain = Convert.ToDecimal(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice * details.SubTotal / 100);

                            //            DataTable campaignByproductValueDiscount = new DataTable("campaignByproductValueDiscount");

                            //            InvoiceDiscount += (details.SubTotal ?? 0) - DiscountGain;
                            //            SumtotalAfterDisCount = (details.SubTotal ?? 0) - DiscountGain;

                            //            // Define columns with their default values
                            //            var columns = new[]
                            //             {
                            //                new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleVM.Id },
                            //                new DataColumn("CustomerId", typeof(int)) { DefaultValue = CampaignDetailByproductValueVM.CustomerId },
                            //                new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                            //                new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 26 },
                            //                new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductValueVM.ProductId },
                            //                new DataColumn("DiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice)},
                            //                new DataColumn("DiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(DiscountGain) }
                            //            };

                            //            // Add columns to DataTable
                            //            campaignByproductValueDiscount.Columns.AddRange(columns);

                            //            // Add a row with the default values
                            //            campaignByproductValueDiscount.Rows.Add();

                            //            var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByProductUnitRate", campaignByproductValueDiscount, conn, transaction);
                            //            if (resultt.Status.ToLower() != "success")
                            //            {
                            //                throw new Exception(result.ExMessage.ToString());
                            //            }


                            //        }


                            //        #endregion
                            //    }


                            //}
                            //#endregion

                            //#region  CampaignByProductTotalValue Discount
                            //if (CampaignByProductTotalValueExist)
                            //{
                            //    var CampaignDetailByproductTotalValue = await _repoCampaign.CampaignByProductTotalValueList(new[] { "D.CampaignId" }, new[] { CampaignByProductTotalValueId.ToString() }, null, conn, transaction);

                            //    if (CampaignDetailByproductTotalValue.Status == "Success" && CampaignDetailByproductTotalValue.DataVM is DataTable dt)
                            //    {
                            //        string json = JsonConvert.SerializeObject(dt);
                            //        var CampaignByProductTotalValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);

                            //        CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                            //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);

                            //        if (CampaignDetailByproductTotalValueVM == null)
                            //        {
                            //            CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                            //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);
                            //        }

                            //        #region Calculation 

                            //        if (CampaignDetailByproductTotalValueVM != null)
                            //        {
                            //            details.CampaignHeaderId = CampaignDetailByproductTotalValueVM.CampaignId;
                            //            details.CampaignDetailsId = CampaignDetailByproductTotalValueVM.Id;
                            //            details.CampaignTypeId = CampaignDetailByproductTotalValueVM.CampaignTypeId;
                            //            LineDiscountGain = Convert.ToDecimal(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice * SumtotalAfterDisCount / 100);

                            //            InvoiceDiscount += -LineDiscountGain;

                            //            DataTable campaignByproductTotalValueDiscount = new DataTable("campaignByproductTotalValueDiscount");

                            //            // Define columns with their default values
                            //            var columns = new[]
                            //             {
                            //                new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleVM.Id },
                            //                new DataColumn("CustomerId", typeof(int)) { DefaultValue = CampaignDetailByproductTotalValueVM.CustomerId },
                            //                new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                            //                new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 27 },
                            //                new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductTotalValueVM.ProductId },
                            //                new DataColumn("LineDiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice)},
                            //                new DataColumn("LineDiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(LineDiscountGain) }
                            //            };

                            //            // Add columns to DataTable
                            //            campaignByproductTotalValueDiscount.Columns.AddRange(columns);

                            //            // Add a row with the default values
                            //            campaignByproductTotalValueDiscount.Rows.Add();

                            //            var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByProductTotalPrice", campaignByproductTotalValueDiscount, conn, transaction);
                            //            if (resultt.Status.ToLower() != "success")
                            //            {
                            //                throw new Exception(result.ExMessage.ToString());
                            //            }

                            //        }


                            //        #endregion
                            //    }


                            //}
                            //#endregion



                            //#endregion

                            details.SaleOrderId = Convert.ToInt32(details.SaleOrderId);
                            details.SaleOrderDetailId = Convert.ToInt32(details.SaleOrderDetailId);
                            details.SaleDeliveryId = Convert.ToInt32(result.Id);
                            details.BranchId = saleDelivery.BranchId;
                            details.Line = LineNo;
                            details.SaleOrderId = saleDelivery.SaleOrderId;

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
                            details.CustomerId = saleDelivery.CustomerId;

                            var resultDetail = await _repo.InsertDetails(details, conn, transaction);


                            //Add

                            if (resultDetail.Status.ToLower() == "success")
                            {
                                LineNo++;
                                if (details.FromSaleOrderDetailId > 0)
                                {
                                    details.SaleOrderDetailId = details.FromSaleOrderDetailId;
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

                            //End

                            if (resultDetail.Status.ToLower() == "success")
                            {

                                if (details.SaleOrderId == null || details.SaleOrderId == 0)
                                {

                                }
                                else
                                {
                                    _orderRepo.UpdateSaleOrderIsComplete(details.SaleOrderId, conn, transaction);
                                }
                                LineNo++;

                            }
                            else
                            {
                                result.Message = resultDetail.Message;
                                throw new Exception(result.Message);
                            }
                        }
                        var grndResult = await _repo.UpdateGrandTotal(saleDelivery, conn, transaction);

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


        public async Task<ResultVM> Update(SaleDeliveryVM saleDelivery)
        {
            SaleOrderRepository _orderRepo = new SaleOrderRepository();
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
            //CampaignRepository _repoCampaign = new CampaignRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            SaleService _serviceSale = new SaleService();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PeramModel paramModel = new PeramModel();
            CommonVM commonVM = new CommonVM();
            commonVM.Group = "StockControl";
            commonVM.Name = "StockControl";


            _commonRepo = new CommonRepository();
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
                if (saleDelivery.saleDeliveryDetailList == null || !saleDelivery.saleDeliveryDetailList.Any())
                {
                    throw new Exception("Sale Delivery must have at least one detail!");
                }

                // Sum up the quantities if there are multiple items in the list
                var totalQuantity = saleDelivery.saleDeliveryDetailList.Sum(detail => detail.Quantity);

                if (totalQuantity == 0)
                {
                    throw new Exception("Quantity Required!");
                }

                #region Stock Check

                paramModel.FromDate = saleDelivery.InvoiceDateTime;
                paramModel.CustomerId = saleDelivery.CustomerId;
                paramModel.BranchId = saleDelivery.BranchId.ToString();

                result = await _repo.SettingsValue(new[] { "SettingGroup", "SettingName" }, new[] { commonVM.Group, commonVM.Name }, new PeramModel(), conn, transaction);
                string settingValue = result.Value.ToString();

                if (settingValue.ToLower() == "y")
                {
                    foreach (var product in saleDelivery.saleDeliveryDetailList)
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
                (CampaignByQuantitiesExist, CampaignByQuantitiesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 25, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductValuesExist, CampaignByProductValuesId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 26, conn, transaction);
                #endregion

                #region CampaignByByProductValuesExist
                (CampaignByProductTotalValueExist, CampaignByProductTotalValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 27, conn, transaction);
                #endregion

                #region CampaignByInvoiceValueExist
                (CampaignByInvoiceValueExist, CampaignByInvoiceValueId) = _commonRepo.CampaignExists(Convert.ToDateTime(saleDelivery.DeliveryDate), saleDelivery.BranchId ?? 1, 24, conn, transaction);
                #endregion

                #endregion

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


                var record = _commonRepo.DetailsDelete("SaleDeleveryDetails", new[] { "SaleDeliveryId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                var saleRecord = _commonRepo.DetailsDelete("SaleDetails", new[] { "SaleDeliveryId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);
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

                    //_serviceSale.Update(saleVM);

                    foreach (var details in saleDelivery.saleDeliveryDetailList)
                    {


                        ResultVM Deleterecord = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

                        Deleterecord = _commonRepo.DetailsDelete("SaleDeliveryCampaignByQuantityDiscount", new[] { "SaleDetailId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);
                        Deleterecord = _commonRepo.DetailsDelete("SaleDeliveryCampaignByProductUnitRate", new[] { "SaleDetailId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);
                        Deleterecord = _commonRepo.DetailsDelete("SaleDeliveryCampaignByProductTotalPrice", new[] { "SaleDetailId" }, new[] { saleDelivery.Id.ToString() }, conn, transaction);

                        //#region Campaign set up
                        //CampaignDetailByQuantityVM CampaignDetailByQuantityVM = new CampaignDetailByQuantityVM();
                        //CampaignDetailByProductValueVM CampaignDetailByproductValueVM = new CampaignDetailByProductValueVM();
                        //CampaignDetailByProductTotalValueVM CampaignDetailByproductTotalValueVM = new CampaignDetailByProductTotalValueVM();

                        //#region  CampaignByQuantity Discount
                        //if (CampaignByQuantitiesExist)
                        //{
                        //    var CampaignByQuantity = await _repoCampaign.CampaignByQuantityDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByQuantitiesId.ToString() }, null, conn, transaction);

                        //    if (CampaignByQuantity.Status == "Success" && CampaignByQuantity.DataVM is DataTable dt)
                        //    {
                        //        string json = JsonConvert.SerializeObject(dt);
                        //        var CampaignByQuantitydetails = JsonConvert.DeserializeObject<List<CampaignDetailByQuantityVM>>(json);
                        //        CampaignDetailByQuantityVM = CampaignByQuantitydetails
                        //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId);

                        //        if (CampaignDetailByQuantityVM == null)
                        //        {
                        //            CampaignDetailByQuantityVM = CampaignByQuantitydetails
                        //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId);
                        //        }

                        //        #region Calculation 

                        //        if (CampaignDetailByQuantityVM != null)
                        //        {
                        //            details.CampaignHeaderId = CampaignDetailByQuantityVM.CampaignId;
                        //            details.CampaignDetailsId = CampaignDetailByQuantityVM.Id;
                        //            details.CampaignTypeId = CampaignDetailByQuantityVM.CampaignTypeId;

                        //            int Quantityabilty = (int)Math.Floor((decimal)details.Quantity / CampaignDetailByQuantityVM.FromQuantity);
                        //            int FreeQuantityGain = Convert.ToInt32(CampaignDetailByQuantityVM.FreeQuantity * Quantityabilty);
                        //            DataTable campaignByQuantityDiscount = new DataTable("SaleOrderCampaignByQuantityDiscount");

                        //            // Define columns with their default values
                        //            var columns = new[]
                        //             {
                        //                    new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleDelivery.Id },
                        //                    new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleDelivery.CustomerId },
                        //                    new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                        //                    new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 25 },
                        //                    new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.ProductId },
                        //                    new DataColumn("FreeProductId", typeof(int)) { DefaultValue = CampaignDetailByQuantityVM.FreeProductId },
                        //                    new DataColumn("FreeQuantity", typeof(int)) { DefaultValue = FreeQuantityGain }
                        //                };

                        //            // Add columns to DataTable
                        //            campaignByQuantityDiscount.Columns.AddRange(columns);

                        //            // Add a row with the default values
                        //            campaignByQuantityDiscount.Rows.Add();
                        //            if (Quantityabilty != 0)
                        //            {
                        //                var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByQuantityDiscount", campaignByQuantityDiscount, conn, transaction);

                        //                if (resultt.Status.ToLower() != "success")
                        //                {
                        //                    throw new Exception(result.ExMessage.ToString());
                        //                }
                        //            }

                        //        }


                        //        #endregion
                        //    }


                        //}
                        //#endregion

                        //#region  CampaignByProductValues Discount
                        //if (CampaignByProductValuesExist)
                        //{
                        //    var CampaignByProductValues = await _repoCampaign.CampaignProductValueDetailsList(new[] { "D.CampaignId" }, new[] { CampaignByProductValuesId.ToString() }, null, conn, transaction);

                        //    if (CampaignByProductValues.Status == "Success" && CampaignByProductValues.DataVM is DataTable dt)
                        //    {
                        //        string json = JsonConvert.SerializeObject(dt);
                        //        var CampaignByProductValuesdetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductValueVM>>(json);
                        //        CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                        //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);

                        //        if (CampaignDetailByproductValueVM == null)
                        //        {
                        //            CampaignDetailByproductValueVM = CampaignByProductValuesdetails
                        //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromQuantity <= details.Quantity && record.ToQuantity >= details.Quantity);
                        //        }

                        //        #region Calculation 

                        //        if (CampaignDetailByproductValueVM != null)
                        //        {
                        //            details.CampaignHeaderId = CampaignDetailByproductValueVM.CampaignId;
                        //            details.CampaignDetailsId = CampaignDetailByproductValueVM.Id;
                        //            details.CampaignTypeId = CampaignDetailByproductValueVM.CampaignTypeId;
                        //            DiscountGain = Convert.ToDecimal(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice * details.SubTotal / 100);

                        //            DataTable campaignByproductValueDiscount = new DataTable("campaignByproductValueDiscount");


                        //            SumtotalAfterDisCount = (details.SubTotal ?? 0) - DiscountGain;
                        //            VATAmountAfterDisCount = ((SumtotalAfterDisCount + details.SDAmount ?? 0) * details.VATRate ?? 0) / 100;
                        //            InvoiceDiscount += SumtotalAfterDisCount + VATAmountAfterDisCount;

                        //            // Define columns with their default values
                        //            var columns = new[]
                        //             {
                        //                    new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleDelivery.Id },
                        //                    new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleDelivery.CustomerId },
                        //                    new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                        //                    new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 26 },
                        //                    new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductValueVM.ProductId },
                        //                    new DataColumn("DiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductValueVM.DiscountRateBasedOnUnitPrice)},
                        //                    new DataColumn("DiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(DiscountGain) }
                        //                };

                        //            // Add columns to DataTable
                        //            campaignByproductValueDiscount.Columns.AddRange(columns);

                        //            // Add a row with the default values
                        //            campaignByproductValueDiscount.Rows.Add();

                        //            var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByProductUnitRate", campaignByproductValueDiscount, conn, transaction);
                        //            if (resultt.Status.ToLower() != "success")
                        //            {
                        //                throw new Exception(result.ExMessage.ToString());
                        //            }


                        //        }


                        //        #endregion
                        //    }


                        //}
                        //#endregion

                        //#region  CampaignByProductTotalValue Discount
                        //if (CampaignByProductTotalValueExist)
                        //{
                        //    var CampaignDetailByproductTotalValue = await _repoCampaign.CampaignByProductTotalValueList(new[] { "D.CampaignId" }, new[] { CampaignByProductTotalValueId.ToString() }, null, conn, transaction);

                        //    if (CampaignDetailByproductTotalValue.Status == "Success" && CampaignDetailByproductTotalValue.DataVM is DataTable dt)
                        //    {
                        //        string json = JsonConvert.SerializeObject(dt);
                        //        var CampaignByProductTotalValuedetails = JsonConvert.DeserializeObject<List<CampaignDetailByProductTotalValueVM>>(json);

                        //        CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                        //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);

                        //        if (CampaignDetailByproductTotalValueVM == null)
                        //        {
                        //            CampaignDetailByproductTotalValueVM = CampaignByProductTotalValuedetails
                        //   .FirstOrDefault(record => record.CustomerId == 0 && record.ProductId == details.ProductId && record.FromAmount <= SumtotalAfterDisCount && record.ToAmount >= SumtotalAfterDisCount);
                        //        }

                        //        #region Calculation 

                        //        if (CampaignDetailByproductTotalValueVM != null)
                        //        {
                        //            details.CampaignHeaderId = CampaignDetailByproductTotalValueVM.CampaignId;
                        //            details.CampaignDetailsId = CampaignDetailByproductTotalValueVM.Id;
                        //            details.CampaignTypeId = CampaignDetailByproductTotalValueVM.CampaignTypeId;
                        //            LineDiscountGain = Convert.ToDecimal(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice * SumtotalAfterDisCount / 100);

                        //            InvoiceDiscount += -LineDiscountGain;

                        //            DataTable campaignByproductTotalValueDiscount = new DataTable("campaignByproductTotalValueDiscount");

                        //            // Define columns with their default values
                        //            var columns = new[]
                        //             {
                        //                    new DataColumn("SaleDetailId", typeof(int)) { DefaultValue = saleDelivery.Id },
                        //                    new DataColumn("CustomerId", typeof(int)) { DefaultValue = saleDelivery.CustomerId },
                        //                    new DataColumn("CampaignId", typeof(int)) { DefaultValue = CampaignByQuantitiesId },
                        //                    new DataColumn("CampaignTypeId", typeof(int)) { DefaultValue = 27 },
                        //                    new DataColumn("ProductId", typeof(int)) { DefaultValue = CampaignDetailByproductTotalValueVM.ProductId },
                        //                    new DataColumn("LineDiscountRate", typeof(int)) { DefaultValue = Convert.ToInt32(CampaignDetailByproductTotalValueVM.DiscountRateBasedOnTotalPrice)},
                        //                    new DataColumn("LineDiscountAmount", typeof(decimal)) { DefaultValue = Convert.ToDecimal(LineDiscountGain) }
                        //                };

                        //            // Add columns to DataTable
                        //            campaignByproductTotalValueDiscount.Columns.AddRange(columns);

                        //            // Add a row with the default values
                        //            campaignByproductTotalValueDiscount.Rows.Add();

                        //            var resultt = await _commonRepo.BulkInsert("SaleDeliveryCampaignByProductTotalPrice", campaignByproductTotalValueDiscount, conn, transaction);
                        //            if (resultt.Status.ToLower() != "success")
                        //            {
                        //                throw new Exception(result.ExMessage.ToString());
                        //            }


                        //        }


                        //        #endregion
                        //    }


                        //}
                        //#endregion



                        //#endregion

                        details.SaleDeliveryId = saleDelivery.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = saleDelivery.BranchId;
                        details.Line = LineNo;

                        //#region  CampaignDetailByInvoiceValues Discount
                        //CampaignDetailByInvoiceValueVM CampaignDetailByInvoiceValue = new CampaignDetailByInvoiceValueVM();

                        //if (CampaignByInvoiceValueExist)
                        //{
                        //    var CampaignByInvoiceValue = await _repoCampaign.CampaignDetailByInvoiceValueList(new[] { "D.CampaignId" }, new[] { CampaignByInvoiceValueId.ToString() }, null, conn, transaction);

                        //    if (CampaignByInvoiceValue.Status == "Success" && CampaignByInvoiceValue.DataVM is DataTable dt)
                        //    {
                        //        string json = JsonConvert.SerializeObject(dt);
                        //        var CampaignByInvoicedetails = JsonConvert.DeserializeObject<List<CampaignDetailByInvoiceValueVM>>(json);
                        //        CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                        //  .FirstOrDefault(record => record.CustomerId == saleDelivery.CustomerId);

                        //        if (CampaignDetailByInvoiceValue == null)
                        //        {
                        //            CampaignDetailByInvoiceValue = CampaignByInvoicedetails
                        //   .FirstOrDefault(record => record.CustomerId == 0);
                        //        }

                        //        #region Calculation 

                        //        if (CampaignDetailByInvoiceValue != null)
                        //        {
                        //            saleDelivery.InvoiceDiscountRate = CampaignByInvoicedetails.FirstOrDefault().DiscountRateBasedOnTotalPrice;

                        //            _repo.InvoiceUpdate(saleDelivery);

                        //        }


                        //        #endregion
                        //    }


                        //}
                        //#endregion

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
                            details.VATAmount = ((details.SubTotal + details.SDAmount) * details.VATRate) / 100;
                            //details.VATAmount = (details.SubTotal * details.VATRate) / 100;
                        }

                        details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

                        #endregion


                        var resultDetail = await _repo.InsertDetails(details, conn, transaction);
                        if (resultDetail.Status.ToLower() == "success")
                        {

                            LineNo++;
                            _orderRepo.UpdateSaleOrderIsComplete(details.SaleOrderId, conn, transaction);
                        }
                        else
                        {
                            throw new Exception(resultDetail.Message);
                        }
                    }
                    await _serviceSale.DetailInsert(saleVM);
                    var grndResult = await _repo.UpdateGrandTotal(saleDelivery, conn, transaction);

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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

        public async Task<ResultVM> FromSaleOrderGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.FromSaleOrderGridData(options, conditionalFields, conditionalValues, conn, transaction);

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

        public async Task<ResultVM> GetSDGridData(GridOptions options)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.GetSDGridData(options, conn, transaction);

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

        public async Task<ResultVM> SaleDeliveryList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.SaleDeliveryList(conditionalFields, conditionalValues, vm, conn, transaction);

                var lst = new List<SaleDeliveryReturnVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<SaleDeliveryReturnVM>>(data);

                var detailsDataList = await _repo.SaleDeliveryDetailsList(new[] { "D.SaleDeliveryId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleDeliveryReturnDetailVM>>(json);

                    lst.FirstOrDefault().saleDeliveryReturnDetailList = details;
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

        public SaleVM MapSaleDeliveryToSale(SaleDeliveryVM saleDelivery)
        {
            return new SaleVM
            {
                // Map the common fields between SaleDeliveryVM and SaleVM
                Code = saleDelivery.Code,
                BranchId = saleDelivery.BranchId ?? 0,
                CustomerId = saleDelivery.CustomerId ?? 0,
                SalePersonId = saleDelivery.SalePersonId ?? 0,
                RouteId = saleDelivery.RouteId ?? 0,
                DeliveryAddress = saleDelivery.DeliveryAddress,
                VehicleNo = saleDelivery.VehicleNo,
                VehicleType = saleDelivery.VehicleType,
                InvoiceDateTime = saleDelivery.InvoiceDateTime,
                DeliveryDate = saleDelivery.DeliveryDate,
                GrandTotalAmount = saleDelivery.GrandTotalAmount ?? 0,
                GrandTotalSDAmount = saleDelivery.GrandTotalSDAmount ?? 0,
                GrandTotalVATAmount = saleDelivery.GrandTotalVATAmount ?? 0,
                Comments = saleDelivery.Comments,
                IsPrint = false,
                PrintBy = "",
                PrintOn = "",
                TransactionType = "Sale",
                IsPost = false,
                PostBy = "",
                PostedOn = "",
                FiscalYear = "",
                PeriodId = "",
                CurrencyId = saleDelivery.CurrencyId ?? 0,
                CurrencyRateFromBDT = saleDelivery.CurrencyRateFromBDT ?? 1,
                CreatedBy = "",
                CreatedOn = DateTime.Now.ToString(),
                LastModifiedBy = "",
                LastModifiedOn = "",
                CreatedFrom = saleDelivery.CreatedFrom,
                LastUpdateFrom = "",
                saleDetailsList = saleDelivery.saleDeliveryDetailList.Select(detail => new SaleDetailVM
                {
                    // You can map SaleDeliveryDetailVM to SaleDetailVM here
                    Id = 0,
                    SaleDeliveryId = saleDelivery.Id,
                    SaleDeliveryDetailId = detail.Id,
                    SaleOrderId = detail.SaleOrderId,
                    SaleOrderDetailId = detail.SaleOrderDetailId,
                    BranchId = detail.BranchId,
                    Line = detail.Line ?? 0,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitRate = detail.UnitRate,
                    SubTotal = detail.SubTotal,
                    VATRate = detail.VATRate,
                    VATAmount = detail.VATAmount ?? 0,
                    LineTotal = detail.LineTotal ?? 0,
                    UOMId = detail.UOMId ?? 0,
                    UOMName = detail.UOMName,
                    UOMFromId = detail.UOMFromId ?? 0,
                    UOMFromName = detail.UOMFromName,
                    UOMConversion = detail.UOMConversion ?? 1,
                    Comments = detail.Comments,
                    VATType = "",
                    TransactionType = "Sale",
                    IsPost = false
                }).ToList()
            };
        }

        public async Task<ResultVM> GetSaleDeliveryDetailDataById(GridOptions options, int masterId)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.GetSaleDeliveryDetailDataById(options, masterId, conn, transaction);

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
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.ProductWiseSaleDelivery(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> IncoiceReportPreview(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.IncoiceReportPreview(conditionalFields, conditionalValues, vm, conn, transaction);

                var companyData = await new CompanyProfileRepository().List(new[] { "D.Id" }, new[] { vm.CompanyId }, null, conn, transaction);
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
        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Sale Delivery Invoice" };
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

        public async Task<ResultVM> GetDeliveryNoWiseGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.GetDeliveryNoWiseGridData(options, conditionalFields, conditionalValues, conn, transaction);

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
        public async Task<ResultVM> GetSaleDeleveryByCustomerAndBranch(int CustomerId, int branchId)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                // Create a new connection to the database
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                // Begin a new transaction
                transaction = conn.BeginTransaction();

                // Pass the parameters directly to the repository's method (adjust this according to your repo method signature)
                result = await _repo.ListGetSaleDeleveryByCustomerAndBranch(CustomerId, branchId, conn, transaction);

                // Commit the transaction if the connection is new
                if (isNewConnection)
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception ex)
            {
                // Rollback the transaction if an error occurs
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                return result;
            }
            finally
            {
                // Close the connection if it was opened
                if (isNewConnection && conn != null)
                {
                    conn.Close();
                }
            }
        }

        public async Task<ResultVM> GetSaleDueListByCustomerId(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleDeliveryRepository _repo = new SaleDeliveryRepository();
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

                result = await _repo.GetSaleDueListByCustomerId(options, conditionalFields, conditionalValues, conn, transaction);

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
