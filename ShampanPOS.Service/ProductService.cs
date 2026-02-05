using ShampanPOS.Repository;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ShampanPOS.ViewModel.Utility;
using ShampanPOS.ViewModel.KendoCommon;
using Newtonsoft.Json;

namespace ShampanPOS.Service
{
    public class ProductService
    {
        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(ProductVM product)
        {
            string CodeGroup = "Product";
            string CodeName = "Product";

            ProductRepository _repo = new ProductRepository();
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

                #region Check Exist Data
                string[] conditionField = { "Name" };
                string[] conditionValue = { product.Name.Trim() };

                bool exist = _commonRepo.CheckExists("Products", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion

                string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    product.Code = code;

                    result = await _repo.Insert(product, conn, transaction);

                    if (result.Status.ToLower() == "success")
                    {

                        ProductVM productData = (ProductVM)result.DataVM;
                        ProductUOMFactorsVM factors = new ProductUOMFactorsVM();
                        factors.Name = "Pcs";
                        factors.Packsize = "1x1";
                        factors.ConversationFactor = 1;
                        factors.ProductId = productData.Id;
                        factors.IsArchive = false;
                        factors.IsActive = true;
                        factors.CreatedBy = product.CreatedBy;
                        factors.CreatedOn = product.CreatedOn;
                        factors.CreatedFrom = product.CreatedFrom;

                        var PorductUomFactors = await _repo.InsertProductUOMFactorss(factors, conn, transaction);

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

        //public async Task<ResultVM> InsertFromMasterItem(ProductVM product)
        //{
        //    ProductRepository _repo = new ProductRepository();
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

        //        ProductGroupService productGroupService = new ProductGroupService();
        //        ProductGroupVM pgvm = new ProductGroupVM();

        //        pgvm.Name = product.MasterItemGroupName;
        //        pgvm.IsActive = product.IsActive;
        //        pgvm.IsArchive = product.IsArchive;
        //        pgvm.CreatedBy = product.CreatedBy;
        //        pgvm.CreatedOn = product.CreatedOn;

        //        var group = await productGroupService.Insert(pgvm);
        //        if (group.Status.ToLower() == "success")
        //        {

        //            ProductGroupVM productGroupVM = (ProductGroupVM)group.DataVM;

        //            ProductVM pvm = new ProductVM();
        //            var details = pvm.MasterItemList;

        //            pvm.Code = product.Code;
        //            if (product.MasterItemList != null && product.MasterItemList.Any())
        //            {
        //                pvm.Name = product.MasterItemList.First().Name;
        //            }
        //            pvm.ProductGroupId = productGroupVM.Id;
        //            pvm.IsActive = product.IsActive;
        //            pvm.IsArchive = product.IsArchive;
        //            pvm.CreatedBy = product.CreatedBy;
        //            pvm.CreatedOn = product.CreatedOn;


        //            if (string.IsNullOrWhiteSpace(pvm.Code))
        //            {
        //                pvm.Code = _commonRepo.CodeGenerationNo(
        //                    "Product",
        //                    "Product",
        //                    conn,
        //                    transaction
        //                );
        //            }

        //            result = await _repo.Insert(pvm, conn, transaction);
        //        }
        //        else {
        //            var name = product.MasterItemGroupName;

        //            var retusls = productGroupService.grouplist(new[] { "M.Name" }, new[] { name }, null);

        //            ProductVM pvm = new ProductVM();


        //            pvm.Code = product.Code;
        //            if (product.MasterItemList != null && product.MasterItemList.Any())
        //            {
        //                pvm.Name = product.MasterItemList.First().Name;
        //            }
        //            pvm.ProductGroupId = retusls.Id;
        //            pvm.IsActive = product.IsActive;
        //            pvm.IsArchive = product.IsArchive;
        //            pvm.CreatedBy = product.CreatedBy;
        //            pvm.CreatedOn = product.CreatedOn;


        //            if (string.IsNullOrWhiteSpace(pvm.Code))
        //            {
        //                pvm.Code = _commonRepo.CodeGenerationNo(
        //                    "Product",
        //                    "Product",
        //                    conn,
        //                    transaction
        //                );
        //            }

        //            result = await _repo.Insert(pvm, conn, transaction);
        //        }

        //        if (result.Status.ToLower() == "success")
        //        {

        //            ProductVM productData = (ProductVM)result.DataVM;
        //            ProductUOMFactorsVM factors = new ProductUOMFactorsVM();
        //            factors.Name = "Pcs";
        //            factors.Packsize = "1x1";
        //            factors.ConversationFactor = 1;
        //            factors.ProductId = productData.Id;
        //            factors.IsArchive = false;
        //            factors.IsActive = true;
        //            factors.CreatedBy = product.CreatedBy;
        //            factors.CreatedOn = product.CreatedOn;
        //            factors.CreatedFrom = product.CreatedFrom;

        //            var PorductUomFactors = await _repo.InsertProductUOMFactorss(factors, conn, transaction);

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


        public async Task<ResultVM> Update(ProductVM product)
        {
            ProductRepository _repo = new ProductRepository();
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

                #region Check Exist Data
                string[] conditionField = { "Id not", "Name" };
                string[] conditionValue = { product.Id.ToString(), product.Name.Trim() };

                bool exist = _commonRepo.CheckExists("Products", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion

                result = await _repo.Update(product, conn, transaction);

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

        public async Task<ResultVM> Delete(CommonVM vm)
        {
            ProductRepository _repo = new ProductRepository();
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

                result = await _repo.Delete(vm, conn, transaction);

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
            ProductRepository _repo = new ProductRepository();
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
            ProductRepository _repo = new ProductRepository();
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
            ProductRepository _repo = new ProductRepository();
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


        //public async Task<ResultVM> GetProductModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    ProductRepository _repo = new ProductRepository();
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

        //        result = await _repo.GetProductModalData(conditionalFields, conditionalValues, vm, conn, transaction);

        //        if (result.Status.ToLower() == "success")
        //        {
        //            var countResult = await _repo.GetProductModalCountData(conditionalFields, conditionalValues, vm, conn, transaction);

        //            if (countResult.Status.ToLower() == "success")
        //            {
        //                result.Count = countResult.Count;
        //            }
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

        //public async Task<ResultVM> GetUOMFromNameData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    ProductRepository _repo = new ProductRepository();
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

        //        result = await _repo.GetUOMFromNameData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        //public async Task<ResultVM> GetProductGroupModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        //{
        //    ProductRepository _repo = new ProductRepository();
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

        //        result = await _repo.GetProductGroupModalData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> GetGridData(GridOptions options)
        {
            ProductRepository _repo = new ProductRepository();
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

                result = await _repo.GetGridData(options, conn, transaction);

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
            ProductRepository _repo = new ProductRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Product" };
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



        public async Task<ResultVM> InsertFromMasterItem(ProductVM product)
        {
            ProductRepository _repo = new ProductRepository();
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

                ProductGroupService productGroupService = new ProductGroupService();

                if (product.MasterItemList == null || !product.MasterItemList.Any())
                    throw new Exception("No supplier found to save.");

                // Group suppliers by their group name
                var groupedItems = product.MasterItemList
                    .Where(x => !string.IsNullOrWhiteSpace(x.MasterItemGroupName))
                    .GroupBy(x => x.MasterItemGroupName.Trim())
                    .ToList();


                foreach (var group in groupedItems)
                {
                    string groupName = group.Key;
                    if (string.IsNullOrWhiteSpace(groupName))
                        throw new Exception("Supplier group name is required.");

                    // Check if the group exists
                    var groupResult = await productGroupService.grouplist(
                        new[] { "M.Name" }, new[] { groupName }, null);

                    ProductGroupVM productGroupVM = groupResult?.Status == "Success" && groupResult.DataVM is List<ProductGroupVM> list && list.Any() ? list.First() : null;

                    // Create group if it doesn't exist
                    if (productGroupVM == null)
                    {
                        ProductGroupVM newGroup = new ProductGroupVM
                        {
                            Name = groupName,
                            Description = product.Description,
                            IsActive = product.IsActive,
                            IsArchive = product.IsArchive,
                            CreatedBy = product.CreatedBy,
                            CreatedOn = product.CreatedOn,
                            CompanyId = product.CompanyId,
                            UserId = product.UserId
                        };

                        var insertResult = await productGroupService.Insert(newGroup);
                        if (insertResult.Status != "Success")
                        {
                            //throw new Exception(insertResult.Message);

                            var name = product.MasterItemGroupName;

                            var retusls = productGroupService.grouplist(new[] { "M.Name" }, new[] { name }, null);

                            foreach (var item in group)
                            {
                                string supplierName = item.Name.Trim();

                                if (_repo.Exists(supplierName, conn, transaction))
                                    continue;

                                ProductVM pvm = new ProductVM
                                {
                                    CompanyId = product.CompanyId,
                                    UserId = product.UserId,
                                    Name = supplierName,
                                    Code = string.IsNullOrWhiteSpace(item.Code) ? _commonRepo.CodeGenerationNo("Supplier", "Supplier", conn, transaction) : item.Code,
                                    ProductGroupId = retusls.Id,
                                    BanglaName = item.BanglaName,
                                    UOMId = item.UOMId,
                                    HSCodeNo = item.HSCodeNo,
                                    VATRate = item.VATRate,
                                    SDRate = item.SDRate,
                                    IsActive = product.IsActive,
                                    IsArchive = product.IsArchive,
                                    CreatedBy = product.CreatedBy,
                                    CreatedOn = product.CreatedOn
                                };

                                result = await _repo.Insert(pvm, conn, transaction);

                                if (result.Status != "Success")
                                    throw new Exception(result.Message);
                            }

                        }
                        else
                        {
                            productGroupVM = (ProductGroupVM)insertResult.DataVM;
                            // Insert suppliers under this group
                            foreach (var item in group)
                            {
                                string supplierName = item.Name.Trim();

                                if (_repo.Exists(supplierName, conn, transaction))
                                    continue;

                                ProductVM pvm = new ProductVM
                                {
                                    CompanyId = product.CompanyId,
                                    UserId = product.UserId,

                                    Name = supplierName,
                                    Code = string.IsNullOrWhiteSpace(item.Code)
                                        ? _commonRepo.CodeGenerationNo("Supplier", "Supplier", conn, transaction)
                                        : item.Code,
                                    ProductGroupId = productGroupVM.Id,
                                    BanglaName = item.BanglaName,
                                    UOMId = item.UOMId,
                                    HSCodeNo = item.HSCodeNo,
                                    VATRate = item.VATRate,
                                    SDRate = item.SDRate,
                                    IsActive = product.IsActive,
                                    IsArchive = product.IsArchive,
                                    CreatedBy = product.CreatedBy,
                                    CreatedOn = product.CreatedOn
                                };

                                result = await _repo.Insert(pvm, conn, transaction);

                                if (result.Status != "Success")
                                    throw new Exception(result.Message);
                            }
                        }


                    }


                }

                transaction.Commit();

                return new ResultVM
                {
                    Status = "Success",
                    Message = "Suppliers saved successfully."
                };
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                return new ResultVM
                {
                    Status = "Fail",
                    Message = ex.Message,
                    ExMessage = ex.ToString()
                };
            }
            finally
            {
                if (isNewConnection)
                    conn?.Close();
            }
        }

 

    }

}
