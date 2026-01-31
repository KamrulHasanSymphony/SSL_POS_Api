using ShampanPOS.ViewModel;
using ShampanPOS.ViewModel.CommonVMs;
using ShampanPOS.ViewModel.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ShampanPOS.Repository
{

    public class CommonRepository
    {
        protected SqlConnection _context;
        protected SqlTransaction _transaction;

        #region
        public async Task<ResultVM> NextPrevious(string id, string status, string tableName, string type, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                int getId = 0;
                int branchId = 0;
                string sqlText = "";

                if (type.ToLower() == "transactional")
                {
                    sqlText = $@" SELECT ISNULL(BranchId,0) BranchId  FROM {tableName} WHERE Id=@Id ";

                    SqlDataAdapter command = CreateAdapter(sqlText, conn, transaction);
                    command.SelectCommand.Parameters.AddWithValue("@Id", id);

                    DataTable dt = new DataTable();
                    command.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        branchId = Convert.ToInt16(dt.Rows[0]["BranchId"]);
                    }
                }

                string getSqlText = "";
                if (status.ToLower() == "previous")
                {
                    getSqlText = $@" SELECT TOP 1 Id  FROM {tableName} WHERE 1=1 AND  Id<@Id ";

                    if (type.ToLower() == "transactional")
                    {
                        getSqlText += " AND BranchId=@BranchId ";
                    }
                }
                else if (status.ToLower() == "next")
                {
                    getSqlText = $@" SELECT TOP 1 Id  FROM {tableName} WHERE 1=1 AND  Id>@Id ";

                    if (type.ToLower() == "transactional")
                    {
                        getSqlText += " AND BranchId=@BranchId ";
                    }
                }
                if (status.ToLower() == "previous")
                {
                    getSqlText += " ORDER BY Id DESC ";
                }
                else if (status.ToLower() == "next")
                {
                    getSqlText += " ORDER BY Id ASC ";
                }

                SqlDataAdapter preCommand = CreateAdapter(getSqlText, conn, transaction);

                preCommand.SelectCommand.Parameters.AddWithValue("@Id", id);
                preCommand.SelectCommand.Parameters.AddWithValue("@BranchId", branchId);

                DataTable table = new DataTable();
                preCommand.Fill(table);

                if (table.Rows.Count > 0)
                {
                    getId = Convert.ToInt16(table.Rows[0]["Id"]);
                }

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Id = getId.ToString();
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public string GenerateCode(string CodeGroup, string CodeName, string EntryDate, int? branchId = 0, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                _context = conn;
                _transaction = transaction;

                string sqlText = "";

                string NewCode = "";
                string CodePreFix = "";
                string CodeGenerationFormat = "Branch/Number/Year";
                string CodeGenerationMonthYearFormat = "MMYY";
                string BranchCode = "001";
                string CurrentYear = "2020";
                string BranchNumber = "1";
                int CodeLength = 0;
                int nextNumber = 0;

                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataSet ds = new DataSet();

                DateTime TransactionDate = Convert.ToDateTime(EntryDate);
                string year = Convert.ToDateTime(DateTime.Now).ToString("yyyy");

                int BranchId = Convert.ToInt32(branchId);


                sqlText += " SELECT top 1  SettingName FROM Settings ";
                sqlText += " WHERE (SettingGroup ='" + CodeGenerationFormat + "') and   (SettingValue ='Y')  ";

                sqlText += " SELECT top 1  SettingName FROM Settings";
                sqlText += " WHERE  (SettingGroup ='" + CodeGenerationFormat + "') and   (SettingValue ='Y')  ";


                sqlText += " SELECT  top 1 Code FROM BranchProfiles ";
                sqlText += " WHERE  (Id ='" + BranchId + "')   ";

                sqlText += " SELECT   count(Code) BranchNumber FROM BranchProfiles where IsArchive=0 and IsActive=1 ";

                sqlText += "  SELECT * from  CodeGenerations where CurrentYear<='2020' ";

                sqlText += "  SELECT YEAR from FiscalYears where '" + Convert.ToDateTime(TransactionDate).ToString("yyyy/MM/dd") + "' between YearStart and YearEnd ";

                SqlCommand command = CreateCommand(sqlText);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(ds);


                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                    CodeGenerationFormat = ds.Tables[0].Rows[0][0].ToString();

                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    CodeGenerationMonthYearFormat = ds.Tables[1].Rows[0][0].ToString();
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                    BranchCode = ds.Tables[2].Rows[0][0].ToString();

                BranchNumber = BranchId.ToString();


                sqlText = "  ";
                sqlText += "  update CodeGenerations set CurrentYear ='2020'  where CurrentYear <='2020'   ";

                command = CreateCommand(sqlText);
                command.ExecuteNonQuery();

                if (ds.Tables[5] != null && ds.Tables[5].Rows.Count > 0)
                {
                    CurrentYear = ds.Tables[5].Rows[0][0].ToString();
                }
                else
                {
                    throw new ArgumentNullException("Fiscal year not set yet!");
                }

                sqlText = "  ";

                sqlText += " SELECT * FROM Codes";
                sqlText += " WHERE ( CodeGroup = @CodeGroup ) AND (CodeName = @CodeName) ";

                command.CommandText = sqlText;


                command.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                command.Parameters.AddWithValue("@CodeName", CodeName);

                dataAdapter = new SqlDataAdapter(command);


                dataAdapter.Fill(dt1);
                if (dt1 == null || dt1.Rows.Count <= 0)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    CodePreFix = dt1.Rows[0]["prefix"].ToString();
                    CodeLength = Convert.ToInt32(dt1.Rows[0]["Lenth"]);
                }

                sqlText = "  ";

                sqlText += @" 
SELECT top 1 
Id
,CurrentYear
,BranchId
,Prefix
,ISNULL(LastId,0) LastId
FROM CodeGenerations 
WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix order by LastId Desc
";


                command.CommandText = sqlText;


                command.Parameters.AddWithValue("@BranchId", BranchId);
                command.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                command.Parameters.AddWithValue("@Prefix", CodePreFix);


                dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt2);


                if (dt2 == null || dt2.Rows.Count <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        " INSERT INTO CodeGenerations(	CurrentYear,BranchId,Prefix,LastId)";
                    sqlText += " VALUES(";
                    sqlText += " @CurrentYear,";
                    sqlText += " @BranchId,";
                    sqlText += " @Prefix,";
                    sqlText += " 1";
                    sqlText += " )";

                    command.CommandText = sqlText;

                    object objfoundId1 = command.ExecuteNonQuery();

                    nextNumber = 1;
                }
                else
                {
                    if (nextNumber != 1)
                    {
                        nextNumber = dt2.Rows[0]["LastId"] == null ? 1 : Convert.ToInt32(dt2.Rows[0]["LastId"]) + 1;
                    }

                    sqlText = "  ";
                    sqlText += " update  CodeGenerations set LastId='" + nextNumber + "'";
                    sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix";


                    command.CommandText = sqlText;
                    command.ExecuteNonQuery();
                }

                NewCode = NextId(CodeGenerationMonthYearFormat, BranchNumber, CodeGenerationFormat, BranchCode.Trim(), CodeLength, nextNumber, CodePreFix, TransactionDate.ToString());
                return NewCode;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string NextId(string CodeGenerationMonthYearFormat, string BranchNumber, string CodeGenerationFormat, string BranchCode, int CodeLength
           , int nextNumber, string CodePreFix, string TransactionDate)
        {
            string NewCode = "";
            #region try
            try
            {
                CodeGenerationMonthYearFormat = CodeGenerationMonthYearFormat.Replace("Y", "y");
                if (Convert.ToInt32(BranchNumber) < 1)
                {
                    CodeGenerationFormat = CodeGenerationFormat.Replace("B/", "");
                }

                var my = Convert.ToDateTime(TransactionDate).ToString(CodeGenerationMonthYearFormat);
                var nextNumb = nextNumber.ToString().PadLeft(CodeLength, '0');
                CodeGenerationFormat = CodeGenerationFormat.Replace("Branch", BranchCode);
                CodeGenerationFormat = CodeGenerationFormat.Replace("Number", nextNumb);
                CodeGenerationFormat = CodeGenerationFormat.Replace("Year", my);

                NewCode = CodePreFix + "-" + CodeGenerationFormat;
            }

            #endregion
            catch (Exception ex)
            {
                throw ex;
            }

            return NewCode;
        }


        public string CodeGenerationNo(string codeGroup, string codeName, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                _context = conn;
                _transaction = transaction;

                string sqlText = "";

                string NewCode = "";
                string CodePreFix = "";
                int CodeLength = 0;
                int nextNumber = 0;

                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataSet ds = new DataSet();

                SqlCommand command = CreateCommand(sqlText);

                sqlText = "  ";

                sqlText += " SELECT * FROM Codes ";
                sqlText += " WHERE ( CodeGroup = @CodeGroup ) AND (CodeName = @CodeName) ";

                command.CommandText = sqlText;

                command.Parameters.AddWithValue("@CodeGroup", codeGroup);
                command.Parameters.AddWithValue("@CodeName", codeName);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt1);

                if (dt1 == null || dt1.Rows.Count <= 0)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    CodePreFix = dt1.Rows[0]["prefix"].ToString();
                    CodeLength = Convert.ToInt32(dt1.Rows[0]["Lenth"]);
                }

                sqlText = "  ";
                sqlText += @" 
SELECT top 1 
Id
,CurrentYear
,BranchId
,Prefix
,ISNULL(LastId,0) LastId
FROM CodeGenerations 
WHERE Prefix=@Prefix 
order by LastId Desc ";


                command.CommandText = sqlText;

                command.Parameters.AddWithValue("@Prefix", CodePreFix);

                dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt2);


                if (dt2 == null || dt2.Rows.Count <= 0)
                {
                    sqlText = "  ";
                    sqlText +=
                        " INSERT INTO CodeGenerations (CurrentYear,BranchId,Prefix,LastId)";
                    sqlText += " VALUES(";
                    sqlText += " FORMAT(GETDATE(),'yyyy'),";
                    sqlText += " 0,";
                    sqlText += " @Prefix,";
                    sqlText += " 1 ";
                    sqlText += " )";

                    command.CommandText = sqlText;

                    object objfoundId1 = command.ExecuteNonQuery();

                    nextNumber = 1;
                }
                else
                {
                    if (nextNumber != 1)
                    {
                        nextNumber = dt2.Rows[0]["LastId"] == null ? 1 : Convert.ToInt32(dt2.Rows[0]["LastId"]) + 1;
                    }

                    sqlText = "  ";
                    sqlText += " update  CodeGenerations set LastId='" + nextNumber + "'";
                    sqlText += " WHERE Prefix=@Prefix";


                    command.CommandText = sqlText;
                    command.ExecuteNonQuery();
                }

                NewCode = NextCodeGenerationNo(CodeLength, nextNumber, CodePreFix);
                return NewCode;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string NextCodeGenerationNo(int CodeLength, int nextNumber, string CodePreFix)
        {
            string NewCode = "0";

            try
            {
                string CodeGenerationFormat = string.Empty;
                var nextNumb = nextNumber.ToString().PadLeft(CodeLength, '0');
                NewCode = CodePreFix + "-" + nextNumb;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return NewCode;

        }

        protected SqlCommand CreateCommand(string query)
        {
            return new SqlCommand(query, _context, _transaction);
        }

        protected SqlCommand CreateCommand(string query, SqlConnection context, SqlTransaction transaction)
        {
            return new SqlCommand(query, context, transaction);
        }

        protected SqlDataAdapter CreateAdapter(string query)
        {
            var cmd = new SqlCommand(query, _context, _transaction);
            return new SqlDataAdapter(cmd);
        }

        protected SqlDataAdapter CreateAdapter(string query, SqlConnection context, SqlTransaction transaction)
        {
            var cmd = new SqlCommand(query, context, transaction);
            return new SqlDataAdapter(cmd);
        }

        protected SqlDataAdapter CreateAdapter(SqlCommand cmd)
        {
            return new SqlDataAdapter(cmd);
        }

        public string GetSettingsValue(string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                string sqlText = @" SELECT SettingValue FROM Settings WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand objComm = CreateCommand(sqlText);

                objComm = ApplyParameters(objComm, conditionalFields, conditionalValue);

                SqlDataAdapter adapter = new SqlDataAdapter(objComm);
                DataTable dt = new DataTable();

                adapter.Fill(dt);

                string settingValue = "2";

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    settingValue = row["SettingValue"].ToString();
                }
                return settingValue;
            }
            catch (Exception ex)
            {
                throw ex;
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

        public int GetCount(string tableName, string fieldName, string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                string sqlText = "SELECT COUNT (" + fieldName + ") TotalRecords FROM " + tableName + " WHERE 1=1  ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue, false);

                SqlCommand command = CreateCommand(sqlText);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                int totalRecords = Convert.ToInt32(command.ExecuteScalar());

                return totalRecords;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Delete(string tableName, string[] conditionalFields, string[] conditionalValue, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                string sqlText = " UPDATE   " + tableName + " SET IsArchive = 1 AND IsActive = 0 WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText, conn, transaction);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                int totalRecords = Convert.ToInt32(command.ExecuteNonQuery());

                return totalRecords;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<ResultVM> DeleteSaleData(string date, SqlConnection conn, SqlTransaction transaction)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string sqlText = @"
            DELETE FROM SaleDetails
            WHERE SaleId IN (
                SELECT s.Code FROM Sales s 
                WHERE s.DeliveryDate >= @Date AND s.DeliveryDate < DATEADD(d, 1, @Date)
            );
            DELETE FROM Sales
            WHERE DeliveryDate >= @Date AND DeliveryDate < DATEADD(d, 1, @Date);";

                using (SqlCommand command = new SqlCommand(sqlText, conn, transaction))
                {
                    DateTime parsedDate = DateTime.Parse(date);
                    command.Parameters.AddWithValue("@Date", parsedDate);

                    int totalRecords = await command.ExecuteNonQueryAsync();
                    result.Status = "Success";
                    result.Message = $"{totalRecords} records deleted.";
                    result.Id = totalRecords.ToString();
                    result.DataVM = totalRecords;
                }
            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.ToString();
            }
            return result;
        }

        public ResultVM DetailsDelete(string tableName, string[] conditionalFields, string[] conditionalValue, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, Id = "0", DataVM = null };
            try
            {
                string sqlText = " DELETE FROM " + tableName + "  WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText, conn, transaction);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                int totalRecords = Convert.ToInt32(command.ExecuteNonQuery());

                result.Status = "Success";
                result.Message = "Details data deleted.";
                result.Id = totalRecords.ToString();
                result.DataVM = null;

            }
            catch (Exception ex)
            {
                result.Id = "-1";
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
            }

            return result;
        }

        public bool CheckExists(string tableName, string[] conditionalFields, string[] conditionalValue, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                string sqlText = " SELECT COUNT(*)  FROM " + tableName + " WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText, conn, transaction);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                int totalRecords = Convert.ToInt32(command.ExecuteScalar());

                return totalRecords > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckIfProductReplaceCountsEqual(string code, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                string sqlText = @"
SELECT 
    CASE 
        WHEN 
            (SELECT COUNT(*) 
             FROM ProductReplaceReceiveDetails RRD
             WHERE RRD.ProductReplaceReceiveId = 
                 (SELECT TOP 1 Id FROM ProductReplaceReceive WHERE Code = @Code)
            ) 
            =
            (SELECT COUNT(*) 
             FROM ProductReplaceIssueDetails PRID
             WHERE PRID.ProductReplaceIssueId = 
                 (SELECT TOP 1 Id FROM ProductReplaceIssue WHERE ReceiveCode = @Code)
            )
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsEqual;
";

                SqlCommand command = CreateCommand(sqlText, conn, transaction);
                command.Parameters.AddWithValue("@Code", code);

                bool isEqual = Convert.ToBoolean(command.ExecuteScalar());
                return isEqual;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckPostStatus(string tableName, string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                bool ÌsPost = false;
                string Post = "";

                DataTable dt = new DataTable();

                string sqlText = " SELECT IsPost  FROM " + tableName + " WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Post = dt.Rows[0]["IsPost"].ToString();
                    if (!string.IsNullOrEmpty(Post) && Post.Trim().ToLower() == "y")
                    {
                        ÌsPost = true;
                    }
                }

                return ÌsPost;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckCompletedStatus(string tableName, string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                bool IsCompleted = false;
                string Completed = "";

                DataTable dt = new DataTable();

                string sqlText = " SELECT IsCompleted  FROM " + tableName + " WHERE 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Completed = dt.Rows[0]["IsCompleted"].ToString();
                    if (!string.IsNullOrEmpty(Completed) && Completed == "1")
                    {
                        IsCompleted = true;
                    }
                }

                return IsCompleted;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckPushStatus(string tableName, string[] conditionalFields, string[] conditionalValue)
        {
            try
            {
                bool ÌsPush = false;
                string Push = "";

                DataTable dt = new DataTable();

                string sqlText = "select IsPush  from " + tableName + " where 1=1 ";

                sqlText = ApplyConditions(sqlText, conditionalFields, conditionalValue);

                SqlCommand command = CreateCommand(sqlText);

                command = ApplyParameters(command, conditionalFields, conditionalValue);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    Push = dt.Rows[0]["IsPush"].ToString();
                    if (!string.IsNullOrEmpty(Push) && Push.Trim().ToLower() == "y")
                    {
                        ÌsPush = true;
                    }
                }

                return ÌsPush;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }

                list.Add(dict);
            }

            return list;
        }

        #endregion

        public async Task<ResultVM> SettingsValue(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT ISNULL(SettingValue,'') SettingValue FROM [Settings]
WHERE 1 = 1 ";

                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Value = dataTable.Rows.Count > 0 ? dataTable.Rows[0]["SettingValue"].ToString() : "0";
                result.DataVM = dataTable;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> EnumList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT [Id]
      ,[Name]
 FROM [EnumTypes]
WHERE 
    1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new EnumTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),


                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> ParentAreaList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"

                 Select
                 s.Id,
                 S.Code,
                 s.Name
                 from Areas s

                 Where IsActive = 1   ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new AreaVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),


                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> ProductList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                         SELECT 
                         ISNULL(H.Id, 0) AS Id,
                         ISNULL(H.Code, '') AS Code,               
                         ISNULL(H.Name, '') AS Name,
                         ISNULL(H.Description, '') AS Description
                         FROM Products H
                         WHERE IsArchive != 1 AND IsActive = 1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> ProductGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
						 SELECT 
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,               
                        ISNULL(H.Name, '') AS Name
                        FROM ProductGroups H
                        WHERE IsArchive != 1 AND IsActive = 1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductGroupVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> MasterItemGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
						 SELECT 
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,               
                        ISNULL(H.Name, '') AS Name
                        FROM MasterItemGroup H
                        WHERE IsArchive != 1 AND IsActive = 1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new MasterItemGroupVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> UOMList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
						 SELECT 
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,               
                        ISNULL(H.Name, '') AS Name
                        FROM UOMs H
                        WHERE IsArchive != 1 AND IsActive = 1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new UOMVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> EnumTypeList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                                SELECT

			                    ISNULL(H.Id,0)	 EnumTypeId,
			                    ISNULL(H.Name,'') EnumName	
		
			                    FROM EnumTypes H  where EnumType='location'";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new EnumTypeVM
                {
                    Id = Convert.ToInt32(row["EnumTypeId"]),
                    Name = row["EnumName"]?.ToString()

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> CustomerList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                string sqlQuery = @"
                SELECT DISTINCT

                 ISNULL(H.Id, 0) Id
                ,ISNULL(H.Code, '') Code
                ,ISNULL(H.Name, '') Name
                ,ISNULL(H.Address, '') Address
                ,ISNULL(H.Email, '') Email
                ,ISNULL(H.BanglaName, '') BanglaName
                ,ISNULL(H.Comments, '') Comments


                ,CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive'   END Status

                FROM Customers H

                WHERE H.IsActive = 1

                             ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                    Address = row["Address"]?.ToString(),
                    Email = row["Email"]?.ToString(),
                    BanglaName = row["BanglaName"]?.ToString(),
                    Comments = row["Comments"]?.ToString(),
                    //BranchId = Convert.ToInt32(row["BranchId"]),


                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> CustomerGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
						 SELECT 
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,               
                        ISNULL(H.Name, '') AS Name
                        FROM CustomerGroups H
                        WHERE IsArchive != 1 AND IsActive = 1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerGroupVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> SupplierList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                string sqlQuery = @"
                 Select
                 ISNULL(H.Id,0)	Id,
                 ISNULL(H.Name,'') Name,              
                 ISNULL(H.Code,'') Code
                 FROM Suppliers H
                 WHERE H.IsActive =1 ";
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);
                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
                objComm.Fill(dataTable);
                var modelList = dataTable.AsEnumerable().Select(row => new SupplierVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString()
                }).ToList();
                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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
        public async Task<ResultVM> SupplierGroupList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
						 Select
                         ISNULL(H.Id,0)	Id,
                         ISNULL(H.Name,'') Name,
                         ISNULL(H.Code,'') Code,
                         ISNULL(H.IsActive, 0) AS IsActive,
                         CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive' END AS Status

                        FROM SupplierGroups H
                        WHERE H.IsActive = 1 And H.IsArchive != 1  ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new CustomerVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


//        public async Task<ResultVM> GetProductModalForSaleCountData(
//    string[] conditionalFields,
//    string[] conditionalValues,
//    PeramModel vm = null,
//    SqlConnection conn = null,
//    SqlTransaction transaction = null)
//        {
//            bool isNewConnection = false;
//            DataTable dataTable = new DataTable();
//            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

//            try
//            {
//                if (conn == null)
//                {
//                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
//                    conn.Open();
//                    isNewConnection = true;
//                }

//                // ✅ Get Customer Category
//                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
//                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
//                object res = checkCommand.ExecuteScalar();
//                string customerCategory = res != null ? res.ToString() : "0";

//                // ✅ Main Count Query (Removed @FromDate)
//                string query = @"
//SELECT
//    COALESCE(COUNT(P.Id), 0) AS FilteredCount
//FROM Products P
//LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
//LEFT OUTER JOIN ProductSalePriceBatchHistories PBH ON P.Id = PBH.ProductId
//LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
//WHERE PBH.PriceCategory = @PriceCategory 
//  AND PBH.BranchId = @BranchId 
//  AND P.IsActive = 1
//";

//                // ✅ Apply dynamic conditions
//                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

//                // ✅ Create adapter and apply parameters
//                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
//                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

//                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
//                }
//                if (customerCategory != null)
//                {
//                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
//                }

//                objComm.Fill(dataTable);

//                // ✅ Return result
//                result.Status = "Success";
//                result.Message = "Data retrieved successfully.";
//                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.ExMessage = ex.Message;
//                result.Message = ex.Message;
//                return result;
//            }
//            finally
//            {
//                if (isNewConnection && conn != null)
//                {
//                    conn.Close();
//                }
//            }
//        }

        public async Task<ResultVM> GetProductModalForPurchaseCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string query = @"
SELECT

COALESCE(COUNT(P.Id), 0) AS FilteredCount

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

WHERE P.IsActive = 1  
";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> BulkInsert(string tableName, DataTable dt, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            var isNewConnection = false;

            try
            {
                #region Connection Management
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                #endregion

                #region Bulk Insert
                using (var sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    sqlBulkCopy.BulkCopyTimeout = 0;
                    sqlBulkCopy.DestinationTableName = tableName;

                    // Configure notification if needed
                    SqlRowsCopiedEventHandler rowsCopiedCallBack = null;
                    if (rowsCopiedCallBack != null)
                    {
                        sqlBulkCopy.NotifyAfter = 500;
                        sqlBulkCopy.SqlRowsCopied += rowsCopiedCallBack;
                    }

                    // Set column mappings
                    foreach (DataColumn column in dt.Columns)
                    {
                        sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    // Perform bulk insert
                    await sqlBulkCopy.WriteToServerAsync(dt);
                }
                #endregion
                result.Status = "Success";
                result.Message = "Data Imported Successfully";
                result.DataVM = dt;
            }
            catch (Exception ex)
            {

                result.Status = "Fail";
                result.ExMessage = ex.Message;
                result.Message = ex.Message.ToString();
                return result;
            }
            finally
            {
                #region Connection Cleanup
                if (isNewConnection && conn != null)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        await conn.CloseAsync();
                    }
                }


                #endregion
            }
            return result;
        }

        public (bool exists, int campaignId) CampaignExists(DateTime invoiceDate, int branchId, int enumTypeId, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            try
            {
                string sqlText = @"
               SELECT TOP 1 c.id 
            FROM Campaigns c
            WHERE c.IsPost = 1 
            AND c.IsActive = 1
            AND c.BranchId =@BranchId
            AND @InvoiceDate BETWEEN c.CampaignStartDate and c.CampaignEndDate
            AND c.EnumTypeId = @EnumTypeId";

                using (var command = CreateCommand(sqlText, conn, transaction))
                {
                    // Add parameters
                    command.Parameters.AddWithValue("@InvoiceDate", invoiceDate);
                    command.Parameters.AddWithValue("@BranchId", branchId);
                    command.Parameters.AddWithValue("@EnumTypeId", enumTypeId);

                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int campaignId = Convert.ToInt32(result);
                        return (true, campaignId); // Campaign exists, return Id
                    }

                    return (false, 0); // No campaign found
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public async Task<ResultVM> HasDayEndData(string branchId, SqlConnection conn = null, SqlTransaction transaction = null)
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
                    await conn.OpenAsync();
                    isNewConnection = true;
                }

                string query = @"
        SELECT U.DateValue
        FROM
        (
            SELECT DISTINCT CAST(SD.DeliveryDate AS DATE) AS DateValue
            FROM SaleDeleveries SD
            LEFT JOIN BranchProfiles B ON SD.BranchId = B.Id
            WHERE B.Code = @BranchId

            UNION ALL

            SELECT DISTINCT CAST(SR.DeliveryDate AS DATE) AS DateValue
            FROM SaleDeleveryReturns SR
            LEFT JOIN BranchProfiles B ON SR.BranchId = B.Id
            WHERE B.Code = @BranchId

            UNION ALL

            SELECT DISTINCT CAST(P.PurchaseDate AS DATE) AS DateValue
            FROM Purchases P
            LEFT JOIN BranchProfiles B ON P.BranchId = B.Id
            WHERE B.Code = @BranchId

            UNION ALL

            SELECT DISTINCT CAST(PR.PurchaseReturnDate AS DATE) AS DateValue
            FROM PurchaseReturns PR
            LEFT JOIN BranchProfiles B ON PR.BranchId = B.Id
            WHERE B.Code = @BranchId
        ) U
        LEFT JOIN DayEndHeaders D
            ON CAST(D.[Date] AS DATE) = U.DateValue
        WHERE D.[Date] IS NULL
        ORDER BY U.DateValue";

                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Transaction = transaction;
                    adapter.SelectCommand.Parameters.AddWithValue("@BranchId", branchId);
                    adapter.Fill(dataTable);
                }

                // ✅ Return date as formatted string
                var modelList = dataTable.AsEnumerable()
                .Select(row => new
                {
                    DateValue = Convert.ToDateTime(row["DateValue"]).ToString("dd-MM-yyyy") // capital D
                })
                .ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> ParentBranchProfileList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"

            SELECT '' Code,0 Id,'All' Name,'' Email
            UNION ALL
			SELECT DISTINCT
			 ISNULL(H.Code,'') Code
			,ISNULL(H.Id,0) Id
			,ISNULL(H.Name,'') Name
		    ,ISNULL(H.Email,'') Email
			
		
			FROM BranchProfiles H

			WHERE H.IsActive=1 ";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new BranchProfileVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                    Email = row["Email"]?.ToString(),


                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> GetProductModalForPurchaseData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 
ISNULL(P.Id,0)ProductId , 
ISNULL(P.Name,'') ProductName,
ISNULL(P.BanglaName,'') BanglaName, 
ISNULL(P.Code,'') ProductCode, 
ISNULL(P.HSCodeNo,'') HSCodeNo,
ISNULL(P.ProductGroupId,0) ProductGroupId,
ISNULL(PG.Name,'') ProductGroupName,
ISNULL(P.UOMId,0) UOMId,
ISNULL(UOM.Name,'') UOMName,
ISNULL(P.PurchasePrice,0) PurchasePrice,
ISNULL(P.SalePrice,0) SalesPrice,
CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END Status,
ISNULL(P.SDRate,0) SDRate,
ISNULL(P.VATRate,0) VATRate

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs uom ON P.UOMId = uom.Id

WHERE P.IsActive = 1 
";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM 
                {
                    ProductId = row.Field<int>("ProductId"),
                    ProductName = row.Field<string>("ProductName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    UOMName = row.Field<string>("UOMName"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice"),
                    SDRate = row.Field<decimal>("SDRate"),
                    VATRate = row.Field<decimal>("VATRate"),
                    Status = row.Field<string>("Status")

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> GetProductModalForSaleCountData(
    string[] conditionalFields,
    string[] conditionalValues,
    PeramModel vm = null,
    SqlConnection conn = null,
    SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                // ✅ Get Customer Category
                SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
                checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
                object res = checkCommand.ExecuteScalar();
                string customerCategory = res != null ? res.ToString() : "0";

                // ✅ Main Count Query (Removed @FromDate)
                string query = @"
SELECT
    COALESCE(COUNT(P.Id), 0) AS FilteredCount
FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
WHERE P.IsActive = 1
";

                // ✅ Apply dynamic conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                // ✅ Create adapter and apply parameters
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (customerCategory != null)
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
                }

                objComm.Fill(dataTable);

                // ✅ Return result
                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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




        public async Task<ResultVM> GetProductModalForSaleData(
            string[] conditionalFields,
            string[] conditionalValues,
            PeramModel vm = null,
            SqlConnection conn = null,
            SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                // ✅ Get Customer Category
                //SqlCommand checkCommand = new SqlCommand("SELECT CustomerCategory FROM Customers WHERE Id=@Id", conn, transaction);
                //checkCommand.Parameters.Add("@Id", SqlDbType.BigInt).Value = vm.CustomerId;
                //object res = checkCommand.ExecuteScalar();
                //string customerCategory = res != null ? res.ToString() : "0";

                // ✅ Main Query (All @FromDate Removed)
                string query = @"
SELECT 
    ISNULL(P.Id,0) AS ProductId, 
    ISNULL(P.Name,'') AS ProductName,
    ISNULL(P.BanglaName,'') AS BanglaName, 
    ISNULL(P.Code,'') AS ProductCode, 
    ISNULL(P.HSCodeNo,'') AS HSCodeNo,ISNULL(P.ImagePath, '') AS ImagePath,
    CASE  WHEN P.ImagePath IS NULL OR P.ImagePath = '' THEN '/Content/Products/No_Image_Available.jpg' ELSE P.ImagePath END AS ImagePathImage,
    ISNULL(P.ProductGroupId, 0) AS ProductGroupId,
    ISNULL(PG.Name,'') AS ProductGroupName,
    0 AS UOMId,
    ISNULL(UOM.Name,'') AS UOMName,
    CASE WHEN P.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status,
    ISNULL(P.SDRate,0) SDRate,
	ISNULL(P.VATRate,0) VATRate,
    ISNULL(P.PurchasePrice,0) PurchasePrice,
    ISNULL(P.SalePrice,0) SalesPrice

FROM Products P
LEFT OUTER JOIN ProductGroups PG ON P.ProductGroupId = PG.Id
LEFT OUTER JOIN UOMs UOM ON P.UOMId = UOM.Id
";

                // ✅ Apply additional filters (dynamic)
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @" ORDER BY " + vm.OrderName + " " + vm.orderDir;
                query += @" OFFSET " + vm.startRec + " ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                // ✅ Prepare and execute
                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }
                //if (customerCategory != null)
                //{
                //    objComm.SelectCommand.Parameters.AddWithValue("@PriceCategory", customerCategory);
                //}

                objComm.Fill(dataTable);

                string setting = @"
                    SELECT 
                    ISNULL(SettingValue, 'http://103.231.239.121:8012/') AS SettingValue
                    FROM Settings
                    WHERE SettingGroup = 'DMSUIUrl'
                ";

                string apiBaseUrl = "";

                using (SqlCommand cmdSetting = new SqlCommand(setting, conn, transaction))
                {
                    var value = cmdSetting.ExecuteScalar();
                    apiBaseUrl = value?.ToString() ?? "";
                }

                // ✅ Map Data
                var modelList = dataTable.AsEnumerable().Select(row => new ProductDataVM
                {
                    ProductId = row.Field<int>("ProductId"),
                    ProductName = row.Field<string>("ProductName"),
                    BanglaName = row.Field<string>("BanglaName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    HSCodeNo = row.Field<string>("HSCodeNo"),
                    ImagePath = row.Field<string>("ImagePath"),
                    ImagePathImage = apiBaseUrl + (row.Field<string>("ImagePathImage") ?? "http://103.231.239.121:8012/Content/Products/No_Image_Available.jpg"),
                    ProductGroupId = row.Field<int>("ProductGroupId"),
                    ProductGroupName = row.Field<string>("ProductGroupName"),
                    UOMId = row.Field<int>("UOMId"),
                    UOMName = row.Field<string>("UOMName"),
                    Status = row.Field<string>("Status"),
                    SDRate = row.Field<decimal>("SDRate"),
                    VATRate = row.Field<decimal>("VATRate"),
                    PurchasePrice = row.Field<decimal>("PurchasePrice"),
                    SalesPrice = row.Field<decimal>("SalesPrice")
                }).ToList();


                // ✅ Return Success
                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> SaleOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string sqlQuery = @"
                      SELECT DISTINCT
                        ISNULL(H.Id, 0) AS Id,
                        ISNULL(H.Code, '') AS Code,
                        ISNULL(H.BranchId, 1) AS BranchId,
                        ISNULL(H.CustomerId, '') AS CustomerId,
			            ISNULL(C.Name, '')AS CustomerName
                    FROM SaleOrders H
                    LEFT OUTER JOIN Customers C ON H.CustomerId= C.Id
                ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new SaleOrderVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CustomerName = row["CustomerName"]?.ToString(),
                    Code = row["Code"]?.ToString(),
                    BranchId = Convert.ToInt32(row["BranchId"]?.ToString())

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> BankIdList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
                SELECT
                    ISNULL(M.Id, 0) AS Id,
                    ISNULL(M.Code, '') AS Code,
                    ISNULL(M.Name, '') AS Name,
                    ISNULL(M.BanglaName, '') AS BanglaName,
                    ISNULL(M.Address, '') AS Address

                FROM BankInformations M
                WHERE 1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new BankInformationVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"]?.ToString(),
                    BanglaName = row["BanglaName"]?.ToString(),
                    Address = row["Address"]?.ToString(),
                    Code = row["Code"]?.ToString(),



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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



        public async Task<ResultVM> GetPurchaseOrderIdData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 

    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(S.Address, '') AS SupplierAddress,
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,
    ISNULL(M.Comments, '') AS Comments
    
FROM 
    PurchaseOrders M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id

WHERE  1 = 1


";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseOrderVM
                {
                    Id = row.Field<int>("Id"),
                    SupplierId = row.Field<int>("SupplierId"),
                    Code = row.Field<string>("Code"),
                    SupplierName = row.Field<string>("SupplierName"),
                    SupplierAddress = row.Field<string>("SupplierAddress"),
                    Comments = row.Field<string>("Comments"),
                    DeliveryDateTime = row["DeliveryDateTime"].ToString(),
                    OrderDate = row["OrderDate"].ToString(),

                    Status = row.Field<string>("Status")

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> GetPurchaseOrderCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string query = @"
SELECT

COALESCE(COUNT(M.Id), 0) AS FilteredCount

FROM 
    PurchaseOrders M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id

WHERE 1 = 1   
";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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

        public async Task<ResultVM> PurchaseOrderList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT 

    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(S.Address, '') AS SupplierAddress,
    ISNULL(FORMAT(M.OrderDate, 'yyyy-MM-dd'), '1900-01-01') AS OrderDate,
    ISNULL(FORMAT(M.DeliveryDateTime, 'yyyy-MM-dd'), '1900-01-01') AS DeliveryDateTime,
    ISNULL(M.Comments, '') AS Comments
    
FROM 
    PurchaseOrders M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id

WHERE  1 = 1";


                query = ApplyConditions(query, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseOrderVM
                {
                    Id = row.Field<int>("Id"),
                    SupplierId = row.Field<int>("SupplierId"),
                    Code = row.Field<string>("Code"),
                    SupplierName = row.Field<string>("SupplierName"),
                    SupplierAddress = row.Field<string>("SupplierAddress"),
                    Comments = row.Field<string>("Comments"),
                    DeliveryDateTime = row["DeliveryDateTime"].ToString(),
                    OrderDate = row["OrderDate"].ToString(),

                    Status = row.Field<string>("Status")



                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> BankAccountList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                string sqlQuery = @"
            SELECT
                ISNULL(M.Id, 0) AS Id,
	            ISNULL(M.BankId, 0) AS BankId,
                ISNULL(M.AccountName, '') AS AccountName,
                ISNULL(M.BranchName, '') AS BranchName,
                ISNULL(M.AccountNo, '') AS AccountNo,
                ISNULL(M.Comments, '') AS Comments


            FROM BankAccounts M
            WHERE 1 = 1 ";
                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);
                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);
                objComm.Fill(dataTable);
                var modelList = dataTable.AsEnumerable().Select(row => new BankAccountVM
                {
                    Id = row.Field<int>("Id"),
                    BankId = row.Field<int>("BankId"),
                    AccountName = row.Field<string>("AccountName"),
                    BranchName = row.Field<string>("BranchName"),
                    AccountNo = row.Field<string>("AccountNo"),
                    Comments = row.Field<string>("Comments")
                }).ToList();
                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> GetPurchaseData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(M.PurchaseOrderId, 0) AS PurchaseOrderId,
    ISNULL(E.Code, '') AS PurchaseOrderCode,
    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.GrandTotal, 0) AS GrandTotal  
FROM 
    Purchases M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId, 0) = S.Id
LEFT OUTER JOIN PurchaseOrders E ON ISNULL(M.PurchaseOrderId, 0) = E.Id
WHERE 1 = 1


";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseDataVM
                {
                    Id = row.Field<int>("Id"),
                    SupplierId = row.Field<int>("SupplierId"),
                    PurchaseOrderId = row.Field<int>("PurchaseOrderId"),
                    Code = row.Field<string>("Code"),
                    PurchaseOrderCode = row.Field<string>("PurchaseOrderCode"),
                    SupplierName = row.Field<string>("SupplierName"),
                    GrandTotal = row.Field<decimal>("GrandTotal"),
                    Comments = row.Field<string>("Comments")

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> GetPurchaseCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string query = @"
SELECT

COALESCE(COUNT(M.Id), 0) AS FilteredCount

FROM 
    Purchases M
LEFT OUTER JOIN Suppliers S ON ISNULL(M.SupplierId,0) = S.Id
LEFT OUTER JOIN PurchaseOrders E ON ISNULL(M.PurchaseOrderId,0) = E.Id

WHERE 1 = 1
";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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






        public async Task<ResultVM> GetSaleData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }

                string query = @"
SELECT
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.CustomerId, 0) AS CustomerId,
    ISNULL(M.SaleOrderId, 0) AS SaleOrderId,
    ISNULL(C.Code, '') AS SaleOrderCode,
    ISNULL(S.Name, '') AS CustomerName,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.GrandTotal, 0) AS GrandTotal  
FROM 
    Sales M
LEFT OUTER JOIN Customers S ON ISNULL(M.CustomerId, 0) = S.Id
LEFT OUTER JOIN SaleOrders C ON ISNULL(M.SaleOrderId, 0) = C.Id
WHERE 1 = 1


";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);
                query += @"  ORDER BY " + vm.OrderName + "  " + vm.orderDir;
                query += @" OFFSET  " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + " ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new SaleDataVM
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    SaleOrderId = row.Field<int>("SaleOrderId"),
                    Code = row.Field<string>("Code"),
                    SaleOrderCode = row.Field<string>("SaleOrderCode"),
                    CustomerName = row.Field<string>("CustomerName"),
                    GrandTotal = row.Field<decimal>("GrandTotal"),
                    Comments = row.Field<string>("Comments")

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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


        public async Task<ResultVM> GetSaleCountData(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            bool isNewConnection = false;
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(DatabaseHelper.GetConnectionString());
                    conn.Open();
                    isNewConnection = true;
                }
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }
                string query = @"
SELECT

COALESCE(COUNT(M.Id), 0) AS FilteredCount

FROM 
    Sales M
LEFT OUTER JOIN Customers S ON ISNULL(M.CustomerId, 0) = S.Id
LEFT OUTER JOIN SaleOrders C ON ISNULL(M.SaleOrderId, 0) = C.Id
WHERE 1 = 1
";

                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query = query.Replace("@param", " PBH.BranchId = @BranchId AND CAST(PBH.EffectDate AS DATE) <= " + "@FromDate" + " AND ");
                }
                else
                {
                    query = query.Replace("@param", "");
                }

                // Apply additional conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);
                return result;
            }
            catch (Exception ex)
            {
                result.ExMessage = ex.Message;
                result.Message = ex.Message;
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





        public async Task<ResultVM> GetPurchaseDatabysupplier(
            string[] conditionalFields,
            string[] conditionalValues,
            PeramModel vm,
            SqlConnection conn,
            SqlTransaction transaction)
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

                string query = @"
SELECT
    ISNULL(M.Id, 0) AS Id,
    ISNULL(M.Code, '') AS Code,
    ISNULL(M.SupplierId, 0) AS SupplierId,
    ISNULL(M.PurchaseOrderId, 0) AS PurchaseOrderId,
    ISNULL(E.Code, '') AS PurchaseOrderCode,
    ISNULL(S.Name, '') AS SupplierName,
    ISNULL(M.Comments, '') AS Comments,
    ISNULL(M.GrandTotal, 0) AS GrandTotal
FROM Purchases M
LEFT JOIN Suppliers S ON M.SupplierId = S.Id
LEFT JOIN PurchaseOrders E ON M.PurchaseOrderId = E.Id
WHERE 1 = 1
AND (@SupplierId = 0 OR M.SupplierId = @SupplierId)
";

                // Date filter
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query += @" AND CAST(M.EffectDate AS DATE) <= @FromDate ";
                }

                // Apply dynamic search / conditions
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                // ---------- FIX 1: SAFE ORDER BY ----------
                string orderName = "M.Id";
                string orderDir = "ASC";

                if (!string.IsNullOrWhiteSpace(vm?.OrderName))
                {
                    orderName = vm.OrderName;
                }

                if (!string.IsNullOrWhiteSpace(vm?.orderDir))
                {
                    orderDir = vm.orderDir;
                }

                query += @" ORDER BY " + orderName + " " + orderDir;
                query += @" OFFSET " + vm.startRec + @" ROWS FETCH NEXT " + vm.pageSize + @" ROWS ONLY";

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // Apply dynamic parameters
                objComm.SelectCommand =
                    ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                // ---------- FIX 2: CORRECT SupplierId ----------
                int supplierId = 0;
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    int.TryParse(vm.Id, out supplierId);
                }

                objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", supplierId);

                // Branch
                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }

                // Date
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                // Execute
                objComm.Fill(dataTable);

                // Map result
                var modelList = dataTable.AsEnumerable().Select(row => new PurchaseDataVM
                {
                    Id = row.Field<int>("Id"),
                    SupplierId = row.Field<int>("SupplierId"),
                    PurchaseOrderId = row.Field<int>("PurchaseOrderId"),
                    Code = row.Field<string>("Code"),
                    PurchaseOrderCode = row.Field<string>("PurchaseOrderCode"),
                    SupplierName = row.Field<string>("SupplierName"),
                    GrandTotal = row.Field<decimal>("GrandTotal"),
                    Comments = row.Field<string>("Comments")
                }).ToList();

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
                result.Count = modelList.Count;

                return result;
            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
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






        public async Task<ResultVM> GetPurchasebysupplierCountData(
            string[] conditionalFields,
            string[] conditionalValues,
            PeramModel vm,
            SqlConnection conn,
            SqlTransaction transaction)
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

                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                }

                string query = @"
SELECT
    COALESCE(COUNT(M.Id), 0) AS FilteredCount
FROM Purchases M
LEFT JOIN Suppliers S ON M.SupplierId = S.Id
LEFT JOIN PurchaseOrders E ON M.PurchaseOrderId = E.Id
WHERE 1 = 1
AND (@SupplierId = 0 OR M.SupplierId = @SupplierId)
";

                // Date filter (if required)
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    query += @" AND CAST(M.EffectDate AS DATE) <= @FromDate ";
                }

                // Apply dynamic conditions (search, etc.)
                query = ApplyConditions(query, conditionalFields, conditionalValues, true);

                SqlDataAdapter objComm = CreateAdapter(query, conn, transaction);

                // Apply dynamic parameters
                objComm.SelectCommand =
                    ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);

                // 🔥 SupplierId parameter (MAIN FIX)
                int supplierId = 0;
                if (vm != null && !string.IsNullOrEmpty(vm.Id))
                {
                    int.TryParse(vm.Id, out supplierId);
                }
                objComm.SelectCommand.Parameters.AddWithValue("@SupplierId", supplierId);

                // Branch
                if (vm != null && !string.IsNullOrEmpty(vm.BranchId))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@BranchId", vm.BranchId);
                }

                // Date
                if (vm != null && !string.IsNullOrEmpty(vm.FromDate))
                {
                    objComm.SelectCommand.Parameters.AddWithValue("@FromDate", vm.FromDate);
                }

                objComm.Fill(dataTable);

                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.Count = Convert.ToInt32(dataTable.Rows[0][0]);

                return result;
            }
            catch (Exception ex)
            {
                result.Status = "Fail";
                result.Message = ex.Message;
                result.ExMessage = ex.Message;
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

        public async Task<ResultVM> GetItemList(string[] conditionalFields, string[] conditionalValues, PeramModel vm = null, SqlConnection conn = null, SqlTransaction transaction = null)
        {
            DataTable dataTable = new DataTable();
            ResultVM result = new ResultVM { Status = "Fail", Message = "Error", ExMessage = null, DataVM = null };

            try
            {
                if (conn == null)
                {
                    throw new Exception("Database connection fail!");
                }
                string sqlQuery = @"
	         SELECT DISTINCT 

             ISNULL(H.Id, 0) Id
            ,ISNULL(H.Code, '') Code 
            ,ISNULL(H.Name, '') Name 
            ,ISNULL(H.IsActive, 0) IsActive
            ,ISNULL(H.IsArchive, 0) IsArchive
            ,CASE WHEN ISNULL(H.IsActive, 0) = 1 THEN 'Active' ELSE 'Inactive'   END Status
            FROM MasterItem H
            Where 1=1
            And H.IsActive = 1

             ";


                sqlQuery = ApplyConditions(sqlQuery, conditionalFields, conditionalValues, false);

                SqlDataAdapter objComm = CreateAdapter(sqlQuery, conn, transaction);

                // SET additional conditions param
                objComm.SelectCommand = ApplyParameters(objComm.SelectCommand, conditionalFields, conditionalValues);



                objComm.Fill(dataTable);

                var modelList = dataTable.AsEnumerable().Select(row => new MasterItemVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Code = row["Code"]?.ToString(),
                    Name = row["Name"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    IsArchive = Convert.ToBoolean(row["IsArchive"]),

                }).ToList();


                result.Status = "Success";
                result.Message = "Data retrieved successfully.";
                result.DataVM = modelList;
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
