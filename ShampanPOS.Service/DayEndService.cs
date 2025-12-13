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
using System.Data;

namespace ShampanPOS.Service
{
    public class DayEndService
    {
        public async Task<ResultVM> ProcessDataInsert(DayEndHeadersVM vm)
        {
            CommonRepository _commonRepo = new CommonRepository();
            DayEndRepository _repo = new DayEndRepository();
            SaleService _serviceSale = new SaleService();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            ResultVM insertSaleResult = null;
            ResultVM PurcaseResult = null;

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                // Check if DayEndHeaders exists
                ResultVM dayEndData = _repo.DayEndList(vm);

                // Process Sales Data List
                ResultVM resultPs = await _repo.SaleProcessDataList(vm, conn, transaction);
                if (resultPs.Status == "Success" && resultPs.DataVM is List<DayEndHeadersVM> saleList && saleList.Any())
                {
                    // Always insert the master sale record
                    var masterSaleData = saleList.First();
                    if (!(dayEndData.DataVM is List<DayEndHeadersVM> existingDayEndDetails) || !existingDayEndDetails.Any())
                    {
                        insertSaleResult = await _repo.Insert(masterSaleData, conn, transaction);
                        if (insertSaleResult == null || string.IsNullOrEmpty(insertSaleResult.Id))
                        {
                            throw new Exception("Sale insert failed; insertSaleResult is null.");
                        }

                        int masterId = int.Parse(insertSaleResult.Id);
                        foreach (var saleData in saleList)
                        {
                            saleData.Id = masterId;
                            ResultVM detailResult = await _repo.InsertDayEndDetails(saleData, conn, transaction);
                            if (detailResult.Status != "Success")
                            {
                                throw new Exception($"Sale details insert/update error: {detailResult.Message}");
                            }
                        }
                    }
                    else
                    {
                        if (dayEndData.Status == "Success" && dayEndData.DataVM is List<DayEndHeadersVM> existingDayEndDetailsList && existingDayEndDetailsList.Any())
                        {
                            int masterId = existingDayEndDetailsList.First().Id;
                            if (masterId == 0)
                            {
                                throw new Exception("No valid Master ID found for existing DayEndDetails.");
                            }

                            foreach (var saleData in saleList)
                            {
                                saleData.Id = masterId;
                                ResultVM detailResult = await _repo.UpdateDayEndDetails(saleData, conn, transaction);
                                // Optionally check detailResult.Status here if needed
                            }
                        }
                    }
                }
                

                // Process Purchase Data List
                ResultVM resultPur = await _repo.PurchaseProcessDataList(vm, conn, transaction);
                if (resultPur.Status == "Success" && resultPur.DataVM is List<DayEndHeadersVM> purchaseList && purchaseList.Any())
                {
                    if (dayEndData.Status == "Success" && dayEndData.DataVM is List<DayEndHeadersVM> existingPurchaseListt && existingPurchaseListt.Any())
                    {
                        
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(insertSaleResult?.Id))
                        {
                            PurcaseResult = await _repo.Insert(vm, conn, transaction);
                            if (PurcaseResult == null || string.IsNullOrEmpty(PurcaseResult.Id))
                            {
                                throw new Exception("Dayend  insert failed; insertDayendResult is null.");
                            }
                        }
                    }
                        foreach (var purchaseData in purchaseList)
                        {
                            ResultVM purchaseResult;
                            if (dayEndData.Status == "Success" && dayEndData.DataVM is List<DayEndHeadersVM> existingPurchaseList && existingPurchaseList.Any())
                            {
                                purchaseData.Id = existingPurchaseList.First().Id;
                                purchaseResult = await _repo.UpdateDayEndDetails(purchaseData, conn, transaction);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(insertSaleResult?.Id))
                                {

                                    purchaseData.Id = int.Parse(insertSaleResult.Id);
                                    purchaseResult = await _repo.InsertDayEndDetails(purchaseData, conn, transaction);
                                    if (purchaseResult.Status != "Success")
                                    {
                                        throw new Exception($"Purchase details insert/update error: {purchaseResult.Message}");
                                    }
                                }
                                else
                                {

                                    purchaseData.Id = int.Parse(PurcaseResult.Id);
                                    purchaseResult = await _repo.InsertDayEndDetails(purchaseData, conn, transaction);
                                    if (purchaseResult.Status != "Success")
                                    {
                                        throw new Exception($"Purchase details insert/update error: {purchaseResult.Message}");
                                    }
                                }
                            }


                        }
                }

                // Process Sales Data List
                // Process Sales Data List using DataSet
                DataSet salesData = _repo.SalesDataList(vm);
                SaleRepository _saleRepo = new SaleRepository();

                // Check that the master table exists and has rows
                if (salesData != null && salesData.Tables.Count > 0 && salesData.Tables[0].Rows.Count > 0)
                {
                    // Delete existing sales data for the given date
                    ResultVM deleteResult = await _saleRepo.DeleteSaleData(vm.Date, conn, transaction);
                    if (deleteResult.Status != "Success")
                    {
                        throw new Exception($"Failed to delete existing sales data: {deleteResult.Message}");
                    }
                }

                // Bulk Insert if master data exists
                if (salesData != null && salesData.Tables.Count > 0 && salesData.Tables[0].Rows.Count > 0)
                {
                    // Get the master and detail tables from the DataSet
                    DataTable dtSaleMasterFromDS = salesData.Tables[0];
                    DataTable dtSaleDetailFromDS = salesData.Tables[1];

                    // Create new DataTables for bulk insert with the required schema
                    DataTable dtSales = new DataTable("Sales");
                    
                    dtSales.Columns.Add("Code", typeof(string));
                    dtSales.Columns.Add("BranchId", typeof(int));
                    dtSales.Columns.Add("CustomerId", typeof(int));
                    dtSales.Columns.Add("SalePersonId", typeof(int));
                    dtSales.Columns.Add("RouteId", typeof(int));
                    dtSales.Columns.Add("DeliveryAddress", typeof(string));
                    dtSales.Columns.Add("VehicleNo", typeof(string));
                    dtSales.Columns.Add("VehicleType", typeof(string));
                    dtSales.Columns.Add("InvoiceDateTime", typeof(DateTime));
                    dtSales.Columns.Add("DeliveryDate", typeof(DateTime));
                    dtSales.Columns.Add("GrandTotalAmount", typeof(decimal));
                    dtSales.Columns.Add("GrandTotalSDAmount", typeof(decimal));
                    dtSales.Columns.Add("GrandTotalVATAmount", typeof(decimal));
                    dtSales.Columns.Add("Comments", typeof(string));
                    dtSales.Columns.Add("IsPrint", typeof(bool));
                    dtSales.Columns.Add("TransactionType", typeof(string));
                    dtSales.Columns.Add("IsPost", typeof(bool));
                    dtSales.Columns.Add("FiscalYear", typeof(string));
                    dtSales.Columns.Add("PeriodId", typeof(string));
                    dtSales.Columns.Add("CurrencyId", typeof(int));
                    dtSales.Columns.Add("CurrencyRateFromBDT", typeof(decimal));
                    dtSales.Columns.Add("CreatedBy", typeof(string));
                    dtSales.Columns.Add("CreatedOn", typeof(DateTime));

                    DataTable dtSaleDetails = new DataTable("SaleDetails");
                    dtSaleDetails.Columns.Add("SaleId", typeof(string));
                    dtSaleDetails.Columns.Add("SaleDeliveryId", typeof(int));
                    dtSaleDetails.Columns.Add("SaleDeliveryDetailId", typeof(int)); // Nullable value
                    dtSaleDetails.Columns.Add("SaleOrderId", typeof(int));
                    dtSaleDetails.Columns.Add("SaleOrderDetailId", typeof(int));
                    dtSaleDetails.Columns.Add("BranchId", typeof(int));
                    dtSaleDetails.Columns.Add("Line", typeof(int));
                    dtSaleDetails.Columns.Add("ProductId", typeof(int));
                    dtSaleDetails.Columns.Add("Quantity", typeof(decimal));
                    dtSaleDetails.Columns.Add("UnitRate", typeof(decimal));
                    dtSaleDetails.Columns.Add("SubTotal", typeof(decimal));
                    dtSaleDetails.Columns.Add("SD", typeof(decimal));
                    dtSaleDetails.Columns.Add("SDAmount", typeof(decimal));
                    dtSaleDetails.Columns.Add("VATRate", typeof(decimal));
                    dtSaleDetails.Columns.Add("VATAmount", typeof(decimal));
                    dtSaleDetails.Columns.Add("LineTotal", typeof(decimal));
                    dtSaleDetails.Columns.Add("UOMId", typeof(int));
                    dtSaleDetails.Columns.Add("UOMFromId", typeof(int));
                    dtSaleDetails.Columns.Add("UOMconversion", typeof(decimal));
                    dtSaleDetails.Columns.Add("Comments", typeof(string));
                    dtSaleDetails.Columns.Add("VATType", typeof(string));
                    dtSaleDetails.Columns.Add("TransactionType", typeof(string));
                    dtSaleDetails.Columns.Add("IsPost", typeof(bool));
                    dtSaleDetails.Columns.Add("Code", typeof(string));

                    // Create a mapping dictionary to map original SaleDeliveryId (from DS) to a new generated SaleId
                    Dictionary<int, int> saleIdMapping = new Dictionary<int, int>();

                    // Process master data (dtSaleMasterFromDS)
                    foreach (DataRow row in dtSaleMasterFromDS.Rows)
                    {

                        DataRow masterRow = dtSales.NewRow();
                        masterRow["Code"] = row["Code"] ?? (object)DBNull.Value;
                        masterRow["BranchId"] = Convert.ToInt32(row["BranchId"]);
                        masterRow["CustomerId"] = Convert.ToInt32(row["CustomerId"]);
                        masterRow["SalePersonId"] = Convert.ToInt32(row["SalePersonId"]);
                        masterRow["RouteId"] = Convert.ToInt32(row["RouteId"]);
                        masterRow["DeliveryAddress"] = row["DeliveryAddress"] ?? (object)DBNull.Value;
                        masterRow["VehicleNo"] = row["VehicleNo"] ?? (object)DBNull.Value;
                        masterRow["VehicleType"] = row["VehicleType"] ?? (object)DBNull.Value;
                        masterRow["InvoiceDateTime"] = Convert.ToDateTime(row["InvoiceDateTime"]);
                        masterRow["DeliveryDate"] = Convert.ToDateTime(row["DeliveryDate"]);
                        masterRow["GrandTotalAmount"] = 0;
                        masterRow["GrandTotalSDAmount"] = 0;
                        masterRow["GrandTotalVATAmount"] = 0;
                        masterRow["Comments"] = row["Comments"] ?? (object)DBNull.Value;
                        masterRow["IsPrint"] = false;
                        masterRow["TransactionType"] = row["TransactionType"] ?? (object)DBNull.Value;
                        masterRow["IsPost"] = false;
                        masterRow["FiscalYear"] = row["FiscalYear"] ?? (object)DBNull.Value;
                        masterRow["PeriodId"] = row["PeriodId"] ?? (object)DBNull.Value;
                        masterRow["CurrencyId"] = Convert.ToInt32(row["CurrencyId"]);
                        masterRow["CurrencyRateFromBDT"] = Convert.ToDecimal(row["CurrencyRateFromBDT"]);
                        masterRow["CreatedBy"] = "ERP";
                        masterRow["CreatedOn"] = DateTime.Now;
                        dtSales.Rows.Add(masterRow);
                    }

                    // Process detail data (dtSaleDetailFromDS)
                    foreach (DataRow row in dtSaleDetailFromDS.Rows)
                    {
                        DataRow detailRow = dtSaleDetails.NewRow();

                        
                        // Set SaleId to the new master id from our mapping
                        detailRow["SaleId"] = row["Code"] ?? (object)DBNull.Value;
                        detailRow["SaleDeliveryId"] = row["SaleDeliveryId"];
                        detailRow["SaleDeliveryDetailId"] = row.Table.Columns.Contains("SaleDeliveryDetailId") ? row["SaleDeliveryDetailId"] : 0;
                        detailRow["SaleOrderId"] = row.Table.Columns.Contains("SaleOrderId") ? row["SaleOrderId"] : 0;
                        detailRow["SaleOrderDetailId"] = row.Table.Columns.Contains("SaleOrderDetailId") ? row["SaleOrderDetailId"] : 0;
                        detailRow["BranchId"] = Convert.ToInt32(row["BranchId"]);
                        detailRow["Line"] = Convert.ToInt32(row["Line"]);
                        detailRow["ProductId"] = Convert.ToInt32(row["ProductId"]);
                        detailRow["Quantity"] = Convert.ToDecimal(row["Quantity"]);
                        detailRow["UnitRate"] = Convert.ToDecimal(row["UnitRate"]);
                        detailRow["SubTotal"] = Convert.ToDecimal(row["SubTotal"]);
                        detailRow["SD"] = Convert.ToDecimal(row["SD"]);
                        detailRow["SDAmount"] = Convert.ToDecimal(row["SDAmount"]);
                        detailRow["VATRate"] = Convert.ToDecimal(row["VATRate"]);
                        detailRow["VATAmount"] = Convert.ToDecimal(row["VATAmount"]);
                        detailRow["LineTotal"] = Convert.ToDecimal(row["LineTotal"]);
                        detailRow["UOMId"] = Convert.ToInt32(row["UOMId"]);
                        detailRow["UOMFromId"] = Convert.ToInt32(row["UOMFromId"]);
                        detailRow["UOMconversion"] = Convert.ToDecimal(row["UOMconversion"]);
                        detailRow["Comments"] = row["Comments"] ?? (object)DBNull.Value;
                        detailRow["VATType"] = row["VatType"] ?? (object)DBNull.Value;
                        detailRow["TransactionType"] = row["TransactionType"] ?? (object)DBNull.Value;
                        detailRow["IsPost"] = false;
                        detailRow["Code"] = row["Code"] ?? (object)DBNull.Value;
                        dtSaleDetails.Rows.Add(detailRow);
                    }

                    // Bulk Insert using your repository's bulk insert methods
                    await _commonRepo.BulkInsert("Sales", dtSales, conn, transaction);
                    await _commonRepo.BulkInsert("SaleDetails", dtSaleDetails, conn, transaction);
                }


                // Commit the transaction if all operations succeed
                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Data processed successfully.";
                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }
                result.ExMessage = ex.ToString();
                result.Message = "An error occurred during processing.";
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
