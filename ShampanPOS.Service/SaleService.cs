using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System.Data;
using System.Data.SqlClient;

namespace ShampanPOS.Service
{
    public class SaleService
    {
        CommonRepository _commonRepo = new CommonRepository();


        public async Task<ResultVM> Insert(SaleVM sale)
        {
            string CodeGroup = "Sale";
            string CodeName = "Sale";
            SaleRepository _repo = new SaleRepository();
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


                string code = _commonRepo.GenerateCode(CodeGroup, CodeName, sale.InvoiceDateTime, sale.BranchId, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    sale.Code = code;
                    result = await _repo.Insert(sale, conn, transaction);
                }

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    decimal subtotal = 0;

                    foreach (var details in sale.saleDetailsList)
                    {

                        details.SaleId = sale.Id;
                        details.SaleOrderId = sale.SaleOrderId;
                        //details.SaleOrderDetailId = sale.SaleOrderDetailId;
                        //details.ProductId = sale.ProductId;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = sale.BranchId;
                        details.Line = LineNo;
                        details.CompanyId = sale.CompanyId;


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


                        if (details.SaleOrderDetailId.HasValue)
                        {
                            decimal remainQty = details.RemainQty ?? 0;
                            decimal sellQty = details.Quantity ?? 0;

                            if (sellQty > remainQty)
                            {
                                result.Status = "Fail";
                                result.Message =
                                    $"Posting quantity ({sellQty}) cannot be greater than remaining quantity ({remainQty}).";

                                if (transaction != null)
                                    transaction.Rollback();

                                return result;
                            }
                            else
                            {

                                var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                                if (resultDetail.Status.ToLower() == "success")
                                {
                                    if (details.SaleOrderDetailId.HasValue && details.ProductId.HasValue)
                                    {
                                        var updateResult = await _repo.UpdateSaleOrderDetails(
                                            details.SaleOrderDetailId.Value,
                                            details.ProductId.Value,
                                            conn,
                                            transaction
                                        );

                                        if (updateResult.Status.ToLower() != "success")
                                        {
                                            throw new Exception(updateResult.Message);
                                        }
                                    }

                                    LineNo++;
                                }
                                else
                                {
                                    result.Message = resultDetail.Message;
                                    throw new Exception(result.Message);
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




        //public async Task<ResultVM> Insert(SaleVM sale)
        //{
        //    string CodeGroup = "Sale";
        //    string CodeName = "Sale";

        //    SaleRepository _repo = new SaleRepository();
        //    _commonRepo = new CommonRepository();

        //    ResultVM result = new ResultVM
        //    {
        //        Status = "Fail",
        //        Message = "Error",
        //        ExMessage = null,
        //        Id = "0",
        //        DataVM = null
        //    };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        conn.Open();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        string code = _commonRepo.GenerateCode(
        //            CodeGroup,
        //            CodeName,
        //            sale.InvoiceDateTime,
        //            sale.BranchId,
        //            conn,
        //            transaction
        //        );

        //        if (!string.IsNullOrEmpty(code))
        //        {
        //            sale.Code = code;
        //            result = await _repo.Insert(sale, conn, transaction);
        //        }

        //        if (result.Status.ToLower() == "success")
        //        {
        //            int LineNo = 1;

        //            foreach (var details in sale.saleDetailsList)
        //            {
        //                details.SaleId = sale.Id;
        //                details.SaleOrderId = sale.SaleOrderId;
        //                details.SDAmount = 0;
        //                details.VATAmount = 0;
        //                details.BranchId = sale.BranchId;
        //                details.Line = LineNo;
        //                details.CompanyId = sale.CompanyId;

        //                #region Line Total Summation
        //                if (details.SD > 0)
        //                    details.SDAmount = (details.SubTotal * details.SD) / 100;

        //                if (details.VATRate > 0)
        //                    details.VATAmount =
        //                        ((details.SubTotal + details.SDAmount) * details.VATRate) / 100;

        //                details.LineTotal =
        //                    details.SubTotal + details.SDAmount + details.VATAmount;
        //                #endregion

        //                #region RemainQty Check
        //                if (details.SaleOrderDetailId.HasValue)
        //                {
        //                    decimal remainQty = details.RemainQty ?? 0;
        //                    decimal postQty = details.Quantity ?? 0;

        //                    if (postQty > remainQty)
        //                    {
        //                        result.Status = "Fail";
        //                        result.Message =
        //                            $"Posting quantity ({postQty}) cannot be greater than remaining quantity ({remainQty}).";

        //                        if (transaction != null)
        //                            transaction.Rollback();

        //                        return result;
        //                    }
        //                }
        //                #endregion

        //                var resultDetail = await _repo.InsertDetails(details, conn, transaction);

        //                if (resultDetail.Status.ToLower() == "success")
        //                {
        //                    // ✅ ONLY CHANGE: UpdateQuantity called here
        //                    if (details.SaleOrderDetailId.HasValue && details.ProductId.HasValue)
        //                    {
        //                        var updateResult = await _repo.UpdateQuantity(
        //                            details.SaleOrderDetailId,
        //                            details.ProductId,
        //                            conn,
        //                            transaction
        //                        );

        //                        if (updateResult.Status.ToLower() != "success")
        //                            throw new Exception(updateResult.Message);
        //                    }

        //                    LineNo++;
        //                }
        //                else
        //                {
        //                    result.Message = resultDetail.Message;
        //                    throw new Exception(result.Message);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception(result.Message);
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
        //            transaction.Rollback();

        //        result.Message = ex.ToString();
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //            conn.Close();
        //    }
        //}






        public async Task<ResultVM> Update(SaleVM sale)
        {
            SaleRepository _repo = new SaleRepository();
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

                //var record = _commonRepo.DetailsDelete("SaleDetails", new[] { "SaleId" }, new[] { sale.Id.ToString() }, conn, transaction);

                //if (record.Status == "Fail")
                //{
                //    throw new Exception("Error in Delete for Details Data.");
                //}

                result = await _repo.Update(sale, conn, transaction);


                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;

                    decimal subtotal = 0;



                    foreach (var details in sale.saleDetailsList)
                    {
                        details.SaleId = sale.Id;
                        details.SDAmount = 0;
                        details.VATAmount = 0;
                        details.BranchId = sale.BranchId;
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

                        #region RemainQty Check

                        // If Sale Order exists, then check remain qty
                        if (details.SaleOrderDetailId.HasValue)
                        {
                            var rrr = await _repo.CheckRemaingQuantity( details.SaleOrderDetailId, details.ProductId, details.Id, conn, transaction
                            );

                            decimal CompleteQty = 0;
                            if (rrr.Status.ToLower() == "success" && rrr.DataVM != null)
                            {
                                CompleteQty = Convert.ToDecimal(rrr.DataVM);
                            }

                            decimal orderQty = details.OrderQuantity ?? 0;
                            decimal sellQty = details.Quantity ?? 0;
                            decimal remainQty = orderQty - CompleteQty;

                            if (sellQty > remainQty)
                            {
                                result.Status = "Fail";
                                result.Message =
                                    $"Posting quantity ({sellQty}) cannot be greater than remaining quantity ({remainQty}).";

                                if (transaction != null)
                                    transaction.Rollback();

                                return result;
                            }
                            else
                            {

                                var record = _commonRepo.DetailsDelete("SaleDetails", new[] { "SaleId" }, new[] { sale.Id.ToString() }, conn, transaction);

                                if (record.Status == "Fail")
                                {
                                    throw new Exception("Error in Delete for Details Data.");
                                }
                                else
                                {

                                    var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                                    if (resultDetail.Status.ToLower() == "success")
                                    {
                                        if (details.SaleOrderDetailId.HasValue && details.ProductId.HasValue)
                                        {
                                            var updateResult = await _repo.UpdateSaleOrderDetails(
                                                details.SaleOrderDetailId.Value,
                                                details.ProductId.Value,
                                                conn,
                                                transaction
                                            );

                                            if (updateResult.Status.ToLower() != "success")
                                            {
                                                throw new Exception(updateResult.Message);
                                            }
                                        }

                                        LineNo++;
                                    }
                                    else
                                    {
                                        result.Message = resultDetail.Message;
                                        throw new Exception(result.Message);
                                    }
                                }
                            }
                        }

                        #endregion

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

        //public async Task<ResultVM> Update(SaleVM sale)
        //{
        //    SaleRepository _repo = new SaleRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
        //    _commonRepo = new CommonRepository();
        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;
        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        conn.Open();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        //var record = _commonRepo.DetailsDelete("SaleDetails", new[] { "SaleId" }, new[] { sale.Id.ToString() }, conn, transaction);

        //        //if (record.Status == "Fail")
        //        //{
        //        //    throw new Exception("Error in Delete for Details Data.");
        //        //}

        //        result = await _repo.Update(sale, conn, transaction);


        //        if (result.Status.ToLower() == "success")
        //        {
        //            int LineNo = 1;

        //            decimal subtotal = 0;



        //            foreach (var details in sale.saleDetailsList)
        //            {
        //                details.SaleId = sale.Id;
        //                details.SDAmount = 0;
        //                details.VATAmount = 0;
        //                details.BranchId = sale.BranchId;
        //                details.Line = LineNo;


        //                #region Line Total Summation
        //                if (details.SD > 0)
        //                {
        //                    details.SDAmount = (details.SubTotal * details.SD) / 100;
        //                }
        //                if (details.VATRate > 0)
        //                {
        //                    details.VATAmount = ((details.SubTotal + details.SDAmount) * details.VATRate) / 100;
        //                    // details.VATAmount = (details.SubTotal * details.VATRate) / 100;
        //                }

        //                details.LineTotal = details.SubTotal + details.SDAmount + details.VATAmount;

        //                #endregion

        //                #region RemainQty Check

        //                // If Sale Order exists, then check remain qty
        //                if (details.SaleOrderDetailId.HasValue)
        //                {
        //                    var rrr = await _repo.CheckRemaingQuantity(
        //                        details.SaleOrderDetailId,
        //                        details.ProductId,
        //                        details.Id,
        //                        conn,
        //                        transaction
        //                    );

        //                    decimal CompleteQty = 0;
        //                    if (rrr.Status.ToLower() == "success" && rrr.DataVM != null)
        //                    {
        //                        CompleteQty = Convert.ToDecimal(rrr.DataVM);
        //                    }

        //                    decimal orderQty = details.OrderQuantity ?? 0;
        //                    decimal sellQty = details.Quantity ?? 0;
        //                    decimal remainQty = orderQty - CompleteQty;

        //                    if (sellQty > remainQty)
        //                    {
        //                        //result.Status = "Fail";
        //                        //result.Message =
        //                        //    $"Posting quantity ({sellQty}) cannot be greater than remaining quantity ({remainQty}).";

        //                        //if (transaction != null)
        //                        //    transaction.Rollback();

        //                        //return result;

        //                        throw new Exception($"Posting quantity ({sellQty}) cannot be greater than remaining quantity ({remainQty}).");
        //                    }
        //                }

        //                #endregion


        //                var resultDetail = await _repo.InsertDetails(details, conn, transaction);

        //                if (resultDetail.Status.ToLower() == "success")
        //                {
        //                    // 🔹 Update Sale Order Details (RemainQty / PostedQty)
        //                    if (details.SaleOrderDetailId.HasValue && details.ProductId.HasValue)
        //                    {
        //                        var updateResult = await _repo.UpdateSaleOrderDetails(
        //                            details.SaleOrderDetailId.Value,
        //                            details.ProductId.Value,
        //                            conn,
        //                            transaction
        //                        );

        //                        if (updateResult.Status.ToLower() != "success")
        //                        {
        //                            throw new Exception(updateResult.Message);
        //                        }
        //                    }

        //                    LineNo++;
        //                }
        //                else
        //                {
        //                    throw new Exception(resultDetail.Message);
        //                }
        //            }

        //        }
        //        else
        //        {
        //            throw new Exception(result.Message);
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
        //        result.Status = "Fail";
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


        public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SaleRepository _repo = new SaleRepository();
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
            SaleRepository _repo = new SaleRepository();
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
            SaleRepository _repo = new SaleRepository();
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
            SaleRepository _repo = new SaleRepository();
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


        //public async Task<ResultVM> MultiplePost(CommonVM vm)
        //{
        //    SaleRepository _repo = new SaleRepository();
        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, IDs = vm.IDs, DataVM = null };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;
        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        conn.Open();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        foreach (var item in vm.IDs)
        //        {
        //            int saleId = int.Parse(item);

        //            result = _repo.SaleOrderQtyCheck(saleId, conn, transaction);

        //            if (result.Status == "Success" && result.DataVM is DataTable dt)
        //            {
        //                string json = JsonConvert.SerializeObject(dt);
        //                List<SaleDetailVM> details = JsonConvert.DeserializeObject<List<SaleDetailVM>>(json);

        //                foreach (var saleDetail in details)
        //                {
        //                    int? salesorderId = saleDetail.SaleOrderId;
        //                    int? productId = saleDetail.ProductId;
        //                    string? productName = saleDetail.ProductName;

        //                    // Validation 1: Quantity > RemainQty
        //                    if ((saleDetail.Quantity ?? 0) > (saleDetail.RemainQty ?? 0))
        //                    {
        //                        result.Status = "Fail";
        //                        result.Message = $"Posting quantity cannot be greater than remaining quantity for {productName}.";
        //                        result.ExMessage = result.Message;


        //                        if (transaction != null)
        //                        {
        //                            transaction.Rollback();
        //                        }

        //                        return result;
        //                    }


        //                    decimal cqty = (saleDetail.CompletedQty ?? 0) + (saleDetail.Quantity ?? 0);
        //                    decimal remainQty = (saleDetail.OrderQuantity ?? 0) - cqty;

        //                    // Validation 2: Negative remain qty
        //                    if (remainQty < 0)
        //                    {
        //                        result.Status = "Fail";
        //                        result.Message = "Remain Quantity cannot be negative.";
        //                        result.ExMessage = result.Message;

        //                        // 🔥 VERY IMPORTANT
        //                        if (transaction != null)
        //                        {
        //                            transaction.Rollback();
        //                        }

        //                        return result;
        //                    }


        //                    SaleOrderDetailVM detail = new SaleOrderDetailVM
        //                    {
        //                        CompletedQty = cqty,
        //                        RemainQty = remainQty,
        //                        ProductId = productId,
        //                        SaleOrderId = salesorderId
        //                    };

        //                    result = await _repo.UpdateSaleOrderDetails(detail, conn, transaction);

        //                    if (result.Status != "Success")
        //                    {
        //                        result.Status = "Fail";
        //                        return result;
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                throw new Exception(result.Message);
        //            }


        //        }


        //        result = await _repo.MultiplePost(vm, conn, transaction);

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



        //public async Task<ResultVM> MultiplePost(CommonVM vm)
        //{
        //    SaleRepository _repo = new SaleRepository();
        //    ResultVM result = new ResultVM
        //    {
        //        Status = "Fail",
        //        Message = "Error",
        //        ExMessage = null,
        //        IDs = vm.IDs,
        //        DataVM = null
        //    };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        ------------------------------------------------
        //         ✅ SIMPLE CONDITION ONLY(NO QUERY)
        //         ------------------------------------------------
        //         Assumption:
        //        vm.Quantity = Running Quantity
        //         vm.CompletedQty = Completed Quantity
        //         vm.OrderQty = Order Quantity

        //        if ((vm.Quantity + vm.CompletedQty) >= vm.OrderQty)
        //        {
        //            result.Status = "Fail";
        //            result.Message = "Quantity + CompletedQty cannot be equal or greater than OrderQty.";
        //            return result;
        //        }

        //        Optional calculation(if you want to keep it in VM)
        //        vm.RemainQty = vm.OrderQty - vm.CompletedQty;

        //        ------------------------------------------------
        //        DB WORK
        //        ------------------------------------------------
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        conn.Open();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        result = await _repo.MultiplePost(vm, conn, transaction);

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

        //        result.Status = "Fail";
        //        result.ExMessage = ex.Message;
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



        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SaleRepository _repo = new SaleRepository();
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
            SaleRepository _repo = new SaleRepository();
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

        //public async Task<ResultVM> SummaryReport(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    SaleRepository _repo = new SaleRepository();
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

        //        result = await _repo.ProductWiseSale(conditionalFields, conditionalValues, vm, conn, transaction);

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
            SaleRepository _repo = new SaleRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Sale Invoice" };
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


        public async Task<ResultVM> GetSaleDetailDataById(GridOptions options, int masterId)
        {
            SaleRepository _repo = new SaleRepository();
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

                result = await _repo.GetSaleDetailDataById(options, masterId, conn, transaction);

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

        public async Task<ResultVM> SaleList(string?[] IDs)
        {
            SaleRepository _repo = new SaleRepository();
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

                result = await _repo.SaleList(IDs, conn, transaction);

                var lst = new List<SaleReturnVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<SaleReturnVM>>(data);

                //bool allSame = lst.Select(p => p.CustomerId).Distinct().Count() == 1;
                //if (!allSame)
                //{
                //    throw new Exception("Customer is not distinct!");
                //}


                var detailsDataList = await _repo.SaleDetailsList(IDs, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<SaleReturnDetailVM>>(json);
                    details.ToList().ForEach(item => item.SaleReturnCode = lst.FirstOrDefault().Code);
                    lst.FirstOrDefault().saleReturnDetailList = details;
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



        //public async Task<ResultVM> SaleListForPayment(string?[] IDs)
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

        //        result = await _repo.SaleListForPayment(IDs, conn, transaction);

        //        var lst = new List<CollectionVM>();

        //        string data = JsonConvert.SerializeObject(result.DataVM);
        //        lst = JsonConvert.DeserializeObject<List<CollectionVM>>(data);

        //        bool allSame = lst.Select(p => p.CustomerId).Distinct().Count() == 1;
        //        if (!allSame)
        //        {
        //            throw new Exception("Supplier is not distinct!");
        //        }

        //        var detailsDataList = await _repo.SaleDetailsListForPayment(IDs, conn, transaction);

        //        if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
        //        {
        //            string json = JsonConvert.SerializeObject(dt);
        //            var details = JsonConvert.DeserializeObject<List<CollectionDetailVM>>(json);

        //            // Check if lst is not null and contains items
        //            if (lst != null && lst.Any())
        //            {
        //                lst.FirstOrDefault().collectionDetailList = details;
        //                result.DataVM = lst;
        //            }
        //            else
        //            {
        //                // Handle the case where lst is null or empty
        //                // You can log or set default values here
        //                result.Status = "Fail";
        //                result.Message = "lst is null or empty.";
        //            }
        //        }
        //        else
        //        {
        //            // Handle failure in detailsDataList.Status or invalid DataVM
        //            result.Status = "Fail";
        //            result.Message = "Failed to retrieve purchase details.";
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
        //        result.Status = "Fail";
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


        public async Task<ResultVM> SaleListForPayment(string?[] IDs)
        {
            SaleRepository _repo = new SaleRepository();
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

                result = await _repo.SaleListForPayment(IDs, conn, transaction);

                var lst = new List<CollectionVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<CollectionVM>>(data);

                bool allSame = lst.Select(p => p.CustomerId).Distinct().Count() == 1;
                if (!allSame)
                {
                    throw new Exception("Supplier is not distinct!");
                }

                var detailsDataList = await _repo.SaleDetailsListForPayment(IDs, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<CollectionDetailVM>>(json);

                    // Check if lst is not null and contains items
                    if (lst != null && lst.Any())
                    {
                        lst.FirstOrDefault().collectionDetailList = details;
                        result.DataVM = lst;
                    }
                    else
                    {
                        // Handle the case where lst is null or empty
                        // You can log or set default values here
                        result.Status = "Fail";
                        result.Message = "lst is null or empty.";
                    }
                }
                else
                {
                    // Handle failure in detailsDataList.Status or invalid DataVM
                    result.Status = "Fail";
                    result.Message = "Failed to retrieve purchase details.";
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



        public async Task<ResultVM> FromSaleGridData(GridOptions options)
        {
            SaleRepository _repo = new SaleRepository();
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

                result = await _repo.FromSaleGridData(options, conn, transaction);

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
