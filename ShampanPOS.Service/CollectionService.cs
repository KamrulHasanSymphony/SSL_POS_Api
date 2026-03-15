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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class CollectionService
    {

        CommonRepository _commonRepo = new CommonRepository();
        public async Task<ResultVM> Insert(CollectionVM model)
        {
            string CodeGroup = "Collection";
            string CodeName = "Collection";

            CollectionRepository _repo = new CollectionRepository();
            _commonRepo = new CommonRepository();
            SaleCreditCardRepository _saleCreditCardRepo = new SaleCreditCardRepository();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            bool isNewConnection = false;
            bool isSuccess = false;

            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                await conn.OpenAsync();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

                if (string.IsNullOrEmpty(code))
                    throw new Exception("Code Generation Failed!");

                model.Code = code;

                result = await _repo.Insert(model, conn, transaction);
                model.Id = Convert.ToInt32(result.Id);

                if (!result.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    throw new Exception(result.Message);

                int lineNo = 1;

                foreach (var details in model.collectionDetailList)
                {
                    details.CollectionId = model.Id;
                    details.CustomerId = model.CustomerId;

                    decimal paid = details.PaidAmount ?? 0;
                    decimal due = details.DueAmount ?? 0;
                    decimal collection = details.CollectionAmount ?? 0;

                    if (collection > due)
                        throw new Exception("Collection amount cannot be greater than due.");

                    details.PaidAmount = paid;
                    details.DueAfter = due - collection;

                    var resultDetail = await _repo.InsertDetails(details, conn, transaction);

                    if (!resultDetail.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        throw new Exception(resultDetail.Message);

                    var saleUpdate = await _repo.UpdateSale(details, conn, transaction);

                    if (!saleUpdate.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        throw new Exception(saleUpdate.Message);

                    SaleCreditCardVM card = new SaleCreditCardVM
                    {
                        SaleId = details.SaleId,
                        CreditCardId = model.BankAccountId,
                        CardTotal = collection,
                        Remarks = "Collection Payment"
                    };

                    var cardInsert = await _saleCreditCardRepo.Insert(card, conn, transaction);

                    if (!cardInsert.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                        throw new Exception(cardInsert.Message);

                    lineNo++;
                }

                isSuccess = true;

                if (isNewConnection && isSuccess)
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

                result.Status = "Fail";
                result.Message = ex.Message;
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
        //public async Task<ResultVM> Insert(CollectionVM model)
        //{
        //    string CodeGroup = "Collection";
        //    string CodeName = "Collection";

        //    CollectionRepository _repo = new CollectionRepository();
        //    _commonRepo = new CommonRepository();

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


        //        string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

        //        if (!string.IsNullOrEmpty(code))
        //        {
        //            model.Code = code;

        //            result = await _repo.Insert(model, conn, transaction);
        //            model.Id = Convert.ToInt32(result.Id);

        //            if (result.Status.ToLower() == "success")
        //            {
        //                int LineNo = 1;
        //                foreach (var details in model.collectionDetailList)
        //                {
        //                    details.CollectionId = model.Id;
        //                    details.CustomerId = model.CustomerId;


        //                    decimal paid = details.PaidAmount ?? 0;
        //                    decimal due = details.DueAmount ?? 0;
        //                    decimal collection = details.CollectionAmount ?? 0;

        //                    details.PaidAmount = paid + collection;
        //                    details.DueAfter = due - collection;

        //                    var resultDetail = await _repo.InsertDetails(details, conn, transaction);

        //                    if (resultDetail.Status.ToLower() == "success")
        //                    {
        //                        var collectionPaid = await _repo.UpdateSale(details, conn, transaction);

        //                        LineNo++;
        //                    }
        //                    else
        //                    {
        //                        throw new Exception(resultDetail.Message);
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                throw new Exception(result.Message);
        //            }

        //            if (isNewConnection && result.Status == "Success")
        //            {
        //                transaction.Commit();
        //            }
        //            else
        //            {
        //                throw new Exception(result.Message);
        //            }

        //            return result;
        //        }
        //        else
        //        {
        //            throw new Exception("Code Generation Failed!");
        //        }
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

        public async Task<ResultVM> Update(CollectionVM model)
        {
            CollectionRepository _repo = new CollectionRepository();
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

            

                var record = _commonRepo.DetailsDelete("CollectionDetails", new[] { "CollectionId" }, new[] { model.Id.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                result = await _repo.Update(model, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    int LineNo = 1;
                    foreach (var details in model.collectionDetailList)
                    {
                        details.CollectionId = model.Id;
                        details.CustomerId = model.CustomerId;

                        // 🔥 Server Side Calculation
                        decimal paid = details.PaidAmount ?? 0;
                        decimal due = details.DueAmount ?? 0;
                        decimal collection = details.CollectionAmount ?? 0;

                        details.PaidAmount = paid + collection;
                        details.DueAfter = due - collection;

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
            CollectionRepository _repo = new CollectionRepository();
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

        //public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    CollectionRepository _repo = new CollectionRepository();
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

        //        result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

        //        var lst = new List<CollectionVM>();

        //        string data = JsonConvert.SerializeObject(result.DataVM);
        //        lst = JsonConvert.DeserializeObject<List<CollectionVM>>(data);

        //        var detailsDataList = await _repo.DetailsList(new[] { "D.BankAccountId" }, conditionalValues, vm, conn, transaction);

        //        if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
        //        {
        //            string json = JsonConvert.SerializeObject(dt);
        //            var details = JsonConvert.DeserializeObject<List<CollectionDetailVM>>(json);

        //            lst.FirstOrDefault().collectionDetailList = details;
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
            CollectionRepository _repo = new CollectionRepository();
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

                var lst = new List<CollectionVM>();

                string data = JsonConvert.SerializeObject(result.DataVM);
                lst = JsonConvert.DeserializeObject<List<CollectionVM>>(data);

                var detailsDataList = _repo.DetailsList(new[] { "D.CollectionId" }, conditionalValues, vm, conn, transaction);

                if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<CollectionDetailVM>>(json);

                    lst.FirstOrDefault().collectionDetailList = details;
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






        //public async Task<ResultVM> List(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    CollectionRepository _repo = new CollectionRepository();
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

        //        result = await _repo.List(conditionalFields, conditionalValues, vm, conn, transaction);

        //        if (isNewConnection)
        //        {
        //            transaction.Commit();
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




        public async Task<ResultVM> ListAsDataTable(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            CollectionRepository _repo = new CollectionRepository();
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
            CollectionRepository _repo = new CollectionRepository();
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
            CollectionRepository _repo = new CollectionRepository();
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
            CollectionRepository _repo = new CollectionRepository();
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

                if (result.Status == "Success" && !string.IsNullOrEmpty(companyName) && result.DataVM is GridEntity<CollectionVM> gridData)
                {
                    var items = gridData.Items;
                    //items.ToList().ForEach(item => item.CompanyName = companyName);
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
            CollectionRepository _repo = new CollectionRepository();
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
    }
}
