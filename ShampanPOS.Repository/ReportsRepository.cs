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

                if (vm?.IsSummary == true && vm?.TransactionType == "Payment")
                {
                    query = new StringBuilder(@"
                SELECT
                    S.Id AS SupplierId,
                    S.Code AS SupplierCode,
                    S.Name AS PartyName,
                    ISNULL(SUM(P.TotalPaymentAmount), 0) AS Amount,
                    COUNT(P.Id) AS TransactionCount,
                    MAX(P.TransactionDate) AS LastTransactionDate,
                    NULL AS TransactionId,
                    NULL AS TransactionCode,
                    NULL AS TransactionDate,
                    NULL AS Reference,
                    NULL AS Comments,
                    NULL AS AccountId,
                    NULL AS AccountNo,
                    NULL AS AccountName,
                    NULL AS BankId,
                    NULL AS BankName,
                    NULL AS BankCode,
                    0 AS BranchId,
                    CAST(0 AS BIT) AS IsCash,
                    NULL AS ChequeNo,
                    NULL AS ChequeBankName,
                    NULL AS ChequeDate,
                    NULL AS CreatedBy,
                    NULL AS CreatedOn,
                    'Payment' AS TransactionType,
                    NULL AS SourceTable,
                    NULL AS InOut,
                    NULL AS BranchName
                FROM Payments P
                LEFT JOIN Suppliers S ON P.SupplierId = S.Id
                LEFT JOIN BankAccounts A ON P.BankAccountId = A.Id
                LEFT JOIN BankInformations B ON A.BankId = B.Id
                WHERE 1=1
            ");
                }
                else if (vm?.IsSummary == true && vm?.TransactionType == "Collection")
                {
                    query = new StringBuilder(@"
                SELECT
                    C2.Id AS SupplierId,
                    C2.Code AS SupplierCode,
                    C2.Name AS PartyName,
                    ISNULL(SUM(C.TotalCollectAmount), 0) AS Amount,
                    COUNT(C.Id) AS TransactionCount,
                    MAX(C.TransactionDate) AS LastTransactionDate,
                    NULL AS TransactionId,
                    NULL AS TransactionCode,
                    NULL AS TransactionDate,
                    NULL AS Reference,
                    NULL AS Comments,
                    NULL AS AccountId,
                    NULL AS AccountNo,
                    NULL AS AccountName,
                    NULL AS BankId,
                    NULL AS BankName,
                    NULL AS BankCode,
                    0 AS BranchId,
                    CAST(0 AS BIT) AS IsCash,
                    NULL AS ChequeNo,
                    NULL AS ChequeBankName,
                    NULL AS ChequeDate,
                    NULL AS CreatedBy,
                    NULL AS CreatedOn,
                    'Collection' AS TransactionType,
                    NULL AS SourceTable,
                    NULL AS InOut,
                    NULL AS BranchName
                FROM Collections C
                LEFT JOIN Customers C2 ON C.CustomerId = C2.Id
                LEFT JOIN BankAccounts A ON C.BankAccountId = A.Id
                LEFT JOIN BankInformations B ON A.BankId = B.Id
                WHERE 1=1
            ");
                }
                else if (vm?.IsSummary == true)
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
                else if (vm?.TransactionType == "OutstandingBalance")
                {
                    query = new StringBuilder(@"
        SELECT
            BA.Id AS AccountId,
            BA.AccountNo,
            BA.AccountName,
            BI.Id AS BankId,
            BI.Name AS BankName,
            BI.Code AS BankCode,
            ISNULL(SUM(CASE WHEN T.InOut = 'In' THEN T.Amount ELSE -T.Amount END), 0) AS Amount,
            NULL AS TransactionId,
            NULL AS TransactionCode,
            NULL AS TransactionDate,
            NULL AS Reference,
            NULL AS Comments,
            0 AS BranchId,
            CAST(0 AS BIT) AS IsCash,
            NULL AS ChequeNo,
            NULL AS ChequeBankName,
            NULL AS ChequeDate,
            NULL AS CreatedBy,
            NULL AS CreatedOn,
            NULL AS InOut,
            NULL AS SourceTable,
            NULL AS PartyName,
            NULL AS BranchName,
            'OutstandingBalance' AS TransactionType
        FROM BankAccounts BA
        LEFT JOIN BankInformations BI ON BA.BankId = BI.Id
        LEFT JOIN (

            -- Sales Credit Card → IN
            SELECT SCC.CreditCardId AS BankAccountId, 'In' AS InOut, SCC.Cardtotal AS Amount
            FROM SaleCreditCards SCC
            LEFT JOIN Sales SLS ON SCC.SaleId = SLS.Id

            UNION ALL

            -- Collections → IN
            SELECT CLC.BankAccountId, 'In' AS InOut, CLC.TotalCollectAmount AS Amount
            FROM Collections CLC

            UNION ALL

            -- Payments → OUT
            SELECT PYM.BankAccountId, 'Out' AS InOut, PYM.TotalPaymentAmount AS Amount
            FROM Payments PYM

            UNION ALL

            -- Deposits FromBankAccount → OUT
            SELECT DPS.FromBankAccountId AS BankAccountId, 'Out' AS InOut, DPS.TotalDepositAmount AS Amount
            FROM Deposits DPS
            WHERE DPS.IsActive = 1 AND DPS.IsArchive = 0

            UNION ALL

            -- Deposits ToBankAccount → IN
            SELECT DPS.ToBankAccountId AS BankAccountId, 'In' AS InOut, DPS.TotalDepositAmount AS Amount
            FROM Deposits DPS
            WHERE DPS.IsActive = 1 AND DPS.IsArchive = 0

            UNION ALL

            -- Withdrawals FromBankAccount → OUT
            SELECT WDL.FromBankAccountId AS BankAccountId, 'Out' AS InOut, WDL.TotalDepositAmount AS Amount
            FROM Withdrawals WDL
            WHERE WDL.IsActive = 1 AND WDL.IsArchive = 0

            UNION ALL

            -- Withdrawals ToBankAccount → IN
            SELECT WDL.ToBankAccountId AS BankAccountId, 'In' AS InOut, WDL.TotalDepositAmount AS Amount
            FROM Withdrawals WDL
            WHERE WDL.IsActive = 1 AND WDL.IsArchive = 0

        ) AS T ON T.BankAccountId = BA.Id
        WHERE BA.IsActive = 1
        GROUP BY BA.Id, BA.AccountNo, BA.AccountName, BI.Id, BI.Name, BI.Code
    ");
                }
                else if (vm?.TransactionType == "Payment")
                {
                    query = new StringBuilder(@"
        SELECT * FROM (
            SELECT
                'Payment' AS TransactionType,
                NULL AS SourceTable,
                P.Id AS TransactionId,
                P.Code AS TransactionCode,
                P.TransactionDate,
                P.TotalPaymentAmount AS Amount,
                B.Id AS BankId,
                B.Name AS BankName,
                B.Code AS BankCode,
                A.Id AS AccountId,
                A.AccountName,
                A.AccountNo,
                A.BranchName,
                S.Id AS SupplierId,
                S.Name AS PartyName,
                NULL AS Reference,
                NULL AS Comments,
                P.BranchId,
                CAST(0 AS BIT) AS IsCash,
                NULL AS ChequeNo,
                NULL AS ChequeBankName,
                NULL AS ChequeDate,
                NULL AS CreatedBy,
                NULL AS CreatedOn,
                NULL AS InOut
            FROM Payments P
            LEFT JOIN BankAccounts A ON P.BankAccountId = A.Id
            LEFT JOIN BankInformations B ON A.BankId = B.Id
            LEFT JOIN Suppliers S ON P.SupplierId = S.Id
        ) AS PaymentReport
        WHERE 1=1
    ");
                }
                else if (vm?.TransactionType == "Collection")
                {
                    query = new StringBuilder(@"
        SELECT * FROM (
            SELECT
                'Collection' AS TransactionType,
                NULL AS SourceTable,
                C.Id AS TransactionId,
                C.Code AS TransactionCode,
                C.TransactionDate,
                C.TotalCollectAmount AS Amount,
                B.Id AS BankId,
                B.Name AS BankName,
                B.Code AS BankCode,
                A.Id AS AccountId,
                A.AccountName,
                A.AccountNo,
                A.BranchName,
                Cust.Id AS SupplierId,
                Cust.Name AS PartyName,
                NULL AS Reference,
                NULL AS Comments,
                0 AS BranchId,
                CAST(0 AS BIT) AS IsCash,
                NULL AS ChequeNo,
                NULL AS ChequeBankName,
                NULL AS ChequeDate,
                NULL AS CreatedBy,
                NULL AS CreatedOn,
                NULL AS InOut
            FROM Collections C
            LEFT JOIN BankAccounts A ON C.BankAccountId = A.Id
            LEFT JOIN BankInformations B ON A.BankId = B.Id
            LEFT JOIN Customers Cust ON C.CustomerId = Cust.Id
        ) AS CollectionReport
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

                // Payment/Collection Summary এর জন্য আলাদা date filter column name
                if (vm?.IsSummary == true && vm?.TransactionType == "Payment")
                {
                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND P.TransactionDate >= @FromDate");
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND P.TransactionDate <= @ToDate");
                    if ((vm?.BankId ?? 0) > 0)
                        query.Append(" AND B.Id = @BankId");
                    if ((vm?.BankAccountId ?? 0) > 0)
                        query.Append(" AND A.Id = @BankAccountId");
                    if ((vm?.SupplierId ?? 0) > 0)
                        query.Append(" AND P.SupplierId = @SupplierId");
                    query.Append(" GROUP BY S.Id, S.Code, S.Name");
                    query.Append(" ORDER BY S.Name ASC");
                }
                else if (vm?.IsSummary == true && vm?.TransactionType == "Collection")
                {
                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND C.TransactionDate >= @FromDate");
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND C.TransactionDate <= @ToDate");
                    if ((vm?.BankId ?? 0) > 0)
                        query.Append(" AND B.Id = @BankId");
                    if ((vm?.BankAccountId ?? 0) > 0)
                        query.Append(" AND A.Id = @BankAccountId");
                    if ((vm?.CustomerId ?? 0) > 0)
                        query.Append(" AND C.CustomerId = @CustomerId");
                    query.Append(" GROUP BY C2.Id, C2.Code, C2.Name");
                    query.Append(" ORDER BY C2.Name ASC");
                }
                else if (vm?.TransactionType != "OutstandingBalance")
                {
                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND TransactionDate >= @FromDate");

                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND TransactionDate <= @ToDate");

                    if ((vm?.TransactionId ?? 0) > 0)
                        query.Append(" AND TransactionId = @TransactionId");

                    if ((vm?.BranchId ?? 0) > 0)
                        query.Append(" AND BranchId = @BranchId");
                }

                // OutstandingBalance এ BankId/AccountId HAVING এ filter হবে
                if (vm?.IsSummary != true || (vm?.TransactionType != "Payment" && vm?.TransactionType != "Collection"))
                {
                    if (vm?.TransactionType == "OutstandingBalance")
                    {
                        if ((vm?.BankId ?? 0) > 0)
                            query.Append(" HAVING BI.Id = @BankId");

                        if ((vm?.BankAccountId ?? 0) > 0)
                            query.Append((vm?.BankId ?? 0) > 0
                                ? " AND BA.Id = @BankAccountId"
                                : " HAVING BA.Id = @BankAccountId");
                    }
                    else
                    {
                        if ((vm?.BankId ?? 0) > 0)
                            query.Append(" AND BankId = @BankId");

                        if ((vm?.BankAccountId ?? 0) > 0)
                            query.Append(" AND AccountId = @BankAccountId");
                    }

                    if (vm?.TransactionType != "Statement"
                        && vm?.TransactionType != "Payment"
                        && vm?.TransactionType != "Collection"
                        && vm?.TransactionType != "OutstandingBalance")
                    {
                        if ((vm?.DepositId ?? 0) > 0)
                            query.Append(" AND (TransactionType = 'Deposit' AND TransactionId = @DepositId)");

                        if ((vm?.WithdrawalId ?? 0) > 0)
                            query.Append(" AND (TransactionType = 'Withdrawal' AND TransactionId = @WithdrawalId)");

                        if (!string.IsNullOrWhiteSpace(vm?.TransactionType))
                            query.Append(" AND TransactionType = @TransactionType");
                    }

                    // Collection → CustomerId filter
                    if (vm?.TransactionType == "Collection" && (vm?.CustomerId ?? 0) > 0)
                        query.Append(" AND SupplierId = @CustomerId");

                    // Payment → SupplierId filter
                    if (vm?.TransactionType == "Payment" && (vm?.SupplierId ?? 0) > 0)
                        query.Append(" AND SupplierId = @SupplierId");

                    if (vm?.TransactionType == "OutstandingBalance")
                        query.Append(" ORDER BY BI.Name ASC, BA.AccountName ASC");
                    else
                        query.Append(" ORDER BY TransactionDate ASC, TransactionId ASC");
                }
                #endregion

                query = new StringBuilder(ApplyConditions(
                    query.ToString(), conditionalFields, conditionalValues, true));

                objComm = CreateAdapter(query.ToString(), conn, transaction);

                objComm.SelectCommand = ApplyParameters(
                    objComm.SelectCommand, conditionalFields, conditionalValues);

                #region Parameters

                bool isSummaryPaymentOrCollection = vm?.IsSummary == true
                    && (vm?.TransactionType == "Payment" || vm?.TransactionType == "Collection");

                // FromDate / ToDate
                if (vm?.TransactionType != "OutstandingBalance")
                {
                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        objComm.SelectCommand.Parameters.AddWithValue("@FromDate", DateTime.Parse(vm.FromDate));
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        objComm.SelectCommand.Parameters.AddWithValue("@ToDate", DateTime.Parse(vm.ToDate));
                }

                // BankId / BankAccountId
                if ((vm?.BankId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@BankId", vm.BankId);

                if ((vm?.BankAccountId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@BankAccountId", vm.BankAccountId);

                // Summary Payment/Collection specific
                if (isSummaryPaymentOrCollection)
                {
                    if (vm?.TransactionType == "Payment" && (vm?.SupplierId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", vm.SupplierId);

                    if (vm?.TransactionType == "Collection" && (vm?.CustomerId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId);
                }
                else
                {
                    // Details mode only
                    if ((vm?.TransactionId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@TransactionId", vm.TransactionId);

                    if ((vm?.BranchId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);

                    if ((vm?.DepositId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@DepositId", vm.DepositId);

                    if ((vm?.WithdrawalId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@WithdrawalId", vm.WithdrawalId);

                    if (vm?.TransactionType == "Collection" && (vm?.CustomerId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId);

                    if (vm?.TransactionType == "Payment" && (vm?.SupplierId ?? 0) > 0)
                        objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", vm.SupplierId);

                    // @TransactionType parameter শুধু Deposit/Withdrawal/All এর জন্য
                    if (vm?.TransactionType != "Statement"
                        && vm?.TransactionType != "OutstandingBalance"
                        && vm?.TransactionType != "Payment"
                        && vm?.TransactionType != "Collection")
                    {
                        objComm.SelectCommand.Parameters.AddWithValue("@TransactionType",
                            string.IsNullOrWhiteSpace(vm?.TransactionType)
                                ? (object)DBNull.Value
                                : vm.TransactionType);
                    }
                }

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
                    TransactionCount = dataTable.Columns.Contains("TransactionCount") && row["TransactionCount"] != DBNull.Value ? Convert.ToInt32(row["TransactionCount"]) : 0,
                    LastTransactionDate = dataTable.Columns.Contains("LastTransactionDate") && row["LastTransactionDate"] != DBNull.Value ? Convert.ToDateTime(row["LastTransactionDate"]).ToString("dd-MMM-yyyy") : "",
                    SupplierCode = dataTable.Columns.Contains("SupplierCode") ? row.Field<string>("SupplierCode") ?? "" : "",
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
                    SourceTable = dataTable.Columns.Contains("SourceTable") ? row.Field<string>("SourceTable") ?? "" : "",
                    PartyName = dataTable.Columns.Contains("PartyName") ? row.Field<string>("PartyName") ?? "" : "",
                    BankBranchName = dataTable.Columns.Contains("BranchName") ? row.Field<string>("BranchName") ?? "" : ""
                }).ToList();

                // Outstanding Balance — Amount itself is the balance
                if (vm?.TransactionType == "OutstandingBalance")
                {
                    foreach (var item in modelList)
                    {
                        item.RunningBalance = item.Amount;
                    }
                }

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


        public async Task<ResultVM> CustomerSaleCollectionReportList(
    CustomerSaleCollectionReportVM vm = null,
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

                if (vm?.IsSummary == true)
                {
                    // ── SUMMARY QUERY (Fixed using CTE & Unified Logic Streams) ──
                    query = new StringBuilder(@"
                WITH RawSales AS (
                    SELECT 
                        CustomerId, 
                        Id AS SaleId, 
                        GrandTotal,
                        InvoiceDateTime
                    FROM Sales 
                    WHERE IsPost = 1
                ),
                RawCollections AS (
                    SELECT 
                        CustomerId, 
                        Id AS CollId, 
                        TotalCollectAmount AS Amount, 
                        TransactionDate AS TxDate
                    FROM Collections 
                    WHERE IsActive = 1 AND IsArchive = 0
                    
                    UNION ALL
                    
                    SELECT 
                        S_CC.CustomerId, 
                        SCC.Id AS CollId, 
                        SCC.CardTotal AS Amount, 
                        CAST(S_CC.InvoiceDateTime AS DATE) AS TxDate
                    FROM SaleCreditCards SCC
                    INNER JOIN Sales S_CC ON SCC.SaleId = S_CC.Id
                    WHERE S_CC.IsPost = 1
                ),
                LatestCollection AS (
                    SELECT 
                        CustomerId,
                        TxDate,
                        Amount,
                        ROW_NUMBER() OVER (PARTITION BY CustomerId ORDER BY TxDate DESC, CollId DESC) AS RowSeq
                    FROM RawCollections
                )
                SELECT 
                    C.Id AS CustomerId,
                    C.Code AS CustomerCode,
                    C.Name AS CustomerName,
                    ISNULL(S.SaleCount, 0) AS SaleCount,
                    ISNULL(S.TotalSaleAmount, 0) AS TotalSaleAmount,
                    ISNULL(COL.CollectionCount, 0) AS CollectionCount,
                    ISNULL(COL.TotalCollectionAmount, 0) AS TotalCollectionAmount,
                    (ISNULL(S.TotalSaleAmount, 0) - ISNULL(COL.TotalCollectionAmount, 0)) AS OutstandingAmount,
                    LC.TxDate AS LastCollectionDate,
                    ISNULL(LC.Amount, 0) AS LastCollectionAmount
                FROM Customers C
                LEFT JOIN (
                    SELECT 
                        CustomerId,
                        COUNT(DISTINCT SaleId) AS SaleCount,
                        SUM(GrandTotal) AS TotalSaleAmount
                    FROM RawSales
                    WHERE 1=1
            ");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND InvoiceDateTime >= @FromDate");

                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND InvoiceDateTime <= @ToDate");

                    query.Append(@"
                    GROUP BY CustomerId
                ) S ON C.Id = S.CustomerId
                LEFT JOIN (
                    SELECT 
                        CustomerId,
                        COUNT(DISTINCT CollId) AS CollectionCount,
                        SUM(Amount) AS TotalCollectionAmount
                    FROM RawCollections
                    WHERE 1=1
            ");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND TxDate >= @FromDate");

                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND TxDate <= @ToDate");

                    query.Append(@"
                    GROUP BY CustomerId
                ) COL ON C.Id = COL.CustomerId
                LEFT JOIN LatestCollection LC ON C.Id = LC.CustomerId AND LC.RowSeq = 1
                WHERE 1=1
            ");

                    if ((vm?.CustomerId ?? 0) > 0)
                        query.Append(" AND C.Id = @CustomerId");

                    // Filter logic matching active parameters inside CTE timelines
                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND (S.CustomerId IS NOT NULL OR COL.CustomerId IS NOT NULL)");

                    query.Append(" ORDER BY C.Name ASC");
                }
                else
                {
                    // ── DETAILS QUERY (Working Cleanly) ──
                    query = new StringBuilder(@"
                SELECT * FROM (

                    -- Sales
                    SELECT
                        'Sale'                  AS TransactionType,
                        C.Id                    AS CustomerId,
                        C.Code                  AS CustomerCode,
                        C.Name                  AS CustomerName,
                        S.Id                    AS SaleId,
                        S.Code                  AS SaleCode,
                        CAST(S.InvoiceDateTime AS DATE) AS InvoiceDate,
                        S.GrandTotal            AS SaleAmount,
                        S.PaidAmount,
                        NULL                    AS CollectionId,
                        NULL                    AS CollectionCode,
                        NULL                    AS CollectionDate,
                        0                       AS CollectionAmount
                    FROM Sales S
                    INNER JOIN Customers C ON S.CustomerId = C.Id
                    WHERE S.IsPost = 1

                    UNION ALL

                    -- Standard Collections
                    SELECT
                        'Collection'            AS TransactionType,
                        C.Id                    AS CustomerId,
                        C.Code                  AS CustomerCode,
                        C.Name                  AS CustomerName,
                        NULL                    AS SaleId,
                        NULL                    AS SaleCode,
                        NULL                    AS InvoiceDate,
                        0                       AS SaleAmount,
                        0                       AS PaidAmount,
                        COL.Id                  AS CollectionId,
                        COL.Code                AS CollectionCode,
                        COL.TransactionDate     AS CollectionDate,
                        COL.TotalCollectAmount  AS CollectionAmount
                    FROM Collections COL
                    INNER JOIN Customers C ON COL.CustomerId = C.Id
                    WHERE COL.IsActive = 1 AND COL.IsArchive = 0

                    UNION ALL

                    -- Credit Card Payments
                    SELECT
                        'Collection'            AS TransactionType,
                        C.Id                    AS CustomerId,
                        C.Code                  AS CustomerCode,
                        C.Name                  AS CustomerName,
                        S3.Id                   AS SaleId,
                        S3.Code                 AS SaleCode,
                        NULL                    AS InvoiceDate,
                        0                       AS SaleAmount,
                        0                       AS PaidAmount,
                        SCC.Id                  AS CollectionId,
                        'CC-' + S3.Code         AS CollectionCode,
                        CAST(S3.InvoiceDateTime AS DATE) AS CollectionDate,
                        SCC.CardTotal           AS CollectionAmount
                    FROM SaleCreditCards SCC
                    INNER JOIN Sales S3      ON SCC.SaleId = S3.Id
                    INNER JOIN Customers C   ON S3.CustomerId = C.Id
                    WHERE S3.IsPost = 1

                ) AS ReportData
                WHERE 1=1
            ");

                    if ((vm?.CustomerId ?? 0) > 0)
                        query.Append(" AND CustomerId = @CustomerId");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND (InvoiceDate >= @FromDate OR CollectionDate >= @FromDate)");

                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND (InvoiceDate <= @ToDate OR CollectionDate <= @ToDate)");

                    query.Append(" ORDER BY CustomerName ASC, SaleId ASC, TransactionType ASC");
                }

                objComm = new SqlDataAdapter(query.ToString(), conn);
                if (transaction != null)
                    objComm.SelectCommand.Transaction = transaction;

                // Parameters Assignment
                if ((vm?.CustomerId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@CustomerId", vm.CustomerId);

                if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", DateTime.Parse(vm.FromDate));

                if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", DateTime.Parse(vm.ToDate));

                objComm.Fill(dataTable);

                List<CustomerSaleCollectionReportVM> modelList;

                if (vm?.IsSummary == true)
                {
                    modelList = dataTable.AsEnumerable().Select(row =>
                        new CustomerSaleCollectionReportVM
                        {
                            CustomerId = row.Field<int>("CustomerId"),
                            CustomerCode = row.Field<string>("CustomerCode") ?? "",
                            CustomerName = row.Field<string>("CustomerName") ?? "",
                            SaleCount = row.Field<int>("SaleCount"),
                            TotalSaleAmount = row.Field<decimal?>("TotalSaleAmount") ?? 0,
                            CollectionCount = row.Field<int>("CollectionCount"),
                            TotalCollectionAmount = row.Field<decimal?>("TotalCollectionAmount") ?? 0,
                            OutstandingAmount = row.Field<decimal?>("OutstandingAmount") ?? 0,
                            LastCollectionDate = row["LastCollectionDate"] != DBNull.Value
                                                        ? Convert.ToDateTime(row["LastCollectionDate"])
                                                                 .ToString("dd-MMM-yyyy")
                                                        : "",
                            LastCollectionAmount = row["LastCollectionAmount"] != DBNull.Value
                                                        ? Convert.ToDecimal(row["LastCollectionAmount"])
                                                        : 0
                        }).ToList();
                }
                else
                {
                    modelList = dataTable.AsEnumerable().Select(row =>
                        new CustomerSaleCollectionReportVM
                        {
                            TransactionType = row.Field<string>("TransactionType") ?? "",
                            CustomerId = row.Field<int>("CustomerId"),
                            CustomerCode = row.Field<string>("CustomerCode") ?? "",
                            CustomerName = row.Field<string>("CustomerName") ?? "",
                            SaleId = row.Field<int?>("SaleId") ?? 0,
                            SaleCode = row.Field<string>("SaleCode") ?? "",
                            InvoiceDate = row["InvoiceDate"] != DBNull.Value
                                                   ? Convert.ToDateTime(row["InvoiceDate"])
                                                            .ToString("dd-MMM-yyyy")
                                                   : "",
                            SaleAmount = row.Field<decimal?>("SaleAmount") ?? 0,
                            PaidAmount = row.Field<decimal?>("PaidAmount") ?? 0,
                            CollectionId = row.Field<int?>("CollectionId") ?? 0,
                            CollectionCode = row.Field<string>("CollectionCode") ?? "",
                            CollectionDate = row["CollectionDate"] != DBNull.Value
                                                   ? Convert.ToDateTime(row["CollectionDate"])
                                                            .ToString("dd-MMM-yyyy")
                                                   : "",
                            CollectionAmount = row.Field<decimal?>("CollectionAmount") ?? 0
                        }).ToList();
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

        public async Task<ResultVM> SupplierPurchasePaymentReportList(
    SupplierPurchasePaymentReportVM vm = null,
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

                if (vm?.IsSummary == true)
                {
                    query = new StringBuilder(@"
                SELECT
                    S.Id                                AS SupplierId,
                    S.Code                              AS SupplierCode,
                    S.Name                              AS SupplierName,
                    ISNULL(P.PurchaseCount, 0)          AS PurchaseCount,
                    ISNULL(P.TotalPurchaseAmount, 0)    AS TotalPurchaseAmount,
                    ISNULL(PAY.PaymentCount, 0)         AS PaymentCount,
                    ISNULL(PAY.TotalPaymentAmount, 0)   AS TotalPaymentAmount,
                    (ISNULL(P.TotalPurchaseAmount, 0) - ISNULL(PAY.TotalPaymentAmount, 0)) AS OutstandingAmount
                FROM Suppliers S
                LEFT JOIN (
                    SELECT
                        SupplierId,
                        COUNT(Id)       AS PurchaseCount,
                        SUM(GrandTotal) AS TotalPurchaseAmount
                    FROM Purchases
                    WHERE IsPost = 1
            ");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND PurchaseDate >= @FromDate");
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND PurchaseDate <= @ToDate");

                    query.Append(@"
                    GROUP BY SupplierId
                ) P ON S.Id = P.SupplierId
                LEFT JOIN (
                    SELECT
                        SupplierId,
                        COUNT(Id)               AS PaymentCount,
                        SUM(TotalPaymentAmount) AS TotalPaymentAmount
                    FROM Payments
                    WHERE IsActive = 1 AND IsArchive = 0
            ");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND TransactionDate >= @FromDate");
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND TransactionDate <= @ToDate");

                    query.Append(@"
                    GROUP BY SupplierId
                ) PAY ON S.Id = PAY.SupplierId
                WHERE (P.SupplierId IS NOT NULL OR PAY.SupplierId IS NOT NULL)
            ");

                    if ((vm?.SupplierId ?? 0) > 0)
                        query.Append(" AND S.Id = @SupplierId");

                    query.Append(" ORDER BY S.Name ASC");
                }
                else
                {
                    query = new StringBuilder(@"
                SELECT * FROM (

                    -- Purchases
                    SELECT
                        'Purchase'                      AS TransactionType,
                        S.Id                            AS SupplierId,
                        S.Code                          AS SupplierCode,
                        S.Name                          AS SupplierName,
                        PUR.Id                          AS PurchaseId,
                        PUR.Code                        AS PurchaseCode,
                        CAST(PUR.PurchaseDate AS DATE)  AS PurchaseDate,
                        PUR.GrandTotal                  AS PurchaseAmount,
                        NULL                            AS PaymentId,
                        NULL                            AS PaymentCode,
                        NULL                            AS PaymentDate,
                        0                               AS PaymentAmount
                    FROM Purchases PUR
                    INNER JOIN Suppliers S ON PUR.SupplierId = S.Id
                    WHERE PUR.IsPost = 1

                    UNION ALL

                    -- Payments
                    SELECT
                        'Payment'                       AS TransactionType,
                        S.Id                            AS SupplierId,
                        S.Code                          AS SupplierCode,
                        S.Name                          AS SupplierName,
                        NULL                            AS PurchaseId,
                        NULL                            AS PurchaseCode,
                        NULL                            AS PurchaseDate,
                        0                               AS PurchaseAmount,
                        PAY.Id                          AS PaymentId,
                        PAY.Code                        AS PaymentCode,
                        PAY.TransactionDate             AS PaymentDate,
                        PAY.TotalPaymentAmount          AS PaymentAmount
                    FROM Payments PAY
                    INNER JOIN Suppliers S ON PAY.SupplierId = S.Id
                    WHERE PAY.IsActive = 1 AND PAY.IsArchive = 0

                ) AS ReportData
                WHERE 1=1
            ");

                    if ((vm?.SupplierId ?? 0) > 0)
                        query.Append(" AND SupplierId = @SupplierId");

                    if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                        query.Append(" AND (PurchaseDate >= @FromDate OR PaymentDate >= @FromDate)");
                    if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                        query.Append(" AND (PurchaseDate <= @ToDate OR PaymentDate <= @ToDate)");

                    query.Append(" ORDER BY SupplierName ASC, PurchaseId ASC, TransactionType ASC");
                }

                objComm = new SqlDataAdapter(query.ToString(), conn);
                if (transaction != null)
                    objComm.SelectCommand.Transaction = transaction;

                if ((vm?.SupplierId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", vm.SupplierId);
                if (!string.IsNullOrWhiteSpace(vm?.FromDate))
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", DateTime.Parse(vm.FromDate));
                if (!string.IsNullOrWhiteSpace(vm?.ToDate))
                    objComm.SelectCommand.Parameters.AddWithValue("@ToDate", DateTime.Parse(vm.ToDate));

                objComm.Fill(dataTable);

                List<SupplierPurchasePaymentReportVM> modelList;

                if (vm?.IsSummary == true)
                {
                    modelList = dataTable.AsEnumerable().Select(row =>
                        new SupplierPurchasePaymentReportVM
                        {
                            SupplierIdResult = row.Field<int>("SupplierId"),
                            SupplierCode = row.Field<string>("SupplierCode") ?? "",
                            SupplierNameResult = row.Field<string>("SupplierName") ?? "",
                            PurchaseCount = row.Field<int>("PurchaseCount"),
                            TotalPurchaseAmount = row.Field<decimal?>("TotalPurchaseAmount") ?? 0,
                            PaymentCount = row.Field<int>("PaymentCount"),
                            TotalPaymentAmount = row.Field<decimal?>("TotalPaymentAmount") ?? 0,
                            OutstandingAmount = row.Field<decimal?>("OutstandingAmount") ?? 0
                        }).ToList();
                }
                else
                {
                    modelList = dataTable.AsEnumerable().Select(row =>
                        new SupplierPurchasePaymentReportVM
                        {
                            TransactionType = row.Field<string>("TransactionType") ?? "",
                            SupplierIdResult = row.Field<int>("SupplierId"),
                            SupplierCode = row.Field<string>("SupplierCode") ?? "",
                            SupplierNameResult = row.Field<string>("SupplierName") ?? "",
                            PurchaseId = row.Field<int?>("PurchaseId") ?? 0,
                            PurchaseCode = row.Field<string>("PurchaseCode") ?? "",
                            PurchaseDate = row["PurchaseDate"] != DBNull.Value
                                                    ? Convert.ToDateTime(row["PurchaseDate"]).ToString("dd-MMM-yyyy") : "",
                            PurchaseAmount = row.Field<decimal?>("PurchaseAmount") ?? 0,
                            PaymentId = row.Field<int?>("PaymentId") ?? 0,
                            PaymentCode = row.Field<string>("PaymentCode") ?? "",
                            PaymentDate = row["PaymentDate"] != DBNull.Value
                                                    ? Convert.ToDateTime(row["PaymentDate"]).ToString("dd-MMM-yyyy") : "",
                            PaymentAmount = row.Field<decimal?>("PaymentAmount") ?? 0
                        }).ToList();
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




        public async Task<ResultVM> GetSupplierPaymentDueList(
    string[] conditionalFields,
    string[] conditionalValues,
    SupplierPaymentDueVM vm = null,
    SqlConnection conn = null,
    SqlTransaction transaction = null)
        {
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
                StringBuilder query = new StringBuilder(@"
            SELECT
                S.Id                                             AS SupplierId,
                S.Code                                           AS SupplierCode,
                S.Name                                           AS SupplierName,
                ISNULL(SUM(PUR.GrandTotal), 0)                   AS PurchaseAmount,
                COUNT(DISTINCT PUR.Id)                           AS PurchaseCount,
                ISNULL(SUM(PAY.TotalPaymentAmount), 0)           AS TotalPaymentAmount,
                COUNT(DISTINCT PAY.Id)                           AS PaymentCount,
                ISNULL(MAX(PAY.TotalPaymentAmount), 0)           AS LastPaymentAmount,
                ISNULL(SUM(PUR.GrandTotal), 0)
                    - ISNULL(SUM(PAY.TotalPaymentAmount), 0)     AS DueAmount
            FROM Suppliers S
            LEFT JOIN Purchases PUR
                ON PUR.SupplierId = S.Id
                AND PUR.BranchId  = @BranchId
                AND PUR.IsPost    = 1
            LEFT JOIN Payments PAY
                ON PAY.SupplierId = S.Id
                AND PAY.BranchId  = @BranchId
                AND PAY.IsActive  = 1
                AND PAY.IsArchive = 0
            WHERE S.IsActive  = 1
                AND S.IsArchive = 0
                AND S.BranchId  = @BranchId
        ");

                if ((vm?.SupplierId ?? 0) > 0)
                    query.Append(" AND S.Id = @SupplierId");

                query = new StringBuilder(ApplyConditions(query.ToString(), conditionalFields, conditionalValues, false));


                query.Append(" GROUP BY S.Id, S.Code, S.Name");

                if ((vm?.SupplierId ?? 0) == 0)
                {
                    query.Append(@"
                HAVING (ISNULL(SUM(PUR.GrandTotal), 0)
                    - ISNULL(SUM(PAY.TotalPaymentAmount), 0)) != 0
            ");
                }

                query.Append(" ORDER BY S.Name ASC");


                SqlDataAdapter objComm = CreateAdapter(query.ToString(), conn, transaction);

                objComm.SelectCommand = ApplyParameters( objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);

                if ((vm?.SupplierId ?? 0) > 0)
                    objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", vm.SupplierId);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new SupplierPaymentDueVM
                {
                    SupplierId = row.Field<int>("SupplierId"),
                    SupplierCode = row.Field<string>("SupplierCode") ?? "",
                    SupplierName = row.Field<string>("SupplierName") ?? "",
                    PurchaseAmount = row.Field<decimal>("PurchaseAmount"),
                    PurchaseCount = row.Field<int>("PurchaseCount"),
                    TotalPaymentAmount = row.Field<decimal>("TotalPaymentAmount"),
                    PaymentCount = row.Field<int>("PaymentCount"),
                    LastPaymentAmount = row.Field<decimal>("LastPaymentAmount"),
                    DueAmount = row.Field<decimal>("DueAmount")
                }).ToList();

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
