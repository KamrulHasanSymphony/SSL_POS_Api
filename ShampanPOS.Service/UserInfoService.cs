using ShampanPOS.Repository;
using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Service
{
    public class UserInfoService
    {
        public async Task<ResultVM> Insert(UserInfoVM userinfo)
        {
            string CodeGroup = "UserInfo";
            string CodeName = "UserInfo";
            CommonRepository _commonRepo = new CommonRepository();
            UserInfoRepository _repo = new UserInfoRepository();

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

                //#region Check Exist Data
                //string[] conditionField = { "AccountNo" };
                //string[] conditionValue = { bankaccount.AccountNo.Trim() };

                //bool exist = _commonRepo.CheckExists("BankAccounts", conditionField, conditionValue, conn, transaction);

                //if (exist)
                //{
                //    result.Message = "Data Already Exist!";
                //    throw new Exception("Data Already Exist!");
                //}
                //#endregion

        
                result = await _repo.Insert(userinfo, conn, transaction);

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

    }
}
