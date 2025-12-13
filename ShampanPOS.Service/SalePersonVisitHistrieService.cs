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
    public class SalePersonVisitHistrieService
    {
        public async Task<ResultVM> Insert(SalePersonVisitHistrieVM SalePersonVisitHistrie)
        {

            CommonRepository _commonRepo = new CommonRepository();
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();

            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            bool isNewConnection = false;
            bool CampaignCheckExists = false;

            SqlConnection conn = null;
            SqlTransaction transaction = null;
            try
            {
                conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                conn.Open();
                isNewConnection = true;

                transaction = conn.BeginTransaction();

                CampaignCheckExists = await _repo.SalePersonVisitHistrieCheckExists(SalePersonVisitHistrie, conn, transaction);
                if (CampaignCheckExists)
                {
                    throw new Exception("This Route Already Visited !");
                }


                result = await _repo.Insert(SalePersonVisitHistrie, conn, transaction);

                ResultVM customer = await _repo.CostomerList(SalePersonVisitHistrie, conn, transaction);

                string customerjson = JsonConvert.SerializeObject(customer.DataVM);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(customerjson);

                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("No Customer Found For This Route!");
                }

                if (customer.Status == "Success" && dt.Rows.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(dt);
                    var details = JsonConvert.DeserializeObject<List<CustomerVM>>(json);

                    List<SalePersonVisitHistrieDetailVM> SalePersonVisitHistrieDetails = new List<SalePersonVisitHistrieDetailVM>();

                    foreach (var item in details)
                    {
                        SalePersonVisitHistrieDetailVM SalePersondetails = new SalePersonVisitHistrieDetailVM();
                        SalePersondetails.SalePersonVisitHistroyId = SalePersonVisitHistrie.Id;
                        SalePersondetails.BranchId = SalePersonVisitHistrie.BranchId;
                        SalePersondetails.CustomerId = item.Id;
                        SalePersondetails.IsVisited = false;

                        SalePersondetails.SalePersonVisitHistroyId = Convert.ToInt32(result.Id);
                        SalePersonVisitHistrieDetails.Add(SalePersondetails);
                    }
                    SalePersonVisitHistrie.SalePersonVisitHistrieDetails = SalePersonVisitHistrieDetails;

                }
                foreach (var details in SalePersonVisitHistrie.SalePersonVisitHistrieDetails)
                {
                    result = await _repo.DetailsInsert(new List<SalePersonVisitHistrieDetailVM> { details }, conn, transaction);
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


        public async Task<ResultVM> Update(SalePersonVisitHistrieVM VM)
        {
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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

                // Update the main record
                result = await _repo.Update(VM, conn, transaction);

                if (result.Status == "Success" && VM.SalePersonVisitHistrieDetails != null)
                {
                    // Loop through each SalePersonVisitHistrieDetail and update
                    foreach (var details in VM.SalePersonVisitHistrieDetails)
                    {
                        result = await _repo.DetailsUpdate(new List<SalePersonVisitHistrieDetailVM> { details }, conn, transaction);

                        if (result.Status != "Success")
                        {
                            throw new Exception(result.Message);
                        }
                    }
                }

                // Commit the transaction if everything is successful
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
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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

        public async Task<ResultVM> CostomerList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null)
        {
            SalePersonVisitHistrieVM salePersonVisitHistrie = new SalePersonVisitHistrieVM();
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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


                result = await _repo.CostomerList(salePersonVisitHistrie, conn, transaction);
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
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository(); // Assuming this is the repository for SalePersonVisitHistrieVM
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

                //Fetch data using the repository
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
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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

        public async Task<ResultVM> GetGridData(GridOptions options, string salePersonId, string[] conditionalFields, string[] conditionalValues)
        {
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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

                result = await _repo.GetGridData(options, salePersonId, conditionalFields, conditionalValues, conn, transaction);

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





        public async Task<ResultVM> GetAllGridData(GridOptions options, string[] conditionalFields, string[] conditionalValues)
        {
            SalePersonVisitHistrieRepository _repo = new SalePersonVisitHistrieRepository();
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

                result = await _repo.GetAllGridData(options, conditionalFields, conditionalValues, conn, transaction);

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
