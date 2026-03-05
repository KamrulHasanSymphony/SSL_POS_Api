using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class SaleCreditCardRepository : CommonRepository
    {
        public async Task<ResultVM> Insert(SaleCreditCardVM details, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                Id = "0",
                DataVM = null
            };

            try
            {
                if (conn == null)
                    throw new Exception("Database connection failed!");

                string query = @"
        INSERT INTO SaleCreditCards
        (
            SaleId,
            CreditCardId,
            CardTotal,
            Remarks
        )
        VALUES
        (
            @SaleId,
            @CreditCardId,
            @CardTotal,
            @Remarks
        );
        SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@SaleId", details.SaleId ?? 0);
                    cmd.Parameters.AddWithValue("@CreditCardId", details.CreditCardId ?? 0);
                    cmd.Parameters.AddWithValue("@CardTotal", details.CardTotal ?? 0);
                    cmd.Parameters.AddWithValue("@Remarks", details.Remarks ?? "");

                    object newId = await cmd.ExecuteScalarAsync();
                    details.Id = Convert.ToInt32(newId);

                    result.Status = "Success";
                    result.Message = "Sale Credit Card inserted successfully.";
                    result.Id = details.Id.ToString();
                    result.DataVM = details;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Message = "Insert failed!";
                result.ExMessage = ex.ToString();
                return result;
            }
        }


        public ResultVM DDetailsList(string[] conditionalFields, string[] conditionalValues, PeramModel vm, SqlConnection conn, SqlTransaction transaction)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                    throw new Exception("Database connection fail!");

                string query = @"
                    SELECT 
             ISNULL(M.Id, 0) AS Id,
             ISNULL(M.SaleId, 0) AS SaleId,
             ISNULL(M.CreditCardId, 0) AS CreditCardId,
             ISNULL(M.CardTotal, 0) AS CardTotal,
             ISNULL(M.Remarks, '') AS Remarks
         FROM SaleCreditCards M
         WHERE 1=1 ";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter da = CreateAdapter(query, conn, transaction);
                da.SelectCommand = ApplyParameters(da.SelectCommand, conditionalFields, conditionalValues);
                da.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Sale Credit Card details data retrieved successfully.";
                result.DataVM = dataTable;

                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
                return result;
            }
        }


    }
}
