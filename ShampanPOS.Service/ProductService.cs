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


        public async Task<ResultVM> GetProductModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
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

                result = await _repo.GetProductModalData(conditionalFields, conditionalValues, vm, conn, transaction);

                if (result.Status.ToLower() == "success")
                {
                    var countResult = await _repo.GetProductModalCountData(conditionalFields, conditionalValues, vm, conn, transaction);

                    if (countResult.Status.ToLower() == "success")
                    {
                        result.Count = countResult.Count;
                    }
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

        public async Task<ResultVM> GetUOMFromNameData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
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

                result = await _repo.GetUOMFromNameData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        public async Task<ResultVM> GetProductGroupModalData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
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

                result = await _repo.GetProductGroupModalData(conditionalFields, conditionalValues, vm, conn, transaction);

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

        //public async Task<ResultVM> ExportProductExcel(CommonVM vm)
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

        //        result = await _repo.ExportProductExcel(vm, conn, transaction);

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

        //public async Task<ResultVM> ExportProductPursaseExcel(CommonVM vm)
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

        //        result = await _repo.ExportProductPursaseExcel(vm, conn, transaction);

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
        public async Task<ResultVM> ImportExcelFileInsert(ProductVM model)
        {

            SupplierRepository _repoSupplier = new SupplierRepository();
            BranchProfileRepository _repoBranch = new BranchProfileRepository();
            ProductRepository _repoProduct = new ProductRepository();
            UOMRepository _repoUOM = new UOMRepository();
            CommonService _commonService = new CommonService();
            PurchaseRepository _repo = new PurchaseRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };
            DataTable TempProductData = new DataTable("TempProductData");
            PeramModel vm = new PeramModel();
            vm.OrderName = "P.Id";
            vm.startRec = 0;
            vm.pageSize = 10;
            ProductVM product = new ProductVM();


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

                var record = _commonRepo.DetailsDelete("TempProductData", new[] { "" }, new[] { "" }, conn, transaction);

                if (record.Status == "Success")
                {
                    var columns = new[]
                    {
                                 new DataColumn("ProductName", typeof(string)),
                                 new DataColumn("ProductCode", typeof(string)),
                                 new DataColumn("EffectDate", typeof(DateTime)),
                                 new DataColumn("GroupName", typeof(string)),
                                 new DataColumn("SalesPrice", typeof(decimal)),
                                 new DataColumn("CostPrice", typeof(decimal)),
                                 new DataColumn("BranchId", typeof(int))

                    };

                    TempProductData.Columns.AddRange(columns);

                    foreach (var item in model.ProductImportList)
                    {
                        TempProductData.Rows.Add(
                            item.ProductName,
                            item.ProductCode,
                            item.EffectDate,
                            item.GroupName,
                            item.SalesPrice,
                            item.CostPrice,
                            item.BranchId

                        );
                    }
                }

                #endregion


                #region BulKInsert

                var resultt = await _commonRepo.BulkInsert("TempProductData", TempProductData, conn, transaction);

                if (resultt.Status.ToLower() != "success")
                {
                    return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
                }

                if (resultt.Status.ToLower() == "success" && isNewConnection)
                {
                    transaction.Commit();
                }

                #endregion


                #region Get TempProduct Data

                transaction = conn.BeginTransaction();


                var tempProductList = await _repoProduct.TempProductList(new[] { "" }, new[] { "" }, vm, conn, transaction);

                if (tempProductList.Status == "Success" && tempProductList.DataVM is DataTable dt)
                {

                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<ProductImportVM>>(json);
                    product.ProductImportList = details;
                    result.DataVM = product;

                }

                #endregion

                #region  Check Code 

                bool productCodeExist = true;

                foreach (var item in product.ProductImportList)
                {
                    string[] conditionField = { "Code" };
                    string[] conditionValue = { item.ProductCode.Trim() };

                    productCodeExist = _commonRepo.CheckExists("Products", conditionField, conditionValue, conn, transaction);

                    if (!productCodeExist)
                    {
                        return new ResultVM { Status = "Fail", Message = $"This Product Code {item.ProductCode.Trim()} does not exist", DataVM = null };
                    }
                }

                #endregion


                #region Set ProductId

                foreach (var item in product.ProductImportList)
                {
                    result = await _repoProduct.List(new[] { "M.Code" }, new[] { item.ProductCode }, vm, conn, transaction);
                    var ProductCodeString = JsonConvert.SerializeObject(result.DataVM);
                    var ProductList = JsonConvert.DeserializeObject<List<PurchaseDetailVM>>(ProductCodeString);
                    var ProductId = ProductList.FirstOrDefault().Id;
                    item.ProductId = ProductId;
                }

                #endregion


                //Add

                //ProductPriceGroupVM pp = new ProductPriceGroupVM();


                //End


                #region BulKInsert

                if (model.BranchProfileList != null && model.BranchProfileList.Count > 0)
                {
                    foreach (var branchprofile in model.BranchProfileList)
                    {



                        if (product.ProductImportList != null && product.ProductImportList.Count > 0)
                        {

                            DataTable detailsTable = new DataTable();
                            detailsTable.Columns.Add("ProductId", typeof(int));
                            detailsTable.Columns.Add("BranchId", typeof(int));
                            detailsTable.Columns.Add("BatchNo", typeof(string));
                            detailsTable.Columns.Add("PriceCategory", typeof(string));

                            detailsTable.Columns.Add("EntryDate", typeof(string));
                            detailsTable.Columns.Add("EffectDate", typeof(string));
                            detailsTable.Columns.Add("MFGDate", typeof(string));
                            detailsTable.Columns.Add("EXPDate", typeof(string));

                            detailsTable.Columns.Add("SD", typeof(decimal));
                            detailsTable.Columns.Add("SDAmount", typeof(decimal));
                            detailsTable.Columns.Add("VATRate", typeof(decimal));
                            detailsTable.Columns.Add("VATAmount", typeof(decimal));
                            detailsTable.Columns.Add("CostPrice", typeof(decimal));
                            detailsTable.Columns.Add("SalesPrice", typeof(decimal));
                            detailsTable.Columns.Add("PurchasePrice", typeof(decimal));
                            detailsTable.Columns.Add("CreatedBy", typeof(string));

                            detailsTable.Columns.Add("CreatedOn", typeof(string));

                            detailsTable.Columns.Add("LastModifiedBy", typeof(string));

                            detailsTable.Columns.Add("LastModifiedOn", typeof(string));

                            detailsTable.Columns.Add("CreatedFrom", typeof(string));
                            detailsTable.Columns.Add("LastUpdateFrom", typeof(string));

                            foreach (var itemData in product.ProductImportList)
                            {
                                detailsTable.Rows.Add(

                                    itemData.ProductId,
                                    //itemData.BranchId,
                                    branchprofile.Id,
                                    DBNull.Value,
                                    itemData.GroupName,

                                    DBNull.Value,
                                    itemData.EffectDate,
                                    DBNull.Value,
                                    DBNull.Value,

                                    0,
                                    0,
                                    itemData.VATRate,
                                    0,
                                    itemData.CostPrice,
                                    itemData.SalesPrice,
                                    0,
                                    model.CreatedBy,

                                    DateTime.Now,

                                    DBNull.Value,

                                    DBNull.Value,

                                    DBNull.Value,
                                    DBNull.Value

                                    );
                            }


                            var resultBulk = await _commonRepo.BulkInsert("ProductSalePriceBatchHistories", detailsTable, conn, transaction);

                            if (resultBulk.Status.ToLower() != "success")
                            {
                                return new ResultVM { Status = "Fail", Message = "Excel template is not correct", DataVM = null };
                            }

                            //if (resultBulk.Status.ToLower() == "success" && isNewConnection)
                            //{
                            //    transaction.Commit();
                            //}

                        }





                    }
                }


                if (isNewConnection)
                {
                    transaction.Commit();
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


        //public async Task<ResultVM> ExportProductStockExcel(CommonVM vm)
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

        //        result = await _repo.ExportProductStockExcel(vm, conn, transaction);

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

        //public async Task<ResultVM> PurchaseImportExcelFileInsert(ProductPriceGroupVM productPriceGroup)
        //{
        //    string CodeGroup = "ProductPriceGroup";
        //    string CodeName = "ProductPriceGroup";

        //    CommonRepository _commonRepo = new CommonRepository();
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


        //        //#region Check Exist Data

        //        //string[] conditionField = { "Name" };
        //        //string[] conditionValue = { productPriceGroup.Name.ToString() };
        //        //bool exist = _commonRepo.CheckExists("ProductPriceGroups", conditionField, conditionValue, conn, transaction);
        //        //if (exist)
        //        //{
        //        //    result.Message = "Data Already Exist!";
        //        //    throw new Exception("Data Already Exist!");
        //        //}

        //        //#endregion


        //        result = await _repo.PurchaseImportExcelFileInsert(productPriceGroup, conn, transaction);


        //        if (result.Status == "Success" && isNewConnection)
        //        {
        //            transaction.Commit();
        //        }
        //        else
        //        {
        //            transaction.Rollback();
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
        //        //result.Message = "Error in inserting fiscal year.";
        //        result.Message = result.Message;
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

    }

}
