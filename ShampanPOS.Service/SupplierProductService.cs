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
    public class SupplierProductService
    {
        CommonRepository _commonRepo = new CommonRepository();

        //public async Task<ResultVM> Insert(SupplierProductVM supplierproduct)
        //{
        //    SupplierProductRepository _repo = new SupplierProductRepository();

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

        //        int insertedCount = 0;
        //        int skippedCount = 0;

        //        foreach (var item in supplierproduct.MasterItemList)
        //        {
        //            #region Check Exist Data (Supplier + Product)
        //            string[] conditionField =
        //            {
        //                "SupplierId",
        //                "ProductId",
        //                "IsActive"
        //            };

        //            string[] conditionValue =
        //            {
        //                supplierproduct.SupplierId.ToString(),
        //                item.Id.ToString(),
        //                "1"
        //            };

        //            bool exist = _commonRepo.CheckExists(
        //                "SupplierProduct",
        //                conditionField,
        //                conditionValue,
        //                conn,
        //                transaction
        //            );
        //            #endregion

        //            if (exist)
        //            {
        //                skippedCount++;
        //                continue;
        //            }
        //            item.SupplierId = supplierproduct.SupplierId;
        //            await _repo.Insert(item, conn, transaction, supplierproduct);
        //            insertedCount++;
        //        }

        //        if (insertedCount == 0 && skippedCount > 0)
        //        {
        //            transaction.Rollback();
        //            return new ResultVM
        //            {
        //                Status = "Fail",
        //                Message = "All selected products already exist for this supplier."
        //            };
        //        }

        //        //if (isNewConnection && result.Status == "Success")
        //        //{
        //            transaction.Commit();
        //        //}
        //        //else
        //        //{
        //        //    throw new Exception(result.Message);
        //        //}

        //        // 🟢 Partial / Full success
        //        return new ResultVM
        //        {
        //            Status = "Success",
        //            Message = $"{insertedCount} added, {skippedCount} skipped.",
        //            DataVM = new
        //            {
        //                Inserted = insertedCount,
        //                Skipped = skippedCount
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null)
        //            transaction.Rollback();

        //        return new ResultVM
        //        {
        //            Status = "Error",
        //            Message = "Something went wrong",
        //            ExMessage = ex.ToString()
        //        };
        //    }
        //    finally
        //    {
        //        if (conn != null)
        //            conn.Close();
        //    }
        //}

        public async Task<ResultVM> Insert(SupplierProductVM supplierproduct)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();

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

                var record = _commonRepo.DetailsDelete("SupplierProduct", new[] { "SupplierId" }, new[] { supplierproduct.SupplierId.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                foreach (var item in supplierproduct.MasterItemList)
                {
                    item.SupplierId = supplierproduct.SupplierId;
                    var insertResult = await _repo.Insert(item, conn, transaction, supplierproduct);

                    if (insertResult.Status == "Fail")
                    {
                        throw new Exception(insertResult.Message);
                    }
                }


                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Save Successfully";
                result.DataVM = supplierproduct;

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                result.Message = "Something went wrong";
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

        public async Task<ResultVM> IntegrationInsert(SupplierProductVM supplierproduct)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();
            SupplierGroupRepository _supplierGroupRepo = new SupplierGroupRepository();
            SupplierRepository _supplierRepo = new SupplierRepository();
            ProductGroupRepository _productGroupRepo = new ProductGroupRepository();
            ProductRepository _productRepo = new ProductRepository();

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

                if (supplierproduct.MasterItemList != null && supplierproduct.MasterItemList.Any())
                {
                    var firstItem = supplierproduct.MasterItemList.First();

                    #region Supplier

                    bool exist = _commonRepo.CheckExists(
                    "SupplierGroups",
                        new[] { "Code" },
                        new[] { firstItem.MasterSupplierGroupCode },
                        conn,
                        transaction
                    );

                    if (!exist)
                    {
                        SupplierGroupVM supplierGroup = new SupplierGroupVM
                        {
                            Code = firstItem.MasterSupplierGroupCode,
                            Name = firstItem.MasterSupplierGroupName,
                            UserId = supplierproduct.CreatedBy,
                            Description = "-",
                            Comments = "-",
                            CompanyId = supplierproduct.CompanyId,
                            IsArchive = false,
                            CreatedBy = supplierproduct.CreatedBy
                        };

                        var insertResult = await _supplierGroupRepo.Insert(supplierGroup, conn, transaction);

                        supplierproduct.MasterSupplierGroupId = Convert.ToInt32(insertResult.Id);
                    }
                    else
                    {
                        var listResult = await _supplierGroupRepo.List(
                            new[] { "Code" },
                            new[] { firstItem.MasterSupplierGroupCode },
                            null,
                            conn,
                            transaction
                        );

                        var list = listResult.DataVM as List<SupplierGroupVM>;

                        if (list != null && list.Any())
                        {
                            supplierproduct.MasterSupplierGroupId = list.First().Id;
                        }
                    }

                    bool supplierexist = _commonRepo.CheckExists(
                            "Suppliers",
                            new[] { "Code" },
                            new[] { firstItem.SupplierCode },
                            conn,
                            transaction
                        );
                    if (!supplierexist)
                    {
                        SupplierVM supplierVM = new SupplierVM
                        {
                            Code = firstItem.SupplierCode,
                            Name = firstItem.SupplierName,
                            UserId = supplierproduct.CreatedBy,
                            SupplierGroupId = supplierproduct.MasterSupplierGroupId,
                            Description = "-",
                            Comments = "-",
                            CompanyId = supplierproduct.CompanyId,
                            IsArchive = false,
                            CreatedBy = supplierproduct.CreatedBy
                        };

                        result = await _supplierRepo.Insert(supplierVM, conn, transaction);
                        supplierproduct.SupplierId = Convert.ToInt32(result.Id);
                    }
                    else
                    {
                        var listResult = await _supplierRepo.List(
                            new[] { "M.Code" },
                            new[] { firstItem.SupplierCode },
                            null,
                            conn,
                            transaction
                        );

                        var slist = listResult.DataVM as List<SupplierVM>;

                        if (slist != null && slist.Any())
                        {
                            supplierproduct.SupplierId = slist.First().Id;
                        }
                    }

                    #endregion

                    #region Product

                    foreach (var item in supplierproduct.MasterItemList)
                    {
                        // ---------- Product Group Check ----------
                        bool pgexist = _commonRepo.CheckExists(
                            "ProductGroups",
                            new[] { "Code" },
                            new[] { item.MasterItemGroupCode },
                            conn,
                            transaction
                        );

                        int productGroupId = 0;

                        if (!pgexist)
                        {
                            ProductGroupVM productGroup = new ProductGroupVM
                            {
                                Code = item.MasterItemGroupCode,
                                Name = item.MasterItemGroupName,
                                UserId = supplierproduct.CreatedBy,
                                Description = "-",
                                Comments = "-",
                                CompanyId = supplierproduct.CompanyId,
                                IsArchive = false,
                                CreatedBy = supplierproduct.CreatedBy,
                                CreatedFrom = supplierproduct.CreatedFrom
                            };

                            var pginsertResult = await _productGroupRepo.Insert(productGroup, conn, transaction);
                            productGroupId = Convert.ToInt32(pginsertResult.Id);
                        }
                        else
                        {
                            var pglistResult = await _productGroupRepo.List(
                                new[] { "M.Code" },
                                new[] { item.MasterItemGroupCode },
                                null,
                                conn,
                                transaction
                            );

                            var pglist = pglistResult.DataVM as List<ProductGroupVM>;

                            if (pglist != null && pglist.Any())
                            {
                                productGroupId = pglist.First().Id;
                            }
                        }

                        // ---------- Product Check ----------
                        bool productexist = _commonRepo.CheckExists(
                            "Products",
                            new[] { "Code" },
                            new[] { item.MasterItemCode },
                            conn,
                            transaction
                        );

                        if (!productexist)
                        {
                            ProductVM productVM = new ProductVM
                            {
                                Code = item.MasterItemCode,
                                Name = item.MasterItemName,
                                UserId = supplierproduct.CreatedBy,
                                ProductGroupId = productGroupId,
                                Description = "-",
                                BanglaName = "-",
                                CompanyId = supplierproduct.CompanyId,
                                IsArchive = false,
                                CreatedBy = supplierproduct.CreatedBy,
                                CreatedFrom = supplierproduct.CreatedFrom,
                                UOMId = supplierproduct.UOMId,
                                HSCodeNo = item.HSCodeNo,
                                VATRate = 0,
                                SDRate = 0,
                                PurchasePrice = 0,
                                SalePrice = 0,
                                ImagePath = ""
                            };

                            var insertResult = await _productRepo.Insert(productVM, conn, transaction);
                            item.Id = Convert.ToInt32(insertResult.Id);
                        }
                        else
                        {
                            var plistResult = await _productRepo.List(
                                new[] { "M.Code" },
                                new[] { item.MasterItemCode },
                                null,
                                conn,
                                transaction
                            );

                            var plist = plistResult.DataVM as List<ProductVM>;

                            if (plist != null && plist.Any())
                            {
                                item.Id = plist.First().Id;
                            }
                        }
                    }

                    #endregion
                    #region Supplier Product

                    foreach (var item in supplierproduct.MasterItemList)
                    {
                        item.SupplierId = supplierproduct.SupplierId;
                        var insertResult = await _repo.InsertOrUpdateSupplierProduct(item, conn, transaction, supplierproduct);

                        if (insertResult.Status == "Fail")
                        {
                            throw new Exception(insertResult.Message);
                        }
                    }
                    #endregion

                }


                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Save Successfully";
                result.DataVM = supplierproduct;

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                result.Message = "Something went wrong";
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


        public async Task<ResultVM> Update(SupplierProductVM supplierproduct)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();
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

                int insertedCount = 0;
                int skippedCount = 0;

                //#region Check Exist Data
                //string[] conditionField = { "Id not", "TelephoneNo", "IsActive" };
                //string[] conditionValue = { supplierproduct.Id.ToString(), supplierproduct.TelephoneNo.Trim(), "1" };

                //bool exist = _commonRepo.CheckExists("BankInformations", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion

                //result = await _repo.Update(supplierproduct, conn, transaction);

                var record = _commonRepo.DetailsDelete("SupplierProduct", new[] { "SupplierId" }, new[] { supplierproduct.SupplierId.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                // 🔥 STEP 2: Insert New Details
                foreach (var item in supplierproduct.MasterItemList)
                {
                    #region Check Exist Data (Supplier + Product)
                    string[] conditionField =
                    {
                 "SupplierId",
                 "ProductId",
                 "IsActive"
             };

                    string[] conditionValue =
                    {
                 supplierproduct.SupplierId.ToString(),
                 item.Id.ToString(),
                 "1"
             };

                    bool exist = _commonRepo.CheckExists(
                        "SupplierProduct",
                        conditionField,
                        conditionValue,
                        conn,
                        transaction
                    );
                    #endregion

                    if (exist)
                    {
                        skippedCount++;
                        continue;
                    }
                    //item.SupplierId = mastersupplieritem.MasterSupplierId;

                    await _repo.Insert(item, conn, transaction, supplierproduct);
                    insertedCount++;
                }
                if (insertedCount == 0 && skippedCount > 0)
                {
                    transaction.Rollback();
                    return new ResultVM
                    {
                        Status = "Fail",
                        Message = "All selected products already exist for this supplier."
                    };
                }

                transaction.Commit();

                return new ResultVM
                {
                    Status = "Success",
                    Message = $"{insertedCount} added, {skippedCount} skipped.",
                    DataVM = new
                    {
                        Inserted = insertedCount,
                        Skipped = skippedCount
                    }
                };
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

        public async Task<ResultVM> Delete(CommonVM vm)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();
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
            SupplierProductRepository _repo = new SupplierProductRepository();
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


        public async Task<ResultVM> Dropdown()
        {
            SupplierProductRepository _repo = new SupplierProductRepository();
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


        public async Task<ResultVM> GetGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();
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
                    //transaction.Rollback();
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


        public async Task<ResultVM> InsertSupplierProduct(SupplierProductVM supplierproduct)
        {
            SupplierProductRepository _repo = new SupplierProductRepository();

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

                if (supplierproduct.MasterItemList == null ||!supplierproduct.MasterItemList.Any())
                {
                    result.Status = "Fail";
                    result.Message = "No products selected.";
                    return result;
                }

                var record = _commonRepo.DetailsDelete("SupplierProduct", new[] { "SupplierId" }, new[] { supplierproduct.SupplierId.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                foreach (var item in supplierproduct.MasterItemList)
                {
                    item.SupplierId = supplierproduct.SupplierId;
                    var insertResult = await _repo.Insert(item, conn, transaction, supplierproduct);

                    if (insertResult.Status == "Fail")
                    {
                        throw new Exception(insertResult.Message);
                    }
                }


                if (isNewConnection)
                {
                    transaction.Commit();
                }

                result.Status = "Success";
                result.Message = "Save Successfully";
                result.DataVM = supplierproduct;

                return result;
            }
            catch (Exception ex)
            {
                if (transaction != null && isNewConnection)
                {
                    transaction.Rollback();
                }

                result.ExMessage = ex.ToString();
                result.Message = "Something went wrong";
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
