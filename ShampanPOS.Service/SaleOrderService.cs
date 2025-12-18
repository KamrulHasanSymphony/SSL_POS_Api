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
            ProductRepository _repoProduct = new ProductRepository();
            PeramModel paramModel = new PeramModel();

            CommonVM commonVM = new CommonVM();

            commonVM.Group = "StockControl";
            commonVM.Name = "StockControl";

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
                if (saleOrder.CustomerId == 0 || saleOrder.CustomerId == null)
                {
                    throw new Exception("Customer Is Required!");
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

                

                //#region Currency Data

                //string CurrencyFromId = "";
                //var currencyData = await new BranchProfileRepository().List(new[] { "H.Id" }, new[] { saleOrder.BranchId.ToString() }, null, conn, transaction);

                //if (currencyData.Status == "Success" && currencyData.DataVM is List<BranchProfileVM> branch)
                //{
                //    CurrencyFromId = branch.FirstOrDefault().CurrencyId.ToString();
                //}
                //else
                //{
                //    throw new Exception("Currency data not found!");
                //}

                //#endregion


                //#region CustomerData
                //var customerData = _repoCustomer.List(new[] { "M.Id" }, new[] { saleOrder.CustomerId.ToString() }, null, conn, transaction);
                //string jsonString = JsonConvert.SerializeObject(customerData.Result.DataVM);
                //List<CustomerVM> customer = JsonConvert.DeserializeObject<List<CustomerVM>>(jsonString);

                ////CustomerVM vm = JsonConvert.DeserializeObject<List<CustomerVM>>(customerData.Result.DataVM.ToString()).FirstOrDefault();

                ////List<CustomerVM> customer = IdentityExtensions.DeserializeJson<List<CustomerVM>>(customerData.Result.DataVM.ToString());

                //saleOrder.RouteId = customer.FirstOrDefault().RouteId;
                //#endregion

                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, saleOrder.OrderDate, saleOrder.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    saleOrder.Code = code;

                    result = await _repo.Insert(saleOrder, conn, transaction);
                    saleOrder.Id = Convert.ToInt32(result.Id);

                    if (result.Status.ToLower() == "success")
                    {



                        int LineNo = 1;

                        
                        decimal subtotal = 0;
                        decimal DiscountGain = 0;
                        decimal SumtotalAfterDisCount = 0;
                        decimal VATAmountAfterDisCount = 0;
                        decimal LineDiscountGain = 0;
                        decimal InvoiceDiscount = 0;
                        decimal totalInvoiceValue = 0;

                        foreach (var details in saleOrder.saleOrderDetailsList)
                        {
                       
                            details.SaleOrderId = saleOrder.Id;
                            details.SDAmount = 0;
                            details.VATAmount = 0;
                            details.BranchId = saleOrder.BranchId;
                            details.Line = LineNo;

            
        


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
                            }
                            else
                            {
                                result.Message = resultDetail.Message;
                                throw new Exception(result.Message);
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

        public async Task<ResultVM> Update(SaleOrderVM saleOrder)
        {
            SaleOrderRepository _repo = new SaleOrderRepository();
            CustomerRepository _repoCustomer = new CustomerRepository();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            PeramModel paramModel = new PeramModel();
            CommonVM commonVM = new CommonVM();

            commonVM.Group = "StockControl";
            commonVM.Name = "StockControl";

            bool isNewConnection = false;
           
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

                var record = _commonRepo.DetailsDelete("SaleOrderDetails", new[] { "SaleOrderId" }, new[] { saleOrder.Id.ToString() }, conn, transaction);


                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }


                result = await _repo.Update(saleOrder, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    
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

                        if (Deleterecord.Status == "Fail")
                        {
                            throw new Exception("Error in Delete for Details Data.");
                        }


                        details.SaleOrderId = saleOrder.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = saleOrder.BranchId;
                        details.Line = LineNo;


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
        //public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    SaleOrderRepository _repo = new SaleOrderRepository();
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

        //        result = await _repo.SaleOrderList(conditionalFields, conditionalValues, vm, conn, transaction);

        //        var lst = new List<SaleDeliveryVM>();

        //        string data = JsonConvert.SerializeObject(result.DataVM);
        //        lst = JsonConvert.DeserializeObject<List<SaleDeliveryVM>>(data);

        //        var detailsDataList = await _repo.SaleOrderDetailsList(new[] { "D.SaleOrderId" }, conditionalValues, vm, conn, transaction);

        //        if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
        //        {
        //            string json = JsonConvert.SerializeObject(dt);
        //            var details = JsonConvert.DeserializeObject<List<SaleDeliveryDetailVM>>(json);

        //            lst.FirstOrDefault().saleDeliveryDetailList = details;
        //            result.DataVM = lst;
        //        }

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

        //public async Task<ResultVM> SummaryReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    SaleOrderRepository _repo = new SaleOrderRepository();
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

        //        result = await _repo.ProductWiseSaleOrder(conditionalFields, conditionalValues, vm, conn, transaction);

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
