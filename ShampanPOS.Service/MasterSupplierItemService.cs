using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.KendoCommon;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class MasterSupplierItemService
    {

        CommonRepository _commonRepo = new CommonRepository();

        public async Task<ResultVM> Insert(MasterSupplierItemVM mastersupplieritem)
        {
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();

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

                foreach (var item in mastersupplieritem.MasterItemList)
                {
                    #region Check Exist Data (Supplier + Product)
                    string[] conditionField ={"MasterSupplierId","MasterProductId","IsActive"};

                    string[] conditionValue ={item.MasterSupplierId.ToString(),item.ProductId.ToString(),"1"};

                    bool exist = _commonRepo.CheckExists("MasterSupplierItem",conditionField,conditionValue,conn,transaction);
                    #endregion

                    if (exist)
                    {
                        skippedCount++;
                        continue;
                    }

                    await _repo.Insert(item, conn, transaction, mastersupplieritem);
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

                if (isNewConnection && result.Status == "Success")
                {
                    transaction.Commit();
                }
                else
                {
                    throw new Exception(result.Message);
                }

                // 🟢 Partial / Full success
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
                if (transaction != null)
                    transaction.Rollback();

                return new ResultVM
                {
                    Status = "Error",
                    Message = "Something went wrong",
                    ExMessage = ex.ToString()
                };
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }


        //public async Task<ResultVM> Insert(MasterSupplierItemVM mastersupplieritem)
        //{
        //    MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();

        //    ResultVM result = new ResultVM{Status = "Fail",Message = "Error",ExMessage = null,Id = "0",DataVM = null};

        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        await conn.OpenAsync();

        //        transaction = conn.BeginTransaction();

        //        var detailsList = mastersupplieritem.MasterItemList;

        //        result = await _repo.Insert(detailsList, conn, transaction, mastersupplieritem);

        //        transaction.Commit();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null) transaction.Rollback();

        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (conn != null) conn.Close();
        //    }
        //}



        public async Task<ResultVM> Update(MasterSupplierItemVM mastersupplieritem)
        {
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();
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

                //result = await _repo.Update(mastersupplieritem, conn, transaction);


                var record = _commonRepo.DetailsDelete("MasterSupplierItem", new[] { "MasterSupplierId" }, new[] { mastersupplieritem.MasterSupplierId.ToString() }, conn, transaction);

                if (record.Status == "Fail")
                {
                    throw new Exception("Error in Delete for Details Data.");
                }

                // 🔥 STEP 2: Insert New Details
                foreach (var item in mastersupplieritem.MasterItemList)
                {
                    #region Check Exist Data (Supplier + Product)
                    string[] conditionField ={"MasterSupplierId","MasterProductId","IsActive"};

                    string[] conditionValue ={item.MasterSupplierId.ToString(),item.ProductId.ToString(),"1"};

                    bool exist = _commonRepo.CheckExists("MasterSupplierItem",conditionField,conditionValue,conn,transaction);
                    #endregion

                    if (exist)
                    {
                        skippedCount++;
                        continue;
                    }
                    //item.SupplierId = mastersupplieritem.MasterSupplierId;

                    await _repo.Insert(item, conn, transaction, mastersupplieritem);
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
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();
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
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();
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
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();
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
            MasterSupplierItemRepository _repo = new MasterSupplierItemRepository();
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
    }
}
