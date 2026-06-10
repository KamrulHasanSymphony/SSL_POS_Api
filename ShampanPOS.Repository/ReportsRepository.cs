using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShampanPOS.Repository
{
    public class ReportsRepository
    {
        public async Task<ResultVM> BankTransactionReportList(
    string[] conditionalFields,
    string[] conditionalValues,
    BankTransactionReportVM vm = null,
    SqlConnection conn = null,
    SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();

            ResultVM result = new ResultVM
            {
                Status = "Fail",
                Message = "Error",
                ExMessage = null,
                DataVM = null
            };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                StringBuilder query;
                SqlDataAdapter objComm;

                #region Base Query

                if (vm?.IsSummary == true)
                {
                    query = new StringBuilder(@"
                SELECT * FROM (
                    SELECT
                        'Deposit' AS TransactionType,
                        d.Id AS TransactionId,
                        d.Code AS TransactionCode,
                        d.TransactionDate,
                        d.Reference,
                        SUM(d.TotalDepositAmount) AS Amount,
                        d.IsCash,
                        d.ChequeNo,
                        d.ChequeBankName,
                        d.ChequeDate,
                        d.Comments,
                        d.ToBankAccountId AS AccountId,
                        ba.AccountNo,
                        ba.AccountName,
                        bi.Id AS BankId,
                        bi.Name AS BankName,
                        bi.Code AS BankCode,
                        d.BranchId,
                        d.IsActive,
                        d.IsArchive,
                        d.CreatedBy,
                        d.CreatedOn
                    FROM Deposits d
                    LEFT JOIN BankAccounts ba ON d.ToBankAccountId = ba.Id
                    LEFT JOIN BankInformations bi ON ba.BankId = bi.Id
                    WHERE d.IsActive = 1 AND d.IsArchive = 0
                      AND (@TransactionType IS NULL OR @TransactionType = 'Deposit')
                    GROUP BY
                        d.Id, d.Code, d.TransactionDate, d.Reference,
                        d.IsCash, d.ChequeNo, d.ChequeBankName, d.ChequeDate,
                        d.Comments, d.ToBankAccountId, ba.AccountNo, ba.AccountName,
                        bi.Id, bi.Name, bi.Code, d.BranchId,
                        d.IsActive, d.IsArchive, d.CreatedBy, d.CreatedOn

                    UNION ALL

                    SELECT
                        'Withdrawal' AS TransactionType,
                        w.Id AS TransactionId,
                        w.Code AS TransactionCode,
                        w.TransactionDate,
                        w.Reference,
                        SUM(w.TotalDepositAmount) AS Amount,
                        w.IsCash,
                        w.ChequeNo,
                        w.ChequeBankName,
                        w.ChequeDate,
                        w.Comments,
                        w.FromBankAccountId AS AccountId,
                        ba.AccountNo,
                        ba.AccountName,
                        bi.Id AS BankId,
                        bi.Name AS BankName,
                        bi.Code AS BankCode,
                        w.BranchId,
                        w.IsActive,
                        w.IsArchive,
                        w.CreatedBy,
                        w.CreatedOn
                    FROM Withdrawals w
                    LEFT JOIN BankAccounts ba ON w.FromBankAccountId = ba.Id
                    LEFT JOIN BankInformations bi ON ba.BankId = bi.Id
                    WHERE w.IsActive = 1 AND w.IsArchive = 0
                      AND (@TransactionType IS NULL OR @TransactionType = 'Withdrawal')
                    GROUP BY
                        w.Id, w.Code, w.TransactionDate, w.Reference,
                        w.IsCash, w.ChequeNo, w.ChequeBankName, w.ChequeDate,
                        w.Comments, w.FromBankAccountId, ba.AccountNo, ba.AccountName,
                        bi.Id, bi.Name, bi.Code, w.BranchId,
                        w.IsActive, w.IsArchive, w.CreatedBy, w.CreatedOn
                ) AS CombinedReport
                WHERE 1=1
            ");
                }
                else if (vm?.TransactionType == "Statement")
                {
                    query = new StringBuilder(@"
        SELECT * FROM (

    -- Sales (Credit Card) → IN
    SELECT
        'SaleCard' AS TransactionType,
        'Sale' AS SourceTable,
        'In' AS InOut,
        SLS.Id AS TransactionId,
        CAST(SLS.Code AS NVARCHAR(50)) AS TransactionCode,
        CAST(SLS.InvoiceDateTime AS DATE) AS TransactionDate,
        SCC.Cardtotal AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        NULL AS Reference,
        NULL AS Comments,
        0 AS BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM SaleCreditCards SCC
    LEFT JOIN Sales SLS ON SCC.SaleId = SLS.Id
    LEFT JOIN BankAccounts BA ON SCC.CreditCardId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id

    UNION ALL

    -- Collections → IN
    SELECT
        'Collection' AS TransactionType,
        'Collection' AS SourceTable,
        'In' AS InOut,
        CLC.Id AS TransactionId,
        CLC.Code AS TransactionCode,
        CLC.TransactionDate,
        CLC.TotalCollectAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        NULL AS Reference,
        NULL AS Comments,
        0 AS BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Collections CLC
    LEFT JOIN BankAccounts BA ON CLC.BankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id

    UNION ALL

    -- Payments → OUT
    SELECT
        'Payment' AS TransactionType,
        'Payment' AS SourceTable,
        'Out' AS InOut,
        PYM.Id AS TransactionId,
        PYM.Code AS TransactionCode,
        PYM.TransactionDate,
        PYM.TotalPaymentAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        NULL AS Reference,
        NULL AS Comments,
        PYM.BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Payments PYM
    LEFT JOIN BankAccounts BA ON PYM.BankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id

    UNION ALL

    -- Deposits FromBankAccount → IN
    SELECT
        'Deposit' AS TransactionType,
        'Deposit' AS SourceTable,
        'In' AS InOut,
        DPS.Id AS TransactionId,
        DPS.Code AS TransactionCode,
        DPS.TransactionDate,
        DPS.TotalDepositAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        DPS.Reference,
        DPS.Comments,
        DPS.BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Deposits DPS
    LEFT JOIN BankAccounts BA ON DPS.FromBankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id
    WHERE DPS.IsActive = 1 AND DPS.IsArchive = 0

    UNION ALL

    -- Deposits ToBankAccount → OUT
    SELECT
        'Deposit' AS TransactionType,
        'Deposit' AS SourceTable,
        'Out' AS InOut,
        DPS.Id AS TransactionId,
        DPS.Code AS TransactionCode,
        DPS.TransactionDate,
        DPS.TotalDepositAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        DPS.Reference,
        DPS.Comments,
        DPS.BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Deposits DPS
    LEFT JOIN BankAccounts BA ON DPS.ToBankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id
    WHERE DPS.IsActive = 1 AND DPS.IsArchive = 0

    UNION ALL

    -- Withdrawals FromBankAccount → OUT
    SELECT
        'Withdrawal' AS TransactionType,
        'Withdrawal' AS SourceTable,
        'Out' AS InOut,
        WDL.Id AS TransactionId,
        WDL.Code AS TransactionCode,
        WDL.TransactionDate,
        WDL.TotalDepositAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        WDL.Reference,
        WDL.Comments,
        WDL.BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Withdrawals WDL
    LEFT JOIN BankAccounts BA ON WDL.FromBankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id
    WHERE WDL.IsActive = 1 AND WDL.IsArchive = 0

    UNION ALL

    -- Withdrawals ToBankAccount → IN
    SELECT
        'Withdrawal' AS TransactionType,
        'Withdrawal' AS SourceTable,
        'In' AS InOut,
        WDL.Id AS TransactionId,
        WDL.Code AS TransactionCode,
        WDL.TransactionDate,
        WDL.TotalDepositAmount AS Amount,
        BA.Id AS AccountId,
        BA.AccountNo,
        BA.AccountName,
        BI.Id AS BankId,
        BI.Name AS BankName,
        BI.Code AS BankCode,
        WDL.Reference,
        WDL.Comments,
        WDL.BranchId,
        CAST(0 AS BIT) AS IsCash,       -- Added missing column
        NULL AS ChequeNo,              -- Added missing column
        NULL AS ChequeBankName,        -- Added missing column
        NULL AS ChequeDate,            -- Added missing column
        NULL AS CreatedBy,             -- Added missing column
        NULL AS CreatedOn              -- Added missing column
    FROM Withdrawals WDL
    LEFT JOIN BankAccounts BA ON WDL.ToBankAccountId = BA.Id
    LEFT JOIN BankInformations BI ON BA.BankId = BI.Id
    WHERE WDL.IsActive = 1 AND WDL.IsArchive = 0

) AS StatementReport
WHERE 1=1
    ");
                }
                else
                {
                    query = new StringBuilder(@"
            SELECT * FROM (
                SELECT
                    'Deposit' AS TransactionType,
                    d.Id AS TransactionId,
                        d.Code AS TransactionCode,
                        d.TransactionDate,
                        d.Reference,
                        d.TotalDepositAmount AS Amount,
                        d.IsCash,
                        d.ChequeNo,
                        d.ChequeBankName,
                        d.ChequeDate,
                        d.Comments,
                        d.ToBankAccountId AS AccountId,
                        ba.AccountNo,
                        ba.AccountName,
                        bi.Id AS BankId,
                        bi.Name AS BankName,
                        bi.Code AS BankCode,
                        d.BranchId,
                        d.IsActive,
                        d.IsArchive,
                        d.CreatedBy,
                        d.CreatedOn
                    FROM Deposits d
                    LEFT JOIN BankAccounts ba ON d.ToBankAccountId = ba.Id
                    LEFT JOIN BankInformations bi ON ba.BankId = bi.Id
                    WHERE d.IsActive = 1 AND d.IsArchive = 0
                      AND (@TransactionType IS NULL OR @TransactionType = 'Deposit')

                    UNION ALL

                    SELECT
                        'Withdrawal' AS TransactionType,
                        w.Id AS TransactionId,
                        w.Code AS TransactionCode,
                        w.TransactionDate,
                        w.Reference,
                        w.TotalDepositAmount AS Amount,
                        w.IsCash,
                        w.ChequeNo,
                        w.ChequeBankName,
                        w.ChequeDate,
                        w.Comments,
                        w.FromBankAccountId AS AccountId,
                        ba.AccountNo,
                        ba.AccountName,
                        bi.Id AS BankId,
                        bi.Name AS BankName,
                        bi.Code AS BankCode,
                        w.BranchId,
                        w.IsActive,
                        w.IsArchive,
                        w.CreatedBy,
                        w.CreatedOn
                    FROM Withdrawals w
                    LEFT JOIN BankAccounts ba ON w.FromBankAccountId = ba.Id
                    LEFT JOIN BankInformations bi ON ba.BankId = bi.Id
                    WHERE w.IsActive = 1 AND w.IsArchive = 0
                      AND (@TransactionType IS NULL OR @TransactionType = 'Withdrawal')
                ) AS CombinedReport
                WHERE 1=1
            ");
                }

                #endregion

                #region Dynamic Filters

                if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                    query.Append(" AND TransactionDate >= @FromDate");

                if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                    query.Append(" AND TransactionDate <= @ToDate");

                if ((vm?.BankId ?? 0) > 0)
                    query.Append(" AND BankId = @BankId");

                if ((vm?.BankAccountId ?? 0) > 0)
                    query.Append(" AND AccountId = @BankAccountId");

                if ((vm?.TransactionId ?? 0) > 0)
                    query.Append(" AND TransactionId = @TransactionId");

                if ((vm?.BranchId ?? 0) > 0)
                    query.Append(" AND BranchId = @BranchId");

                if (vm?.TransactionType != "Statement")
                {
                    if ((vm?.DepositId ?? 0) > 0)
                        query.Append(" AND (TransactionType = 'Deposit' AND TransactionId = @DepositId)");

                    if ((vm?.WithdrawalId ?? 0) > 0)
                        query.Append(" AND (TransactionType = 'Withdrawal' AND TransactionId = @WithdrawalId)");

                    if (!string.IsNullOrWhiteSpace(vm?.TransactionType))
                        query.Append(" AND TransactionType = @TransactionType");
                }

                query.Append(" ORDER BY TransactionDate ASC, TransactionId ASC");

                #endregion

                query = new StringBuilder(ApplyConditions(
                    query.ToString(), conditionalFields, conditionalValues, true));

                objComm = CreateAdapter(query.ToString(), conn, transaction);

                objComm.SelectCommand = ApplyParameters(
                    objComm.SelectCommand, conditionalFields, conditionalValues);

                #region Parameters
                if (!string.IsNullOrWhiteSpace(vm?.FromDate)) objComm.SelectCommand.Parameters.AddWithValue("@FromDate", DateTime.Parse(vm.FromDate));
                if (!string.IsNullOrWhiteSpace(vm?.ToDate)) objComm.SelectCommand.Parameters.AddWithValue("@ToDate", DateTime.Parse(vm.ToDate));

                if ((vm?.BankId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@BankId", vm.BankId);

                if ((vm?.BankAccountId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@BankAccountId", vm.BankAccountId);

                if ((vm?.TransactionId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@TransactionId", vm.TransactionId);

                if ((vm?.BranchId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                // In Parameters region — add after TransactionId parameter:
                if ((vm?.DepositId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@DepositId", vm.DepositId);

                if ((vm?.WithdrawalId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@WithdrawalId", vm.WithdrawalId);

                // Always pass TransactionType (NULL if not set)
                objComm.SelectCommand.Parameters.AddWithValue("@TransactionType",
                    string.IsNullOrWhiteSpace(vm?.TransactionType)
                        ? (object)DBNull.Value
                        : vm.TransactionType);

                // Statement Opening Balance
                if (vm?.TransactionType == "Statement" && !string.IsNullOrWhiteSpace(vm?.FromDate))
                {
                    var openingBalanceQuery = new StringBuilder(@"
                        SELECT ISNULL(SUM(CASE WHEN InOut = 'In' THEN Amount ELSE -Amount END), 0) AS OpeningBalance
                        FROM (
                            SELECT 'In' AS InOut, SCC.Cardtotal AS Amount
                            FROM SaleCreditCards SCC
                            LEFT JOIN Sales SLS ON SCC.SaleId = SLS.Id
                            LEFT JOIN BankAccounts BA ON SCC.CreditCardId = BA.Id
                            WHERE CAST(SLS.InvoiceDateTime AS DATE) < @FromDate
                            UNION ALL
                            SELECT 'In' AS InOut, CLC.TotalCollectAmount AS Amount
                            FROM Collections CLC
                            LEFT JOIN BankAccounts BA ON CLC.BankAccountId = BA.Id
                            WHERE CLC.TransactionDate < @FromDate
                            UNION ALL
                            SELECT 'Out' AS InOut, PYM.TotalPaymentAmount AS Amount
                            FROM Payments PYM
                            LEFT JOIN BankAccounts BA ON PYM.BankAccountId = BA.Id
                            WHERE PYM.TransactionDate < @FromDate
                            UNION ALL
                            SELECT 'In' AS InOut, DPS.TotalDepositAmount AS Amount
                            FROM Deposits DPS
                            LEFT JOIN BankAccounts BA ON DPS.FromBankAccountId = BA.Id
                            WHERE DPS.TransactionDate < @FromDate
                            AND DPS.IsActive = 1 AND DPS.IsArchive = 0
                            UNION ALL
                            SELECT 'Out' AS InOut, DPS.TotalDepositAmount AS Amount
                            FROM Deposits DPS
                            LEFT JOIN BankAccounts BA ON DPS.ToBankAccountId = BA.Id
                            WHERE DPS.TransactionDate < @FromDate
                            AND DPS.IsActive = 1 AND DPS.IsArchive = 0
                            UNION ALL
                            SELECT 'Out' AS InOut, WDL.TotalDepositAmount AS Amount
                            FROM Withdrawals WDL
                            LEFT JOIN BankAccounts BA ON WDL.FromBankAccountId = BA.Id
                            WHERE WDL.TransactionDate < @FromDate
                            AND WDL.IsActive = 1 AND WDL.IsArchive = 0
                            UNION ALL
                            SELECT 'In' AS InOut, WDL.TotalDepositAmount AS Amount
                            FROM Withdrawals WDL
                            LEFT JOIN BankAccounts BA ON WDL.ToBankAccountId = BA.Id
                            WHERE WDL.TransactionDate < @FromDate
                            AND WDL.IsActive = 1 AND WDL.IsArchive = 0
                        ) AS OpeningData
                    ");

                    // BankId filter
                    if ((vm?.BankId ?? 0) > 0)
                    {
                        openingBalanceQuery = new StringBuilder(openingBalanceQuery.ToString()
                            .Replace("WHERE CAST(SLS.InvoiceDateTime AS DATE) < @FromDate",
                                     "LEFT JOIN BankInformations BI ON BA.BankId = BI.Id WHERE CAST(SLS.InvoiceDateTime AS DATE) < @FromDate AND BI.Id = @BankId_OB")
                            .Replace("WHERE CLC.TransactionDate < @FromDate",
                                     "LEFT JOIN BankInformations BI ON BA.BankId = BI.Id WHERE CLC.TransactionDate < @FromDate AND BI.Id = @BankId_OB")
                            .Replace("WHERE PYM.TransactionDate < @FromDate",
                                     "LEFT JOIN BankInformations BI ON BA.BankId = BI.Id WHERE PYM.TransactionDate < @FromDate AND BI.Id = @BankId_OB")
                        );
                    }

                    // BankAccountId filter
                    if ((vm?.BankAccountId ?? 0) > 0)
                    {
                        openingBalanceQuery = new StringBuilder(openingBalanceQuery.ToString()
                            .Replace("WHERE CAST(SLS.InvoiceDateTime AS DATE) < @FromDate",
                                     "WHERE CAST(SLS.InvoiceDateTime AS DATE) < @FromDate AND BA.Id = @BankAccountId_OB")
                            .Replace("WHERE CLC.TransactionDate < @FromDate",
                                     "WHERE CLC.TransactionDate < @FromDate AND BA.Id = @BankAccountId_OB")
                            .Replace("WHERE PYM.TransactionDate < @FromDate",
                                     "WHERE PYM.TransactionDate < @FromDate AND BA.Id = @BankAccountId_OB")
                            .Replace("WHERE DPS.TransactionDate < @FromDate\r\n                            AND DPS.IsActive = 1 AND DPS.IsArchive = 0\r\n                            UNION ALL\r\n                            SELECT 'Out'",
                                     "WHERE DPS.TransactionDate < @FromDate AND DPS.IsActive = 1 AND DPS.IsArchive = 0 AND BA.Id = @BankAccountId_OB\r\n                            UNION ALL\r\n                            SELECT 'Out'")
                        );
                    }

                    using (var obCmd = new SqlCommand(openingBalanceQuery.ToString(), conn, transaction))
                    {
                        obCmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(vm.FromDate));
                        if ((vm?.BankId ?? 0) > 0)
                            obCmd.Parameters.AddWithValue("@BankId_OB", vm.BankId);
                        if ((vm?.BankAccountId ?? 0) > 0)
                            obCmd.Parameters.AddWithValue("@BankAccountId_OB", vm.BankAccountId);

                        var obResult = await obCmd.ExecuteScalarAsync();
                        vm.OpeningBalance = obResult != DBNull.Value ? Convert.ToDecimal(obResult) : 0;
                    }
                }
                else if (vm?.TransactionType == "Statement")
                {
                    vm.OpeningBalance = 0;
                }

                #endregion

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new BankTransactionReportVM
                {
                    TransactionId = row.Field<int?>("TransactionId") ?? 0,
                    TransactionCode = row.Field<string>("TransactionCode") ?? "",
                    TransactionType = row.Field<string>("TransactionType") ?? "",
                    TransactionDate = dataTable.Columns.Contains("TransactionDate") && row["TransactionDate"] != DBNull.Value && DateTime.TryParse(row["TransactionDate"]?.ToString(), out DateTime parsedDate) ? parsedDate.ToString("dd-MMM-yyyy") : "",
                    Reference = row.Field<string>("Reference") ?? "",
                    Amount = row.Field<decimal?>("Amount") ?? 0,
                    IsCash = row.Field<bool?>("IsCash") ?? false,
                    ChequeNo = row.Field<string>("ChequeNo") ?? "",
                    ChequeBankName = row.Field<string>("ChequeBankName") ?? "",
                    ChequeDate = dataTable.Columns.Contains("ChequeDate") && row["ChequeDate"] != DBNull.Value && DateTime.TryParse(row["ChequeDate"]?.ToString(), out DateTime cparsedDate) ? cparsedDate.ToString("dd-MMM-yyyy") : "",
                    Comments = row.Field<string>("Comments") ?? "",
                    AccountId = row.Field<int?>("AccountId") ?? 0,
                    AccountNo = row.Field<string>("AccountNo") ?? "",
                    AccountName = row.Field<string>("AccountName") ?? "",
                    BankId = row.Field<int?>("BankId") ?? 0,
                    BankName = row.Field<string>("BankName") ?? "",
                    BankCode = row.Field<string>("BankCode") ?? "",
                    BranchId = row.Field<int?>("BranchId") ?? 0,
                    CreatedBy = row.Field<string>("CreatedBy") ?? "",
                    CreatedOn = dataTable.Columns.Contains("CreatedOn") ? row["CreatedOn"]?.ToString() : "",
                    InOut = dataTable.Columns.Contains("InOut") ? row.Field<string>("InOut") ?? "" : "",
                    SourceTable = dataTable.Columns.Contains("SourceTable") ? row.Field<string>("SourceTable") ?? "" : ""
                }).ToList();

                // Running Balance for Statement
                if (vm?.TransactionType == "Statement")
                {
                    decimal runningBalance = vm.OpeningBalance??0;
                    foreach (var item in modelList)
                    {
                        if (item.InOut == "In")
                            runningBalance += item.Amount ?? 0;
                        else
                            runningBalance -= item.Amount ?? 0;

                        item.RunningBalance = runningBalance;
                    }
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully";
                result.DataVM = modelList;

                return result;
            }
            catch (Exception ex)
            {
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
                    conn.Dispose();
                }
            }
        }


        protected SqlDataAdapter CreateAdapter(string query, SqlConnection context, SqlTransaction transaction)
        {
            var cmd = new SqlCommand(query, context, transaction);
            return new SqlDataAdapter(cmd);
        }

        public string ApplyConditions(string sqlText, string[] conditionalFields, string[] conditionalValue, bool orOperator = false)
        {
            try
            {
                string cField = "";
                string field = "";
                bool conditionFlag = true;
                var checkValueExist = conditionalValue == null ? false : conditionalValue.ToList().Any(x => !string.IsNullOrEmpty(x));
                var checkConditioanlValue = conditionalValue == null ? false : conditionalValue.ToList().Any(x => !string.IsNullOrEmpty(x));

                if (checkValueExist && orOperator && checkConditioanlValue)
                {
                    sqlText += " and (";
                }

                if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
                {
                    for (int i = 0; i < conditionalFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionalFields[i]) || string.IsNullOrWhiteSpace(conditionalValue[i]))
                        {
                            continue;
                        }
                        cField = conditionalFields[i].ToString();
                        field = StringReplacing(cField);
                        cField = cField.Replace(".", "");
                        string operand = " AND ";

                        if (orOperator)
                        {
                            operand = " OR ";

                            if (conditionFlag)
                            {
                                operand = "  ";
                                conditionFlag = false;
                            }
                        }


                        if (conditionalFields[i].ToLower().Contains("like"))
                        {
                            sqlText += operand + conditionalFields[i] + " '%'+ " + " @" + cField.Replace("like", "").Trim() + " +'%'";
                        }
                        //else if (conditionalFields[i].Contains(">=") || conditionalFields[i].Contains("<="))
                        //{
                        //    sqlText += operand + conditionalFields[i] + " @" + cField;
                        //}
                        else if (conditionalFields[i].Contains(">") || conditionalFields[i].Contains("<"))
                        {
                            sqlText += operand + conditionalFields[i] + " @" + cField;
                        }

                        else if (conditionalFields[i].ToLower().Contains("between"))
                        {
                            cField = cField.Replace(" between", "");
                            field = field.Replace(" between", "");
                            string param = conditionalFields[i].Replace(" between", "");
                            sqlText += operand + param + " BETWEEN  @" + cField + " AND @" + field;
                        }
                        else if (conditionalFields[i].ToLower().Contains("not"))
                        {
                            cField = cField.Replace(" not", "");
                            string param = conditionalFields[i].Replace(" not", "");
                            sqlText += operand + param + " != @" + cField;
                        }
                        else if (conditionalFields[i].Contains("in", StringComparison.OrdinalIgnoreCase))
                        {
                            var test = conditionalFields[i].Split(" in");

                            if (test.Length > 1)
                            {
                                sqlText += operand + conditionalFields[i] + "(" + conditionalValue[i] + ")";
                            }
                            else
                            {
                                sqlText += operand + conditionalFields[i] + "= '" + Convert.ToString(conditionalValue[i]) + "'";
                            }
                        }
                        else
                        {
                            sqlText += operand + conditionalFields[i] + "= @" + cField;
                        }
                    }
                }

                if (checkValueExist && orOperator && checkConditioanlValue)
                {
                    sqlText += " )";
                }

                return sqlText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SqlCommand ApplyParameters(SqlCommand objComm, string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                string cField = "";
                if (conditionalFields != null && conditionalValue != null && conditionalFields.Length == conditionalValue.Length)
                {
                    for (int j = 0; j < conditionalFields.Length; j++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionalFields[j]) || string.IsNullOrWhiteSpace(conditionalValue[j]))
                        {
                            continue;
                        }
                        cField = conditionalFields[j].ToString();
                        //cField = StringReplacing(cField);
                        cField = cField.Replace(".", "");

                        if (conditionalFields[j].ToLower().Contains("like"))
                        {
                            objComm.Parameters.AddWithValue("@" + cField.Replace("like", "").Trim(), conditionalValue[j]);
                        }
                        else if (conditionalFields[j].ToLower().Contains("not"))
                        {
                            cField = cField.Replace(" not", "");
                            objComm.Parameters.AddWithValue("@" + cField, conditionalValue[j]);
                        }
                        else
                        {
                            objComm.Parameters.AddWithValue("@" + cField, conditionalValue[j]);
                        }
                    }
                }

                return objComm;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string StringReplacing(string stringToReplace)
        {
            try
            {
                string newString = stringToReplace;
                if (stringToReplace.Contains("."))
                {
                    newString = Regex.Replace(stringToReplace, @"^[^.]*.", "", RegexOptions.IgnorePatternWhitespace);
                }
                newString = newString.Replace(">", "From");
                newString = newString.Replace("<", "To");
                newString = newString.Replace("!", "");
                newString = newString.Replace("[", "");
                newString = newString.Replace("]", "");
                return newString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
