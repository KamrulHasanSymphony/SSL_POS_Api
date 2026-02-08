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
    public class SupplierService
    {
        CommonRepository _commonRepo = new CommonRepository();
        public async Task<ResultVM> Insert(SupplierVM supplier)

        {
            string CodeGroup = "Supplier";
            string CodeName = "Supplier";
            CommonRepository _commonRepo = new CommonRepository();
            SupplierRepository _repo = new SupplierRepository();

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
                string[] conditionValue = { supplier.Name.Trim() };

                bool exist = _commonRepo.CheckExists("Suppliers", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion

                string code = _commonRepo.CodeGenerationNo(CodeGroup, CodeName, conn, transaction);

                if (!string.IsNullOrEmpty(code))
                {
                    supplier.Code = code;

                    result = await _repo.Insert(supplier, conn, transaction);

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

        public async Task<ResultVM> Update(SupplierVM supplier)
        {
            SupplierRepository _repo = new SupplierRepository();
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
                string[] conditionValue = { supplier.Id.ToString(), supplier.Name.Trim() };

                bool exist = _commonRepo.CheckExists("Suppliers", conditionField, conditionValue, conn, transaction);

                if (exist)
                {
                    result.Message = "Data Already Exist!";
                    throw new Exception("Data Already Exist!");
                }
                #endregion

                result = await _repo.Update(supplier, conn, transaction);

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

        public async Task<ResultVM> Delete(CommonVM vm)
        {
            SupplierRepository _repo = new SupplierRepository();
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
            SupplierRepository _repo = new SupplierRepository();
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
            SupplierRepository _repo = new SupplierRepository();
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

              result = await _repo.ListAsDataTable(conditionalFields, conditionalValues, vm,conn, transaction);

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

        public async Task<ResultVM> Dropdown()
        {
            SupplierRepository _repo = new SupplierRepository();
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
            SupplierRepository _repo = new SupplierRepository();
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

        public async Task<ResultVM> ReportPreview(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SupplierRepository _repo = new SupplierRepository();
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
                        var ReportType = new DataColumn("ReportType") { DefaultValue = "Supplier" };
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


        //public async Task<ResultVM> InsertFromMasterSupplier(SupplierVM supplier)
        //{
        //    SupplierRepository _repo = new SupplierRepository();
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

        //        SupplierGroupService supplierGroupService = new SupplierGroupService();
        //        SupplierGroupVM pgvm = new SupplierGroupVM();

        //        pgvm.Name = supplier.MasterSupplierGroupName;
        //        pgvm.IsActive = supplier.IsActive;
        //        pgvm.IsArchive = supplier.IsArchive;
        //        pgvm.CreatedBy = supplier.CreatedBy;
        //        pgvm.CreatedOn = supplier.CreatedOn;

        //        var group = await supplierGroupService.Insert(pgvm);
        //        if (group.Status.ToLower() == "success")
        //        {

        //            SupplierGroupVM supplierGroupVM = (SupplierGroupVM)group.DataVM;

        //            SupplierVM pvm = new SupplierVM();
        //            var details = pvm.MasterSupplierList;

        //            pvm.Code = supplier.Code;
        //            if (supplier.MasterSupplierList != null && supplier.MasterSupplierList.Any())
        //            {
        //                pvm.Name = supplier.MasterSupplierList.First().Name;
        //            }
        //            pvm.SupplierGroupId = supplierGroupVM.Id;
        //            pvm.IsActive = supplier.IsActive;
        //            pvm.IsArchive = supplier.IsArchive;
        //            pvm.CreatedBy = supplier.CreatedBy;
        //            pvm.CreatedOn = supplier.CreatedOn;


        //            if (string.IsNullOrWhiteSpace(pvm.Code))
        //            {
        //                pvm.Code = _commonRepo.CodeGenerationNo(
        //                    "Supplier",
        //                    "Supplier",
        //                    conn,
        //                    transaction
        //                );
        //            }

        //            result = await _repo.Insert(pvm, conn, transaction);
        //        }
        //        else
        //        {
        //            var name = supplier.MasterSupplierGroupName;

        //            var retusls = supplierGroupService.grouplist(new[] { "M.Name" }, new[] { name }, null);

        //            SupplierVM pvm = new SupplierVM();


        //            pvm.Code = supplier.Code;
        //            if (supplier.MasterSupplierList != null && supplier.MasterSupplierList.Any())
        //            {
        //                pvm.Name = supplier.MasterSupplierList.First().Name;
        //            }
        //            pvm.SupplierGroupId = retusls.Id;
        //            pvm.IsActive = supplier.IsActive;
        //            pvm.IsArchive = supplier.IsArchive;
        //            pvm.CreatedBy = supplier.CreatedBy;
        //            pvm.CreatedOn = supplier.CreatedOn;


        //            if (string.IsNullOrWhiteSpace(pvm.Code))
        //            {
        //                pvm.Code = _commonRepo.CodeGenerationNo(
        //                    "Supplier",
        //                    "Supplier",
        //                    conn,
        //                    transaction
        //                );
        //            }

        //            result = await _repo.Insert(pvm, conn, transaction);
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


        public async Task<ResultVM> InsertFromMasterSupplier(SupplierVM supplier)
        {
            SupplierRepository _repo = new SupplierRepository();
            _commonRepo = new CommonRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error" };

            bool isNewConnection = false;
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                await conn.OpenAsync();
                isNewConnection = true;
                transaction = conn.BeginTransaction();

                SupplierGroupService supplierGroupService = new SupplierGroupService();

                if (supplier.MasterSupplierList == null || !supplier.MasterSupplierList.Any())
                    throw new Exception("No supplier found to save.");

                // Group suppliers by their group name
                var groupedSuppliers = supplier.MasterSupplierList
                    .Where(x => !string.IsNullOrWhiteSpace(x.MasterSupplierGroupName))
                    .GroupBy(x => x.MasterSupplierGroupName.Trim())
                    .ToList();

                foreach (var group in groupedSuppliers)
                {
                    string groupName = group.Key;
                    if (string.IsNullOrWhiteSpace(groupName))
                        throw new Exception("Supplier group name is required.");

                    // Check if the group exists
                    var groupResult = await supplierGroupService.grouplist(
                        new[] { "M.Name" }, new[] { groupName }, null);

                    SupplierGroupVM supplierGroupVM = groupResult?.Status == "Success" && groupResult.DataVM is List<SupplierGroupVM> list &&list.Any() ? list.First(): null;

                    // Create group if it doesn't exist
                    if (supplierGroupVM == null)
                    {
                        SupplierGroupVM newGroup = new SupplierGroupVM
                        {
                            Name = groupName,
                            Description = supplier.Description,
                            IsActive = supplier.IsActive,
                            IsArchive = supplier.IsArchive,
                            CreatedBy = supplier.CreatedBy,
                            CreatedOn = supplier.CreatedOn,
                            CompanyId = supplier.CompanyId,
                            UserId = supplier.UserId

                        };

                        var insertResult = await supplierGroupService.Insert(newGroup);
                        if (insertResult.Status != "Success") {
                            //throw new Exception(insertResult.Message);

                            var name = supplier.MasterSupplierGroupName;

                            var retusls = supplierGroupService.grouplist(new[] { "M.Name" }, new[] { name }, null);

                            // supplierGroupVM = (SupplierGroupVM)insertResult.DataVM;
                            // Insert suppliers under this group
                            foreach (var item in group)
                            {
                                string supplierName = item.Name.Trim();

                                if (_repo.Exists(supplierName, conn, transaction))
                                    continue;

                                SupplierVM pvm = new SupplierVM
                                {
                                    CompanyId = supplier.CompanyId,  
                                    UserId = supplier.UserId,
                                    Name = supplierName,
                                    Code = string.IsNullOrWhiteSpace(item.Code)? _commonRepo.CodeGenerationNo("Supplier", "Supplier", conn, transaction): item.Code,
                                    SupplierGroupId = retusls.Id,
                                    BanglaName = item.BanglaName,  
                                    Address = item.Address,        
                                    City = item.City,            
                                    TelephoneNo = item.TelephoneNo, 
                                    Email = item.Email,           
                                    ContactPerson = item.ContactPerson,
                                    IsActive = supplier.IsActive,
                                    IsArchive = supplier.IsArchive,
                                    CreatedBy = supplier.CreatedBy,
                                    CreatedOn = supplier.CreatedOn,
                                    CreatedFrom = supplier.CreatedFrom,
                                };

                                result = await _repo.Insert(pvm, conn, transaction);

                                if (result.Status != "Success")
                                    throw new Exception(result.Message);
                            }

                        }
                        else {
                            supplierGroupVM = (SupplierGroupVM)insertResult.DataVM;
                            // Insert suppliers under this group
                            foreach (var item in group)
                            {
                                string supplierName = item.Name.Trim();

                                if (_repo.Exists(supplierName, conn, transaction))
                                    continue;

                                SupplierVM pvm = new SupplierVM
                                {
                                    CompanyId = supplier.CompanyId,
                                    UserId = supplier.UserId,
                                    Name = supplierName,
                                    Code = string.IsNullOrWhiteSpace(item.Code)
                                        ? _commonRepo.CodeGenerationNo("Supplier", "Supplier", conn, transaction)
                                        : item.Code,
                                    SupplierGroupId = supplierGroupVM.Id,
                                    BanglaName = item.BanglaName,
                                    Address = item.Address,
                                    City = item.City,
                                    TelephoneNo = item.TelephoneNo,
                                    Email = item.Email,
                                    ContactPerson = item.ContactPerson,
                                    IsActive = supplier.IsActive,
                                    IsArchive = supplier.IsArchive,
                                    CreatedBy = supplier.CreatedBy,
                                    CreatedOn = supplier.CreatedOn
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

        public async Task<ResultVM> ReportList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SupplierRepository _repo = new SupplierRepository();
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

                result = await _repo.ReportList(conditionalFields, conditionalValues, vm, conn, transaction);

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


        //public async Task<ResultVM> InsertFromMasterSupplier(SupplierVM supplier)
        //{
        //    SupplierRepository _repo = new SupplierRepository();
        //    _commonRepo = new CommonRepository();

        //    ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

        //    bool isNewConnection = false;
        //    SqlConnection conn = null;
        //    SqlTransaction transaction = null;

        //    try
        //    {
        //        conn = new SqlConnection(DatabaseHelper.GetConnectionString());
        //        await conn.OpenAsync();
        //        isNewConnection = true;

        //        transaction = conn.BeginTransaction();

        //        SupplierGroupService supplierGroupService = new SupplierGroupService();

        //        // =========================
        //        // GROUP EXIST CHECK
        //        // =========================

        //        string groupName = supplier.MasterSupplierGroupName?.Trim();
        //        if (string.IsNullOrWhiteSpace(groupName))
        //            throw new Exception("Supplier group name is required.");

        //        var groupResult = await supplierGroupService.grouplist(new[] { "M.Name" }, new[] { groupName }, null);

        //        SupplierGroupVM supplierGroupVM = null;

        //        if (groupResult?.Status == "Success" && groupResult.DataVM is List<SupplierGroupVM> groupList && groupList.Any())
        //        {
        //            supplierGroupVM = groupList.First();
        //        }

        //        // =========================
        //        // CREATE GROUP IF NOT EXISTS
        //        // =========================
        //        if (supplierGroupVM == null)
        //        {
        //            SupplierGroupVM pgvm = new SupplierGroupVM
        //            {
        //                Name = groupName,
        //                IsActive = supplier.IsActive,
        //                IsArchive = supplier.IsArchive,
        //                CreatedBy = supplier.CreatedBy,
        //                CreatedOn = supplier.CreatedOn
        //            };

        //            var groupInsert = await supplierGroupService.Insert(pgvm);

        //            if (groupInsert.Status != "Success")
        //                throw new Exception(groupInsert.Message);

        //            supplierGroupVM = (SupplierGroupVM)groupInsert.DataVM;
        //        }

        //        // =========================
        //        // SUPPLIER INSERT (MULTIPLE)
        //        // =========================

        //        if (supplier.MasterSupplierList == null || !supplier.MasterSupplierList.Any())
        //            throw new Exception("No supplier found to save.");

        //        // 🔹 clean + distinct supplier list
        //        var supplierList = supplier.MasterSupplierList.Where(x => !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name.Trim().ToLower()).Select(g => g.First()).ToList();

        //        foreach (var item in supplierList)
        //        {
        //            string supplierName = item.Name.Trim();

        //            bool supplierExists = _repo.Exists(supplierName, conn, transaction);

        //            // 🔥 already exists → silently skip
        //            if (supplierExists)
        //                continue;

        //            SupplierVM pvm = new SupplierVM
        //            {
        //                Name = supplierName,
        //                Code = string.IsNullOrWhiteSpace(item.Code)? _commonRepo.CodeGenerationNo("Supplier", "Supplier", conn, transaction): item.Code,
        //                SupplierGroupId = supplierGroupVM.Id,
        //                IsActive = supplier.IsActive,
        //                IsArchive = supplier.IsArchive,
        //                CreatedBy = supplier.CreatedBy,
        //                CreatedOn = supplier.CreatedOn
        //            };

        //            result = await _repo.Insert(pvm, conn, transaction);

        //            if (result.Status != "Success")
        //                throw new Exception(result.Message);
        //        }

        //        transaction.Commit();

        //        return new ResultVM
        //        {
        //            Status = "Success",
        //            Message = "Suppliers saved successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //            transaction.Rollback();

        //        return new ResultVM
        //        {
        //            Status = "Fail",
        //            Message = ex.Message,
        //            ExMessage = ex.ToString()
        //        };
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //            conn.Close();
        //    }
        //}











        //public async Task<ResultVM> InsertFromMasterSupplier(SupplierVM supplier)
        //{
        //    SupplierRepository _repo = new SupplierRepository();
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

        //        SupplierGroupService supplierGroupService = new SupplierGroupService();

        //        // 🔹 Check group exists
        //        var groupResult = supplierGroupService.grouplist(new[] { "M.Name" }, new[] { supplier.MasterSupplierGroupName }, null);

        //        int supplierGroupId = 0;

        //        if (groupResult != null && groupResult.Id != 0)
        //        {
        //            supplierGroupId = groupResult.Id;
        //        }
        //        else
        //        {
        //            SupplierGroupVM pgvm = new SupplierGroupVM
        //            {
        //                Name = supplier.MasterSupplierGroupName,
        //                IsActive = supplier.IsActive,
        //                IsArchive = supplier.IsArchive,
        //                CreatedBy = supplier.CreatedBy,
        //                CreatedOn = supplier.CreatedOn
        //            };

        //            var insertGroup = await supplierGroupService.Insert(pgvm);

        //            if (insertGroup.Status.ToLower() != "success")
        //                throw new Exception(insertGroup.Message);

        //            supplierGroupId = ((SupplierGroupVM)insertGroup.DataVM).Id;
        //        }

        //        if (supplier.MasterSupplierList == null || !supplier.MasterSupplierList.Any())
        //            throw new Exception("No supplier details found.");

        //        foreach (var item in supplier.MasterSupplierList)
        //        {

        //            // 🔹 Supplier prepare
        //            SupplierVM pvm = new SupplierVM

        //            {
        //                SupplierGroupId = supplierGroupId,
        //                IsActive = supplier.IsActive,
        //                IsArchive = supplier.IsArchive,
        //                CreatedBy = supplier.CreatedBy,
        //                CreatedOn = supplier.CreatedOn,
        //                Code = item.Code
        //            };

        //            pvm.Name = item.Name;

        //            if (string.IsNullOrWhiteSpace(pvm.Code))
        //            {
        //                pvm.Code = _commonRepo.CodeGenerationNo(
        //                    "Supplier",
        //                    "Supplier",
        //                    conn,
        //                    transaction
        //                );
        //            }

        //            result = await _repo.Insert(pvm, conn, transaction);

        //            if (result.Status != "Success")
        //                throw new Exception(result.Message);
        //        }

        //        transaction.Commit();
        //        result.Status = "Success";
        //        result.Message = "All suppliers saved successfully";

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (transaction != null && isNewConnection)
        //            transaction.Rollback();

        //        result.Message = ex.Message;
        //        result.ExMessage = ex.ToString();
        //        return result;
        //    }
        //    finally
        //    {
        //        if (isNewConnection && conn != null)
        //            conn.Close();
        //    }
        //}





        //public async Task<ResultVM> GetPurchaseBySupplier(string?[] IDs)
        //{
        //    SupplierRepository _repo = new SupplierRepository();
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

        //        result = await _repo.GetPurchaseBySupplier(IDs, conn, transaction);

        //        var lst = new List<PaymentVM>();

        //        string data = JsonConvert.SerializeObject(result.DataVM);
        //        lst = JsonConvert.DeserializeObject<List<PaymentVM>>(data);

        //        //bool allSame = lst.Select(p => p.CustomerId).Distinct().Count() == 1;
        //        //if (!allSame)
        //        //{
        //        //    throw new Exception("Supplier is not distinct!");
        //        //}

        //        var detailsDataList = await _repo.GetDetails(IDs, conn, transaction);

        //        if (detailsDataList.Status == "Success" && detailsDataList.DataVM is DataTable dt)
        //        {
        //            string json = JsonConvert.SerializeObject(dt);
        //            var details = JsonConvert.DeserializeObject<List<PaymentDetailVM>>(json);

        //            // Check if lst is not null and contains items
        //            if (lst != null && lst.Any())
        //            {
        //                lst.FirstOrDefault().paymentDetailList = details;
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


    }


}
